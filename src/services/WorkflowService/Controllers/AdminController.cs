using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Controllers;

/// <summary>
/// Administrative controller for workflow system management
/// </summary>
[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
[RequiresPermission(Permissions.Workflow.Admin)]
public class AdminController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<AdminController> _logger;

    public AdminController(
        WorkflowDbContext context,
        ILogger<AdminController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get workflow system statistics and health
    /// </summary>
    [HttpGet("stats")]
    public async Task<ActionResult<ApiResponseDto<WorkflowStatsDto>>> GetWorkflowStats()
    {
        try
        {
            var stats = new WorkflowStatsDto
            {
                TotalDefinitions = await _context.WorkflowDefinitions.CountAsync(),
                PublishedDefinitions = await _context.WorkflowDefinitions.CountAsync(d => d.IsPublished),
                
                TotalInstances = await _context.WorkflowInstances.CountAsync(),
                RunningInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == InstanceStatus.Running),
                CompletedInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == InstanceStatus.Completed),
                FailedInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == InstanceStatus.Failed),
                SuspendedInstances = await _context.WorkflowInstances.CountAsync(i => i.Status == InstanceStatus.Suspended),
                
                TotalTasks = await _context.WorkflowTasks.CountAsync(),
                PendingTasks = await _context.WorkflowTasks.CountAsync(t => 
                    t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned),
                ActiveTasks = await _context.WorkflowTasks.CountAsync(t => 
                    t.Status == WorkflowTaskStatus.Claimed || t.Status == WorkflowTaskStatus.InProgress),
                CompletedTasks = await _context.WorkflowTasks.CountAsync(t => t.Status == WorkflowTaskStatus.Completed),
                OverdueTasks = await _context.WorkflowTasks.CountAsync(t => 
                    t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow && 
                    t.Status != WorkflowTaskStatus.Completed && t.Status != WorkflowTaskStatus.Cancelled),
                
                TotalEvents = await _context.WorkflowEvents.CountAsync(),
                EventsLast24Hours = await _context.WorkflowEvents.CountAsync(e => 
                    e.OccurredAt >= DateTime.UtcNow.AddDays(-1))
            };

            return Ok(ApiResponseDto<WorkflowStatsDto>.SuccessResult(
                stats, 
                "Workflow statistics retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow statistics");
            return StatusCode(500, ApiResponseDto<WorkflowStatsDto>.ErrorResult(
                "An error occurred while retrieving workflow statistics"));
        }
    }

    /// <summary>
    /// Retry a failed workflow instance
    /// </summary>
    [HttpPost("instances/{id}/retry")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> RetryInstance(
        int id,
        [FromBody] RetryInstanceRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow instance not found"));
            }

            if (instance.Status != InstanceStatus.Failed)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Only failed instances can be retried"));
            }

            // Reset instance to running state
            instance.Status = InstanceStatus.Running;
            instance.ErrorMessage = null;
            instance.CompletedAt = null;

            // TODO: Reset failed tasks to appropriate state
            // TODO: Trigger workflow engine to continue from failure point

            // Create retry event
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Admin",
                Name = "InstanceRetried",
                Data = $"{{\"retriedBy\": {currentUserId}, \"reason\": \"{request.RetryReason ?? ""}\", \"resetToNode\": \"{request.ResetToNodeId ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            var responseDto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                WorkflowDefinitionName = instance.WorkflowDefinition.Name,
                DefinitionVersion = instance.DefinitionVersion,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                Context = instance.Context,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                StartedByUserId = instance.StartedByUserId,
                ErrorMessage = instance.ErrorMessage,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt
            };

            _logger.LogInformation("Admin {UserId} retried failed instance {InstanceId}", 
                currentUserId, id);

            return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                responseDto, 
                "Workflow instance retry initiated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while retrying the workflow instance"));
        }
    }

    /// <summary>
    /// Move an instance to a specific node (advanced admin operation)
    /// </summary>
    [HttpPost("instances/{id}/move-to-node")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> MoveInstanceToNode(
        int id,
        [FromBody] MoveToNodeRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow instance not found"));
            }

            if (instance.Status == InstanceStatus.Completed)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Cannot move completed instances"));
            }

            // TODO: Validate that the target node exists in the workflow definition
            // TODO: Cancel any active tasks for current nodes
            // TODO: Update instance state to new node

            // Update current node
            instance.CurrentNodeIds = $"[\"{request.TargetNodeId}\"]";
            instance.Status = InstanceStatus.Running;

            // Merge context if provided
            if (!string.IsNullOrEmpty(request.ContextUpdates))
            {
                // TODO: Merge JSON context properly
                instance.Context = request.ContextUpdates;
            }

            // Create move event
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Admin",
                Name = "InstanceMovedToNode",
                Data = $"{{\"movedBy\": {currentUserId}, \"targetNodeId\": \"{request.TargetNodeId}\", \"reason\": \"{request.Reason ?? ""}\", \"contextUpdates\": {request.ContextUpdates ?? "null"}}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            var responseDto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                WorkflowDefinitionName = instance.WorkflowDefinition.Name,
                DefinitionVersion = instance.DefinitionVersion,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                Context = instance.Context,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                StartedByUserId = instance.StartedByUserId,
                ErrorMessage = instance.ErrorMessage,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt
            };

            _logger.LogInformation("Admin {UserId} moved instance {InstanceId} to node {NodeId}", 
                currentUserId, id, request.TargetNodeId);

            return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                responseDto, 
                $"Instance moved to node '{request.TargetNodeId}' successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving instance {Id} to node", id);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while moving the instance"));
        }
    }

    /// <summary>
    /// Reset a task to a different status (admin operation)
    /// </summary>
    [HttpPost("tasks/{id}/reset")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> ResetTask(
        int id,
        [FromBody] ResetTaskRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
            {
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult(
                    "Workflow task not found"));
            }

            var oldStatus = task.Status;

            // Reset task properties based on new status
            task.Status = request.NewStatus;
            
            switch (request.NewStatus)
            {
                case WorkflowTaskStatus.Created:
                    task.AssignedToUserId = null;
                    task.ClaimedAt = null;
                    task.CompletedAt = null;
                    task.CompletionData = null;
                    task.ErrorMessage = null;
                    break;
                    
                case WorkflowTaskStatus.Assigned:
                    task.AssignedToUserId = request.AssignToUserId;
                    task.ClaimedAt = null;
                    task.CompletedAt = null;
                    task.CompletionData = null;
                    task.ErrorMessage = null;
                    break;
                    
                case WorkflowTaskStatus.Failed:
                    task.ErrorMessage = request.ErrorMessage;
                    task.CompletedAt = DateTime.UtcNow;
                    break;
            }

            // Create reset event
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Admin",
                Name = "TaskReset",
                Data = $"{{\"taskId\": {task.Id}, \"resetBy\": {currentUserId}, \"oldStatus\": \"{oldStatus}\", \"newStatus\": \"{request.NewStatus}\", \"reason\": \"{request.Reason ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            var responseDto = new WorkflowTaskDto
            {
                Id = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                NodeId = task.NodeId,
                TaskName = task.TaskName,
                Status = task.Status,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToRole = task.AssignedToRole,
                DueDate = task.DueDate,
                Data = task.Data,
                ClaimedAt = task.ClaimedAt,
                CompletedAt = task.CompletedAt,
                CompletionData = task.CompletionData,
                ErrorMessage = task.ErrorMessage,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };

            _logger.LogInformation("Admin {UserId} reset task {TaskId} from {OldStatus} to {NewStatus}", 
                currentUserId, id, oldStatus, request.NewStatus);

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(
                responseDto, 
                $"Task reset from {oldStatus} to {request.NewStatus} successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resetting task {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult(
                "An error occurred while resetting the task"));
        }
    }

    /// <summary>
    /// Get workflow events for debugging and audit
    /// </summary>
    [HttpGet("events")]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowEventDto>>>> GetWorkflowEvents(
        [FromQuery] int? instanceId = null,
        [FromQuery] string? eventType = null,
        [FromQuery] int? userId = null,
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100)
    {
        try
        {
            var query = _context.WorkflowEvents.AsQueryable();

            // Apply filters
            if (instanceId.HasValue)
            {
                query = query.Where(e => e.WorkflowInstanceId == instanceId.Value);
            }

            if (!string.IsNullOrEmpty(eventType))
            {
                query = query.Where(e => e.Type == eventType);
            }

            if (userId.HasValue)
            {
                query = query.Where(e => e.UserId == userId.Value);
            }

            if (from.HasValue)
            {
                query = query.Where(e => e.OccurredAt >= from.Value);
            }

            if (to.HasValue)
            {
                query = query.Where(e => e.OccurredAt <= to.Value);
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var events = await query
                .OrderByDescending(e => e.OccurredAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new WorkflowEventDto
                {
                    Id = e.Id,
                    WorkflowInstanceId = e.WorkflowInstanceId,
                    Type = e.Type,
                    Name = e.Name,
                    Data = e.Data,
                    OccurredAt = e.OccurredAt,
                    UserId = e.UserId,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponseDto<List<WorkflowEventDto>>.SuccessResult(
                events, 
                $"Retrieved {events.Count} of {totalCount} workflow events"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow events");
            return StatusCode(500, ApiResponseDto<List<WorkflowEventDto>>.ErrorResult(
                "An error occurred while retrieving workflow events"));
        }
    }

    /// <summary>
    /// Bulk cancel instances by criteria (emergency operation)
    /// </summary>
    [HttpPost("instances/bulk-cancel")]
    public async Task<ActionResult<ApiResponseDto<BulkOperationResultDto>>> BulkCancelInstances(
        [FromBody] BulkCancelInstancesRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var query = _context.WorkflowInstances.AsQueryable();

            // Apply filters
            if (request.WorkflowDefinitionId.HasValue)
            {
                query = query.Where(i => i.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(i => i.Status == request.Status.Value);
            }

            if (request.StartedBefore.HasValue)
            {
                query = query.Where(i => i.StartedAt < request.StartedBefore.Value);
            }

            // Get instances to cancel
            var instancesToCancel = await query
                .Where(i => i.Status == InstanceStatus.Running || i.Status == InstanceStatus.Suspended)
                .ToListAsync();

            if (!instancesToCancel.Any())
            {
                return BadRequest(ApiResponseDto<BulkOperationResultDto>.ErrorResult(
                    "No instances found matching the criteria"));
            }

            if (instancesToCancel.Count > 100)
            {
                return BadRequest(ApiResponseDto<BulkOperationResultDto>.ErrorResult(
                    "Cannot cancel more than 100 instances at once"));
            }

            // Cancel instances
            var cancelledCount = 0;
            var events = new List<WorkflowEvent>();

            foreach (var instance in instancesToCancel)
            {
                instance.Status = InstanceStatus.Cancelled;
                instance.CompletedAt = DateTime.UtcNow;
                cancelledCount++;

                events.Add(new WorkflowEvent
                {
                    WorkflowInstanceId = instance.Id,
                    Type = "Admin",
                    Name = "BulkCancelled",
                    Data = $"{{\"cancelledBy\": {currentUserId}, \"reason\": \"{request.Reason ?? ""}\", \"bulkOperation\": true}}",
                    OccurredAt = DateTime.UtcNow,
                    UserId = currentUserId
                });
            }

            _context.WorkflowEvents.AddRange(events);
            await _context.SaveChangesAsync();

            var result = new BulkOperationResultDto
            {
                SuccessCount = cancelledCount,
                FailureCount = 0,
                TotalCount = cancelledCount,
                OperationType = "Cancel Instances"
            };

            _logger.LogWarning("Admin {UserId} bulk cancelled {Count} instances: {Reason}", 
                currentUserId, cancelledCount, request.Reason);

            return Ok(ApiResponseDto<BulkOperationResultDto>.SuccessResult(
                result, 
                $"Successfully cancelled {cancelledCount} workflow instances"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk cancel operation");
            return StatusCode(500, ApiResponseDto<BulkOperationResultDto>.ErrorResult(
                "An error occurred while performing the bulk cancel operation"));
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
