using DTOs.Entities;
using DTOs.Common;

namespace Contracts.Services;

public interface IRoleService
{
    Task<RoleInfo> CreateRoleAsync(string name, string description, List<string> permissions, CancellationToken cancellationToken = default);
    
    // 🔧 .NET 9 FIX: Change return type to bool for UpdateRoleAsync
    Task<bool> UpdateRoleAsync(int roleId, string name, string description, List<string> permissions, CancellationToken cancellationToken = default);
    
    // 🔧 .NET 9 FIX: Change return type to bool for DeleteRoleAsync  
    Task<bool> DeleteRoleAsync(int roleId, CancellationToken cancellationToken = default);
    
    Task<RoleInfo?> GetRoleByIdAsync(int roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleInfo>> GetTenantRolesAsync(CancellationToken cancellationToken = default);
    Task<RoleInfo?> GetRoleWithPermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    
    // 🔧 .NET 9 FIX: Change return type to void for AssignRoleToUserAsync
    Task AssignRoleToUserAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    
    // 🔧 .NET 9 FIX: Change return type to void for RemoveRoleFromUserAsync
    Task RemoveRoleFromUserAsync(int userId, int roleId, CancellationToken cancellationToken = default);
    
    Task<IEnumerable<UserInfo>> GetUsersInRoleAsync(int roleId, CancellationToken cancellationToken = default);
    Task<IEnumerable<RoleInfo>> GetUserRolesAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> IsRoleNameAvailableAsync(string name, int? excludeRoleId = null, CancellationToken cancellationToken = default);
    Task CreateDefaultRolesForTenantAsync(int tenantId, CancellationToken cancellationToken = default);
    Task<PagedResult<RoleInfo>> GetTenantRolesPagedAsync(int page, int pageSize, string? searchTerm = null, CancellationToken cancellationToken = default);
    Task<List<string>> GetRolePermissionsAsync(int roleId, CancellationToken cancellationToken = default);
    
    // 🔧 .NET 9 FIX: Change return type to bool for UpdateRolePermissionsAsync
    Task<bool> UpdateRolePermissionsAsync(int roleId, List<string> permissions, CancellationToken cancellationToken = default);
}

public class RoleInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsSystemRole { get; set; }
    public bool IsDefault { get; set; }
    public int? TenantId { get; set; } // 🔧 .NET 9 FIX: Make nullable for system roles
    public List<string> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class UserInfo
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}".Trim();
    public bool IsActive { get; set; }
    
    // 🔧 FIX: Add TenantId property
    public int TenantId { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = new List<T>();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
}
