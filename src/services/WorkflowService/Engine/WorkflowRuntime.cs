using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using DTOs.Workflow.Enums;

namespace WorkflowService.Engine;

/// <summary>
/// Main workflow runtime engine implementation
/// </summary>
public class WorkflowRuntime : IWorkflowRuntime
{
    private readonly WorkflowDbContext _context;
    private readonly IEnumerable<INodeExecutor> _executors;
    private readonly ILogger<WorkflowRuntime> _logger;

    public WorkflowRuntime(
        WorkflowDbContext context,
        IEnumerable<INodeExecutor> executors,
        ILogger<WorkflowRuntime> logger)
    {
        _context = context;
        _executors = executors;
        _logger = logger;
    }

    public async Task<WorkflowInstance> StartWorkflowAsync(int definitionId, string initialContext, int? startedByUserId = null, CancellationToken cancellationToken = default)
    {
        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId && d.IsPublished, cancellationToken);

        if (definition == null)
        {
            throw new InvalidOperationException($"Published workflow definition {definitionId} not found");
        }

        var workflowDef = WorkflowDefinitionJson.FromJson(definition.JSONDefinition);
        var startNode = workflowDef.Nodes.FirstOrDefault(n => n.IsStart());

        if (startNode == null)
        {
            throw new InvalidOperationException("Workflow definition must have a Start node");
        }

        // Create workflow instance
        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = definitionId,
            DefinitionVersion = definition.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = JsonSerializer.Serialize(new[] { startNode.Id }),
            Context = initialContext,
            StartedAt = DateTime.UtcNow,
            StartedByUserId = startedByUserId
        };

        _context.WorkflowInstances.Add(instance);
        await _context.SaveChangesAsync(cancellationToken);

        // Create start event
        await CreateEventAsync(instance.Id, "Instance", "Started", $"{{\"startNodeId\": \"{startNode.Id}\"}}", startedByUserId);

        _logger.LogInformation("Started workflow instance {InstanceId} from definition {DefinitionId}", instance.Id, definitionId);

        // Execute start node immediately
        await ContinueWorkflowAsync(instance.Id, cancellationToken);

        return instance;
    }

    public async Task ContinueWorkflowAsync(int instanceId, CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
        {
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");
        }

        if (instance.Status != InstanceStatus.Running)
        {
            _logger.LogWarning("Cannot continue workflow instance {InstanceId} in status {Status}", instanceId, instance.Status);
            return;
        }

        var workflowDef = WorkflowDefinitionJson.FromJson(instance.WorkflowDefinition.JSONDefinition);
        var currentNodeIds = JsonSerializer.Deserialize<string[]>(instance.CurrentNodeIds) ?? Array.Empty<string>();

        foreach (var nodeId in currentNodeIds)
        {
            var node = workflowDef.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node == null)
            {
                _logger.LogError("Node {NodeId} not found in workflow definition", nodeId);
                continue;
            }

            await ExecuteNodeAsync(node, instance, workflowDef, cancellationToken);
        }
    }

    public async Task SignalWorkflowAsync(int instanceId, string signalName, string signalData, int? userId = null, CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
        {
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");
        }

        // Create signal event
        await CreateEventAsync(instanceId, "Signal", signalName, signalData, userId);

        _logger.LogInformation("Received signal '{SignalName}' for workflow instance {InstanceId}", signalName, instanceId);

        // Continue workflow execution
        await ContinueWorkflowAsync(instanceId, cancellationToken);
    }

    public async Task CompleteTaskAsync(int taskId, string completionData, int completedByUserId, CancellationToken cancellationToken = default)
    {
        var task = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

        if (task == null)
        {
            throw new InvalidOperationException($"Workflow task {taskId} not found");
        }

        if (task.Status != DTOs.Workflow.Enums.TaskStatus.Claimed && task.Status != DTOs.Workflow.Enums.TaskStatus.InProgress)
        {
            throw new InvalidOperationException($"Task {taskId} is not in a completable state: {task.Status}");
        }

        // Update task
        task.Status = DTOs.Workflow.Enums.TaskStatus.Completed;
        task.CompletedAt = DateTime.UtcNow;
        task.CompletionData = completionData;

        // Update workflow context with task completion data
        UpdateWorkflowContext(task.WorkflowInstance, task.NodeId, completionData);

        // Create completion event
        await CreateEventAsync(task.WorkflowInstanceId, "Task", "Completed", 
            $"{{\"taskId\": {taskId}, \"nodeId\": \"{task.NodeId}\", \"completionData\": {completionData}}}", completedByUserId);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Completed task {TaskId} for workflow instance {InstanceId}", taskId, task.WorkflowInstanceId);

        // Continue workflow execution
        await ContinueWorkflowAsync(task.WorkflowInstanceId, cancellationToken);
    }

    public async Task CancelWorkflowAsync(int instanceId, string reason, int? cancelledByUserId = null, CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
        {
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");
        }

        instance.Status = InstanceStatus.Cancelled;
        instance.CompletedAt = DateTime.UtcNow;

        // Cancel any active tasks
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

        // Create cancellation event
        await CreateEventAsync(instanceId, "Instance", "Cancelled", $"{{\"reason\": \"{reason}\"}}", cancelledByUserId);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Cancelled workflow instance {InstanceId}: {Reason}", instanceId, reason);
    }

    public async Task RetryWorkflowAsync(int instanceId, string? resetToNodeId = null, CancellationToken cancellationToken = default)
    {
        var instance = await _context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .FirstOrDefaultAsync(i => i.Id == instanceId, cancellationToken);

        if (instance == null)
        {
            throw new InvalidOperationException($"Workflow instance {instanceId} not found");
        }

        if (instance.Status != InstanceStatus.Failed)
        {
            throw new InvalidOperationException($"Can only retry failed workflow instances. Current status: {instance.Status}");
        }

        // Reset instance state
        instance.Status = InstanceStatus.Running;
        instance.ErrorMessage = null;

        if (!string.IsNullOrEmpty(resetToNodeId))
        {
            instance.CurrentNodeIds = JsonSerializer.Serialize(new[] { resetToNodeId });
        }

        // Create retry event
        await CreateEventAsync(instanceId, "Instance", "Retried", 
            $"{{\"resetToNodeId\": \"{resetToNodeId ?? "current"}\"}}", null);

        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Retrying workflow instance {InstanceId}", instanceId);

        // Continue workflow execution
        await ContinueWorkflowAsync(instanceId, cancellationToken);
    }

    private async Task ExecuteNodeAsync(WorkflowNode node, WorkflowInstance instance, WorkflowDefinitionJson workflowDef, CancellationToken cancellationToken)
    {
        try
        {
            var executor = _executors.FirstOrDefault(e => e.CanExecute(node));
            if (executor == null)
            {
                throw new InvalidOperationException($"No executor found for node type {node.Type}");
            }

            _logger.LogDebug("Executing node {NodeId} of type {NodeType} for instance {InstanceId}", 
                node.Id, node.Type, instance.Id);

            var result = await executor.ExecuteAsync(node, instance, instance.Context, cancellationToken);

            if (!result.IsSuccess)
            {
                await HandleNodeExecutionFailure(instance, node, result.ErrorMessage);
                return;
            }

            // Update context if changed
            if (!string.IsNullOrEmpty(result.UpdatedContext))
            {
                instance.Context = result.UpdatedContext;
            }

            // Handle node completion
            if (!result.ShouldWait)
            {
                AdvanceToNextNodes(instance, workflowDef, result.NextNodeIds);
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Create node execution event
            await CreateEventAsync(instance.Id, "Node", "Executed", 
                $"{{\"nodeId\": \"{node.Id}\", \"nodeType\": \"{node.Type}\", \"nextNodes\": {JsonSerializer.Serialize(result.NextNodeIds)}}}", null);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing node {NodeId} for instance {InstanceId}", node.Id, instance.Id);
            await HandleNodeExecutionFailure(instance, node, ex.Message);
        }
    }

    private void AdvanceToNextNodes(WorkflowInstance instance, WorkflowDefinitionJson workflowDef, List<string> nextNodeIds)
    {
        if (!nextNodeIds.Any())
        {
            // No next nodes - workflow is complete
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            instance.CurrentNodeIds = JsonSerializer.Serialize(Array.Empty<string>());
        }
        else
        {
            // Update current nodes
            instance.CurrentNodeIds = JsonSerializer.Serialize(nextNodeIds.ToArray());

            // Check if any next nodes are End nodes
            var endNodes = nextNodeIds.Where(id => 
                workflowDef.Nodes.Any(n => n.Id == id && n.IsEnd())).ToList();

            if (endNodes.Any())
            {
                instance.Status = InstanceStatus.Completed;
                instance.CompletedAt = DateTime.UtcNow;
            }
        }
    }

    private async Task HandleNodeExecutionFailure(WorkflowInstance instance, WorkflowNode node, string? errorMessage)
    {
        instance.Status = InstanceStatus.Failed;
        instance.ErrorMessage = errorMessage;
        instance.CompletedAt = DateTime.UtcNow;

        await CreateEventAsync(instance.Id, "Node", "Failed", 
            $"{{\"nodeId\": \"{node.Id}\", \"error\": \"{errorMessage}\"}}", null);

        await _context.SaveChangesAsync();

        _logger.LogError("Node {NodeId} execution failed for instance {InstanceId}: {Error}", 
            node.Id, instance.Id, errorMessage);
    }

    private void UpdateWorkflowContext(WorkflowInstance instance, string nodeId, string completionData)
    {
        try
        {
            var context = JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context) ?? new();
            var completion = JsonSerializer.Deserialize<Dictionary<string, object>>(completionData) ?? new();

            // Add completion data under node-specific key
            context[$"task_{nodeId}"] = completion;

            instance.Context = JsonSerializer.Serialize(context);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update workflow context for instance {InstanceId}", instance.Id);
        }
    }

    private Task CreateEventAsync(int instanceId, string type, string name, string data, int? userId)
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

        // Also create outbox message for future event publishing
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
            })
        };

        _context.OutboxMessages.Add(outboxMessage);

        return Task.CompletedTask;
    }
}
