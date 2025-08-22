using Common.Services;
using DTOs.Entities;
using Contracts.Services;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;

namespace Common.Middleware;

/// <summary>
/// Middleware to detect and log security events.
/// Phase 11 - Enhanced Security and Monitoring.
/// Handles two-phase authentication flow (initial login then tenant selection).
/// </summary>
public class SecurityEventMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SecurityEventMiddleware> _logger;

    public SecurityEventMiddleware(RequestDelegate next, ILogger<SecurityEventMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IEnhancedAuditService auditService)
    {
        // 🔧 FIX: Extract tenant ID from JWT claims (handles Phase 1 and Phase 2)
        var tenantId = GetTenantIdFromContext(context);
        
        try
        {
            // Monitor request before processing
            await MonitorIncomingRequest(context, auditService, tenantId);
            
            await _next(context);
            
            // Monitor response after processing
            await MonitorOutgoingResponse(context, auditService, tenantId);
        }
        catch (Exception ex)
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "UnhandledException",
                severity: "High",
                context: context,
                details: new { Exception = ex.Message, StackTrace = ex.StackTrace }
            );
            
            throw; // Re-throw to maintain normal exception handling
        }
    }

    /// <summary>
    /// Get tenant ID from JWT claims or return 0 if not available (Phase 1 scenario)
    /// </summary>
    private int GetTenantIdFromContext(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
            return 0;

        // Try multiple claim names based on your JWT structure
        var tenantClaims = new[] { "tenant_id", "current_tenant_id", "TenantId" };
        
        foreach (var claimName in tenantClaims)
        {
            var tenantClaim = user.FindFirst(claimName)?.Value;
            if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out var tenantId))
            {
                return tenantId;
            }
        }

        // Return 0 for Phase 1 (pre-tenant selection) scenarios
        return 0;
    }

    private async Task MonitorIncomingRequest(HttpContext context, IEnhancedAuditService auditService, int tenantId)
    {
        var request = context.Request;
        
        // 🔧 FIX: Skip tenant selection endpoints to avoid false positives
        if (IsTenantSelectionEndpoint(request.Path))
        {
            return; // Don't monitor tenant selection endpoints
        }

        // Check for suspicious patterns
        if (IsSuspiciousRequest(request))
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "SuspiciousRequest",
                severity: "Medium",
                context: context,
                details: new
                {
                    Path = request.Path.Value,
                    Method = request.Method,
                    QueryString = request.QueryString.Value,
                    ContentLength = request.ContentLength,
                    Reason = "Suspicious request pattern detected"
                }
            );
        }

        // Check for potential SQL injection
        if (HasSqlInjectionPatterns(request))
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "PotentialSqlInjection",
                severity: "High",
                context: context,
                details: new
                {
                    Path = request.Path.Value,
                    QueryString = request.QueryString.Value,
                    Reason = "SQL injection patterns detected"
                }
            );
        }

        // Check for XSS attempts
        if (HasXssPatterns(request))
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "PotentialXssAttack",
                severity: "High",
                context: context,
                details: new
                {
                    Path = request.Path.Value,
                    QueryString = request.QueryString.Value,
                    Reason = "XSS patterns detected"
                }
            );
        }
    }

    private async Task MonitorOutgoingResponse(HttpContext context, IEnhancedAuditService auditService, int tenantId)
    {
        var response = context.Response;
        var request = context.Request;
        
        // Log authentication failures
        if (response.StatusCode == StatusCodes.Status401Unauthorized)
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "UnauthorizedAccess",
                severity: "Medium",
                context: context,
                details: new
                {
                    Path = request.Path.Value,
                    Method = request.Method,
                    StatusCode = response.StatusCode,
                    Reason = "Unauthorized access attempt",
                    HasTenantContext = tenantId > 0
                }
            );
        }

        // Log authorization failures
        if (response.StatusCode == StatusCodes.Status403Forbidden)
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "ForbiddenAccess",
                severity: "Medium",
                context: context,
                details: new
                {
                    Path = request.Path.Value,
                    Method = request.Method,
                    StatusCode = response.StatusCode,
                    Reason = "Forbidden resource access attempt",
                    HasTenantContext = tenantId > 0
                }
            );
        }

        // Log rate limiting
        if (response.StatusCode == StatusCodes.Status429TooManyRequests)
        {
            await LogSecurityEvent(
                auditService: auditService,
                tenantId: tenantId,
                eventType: "RateLimitExceeded",
                severity: "Low",
                context: context,
                details: new
                {
                    Path = request.Path.Value,
                    Method = request.Method,
                    StatusCode = response.StatusCode,
                    Reason = "Rate limit exceeded"
                }
            );
        }
    }

    /// <summary>
    /// Check if the request is to a tenant selection endpoint
    /// </summary>
    private bool IsTenantSelectionEndpoint(PathString path)
    {
        var pathValue = path.Value?.ToLower() ?? "";
        var tenantSelectionPaths = new[] 
        { 
            "/api/auth/select-tenant", 
            "/api/auth/tenants", 
            "/api/auth/switch-tenant",
            "/api/tenants/available"
        };
        
        return tenantSelectionPaths.Any(tsp => pathValue.Contains(tsp));
    }

    private bool IsSuspiciousRequest(HttpRequest request)
    {
        // Check for suspicious user agents
        var userAgent = request.Headers["User-Agent"].ToString().ToLower();
        var suspiciousAgents = new[] { "bot", "scanner", "crawler", "sqlmap", "nikto", "burp" };
        
        if (suspiciousAgents.Any(agent => userAgent.Contains(agent)))
        {
            return true;
        }

        // Check for excessive header count
        if (request.Headers.Count > 50)
        {
            return true;
        }

        // Check for suspicious paths
        var path = request.Path.Value?.ToLower() ?? "";
        var suspiciousPaths = new[] { "admin", "config", "debug", "test", ".env", "wp-admin" };
        
        return suspiciousPaths.Any(suspiciousPath => path.Contains(suspiciousPath));
    }

    private bool HasSqlInjectionPatterns(HttpRequest request)
    {
        var queryString = request.QueryString.Value?.ToLower() ?? "";
        var sqlPatterns = new[] { "union select", "drop table", "exec(", "execute(", "sp_", "xp_", "'or'1'='1" };
        
        return sqlPatterns.Any(pattern => queryString.Contains(pattern));
    }

    private bool HasXssPatterns(HttpRequest request)
    {
        var queryString = request.QueryString.Value?.ToLower() ?? "";
        var xssPatterns = new[] { "<script", "javascript:", "onerror=", "onload=", "eval(", "alert(" };
        
        return xssPatterns.Any(pattern => queryString.Contains(pattern));
    }

    private async Task LogSecurityEvent(
        IEnhancedAuditService auditService,
        int tenantId,
        string eventType,
        string severity,
        HttpContext context,
        object details)
    {
        try
        {
            var securityEvent = new SecurityEventAuditEntry
            {
                TenantId = tenantId, // Will be 0 for Phase 1 (pre-tenant selection)
                EventType = eventType,
                IpAddress = GetClientIpAddress(context),
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                UserId = GetCurrentUserId(context),
                UserEmail = GetCurrentUserEmail(context),
                Details = JsonSerializer.Serialize(details),
                Severity = severity,
                Resource = $"{context.Request.Method} {context.Request.Path}",
                Action = context.Request.Method,
                Timestamp = DateTime.UtcNow,
                Investigated = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await auditService.LogSecurityEventAsync(securityEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log security event: {EventType}", eventType);
        }
    }

    private string? GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString();
    }

    private int? GetCurrentUserId(HttpContext context)
    {
        // 🔧 FIX: Use fully qualified System.Security.Claims.ClaimTypes to avoid ambiguity
        var userIdClaim = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private string? GetCurrentUserEmail(HttpContext context)
    {
        // 🔧 FIX: Use fully qualified System.Security.Claims.ClaimTypes to avoid ambiguity
        return context.User?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
    }
}
