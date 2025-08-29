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
/// Main workflow runtime engine implementation
/// </summary>
public class WorkflowRuntime : IWorkflowRuntime
{
    private readonly WorkflowDbContext _context;
    private readonly IEnumerable<INodeExecutor> _executors;
    private readonly ILogger<WorkflowRuntime> _logger;
    private readonly ITenantProvider _tenantProvider;

    public WorkflowRuntime(
        WorkflowDbContext context,
        IEnumerable<INodeExecutor> executors,
        ITenantProvider tenantProvider,
        ILogger<WorkflowRuntime> logger)
    {
        _context = context;
        _executors = executors;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

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

        // Adapt raw builder JSON (from/to, label, x,y, lowercase types) into runtime model
        var workflowDef = BuilderDefinitionAdapter.Parse(definition.JSONDefinition);

        var startNode = workflowDef.Nodes.FirstOrDefault(n => n.IsStart());
        if (startNode == null)
            throw new InvalidOperationException("Workflow definition must have a Start node");

        _logger.LogInformation("Found start node: {NodeId} of type {NodeType}", startNode.Id, startNode.Type);

        _logger.LogInformation(
            "Parsed workflow definition {DefinitionId}: Nodes={NodeCount}, Edges={EdgeCount}",
            definitionId, workflowDef.Nodes.Count, workflowDef.Edges.Count);

        if (workflowDef.Edges.Count == 0)
        {
            _logger.LogWarning(
                "Workflow definition {DefinitionId} has zero edges after adaptation. Raw JSON length={Len}",
                definitionId, definition.JSONDefinition?.Length);
        }
        else
        {
            _logger.LogDebug("Edge samples: {Samples}",
                string.Join(", ",
                    workflowDef.Edges.Take(3).Select(e => $"{e.Source}->{e.Target}")));
        }

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

        _logger.LogInformation("Created workflow instance {InstanceId} with start node {StartNodeId}",
            instance.Id, startNode.Id);

        await CreateEventAsync(instance.Id, "Instance", "Started",
            $"{{\"startNodeId\": \"{startNode.Id}\"}}", startedByUserId);

        _logger.LogInformation("Starting workflow execution for instance {InstanceId}", instance.Id);

        // Kick off first execution pass
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

        if (instance.Status != InstanceStatus.Running)
        {
            _logger.LogWarning("Cannot continue workflow instance {InstanceId} in status {Status}",
                instanceId, instance.Status);
            return;
        }

        var workflowDef = BuilderDefinitionAdapter.Parse(instance.WorkflowDefinition.JSONDefinition);

        var currentNodeIds = JsonSerializer.Deserialize<string[]>(instance.CurrentNodeIds) ?? Array.Empty<string>();

        _logger.LogInformation("Continuing workflow instance {InstanceId} with current nodes: [{CurrentNodes}]",
            instanceId, string.Join(", ", currentNodeIds));

        foreach (var nodeId in currentNodeIds)
        {
            var node = workflowDef.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null)
            {
                _logger.LogError("Node {NodeId} not found in workflow definition. Available nodes: [{AvailableNodes}]",
                    nodeId, string.Join(", ", workflowDef.Nodes.Select(n => $"{n.Id}({n.Type})")));
                continue;
            }

            _logger.LogInformation("Found node {NodeId} of type {NodeType}, executing...", node.Id, node.Type);
            await ExecuteNodeAsync(node, instance, workflowDef, cancellationToken);

            // Instance status may have changed (e.g., Completed)
            if (instance.Status != InstanceStatus.Running)
                break;
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

        await CreateEventAsync(instanceId, "Signal", signalName, signalData, userId);

        _logger.LogInformation("Received signal '{SignalName}' for workflow instance {InstanceId}",
            signalName, instanceId);

        await ContinueWorkflowAsync(instanceId, cancellationToken);
    }

    public async Task CompleteTaskAsync(
        int taskId,
        string completionData,
        int completedByUserId,
        CancellationToken cancellationToken = default)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task == null)
            throw new InvalidOperationException($"Workflow task {taskId} not found");

        if (task.Status != DTOs.Workflow.Enums.TaskStatus.Claimed &&
            task.Status != DTOs.Workflow.Enums.TaskStatus.InProgress)
            throw new InvalidOperationException($"Task {taskId} is not in a completable state: {task.Status}");

        task.Status = DTOs.Workflow.Enums.TaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.CompletionData = completionData;

        UpdateWorkflowContext(task.WorkflowInstance, task.NodeId, completionData);

        await CreateEventAsync(task.WorkflowInstanceId, "Task", "Completed",
            $"{{\"taskId\": {taskId}, \"nodeId\": \"{task.NodeId}\", \"completionData\": {completionData}}}",
            completedByUserId);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Completed task {TaskId} for workflow instance {InstanceId}",
            taskId, task.WorkflowInstanceId);

        await ContinueWorkflowAsync(task.WorkflowInstanceId, cancellationToken);
    }

    public async Task CancelWorkflowAsync(
        int instanceId,
        string reason,
        int? cancelledByUserId = null,
        CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        instance.Status = InstanceStatus.Cancelled;
        instance.CompletedAt = DateTime.UtcNow;

        var activeTasks = await _context.WorkflowTasks
            .Where(t => t.WorkflowInstanceId == instanceId &&
                        (t.Status == DTOs.Workflow.Enums.TaskStatus.Created ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.Assigned ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.Claimed ||
                         t.Status == DTOs.Workflow.Enums.TaskStatus.InProgress))
            .ToListAsync(cancellationToken);

        foreach (var task in activeTasks)
        {
            task.Status = DTOs.Workflow.Enums.TaskStatus.Cancelled;
            task.CompletedAt = DateTime.UtcNow;
        }

        await CreateEventAsync(instanceId, "Instance", "Cancelled",
            $"{{\"reason\": \"{reason}\"}}", cancelledByUserId);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cancelled workflow instance {InstanceId}: {Reason}", instanceId, reason);
    }

    public async Task RetryWorkflowAsync(
        int instanceId,
        string? resetToNodeId = null,
        CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");

        if (instance.Status != InstanceStatus.Failed)
            throw new InvalidOperationException(
                $"Can only retry failed workflow instances. Current status: {instance.Status}");

        instance.Status = InstanceStatus.Running;
        instance.ErrorMessage = null;

        if (!string.IsNullOrEmpty(resetToNodeId))
            instance.CurrentNodeIds = JsonSerializer.Serialize(new[] { resetToNodeId });

        await CreateEventAsync(instanceId, "Instance", "Retried",
            $"{{\"resetToNodeId\": \"{resetToNodeId ?? "current"}\"}}", null);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Retrying workflow instance {InstanceId}", instanceId);

        await ContinueWorkflowAsync(instanceId, cancellationToken);
    }

    private async Task ExecuteNodeAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        WorkflowDefinitionJson workflowDef,
        CancellationToken cancellationToken)
    {
        try
        {
            var executor = _executors.FirstOrDefault(e => e.CanExecute(node));
            if (executor == null)
                throw new InvalidOperationException($"No executor found for node type {node.Type}");

            _logger.LogInformation(
                "Executing node {NodeId} of type {NodeType} for instance {InstanceId} using executor {ExecutorType}",
                node.Id, node.Type, instance.Id, executor.GetType().Name);

            var result = await executor.ExecuteAsync(node, instance, instance.Context, cancellationToken);

            if (!result.IsSuccess)
            {
                _logger.LogError("Node execution failed for {NodeId}: {ErrorMessage}",
                    node.Id, result.ErrorMessage);
                await HandleNodeExecutionFailure(instance, node, result.ErrorMessage);
                return;
            }

            _logger.LogInformation(
                "Node {NodeId} executed successfully. ShouldWait: {ShouldWait}, TaskCreated: {TaskCreated}",
                node.Id, result.ShouldWait, result.CreatedTask != null);

            if (!string.IsNullOrEmpty(result.UpdatedContext))
                instance.Context = result.UpdatedContext;

            if (result.CreatedTask != null)
            {
                _context.WorkflowTasks.Add(result.CreatedTask);
                _logger.LogInformation(
                    "Created task {TaskName} (pending ID) for workflow instance {InstanceId}",
                    result.CreatedTask.TaskName, instance.Id);
            }

            if (!result.ShouldWait)
            {
                var nextNodeIds = result.NextNodeIds.Any()
                    ? result.NextNodeIds
                    : GetNextNodeIds(node, workflowDef);

                _logger.LogInformation(
                    "Node {NodeId} completed, advancing to next nodes: [{NextNodes}]",
                    node.Id, string.Join(", ", nextNodeIds));

                AdvanceToNextNodes(instance, workflowDef, nextNodeIds);
            }
            else
            {
                _logger.LogInformation("Node {NodeId} is waiting (human task or async wait)", node.Id);
            }

            await _context.SaveChangesAsync(cancellationToken);

            await CreateEventAsync(instance.Id, "Node", "Executed",
                $"{{\"nodeId\": \"{node.Id}\", \"nodeType\": \"{node.Type}\", \"shouldWait\": {result.ShouldWait.ToString().ToLower()}, \"taskCreated\": {(result.CreatedTask != null).ToString().ToLower()}}}",
                null);

            if (!result.ShouldWait && instance.Status == InstanceStatus.Running)
            {
                _logger.LogInformation("Continuing workflow execution for instance {InstanceId}", instance.Id);
                await ContinueWorkflowAsync(instance.Id, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error executing node {NodeId} for instance {InstanceId}",
                node.Id, instance.Id);
            await HandleNodeExecutionFailure(instance, node, ex.Message);
        }
    }

    private List<string> GetNextNodeIds(WorkflowNode currentNode, WorkflowDefinitionJson workflowDef)
    {
        var outgoingEdges = workflowDef.Edges
            .Where(e => e.Source == currentNode.Id)
            .ToList();

        _logger.LogInformation(
            "Found {EdgeCount} outgoing edges from node {NodeId}: [{Edges}]",
            outgoingEdges.Count,
            currentNode.Id,
            string.Join(", ", outgoingEdges.Select(e => $"{e.Source}->{e.Target}")));

        return outgoingEdges.Select(e => e.Target).ToList();
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
            return;
        }

        instance.CurrentNodeIds = JsonSerializer.Serialize(nextNodeIds.ToArray());

        var endNodes = nextNodeIds.Where(id =>
            workflowDef.Nodes.Any(n => n.Id == id && n.IsEnd())).ToList();

        if (endNodes.Any())
        {
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
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
            $"{{\"nodeId\": \"{node.Id}\", \"error\": \"{errorMessage}\"}}",
            null);

        await _context.SaveChangesAsync();

        _logger.LogError("Node {NodeId} execution failed for instance {InstanceId}: {Error}",
            node.Id, instance.Id, errorMessage);
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
            _logger.LogWarning(ex,
                "Failed to update workflow context for instance {InstanceId}",
                instance.Id);
        }
    }

    private Task CreateEventAsync(
        int instanceId,
        string type,
        string name,
        string data,
        int? userId)
    {
        var workflowEvent = new WorkflowEvent
        {
            WorkflowInstanceId = instanceId,
            Type = type,
            Name = name,
            Data = data,
            OccurredAt = DateTime.UtcNow,
            UserId = userId
        };

        _context.WorkflowEvents.Add(workflowEvent);

        var outboxMessage = new OutboxMessage
        {
            Type = $"Workflow{type}{name}",
            Payload = JsonSerializer.Serialize(new
            {
                InstanceId = instanceId,
                EventType = type,
                EventName = name,
                EventData = data,
                UserId = userId,
                OccurredAt = DateTime.UtcNow
            }),
            Processed = false,
            EventType = $"Workflow.{type}.{name}".ToLower(),
            EventData = data,
            IsProcessed = false,
            RetryCount = 0
        };

        _context.OutboxMessages.Add(outboxMessage);

        return Task.CompletedTask;
    }
}
