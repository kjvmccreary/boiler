using Common.Caching;
using Common.Data;
using Common.Services;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Services
{
    /// <summary>
    /// Cached implementation of the permission service for high-performance RBAC operations
    /// </summary>
    public class CachedPermissionService : IPermissionService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ITenantProvider _tenantProvider;
        private readonly IPermissionCache _cache;
        private readonly ILogger<CachedPermissionService> _logger;
        
        public CachedPermissionService(
            IServiceProvider serviceProvider,
            ITenantProvider tenantProvider,
            IPermissionCache cache,
            ILogger<CachedPermissionService> logger)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _tenantProvider = tenantProvider ?? throw new ArgumentNullException(nameof(tenantProvider));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (!tenantId.HasValue)
                {
                    _logger.LogWarning("No tenant context available for permission check: {Permission} for user {UserId}", permission, userId);
                    return false;
                }

                // Try cache first
                var userPermissions = await _cache.GetUserPermissionsAsync(userId, tenantId.Value);
                if (userPermissions != null)
                {
                    var hasPermission = userPermissions.Contains(permission);
                    _logger.LogDebug("Cache hit - Permission check: User {UserId} has permission {Permission}: {HasPermission} (took {Elapsed}ms)", 
                        userId, permission, hasPermission, stopwatch.ElapsedMilliseconds);
                    return hasPermission;
                }

                _logger.LogDebug("Cache miss for user {UserId} permissions", userId);
                
                // Load from database with optimized query
                var permissions = await LoadUserPermissionsFromDatabaseAsync(userId, tenantId.Value, cancellationToken);
                
                // Cache the result
                await _cache.SetUserPermissionsAsync(userId, tenantId.Value, permissions);
                
                var result = permissions.Contains(permission);
                _logger.LogDebug("Database - Permission check: User {UserId} has permission {Permission}: {HasPermission} (took {Elapsed}ms)", 
                    userId, permission, result, stopwatch.ElapsedMilliseconds);
                
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permission, userId);
                return false;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (!tenantId.HasValue)
                {
                    _logger.LogWarning("No tenant context available for getting user permissions: {UserId}", userId);
                    return Enumerable.Empty<string>();
                }

                // Try cache first
                var cached = await _cache.GetUserPermissionsAsync(userId, tenantId.Value);
                if (cached != null)
                {
                    _logger.LogDebug("Cache hit - Retrieved {Count} permissions for user {UserId} (took {Elapsed}ms)", 
                        cached.Count(), userId, stopwatch.ElapsedMilliseconds);
                    return cached;
                }

                _logger.LogDebug("Cache miss for user {UserId} permissions", userId);
                
                // Load from database with optimized query
                var permissions = await LoadUserPermissionsFromDatabaseAsync(userId, tenantId.Value, cancellationToken);
                
                // Cache the result
                await _cache.SetUserPermissionsAsync(userId, tenantId.Value, permissions);
                
                _logger.LogDebug("Database - Retrieved {Count} permissions for user {UserId} (took {Elapsed}ms)", 
                    permissions.Count(), userId, stopwatch.ElapsedMilliseconds);
                
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
                return Enumerable.Empty<string>();
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                // Try cache first
                var cached = await _cache.GetUserPermissionsAsync(userId, tenantId);
                if (cached != null)
                {
                    _logger.LogDebug("Cache hit - Retrieved {Count} permissions for user {UserId} in tenant {TenantId} (took {Elapsed}ms)", 
                        cached.Count(), userId, tenantId, stopwatch.ElapsedMilliseconds);
                    return cached;
                }

                _logger.LogDebug("Cache miss for user {UserId} permissions in tenant {TenantId}", userId, tenantId);
                
                // Load from database with optimized query
                var permissions = await LoadUserPermissionsFromDatabaseAsync(userId, tenantId, cancellationToken);
                
                // Cache the result
                await _cache.SetUserPermissionsAsync(userId, tenantId, permissions);
                
                _logger.LogDebug("Database - Retrieved {Count} permissions for user {UserId} in tenant {TenantId} (took {Elapsed}ms)", 
                    permissions.Count(), userId, tenantId, stopwatch.ElapsedMilliseconds);
                
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for user {UserId} in tenant {TenantId}", userId, tenantId);
                return Enumerable.Empty<string>();
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// Optimized database query to load user permissions with single query
        /// </summary>
        private async Task<IEnumerable<string>> LoadUserPermissionsFromDatabaseAsync(
            int userId, 
            int tenantId, 
            CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            _logger.LogDebug("Loading permissions from database for user {UserId} in tenant {TenantId}", userId, tenantId);
            
            var permissions = await context.UserRoles
                .IgnoreQueryFilters()
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId && ur.IsActive)
                .Join(context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Name)
                .Distinct()
                .ToListAsync(cancellationToken);

            return permissions;
        }

        /// <summary>
        /// Batch load permissions for multiple users (for cache warming)
        /// </summary>
        public async Task<Dictionary<int, IEnumerable<string>>> GetBatchUserPermissionsAsync(
            IEnumerable<int> userIds, 
            int tenantId, 
            CancellationToken cancellationToken = default)
        {
            var stopwatch = Stopwatch.StartNew();
            var result = new Dictionary<int, IEnumerable<string>>();
            var uncachedUsers = new List<int>();

            try
            {
                // Check cache for each user
                foreach (var userId in userIds)
                {
                    var cached = await _cache.GetUserPermissionsAsync(userId, tenantId);
                    if (cached != null)
                    {
                        result[userId] = cached;
                    }
                    else
                    {
                        uncachedUsers.Add(userId);
                    }
                }

                if (uncachedUsers.Any())
                {
                    _logger.LogDebug("Batch loading permissions for {Count} uncached users in tenant {TenantId}", 
                        uncachedUsers.Count, tenantId);
                    
                    // Batch load uncached users with optimized query
                    var batchPermissions = await LoadBatchPermissionsFromDatabaseAsync(
                        uncachedUsers, 
                        tenantId, 
                        cancellationToken);
                    
                    foreach (var kvp in batchPermissions)
                    {
                        result[kvp.Key] = kvp.Value;
                        // Cache individual user permissions
                        await _cache.SetUserPermissionsAsync(kvp.Key, tenantId, kvp.Value);
                    }
                }

                _logger.LogDebug("Batch retrieved permissions for {Total} users ({Cached} cached, {Database} from database) in {Elapsed}ms", 
                    userIds.Count(), result.Count - uncachedUsers.Count, uncachedUsers.Count, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error batch loading permissions for users in tenant {TenantId}", tenantId);
                return result;
            }
            finally
            {
                stopwatch.Stop();
            }
        }

        /// <summary>
        /// Optimized batch query to load permissions for multiple users at once
        /// </summary>
        private async Task<Dictionary<int, IEnumerable<string>>> LoadBatchPermissionsFromDatabaseAsync(
            IEnumerable<int> userIds,
            int tenantId,
            CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var userPermissions = await context.UserRoles
                .IgnoreQueryFilters()
                .Where(ur => userIds.Contains(ur.UserId) && ur.TenantId == tenantId && ur.IsActive)
                .Join(context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => new { ur.UserId, rp.PermissionId })
                .Join(context.Permissions,
                    urp => urp.PermissionId,
                    p => p.Id,
                    (urp, p) => new { urp.UserId, PermissionName = p.Name })
                .ToListAsync(cancellationToken);

            var result = new Dictionary<int, IEnumerable<string>>();
            
            foreach (var userId in userIds)
            {
                var permissions = userPermissions
                    .Where(up => up.UserId == userId)
                    .Select(up => up.PermissionName)
                    .Distinct()
                    .ToList();

                result[userId] = permissions;
            }

            return result;
        }

        public async Task<bool> UserHasAnyPermissionAsync(int userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
        {
            var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
            return permissions.Any(p => userPermissions.Contains(p));
        }

        public async Task<bool> UserHasAllPermissionsAsync(int userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default)
        {
            var userPermissions = await GetUserPermissionsAsync(userId, cancellationToken);
            return permissions.All(p => userPermissions.Contains(p));
        }

        // These methods don't need caching as they're admin/setup operations and called infrequently
        public async Task<IEnumerable<PermissionInfo>> GetAllAvailablePermissionsAsync(CancellationToken cancellationToken = default)
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            
            var permissions = await context.Permissions
                .Where(p => p.IsActive)
                .Select(p => new PermissionInfo
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Description = p.Description,
                    IsActive = p.IsActive
                })
                .ToListAsync(cancellationToken);

            return permissions;
        }

        public async Task<Dictionary<string, List<PermissionInfo>>> GetPermissionsByCategoryAsync(CancellationToken cancellationToken = default)
        {
            var permissions = await GetAllAvailablePermissionsAsync(cancellationToken);
            return permissions.GroupBy(p => p.Category)
                .ToDictionary(g => g.Key, g => g.ToList());
        }

        public async Task<List<string>> GetPermissionCategoriesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var categories = await context.Permissions
                    .Where(p => p.IsActive)
                    .Select(p => p.Category)
                    .Distinct()
                    .OrderBy(c => c)
                    .ToListAsync(cancellationToken);

                return categories;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permission categories");
                return new List<string>();
            }
        }

        public async Task<List<PermissionInfo>> GetPermissionsForCategoryAsync(string category, CancellationToken cancellationToken = default)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var permissions = await context.Permissions
                    .Where(p => p.IsActive && p.Category == category)
                    .Select(p => new PermissionInfo
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Category = p.Category,
                        Description = p.Description,
                        IsActive = p.IsActive
                    })
                    .OrderBy(p => p.Name)
                    .ToListAsync(cancellationToken);

                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting permissions for category {Category}", category);
                return new List<PermissionInfo>();
            }
        }

        /// <summary>
        /// Invalidate user permission cache
        /// </summary>
        public async Task InvalidateUserCacheAsync(int userId, int tenantId)
        {
            await _cache.InvalidateUserPermissionsAsync(userId, tenantId);
            _logger.LogDebug("Invalidated permission cache for user {UserId} in tenant {TenantId}", userId, tenantId);
        }

        /// <summary>
        /// Invalidate all permission caches for a tenant
        /// </summary>
        public async Task InvalidateTenantCacheAsync(int tenantId)
        {
            await _cache.InvalidateTenantPermissionsAsync(tenantId);
            _logger.LogInformation("Invalidated all permission caches for tenant {TenantId}", tenantId);
        }

        /// <summary>
        /// Warm the cache for a specific tenant by pre-loading permissions for active users
        /// </summary>
        public async Task WarmCacheForTenantAsync(int tenantId)
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogInformation("Starting permission cache warming for tenant {TenantId}", tenantId);
            
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                
                var activeUsers = await context.UserRoles
                    .IgnoreQueryFilters()
                    .Where(ur => ur.TenantId == tenantId && ur.IsActive)
                    .Select(ur => ur.UserId)
                    .Distinct()
                    .ToListAsync();

                if (!activeUsers.Any())
                {
                    _logger.LogInformation("No active users found for tenant {TenantId}, cache warming skipped", tenantId);
                    return;
                }

                // Batch load permissions
                await GetBatchUserPermissionsAsync(activeUsers, tenantId);
                
                _logger.LogInformation("Warmed permission cache for {Count} users in tenant {TenantId} in {Elapsed}ms", 
                    activeUsers.Count, tenantId, stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error warming cache for tenant {TenantId}", tenantId);
            }
            finally
            {
                stopwatch.Stop();
            }
        }
    }
}
