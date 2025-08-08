using System.Collections.Concurrent;
using System.Text.Json;

namespace ApiGateway.Middleware;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RateLimitingMiddleware> _logger;
    private readonly RateLimitOptions _options;
    
    // In-memory cache for rate limiting (for production, use Redis)
    private static readonly ConcurrentDictionary<string, RateLimitInfo> _cache = new();

    public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger, IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _options = configuration.GetSection("RateLimit").Get<RateLimitOptions>() ?? new RateLimitOptions();
    }

    // ✅ ADDED: Method to clear cache for testing
    public static void ClearCache()
    {
        _cache.Clear();
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var key = GenerateRateLimitKey(context);
        var rateLimitInfo = GetOrCreateRateLimitInfo(key);

        bool rateLimitExceeded;
        
        // Check rate limit without await inside lock
        lock (rateLimitInfo)
        {
            // Clean up old entries
            var now = DateTime.UtcNow;
            rateLimitInfo.Requests.RemoveAll(r => now - r > TimeSpan.FromMinutes(_options.WindowMinutes));

            // Check if rate limit exceeded
            rateLimitExceeded = rateLimitInfo.Requests.Count >= _options.MaxRequests;

            if (!rateLimitExceeded)
            {
                // Add current request
                rateLimitInfo.Requests.Add(now);
            }
        }

        // Handle rate limit exceeded outside of lock
        if (rateLimitExceeded)
        {
            _logger.LogWarning("Rate limit exceeded for key: {Key}. {Count}/{Limit} requests in {Window} minutes",
                key, rateLimitInfo.Requests.Count, _options.MaxRequests, _options.WindowMinutes);

            await HandleRateLimitExceeded(context, rateLimitInfo);
            return;
        }

        // Add rate limit headers
        AddRateLimitHeaders(context, rateLimitInfo);

        await _next(context);
    }

    private string GenerateRateLimitKey(HttpContext context)
    {
        var tenantId = context.Items["TenantId"]?.ToString() ?? "default";
        var userId = context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("user_id")?.Value;
        var clientIp = GetClientIpAddress(context);

        // Prioritize tenant-based rate limiting
        if (!string.IsNullOrEmpty(tenantId) && tenantId != "default")
        {
            return $"tenant:{tenantId}";
        }

        // Then user-based if authenticated
        if (!string.IsNullOrEmpty(userId))
        {
            return $"user:{userId}";
        }

        // Fall back to IP-based
        return $"ip:{clientIp}";
    }

    private RateLimitInfo GetOrCreateRateLimitInfo(string key)
    {
        return _cache.GetOrAdd(key, _ => new RateLimitInfo());
    }

    private async Task HandleRateLimitExceeded(HttpContext context, RateLimitInfo rateLimitInfo)
    {
        context.Response.StatusCode = 429; // Too Many Requests
        context.Response.ContentType = "application/json";

        var oldestRequest = rateLimitInfo.Requests.Count > 0 ? rateLimitInfo.Requests.Min() : DateTime.UtcNow;
        var retryAfter = (int)Math.Ceiling((_options.WindowMinutes * 60) - (DateTime.UtcNow - oldestRequest).TotalSeconds);

        context.Response.Headers["Retry-After"] = retryAfter.ToString();
        context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = "0";
        context.Response.Headers["X-RateLimit-Reset"] = DateTimeOffset.UtcNow.AddSeconds(retryAfter).ToUnixTimeSeconds().ToString();

        var response = new
        {
            error = "Rate limit exceeded",
            message = $"Too many requests. Limit: {_options.MaxRequests} per {_options.WindowMinutes} minutes",
            retryAfter = retryAfter
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }

    private void AddRateLimitHeaders(HttpContext context, RateLimitInfo rateLimitInfo)
    {
        var remaining = Math.Max(0, _options.MaxRequests - rateLimitInfo.Requests.Count);
        var oldestRequest = rateLimitInfo.Requests.Count > 0 ? rateLimitInfo.Requests.Min() : DateTime.UtcNow;
        var resetTime = oldestRequest.AddMinutes(_options.WindowMinutes);

        context.Response.Headers["X-RateLimit-Limit"] = _options.MaxRequests.ToString();
        context.Response.Headers["X-RateLimit-Remaining"] = remaining.ToString();
        context.Response.Headers["X-RateLimit-Reset"] = new DateTimeOffset(resetTime).ToUnixTimeSeconds().ToString();
    }

    private string GetClientIpAddress(HttpContext context)
    {
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}

public class RateLimitOptions
{
    public int MaxRequests { get; set; } = 100;
    public int WindowMinutes { get; set; } = 1;
}

public class RateLimitInfo
{
    public List<DateTime> Requests { get; set; } = new();
}
