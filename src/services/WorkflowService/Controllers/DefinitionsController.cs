using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.AspNetCore.Mvc;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowService.Services.Validation;
using WorkflowService.Engine;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/definitions")]
public class DefinitionsController : ControllerBase
{
    private readonly IDefinitionService _definitionService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<DefinitionsController> _logger;
    private readonly IWorkflowGraphValidator _graphValidator;
    private readonly WorkflowDbContext _db;

    public DefinitionsController(
        IDefinitionService definitionService,
        ITenantProvider tenantProvider,
        WorkflowDbContext db,
        ILogger<DefinitionsController> logger,
        IWorkflowGraphValidator graphValidator)
    {
        _definitionService = definitionService;
        _tenantProvider = tenantProvider;
        _db = db;
        _logger = logger;
        _graphValidator = graphValidator;
    }

    [HttpGet]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowDefinitionDto>>>> GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool? published = null,
        [FromQuery] bool includeArchived = false,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var req = new GetWorkflowDefinitionsRequestDto
        {
            SearchTerm = search,
            IsPublished = published,
            Page = page,
            PageSize = pageSize,
            IncludeArchived = includeArchived
        };

        var resp = await _definitionService.GetAllAsync(req, ct);
        if (!resp.Success || resp.Data == null)
            return StatusCode(500, ApiResponseDto<List<WorkflowDefinitionDto>>.ErrorResult(resp.Message ?? "Failed"));

        // New: server‑side archived filtering unless explicitly requested
        var items = includeArchived
            ? resp.Data.Items
            : resp.Data.Items.Where(d => !d.IsArchived).ToList();

        return Ok(ApiResponseDto<List<WorkflowDefinitionDto>>.SuccessResult(items));
    }

    [HttpGet("{id:int}")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Get(int id, CancellationToken ct)
    {
        var resp = await _definitionService.GetByIdAsync(id, null, ct);
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
        var resp = await _definitionService.ValidateDefinitionAsync(request, ct);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpPost("draft")]
    [RequiresPermission(Permissions.Workflow.CreateDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> CreateDraft(
        [FromBody] CreateWorkflowDefinitionDto request,
        CancellationToken ct)
    {
        var resp = await _definitionService.CreateDraftAsync(request, ct);
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
        var resp = await _definitionService.UpdateDraftAsync(id, request, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return Ok(resp);
    }

    [HttpDelete("{id:int}")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteDraft(int id, CancellationToken ct)
    {
        var resp = await _definitionService.DeleteDraftAsync(id, ct);
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
        var resp = await _definitionService.CreateNewVersionAsync(id, request, ct);
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

        var defResp = await _definitionService.GetByIdAsync(id, null, ct);
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

        var resp = await _definitionService.PublishAsync(id, request, ct);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    [HttpGet("{id:int}/usage")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionInstanceUsageDto>>> Usage(int id, CancellationToken ct)
    {
        var resp = await _definitionService.GetUsageAsync(id, ct);
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
            var resp = await _definitionService.UnpublishAsync(id, request, ct);
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
        var defResp = await _definitionService.GetByIdAsync(id, null, ct);
        if (!defResp.Success || defResp.Data == null)
            return NotFound(ApiResponseDto<ValidationResultDto>.ErrorResult("Definition not found"));

        var validate = await _definitionService.ValidateDefinitionAsync(new ValidateDefinitionRequestDto
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
            serviceResp = await _definitionService.GetByIdAsync(id, null, ct);
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
