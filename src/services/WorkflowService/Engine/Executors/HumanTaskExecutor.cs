using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for human task nodes
/// </summary>
public class HumanTaskExecutor : INodeExecutor
{
    private readonly ILogger<HumanTaskExecutor> _logger;

    public string NodeType => NodeTypes.HumanTask;

    public HumanTaskExecutor(ILogger<HumanTaskExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => node.IsHumanTask();

    public Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Create workflow task
            var task = new WorkflowTask
            {
                WorkflowInstanceId = instance.Id,
                NodeId = node.Id,
                TaskName = node.GetTaskName(),
                Status = WorkflowTaskStatus.Created,
                Data = JsonSerializer.Serialize(new
                {
                    FormSchema = node.GetFormSchema(),
                    Instructions = node.GetProperty<string>("instructions", ""),
                    Priority = node.GetProperty<string>("priority", "normal")
                }),
                DueDate = node.GetTimeoutDuration().HasValue 
                    ? DateTime.UtcNow.Add(node.GetTimeoutDuration()!.Value)
                    : null
            };

            // Assign task
            var assignedUserId = node.GetAssignedToUserId();
            var assignedRole = node.GetAssignedToRole();
            
            if (!string.IsNullOrEmpty(assignedUserId) && int.TryParse(assignedUserId, out var userId))
            {
                task.AssignedToUserId = userId;
                task.Status = WorkflowTaskStatus.Assigned;
            }
            else if (!string.IsNullOrEmpty(assignedRole))
            {
                task.AssignedToRole = assignedRole;
                task.Status = WorkflowTaskStatus.Assigned;
            }

            _logger.LogInformation("Created human task {TaskName} for workflow instance {InstanceId}", 
                node.GetTaskName(), instance.Id);

            var result = new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = true, // Human tasks always wait for completion
                CreatedTask = task,
                NextNodeIds = new List<string>() // Will be set when task is completed
            };

            return Task.FromResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing human task node {NodeId}", node.Id);
            var result = new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
            return Task.FromResult(result);
        }
    }
}
