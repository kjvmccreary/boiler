namespace DTOs.Entities;

public class AuditEntry : TenantEntity
{
    public int? UserId { get; set; }
    public string Action { get; set; } = string.Empty;
    public string Resource { get; set; } = string.Empty;
    public string? ResourceId { get; set; } // ID of affected resource
    public string? Details { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public User? User { get; set; }
    public Tenant? Tenant { get; set; }
}
