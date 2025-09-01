using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using DTOs.Workflow.Enums;
using Contracts.Services;

namespace WorkflowService.Engine;

/// <summary>
/// Unified workflow runtime:
/// - Human / Automatic / Timer / Gateway execution
/// - Timer tasks + background worker compatibility
/// - Conditional gateways (labels optional; inference supported)
/// - Rich event + outbox emission
/// - Structured logging for deep diagnostics
/// </summary>
public class WorkflowRuntime : IWorkflowRuntime
{
    private readonly WorkflowDbContext _context;
    private readonly IEnumerable<INodeExecutor> _executors;
    private readonly ILogger<WorkflowRuntime> _logger;
    private readonly ITenantProvider _tenantProvider;
    private readonly IConditionEvaluator _conditionEvaluator;

    // Cache to avoid repeated tenant lookups per instance during a run
    private readonly Dictionary<int, int> _instanceTenantCache = new();

    public WorkflowRuntime(
        WorkflowDbContext context,
        IEnumerable<INodeExecutor> executors,
        ITenantProvider tenantProvider,
        IConditionEvaluator conditionEvaluator,
        ILogger<WorkflowRuntime> logger)
    {
        _context = context;
        _executors = executors;
        _tenantProvider = tenantProvider;
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
    }

    #region Public API

    public async Task<WorkflowInstance> StartWorkflowAsync(
        int definitionId,
        string initialContext,
        int? startedByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            throw new InvalidOperationException("Tenant context is required to start workflow");

        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId && d.IsPublished, cancellationToken);

        if (definition == null)
            throw new InvalidOperationException($"Published workflow definition {definitionId} not found");

        var workflowDef = BuilderDefinitionAdapter.Parse(definition.JSONDefinition);
        var startNode = workflowDef.Nodes.FirstOrDefault(n => n.IsStart());
        if (startNode == null)
            throw new InvalidOperationException("Workflow definition must contain a Start node");

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
        await _context.SaveChangesAsync(cancellationToken);

        _instanceTenantCache[instance.Id] = tenantId.Value;

        _logger.LogInformation("WF_START Instance={InstanceId} Def={DefinitionId} Version={Version} StartNode={StartNode}",
            instance.Id, definitionId, definition.Version, startNode.Id);

        await CreateEventAsync(instance.Id, "Instance", "Started",
            $"{{\"startNodeId\":\"{startNode.Id}\"}}", startedByUserId);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
        return instance;
    }

    public async Task ContinueWorkflowAsync(int instanceId, CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");

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
    }

    public async Task SignalWorkflowAsync(
        int instanceId,
        string signalName,
        string signalData,
        int? userId = null,
        CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        await CreateEventAsync(instanceId, "Signal", signalName, signalData, userId);
        _logger.LogInformation("WF_SIGNAL Instance={InstanceId} Signal={Signal}", instanceId, signalName);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
    }

    public async Task CompleteTaskAsync(
        int taskId,
        string completionData,
        int completedByUserId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task == null)
            throw new InvalidOperationException($"Workflow task {taskId} not found");

        var instance = task.WorkflowInstance;
        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        if (instance.Status != InstanceStatus.Running)
            throw new InvalidOperationException($"Cannot complete task; instance {instance.Id} not running");

        var workflowDef = BuilderDefinitionAdapter.Parse(instance.WorkflowDefinition.JSONDefinition);
        var node = workflowDef.Nodes.FirstOrDefault(n => n.Id == task.NodeId)
                   ?? throw new InvalidOperationException($"Node {task.NodeId} not found in definition for task {task.Id}");

        var isTimer = node.IsTimer();

        if (!isTimer &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.Claimed &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.InProgress)
            throw new InvalidOperationException($"Task {taskId} not in completable state: {task.Status}");

        if (isTimer &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.Created &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.Assigned)
            throw new InvalidOperationException($"Timer task {taskId} unexpected status {task.Status}");

        task.Status = DTOs.Workflow.Enums.TaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.CompletionData = completionData;
        UpdateWorkflowContext(instance, task.NodeId, completionData);

        await CreateEventAsync(instance.Id, "Task", isTimer ? "TimerCompleted" : "Completed",
            JsonSerializer.Serialize(new
            {
                taskId,
                nodeId = task.NodeId,
                completionData = TryParseOrRaw(completionData),
                system = isTimer && completedByUserId == 0
            }), completedByUserId == 0 ? null : completedByUserId);

        // Advancement after wait-state (e.g., human task)
        var currentNodes = JsonSerializer.Deserialize<List<string>>(instance.CurrentNodeIds) ?? new List<string>();
        var removed = currentNodes.Remove(task.NodeId);

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
            await CreateEventAsync(instance.Id, "Instance", "Completed",
                "{\"reason\":\"end-of-path\"}", completedByUserId);
            _logger.LogInformation("WF_ADVANCE_COMPLETE Instance={InstanceId} Node={NodeId} EndOfPath RemovedNode={Removed}",
                instance.Id, node.Id, removed);
        }
        else
        {
            foreach (var n in nextNodeIds)
                if (!currentNodes.Contains(n))
                    currentNodes.Add(n);

            instance.CurrentNodeIds = JsonSerializer.Serialize(currentNodes);
            _logger.LogInformation("WF_ADVANCE Instance={InstanceId} FromNode={NodeId} Next=[{Next}] Removed={Removed}",
                instance.Id, node.Id, string.Join(",", nextNodeIds), removed);
        }

        await _context.SaveChangesAsync(cancellationToken);

        if (instance.Status == InstanceStatus.Running)
            await ContinueWorkflowAsync(instance.Id, cancellationToken);
    }

    public async Task CancelWorkflowAsync(
        int instanceId,
        string reason,
        int? cancelledByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        instance.Status = InstanceStatus.Cancelled;
        instance.CompletedAt = DateTime.UtcNow;

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
        }

        await CreateEventAsync(instanceId, "Instance", "Cancelled",
            $"{{\"reason\":\"{reason}\"}}", cancelledByUserId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("WF_CANCEL Instance={InstanceId} Reason={Reason}", instanceId, reason);
    }

    public async Task RetryWorkflowAsync(
        int instanceId,
        string? resetToNodeId = null,
        CancellationToken cancellationToken = default)
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

        await CreateEventAsync(instanceId, "Instance", "Retried",
            JsonSerializer.Serialize(new { resetToNodeId = resetToNodeId ?? "current" }), null);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("WF_RETRY Instance={InstanceId} ResetTo={ResetNode}", instanceId, resetToNodeId);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
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
                instance.Context = result.UpdatedContext;

            if (result.CreatedTask != null)
            {
                _context.WorkflowTasks.Add(result.CreatedTask);
                await CreateEventAsync(instance.Id, "Node", "NodeActivated",
                    JsonSerializer.Serialize(new
                    {
                        nodeId = node.Id,
                        nodeType = node.Type,
                        taskId = 0,
                        label = node.Properties.GetValueOrDefault("label") ?? node.Id
                    }), null);
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

            await _context.SaveChangesAsync(cancellationToken);

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
            return RecordAndReturn(workflowDef, edges.Select(e => e.Target).ToList(), currentNode, instance, "LinearAdvance");

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
            return edges.Select(e => e.Target).ToList();

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

    private static string? SafeString(object? v) =>
        v switch
        {
            null => null,
            string s => string.IsNullOrWhiteSpace(s) ? null : s,
            JsonElement je when je.ValueKind == JsonValueKind.String => je.GetString(),
            _ => v.ToString()
        };

    #endregion

    #region Advancement / Events / Context

    private List<string> RecordAndReturn(IEnumerable<string?> targets, WorkflowNode currentNode, WorkflowInstance instance, string mode)
    {
        // Filter out null / empty (avoids nullable warnings)
        var clean = targets
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Cast<string>()
            .ToList();

        foreach (var t in clean)
        {
            CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                JsonSerializer.Serialize(new { from = currentNode.Id, to = t, mode }), null)
                .GetAwaiter().GetResult();
        }
        return clean;
    }

    // New overload including workflowDef so we can find original edge id
    private List<string> RecordAndReturn(WorkflowDefinitionJson workflowDef, IEnumerable<string?> targets, WorkflowNode currentNode, WorkflowInstance instance, string mode)
    {
        var clean = targets
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Cast<string>()
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

    private static string? FindEdgeId(string from, string to, WorkflowDefinitionJson def)
        => def.Edges.FirstOrDefault(e => e.Source == from && e.Target == to)?.Id;

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
            CreateEventAsync(instance.Id, "Instance", "Completed",
                "{\"reason\":\"no-next-nodes\"}", null).GetAwaiter().GetResult();
            _logger.LogInformation("WF_INSTANCE_COMPLETE Instance={InstanceId}", instance.Id);
            return;
        }

        instance.CurrentNodeIds = JsonSerializer.Serialize(nextNodeIds.ToArray());

        var endNodes = nextNodeIds.Where(id =>
            workflowDef.Nodes.Any(n => n.Id == id && n.IsEnd())).ToList();

        if (endNodes.Any())
        {
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
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

        await CreateEventAsync(instance.Id, "Node", "Failed",
            $"{{\"nodeId\":\"{node.Id}\",\"error\":\"{errorMessage}\"}}", null);

        await _context.SaveChangesAsync();
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
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WF_CONTEXT_UPDATE_FAIL Instance={InstanceId} Node={NodeId}", instance.Id, nodeId);
        }
    }

    private Task CreateEventAsync(
        int instanceId,
        string type,
        string name,
        string data,
        int? userId)
    {
        // Resolve tenant (cached earlier)
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

        var workflowEvent = new WorkflowEvent
        {
            WorkflowInstanceId = instanceId,
            TenantId = tenantId,
            Type = type,
            Name = name,
            Data = data,
            OccurredAt = DateTime.UtcNow,
            UserId = userId
        };
        _context.WorkflowEvents.Add(workflowEvent);

        var outboxMessage = new OutboxMessage
        {
            EventType = $"workflow.{type}.{name}".ToLowerInvariant(),
            EventData = data,
            IsProcessed = false,
            RetryCount = 0,
            TenantId = tenantId
        };
        _context.OutboxMessages.Add(outboxMessage);

        return Task.CompletedTask;
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
}
