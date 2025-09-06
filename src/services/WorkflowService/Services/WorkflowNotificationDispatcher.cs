using Microsoft.AspNetCore.SignalR;
using WorkflowService.Domain.Models;
using WorkflowService.Hubs;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow.Enums;

namespace WorkflowService.Services;

public class WorkflowNotificationDispatcher : IWorkflowNotificationDispatcher
{
    private readonly IHubContext<WorkflowNotificationsHub> _hub;
    private readonly ILogger<WorkflowNotificationDispatcher> _logger;

    public WorkflowNotificationDispatcher(
        IHubContext<WorkflowNotificationsHub> hub,
        ILogger<WorkflowNotificationDispatcher> logger)
    {
        _hub = hub;
        _logger = logger;
    }

    public Task NotifyInstanceAsync(WorkflowInstance instance, CancellationToken ct = default) =>
        NotifyInstanceAsync(instance.TenantId, instance.Id, instance.Status, instance.CurrentNodeIds,
            instance.CompletedAt, instance.ErrorMessage, ct);

    public async Task NotifyInstanceAsync(
        int tenantId,
        int instanceId,
        InstanceStatus status,
        string currentNodeIds,
        DateTime? completedAt,
        string? errorMessage,
        CancellationToken ct = default)
    {
        var payload = new
        {
            instanceId,
            status = status.ToString(),
            currentNodeIds,
            completedAt,
            errorMessage
        };

        _logger.LogDebug("WF_NOTIFY InstanceUpdated Tenant={TenantId} Instance={InstanceId} Status={Status}",
            tenantId, instanceId, status);

        await _hub.Clients.Group(WorkflowNotificationsHub.TenantGroup(tenantId))
            .SendAsync("InstanceUpdated", payload, ct);

        await _hub.Clients.Group(WorkflowNotificationsHub.InstanceGroup(instanceId))
            .SendAsync("InstanceUpdated", payload, ct);
    }

    public async Task NotifyInstancesChangedAsync(int tenantId, CancellationToken ct = default)
    {
        _logger.LogDebug("WF_NOTIFY InstancesChanged Tenant={TenantId}", tenantId);
        await _hub.Clients.Group(WorkflowNotificationsHub.TenantGroup(tenantId))
            .SendAsync("InstancesChanged", new { tenantId }, ct);
    }

    public async Task NotifyInstanceProgressAsync(int tenantId, int instanceId, int percentage, int visitedCount,
        int totalNodes, string status, IEnumerable<string> activeNodeIds, CancellationToken ct = default)
    {
        var payload = new
        {
            instanceId,
            percentage,
            visitedCount,
            totalNodes,
            status,
            activeNodeIds = activeNodeIds?.ToArray() ?? Array.Empty<string>()
        };

        _logger.LogDebug("WF_NOTIFY InstanceProgress Tenant={TenantId} Instance={InstanceId} %={Pct}",
            tenantId, instanceId, percentage);

        await _hub.Clients.Group(WorkflowNotificationsHub.TenantGroup(tenantId))
            .SendAsync("InstanceProgress", payload, ct);

        await _hub.Clients.Group(WorkflowNotificationsHub.InstanceGroup(instanceId))
            .SendAsync("InstanceProgress", payload, ct);
    }
}
