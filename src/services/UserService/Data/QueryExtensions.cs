using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using DTOs.Entities;

namespace UserService.Data
{
    /// <summary>
    /// EF Core query extensions for optimized RBAC queries
    /// </summary>
    public static class QueryExtensions
    {
        /// <summary>
        /// Include permissions for user roles with optimized loading
        /// </summary>
        public static IQueryable<UserRole> IncludePermissions(this IQueryable<UserRole> query)
        {
            return query
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission);
        }

        /// <summary>
        /// Include permissions for roles with optimized loading
        /// </summary>
        public static IQueryable<Role> IncludePermissions(this IQueryable<Role> query)
        {
            return query
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
        }

        /// <summary>
        /// Include roles and permissions for users with tenant filtering
        /// </summary>
        public static IQueryable<User> IncludeRolesAndPermissions(
            this IQueryable<User> query, 
            int tenantId)
        {
            return query
                .Include(u => u.UserRoles.Where(ur => ur.TenantId == tenantId && ur.IsActive))
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission);
        }

        /// <summary>
        /// Filter active user roles for a specific tenant
        /// </summary>
        public static IQueryable<UserRole> ForTenant(this IQueryable<UserRole> query, int tenantId)
        {
            return query.Where(ur => ur.TenantId == tenantId && ur.IsActive);
        }

        /// <summary>
        /// Filter active roles for a specific tenant (includes system roles)
        /// </summary>
        public static IQueryable<Role> ForTenant(this IQueryable<Role> query, int? tenantId)
        {
            return query.Where(r => r.IsActive && (r.TenantId == tenantId || r.IsSystemRole));
        }

        /// <summary>
        /// Filter active permissions
        /// </summary>
        public static IQueryable<Permission> Active(this IQueryable<Permission> query)
        {
            return query.Where(p => p.IsActive);
        }

        /// <summary>
        /// Get permissions for specific roles
        /// </summary>
        public static IQueryable<Permission> ForRoles(this IQueryable<Permission> query, IEnumerable<int> roleIds)
        {
            return query
                .Where(p => p.RolePermissions.Any(rp => roleIds.Contains(rp.RoleId)))
                .Distinct();
        }

        /// <summary>
        /// Compiled query for frequent permission checks - optimized for hot path
        /// </summary>
        public static readonly Func<Common.Data.ApplicationDbContext, int, int, string, Task<bool>> HasPermissionCompiled =
            EF.CompileAsyncQuery((Common.Data.ApplicationDbContext context, int userId, int tenantId, string permission) =>
                context.UserRoles
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
                    .Any(p => p == permission));

        /// <summary>
        /// Compiled query to get user permission names - optimized for hot path
        /// </summary>
        public static readonly Func<Common.Data.ApplicationDbContext, int, int, Task<string[]>> GetUserPermissionsCompiled =
            EF.CompileAsyncQuery((Common.Data.ApplicationDbContext context, int userId, int tenantId) =>
                context.UserRoles
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
                    .ToArray());
    }
}
