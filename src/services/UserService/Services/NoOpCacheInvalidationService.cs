using Microsoft.Extensions.Logging;

namespace UserService.Services;

/// <summary>
/// No-op cache invalidation service for testing and performance environments
/// where Redis is not available but the interface is still required
/// </summary>
public class NoOpCacheInvalidationService : ICacheInvalidationService
{
    private readonly ILogger<NoOpCacheInvalidationService> _logger;

    public NoOpCacheInvalidationService(ILogger<NoOpCacheInvalidationService> logger)
    {
        _logger = logger;
    }

    public async Task OnRoleUpdatedAsync(int roleId, int tenantId)
    {
        _logger.LogDebug("No-op cache invalidation for role {RoleId} update in tenant {TenantId}", roleId, tenantId);
        await Task.CompletedTask;
    }

    public async Task OnUserRoleAssignedAsync(int userId, int roleId, int tenantId)
    {
        _logger.LogDebug("No-op cache invalidation for role {RoleId} assignment to user {UserId} in tenant {TenantId}", 
            roleId, userId, tenantId);
        await Task.CompletedTask;
    }

    public async Task OnUserRoleRevokedAsync(int userId, int roleId, int tenantId)
    {
        _logger.LogDebug("No-op cache invalidation for role {RoleId} revocation from user {UserId} in tenant {TenantId}", 
            roleId, userId, tenantId);
        await Task.CompletedTask;
    }

    public async Task OnPermissionChangedAsync(int permissionId)
    {
        _logger.LogDebug("No-op cache invalidation for permission {PermissionId} change", permissionId);
        await Task.CompletedTask;
    }

    public async Task OnTenantDeactivatedAsync(int tenantId)
    {
        _logger.LogDebug("No-op cache invalidation for deactivated tenant {TenantId}", tenantId);
        await Task.CompletedTask;
    }

    public async Task OnUserDeactivatedAsync(int userId)
    {
        _logger.LogDebug("No-op cache invalidation for deactivated user {UserId}", userId);
        await Task.CompletedTask;
    }
}
