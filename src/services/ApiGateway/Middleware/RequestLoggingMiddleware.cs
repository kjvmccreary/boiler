using System.Diagnostics;
using System.Text;

namespace ApiGateway.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = Guid.NewGuid().ToString("N")[..8];
        context.Items["CorrelationId"] = correlationId;
        
        // Add correlation ID to response headers
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        var request = context.Request;
        
        // Log request
        _logger.LogInformation("REQUEST {CorrelationId}: {Method} {Path} from {RemoteIP} | Tenant: {TenantId} | User: {UserId}",
            correlationId,
            request.Method,
            request.Path + request.QueryString,
            GetClientIpAddress(context),
            context.Items["TenantId"]?.ToString() ?? "Unknown",
            context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("user_id")?.Value ?? "Anonymous");

        // Log request headers (excluding sensitive ones)
        LogRequestHeaders(context, correlationId);

        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "REQUEST {CorrelationId}: Unhandled exception occurred", correlationId);
            throw;
        }
        finally
        {
            stopwatch.Stop();
            
            // Log response
            _logger.LogInformation("RESPONSE {CorrelationId}: {StatusCode} in {ElapsedMs}ms | Size: {ResponseSize}",
                correlationId,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                context.Response.ContentLength?.ToString() ?? "Unknown");

            // Log slow requests
            if (stopwatch.ElapsedMilliseconds > 1000)
            {
                _logger.LogWarning("SLOW REQUEST {CorrelationId}: {Method} {Path} took {ElapsedMs}ms",
                    correlationId,
                    request.Method,
                    request.Path,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }

    private void LogRequestHeaders(HttpContext context, string correlationId)
    {
        var sensitiveHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Authorization", "Cookie", "X-Api-Key", "X-Auth-Token"
        };

        var headers = context.Request.Headers
            .Where(h => !sensitiveHeaders.Contains(h.Key))
            .Select(h => $"{h.Key}: {string.Join(", ", h.Value.AsEnumerable())}")
            .ToArray(); // Convert to array to avoid ambiguity

        if (headers.Length > 0)
        {
            _logger.LogDebug("REQUEST {CorrelationId} Headers: {Headers}",
                correlationId, string.Join(" | ", headers)); // Now unambiguous
        }
    }

    private string GetClientIpAddress(HttpContext context)
    {
        // Check for forwarded IP first (from load balancer/proxy)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        return context.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }
}
