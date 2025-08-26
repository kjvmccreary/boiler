using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Services;

public class InstanceService : IInstanceService
{
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<InstanceService> _logger;

    public InstanceService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IWorkflowRuntime workflowRuntime,
        IEventPublisher eventPublisher,
        ILogger<InstanceService> logger)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _workflowRuntime = workflowRuntime;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ApiResponseDto<WorkflowInstanceDto>> StartInstanceAsync(StartInstanceRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Tenant context required");
            }

            // Verify workflow definition exists and is published
            var definition = await _context.WorkflowDefinitions
                .FirstOrDefaultAsync(d => d.Id == request.WorkflowDefinitionId && 
                                         d.TenantId == tenantId.Value && 
                                         d.IsPublished, cancellationToken);

            if (definition == null)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Published workflow definition not found");
            }

            // Get current user ID for audit trail
            var currentUserId = GetCurrentUserId();

            // Start workflow instance using the engine
            var instance = await _workflowRuntime.StartWorkflowAsync(
                request.WorkflowDefinitionId, 
                request.InitialContext ?? "{}", 
                currentUserId, 
                cancellationToken);

            // Publish event
            await _eventPublisher.PublishInstanceStartedAsync(instance, cancellationToken);

            var dto = _mapper.Map<WorkflowInstanceDto>(instance);

            _logger.LogInformation("Started workflow instance {InstanceId} from definition {DefinitionId} for tenant {TenantId}", 
                instance.Id, request.WorkflowDefinitionId, tenantId);

            return ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto, "Workflow instance started successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting workflow instance for definition {DefinitionId}", request.WorkflowDefinitionId);
            return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Failed to start workflow instance");
        }
    }

    public async Task<ApiResponseDto<WorkflowInstanceDto>> GetByIdAsync(int instanceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Tenant context required");
            }

            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .FirstOrDefaultAsync(i => i.Id == instanceId && i.TenantId == tenantId.Value, cancellationToken);

            if (instance == null)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Workflow instance not found");
            }

            var dto = _mapper.Map<WorkflowInstanceDto>(instance);

            return ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instance {InstanceId}", instanceId);
            return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Failed to retrieve workflow instance");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>> GetAllAsync(GetInstancesRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>.ErrorResult("Tenant context required");
            }

            var query = _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .Where(i => i.TenantId == tenantId.Value);

            // Apply filters
            if (request.WorkflowDefinitionId.HasValue)
            {
                query = query.Where(i => i.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);
            }

            if (request.Status.HasValue)
            {
                query = query.Where(i => i.Status == request.Status.Value);
            }

            if (request.StartedAfter.HasValue)
            {
                query = query.Where(i => i.StartedAt >= request.StartedAfter.Value);
            }

            if (request.StartedBefore.HasValue)
            {
                query = query.Where(i => i.StartedAt <= request.StartedBefore.Value);
            }

            if (request.StartedByUserId.HasValue)
            {
                query = query.Where(i => i.StartedByUserId == request.StartedByUserId.Value);
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                query = query.Where(i => i.WorkflowDefinition.Name.Contains(request.SearchTerm) ||
                                        (i.ErrorMessage != null && i.ErrorMessage.Contains(request.SearchTerm)));
            }

            // Apply sorting
            query = request.SortBy.ToLower() switch
            {
                "completedat" => request.SortDescending ? query.OrderByDescending(i => i.CompletedAt) : query.OrderBy(i => i.CompletedAt),
                "status" => request.SortDescending ? query.OrderByDescending(i => i.Status) : query.OrderBy(i => i.Status),
                "definitionname" => request.SortDescending ? query.OrderByDescending(i => i.WorkflowDefinition.Name) : query.OrderBy(i => i.WorkflowDefinition.Name),
                _ => request.SortDescending ? query.OrderByDescending(i => i.StartedAt) : query.OrderBy(i => i.StartedAt)
            };

            var totalCount = await query.CountAsync(cancellationToken);
            
            var instances = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<WorkflowInstanceDto>>(instances);

            var pagedResult = new PagedResultDto<WorkflowInstanceDto>
            {
                Items = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
            };

            return ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instances");
            return ApiResponseDto<PagedResultDto<WorkflowInstanceDto>>.ErrorResult("Failed to retrieve workflow instances");
        }
    }

    public async Task<ApiResponseDto<WorkflowInstanceDto>> SignalAsync(int instanceId, SignalInstanceRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Tenant context required");
            }

            var instance = await _context.WorkflowInstances
                .FirstOrDefaultAsync(i => i.Id == instanceId && i.TenantId == tenantId.Value, cancellationToken);

            if (instance == null)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Workflow instance not found");
            }

            if (instance.Status != DTOs.Workflow.Enums.InstanceStatus.Running)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Cannot signal non-running workflow instance");
            }

            var currentUserId = GetCurrentUserId();

            // Signal the workflow instance
            await _workflowRuntime.SignalWorkflowAsync(
                instanceId, 
                request.SignalName, 
                request.SignalData ?? "{}", 
                currentUserId, 
                cancellationToken);

            // Reload instance to get updated state
            await _context.Entry(instance).ReloadAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowInstanceDto>(instance);

            _logger.LogInformation("Signaled workflow instance {InstanceId} with signal {SignalName}", 
                instanceId, request.SignalName);

            return ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto, "Workflow instance signaled successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error signaling workflow instance {InstanceId}", instanceId);
            return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Failed to signal workflow instance");
        }
    }

    public async Task<ApiResponseDto<bool>> TerminateAsync(int instanceId, TerminateInstanceRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant context required");
            }

            var instance = await _context.WorkflowInstances
                .FirstOrDefaultAsync(i => i.Id == instanceId && i.TenantId == tenantId.Value, cancellationToken);

            if (instance == null)
            {
                return ApiResponseDto<bool>.ErrorResult("Workflow instance not found");
            }

            var currentUserId = GetCurrentUserId();

            // Terminate the workflow instance
            await _workflowRuntime.CancelWorkflowAsync(instanceId, request.Reason, currentUserId, cancellationToken);

            _logger.LogInformation("Terminated workflow instance {InstanceId}: {Reason}", instanceId, request.Reason);

            return ApiResponseDto<bool>.SuccessResult(true, "Workflow instance terminated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error terminating workflow instance {InstanceId}", instanceId);
            return ApiResponseDto<bool>.ErrorResult("Failed to terminate workflow instance");
        }
    }

    public async Task<ApiResponseDto<List<WorkflowEventDto>>> GetHistoryAsync(int instanceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<List<WorkflowEventDto>>.ErrorResult("Tenant context required");
            }

            // Verify instance exists and belongs to tenant
            var instanceExists = await _context.WorkflowInstances
                .AnyAsync(i => i.Id == instanceId && i.TenantId == tenantId.Value, cancellationToken);

            if (!instanceExists)
            {
                return ApiResponseDto<List<WorkflowEventDto>>.ErrorResult("Workflow instance not found");
            }

            var events = await _context.WorkflowEvents
                .Where(e => e.WorkflowInstanceId == instanceId)
                .OrderBy(e => e.OccurredAt)
                .ToListAsync(cancellationToken);

            var dtos = _mapper.Map<List<WorkflowEventDto>>(events);

            return ApiResponseDto<List<WorkflowEventDto>>.SuccessResult(dtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instance history {InstanceId}", instanceId);
            return ApiResponseDto<List<WorkflowEventDto>>.ErrorResult("Failed to retrieve workflow instance history");
        }
    }

    public async Task<ApiResponseDto<InstanceStatusDto>> GetStatusAsync(int instanceId, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<InstanceStatusDto>.ErrorResult("Tenant context required");
            }

            var instance = await _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .Include(i => i.Tasks)
                .FirstOrDefaultAsync(i => i.Id == instanceId && i.TenantId == tenantId.Value, cancellationToken);

            if (instance == null)
            {
                return ApiResponseDto<InstanceStatusDto>.ErrorResult("Workflow instance not found");
            }

            // Calculate progress and other metrics
            var activeTasksCount = instance.Tasks.Count(t => 
                t.Status == WorkflowTaskStatus.Created ||
                t.Status == WorkflowTaskStatus.Assigned ||
                t.Status == WorkflowTaskStatus.Claimed ||
                t.Status == WorkflowTaskStatus.InProgress);

            var runtime = instance.CompletedAt.HasValue 
                ? instance.CompletedAt.Value - instance.StartedAt
                : DateTime.UtcNow - instance.StartedAt;

            // Simple progress calculation (could be enhanced with actual workflow analysis)
            var progressPercentage = instance.Status switch
            {
                DTOs.Workflow.Enums.InstanceStatus.Completed => 100.0,
                DTOs.Workflow.Enums.InstanceStatus.Failed => 0.0,
                DTOs.Workflow.Enums.InstanceStatus.Cancelled => 0.0,
                _ => CalculateProgress(instance)
            };

            var statusDto = new InstanceStatusDto
            {
                InstanceId = instanceId,
                Status = instance.Status,
                CurrentNodeIds = instance.CurrentNodeIds,
                CurrentNodeNames = ExtractNodeNames(instance),
                ProgressPercentage = progressPercentage,
                LastUpdated = instance.UpdatedAt,
                Runtime = runtime,
                ActiveTasksCount = activeTasksCount,
                ErrorMessage = instance.ErrorMessage
            };

            return ApiResponseDto<InstanceStatusDto>.SuccessResult(statusDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow instance status {InstanceId}", instanceId);
            return ApiResponseDto<InstanceStatusDto>.ErrorResult("Failed to retrieve workflow instance status");
        }
    }

    private int? GetCurrentUserId()
    {
        // This would typically extract from HttpContext or JWT claims
        // For now, return null and let the system handle it
        return null;
    }

    private double CalculateProgress(WorkflowInstance instance)
    {
        // Simple progress calculation - could be enhanced with actual workflow analysis
        // For now, just return based on runtime (max 80% for running workflows)
        var runtime = DateTime.UtcNow - instance.StartedAt;
        var estimatedTotalTime = TimeSpan.FromHours(1); // Default estimate
        
        var progress = Math.Min(80.0, (runtime.TotalMinutes / estimatedTotalTime.TotalMinutes) * 80.0);
        return Math.Max(5.0, progress); // Minimum 5% for running workflows
    }

    private List<string> ExtractNodeNames(WorkflowInstance instance)
    {
        try
        {
            // Parse current node IDs and map to names from workflow definition
            // This is a simplified implementation
            var nodeIds = System.Text.Json.JsonSerializer.Deserialize<string[]>(instance.CurrentNodeIds) ?? Array.Empty<string>();
            return nodeIds.ToList(); // In a real implementation, you'd map IDs to names
        }
        catch
        {
            return new List<string>();
        }
    }
}
