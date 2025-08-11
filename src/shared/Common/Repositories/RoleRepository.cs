using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Repositories;

public class RoleRepository : TenantRepository<Role>, IRoleRepository
{
    public RoleRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger<RoleRepository> logger)
        : base(context, tenantProvider, logger)
    {
    }

    public override IQueryable<Role> Query()
    {
        // For roles, we include both tenant-specific roles and system roles
        var tenantId = GetCurrentTenantIdSync();
        if (tenantId.HasValue)
        {
            return _dbSet.Where(r => r.TenantId == tenantId.Value || r.TenantId == null);
        }

        // If no tenant context, return all (for system admin scenarios)
        return _dbSet;
    }

    /// <summary>
    /// Get role by name within tenant context (null tenant for system roles)
    /// </summary>
    public async Task<Role?> GetByNameAsync(string name, int? tenantId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId && r.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role by name {RoleName} for tenant {TenantId}", name, tenantId);
            return null;
        }
    }

    /// <summary>
    /// Get all roles for a specific tenant (includes system roles)
    /// </summary>
    public async Task<IEnumerable<Role>> GetTenantRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(r => r.TenantId == tenantId || r.TenantId == null) // Include system roles
                .Where(r => r.IsActive)
                .OrderBy(r => r.IsSystemRole ? 0 : 1) // System roles first
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for tenant {TenantId}", tenantId);
            return Enumerable.Empty<Role>();
        }
    }

    /// <summary>
    /// Get all system roles (TenantId is null)
    /// </summary>
    public async Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(r => r.TenantId == null && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting system roles");
            return Enumerable.Empty<Role>();
        }
    }

    /// <summary>
    /// Get role with its permissions
    /// </summary>
    public async Task<Role?> GetWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await Query()
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId && r.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role {RoleId} with permissions", roleId);
            return null;
        }
    }

    /// <summary>
    /// Get default roles for a tenant
    /// </summary>
    public async Task<IEnumerable<Role>> GetDefaultRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(r => (r.TenantId == tenantId || r.TenantId == null) && r.IsDefault && r.IsActive)
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting default roles for tenant {TenantId}", tenantId);
            return Enumerable.Empty<Role>();
        }
    }

    /// <summary>
    /// Check if role name exists in tenant
    /// </summary>
    public async Task<bool> NameExistsAsync(string name, int? tenantId, int? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var query = _dbSet
                .Where(r => r.Name.ToLower() == name.ToLower() && r.TenantId == tenantId && r.IsActive);

            if (excludeRoleId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRoleId.Value);
            }

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if role name {RoleName} exists for tenant {TenantId}", name, tenantId);
            return false;
        }
    }

    #region Additional Helper Methods

    /// <summary>
    /// Get roles with user count for management UI
    /// </summary>
    public async Task<IEnumerable<Role>> GetRolesWithUserCountAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(r => (r.TenantId == tenantId || r.TenantId == null) && r.IsActive)
                .Include(r => r.UserRoles.Where(ur => ur.IsActive && ur.TenantId == tenantId))
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles with user count for tenant {TenantId}", tenantId);
            return Enumerable.Empty<Role>();
        }
    }

    /// <summary>
    /// Get roles that can be assigned to users (active, non-system roles or system roles that allow assignment)
    /// </summary>
    public async Task<IEnumerable<Role>> GetAssignableRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(r => (r.TenantId == tenantId || r.TenantId == null) && r.IsActive)
                .Where(r => !r.IsSystemRole || r.Name != "SuperAdmin") // Exclude SuperAdmin from assignment
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting assignable roles for tenant {TenantId}", tenantId);
            return Enumerable.Empty<Role>();
        }
    }

    /// <summary>
    /// Search roles by name or description
    /// </summary>
    public async Task<IEnumerable<Role>> SearchRolesAsync(int tenantId, string searchTerm, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return await GetTenantRolesAsync(tenantId, cancellationToken);
            }

            var searchLower = searchTerm.ToLower();
            return await _dbSet
                .Where(r => (r.TenantId == tenantId || r.TenantId == null) && r.IsActive)
                .Where(r => r.Name.ToLower().Contains(searchLower) || 
                           r.Description.ToLower().Contains(searchLower))
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error searching roles for tenant {TenantId} with term {SearchTerm}", tenantId, searchTerm);
            return Enumerable.Empty<Role>();
        }
    }

    /// <summary>
    /// Get role by ID with tenant validation
    /// </summary>
    public async Task<Role?> GetByIdForTenantAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Id == roleId && 
                    (r.TenantId == tenantId || r.TenantId == null) && r.IsActive, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting role {RoleId} for tenant {TenantId}", roleId, tenantId);
            return null;
        }
    }

    /// <summary>
    /// Soft delete a role (set IsActive to false)
    /// </summary>
    public async Task<bool> SoftDeleteAsync(int roleId, CancellationToken cancellationToken = default)
    {
        try
        {
            var role = await GetByIdAsync(roleId, cancellationToken);
            if (role == null || role.IsSystemRole)
            {
                return false;
            }

            role.IsActive = false;
            role.UpdatedAt = DateTime.UtcNow;
            
            await UpdateAsync(role, cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error soft deleting role {RoleId}", roleId);
            return false;
        }
    }

    /// <summary>
    /// Get roles that have specific permission
    /// </summary>
    public async Task<IEnumerable<Role>> GetRolesWithPermissionAsync(int tenantId, string permissionName, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _dbSet
                .Where(r => (r.TenantId == tenantId || r.TenantId == null) && r.IsActive)
                .Where(r => r.RolePermissions.Any(rp => rp.Permission.Name == permissionName))
                .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
                .OrderBy(r => r.IsSystemRole ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToListAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles with permission {PermissionName} for tenant {TenantId}", permissionName, tenantId);
            return Enumerable.Empty<Role>();
        }
    }

    #endregion

    #region Private Helper Methods

    /// <summary>
    /// Synchronously get current tenant ID (use with caution)
    /// </summary>
    private new int? GetCurrentTenantIdSync()
    {
        try
        {
            return _tenantProvider.GetCurrentTenantIdAsync().GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Could not get current tenant ID synchronously");
            return null;
        }
    }

    #endregion
}
