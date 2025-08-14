using DTOs.Entities;
using Common.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Contracts.Services;

namespace Common.Services
{
    /// <summary>
    /// Enhanced permission service with proper tenant isolation and security
    /// </summary>
    public class EnhancedPermissionService : IPermissionService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly ILogger<EnhancedPermissionService> _logger;

        public EnhancedPermissionService(
            ApplicationDbContext context,
            ITenantProvider tenantProvider,
            ILogger<EnhancedPermissionService> logger)
        {
            _context = context;
            _tenantProvider = tenantProvider;
            _logger = logger;
        }

        public async Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (!tenantId.HasValue)
                {
                    _logger.LogWarning("🔒 SECURITY: No tenant context for permission check: {Permission} for user {UserId}", permission, userId);
                    return false;
                }

                // 🔧 FIX: Ensure strict tenant isolation with proper join conditions
                var hasPermission = await _context.UserRoles
                    .Where(ur => ur.UserId == userId 
                               && ur.TenantId == tenantId.Value 
                               && ur.IsActive 
                               && (!ur.ExpiresAt.HasValue || ur.ExpiresAt > DateTime.UtcNow))
                    .Join(_context.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => new { UserRoleTenantId = ur.TenantId, RoleId = r.Id, r.IsActive, RoleTenantId = r.TenantId })
                    .Where(x => x.IsActive && (x.RoleTenantId == tenantId.Value || !x.RoleTenantId.HasValue)) // Allow system roles
                    .Join(_context.RolePermissions,
                        x => x.RoleId,
                        rp => rp.RoleId,
                        (x, rp) => rp.PermissionId)
                    .Join(_context.Permissions,
                        permId => permId,
                        p => p.Id,
                        (permId, p) => p.Name)
                    .AnyAsync(p => p == permission, cancellationToken);

                _logger.LogDebug("🔒 SECURITY: User {UserId} in tenant {TenantId} has permission {Permission}: {HasPermission}", 
                    userId, tenantId.Value, permission, hasPermission);
                
                return hasPermission;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error checking permission {Permission} for user {UserId}", permission, userId);
                return false;
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (!tenantId.HasValue)
                {
                    _logger.LogWarning("🔒 SECURITY: No tenant context for getting user permissions: {UserId}", userId);
                    return Enumerable.Empty<string>();
                }

                return await GetUserPermissionsForTenantAsync(userId, tenantId.Value, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error getting permissions for user {UserId}", userId);
                return Enumerable.Empty<string>();
            }
        }

        public async Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("🔍 PERMISSION DEBUG: Getting permissions for user {UserId} in tenant {TenantId}", userId, tenantId);
                
                // 🔧 FIX: Enhanced tenant isolation with proper role verification
                var permissions = await _context.UserRoles
                    .Where(ur => ur.UserId == userId 
                               && ur.TenantId == tenantId 
                               && ur.IsActive 
                               && (!ur.ExpiresAt.HasValue || ur.ExpiresAt > DateTime.UtcNow))
                    .Join(_context.Roles,
                        ur => ur.RoleId,
                        r => r.Id,
                        (ur, r) => new { UserRoleTenantId = ur.TenantId, RoleId = r.Id, r.IsActive, RoleTenantId = r.TenantId })
                    .Where(x => x.IsActive && (x.RoleTenantId == tenantId || !x.RoleTenantId.HasValue)) // System roles allowed
                    .Join(_context.RolePermissions,
                        x => x.RoleId,
                        rp => rp.RoleId,
                        (x, rp) => rp.PermissionId)
                    .Join(_context.Permissions,
                        permId => permId,
                        p => p.Id,
                        (permId, p) => p.Name)
                    .Distinct()
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("🔍 PERMISSION DEBUG: Final permissions for user {UserId} in tenant {TenantId}: [{Permissions}]", 
                    userId, tenantId, string.Join(", ", permissions));
                
                return permissions;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔒 SECURITY: Error getting permissions for user {UserId} in tenant {TenantId}", userId, tenantId);
                return Enumerable.Empty<string>();
            }
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

        public async Task<IEnumerable<PermissionInfo>> GetAllAvailablePermissionsAsync(CancellationToken cancellationToken = default)
        {
            var permissions = await _context.Permissions
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
                var categories = await _context.Permissions
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
                // 🔧 FIX: Add input sanitization
                if (string.IsNullOrWhiteSpace(category))
                {
                    _logger.LogWarning("🔒 SECURITY: Empty category requested");
                    return new List<PermissionInfo>();
                }

                var permissions = await _context.Permissions
                    .Where(p => p.IsActive && p.Category == category.Trim())
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
    }
}
