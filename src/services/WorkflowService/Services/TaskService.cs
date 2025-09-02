using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Services;

public class TaskService : ITaskService
{
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<TaskService> _logger;
    private readonly IUserContext _userContext;
    private readonly ITaskNotificationDispatcher _taskNotifier;

    public TaskService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IWorkflowRuntime workflowRuntime,
        IEventPublisher eventPublisher,
        ILogger<TaskService> logger,
        IUserContext userContext,
        ITaskNotificationDispatcher taskNotifier)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _workflowRuntime = workflowRuntime;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _userContext = userContext;
        _taskNotifier = taskNotifier;
    }

    public async Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetMyTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Tenant context required");

            var currentUserId = _userContext.UserId;
            if (!currentUserId.HasValue)
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("User context required");

            var roles = (_userContext.Roles ?? Array.Empty<string>()).ToList();
            bool hasRoles = roles.Count > 0;

            var query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance).ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value)
                .Where(t => t.NodeType == "human")
                .Where(t =>
                    t.AssignedToUserId == currentUserId.Value
                    || (t.AssignedToRole != null && hasRoles && roles.Contains(t.AssignedToRole))
                    || (t.AssignedToUserId == null && t.AssignedToRole == null &&
                        (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned))
                );

            var result = await ApplyTaskFiltersAndPagination(query, request, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting my tasks");
            return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Failed to retrieve tasks");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetAllTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Tenant context required");

            var query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value);

            var result = await ApplyTaskFiltersAndPagination(query, request, cancellationToken);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all tasks");
            return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Failed to retrieve tasks");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> GetTaskByIdAsync(int taskId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                                          t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");

            var dto = _mapper.Map<WorkflowTaskDto>(task);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to retrieve task");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> ClaimTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var currentUserId = _userContext.UserId;
            if (!currentUserId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");

            var roles = (_userContext.Roles ?? Array.Empty<string>()).ToList();

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                                          t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");

            if (task.NodeType != "human")
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Non-human tasks cannot be claimed");

            if (task.Status != WorkflowTaskStatus.Created && task.Status != WorkflowTaskStatus.Assigned)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task cannot be claimed in its current state");

            if (task.AssignedToUserId.HasValue && task.AssignedToUserId.Value != currentUserId.Value)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is assigned to another user");

            if (!string.IsNullOrEmpty(task.AssignedToRole) &&
                !roles.Contains(task.AssignedToRole, StringComparer.OrdinalIgnoreCase))
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("You lack the required role");

            task.Status = WorkflowTaskStatus.Claimed;
            task.AssignedToUserId = currentUserId.Value;
            task.ClaimedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            await _eventPublisher.PublishTaskAssignedAsync(task, currentUserId.Value, cancellationToken);

            // Notify user (their counts changed)
            await SafeNotifyUser(tenantId.Value, currentUserId.Value, cancellationToken);

            var dto = _mapper.Map<WorkflowTaskDto>(task);
            _logger.LogInformation("User {UserId} claimed task {TaskId}", currentUserId.Value, taskId);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto, "Task claimed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error claiming task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to claim task");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> CompleteTaskAsync(int taskId, CompleteTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var currentUserId = _userContext.UserId;
            if (!currentUserId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");

            await _workflowRuntime.CompleteTaskAsync(taskId, request.CompletionData ?? "{}", currentUserId.Value, cancellationToken, autoCommit: false);

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");

            // Notify user + tenant (completion affects counts & available tasks downstream)
            await SafeNotifyUser(task.TenantId, currentUserId.Value, cancellationToken);
            await SafeNotifyTenant(task.TenantId, cancellationToken);

            var dto = _mapper.Map<WorkflowTaskDto>(task);
            _logger.LogInformation("User {UserId} completed task {TaskId}", currentUserId.Value, taskId);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto, "Task completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to complete task");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> ReleaseTaskAsync(int taskId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var currentUserId = _userContext.UserId;
            if (!currentUserId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                                          t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");

            if (task.Status != WorkflowTaskStatus.Claimed && task.Status != WorkflowTaskStatus.InProgress)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task cannot be released in its current state");

            if (task.AssignedToUserId != currentUserId.Value)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is not assigned to you");

            task.Status = !string.IsNullOrEmpty(task.AssignedToRole) ? WorkflowTaskStatus.Assigned : WorkflowTaskStatus.Created;
            task.AssignedToUserId = null;
            task.ClaimedAt = null;
            task.UpdatedAt = DateTime.UtcNow;

            var dto = _mapper.Map<WorkflowTaskDto>(task);

            // User loses a claimed task; others may now see it as available.
            await SafeNotifyUser(tenantId.Value, currentUserId.Value, cancellationToken);
            await SafeNotifyTenant(tenantId.Value, cancellationToken);

            _logger.LogInformation("User {UserId} released task {TaskId}", currentUserId.Value, taskId);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto, "Task released successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error releasing task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to release task");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> ReassignTaskAsync(int taskId, ReassignTaskRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                                          t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");

            if (task.Status == WorkflowTaskStatus.Completed || task.Status == WorkflowTaskStatus.Cancelled)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Cannot reassign completed or cancelled task");

            var previousUser = task.AssignedToUserId;

            task.AssignedToUserId = request.AssignToUserId;
            task.AssignedToRole = request.AssignToRole;
            task.Status = request.AssignToUserId.HasValue || !string.IsNullOrEmpty(request.AssignToRole)
                ? WorkflowTaskStatus.Assigned : WorkflowTaskStatus.Created;
            task.ClaimedAt = null;
            task.UpdatedAt = DateTime.UtcNow;

            if (request.AssignToUserId.HasValue)
                await _eventPublisher.PublishTaskAssignedAsync(task, request.AssignToUserId.Value, cancellationToken);

            // Notify new assignee if user; notify old user (count drop); always tenant
            if (previousUser.HasValue)
                await SafeNotifyUser(tenantId.Value, previousUser.Value, cancellationToken);
            if (request.AssignToUserId.HasValue)
                await SafeNotifyUser(tenantId.Value, request.AssignToUserId.Value, cancellationToken);
            await SafeNotifyTenant(tenantId.Value, cancellationToken);

            var dto = _mapper.Map<WorkflowTaskDto>(task);
            _logger.LogInformation("Task {TaskId} reassigned: {Reason}", taskId, request.Reason);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto, "Task reassigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reassigning task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to reassign task");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> AssignTaskAsync(int taskId, AssignTaskRequestDto request, int performedByUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            if (task.Status is WorkflowTaskStatus.Completed or WorkflowTaskStatus.Cancelled)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot assign task in {task.Status} status");

            var previousUser = task.AssignedToUserId;

            task.AssignedToUserId = request.AssignedToUserId;
            task.AssignedToRole = request.AssignedToRole;
            task.Status = WorkflowTaskStatus.Assigned;
            task.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowEvents.Add(new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                TenantId = task.TenantId,
                Type = "Task",
                Name = "Assigned",
                Data = $"{{\"taskId\":{task.Id},\"assignedBy\":{performedByUserId},\"assignedToUserId\":{(request.AssignedToUserId?.ToString() ?? "null")},\"assignedToRole\":\"{request.AssignedToRole ?? ""}\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = performedByUserId
            });

            if (previousUser.HasValue && previousUser.Value != request.AssignedToUserId)
                await SafeNotifyUser(tenantId.Value, previousUser.Value, cancellationToken);
            if (request.AssignedToUserId.HasValue)
                await SafeNotifyUser(tenantId.Value, request.AssignedToUserId.Value, cancellationToken);
            await SafeNotifyTenant(tenantId.Value, cancellationToken);

            var dto = _mapper.Map<WorkflowTaskDto>(task);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto, "Task assigned successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to assign task");
        }
    }

    public async Task<ApiResponseDto<WorkflowTaskDto>> CancelTaskAsync(int taskId, int performedByUserId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            if (task.Status is WorkflowTaskStatus.Completed or WorkflowTaskStatus.Cancelled)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult($"Cannot cancel task in {task.Status} status");

            var previousUser = task.AssignedToUserId;

            task.Status = WorkflowTaskStatus.Cancelled;
            task.CompletedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            _context.WorkflowEvents.Add(new WorkflowEvent
            {
                WorkflowInstanceId = task.WorkflowInstanceId,
                TenantId = task.TenantId,
                Type = "Task",
                Name = "Cancelled",
                Data = $"{{\"taskId\":{task.Id},\"cancelledBy\":{performedByUserId}}}",
                OccurredAt = DateTime.UtcNow,
                UserId = performedByUserId
            });

            if (previousUser.HasValue)
                await SafeNotifyUser(tenantId.Value, previousUser.Value, cancellationToken);
            await SafeNotifyTenant(tenantId.Value, cancellationToken);

            var dto = _mapper.Map<WorkflowTaskDto>(task);
            return ApiResponseDto<WorkflowTaskDto>.SuccessResult(dto, "Task cancelled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling task {TaskId}", taskId);
            return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Failed to cancel task");
        }
    }

    public async Task<ApiResponseDto<TaskStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<TaskStatisticsDto>.ErrorResult("Tenant context required");

            var tasks = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value)
                .ToListAsync(cancellationToken);

            var completedTasks = tasks.Where(t => t.Status == WorkflowTaskStatus.Completed).ToList();
            var averageCompletionTime = completedTasks.Any()
                ? completedTasks.Where(t => t.CompletedAt.HasValue && t.CreatedAt != default)
                    .Average(t => (t.CompletedAt!.Value - t.CreatedAt).TotalHours)
                : 0;

            var statistics = new TaskStatisticsDto
            {
                TotalTasks = tasks.Count,
                PendingTasks = tasks.Count(t => t.Status == WorkflowTaskStatus.Created),
                InProgressTasks = tasks.Count(t => t.Status == WorkflowTaskStatus.Claimed || t.Status == WorkflowTaskStatus.InProgress),
                CompletedTasks = tasks.Count(t => t.Status == WorkflowTaskStatus.Completed),
                OverdueTasks = tasks.Count(t => t.DueDate.HasValue && t.DueDate.Value < DateTime.UtcNow &&
                                                t.Status != WorkflowTaskStatus.Completed && t.Status != WorkflowTaskStatus.Cancelled),
                TasksByType = tasks.GroupBy(t => t.NodeId).ToDictionary(g => g.Key, g => g.Count()),
                TasksByStatus = tasks.GroupBy(t => t.Status).ToDictionary(g => g.Key, g => g.Count()),
                AverageCompletionTime = averageCompletionTime,
                LastUpdated = DateTime.UtcNow
            };

            return ApiResponseDto<TaskStatisticsDto>.SuccessResult(statistics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task statistics");
            return ApiResponseDto<TaskStatisticsDto>.ErrorResult("Failed to retrieve task statistics");
        }
    }

    private async Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> ApplyTaskFiltersAndPagination(
        IQueryable<WorkflowTask> query,
        GetTasksRequestDto request,
        CancellationToken cancellationToken)
    {
        if (request.Status.HasValue)
            query = query.Where(t => t.Status == request.Status.Value);

        if (request.WorkflowDefinitionId.HasValue)
            query = query.Where(t => t.WorkflowInstance.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);

        if (request.AssignedToUserId.HasValue)
            query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);

        if (!string.IsNullOrEmpty(request.AssignedToRole))
            query = query.Where(t => t.AssignedToRole == request.AssignedToRole);

        if (request.DueBefore.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value <= request.DueBefore.Value);

        if (request.DueAfter.HasValue)
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value >= request.DueAfter.Value);

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            var term = request.SearchTerm;
            query = query.Where(t =>
                t.TaskName.Contains(term) ||
                t.WorkflowInstance.WorkflowDefinition.Name.Contains(term));
        }

        query = request.SortBy.ToLower() switch
        {
            "taskname" => request.SortDescending ? query.OrderByDescending(t => t.TaskName) : query.OrderBy(t => t.TaskName),
            "status" => request.SortDescending ? query.OrderByDescending(t => t.Status) : query.OrderBy(t => t.Status),
            "duedate" => request.SortDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
            "completedat" => request.SortDescending ? query.OrderByDescending(t => t.CompletedAt) : query.OrderBy(t => t.CompletedAt),
            _ => request.SortDescending ? query.OrderByDescending(t => t.CreatedAt) : query.OrderBy(t => t.CreatedAt)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var tasks = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(t => new TaskSummaryDto
            {
                Id = t.Id,
                TaskName = t.TaskName,
                Status = t.Status,
                WorkflowDefinitionName = t.WorkflowInstance.WorkflowDefinition.Name,
                WorkflowInstanceId = t.WorkflowInstanceId,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                NodeId = t.NodeId,
                NodeType = t.NodeType
            })
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResultDto<TaskSummaryDto>
        {
            Items = tasks,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
        };

        return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.SuccessResult(pagedResult);
    }

    // Notification helpers – swallow errors to avoid impacting main path
    private async Task SafeNotifyUser(int tenantId, int userId, CancellationToken ct)
    {
        try { await _taskNotifier.NotifyUserAsync(tenantId, userId, ct); } catch { }
    }
    private async Task SafeNotifyTenant(int tenantId, CancellationToken ct)
    {
        try { await _taskNotifier.NotifyTenantAsync(tenantId, ct); } catch { }
    }

    public async Task<ApiResponseDto<List<TaskSummaryDto>>> GetMyTasksListAsync(
        DTOs.Workflow.Enums.TaskStatus? status,
        bool includeRoleTasks,
        bool includeUnassigned,
        CancellationToken cancellationToken = default)
    {
        var request = new GetTasksRequestDto
        {
            Status = status,
            Page = 1,
            PageSize = 500
        };

        var paged = await GetMyTasksAsync(request, cancellationToken);
        if (!paged.Success || paged.Data == null)
            return ApiResponseDto<List<TaskSummaryDto>>.ErrorResult(paged.Message, paged.Errors);

        var items = paged.Data.Items.AsEnumerable();
        var list = items.ToList();
        return ApiResponseDto<List<TaskSummaryDto>>.SuccessResult(list, $"Retrieved {list.Count} tasks");
    }

    public async Task<ApiResponseDto<TaskCountsDto>> GetMyTaskCountsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<TaskCountsDto>.ErrorResult("Tenant context required");

            var userId = _userContext.UserId;
            if (!userId.HasValue)
                return ApiResponseDto<TaskCountsDto>.ErrorResult("User context required");

            var roles = (_userContext.Roles ?? Array.Empty<string>()).ToArray();
            var nowUtc = DateTime.UtcNow.Date;

            var rows = await _context.WorkflowTasks
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value && t.NodeType == "human")
                .Select(t => new
                {
                    t.Status,
                    t.AssignedToUserId,
                    t.AssignedToRole,
                    t.DueDate,
                    t.CompletedAt,
                    t.TenantId
                })
                .ToListAsync(cancellationToken);

            var counts = new TaskCountsDto
            {
                Available = rows.Count(t =>
                    t.AssignedToUserId == null &&
                    t.AssignedToRole == null &&
                    (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned)),

                AssignedToMe = rows.Count(t =>
                    t.AssignedToUserId == userId.Value &&
                    (t.Status == WorkflowTaskStatus.Assigned || t.Status == WorkflowTaskStatus.Created)),

                AssignedToMyRoles = rows.Count(t =>
                    t.AssignedToUserId == null &&
                    t.AssignedToRole != null &&
                    roles.Contains(t.AssignedToRole, StringComparer.OrdinalIgnoreCase) &&
                    t.Status == WorkflowTaskStatus.Assigned),

                Claimed = rows.Count(t => t.AssignedToUserId == userId.Value && t.Status == WorkflowTaskStatus.Claimed),
                InProgress = rows.Count(t => t.AssignedToUserId == userId.Value && t.Status == WorkflowTaskStatus.InProgress),
                CompletedToday = rows.Count(t =>
                    t.CompletedAt.HasValue &&
                    t.CompletedAt.Value >= nowUtc && t.CompletedAt.Value < nowUtc.AddDays(1)),
                Overdue = rows.Count(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value < DateTime.UtcNow &&
                    t.Status != WorkflowTaskStatus.Completed &&
                    t.Status != WorkflowTaskStatus.Cancelled),
                Failed = rows.Count(t => t.Status == WorkflowTaskStatus.Failed)
            };

            return ApiResponseDto<TaskCountsDto>.SuccessResult(counts);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting task counts");
            return ApiResponseDto<TaskCountsDto>.ErrorResult("Failed to retrieve task counts");
        }
    }
}
