using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Common.Repositories;

public class UserRoleRepository : TenantRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        ILogger<UserRoleRepository> logger)
        : base(context, tenantProvider, logger)
    {
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId && ur.IsActive)
            .Include(ur => ur.Role)
            .OrderBy(ur => ur.Role.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetRoleUsersAsync(int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ur => ur.RoleId == roleId && ur.TenantId == tenantId && ur.IsActive)
            .Include(ur => ur.User)
            .OrderBy(ur => ur.User.FirstName)
            .ThenBy(ur => ur.User.LastName)
            .ToListAsync(cancellationToken);
    }

    public async Task<UserRole?> GetUserRoleAsync(int userId, int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.TenantId == tenantId, cancellationToken);
    }

    public async Task<IEnumerable<UserRole>> GetUserRolesWithDetailsAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId && ur.IsActive)
            .Include(ur => ur.Role)
            .ThenInclude(r => r.RolePermissions)
            .ThenInclude(rp => rp.Permission)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> UserHasRoleAsync(int userId, int roleId, int tenantId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(ur => ur.UserId == userId && ur.RoleId == roleId && ur.TenantId == tenantId && ur.IsActive, cancellationToken);
    }

    public async Task RemoveUserRolesAsync(int userId, int tenantId, CancellationToken cancellationToken = default)
    {
        var userRoles = await _dbSet
            .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
            .ToListAsync(cancellationToken);

        _dbSet.RemoveRange(userRoles);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
