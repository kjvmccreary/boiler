using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Services.Interfaces;
using System.Security.Claims;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using System.Text.Json;

namespace WorkflowService.Controllers;

/// <summary>
/// Controller for managing workflow instances
/// </summary>
[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class InstancesController : ControllerBase
{
    private readonly IInstanceService _instanceService; // ✅ CHANGE: Use service instead of direct DB access
    private readonly ILogger<InstancesController> _logger;

    // Added for suspend/resume endpoints
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;

    public InstancesController(
        IInstanceService instanceService, // ✅ CHANGE: Inject service
        ILogger<InstancesController> logger,
        WorkflowDbContext context,
        IMapper mapper)
    {
        _instanceService = instanceService; // ✅ CHANGE: Use service
        _logger = logger;
        _context = context;
        _mapper = mapper;
    }

    /// <summary>
    /// Get all workflow instances for the current tenant
    /// </summary>
    [HttpGet]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>>> GetInstances(
        [FromQuery] InstanceStatus? status = null,
        [FromQuery] int? workflowDefinitionId = null,
        [FromQuery] int? startedByUserId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        try
        {
            var request = new GetInstancesRequestDto
            {
                Status = status,
                WorkflowDefinitionId = workflowDefinitionId,
                StartedByUserId = startedByUserId,
                Page = page,
                PageSize = pageSize
            };

            var result = await _instanceService.GetAllAsync(request);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving workflow instances");
            return StatusCode(500, ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>.ErrorResult(
                "An error occurred while retrieving workflow instances"));
        }
    }

    /// <summary>
    /// Get all workflow instances for the current tenant (flat list)
    /// </summary>
    [HttpGet("flat")]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<PagedResultDto<WorkflowInstanceDto>>> GetInstancesFlat(
        [FromQuery] InstanceStatus? status = null,
        [FromQuery] int? workflowDefinitionId = null,
        [FromQuery] int? startedByUserId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
    {
        var request = new GetInstancesRequestDto
        {
            Status = status,
            WorkflowDefinitionId = workflowDefinitionId,
            StartedByUserId = startedByUserId,
            Page = page,
            PageSize = pageSize
        };

        var result = await _instanceService.GetAllAsync(request);
        if (!result.Success || result.Data == null)
            return StatusCode(500);

        return Ok(result.Data);
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
            var result = await _instanceService.GetByIdAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            
            return StatusCode(500, result);
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
            // ✅ FIX: Use the service layer which calls the workflow engine!
            var result = await _instanceService.StartInstanceAsync(request);
            
            if (result.Success)
            {
                _logger.LogInformation("Started workflow instance {Id} from definition {DefinitionId} by user {UserId}", 
                    result.Data?.Id, request.WorkflowDefinitionId, GetCurrentUserId());

                return CreatedAtAction(
                    nameof(GetInstance),
                    new { id = result.Data?.Id },
                    result);
            }
            
            if (result.Message?.Contains("not found") == true)
            {
                return BadRequest(result);
            }
            
            return StatusCode(500, result);
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
            // ✅ FIX: Use the service layer
            var result = await _instanceService.SignalAsync(id, request);
            
            if (result.Success)
            {
                _logger.LogInformation("Sent signal '{SignalName}' to workflow instance {Id} by user {UserId}", 
                    request.SignalName, id, GetCurrentUserId());
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
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
            var request = new TerminateInstanceRequestDto
            {
                Reason = "Manual termination"
            };

            // ✅ FIX: Use the service layer
            var result = await _instanceService.TerminateAsync(id, request);
            
            if (result.Success)
            {
                _logger.LogInformation("Terminated workflow instance {Id} by user {UserId}", 
                    id, GetCurrentUserId());
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating workflow instance {Id}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult(
                "An error occurred while terminating the workflow instance"));
        }
    }

    /// <summary>
    /// Get workflow instance execution history
    /// </summary>
    [HttpGet("{id}/history")]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<ApiResponseDto<List<WorkflowEventDto>>>> GetInstanceHistory(int id)
    {
        try
        {
            var result = await _instanceService.GetHistoryAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instance history {Id}", id);
            return StatusCode(500, ApiResponseDto<List<WorkflowEventDto>>.ErrorResult(
                "An error occurred while retrieving the workflow instance history"));
        }
    }

    /// <summary>
    /// Get workflow instance status
    /// </summary>
    [HttpGet("{id}/status")]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<ApiResponseDto<InstanceStatusDto>>> GetInstanceStatus(int id)
    {
        try
        {
            var result = await _instanceService.GetStatusAsync(id);
            
            if (result.Success)
            {
                return Ok(result);
            }
            
            if (result.Message?.Contains("not found") == true)
            {
                return NotFound(result);
            }
            
            return StatusCode(500, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instance status {Id}", id);
            return StatusCode(500, ApiResponseDto<InstanceStatusDto>.ErrorResult(
                "An error occurred while retrieving the workflow instance status"));
        }
    }

    /// <summary>
    /// Suspend a running workflow instance
    /// </summary>
    [HttpPost("{id}/suspend")]
    [RequiresPermission("workflow.manage_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> Suspend(int id)
    {
        var inst = await _context.WorkflowInstances.FirstOrDefaultAsync(i => i.Id == id);
        if (inst == null) return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Not found"));
        if (inst.Status != InstanceStatus.Running)
            return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Only running instances can be suspended"));
        inst.Status = InstanceStatus.Suspended;
        inst.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
            _mapper.Map<WorkflowInstanceDto>(inst), "Suspended"));
    }

    /// <summary>
    /// Resume a suspended workflow instance
    /// </summary>
    [HttpPost("{id}/resume")]
    [RequiresPermission("workflow.manage_instances")]
    public async Task<ActionResult<ApiResponseDto<WorkflowInstanceDto>>> Resume(int id)
    {
        var inst = await _context.WorkflowInstances.FirstOrDefaultAsync(i => i.Id == id);
        if (inst == null) return NotFound(ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Not found"));
        if (inst.Status != InstanceStatus.Suspended)
            return BadRequest(ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Only suspended instances can be resumed"));
        inst.Status = InstanceStatus.Running;
        inst.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return Ok(ApiResponseDto<WorkflowInstanceDto>.SuccessResult(
            _mapper.Map<WorkflowInstanceDto>(inst), "Resumed"));
    }

    /// <summary>
    /// Get a consolidated runtime snapshot (instance + definition JSON + tasks + events + derived path)
    /// </summary>
    [HttpGet("{id}/runtime-snapshot")]
    [RequiresPermission("workflow.view_instances")]
    public async Task<ActionResult<ApiResponseDto<InstanceRuntimeSnapshotDto>>> GetRuntimeSnapshot(int id)
    {
        try
        {
            // Load instance
            var instResult = await _instanceService.GetByIdAsync(id);
            if (!instResult.Success || instResult.Data == null)
                return NotFound(ApiResponseDto<InstanceRuntimeSnapshotDto>.ErrorResult("Instance not found"));

            var instanceEntity = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == id);

            if (instanceEntity == null)
                return NotFound(ApiResponseDto<InstanceRuntimeSnapshotDto>.ErrorResult("Instance not found"));

            // Definition JSON
            var defJson = instanceEntity.WorkflowDefinition.JSONDefinition;

            // Tasks (summary)
            var taskEntities = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                    .ThenInclude(i => i.WorkflowDefinition)
                .Where(t => t.WorkflowInstanceId == id)
                .ToListAsync();

            var taskSummaries = taskEntities
                .Select(t => new TaskSummaryDto
                {
                    Id = t.Id,
                    TaskName = t.TaskName,
                    Status = t.Status,
                    WorkflowDefinitionName = instanceEntity.WorkflowDefinition.Name,
                    WorkflowInstanceId = t.WorkflowInstanceId,
                    DueDate = t.DueDate,
                    CreatedAt = t.CreatedAt,
                    NodeId = t.NodeId
                })
                .ToList();

            // Events
            var events = await _context.WorkflowEvents
                .Where(e => e.WorkflowInstanceId == id)
                .OrderBy(e => e.OccurredAt)
                .Select(e => new WorkflowEventDto
                {
                    Id = e.Id,
                    WorkflowInstanceId = e.WorkflowInstanceId,
                    Type = e.Type,
                    Name = e.Name,
                    Data = e.Data,
                    OccurredAt = e.OccurredAt,
                    UserId = e.UserId,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();

            // Derive path details
            var traversedEdgeIds = new List<string>();
            var visitedNodeIds = new List<string>();

            foreach (var ev in events)
            {
                if (ev.Type == "Edge" && ev.Name == "EdgeTraversed")
                {
                    try
                    {
                        var json = JsonDocument.Parse(ev.Data);
                        if (json.RootElement.TryGetProperty("edgeId", out var edgeProp) &&
                            edgeProp.ValueKind == JsonValueKind.String)
                        {
                            traversedEdgeIds.Add(edgeProp.GetString()!);
                        }
                    }
                    catch { /* ignore malformed */ }
                }
                else if (ev.Type == "Node")
                {
                    try
                    {
                        var json = JsonDocument.Parse(ev.Data);
                        if (json.RootElement.TryGetProperty("nodeId", out var nodeProp) &&
                            nodeProp.ValueKind == JsonValueKind.String)
                        {
                            visitedNodeIds.Add(nodeProp.GetString()!);
                        }
                    }
                    catch { /* ignore */ }
                }
            }

            // Parse current node ids into array (JSON array OR csv)
            var currentNodeIdsParsed = new List<string>();
            var rawCurrent = instResult.Data.CurrentNodeIds ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(rawCurrent))
            {
                var trimmed = rawCurrent.Trim();
                if (trimmed.StartsWith("["))
                {
                    try
                    {
                        var arr = JsonSerializer.Deserialize<List<string>>(trimmed);
                        if (arr != null) currentNodeIdsParsed.AddRange(arr.Where(s => !string.IsNullOrWhiteSpace(s)));
                    }
                    catch { }
                }
                else
                {
                    currentNodeIdsParsed.AddRange(trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
                }
            }

            var snapshot = new InstanceRuntimeSnapshotDto
            {
                Instance = instResult.Data,
                DefinitionJson = defJson,
                Tasks = taskSummaries,
                Events = events,
                TraversedEdgeIds = traversedEdgeIds.Distinct().ToList(),
                VisitedNodeIds = visitedNodeIds.Distinct().ToList(),
                CurrentNodeIds = currentNodeIdsParsed
            };

            return Ok(ApiResponseDto<InstanceRuntimeSnapshotDto>.SuccessResult(snapshot, "Runtime snapshot retrieved"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error building runtime snapshot for instance {Id}", id);
            return StatusCode(500, ApiResponseDto<InstanceRuntimeSnapshotDto>.ErrorResult("Failed to build runtime snapshot"));
        }
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : 0;
    }
}
