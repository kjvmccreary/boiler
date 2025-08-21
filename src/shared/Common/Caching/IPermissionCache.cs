using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Caching
{
    /// <summary>
    /// Specialized cache interface for RBAC permission data
    /// </summary>
    public interface IPermissionCache
    {
        /// <summary>
        /// Get cached permissions for a user in a specific tenant
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>List of permission names or null if not cached</returns>
        Task<IEnumerable<string>?> GetUserPermissionsAsync(int userId, int tenantId);

        /// <summary>
        /// Cache permissions for a user in a specific tenant
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="permissions">List of permission names</param>
        Task SetUserPermissionsAsync(int userId, int tenantId, IEnumerable<string> permissions);

        /// <summary>
        /// Invalidate cached permissions for a specific user
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="tenantId">Tenant ID</param>
        Task InvalidateUserPermissionsAsync(int userId, int tenantId);

        /// <summary>
        /// Invalidate all cached permissions for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        Task InvalidateTenantPermissionsAsync(int tenantId);

        /// <summary>
        /// Get cached role-to-permissions mapping for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <returns>Dictionary of role ID to permission names</returns>
        Task<Dictionary<int, IEnumerable<string>>?> GetRolePermissionsAsync(int tenantId);

        /// <summary>
        /// Cache role-to-permissions mapping for a tenant
        /// </summary>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="rolePermissions">Role ID to permissions mapping</param>
        Task SetRolePermissionsAsync(int tenantId, Dictionary<int, IEnumerable<string>> rolePermissions);

        /// <summary>
        /// Invalidate role permissions cache when a role is modified
        /// </summary>
        /// <param name="roleId">Role ID</param>
        /// <param name="tenantId">Tenant ID</param>
        Task InvalidateRolePermissionsAsync(int roleId, int tenantId);

        /// <summary>
        /// Remove cache entries matching a pattern (e.g., "permissions:*" for all permission caches)
        /// </summary>
        /// <param name="pattern">Redis pattern to match keys</param>
        Task RemoveByPatternAsync(string pattern);
    }
}
