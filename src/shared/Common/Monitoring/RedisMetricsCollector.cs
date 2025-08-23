using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using DTOs.Monitoring;

namespace Common.Monitoring;

/// <summary>
/// Redis-based metrics collector with high performance and persistence
/// Phase 11 Session 2 - Monitoring Infrastructure
/// </summary>
public class RedisMetricsCollector : IMetricsCollector
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<RedisMetricsCollector> _logger;
    
    // Metric key prefixes
    private const string PERMISSION_KEY_PREFIX = "metrics:permission";
    private const string CACHE_KEY_PREFIX = "metrics:cache";
    private const string SECURITY_KEY_PREFIX = "metrics:security";
    private const string REQUEST_KEY_PREFIX = "metrics:request";
    private const string SYSTEM_KEY_PREFIX = "metrics:system";
    private const string HEALTH_KEY_PREFIX = "metrics:health";

    public RedisMetricsCollector(IDistributedCache cache, ILogger<RedisMetricsCollector> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public void RecordPermissionCheck(string permission, bool granted, TimeSpan duration, bool cacheHit = false, int tenantId = 0)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{PERMISSION_KEY_PREFIX}:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<PermissionHourlyMetrics>(key);
                
                metrics.TotalChecks++;
                metrics.TotalDuration += duration.TotalMilliseconds;
                
                if (granted) metrics.GrantedChecks++;
                else metrics.DeniedChecks++;
                
                if (cacheHit) metrics.CacheHits++;
                
                // Track by permission
                if (!metrics.PermissionBreakdown.ContainsKey(permission))
                    metrics.PermissionBreakdown[permission] = new PermissionStats();
                
                metrics.PermissionBreakdown[permission].Count++;
                metrics.PermissionBreakdown[permission].TotalDuration += duration.TotalMilliseconds;
                if (granted) metrics.PermissionBreakdown[permission].Granted++;
                
                // Track by tenant
                if (tenantId > 0)
                {
                    if (!metrics.TenantBreakdown.ContainsKey(tenantId))
                        metrics.TenantBreakdown[tenantId] = new TenantPermissionStats();
                    
                    metrics.TenantBreakdown[tenantId].Count++;
                    if (granted) metrics.TenantBreakdown[tenantId].Granted++;
                }
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record permission check metric");
        }
    }

    public void RecordRoleChange(string changeType, int roleId, int tenantId)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{SYSTEM_KEY_PREFIX}:roles:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<RoleChangeHourlyMetrics>(key);
                
                metrics.TotalChanges++;
                
                if (!metrics.ChangesByType.ContainsKey(changeType))
                    metrics.ChangesByType[changeType] = 0;
                metrics.ChangesByType[changeType]++;
                
                if (tenantId > 0)
                {
                    if (!metrics.TenantBreakdown.ContainsKey(tenantId))
                        metrics.TenantBreakdown[tenantId] = 0;
                    metrics.TenantBreakdown[tenantId]++;
                }
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record role change metric");
        }
    }

    public void RecordAuthorizationPerformance(string operation, TimeSpan duration, int tenantId)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{SYSTEM_KEY_PREFIX}:auth:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<AuthorizationHourlyMetrics>(key);
                
                metrics.TotalOperations++;
                metrics.TotalDuration += duration.TotalMilliseconds;
                
                if (!metrics.OperationBreakdown.ContainsKey(operation))
                    metrics.OperationBreakdown[operation] = new OperationStats();
                
                var opStats = metrics.OperationBreakdown[operation];
                opStats.Count++;
                opStats.TotalDuration += duration.TotalMilliseconds;
                opStats.MinDuration = Math.Min(opStats.MinDuration, duration.TotalMilliseconds);
                opStats.MaxDuration = Math.Max(opStats.MaxDuration, duration.TotalMilliseconds);
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record authorization performance metric");
        }
    }

    public void RecordCacheHit(string cacheKey, bool hit, TimeSpan? duration = null)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{CACHE_KEY_PREFIX}:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<CacheHourlyMetrics>(key);
                
                metrics.TotalOperations++;
                if (hit) metrics.Hits++;
                else metrics.Misses++;
                
                if (duration.HasValue)
                {
                    metrics.TotalDuration += duration.Value.TotalMilliseconds;
                }
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record cache hit metric");
        }
    }

    public void RecordCacheOperation(string operation, string key, TimeSpan duration, bool success)
    {
        RecordCacheHit(key, success, duration);
    }

    public void RecordSecurityEvent(string eventType, string severity, int tenantId = 0)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{SECURITY_KEY_PREFIX}:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<SecurityHourlyMetrics>(key);
                
                metrics.TotalEvents++;
                
                if (!metrics.EventsByType.ContainsKey(eventType))
                    metrics.EventsByType[eventType] = 0;
                metrics.EventsByType[eventType]++;
                
                if (!metrics.EventsBySeverity.ContainsKey(severity))
                    metrics.EventsBySeverity[severity] = 0;
                metrics.EventsBySeverity[severity]++;
                
                if (tenantId > 0)
                {
                    if (!metrics.TenantBreakdown.ContainsKey(tenantId))
                        metrics.TenantBreakdown[tenantId] = 0;
                    metrics.TenantBreakdown[tenantId]++;
                }
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record security event metric");
        }
    }

    public void RecordFailedLogin(string reason, string ipAddress, int tenantId = 0)
    {
        RecordSecurityEvent("FailedLogin", "Medium", tenantId);
    }

    public void RecordSuspiciousActivity(string activityType, string details, int tenantId = 0)
    {
        RecordSecurityEvent($"SuspiciousActivity_{activityType}", "High", tenantId);
    }

    public void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration, int tenantId = 0)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{REQUEST_KEY_PREFIX}:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<RequestHourlyMetrics>(key);
                
                metrics.TotalRequests++;
                metrics.TotalDuration += duration.TotalMilliseconds;
                
                if (statusCode >= 400) metrics.ErrorCount++;
                if (duration.TotalMilliseconds > 1000) metrics.SlowRequestCount++;
                
                // Track by endpoint
                var endpoint = $"{method} {path}";
                if (!metrics.EndpointBreakdown.ContainsKey(endpoint))
                    metrics.EndpointBreakdown[endpoint] = new EndpointStats();
                
                var endpointStats = metrics.EndpointBreakdown[endpoint];
                endpointStats.Count++;
                endpointStats.TotalDuration += duration.TotalMilliseconds;
                if (statusCode >= 400) endpointStats.ErrorCount++;
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record HTTP request metric");
        }
    }

    public void RecordApiCall(string endpoint, bool success, TimeSpan duration, int tenantId = 0)
    {
        var statusCode = success ? 200 : 500;
        RecordHttpRequest("API", endpoint, statusCode, duration, tenantId);
    }

    public void RecordDatabaseQuery(string operation, TimeSpan duration, bool success)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{SYSTEM_KEY_PREFIX}:database:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<DatabaseHourlyMetrics>(key);
                
                metrics.TotalQueries++;
                metrics.TotalDuration += duration.TotalMilliseconds;
                
                if (!success) metrics.ErrorCount++;
                
                if (!metrics.OperationBreakdown.ContainsKey(operation))
                    metrics.OperationBreakdown[operation] = new OperationStats();
                
                var opStats = metrics.OperationBreakdown[operation];
                opStats.Count++;
                opStats.TotalDuration += duration.TotalMilliseconds;
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record database query metric");
        }
    }

    public void RecordRedisOperation(string operation, TimeSpan duration, bool success)
    {
        try
        {
            var timestamp = GetHourlyTimestamp();
            var key = $"{SYSTEM_KEY_PREFIX}:redis:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<RedisHourlyMetrics>(key);
                
                metrics.TotalOperations++;
                metrics.TotalDuration += duration.TotalMilliseconds;
                
                if (!success) metrics.ErrorCount++;
                
                if (!metrics.OperationBreakdown.ContainsKey(operation))
                    metrics.OperationBreakdown[operation] = new OperationStats();
                
                var opStats = metrics.OperationBreakdown[operation];
                opStats.Count++;
                opStats.TotalDuration += duration.TotalMilliseconds;
                
                await SaveHourlyMetrics(key, metrics);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record Redis operation metric");
        }
    }

    public void RecordHealthCheck(string checkName, bool healthy, TimeSpan duration)
    {
        try
        {
            var timestamp = GetMinuteTimestamp(); // Health checks are more frequent
            var key = $"{HEALTH_KEY_PREFIX}:{timestamp}";
            
            _ = Task.Run(async () =>
            {
                var metrics = await GetOrCreateHourlyMetrics<HealthCheckMinuteMetrics>(key);
                
                metrics.TotalChecks++;
                metrics.TotalDuration += duration.TotalMilliseconds;
                
                if (!metrics.CheckBreakdown.ContainsKey(checkName))
                    metrics.CheckBreakdown[checkName] = new HealthCheckStats();
                
                var checkStats = metrics.CheckBreakdown[checkName];
                checkStats.Count++;
                checkStats.TotalDuration += duration.TotalMilliseconds;
                if (healthy) checkStats.SuccessCount++;
                
                await SaveHourlyMetrics(key, metrics, TimeSpan.FromMinutes(5)); // Shorter retention
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record health check metric");
        }
    }

    public async Task<MetricsSummary> GetSummaryAsync(TimeSpan period)
    {
        var summary = new MetricsSummary
        {
            Period = period,
            EndTime = DateTime.UtcNow,
            StartTime = DateTime.UtcNow - period
        };

        try
        {
            var hours = (int)Math.Ceiling(period.TotalHours);
            var tasks = new List<Task>();

            // Collect request metrics
            tasks.Add(CollectRequestMetrics(summary, hours));
            
            // Collect permission metrics
            tasks.Add(CollectPermissionMetrics(summary, hours));
            
            // Collect cache metrics
            tasks.Add(CollectCacheMetrics(summary, hours));
            
            // Collect security metrics
            tasks.Add(CollectSecurityMetrics(summary, hours));
            
            // Collect system metrics
            tasks.Add(CollectSystemMetrics(summary, hours));

            await Task.WhenAll(tasks);
            
            // Calculate derived metrics
            CalculateDerivedMetrics(summary);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate metrics summary");
        }

        return summary;
    }

    // 🔧 ADD: Missing GetSystemMetricsAsync method
    public async Task<MetricsSummary> GetSystemMetricsAsync(TimeSpan period)
    {
        // For now, this is an alias for GetSummaryAsync
        // In the future, this could provide a more system-focused view
        return await GetSummaryAsync(period);
    }

    public async Task<Dictionary<string, object>> GetRealTimeMetricsAsync()
    {
        var metrics = new Dictionary<string, object>();
        
        try
        {
            var currentHour = GetHourlyTimestamp();
            var previousHour = GetHourlyTimestamp(DateTime.UtcNow.AddHours(-1));

            // Get current hour metrics
            var requestMetrics = await GetHourlyMetrics<RequestHourlyMetrics>($"{REQUEST_KEY_PREFIX}:{currentHour}");
            var permissionMetrics = await GetHourlyMetrics<PermissionHourlyMetrics>($"{PERMISSION_KEY_PREFIX}:{currentHour}");
            var cacheMetrics = await GetHourlyMetrics<CacheHourlyMetrics>($"{CACHE_KEY_PREFIX}:{currentHour}");

            metrics["current_hour"] = new
            {
                requests = requestMetrics?.TotalRequests ?? 0,
                errors = requestMetrics?.ErrorCount ?? 0,
                avg_response_time = requestMetrics?.TotalRequests > 0 
                    ? (requestMetrics.TotalDuration / requestMetrics.TotalRequests) 
                    : 0,
                permission_checks = permissionMetrics?.TotalChecks ?? 0,
                cache_hit_rate = cacheMetrics?.TotalOperations > 0 
                    ? (double)(cacheMetrics.Hits) / cacheMetrics.TotalOperations * 100 
                    : 0
            };

            metrics["timestamp"] = DateTime.UtcNow;
            metrics["period"] = "current_hour";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get real-time metrics");
        }

        return metrics;
    }

    public async Task<List<MetricTrend>> GetTrendsAsync(string metricName, TimeSpan period)
    {
        var trends = new List<MetricTrend>();
        
        try
        {
            var hours = (int)Math.Ceiling(period.TotalHours);
            var endTime = DateTime.UtcNow;

            for (int i = 0; i < hours; i++)
            {
                var hourTime = endTime.AddHours(-i);
                var timestamp = GetHourlyTimestamp(hourTime);
                
                var trend = new MetricTrend
                {
                    Timestamp = hourTime,
                    MetricName = metricName
                };

                // Get metric value based on type
                switch (metricName.ToLower())
                {
                    case "requests":
                        var requestMetrics = await GetHourlyMetrics<RequestHourlyMetrics>($"{REQUEST_KEY_PREFIX}:{timestamp}");
                        trend.Value = requestMetrics?.TotalRequests ?? 0;
                        break;
                        
                    case "permission_checks":
                        var permissionMetrics = await GetHourlyMetrics<PermissionHourlyMetrics>($"{PERMISSION_KEY_PREFIX}:{timestamp}");
                        trend.Value = permissionMetrics?.TotalChecks ?? 0;
                        break;
                        
                    case "cache_hit_rate":
                        var cacheMetrics = await GetHourlyMetrics<CacheHourlyMetrics>($"{CACHE_KEY_PREFIX}:{timestamp}");
                        trend.Value = cacheMetrics?.TotalOperations > 0 
                            ? (double)(cacheMetrics.Hits) / cacheMetrics.TotalOperations * 100 
                            : 0;
                        break;
                }

                trends.Add(trend);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get metric trends for {MetricName}", metricName);
        }

        return trends.OrderBy(t => t.Timestamp).ToList();
    }

    public async Task<Common.Monitoring.HealthStatus> GetHealthStatusAsync() // 🔧 FIX: Fully qualified type
    {
        var healthStatus = new Common.Monitoring.HealthStatus
        {
            Timestamp = DateTime.UtcNow
        };

        try
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            // Check recent health check results
            var currentMinute = GetMinuteTimestamp();
            var healthMetrics = await GetHourlyMetrics<HealthCheckMinuteMetrics>($"{HEALTH_KEY_PREFIX}:{currentMinute}");
            
            if (healthMetrics != null)
            {
                foreach (var check in healthMetrics.CheckBreakdown)
                {
                    var successRate = check.Value.SuccessCount / (double)check.Value.Count;
                    var avgDuration = check.Value.TotalDuration / check.Value.Count;
                    
                    healthStatus.Checks[check.Key] = new Common.Monitoring.HealthStatus.HealthCheckResult
                    {
                        IsHealthy = successRate > 0.95, // 95% success rate threshold
                        Status = successRate > 0.95 ? "Healthy" : 
                                successRate > 0.8 ? "Degraded" : "Unhealthy",
                        Duration = TimeSpan.FromMilliseconds(avgDuration),
                        Description = $"Success rate: {successRate:P}, Avg duration: {avgDuration:F1}ms",
                        Data = new Dictionary<string, object>
                        {
                            ["success_rate"] = successRate,
                            ["avg_duration_ms"] = avgDuration,
                            ["total_checks"] = check.Value.Count
                        }
                    };
                }
            }
            
            stopwatch.Stop();
            healthStatus.TotalDuration = stopwatch.Elapsed;
            
            // Determine overall status
            if (healthStatus.Checks.Values.All(c => c.IsHealthy))
                healthStatus.Status = "Healthy";
            else if (healthStatus.Checks.Values.Any(c => c.Status == "Unhealthy"))
                healthStatus.Status = "Unhealthy";
            else
                healthStatus.Status = "Degraded";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get health status");
            healthStatus.Status = "Unknown";
        }

        return healthStatus;
    }

    #region Private Helper Methods

    private string GetHourlyTimestamp(DateTime? dateTime = null)
    {
        var dt = dateTime ?? DateTime.UtcNow;
        return dt.ToString("yyyyMMddHH");
    }

    private string GetMinuteTimestamp(DateTime? dateTime = null)
    {
        var dt = dateTime ?? DateTime.UtcNow;
        return dt.ToString("yyyyMMddHHmm");
    }

    private async Task<T?> GetOrCreateHourlyMetrics<T>(string key) where T : new()
    {
        var existingData = await _cache.GetStringAsync(key);
        return string.IsNullOrEmpty(existingData) 
            ? new T() 
            : JsonSerializer.Deserialize<T>(existingData);
    }

    private async Task<T?> GetHourlyMetrics<T>(string key) where T : class
    {
        var data = await _cache.GetStringAsync(key);
        return string.IsNullOrEmpty(data) ? null : JsonSerializer.Deserialize<T>(data);
    }

    private async Task SaveHourlyMetrics<T>(string key, T metrics, TimeSpan? expiration = null)
    {
        var data = JsonSerializer.Serialize(metrics);
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromHours(25) // Keep for 25 hours
        };
        
        await _cache.SetStringAsync(key, data, options);
    }

    private async Task CollectRequestMetrics(MetricsSummary summary, int hours)
    {
        var totalRequests = 0L;
        var totalErrors = 0L;
        var totalDuration = 0.0;
        var slowRequests = 0L;

        for (int i = 0; i < hours; i++)
        {
            var timestamp = GetHourlyTimestamp(DateTime.UtcNow.AddHours(-i));
            var metrics = await GetHourlyMetrics<RequestHourlyMetrics>($"{REQUEST_KEY_PREFIX}:{timestamp}");
            
            if (metrics != null)
            {
                totalRequests += metrics.TotalRequests;
                totalErrors += metrics.ErrorCount;
                totalDuration += metrics.TotalDuration;
                slowRequests += metrics.SlowRequestCount;
            }
        }

        summary.TotalRequests = totalRequests;
        summary.TotalErrors = totalErrors;
        summary.ErrorRate = totalRequests > 0 ? (double)totalErrors / totalRequests * 100 : 0;
        summary.AverageResponseTime = totalRequests > 0 ? totalDuration / totalRequests : 0;
        summary.RequestsPerSecond = totalRequests / summary.Period.TotalSeconds;
    }

    private async Task CollectPermissionMetrics(MetricsSummary summary, int hours)
    {
        var totalChecks = 0L;
        var grantedChecks = 0L;
        var totalDuration = 0.0;
        var cacheHits = 0L;

        for (int i = 0; i < hours; i++)
        {
            var timestamp = GetHourlyTimestamp(DateTime.UtcNow.AddHours(-i));
            var metrics = await GetHourlyMetrics<PermissionHourlyMetrics>($"{PERMISSION_KEY_PREFIX}:{timestamp}");
            
            if (metrics != null)
            {
                totalChecks += metrics.TotalChecks;
                grantedChecks += metrics.GrantedChecks;
                totalDuration += metrics.TotalDuration;
                cacheHits += metrics.CacheHits;
            }
        }

        summary.TotalPermissionChecks = totalChecks;
        summary.GrantedPermissions = grantedChecks;
        summary.DeniedPermissions = totalChecks - grantedChecks;
        summary.PermissionGrantRate = totalChecks > 0 ? (double)grantedChecks / totalChecks * 100 : 0;
        summary.AveragePermissionCheckTime = totalChecks > 0 ? totalDuration / totalChecks : 0;
        
        // 🔧 ADD: Calculate PermissionCacheHitRate
        summary.PermissionCacheHitRate = totalChecks > 0 ? (double)cacheHits / totalChecks * 100 : 0;
    }

    private async Task CollectCacheMetrics(MetricsSummary summary, int hours)
    {
        var totalOperations = 0L;
        var hits = 0L;
        var totalDuration = 0.0;

        for (int i = 0; i < hours; i++)
        {
            var timestamp = GetHourlyTimestamp(DateTime.UtcNow.AddHours(-i));
            var metrics = await GetHourlyMetrics<CacheHourlyMetrics>($"{CACHE_KEY_PREFIX}:{timestamp}");
            
            if (metrics != null)
            {
                totalOperations += metrics.TotalOperations;
                hits += metrics.Hits;
                totalDuration += metrics.TotalDuration;
            }
        }

        summary.TotalCacheOperations = totalOperations;
        summary.CacheHits = hits;
        summary.CacheMisses = totalOperations - hits;
        summary.CacheHitRate = totalOperations > 0 ? (double)hits / totalOperations * 100 : 0;
        summary.AverageCacheResponseTime = totalOperations > 0 ? totalDuration / totalOperations : 0;
    }

    private async Task CollectSecurityMetrics(MetricsSummary summary, int hours)
    {
        var totalEvents = 0L;
        var eventsByType = new Dictionary<string, long>();
        var eventsBySeverity = new Dictionary<string, long>();

        for (int i = 0; i < hours; i++)
        {
            var timestamp = GetHourlyTimestamp(DateTime.UtcNow.AddHours(-i));
            var metrics = await GetHourlyMetrics<SecurityHourlyMetrics>($"{SECURITY_KEY_PREFIX}:{timestamp}");
            
            if (metrics != null)
            {
                totalEvents += metrics.TotalEvents;
                
                foreach (var eventType in metrics.EventsByType)
                {
                    if (!eventsByType.ContainsKey(eventType.Key))
                        eventsByType[eventType.Key] = 0;
                    eventsByType[eventType.Key] += eventType.Value;
                }
                
                foreach (var severity in metrics.EventsBySeverity)
                {
                    if (!eventsBySeverity.ContainsKey(severity.Key))
                        eventsBySeverity[severity.Key] = 0;
                    eventsBySeverity[severity.Key] += severity.Value;
                }
            }
        }

        summary.TotalSecurityEvents = totalEvents;
        summary.SecurityEventsByType = eventsByType;
        summary.SecurityEventsBySeverity = eventsBySeverity;
        summary.FailedLogins = eventsByType.GetValueOrDefault("FailedLogin", 0);
    }

    private async Task CollectSystemMetrics(MetricsSummary summary, int hours)
    {
        var dbQueries = 0L;
        var dbDuration = 0.0;
        var redisOps = 0L;
        var redisDuration = 0.0;

        for (int i = 0; i < hours; i++)
        {
            var timestamp = GetHourlyTimestamp(DateTime.UtcNow.AddHours(-i));
            
            var dbMetrics = await GetHourlyMetrics<DatabaseHourlyMetrics>($"{SYSTEM_KEY_PREFIX}:database:{timestamp}");
            if (dbMetrics != null)
            {
                dbQueries += dbMetrics.TotalQueries;
                dbDuration += dbMetrics.TotalDuration;
            }
            
            var redisMetrics = await GetHourlyMetrics<RedisHourlyMetrics>($"{SYSTEM_KEY_PREFIX}:redis:{timestamp}");
            if (redisMetrics != null)
            {
                redisOps += redisMetrics.TotalOperations;
                redisDuration += redisMetrics.TotalDuration;
            }
        }

        summary.DatabaseQueries = dbQueries;
        summary.AverageDatabaseQueryTime = dbQueries > 0 ? dbDuration / dbQueries : 0;
        summary.RedisOperations = redisOps;
        summary.AverageRedisResponseTime = redisOps > 0 ? redisDuration / redisOps : 0;
    }

    private void CalculateDerivedMetrics(MetricsSummary summary)
    {
        // Calculate P95 response time (simplified estimation)
        summary.P95ResponseTime = summary.AverageResponseTime * 1.5; // Rough estimation
        
        // Additional calculations can be added here
    }

    #endregion

    #region Metric Data Models

    private class PermissionHourlyMetrics
    {
        public long TotalChecks { get; set; }
        public long GrantedChecks { get; set; }
        public long DeniedChecks { get; set; }
        public long CacheHits { get; set; }
        public double TotalDuration { get; set; }
        public Dictionary<string, PermissionStats> PermissionBreakdown { get; set; } = new();
        public Dictionary<int, TenantPermissionStats> TenantBreakdown { get; set; } = new();
    }

    private class PermissionStats
    {
        public long Count { get; set; }
        public long Granted { get; set; }
        public double TotalDuration { get; set; }
    }

    private class TenantPermissionStats
    {
        public long Count { get; set; }
        public long Granted { get; set; }
    }

    private class RoleChangeHourlyMetrics
    {
        public long TotalChanges { get; set; }
        public Dictionary<string, long> ChangesByType { get; set; } = new();
        public Dictionary<int, long> TenantBreakdown { get; set; } = new();
    }

    private class AuthorizationHourlyMetrics
    {
        public long TotalOperations { get; set; }
        public double TotalDuration { get; set; }
        public Dictionary<string, OperationStats> OperationBreakdown { get; set; } = new();
    }

    private class CacheHourlyMetrics
    {
        public long TotalOperations { get; set; }
        public long Hits { get; set; }
        public long Misses { get; set; }
        public double TotalDuration { get; set; }
    }

    private class SecurityHourlyMetrics
    {
        public long TotalEvents { get; set; }
        public Dictionary<string, long> EventsByType { get; set; } = new();
        public Dictionary<string, long> EventsBySeverity { get; set; } = new();
        public Dictionary<int, long> TenantBreakdown { get; set; } = new();
    }

    private class RequestHourlyMetrics
    {
        public long TotalRequests { get; set; }
        public long ErrorCount { get; set; }
        public long SlowRequestCount { get; set; }
        public double TotalDuration { get; set; }
        public Dictionary<string, EndpointStats> EndpointBreakdown { get; set; } = new();
    }

    private class DatabaseHourlyMetrics
    {
        public long TotalQueries { get; set; }
        public long ErrorCount { get; set; }
        public double TotalDuration { get; set; }
        public Dictionary<string, OperationStats> OperationBreakdown { get; set; } = new();
    }

    private class RedisHourlyMetrics
    {
        public long TotalOperations { get; set; }
        public long ErrorCount { get; set; }
        public double TotalDuration { get; set; }
        public Dictionary<string, OperationStats> OperationBreakdown { get; set; } = new();
    }

    private class HealthCheckMinuteMetrics
    {
        public long TotalChecks { get; set; }
        public double TotalDuration { get; set; }
        public Dictionary<string, HealthCheckStats> CheckBreakdown { get; set; } = new();
    }

    private class OperationStats
    {
        public long Count { get; set; }
        public double TotalDuration { get; set; }
        public double MinDuration { get; set; } = double.MaxValue;
        public double MaxDuration { get; set; }
    }

    private class EndpointStats
    {
        public long Count { get; set; }
        public long ErrorCount { get; set; }
        public double TotalDuration { get; set; }
    }

    private class HealthCheckStats
    {
        public long Count { get; set; }
        public long SuccessCount { get; set; }
        public double TotalDuration { get; set; }
    }

    #endregion
}
