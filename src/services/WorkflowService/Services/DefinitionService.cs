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
    private readonly IGraphValidationService _graphValidator;
    private readonly ILogger<DefinitionService> _logger;

    public DefinitionService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IEventPublisher eventPublisher,
        IGraphValidationService graphValidator,
        ILogger<DefinitionService> logger)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _eventPublisher = eventPublisher;
        _graphValidator = graphValidator;
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

            var validation = _graphValidator.Validate(request.JSONDefinition, strict: true);
            if (!validation.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Invalid workflow definition: {string.Join("; ", validation.Errors)}");

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
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Draft workflow definition created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateDraft failed");
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
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Cannot modify a published definition (immutability enforced)");

            if (!string.IsNullOrEmpty(request.JSONDefinition))
            {
                var validation = _graphValidator.Validate(request.JSONDefinition, strict: true);
                if (!validation.IsValid)
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Invalid workflow definition: {string.Join("; ", validation.Errors)}");
                definition.JSONDefinition = request.JSONDefinition.EnrichEdgesForGateway();
            }
            else
            {
                ApplyGatewayBackfill(definition, persistIfChanged: false);
            }

            if (!string.IsNullOrEmpty(request.Name)) definition.Name = request.Name;
            if (request.Description != null) definition.Description = request.Description;
            if (request.Tags != null) definition.Tags = request.Tags;

            definition.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowDefinitionDto>(definition);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Draft updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateDraft failed {DefinitionId}", definitionId);
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
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Already published (immutability enforced)");

            var validation = _graphValidator.Validate(definition.JSONDefinition, strict: true);
            if (!validation.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Cannot publish invalid definition: {string.Join("; ", validation.Errors)}");

            definition.IsPublished = true;
            definition.PublishedAt = DateTime.UtcNow;
            definition.PublishNotes = request.PublishNotes;
            definition.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishDefinitionPublishedAsync(definition, cancellationToken);

            var dto = _mapper.Map<WorkflowDefinitionDto>(definition);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Definition published");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Publish failed {DefinitionId}", definitionId);
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
            _logger.LogError(ex, "GetById failed {DefinitionId}", definitionId);
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
            _logger.LogError(ex, "GetAll failed");
            return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.ErrorResult("Failed to retrieve workflow definitions");
        }
    }

    public async Task<ApiResponseDto<ValidationResultDto>> ValidateDefinitionAsync(ValidateDefinitionRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _graphValidator.Validate(request.JSONDefinition, strict: true);
            if (!result.IsValid)
            {
                // Map List<string> → List<ErrorDto>
                var errorDtos = result.Errors.Select(e => new ErrorDto
                {
                    Code = "Validation",
                    Message = e
                }).ToList();

                return ApiResponseDto<ValidationResultDto>.ErrorResult("Invalid", errorDtos);
            }
            return ApiResponseDto<ValidationResultDto>.SuccessResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validate failed");
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

            var hasInstances = await _context.WorkflowInstances.AnyAsync(i => i.WorkflowDefinitionId == definitionId, cancellationToken);
            if (hasInstances)
                return ApiResponseDto<bool>.ErrorResult("Cannot delete definition with existing instances");

            _context.WorkflowDefinitions.Remove(definition);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponseDto<bool>.SuccessResult(true, "Draft deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteDraft failed {DefinitionId}", definitionId);
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

            var validation = _graphValidator.Validate(request.JSONDefinition, strict: true);
            if (!validation.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult($"Invalid workflow definition: {string.Join("; ", validation.Errors)}");

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
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "New version created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateNewVersion failed {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to create new version");
        }
    }
}
