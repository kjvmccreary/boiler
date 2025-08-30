using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Services.Interfaces;
using WorkflowService.Persistence;
using Contracts.Services;
using WorkflowService.Domain.Dsl;
using System.Security.Claims;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/definitions")]
public class DefinitionsController : ControllerBase
{
    private readonly IDefinitionService _definitionService;
    private readonly ITenantProvider _tenantProvider;
    private readonly WorkflowDbContext _db;
    private readonly ILogger<DefinitionsController> _logger;

    public DefinitionsController(
        IDefinitionService definitionService,
        ITenantProvider tenantProvider,
        WorkflowDbContext db,
        ILogger<DefinitionsController> logger)
    {
        _definitionService = definitionService;
        _tenantProvider = tenantProvider;
        _db = db;
        _logger = logger;
    }

    // LIST
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

    // GET single
    [HttpGet("{id:int}")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Get(int id, CancellationToken ct)
    {
        var resp = await _definitionService.GetByIdAsync(id, null, ct);
        if (!resp.Success || resp.Data == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(resp.Message ?? "Definition not found"));
        return Ok(resp);
    }

    // VALIDATE arbitrary JSON
    [HttpPost("validate")]
    [RequiresPermission(Permissions.Workflow.CreateDefinitions)]
    public async Task<ActionResult<ApiResponseDto<ValidationResultDto>>> Validate(
        [FromBody] ValidateDefinitionRequestDto request,
        CancellationToken ct)
    {
        var resp = await _definitionService.ValidateDefinitionAsync(request, ct);
        return resp.Success ? Ok(resp) : BadRequest(resp);
    }

    // CREATE draft
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

    // UPDATE draft
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

    // DELETE draft
    [HttpDelete("{id:int}")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteDraft(int id, CancellationToken ct)
    {
        var resp = await _definitionService.DeleteDraftAsync(id, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return Ok(resp);
    }

    // NEW VERSION
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

    // REVALIDATE existing stored JSON (draft or published)
    [HttpPost("{id:int}/revalidate")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<ValidationResultDto>>> Revalidate(int id, CancellationToken ct)
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

    // PUBLISH
    [HttpPost("{id:int}/publish")]
    [RequiresPermission(Permissions.Workflow.PublishDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Publish(
        int id,
        [FromBody] PublishDefinitionRequestDto request,
        CancellationToken ct)
    {
        var resp = await _definitionService.PublishAsync(id, request, ct);
        if (!resp.Success)
            return BadRequest(resp);
        return Ok(resp);
    }

    // UNPUBLISH
    [HttpPost("{id:int}/unpublish")]
    [RequiresPermission(Permissions.Workflow.PublishDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Unpublish(int id, CancellationToken ct)
    {
        var tenantId = await RequireTenantAsync();
        if (tenantId == null)
            return Unauthorized(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required"));

        var def = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId, ct);
        if (def == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));
        if (!def.IsPublished)
            return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Already unpublished"));

        def.IsPublished = false;
        def.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(Map(def), "Definition unpublished"));
    }

    // ARCHIVE
    [HttpPost("{id:int}/archive")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Archive(int id, CancellationToken ct)
    {
        var tenantId = await RequireTenantAsync();
        if (tenantId == null)
            return Unauthorized(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Tenant context required"));

        var def = await _db.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId, ct);
        if (def == null)
            return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));
        if (def.IsArchived)
            return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Already archived"));

        def.IsArchived = true;
        def.ArchivedAt = DateTime.UtcNow;
        def.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(ct);
        return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(Map(def), "Definition archived"));
    }

    // TERMINATE running instances
    [HttpPost("{id:int}/terminate-running")]
    [RequiresPermission(Permissions.Workflow.ManageInstances)]
    public async Task<ActionResult<ApiResponseDto<object>>> TerminateRunning(int id, CancellationToken ct)
    {
        var tenantId = await RequireTenantAsync();
        if (tenantId == null)
            return Unauthorized(ApiResponseDto<object>.ErrorResult("Tenant context required"));

        var exists = await _db.WorkflowDefinitions.AnyAsync(d => d.Id == id && d.TenantId == tenantId, ct);
        if (!exists)
            return NotFound(ApiResponseDto<object>.ErrorResult("Definition not found"));

        var running = await _db.WorkflowInstances
            .Where(i => i.WorkflowDefinitionId == id &&
                        i.TenantId == tenantId &&
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

    // Helpers
    private async Task<int?> RequireTenantAsync()
    {
        var t = await _tenantProvider.GetCurrentTenantIdAsync();
        if (t.HasValue) return t.Value;

        var claim = User.FindFirst("tenantId")?.Value;
        if (int.TryParse(claim, out var parsed)) return parsed;

        if (Request.Headers.TryGetValue("X-Tenant-ID", out var header) &&
            int.TryParse(header.FirstOrDefault(), out var headerVal))
            return headerVal;

        return null;
    }

    private WorkflowDefinitionDto Map(WorkflowService.Domain.Models.WorkflowDefinition def) => new()
    {
        Id = def.Id,
        Name = def.Name,
        Version = def.Version,
        JSONDefinition = def.JSONDefinition.EnrichEdgesForGateway(),
        IsPublished = def.IsPublished,
        Description = def.Description,
        PublishedAt = def.PublishedAt,
        PublishedByUserId = def.PublishedByUserId,
        CreatedAt = def.CreatedAt,
        UpdatedAt = def.UpdatedAt,
        IsArchived = def.IsArchived,
        ArchivedAt = def.ArchivedAt
    };
}
