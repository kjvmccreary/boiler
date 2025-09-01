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
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus; // 🔧 ADD: Alias to resolve ambiguity

namespace WorkflowService.Services;

public class TaskService : ITaskService
{
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<TaskService> _logger;

    public TaskService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IWorkflowRuntime workflowRuntime,
        IEventPublisher eventPublisher,
        ILogger<TaskService> logger)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _workflowRuntime = workflowRuntime;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetMyTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Tenant context required");
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("User context required");
            }

            var query = _context.WorkflowTasks
                .Include(t => t.WorkflowInstance).ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value)
                .Where(t => t.NodeType == "human") // NEW filter
                .Where(t => t.AssignedToUserId == currentUserId.Value
                    || (t.AssignedToRole != null && UserHasRole(t.AssignedToRole)));

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
            {
                return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.ErrorResult("Tenant context required");
            }

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
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");
            }

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && 
                                         t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            }

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
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");
            }

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && 
                                         t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            }

            if (task.Status != WorkflowTaskStatus.Created && task.Status != WorkflowTaskStatus.Assigned)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task cannot be claimed in its current state");
            }

            // Check if user can claim this task
            if (task.AssignedToUserId.HasValue && task.AssignedToUserId.Value != currentUserId.Value)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is assigned to another user");
            }

            if (!string.IsNullOrEmpty(task.AssignedToRole) && !UserHasRole(task.AssignedToRole))
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("You don't have the required role to claim this task");
            }

            // Claim the task
            task.Status = WorkflowTaskStatus.Claimed;
            task.AssignedToUserId = currentUserId.Value;
            task.ClaimedAt = DateTime.UtcNow;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // Publish event
            await _eventPublisher.PublishTaskAssignedAsync(task, currentUserId.Value, cancellationToken);

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
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");
            }

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && 
                                         t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            }

            if (task.Status != WorkflowTaskStatus.Claimed && task.Status != WorkflowTaskStatus.InProgress)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task cannot be completed in its current state");
            }

            if (task.AssignedToUserId != currentUserId.Value)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is not assigned to you");
            }

            // Complete the task through the workflow runtime
            await _workflowRuntime.CompleteTaskAsync(taskId, request.CompletionData ?? "{}", currentUserId.Value, cancellationToken);

            // Reload task to get updated state
            await _context.Entry(task).ReloadAsync(cancellationToken);

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
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");
            }

            var currentUserId = GetCurrentUserId();
            if (!currentUserId.HasValue)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("User context required");
            }

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && 
                                         t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            }

            if (task.Status != WorkflowTaskStatus.Claimed && task.Status != WorkflowTaskStatus.InProgress)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task cannot be released in its current state");
            }

            if (task.AssignedToUserId != currentUserId.Value)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task is not assigned to you");
            }

            // Release the task
            task.Status = !string.IsNullOrEmpty(task.AssignedToRole) ? WorkflowTaskStatus.Assigned : WorkflowTaskStatus.Created;
            task.AssignedToUserId = null;
            task.ClaimedAt = null;
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

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
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Tenant context required");
            }

            var task = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .FirstOrDefaultAsync(t => t.Id == taskId && 
                                         t.WorkflowInstance.TenantId == tenantId.Value, cancellationToken);

            if (task == null)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Task not found");
            }

            if (task.Status == WorkflowTaskStatus.Completed || task.Status == WorkflowTaskStatus.Cancelled)
            {
                return ApiResponseDto<WorkflowTaskDto>.ErrorResult("Cannot reassign completed or cancelled task");
            }

            // Reassign the task
            task.AssignedToUserId = request.AssignToUserId;
            task.AssignedToRole = request.AssignToRole;
            task.Status = request.AssignToUserId.HasValue || !string.IsNullOrEmpty(request.AssignToRole) 
                ? WorkflowTaskStatus.Assigned : WorkflowTaskStatus.Created;
            task.ClaimedAt = null; // Reset claim time
            task.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);

            // Publish event if assigned to specific user
            if (request.AssignToUserId.HasValue)
            {
                await _eventPublisher.PublishTaskAssignedAsync(task, request.AssignToUserId.Value, cancellationToken);
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
            {
                return ApiResponseDto<TaskStatisticsDto>.ErrorResult("Tenant context required");
            }

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
        // Apply filters
        if (request.Status.HasValue)
        {
            query = query.Where(t => t.Status == request.Status.Value);
        }

        if (request.WorkflowDefinitionId.HasValue)
        {
            query = query.Where(t => t.WorkflowInstance.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);
        }

        if (request.AssignedToUserId.HasValue)
        {
            query = query.Where(t => t.AssignedToUserId == request.AssignedToUserId.Value);
        }

        if (!string.IsNullOrEmpty(request.AssignedToRole))
        {
            query = query.Where(t => t.AssignedToRole == request.AssignedToRole);
        }

        if (request.DueBefore.HasValue)
        {
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value <= request.DueBefore.Value);
        }

        if (request.DueAfter.HasValue)
        {
            query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value >= request.DueAfter.Value);
        }

        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(t => t.TaskName.Contains(request.SearchTerm) ||
                                    t.WorkflowInstance.WorkflowDefinition.Name.Contains(request.SearchTerm));
        }

        // Apply sorting
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
            .Select(t => new TaskSummaryDto {
                Id = t.Id,
                TaskName = t.TaskName,
                Status = t.Status,
                WorkflowDefinitionName = t.WorkflowInstance.WorkflowDefinition.Name,
                WorkflowInstanceId = t.WorkflowInstanceId,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                NodeId = t.NodeId,
                NodeType = t.NodeType // NEW
})
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResultDto<TaskSummaryDto>
        {
            Items = tasks,
            TotalCount = totalCount,
            Page = request.Page,
            PageSize = request.PageSize
            //TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
        };

        return ApiResponseDto<PagedResultDto<TaskSummaryDto>>.SuccessResult(pagedResult);
    }

    private int? GetCurrentUserId()
    {
        // This would typically extract from HttpContext or JWT claims
        // For now, return null and let the system handle it
        return null;
    }

    private bool UserHasRole(string roleName)
    {
        // This would check the current user's roles
        // For now, return true as a placeholder
        return true;
    }
}
