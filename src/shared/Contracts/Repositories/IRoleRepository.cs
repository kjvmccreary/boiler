using DTOs.Entities;

namespace Contracts.Repositories;

public interface IRoleRepository : IRepository<Role>
{
    /// <summary>
    /// Get role by name within tenant context (null tenant for system roles)
    /// </summary>
    Task<Role?> GetByNameAsync(string name, int? tenantId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all roles for a specific tenant (includes system roles)
    /// </summary>
    Task<IEnumerable<Role>> GetTenantRolesAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all system roles (TenantId is null)
    /// </summary>
    Task<IEnumerable<Role>> GetSystemRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get role with its permissions
    /// </summary>
    Task<Role?> GetWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get default roles for a tenant
    /// </summary>
    Task<IEnumerable<Role>> GetDefaultRolesAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if role name exists in tenant
    /// </summary>
    Task<bool> NameExistsAsync(string name, int? tenantId, int? excludeRoleId = null, CancellationToken cancellationToken = default);
}
