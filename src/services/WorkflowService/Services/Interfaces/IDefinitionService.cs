using DTOs.Common;
using DTOs.Workflow;

namespace WorkflowService.Services.Interfaces;

public interface IDefinitionService
{
    Task<ApiResponseDto<WorkflowDefinitionDto>> CreateDraftAsync(CreateWorkflowDefinitionDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowDefinitionDto>> UpdateDraftAsync(int definitionId, UpdateWorkflowDefinitionDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowDefinitionDto>> PublishAsync(int definitionId, PublishDefinitionRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowDefinitionDto>> GetByIdAsync(int definitionId, int? version = null, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>> GetAllAsync(GetWorkflowDefinitionsRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<ValidationResultDto>> ValidateDefinitionAsync(ValidateDefinitionRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<bool>> DeleteDraftAsync(int definitionId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowDefinitionDto>> CreateNewVersionAsync(int definitionId, CreateNewVersionRequestDto request, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowDefinitionInstanceUsageDto>> GetUsageAsync(int definitionId, CancellationToken cancellationToken = default);
    Task<ApiResponseDto<WorkflowDefinitionDto>> UnpublishAsync(int definitionId, UnpublishDefinitionRequestDto request, CancellationToken cancellationToken = default);
}
