namespace DTOs.Entities;

/// <summary>
/// Permission entity defining what actions can be performed in the system
/// These are system-wide and not tenant-specific
/// </summary>
public class Permission : BaseEntity
{
    /// <summary>
    /// Unique name/key for the permission (e.g., "users.view", "reports.create")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Category this permission belongs to (e.g., "Users", "Reports", "System")
    /// </summary>
    public string Category { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of what this permission allows
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// True if this permission is currently available for assignment
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
