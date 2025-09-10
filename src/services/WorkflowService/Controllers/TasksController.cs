using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Services.Interfaces;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<TasksController> _logger;
    private readonly ITaskService _taskService;
    private readonly IUnitOfWork _unitOfWork;

    public TasksController(
        WorkflowDbContext context,
        ILogger<TasksController> logger,
        ITaskService taskService,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _logger = logger;
        _taskService = taskService;
        _unitOfWork = unitOfWork;
    }

    [HttpGet]
    [RequiresPermission(Permissions.Workflow.ViewTasks)]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowTaskDto>>>> GetTasks(
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
            var query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .AsQueryable();

            if (status.HasValue) query = query.Where(t => t.Status == status.Value);
            if (workflowInstanceId.HasValue) query = query.Where(t => t.WorkflowInstanceId == workflowInstanceId.Value);
            if (assignedToUserId.HasValue) query = query.Where(t => t.AssignedToUserId == assignedToUserId.Value);
            if (!string.IsNullOrEmpty(assignedToRole)) query = query.Where(t => t.AssignedToRole == assignedToRole);
            if (overdue == true)
            {
                var now = DateTime.UtcNow;
                query = query.Where(t => t.DueDate.HasValue && t.DueDate < now &&
                                         t.Status != WorkflowTaskStatus.Completed &&
                                         t.Status != WorkflowTaskStatus.Cancelled);
            }

            var total = await query.CountAsync();
            var items = await query
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
                    NodeType = t.NodeType,
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

            return Ok(ApiResponseDto<List<WorkflowTaskDto>>.SuccessResult(items,
                $"Retrieved {items.Count} of {total} workflow tasks"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_API_TASKS_GET_ERROR");
            return StatusCode(500, ApiResponseDto<List<WorkflowTaskDto>>.ErrorResult(
                "An error occurred while retrieving workflow tasks"));
        }
    }

    [HttpGet("mine")]
    [RequiresPermission(Permissions.Workflow.ViewTasks)]
    public async Task<ActionResult<ApiResponseDto<List<TaskSummaryDto>>>> GetMyTasks(
        [FromQuery] WorkflowTaskStatus? status = null,
        [FromQuery] bool includeRoleTasks = true,
        [FromQuery] bool includeUnassigned = true,
        CancellationToken cancellationToken = default)
    {
        var response = await _taskService.GetMyTasksListAsync(status, includeRoleTasks, includeUnassigned, cancellationToken);
        if (!response.Success)
            return BadRequest(response);

        _logger.LogInformation("WF_API_MY_TASKS Count={Count}", response.Data!.Count);
        return Ok(response);
    }

    [HttpGet("mine/summary")]
    [RequiresPermission(Permissions.Workflow.ViewTasks)]
    public async Task<ActionResult<ApiResponseDto<TaskCountsDto>>> GetMyTaskSummary(CancellationToken ct)
    {
        var result = await _taskService.GetMyTaskCountsAsync(ct);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [RequiresPermission(Permissions.Workflow.ViewTasks)]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> GetTask(int id)
    {
        var result = await _taskService.GetTaskByIdAsync(id);
        if (!result.Success) return NotFound(result);
        return Ok(result);
    }

    [HttpPost("{id}/claim")]
    [RequiresPermission(Permissions.Workflow.ClaimTasks)]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> ClaimTask(int id)
    {
        var result = await _taskService.ClaimTaskAsync(id);
        if (!result.Success) return BadRequest(result);
        await _unitOfWork.CommitAsync();
        return Ok(result);
    }

    [HttpPost("{id}/complete")]
    [RequiresPermission(Permissions.Workflow.CompleteTasks)]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> CompleteTask(int id, [FromBody] CompleteTaskRequestDto request)
    {
        var result = await _taskService.CompleteTaskAsync(id, request);
        if (!result.Success) return BadRequest(result);
        await _unitOfWork.CommitAsync();
        return Ok(result);
    }

    [HttpPost("{id}/assign")]
    [RequiresPermission(Permissions.Workflow.Admin)]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> AssignTask(int id, [FromBody] AssignTaskRequestDto request)
    {
        var userId = GetCurrentUserId();
        var result = await _taskService.AssignTaskAsync(id, request, userId);
        if (!result.Success) return BadRequest(result);
        await _unitOfWork.CommitAsync();
        return Ok(result);
    }

    [HttpPost("{id}/cancel")]
    [RequiresPermission(Permissions.Workflow.Admin)]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> CancelTask(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _taskService.CancelTaskAsync(id, userId);
        if (!result.Success) return BadRequest(result);
        await _unitOfWork.CommitAsync();
        return Ok(result);
    }

    [HttpPost("{id}/release")]
    [RequiresPermission(Permissions.Workflow.ClaimTasks)]
    public async Task<ActionResult<ApiResponseDto<WorkflowTaskDto>>> ReleaseTask(int id)
    {
        var result = await _taskService.ReleaseTaskAsync(id);
        if (!result.Success) return BadRequest(result);
        await _unitOfWork.CommitAsync();
        return Ok(result);
    }

    [HttpGet("active-counts")]
    [RequiresPermission(Permissions.Workflow.ViewTasks)]
    public async Task<ActionResult<ApiResponseDto<ActiveTasksCountDto>>> GetActiveCounts(CancellationToken ct)
    {
        var result = await _taskService.GetActiveTasksCountAsync(ct);
        if (!result.Success) return BadRequest(result);
        return Ok(result);
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
