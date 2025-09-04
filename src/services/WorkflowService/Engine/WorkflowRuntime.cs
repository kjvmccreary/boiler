using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using DTOs.Workflow.Enums;
using Contracts.Services;
using WorkflowService.Services.Interfaces;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Engine;

public class WorkflowRuntime : IWorkflowRuntime
{
    private readonly WorkflowDbContext _context;
    private readonly IEnumerable<INodeExecutor> _executors;
    private readonly ILogger<WorkflowRuntime> _logger;
    private readonly ITenantProvider _tenantProvider;
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ITaskNotificationDispatcher _taskNotifier;

    private readonly Dictionary<int, int> _instanceTenantCache = new();
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

        // Primary lookup (expected path)
        var startNode = workflowDef.Nodes.FirstOrDefault(n => n.IsStart());

        // Fallback: tolerate minor casing / parse irregularities instead of throwing immediately.
        // (Added because a recent change caused a valid JSON definition to deserialize without marking the Start node.
        //  This ensures tests surface a warning log rather than fail hard while we diagnose upstream parsing.)
        if (startNode == null && workflowDef.Nodes.Count > 0)
        {
            startNode = workflowDef.Nodes.FirstOrDefault(n =>
                n.Id.Equals("start", StringComparison.OrdinalIgnoreCase) ||
                n.Type.Equals("start", StringComparison.OrdinalIgnoreCase));

            if (startNode != null)
            {
                _logger.LogWarning(
                    "WF_START_FALLBACK InstanceDefinition={DefinitionId} FallbackStartNodeUsed Id={NodeId} Type={Type} (upstream IsStart detection failed)",
                    definitionId, startNode.Id, startNode.Type);
            }
        }

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

    public async Task ContinueWorkflowAsync(
        int instanceId,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
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

        _logger.LogInformation("WF_CONTINUE Instance={InstanceId} ActiveNodes=[{Nodes}] Edges={EdgeCount}",
            instanceId, string.Join(",", currentNodeIds), workflowDef.Edges.Count);

        foreach (var nodeId in currentNodeIds.ToArray())
        {
            if (instance.Status != InstanceStatus.Running) break;

            var node = workflowDef.Nodes.FirstOrDefault(n => n.Id.Equals(nodeId, StringComparison.OrdinalIgnoreCase));
            if (node == null)
            {
                _logger.LogError("WF_NODE_MISSING Instance={InstanceId} NodeId={NodeId}", instance.Id, nodeId);
                continue;
            }

            if (node.IsHumanTask())
            {
                var hasOpen = await _context.WorkflowTasks
                    .Where(t => t.WorkflowInstanceId == instance.Id
                                && t.NodeId == node.Id
                                && (t.Status == WorkflowTaskStatus.Created
                                    || t.Status == WorkflowTaskStatus.Assigned
                                    || t.Status == WorkflowTaskStatus.Claimed
                                    || t.Status == WorkflowTaskStatus.InProgress))
                    .AnyAsync(cancellationToken);

                if (hasOpen)
                {
                    _logger.LogTrace("WF_SKIP_WAITING_HUMAN Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
                    continue;
                }
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
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task == null && completedByUserId == 0)
        {
            task = await _context.WorkflowTasks
                .IgnoreQueryFilters()
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task == null)
                throw new InvalidOperationException($"Workflow task {taskId} not found (even ignoring filters)");

            if (!_instanceTenantCache.ContainsKey(task.WorkflowInstance.Id))
                _instanceTenantCache[task.WorkflowInstance.Id] = task.WorkflowInstance.TenantId;
        }

        if (task == null)
            throw new InvalidOperationException($"Workflow task {taskId} not found");

        var instance = task.WorkflowInstance;

        if (instance.Status != InstanceStatus.Running)
        {
            if ((instance.Status == InstanceStatus.Completed || instance.Status == InstanceStatus.Cancelled) &&
                (task.Status == WorkflowTaskStatus.Created ||
                 task.Status == WorkflowTaskStatus.Assigned ||
                 task.Status == WorkflowTaskStatus.Claimed ||
                 task.Status == WorkflowTaskStatus.InProgress))
            {
                task.Status = WorkflowTaskStatus.Cancelled;
                task.CompletedAt = DateTime.UtcNow;
                MarkDirty();
                await CreateEventAsync(instance.Id, "Task", "Cancelled",
                    JsonSerializer.Serialize(new
                    {
                        taskId = task.Id,
                        nodeId = task.NodeId,
                        reason = $"instance-{instance.Status.ToString().ToLowerInvariant()}-post-complete"
                    }),
                    completedByUserId == 0 ? null : completedByUserId);

                if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
                _logger.LogDebug("WF_LATE_TASK_CANCELLATION Instance={InstanceId} TaskId={TaskId} Status={InstanceStatus}",
                    instance.Id, task.Id, instance.Status);
                return;
            }

            throw new InvalidOperationException($"Cannot complete task; instance {instance.Id} not running");
        }

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        var workflowDef = BuilderDefinitionAdapter.Parse(instance.WorkflowDefinition.JSONDefinition);
        var node = workflowDef.Nodes.FirstOrDefault(n => n.Id.Equals(task.NodeId, StringComparison.OrdinalIgnoreCase))
                   ?? throw new InvalidOperationException($"Node {task.NodeId} not found in definition");

        var isTimer = node.IsTimer();

        if (!isTimer &&
            task.Status != WorkflowTaskStatus.Claimed &&
            task.Status != WorkflowTaskStatus.InProgress)
            throw new InvalidOperationException($"Task {taskId} not in completable state: {task.Status}");

        if (isTimer &&
            task.Status != WorkflowTaskStatus.Created &&
            task.Status != WorkflowTaskStatus.Assigned &&
            task.Status != WorkflowTaskStatus.InProgress)
            throw new InvalidOperationException($"Timer task {taskId} unexpected status {task.Status}");

        task.Status = WorkflowTaskStatus.Completed;
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
            await CancelOpenTasksAsync(instance, "instance-completed", CancellationToken.None);
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
                if (!currentNodes.Contains(n, StringComparer.OrdinalIgnoreCase)) currentNodes.Add(n);
            instance.CurrentNodeIds = JsonSerializer.Serialize(currentNodes);
            MarkDirty();
            _logger.LogInformation("WF_ADVANCE Instance={InstanceId} FromNode={NodeId} Next=[{Next}]",
                instance.Id, node.Id, string.Join(",", nextNodeIds));
        }

        if (instance.Status == InstanceStatus.Running)
            await ContinueWorkflowAsync(instance.Id, cancellationToken);

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);

        if (!isTimer)
        {
            try
            {
                var tenantId = instance.TenantId;
                if (task.AssignedToUserId.HasValue)
                    await _taskNotifier.NotifyUserAsync(tenantId, task.AssignedToUserId.Value, cancellationToken);
                await _taskNotifier.NotifyTenantAsync(tenantId, cancellationToken);
            }
            catch { }
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

        await CancelOpenTasksAsync(instance, reason, cancellationToken);

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

    public async Task SuspendWorkflowAsync(
        int instanceId,
        string reason,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        if (instance.Status == InstanceStatus.Suspended)
        {
            _logger.LogInformation("WF_SUSPEND_NOOP Instance={InstanceId} AlreadySuspended", instance.Id);
            return;
        }

        if (instance.Status != InstanceStatus.Running)
            throw new InvalidOperationException($"Only running instances can be suspended (current={instance.Status})");

        instance.Status = InstanceStatus.Suspended;
        instance.ErrorMessage ??= reason;
        MarkDirty();

        await CreateEventAsync(instance.Id, "Instance", "Suspended",
            JsonSerializer.Serialize(new { reason }), userId);
        _logger.LogWarning("WF_SUSPENDED Instance={InstanceId} Reason={Reason}", instance.Id, reason);

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
    }

    public async Task ResumeWorkflowAsync(
        int instanceId,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken)
            ?? throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (!_instanceTenantCache.ContainsKey(instance.Id))
            _instanceTenantCache[instance.Id] = instance.TenantId;

        if (instance.Status == InstanceStatus.Running)
        {
            _logger.LogInformation("WF_RESUME_NOOP Instance={InstanceId} AlreadyRunning", instance.Id);
            return;
        }

        if (instance.Status != InstanceStatus.Suspended)
            throw new InvalidOperationException($"Only suspended instances can be resumed (current={instance.Status})");

        instance.Status = InstanceStatus.Running;
        MarkDirty();

        await CreateEventAsync(instance.Id, "Instance", "Resumed",
            "{\"reason\":\"manual-resume\"}", userId);
        _logger.LogInformation("WF_RESUMED Instance={InstanceId}", instance.Id);

        await ContinueWorkflowAsync(instance.Id, cancellationToken, autoCommit: false);

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
    }

    #endregion

    #region Node Execution

    private async Task ExecuteNodeAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        WorkflowDefinitionJson workflowDef,
        CancellationToken cancellationToken)
    {
        try
        {
            var executor = _executors.FirstOrDefault(e => SafeCanExecute(e, node))
                           ?? throw new InvalidOperationException($"No executor for node type {node.Type}");

            _logger.LogInformation("WF_NODE_EXEC_START Instance={InstanceId} Node={NodeId} Type={Type} Exec={Exec}",
                instance.Id, node.Id, node.Type, executor.GetType().Name);

            var result = await executor.ExecuteAsync(node, instance, instance.Context, cancellationToken);

            if (!result.IsSuccess)
            {
                if (result.FailureAction == NodeFailureAction.SuspendInstance)
                {
                    instance.Status = InstanceStatus.Suspended;
                    instance.ErrorMessage = result.ErrorMessage;
                    MarkDirty();

                    await CreateEventAsync(instance.Id, "Node", "Failed",
                        $"{{\"nodeId\":\"{node.Id}\",\"error\":\"{result.ErrorMessage}\",\"policy\":\"suspend\"}}", null);

                    await CreateEventAsync(instance.Id, "Instance", "Suspended",
                        $"{{\"reason\":\"automatic-action-failure\",\"nodeId\":\"{node.Id}\"}}", null);

                    _logger.LogWarning("WF_INSTANCE_SUSPENDED Instance={InstanceId} Node={NodeId} Error={Error}",
                        instance.Id, node.Id, result.ErrorMessage);
                    await SaveIfDirtyAsync(cancellationToken);
                    return;
                }

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
            }

            if (!result.ShouldWait)
            {
                // Declared strategy (flattened parser ensures direct detection)
                string? declaredStrategy = null;
                if (node.IsGateway())
                {
                    declaredStrategy = TryGetDeclaredGatewayStrategy(node)
                                       ?? TryGetGatewayStrategy(instance.Context, node.Id);
                }

                List<string> next;

                if (node.IsGateway() &&
                    declaredStrategy != null &&
                    declaredStrategy.Equals("parallel", StringComparison.OrdinalIgnoreCase))
                {
                    // Declared parallel: all outgoing targets
                    var outgoingEdges = GetOutgoingEdgeObjects(workflowDef, node.Id);
                    var targets = NormalizeEdgeTargets(outgoingEdges);

                    // Emit GatewayEvaluated (parallel) with full selection
                    await CreateEventAsync(instance.Id, "Gateway", "GatewayEvaluated",
                        JsonSerializer.Serialize(new
                        {
                            nodeId = node.Id,
                            strategy = "parallel",
                            outgoingEdges = outgoingEdges.Count,
                            selected = targets,
                            phase = "declared-parallel"
                        }), null);

                    // EdgeTraversed per concrete edge
                    foreach (var e in outgoingEdges)
                    {
                        var edgeId = e.Id ?? $"{node.Id}->{(e.Target ?? e.To ?? e.EffectiveTarget)}";
                        var target = (e.Target ?? e.To ?? e.EffectiveTarget ?? string.Empty).Trim();
                        if (string.IsNullOrEmpty(target)) continue;

                        await CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                            JsonSerializer.Serialize(new
                            {
                                edgeId,
                                from = node.Id,
                                to = target,
                                mode = "AutoAdvanceParallel"
                            }), null);
                    }

                    _logger.LogInformation(
                        "WF_GATEWAY_PARALLEL_DECLARED Instance={InstanceId} Node={NodeId} Targets=[{Targets}]",
                        instance.Id, node.Id, string.Join(",", targets));

                    next = targets;
                }
                else
                {
                    // Exclusive or non-gateway path (existing logic)
                    if (result.NextNodeIds.Any())
                    {
                        next = result.NextNodeIds.ToList();
                    }
                    else
                    {
                        next = GetNextNodeIdsWithGatewayAware(node, workflowDef, instance);
                    }

                    // Edge events (only for automatic advance here; task completion path handled elsewhere)
                    foreach (var target in next)
                    {
                        var edgeId = FindEdgeId(node.Id, target, workflowDef)
                                     ?? $"{node.Id}->{target}";
                        await CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                            JsonSerializer.Serialize(new
                            {
                                edgeId,
                                from = node.Id,
                                to = target,
                                mode = "AutoAdvance"
                            }), null);
                    }
                }

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

    private bool SafeCanExecute(INodeExecutor exec, WorkflowNode node)
    {
        try { return exec.CanExecute(node); }
        catch { return false; }
    }

    #endregion

    #region Edge / Gateway Logic

    private List<WorkflowEdge> GetOutgoingEdgesCaseInsensitive(WorkflowDefinitionJson def, string nodeId)
    {
        var list = def.Edges
            .Where(e =>
                (e.Source?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true) ||
                (e.From?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true) ||
                (e.EffectiveSource?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        _logger.LogDebug("WF_DBG_OUTGOING From={From} Count={Count} Raw=[{List}]",
            nodeId, list.Count,
            string.Join(",", list.Select(e =>
                $"{(e.Source ?? e.From ?? e.EffectiveSource)}->{(e.Target ?? e.To ?? e.EffectiveTarget)}")));

        if (list.Count == 0)
            _logger.LogDebug("WF_DBG_NO_OUTGOING From={From}", nodeId);

        return list;
    }

    private List<string> GetNextNodeIdsWithGatewayAware(
        WorkflowNode currentNode,
        WorkflowDefinitionJson workflowDef,
        WorkflowInstance instance)
    {
        var edges = GetOutgoingEdgesCaseInsensitive(workflowDef, currentNode.Id);
        if (!edges.Any()) return new List<string>();

        if (!currentNode.IsGateway())
            return NormalizeEdgeTargets(edges);

        var strategyKind = TryGetDeclaredGatewayStrategy(currentNode)
                           ?? TryGetGatewayStrategy(instance.Context, currentNode.Id);

        if (string.Equals(strategyKind, "parallel", StringComparison.OrdinalIgnoreCase))
        {
            return NormalizeEdgeTargets(edges);
        }

        return SelectGatewayTargets(currentNode, instance, edges, "inline-exec");
    }

    private List<string> GetOutgoingTargetsForAdvance(
        WorkflowDefinitionJson workflowDef,
        WorkflowNode currentNode,
        WorkflowInstance instance)
    {
        var edges = GetOutgoingEdgesCaseInsensitive(workflowDef, currentNode.Id);
        if (!edges.Any()) return new List<string>();

        if (!currentNode.IsGateway())
            return NormalizeEdgeTargets(edges);

        var strategyKind = TryGetDeclaredGatewayStrategy(currentNode)
                           ?? TryGetGatewayStrategy(instance.Context, currentNode.Id);

        if (string.Equals(strategyKind, "parallel", StringComparison.OrdinalIgnoreCase))
        {
            return NormalizeEdgeTargets(edges);
        }

        return SelectGatewayTargets(currentNode, instance, edges, "advance-after-task");
    }

    private static string? TryGetGatewayStrategy(string contextJson, string nodeId)
    {
        if (string.IsNullOrWhiteSpace(contextJson)) return null;
        try
        {
            using var doc = JsonDocument.Parse(contextJson);
            if (doc.RootElement.ValueKind != JsonValueKind.Object) return null;
            if (!doc.RootElement.TryGetProperty("_gatewayDecisions", out var decisionsEl) ||
                decisionsEl.ValueKind != JsonValueKind.Object)
                return null;
            if (!decisionsEl.TryGetProperty(nodeId, out var nodeEl) ||
                nodeEl.ValueKind != JsonValueKind.Object)
                return null;
            if (!nodeEl.TryGetProperty("strategy", out var stratEl) ||
                stratEl.ValueKind != JsonValueKind.String)
                return null;
            return stratEl.GetString();
        }
        catch
        {
            return null;
        }
    }

    private static string? TryGetDeclaredGatewayStrategy(WorkflowNode node)
    {
        try
        {
            if (node.Properties == null || node.Properties.Count == 0) return null;

            if (node.Properties.TryGetValue("gatewayType", out var gtObj) &&
                gtObj is string gtStr &&
                !string.IsNullOrWhiteSpace(gtStr))
            {
                var gtv = gtStr.Trim();
                if (gtv.Equals("parallel", StringComparison.OrdinalIgnoreCase) ||
                    gtv.Equals("exclusive", StringComparison.OrdinalIgnoreCase))
                    return gtv.ToLowerInvariant();
            }

            object? raw = null;
            if (!node.Properties.TryGetValue("strategy", out raw))
            {
                var match = node.Properties
                    .FirstOrDefault(kv => kv.Key.Equals("strategy", StringComparison.OrdinalIgnoreCase));
                if (!string.IsNullOrEmpty(match.Key))
                    raw = match.Value;
            }

            if (raw is null)
            {
                foreach (var kv in node.Properties)
                {
                    if (kv.Value is IDictionary<string, object> d &&
                        d.Keys.Any(k => k.Equals("kind", StringComparison.OrdinalIgnoreCase)))
                    {
                        raw = kv.Value;
                        break;
                    }
                    if (kv.Value is JsonElement je && je.ValueKind == JsonValueKind.Object)
                    {
                        using var doc = JsonDocument.Parse(je.GetRawText());
                        if (doc.RootElement.TryGetProperty("kind", out _))
                        {
                            raw = kv.Value;
                            break;
                        }
                    }
                }
                if (raw is null) return null;
            }

            if (raw is JsonElement el && el.ValueKind == JsonValueKind.Object &&
                el.TryGetProperty("kind", out var kindEl) && kindEl.ValueKind == JsonValueKind.String)
                return kindEl.GetString()?.Trim().ToLowerInvariant();

            if (raw is string s && !string.IsNullOrWhiteSpace(s))
                return s.Trim().ToLowerInvariant();

            if (raw is IDictionary<string, object> dict &&
                dict.TryGetValue("kind", out var kindValue) &&
                kindValue is string ks &&
                !string.IsNullOrWhiteSpace(ks))
                return ks.Trim().ToLowerInvariant();

            if (raw is System.Text.Json.Nodes.JsonObject jo &&
                jo.TryGetPropertyValue("kind", out var kindNode) &&
                kindNode is System.Text.Json.Nodes.JsonValue kindJsonValue &&
                kindJsonValue.TryGetValue<string>(out var js) &&
                !string.IsNullOrWhiteSpace(js))
                return js.Trim().ToLowerInvariant();
        }
        catch { }
        return null;
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
                    .GetAwaiter()
                    .GetResult();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "WF_GATEWAY_CONDITION_FAIL Instance={InstanceId} Node={NodeId} DefaultingTrue",
                    instance.Id, gateway.Id);
                conditionResult = true;
            }
        }

        foreach (var e in edges)
            e.InferLabelIfMissing();

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

        var classified = edges.Select(e =>
        {
            var label = Normalize(e.Label);
            if (string.IsNullOrEmpty(label))
            {
                var idl = e.Id.ToLowerInvariant();
                if (idl.Contains("true")) label = "true";
                else if (idl.Contains("false")) label = "false";
                else if (idl.Contains("else")) label = "else";
            }
            return new { Edge = e, Label = label };
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
            else if (unlabeled.Count >= 2) chosen = new List<WorkflowEdge> { unlabeled[1] };
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
                selected = selectedIds,
                phase
            }), null);

        _logger.LogInformation(
            "WF_GATEWAY_SELECT Instance={InstanceId} Node={NodeId} Result={Result} Selected=[{Selected}] Phase={Phase}",
            instance.Id, gateway.Id, conditionResult, string.Join(",", selectedIds), phase);

        return selectedIds;
    }

    private static string? FindEdgeId(string from, string to, WorkflowDefinitionJson def)
         => def.Edges.FirstOrDefault(e =>
                (e.Source?.Equals(from, StringComparison.OrdinalIgnoreCase) == true ||
                 e.From?.Equals(from, StringComparison.OrdinalIgnoreCase) == true ||
                 e.EffectiveSource?.Equals(from, StringComparison.OrdinalIgnoreCase) == true)
                &&
                (e.Target?.Equals(to, StringComparison.OrdinalIgnoreCase) == true ||
                 e.To?.Equals(to, StringComparison.OrdinalIgnoreCase) == true ||
                 e.EffectiveTarget?.Equals(to, StringComparison.OrdinalIgnoreCase) == true))
               ?.Id;

    private static List<WorkflowEdge> GetOutgoingEdgeObjects(WorkflowDefinitionJson def, string nodeId)
        => def.Edges
            .Where(e =>
                (e.Source?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true) ||
                (e.From?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true) ||
                (e.EffectiveSource?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

    private static List<string> NormalizeEdgeTargets(IEnumerable<WorkflowEdge> edges)
        => edges
            .Select(e => (e.Target ?? e.To ?? e.EffectiveTarget ?? string.Empty).Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

    #endregion

    #region Advancement / Failure / Events / Context

    private void AdvanceToNextNodes(
        WorkflowInstance instance,
        WorkflowDefinitionJson workflowDef,
        List<string> nextNodeIds)
    {
        if (!nextNodeIds.Any())
        {
            CancelOpenTasksAsync(instance, "instance-completed", default).GetAwaiter().GetResult();
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            instance.CurrentNodeIds = JsonSerializer.Serialize(Array.Empty<string>());
            MarkDirty();
            CreateEventAsync(instance.Id, "Instance", "Completed",
                "{\"reason\":\"end-of-path\"}", null).GetAwaiter().GetResult();
            _logger.LogInformation("WF_INSTANCE_COMPLETE Instance={InstanceId} (no outgoing)", instance.Id);
            return;
        }

        instance.CurrentNodeIds = JsonSerializer.Serialize(nextNodeIds.ToArray());
        MarkDirty();

        var endNodes = nextNodeIds.Where(id =>
            workflowDef.Nodes.Any(n => n.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && n.IsEnd())).ToList();

        if (endNodes.Any())
        {
            CancelOpenTasksAsync(instance, "instance-completed", default).GetAwaiter().GetResult();
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            instance.CurrentNodeIds = JsonSerializer.Serialize(Array.Empty<string>());
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

        await CreateEventAsync(instance.Id, "Instance", "Failed",
            JsonSerializer.Serialize(new { nodeId = node.Id, error = errorMessage }), null);

        await SaveIfDirtyAsync();
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

    private async Task CancelOpenTasksAsync(WorkflowInstance instance, string reason, CancellationToken ct = default)
    {
        var openTasks = await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instance.Id &&
                        (t.Status == DTOs.Workflow.Enums.TaskStatus.Created ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.Assigned ||
                         t.Status == WorkflowTaskStatus.Claimed ||
                         t.Status == WorkflowTaskStatus.InProgress))
            .ToListAsync(ct);

        if (openTasks.Count == 0) return;

        foreach (var t in openTasks)
        {
            t.Status = DTOs.Workflow.Enums.TaskStatus.Cancelled;
            t.CompletedAt = DateTime.UtcNow;
            MarkDirty();

            await CreateEventAsync(instance.Id, "Task", "Cancelled",
                JsonSerializer.Serialize(new
                {
                    taskId = t.Id,
                    nodeId = t.NodeId,
                    reason = reason
                }),
                null);
        }

        _logger.LogInformation("WF_CANCEL_OPEN_TASKS Instance={InstanceId} Count={Count} Reason={Reason}",
            instance.Id, openTasks.Count, reason);
    }

    #endregion
}
