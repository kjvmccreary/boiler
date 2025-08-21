using Common.Caching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UserService.Services
{
    /// <summary>
    /// Redis-based implementation of permission caching for RBAC system
    /// </summary>
    public class RedisPermissionCache : IPermissionCache
    {
        private readonly ICacheService _cache;
        private readonly ILogger<RedisPermissionCache> _logger;

        // Cache expiration settings
        private readonly TimeSpan _userPermissionExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _rolePermissionExpiration = TimeSpan.FromMinutes(10);

        public RedisPermissionCache(
            ICacheService cache,
            ILogger<RedisPermissionCache> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<string>?> GetUserPermissionsAsync(int userId, int tenantId)
        {
            var key = GetUserPermissionKey(userId, tenantId);
            var permissions = await _cache.GetAsync<IEnumerable<string>>(key);
            
            if (permissions != null)
            {
                _logger.LogDebug("Found cached permissions for user {UserId} in tenant {TenantId}", 
                    userId, tenantId);
            }
            
            return permissions;
        }

        public async Task SetUserPermissionsAsync(
            int userId,
            int tenantId,
            IEnumerable<string> permissions)
        {
            var key = GetUserPermissionKey(userId, tenantId);
            await _cache.SetAsync(key, permissions, _userPermissionExpiration);
            
            _logger.LogDebug("Cached permissions for user {UserId} in tenant {TenantId} (expires in {Expiration})", 
                userId, tenantId, _userPermissionExpiration);
        }

        public async Task InvalidateUserPermissionsAsync(int userId, int tenantId)
        {
            var key = GetUserPermissionKey(userId, tenantId);
            await _cache.RemoveAsync(key);
            
            _logger.LogDebug("Invalidated permission cache for user {UserId} in tenant {TenantId}", 
                userId, tenantId);
        }

        public async Task InvalidateTenantPermissionsAsync(int tenantId)
        {
            var pattern = $"permissions:tenant:{tenantId}:*";
            await RemoveByPatternAsync(pattern);
            
            _logger.LogInformation("Invalidated all permission caches for tenant {TenantId}", tenantId);
        }

        public async Task<Dictionary<int, IEnumerable<string>>?> GetRolePermissionsAsync(int tenantId)
        {
            var key = GetRolePermissionKey(tenantId);
            var rolePermissions = await _cache.GetAsync<Dictionary<int, IEnumerable<string>>>(key);
            
            if (rolePermissions != null)
            {
                _logger.LogDebug("Found cached role permissions for tenant {TenantId}", tenantId);
            }
            
            return rolePermissions;
        }

        public async Task SetRolePermissionsAsync(
            int tenantId,
            Dictionary<int, IEnumerable<string>> rolePermissions)
        {
            var key = GetRolePermissionKey(tenantId);
            await _cache.SetAsync(key, rolePermissions, _rolePermissionExpiration);
            
            _logger.LogDebug("Cached role permissions for tenant {TenantId} (expires in {Expiration})", 
                tenantId, _rolePermissionExpiration);
        }

        public async Task InvalidateRolePermissionsAsync(int roleId, int tenantId)
        {
            // Invalidate the role cache for this tenant
            var roleKey = GetRolePermissionKey(tenantId);
            await _cache.RemoveAsync(roleKey);
            
            // Also invalidate all user caches for this tenant since role changed
            await InvalidateTenantPermissionsAsync(tenantId);
            
            _logger.LogInformation("Invalidated role permission caches for role {RoleId} in tenant {TenantId}", 
                roleId, tenantId);
        }

        /// <summary>
        /// Remove cache entries matching a pattern using the underlying cache service
        /// </summary>
        /// <param name="pattern">Redis pattern to match keys</param>
        public async Task RemoveByPatternAsync(string pattern)
        {
            await _cache.RemoveByPatternAsync(pattern);
            _logger.LogDebug("Removed cache entries matching pattern: {Pattern}", pattern);
        }

        /// <summary>
        /// Generate cache key for user permissions
        /// Format: permissions:tenant:{tenantId}:user:{userId}
        /// </summary>
        private string GetUserPermissionKey(int userId, int tenantId)
        {
            return $"permissions:tenant:{tenantId}:user:{userId}";
        }

        /// <summary>
        /// Generate cache key for role permissions mapping
        /// Format: permissions:tenant:{tenantId}:roles
        /// </summary>
        private string GetRolePermissionKey(int tenantId)
        {
            return $"permissions:tenant:{tenantId}:roles";
        }
    }
}
