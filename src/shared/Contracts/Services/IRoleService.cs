using DTOs.Entities;
using DTOs.Common; // Add for pagination

namespace Contracts.Services;

/// <summary>
/// Service for managing roles within a tenant context
/// NOTE: This interface will be fully implemented in Phase 3 when Role entities are created
/// For Phase 2, we only define the contract.
/// </summary>
public interface IRoleService
{
    /// <summary>
    /// Create a new role for the current tenant
    /// Returns: Role information including assigned permissions
    /// </summary>
    Task<RoleInfo> CreateRoleAsync(string name, string description, List<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing role (tenant-scoped)
    /// </summary>
    Task<RoleInfo> UpdateRoleAsync(int roleId, string name, string description, List<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete a role (only custom roles, not system roles)
    /// </summary>
    Task DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get role by ID (tenant-scoped)
    /// </summary>
    Task<RoleInfo?> GetRoleByIdAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all roles for current tenant
    /// </summary>
    Task<IEnumerable<RoleInfo>> GetTenantRolesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get role with its permissions
    /// </summary>
    Task<RoleInfo?> GetRoleWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Assign a role to a user in current tenant context
    /// </summary>
    Task AssignRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    Task RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get users assigned to a role (returns basic user info)
    /// </summary>
    Task<IEnumerable<UserInfo>> GetUsersInRoleAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get roles assigned to a user in current tenant
    /// </summary>
    Task<IEnumerable<RoleInfo>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a role name is available (not taken by another role in tenant)
    /// </summary>
    Task<bool> IsRoleNameAvailableAsync(string name, int? excludeRoleId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Create default roles for a new tenant
    /// </summary>
    Task CreateDefaultRolesForTenantAsync(int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get paged roles for current tenant with optional search functionality
    /// </summary>
    Task<PagedResult<RoleInfo>> GetTenantRolesPagedAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the permissions of a specific role
    /// </summary>
    Task<List<string>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update the permissions of a specific role
    /// </summary>
    Task UpdateRolePermissionsAsync(int roleId, List<string> permissions, CancellationToken cancellationToken = default);
}

/// <summary>
/// Basic role information for Phase 2 (will be replaced with actual Role entity in Phase 3)
/// </summary>
public class RoleInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public bool IsDefault { get; set; }
    public int TenantId { get; set; }
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Basic user information for role assignments (avoids User entity dependency)
/// </summary>
public class UserInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsActive { get; set; }
}

/// <summary>
/// Simple paged result (not wrapped in ApiResponseDto)
/// </summary>
public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
