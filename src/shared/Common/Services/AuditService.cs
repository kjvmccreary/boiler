using DTOs.Entities;
using Common.Data;
using Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;

namespace Common.Services
{
    public enum AuditAction
    {
        Login,
        Logout,
        PermissionCheck,
        RoleAssigned,
        RoleRemoved,
        PermissionGranted,
        PermissionRevoked,
        UserCreated,
        UserUpdated,
        UserDeactivated,
        RoleCreated,
        RoleUpdated,
        RoleDeleted,
        UnauthorizedAccess,
        SecurityViolation
    }

    // ✅ GOOD: AuditEntry class is now properly located in DTOs/Entities/AuditEntry.cs

    public interface IAuditService
    {
        Task LogAsync(AuditAction action, string resource, object? details = null, bool success = true, string? errorMessage = null);
        Task LogSecurityViolationAsync(string violation, object? details = null);
        Task LogPermissionCheckAsync(int userId, string permission, bool granted);
        Task LogUnauthorizedAccessAsync(string resource, object? details = null);
        Task<List<AuditEntry>> GetAuditLogAsync(int? userId = null, DateTime? from = null, DateTime? to = null, int pageSize = 100);
    }

    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITenantProvider _tenantProvider;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            ApplicationDbContext context,
            ITenantProvider tenantProvider,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _tenantProvider = tenantProvider;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task LogAsync(AuditAction action, string resource, object? details = null, bool success = true, string? errorMessage = null)
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync() ?? 0;
                var userId = GetCurrentUserId();

                var auditEntry = new AuditEntry
                {
                    TenantId = tenantId,
                    UserId = userId,
                    Action = action.ToString(),
                    Resource = resource,
                    Details = details != null ? JsonSerializer.Serialize(details) : null,
                    IpAddress = GetClientIpAddress(),
                    UserAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown",
                    Success = success,
                    ErrorMessage = errorMessage,
                    Timestamp = DateTime.UtcNow
                };

                // For critical security events, log immediately
                if (action == AuditAction.SecurityViolation || action == AuditAction.UnauthorizedAccess)
                {
                    _logger.LogWarning("🚨 SECURITY ALERT: {Action} - {Resource} - User: {UserId} - IP: {IpAddress} - Details: {Details}",
                        action, resource, userId, auditEntry.IpAddress, auditEntry.Details);
                }

                // Add to database context
                _context.Set<AuditEntry>().Add(auditEntry);
                
                // 🔧 FIX: Always save immediately to ensure audit entries are persisted
                await _context.SaveChangesAsync();
                
                _logger.LogInformation("📝 AUDIT: {Action} - {Resource} - Success: {Success}", 
                    action, resource, success);
            }
            catch (Exception ex)
            {
                // Never let audit logging break the main flow
                _logger.LogError(ex, "📝 AUDIT ERROR: Failed to log audit entry for action {Action}", action);
            }
        }

        public async Task LogSecurityViolationAsync(string violation, object? details = null)
        {
            await LogAsync(AuditAction.SecurityViolation, violation, details, false);
        }

        public async Task LogPermissionCheckAsync(int userId, string permission, bool granted)
        {
            await LogAsync(AuditAction.PermissionCheck, permission, new { userId, granted }, granted);
        }

        public async Task LogUnauthorizedAccessAsync(string resource, object? details = null)
        {
            await LogAsync(AuditAction.UnauthorizedAccess, resource, details, false);
        }

        public async Task<List<AuditEntry>> GetAuditLogAsync(int? userId = null, DateTime? from = null, DateTime? to = null, int pageSize = 100)
        {
            try
            {
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (!tenantId.HasValue)
                    return new List<AuditEntry>();

                var query = _context.Set<AuditEntry>()
                    .Where(a => a.TenantId == tenantId.Value);

                if (userId.HasValue)
                    query = query.Where(a => a.UserId == userId.Value);

                if (from.HasValue)
                    query = query.Where(a => a.Timestamp >= from.Value);

                if (to.HasValue)
                    query = query.Where(a => a.Timestamp <= to.Value);

                return await query
                    .OrderByDescending(a => a.Timestamp)
                    .Take(Math.Min(pageSize, 1000)) // Cap at 1000 records
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit log");
                return new List<AuditEntry>();
            }
        }

        private int? GetCurrentUserId()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext?.User?.Identity?.IsAuthenticated == true)
                {
                    // 🔧 FIX: Use fully qualified name to resolve ambiguity between Common.Constants.ClaimTypes and System.Security.Claims.ClaimTypes
                    var userIdClaim = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value  // Standard claim
                                   ?? httpContext.User.FindFirst("sub")?.Value                        // JWT standard
                                   ?? httpContext.User.FindFirst("uid")?.Value                        // Custom claim
                                   ?? httpContext.User.FindFirst("user_id")?.Value                    // Alternative
                                   ?? httpContext.User.FindFirst("userId")?.Value;                    // Alternative

                    if (int.TryParse(userIdClaim, out var userId))
                    {
                        return userId;
                    }

                    // 🔧 FIX: Log what claims are available for debugging
                    var claims = string.Join(", ", httpContext.User.Claims.Select(c => $"{c.Type}:{c.Value}"));
                    _logger.LogDebug("Available claims for user ID extraction: {Claims}", claims);
                }
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Error extracting user ID from claims");
                return null;
            }
        }

        private string GetClientIpAddress()
        {
            try
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null) return "Unknown";

                // Check for forwarded IP first (load balancer scenarios)
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ips = forwardedFor.Split(',', StringSplitOptions.RemoveEmptyEntries);
                    if (ips.Length > 0)
                        return ips[0].Trim();
                }

                // Check for real IP header
                var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
                if (!string.IsNullOrEmpty(realIp))
                    return realIp;

                // Fall back to connection remote IP
                return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            }
            catch
            {
                return "Unknown";
            }
        }
    }
}
