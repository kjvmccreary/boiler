using DTOs.Common;
using DTOs.Workflow;

namespace WorkflowService.Services.Interfaces;

public interface IInstanceService
{
    Task<ApiResponseDto<WorkflowInstanceDto>> StartInstanceAsync(StartInstanceRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowInstanceDto>> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>> GetAllAsync(GetInstancesRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowInstanceDto>> SignalAsync(int instanceId, SignalInstanceRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> TerminateAsync(int instanceId, TerminateInstanceRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<List<WorkflowEventDto>>> GetHistoryAsync(int instanceId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<InstanceStatusDto>> GetStatusAsync(int instanceId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowInstanceDto>> SuspendAsync(int instanceId, string reason, CancellationToken ct = default);
    Task<ApiResponseDto<WorkflowInstanceDto>> ResumeAsync(int instanceId, CancellationToken ct = default);
}
