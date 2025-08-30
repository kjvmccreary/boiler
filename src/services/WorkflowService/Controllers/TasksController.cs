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
using System.Linq.Expressions; // ✅ Needed for expression projection
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using WorkflowService.Engine.Interfaces;
using Contracts.Services;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<TasksController> _logger;
    private readonly IWorkflowRuntime _runtime;
    private readonly ITenantProvider _tenantProvider;

    // ✅ EF Core-friendly projection (Expression so async LINQ remains IQueryable)
    private static readonly Expression<Func<WorkflowTask, WorkflowTaskDto>> TaskProjection = t => new WorkflowTaskDto
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

    public TasksController(
        WorkflowDbContext context,
        ILogger<TasksController> logger,
        IWorkflowRuntime runtime,
        ITenantProvider tenantProvider)
    {
        _context = context;
        _logger = logger;
        _runtime = runtime;
        _tenantProvider = tenantProvider;
    }

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
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<List<WorkflowTaskDto>>.ErrorResult("Tenant context required"));

            var currentUserId = GetCurrentUserId();
            var userRoles = GetCurrentUserRoles();

            IQueryable<WorkflowTask> query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value);

            if (mine == true)
            {
                query = query.Where(t =>
                    t.AssignedToUserId == currentUserId
                    || (mineIncludeRoles && t.AssignedToRole != null && userRoles.Contains(t.AssignedToRole))
                    || (mineIncludeUnassigned && t.AssignedToUserId == null && t.AssignedToRole == null &&
                        (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned)));
            }

            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (workflowInstanceId.HasValue) query = query.Where(t => t.WorkflowInstanceId == workflowInstanceId.Value);
            if (assignedToUserId.HasValue) query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);
            if (!string.IsNullOrEmpty(assignedToRole)) query = query.Where(t => t.AssignedToRole == assignedToRole);

            if (overdue == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(t => t.DueDate.HasValue && t.DueDate < now &&
                    t.Status != WorkflowTaskStatus.Completed && t.Status != WorkflowTaskStatus.Cancelled);
            }

            var totalCount = await query.CountAsync();

            var tasks = await query
                .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
                .ThenByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(TaskProjection)         // ✅ stays IQueryable → async works
                .ToListAsync();

            _logger.LogInformation("WF_API_TASKS_GET Count={Returned}/{Total}", tasks.Count, totalCount);

            return Ok(ApiResponseDto<List<WorkflowTaskDto>>.SuccessResult(
                tasks, $"Retrieved {tasks.Count} of {totalCount} workflow tasks"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASKS_GET_ERROR");
            return StatusCode(500, ApiResponseDto<List<WorkflowTaskDto>>.ErrorResult(
                "An error occurred while retrieving workflow tasks"));
        }
    }

    [HttpGet("mine")]
    [RequiresPermission("workflow.view_tasks")]
    public async Task<ActionResult<ApiResponseDto<List<TaskSummaryDto>>>> GetMyTasks(
        [FromQuery] WorkflowTaskStatus? status = null,
        [FromQuery] bool includeRoleTasks = true,
        [FromQuery] bool includeUnassigned = true)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<List<TaskSummaryDto>>.ErrorResult("Tenant context required"));

            var currentUserId = GetCurrentUserId();
            var userRoles = GetCurrentUserRoles();

            var query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value)
                .Where(t =>
                    t.AssignedToUserId == currentUserId
                    || (includeRoleTasks && t.AssignedToRole != null && userRoles.Contains(t.AssignedToRole))
                    || (includeUnassigned && t.AssignedToUserId == null && t.AssignedToRole == null &&
                        (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned)));

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            var tasks = await query
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
                    CreatedAt = t.CreatedAt,
                    NodeId = t.NodeId
                })
                .ToListAsync();

            _logger.LogInformation("WF_API_MY_TASKS Count={Count}", tasks.Count);

            return Ok(ApiResponseDto<List<TaskSummaryDto>>.SuccessResult(
                tasks, $"Retrieved {tasks.Count} tasks (you / roles / claimable)"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_MY_TASKS_ERROR");
            return StatusCode(500, ApiResponseDto<List<TaskSummaryDto>>.ErrorResult(
                "An error occurred while retrieving your tasks"));
        }
    }

    [HttpGet("{id}")]
    [RequiresPermission("workflow.view_tasks")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> GetTask(int id)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required"));

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.Id == id && t.WorkflowInstance.TenantId == tenantId.Value)
                .Select(TaskProjection)          // ✅ expression keeps query async-capable
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(task, "Workflow task retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASK_GET_ERROR Id={Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult(
                "An error occurred while retrieving the workflow task"));
        }
    }

    [HttpPost("{id}/claim")]
    [RequiresPermission("workflow.claim_tasks")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> ClaimTask(int id, [FromBody] ClaimTaskRequestDto request)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required"));

            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == id && t.WorkflowInstance.TenantId == tenantId.Value);

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            if (task.Status != WorkflowTaskStatus.Created && task.Status != WorkflowTaskStatus.Assigned)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot claim task in {task.Status} status"));

            if (task.AssignedToUserId.HasValue && task.AssignedToUserId.Value != currentUserId)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is already assigned to another user"));

            task.Status = WorkflowTaskStatus.Claimed;
            task.AssignedToUserId = currentUserId;
            task.ClaimedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowEvents.Add(new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Claimed",
                Data = $"{{\"taskId\": {task.Id}, \"claimedBy\": {currentUserId}, \"notes\": \"{request.ClaimNotes ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            });

            await _context.SaveChangesAsync();

            _logger.LogInformation("WF_API_TASK_CLAIM Task={TaskId} Instance={InstanceId} User={UserId}",
                task.Id, task.WorkflowInstanceId, currentUserId);

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task claimed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASK_CLAIM_ERROR Task={TaskId}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while claiming the workflow task"));
        }
    }

    [HttpPost("{id}/complete")]
    [RequiresPermission("workflow.complete_tasks")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> CompleteTask(int id, [FromBody] CompleteTaskRequestDto request)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required"));

            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(t => t.Id == id && t.WorkflowInstance.TenantId == tenantId.Value);

            if (task == null)
                return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));

            if (task.Status != WorkflowTaskStatus.Claimed && task.Status != WorkflowTaskStatus.InProgress)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot complete task in {task.Status} status"));

            if (task.AssignedToUserId != currentUserId)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("You can only complete tasks assigned to you"));

            await _runtime.CompleteTaskAsync(task.Id, request.CompletionData ?? "{}", currentUserId, CancellationToken.None);

            await _context.Entry(task).ReloadAsync();

            _logger.LogInformation("WF_API_TASK_COMPLETE Task={TaskId} Instance={InstanceId} User={UserId}",
                task.Id, task.WorkflowInstanceId, currentUserId);

            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASK_COMPLETE_ERROR Task={TaskId}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while completing the workflow task"));
        }
    }

    [HttpPost("{id}/assign")]
    [RequiresPermission("workflow.admin")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> AssignTask(int id, [FromBody] AssignTaskRequestDto request)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required"));

            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == id && t.WorkflowInstance.TenantId == tenantId.Value);

            if (task == null) return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));
            if (task.Status is WorkflowTaskStatus.Completed or WorkflowTaskStatus.Cancelled)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot assign task in {task.Status} status"));

            task.AssignedToUserId = request.AssignedToUserId;
            task.AssignedToRole = request.AssignedToRole;
            task.Status = WorkflowTaskStatus.Assigned;
            task.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowEvents.Add(new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Assigned",
                Data = $"{{\"taskId\": {task.Id}, \"assignedBy\": {currentUserId}, \"assignedToUserId\": {(request.AssignedToUserId?.ToString() ?? "null")}, \"assignedToRole\": \"{request.AssignedToRole ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("WF_API_TASK_ASSIGN Task={TaskId} By={UserId}", task.Id, currentUserId);
            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task assigned successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASK_ASSIGN_ERROR Task={TaskId}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while assigning the workflow task"));
        }
    }

    [HttpPost("{id}/cancel")]
    [RequiresPermission("workflow.admin")]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> CancelTask(int id)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required"));

            var currentUserId = GetCurrentUserId();
            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == id && t.WorkflowInstance.TenantId == tenantId.Value);

            if (task == null) return NotFound(ApiResponseDto<WorkflowTaskDto>.ErrorResult("Workflow task not found"));
            if (task.Status is WorkflowTaskStatus.Completed or WorkflowTaskStatus.Cancelled)
                return BadRequest(ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot cancel task in {task.Status} status"));

            task.Status = WorkflowTaskStatus.Cancelled;
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowEvents.Add(new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                Type = "Task",
                Name = "Cancelled",
                Data = $"{{\"taskId\": {task.Id}, \"cancelledBy\": {currentUserId}}}",
                OccurredAt = DateTime.UtcNow,
                UserId = currentUserId
            });

            await _context.SaveChangesAsync();
            _logger.LogInformation("WF_API_TASK_CANCEL Task={TaskId} By={UserId}", task.Id, currentUserId);
            return Ok(ApiResponseDto<WorkflowTaskDto>.SuccessResult(MapTask(task), "Task cancelled successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASK_CANCEL_ERROR Task={TaskId}", id);
            return StatusCode(500, ApiResponseDto<WorkflowTaskDto>.ErrorResult("An error occurred while cancelling the workflow task"));
        }
    }

    // ✅ Helper mappers
    private static WorkflowTaskDto MapTask(WorkflowTask t) => new()
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

    private HashSet<string> GetCurrentUserRoles() =>
        User.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
            .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
}
