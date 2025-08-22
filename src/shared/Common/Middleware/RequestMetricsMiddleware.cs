using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using Common.Services;
using DTOs.Monitoring;

namespace Common.Middleware;

/// <summary>
/// Middleware to collect request performance metrics
/// Phase 11 - Enhanced Security and Monitoring
/// </summary>
public class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestMetricsMiddleware> _logger;

    public RequestMetricsMiddleware(RequestDelegate next, ILogger<RequestMetricsMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IMonitoringService monitoringService)
    {
        var stopwatch = Stopwatch.StartNew();
        var tenantId = GetTenantIdFromContext(context);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();

            try
            {
                var metrics = new RequestMetrics
                {
                    Path = context.Request.Path.Value ?? "unknown",
                    Method = context.Request.Method,
                    StatusCode = context.Response.StatusCode,
                    ResponseTimeMs = stopwatch.Elapsed.TotalMilliseconds,
                    TenantId = tenantId,
                    Timestamp = DateTime.UtcNow
                };

                await monitoringService.RecordRequestMetricsAsync(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record request metrics");
            }
        }
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
}
