using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using TaskStatusAlias = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Services.Interfaces;

public interface ITaskService
{
    Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetMyTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<PagedResultDto<TaskSummaryDto>>> GetAllTasksAsync(GetTasksRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> GetTaskByIdAsync(int taskId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> ClaimTaskAsync(int taskId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> CompleteTaskAsync(int taskId, CompleteTaskRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> ReleaseTaskAsync(int taskId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> ReassignTaskAsync(int taskId, ReassignTaskRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<TaskStatisticsDto>> GetStatisticsAsync(CancellationToken cancellationToken = default);

    Task<ApiResponseDto<List<TaskSummaryDto>>> GetMyTasksListAsync(
        TaskStatusAlias? status,
        bool includeRoleTasks,
        bool includeUnassigned,
        CancellationToken cancellationToken = default);

    Task<ApiResponseDto<TaskCountsDto>> GetMyTaskCountsAsync(CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> AssignTaskAsync(int taskId, AssignTaskRequestDto request, int performedByUserId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowTaskDto>> CancelTaskAsync(int taskId, int performedByUserId, CancellationToken cancellationToken = default);

    Task<ApiResponseDto<ActiveTasksCountDto>> GetActiveTasksCountAsync(CancellationToken ct = default);
}
