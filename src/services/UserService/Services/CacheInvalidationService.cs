using Common.Caching;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace UserService.Services
{
    /// <summary>
    /// Service responsible for invalidating caches when RBAC data changes
    /// </summary>
    public interface ICacheInvalidationService
    {
        Task OnRoleUpdatedAsync(int roleId, int tenantId);
        Task OnUserRoleAssignedAsync(int userId, int roleId, int tenantId);
        Task OnUserRoleRevokedAsync(int userId, int roleId, int tenantId);
        Task OnPermissionChangedAsync(int permissionId);
        Task OnTenantDeactivatedAsync(int tenantId);
        Task OnUserDeactivatedAsync(int userId);
    }

    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly IPermissionCache _cache;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(
            IPermissionCache cache,
            ILogger<CacheInvalidationService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnRoleUpdatedAsync(int roleId, int tenantId)
        {
            _logger.LogInformation("Invalidating cache due to role {RoleId} update in tenant {TenantId}", roleId, tenantId);
            
            try
            {
                // Invalidate role permissions cache
                await _cache.InvalidateRolePermissionsAsync(roleId, tenantId);
                
                _logger.LogDebug("Successfully invalidated cache for role {RoleId} update", roleId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for role {RoleId} update", roleId);
            }
        }

        public async Task OnUserRoleAssignedAsync(int userId, int roleId, int tenantId)
        {
            _logger.LogInformation("Invalidating cache due to role {RoleId} assignment to user {UserId} in tenant {TenantId}", 
                roleId, userId, tenantId);
            
            try
            {
                // Invalidate user's permission cache
                await _cache.InvalidateUserPermissionsAsync(userId, tenantId);
                
                _logger.LogDebug("Successfully invalidated cache for user {UserId} role assignment", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for user {UserId} role assignment", userId);
            }
        }

        public async Task OnUserRoleRevokedAsync(int userId, int roleId, int tenantId)
        {
            _logger.LogInformation("Invalidating cache due to role {RoleId} revocation from user {UserId} in tenant {TenantId}", 
                roleId, userId, tenantId);
            
            try
            {
                // Invalidate user's permission cache
                await _cache.InvalidateUserPermissionsAsync(userId, tenantId);
                
                _logger.LogDebug("Successfully invalidated cache for user {UserId} role revocation", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for user {UserId} role revocation", userId);
            }
        }

        public async Task OnPermissionChangedAsync(int permissionId)
        {
            // This is a system-wide change, invalidate everything
            _logger.LogWarning("System-wide permission change detected for permission {PermissionId}, clearing all caches", permissionId);
            
            try
            {
                // For system-wide permission changes, we need to clear all caches
                // This is a rare but critical operation
                await _cache.RemoveByPatternAsync("permissions:*");
                
                _logger.LogInformation("Successfully cleared all permission caches due to system permission change");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing caches for system permission change");
            }
        }

        public async Task OnTenantDeactivatedAsync(int tenantId)
        {
            _logger.LogInformation("Invalidating all caches for deactivated tenant {TenantId}", tenantId);
            
            try
            {
                await _cache.InvalidateTenantPermissionsAsync(tenantId);
                
                _logger.LogInformation("Successfully cleared all caches for deactivated tenant {TenantId}", tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing caches for deactivated tenant {TenantId}", tenantId);
            }
        }

        public async Task OnUserDeactivatedAsync(int userId)
        {
            _logger.LogInformation("Invalidating caches for deactivated user {UserId}", userId);
            
            try
            {
                // We need to invalidate this user's cache across all tenants they might belong to
                // Since we don't know all tenants, we use a pattern-based removal
                await _cache.RemoveByPatternAsync($"permissions:*:user:{userId}");
                
                _logger.LogDebug("Successfully cleared all caches for deactivated user {UserId}", userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing caches for deactivated user {UserId}", userId);
            }
        }
    }
}
