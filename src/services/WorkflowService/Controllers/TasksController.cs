using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<TasksController> _logger;
    private readonly IWorkflowExecutionService _execution;

    public TasksController(
        WorkflowDbContext context,
        ILogger<TasksController> logger,
        IWorkflowExecutionService execution)
    {
        _context = context;
        _logger = logger;
        _execution = execution;
    }

    /// <summary>
    /// General task search.
    /// mine=true by default only includes tasks directly assigned to the user.
    /// Use mineIncludeRoles=true to also include tasks assigned to any of the user's roles.
    /// Use mineIncludeUnassigned=true to include claimable (unassigned) tasks.
    /// </summary>
    [HttpGet]
    [RequiresPermission("workflow.view_tasks")]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowTaskDto>>>> GetTasks(
        [FromQuery] bool? mine = null,
        [FromQuery] bool mineIncludeRoles = false,
        [FromQuery] bool mineIncludeUnassigned = false,
        [FromQuery] WorkflowTaskStatus? status = null,
        [FromQuery] int? workflowInstanceId = null,
        [FromQuery] int? assignedToUserId = null,
        [FromQuery] string? assignedToRole = null,
        [FromQuery] bool? overdue = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userRoles = GetCurrentUserRoles();

            var query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
                .AsQueryable();

            // Mine filtering expansion
            if (mine == true)
            {
                query = query.Where(t =>
                    t.AssignedToUserId == currentUserId
                    || (mineIncludeRoles && t.AssignedToRole != null && userRoles.Contains(t.AssignedToRole))
                    || (mineIncludeUnassigned && t.AssignedToUserId == null && t.AssignedToRole == null &&
                        (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned))
                );
            }

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (workflowInstanceId.HasValue)
                query = query.Where(t => t.WorkflowInstanceId == workflowInstanceId.Value);

            if (assignedToUserId.HasValue)
                query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);

            if (!string.IsNullOrEmpty(assignedToRole))
                query = query.Where(t => t.AssignedToRole == assignedToRole);

            if (overdue == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value < now &&
                                         t.Status != WorkflowTaskStatus.Completed &&
                                         t.Status != WorkflowTaskStatus.Cancelled);
            }

            var totalCount = await query.CountAsync();
            var tasks = await query
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new WorkflowTaskDto
                {
                    Id = t.Id,
                    WorkflowInstanceId = t.WorkflowInstanceId,
                    NodeId = t.NodeId,
                    TaskName = t.TaskName,
                    Status = t.Status,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToRole = t.AssignedToRole,
                    DueDate = t.DueDate,
                    Data = t.Data,
                    ClaimedAt = t.ClaimedAt,
                    CompletedAt = t.CompletedAt,
                    CompletionData = t.CompletionData,
                    ErrorMessage = t.ErrorMessage,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponseDto<List<WorkflowTaskDto>>.SuccessResult(
                tasks,
                $"Retrieved {tasks.Count} of {totalCount} workflow tasks"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow tasks");
            return StatusCode(500, ApiResponseDto<List<WorkflowTaskDto>>.ErrorResult(
                "An error occurred while retrieving workflow tasks"));
        }
    }

    /// <summary>
    /// Focused "My Tasks" endpoint with optional inclusion of role and unassigned tasks.
    /// </summary>
    [HttpGet("mine")]
    [RequiresPermission("workflow.view_tasks")]
    public async Task<ActionResult<ApiResponseDto<List<TaskSummaryDto>>>> GetMyTasks(
        [FromQuery] WorkflowTaskStatus? status = null,
        [FromQuery] bool includeRoleTasks = true,
        [FromQuery] bool includeUnassigned = true)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var userRoles = GetCurrentUserRoles();

            var baseQuery = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
                .AsQueryable();

            baseQuery = baseQuery.Where(t =>
                t.AssignedToUserId == currentUserId
                || (includeRoleTasks && t.AssignedToRole != null && userRoles.Contains(t.AssignedToRole))
                || (includeUnassigned && t.AssignedToUserId == null && t.AssignedToRole == null &&
                    (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned)));

            if (status.HasValue)
                baseQuery = baseQuery.Where(t => t.Status == status.Value);

            var tasks = await baseQuery
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(t => t.CreatedAt)
                .Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    TaskName = t.TaskName,
                    Status = t.Status,
                    WorkflowDefinitionName = t.WorkflowInstance.WorkflowDefinition.Name,
                    WorkflowInstanceId = t.WorkflowInstanceId,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            return Ok(ApiResponseDto<List<TaskSummaryDto>>.SuccessResult(
                tasks,
                $"Retrieved {tasks.Count} tasks (you / your roles / claimable)"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user's tasks");
            return StatusCode(500, ApiResponseDto<List<TaskSummaryDto>>.ErrorResult(
                "An error occurred while retrieving your tasks"));
        }
    }

    /// <summary>
    /// Get a specific workflow task by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequiresPermission("workflow.view_tasks")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> GetTask(int id)
    {
        try
        {
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.Id == id)
                .Select(t => new WorkflowTaskDto
                {
                    Id = t.Id,
                    WorkflowInstanceId = t.WorkflowInstanceId,
                    NodeId = t.NodeId,
                    TaskName = t.TaskName,
                    Status = t.Status,
                    AssignedToUserId = t.AssignedToUserId,
                    AssignedToRole = t.AssignedToRole,
                    DueDate = t.DueDate,
                    Data = t.Data,
                    ClaimedAt = t.ClaimedAt,
                    CompletedAt = t.CompletedAt,
                    CompletionData = t.CompletionData,
                    ErrorMessage = t.ErrorMessage,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(task, "Workflow task retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow task {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult(
                "An error occurred while retrieving the workflow task"));
        }
    }

    /// <summary>
    /// Claim a workflow task (assign it to yourself)
    /// </summary>
    [HttpPost("{id}/claim")]
    [RequiresPermission("workflow.claim_tasks")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> ClaimTask(
        int id,
        [FromBody] ClaimTaskRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            if (task.Status != WorkflowTaskStatus.Created && task.Status != WorkflowTaskStatus.Assigned)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot claim task in {task.Status} status"));

            if (task.AssignedToUserId.HasValue && task.AssignedToUserId.Value != currentUserId)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is already assigned to another user"));

            task.Status = WorkflowTaskStatus.Claimed;
            task.AssignedToUserId = currentUserId;
            task.ClaimedAt = DateTime.UtcNow;

            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Claimed",
                Data = $"{{\"taskId\": {task.Id}, \"claimedBy\": {currentUserId}, \"notes\": \"{request.ClaimNotes ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task claimed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error claiming workflow task {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while claiming the workflow task"));
        }
    }

    /// <summary>
    /// Complete a workflow task
    /// </summary>
    [HttpPost("{id}/complete")]
    [RequiresPermission("workflow.complete_tasks")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> CompleteTask(
        int id,
        [FromBody] CompleteTaskRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            if (task.Status != WorkflowTaskStatus.Claimed && task.Status != WorkflowTaskStatus.InProgress)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot complete task in {task.Status} status"));

            if (task.AssignedToUserId != currentUserId)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("You can only complete tasks assigned to you"));

            task.Status = WorkflowTaskStatus.Completed;
            task.CompletedAt = DateTime.UtcNow;
            task.CompletionData = request.CompletionData ?? "{}";

            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Completed",
                Data = $"{{\"taskId\": {task.Id}, \"completedBy\": {currentUserId}, \"completionData\": {task.CompletionData}, \"notes\": \"{request.CompletionNotes ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            try
            {
                await _execution.AdvanceAfterTaskCompletionAsync(task);
            }
            catch (Exception execEx)
            {
                _logger.LogError(execEx, "Advance failed after completing task {TaskId}", task.Id);
            }

            // THEN map & return response (task now completed; new tasks may exist)
            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing workflow task {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while completing the workflow task"));
        }
    }

    /// <summary>
    /// Assign a task to a specific user (admin operation)
    /// </summary>
    [HttpPost("{id}/assign")]
    [RequiresPermission("workflow.admin")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> AssignTask(
        int id,
        [FromBody] AssignTaskRequestDto request)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            if (task.Status == WorkflowTaskStatus.Completed || task.Status == WorkflowTaskStatus.Cancelled)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot assign task in {task.Status} status"));

            task.AssignedToUserId = request.AssignedToUserId;
            task.AssignedToRole = request.AssignedToRole;
            task.Status = WorkflowTaskStatus.Assigned;

            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Assigned",
                Data =
                    $"{{\"taskId\": {task.Id}, \"assignedBy\": {currentUserId}, \"assignedToUserId\": {(request.AssignedToUserId?.ToString() ?? "null")}, \"assignedToRole\": \"{request.AssignedToRole ?? ""}\", \"notes\": \"{request.AssignmentNotes ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning workflow task {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while assigning the workflow task"));
        }
    }

    /// <summary>
    /// Cancel a workflow task (admin operation)
    /// </summary>
    [HttpPost("{id}/cancel")]
    [RequiresPermission("workflow.admin")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> CancelTask(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .FirstOrDefaultAsync(t => t.Id == id);

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            if (task.Status == WorkflowTaskStatus.Completed || task.Status == WorkflowTaskStatus.Cancelled)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot cancel task in {task.Status} status"));

            task.Status = WorkflowTaskStatus.Cancelled;
            task.CompletedAt = DateTime.UtcNow;

            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Cancelled",
                Data = $"{{\"taskId\": {task.Id}, \"cancelledBy\": {currentUserId}}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task cancelled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling workflow task {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while cancelling the workflow task"));
        }
    }

    private WorkflowTaskDto MapTask(WorkflowTask t) => new()
    {
        Id = t.Id,
        WorkflowInstanceId = t.WorkflowInstanceId,
        NodeId = t.NodeId,
        TaskName = t.TaskName,
        Status = t.Status,
        AssignedToUserId = t.AssignedToUserId,
        AssignedToRole = t.AssignedToRole,
        DueDate = t.DueDate,
        Data = t.Data,
        ClaimedAt = t.ClaimedAt,
        CompletedAt = t.CompletedAt,
        CompletionData = t.CompletionData,
        ErrorMessage = t.ErrorMessage,
        CreatedAt = t.CreatedAt,
        UpdatedAt = t.UpdatedAt
    };

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }

    private HashSet<string> GetCurrentUserRoles()
    {
        // Gather roles from standard & custom claims
        var roles = User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
            .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        return roles;
    }
}
