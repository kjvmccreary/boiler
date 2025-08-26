using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Service for managing workflow definitions
/// </summary>
public interface IDefinitionService
{
    /// <summary>
    /// Create a new draft workflow definition
    /// </summary>
    Task<ApiResponseDto<WorkflowDefinitionDto>> CreateDraftAsync(CreateWorkflowDefinitionDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing draft workflow definition
    /// </summary>
    Task<ApiResponseDto<WorkflowDefinitionDto>> UpdateDraftAsync(int definitionId, UpdateWorkflowDefinitionDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish a draft workflow definition (makes it immutable)
    /// </summary>
    Task<ApiResponseDto<WorkflowDefinitionDto>> PublishAsync(int definitionId, PublishDefinitionRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get workflow definition by ID and version
    /// </summary>
    Task<ApiResponseDto<WorkflowDefinitionDto>> GetByIdAsync(int definitionId, int? version = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all workflow definitions for current tenant
    /// </summary>
    Task<ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>> GetAllAsync(GetWorkflowDefinitionsRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate workflow definition JSON
    /// </summary>
    Task<ApiResponseDto<ValidationResultDto>> ValidateDefinitionAsync(ValidateDefinitionRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a draft workflow definition
    /// </summary>
    Task<ApiResponseDto<bool>> DeleteDraftAsync(int definitionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a new version of an existing definition
    /// </summary>
    Task<ApiResponseDto<WorkflowDefinitionDto>> CreateNewVersionAsync(int definitionId, CreateNewVersionRequestDto request, CancellationToken cancellationToken = default);
}
