using Microsoft.Extensions.Diagnostics.HealthChecks;
using Common.Caching;
using Common.Monitoring;
using Contracts.Services;
using System.Diagnostics;

namespace UserService.HealthChecks;

/// <summary>
/// Enhanced health check for permission cache with performance metrics
/// Phase 11 Session 2 - Monitoring Infrastructure
/// </summary>
public class EnhancedPermissionCacheHealthCheck : IHealthCheck
{
    private readonly ICacheService _cacheService;
    private readonly IPermissionService _permissionService;
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger<EnhancedPermissionCacheHealthCheck> _logger;

    public EnhancedPermissionCacheHealthCheck(
        ICacheService cacheService,
        IPermissionService permissionService,
        IMetricsCollector metricsCollector,
        ILogger<EnhancedPermissionCacheHealthCheck> logger)
    {
        _cacheService = cacheService;
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
            // Test 1: Basic Cache Connectivity
            await TestCacheConnectivity(healthData, issues);
            
            // Test 2: Cache Performance
            await TestCachePerformance(healthData, issues);
            
            // Test 3: Permission Cache Functionality
            await TestPermissionCacheFunctionality(healthData, issues);
            
            // Test 4: Cache Hit Ratio Analysis
            await TestCacheHitRatio(healthData, issues);
            
            stopwatch.Stop();
            
            // Record health check metrics
            var isHealthy = issues.Count == 0;
            _metricsCollector.RecordHealthCheck("permission_cache", isHealthy, stopwatch.Elapsed);
            
            healthData["check_duration_ms"] = stopwatch.ElapsedMilliseconds;
            healthData["timestamp"] = DateTime.UtcNow;
            healthData["issues_found"] = issues.Count;
            
            if (issues.Count == 0)
            {
                return HealthCheckResult.Healthy(
                    "Permission cache is operating optimally",
                    healthData);
            }
            else if (issues.Any(i => i.Contains("CRITICAL")))
            {
                return HealthCheckResult.Unhealthy(
                    $"Critical permission cache issues: {string.Join("; ", issues)}",
                    data: healthData);
            }
            else
            {
                return HealthCheckResult.Degraded(
                    $"Permission cache performance issues: {string.Join("; ", issues)}",
                    data: healthData);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsCollector.RecordHealthCheck("permission_cache", false, stopwatch.Elapsed);
            
            _logger.LogError(ex, "Permission cache health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Permission cache health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["check_duration_ms"] = stopwatch.ElapsedMilliseconds,
                    ["error_type"] = ex.GetType().Name,
                    ["error_message"] = ex.Message
                });
        }
    }

    private async Task TestCacheConnectivity(Dictionary<string, object> healthData, List<string> issues)
    {
        var connectivityStopwatch = Stopwatch.StartNew();
        
        try
        {
            // Test basic connectivity with a simple ping
            var testKey = $"health_check_test_{Guid.NewGuid()}";
            var testValue = DateTime.UtcNow.ToString();
            
            await _cacheService.SetAsync(testKey, testValue, TimeSpan.FromSeconds(10));
            var retrievedValue = await _cacheService.GetAsync<string>(testKey);
            await _cacheService.RemoveAsync(testKey);
            
            connectivityStopwatch.Stop();
            
            if (retrievedValue != testValue)
            {
                issues.Add("CRITICAL: Cache connectivity test failed - data integrity issue");
            }
            
            healthData["connectivity_test_ms"] = connectivityStopwatch.ElapsedMilliseconds;
            healthData["connectivity_status"] = retrievedValue == testValue ? "PASS" : "FAIL";
            
            if (connectivityStopwatch.ElapsedMilliseconds > 100)
            {
                issues.Add($"WARNING: Cache connectivity slow ({connectivityStopwatch.ElapsedMilliseconds}ms > 100ms)");
            }
        }
        catch (Exception ex)
        {
            connectivityStopwatch.Stop();
            issues.Add($"CRITICAL: Cache connectivity failed - {ex.Message}");
            healthData["connectivity_error"] = ex.Message;
        }
    }

    private async Task TestCachePerformance(Dictionary<string, object> healthData, List<string> issues)
    {
        const int testOperations = 10;
        var performanceStopwatch = Stopwatch.StartNew();
        var operationTimes = new List<long>();
        
        try
        {
            for (int i = 0; i < testOperations; i++)
            {
                var opStopwatch = Stopwatch.StartNew();
                var testKey = $"perf_test_{i}_{Guid.NewGuid()}";
                
                await _cacheService.SetAsync(testKey, $"test_value_{i}", TimeSpan.FromSeconds(10));
                await _cacheService.GetAsync<string>(testKey);
                await _cacheService.RemoveAsync(testKey);
                
                opStopwatch.Stop();
                operationTimes.Add(opStopwatch.ElapsedMilliseconds);
            }
            
            performanceStopwatch.Stop();
            
            var avgOperationTime = operationTimes.Average();
            var maxOperationTime = operationTimes.Max();
            var p95OperationTime = operationTimes.OrderBy(x => x).Skip((int)(testOperations * 0.95)).First();
            
            healthData["performance_test_operations"] = testOperations;
            healthData["avg_operation_time_ms"] = avgOperationTime;
            healthData["max_operation_time_ms"] = maxOperationTime;
            healthData["p95_operation_time_ms"] = p95OperationTime;
            healthData["total_performance_test_ms"] = performanceStopwatch.ElapsedMilliseconds;
            
            // Performance thresholds
            if (avgOperationTime > 10)
            {
                issues.Add($"WARNING: Average cache operation time high ({avgOperationTime:F1}ms > 10ms)");
            }
            
            if (maxOperationTime > 50)
            {
                issues.Add($"WARNING: Maximum cache operation time high ({maxOperationTime}ms > 50ms)");
            }
            
            if (p95OperationTime > 25)
            {
                issues.Add($"WARNING: P95 cache operation time high ({p95OperationTime}ms > 25ms)");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"CRITICAL: Cache performance test failed - {ex.Message}");
            healthData["performance_test_error"] = ex.Message;
        }
    }

    private async Task TestPermissionCacheFunctionality(Dictionary<string, object> healthData, List<string> issues)
    {
        try
        {
            // Test permission-specific cache operations
            var testUserId = 99999; // Use a test user ID that won't conflict
            var testPermissions = new[] { "test.permission.1", "test.permission.2" };
            var cacheKey = $"user_permissions:{testUserId}";
            
            var functionalityStopwatch = Stopwatch.StartNew();
            
            // Test setting permissions in cache
            await _cacheService.SetAsync(cacheKey, testPermissions, TimeSpan.FromMinutes(1));
            
            // Test retrieving permissions from cache
            var cachedPermissions = await _cacheService.GetAsync<string[]>(cacheKey);
            
            // Test cache invalidation
            await _cacheService.RemoveAsync(cacheKey);
            var afterInvalidation = await _cacheService.GetAsync<string[]>(cacheKey);
            
            functionalityStopwatch.Stop();
            
            healthData["permission_cache_test_ms"] = functionalityStopwatch.ElapsedMilliseconds;
            healthData["permission_cache_set"] = cachedPermissions != null ? "PASS" : "FAIL";
            healthData["permission_cache_invalidate"] = afterInvalidation == null ? "PASS" : "FAIL";
            
            if (cachedPermissions == null || !cachedPermissions.SequenceEqual(testPermissions))
            {
                issues.Add("CRITICAL: Permission cache functionality test failed - data not properly cached");
            }
            
            if (afterInvalidation != null)
            {
                issues.Add("WARNING: Permission cache invalidation may not be working properly");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"CRITICAL: Permission cache functionality test failed - {ex.Message}");
            healthData["permission_cache_functionality_error"] = ex.Message;
        }
    }

    private async Task TestCacheHitRatio(Dictionary<string, object> healthData, List<string> issues)
    {
        try
        {
            // Get cache hit ratio from metrics collector
            var realtimeMetrics = await _metricsCollector.GetRealTimeMetricsAsync();
            
            if (realtimeMetrics.TryGetValue("current_hour", out var currentHourObj))
            {
                if (currentHourObj is Dictionary<string, object> currentHour)
                {
                    if (currentHour.TryGetValue("cache_hit_rate", out var hitRateObj) && 
                        hitRateObj is double hitRate)
                    {
                        healthData["cache_hit_rate_percent"] = hitRate;
                        
                        if (hitRate < 70)
                        {
                            issues.Add($"CRITICAL: Cache hit rate critically low ({hitRate:F1}% < 70%)");
                        }
                        else if (hitRate < 85)
                        {
                            issues.Add($"WARNING: Cache hit rate below optimal ({hitRate:F1}% < 85%)");
                        }
                        else if (hitRate >= 95)
                        {
                            healthData["cache_performance"] = "EXCELLENT";
                        }
                        else
                        {
                            healthData["cache_performance"] = "GOOD";
                        }
                    }
                }
            }
            else
            {
                healthData["cache_hit_rate_status"] = "NO_DATA";
                issues.Add("INFO: No cache hit rate data available yet");
            }
        }
        catch (Exception ex)
        {
            healthData["cache_hit_ratio_error"] = ex.Message;
            _logger.LogWarning(ex, "Failed to retrieve cache hit ratio metrics");
        }
    }
}
