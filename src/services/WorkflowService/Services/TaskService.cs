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
using System.Linq;
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
    private readonly ITaskNotificationDispatcher _notificationDispatcher;

    public TaskService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IWorkflowRuntime workflowRuntime,
        IEventPublisher eventPublisher,
        ILogger<TaskService> logger,
        IUserContext userContext,
        ITaskNotificationDispatcher notificationDispatcher)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _workflowRuntime = workflowRuntime;
        _eventPublisher = eventPublisher;
        _logger = logger;
        _userContext = userContext;
        _notificationDispatcher = notificationDispatcher;
    }

    public async Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetMyTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Tenant context required");

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("User context required");

            // Capture roles in a simple List<string> so EF can translate roles.Contains(column)
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

            var currentUserId = GetCurrentUserId();
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

            await _context.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishTaskAssignedAsync(task, currentUserId.Value, cancellationToken);
            await _notificationDispatcher.NotifyUserAsync(tenantId.Value, currentUserId.Value, cancellationToken);

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
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId &&
                                          t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");

            if (task.NodeType != "human")
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Non-human tasks cannot be completed manually");

            if (task.Status != WorkflowTaskStatus.Claimed && task.Status != WorkflowTaskStatus.InProgress)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task cannot be completed in its current state");

            if (task.AssignedToUserId != currentUserId.Value)
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is not assigned to you");

            await _workflowRuntime.CompleteTaskAsync(taskId, request.CompletionData ?? "{}", currentUserId.Value, cancellationToken);
            await _context.Entry(task).ReloadAsync(cancellationToken);
            await _notificationDispatcher.NotifyUserAsync(tenantId.Value, currentUserId.Value, cancellationToken);

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

            var currentUserId = GetCurrentUserId();
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

            await _context.SaveChangesAsync(cancellationToken);
            await _notificationDispatcher.NotifyUserAsync(tenantId.Value, currentUserId.Value, cancellationToken);

            var dto = _mapper.Map<WorkflowTaskDto>(task);
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

            task.AssignedToUserId = request.AssignToUserId;
            task.AssignedToRole = request.AssignToRole;
            task.Status = request.AssignToUserId.HasValue || !string.IsNullOrEmpty(request.AssignToRole)
                ? WorkflowTaskStatus.Assigned : WorkflowTaskStatus.Created;
            task.ClaimedAt = null;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            if (request.AssignToUserId.HasValue)
            {
                await _eventPublisher.PublishTaskAssignedAsync(task, request.AssignToUserId.Value, cancellationToken);
                await _notificationDispatcher.NotifyUserAsync(tenantId.Value, request.AssignToUserId.Value, cancellationToken);
            }
            else
            {
                await _notificationDispatcher.NotifyTenantAsync(tenantId.Value, cancellationToken);
            }

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

    private int? GetCurrentUserId() => _userContext.UserId;

    // IMPLEMENTATION of interface method
    public async Task<ApiResponseDto<List<TaskSummaryDto>>> GetMyTasksListAsync(
        DTOs.Workflow.Enums.TaskStatus? status,
        bool includeRoleTasks,
        bool includeUnassigned,
        CancellationToken cancellationToken = default)
    {
        // Build a request with a sufficiently large page size (tune if needed)
        var request = new GetTasksRequestDto
        {
            Status = status,
            Page = 1,
            PageSize = 500 // adjust or make configurable
        };

        var paged = await GetMyTasksAsync(request, cancellationToken);
        if (!paged.Success || paged.Data == null)
            return ApiResponseDto<List<TaskSummaryDto>>.ErrorResult(paged.Message, paged.Errors);

        var items = paged.Data.Items.AsEnumerable();

        // Apply includeRoleTasks/includeUnassigned flags AFTER base filtering
        if (!includeRoleTasks)
        {
            items = items.Where(t => t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned || t.Status == WorkflowTaskStatus.Claimed
                                     ? true
                                     : true); // (role-based tasks already filtered in base query by NodeType/user/role logic;
                                              // leave as placeholder in case future expansion marks role-only tasks separately)
        }

        if (!includeUnassigned)
        {
            items = items.Where(t => !(t.Status == WorkflowTaskStatus.Created && t.TaskName != null)); 
            // NOTE: Adjust this if you later add an explicit AssignedTo... field to TaskSummaryDto.
        }

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

            var userId = GetCurrentUserId();
            if (!userId.HasValue)
                return ApiResponseDto<TaskCountsDto>.ErrorResult("User context required");

            var roles = (_userContext.Roles ?? Array.Empty<string>()).ToArray();
            var nowUtc = DateTime.UtcNow.Date;

            // Base query: human tasks in tenant
            var q = _context.WorkflowTasks
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value && t.NodeType == "human");

            // Materialize minimal columns
            var rows = await q.Select(t => new
            {
                t.Status,
                t.AssignedToUserId,
                t.AssignedToRole,
                t.DueDate,
                t.CompletedAt
            }).ToListAsync(cancellationToken);

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
