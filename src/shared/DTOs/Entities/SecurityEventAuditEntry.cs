using System.ComponentModel.DataAnnotations;

namespace DTOs.Entities;

/// <summary>
/// Audit entry for security events and violations
/// Phase 11 - Enhanced Security & Monitoring
/// </summary>
public class SecurityEventAuditEntry : BaseEntity
{
    [Required]
    public int TenantId { get; set; }
    
    [Required, MaxLength(100)]
    public string EventType { get; set; } = string.Empty; // UnauthorizedAccess, SuspiciousActivity, RateLimitExceeded, etc.
    
    [MaxLength(50)]
    public string? IpAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    public int? UserId { get; set; }
    
    [MaxLength(255)]
    public string? UserEmail { get; set; }
    
    /// <summary>
    /// JSON object containing event-specific details
    /// </summary>
    public string? Details { get; set; }
    
    [Required, MaxLength(20)]
    public string Severity { get; set; } = "Medium"; // Low, Medium, High, Critical
    
    [MaxLength(255)]
    public string? Resource { get; set; }
    
    [MaxLength(100)]
    public string? Action { get; set; }
    
    [Required]
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Whether this event has been investigated
    /// </summary>
    public bool Investigated { get; set; } = false;
    
    [MaxLength(1000)]
    public string? InvestigationNotes { get; set; }

    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public User? User { get; set; }
}
