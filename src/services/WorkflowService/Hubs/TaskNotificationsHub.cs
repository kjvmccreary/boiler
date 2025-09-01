using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Contracts.Services; // ✅ Correct namespace for ITenantProvider

namespace WorkflowService.Hubs;

[Authorize]
public class TaskNotificationsHub : Hub
{
    private readonly ITenantProvider _tenantProvider;

    public TaskNotificationsHub(ITenantProvider tenantProvider)
    {
        _tenantProvider = tenantProvider;
    }

    public override async Task OnConnectedAsync()
    {
        var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
        if (tenantId.HasValue)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, TenantGroup(tenantId.Value));
        }

        var userIdClaim = Context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, UserGroup(userId));
        }

        await base.OnConnectedAsync();
    }

    public static string TenantGroup(int tenantId) => $"tenant-{tenantId}";
    public static string UserGroup(int userId) => $"user-{userId}";
}
