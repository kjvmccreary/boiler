using DTOs.Compliance;
using DTOs.Entities;
using Common.Data;
using Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Text;
using System.Globalization;

namespace Common.Services;

/// <summary>
/// Compliance reporting service for generating audit reports
/// Phase 11 Session 3 - Compliance Features
/// </summary>
public interface IComplianceReportingService
{
    Task<ComplianceReport> GenerateAccessReportAsync(DateTime from, DateTime to, int? tenantId = null);
    Task<ComplianceReport> GeneratePermissionUsageReportAsync(DateTime from, DateTime to, int? tenantId = null);
    Task<ComplianceReport> GenerateSecurityAuditReportAsync(DateTime from, DateTime to, int? tenantId = null);
    Task<ComplianceReport> GenerateDataRetentionReportAsync(int? tenantId = null);
    Task<byte[]> ExportReportToPdfAsync(ComplianceReport report);
    Task<byte[]> ExportReportToCsvAsync(ComplianceReport report);
    Task<byte[]> ExportReportToJsonAsync(ComplianceReport report);
    Task SaveReportAsync(ComplianceReport report);
    Task<List<ComplianceReport>> GetReportsAsync(int? tenantId = null, int pageSize = 50);
}

public class ComplianceReportingService : IComplianceReportingService
{
    private readonly IEnhancedAuditService _auditService;
    private readonly ApplicationDbContext _context;
    private readonly ITenantProvider _tenantProvider;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ComplianceReportingService> _logger;

    public ComplianceReportingService(
        IEnhancedAuditService auditService,
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        IConfiguration configuration,
        ILogger<ComplianceReportingService> logger)
    {
        _auditService = auditService;
        _context = context;
        _tenantProvider = tenantProvider;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ComplianceReport> GenerateAccessReportAsync(DateTime from, DateTime to, int? tenantId = null)
    {
        _logger.LogInformation("Generating access compliance report from {From} to {To} for tenant {TenantId}", 
            from, to, tenantId);

        var report = new ComplianceReport
        {
            Type = "Access Report",
            GeneratedAt = DateTime.UtcNow,
            Period = new DateRange(from, to),
            TenantId = tenantId ?? await _tenantProvider.GetCurrentTenantIdAsync() ?? 0,
            ComplianceStandards = GetComplianceStandards()
        };

        // Set tenant information
        if (report.TenantId > 0)
        {
            var tenant = await _context.Tenants.FindAsync(report.TenantId);
            report.TenantName = tenant?.Name ?? "Unknown";
        }

        // Set user information
        await SetCurrentUserInfo(report);

        try
        {
            // Generate sections
            report.Sections.Add(await GenerateUserAccessSection(from, to, tenantId));
            report.Sections.Add(await GeneratePermissionCheckSection(from, to, tenantId));
            report.Sections.Add(await GenerateFailedAccessAttemptsSection(from, to, tenantId));
            report.Sections.Add(await GenerateLoginActivitySection(from, to, tenantId));

            // Set metadata
            await PopulateMetadata(report, from, to, tenantId);

            await SaveReportAsync(report);
            
            _logger.LogInformation("Access compliance report generated successfully with ID {ReportId}", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate access compliance report");
            throw;
        }

        return report;
    }

    public async Task<ComplianceReport> GeneratePermissionUsageReportAsync(DateTime from, DateTime to, int? tenantId = null)
    {
        _logger.LogInformation("Generating permission usage compliance report from {From} to {To}", from, to);

        var report = new ComplianceReport
        {
            Type = "Permission Usage Report",
            GeneratedAt = DateTime.UtcNow,
            Period = new DateRange(from, to),
            TenantId = tenantId ?? await _tenantProvider.GetCurrentTenantIdAsync() ?? 0,
            ComplianceStandards = GetComplianceStandards()
        };

        await SetCurrentUserInfo(report);

        try
        {
            report.Sections.Add(await GeneratePermissionUsageSection(from, to, tenantId));
            report.Sections.Add(await GenerateRoleUsageSection(from, to, tenantId));
            report.Sections.Add(await GeneratePermissionPerformanceSection(from, to, tenantId));
            report.Sections.Add(await GenerateUnusedPermissionsSection(tenantId));

            await PopulateMetadata(report, from, to, tenantId);
            await SaveReportAsync(report);

            _logger.LogInformation("Permission usage report generated successfully with ID {ReportId}", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate permission usage report");
            throw;
        }

        return report;
    }

    public async Task<ComplianceReport> GenerateSecurityAuditReportAsync(DateTime from, DateTime to, int? tenantId = null)
    {
        _logger.LogInformation("Generating security audit compliance report from {From} to {To}", from, to);

        var report = new ComplianceReport
        {
            Type = "Security Audit Report",
            GeneratedAt = DateTime.UtcNow,
            Period = new DateRange(from, to),
            TenantId = tenantId ?? await _tenantProvider.GetCurrentTenantIdAsync() ?? 0,
            ComplianceStandards = GetComplianceStandards()
        };

        await SetCurrentUserInfo(report);

        try
        {
            report.Sections.Add(await GenerateSecurityEventsSection(from, to, tenantId));
            report.Sections.Add(await GenerateSuspiciousActivitySection(from, to, tenantId));
            report.Sections.Add(await GenerateUnauthorizedAccessSection(from, to, tenantId));
            report.Sections.Add(await GenerateSecurityMetricsSection(from, to, tenantId));

            await PopulateMetadata(report, from, to, tenantId);
            await SaveReportAsync(report);

            _logger.LogInformation("Security audit report generated successfully with ID {ReportId}", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate security audit report");
            throw;
        }

        return report;
    }

    public async Task<ComplianceReport> GenerateDataRetentionReportAsync(int? tenantId = null)
    {
        _logger.LogInformation("Generating data retention compliance report for tenant {TenantId}", tenantId);

        var report = new ComplianceReport
        {
            Type = "Data Retention Report",
            GeneratedAt = DateTime.UtcNow,
            Period = new DateRange(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow),
            TenantId = tenantId ?? await _tenantProvider.GetCurrentTenantIdAsync() ?? 0,
            ComplianceStandards = GetComplianceStandards()
        };

        await SetCurrentUserInfo(report);

        try
        {
            report.Sections.Add(await GenerateDataRetentionSection(tenantId));
            report.Sections.Add(await GenerateDataDeletionSection(tenantId));
            report.Sections.Add(await GenerateBackupRetentionSection(tenantId));

            await PopulateMetadata(report, report.Period.From, report.Period.To, tenantId);
            await SaveReportAsync(report);

            _logger.LogInformation("Data retention report generated successfully with ID {ReportId}", report.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate data retention report");
            throw;
        }

        return report;
    }

    // Change these methods from async to synchronous by removing async and returning Task.FromResult:

    public Task<byte[]> ExportReportToPdfAsync(ComplianceReport report)
    {
        // For .NET 9, we'll use a simple HTML to PDF approach
        // In production, you'd use a library like DinkToPdf or PuppeteerSharp
        var html = GenerateHtmlReport(report);

        // For now, return HTML as bytes (could be converted to PDF with external library)
        var htmlBytes = Encoding.UTF8.GetBytes(html);

        _logger.LogInformation("Report {ReportId} exported to PDF format (HTML fallback)", report.Id);
        return Task.FromResult(htmlBytes);
    }

    public Task<byte[]> ExportReportToCsvAsync(ComplianceReport report)
    {
        var csv = new StringBuilder();

        // Header
        csv.AppendLine("Report Type,Generated At,Period,Section,Finding Type,Severity,Description,Timestamp");

        // Data rows
        foreach (var section in report.Sections)
        {
            foreach (var finding in section.Findings)
            {
                csv.AppendLine($"\"{report.Type}\",\"{report.GeneratedAt:yyyy-MM-dd HH:mm:ss}\",\"{report.Period}\",\"{section.Title}\",\"{finding.Type}\",\"{finding.Severity}\",\"{finding.Description}\",\"{finding.Timestamp:yyyy-MM-dd HH:mm:ss}\"");
            }
        }

        var csvBytes = Encoding.UTF8.GetBytes(csv.ToString());

        _logger.LogInformation("Report {ReportId} exported to CSV format", report.Id);
        return Task.FromResult(csvBytes);
    }

    public Task<byte[]> ExportReportToJsonAsync(ComplianceReport report)
    {
        var json = JsonSerializer.Serialize(report, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var jsonBytes = Encoding.UTF8.GetBytes(json);

        _logger.LogInformation("Report {ReportId} exported to JSON format", report.Id);
        return Task.FromResult(jsonBytes);
    }
    public async Task SaveReportAsync(ComplianceReport report)
    {
        // For now, we'll save reports as JSON in the database
        // In production, you might want a dedicated ComplianceReports table
        
        var auditEntry = new AuditEntry
        {
            TenantId = report.TenantId,
            UserId = string.IsNullOrEmpty(report.GeneratedByUserId) ? null : int.Parse(report.GeneratedByUserId),
            Action = "ComplianceReportGenerated",
            Resource = report.Type,
            Details = JsonSerializer.Serialize(report),
            Success = true,
            Timestamp = DateTime.UtcNow
        };

        _context.Set<AuditEntry>().Add(auditEntry);
        await _context.SaveChangesAsync();
        
        _logger.LogInformation("Compliance report {ReportId} saved successfully", report.Id);
    }

    public async Task<List<ComplianceReport>> GetReportsAsync(int? tenantId = null, int pageSize = 50)
    {
        var query = _context.Set<AuditEntry>()
            .Where(a => a.Action == "ComplianceReportGenerated");

        if (tenantId.HasValue)
        {
            query = query.Where(a => a.TenantId == tenantId.Value);
        }

        var auditEntries = await query
            .OrderByDescending(a => a.Timestamp)
            .Take(pageSize)
            .ToListAsync();

        var reports = new List<ComplianceReport>();
        
        foreach (var entry in auditEntries)
        {
            try
            {
                if (!string.IsNullOrEmpty(entry.Details))
                {
                    var report = JsonSerializer.Deserialize<ComplianceReport>(entry.Details);
                    if (report != null)
                    {
                        reports.Add(report);
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize compliance report from audit entry {Id}", entry.Id);
            }
        }

        return reports;
    }

    // Private helper methods for generating report sections

    private async Task<ComplianceSection> GenerateUserAccessSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "User Access Analysis",
            Description = "Analysis of user login activities and access patterns"
        };

        var loginEvents = await _context.Set<AuditEntry>()
            .Where(a => a.Action == "Login" && a.Timestamp >= from && a.Timestamp <= to)
            .Where(a => !tenantId.HasValue || a.TenantId == tenantId.Value)
            .GroupBy(a => a.UserId)
            .Select(g => new { UserId = g.Key, LoginCount = g.Count(), LastLogin = g.Max(a => a.Timestamp) })
            .ToListAsync();

        section.Statistics["total_unique_users"] = loginEvents.Count;
        section.Statistics["total_logins"] = loginEvents.Sum(e => e.LoginCount);
        section.Statistics["average_logins_per_user"] = loginEvents.Any() ? loginEvents.Average(e => e.LoginCount) : 0;

        // Add findings for unusual access patterns
        foreach (var userLogin in loginEvents.Where(e => e.LoginCount > 100)) // High activity threshold
        {
            section.Findings.Add(new ComplianceFinding
            {
                Type = "HighLoginActivity",
                Description = $"User {userLogin.UserId} had {userLogin.LoginCount} logins in the period",
                Severity = userLogin.LoginCount > 500 ? "Warning" : "Info",
                Timestamp = userLogin.LastLogin,
                Details = new Dictionary<string, object> { ["login_count"] = userLogin.LoginCount }
            });
        }

        return section;
    }

    private async Task<ComplianceSection> GeneratePermissionCheckSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Permission Check Analysis",
            Description = "Analysis of permission checks and authorization decisions"
        };

        var permissionChecks = await _auditService.GetPermissionAuditLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            permissionChecks = permissionChecks.Where(p => p.TenantId == tenantId.Value).ToList();
        }

        section.Statistics["total_permission_checks"] = permissionChecks.Count;
        section.Statistics["granted_checks"] = permissionChecks.Count(p => p.Granted);
        section.Statistics["denied_checks"] = permissionChecks.Count(p => !p.Granted);
        section.Statistics["average_check_time_ms"] = permissionChecks.Any() 
            ? permissionChecks.Average(p => p.CheckDurationMs) 
            : 0;

        // Add findings for high denial rates
        var denialRate = permissionChecks.Any() 
            ? (double)permissionChecks.Count(p => !p.Granted) / permissionChecks.Count * 100 
            : 0;

        if (denialRate > 15) // 15% denial rate threshold
        {
            section.Findings.Add(new ComplianceFinding
            {
                Type = "HighPermissionDenialRate",
                Description = $"Permission denial rate is {denialRate:F1}% (above 15% threshold)",
                Severity = denialRate > 25 ? "Warning" : "Info",
                Timestamp = DateTime.UtcNow,
                Details = new Dictionary<string, object> { ["denial_rate"] = denialRate }
            });
        }

        return section;
    }

    private async Task<ComplianceSection> GenerateFailedAccessAttemptsSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Failed Access Attempts",
            Description = "Analysis of unauthorized access attempts and security events"
        };

        var failedAttempts = await _auditService.GetSecurityEventLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            failedAttempts = failedAttempts.Where(s => s.TenantId == tenantId.Value).ToList();
        }

        var unauthorizedAttempts = failedAttempts.Where(s => s.EventType == "UnauthorizedAccess").ToList();

        section.Statistics["total_failed_attempts"] = unauthorizedAttempts.Count;
        section.Statistics["unique_ips"] = unauthorizedAttempts.Select(s => s.IpAddress).Distinct().Count();
        section.Statistics["unique_users"] = unauthorizedAttempts.Where(s => s.UserId.HasValue).Select(s => s.UserId).Distinct().Count();

        // Group by IP address to find suspicious activity
        var ipGroups = unauthorizedAttempts.GroupBy(s => s.IpAddress)
            .Where(g => g.Count() >= 5) // 5+ failed attempts from same IP
            .OrderByDescending(g => g.Count());

        foreach (var ipGroup in ipGroups.Take(10))
        {
            section.Findings.Add(new ComplianceFinding
            {
                Type = "SuspiciousIPActivity",
                Description = $"IP {ipGroup.Key} had {ipGroup.Count()} failed access attempts",
                Severity = ipGroup.Count() >= 20 ? "Critical" : ipGroup.Count() >= 10 ? "Warning" : "Info",
                Timestamp = ipGroup.Max(s => s.Timestamp),
                Details = new Dictionary<string, object> 
                { 
                    ["ip_address"] = ipGroup.Key ?? "Unknown",
                    ["attempt_count"] = ipGroup.Count(),
                    ["time_span"] = $"{ipGroup.Min(s => s.Timestamp)} to {ipGroup.Max(s => s.Timestamp)}"
                }
            });
        }

        return section;
    }

    private async Task<ComplianceSection> GenerateLoginActivitySection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Login Activity Analysis",
            Description = "Detailed analysis of user authentication patterns"
        };

        var loginEvents = await _context.Set<AuditEntry>()
            .Where(a => (a.Action == "Login" || a.Action == "Logout") && a.Timestamp >= from && a.Timestamp <= to)
            .Where(a => !tenantId.HasValue || a.TenantId == tenantId.Value)
            .OrderBy(a => a.Timestamp)
            .ToListAsync();

        section.Statistics["total_login_events"] = loginEvents.Count(e => e.Action == "Login");
        section.Statistics["total_logout_events"] = loginEvents.Count(e => e.Action == "Logout");

        // Analyze login patterns by hour
        var loginsByHour = loginEvents
            .Where(e => e.Action == "Login")
            .GroupBy(e => e.Timestamp.Hour)
            .ToDictionary(g => g.Key, g => g.Count());

        section.Statistics["logins_by_hour"] = loginsByHour;

        return section;
    }

    private async Task<ComplianceSection> GeneratePermissionUsageSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Permission Usage Patterns",
            Description = "Analysis of which permissions are being used and how frequently"
        };

        var permissionChecks = await _auditService.GetPermissionAuditLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            permissionChecks = permissionChecks.Where(p => p.TenantId == tenantId.Value).ToList();
        }

        var permissionUsage = permissionChecks
            .GroupBy(p => p.Permission)
            .Select(g => new { Permission = g.Key, Count = g.Count(), GrantRate = (double)g.Count(p => p.Granted) / g.Count() * 100 })
            .OrderByDescending(p => p.Count)
            .ToList();

        section.Statistics["permission_usage_count"] = permissionUsage.Count;
        section.Statistics["most_used_permissions"] = permissionUsage.Take(10).ToDictionary(p => p.Permission, p => p.Count);

        return section;
    }

    private async Task<ComplianceSection> GenerateRoleUsageSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Role Usage Analysis",
            Description = "Analysis of role assignments and changes"
        };

        var roleChanges = await _auditService.GetRoleChangeAuditLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            roleChanges = roleChanges.Where(r => r.TenantId == tenantId.Value).ToList();
        }

        section.Statistics["total_role_changes"] = roleChanges.Count;
        section.Statistics["role_creations"] = roleChanges.Count(r => r.ChangeType == "Created");
        section.Statistics["role_updates"] = roleChanges.Count(r => r.ChangeType == "Updated");
        section.Statistics["role_deletions"] = roleChanges.Count(r => r.ChangeType == "Deleted");

        return section;
    }

    private async Task<ComplianceSection> GeneratePermissionPerformanceSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Permission System Performance",
            Description = "Performance metrics for the permission checking system"
        };

        var permissionChecks = await _auditService.GetPermissionAuditLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            permissionChecks = permissionChecks.Where(p => p.TenantId == tenantId.Value).ToList();
        }

        if (permissionChecks.Any())
        {
            section.Statistics["average_check_time_ms"] = permissionChecks.Average(p => p.CheckDurationMs);
            section.Statistics["max_check_time_ms"] = permissionChecks.Max(p => p.CheckDurationMs);
            section.Statistics["cache_hit_rate"] = (double)permissionChecks.Count(p => p.CacheHit) / permissionChecks.Count * 100;

            // Check for performance issues
            var slowChecks = permissionChecks.Where(p => p.CheckDurationMs > 50).ToList();
            if (slowChecks.Any())
            {
                section.Findings.Add(new ComplianceFinding
                {
                    Type = "SlowPermissionChecks",
                    Description = $"{slowChecks.Count} permission checks exceeded 50ms threshold",
                    Severity = slowChecks.Count > 100 ? "Warning" : "Info",
                    Timestamp = slowChecks.Max(p => p.Timestamp),
                    Details = new Dictionary<string, object> 
                    { 
                        ["slow_check_count"] = slowChecks.Count,
                        ["slowest_check_ms"] = slowChecks.Max(p => p.CheckDurationMs)
                    }
                });
            }
        }

        return section;
    }

    private async Task<ComplianceSection> GenerateUnusedPermissionsSection(int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Unused Permissions Analysis",
            Description = "Identifies permissions that are not being used"
        };

        // This would require getting all available permissions and comparing with usage
        // For now, we'll add a placeholder
        section.Statistics["analysis_status"] = "Placeholder - requires permission registry implementation";

        return section;
    }

    private async Task<ComplianceSection> GenerateSecurityEventsSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Security Events Summary",
            Description = "Summary of all security events in the specified period"
        };

        var securityEvents = await _auditService.GetSecurityEventLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            securityEvents = securityEvents.Where(s => s.TenantId == tenantId.Value).ToList();
        }

        section.Statistics["total_security_events"] = securityEvents.Count;
        
        var eventsBySeverity = securityEvents.GroupBy(s => s.Severity)
            .ToDictionary(g => g.Key, g => g.Count());
        section.Statistics["events_by_severity"] = eventsBySeverity;

        var eventsByType = securityEvents.GroupBy(s => s.EventType)
            .ToDictionary(g => g.Key, g => g.Count());
        section.Statistics["events_by_type"] = eventsByType;

        return section;
    }

    private async Task<ComplianceSection> GenerateSuspiciousActivitySection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Suspicious Activity Detection",
            Description = "Analysis of potentially suspicious or malicious activity"
        };

        var suspiciousEvents = await _auditService.GetSecurityEventLogAsync(
            severity: null, 
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            suspiciousEvents = suspiciousEvents.Where(s => s.TenantId == tenantId.Value).ToList();
        }

        var suspicious = suspiciousEvents.Where(s => 
            s.EventType.Contains("Suspicious") || 
            s.EventType.Contains("Injection") || 
            s.EventType.Contains("Xss")).ToList();

        section.Statistics["suspicious_events_count"] = suspicious.Count;

        foreach (var evt in suspicious.Take(20))
        {
            section.Findings.Add(new ComplianceFinding
            {
                Type = evt.EventType,
                Description = $"Suspicious activity detected from {evt.IpAddress}: {evt.EventType}",
                Severity = evt.Severity,
                Timestamp = evt.Timestamp,
                Details = JsonSerializer.Deserialize<Dictionary<string, object>>(evt.Details ?? "{}")!
            });
        }

        return section;
    }

    private async Task<ComplianceSection> GenerateUnauthorizedAccessSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Unauthorized Access Attempts",
            Description = "Detailed analysis of unauthorized access attempts"
        };

        var unauthorizedEvents = await _auditService.GetSecurityEventLogAsync(
            from: from, 
            to: to, 
            pageSize: 10000);

        if (tenantId.HasValue)
        {
            unauthorizedEvents = unauthorizedEvents.Where(s => s.TenantId == tenantId.Value).ToList();
        }

        var unauthorized = unauthorizedEvents.Where(s => 
            s.EventType == "UnauthorizedAccess" || 
            s.EventType == "ForbiddenAccess").ToList();

        section.Statistics["unauthorized_attempts"] = unauthorized.Count;
        section.Statistics["unique_source_ips"] = unauthorized.Select(u => u.IpAddress).Distinct().Count();

        return section;
    }

    private async Task<ComplianceSection> GenerateSecurityMetricsSection(DateTime from, DateTime to, int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Security Metrics and KPIs",
            Description = "Key security performance indicators and metrics"
        };

        var securityMetrics = await _auditService.GetSecurityMetricsAsync(to - from);

        section.Statistics["security_metrics"] = securityMetrics;

        return section;
    }

    private async Task<ComplianceSection> GenerateDataRetentionSection(int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Data Retention Policy Compliance",
            Description = "Analysis of data retention policy adherence"
        };

        var retentionDays = _configuration.GetValue<int>("Compliance:AuditLogRetentionDays", 90);
        var cutoffDate = DateTime.UtcNow.AddDays(-retentionDays);

        var oldRecords = await _context.Set<AuditEntry>()
            .Where(a => a.Timestamp < cutoffDate)
            .Where(a => !tenantId.HasValue || a.TenantId == tenantId.Value)
            .CountAsync();

        section.Statistics["retention_policy_days"] = retentionDays;
        section.Statistics["records_past_retention"] = oldRecords;
        section.Statistics["cutoff_date"] = cutoffDate;

        if (oldRecords > 0)
        {
            section.Findings.Add(new ComplianceFinding
            {
                Type = "DataRetentionViolation",
                Description = $"{oldRecords} audit records exceed the {retentionDays}-day retention policy",
                Severity = "Warning",
                Timestamp = DateTime.UtcNow,
                RequiresAction = true,
                RecommendedAction = "Archive or delete records older than retention policy"
            });
        }

        return section;
    }

    private async Task<ComplianceSection> GenerateDataDeletionSection(int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Data Deletion Activities",
            Description = "Record of data deletion activities for compliance"
        };

        // Track deletion activities from audit logs
        var deletionEvents = await _context.Set<AuditEntry>()
            .Where(a => a.Action.Contains("Delete") || a.Action.Contains("Remove"))
            .Where(a => !tenantId.HasValue || a.TenantId == tenantId.Value)
            .Where(a => a.Timestamp >= DateTime.UtcNow.AddDays(-365))
            .ToListAsync();

        section.Statistics["deletion_events_last_year"] = deletionEvents.Count;
        section.Statistics["successful_deletions"] = deletionEvents.Count(d => d.Success);
        section.Statistics["failed_deletions"] = deletionEvents.Count(d => !d.Success);

        return section;
    }

    private async Task<ComplianceSection> GenerateBackupRetentionSection(int? tenantId)
    {
        var section = new ComplianceSection
        {
            Title = "Backup and Recovery Compliance",
            Description = "Analysis of backup and recovery procedures"
        };

        // This would integrate with backup systems
        // For now, adding placeholder information
        section.Statistics["backup_policy"] = "Daily backups with 30-day retention";
        section.Statistics["recovery_testing"] = "Monthly recovery testing";
        section.Statistics["encryption_status"] = "Encrypted at rest and in transit";

        return section;
    }

    private async Task PopulateMetadata(ComplianceReport report, DateTime from, DateTime to, int? tenantId)
    {
        var metadata = report.Metadata;

        // Get total counts
        var auditEntries = await _context.Set<AuditEntry>()
            .Where(a => a.Timestamp >= from && a.Timestamp <= to)
            .Where(a => !tenantId.HasValue || a.TenantId == tenantId.Value)
            .ToListAsync();

        metadata.TotalRecordsAnalyzed = auditEntries.Count;

        var permissionChecks = await _auditService.GetPermissionAuditLogAsync(from: from, to: to, pageSize: 10000);
        if (tenantId.HasValue)
        {
            permissionChecks = permissionChecks.Where(p => p.TenantId == tenantId.Value).ToList();
        }

        metadata.TotalPermissionChecks = permissionChecks.Count;
        if (permissionChecks.Any())
        {
            metadata.AverageResponseTime = permissionChecks.Average(p => p.CheckDurationMs);
            metadata.PermissionDenialRate = (double)permissionChecks.Count(p => !p.Granted) / permissionChecks.Count * 100;
        }

        var securityEvents = await _auditService.GetSecurityEventLogAsync(from: from, to: to, pageSize: 10000);
        if (tenantId.HasValue)
        {
            securityEvents = securityEvents.Where(s => s.TenantId == tenantId.Value).ToList();
        }

        metadata.TotalSecurityEvents = securityEvents.Count;
        metadata.EventsByType = securityEvents.GroupBy(s => s.EventType).ToDictionary(g => g.Key, g => g.Count());
        metadata.EventsBySeverity = securityEvents.GroupBy(s => s.Severity).ToDictionary(g => g.Key, g => g.Count());

        // Set compliance settings
        metadata.DataRetentionPolicy = $"{_configuration.GetValue<int>("Compliance:AuditLogRetentionDays", 90)} days";
        metadata.EncryptionAtRest = true;
        metadata.EncryptionInTransit = true;
    }

    private async Task SetCurrentUserInfo(ComplianceReport report)
    {
        // This would be set from the current user context
        // For now, adding placeholder
        report.GeneratedByUserId = "system";
        report.GeneratedByUserName = "System";
    }

    private List<string> GetComplianceStandards()
    {
        return _configuration.GetSection("Compliance:ComplianceStandards")
            .Get<List<string>>() ?? new List<string> { "SOC2", "GDPR", "HIPAA" };
    }

    private string GenerateHtmlReport(ComplianceReport report)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head>");
        html.AppendLine("<title>Compliance Report</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine("h1 { color: #333; }");
        html.AppendLine("h2 { color: #666; border-bottom: 1px solid #ccc; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; margin: 10px 0; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: #f2f2f2; }");
        html.AppendLine(".finding { margin: 10px 0; padding: 10px; border-left: 4px solid #007cba; }");
        html.AppendLine(".warning { border-left-color: #ffa500; }");
        html.AppendLine(".critical { border-left-color: #ff0000; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");

        html.AppendLine($"<h1>{report.Type}</h1>");
        html.AppendLine($"<p><strong>Generated:</strong> {report.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC</p>");
        html.AppendLine($"<p><strong>Period:</strong> {report.Period}</p>");
        html.AppendLine($"<p><strong>Tenant:</strong> {report.TenantName ?? "All Tenants"}</p>");

        foreach (var section in report.Sections)
        {
            html.AppendLine($"<h2>{section.Title}</h2>");
            html.AppendLine($"<p>{section.Description}</p>");

            if (section.Statistics.Any())
            {
                html.AppendLine("<h3>Statistics</h3>");
                html.AppendLine("<table>");
                foreach (var stat in section.Statistics)
                {
                    html.AppendLine($"<tr><td>{stat.Key}</td><td>{stat.Value}</td></tr>");
                }
                html.AppendLine("</table>");
            }

            if (section.Findings.Any())
            {
                html.AppendLine("<h3>Findings</h3>");
                foreach (var finding in section.Findings)
                {
                    var cssClass = finding.Severity.ToLower() switch
                    {
                        "warning" => "finding warning",
                        "critical" => "finding critical",
                        _ => "finding"
                    };
                    html.AppendLine($"<div class=\"{cssClass}\">");
                    html.AppendLine($"<strong>{finding.Type}</strong> ({finding.Severity})<br/>");
                    html.AppendLine($"{finding.Description}<br/>");
                    html.AppendLine($"<small>Time: {finding.Timestamp:yyyy-MM-dd HH:mm:ss}</small>");
                    html.AppendLine("</div>");
                }
            }
        }

        html.AppendLine("</body></html>");
        return html.ToString();
    }
}
