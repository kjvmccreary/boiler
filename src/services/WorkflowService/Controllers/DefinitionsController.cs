using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowService.Persistence;
using WorkflowService.Engine;
using WorkflowService.Services.Validation;
using Microsoft.EntityFrameworkCore;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/definitions")]
[Authorize(Policy = "workflow.read")]
public class DefinitionsController : ControllerBase
{
    private readonly IDefinitionService _definitions;
    private readonly ITenantProvider _tenantProvider;
    private readonly WorkflowDbContext _db;
    private readonly IWorkflowGraphValidator _graphValidator;
    private readonly ILogger<DefinitionsController> _logger;

    public DefinitionsController(
        IDefinitionService definitions,
        ITenantProvider tenantProvider,
        WorkflowDbContext db,
        ILogger<DefinitionsController> logger,
        IWorkflowGraphValidator graphValidator)
    {
        _definitions = definitions;
        _tenantProvider = tenantProvider;
        _db = db;
        _logger = logger;
        _graphValidator = graphValidator;
    }

    /// <summary>
    /// Returns workflow definitions (paged).
    /// </summary>
    /// <remarks>
    /// Filtering &amp; paging rules:
    /// - By default archived definitions are excluded (<c>includeArchived=false</c>).
    /// - Set <c>includeArchived=true</c> to include archived definitions in results.
    /// - You can filter by publication status using <c>published=true|false</c>.
    /// - Text search (name / description / tags) via <c>search</c> query parameter.
    /// - Tag filtering via comma-delimited <c>tags</c>.
    /// - Sorting: <c>sortBy</c> (e.g. <c>createdAt</c>, <c>updatedAt</c>, <c>name</c>) with <c>desc=true</c> for descending.
    /// - Paging controlled by <c>page</c> (1-based) and <c>pageSize</c>.
    ///
    /// Response envelope:
    /// <code>
    /// {
    ///   "success": true,
    ///   "message": null,
    ///   "errors": null,
    ///   "data": {
    ///     "items": [
    ///       {
    ///         "id": 123,
    ///         "name": "Onboarding",
    ///         "version": 3,
    ///         "jsonDefinition": "...",
    ///         "description": "Employee onboarding flow",
    ///         "isPublished": true,
    ///         "isArchived": false,
    ///         "publishedAt": "2025-09-08T12:34:56Z",
    ///         "archivedAt": null,
    ///         "createdAt": "2025-08-01T09:10:11Z",
    ///         "updatedAt": "2025-09-08T12:34:56Z",
    ///         "activeInstanceCount": 4,
    ///         "publishNotes": "Minor SLA update",
    ///         "versionNotes": "v3 adjustments",
    ///         "tags": "hr,onboarding"
    ///       }
    ///     ],
    ///     "totalCount": 42,
    ///     "page": 1,
    ///     "pageSize": 20
    ///   }
    /// }
    /// </code>
    ///
    /// Notes:
    /// - <c>activeInstanceCount</c> may be 0 for archived or draft-only definitions.
    /// - <c>publishNotes</c>, <c>versionNotes</c>, and <c>tags</c> can be null.
    /// - When <c>includeArchived=false</c>, records where <c>isArchived=true</c> are filtered out before paging.
    /// </remarks>
    /// <param name="search">Text search across name, description, tags.</param>
    /// <param name="published">Filter by publication state (true = only published, false = only drafts).</param>
    /// <param name="tags">Comma-delimited tag filter (AND semantics optionally implemented server-side).</param>
    /// <param name="sortBy">Field to sort by (default server-defined).</param>
    /// <param name="desc">Set true for descending sort.</param>
    /// <param name="page">1-based page index.</param>
    /// <param name="pageSize">Page size (max limits may apply).</param>
    /// <param name="includeArchived">Include archived definitions (default false).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <response code="200">Paged list of workflow definitions.</response>
    /// <response code="401">Unauthorized / missing tenant context.</response>
    /// <response code="403">Forbidden (insufficient permissions).</response>
    [HttpGet]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<WorkflowDefinitionDto>>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool? published = null,
        [FromQuery] string? tags = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool desc = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] bool includeArchived = false,
        CancellationToken ct = default)
    {
        var request = new GetWorkflowDefinitionsRequestDto
        {
            SearchTerm = search,
            IsPublished = published,
            Tags = tags,
            SortBy = sortBy,
            SortDescending = desc,
            Page = page,
            PageSize = pageSize,
            IncludeArchived = includeArchived
        };

        var result = await _definitions.GetAllAsync(request, ct);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Get(int id, CancellationToken ct)
    {
        var resp = await _definitions.GetByIdAsync(id, null, ct);
        if (!resp.Success || resp.Data == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(resp.Message ?? "Definition not found"));
        return Ok(resp);
    }

    [HttpPost("validate")]
    [RequiresPermission(Permissions.Workflow.CreateDefinitions)]
    public async Task<ActionResult<ApiResponseDto<ValidationResultDto>>> Validate(
        [FromBody] ValidateDefinitionRequestDto request,
        CancellationToken ct)
    {
        var resp = await _definitions.ValidateDefinitionAsync(request, ct);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpPost("draft")]
    [RequiresPermission(Permissions.Workflow.CreateDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> CreateDraft(
        [FromBody] CreateWorkflowDefinitionDto request,
        CancellationToken ct)
    {
        var resp = await _definitions.CreateDraftAsync(request, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return CreatedAtAction(nameof(Get), new { id = resp.Data!.Id }, resp);
    }

    [HttpPut("{id:int}")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> UpdateDraft(
        int id,
        [FromBody] UpdateWorkflowDefinitionDto request,
        CancellationToken ct)
    {
        var resp = await _definitions.UpdateDraftAsync(id, request, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return Ok(resp);
    }

    [HttpDelete("{id:int}")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteDraft(int id, CancellationToken ct)
    {
        var resp = await _definitions.DeleteDraftAsync(id, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return Ok(resp);
    }

    [HttpPost("{id:int}/new-version")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> CreateNewVersion(
        int id,
        [FromBody] CreateNewVersionRequestDto request,
        CancellationToken ct)
    {
        var resp = await _definitions.CreateNewVersionAsync(id, request, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return Ok(resp);
    }

    [HttpPost("{id:int}/publish")]
    [RequiresPermission(Permissions.Workflow.PublishDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Publish(
        int id,
        [FromBody] PublishDefinitionRequestDto? request,
        CancellationToken ct = default)
    {
        request ??= new PublishDefinitionRequestDto();

        var defResp = await _definitions.GetByIdAsync(id, null, ct);
        if (!defResp.Success || defResp.Data == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));

        try
        {
            var json = defResp.Data.JSONDefinition;
            var parsed = BuilderDefinitionAdapter.Parse(json);
            var validation = _graphValidator.ValidateForPublish(parsed);

            if (validation.Errors.Any())
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Validation failed",
                    errors = validation.Errors
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Publish validation parse failure for definition {Id}", id);
            return BadRequest(new
            {
                success = false,
                message = "Validation failed",
                errors = new[] { "Invalid workflow JSON" }
            });
        }

        var resp = await _definitions.PublishAsync(id, request, ct);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpGet("{id:int}/usage")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionInstanceUsageDto>>> Usage(int id, CancellationToken ct)
    {
        var resp = await _definitions.GetUsageAsync(id, ct);
        if (!resp.Success) return NotFound(resp);
        return Ok(resp);
    }

    [HttpPost("{id:int}/unpublish")]
    [RequiresPermission(Permissions.Workflow.PublishDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Unpublish(
        int id,
        [FromBody] UnpublishDefinitionRequestDto? request = null,
        CancellationToken ct = default)
    {
        request ??= new UnpublishDefinitionRequestDto();

        try
        {
            var resp = await _definitions.UnpublishAsync(id, request, ct);
            if (resp != null && resp.Success) return Ok(resp);
            if (resp != null && !resp.Success)
            {
                var code = resp.Errors?.FirstOrDefault()?.Code;
                if (code == "ActiveInstancesPresent") return Conflict(resp);
                return BadRequest(resp);
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Published workflow JSONDefinition", StringComparison.OrdinalIgnoreCase))
        {
            var errors = new List<ErrorDto> { new() { Code = "ImmutabilityViolation", Message = ex.Message } };
            return Conflict(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Immutability violation", errors));
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Service unpublish path failed, falling back to direct DB mutation");
        }

        var def = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (def == null) return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));
        def.IsPublished = false;
        def.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(new WorkflowDefinitionDto
        {
            Id = def.Id,
            Name = def.Name,
            Version = def.Version,
            JSONDefinition = def.JSONDefinition,
            IsPublished = def.IsPublished,
            CreatedAt = def.CreatedAt,
            UpdatedAt = def.UpdatedAt
        }));
    }

    [HttpPost("{id:int}/revalidate")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<ValidationResultDto>>> Revalidate(int id, CancellationToken ct = default)
    {
        var defResp = await _definitions.GetByIdAsync(id, null, ct);
        if (!defResp.Success || defResp.Data == null)
            return NotFound(ApiResponseDto<ValidationResultDto>.ErrorResult("Definition not found"));

        var validate = await _definitions.ValidateDefinitionAsync(new ValidateDefinitionRequestDto
        {
            JSONDefinition = defResp.Data.JSONDefinition
        }, ct);

        return validate.Success ? Ok(validate) : BadRequest(validate);
    }

    [HttpPost("{id:int}/terminate-running")]
    [RequiresPermission(Permissions.Workflow.ManageInstances)]
    public async Task<ActionResult<ApiResponseDto<object>>> TerminateRunning(int id, CancellationToken ct = default)
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (!tenantId.HasValue)
            return Unauthorized(ApiResponseDto<object>.ErrorResult("Tenant context required"));

        var exists = await _db.WorkflowDefinitions.AnyAsync(d => d.Id == id && d.TenantId == tenantId.Value, ct);
        if (!exists)
            return NotFound(ApiResponseDto<object>.ErrorResult("Definition not found"));

        var running = await _db.WorkflowInstances
            .Where(i => i.WorkflowDefinitionId == id &&
                        i.TenantId == tenantId.Value &&
                        i.Status == DTOs.Workflow.Enums.InstanceStatus.Running)
            .ToListAsync(ct);

        foreach (var inst in running)
        {
            inst.Status = DTOs.Workflow.Enums.InstanceStatus.Cancelled;
            inst.CompletedAt = DateTime.UtcNow;
            inst.UpdatedAt = DateTime.UtcNow;
        }
        await _db.SaveChangesAsync(ct);

        return Ok(ApiResponseDto<object>.SuccessResult(new { terminated = running.Count }, "Running instances terminated"));
    }

    [HttpPost("{id:int}/archive")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Archive(int id, CancellationToken ct = default)
    {
        ApiResponseDto<WorkflowDefinitionDto>? serviceResp = null;
        try
        {
            serviceResp = await _definitions.GetByIdAsync(id, null, ct);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Service GetByIdAsync threw during archive; falling back to direct DB lookup");
        }

        WorkflowDefinitionDto? dto = null;
        if (serviceResp?.Success == true && serviceResp.Data != null)
        {
            dto = serviceResp.Data;
        }

        if (dto == null)
        {
            var entity = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id, ct);
            if (entity == null)
                return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));

            if (entity.IsPublished)
                return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Unpublish before archiving"));

            entity.IsArchived = true;
            entity.ArchivedAt = DateTime.UtcNow;
            entity.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);

            dto = new WorkflowDefinitionDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Version = entity.Version,
                JSONDefinition = entity.JSONDefinition,
                IsPublished = entity.IsPublished,
                IsArchived = entity.IsArchived,
                ArchivedAt = entity.ArchivedAt,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Archived"));
        }

        var dbEntity = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id, ct);
        if (dbEntity == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));

        if (dbEntity.IsPublished)
            return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Unpublish before archiving"));

        dbEntity.IsArchived = true;
        dbEntity.ArchivedAt = DateTime.UtcNow;
        dbEntity.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);

        dto.IsArchived = true;
        dto.ArchivedAt = dbEntity.ArchivedAt;
        dto.UpdatedAt = dbEntity.UpdatedAt;

        return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Archived"));
    }

    private int GetCurrentUserId()
    {
        var claimVal = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claimVal, out var id) ? id : 0;
    }
}
