using Microsoft.AspNetCore.SignalR;
using WorkflowService.Hubs;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

public class TaskNotificationDispatcher : ITaskNotificationDispatcher
{
    private readonly IHubContext<TaskNotificationsHub> _hubContext;
    private readonly ILogger<TaskNotificationDispatcher> _logger;

    public TaskNotificationDispatcher(
        IHubContext<TaskNotificationsHub> hubContext,
        ILogger<TaskNotificationDispatcher> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    public async Task NotifyTenantAsync(int tenantId, CancellationToken ct = default)
    {
        _logger.LogDebug("TASK_NOTIFY Tenant={TenantId}", tenantId);
        await _hubContext.Clients.Group(TaskNotificationsHub.TenantGroup(tenantId))
            .SendAsync("TasksChanged", new { scope = "tenant", tenantId }, ct);
    }

    public async Task NotifyUserAsync(int tenantId, int userId, CancellationToken ct = default)
    {
        _logger.LogDebug("TASK_NOTIFY User={UserId} Tenant={TenantId}", userId, tenantId);
        // fire to tenant + user group
        await _hubContext.Clients.Group(TaskNotificationsHub.UserGroup(userId))
            .SendAsync("TasksChanged", new { scope = "user", tenantId, userId }, ct);
        await NotifyTenantAsync(tenantId, ct);
    }
}
