using DTOs.Common;
using DTOs.Workflow;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Administrative service for workflow management (admin only)
/// </summary>
public interface IAdminService
{
    /// <summary>
    /// Retry a failed workflow instance
    /// </summary>
    Task<ApiResponseDto<WorkflowInstanceDto>> RetryInstanceAsync(int instanceId, RetryInstanceRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Move workflow instance to a specific node
    /// </summary>
    Task<ApiResponseDto<WorkflowInstanceDto>> MoveToNodeAsync(int instanceId, MoveToNodeRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Force complete a workflow instance
    /// </summary>
    Task<ApiResponseDto<WorkflowInstanceDto>> ForceCompleteAsync(int instanceId, ForceCompleteRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get workflow analytics for tenant
    /// </summary>
    Task<ApiResponseDto<WorkflowAnalyticsDto>> GetAnalyticsAsync(GetAnalyticsRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get system health status for workflow engine
    /// </summary>
    Task<ApiResponseDto<WorkflowSystemHealthDto>> GetSystemHealthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Bulk operations on instances
    /// </summary>
    Task<ApiResponseDto<BulkOperationResultDto>> BulkOperationAsync(BulkInstanceOperationRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get audit trail for workflow operations
    /// </summary>
    Task<ApiResponseDto<PagedResultDto<WorkflowAuditEntryDto>>> GetAuditTrailAsync(GetAuditTrailRequestDto request, CancellationToken cancellationToken = default);
}
