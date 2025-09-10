using Contracts.Services;
using Microsoft.AspNetCore.SignalR;
using WorkflowService.Hubs;
using WorkflowService.Services.Interfaces;
using WorkflowService.Persistence;
using DTOs.Workflow;

namespace WorkflowService.Services;

public class TaskNotificationDispatcher : ITaskNotificationDispatcher
{
    private readonly IHubContext<TaskNotificationsHub> _hubContext;
    private readonly ILogger<TaskNotificationDispatcher> _logger;
    private readonly WorkflowDbContext _db;
    private readonly ITenantProvider _tenantProvider;
    private readonly IUserContext _userContext;

    public TaskNotificationDispatcher(
        IHubContext<TaskNotificationsHub> hubContext,
        ILogger<TaskNotificationDispatcher> logger,
        WorkflowDbContext db,
        ITenantProvider tenantProvider,
        IUserContext userContext)
    {
        _hubContext = hubContext;
        _logger = logger;
        _db = db;
        _tenantProvider = tenantProvider;
        _userContext = userContext;
    }

    public async Task NotifyTenantAsync(int tenantId, CancellationToken ct = default)
    {
        _logger.LogDebug("TASK_NOTIFY Tenant={TenantId}", tenantId);
        await _hubContext.Clients.Group(TaskNotificationsHub.TenantGroup(tenantId))
            .SendAsync("TasksChanged", new { scope = "tenant", tenantId }, ct);

        // Capture scoped values before fire-and-forget
        var userId = _userContext.UserId;
        var roles = _userContext.Roles ?? Array.Empty<string>();
        _ = PushActiveCountsAsync(tenantId, userId, roles, ct);
    }

    public async Task NotifyUserAsync(int tenantId, int userId, CancellationToken ct = default)
    {
        _logger.LogDebug("TASK_NOTIFY User={UserId} Tenant={TenantId}", userId, tenantId);
        await _hubContext.Clients.Group(TaskNotificationsHub.UserGroup(userId))
            .SendAsync("TasksChanged", new { scope = "user", tenantId, userId }, ct);

        await NotifyTenantAsync(tenantId, ct);
    }

    private async Task PushActiveCountsAsync(int tenantId, int? userId, IEnumerable<string> roles, CancellationToken ct)
    {
        try
        {
            if (!userId.HasValue)
            {
                _logger.LogDebug("TASK_NOTIFY ActiveCounts skipped (no user id)");
                return;
            }
            var counts = await _db.ComputeActiveCountsAsync(
                tenantId,
                userId.Value,
                roles.ToArray(),
                ct);

            await _hubContext.Clients.Group(TaskNotificationsHub.TenantGroup(tenantId))
                .SendAsync("ActiveTasksCountChanged", counts, ct);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "TASK_NOTIFY ActiveTasksCount push failed");
        }
    }
}
