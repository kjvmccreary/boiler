using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WorkflowService.Controllers;

/// <summary>
/// Controller for managing workflow instances
/// </summary>
[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class InstancesController : ControllerBase
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<InstancesController> _logger;

    public InstancesController(
        WorkflowDbContext context,
        ILogger<InstancesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get all workflow instances for the current tenant
    /// </summary>
    [HttpGet]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowInstanceDto>>>> GetInstances(
        [FromQuery] InstanceStatus? status = null,
        [FromQuery] int? workflowDefinitionId = null,
        [FromQuery] int? startedByUserId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var query = _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .AsQueryable();

            // Apply filters
            if (status.HasValue)
            {
                query = query.Where(i => i.Status == status.Value);
            }

            if (workflowDefinitionId.HasValue)
            {
                query = query.Where(i => i.WorkflowDefinitionId == workflowDefinitionId.Value);
            }

            if (startedByUserId.HasValue)
            {
                query = query.Where(i => i.StartedByUserId == startedByUserId.Value);
            }

            // Apply pagination
            var totalCount = await query.CountAsync();
            var instances = await query
                .OrderByDescending(i => i.StartedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(i => new WorkflowInstanceDto
                {
                    Id = i.Id,
                    WorkflowDefinitionId = i.WorkflowDefinitionId,
                    WorkflowDefinitionName = i.WorkflowDefinition.Name,
                    DefinitionVersion = i.DefinitionVersion,
                    Status = i.Status,
                    CurrentNodeIds = i.CurrentNodeIds,
                    Context = i.Context,
                    StartedAt = i.StartedAt,
                    CompletedAt = i.CompletedAt,
                    StartedByUserId = i.StartedByUserId,
                    ErrorMessage = i.ErrorMessage,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();

            return Ok(ApiResponseDto<List<WorkflowInstanceDto>>.SuccessResult(
                instances, 
                $"Retrieved {instances.Count} of {totalCount} workflow instances"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instances");
            return StatusCode(500, ApiResponseDto<List<WorkflowInstanceDto>>.ErrorResult(
                "An error occurred while retrieving workflow instances"));
        }
    }

    /// <summary>
    /// Get a specific workflow instance by ID
    /// </summary>
    [HttpGet("{id}")]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> GetInstance(int id)
    {
        try
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .Where(i => i.Id == id)
                .Select(i => new WorkflowInstanceDto
                {
                    Id = i.Id,
                    WorkflowDefinitionId = i.WorkflowDefinitionId,
                    WorkflowDefinitionName = i.WorkflowDefinition.Name,
                    DefinitionVersion = i.DefinitionVersion,
                    Status = i.Status,
                    CurrentNodeIds = i.CurrentNodeIds,
                    Context = i.Context,
                    StartedAt = i.StartedAt,
                    CompletedAt = i.CompletedAt,
                    StartedByUserId = i.StartedByUserId,
                    ErrorMessage = i.ErrorMessage,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (instance == null)
            {
                return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow instance not found"));
            }

            return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                instance, 
                "Workflow instance retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while retrieving the workflow instance"));
        }
    }

    /// <summary>
    /// Start a new workflow instance
    /// </summary>
    [HttpPost]
    [RequiresPermission("workflow.start_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> StartInstance(
        [FromBody] StartInstanceRequestDto request)
    {
        try
        {
            // Verify the workflow definition exists and is published
            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == request.WorkflowDefinitionId);

            if (definition == null)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow definition not found"));
            }

            if (!definition.IsPublished)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Cannot start instance from unpublished workflow definition"));
            }

            // TODO: Parse JSON definition to find start node
            // TODO: Validate initial context against definition
            
            var instance = new WorkflowInstance
            {
                WorkflowDefinitionId = request.WorkflowDefinitionId,
                DefinitionVersion = definition.Version,
                Status = InstanceStatus.Running,
                CurrentNodeIds = "[\"start\"]", // TODO: Get actual start node from definition
                Context = request.InitialContext ?? "{}",
                StartedAt = DateTime.UtcNow,
                StartedByUserId = GetCurrentUserId()
            };

            _context.WorkflowInstances.Add(instance);
            await _context.SaveChangesAsync();

            // TODO: Trigger workflow engine to process start node
            // TODO: Create initial workflow event

            var responseDto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                WorkflowDefinitionName = definition.Name,
                DefinitionVersion = instance.DefinitionVersion,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                Context = instance.Context,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                StartedByUserId = instance.StartedByUserId,
                ErrorMessage = instance.ErrorMessage,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt
            };

            _logger.LogInformation("Started workflow instance {Id} from definition {DefinitionId} by user {UserId}", 
                instance.Id, request.WorkflowDefinitionId, GetCurrentUserId());

            return CreatedAtAction(
                nameof(GetInstance),
                new { id = instance.Id },
                ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                    responseDto, 
                    "Workflow instance started successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting workflow instance from definition {DefinitionId}", 
                request.WorkflowDefinitionId);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while starting the workflow instance"));
        }
    }

    /// <summary>
    /// Send a signal to a workflow instance
    /// </summary>
    [HttpPost("{id}/signal")]
    [RequiresPermission("workflow.manage_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> SignalInstance(
        int id,
        [FromBody] SignalInstanceRequestDto request)
    {
        try
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow instance not found"));
            }

            if (instance.Status != InstanceStatus.Running && instance.Status != InstanceStatus.Suspended)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    $"Cannot signal instance in {instance.Status} status"));
            }

            // TODO: Process signal with workflow engine
            // TODO: Update instance state based on signal processing
            // TODO: Create workflow event for signal

            // Log the signal for now
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Signal",
                Name = request.SignalName,
                Data = request.SignalData ?? "{}",
                OccurredAt = DateTime.UtcNow,
                UserId = GetCurrentUserId()
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            var responseDto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                WorkflowDefinitionName = instance.WorkflowDefinition.Name,
                DefinitionVersion = instance.DefinitionVersion,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                Context = instance.Context,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                StartedByUserId = instance.StartedByUserId,
                ErrorMessage = instance.ErrorMessage,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt
            };

            _logger.LogInformation("Sent signal '{SignalName}' to workflow instance {Id} by user {UserId}", 
                request.SignalName, id, GetCurrentUserId());

            return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                responseDto, 
                $"Signal '{request.SignalName}' sent to workflow instance successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending signal to workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while sending signal to the workflow instance"));
        }
    }

    /// <summary>
    /// Terminate a workflow instance
    /// </summary>
    [HttpDelete("{id}")]
    [RequiresPermission("workflow.manage_instances")]
    public async Task<ActionResult<ApiResponseDto<bool>>> TerminateInstance(int id)
    {
        try
        {
            var instance = await _context.WorkflowInstances
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult(
                    "Workflow instance not found"));
            }

            if (instance.Status == InstanceStatus.Completed || instance.Status == InstanceStatus.Cancelled)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult(
                    $"Cannot terminate instance that is already {instance.Status}"));
            }

            // Update instance status
            instance.Status = InstanceStatus.Cancelled;
            instance.CompletedAt = DateTime.UtcNow;

            // TODO: Cancel any active tasks
            // TODO: Cleanup resources
            // TODO: Trigger workflow engine cleanup

            // Create termination event
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Instance",
                Name = "Terminated",
                Data = $"{{\"terminatedBy\": {GetCurrentUserId()}, \"reason\": \"Manual termination\"}}",
                OccurredAt = DateTime.UtcNow,
                UserId = GetCurrentUserId()
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Terminated workflow instance {Id} by user {UserId}", 
                id, GetCurrentUserId());

            return Ok(ApiResponseDto<bool>.SuccessResult(
                true, 
                "Workflow instance terminated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult(
                "An error occurred while terminating the workflow instance"));
        }
    }

    /// <summary>
    /// Suspend a workflow instance
    /// </summary>
    [HttpPost("{id}/suspend")]
    [RequiresPermission("workflow.manage_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> SuspendInstance(int id)
    {
        try
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow instance not found"));
            }

            if (instance.Status != InstanceStatus.Running)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    $"Cannot suspend instance in {instance.Status} status"));
            }

            instance.Status = InstanceStatus.Suspended;

            // Create suspension event
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Instance",
                Name = "Suspended",
                Data = $"{{\"suspendedBy\": {GetCurrentUserId()}}}",
                OccurredAt = DateTime.UtcNow,
                UserId = GetCurrentUserId()
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            var responseDto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                WorkflowDefinitionName = instance.WorkflowDefinition.Name,
                DefinitionVersion = instance.DefinitionVersion,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                Context = instance.Context,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                StartedByUserId = instance.StartedByUserId,
                ErrorMessage = instance.ErrorMessage,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt
            };

            _logger.LogInformation("Suspended workflow instance {Id} by user {UserId}", 
                id, GetCurrentUserId());

            return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                responseDto, 
                "Workflow instance suspended successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error suspending workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while suspending the workflow instance"));
        }
    }

    /// <summary>
    /// Resume a suspended workflow instance
    /// </summary>
    [HttpPost("{id}/resume")]
    [RequiresPermission("workflow.manage_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> ResumeInstance(int id)
    {
        try
        {
            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instance == null)
            {
                return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    "Workflow instance not found"));
            }

            if (instance.Status != InstanceStatus.Suspended)
            {
                return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                    $"Cannot resume instance in {instance.Status} status"));
            }

            instance.Status = InstanceStatus.Running;

            // TODO: Trigger workflow engine to continue processing

            // Create resumption event
            var workflowEvent = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Instance",
                Name = "Resumed",
                Data = $"{{\"resumedBy\": {GetCurrentUserId()}}}",
                OccurredAt = DateTime.UtcNow,
                UserId = GetCurrentUserId()
            };

            _context.WorkflowEvents.Add(workflowEvent);
            await _context.SaveChangesAsync();

            var responseDto = new WorkflowInstanceDto
            {
                Id = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                WorkflowDefinitionName = instance.WorkflowDefinition.Name,
                DefinitionVersion = instance.DefinitionVersion,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                Context = instance.Context,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                StartedByUserId = instance.StartedByUserId,
                ErrorMessage = instance.ErrorMessage,
                CreatedAt = instance.CreatedAt,
                UpdatedAt = instance.UpdatedAt
            };

            _logger.LogInformation("Resumed workflow instance {Id} by user {UserId}", 
                id, GetCurrentUserId());

            return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
                responseDto, 
                "Workflow instance resumed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resuming workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<WorkflowInstanceDto>.ErrorResult(
                "An error occurred while resuming the workflow instance"));
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
