using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Common.Monitoring;

namespace Common.Caching;

/// <summary>
/// Memory-based cache service implementation for testing and performance environments
/// </summary>
public class MemoryCacheService : ICacheService
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryCacheService> _logger;

    public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out T? value);
        return await Task.FromResult(value);
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        var options = new MemoryCacheEntryOptions();
        if (expiration.HasValue)
        {
            options.AbsoluteExpirationRelativeToNow = expiration;
        }
        
        _cache.Set(key, value, options);
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(string key)
    {
        _cache.Remove(key);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string key)
    {
        return await Task.FromResult(_cache.TryGetValue(key, out _));
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        // Memory cache doesn't support pattern removal - this is a no-op
        _logger.LogWarning("Pattern removal not supported in memory cache: {Pattern}", pattern);
        await Task.CompletedTask;
    }

    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out T? cached) && cached != null)
        {
            return cached;
        }

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }
}

/// <summary>
/// Memory-based permission cache implementation
/// </summary>
public class MemoryPermissionCache : IPermissionCache
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<MemoryPermissionCache> _logger;

    public MemoryPermissionCache(IMemoryCache cache, ILogger<MemoryPermissionCache> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<string>?> GetUserPermissionsAsync(int userId, int tenantId)
    {
        var key = $"permissions:user:{userId}:tenant:{tenantId}";
        _cache.TryGetValue(key, out IEnumerable<string>? permissions);
        return await Task.FromResult(permissions);
    }

    public async Task SetUserPermissionsAsync(int userId, int tenantId, IEnumerable<string> permissions)
    {
        var key = $"permissions:user:{userId}:tenant:{tenantId}";
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
        };
        _cache.Set(key, permissions.ToList(), options);
        await Task.CompletedTask;
    }

    public async Task InvalidateUserPermissionsAsync(int userId, int tenantId)
    {
        var key = $"permissions:user:{userId}:tenant:{tenantId}";
        _cache.Remove(key);
        await Task.CompletedTask;
    }

    public async Task InvalidateTenantPermissionsAsync(int tenantId)
    {
        // Note: Memory cache doesn't support pattern-based removal
        _logger.LogWarning("Tenant-wide permission invalidation not fully supported in memory cache for tenant {TenantId}", tenantId);
        await Task.CompletedTask;
    }

    public async Task<Dictionary<int, IEnumerable<string>>?> GetRolePermissionsAsync(int tenantId)
    {
        var key = $"permissions:roles:tenant:{tenantId}";
        _cache.TryGetValue(key, out Dictionary<int, IEnumerable<string>>? rolePermissions);
        return await Task.FromResult(rolePermissions);
    }

    public async Task SetRolePermissionsAsync(int tenantId, Dictionary<int, IEnumerable<string>> rolePermissions)
    {
        var key = $"permissions:roles:tenant:{tenantId}";
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        };
        _cache.Set(key, rolePermissions, options);
        await Task.CompletedTask;
    }

    public async Task InvalidateRolePermissionsAsync(int roleId, int tenantId)
    {
        var key = $"permissions:roles:tenant:{tenantId}";
        _cache.Remove(key);
        await Task.CompletedTask;
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        _logger.LogWarning("Pattern removal not supported in memory cache: {Pattern}", pattern);
        await Task.CompletedTask;
    }
}

/// <summary>
/// Memory-based metrics collector for testing
/// </summary>
public class MemoryMetricsCollector : IMetricsCollector
{
    private readonly ConcurrentDictionary<string, object> _metrics = new();
    private readonly ILogger<MemoryMetricsCollector> _logger;

    public MemoryMetricsCollector(ILogger<MemoryMetricsCollector> logger)
    {
        _logger = logger;
    }

    public void RecordPermissionCheck(string permission, bool granted, TimeSpan duration, bool cacheHit = false, int tenantId = 0)
    {
        // Store in memory for testing
    }

    public void RecordRoleChange(string changeType, int roleId, int tenantId)
    {
        // Store in memory for testing
    }

    public void RecordAuthorizationPerformance(string operation, TimeSpan duration, int tenantId)
    {
        // Store in memory for testing
    }

    public void RecordCacheHit(string cacheKey, bool hit, TimeSpan? duration = null)
    {
        // Store in memory for testing
    }

    public void RecordCacheOperation(string operation, string key, TimeSpan duration, bool success)
    {
        // Store in memory for testing
    }

    public void RecordSecurityEvent(string eventType, string severity, int tenantId = 0)
    {
        // Store in memory for testing
    }

    public void RecordFailedLogin(string reason, string ipAddress, int tenantId = 0)
    {
        // Store in memory for testing
    }

    public void RecordSuspiciousActivity(string activityType, string details, int tenantId = 0)
    {
        // Store in memory for testing
    }

    public void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration, int tenantId = 0)
    {
        // Store in memory for testing
    }

    public void RecordApiCall(string endpoint, bool success, TimeSpan duration, int tenantId = 0)
    {
        // Store in memory for testing
    }

    public void RecordDatabaseQuery(string operation, TimeSpan duration, bool success)
    {
        // Store in memory for testing
    }

    public void RecordRedisOperation(string operation, TimeSpan duration, bool success)
    {
        // Store in memory for testing
    }

    public async Task<MetricsSummary> GetSummaryAsync(TimeSpan period)
    {
        return await Task.FromResult(new MetricsSummary());
    }

    public async Task<Dictionary<string, object>> GetRealTimeMetricsAsync()
    {
        return await Task.FromResult(new Dictionary<string, object>(_metrics));
    }

    public async Task<List<MetricTrend>> GetTrendsAsync(string metricName, TimeSpan period)
    {
        return await Task.FromResult(new List<MetricTrend>());
    }

    public async Task<HealthStatus> GetHealthStatusAsync()
    {
        return await Task.FromResult(new HealthStatus 
        { 
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            TotalDuration = TimeSpan.Zero,
            Checks = new Dictionary<string, HealthStatus.HealthCheckResult>()
        });
    }

    public void RecordHealthCheck(string checkName, bool healthy, TimeSpan duration)
    {
        // Store in memory for testing
    }
}
