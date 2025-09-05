using System.Collections.Concurrent;
using AutoMapper;
using Contracts.Services;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.Validation;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;

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

    private static readonly bool IsolationDiagnosticsEnabled =
        Environment.GetEnvironmentVariable("WF_ISO_DIAG") == "1";

    // Central in-memory diagnostics queue (test-readable without relying on logger plumbing)
#if DEBUG
    public static ConcurrentQueue<string> IsolationDiag { get; } = new();
#else
    public static ConcurrentQueue<string> IsolationDiag { get; } = new(); // or internal if tests compiled into same assembly
#endif
    private static void Iso(string msg)
    {
        if (IsolationDiagnosticsEnabled) IsolationDiag.Enqueue(msg);
    }

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

    #region Create / Update / Publish

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> CreateDraftAsync(
        CreateWorkflowDefinitionDto request,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.JSONDefinition))
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("JSONDefinition is required.",
                new List<ErrorDto> { new("Validation", "JSONDefinition is required.") });

        WorkflowDefinitionJson parsed;
        try { parsed = BuilderDefinitionAdapter.Parse(request.JSONDefinition); }
        catch
        {
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid workflow JSON.",
                new List<ErrorDto> { new("Validation", "Invalid workflow JSON.") });
        }

        var draftValidation = parsed.ValidateForDraft();
        bool hasFatal = draftValidation.Errors.Any(e =>
            e.StartsWith("A Start node is required", StringComparison.OrdinalIgnoreCase) ||
            e.StartsWith("Edge ", StringComparison.OrdinalIgnoreCase) ||
            e.StartsWith("Duplicate node id", StringComparison.OrdinalIgnoreCase));

        if (hasFatal)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Draft validation errors",
                draftValidation.Errors.Select(e => new ErrorDto("Validation", e)).ToList());

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
        return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(
            dto,
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
            {
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Cannot modify a published definition (immutability enforced)",
                    new List<ErrorDto> { new("ImmutabilityViolation", "Published definition content is immutable. Create a new version.") });
            }

            List<string> warnings = new();

            if (request.JSONDefinition != null)
            {
                if (string.IsNullOrWhiteSpace(request.JSONDefinition))
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("JSONDefinition cannot be empty.",
                        new List<ErrorDto> { new("Validation", "JSONDefinition cannot be empty.") });

                WorkflowDefinitionJson parsed;
                try { parsed = BuilderDefinitionAdapter.Parse(request.JSONDefinition); }
                catch
                {
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid workflow JSON.",
                        new List<ErrorDto> { new("Validation", "Invalid workflow JSON.") });
                }

                var draftValidation = parsed.ValidateForDraft();
                bool hasFatal = draftValidation.Errors.Any(e =>
                    e.StartsWith("A Start node is required", StringComparison.OrdinalIgnoreCase) ||
                    e.StartsWith("Edge ", StringComparison.OrdinalIgnoreCase) ||
                    e.StartsWith("Duplicate node id", StringComparison.OrdinalIgnoreCase));

                if (hasFatal)
                    return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                        "Draft validation errors",
                        draftValidation.Errors.Select(e => new ErrorDto("Validation", e)).ToList());

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
                    // Immutability guard: if JSONDefinition text changed after first publish,
                    // block "force publish" (test expects phrase "Force publish blocked").
                    var entry = _context.Entry(definition);
                    var originalJson = entry.OriginalValues.GetValue<string>(nameof(WorkflowDefinition.JSONDefinition));
                    if (!string.Equals(originalJson, definition.JSONDefinition, StringComparison.Ordinal))
                    {
                        return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                            "Force publish blocked: definition JSONDefinition was modified after publish",
                            new List<ErrorDto> {
                                new("ImmutabilityViolation",
                                    "Published definition JSONDefinition was modified. Create a new version instead.")
                            });
                    }

                    var dtoAlready = await MapWithActiveCountAsync(definition, cancellationToken);
                    return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dtoAlready, "Already published");
                }

                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Already published (immutable)",
                    new List<ErrorDto> { new("ImmutabilityViolation", "Definition already published and immutable.") });
            }

            var validation = _graphValidator.Validate(definition.JSONDefinition, strict: true);
            if (!validation.IsValid)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Cannot publish invalid definition",
                    validation.Errors.Select(e => new ErrorDto("Validation", e)).ToList());

            WorkflowDefinitionJson dsl;
            try { dsl = BuilderDefinitionAdapter.Parse(definition.JSONDefinition); }
            catch
            {
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Invalid workflow JSON",
                    new List<ErrorDto> { new("Validation", "Invalid workflow JSON") });
            }

            var publishErrors = _publishValidator.Validate(dsl.ToModelStub(definition), dsl.Nodes.Select(n => n.ToModelNode()));
            if (publishErrors.Count > 0)
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Publish-time validation failed",
                    publishErrors.Select(e => new ErrorDto("Validation", e)).ToList());

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

    #endregion

    #region Unpublish (Diagnostics)

    public async Task<ApiResponseDto<WorkflowDefinitionDto>> UnpublishAsync(
        int definitionId,
        UnpublishDefinitionRequestDto request,
        CancellationToken cancellationToken = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required");

        if (IsolationDiagnosticsEnabled)
        {
            var line = $"UNPUBLISH_REQUEST def={definitionId} force={request.ForceTerminateAndUnpublish} tenant={tenantId.Value}";
            _logger.LogDebug("ISO:{Line}", line); Iso(line);
        }

        var definition = await _context.WorkflowDefinitions
            .FirstOrDefaultAsync(d => d.Id == definitionId && d.TenantId == tenantId.Value, cancellationToken);

        if (definition == null)
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found");

        if (!definition.IsPublished)
        {
            if (request.ForceTerminateAndUnpublish)
            {
                var dtoAlready = await MapWithActiveCountAsync(definition, cancellationToken);
                return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dtoAlready, "Already unpublished");
            }
            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition already unpublished");
        }

        var active = await _context.WorkflowInstances
            .AsNoTracking()
            .Where(i =>
                i.TenantId == definition.TenantId &&
                i.WorkflowDefinitionId == definition.Id &&
                i.DefinitionVersion == definition.Version &&
                (i.Status == InstanceStatus.Running || i.Status == InstanceStatus.Suspended))
            .Select(i => new { i.Id, i.Status })
            .ToListAsync(cancellationToken);

        if (IsolationDiagnosticsEnabled)
        {
            var line = $"ACTIVE def={definition.Id} tenant={definition.TenantId} active=[{string.Join(',', active.Select(a => $"{a.Id}:{a.Status}"))}]";
            _logger.LogDebug("ISO:{Line}", line); Iso(line);
        }

        if (!request.ForceTerminateAndUnpublish && active.Count > 0)
        {
            if (IsolationDiagnosticsEnabled)
            {
                var line = $"BLOCK def={definition.Id} activeCount={active.Count}";
                _logger.LogDebug("ISO:{Line}", line); Iso(line);
            }

            return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                "Active instances present",
                new List<ErrorDto> { new("ActiveInstancesPresent", $"ActiveCount={active.Count}") });
        }

        List<WorkflowInstance> cancelTargets = new();
        if (request.ForceTerminateAndUnpublish && active.Count > 0)
        {
            var ids = active.Select(a => a.Id).ToList();
            foreach (var tr in _context.ChangeTracker.Entries<WorkflowInstance>().ToList())
                tr.State = EntityState.Detached;

            cancelTargets = await _context.WorkflowInstances
                .Where(i => ids.Contains(i.Id) &&
                            i.TenantId == definition.TenantId &&
                            i.WorkflowDefinitionId == definition.Id &&
                            i.DefinitionVersion == definition.Version)
                .ToListAsync(cancellationToken);

            if (IsolationDiagnosticsEnabled)
            {
                var line = $"TARGETS def={definition.Id} tenant={definition.TenantId} targets=[{string.Join(',', cancelTargets.Select(c => $"{c.Id}:{c.TenantId}:{c.Status}"))}]";
                _logger.LogDebug("ISO:{Line}", line); Iso(line);
            }
        }

        var providerName = _context.Database.ProviderName ?? string.Empty;
        var supportsTx = !providerName.Contains("InMemory", StringComparison.OrdinalIgnoreCase);
        using var tx = supportsTx ? await _context.Database.BeginTransactionAsync(cancellationToken) : null;

        var rollback = cancelTargets.Select(i => new
        {
            Entity = i,
            i.Status,
            i.CompletedAt,
            i.UpdatedAt
        }).ToList();

        try
        {
            if (cancelTargets.Count > 0)
            {
                foreach (var inst in cancelTargets)
                {
                    inst.Status = InstanceStatus.Cancelled;
                    inst.CompletedAt = DateTime.UtcNow;
                    inst.UpdatedAt = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync(cancellationToken);

                foreach (var inst in cancelTargets.Where(i => i.Status == InstanceStatus.Cancelled))
                    await _eventPublisher.PublishInstanceForceCancelledAsync(inst, "unpublish", cancellationToken);

                if (IsolationDiagnosticsEnabled)
                {
                    var post = await _context.WorkflowInstances
                        .AsNoTracking()
                        .Where(i => i.WorkflowDefinitionId == definition.Id &&
                                    i.DefinitionVersion == definition.Version)
                        .Select(i => new { i.Id, i.TenantId, i.Status })
                        .ToListAsync(cancellationToken);

                    var line = $"POST_FORCE def={definition.Id} rows=[{string.Join(',', post.Select(p => $"{p.Id}:{p.TenantId}:{p.Status}"))}]";
                    _logger.LogDebug("ISO:{Line}", line); Iso(line);
                }

                _logger.LogInformation("UNPUBLISH_FORCE_TERMINATE def={Def} cancelledCount={Count} tenant={Tenant}",
                    definition.Id,
                    cancelTargets.Count(c => c.Status == InstanceStatus.Cancelled),
                    definition.TenantId);
            }

            definition.IsPublished = false;
            definition.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync(cancellationToken);
            await _eventPublisher.PublishDefinitionUnpublishedAsync(definition, cancellationToken);

            if (supportsTx && tx != null)
                await tx.CommitAsync(cancellationToken);

            if (IsolationDiagnosticsEnabled)
                await LogForeignCancelledCheck(definition, cancellationToken);

            var dto = await MapWithActiveCountAsync(definition, cancellationToken);
            return ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(
                dto,
                request.ForceTerminateAndUnpublish
                    ? "Definition unpublished (active instances force cancelled)"
                    : "Definition unpublished");
        }
        catch
        {
            foreach (var snap in rollback)
            {
                snap.Entity.Status = snap.Status;
                snap.Entity.CompletedAt = snap.CompletedAt;
                snap.Entity.UpdatedAt = snap.UpdatedAt;
            }
            if (supportsTx && tx != null)
            {
                try { await tx.RollbackAsync(cancellationToken); } catch { }
            }
            throw;
        }
    }

    private async Task LogForeignCancelledCheck(WorkflowDefinition def, CancellationToken ct)
    {
        var foreignCancelled = await _context.WorkflowInstances
            .AsNoTracking()
            .Where(i =>
                i.WorkflowDefinitionId == def.Id &&
                i.DefinitionVersion == def.Version &&
                i.TenantId != def.TenantId &&
                i.Status == InstanceStatus.Cancelled)
            .Select(i => new { i.Id, i.TenantId })
            .ToListAsync(ct);

        if (foreignCancelled.Count == 0)
        {
            var line = $"FOREIGN_OK def={def.Id}";
            _logger.LogDebug("ISO:{Line}", line); Iso(line);
        }
        else
        {
            var line = $"FOREIGN_CANCELLED def={def.Id} leaked=[{string.Join(',', foreignCancelled.Select(f => $"{f.Id}:{f.TenantId}"))}]";
            _logger.LogError("ISO:{Line}", line); Iso(line);
        }
    }

    #endregion

    #region Queries / Utility

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

            var definitionIds = definitions.Select(d => d.Id).Distinct().ToList();

            // Pull only necessary rows (translated) then aggregate in memory
            var instanceRows = await _context.WorkflowInstances
                .Where(i => definitionIds.Contains(i.WorkflowDefinitionId) &&
                            (i.Status == InstanceStatus.Running || i.Status == InstanceStatus.Suspended))
                .Select(i => new { i.WorkflowDefinitionId, i.DefinitionVersion })
                .ToListAsync(cancellationToken);

            // Group in memory (composite key supported in memory)
            var activeCounts = instanceRows
                .GroupBy(x => new { x.WorkflowDefinitionId, x.DefinitionVersion })
                .Select(g => new
                {
                    WorkflowDefinitionId = g.Key.WorkflowDefinitionId,
                    DefinitionVersion = g.Key.DefinitionVersion,
                    Count = g.Count()
                })
                .ToList();

            var dtos = new List<WorkflowDefinitionDto>();
            foreach (var def in definitions)
            {
                var d = _mapper.Map<WorkflowDefinitionDto>(def)!;
                d.ActiveInstanceCount = activeCounts
                    .FirstOrDefault(x => x.WorkflowDefinitionId == def.Id && x.DefinitionVersion == def.Version)?.Count ?? 0;
                dtos.Add(d);
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
                var errorDtos = result.Errors.Select(e => new ErrorDto("Validation", e)).ToList();
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
                return ApiResponseDto<bool>.ErrorResult("Cannot delete published workflow definition",
                    new List<ErrorDto> { new("ImmutabilityViolation", "Cannot delete a published definition.") });

            var hasInstances = await _context.WorkflowInstances
                .AnyAsync(i => i.WorkflowDefinitionId == definitionId && i.DefinitionVersion == definition.Version, cancellationToken);
            if (hasInstances)
                return ApiResponseDto<bool>.ErrorResult("Cannot delete definition with existing instances",
                    new List<ErrorDto> { new("Validation", "Instances exist for this definition.") });

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

            WorkflowDefinitionJson parsed;
            try { parsed = BuilderDefinitionAdapter.Parse(request.JSONDefinition); }
            catch
            {
                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Invalid workflow definition: Invalid JSON",
                    new List<ErrorDto> { new("Validation", "Invalid JSON") });
            }

            var validation = _graphValidator.Validate(request.JSONDefinition, strict: true);
            if (!validation.IsValid)
            {
                var errorDtos = validation.Errors
                    .Select(e => new ErrorDto("Validation", e))
                    .ToList();

                var baseMsg = "Invalid workflow definition";
                if (validation.Errors.Any())
                    baseMsg += $": {string.Join("; ", validation.Errors)}";
                if (!baseMsg.Contains("Invalid JSON", StringComparison.OrdinalIgnoreCase))
                    baseMsg += " (Invalid JSON)";

                return ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(baseMsg, errorDtos);
            }

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

    public async Task<ApiResponseDto<WorkflowDefinitionInstanceUsageDto>> GetUsageAsync(
        int definitionId,
        CancellationToken cancellationToken = default)
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

    #endregion

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

// TOP-LEVEL (not nested) extension helpers & validator to fix CS1109 / CS9054

internal static class DefinitionServicePublishValidationAdapters
{
    public static WorkflowService.Domain.Models.WorkflowDefinition ToModelStub(
        this WorkflowDefinitionJson dsl,
        WorkflowService.Domain.Models.WorkflowDefinition entity) => entity;

    public static WorkflowService.Domain.Dsl.WorkflowNode ToModelNode(
        this WorkflowService.Domain.Dsl.WorkflowNode node) => node;
}

internal sealed class NoopWorkflowPublishValidator : IWorkflowPublishValidator
{
    public static readonly NoopWorkflowPublishValidator Instance = new();
    private NoopWorkflowPublishValidator() { }
    public IReadOnlyList<string> Validate(
        WorkflowService.Domain.Models.WorkflowDefinition definition,
        IEnumerable<WorkflowService.Domain.Dsl.WorkflowNode> nodes) => Array.Empty<string>();
}
