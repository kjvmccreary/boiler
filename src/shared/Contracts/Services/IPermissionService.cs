namespace Contracts.Services;

/// <summary>
/// Service for checking user permissions and managing permission-based authorization
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Check if a user has a specific permission in their current tenant context
    /// </summary>
    Task<bool> UserHasPermissionAsync(int userId, string permission, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all permissions for a user in their current tenant context
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionsAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get user permissions for a specific tenant
    /// </summary>
    Task<IEnumerable<string>> GetUserPermissionsForTenantAsync(int userId, int tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has any of the specified permissions
    /// </summary>
    Task<bool> UserHasAnyPermissionAsync(int userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if user has all of the specified permissions
    /// </summary>
    Task<bool> UserHasAllPermissionsAsync(int userId, IEnumerable<string> permissions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all available permissions in the system (for admin UI)
    /// </summary>
    Task<IEnumerable<PermissionInfo>> GetAllAvailablePermissionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Get permissions grouped by category (for UI display)
    /// </summary>
    Task<Dictionary<string, List<PermissionInfo>>> GetPermissionsByCategoryAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Information about a permission for UI display
/// </summary>
public class PermissionInfo
{
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
