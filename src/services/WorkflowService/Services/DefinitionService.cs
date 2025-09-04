using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowService.Engine;
using WorkflowService.Engine.Validation;
using DTOs.Workflow.Enums;

namespace WorkflowService.Services;

public class DefinitionService : IDefinitionService
{
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IEventPublisher _eventPublisher;
    private readonly IGraphValidationService _graphValidator;
    private readonly ILogger<DefinitionService> _logger;
    private readonly IWorkflowPublishValidator _publishValidator;

    public DefinitionService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IEventPublisher eventPublisher,
        IGraphValidationService graphValidator,
        ILogger<DefinitionService> logger,
        IWorkflowPublishValidator? publishValidator = null)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _eventPublisher = eventPublisher;
        _graphValidator = graphValidator;
        _logger = logger;
        _publishValidator = publishValidator ?? NoopWorkflowPublishValidator.Instance;
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

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> CreateDraftAsync(
        CreateWorkflowDefinitionDto request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.JSONDefinition))
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("JSONDefinition is required.");

        WorkflowDefinitionJson parsed;
        try { parsed = BuilderDefinitionAdapter.Parse(request.JSONDefinition); }
        catch { return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid workflow JSON."); }

        var draftValidation = parsed.ValidateForDraft();
        bool hasFatal = draftValidation.Errors.Any(e =>
            e.StartsWith("A Start node is required", StringComparison.OrdinalIgnoreCase) ||
            e.StartsWith("Edge ", StringComparison.OrdinalIgnoreCase) ||
            e.StartsWith("Duplicate node id", StringComparison.OrdinalIgnoreCase));

        if (hasFatal)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Draft validation errors: " + string.Join("; ", draftValidation.Errors));

        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

        var def = new WorkflowDefinition
        {
            TenantId = tenantId.Value,
            Name = request.Name,
            Description = request.Description,
            JSONDefinition = request.JSONDefinition,
            IsPublished = false,
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.WorkflowDefinitions.Add(def);
        await _context.SaveChangesAsync(ct);

        var dto = _mapper.Map<WorkflowDefinitionDto>(def);
        return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto,
            draftValidation.Warnings.Any() ? "Draft saved with warnings" : "Draft saved");
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> UpdateDraftAsync(
        int definitionId,
        UpdateWorkflowDefinitionDto request,
        CancellationToken cancellationToken = default)
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

            List<string> warnings = new();

            if (request.JSONDefinition != null)
            {
                if (string.IsNullOrWhiteSpace(request.JSONDefinition))
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("JSONDefinition cannot be empty.");

                WorkflowDefinitionJson parsed;
                try { parsed = BuilderDefinitionAdapter.Parse(request.JSONDefinition); }
                catch { return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid workflow JSON."); }

                var draftValidation = parsed.ValidateForDraft();
                bool hasFatal = draftValidation.Errors.Any(e =>
                    e.StartsWith("A Start node is required", StringComparison.OrdinalIgnoreCase) ||
                    e.StartsWith("Edge ", StringComparison.OrdinalIgnoreCase) ||
                    e.StartsWith("Duplicate node id", StringComparison.OrdinalIgnoreCase));

                if (hasFatal)
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Draft validation errors: " + string.Join("; ", draftValidation.Errors));

                warnings = draftValidation.Warnings;
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

            var dto = await MapWithActiveCountAsync(definition, cancellationToken);
            var msg = warnings.Any() ? "Draft updated with warnings" : "Draft updated";
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, msg);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "UpdateDraft failed {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to update workflow definition");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> PublishAsync(
        int definitionId,
        PublishDefinitionRequestDto request,
        CancellationToken cancellationToken = default)
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
            {
                if (request.ForcePublish)
                {
                    // Detect post-publish JSON mutation attempt
                    var entry = _context.Entry(definition);
                    if (entry.State == EntityState.Modified)
                    {
                        var originalJson = entry.OriginalValues.GetValue<string>("JSONDefinition");
                        var currentJson = definition.JSONDefinition;
                        if (!string.Equals(originalJson, currentJson, StringComparison.Ordinal))
                        {
                            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                                "Force publish blocked: JSONDefinition changed. Create a new version.");
                        }
                    }

                    var dtoIdem = await MapWithActiveCountAsync(definition, cancellationToken);
                    return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dtoIdem, "Already published");
                }

                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Already published (immutable)");
            }

            var validation = _graphValidator.Validate(definition.JSONDefinition, strict: true);
            if (!validation.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    $"Cannot publish invalid definition: {string.Join("; ", validation.Errors)}");

            WorkflowDefinitionJson dsl;
            try { dsl = BuilderDefinitionAdapter.Parse(definition.JSONDefinition); }
            catch { return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid workflow JSON"); }

            var publishErrors = _publishValidator.Validate(dsl.ToModelStub(definition), dsl.Nodes.Select(n => n.ToModelNode()));
            if (publishErrors.Count > 0)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Publish-time validation failed: " + string.Join("; ", publishErrors));

            definition.IsPublished = true;
            definition.PublishedAt = DateTime.UtcNow;
            definition.PublishNotes = request.PublishNotes;
            definition.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishDefinitionPublishedAsync(definition, cancellationToken);

            var dto = await MapWithActiveCountAsync(definition, cancellationToken);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Definition published");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Publish failed {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to publish workflow definition");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> GetByIdAsync(
        int definitionId,
        int? version = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

            var query = _context.WorkflowDefinitions
                .Where(d => d.Id == definitionId && d.TenantId == tenantId.Value);

            if (version.HasValue)
                query = query.Where(d => d.Version == version.Value);

            var definition = await query.FirstOrDefaultAsync(cancellationToken);
            if (definition == null)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found");

            ApplyGatewayBackfill(definition, persistIfChanged: false);
            var dto = await MapWithActiveCountAsync(definition, cancellationToken);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetById failed {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to retrieve workflow definition");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>> GetAllAsync(
        GetWorkflowDefinitionsRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.ErrorResult("Tenant context required");

            var query = _context.WorkflowDefinitions.Where(d => d.TenantId == tenantId.Value);

            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var term = request.SearchTerm.Trim().ToLowerInvariant();
                query = query.Where(d =>
                    d.Name.ToLower().Contains(term) ||
                    (d.Description != null && d.Description.ToLower().Contains(term)));
            }

            if (request.IsPublished.HasValue)
                query = query.Where(d => d.IsPublished == request.IsPublished.Value);

            if (!string.IsNullOrWhiteSpace(request.Tags))
                query = query.Where(d => d.Tags != null && d.Tags.Contains(request.Tags));

            var sortKey = string.IsNullOrWhiteSpace(request.SortBy)
                ? "createdat"
                : request.SortBy.Trim().ToLowerInvariant();

            query = sortKey switch
            {
                "name" => request.SortDescending
                    ? query.OrderByDescending(d => d.Name)
                    : query.OrderBy(d => d.Name),
                "version" => request.SortDescending
                    ? query.OrderByDescending(d => d.Version).ThenBy(d => d.Name)
                    : query.OrderBy(d => d.Version).ThenBy(d => d.Name),
                "publishedat" => request.SortDescending
                    ? query.OrderByDescending(d => d.PublishedAt.HasValue).ThenByDescending(d => d.PublishedAt).ThenBy(d => d.Name)
                    : query.OrderByDescending(d => d.PublishedAt.HasValue).ThenBy(d => d.PublishedAt).ThenBy(d => d.Name),
                _ => request.SortDescending
                    ? query.OrderByDescending(d => d.CreatedAt).ThenBy(d => d.Name)
                    : query.OrderBy(d => d.CreatedAt).ThenBy(d => d.Name)
            };

            var page = request.Page <= 0 ? 1 : request.Page;
            var pageSize = request.PageSize <= 0 ? 20 : request.PageSize;

            var totalCount = await query.CountAsync(cancellationToken);
            var definitions = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            foreach (var def in definitions)
                ApplyGatewayBackfill(def, persistIfChanged: false);

            // Active counts (single grouped query)
            var defKeys = definitions.Select(d => new { d.Id, d.Version }).ToList();
            var activeCounts = await _context.WorkflowInstances
                .Where(i => defKeys.Contains(new { Id = i.WorkflowDefinitionId, Version = i.DefinitionVersion }) &&
                            (i.Status == InstanceStatus.Running || i.Status == InstanceStatus.Suspended))
                .GroupBy(i => new { i.WorkflowDefinitionId, i.DefinitionVersion })
                .Select(g => new
                {
                    g.Key.WorkflowDefinitionId,
                    g.Key.DefinitionVersion,
                    Count = g.Count()
                }).ToListAsync(cancellationToken);

            var dtos = new List<WorkflowDefinitionDto>();
            foreach (var def in definitions)
            {
                var dto = _mapper.Map<WorkflowDefinitionDto>(def)!;
                dto.ActiveInstanceCount = activeCounts
                    .FirstOrDefault(x => x.WorkflowDefinitionId == def.Id && x.DefinitionVersion == def.Version)?.Count ?? 0;
                dtos.Add(dto);
            }

            var paged = new PagedResultDto<WorkflowDefinitionDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.SuccessResult(paged);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAll failed");
            return ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>.ErrorResult("Failed to retrieve workflow definitions");
        }
    }

    public async Task<ApiResponseDto<ValidationResultDto>> ValidateDefinitionAsync(
        ValidateDefinitionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = _graphValidator.Validate(request.JSONDefinition, strict: true);
            if (!result.IsValid)
            {
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
            _logger.LogError(ex, "ValidateDefinition failed");
            return ApiResponseDto<ValidationResultDto>.ErrorResult("Failed to validate workflow definition");
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteDraftAsync(
        int definitionId,
        CancellationToken cancellationToken = default)
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
                .AnyAsync(i => i.WorkflowDefinitionId == definitionId && i.DefinitionVersion == definition.Version, cancellationToken);
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

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> CreateNewVersionAsync(
        int definitionId,
        CreateNewVersionRequestDto request,
        CancellationToken cancellationToken = default)
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
                Description = request.Description ?? existing.Description,
                JSONDefinition = request.JSONDefinition.EnrichEdgesForGateway(),
                Version = existing.Version + 1,
                IsPublished = false,
                Tags = request.Tags ?? existing.Tags,
                VersionNotes = request.VersionNotes,
                ParentDefinitionId = existing.Id,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.WorkflowDefinitions.Add(newDef);
            await _context.SaveChangesAsync(cancellationToken);

            var dto = await MapWithActiveCountAsync(newDef, cancellationToken);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "New version created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "CreateNewVersion failed {DefinitionId}", definitionId);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to create new version");
        }
    }

    public async Task<ApiResponseDto<WorkflowDefinitionInstanceUsageDto>> GetUsageAsync(int definitionId, CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            return ApiResponseDto<WorkflowDefinitionInstanceUsageDto>.ErrorResult("Tenant context required");

        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId && d.TenantId == tenantId.Value, cancellationToken);

        if (definition == null)
            return ApiResponseDto<WorkflowDefinitionInstanceUsageDto>.ErrorResult("Definition not found");

        var counts = await _context.WorkflowInstances
            .Where(i => i.WorkflowDefinitionId == definition.Id && i.DefinitionVersion == definition.Version)
            .GroupBy(i => i.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        int running = counts.Where(c => c.Status == InstanceStatus.Running).Select(c => c.Count).FirstOrDefault();
        int suspended = counts.Where(c => c.Status == InstanceStatus.Suspended).Select(c => c.Count).FirstOrDefault();
        int completed = counts.Where(c => c.Status == InstanceStatus.Completed).Select(c => c.Count).FirstOrDefault();

        var latestVersion = await _context.WorkflowDefinitions
            .Where(d => d.TenantId == tenantId.Value && d.Name == definition.Name)
            .MaxAsync(d => d.Version, cancellationToken);

        var dto = new WorkflowDefinitionInstanceUsageDto
        {
            DefinitionId = definition.Id,
            Version = definition.Version,
            RunningCount = running,
            SuspendedCount = suspended,
            CompletedCount = completed,
            LatestVersion = latestVersion
        };

        return ApiResponseDto<WorkflowDefinitionInstanceUsageDto>.SuccessResult(dto);
    }

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> UnpublishAsync(int definitionId, UnpublishDefinitionRequestDto request, CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId && d.TenantId == tenantId.Value, cancellationToken);

        if (definition == null)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found");
        if (!definition.IsPublished)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition already unpublished");

        var instances = await _context.WorkflowInstances
            .Where(i => i.WorkflowDefinitionId == definition.Id && i.DefinitionVersion == definition.Version)
            .ToListAsync(cancellationToken);

        var running = instances.Where(i => i.Status == InstanceStatus.Running).ToList();
        var suspended = instances.Where(i => i.Status == InstanceStatus.Suspended).ToList();

        if ((running.Any() || suspended.Any()) && !request.ForceTerminateAndUnpublish)
        {
            var errors = new List<ErrorDto>
            {
                new() { Code = "ActiveInstancesPresent", Message = $"Running={running.Count}, Suspended={suspended.Count}" }
            };
            _logger.LogWarning("Unpublish blocked due to active instances def={DefinitionId} run={Run} susp={Susp}",
                definition.Id, running.Count, suspended.Count);
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Active instances present", errors);
        }

        var cancelTargets = new List<WorkflowInstance>();
        if (request.ForceTerminateAndUnpublish && (running.Any() || suspended.Any()))
            cancelTargets = running.Concat(suspended).ToList();

        // Snapshot for manual rollback in providers without transactions (InMemory)
        var snapshot = cancelTargets.Select(i => new
        {
            Instance = i,
            Status = i.Status,
            CompletedAt = i.CompletedAt,
            UpdatedAt = i.UpdatedAt
        }).ToList();

        var providerName = _context.Database.ProviderName ?? string.Empty;
        var supportsTx = !providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase);

        using var tx = supportsTx ? await _context.Database.BeginTransactionAsync(cancellationToken) : null;

        try
        {
            if (cancelTargets.Any())
            {
                foreach (var inst in cancelTargets)
                {
                    inst.Status = InstanceStatus.Cancelled;
                    inst.CompletedAt = DateTime.UtcNow;
                    inst.UpdatedAt = DateTime.UtcNow;
                }
                await _context.SaveChangesAsync(cancellationToken);

                foreach (var inst in cancelTargets)
                {
                    // If publisher throws we want rollback behavior
                    await _eventPublisher.PublishInstanceForceCancelledAsync(inst, "unpublish", cancellationToken);
                }
            }

            definition.IsPublished = false;
            definition.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishDefinitionUnpublishedAsync(definition, cancellationToken);

            if (supportsTx && tx != null)
                await tx.CommitAsync(cancellationToken);

            var dto = await MapWithActiveCountAsync(definition, cancellationToken);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto,
                request.ForceTerminateAndUnpublish
                    ? "Definition unpublished (active instances force cancelled)"
                    : "Definition unpublished");
        }
        catch
        {
            // Manual rollback for InMemory (no real transaction)
            foreach (var snap in snapshot)
            {
                snap.Instance.Status = snap.Status;
                snap.Instance.CompletedAt = snap.CompletedAt;
                snap.Instance.UpdatedAt = snap.UpdatedAt;
            }

            if (supportsTx && tx != null)
            {
                try { await tx.RollbackAsync(cancellationToken); } catch { /* ignore */ }
            }
            // Rethrow to satisfy atomic failure test expectations
            throw;
        }
    }

    private async Task<WorkflowDefinitionDto> MapWithActiveCountAsync(WorkflowDefinition def, CancellationToken ct)
    {
        var count = await _context.WorkflowInstances
            .Where(i => i.WorkflowDefinitionId == def.Id &&
                        i.DefinitionVersion == def.Version &&
                        (i.Status == InstanceStatus.Running || i.Status == InstanceStatus.Suspended))
            .CountAsync(ct);

        var dto = _mapper.Map<WorkflowDefinitionDto>(def)!;
        dto.ActiveInstanceCount = count;
        return dto;
    }
}

file static class DefinitionServicePublishValidationAdapters
{
    public static WorkflowService.Domain.Models.WorkflowDefinition ToModelStub(
        this WorkflowDefinitionJson dsl,
        WorkflowService.Domain.Models.WorkflowDefinition entity) => entity;

    public static WorkflowService.Domain.Dsl.WorkflowNode ToModelNode(
        this WorkflowService.Domain.Dsl.WorkflowNode node) => node;
}

file sealed class NoopWorkflowPublishValidator : IWorkflowPublishValidator
{
    public static readonly NoopWorkflowPublishValidator Instance = new();
    private NoopWorkflowPublishValidator() { }
    public IReadOnlyList<string> Validate(WorkflowService.Domain.Models.WorkflowDefinition definition, IEnumerable<WorkflowService.Domain.Dsl.WorkflowNode> nodes)
        => Array.Empty<string>();
}
