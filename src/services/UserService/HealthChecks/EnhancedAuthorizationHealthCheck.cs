using Microsoft.Extensions.Diagnostics.HealthChecks;
using Common.Monitoring;
using Contracts.Services;
using System.Diagnostics;

namespace UserService.HealthChecks;

/// <summary>
/// Enhanced authorization system health check with performance validation
/// Phase 11 Session 2 - Monitoring Infrastructure
/// </summary>
public class EnhancedAuthorizationHealthCheck : IHealthCheck
{
    private readonly IPermissionService _permissionService;
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger<EnhancedAuthorizationHealthCheck> _logger;

    public EnhancedAuthorizationHealthCheck(
        IPermissionService permissionService,
        IMetricsCollector metricsCollector,
        ILogger<EnhancedAuthorizationHealthCheck> logger)
    {
        _permissionService = permissionService;
        _metricsCollector = metricsCollector;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var healthData = new Dictionary<string, object>();
        var issues = new List<string>();
        
        try
        {
            // Test 1: Permission Service Connectivity
            await TestPermissionServiceConnectivity(healthData, issues, cancellationToken);
            
            // Test 2: Permission Check Performance
            await TestPermissionCheckPerformance(healthData, issues, cancellationToken);
            
            // Test 3: Permission Service Load Test
            await TestPermissionServiceLoad(healthData, issues, cancellationToken);
            
            // Test 4: Recent Authorization Metrics Analysis
            await AnalyzeRecentAuthorizationMetrics(healthData, issues);
            
            stopwatch.Stop();
            
            // Record health check metrics
            var isHealthy = !issues.Any(i => i.Contains("CRITICAL"));
            _metricsCollector.RecordHealthCheck("authorization", isHealthy, stopwatch.Elapsed);
            
            healthData["check_duration_ms"] = stopwatch.ElapsedMilliseconds;
            healthData["timestamp"] = DateTime.UtcNow;
            healthData["issues_found"] = issues.Count;
            
            if (issues.Count == 0)
            {
                return HealthCheckResult.Healthy(
                    "Authorization system is operating optimally",
                    healthData);
            }
            else if (issues.Any(i => i.Contains("CRITICAL")))
            {
                return HealthCheckResult.Unhealthy(
                    $"Critical authorization issues: {string.Join("; ", issues)}",
                    data: healthData);
            }
            else
            {
                return HealthCheckResult.Degraded(
                    $"Authorization performance issues: {string.Join("; ", issues)}",
                    data: healthData);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsCollector.RecordHealthCheck("authorization", false, stopwatch.Elapsed);
            
            _logger.LogError(ex, "Authorization health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Authorization health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["check_duration_ms"] = stopwatch.ElapsedMilliseconds,
                    ["error_type"] = ex.GetType().Name,
                    ["error_message"] = ex.Message
                });
        }
    }

    private async Task TestPermissionServiceConnectivity(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        var connectivityStopwatch = Stopwatch.StartNew();
        
        try
        {
            // Test getting all available permissions (lightweight operation)
            var permissions = await _permissionService.GetAllAvailablePermissionsAsync(cancellationToken);
            
            connectivityStopwatch.Stop();
            
            var permissionCount = permissions.Count();
            healthData["available_permissions_count"] = permissionCount;
            healthData["permission_service_connectivity_ms"] = connectivityStopwatch.ElapsedMilliseconds;
            
            if (permissionCount == 0)
            {
                issues.Add("CRITICAL: Permission service returned no permissions");
            }
            else if (permissionCount < 10)
            {
                issues.Add("WARNING: Permission service returned fewer permissions than expected");
            }
            
            if (connectivityStopwatch.ElapsedMilliseconds > 500)
            {
                issues.Add($"WARNING: Permission service connectivity slow ({connectivityStopwatch.ElapsedMilliseconds}ms > 500ms)");
            }
        }
        catch (Exception ex)
        {
            connectivityStopwatch.Stop();
            issues.Add($"CRITICAL: Permission service connectivity failed - {ex.Message}");
            healthData["permission_service_connectivity_error"] = ex.Message;
        }
    }

    private async Task TestPermissionCheckPerformance(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        const int testUserId = 1; // Use first user for testing
        const string testPermission = "users.view"; // Common permission
        const int performanceTestCount = 5;
        
        var performanceTimes = new List<long>();
        var performanceStopwatch = Stopwatch.StartNew();
        
        try
        {
            for (int i = 0; i < performanceTestCount; i++)
            {
                var checkStopwatch = Stopwatch.StartNew();
                
                // Perform permission check
                var hasPermission = await _permissionService.UserHasPermissionAsync(testUserId, testPermission, cancellationToken);
                
                checkStopwatch.Stop();
                performanceTimes.Add(checkStopwatch.ElapsedMilliseconds);
                
                // Record the check for metrics
                _metricsCollector.RecordPermissionCheck(testPermission, hasPermission, checkStopwatch.Elapsed, true, 1);
            }
            
            performanceStopwatch.Stop();
            
            var avgCheckTime = performanceTimes.Average();
            var maxCheckTime = performanceTimes.Max();
            var minCheckTime = performanceTimes.Min();
            
            healthData["permission_check_avg_ms"] = avgCheckTime;
            healthData["permission_check_max_ms"] = maxCheckTime;
            healthData["permission_check_min_ms"] = minCheckTime;
            healthData["permission_check_test_count"] = performanceTestCount;
            healthData["permission_check_total_test_ms"] = performanceStopwatch.ElapsedMilliseconds;
            
            // Performance thresholds based on Phase 10 requirements
            if (avgCheckTime > 10)
            {
                issues.Add($"CRITICAL: Average permission check time exceeds threshold ({avgCheckTime:F1}ms > 10ms)");
            }
            else if (avgCheckTime > 5)
            {
                issues.Add($"WARNING: Average permission check time elevated ({avgCheckTime:F1}ms > 5ms)");
            }
            
            if (maxCheckTime > 50)
            {
                issues.Add($"WARNING: Maximum permission check time high ({maxCheckTime}ms > 50ms)");
            }
            
            // Check for consistency
            var timeVariance = performanceTimes.Select(t => Math.Pow(t - avgCheckTime, 2)).Average();
            var standardDeviation = Math.Sqrt(timeVariance);
            
            healthData["permission_check_std_dev"] = standardDeviation;
            
            if (standardDeviation > avgCheckTime * 0.5) // 50% of average
            {
                issues.Add($"WARNING: Permission check performance inconsistent (std dev: {standardDeviation:F1}ms)");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"CRITICAL: Permission check performance test failed - {ex.Message}");
            healthData["permission_check_performance_error"] = ex.Message;
        }
    }

    private async Task TestPermissionServiceLoad(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        const int loadTestOperations = 20;
        const int concurrentOperations = 5;
        
        var loadTestStopwatch = Stopwatch.StartNew();
        
        try
        {
            var semaphore = new SemaphoreSlim(concurrentOperations, concurrentOperations);
            var tasks = new List<Task<(bool success, long duration)>>();
            
            for (int i = 0; i < loadTestOperations; i++)
            {
                tasks.Add(PerformConcurrentPermissionCheck(semaphore, i % 3 + 1, "users.view", cancellationToken));
            }
            
            var results = await Task.WhenAll(tasks);
            
            loadTestStopwatch.Stop();
            
            var successfulChecks = results.Count(r => r.success);
            var avgDuration = results.Where(r => r.success).Select(r => r.duration).DefaultIfEmpty(0).Average();
            var maxDuration = results.Where(r => r.success).Select(r => r.duration).DefaultIfEmpty(0).Max();
            
            healthData["load_test_operations"] = loadTestOperations;
            healthData["load_test_concurrent"] = concurrentOperations;
            healthData["load_test_successful"] = successfulChecks;
            healthData["load_test_success_rate"] = (double)successfulChecks / loadTestOperations * 100;
            healthData["load_test_avg_duration_ms"] = avgDuration;
            healthData["load_test_max_duration_ms"] = maxDuration;
            healthData["load_test_total_ms"] = loadTestStopwatch.ElapsedMilliseconds;
            
            var successRate = (double)successfulChecks / loadTestOperations;
            
            if (successRate < 0.95) // 95% success rate threshold
            {
                issues.Add($"CRITICAL: Load test success rate low ({successRate:P} < 95%)");
            }
            else if (successRate < 0.99)
            {
                issues.Add($"WARNING: Load test success rate below optimal ({successRate:P} < 99%)");
            }
            
            if (avgDuration > 15)
            {
                issues.Add($"WARNING: Load test average duration high under concurrent load ({avgDuration:F1}ms > 15ms)");
            }
            
            if (maxDuration > 100)
            {
                issues.Add($"WARNING: Load test maximum duration high ({maxDuration}ms > 100ms)");
            }
        }
        catch (Exception ex)
        {
            loadTestStopwatch.Stop();
            issues.Add($"CRITICAL: Permission service load test failed - {ex.Message}");
            healthData["load_test_error"] = ex.Message;
        }
    }

    private async Task<(bool success, long duration)> PerformConcurrentPermissionCheck(
        SemaphoreSlim semaphore, 
        int userId, 
        string permission, 
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken);
        
        try
        {
            var stopwatch = Stopwatch.StartNew();
            var result = await _permissionService.UserHasPermissionAsync(userId, permission, cancellationToken);
            stopwatch.Stop();
            
            return (true, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Concurrent permission check failed for user {UserId}", userId);
            return (false, 0);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task AnalyzeRecentAuthorizationMetrics(Dictionary<string, object> healthData, List<string> issues)
    {
        try
        {
            // Get recent metrics from the metrics collector
            var realtimeMetrics = await _metricsCollector.GetRealTimeMetricsAsync();
            
            if (realtimeMetrics.TryGetValue("current_hour", out var currentHourObj))
            {
                if (currentHourObj is Dictionary<string, object> currentHour)
                {
                    if (currentHour.TryGetValue("permission_checks", out var checksObj) && 
                        checksObj is long permissionChecks)
                    {
                        healthData["recent_permission_checks"] = permissionChecks;
                        
                        if (permissionChecks == 0)
                        {
                            healthData["activity_status"] = "NO_ACTIVITY";
                        }
                        else if (permissionChecks > 1000)
                        {
                            healthData["activity_status"] = "HIGH_ACTIVITY";
                        }
                        else
                        {
                            healthData["activity_status"] = "NORMAL_ACTIVITY";
                        }
                    }
                }
            }
            
            // Get trends for permission checks
            var trends = await _metricsCollector.GetTrendsAsync("permission_checks", TimeSpan.FromHours(6));
            
            if (trends.Count >= 2)
            {
                var recentTrend = trends.TakeLast(3).Select(t => t.Value).ToList();
                var avgRecent = recentTrend.Average();
                var isIncreasing = recentTrend.Last() > recentTrend.First();
                
                healthData["permission_check_trend_6h"] = isIncreasing ? "INCREASING" : "DECREASING";
                healthData["permission_check_avg_recent"] = avgRecent;
                
                if (avgRecent > 2000) // High load threshold
                {
                    issues.Add($"INFO: High authorization load detected (avg: {avgRecent:F0} checks/hour)");
                }
            }
        }
        catch (Exception ex)
        {
            healthData["metrics_analysis_error"] = ex.Message;
            _logger.LogWarning(ex, "Failed to analyze recent authorization metrics");
        }
    }
}
