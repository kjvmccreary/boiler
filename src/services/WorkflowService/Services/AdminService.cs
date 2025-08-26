using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus; // Added alias for clarity

namespace WorkflowService.Services;

public class AdminService : IAdminService
{
    private readonly WorkflowDbContext _context;
    private readonly IMapper _mapper;
    private readonly ITenantProvider _tenantProvider;
    private readonly IWorkflowRuntime _workflowRuntime;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        WorkflowDbContext context,
        IMapper mapper,
        ITenantProvider tenantProvider,
        IWorkflowRuntime workflowRuntime,
        IEventPublisher eventPublisher,
        ILogger<AdminService> logger)
    {
        _context = context;
        _mapper = mapper;
        _tenantProvider = tenantProvider;
        _workflowRuntime = workflowRuntime;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    public async Task<ApiResponseDto<WorkflowInstanceDto>> RetryInstanceAsync(int instanceId, RetryInstanceRequestDto request, CancellationToken cancellationToken = default)
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

            if (instance.Status != InstanceStatus.Failed)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Only failed workflow instances can be retried");
            }

            // Retry the workflow instance
            await _workflowRuntime.RetryWorkflowAsync(instanceId, request.ResetToNodeId, cancellationToken);

            // Reload instance to get updated state
            await _context.Entry(instance).ReloadAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowInstanceDto>(instance);

            _logger.LogInformation("Retried workflow instance {InstanceId}", instanceId);

            return ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto, "Workflow instance retried successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrying workflow instance {InstanceId}", instanceId);
            return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Failed to retry workflow instance");
        }
    }

    public async Task<ApiResponseDto<WorkflowInstanceDto>> MoveToNodeAsync(int instanceId, MoveToNodeRequestDto request, CancellationToken cancellationToken = default)
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

            // Validate target node exists in workflow definition
            var workflowDef = WorkflowService.Domain.Dsl.WorkflowDefinitionJson.FromJson(instance.WorkflowDefinition.JSONDefinition);
            var targetNode = workflowDef.Nodes.FirstOrDefault(n => n.Id == request.TargetNodeId);

            if (targetNode == null)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Target node not found in workflow definition");
            }

            // Update instance state
            instance.CurrentNodeIds = System.Text.Json.JsonSerializer.Serialize(new[] { request.TargetNodeId });
            instance.Status = InstanceStatus.Running;
            instance.ErrorMessage = null;
            instance.UpdatedAt = DateTime.UtcNow;

            // Update context if provided
            if (!string.IsNullOrEmpty(request.ContextUpdates))
            {
                try
                {
                    var currentContext = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(instance.Context) ?? new();
                    var updates = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, object>>(request.ContextUpdates) ?? new();
                    
                    foreach (var update in updates)
                    {
                        currentContext[update.Key] = update.Value;
                    }
                    
                    instance.Context = System.Text.Json.JsonSerializer.Serialize(currentContext);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to update context for instance {InstanceId}", instanceId);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Continue workflow from new position
            await _workflowRuntime.ContinueWorkflowAsync(instanceId, cancellationToken);

            // Reload to get final state
            await _context.Entry(instance).ReloadAsync(cancellationToken);

            var dto = _mapper.Map<WorkflowInstanceDto>(instance);

            _logger.LogInformation("Moved workflow instance {InstanceId} to node {NodeId}: {Reason}", 
                instanceId, request.TargetNodeId, request.Reason);

            return ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto, "Workflow instance moved successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error moving workflow instance {InstanceId} to node {NodeId}", instanceId, request.TargetNodeId);
            return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Failed to move workflow instance");
        }
    }

    public async Task<ApiResponseDto<WorkflowInstanceDto>> ForceCompleteAsync(int instanceId, ForceCompleteRequestDto request, CancellationToken cancellationToken = default)
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

            if (instance.Status == InstanceStatus.Completed)
            {
                return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Workflow instance is already completed");
            }

            // Force complete the instance
            instance.Status = InstanceStatus.Completed;
            instance.CompletedAt = DateTime.UtcNow;
            instance.UpdatedAt = DateTime.UtcNow;
            instance.CurrentNodeIds = System.Text.Json.JsonSerializer.Serialize(Array.Empty<string>());

            // Update context if provided
            if (!string.IsNullOrEmpty(request.FinalContext))
            {
                instance.Context = request.FinalContext;
            }

            // Cancel any active tasks
            var activeTasks = await _context.WorkflowTasks
                .Where(t => t.WorkflowInstanceId == instanceId && 
                           (t.Status == WorkflowTaskStatus.Created || 
                            t.Status == WorkflowTaskStatus.Assigned || 
                            t.Status == WorkflowTaskStatus.Claimed ||
                            t.Status == WorkflowTaskStatus.InProgress))
                .ToListAsync(cancellationToken);

            foreach (var task in activeTasks)
            {
                task.Status = WorkflowTaskStatus.Cancelled;
                task.CompletedAt = DateTime.UtcNow;
                task.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync(cancellationToken);

            // Publish completion event
            await _eventPublisher.PublishInstanceCompletedAsync(instance, cancellationToken);

            var dto = _mapper.Map<WorkflowInstanceDto>(instance);

            _logger.LogInformation("Force completed workflow instance {InstanceId}: {Reason}", instanceId, request.Reason);

            return ApiResponseDto<WorkflowInstanceDto>.SuccessResult(dto, "Workflow instance force completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error force completing workflow instance {InstanceId}", instanceId);
            return ApiResponseDto<WorkflowInstanceDto>.ErrorResult("Failed to force complete workflow instance");
        }
    }

    public async Task<ApiResponseDto<WorkflowAnalyticsDto>> GetAnalyticsAsync(GetAnalyticsRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<WorkflowAnalyticsDto>.ErrorResult("Tenant context required");
            }

            var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
            var endDate = request.EndDate ?? DateTime.UtcNow;

            var query = _context.WorkflowInstances
                .Include(i => i.WorkflowDefinition)
                .Where(i => i.TenantId == tenantId.Value && 
                           i.StartedAt >= startDate && i.StartedAt <= endDate);

            if (request.WorkflowDefinitionId.HasValue)
            {
                query = query.Where(i => i.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);
            }

            var instances = await query.ToListAsync(cancellationToken);
            var completedInstances = instances.Where(i => i.Status == InstanceStatus.Completed).ToList();

            var averageCompletionTime = completedInstances.Any() 
                ? completedInstances.Where(i => i.CompletedAt.HasValue)
                    .Average(i => (i.CompletedAt!.Value - i.StartedAt).TotalHours)
                : 0;

            var successRate = instances.Any() 
                ? (double)completedInstances.Count / instances.Count * 100 
                : 0;

            var analytics = new WorkflowAnalyticsDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalInstances = instances.Count,
                CompletedInstances = completedInstances.Count,
                FailedInstances = instances.Count(i => i.Status == InstanceStatus.Failed),
                RunningInstances = instances.Count(i => i.Status == InstanceStatus.Running),
                AverageCompletionTime = averageCompletionTime,
                SuccessRate = successRate,
                InstancesByStatus = instances.GroupBy(i => i.Status.ToString()).ToDictionary(g => g.Key, g => g.Count()),
                InstancesByDefinition = instances.GroupBy(i => i.WorkflowDefinition.Name).ToDictionary(g => g.Key, g => g.Count()),
                InstancesByDate = GetInstancesByDate(instances, request.GroupBy ?? "day"),
                TopBottlenecks = await GetTopBottlenecks(tenantId.Value, startDate, endDate, cancellationToken)
            };

            return ApiResponseDto<WorkflowAnalyticsDto>.SuccessResult(analytics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow analytics");
            return ApiResponseDto<WorkflowAnalyticsDto>.ErrorResult("Failed to retrieve workflow analytics");
        }
    }

    public async Task<ApiResponseDto<WorkflowSystemHealthDto>> GetSystemHealthAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<WorkflowSystemHealthDto>.ErrorResult("Tenant context required");
            }

            var activeInstances = await _context.WorkflowInstances
                .CountAsync(i => i.TenantId == tenantId.Value && i.Status == InstanceStatus.Running, cancellationToken);

            var pendingTasks = await _context.WorkflowTasks
                .Include(t => t.WorkflowInstance)
                .CountAsync(t => t.WorkflowInstance.TenantId == tenantId.Value && 
                                (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned), cancellationToken);

            var recentErrors = await _context.WorkflowInstances
                .Where(i => i.TenantId == tenantId.Value && 
                           i.Status == InstanceStatus.Failed && 
                           i.UpdatedAt >= DateTime.UtcNow.AddHours(-1))
                .CountAsync(cancellationToken);

            var issues = new List<string>();
            var status = "Healthy";

            if (recentErrors > 10)
            {
                issues.Add($"High error rate: {recentErrors} failed instances in the last hour");
                status = "Degraded";
            }

            if (pendingTasks > 1000)
            {
                issues.Add($"High task backlog: {pendingTasks} pending tasks");
                status = "Degraded";
            }

            if (recentErrors > 50)
            {
                status = "Unhealthy";
            }

            var health = new WorkflowSystemHealthDto
            {
                Status = status,
                CheckedAt = DateTime.UtcNow,
                ActiveInstances = activeInstances,
                PendingTasks = pendingTasks,
                BackgroundWorkerStatus = 1, // Assume running for now
                SystemMetrics = new Dictionary<string, object>
                {
                    ["recent_errors"] = recentErrors,
                    ["total_active_instances"] = activeInstances,
                    ["total_pending_tasks"] = pendingTasks,
                    ["last_check"] = DateTime.UtcNow
                },
                Issues = issues
            };

            return ApiResponseDto<WorkflowSystemHealthDto>.SuccessResult(health);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow system health");
            return ApiResponseDto<WorkflowSystemHealthDto>.ErrorResult("Failed to retrieve system health");
        }
    }

    public async Task<ApiResponseDto<BulkOperationResultDto>> BulkOperationAsync(BulkInstanceOperationRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<BulkOperationResultDto>.ErrorResult("Tenant context required");
            }

            var query = _context.WorkflowInstances
                .Where(i => i.TenantId == tenantId.Value);

            // Apply selection criteria
            if (request.InstanceIds?.Any() == true)
            {
                query = query.Where(i => request.InstanceIds.Contains(i.Id));
            }
            else
            {
                if (request.WorkflowDefinitionId.HasValue)
                {
                    query = query.Where(i => i.WorkflowDefinitionId == request.WorkflowDefinitionId.Value);
                }

                if (request.Status.HasValue)
                {
                    query = query.Where(i => i.Status == request.Status.Value);
                }

                if (request.StartedBefore.HasValue)
                {
                    query = query.Where(i => i.StartedAt <= request.StartedBefore.Value);
                }
            }

            var instances = await query.ToListAsync(cancellationToken);
            var successCount = 0;
            var failureCount = 0;

            foreach (var instance in instances)
            {
                try
                {
                    switch (request.Operation.ToLower())
                    {
                        case "cancel":
                            if (instance.Status == InstanceStatus.Running)
                            {
                                await _workflowRuntime.CancelWorkflowAsync(instance.Id, request.Reason, null, cancellationToken);
                                successCount++;
                            }
                            break;

                        case "retry":
                            if (instance.Status == InstanceStatus.Failed)
                            {
                                await _workflowRuntime.RetryWorkflowAsync(instance.Id, null, cancellationToken);
                                successCount++;
                            }
                            break;

                        case "terminate":
                            if (instance.Status != InstanceStatus.Completed)
                            {
                                await _workflowRuntime.CancelWorkflowAsync(instance.Id, request.Reason, null, cancellationToken);
                                successCount++;
                            }
                            break;

                        default:
                            failureCount++;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to apply bulk operation {Operation} to instance {InstanceId}", 
                        request.Operation, instance.Id);
                    failureCount++;
                }
            }

            var result = new BulkOperationResultDto
            {
                SuccessCount = successCount,
                FailureCount = failureCount,
                TotalCount = instances.Count,
                OperationType = request.Operation
            };

            _logger.LogInformation("Bulk operation {Operation} completed: {Success} succeeded, {Failed} failed", 
                request.Operation, successCount, failureCount);

            return ApiResponseDto<BulkOperationResultDto>.SuccessResult(result, 
                $"Bulk operation completed: {successCount} succeeded, {failureCount} failed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error performing bulk operation {Operation}", request.Operation);
            return ApiResponseDto<BulkOperationResultDto>.ErrorResult("Failed to perform bulk operation");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<WorkflowAuditEntryDto>>> GetAuditTrailAsync(GetAuditTrailRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return ApiResponseDto<PagedResultDto<WorkflowAuditEntryDto>>.ErrorResult("Tenant context required");
            }

            var query = _context.WorkflowEvents
                .Include(e => e.WorkflowInstance)
                .Where(e => e.WorkflowInstance.TenantId == tenantId.Value);

            // Apply filters
            if (request.InstanceId.HasValue)
            {
                query = query.Where(e => e.WorkflowInstanceId == request.InstanceId.Value);
            }

            if (request.UserId.HasValue)
            {
                query = query.Where(e => e.UserId == request.UserId.Value);
            }

            if (!string.IsNullOrEmpty(request.Action))
            {
                query = query.Where(e => e.Type.Contains(request.Action) || e.Name.Contains(request.Action));
            }

            if (request.StartDate.HasValue)
            {
                query = query.Where(e => e.OccurredAt >= request.StartDate.Value);
            }

            if (request.EndDate.HasValue)
            {
                query = query.Where(e => e.OccurredAt <= request.EndDate.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            
            var events = await query
                .OrderByDescending(e => e.OccurredAt)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new WorkflowAuditEntryDto
                {
                    Id = e.Id,
                    InstanceId = e.WorkflowInstanceId,
                    UserId = e.UserId,
                    Action = $"{e.Type}.{e.Name}",
                    Details = e.Data,
                    Timestamp = e.OccurredAt,
                    IpAddress = null, // Would need to be stored in events
                    UserAgent = null  // Would need to be stored in events
                })
                .ToListAsync(cancellationToken);

            var pagedResult = new PagedResultDto<WorkflowAuditEntryDto>
            {
                Items = events,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize
                //TotalPages = (int)Math.Ceiling((double)totalCount / request.PageSize)
            };

            return ApiResponseDto<PagedResultDto<WorkflowAuditEntryDto>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting workflow audit trail");
            return ApiResponseDto<PagedResultDto<WorkflowAuditEntryDto>>.ErrorResult("Failed to retrieve audit trail");
        }
    }

    private Dictionary<DateTime, int> GetInstancesByDate(List<WorkflowInstance> instances, string groupBy)
    {
        return groupBy.ToLower() switch
        {
            "week" => instances.GroupBy(i => i.StartedAt.Date.AddDays(-(int)i.StartedAt.DayOfWeek))
                              .ToDictionary(g => g.Key, g => g.Count()),
            "month" => instances.GroupBy(i => new DateTime(i.StartedAt.Year, i.StartedAt.Month, 1))
                               .ToDictionary(g => g.Key, g => g.Count()),
            _ => instances.GroupBy(i => i.StartedAt.Date)
                         .ToDictionary(g => g.Key, g => g.Count())
        };
    }

    private async Task<List<WorkflowPerformanceDto>> GetTopBottlenecks(int tenantId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        // This is a simplified implementation
        // In a real scenario, you'd analyze task durations and identify bottlenecks
        var tasks = await _context.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Where(t => t.WorkflowInstance.TenantId == tenantId &&
                       t.CreatedAt >= startDate && t.CreatedAt <= endDate &&
                       t.CompletedAt.HasValue)
            .ToListAsync(cancellationToken);

        var bottlenecks = tasks
            .Where(t => t.CompletedAt.HasValue)
            .GroupBy(t => t.NodeId)
            .Select(g => new WorkflowPerformanceDto
            {
                NodeId = g.Key,
                NodeName = g.Key, // Would map to actual node name from definition
                NodeType = "HumanTask", // Would get from definition
                AverageTime = g.Average(t => (t.CompletedAt!.Value - t.CreatedAt).TotalHours),
                InstanceCount = g.Count()
            })
            .OrderByDescending(p => p.AverageTime)
            .Take(10)
            .ToList();

        return bottlenecks;
    }
}
