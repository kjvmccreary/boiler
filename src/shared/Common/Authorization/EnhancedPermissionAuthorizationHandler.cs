using Contracts.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Diagnostics;
using DTOs.Entities;
using DTOs.Monitoring;
using Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Common.Caching; // 🔧 ADD: For cache service access

namespace Common.Authorization;

/// <summary>
/// Enhanced permission authorization handler with performance monitoring and audit logging.
/// Phase 11 - Enhanced Security and Monitoring.
/// Handles the two-phase JWT authentication flow (initial login then tenant selection).
/// </summary>
public class EnhancedPermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IPermissionService _permissionService;
    private readonly IEnhancedAuditService _auditService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<EnhancedPermissionAuthorizationHandler> _logger;

    public EnhancedPermissionAuthorizationHandler(
        IPermissionService permissionService,
        IEnhancedAuditService auditService,
        IHttpContextAccessor httpContextAccessor,
        ILogger<EnhancedPermissionAuthorizationHandler> logger)
    {
        _permissionService = permissionService;
        _auditService = auditService;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var stopwatch = Stopwatch.StartNew();
        var httpContext = _httpContextAccessor.HttpContext;
        var resource = GetResourcePath(httpContext);
        var cacheHit = false;
        
        // 🔧 FIX: Extract tenant ID from JWT claims (your specific claim structure)
        var tenantId = GetTenantIdFromClaims(context.User);
        
        try
        {
            // Extract user information - using fully qualified System.Security.Claims.ClaimTypes
            var userIdClaim = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdClaim, out var userId))
            {
                await LogPermissionCheckAsync(
                    tenantId: tenantId ?? 0,
                    userId: 0,
                    permission: requirement.Permission,
                    resource: resource,
                    granted: false,
                    denialReason: "No valid user ID found in claims",
                    checkDurationMs: stopwatch.Elapsed.TotalMilliseconds,
                    cacheHit: false
                );

                _logger.LogWarning("Authorization failed: No valid user ID found in claims for permission {Permission}", 
                    requirement.Permission);
                context.Fail();
                return;
            }

            // 🔧 FIX: Handle Phase 1 (no tenant selected yet)
            if (!tenantId.HasValue)
            {
                await LogPermissionCheckAsync(
                    tenantId: 0,
                    userId: userId,
                    permission: requirement.Permission,
                    resource: resource,
                    granted: false,
                    denialReason: "No tenant context available - user must select tenant first",
                    checkDurationMs: stopwatch.Elapsed.TotalMilliseconds,
                    cacheHit: false
                );

                _logger.LogWarning("Authorization failed: User {UserId} has no tenant context for permission {Permission}", 
                    userId, requirement.Permission);
                context.Fail();
                return;
            }

            // 🔧 FIX: First check if permission is directly in JWT claims (Phase 2 optimization)
            var hasPermissionInClaims = context.User.Claims
                .Any(c => c.Type == "permission" && c.Value == requirement.Permission);

            bool hasPermission;
            if (hasPermissionInClaims)
            {
                hasPermission = true;
                cacheHit = true; // JWT claims are essentially cached permissions
                _logger.LogDebug("Permission {Permission} found in JWT claims for user {UserId}", 
                    requirement.Permission, userId);
            }
            else
            {
                // 🔧 FIX: Check cache before calling permission service
                cacheHit = await CheckIfCacheHitAsync(httpContext, requirement.Permission, userId);
                
                // Fallback to permission service check
                hasPermission = await _permissionService.UserHasPermissionAsync(userId, requirement.Permission);
                
                _logger.LogDebug("Permission {Permission} checked via service for user {UserId}: {HasPermission}, Cache: {CacheHit}", 
                    requirement.Permission, userId, hasPermission, cacheHit);
            }
            
            stopwatch.Stop();

            if (hasPermission)
            {
                // Log successful permission check
                await LogPermissionCheckAsync(
                    tenantId: tenantId.Value,
                    userId: userId,
                    permission: requirement.Permission,
                    resource: resource,
                    granted: true,
                    denialReason: null,
                    checkDurationMs: stopwatch.Elapsed.TotalMilliseconds,
                    cacheHit: cacheHit
                );

                // 🆕 ADD: Record permission check metrics
                if (httpContext != null)
                {
                    await RecordPermissionMetrics(httpContext, requirement.Permission, true, stopwatch.Elapsed.TotalMilliseconds, cacheHit);
                }

                _logger.LogDebug("Authorization succeeded: User {UserId} has permission {Permission} (Duration: {Duration}ms, Cache: {CacheHit})", 
                    userId, requirement.Permission, stopwatch.Elapsed.TotalMilliseconds, cacheHit);
                
                context.Succeed(requirement);
            }
            else
            {
                // Log failed permission check
                await LogPermissionCheckAsync(
                    tenantId: tenantId.Value,
                    userId: userId,
                    permission: requirement.Permission,
                    resource: resource,
                    granted: false,
                    denialReason: "User does not have required permission",
                    checkDurationMs: stopwatch.Elapsed.TotalMilliseconds,
                    cacheHit: cacheHit
                );

                // 🆕 ADD: Record permission check metrics
                if (httpContext != null)
                {
                    await RecordPermissionMetrics(httpContext, requirement.Permission, false, stopwatch.Elapsed.TotalMilliseconds, cacheHit);
                }

                _logger.LogWarning("Authorization failed: User {UserId} does not have permission {Permission} for resource {Resource}", 
                    userId, requirement.Permission, resource);
                
                context.Fail();
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            // Log error in permission check
            await LogPermissionCheckAsync(
                tenantId: tenantId ?? 0,
                userId: 0,
                permission: requirement.Permission,
                resource: resource,
                granted: false,
                denialReason: $"Exception during permission check: {ex.Message}",
                checkDurationMs: stopwatch.Elapsed.TotalMilliseconds,
                cacheHit: false
            );

            _logger.LogError(ex, "Error during permission check for {Permission}", requirement.Permission);
            context.Fail();
        }
    }

    private async Task RecordPermissionMetrics(HttpContext httpContext, string permission, bool granted, double durationMs, bool cacheHit)
    {
        try
        {
            // 🔧 FIX: Use correct .NET 9 service resolution with generic extension method
            var monitoringService = httpContext.RequestServices.GetService<IMonitoringService>();
            if (monitoringService != null)
            {
                var tenantId = GetTenantIdFromClaims(httpContext.User);
                var metrics = new PermissionCheckMetrics
                {
                    Permission = permission,
                    Granted = granted,
                    CheckDurationMs = durationMs,
                    CacheHit = cacheHit,
                    TenantId = tenantId ?? 0,
                    Timestamp = DateTime.UtcNow
                };

                await monitoringService.RecordPermissionCheckMetricsAsync(metrics);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record permission check metrics");
        }
    }

    /// <summary>
    /// Extract tenant ID from JWT claims based on your specific claim structure
    /// </summary>
    private int? GetTenantIdFromClaims(ClaimsPrincipal user)
    {
        // 🔧 FIX: Try multiple claim names based on your JWT structure
        var tenantClaims = new[] { "tenant_id", "current_tenant_id", "TenantId" };
        
        foreach (var claimName in tenantClaims)
        {
            var tenantClaim = user.FindFirst(claimName)?.Value;
            if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out var tenantId))
            {
                return tenantId;
            }
        }

        return null;
    }

    private async Task LogPermissionCheckAsync(
        int tenantId,
        int userId,
        string permission,
        string resource, // 🔧 FIX: Added missing parameter type
        bool granted,
        string? denialReason,
        double checkDurationMs,
        bool cacheHit)
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            
            var auditEntry = new PermissionAuditEntry
            {
                TenantId = tenantId,
                UserId = userId,
                Permission = permission,
                Resource = resource,
                Granted = granted,
                DenialReason = denialReason,
                CheckDurationMs = checkDurationMs,
                CacheHit = cacheHit,
                IpAddress = GetClientIpAddress(httpContext),
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                Timestamp = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _auditService.LogPermissionCheckAsync(auditEntry);
        }
        catch (Exception ex)
        {
            // Don't let audit logging failures break authorization
            _logger.LogError(ex, "Failed to log permission check audit entry");
        }
    }

    private string GetResourcePath(HttpContext? httpContext)
    {
        if (httpContext?.Request == null)
            return "Unknown";

        var path = httpContext.Request.Path.Value ?? "";
        var method = httpContext.Request.Method ?? "";
        return $"{method} {path}";
    }

    private string? GetClientIpAddress(HttpContext? httpContext)
    {
        if (httpContext == null) return null;

        // Check for forwarded IP first (behind proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Check for real IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Fallback to connection IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    /// <summary>
    /// 🔧 FIX: Properly detect cache hits by checking Redis cache directly
    /// </summary>
    private async Task<bool> CheckIfCacheHitAsync(HttpContext? httpContext, string permission, int userId)
    {
        try
        {
            if (httpContext == null) return false;

            // Get cache service from DI container
            var cacheService = httpContext.RequestServices.GetService<ICacheService>();
            if (cacheService == null) return false;

            // Check if the permission cache key exists in Redis
            var cacheKey = $"user_permissions:{userId}";
            var cachedPermissions = await cacheService.GetAsync<List<string>>(cacheKey);
            
            // If permissions were found in cache, it's a cache hit
            var isHit = cachedPermissions != null && cachedPermissions.Contains(permission);
            
            _logger.LogDebug("Cache check for user {UserId}, permission {Permission}: {CacheHit}", 
                userId, permission, isHit ? "HIT" : "MISS");
                
            return isHit;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to check cache hit for user {UserId}, permission {Permission}", userId, permission);
            return false; // Default to cache miss on error
        }
    }
}
