using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Contracts.Services;

namespace WorkflowService.Services;

public class DefinitionService : IDefinitionService
{
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<DefinitionService> _logger;

    public DefinitionService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IEventPublisher eventPublisher,
        ILogger<DefinitionService> logger)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    private bool ApplyGatewayBackfill(WorkflowDefinition definition, bool persistIfChanged)
    {
        var original = definition.JSONDefinition;
        var backfilled = original.EnrichEdgesForGateway();
        if (backfilled != original)
        {
            definition.JSONDefinition = backfilled;
            if (persistIfChanged)
                definition.UpdatedAt = DateTime.UtcNow;
            return true;
        }
        return false;
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> CreateDraftAsync(CreateWorkflowDefinitionDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

            var validationResult = await ValidateDefinitionInternalAsync(request.JSONDefinition);
            if (!validationResult.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Invalid workflow definition: {string.Join(", ", validationResult.Errors)}");

            var definition = new WorkflowDefinition
            {
                TenantId = tenantId.Value,
                Name = request.Name,
                Description = request.Description,
                JSONDefinition = request.JSONDefinition.EnrichEdgesForGateway(),
                Version = 1,
                IsPublished = false,
                Tags = request.Tags,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.WorkflowDefinitions.Add(definition);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowDefinitionDto>(definition);
            _logger.LogInformation("Created draft workflow definition {DefinitionId} for tenant {TenantId}", definition.Id, tenantId);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Draft workflow definition created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating draft workflow definition");
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to create workflow definition");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> UpdateDraftAsync(int definitionId, UpdateWorkflowDefinitionDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == definitionId && d.TenantId == tenantId.Value, cancellationToken);

            if (definition == null)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found");
            if (definition.IsPublished)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Cannot update published workflow definition");

            if (!string.IsNullOrEmpty(request.Name))
                definition.Name = request.Name;
            if (request.Description != null)
                definition.Description = request.Description;
            if (request.Tags != null)
                definition.Tags = request.Tags;

            if (!string.IsNullOrEmpty(request.JSONDefinition))
            {
                var validationResult = await ValidateDefinitionInternalAsync(request.JSONDefinition);
                if (!validationResult.IsValid)
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Invalid workflow definition: {string.Join(", ", validationResult.Errors)}");
                definition.JSONDefinition = request.JSONDefinition.EnrichEdgesForGateway();
            }
            else
            {
                ApplyGatewayBackfill(definition, persistIfChanged: false);
            }

            definition.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowDefinitionDto>(definition);
            _logger.LogInformation("Updated draft workflow definition {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Draft workflow definition updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating draft workflow definition {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to update workflow definition");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> PublishAsync(int definitionId, PublishDefinitionRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == definitionId && d.TenantId == tenantId.Value, cancellationToken);

            if (definition == null)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found");
            if (definition.IsPublished && !request.ForcePublish)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition is already published");

            var validationResult = await ValidateDefinitionInternalAsync(definition.JSONDefinition);
            if (!validationResult.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Cannot publish invalid workflow definition: {string.Join(", ", validationResult.Errors)}");

            definition.IsPublished = true;
            definition.PublishedAt = DateTime.UtcNow;
            definition.PublishNotes = request.PublishNotes;
            definition.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishDefinitionPublishedAsync(definition, cancellationToken);

            var dto = _mapper.Map<WorkflowDefinitionDto>(definition);
            _logger.LogInformation("Published workflow definition {DefinitionId} version {Version}", definitionId, definition.Version);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Workflow definition published successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing workflow definition {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to publish workflow definition");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> GetByIdAsync(int definitionId, int? version = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

            var query = _context.WorkflowDefinitions.Where(d => d.Id == definitionId && d.TenantId == tenantId.Value);
            if (version.HasValue) query = query.Where(d => d.Version == version.Value);

            var definition = await query.FirstOrDefaultAsync(cancellationToken);
            if (definition == null)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found");

            definition.JSONDefinition = definition.JSONDefinition.EnrichEdgesForGateway();
            var dto = _mapper.Map<WorkflowDefinitionDto>(definition);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow definition {DefinitionId} version {Version}", definitionId, version);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to retrieve workflow definition");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>> GetAllAsync(GetWorkflowDefinitionsRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.ErrorResult("Tenant context required");

            var query = _context.WorkflowDefinitions.Where(d => d.TenantId == tenantId.Value);

            if (!string.IsNullOrEmpty(request.SearchTerm))
                query = query.Where(d => d.Name.Contains(request.SearchTerm) ||
                                         (d.Description != null && d.Description.Contains(request.SearchTerm)));
            if (request.IsPublished.HasValue)
                query = query.Where(d => d.IsPublished == request.IsPublished.Value);
            if (!string.IsNullOrEmpty(request.Tags))
                query = query.Where(d => d.Tags != null && d.Tags.Contains(request.Tags));

            query = request.SortBy.ToLower() switch
            {
                "name" => request.SortDescending ? query.OrderByDescending(d => d.Name) : query.OrderBy(d => d.Name),
                "version" => request.SortDescending ? query.OrderByDescending(d => d.Version) : query.OrderBy(d => d.Version),
                "publishedat" => request.SortDescending ? query.OrderByDescending(d => d.PublishedAt) : query.OrderBy(d => d.PublishedAt),
                _ => request.SortDescending ? query.OrderByDescending(d => d.CreatedAt) : query.OrderBy(d => d.CreatedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            var definitions = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            foreach (var def in definitions)
                ApplyGatewayBackfill(def, persistIfChanged: false);

            var dtos = _mapper.Map<List<WorkflowDefinitionDto>>(definitions);

            var paged = new PagedResultDto<WorkflowDefinitionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };
            return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.SuccessResult(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow definitions");
            return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.ErrorResult("Failed to retrieve workflow definitions");
        }
    }

    public async Task<ApiResponseDto<ValidationResultDto>> ValidateDefinitionAsync(ValidateDefinitionRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await ValidateDefinitionInternalAsync(request.JSONDefinition);
            return ApiResponseDto<ValidationResultDto>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating workflow definition");
            return ApiResponseDto<ValidationResultDto>.ErrorResult("Failed to validate workflow definition");
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteDraftAsync(int definitionId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<bool>.ErrorResult("Tenant context required");

            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == definitionId && d.TenantId == tenantId.Value, cancellationToken);
            if (definition == null)
                return ApiResponseDto<bool>.ErrorResult("Workflow definition not found");
            if (definition.IsPublished)
                return ApiResponseDto<bool>.ErrorResult("Cannot delete published workflow definition");

            var hasInstances = await _context.WorkflowInstances
                .AnyAsync(i => i.WorkflowDefinitionId == definitionId, cancellationToken);
            if (hasInstances)
                return ApiResponseDto<bool>.ErrorResult("Cannot delete definition with existing instances");

            _context.WorkflowDefinitions.Remove(definition);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Deleted draft workflow definition {DefinitionId}", definitionId);
            return ApiResponseDto<bool>.SuccessResult(true, "Draft workflow definition deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting draft workflow definition {DefinitionId}", definitionId);
            return ApiResponseDto<bool>.ErrorResult("Failed to delete workflow definition");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> CreateNewVersionAsync(int definitionId, CreateNewVersionRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

            var existing = await _context.WorkflowDefinitions
                .Where(d => d.Id == definitionId && d.TenantId == tenantId.Value)
                .OrderByDescending(d => d.Version)
                .FirstOrDefaultAsync(cancellationToken);
            if (existing == null)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Base workflow definition not found");

            var validationResult = await ValidateDefinitionInternalAsync(request.JSONDefinition);
            if (!validationResult.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Invalid workflow definition: {string.Join(", ", validationResult.Errors)}");

            var newDef = new WorkflowDefinition
            {
                TenantId = tenantId.Value,
                Name = request.Name,
                Description = request.Description,
                JSONDefinition = request.JSONDefinition.EnrichEdgesForGateway(),
                Version = existing.Version + 1,
                IsPublished = false,
                Tags = request.Tags,
                VersionNotes = request.VersionNotes,
                ParentDefinitionId = existing.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.WorkflowDefinitions.Add(newDef);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowDefinitionDto>(newDef);
            _logger.LogInformation("Created new version {Version} for definition {DefinitionId}", newDef.Version, definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "New version created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating new version for workflow definition {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to create new version");
        }
    }

    private async Task<ValidationResultDto> ValidateDefinitionInternalAsync(string jsonDefinition)
    {
        try
        {
            var wf = WorkflowDefinitionJson.FromJson(jsonDefinition);
            var v = wf.Validate();
            return new ValidationResultDto
            {
                IsValid = v.IsValid,
                Errors = v.Errors,
                Warnings = v.Warnings,
                Metadata = new Dictionary<string, object>
                {
                    ["nodeCount"] = wf.Nodes.Count,
                    ["edgeCount"] = wf.Edges.Count,
                    ["hasStartNode"] = wf.Nodes.Any(n => n.IsStart()),
                    ["hasEndNode"] = wf.Nodes.Any(n => n.IsEnd()),
                    ["nodeTypes"] = wf.Nodes.GroupBy(n => n.Type).ToDictionary(g => g.Key, g => g.Count())
                }
            };
        }
        catch (Exception ex)
        {
            return new ValidationResultDto
            {
                IsValid = false,
                Errors = new List<string> { $"Invalid JSON: {ex.Message}" }
            };
        }
    }
}
