using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Service for managing workflow instances
/// </summary>
public interface IInstanceService
{
    /// <summary>
    /// Start a new workflow instance
    /// </summary>
    Task<ApiResponseDto<WorkflowInstanceDto>> StartInstanceAsync(StartInstanceRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get workflow instance by ID
    /// </summary>
    Task<ApiResponseDto<WorkflowInstanceDto>> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all workflow instances for current tenant
    /// </summary>
    Task<ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>> GetAllAsync(GetInstancesRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signal a workflow instance with external event
    /// </summary>
    Task<ApiResponseDto<WorkflowInstanceDto>> SignalAsync(int instanceId, SignalInstanceRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Terminate a workflow instance
    /// </summary>
    Task<ApiResponseDto<bool>> TerminateAsync(int instanceId, TerminateInstanceRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get workflow instance history
    /// </summary>
    Task<ApiResponseDto<List<WorkflowEventDto>>> GetHistoryAsync(int instanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get workflow instance status
    /// </summary>
    Task<ApiResponseDto<InstanceStatusDto>> GetStatusAsync(int instanceId, CancellationToken cancellationToken = default);
}
