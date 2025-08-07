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

    private int? GetCurrentTenantIdSync()
    {
        try
        {
            return _tenantProvider.GetCurrentTenantIdAsync().GetAwaiter().GetResult();
        }
        catch
        {
            return null;
        }
    }

    public async Task<Role?> GetByNameAsync(string name, int? tenantId = null, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.Name == name && r.TenantId == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetTenantRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.TenantId == tenantId || r.TenantId == null) // Include system roles
            .Where(r => r.IsActive)
            .OrderBy(r => r.IsSystemRole) // System roles first
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => r.TenantId == null && r.IsActive)
            .OrderBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<Role?> GetWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .Include(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
    }

    public async Task<IEnumerable<Role>> GetDefaultRolesAsync(int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(r => (r.TenantId == tenantId || r.TenantId == null) && r.IsDefault && r.IsActive)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> NameExistsAsync(string name, int? tenantId, int? excludeRoleId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(r => r.Name == name && r.TenantId == tenantId);
        
        if (excludeRoleId.HasValue)
        {
            query = query.Where(r => r.Id != excludeRoleId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}
