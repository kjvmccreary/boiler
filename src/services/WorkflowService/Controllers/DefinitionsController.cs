using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.AspNetCore.Mvc;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowService.Services.Validation;
using WorkflowService.Engine; // BuilderDefinitionAdapter
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

    // Restored constructor signature expected by legacy tests
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
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 100,
        CancellationToken ct = default)
    {
        var req = new GetWorkflowDefinitionsRequestDto
        {
            SearchTerm = search,
            IsPublished = published,
            Page = page,
            PageSize = pageSize
        };
        var resp = await _definitionService.GetAllAsync(req, ct);
        if (!resp.Success || resp.Data == null)
            return StatusCode(500, ApiResponseDto<List<WorkflowDefinitionDto>>.ErrorResult(resp.Message ?? "Failed"));
        return Ok(ApiResponseDto<List<WorkflowDefinitionDto>>.SuccessResult(resp.Data.Items));
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
            if (!resp.Success)
            {
                var code = resp.Errors?.FirstOrDefault()?.Code;
                if (code == "ActiveInstancesPresent")
                    return Conflict(resp);
                return BadRequest(resp);
            }
            return Ok(resp);
        }
        catch (InvalidOperationException ex) when (ex.Message.StartsWith("Published workflow JSONDefinition", StringComparison.OrdinalIgnoreCase))
        {
            var errors = new List<ErrorDto> { new() { Code = "ImmutabilityViolation", Message = ex.Message } };
            return Conflict(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Immutability violation", errors));
        }
    }

    // Legacy endpoint required by tests: revalidate stored JSON
    [HttpPost("{id:int}/revalidate")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<ValidationResultDto>>> Revalidate(int id, CancellationToken ct = default)
    {
        var defResp = await _definitionService.GetByIdAsync(id, null, ct);
        if (!defResp.Success || defResp.Data == null)
            return NotFound(ApiResponseDto<ValidationResultDto>.ErrorResult("Definition not found"));

        try
        {
            var dsl = BuilderDefinitionAdapter.Parse(defResp.Data.JSONDefinition);
            var vr = _graphValidator.ValidateForPublish(dsl);
            var result = new ValidationResultDto
            {
                IsValid = vr.IsValid,
                Errors = vr.Errors,
                Warnings = vr.Warnings
            };
            return vr.IsValid
                ? Ok(ApiResponseDto<ValidationResultDto>.SuccessResult(result))
                : BadRequest(ApiResponseDto<ValidationResultDto>.ErrorResult("Invalid", vr.Errors.Select(e => new ErrorDto { Code = "Validation", Message = e }).ToList()));
        }
        catch
        {
            return BadRequest(ApiResponseDto<ValidationResultDto>.ErrorResult("Invalid workflow JSON"));
        }
    }

    // Legacy endpoint required by tests: terminate running instances for a definition
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
        var defResp = await _definitionService.GetByIdAsync(id, null, ct);
        if (!defResp.Success || defResp.Data == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));

        if (defResp.Data.IsPublished)
            return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Unpublish before archiving"));

        return Ok(defResp);
    }

    private int GetCurrentUserId()
    {
        var claimVal = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(claimVal, out var id) ? id : 0;
    }
}
