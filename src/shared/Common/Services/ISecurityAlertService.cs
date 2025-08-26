using DTOs.Compliance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text;
using System.Net.Http;

namespace Common.Services;

/// <summary>
/// Security alert service for compliance notifications
/// Phase 11 Session 3 - Compliance Features
/// </summary>
public interface ISecurityAlertService
{
    Task SendAlertAsync(SecurityAlert alert);
    Task SendEmailAlertAsync(SecurityAlert alert);
    Task SendSlackAlertAsync(SecurityAlert alert);
    Task<List<SecurityAlert>> GetActiveAlertsAsync(int? tenantId = null);
    Task ResolveAlertAsync(Guid alertId, string resolvedBy, string resolutionNotes);
    Task<AlertConfiguration> GetAlertConfigurationAsync(int tenantId);
    Task UpdateAlertConfigurationAsync(int tenantId, AlertConfiguration configuration);
}

public class SecurityAlertService : ISecurityAlertService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ILogger<SecurityAlertService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IEnhancedAuditService _auditService;
    private readonly Dictionary<int, AlertConfiguration> _alertConfigurations = new();

    public SecurityAlertService(
        IConfiguration configuration,
        IEmailService emailService,
        ILogger<SecurityAlertService> logger,
        HttpClient httpClient,
        IEnhancedAuditService auditService)
    {
        _configuration = configuration;
        _emailService = emailService;
        _logger = logger;
        _httpClient = httpClient;
        _auditService = auditService;
    }

    public async Task SendAlertAsync(SecurityAlert alert)
    {
        try
        {
            var config = await GetAlertConfigurationAsync(alert.TenantId);
            
            // Check if alert severity meets notification threshold
            if (alert.Severity < config.MinimumSeverityForNotification)
            {
                _logger.LogDebug("Alert {AlertId} severity {Severity} below notification threshold {Threshold}",
                    alert.Id, alert.Severity, config.MinimumSeverityForNotification);
                return;
            }

            // Check cooldown period
            if (await IsInCooldownPeriod(alert, config))
            {
                _logger.LogDebug("Alert {AlertId} is in cooldown period", alert.Id);
                return;
            }

            var tasks = new List<Task>();

            // Send email notifications
            if (config.EnableEmailNotifications && config.EmailRecipients.Any())
            {
                tasks.Add(SendEmailAlertAsync(alert));
            }

            // Send Slack notifications
            if (config.EnableSlackNotifications && !string.IsNullOrEmpty(config.SlackWebhookUrl))
            {
                tasks.Add(SendSlackAlertAsync(alert));
            }

            await Task.WhenAll(tasks);

            // Mark notification as sent
            alert.NotificationSent = true;
            alert.NotificationSentAt = DateTime.UtcNow;
            alert.NotificationRecipients = config.EmailRecipients;

            // Log the alert
            await LogSecurityAlertAsync(alert);

            _logger.LogInformation("Security alert {AlertId} notifications sent successfully", alert.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send security alert {AlertId}", alert.Id);
            throw;
        }
    }

    public async Task SendEmailAlertAsync(SecurityAlert alert)
    {
        try
        {
            var config = await GetAlertConfigurationAsync(alert.TenantId);
            var recipients = config.EmailRecipients;

            if (!recipients.Any())
            {
                _logger.LogWarning("No email recipients configured for tenant {TenantId}", alert.TenantId);
                return;
            }

            var subject = $"[{alert.Severity}] Security Alert: {alert.Type}";
            var body = BuildEmailContent(alert);

            await _emailService.SendAsync(recipients, subject, body);

            _logger.LogInformation("Security alert email sent to {RecipientCount} recipients for alert {AlertId}",
                recipients.Count, alert.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email alert {AlertId}", alert.Id);
            throw;
        }
    }

    public async Task SendSlackAlertAsync(SecurityAlert alert)
    {
        try
        {
            var config = await GetAlertConfigurationAsync(alert.TenantId);
            
            if (string.IsNullOrEmpty(config.SlackWebhookUrl))
            {
                _logger.LogWarning("Slack webhook URL not configured for tenant {TenantId}", alert.TenantId);
                return;
            }

            var slackMessage = BuildSlackMessage(alert);
            var jsonContent = JsonSerializer.Serialize(slackMessage);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(config.SlackWebhookUrl, content);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation("Security alert Slack message sent successfully for alert {AlertId}", alert.Id);
            }
            else
            {
                _logger.LogWarning("Failed to send Slack alert {AlertId}. Status: {StatusCode}, Response: {Response}",
                    alert.Id, response.StatusCode, await response.Content.ReadAsStringAsync());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send Slack alert {AlertId}", alert.Id);
            throw;
        }
    }

    public Task<List<SecurityAlert>> GetActiveAlertsAsync(int? tenantId = null)
    {
        // For now, we'll simulate getting alerts from storage
        // In production, this would query a dedicated alerts table or cache
        var alerts = new List<SecurityAlert>();

        // This is a placeholder implementation
        // You would typically store alerts in Redis or database
        
        _logger.LogInformation("Retrieved {AlertCount} active alerts for tenant {TenantId}", 
            alerts.Count, tenantId);
        
        return Task.FromResult(alerts);
    }

    public Task ResolveAlertAsync(Guid alertId, string resolvedBy, string resolutionNotes)
    {
        try
        {
            // This would update the alert in storage
            // For now, we'll just log the resolution
            
            _logger.LogInformation("Alert {AlertId} resolved by {ResolvedBy}. Notes: {Notes}",
                alertId, resolvedBy, resolutionNotes);

            // Log the resolution as an audit event
            return _auditService.LogAsync(
                Common.Services.AuditAction.SecurityViolation,
                "AlertResolution",
                new { alertId, resolvedBy, resolutionNotes },
                true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve alert {AlertId}", alertId);
            throw;
        }
    }

    public Task<AlertConfiguration> GetAlertConfigurationAsync(int tenantId)
    {
        // Check if we have a cached configuration
        if (_alertConfigurations.TryGetValue(tenantId, out var cachedConfig))
        {
            return Task.FromResult(cachedConfig);
        }

        // Load from configuration (in production, this would come from database)
        var config = new AlertConfiguration
        {
            EnableEmailNotifications = _configuration.GetValue<bool>("Security:EnableEmailNotifications", true),
            EnableSlackNotifications = _configuration.GetValue<bool>("Security:EnableSlackNotifications", false),
            EmailRecipients = _configuration.GetSection("Security:AlertRecipients").Get<List<string>>() ?? new(),
            SlackWebhookUrl = _configuration["Security:SlackWebhookUrl"],
            MinimumSeverityForNotification = Enum.Parse<AlertSeverity>(
                _configuration["Security:MinimumSeverityForNotification"] ?? "Medium"),
            NotificationCooldown = TimeSpan.FromMinutes(
                _configuration.GetValue<int>("Security:NotificationCooldownMinutes", 15))
        };

        // Cache the configuration
        _alertConfigurations[tenantId] = config;

        return Task.FromResult(config);
    }

    public Task UpdateAlertConfigurationAsync(int tenantId, AlertConfiguration configuration)
    {
        // Update the cached configuration
        _alertConfigurations[tenantId] = configuration;

        // In production, you would save this to database
        _logger.LogInformation("Alert configuration updated for tenant {TenantId}", tenantId);

        return _auditService.LogAsync(
            Common.Services.AuditAction.UserUpdated,
            "AlertConfiguration",
            new { tenantId, configuration },
            true);
    }

    private string BuildEmailContent(SecurityAlert alert)
    {
        var html = new StringBuilder();
        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head>");
        html.AppendLine("<title>Security Alert</title>");
        html.AppendLine("<style>");
        html.AppendLine("body { font-family: Arial, sans-serif; margin: 20px; }");
        html.AppendLine(".alert-header { padding: 15px; border-radius: 5px; margin-bottom: 20px; }");
        html.AppendLine(".severity-low { background-color: #d4edda; border: 1px solid #c3e6cb; }");
        html.AppendLine(".severity-medium { background-color: #fff3cd; border: 1px solid #ffeaa7; }");
        html.AppendLine(".severity-high { background-color: #f8d7da; border: 1px solid #f5c6cb; }");
        html.AppendLine(".severity-critical { background-color: #f8d7da; border: 2px solid #dc3545; }");
        html.AppendLine("table { border-collapse: collapse; width: 100%; }");
        html.AppendLine("th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }");
        html.AppendLine("th { background-color: #f2f2f2; }");
        html.AppendLine("</style>");
        html.AppendLine("</head><body>");

        var severityClass = $"severity-{alert.Severity.ToString().ToLower()}";
        html.AppendLine($"<div class=\"alert-header {severityClass}\">");
        html.AppendLine($"<h2>🚨 Security Alert: {alert.Type}</h2>");
        html.AppendLine($"<p><strong>Severity:</strong> {alert.Severity}</p>");
        html.AppendLine($"<p><strong>Time:</strong> {alert.CreatedAt:yyyy-MM-dd HH:mm:ss} UTC</p>");
        html.AppendLine("</div>");

        html.AppendLine("<h3>Alert Details</h3>");
        html.AppendLine("<table>");
        html.AppendLine($"<tr><td><strong>Alert ID</strong></td><td>{alert.Id}</td></tr>");
        html.AppendLine($"<tr><td><strong>Message</strong></td><td>{alert.Message}</td></tr>");
        html.AppendLine($"<tr><td><strong>Source</strong></td><td>{alert.Source}</td></tr>");
        
        if (!string.IsNullOrEmpty(alert.IpAddress))
            html.AppendLine($"<tr><td><strong>IP Address</strong></td><td>{alert.IpAddress}</td></tr>");
        
        if (alert.UserId.HasValue)
            html.AppendLine($"<tr><td><strong>User ID</strong></td><td>{alert.UserId}</td></tr>");

        if (!string.IsNullOrEmpty(alert.CorrelationId))
            html.AppendLine($"<tr><td><strong>Correlation ID</strong></td><td>{alert.CorrelationId}</td></tr>");

        html.AppendLine("</table>");

        if (alert.Details.Any())
        {
            html.AppendLine("<h3>Additional Details</h3>");
            html.AppendLine("<table>");
            foreach (var detail in alert.Details)
            {
                html.AppendLine($"<tr><td><strong>{detail.Key}</strong></td><td>{detail.Value}</td></tr>");
            }
            html.AppendLine("</table>");
        }

        if (alert.Tags.Any())
        {
            html.AppendLine("<h3>Tags</h3>");
            html.AppendLine($"<p>{string.Join(", ", alert.Tags)}</p>");
        }

        html.AppendLine("<hr/>");
        html.AppendLine("<p><small>This is an automated security alert from your application monitoring system.</small></p>");
        html.AppendLine("</body></html>");

        return html.ToString();
    }

    private object BuildSlackMessage(SecurityAlert alert)
    {
        var color = alert.Severity switch
        {
            AlertSeverity.Low => "good",
            AlertSeverity.Medium => "warning",
            AlertSeverity.High => "danger",
            AlertSeverity.Critical => "danger",
            _ => "#cccccc"
        };

        var emoji = alert.Severity switch
        {
            AlertSeverity.Low => "ℹ️",
            AlertSeverity.Medium => "⚠️",
            AlertSeverity.High => "🚨",
            AlertSeverity.Critical => "🔥",
            _ => "📢"
        };

        return new
        {
            text = $"{emoji} Security Alert: {alert.Type}",
            attachments = new[]
            {
                new
                {
                    color = color,
                    fields = new[]
                    {
                        new { title = "Severity", value = alert.Severity.ToString(), @short = true },
                        new { title = "Time", value = alert.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss UTC"), @short = true },
                        new { title = "Message", value = alert.Message, @short = false },
                        new { title = "Source", value = alert.Source, @short = true },
                        new { title = "Alert ID", value = alert.Id.ToString(), @short = true }
                    },
                    footer = "Security Monitoring System",
                    ts = ((DateTimeOffset)alert.CreatedAt).ToUnixTimeSeconds()
                }
            }
        };
    }

    private async Task<bool> IsInCooldownPeriod(SecurityAlert alert, AlertConfiguration config)
    {
        // This would check if a similar alert was sent recently
        // For now, we'll return false (no cooldown)
        return false;
    }

    private async Task LogSecurityAlertAsync(SecurityAlert alert)
    {
        await _auditService.LogSecurityEventAsync(new DTOs.Entities.SecurityEventAuditEntry
        {
            TenantId = alert.TenantId,
            EventType = $"SecurityAlert_{alert.Type}",
            Severity = alert.Severity.ToString(),
            UserId = alert.UserId,
            IpAddress = alert.IpAddress,
            UserAgent = alert.UserAgent,
            Details = JsonSerializer.Serialize(alert.Details),
            Resource = "SecurityAlert",
            Action = "AlertSent",
            Timestamp = DateTime.UtcNow
        });
    }
}
