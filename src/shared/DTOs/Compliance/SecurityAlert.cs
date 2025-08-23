using System.ComponentModel.DataAnnotations;

namespace DTOs.Compliance;

/// <summary>
/// Security alert DTO for compliance and monitoring
/// Phase 11 Session 3 - Compliance Features
/// </summary>
public class SecurityAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public AlertSeverity Severity { get; set; }
    
    [Required]
    public string Message { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime? ResolvedAt { get; set; }
    
    public bool IsResolved => ResolvedAt.HasValue;
    
    public string? ResolvedBy { get; set; }
    
    public string? ResolutionNotes { get; set; }
    
    public int TenantId { get; set; }
    
    public int? UserId { get; set; }
    
    public string? IpAddress { get; set; }
    
    public string? UserAgent { get; set; }
    
    public Dictionary<string, object> Details { get; set; } = new();
    
    public List<string> Tags { get; set; } = new();
    
    public bool NotificationSent { get; set; }
    
    public DateTime? NotificationSentAt { get; set; }
    
    public List<string> NotificationRecipients { get; set; } = new();
    
    public string Source { get; set; } = "System";
    
    public string? CorrelationId { get; set; }
    
    public int Priority => Severity switch
    {
        AlertSeverity.Low => 1,
        AlertSeverity.Medium => 2,
        AlertSeverity.High => 3,
        AlertSeverity.Critical => 4,
        _ => 1
    };
}

public enum AlertSeverity
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

public class AlertConfiguration
{
    public bool EnableEmailNotifications { get; set; } = true;
    public bool EnableSlackNotifications { get; set; }
    public List<string> EmailRecipients { get; set; } = new();
    public string? SlackWebhookUrl { get; set; }
    public AlertSeverity MinimumSeverityForNotification { get; set; } = AlertSeverity.Medium;
    public TimeSpan NotificationCooldown { get; set; } = TimeSpan.FromMinutes(15);
    public Dictionary<string, AlertSeverity> AlertTypeOverrides { get; set; } = new();
}