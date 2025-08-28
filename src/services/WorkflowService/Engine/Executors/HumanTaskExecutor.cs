using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using Contracts.Services; // ✅ ADD: Missing using directive for IRoleService
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for human task nodes
/// </summary>
public class HumanTaskExecutor : INodeExecutor
{
    private readonly ILogger<HumanTaskExecutor> _logger;
    private readonly IRoleService _roleService; // ✅ NOW RESOLVED: IRoleService reference

    public string NodeType => NodeTypes.HumanTask;

    public HumanTaskExecutor(ILogger<HumanTaskExecutor> logger, IRoleService roleService)
    {
        _logger = logger;
        _roleService = roleService;
    }

    public bool CanExecute(WorkflowNode node) => node.IsHumanTask();

    // ✅ FIX: Make method synchronous to match interface signature
    public Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            // ✅ FIXED: Use available method to validate role existence
            var assignedRole = node.GetAssignedToRole();
            if (!string.IsNullOrEmpty(assignedRole))
            {
                // ✅ NEW: Use IsRoleNameAvailableAsync to check if role exists
                // If role is NOT available, it means it exists (inverse logic)
                var roleTask = _roleService.IsRoleNameAvailableAsync(assignedRole, null, cancellationToken);
                roleTask.ContinueWith(task =>
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        var roleExists = !task.Result; // Inverse logic: if not available, it exists
                        if (!roleExists)
                        {
                            _logger.LogWarning("Assigned role '{Role}' no longer exists in tenant {TenantId} for task {NodeId}", 
                                assignedRole, instance.TenantId, node.Id);
                            // Could either fail the task or assign to a default role
                        }
                    }
                }, TaskContinuationOptions.OnlyOnRanToCompletion);
            }

            // Create workflow task
            var task = new WorkflowTask
            {
                TenantId = instance.TenantId, // ✅ FIX: Set tenant ID
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

            _logger.LogInformation("Created human task {TaskName} for workflow instance {InstanceId} with tenant {TenantId}", 
                node.GetTaskName(), instance.Id, instance.TenantId);

            var result = new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = true, // Human tasks always wait for completion
                CreatedTask = task,
                NextNodeIds = new List<string>() // Will be set when task is completed
            };

            // ✅ FIX: Return Task.FromResult instead of direct return
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
            // ✅ FIX: Return Task.FromResult instead of direct return
            return Task.FromResult(result);
        }
    }
}
