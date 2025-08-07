namespace DTOs.Entities;

/// <summary>
/// Junction table linking roles to their permissions
/// </summary>
public class RolePermission : BaseEntity
{
    /// <summary>
    /// ID of the role
    /// </summary>
    public int RoleId { get; set; }

    /// <summary>
    /// ID of the permission
    /// </summary>
    public int PermissionId { get; set; }

    /// <summary>
    /// When this permission was granted to the role
    /// </summary>
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Who granted this permission (for audit purposes)
    /// </summary>
    public string? GrantedBy { get; set; }

    // Navigation properties
    public Role Role { get; set; } = null!;
    public Permission Permission { get; set; } = null!;
}
