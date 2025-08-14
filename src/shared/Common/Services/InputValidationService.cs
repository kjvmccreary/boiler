using System.Text.RegularExpressions;
using System.Web;
using Microsoft.Extensions.Logging;

namespace Common.Services
{
    public interface IInputValidationService
    {
        string SanitizeString(string? input, int maxLength = 255);
        string SanitizeHtml(string? input);
        bool IsValidEmail(string? email);
        bool IsValidPhoneNumber(string? phoneNumber);
        bool IsValidUrl(string? url);
        string SanitizeSearchQuery(string? query);
        bool ContainsSqlInjection(string? input);
        bool ContainsXssAttempt(string? input);
    }

    public class InputValidationService : IInputValidationService
    {
        private readonly ILogger<InputValidationService> _logger;
        
        // Common SQL injection patterns
        private static readonly string[] SqlInjectionPatterns = {
            @"('|(\\')|(;)|(\\;)|(--)|(--)|(#)|(\\#))",
            @"\b(exec|execute|sp_|xp_)\b",
            @"\b(select|insert|update|delete|drop|create|alter|grant|revoke)\b",
            @"\b(union|having|group\s+by|order\s+by)\b",
            @"(script|javascript|vbscript|iframe|object|embed|form)",
            @"(\<|\>|&lt;|&gt;)"
        };

        // XSS patterns
        private static readonly string[] XssPatterns = {
            @"<script\b[^<]*(?:(?!<\/script>)<[^<]*)*<\/script>",
            @"javascript:",
            @"vbscript:",
            @"on\w+\s*=",
            @"<iframe\b[^>]*>",
            @"<object\b[^>]*>",
            @"<embed\b[^>]*>",
            @"<form\b[^>]*>"
        };

        public InputValidationService(ILogger<InputValidationService> logger)
        {
            _logger = logger;
        }

        public string SanitizeString(string? input, int maxLength = 255)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // Remove null characters and control characters
            input = Regex.Replace(input, @"[\x00-\x08\x0B\x0C\x0E-\x1F\x7F]", "");
            
            // Trim whitespace
            input = input.Trim();
            
            // Truncate to max length
            if (input.Length > maxLength)
            {
                input = input.Substring(0, maxLength);
                _logger.LogWarning("🔒 SECURITY: Input truncated to {MaxLength} characters", maxLength);
            }

            // Check for potential security issues
            if (ContainsSqlInjection(input))
            {
                _logger.LogWarning("🚨 SECURITY: Potential SQL injection attempt blocked: {Input}", input.Substring(0, Math.Min(50, input.Length)));
                throw new ArgumentException("Invalid input detected");
            }

            if (ContainsXssAttempt(input))
            {
                _logger.LogWarning("🚨 SECURITY: Potential XSS attempt blocked: {Input}", input.Substring(0, Math.Min(50, input.Length)));
                throw new ArgumentException("Invalid input detected");
            }

            return input;
        }

        public string SanitizeHtml(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            // HTML encode to prevent XSS
            return HttpUtility.HtmlEncode(input);
        }

        public bool IsValidEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                // Use built-in email validation
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        public bool IsValidPhoneNumber(string? phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            // Allow digits, spaces, hyphens, parentheses, and plus sign
            var phoneRegex = new Regex(@"^[\+]?[\d\s\-\(\)]{7,20}$");
            return phoneRegex.IsMatch(phoneNumber);
        }

        public bool IsValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        public string SanitizeSearchQuery(string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return string.Empty;

            // Remove special characters that could be used for injection
            query = Regex.Replace(query, @"[^\w\s\-@.]", "");
            
            // Limit length
            if (query.Length > 100)
                query = query.Substring(0, 100);

            return query.Trim();
        }

        public bool ContainsSqlInjection(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var lowerInput = input.ToLower();
            
            return SqlInjectionPatterns.Any(pattern => 
                Regex.IsMatch(lowerInput, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        }

        public bool ContainsXssAttempt(string? input)
        {
            if (string.IsNullOrEmpty(input))
                return false;

            var lowerInput = input.ToLower();
            
            return XssPatterns.Any(pattern => 
                Regex.IsMatch(lowerInput, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline));
        }
    }
}
