using DTOs.Entities;

namespace Contracts.Repositories;

public interface IUserRoleRepository : IRepository<UserRole>
{
    /// <summary>
    /// Get user roles in a specific tenant
    /// </summary>
    Task<IEnumerable<UserRole>> GetUserRolesAsync(int userId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users with a specific role in a tenant
    /// </summary>
    Task<IEnumerable<UserRole>> GetRoleUsersAsync(int roleId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user role assignment
    /// </summary>
    Task<UserRole?> GetUserRoleAsync(int userId, int roleId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user roles with role and permission details
    /// </summary>
    Task<IEnumerable<UserRole>> GetUserRolesWithDetailsAsync(int userId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has role in tenant
    /// </summary>
    Task<bool> UserHasRoleAsync(int userId, int roleId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove user from all roles in tenant
    /// </summary>
    Task RemoveUserRolesAsync(int userId, int tenantId, CancellationToken cancellationToken = default);
}
