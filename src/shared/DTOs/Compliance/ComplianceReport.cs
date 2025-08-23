using System.ComponentModel.DataAnnotations;

namespace DTOs.Compliance;

/// <summary>
/// Compliance report DTO for audit and regulatory purposes
/// Phase 11 Session 3 - Compliance Features
/// </summary>
public class ComplianceReport
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    
    public DateRange Period { get; set; } = new();
    
    public string? GeneratedByUserId { get; set; }
    
    public string? GeneratedByUserName { get; set; }
    
    public int TenantId { get; set; }
    
    public string? TenantName { get; set; }
    
    public List<ComplianceSection> Sections { get; set; } = new();
    
    public ComplianceMetadata Metadata { get; set; } = new();
    
    public string Status { get; set; } = "Generated";
    
    public DateTime? ExportedAt { get; set; }
    
    public List<string> ComplianceStandards { get; set; } = new();
}

public class DateRange
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    
    public DateRange() { }
    
    public DateRange(DateTime from, DateTime to)
    {
        From = from;
        To = to;
    }
    
    public TimeSpan Duration => To - From;
    
    public override string ToString() => $"{From:yyyy-MM-dd} to {To:yyyy-MM-dd}";
}

public class ComplianceSection
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<ComplianceFinding> Findings { get; set; } = new();
    public Dictionary<string, object> Statistics { get; set; } = new();
    public string Status { get; set; } = "Normal"; // Normal, Warning, Critical
}

public class ComplianceFinding
{
    public string Type { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info"; // Info, Warning, Critical
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Details { get; set; } = new();
    public bool RequiresAction { get; set; }
    public string? RecommendedAction { get; set; }
}

public class ComplianceMetadata
{
    public int TotalRecordsAnalyzed { get; set; }
    public int TotalUsers { get; set; }
    public int TotalPermissionChecks { get; set; }
    public int TotalSecurityEvents { get; set; }
    public double AverageResponseTime { get; set; }
    public double PermissionDenialRate { get; set; }
    public Dictionary<string, int> EventsByType { get; set; } = new();
    public Dictionary<string, int> EventsBySeverity { get; set; } = new();
    public string DataRetentionPolicy { get; set; } = "90 days";
    public bool EncryptionAtRest { get; set; } = true;
    public bool EncryptionInTransit { get; set; } = true;
}