using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.Services;
using Common.Authorization;
using DTOs.Compliance;
using DTOs.Common;

namespace UserService.Controllers;

/// <summary>
/// Controller for compliance reporting and security alerts
/// Phase 11 Session 3 - Compliance Features
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ComplianceController : ControllerBase
{
    private readonly IComplianceReportingService _complianceService;
    private readonly ISecurityAlertService _alertService;
    private readonly ILogger<ComplianceController> _logger;

    public ComplianceController(
        IComplianceReportingService complianceService,
        ISecurityAlertService alertService,
        ILogger<ComplianceController> logger)
    {
        _complianceService = complianceService;
        _alertService = alertService;
        _logger = logger;
    }

    /// <summary>
    /// Generate an access compliance report
    /// </summary>
    [HttpPost("reports/access")]
    [RequiresPermission("compliance.generate_reports")]
    public async Task<IActionResult> GenerateAccessReport([FromBody] GenerateReportRequest request)
    {
        try
        {
            var report = await _complianceService.GenerateAccessReportAsync(
                request.From, 
                request.To, 
                request.TenantId);

            return Ok(new ApiResponseDto<ComplianceReport>
            {
                Success = true,
                Message = "Access compliance report generated successfully",
                Data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate access compliance report");
            return StatusCode(500, new ApiResponseDto<ComplianceReport>
            {
                Success = false,
                Message = "Failed to generate access compliance report"
            });
        }
    }

    /// <summary>
    /// Generate a permission usage compliance report
    /// </summary>
    [HttpPost("reports/permissions")]
    [RequiresPermission("compliance.generate_reports")]
    public async Task<IActionResult> GeneratePermissionUsageReport([FromBody] GenerateReportRequest request)
    {
        try
        {
            var report = await _complianceService.GeneratePermissionUsageReportAsync(
                request.From, 
                request.To, 
                request.TenantId);

            return Ok(new ApiResponseDto<ComplianceReport>
            {
                Success = true,
                Message = "Permission usage report generated successfully",
                Data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate permission usage report");
            return StatusCode(500, new ApiResponseDto<ComplianceReport>
            {
                Success = false,
                Message = "Failed to generate permission usage report"
            });
        }
    }

    /// <summary>
    /// Generate a security audit compliance report
    /// </summary>
    [HttpPost("reports/security")]
    [RequiresPermission("compliance.generate_reports")]
    public async Task<IActionResult> GenerateSecurityAuditReport([FromBody] GenerateReportRequest request)
    {
        try
        {
            var report = await _complianceService.GenerateSecurityAuditReportAsync(
                request.From, 
                request.To, 
                request.TenantId);

            return Ok(new ApiResponseDto<ComplianceReport>
            {
                Success = true,
                Message = "Security audit report generated successfully",
                Data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate security audit report");
            return StatusCode(500, new ApiResponseDto<ComplianceReport>
            {
                Success = false,
                Message = "Failed to generate security audit report"
            });
        }
    }

    /// <summary>
    /// Generate a data retention compliance report
    /// </summary>
    [HttpPost("reports/retention")]
    [RequiresPermission("compliance.generate_reports")]
    public async Task<IActionResult> GenerateDataRetentionReport([FromQuery] int? tenantId = null)
    {
        try
        {
            var report = await _complianceService.GenerateDataRetentionReportAsync(tenantId);

            return Ok(new ApiResponseDto<ComplianceReport>
            {
                Success = true,
                Message = "Data retention report generated successfully",
                Data = report
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate data retention report");
            return StatusCode(500, new ApiResponseDto<ComplianceReport>
            {
                Success = false,
                Message = "Failed to generate data retention report"
            });
        }
    }

    /// <summary>
    /// Get list of generated compliance reports
    /// </summary>
    [HttpGet("reports")]
    [RequiresPermission("compliance.view_reports")]
    public async Task<IActionResult> GetReports([FromQuery] int? tenantId = null, [FromQuery] int pageSize = 50)
    {
        try
        {
            var reports = await _complianceService.GetReportsAsync(tenantId, pageSize);

            return Ok(new ApiResponseDto<List<ComplianceReport>>
            {
                Success = true,
                Message = $"Retrieved {reports.Count} compliance reports",
                Data = reports
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve compliance reports");
            return StatusCode(500, new ApiResponseDto<List<ComplianceReport>>
            {
                Success = false,
                Message = "Failed to retrieve compliance reports"
            });
        }
    }

    /// <summary>
    /// Export a compliance report to various formats
    /// </summary>
    [HttpGet("reports/{reportId}/export")]
    [RequiresPermission("compliance.export_reports")]
    public async Task<IActionResult> ExportReport(Guid reportId, [FromQuery] string format = "pdf")
    {
        try
        {
            // First, get the report (this would normally be from storage)
            var reports = await _complianceService.GetReportsAsync(pageSize: 1000);
            var report = reports.FirstOrDefault(r => r.Id == reportId);

            if (report == null)
            {
                return NotFound(new ApiResponseDto<object>
                {
                    Success = false,
                    Message = "Compliance report not found"
                });
            }

            byte[] exportData;
            string contentType;
            string fileName;

            switch (format.ToLower())
            {
                case "pdf":
                    exportData = await _complianceService.ExportReportToPdfAsync(report);
                    contentType = "application/pdf";
                    fileName = $"compliance-report-{reportId}.pdf";
                    break;
                case "csv":
                    exportData = await _complianceService.ExportReportToCsvAsync(report);
                    contentType = "text/csv";
                    fileName = $"compliance-report-{reportId}.csv";
                    break;
                case "json":
                    exportData = await _complianceService.ExportReportToJsonAsync(report);
                    contentType = "application/json";
                    fileName = $"compliance-report-{reportId}.json";
                    break;
                default:
                    return BadRequest(new ApiResponseDto<object>
                    {
                        Success = false,
                        Message = "Unsupported export format. Supported formats: pdf, csv, json"
                    });
            }

            return File(exportData, contentType, fileName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export compliance report {ReportId}", reportId);
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Message = "Failed to export compliance report"
            });
        }
    }

    /// <summary>
    /// Get active security alerts
    /// </summary>
    [HttpGet("alerts")]
    [RequiresPermission("security.view_alerts")]
    public async Task<IActionResult> GetActiveAlerts([FromQuery] int? tenantId = null)
    {
        try
        {
            var alerts = await _alertService.GetActiveAlertsAsync(tenantId);

            return Ok(new ApiResponseDto<List<SecurityAlert>>
            {
                Success = true,
                Message = $"Retrieved {alerts.Count} active security alerts",
                Data = alerts
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve security alerts");
            return StatusCode(500, new ApiResponseDto<List<SecurityAlert>>
            {
                Success = false,
                Message = "Failed to retrieve security alerts"
            });
        }
    }

    /// <summary>
    /// Resolve a security alert
    /// </summary>
    [HttpPost("alerts/{alertId}/resolve")]
    [RequiresPermission("security.manage_alerts")]
    public async Task<IActionResult> ResolveAlert(Guid alertId, [FromBody] ResolveAlertRequest request)
    {
        try
        {
            await _alertService.ResolveAlertAsync(alertId, request.ResolvedBy, request.ResolutionNotes);

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Security alert resolved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve security alert {AlertId}", alertId);
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Message = "Failed to resolve security alert"
            });
        }
    }

    /// <summary>
    /// Get alert configuration for current tenant
    /// </summary>
    [HttpGet("alerts/configuration")]
    [RequiresPermission("security.view_alert_config")]
    public async Task<IActionResult> GetAlertConfiguration()
    {
        try
        {
            // This would get the current tenant ID from context
            var tenantId = 1; // Placeholder
            var config = await _alertService.GetAlertConfigurationAsync(tenantId);

            return Ok(new ApiResponseDto<AlertConfiguration>
            {
                Success = true,
                Message = "Alert configuration retrieved successfully",
                Data = config
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve alert configuration");
            return StatusCode(500, new ApiResponseDto<AlertConfiguration>
            {
                Success = false,
                Message = "Failed to retrieve alert configuration"
            });
        }
    }

    /// <summary>
    /// Update alert configuration for current tenant
    /// </summary>
    [HttpPut("alerts/configuration")]
    [RequiresPermission("security.manage_alert_config")]
    public async Task<IActionResult> UpdateAlertConfiguration([FromBody] AlertConfiguration configuration)
    {
        try
        {
            // This would get the current tenant ID from context
            var tenantId = 1; // Placeholder
            await _alertService.UpdateAlertConfigurationAsync(tenantId, configuration);

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Alert configuration updated successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update alert configuration");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Message = "Failed to update alert configuration"
            });
        }
    }

    /// <summary>
    /// Test alert notification system
    /// </summary>
    [HttpPost("alerts/test")]
    [RequiresPermission("security.test_alerts")]
    public async Task<IActionResult> TestAlert([FromBody] TestAlertRequest request)
    {
        try
        {
            var testAlert = new SecurityAlert
            {
                Type = "TestAlert",
                Severity = request.Severity,
                Message = request.Message ?? "This is a test security alert",
                TenantId = 1, // Placeholder
                Source = "ComplianceController",
                Tags = new List<string> { "test", "manual" }
            };

            await _alertService.SendAlertAsync(testAlert);

            return Ok(new ApiResponseDto<object>
            {
                Success = true,
                Message = "Test alert sent successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send test alert");
            return StatusCode(500, new ApiResponseDto<object>
            {
                Success = false,
                Message = "Failed to send test alert"
            });
        }
    }
}

// Request DTOs for the compliance endpoints
public class GenerateReportRequest
{
    public DateTime From { get; set; }
    public DateTime To { get; set; }
    public int? TenantId { get; set; }
}

public class ResolveAlertRequest
{
    public string ResolvedBy { get; set; } = string.Empty;
    public string ResolutionNotes { get; set; } = string.Empty;
}

public class TestAlertRequest
{
    public AlertSeverity Severity { get; set; } = AlertSeverity.Medium;
    public string? Message { get; set; }
}
