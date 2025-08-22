using System.ComponentModel.DataAnnotations;

namespace DTOs.Entities;

/// <summary>
/// Audit entry for role modifications with before/after snapshots
/// Phase 11 - Enhanced Security & Monitoring
/// </summary>
public class RoleChangeAuditEntry : BaseEntity
{
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    public int RoleId { get; set; }
    
    [Required, MaxLength(100)]
    public string RoleName { get; set; } = string.Empty;
    
    [Required, MaxLength(50)]
    public string ChangeType { get; set; } = string.Empty; // Created, Updated, Deleted, PermissionsChanged
    
    /// <summary>
    /// JSON representation of the role state before change
    /// </summary>
    public string? OldValue { get; set; }
    
    /// <summary>
    /// JSON representation of the role state after change
    /// </summary>
    public string? NewValue { get; set; }
    
    [Required]
    public int ChangedByUserId { get; set; }
    
    [MaxLength(255)]
    public string? ChangedByEmail { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public Role Role { get; set; } = null!;
    public User ChangedByUser { get; set; } = null!;
}
