using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;
using System.Text;

namespace Common.Services;

/// <summary>
/// Email service for compliance notifications
/// Phase 11 Session 3 - Compliance Features
/// </summary>
public interface IEmailService
{
    Task SendAsync(string to, string subject, string body, bool isHtml = true);
    Task SendAsync(List<string> recipients, string subject, string body, bool isHtml = true);
    Task SendTemplateAsync(string to, string templateName, object model);
}

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendAsync(string to, string subject, string body, bool isHtml = true)
    {
        await SendAsync(new List<string> { to }, subject, body, isHtml);
    }

    public async Task SendAsync(List<string> recipients, string subject, string body, bool isHtml = true)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = _configuration.GetValue<int>("Email:SmtpPort", 587);
            var username = _configuration["Email:Username"];
            var password = _configuration["Email:Password"];
            var fromAddress = _configuration["Email:FromAddress"];
            var fromName = _configuration["Email:FromName"] ?? "System Notifications";

            // For development/testing, log instead of sending actual emails
            if (string.IsNullOrEmpty(smtpHost) || _configuration.GetValue<bool>("Email:MockMode", true))
            {
                _logger.LogInformation("EMAIL (Mock Mode): To: {Recipients}, Subject: {Subject}",
                    string.Join(", ", recipients), subject);
                _logger.LogDebug("EMAIL Body: {Body}", body);
                return;
            }

            using var client = new SmtpClient(smtpHost, smtpPort);
            client.EnableSsl = _configuration.GetValue<bool>("Email:EnableSsl", true);
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential(username, password);

            var message = new MailMessage();
            message.From = new MailAddress(fromAddress ?? username!, fromName);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = isHtml;

            foreach (var recipient in recipients)
            {
                if (IsValidEmail(recipient))
                {
                    message.To.Add(recipient);
                }
                else
                {
                    _logger.LogWarning("Invalid email address skipped: {Email}", recipient);
                }
            }

            if (message.To.Count == 0)
            {
                _logger.LogWarning("No valid recipients found for email: {Subject}", subject);
                return;
            }

            await client.SendMailAsync(message);

            _logger.LogInformation("Email sent successfully to {RecipientCount} recipients: {Subject}",
                message.To.Count, subject);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email: {Subject}", subject);
            throw;
        }
    }

    public async Task SendTemplateAsync(string to, string templateName, object model)
    {
        // Simple template implementation - in production you'd use a proper template engine
        var template = await GetEmailTemplateAsync(templateName);
        var body = ReplaceTokens(template, model);
        var subject = ExtractSubjectFromTemplate(template);

        await SendAsync(to, subject, body);
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string> GetEmailTemplateAsync(string templateName)
    {
        // Placeholder for template loading
        // In production, you'd load from files or database
        return templateName switch
        {
            "security-alert" => @"
                Subject: Security Alert: {{AlertType}}
                
                <h2>Security Alert</h2>
                <p>Alert Type: {{AlertType}}</p>
                <p>Severity: {{Severity}}</p>
                <p>Message: {{Message}}</p>
                <p>Time: {{Timestamp}}</p>
                ",
            "compliance-report" => @"
                Subject: Compliance Report Generated
                
                <h2>Compliance Report Ready</h2>
                <p>Report Type: {{ReportType}}</p>
                <p>Generated: {{GeneratedAt}}</p>
                <p>Period: {{Period}}</p>
                ",
            _ => throw new ArgumentException($"Unknown template: {templateName}")
        };
    }

    private string ReplaceTokens(string template, object model)
    {
        // Simple token replacement - in production use a proper template engine
        var properties = model.GetType().GetProperties();
        var result = template;

        foreach (var prop in properties)
        {
            var value = prop.GetValue(model)?.ToString() ?? "";
            result = result.Replace($"{{{{{prop.Name}}}}}", value);
        }

        return result;
    }

    private string ExtractSubjectFromTemplate(string template)
    {
        var lines = template.Split('\n');
        var subjectLine = lines.FirstOrDefault(l => l.Trim().StartsWith("Subject:"));

        if (subjectLine != null)
        {
            return subjectLine.Replace("Subject:", "").Trim();
        }

        return "System Notification";
    }
}
