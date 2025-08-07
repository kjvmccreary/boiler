namespace DTOs.Entities;

/// <summary>
/// Role entity for tenant-scoped role-based access control
/// </summary>
public class Role : BaseEntity
{
    /// <summary>
    /// Tenant this role belongs to. System roles have TenantId = null
    /// </summary>
    public int? TenantId { get; set; }

    /// <summary>
    /// Name of the role (e.g., "Admin", "User", "Manager")
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable description of what this role does
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// True if this is a system-defined role that cannot be deleted
    /// </summary>
    public bool IsSystemRole { get; set; } = false;

    /// <summary>
    /// True if this role is assigned to new users by default in this tenant
    /// </summary>
    public bool IsDefault { get; set; } = false;

    /// <summary>
    /// True if this role is currently active and can be assigned
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Tenant? Tenant { get; set; }
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
