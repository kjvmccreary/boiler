using System.ComponentModel.DataAnnotations;

namespace DTOs.Entities;

/// <summary>
/// Audit entry specifically for permission checks with performance metrics.
/// Phase 11 - Enhanced Security and Monitoring.
/// </summary>
public class PermissionAuditEntry : BaseEntity
{
    [Required]
    public int TenantId { get; set; }
    
    [Required]
    public int UserId { get; set; }
    
    [Required, MaxLength(100)]
    public string Permission { get; set; } = string.Empty;
    
    [Required, MaxLength(255)]
    public string Resource { get; set; } = string.Empty;
    
    public bool Granted { get; set; }
    
    [MaxLength(500)]
    public string? DenialReason { get; set; }
    
    /// <summary>
    /// Time taken to perform the permission check in milliseconds
    /// </summary>
    public double CheckDurationMs { get; set; }
    
    /// <summary>
    /// Whether the permission was resolved from cache
    /// </summary>
    public bool CacheHit { get; set; }
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}
