namespace DTOs.Entities;

/// <summary>
/// Junction table linking users to their roles within specific tenants
/// </summary>
public class UserRole : TenantEntity
{
    /// <summary>
    /// ID of the user
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID of the role
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// When this role was assigned to the user
    /// </summary>
    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// When this role assignment expires (null = never expires)
    /// </summary>
    public DateTime? ExpiresAt { get; set; }

    /// <summary>
    /// Who assigned this role (for audit purposes)
    /// </summary>
    public string? AssignedBy { get; set; }

    /// <summary>
    /// True if this role assignment is currently active
    /// </summary>
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public User User { get; set; } = null!;
    public Role Role { get; set; } = null!;
}
