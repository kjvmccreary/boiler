using Microsoft.Extensions.Diagnostics.HealthChecks;
using Common.Performance;

namespace UserService.HealthChecks;

/// <summary>
/// Health check for application performance metrics
/// Monitors cache hit ratios and response times
/// </summary>
public class PerformanceHealthCheck : IHealthCheck
{
    private readonly IPerformanceMetricsService _metrics;
    private readonly ILogger<PerformanceHealthCheck> _logger;

    public PerformanceHealthCheck(
        IPerformanceMetricsService metrics, 
        ILogger<PerformanceHealthCheck> logger)
    {
        _metrics = metrics;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stats = await _metrics.GetStatsAsync();
            var healthData = new Dictionary<string, object>();
            
            bool isHealthy = true;
            bool isDegraded = false;
            var issues = new List<string>();
            
            foreach (var operation in stats.Operations)
            {
                var opPrefix = $"operation_{operation.Operation.ToLowerInvariant()}";
                
                healthData[$"{opPrefix}_cache_hit_ratio"] = $"{operation.CacheHitRatio:F1}%";
                healthData[$"{opPrefix}_avg_duration_ms"] = operation.AverageDuration;
                healthData[$"{opPrefix}_total_requests"] = operation.TotalRequests;
                
                // Check cache hit ratio (warning if < 90%, unhealthy if < 70%)
                if (operation.TotalRequests > 100) // Only check if we have significant traffic
                {
                    if (operation.CacheHitRatio < 70)
                    {
                        isHealthy = false;
                        issues.Add($"{operation.Operation} cache hit ratio is critically low: {operation.CacheHitRatio:F1}%");
                    }
                    else if (operation.CacheHitRatio < 90)
                    {
                        isDegraded = true;
                        issues.Add($"{operation.Operation} cache hit ratio is below optimal: {operation.CacheHitRatio:F1}%");
                    }
                }
                
                // Check average response time (warning if > 50ms, unhealthy if > 200ms)
                if (operation.AverageDuration > 200)
                {
                    isHealthy = false;
                    issues.Add($"{operation.Operation} average duration is too high: {operation.AverageDuration}ms");
                }
                else if (operation.AverageDuration > 50)
                {
                    isDegraded = true;
                    issues.Add($"{operation.Operation} average duration is elevated: {operation.AverageDuration}ms");
                }
            }
            
            healthData["total_operations"] = stats.Operations.Count;
            healthData["issues"] = issues;
            
            if (!isHealthy)
            {
                return HealthCheckResult.Unhealthy(
                    $"Performance issues detected: {string.Join("; ", issues)}",
                    data: healthData);
            }
            
            if (isDegraded)
            {
                return HealthCheckResult.Degraded(
                    $"Performance warnings: {string.Join("; ", issues)}",
                    data: healthData);
            }
            
            var totalRequests = stats.Operations.Sum(o => o.TotalRequests);
            var avgCacheHitRatio = stats.Operations.Any() 
                ? stats.Operations.Average(o => o.CacheHitRatio) 
                : 0;
            
            return HealthCheckResult.Healthy(
                $"Performance is optimal (avg cache hit ratio: {avgCacheHitRatio:F1}%, total requests: {totalRequests})",
                data: healthData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Performance health check failed");
            return HealthCheckResult.Unhealthy(
                "Performance health check failed",
                ex);
        }
    }
}
