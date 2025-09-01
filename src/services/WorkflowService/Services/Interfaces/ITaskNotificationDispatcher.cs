namespace WorkflowService.Services.Interfaces;

public interface ITaskNotificationDispatcher
{
    Task NotifyTenantAsync(int tenantId, CancellationToken ct = default);
    Task NotifyUserAsync(int tenantId, int userId, CancellationToken ct = default);
}
