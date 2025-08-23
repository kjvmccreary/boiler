using Microsoft.Extensions.Diagnostics.HealthChecks;
using Common.Monitoring;
using System.Diagnostics;

namespace UserService.HealthChecks;

/// <summary>
/// System-wide health check that provides overall system health summary
/// Phase 11 Session 2 - Monitoring Infrastructure
/// </summary>
public class SystemHealthCheck : IHealthCheck
{
    private readonly IMetricsCollector _metricsCollector;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SystemHealthCheck> _logger;

    public SystemHealthCheck(
        IMetricsCollector metricsCollector,
        IServiceProvider serviceProvider,
        ILogger<SystemHealthCheck> logger)
    {
        _metricsCollector = metricsCollector;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var healthData = new Dictionary<string, object>();
        
        try
        {
            // Get system metrics and health status
            var systemMetrics = await _metricsCollector.GetSummaryAsync(TimeSpan.FromMinutes(5)); // 🔧 FIX: Use GetSummaryAsync instead of GetSystemMetricsAsync
            var monitoringHealthStatus = await _metricsCollector.GetHealthStatusAsync();
            var realtimeMetrics = await _metricsCollector.GetRealTimeMetricsAsync();
            
            stopwatch.Stop();
            
            // Analyze overall system health based on metrics
            var issues = new List<string>();
            var healthScore = CalculateSystemHealthScore(systemMetrics, monitoringHealthStatus, realtimeMetrics, issues);
            
            healthData["health_score_percent"] = healthScore;
            healthData["system_check_duration_ms"] = stopwatch.ElapsedMilliseconds;
            healthData["timestamp"] = DateTime.UtcNow;
            
            // System metrics summary
            healthData["system_metrics"] = new
            {
                total_requests = systemMetrics.TotalRequests,
                error_rate_percent = systemMetrics.ErrorRate,
                avg_response_time_ms = systemMetrics.AverageResponseTime,
                permission_checks = systemMetrics.TotalPermissionChecks,
                cache_hit_rate_percent = systemMetrics.PermissionCacheHitRate,
                security_events = systemMetrics.TotalSecurityEvents
            };
            
            // Health status details
            healthData["health_checks"] = new
            {
                total_checks = monitoringHealthStatus.Checks.Count,
                healthy_checks = monitoringHealthStatus.Checks.Count(c => c.Value.IsHealthy),
                unhealthy_checks = monitoringHealthStatus.Checks.Count(c => !c.Value.IsHealthy),
                overall_status = monitoringHealthStatus.Status
            };
            
            // Real-time metrics
            if (realtimeMetrics.TryGetValue("current_hour", out var currentHourObj))
            {
                healthData["current_hour_metrics"] = currentHourObj;
            }
            
            // Performance indicators
            healthData["performance_indicators"] = new
            {
                database_avg_query_time = systemMetrics.AverageDatabaseQueryTime,
                redis_avg_response_time = systemMetrics.AverageRedisResponseTime,
                requests_per_second = systemMetrics.RequestsPerSecond,
                permission_grant_rate = systemMetrics.PermissionGrantRate
            };
            
            healthData["issues"] = issues;
            healthData["issue_count"] = issues.Count;
            
            // Record system health metrics
            var isSystemHealthy = healthScore >= 80; // 80% threshold for healthy
            _metricsCollector.RecordHealthCheck("system_overall", isSystemHealthy, stopwatch.Elapsed);
            
            // Determine overall status and message
            if (healthScore >= 95)
            {
                return HealthCheckResult.Healthy(
                    $"System excellent: Health score {healthScore:F1}% - All systems operating optimally",
                    healthData);
            }
            else if (healthScore >= 80)
            {
                return HealthCheckResult.Healthy(
                    $"System healthy: Health score {healthScore:F1}% - Minor issues detected but system stable",
                    healthData);
            }
            else if (healthScore >= 60)
            {
                return HealthCheckResult.Degraded(
                    $"System degraded: Health score {healthScore:F1}% - Performance issues detected: {string.Join(", ", issues)}",
                    data: healthData);
            }
            else
            {
                return HealthCheckResult.Unhealthy(
                    $"System unhealthy: Health score {healthScore:F1}% - Critical issues: {string.Join(", ", issues)}",
                    data: healthData);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsCollector.RecordHealthCheck("system_overall", false, stopwatch.Elapsed);
            
            _logger.LogError(ex, "System health check failed");
            
            return HealthCheckResult.Unhealthy(
                "System health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["system_check_duration_ms"] = stopwatch.ElapsedMilliseconds,
                    ["error_type"] = ex.GetType().Name,
                    ["error_message"] = ex.Message,
                    ["timestamp"] = DateTime.UtcNow
                });
        }
    }

    private double CalculateSystemHealthScore(
        MetricsSummary systemMetrics, 
        Common.Monitoring.HealthStatus monitoringHealthStatus,
        Dictionary<string, object> realtimeMetrics, 
        List<string> issues)
    {
        double totalScore = 100.0;
        
        try
        {
            // Error rate impact (25% of total score)
            if (systemMetrics.ErrorRate > 10)
            {
                totalScore -= 25;
                issues.Add($"High error rate: {systemMetrics.ErrorRate:F1}%");
            }
            else if (systemMetrics.ErrorRate > 5)
            {
                totalScore -= 15;
                issues.Add($"Elevated error rate: {systemMetrics.ErrorRate:F1}%");
            }
            else if (systemMetrics.ErrorRate > 1)
            {
                totalScore -= 5;
            }
            
            // Response time impact (20% of total score)
            if (systemMetrics.AverageResponseTime > 1000)
            {
                totalScore -= 20;
                issues.Add($"Slow response time: {systemMetrics.AverageResponseTime:F0}ms");
            }
            else if (systemMetrics.AverageResponseTime > 500)
            {
                totalScore -= 10;
                issues.Add($"Elevated response time: {systemMetrics.AverageResponseTime:F0}ms");
            }
            else if (systemMetrics.AverageResponseTime > 200)
            {
                totalScore -= 5;
            }
            
            // Cache performance impact (15% of total score)
            if (systemMetrics.PermissionCacheHitRate < 70)
            {
                totalScore -= 15;
                issues.Add($"Poor cache performance: {systemMetrics.PermissionCacheHitRate:F1}%");
            }
            else if (systemMetrics.PermissionCacheHitRate < 85)
            {
                totalScore -= 10;
                issues.Add($"Suboptimal cache performance: {systemMetrics.PermissionCacheHitRate:F1}%");
            }
            else if (systemMetrics.PermissionCacheHitRate < 95)
            {
                totalScore -= 5;
            }
            
            // Database performance impact (15% of total score)
            if (systemMetrics.AverageDatabaseQueryTime > 500)
            {
                totalScore -= 15;
                issues.Add($"Slow database queries: {systemMetrics.AverageDatabaseQueryTime:F1}ms");
            }
            else if (systemMetrics.AverageDatabaseQueryTime > 200)
            {
                totalScore -= 10;
                issues.Add($"Elevated database query time: {systemMetrics.AverageDatabaseQueryTime:F1}ms");
            }
            else if (systemMetrics.AverageDatabaseQueryTime > 100)
            {
                totalScore -= 5;
            }
            
            // Security events impact (10% of total score)
            var recentSecurityEvents = systemMetrics.SecurityEventsBySeverity.GetValueOrDefault("High", 0) + 
                                     systemMetrics.SecurityEventsBySeverity.GetValueOrDefault("Critical", 0);
            if (recentSecurityEvents > 10)
            {
                totalScore -= 10;
                issues.Add($"High security activity: {recentSecurityEvents} high/critical events");
            }
            else if (recentSecurityEvents > 5)
            {
                totalScore -= 5;
                issues.Add($"Elevated security activity: {recentSecurityEvents} high/critical events");
            }
            
            // Health check status impact (10% of total score)
            var unhealthyChecks = monitoringHealthStatus.Checks.Count(c => !c.Value.IsHealthy);
            if (unhealthyChecks > 0)
            {
                var impact = Math.Min(10, unhealthyChecks * 3); // Max 10 points, 3 per unhealthy check
                totalScore -= impact;
                issues.Add($"Failed health checks: {unhealthyChecks}");
            }
            
            // Redis performance impact (5% of total score)
            if (systemMetrics.AverageRedisResponseTime > 50)
            {
                totalScore -= 5;
                issues.Add($"Slow Redis performance: {systemMetrics.AverageRedisResponseTime:F1}ms");
            }
            else if (systemMetrics.AverageRedisResponseTime > 20)
            {
                totalScore -= 2;
            }
            
            // Ensure score doesn't go below 0
            totalScore = Math.Max(0, totalScore);
            
            _logger.LogDebug("System health score calculated: {Score:F1}% with {IssueCount} issues", totalScore, issues.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating system health score");
            totalScore = 50; // Default to degraded if calculation fails
            issues.Add("Health score calculation error");
        }
        
        return totalScore;
    }
}
