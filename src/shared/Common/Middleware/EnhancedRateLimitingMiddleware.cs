using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using System.Security.Claims;
using Common.Services;
using DTOs.Entities;
using Contracts.Services;
using Microsoft.Extensions.Configuration;

namespace Common.Middleware;

/// <summary>
/// Enhanced rate limiting middleware with tenant-aware policies and security event logging.
/// Phase 11 - Enhanced Security and Monitoring.
/// ✅ FIXED: Now respects testing environment configuration and disables itself in tests.
/// </summary>
public class EnhancedRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IDistributedCache _cache;
    private readonly ILogger<EnhancedRateLimitingMiddleware> _logger;
    private readonly IConfiguration _configuration;
    
    // ✅ CRITICAL FIX: Check if rate limiting should be disabled
    private readonly bool _isEnabled;
    private readonly bool _isTestingEnvironment;

    // Rate limiting configuration with testing overrides
    private static readonly Dictionary<string, RateLimitPolicy> _policies = new()
    {
        ["api"] = new RateLimitPolicy { RequestsPerMinute = 100, BurstLimit = 10 },
        ["auth"] = new RateLimitPolicy { RequestsPerMinute = 20, BurstLimit = 5 },
        ["admin"] = new RateLimitPolicy { RequestsPerMinute = 50, BurstLimit = 8 },
        ["public"] = new RateLimitPolicy { RequestsPerMinute = 200, BurstLimit = 20 },
        ["tenant-selection"] = new RateLimitPolicy { RequestsPerMinute = 10, BurstLimit = 3 }
    };

    // ✅ TESTING OVERRIDE: Very high limits for testing environments
    private static readonly Dictionary<string, RateLimitPolicy> _testingPolicies = new()
    {
        ["api"] = new RateLimitPolicy { RequestsPerMinute = 10000, BurstLimit = 1000 },
        ["auth"] = new RateLimitPolicy { RequestsPerMinute = 10000, BurstLimit = 1000 },
        ["admin"] = new RateLimitPolicy { RequestsPerMinute = 10000, BurstLimit = 1000 },
        ["public"] = new RateLimitPolicy { RequestsPerMinute = 10000, BurstLimit = 1000 },
        ["tenant-selection"] = new RateLimitPolicy { RequestsPerMinute = 10000, BurstLimit = 1000 }
    };

    public EnhancedRateLimitingMiddleware(
        RequestDelegate next, 
        IDistributedCache cache,
        ILogger<EnhancedRateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _cache = cache;
        _logger = logger;
        _configuration = configuration;
        
        // ✅ CRITICAL FIX: Check environment and configuration to determine if enabled
        _isTestingEnvironment = IsTestingEnvironment();
        _isEnabled = ShouldRateLimitingBeEnabled();
        
        if (!_isEnabled)
        {
            _logger.LogInformation("🔧 Rate limiting is DISABLED for environment: {Environment}", 
                Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown");
        }
    }
    
    private bool IsTestingEnvironment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return string.Equals(environment, "Testing", StringComparison.OrdinalIgnoreCase) ||
               string.Equals(environment, "Test", StringComparison.OrdinalIgnoreCase);
    }
    
    private bool ShouldRateLimitingBeEnabled()
    {
        // ✅ Check multiple configuration sources for rate limiting enablement
        
        // 1. If we're in testing environment, check if explicitly disabled
        if (_isTestingEnvironment)
        {
            var testingEnabled = _configuration.GetValue<bool>("RateLimiting:Enabled", false);
            if (!testingEnabled)
            {
                _logger.LogInformation("🔧 Rate limiting explicitly disabled via configuration in testing environment");
                return false;
            }
        }
        
        // 2. Check global configuration
        var globalEnabled = _configuration.GetValue<bool>("RateLimiting:Enabled", true);
        if (!globalEnabled)
        {
            _logger.LogInformation("🔧 Rate limiting globally disabled via configuration");
            return false;
        }
        
        // 3. Check enhanced security configuration
        var enhancedSecurityEnabled = _configuration.GetValue<bool>("Security:EnableRateLimiting", true);
        if (!enhancedSecurityEnabled)
        {
            _logger.LogInformation("🔧 Rate limiting disabled via Security:EnableRateLimiting configuration");
            return false;
        }
        
        // 4. Default: enabled unless explicitly disabled
        return true;
    }

    public async Task InvokeAsync(HttpContext context, IEnhancedAuditService auditService)
    {
        // ✅ CRITICAL FIX: Skip rate limiting entirely if disabled
        if (!_isEnabled)
        {
            await _next(context);
            return;
        }

        var clientInfo = GetClientInfo(context);
        var policy = GetRateLimitPolicy(context.Request.Path);
        var tenantId = GetTenantIdFromContext(context);

        // ✅ Use testing policies if in testing environment
        if (_isTestingEnvironment)
        {
            policy = GetTestingRateLimitPolicy(context.Request.Path);
        }

        // Create rate limit key based on multiple factors
        var rateLimitKey = CreateRateLimitKey(clientInfo, tenantId, policy.Category);
        
        try
        {
            // Check current rate limit status
            var rateLimitStatus = await GetRateLimitStatusAsync(rateLimitKey, policy);
            
            // Update rate limit counters
            await UpdateRateLimitCountersAsync(rateLimitKey, policy);

            if (rateLimitStatus.IsExceeded)
            {
                // Log rate limit violation (but don't fail in testing)
                if (!_isTestingEnvironment)
                {
                    await LogRateLimitViolation(auditService, context, rateLimitStatus, tenantId);
                    
                    // Return 429 Too Many Requests
                    await HandleRateLimitExceeded(context, rateLimitStatus);
                    return;
                }
                else
                {
                    // In testing, just log but don't block
                    _logger.LogDebug("🧪 Rate limit would be exceeded in production for {Path}, but allowing in testing environment", 
                        context.Request.Path);
                }
            }

            // Add rate limit headers to response
            AddRateLimitHeaders(context, rateLimitStatus);

            // Continue to next middleware
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in rate limiting middleware for {Path}", context.Request.Path);
            
            // On error, allow request to continue (fail open)
            await _next(context);
        }
    }

    private ClientInfo GetClientInfo(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        var ipAddress = !string.IsNullOrEmpty(forwardedFor) 
            ? forwardedFor.Split(',')[0].Trim()
            : context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var userAgent = context.Request.Headers["User-Agent"].ToString();
        var userId = context.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        return new ClientInfo
        {
            IpAddress = ipAddress,
            UserAgent = userAgent,
            UserId = userId,
            IsAuthenticated = context.User?.Identity?.IsAuthenticated == true
        };
    }

    private int GetTenantIdFromContext(HttpContext context)
    {
        var user = context.User;
        if (user?.Identity?.IsAuthenticated != true)
            return 0;

        var tenantClaims = new[] { "tenant_id", "current_tenant_id", "TenantId" };
        
        foreach (var claimName in tenantClaims)
        {
            var tenantClaim = user.FindFirst(claimName)?.Value;
            if (!string.IsNullOrEmpty(tenantClaim) && int.TryParse(tenantClaim, out var tenantId))
            {
                return tenantId;
            }
        }

        return 0;
    }

    private RateLimitPolicy GetRateLimitPolicy(PathString path)
    {
        var pathValue = path.Value?.ToLower() ?? "";

        // Tenant selection endpoints - stricter limits
        if (pathValue.Contains("/api/auth/select-tenant") || 
            pathValue.Contains("/api/auth/tenants") ||
            pathValue.Contains("/api/auth/switch-tenant"))
        {
            return _policies["tenant-selection"];
        }

        // Authentication endpoints
        if (pathValue.StartsWith("/api/auth"))
        {
            return _policies["auth"];
        }

        // Admin endpoints
        if (pathValue.Contains("/admin") || pathValue.Contains("/api/roles") || pathValue.Contains("/api/permissions"))
        {
            return _policies["admin"];
        }

        // Public endpoints
        if (pathValue.StartsWith("/api/public") || pathValue.StartsWith("/health"))
        {
            return _policies["public"];
        }

        // Default API endpoints
        return _policies["api"];
    }

    // ✅ NEW: Get testing-friendly rate limit policies
    private RateLimitPolicy GetTestingRateLimitPolicy(PathString path)
    {
        var pathValue = path.Value?.ToLower() ?? "";

        if (pathValue.Contains("/api/auth/select-tenant") || 
            pathValue.Contains("/api/auth/tenants") ||
            pathValue.Contains("/api/auth/switch-tenant"))
        {
            return _testingPolicies["tenant-selection"];
        }

        if (pathValue.StartsWith("/api/auth"))
        {
            return _testingPolicies["auth"];
        }

        if (pathValue.Contains("/admin") || pathValue.Contains("/api/roles") || pathValue.Contains("/api/permissions"))
        {
            return _testingPolicies["admin"];
        }

        if (pathValue.StartsWith("/api/public") || pathValue.StartsWith("/health"))
        {
            return _testingPolicies["public"];
        }

        return _testingPolicies["api"];
    }

    private string CreateRateLimitKey(ClientInfo clientInfo, int tenantId, string category)
    {
        // Create composite key for rate limiting
        if (clientInfo.IsAuthenticated && !string.IsNullOrEmpty(clientInfo.UserId))
        {
            // User-based rate limiting (most specific)
            return $"rate_limit:user:{clientInfo.UserId}:tenant:{tenantId}:category:{category}";
        }
        else
        {
            // IP-based rate limiting for anonymous users
            return $"rate_limit:ip:{clientInfo.IpAddress}:category:{category}";
        }
    }

    private async Task<RateLimitStatus> GetRateLimitStatusAsync(string key, RateLimitPolicy policy)
    {
        var now = DateTime.UtcNow;
        var windowStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        
        // Sliding window key
        var minuteKey = $"{key}:{windowStart:yyyyMMddHHmm}";
        
        // Get current count from cache
        var countString = await _cache.GetStringAsync(minuteKey);
        var currentCount = string.IsNullOrEmpty(countString) ? 0 : int.Parse(countString);
        
        // Calculate burst window (last 10 seconds)
        var burstWindowStart = now.AddSeconds(-10);
        var burstKey = $"{key}:burst:{burstWindowStart:yyyyMMddHHmmss}";
        var burstCountString = await _cache.GetStringAsync(burstKey);
        var burstCount = string.IsNullOrEmpty(burstCountString) ? 0 : int.Parse(burstCountString);

        var isExceeded = currentCount >= policy.RequestsPerMinute || burstCount >= policy.BurstLimit;
        
        return new RateLimitStatus
        {
            CurrentCount = currentCount,
            BurstCount = burstCount,
            Limit = policy.RequestsPerMinute,
            BurstLimit = policy.BurstLimit,
            IsExceeded = isExceeded,
            WindowStart = windowStart,
            ResetTime = windowStart.AddMinutes(1),
            Key = key
        };
    }

    private async Task UpdateRateLimitCountersAsync(string key, RateLimitPolicy policy)
    {
        var now = DateTime.UtcNow;
        var windowStart = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0);
        
        // Update minute counter
        var minuteKey = $"{key}:{windowStart:yyyyMMddHHmm}";
        var countString = await _cache.GetStringAsync(minuteKey);
        var currentCount = string.IsNullOrEmpty(countString) ? 0 : int.Parse(countString);
        
        await _cache.SetStringAsync(minuteKey, (currentCount + 1).ToString(), 
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2) // Keep for 2 minutes
            });

        // Update burst counter (10-second window)
        var burstWindowStart = now.AddSeconds(-10);
        var burstKey = $"{key}:burst:{burstWindowStart:yyyyMMddHHmmss}";
        var burstCountString = await _cache.GetStringAsync(burstKey);
        var burstCount = string.IsNullOrEmpty(burstCountString) ? 0 : int.Parse(burstCountString);
        
        await _cache.SetStringAsync(burstKey, (burstCount + 1).ToString(),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(15) // Keep for 15 seconds
            });
    }

    private async Task LogRateLimitViolation(
        IEnhancedAuditService auditService, 
        HttpContext context, 
        RateLimitStatus status,
        int tenantId)
    {
        try
        {
            var securityEvent = new SecurityEventAuditEntry
            {
                TenantId = tenantId,
                EventType = "RateLimitExceeded",
                IpAddress = GetClientInfo(context).IpAddress,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                UserId = GetClientInfo(context).UserId != null ? int.Parse(GetClientInfo(context).UserId!) : null,
                Details = JsonSerializer.Serialize(new
                {
                    Path = context.Request.Path.Value,
                    Method = context.Request.Method,
                    CurrentCount = status.CurrentCount,
                    Limit = status.Limit,
                    BurstCount = status.BurstCount,
                    BurstLimit = status.BurstLimit,
                    WindowStart = status.WindowStart,
                    RateLimitKey = status.Key
                }),
                Severity = status.BurstCount > status.BurstLimit ? "Medium" : "Low",
                Resource = $"{context.Request.Method} {context.Request.Path}",
                Action = "RATE_LIMIT_EXCEEDED",
                Timestamp = DateTime.UtcNow,
                Investigated = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await auditService.LogSecurityEventAsync(securityEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log rate limit violation");
        }
    }

    private async Task HandleRateLimitExceeded(HttpContext context, RateLimitStatus status)
    {
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.Response.Headers["Retry-After"] = "60";

        AddRateLimitHeaders(context, status);

        var response = new
        {
            Error = "Rate limit exceeded",
            Message = $"Too many requests. Limit: {status.Limit} per minute, Burst: {status.BurstLimit} per 10 seconds",
            RetryAfter = 60,
            ResetTime = status.ResetTime
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private void AddRateLimitHeaders(HttpContext context, RateLimitStatus status)
    {
        // Use indexer instead of Add to avoid ArgumentException on duplicate keys
        context.Response.Headers["X-RateLimit-Limit"] = status.Limit.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, status.Limit - status.CurrentCount).ToString();
        context.Response.Headers["X-RateLimit-Reset"] = ((DateTimeOffset)status.ResetTime).ToUnixTimeSeconds().ToString();
        context.Response.Headers["X-RateLimit-Burst-Limit"] = status.BurstLimit.ToString();
        context.Response.Headers["X-RateLimit-Burst-Remaining"] = Math.Max(0, status.BurstLimit - status.BurstCount).ToString();
    }

    #region Helper Classes

    private class ClientInfo
    {
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string? UserId { get; set; }
        public bool IsAuthenticated { get; set; }
    }

    private class RateLimitPolicy
    {
        public int RequestsPerMinute { get; set; }
        public int BurstLimit { get; set; }
        public string Category => GetType().Name.Replace("Policy", "").ToLower();
    }

    private class RateLimitStatus
    {
        public int CurrentCount { get; set; }
        public int BurstCount { get; set; }
        public int Limit { get; set; }
        public int BurstLimit { get; set; }
        public bool IsExceeded { get; set; }
        public DateTime WindowStart { get; set; }
        public DateTime ResetTime { get; set; }
        public string Key { get; set; } = string.Empty;
    }

    #endregion
}   
