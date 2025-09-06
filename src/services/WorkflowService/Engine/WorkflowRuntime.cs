using System.Text.Json;
using System.Text.Json.Nodes;
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
    private readonly IWorkflowNotificationDispatcher _instanceNotifier; // NEW

    private readonly Dictionary<int, int> _instanceTenantCache = new();
    private bool _dirty;

    public WorkflowRuntime(
        WorkflowDbContext context,
        IEnumerable<INodeExecutor> executors,
        ITenantProvider tenantProvider,
        IConditionEvaluator conditionEvaluator,
        ITaskNotificationDispatcher taskNotifier,
        ILogger<WorkflowRuntime> logger,
        IWorkflowNotificationDispatcher? instanceNotifier = null) // optional new param (non-breaking)
    {
        _context = context;
        _executors = executors;
        _tenantProvider = tenantProvider;
        _conditionEvaluator = conditionEvaluator;
        _taskNotifier = taskNotifier;
        _logger = logger;
        _instanceNotifier = instanceNotifier ?? new NullWorkflowNotificationDispatcher();
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

        var startNode = workflowDef.Nodes.FirstOrDefault(n => n.IsStart());
        if (startNode == null && workflowDef.Nodes.Count > 0)
        {
            startNode = workflowDef.Nodes.FirstOrDefault(n =>
                n.Id.Equals("start", StringComparison.OrdinalIgnoreCase) ||
                n.Type.Equals("start", StringComparison.OrdinalIgnoreCase));

            if (startNode != null)
            {
                _logger.LogWarning(
                    "WF_START_FALLBACK InstanceDefinition={DefinitionId} FallbackStartNodeUsed Id={NodeId} Type={Type}",
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

        // Notify after persisted start (even if auto-completed)
        await SafeNotifyInstanceAndListAsync(instance);

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

        var safetyCounter = 0;
        while (instance.Status == InstanceStatus.Running)
        {
            safetyCounter++;
            if (safetyCounter > 5000)
            {
                _logger.LogWarning("WF_CONTINUE_SAFETY_BREAK Instance={InstanceId}", instance.Id);
                break;
            }

            var activeNodes = DeserializeActive(instance);
            if (activeNodes.Count == 0)
            {
                await TryCompleteInstanceAsync(instance, "active-set-empty");
                break;
            }

            var iteration = activeNodes.ToArray();
            var madeProgress = false;

            foreach (var nodeId in iteration)
            {
                if (instance.Status != InstanceStatus.Running) break;

                var currentActive = DeserializeActive(instance);
                if (!currentActive.Contains(nodeId, StringComparer.OrdinalIgnoreCase))
                    continue;

                var node = workflowDef.Nodes
                    .FirstOrDefault(n => n.Id.Equals(nodeId, StringComparison.OrdinalIgnoreCase));

                if (node == null)
                {
                    _logger.LogError("WF_NODE_MISSING Instance={InstanceId} NodeId={NodeId}", instance.Id, nodeId);
                    RemoveActiveNode(instance, nodeId);
                    madeProgress = true;
                    continue;
                }

                if (await HasOpenWaitingTaskAsync(instance.Id, node, cancellationToken))
                {
                    _logger.LogTrace("WF_SKIP_WAITING Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
                    continue;
                }

                await ExecuteNodeInternalAsync(node, instance, workflowDef, cancellationToken);
                madeProgress = true;

                if (instance.Status != InstanceStatus.Running)
                    break;
            }

            if (!madeProgress)
                break;
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

        if (autoCommit)
            await SafeNotifyInstanceAndListAsync(instance);
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

                if (autoCommit) await SafeNotifyInstanceAndListAsync(instance);
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
        UpdateWorkflowContextTaskCompletion(instance, task.NodeId, completionData);

        await CreateEventAsync(instance.Id, "Task", isTimer ? "TimerCompleted" : "Completed",
            JsonSerializer.Serialize(new
            {
                taskId,
                nodeId = task.NodeId,
                completionData = TryParseOrRaw(completionData),
                system = isTimer && completedByUserId == 0
            }), completedByUserId == 0 ? null : completedByUserId);

        RemoveActiveNode(instance, node.Id);

        var nextNodeIds = GetOutgoingTargetsForAdvance(workflowDef, node);

        foreach (var target in nextNodeIds)
        {
            var edgeId = FindEdgeId(node.Id, target, workflowDef)
                         ?? $"{node.Id}->{target}";
            await CreateEventAsync(instance.Id, "Edge", "EdgeTraversed",
                JsonSerializer.Serialize(new
                {
                    edgeId,
                    from = node.Id,
                    to = target,
                    mode = "TaskCompletionAdvance"
                }), null);
        }

        // JOIN arrival handling
        var joinReady = new List<string>();
        foreach (var candidate in nextNodeIds)
        {
            var candNode = workflowDef.Nodes
                .FirstOrDefault(n => n.Id.Equals(candidate, StringComparison.OrdinalIgnoreCase));

            if (candNode?.IsJoin() == true)
            {
                var arrival = HandleJoinCandidateArrival(instance, workflowDef, node, candidate);
                if (arrival.Satisfied && arrival.AddedToActive)
                    joinReady.Add(candidate);
            }
            else
            {
                joinReady.Add(candidate);
            }
        }

        AddActiveNodes(instance, joinReady);

        if (!nextNodeIds.Any())
            UpdateParallelBranchProgress(instance, node.Id);

        await TryCompleteIfNoActiveAsync(instance);

        if (instance.Status == InstanceStatus.Running)
            await ContinueWorkflowAsync(instance.Id, cancellationToken, autoCommit: false);

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
            catch
            {
                // ignore
            }
        }

        if (autoCommit)
            await SafeNotifyInstanceAndListAsync(instance);
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

        await CancelOpenTasksAsync(instance, "instance-cancelled", cancellationToken);

        await CreateEventAsync(instanceId, "Instance", "Cancelled",
            $"{{\"reason\":\"{reason}\"}}", cancelledByUserId);
        _logger.LogWarning("WF_CANCEL Instance={InstanceId} Reason={Reason}", instance.Id, reason);

        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
        if (autoCommit) await SafeNotifyInstanceAndListAsync(instance);
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

        _logger.LogInformation("WF_RETRY Instance={InstanceId} ResetTo={ResetNode}", instance.Id, resetToNodeId);

        await ContinueWorkflowAsync(instance.Id, cancellationToken);
        if (autoCommit) await SaveIfDirtyAsync(cancellationToken);
        if (autoCommit) await SafeNotifyInstanceAndListAsync(instance);
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
        if (autoCommit) await SafeNotifyInstanceAndListAsync(instance);
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
        if (autoCommit) await SafeNotifyInstanceAndListAsync(instance);
    }

    #endregion

    #region Internal Execution

    private async Task ExecuteNodeInternalAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        WorkflowDefinitionJson workflowDef,
        CancellationToken cancellationToken)
    {
        try
        {
            var executor = _executors.FirstOrDefault(e => SafeCanExecute(e, node));
            if (executor == null)
            {
                if (node.IsEnd())
                {
                    RemoveActiveNode(instance, node.Id);
                    await CreateEventAsync(instance.Id, "Node", "Executed",
                        JsonSerializer.Serialize(new
                        {
                            nodeId = node.Id,
                            nodeType = node.Type,
                            shouldWait = false,
                            taskCreated = false,
                            fallback = "implicit-end"
                        }), null);
                    await TryCompleteIfNoActiveAsync(instance);
                    return;
                }
                throw new InvalidOperationException($"No executor for node type {node.Type}");
            }

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
                    await SafeNotifyInstanceAndListAsync(instance);
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
                if (!HasLocalOpenTask(instance.Id, node.Id))
                {
                    var dbHas = await _context.WorkflowTasks
                        .AsNoTracking()
                        .Where(t => t.WorkflowInstanceId == instance.Id
                                    && t.NodeId == node.Id
                                    && OpenStatuses.Contains(t.Status))
                        .AnyAsync(cancellationToken);

                    if (dbHas)
                    {
                        _logger.LogDebug("WF_TASK_DUP_DB_SUPPRESS Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
                    }
                    else
                    {
                        _context.WorkflowTasks.Add(result.CreatedTask);
                        MarkDirty();
                        await SaveIfDirtyAsync(cancellationToken);

                        await CreateEventAsync(instance.Id, "Node", "NodeActivated",
                            JsonSerializer.Serialize(new
                            {
                                nodeId = node.Id,
                                nodeType = node.Type,
                                taskId = 0,
                                label = node.Properties.GetValueOrDefault("label") ?? node.Id
                            }), null);
                    }
                }
                else
                {
                    _logger.LogDebug("WF_TASK_DUP_LOCAL_SUPPRESS Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
                }
            }

            if (result.ShouldWait)
            {
                await CreateEventAsync(instance.Id, "Node", "Executed",
                    JsonSerializer.Serialize(new
                    {
                        nodeId = node.Id,
                        nodeType = node.Type,
                        shouldWait = true,
                        taskCreated = result.CreatedTask != null
                    }), null);
                return;
            }

            RemoveActiveNode(instance, node.Id);

            List<string> next;

            if (node.IsGateway())
            {
                var (strategy, selected, outgoingCount) = ReadGatewayDecision(instance.Context, node.Id);

                next = result.NextNodeIds?.Any() == true
                    ? result.NextNodeIds.ToList()
                    : selected.ToList();

                if (!next.Any())
                {
                    var fallbackKind = ExtractGatewayKindFallback(node);
                    var allTargets = GetLinearOutgoingTargets(workflowDef, node.Id);

                    if (fallbackKind.Equals("parallel", StringComparison.OrdinalIgnoreCase))
                    {
                        next = allTargets;
                        CreateOrUpdateParallelGroup(instance, node.Id, next);
                        strategy = "parallel";
                        outgoingCount = allTargets.Count;
                    }
                    else
                    {
                        if (allTargets.Count > 0)
                            next = new List<string> { allTargets[0] };
                        strategy = fallbackKind;
                        outgoingCount = allTargets.Count;
                    }
                }

                await CreateEventAsync(instance.Id, "Gateway", "GatewayEvaluated",
                    JsonSerializer.Serialize(new
                    {
                        nodeId = node.Id,
                        strategy = strategy ?? "exclusive",
                        outgoingEdges = outgoingCount,
                        selected = next
                    }), null);

                var mode = (strategy ?? "exclusive").Equals("parallel", StringComparison.OrdinalIgnoreCase)
                    ? "AutoAdvanceParallel"
                    : "AutoAdvance";

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
                            mode
                        }), null);
                }

                if ((strategy ?? "").Equals("parallel", StringComparison.OrdinalIgnoreCase))
                    CreateOrUpdateParallelGroup(instance, node.Id, next);
            }
            else
            {
                if (result.NextNodeIds.Any())
                {
                    next = result.NextNodeIds.ToList();
                }
                else
                {
                    next = GetLinearOutgoingTargets(workflowDef, node.Id);
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
            }

            var joinReady = new List<string>();
            foreach (var candidate in next)
            {
                var candNode = workflowDef.Nodes.FirstOrDefault(n => n.Id.Equals(candidate, StringComparison.OrdinalIgnoreCase));
                if (candNode?.IsJoin() == true)
                {
                    var arrival = HandleJoinCandidateArrival(instance, workflowDef, node, candidate);
                    if (arrival.Satisfied && arrival.AddedToActive)
                        joinReady.Add(candidate);
                }
                else
                {
                    joinReady.Add(candidate);
                }
            }
            AddActiveNodes(instance, joinReady);

            await CreateEventAsync(instance.Id, "Node", "Executed",
                JsonSerializer.Serialize(new
                {
                    nodeId = node.Id,
                    nodeType = node.Type,
                    shouldWait = false,
                    taskCreated = result.CreatedTask != null
                }), null);

            await TryCompleteIfNoActiveAsync(instance);
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

    #region Gateway Decision Reading Helpers

    private (string? strategy, IEnumerable<string> selectedTargets, int outgoingCount) ReadGatewayDecision(
        string contextJson,
        string nodeId)
    {
        try
        {
            using var doc = JsonDocument.Parse(contextJson);
            if (!doc.RootElement.TryGetProperty("_gatewayDecisions", out var decisions) ||
                decisions.ValueKind != JsonValueKind.Object)
                return (null, Array.Empty<string>(), 0);

            if (!decisions.TryGetProperty(nodeId, out var nodeEntry))
                return (null, Array.Empty<string>(), 0);

            JsonElement decisionEl;

            if (nodeEntry.ValueKind == JsonValueKind.Array)
            {
                if (nodeEntry.GetArrayLength() == 0) return (null, Array.Empty<string>(), 0);
                decisionEl = nodeEntry[nodeEntry.GetArrayLength() - 1];
            }
            else if (nodeEntry.ValueKind == JsonValueKind.Object)
            {
                decisionEl = nodeEntry;
            }
            else return (null, Array.Empty<string>(), 0);

            string? strategy = null;
            if (decisionEl.TryGetProperty("strategy", out var stratEl) &&
                stratEl.ValueKind == JsonValueKind.String)
                strategy = stratEl.GetString();

            var selected = new List<string>();
            if (decisionEl.TryGetProperty("selectedTargets", out var targetsEl) &&
                targetsEl.ValueKind == JsonValueKind.Array)
            {
                foreach (var t in targetsEl.EnumerateArray())
                {
                    if (t.ValueKind == JsonValueKind.String)
                    {
                        var v = t.GetString();
                        if (!string.IsNullOrWhiteSpace(v))
                            selected.Add(v);
                    }
                }
            }

            var outgoing = 0;
            if (decisionEl.TryGetProperty("diagnostics", out var diagEl) &&
                diagEl.ValueKind == JsonValueKind.Object &&
                diagEl.TryGetProperty("outgoingEdgeCount", out var outEl) &&
                outEl.ValueKind == JsonValueKind.Number)
            {
                outgoing = outEl.GetInt32();
            }
            else if (decisionEl.TryGetProperty("outgoingEdges", out var legacyOut) &&
                     legacyOut.ValueKind == JsonValueKind.Number)
            {
                outgoing = legacyOut.GetInt32();
            }

            return (strategy, selected, outgoing);
        }
        catch
        {
            return (null, Array.Empty<string>(), 0);
        }
    }

    private string ExtractGatewayKindFallback(WorkflowNode node)
    {
        if (node.Properties.TryGetValue("strategy", out var sVal))
        {
            if (sVal is string sStr && !string.IsNullOrWhiteSpace(sStr))
                return sStr.Trim();
            if (sVal is JsonElement el)
            {
                if (el.ValueKind == JsonValueKind.String)
                {
                    var sv = el.GetString();
                    if (!string.IsNullOrWhiteSpace(sv)) return sv.Trim();
                }
                if (el.ValueKind == JsonValueKind.Object && el.TryGetProperty("kind", out var kEl) && kEl.ValueKind == JsonValueKind.String)
                {
                    var kv = kEl.GetString();
                    if (!string.IsNullOrWhiteSpace(kv)) return kv.Trim();
                }
            }
        }
        foreach (var kv in node.Properties)
        {
            if (kv.Value is JsonElement jel && jel.ValueKind == JsonValueKind.Object &&
                jel.TryGetProperty("kind", out var kNode) && kNode.ValueKind == JsonValueKind.String)
            {
                var ks = kNode.GetString();
                if (!string.IsNullOrWhiteSpace(ks)) return ks.Trim();
            }
        }
        var gt = node.GetProperty<string>("gatewayType");
        if (!string.IsNullOrWhiteSpace(gt)) return gt.Trim();
        return "exclusive";
    }

    #endregion

    #region Outgoing Utilities (Non-Gateway)

    private List<string> GetLinearOutgoingTargets(WorkflowDefinitionJson def, string nodeId)
    {
        var edges = def.Edges
            .Where(e =>
                (e.Source?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true) ||
                (e.From?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true) ||
                (e.EffectiveSource?.Equals(nodeId, StringComparison.OrdinalIgnoreCase) == true))
            .ToList();

        return edges
            .Select(e => (e.Target ?? e.To ?? e.EffectiveTarget ?? "").Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private List<string> GetOutgoingTargetsForAdvance(
        WorkflowDefinitionJson workflowDef,
        WorkflowNode currentNode)
    {
        return GetLinearOutgoingTargets(workflowDef, currentNode.Id);
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

    #endregion

    #region Active Set / Parallel Groups

    private List<string> DeserializeActive(WorkflowInstance instance)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(instance.CurrentNodeIds)
                   ?.Where(s => !string.IsNullOrWhiteSpace(s))
                   .Distinct(StringComparer.OrdinalIgnoreCase)
                   .ToList()
                   ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private void SerializeActive(WorkflowInstance instance, List<string> nodes)
    {
        instance.CurrentNodeIds = JsonSerializer.Serialize(nodes.Distinct(StringComparer.OrdinalIgnoreCase));
        MarkDirty();
    }

    private void RemoveActiveNode(WorkflowInstance instance, string nodeId)
    {
        var active = DeserializeActive(instance);
        if (active.RemoveAll(a => a.Equals(nodeId, StringComparison.OrdinalIgnoreCase)) > 0)
            SerializeActive(instance, active);
    }

    private void AddActiveNodes(WorkflowInstance instance, IEnumerable<string> nodes)
    {
        if (nodes == null) return;
        var list = DeserializeActive(instance);
        foreach (var n in nodes)
            if (!list.Contains(n, StringComparer.OrdinalIgnoreCase))
                list.Add(n);
        SerializeActive(instance, list);
    }

    private async Task TryCompleteIfNoActiveAsync(WorkflowInstance instance)
    {
        if (instance.Status != InstanceStatus.Running) return;
        var active = DeserializeActive(instance);
        if (active.Count == 0)
            await TryCompleteInstanceAsync(instance, "active-set-drained");
    }

    private async Task TryCompleteInstanceAsync(WorkflowInstance instance, string reason)
    {
        if (instance.Status != InstanceStatus.Running) return;

        if (_dirty)
        {
            await SaveIfDirtyAsync();
        }

        await CancelOpenTasksAsync(instance, "instance-completed", default);
        instance.Status = InstanceStatus.Completed;
        instance.CompletedAt = DateTime.UtcNow;
        instance.CurrentNodeIds = JsonSerializer.Serialize(Array.Empty<string>());
        MarkDirty();
        await CreateEventAsync(instance.Id, "Instance", "Completed",
            JsonSerializer.Serialize(new { reason }), null);
        _logger.LogInformation("WF_INSTANCE_COMPLETE Instance={InstanceId} Reason={Reason}", instance.Id, reason);

        await SaveIfDirtyAsync();
        await SafeNotifyInstanceAndListAsync(instance);
    }

    private void CreateOrUpdateParallelGroup(WorkflowInstance instance, string gatewayNodeId, List<string> branches)
    {
        if (branches == null || branches.Count == 0) return;
        var root = SafeParseContext(instance);

        if (root["_parallelGroups"] is not JsonObject groups)
        {
            groups = new JsonObject();
            root["_parallelGroups"] = groups;
        }

        if (groups[gatewayNodeId] is not JsonObject existing)
        {
            existing = new JsonObject
            {
                ["branches"] = new JsonArray(branches.Select(b => (JsonNode?)b).ToArray()),
                ["remaining"] = new JsonArray(branches.Select(b => (JsonNode?)b).ToArray()),
                ["completed"] = new JsonArray(),
                ["createdAtUtc"] = DateTime.UtcNow.ToString("O"),
                ["joinNodeId"] = null
            };
            groups[gatewayNodeId] = existing;
        }
        else
        {
            if (existing["branches"] is JsonArray arr)
            {
                foreach (var b in branches)
                    if (!arr.Any(n => n?.GetValue<string>()?.Equals(b, StringComparison.OrdinalIgnoreCase) == true))
                        arr.Add(b);
            }
            if (existing["remaining"] is JsonArray rem)
            {
                foreach (var b in branches)
                    if (!rem.Any(n => n?.GetValue<string>()?.Equals(b, StringComparison.OrdinalIgnoreCase) == true))
                        rem.Add(b);
            }
        }

        instance.Context = root.ToJsonString();
        MarkDirty();
    }

    private void UpdateParallelBranchProgress(WorkflowInstance instance, string finishedNodeId)
    {
        var root = SafeParseContext(instance);
        if (root["_parallelGroups"] is not JsonObject groups) return;

        foreach (var kv in groups)
        {
            if (kv.Value is not JsonObject grp) continue;

            if (grp["branches"] is not JsonArray branches) continue;
            if (!branches.Any(b => b?.GetValue<string>()?.Equals(finishedNodeId, StringComparison.OrdinalIgnoreCase) == true))
                continue;

            var remaining = grp["remaining"] as JsonArray ?? new JsonArray();
            var completed = grp["completed"] as JsonArray ?? new JsonArray();

            var toRemove = remaining
                .Where(r => r?.GetValue<string>()?.Equals(finishedNodeId, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
            foreach (var r in toRemove) remaining.Remove(r);

            if (!completed.Any(c => c?.GetValue<string>()?.Equals(finishedNodeId, StringComparison.OrdinalIgnoreCase) == true))
                completed.Add(finishedNodeId);

            grp["remaining"] = remaining;
            grp["completed"] = completed;

            CreateEventAsync(instance.Id, "Parallel", "ParallelBranchProgress",
                JsonSerializer.Serialize(new
                {
                    gatewayNodeId = kv.Key,
                    finishedNodeId,
                    remaining = remaining.Select(r => r?.GetValue<string>()).Where(s => s != null),
                    completed = completed.Select(r => r?.GetValue<string>()).Where(s => s != null)
                }), null).GetAwaiter().GetResult();

            instance.Context = root.ToJsonString();
            MarkDirty();
        }
    }

    private JsonObject SafeParseContext(WorkflowInstance instance)
    {
        try
        {
            var node = JsonNode.Parse(string.IsNullOrWhiteSpace(instance.Context) ? "{}" : instance.Context);
            return node as JsonObject ?? new JsonObject();
        }
        catch
        {
            return new JsonObject();
        }
    }

    #endregion

    #region Failure / Context / Events

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
        await SafeNotifyInstanceAndListAsync(instance);
        _logger.LogError("WF_INSTANCE_FAILED Instance={InstanceId} Node={NodeId} Error={Error}",
            instance.Id, node.Id, errorMessage);
    }

    private void UpdateWorkflowContextTaskCompletion(
        WorkflowInstance instance,
        string nodeId,
        string completionData)
    {
        try
        {
            var context = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context) ?? new();
            object valueToStore;

            if (string.IsNullOrWhiteSpace(completionData))
                valueToStore = new Dictionary<string, object>();
            else if (IsLikelyJson(completionData) && TryDeserializeDictionary(completionData, out var parsed))
                valueToStore = parsed!;
            else
                valueToStore = new Dictionary<string, object> { ["raw"] = completionData };

            context[$"task_{nodeId}"] = valueToStore;
            instance.Context = JsonSerializer.Serialize(context);
            MarkDirty();

            _ = CreateEventAsync(instance.Id,
                "Task",
                "Context_Updated",
                JsonSerializer.Serialize(new
                {
                    taskNodeId = nodeId,
                    taskKey = $"task_{nodeId}",
                    appliedAtUtc = DateTime.UtcNow,
                    dataShape = valueToStore is Dictionary<string, object> dict && dict.ContainsKey("raw") ? "wrapped" : "json",
                    keys = (valueToStore as Dictionary<string, object>)?.Keys
                }),
                null);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WF_CONTEXT_UPDATE_FAIL Instance={InstanceId} Node={NodeId}", instance.Id, nodeId);
        }
    }

    private static bool IsLikelyJson(string s)
    {
        s = s.Trim();
        if (s.Length < 2) return false;
        return (s.StartsWith("{") && s.EndsWith("}")) || (s.StartsWith("[") && s.EndsWith("]"));
    }

    private static bool TryDeserializeDictionary(string json, out Dictionary<string, object>? dict)
    {
        try
        {
            dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json);
            return dict != null;
        }
        catch
        {
            dict = null;
            return false;
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
                        (t.Status == WorkflowTaskStatus.Created ||
                         t.Status == WorkflowTaskStatus.Assigned ||
                         t.Status == WorkflowTaskStatus.Claimed ||
                         t.Status == WorkflowTaskStatus.InProgress))
            .ToListAsync(ct);

        if (openTasks.Count == 0) return;

        foreach (var t in openTasks)
        {
            t.Status = WorkflowTaskStatus.Cancelled;
            t.CompletedAt = DateTime.UtcNow;
            MarkDirty();

            await CreateEventAsync(instance.Id, "Task", "Cancelled",
                JsonSerializer.Serialize(new
                {
                    taskId = t.Id,
                    nodeId = t.NodeId,
                    reason
                }),
                null);
        }

        _logger.LogInformation("WF_CANCEL_OPEN_TASKS Instance={InstanceId} Count={Count} Reason={Reason}",
            instance.Id, openTasks.Count, reason);
    }

    #endregion

    #region Join Helpers

    private readonly struct JoinArrivalResult
    {
        public bool Satisfied { get; init; }
        public bool NewlySatisfied { get; init; }
        public bool AddedToActive { get; init; }
        public List<string> CancelledBranches { get; init; }
    }

    private JoinArrivalResult HandleJoinCandidateArrival(
        WorkflowInstance instance,
        WorkflowDefinitionJson def,
        WorkflowNode sourceNode,
        string joinNodeId)
    {
        var joinNode = def.Nodes.FirstOrDefault(n => n.Id.Equals(joinNodeId, StringComparison.OrdinalIgnoreCase));
        if (joinNode == null) return new JoinArrivalResult { Satisfied = false, NewlySatisfied = false, AddedToActive = false, CancelledBranches = new() };
        if (!joinNode.IsJoin())
            return new JoinArrivalResult { Satisfied = false, NewlySatisfied = false, AddedToActive = true, CancelledBranches = new() }; // not actually a join; treat normal

        var gatewayId = joinNode.GetProperty<string>("gatewayId")?.Trim();
        if (string.IsNullOrWhiteSpace(gatewayId))
        {
            _logger.LogWarning("WF_JOIN_NO_GATEWAY Instance={InstanceId} JoinNode={JoinNode}", instance.Id, joinNode.Id);
            return new JoinArrivalResult { Satisfied = true, NewlySatisfied = true, AddedToActive = true, CancelledBranches = new() }; // fail-open
        }

        var mode = (joinNode.GetProperty<string>("mode") ?? "all").Trim().ToLowerInvariant();
        var cancelRemaining = joinNode.GetProperty<bool?>("cancelRemaining") ?? false;
        var count = joinNode.GetProperty<int?>("count") ?? 0;
        var expression = joinNode.GetProperty<string>("expression");
        // Quorum (B1): optional properties
        var quorumThresholdCount = joinNode.GetProperty<int?>("thresholdCount") ?? 0;
        double quorumThresholdPercent = 0;
        if (joinNode.Properties.TryGetValue("thresholdPercent", out var tpRaw))
        {
            if (tpRaw is int iTp) quorumThresholdPercent = iTp;
            else if (tpRaw is long lTp) quorumThresholdPercent = lTp;
            else if (tpRaw is double dTp) quorumThresholdPercent = dTp;
            else if (tpRaw is float fTp) quorumThresholdPercent = fTp;
            else if (tpRaw is string sTp && double.TryParse(sTp, out var p)) quorumThresholdPercent = p;
        }

        var root = SafeParseContext(instance);
        if (root["_parallelGroups"] is not JsonObject groups ||
            groups[gatewayId] is not JsonObject groupObj)
        {
            _logger.LogWarning("WF_JOIN_MISSING_GROUP Instance={InstanceId} JoinNode={JoinNode} Gateway={GatewayId}", instance.Id, joinNode.Id, gatewayId);
            return new JoinArrivalResult { Satisfied = true, NewlySatisfied = true, AddedToActive = true, CancelledBranches = new() };
        }

        // attach join metadata if missing
        if (!groupObj.TryGetPropertyValue("joinNodeId", out var jVal) || (jVal?.GetValue<string>() ?? "") == "")
            groupObj["joinNodeId"] = joinNode.Id;

        JsonArray branches = groupObj["branches"] as JsonArray ?? new JsonArray();
        groupObj["branches"] = branches;

        if (groupObj["join"] is not JsonObject joinMeta)
        {
            joinMeta = new JsonObject
            {
                ["nodeId"] = joinNode.Id,
                ["mode"] = mode,
                ["cancelRemaining"] = cancelRemaining,
                ["count"] = count,
                ["thresholdPercent"] = quorumThresholdPercent,
                ["thresholdCount"] = quorumThresholdCount,
                ["expression"] = expression,
                ["arrivals"] = new JsonArray(),
                ["satisfied"] = false,
                ["satisfiedAtUtc"] = null
            };

            // ---- B2: Timeout metadata capture (configured on join node) ----
            if (joinNode.Properties.TryGetValue("timeout", out var timeoutRaw) && timeoutRaw is JsonElement toEl && toEl.ValueKind == JsonValueKind.Object)
            {
                int seconds = 0;
                string? onTimeout = null;
                string? timeoutTarget = null;
                if (toEl.TryGetProperty("seconds", out var sEl) && sEl.ValueKind == JsonValueKind.Number)
                    seconds = sEl.GetInt32();
                if (toEl.TryGetProperty("onTimeout", out var otEl) && otEl.ValueKind == JsonValueKind.String)
                    onTimeout = otEl.GetString();
                if (toEl.TryGetProperty("target", out var tgtEl) && tgtEl.ValueKind == JsonValueKind.String)
                    timeoutTarget = tgtEl.GetString();

                if (seconds > 0)
                {
                    var timeoutAt = DateTime.UtcNow.AddSeconds(seconds);
                    joinMeta["timeoutSeconds"] = seconds;
                    joinMeta["timeoutAtUtc"] = timeoutAt.ToString("O");
                    joinMeta["onTimeout"] = (onTimeout ?? "force").Trim().ToLowerInvariant(); // route | fail | force
                    joinMeta["timeoutTarget"] = string.IsNullOrWhiteSpace(timeoutTarget) ? null : timeoutTarget;
                    joinMeta["timeoutTriggered"] = false;
                }
            }
            // ----------------------------------------------------------------
            groupObj["join"] = joinMeta;
        }

        var arrivals = joinMeta["arrivals"] as JsonArray ?? new JsonArray();
        joinMeta["arrivals"] = arrivals;

        var sourceId = sourceNode.Id;

        // record arrival if not already
        if (!arrivals.Any(a => a?.GetValue<string>()?.Equals(sourceId, StringComparison.OrdinalIgnoreCase) == true))
        {
            arrivals.Add(sourceId);

            // Emit arrival event
            CreateEventAsync(instance.Id, "Parallel", "ParallelJoinArrived",
                JsonSerializer.Serialize(new
                {
                    joinNodeId = joinNode.Id,
                    gatewayNodeId = gatewayId,
                    branchNodeId = sourceId,
                    mode,
                    arrivals = arrivals.Select(a => a!.GetValue<string>()),
                    totalBranches = branches.Count
                }), null).GetAwaiter().GetResult();
        }

        bool alreadySatisfied = joinMeta.TryGetPropertyValue("satisfied", out var satVal) &&
                            satVal is JsonValue sv && sv.TryGetValue<bool>(out var b) && b;

        if (alreadySatisfied)
        {
            instance.Context = root.ToJsonString();
            MarkDirty();
            return new JoinArrivalResult { Satisfied = true, NewlySatisfied = false, AddedToActive = false, CancelledBranches = new() };
        }

        // evaluate satisfaction
        bool satisfied;
        if (mode == "quorum")
        {
            // Determine effective quorum count
            int totalBranches = branches.Count;
            int effectiveCount = quorumThresholdCount;
            if (effectiveCount <= 0)
            {
                if (quorumThresholdPercent > 0)
                {
                    effectiveCount = (int)Math.Ceiling(totalBranches * (quorumThresholdPercent / 100.0));
                }
            }
            if (effectiveCount <= 0) effectiveCount = totalBranches; // fallback -> all
            joinMeta["thresholdCount"] = effectiveCount;
            joinMeta["thresholdPercent"] = quorumThresholdPercent;
            satisfied = arrivals.Count >= effectiveCount;
        }
        else
        {
            satisfied = mode switch
            {
                "all" => branches.All(bn => arrivals.Any(a => a!.GetValue<string>()!
                    .Equals(bn!.GetValue<string>(), StringComparison.OrdinalIgnoreCase))),
                "any" => arrivals.Count > 0,
                "count" => count > 0 && arrivals.Count >= count,
                "expression" => EvaluateJoinExpression(mode, expression, branches, arrivals, instance.Context),
                _ => branches.All(bn => arrivals.Any(a => a!.GetValue<string>()!
                    .Equals(bn!.GetValue<string>(), StringComparison.OrdinalIgnoreCase)))
            };
        }

        if (!satisfied)
        {
            instance.Context = root.ToJsonString();
            MarkDirty();
            return new JoinArrivalResult { Satisfied = false, NewlySatisfied = false, AddedToActive = false, CancelledBranches = new() };
        }

        // Mark satisfied
        joinMeta["satisfied"] = true;
        joinMeta["satisfiedAtUtc"] = DateTime.UtcNow.ToString("O");

        // determine cancellations
        var cancelled = new List<string>();
        if (cancelRemaining && (mode == "any" || mode == "count"))
        {
            var active = DeserializeActive(instance);
            var arrivedSet = arrivals
                .Select(a => a!.GetValue<string>())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var br in branches.OfType<JsonValue>()
                         .Select(v => v.GetValue<string>())
                         .Where(id => !arrivedSet.Contains(id)))
            {
                if (active.Contains(br, StringComparer.OrdinalIgnoreCase))
                {
                    RemoveActiveNode(instance, br);
                    cancelled.Add(br);
                    CancelOpenTasksForNode(instance, br, "join-cancelled");
                    CreateEventAsync(instance.Id, "Parallel", "ParallelJoinBranchCancelled",
                        JsonSerializer.Serialize(new
                        {
                            joinNodeId = joinNode.Id,
                            gatewayNodeId = gatewayId,
                            branchNodeId = br,
                            reason = "join-cancelRemaining"
                        }), null).GetAwaiter().GetResult();
                }
            }
        }

        // Emit satisfied event
        CreateEventAsync(instance.Id, "Parallel", "ParallelJoinSatisfied",
            JsonSerializer.Serialize(new
            {
                joinNodeId = joinNode.Id,
                gatewayNodeId = gatewayId,
                mode,
                count,
                cancelRemaining,
                quorumThresholdPercent = mode == "quorum" ? quorumThresholdPercent : (double?)null,
                quorumThresholdCount = mode == "quorum" ? (int?)joinMeta["thresholdCount"]!.GetValue<int>() : null,
                timeoutSeconds = joinMeta.TryGetPropertyValue("timeoutSeconds", out var ts) ? ts?.GetValue<int?>() : null,
                timeoutTriggered = false,
                arrivals = arrivals.Select(a => a!.GetValue<string>()),
                cancelledBranches = cancelled
            }), null).GetAwaiter().GetResult();

        instance.Context = root.ToJsonString();
        MarkDirty();

        return new JoinArrivalResult
        {
            Satisfied = true,
            NewlySatisfied = true,
            AddedToActive = true,
            CancelledBranches = cancelled
        };
    }

    private bool EvaluateJoinExpression(
        string mode,
        string? expression,
        JsonArray branches,
        JsonArray arrivals,
        string currentContextJson)
    {
        if (string.IsNullOrWhiteSpace(expression)) return false;

        try
        {
            // Build ephemeral context overlay with _joinEval
            var overlay = new Dictionary<string, object?>
            {
                ["_joinEval"] = new
                {
                    mode,
                    total = branches.Count,
                    arrived = arrivals.Count,
                    remaining = branches.Count - arrivals.Count,
                    arrivedIds = arrivals.Select(a => a!.GetValue<string>()).ToArray(),
                    branchIds = branches.OfType<JsonValue>().Select(b => b.GetValue<string>()).ToArray()
                }
            };

            // Merge simplistic (shallow) - if needed we can perform real merge later
            using var doc = JsonDocument.Parse(string.IsNullOrWhiteSpace(currentContextJson) ? "{}" : currentContextJson);
            var rootDict = new Dictionary<string, object?>();
            foreach (var prop in doc.RootElement.EnumerateObject())
                rootDict[prop.Name] = prop.Value.ToString();

            foreach (var kv in overlay)
                rootDict[kv.Key] = kv.Value;

            var mergedJson = JsonSerializer.Serialize(rootDict);
            return _conditionEvaluator
                .EvaluateAsync(expression!, mergedJson)
                .GetAwaiter()
                .GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WF_JOIN_EXPR_FAIL Expression evaluation failed");
            return false;
        }
    }

    private void CancelOpenTasksForNode(WorkflowInstance instance, string nodeId, string reason)
    {
        var openTasks = _context.WorkflowTasks
        .Where(t => t.WorkflowInstanceId == instance.Id &&
                    t.NodeId == nodeId &&
                    (t.Status == WorkflowTaskStatus.Created ||
                     t.Status == WorkflowTaskStatus.Assigned ||
                     t.Status == WorkflowTaskStatus.Claimed ||
                     t.Status == WorkflowTaskStatus.InProgress))
        .ToList();

        if (!openTasks.Any()) return;

        foreach (var t in openTasks)
        {
            t.Status = WorkflowTaskStatus.Cancelled;
            t.CompletedAt = DateTime.UtcNow;
            MarkDirty();

            CreateEventAsync(instance.Id, "Task", "Cancelled",
                JsonSerializer.Serialize(new
                {
                    taskId = t.Id,
                    nodeId = t.NodeId,
                    reason
                }), null).GetAwaiter().GetResult();
        }
    }
    #endregion

    #region Local Task Helpers / Open Set

    private static readonly WorkflowTaskStatus[] OpenStatuses =
    {
        WorkflowTaskStatus.Created,
        WorkflowTaskStatus.Assigned,
        WorkflowTaskStatus.Claimed,
        WorkflowTaskStatus.InProgress
    };

    private bool HasLocalOpenTask(int instanceId, string nodeId) =>
        _context.WorkflowTasks.Local.Any(t =>
            t.WorkflowInstanceId == instanceId &&
            t.NodeId == nodeId &&
            OpenStatuses.Contains(t.Status));

    private async Task<bool> HasOpenWaitingTaskAsync(int instanceId, WorkflowNode node, CancellationToken ct)
    {
        if (HasLocalOpenTask(instanceId, node.Id))
            return true;

        return await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId
                        && t.NodeId == node.Id
                        && OpenStatuses.Contains(t.Status))
            .AnyAsync(ct);
    }

    #endregion

    #region Notification Helper

    private async Task SafeNotifyInstanceAndListAsync(WorkflowInstance instance)
    {
        try
        {
            await _instanceNotifier.NotifyInstanceAsync(instance);
            await _instanceNotifier.NotifyInstancesChangedAsync(instance.TenantId);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "WF_NOTIFY_FAIL Instance={InstanceId}", instance.Id);
        }
    }

    #endregion
}
