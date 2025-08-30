using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Persistence;
using WorkflowService.Security;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/definitions")]
public class DefinitionsController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<DefinitionsController> _logger;

    public DefinitionsController(WorkflowDbContext context, ILogger<DefinitionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // -------------------------
    // PRIMARY ENDPOINTS
    // -------------------------

    // GET (list)
    [HttpGet]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowDefinitionDto>>>> GetAll()
    {
        try
        {
            var defs = await _context.WorkflowDefinitions
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ToListAsync();

            var list = defs
                .Select(d => new WorkflowDefinitionDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Version = d.Version,
                    JSONDefinition = d.JSONDefinition.EnrichEdgesForGateway(),
                    IsPublished = d.IsPublished,
                    Description = d.Description,
                    PublishedAt = d.PublishedAt,
                    PublishedByUserId = d.PublishedByUserId,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToList();

            return Ok(ApiResponseDto<List<WorkflowDefinitionDto>>.SuccessResult(list));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definitions");
            return StatusCode(500,
                ApiResponseDto<List<WorkflowDefinitionDto>>.ErrorResult("Failed to retrieve workflow definitions"));
        }
    }

    // GET (single)
    [HttpGet("{id:int}")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> Get(int id)
    {
        try
        {
            var def = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == id);

            if (def == null)
                return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found"));

            var dto = new WorkflowDefinitionDto
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
                UpdatedAt = def.UpdatedAt
            };

            return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definition {Id}", id);
            return StatusCode(500,
                ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to retrieve workflow definition"));
        }
    }

    // POST: draft
    [HttpPost("draft")]
    [RequiresPermission(Permissions.Workflow.CreateDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> CreateDraft(
        [FromBody] CreateWorkflowDefinitionDto request)
    {
        try
        {
            if (await _context.WorkflowDefinitions.AnyAsync(d => d.Name == request.Name))
                return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition name already exists"));

            var enrichedJson = request.JSONDefinition.EnrichEdgesForGateway();

            var entity = new Domain.Models.WorkflowDefinition
            {
                Name = request.Name,
                Version = 1,
                JSONDefinition = enrichedJson,
                IsPublished = false,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
                Tags = string.IsNullOrWhiteSpace(request.Tags) ? null : request.Tags,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.WorkflowDefinitions.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new WorkflowDefinitionDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Version = entity.Version,
                JSONDefinition = entity.JSONDefinition,
                IsPublished = entity.IsPublished,
                Description = entity.Description,
                PublishedAt = entity.PublishedAt,
                PublishedByUserId = entity.PublishedByUserId,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };

            _logger.LogInformation("Created workflow definition draft {Name} (ID={Id})", entity.Name, entity.Id);
            return CreatedAtAction(nameof(Get), new { id = entity.Id },
                ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Draft created"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow definition draft");
            return StatusCode(500,
                ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to create draft"));
        }
    }

    // PUT: update draft
    [HttpPut("{id:int}")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> UpdateDraft(
        int id,
        [FromBody] UpdateWorkflowDefinitionDto request)
    {
        try
        {
            var def = await _context.WorkflowDefinitions.FirstOrDefaultAsync(d => d.Id == id);
            if (def == null)
                return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Definition not found"));
            if (def.IsPublished)
                return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Cannot edit published definition"));

            if (!string.IsNullOrWhiteSpace(request.Name))
                def.Name = request.Name;
            if (request.Description != null)
                def.Description = request.Description;
            if (request.Tags != null)
                def.Tags = request.Tags;

            if (!string.IsNullOrWhiteSpace(request.JSONDefinition))
                def.JSONDefinition = request.JSONDefinition.EnrichEdgesForGateway();
            else
                def.JSONDefinition = def.JSONDefinition.EnrichEdgesForGateway();

            def.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var dto = new WorkflowDefinitionDto
            {
                Id = def.Id,
                Name = def.Name,
                Version = def.Version,
                JSONDefinition = def.JSONDefinition,
                IsPublished = def.IsPublished,
                Description = def.Description,
                PublishedAt = def.PublishedAt,
                PublishedByUserId = def.PublishedByUserId,
                CreatedAt = def.CreatedAt,
                UpdatedAt = def.UpdatedAt
            };
            return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto, "Draft updated"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workflow definition draft {Id}", id);
            return StatusCode(500,
                ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Failed to update draft"));
        }
    }

    // -------------------------
    // LEGACY TEST ALIASES
    // -------------------------
    // Old tests used: GetDefinitions(published: bool?)
    [NonAction]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowDefinitionDto>>>> GetDefinitions(bool? published = null)
    {
        var result = await GetAll();
        if (result.Result is ObjectResult or OkObjectResult)
        {
            var payload = (result.Result as ObjectResult)?.Value as ApiResponseDto<List<WorkflowDefinitionDto>>
                          ?? result.Value;
            if (payload?.Data != null && published.HasValue)
            {
                payload.Data = payload.Data
                    .Where(d => d.IsPublished == published.Value)
                    .ToList();
            }
            return payload == null
                ? ApiResponseDto<List<WorkflowDefinitionDto>>.ErrorResult("Unexpected null payload")
                : ApiResponseDto<List<WorkflowDefinitionDto>>.SuccessResult(payload.Data);
        }
        return result;
    }

    [NonAction]
    public Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> GetDefinition(int id) => Get(id);
}
