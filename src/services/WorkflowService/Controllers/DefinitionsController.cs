using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WorkflowService.Controllers;

/// <summary>
/// Controller for managing workflow definitions (stores RAW builder JSON as-is).
/// </summary>
[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class DefinitionsController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<DefinitionsController> _logger;

    public DefinitionsController(
        WorkflowDbContext context,
        ILogger<DefinitionsController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all workflow definitions for the current tenant
    /// </summary>
    [HttpGet]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowDefinitionDto>>>> GetDefinitions(
        [FromQuery] bool? published = null)
    {
        try
        {
            var query = _context.WorkflowDefinitions.AsQueryable();
            if (published.HasValue)
                query = query.Where(d => d.IsPublished == published.Value);

            var definitions = await query
                .OrderByDescending(d => d.CreatedAt)
                .Select(d => new WorkflowDefinitionDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Version = d.Version,
                    JSONDefinition = d.JSONDefinition,
                    IsPublished = d.IsPublished,
                    Description = d.Description,
                    PublishedAt = d.PublishedAt,
                    PublishedByUserId = d.PublishedByUserId,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponseDto<List<WorkflowDefinitionDto>>.SuccessResult(
                definitions,
                "Workflow definitions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definitions");
            return StatusCode(500, ApiResponseDto<List<WorkflowDefinitionDto>>.ErrorResult(
                "An error occurred while retrieving workflow definitions"));
        }
    }

    /// <summary>
    /// Get a specific workflow definition by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> GetDefinition(int id)
    {
        try
        {
            var definition = await _context.WorkflowDefinitions
                .Where(d => d.Id == id)
                .Select(d => new WorkflowDefinitionDto
                {
                    Id = d.Id,
                    Name = d.Name,
                    Version = d.Version,
                    JSONDefinition = d.JSONDefinition,
                    IsPublished = d.IsPublished,
                    Description = d.Description,
                    PublishedAt = d.PublishedAt,
                    PublishedByUserId = d.PublishedByUserId,
                    CreatedAt = d.CreatedAt,
                    UpdatedAt = d.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (definition == null)
                return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found"));

            return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(
                definition,
                "Workflow definition retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow definition {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                "An error occurred while retrieving the workflow definition"));
        }
    }

    /// <summary>
    /// Create a new workflow definition draft (EF-based)
    /// </summary>
    [HttpPost("draft")]
    [RequiresPermission(Permissions.Workflow.CreateDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> CreateDraft(
        [FromBody] CreateWorkflowDefinitionDto request)
    {
        try
        {
            var nameExists = await _context.WorkflowDefinitions
                .AnyAsync(d => d.Name == request.Name);

            if (nameExists)
                return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "A workflow definition with this name already exists"));

            // Store raw builder JSON unmodified
            var definition = new WorkflowService.Domain.Models.WorkflowDefinition
            {
                Name = request.Name,
                Version = 1,
                JSONDefinition = request.JSONDefinition, // RAW
                IsPublished = false,
                Description = string.IsNullOrWhiteSpace(request.Description) ? null : request.Description,
                Tags = string.IsNullOrWhiteSpace(request.Tags) ? null : request.Tags
            };

            _context.WorkflowDefinitions.Add(definition);
            await _context.SaveChangesAsync();

            var dto = new WorkflowDefinitionDto
            {
                Id = definition.Id,
                Name = definition.Name,
                Version = definition.Version,
                JSONDefinition = definition.JSONDefinition,
                IsPublished = definition.IsPublished,
                Description = definition.Description,
                PublishedAt = definition.PublishedAt,
                PublishedByUserId = definition.PublishedByUserId,
                CreatedAt = definition.CreatedAt,
                UpdatedAt = definition.UpdatedAt
            };

            _logger.LogInformation("Created workflow definition draft {Name} (ID={Id})",
                definition.Name, definition.Id);

            return CreatedAtAction(nameof(GetDefinition),
                new { id = definition.Id },
                ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto,
                    "Workflow definition draft created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating workflow definition draft");
            return StatusCode(500, ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                "An error occurred while creating the workflow definition draft"));
        }
    }

    /// <summary>
    /// Update a workflow definition draft
    /// </summary>
    [HttpPut("{id}")]
    [RequiresPermission(Permissions.Workflow.EditDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> UpdateDefinition(
        int id,
        [FromBody] UpdateWorkflowDefinitionDto request)
    {
        try
        {
            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == id);

            if (definition == null)
                return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found"));

            if (definition.IsPublished)
                return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Cannot modify a published workflow definition"));

            if (!string.IsNullOrEmpty(request.Name))
                definition.Name = request.Name;

            if (!string.IsNullOrEmpty(request.JSONDefinition))
                definition.JSONDefinition = request.JSONDefinition; // RAW

            if (request.Description != null)
                definition.Description = request.Description;

            if (request.Tags != null)
                definition.Tags = string.IsNullOrWhiteSpace(request.Tags) ? null : request.Tags;

            await _context.SaveChangesAsync();

            var dto = new WorkflowDefinitionDto
            {
                Id = definition.Id,
                Name = definition.Name,
                Version = definition.Version,
                JSONDefinition = definition.JSONDefinition,
                IsPublished = definition.IsPublished,
                Description = definition.Description,
                PublishedAt = definition.PublishedAt,
                PublishedByUserId = definition.PublishedByUserId,
                CreatedAt = definition.CreatedAt,
                UpdatedAt = definition.UpdatedAt
            };

            _logger.LogInformation("Updated workflow definition {Id}", id);

            return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto,
                "Workflow definition updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating workflow definition {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                "An error occurred while updating the workflow definition"));
        }
    }

    /// <summary>
    /// Publish a workflow definition (makes it immutable)
    /// </summary>
    [HttpPost("{id}/publish")]
    [RequiresPermission(Permissions.Workflow.PublishDefinitions)]
    public async Task<ActionResult<ApiResponseDto<WorkflowDefinitionDto>>> PublishDefinition(
        int id,
        [FromBody] PublishDefinitionRequestDto request)
    {
        try
        {
            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == id);

            if (definition == null)
                return NotFound(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult("Workflow definition not found"));

            if (definition.IsPublished)
                return BadRequest(ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                    "Workflow definition is already published"));

            // Optionally validate (light)
            // (Intentionally tolerant – engine will enforce at start time)
            definition.IsPublished = true;
            definition.PublishedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var dto = new WorkflowDefinitionDto
            {
                Id = definition.Id,
                Name = definition.Name,
                Version = definition.Version,
                JSONDefinition = definition.JSONDefinition,
                IsPublished = definition.IsPublished,
                Description = definition.Description,
                PublishedAt = definition.PublishedAt,
                PublishedByUserId = definition.PublishedByUserId,
                CreatedAt = definition.CreatedAt,
                UpdatedAt = definition.UpdatedAt
            };

            _logger.LogInformation("Published workflow definition {Id}", id);

            return Ok(ApiResponseDto<WorkflowDefinitionDto>.SuccessResult(dto,
                "Workflow definition published successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error publishing workflow definition {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowDefinitionDto>.ErrorResult(
                "An error occurred while publishing the workflow definition"));
        }
    }

    /// <summary>
    /// Delete a workflow definition (only if not published and no instances)
    /// </summary>
    [HttpDelete("{id}")]
    [RequiresPermission(Permissions.Workflow.DeleteDefinitions)]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteDefinition(int id)
    {
        try
        {
            var definition = await _context.WorkflowDefinitions
                .Include(d => d.Instances)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (definition == null)
                return NotFound(ApiResponseDto<bool>.ErrorResult("Workflow definition not found"));

            if (definition.IsPublished)
                return BadRequest(ApiResponseDto<bool>.ErrorResult(
                    "Cannot delete a published workflow definition"));

            if (definition.Instances.Any())
                return BadRequest(ApiResponseDto<bool>.ErrorResult(
                    "Cannot delete a workflow definition that has instances"));

            _context.WorkflowDefinitions.Remove(definition);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted workflow definition {Id}", id);

            return Ok(ApiResponseDto<bool>.SuccessResult(true,
                "Workflow definition deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting workflow definition {Id}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult(
                "An error occurred while deleting the workflow definition"));
        }
    }
}
