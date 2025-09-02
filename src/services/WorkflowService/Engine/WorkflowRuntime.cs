using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using DTOs.Workflow.Enums;
using Contracts.Services;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Engine;

/// <summary>
/// Workflow runtime with reduced database chatter:
/// - Aggregates changes and commits once per public API call
/// - Executes nodes recursively without per-node SaveChanges
/// - Emits rich events (still tracked, flushed in batch)
/// </summary>
public class WorkflowRuntime : IWorkflowRuntime
{
    private readonly WorkflowDbContext _context;
    private readonly IEnumerable<INodeExecutor> _executors;
    private readonly ILogger<WorkflowRuntime> _logger;
    private readonly ITenantProvider _tenantProvider;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ITaskNotificationDispatcher _taskNotifier;

    // Cache to avoid repeated tenant lookups per instance
    private readonly Dictionary<int, int> _instanceTenantCache = new();

    // Tracks whether tracked entities / events changed and need committing
    private bool _dirty;

    public WorkflowRuntime(
        WorkflowDbContext context,
        IEnumerable<INodeExecutor> executors,
        ITenantProvider tenantProvider,
        IConditionEvaluator conditionEvaluator,
        ITaskNotificationDispatcher taskNotifier,
        ILogger<WorkflowRuntime> logger)
    {
        _context = context;
        _executors = executors;
        _tenantProvider = tenantProvider;
        _conditionEvaluator = conditionEvaluator;
        _taskNotifier = taskNotifier;
        _logger = logger;
    }

    #region Public API

    public async Task<WorkflowInstance> StartWorkflowAsync(
        int definitionId,
        string initialContext,
        int? startedByUserId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            throw new InvalidOperationException("Tenant context is required to start workflow");

        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId && d.IsPublished, cancellationToken);

        if (definition == null)
            throw new InvalidOperationException($"Published workflow definition {definitionId} not found");

        var workflowDef = BuilderDefinitionAdapter.Parse(definition.JSONDefinition);
        var startNode = workflowDef.Nodes.FirstOrDefault(n => n.IsStart())
                        ?? throw new InvalidOperationException("Workflow definition must contain a Start node");

        var instance = new WorkflowInstance
        {
            TenantId = tenantId.Value,
            WorkflowDefinitionId = definitionId,
            DefinitionVersion = definition.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = JsonSerializer.Serialize(new[] { startNode.Id }),
            Context = string.IsNullOrWhiteSpace(initialContext) ? "{}" : initialContext,
            StartedAt = DateTime.UtcNow,
            StartedByUserId = startedByUserId
        };

        _context.WorkflowInstances.Add(instance);
        // Need Id for subsequent events
        await _context.SaveChangesAsync(cancellationToken);
        _dirty = false;

        _instanceTenantCache[instance.Id] = tenantId.Value;

        _logger.LogInformation("WF_START Instance={InstanceId} Def={DefinitionId} Version={Version} StartNode={StartNode}",
            instance.Id, definitionId, definition.Version, startNode.Id);

        await CreateEventAsync(instance.Id, "Instance", "Started",
            $"{{\"startNodeId\":\"{startNode.Id}\"}}", startedByUserId);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);

        return instance;
    }

    public async Task ContinueWorkflowAsync(int instanceId, CancellationToken cancellationToken = default, bool autoCommit = true)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        if (instance.Status != InstanceStatus.Running)
        {
            _logger.LogDebug("WF_CONTINUE_SKIP Instance={InstanceId} Status={Status}", instanceId, instance.Status);
            return;
        }

        var workflowDef = BuilderDefinitionAdapter.Parse(instance.WorkflowDefinition.JSONDefinition);
        var currentNodeIds = JsonSerializer.Deserialize<string[]>(instance.CurrentNodeIds) ?? Array.Empty<string>();

        _logger.LogInformation("WF_CONTINUE Instance={InstanceId} Nodes=[{Nodes}]",
            instanceId, string.Join(",", currentNodeIds));

        foreach (var nodeId in currentNodeIds.ToArray())
        {
            if (instance.Status != InstanceStatus.Running) break;

            var node = workflowDef.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null)
            {
                _logger.LogError("WF_NODE_MISSING Instance={InstanceId} NodeId={NodeId}", instance.Id, nodeId);
                continue;
            }

            await ExecuteNodeAsync(node, instance, workflowDef, cancellationToken);
        }

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
    }

    public async Task SignalWorkflowAsync(
        int instanceId,
        string signalName,
        string signalData,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        await CreateEventAsync(instanceId, "Signal", signalName, signalData, userId);
        _logger.LogInformation("WF_SIGNAL Instance={InstanceId} Signal={Signal}", instanceId, signalName);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
    }

    public async Task CompleteTaskAsync(
        int taskId,
        string completionData,
        int completedByUserId,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        // Primary lookup (respects tenant filter)
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        // Fallback: if system timer completion (completedByUserId == 0) and not found, bypass filter
        if (task == null && completedByUserId == 0)
        {
            task = await _context.WorkflowTasks
                .IgnoreQueryFilters()
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task == null)
                throw new InvalidOperationException($"Workflow task {taskId} not found (even ignoring filters)");

            // Set tenant cache to allow subsequent events
            if (!_instanceTenantCache.ContainsKey(task.WorkflowInstance.Id))
                _instanceTenantCache[task.WorkflowInstance.Id] = task.WorkflowInstance.TenantId;
        }

        if (task == null)
            throw new InvalidOperationException($"Workflow task {taskId} not found");

        var instance = task.WorkflowInstance;

        if (instance.Status != InstanceStatus.Running)
            throw new InvalidOperationException($"Cannot complete task; instance {instance.Id} not running");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        var workflowDef = BuilderDefinitionAdapter.Parse(instance.WorkflowDefinition.JSONDefinition);
        var node = workflowDef.Nodes.FirstOrDefault(n => n.Id == task.NodeId)
                   ?? throw new InvalidOperationException($"Node {task.NodeId} not found in definition");

        var isTimer = node.IsTimer();

        if (!isTimer &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.Claimed &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.InProgress)
            throw new InvalidOperationException($"Task {taskId} not in completable state: {task.Status}");

        if (isTimer &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.Created &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.Assigned &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.InProgress)
            throw new InvalidOperationException($"Timer task {taskId} unexpected status {task.Status}");

        task.Status = DTOs.Workflow.Enums.TaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.CompletionData = completionData;
        MarkDirty();
        UpdateWorkflowContext(instance, task.NodeId, completionData);

        await CreateEventAsync(instance.Id, "Task", isTimer ? "TimerCompleted" : "Completed",
            JsonSerializer.Serialize(new
            {
                taskId,
                nodeId = task.NodeId,
                completionData = TryParseOrRaw(completionData),
                system = isTimer && completedByUserId == 0
            }), completedByUserId == 0 ? null : completedByUserId);

        var currentNodes = JsonSerializer.Deserialize<List<string>>(instance.CurrentNodeIds) ?? new List<string>();
        currentNodes.Remove(task.NodeId);

        var nextNodeIds = GetOutgoingTargetsForAdvance(workflowDef, node, instance);

        foreach (var target in nextNodeIds)
        {
            await CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                JsonSerializer.Serialize(new
                {
                    edgeId = FindEdgeId(node.Id, target, workflowDef),
                    from = node.Id,
                    to = target,
                    mode = "TaskCompletionAdvance"
                }), null);
        }

        if (!nextNodeIds.Any())
        {
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            instance.CurrentNodeIds = JsonSerializer.Serialize(Array.Empty<string>());
            MarkDirty();
            await CreateEventAsync(instance.Id, "Instance", "Completed",
                "{\"reason\":\"end-of-path\"}", completedByUserId == 0 ? null : completedByUserId);
            _logger.LogInformation("WF_ADVANCE_COMPLETE Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
        }
        else
        {
            foreach (var n in nextNodeIds)
                if (!currentNodes.Contains(n)) currentNodes.Add(n);
            instance.CurrentNodeIds = JsonSerializer.Serialize(currentNodes);
            MarkDirty();
            _logger.LogInformation("WF_ADVANCE Instance={InstanceId} FromNode={NodeId} Next=[{Next}]",
                instance.Id, node.Id, string.Join(",", nextNodeIds));
        }

        if (instance.Status == InstanceStatus.Running)
            await ContinueWorkflowAsync(instance.Id, cancellationToken);

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);

        // Notify affected parties if human task completed
        if (!isTimer)
        {
            try
            {
                var tenantId = instance.TenantId;
                if (task.AssignedToUserId.HasValue)
                    await _taskNotifier.NotifyUserAsync(tenantId, task.AssignedToUserId.Value, cancellationToken);
                await _taskNotifier.NotifyTenantAsync(tenantId, cancellationToken);
            }
            catch { /* swallow notification errors */ }
        }
    }

    public async Task CancelWorkflowAsync(
        int instanceId,
        string reason,
        int? cancelledByUserId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        instance.Status = InstanceStatus.Cancelled;
        instance.CompletedAt = DateTime.UtcNow;
        MarkDirty();

        var activeTasks = await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId &&
                        (t.Status == DTOs.Workflow.Enums.TaskStatus.Created ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.Assigned ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.Claimed ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.InProgress))
            .ToListAsync(cancellationToken);

        foreach (var t in activeTasks)
        {
            t.Status = DTOs.Workflow.Enums.TaskStatus.Cancelled;
            t.CompletedAt = DateTime.UtcNow;
            MarkDirty();
        }

        await CreateEventAsync(instanceId, "Instance", "Cancelled",
            $"{{\"reason\":\"{reason}\"}}", cancelledByUserId);
        _logger.LogWarning("WF_CANCEL Instance={InstanceId} Reason={Reason}", instanceId, reason);

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
    }

    public async Task RetryWorkflowAsync(
        int instanceId,
        string? resetToNodeId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        if (instance.Status != InstanceStatus.Failed)
            throw new InvalidOperationException($"Retry only allowed for Failed instances. Current={instance.Status}");

        instance.Status = InstanceStatus.Running;
        instance.ErrorMessage = null;

        if (!string.IsNullOrWhiteSpace(resetToNodeId))
            instance.CurrentNodeIds = JsonSerializer.Serialize(new[] { resetToNodeId });

        MarkDirty();

        await CreateEventAsync(instanceId, "Instance", "Retried",
            JsonSerializer.Serialize(new { resetToNodeId = resetToNodeId ?? "current" }), null);

        _logger.LogInformation("WF_RETRY Instance={InstanceId} ResetTo={ResetNode}", instanceId, resetToNodeId);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
    }

    #endregion

    #region Core Execution

    private async Task ExecuteNodeAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        WorkflowDefinitionJson workflowDef,
        CancellationToken cancellationToken)
    {
        try
        {
            var executor = _executors.FirstOrDefault(e => e.CanExecute(node))
                           ?? throw new InvalidOperationException($"No executor for node type {node.Type}");

            _logger.LogInformation("WF_NODE_EXEC_START Instance={InstanceId} Node={NodeId} Type={Type}",
                instance.Id, node.Id, node.Type);

            var result = await executor.ExecuteAsync(node, instance, instance.Context, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("WF_NODE_EXEC_FAIL Instance={InstanceId} Node={NodeId} Error={Error}",
                    instance.Id, node.Id, result.ErrorMessage);
                await HandleNodeExecutionFailure(instance, node, result.ErrorMessage);
                return;
            }

            if (!string.IsNullOrEmpty(result.UpdatedContext))
            {
                instance.Context = result.UpdatedContext;
                MarkDirty();
            }

            if (result.CreatedTask != null)
            {
                _context.WorkflowTasks.Add(result.CreatedTask);
                MarkDirty();
                await CreateEventAsync(instance.Id, "Node", "NodeActivated",
                    JsonSerializer.Serialize(new
                    {
                        nodeId = node.Id,
                        nodeType = node.Type,
                        taskId = 0,
                        label = node.Properties.GetValueOrDefault("label") ?? node.Id
                    }), null);

                // Notify on creation of human tasks
                if (result.CreatedTask.NodeType == "human")
                {
                    try
                    {
                        var tenantId = instance.TenantId;
                        if (result.CreatedTask.AssignedToUserId.HasValue)
                            await _taskNotifier.NotifyUserAsync(tenantId, result.CreatedTask.AssignedToUserId.Value, cancellationToken);
                        await _taskNotifier.NotifyTenantAsync(tenantId, cancellationToken);
                    }
                    catch { }
                }
            }

            if (!result.ShouldWait)
            {
                var next = result.NextNodeIds.Any()
                    ? result.NextNodeIds
                    : GetNextNodeIdsWithGatewayAware(node, workflowDef, instance);

                _logger.LogInformation("WF_NODE_ADVANCE Instance={InstanceId} Node={NodeId} Next=[{Next}]",
                    instance.Id, node.Id, string.Join(",", next));

                AdvanceToNextNodes(instance, workflowDef, next);
            }
            else
            {
                _logger.LogInformation("WF_NODE_WAIT Instance={InstanceId} Node={NodeId} Type={Type}",
                    instance.Id, node.Id, node.Type);
            }

            await CreateEventAsync(instance.Id, "Node", "Executed",
                JsonSerializer.Serialize(new
                {
                    nodeId = node.Id,
                    nodeType = node.Type,
                    shouldWait = result.ShouldWait,
                    taskCreated = result.CreatedTask != null
                }), null);

            if (!result.ShouldWait && instance.Status == InstanceStatus.Running)
                await ContinueWorkflowAsync(instance.Id, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_NODE_EXEC_EXCEPTION Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
            await HandleNodeExecutionFailure(instance, node, ex.Message);
        }
    }

    #endregion

    #region Gateway Selection

    private List<string> GetNextNodeIdsWithGatewayAware(
        WorkflowNode currentNode,
        WorkflowDefinitionJson workflowDef,
        WorkflowInstance instance)
    {
        var edges = workflowDef.Edges.Where(e => e.Source == currentNode.Id).ToList();
        if (!edges.Any()) return new List<string>();

        if (!currentNode.IsGateway())
        {
            // Filter out null/empty targets before returning
            var linearTargets = edges
                .Select(e => e.Target)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t!)
                .ToList();
            return RecordAndReturn(workflowDef, linearTargets, currentNode, instance, "LinearAdvance");
        }

        var selected = SelectGatewayTargets(currentNode, instance, edges, "inline-exec");
        return RecordAndReturn(workflowDef, selected, currentNode, instance, "GatewayAdvance");
    }

    private List<string> GetOutgoingTargetsForAdvance(
        WorkflowDefinitionJson workflowDef,
        WorkflowNode currentNode,
        WorkflowInstance instance)
    {
        var edges = workflowDef.Edges.Where(e => e.Source == currentNode.Id).ToList();
        if (!edges.Any()) return new List<string>();
        if (!currentNode.IsGateway())
        {
            return edges
                .Select(e => e.Target)
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t!)
                .ToList();
        }

        return SelectGatewayTargets(currentNode, instance, edges, "advance-after-task");
    }

    private List<string> SelectGatewayTargets(
        WorkflowNode gateway,
        WorkflowInstance instance,
        List<WorkflowEdge> edges,
        string phase)
    {
        var conditionJson = gateway.GetProperty<string>("condition") ?? string.Empty;
        bool conditionResult = true;
        if (!string.IsNullOrWhiteSpace(conditionJson) && conditionJson.Trim() != "true")
        {
            try
            {
                conditionResult = _conditionEvaluator
                    .EvaluateAsync(conditionJson, instance.Context)
                    .GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "WF_GATEWAY_CONDITION_FAIL Instance={InstanceId} Node={NodeId} DefaultingTrue",
                    instance.Id, gateway.Id);
                conditionResult = true;
            }
        }

        string Normalize(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            raw = raw.Trim().ToLowerInvariant();
            return raw switch
            {
                "yes" => "true",
                "no" => "false",
                "default" => "else",
                _ => raw
            };
        }

        foreach (var e in edges)
            e.InferLabelIfMissing();

        var classified = edges.Select(e =>
        {
            var eff = Normalize(e.Label);
            if (string.IsNullOrEmpty(eff))
            {
                var idl = e.Id.ToLowerInvariant();
                if (idl.Contains("true")) eff = "true";
                else if (idl.Contains("false")) eff = "false";
                else if (idl.Contains("else")) eff = "else";
            }
            return new { Edge = e, Label = eff };
        }).ToList();

        var trueEdges = classified.Where(c => c.Label == "true").Select(c => c.Edge).ToList();
        var falseEdges = classified.Where(c => c.Label == "false").Select(c => c.Edge).ToList();
        var elseEdges = classified.Where(c => c.Label == "else").Select(c => c.Edge).ToList();
        var unlabeled = classified.Where(c => string.IsNullOrEmpty(c.Label)).Select(c => c.Edge).ToList();

        List<WorkflowEdge> chosen;
        if (conditionResult)
        {
            if (trueEdges.Any()) chosen = trueEdges;
            else if (elseEdges.Any()) chosen = elseEdges;
            else if (unlabeled.Any()) chosen = new List<WorkflowEdge> { unlabeled[0] };
            else chosen = new List<WorkflowEdge>();
        }
        else
        {
            if (falseEdges.Any()) chosen = falseEdges;
            else if (elseEdges.Any()) chosen = elseEdges;
            else if (unlabeled.Count == 2) chosen = new List<WorkflowEdge> { unlabeled[1] };
            else if (unlabeled.Count >= 3) chosen = new List<WorkflowEdge> { unlabeled[1] };
            else chosen = new List<WorkflowEdge>();
        }

        var selectedIds = chosen.Select(c => c.EffectiveTarget).ToList();

        _ = CreateEventAsync(instance.Id, "Gateway", "GatewayEvaluated",
            JsonSerializer.Serialize(new
            {
                nodeId = gateway.Id,
                condition = conditionJson,
                result = conditionResult,
                outgoingEdges = edges.Count,
                labeledTrue = trueEdges.Count,
                labeledFalse = falseEdges.Count,
                labeledElse = elseEdges.Count,
                unlabeled = unlabeled.Count,
                selected = selectedIds,
                phase
            }), null);

        _logger.LogInformation(
            "WF_GATEWAY_SELECT Instance={InstanceId} Node={NodeId} Result={Result} Sel=[{Selected}] True={TrueCnt} False={FalseCnt} Else={ElseCnt} Unlab={UnlabCnt} Phase={Phase}",
            instance.Id,
            gateway.Id,
            conditionResult,
            string.Join(",", selectedIds),
            trueEdges.Count,
            falseEdges.Count,
            elseEdges.Count,
            unlabeled.Count,
            phase);

        return selectedIds;
    }

    private static string? FindEdgeId(string from, string to, WorkflowDefinitionJson def)
        => def.Edges.FirstOrDefault(e => e.Source == from && e.Target == to)?.Id;

    #endregion

    #region Advancement / Events / Context

    private List<string> RecordAndReturn(
        WorkflowDefinitionJson workflowDef,
        IEnumerable<string?> targets,
        WorkflowNode currentNode,
        WorkflowInstance instance,
        string mode)
    {
        var clean = targets
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t!)
            .ToList();

        foreach (var t in clean)
        {
            var edgeId = FindEdgeId(currentNode.Id, t, workflowDef);
            CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                JsonSerializer.Serialize(new
                {
                    edgeId,
                    from = currentNode.Id,
                    to = t,
                    mode
                }), null).GetAwaiter().GetResult();
        }
        return clean;
    }

    private List<string> RecordAndReturn(
        IEnumerable<string?> targets,
        WorkflowNode currentNode,
        WorkflowInstance instance,
        string mode)
    {
        var clean = targets
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t!)
            .ToList();

        foreach (var t in clean)
        {
            CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                JsonSerializer.Serialize(new
                {
                    from = currentNode.Id,
                    to = t,
                    mode
                }), null).GetAwaiter().GetResult();
        }
        return clean;
    }

    private void AdvanceToNextNodes(
        WorkflowInstance instance,
        WorkflowDefinitionJson workflowDef,
        List<string> nextNodeIds)
    {
        if (!nextNodeIds.Any())
        {
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            instance.CurrentNodeIds = JsonSerializer.Serialize(Array.Empty<string>());
            MarkDirty();
            CreateEventAsync(instance.Id, "Instance", "Completed",
                "{\"reason\":\"no-next-nodes\"}", null).GetAwaiter().GetResult();
            _logger.LogInformation("WF_INSTANCE_COMPLETE Instance={InstanceId}", instance.Id);
            return;
        }

        instance.CurrentNodeIds = JsonSerializer.Serialize(nextNodeIds.ToArray());
        MarkDirty();

        var endNodes = nextNodeIds.Where(id =>
            workflowDef.Nodes.Any(n => n.Id == id && n.IsEnd())).ToList();

        if (endNodes.Any())
        {
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            MarkDirty();
            CreateEventAsync(instance.Id, "Instance", "Completed",
                JsonSerializer.Serialize(new { endNodes }), null).GetAwaiter().GetResult();
            _logger.LogInformation("WF_INSTANCE_COMPLETE_END Instance={InstanceId} EndNodes=[{Ends}]",
                instance.Id, string.Join(",", endNodes));
        }
    }

    private async Task HandleNodeExecutionFailure(
        WorkflowInstance instance,
        WorkflowNode node,
        string? errorMessage)
    {
        instance.Status = InstanceStatus.Failed;
        instance.ErrorMessage = errorMessage;
        instance.CompletedAt = DateTime.UtcNow;
        MarkDirty();

        await CreateEventAsync(instance.Id, "Node", "Failed",
            $"{{\"nodeId\":\"{node.Id}\",\"error\":\"{errorMessage}\"}}", null);

        await SaveIfDirtyAsync(); // commit failure immediately
        _logger.LogError("WF_INSTANCE_FAILED Instance={InstanceId} Node={NodeId} Error={Error}",
            instance.Id, node.Id, errorMessage);
    }

    private void UpdateWorkflowContext(
        WorkflowInstance instance,
        string nodeId,
        string completionData)
    {
        try
        {
            var context = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context) ?? new();
            var completion = string.IsNullOrWhiteSpace(completionData)
                ? new Dictionary<string, object>()
                : JsonSerializer.Deserialize<Dictionary<string, object>>(completionData) ?? new();

            context[$"task_{nodeId}"] = completion;
            instance.Context = JsonSerializer.Serialize(context);
            MarkDirty();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WF_CONTEXT_UPDATE_FAIL Instance={InstanceId} Node={NodeId}", instance.Id, nodeId);
        }
    }

    private async Task CreateEventAsync(
        int instanceId,
        string type,
        string name,
        string data,
        int? userId)
    {
        int tenantId;
        if (!_instanceTenantCache.TryGetValue(instanceId, out tenantId))
        {
            tenantId = _context.WorkflowInstances
                .AsNoTracking()
                .Where(i => i.Id == instanceId)
                .Select(i => i.TenantId)
                .FirstOrDefault();
            if (tenantId > 0)
                _instanceTenantCache[instanceId] = tenantId;
        }

        _context.WorkflowEvents.Add(new WorkflowEvent
        {
            WorkflowInstanceId = instanceId,
            TenantId = tenantId,
            Type = type,
            Name = name,
            Data = data,
            OccurredAt = DateTime.UtcNow,
            UserId = userId
        });

        _context.OutboxMessages.Add(new OutboxMessage
        {
            EventType = $"workflow.{type}.{name}".ToLowerInvariant(),
            EventData = data,
            IsProcessed = false,
            RetryCount = 0,
            TenantId = tenantId
        });

        MarkDirty();
        await Task.CompletedTask;
    }

    private static object TryParseOrRaw(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch
        {
            return json;
        }
    }

    #endregion

    #region Persistence Helpers

    private void MarkDirty() => _dirty = true;

    private async Task SaveIfDirtyAsync(CancellationToken ct = default)
    {
        if (!_dirty) return;
        await _context.SaveChangesAsync(ct);
        _dirty = false;
    }

    #endregion
}
