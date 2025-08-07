using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using Microsoft.EntityFrameworkCore;

namespace Common.Services;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<PermissionService> _logger;

    public PermissionService(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger<PermissionService> logger)
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
                _logger.LogWarning("No tenant context available for permission check: {Permission} for user {UserId}", permission, userId);
                return false;
            }

            // FIX: UserId is int, TenantId is int - no conversion needed
            var hasPermission = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId.Value)
                .Join(_context.RolePermissions, 
                    ur => ur.RoleId, 
                    rp => rp.RoleId, 
                    (ur, rp) => rp)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Name)
                .AnyAsync(p => p == permission, cancellationToken);

            _logger.LogDebug("Permission check: User {UserId} has permission {Permission}: {HasPermission}", userId, permission, hasPermission);
            return hasPermission;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permission, userId);
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
                _logger.LogWarning("No tenant context available for getting user permissions: {UserId}", userId);
                return Enumerable.Empty<string>();
            }

            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId.Value)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Name)
                .Distinct()
                .ToListAsync(cancellationToken);

            _logger.LogDebug("Retrieved {Count} permissions for user {UserId}", permissions.Count, userId);
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting permissions for user {UserId}", userId);
            return Enumerable.Empty<string>();
        }
    }

    public async Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("🔍 PERMISSION DEBUG: Getting permissions for user {UserId} in tenant {TenantId}", userId, tenantId);
            
            // Step 1: Check if user has roles in this tenant
            var userRoles = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                .ToListAsync(cancellationToken);
                
            _logger.LogInformation("🔍 PERMISSION DEBUG: Found {UserRoleCount} user roles for user {UserId} in tenant {TenantId}: {RoleIds}", 
                userRoles.Count, userId, tenantId, string.Join(",", userRoles.Select(ur => ur.RoleId)));
            
            if (!userRoles.Any())
            {
                _logger.LogWarning("🔍 PERMISSION DEBUG: No user roles found for user {UserId} in tenant {TenantId}", userId, tenantId);
                return Enumerable.Empty<string>();
            }
            
            // Step 2: Check role permissions for these roles
            var roleIds = userRoles.Select(ur => ur.RoleId).ToList();
            var rolePermissions = await _context.RolePermissions
                .Where(rp => roleIds.Contains(rp.RoleId))
                .ToListAsync(cancellationToken);
                
            _logger.LogInformation("🔍 PERMISSION DEBUG: Found {RolePermissionCount} role permissions for roles {RoleIds}", 
                rolePermissions.Count, string.Join(",", roleIds));
            
            if (!rolePermissions.Any())
            {
                _logger.LogWarning("🔍 PERMISSION DEBUG: No role permissions found for roles {RoleIds}", string.Join(",", roleIds));
                return Enumerable.Empty<string>();
            }
            
            // Step 3: Get the actual permission names
            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                .Join(_context.RolePermissions,
                    ur => ur.RoleId,
                    rp => rp.RoleId,
                    (ur, rp) => rp)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => p.Name)
                .Distinct()
                .ToListAsync(cancellationToken);

            _logger.LogInformation("🔍 PERMISSION DEBUG: Final permissions for user {UserId} in tenant {TenantId}: [{Permissions}]", 
                userId, tenantId, string.Join(", ", permissions));
            
            return permissions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔍 PERMISSION DEBUG: Error getting permissions for user {UserId} in tenant {TenantId}", userId, tenantId);
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
                Name = p.Name,
                Category = p.Category,
                Description = p.Description
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
}
