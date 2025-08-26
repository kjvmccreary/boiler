using DTOs.Common;
using DTOs.Workflow;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Service for managing workflow tasks
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Get tasks assigned to current user
    /// </summary>
    Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetMyTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all tasks for current tenant (admin only)
    /// </summary>
    Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetAllTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get task details by ID
    /// </summary>
    Task<ApiResponseDto<WorkflowTaskDto>> GetTaskByIdAsync(int taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Claim a task for current user
    /// </summary>
    Task<ApiResponseDto<WorkflowTaskDto>> ClaimTaskAsync(int taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Complete a task
    /// </summary>
    Task<ApiResponseDto<WorkflowTaskDto>> CompleteTaskAsync(int taskId, CompleteTaskRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Release a claimed task
    /// </summary>
    Task<ApiResponseDto<WorkflowTaskDto>> ReleaseTaskAsync(int taskId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reassign a task to another user
    /// </summary>
    Task<ApiResponseDto<WorkflowTaskDto>> ReassignTaskAsync(int taskId, ReassignTaskRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get task statistics for current tenant
    /// </summary>
    Task<ApiResponseDto<TaskStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default);
}
