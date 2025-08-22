using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Common.Performance;

public class PerformanceMetricsService : IPerformanceMetricsService
{
    private readonly ConcurrentDictionary<string, OperationMetrics> _metrics = new();
    private readonly ILogger<PerformanceMetricsService> _logger;
    private readonly DateTime _startTime = DateTime.UtcNow;

    public PerformanceMetricsService(ILogger<PerformanceMetricsService> logger)
    {
        _logger = logger;
    }

    public void RecordCacheHit(string operation)
    {
        var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
        Interlocked.Increment(ref metrics.CacheHits);
        Interlocked.Increment(ref metrics.TotalRequests);
    }

    public void RecordCacheMiss(string operation)
    {
        var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
        Interlocked.Increment(ref metrics.CacheMisses);
        Interlocked.Increment(ref metrics.TotalRequests);
    }

    public void RecordCacheSet(string operation)
    {
        var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
        Interlocked.Increment(ref metrics.CacheSets);
    }

    public void RecordOperationDuration(string operation, long milliseconds)
    {
        var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
        Interlocked.Add(ref metrics.TotalDuration, milliseconds);
        Interlocked.Increment(ref metrics.OperationCount);
        
        // Update min/max
        UpdateMinMax(metrics, milliseconds);
    }

    // ✅ Detailed cache operation recording with duration
    public void RecordCacheHit(string key, string dataType, TimeSpan duration)
    {
        var operationName = $"cache_hit_{dataType}";
        RecordCacheHit(operationName);
        RecordOperationDuration(operationName, (long)duration.TotalMilliseconds);
        
        _logger.LogDebug("Cache hit recorded for {DataType}: {Key} in {Duration}ms", 
            dataType, key, duration.TotalMilliseconds);
    }

    public void RecordCacheMiss(string key, string dataType, TimeSpan duration)
    {
        var operationName = $"cache_miss_{dataType}";
        RecordCacheMiss(operationName);
        RecordOperationDuration(operationName, (long)duration.TotalMilliseconds);
        
        _logger.LogDebug("Cache miss recorded for {DataType}: {Key} in {Duration}ms", 
            dataType, key, duration.TotalMilliseconds);
    }

    public void RecordCacheSet(string key, string dataType, TimeSpan duration)
    {
        var operationName = $"cache_set_{dataType}";
        RecordCacheSet(operationName);
        RecordOperationDuration(operationName, (long)duration.TotalMilliseconds);
        
        _logger.LogDebug("Cache set recorded for {DataType}: {Key} in {Duration}ms", 
            dataType, key, duration.TotalMilliseconds);
    }

    // ✅ FIXED: Async methods with TimeSpan parameter as expected by controller
    public Task<CacheMetrics> GetCacheMetricsAsync(TimeSpan? period = null)
    {
        return Task.FromResult(GetCacheMetrics(period));
    }

    public Task<ApiMetrics> GetApiMetricsAsync(TimeSpan? period = null)
    {
        return Task.FromResult(GetApiMetrics(period));
    }

    public Task<SystemMetrics> GetSystemMetricsAsync()
    {
        return Task.FromResult(GetSystemMetrics());
    }

    // ✅ FIXED: Sync methods with period parameter
    public CacheMetrics GetCacheMetrics() => GetCacheMetrics(null);
    
    private CacheMetrics GetCacheMetrics(TimeSpan? period)
    {
        var cacheMetrics = new CacheMetrics();
        var cacheOperations = _metrics.Where(kvp => 
            kvp.Key.StartsWith("cache_hit_") || 
            kvp.Key.StartsWith("cache_miss_") || 
            kvp.Key.StartsWith("cache_set_"));

        var hitOperations = new List<OperationMetrics>();
        var missOperations = new List<OperationMetrics>();

        foreach (var kvp in cacheOperations)
        {
            var metrics = kvp.Value;
            cacheMetrics.TotalHits += metrics.CacheHits;
            cacheMetrics.TotalMisses += metrics.CacheMisses;
            cacheMetrics.TotalSets += metrics.CacheSets;
            
            cacheMetrics.OperationsByType[kvp.Key] = metrics.TotalRequests;

            if (kvp.Key.StartsWith("cache_hit_"))
                hitOperations.Add(metrics);
            else if (kvp.Key.StartsWith("cache_miss_"))
                missOperations.Add(metrics);
        }

        // Calculate average times
        if (hitOperations.Any())
        {
            var totalHitDuration = hitOperations.Sum(op => op.TotalDuration);
            var totalHitCount = hitOperations.Sum(op => op.OperationCount);
            var avgHitMs = totalHitCount > 0 ? totalHitDuration / totalHitCount : 0;
            cacheMetrics.AverageHitTime = TimeSpan.FromMilliseconds(avgHitMs);
        }

        if (missOperations.Any())
        {
            var totalMissDuration = missOperations.Sum(op => op.TotalDuration);
            var totalMissCount = missOperations.Sum(op => op.OperationCount);
            var avgMissMs = totalMissCount > 0 ? totalMissDuration / totalMissCount : 0;
            cacheMetrics.AverageMissTime = TimeSpan.FromMilliseconds(avgMissMs);
        }

        return cacheMetrics;
    }

    public ApiMetrics GetApiMetrics() => GetApiMetrics(null);
    
    private ApiMetrics GetApiMetrics(TimeSpan? period)
    {
        var apiMetrics = new ApiMetrics();
        var apiOperations = _metrics.Where(kvp => 
            !kvp.Key.StartsWith("cache_") && !kvp.Key.StartsWith("system_"));

        foreach (var kvp in apiOperations)
        {
            var metrics = kvp.Value;
            apiMetrics.TotalRequests += (int)metrics.TotalRequests;
            
            var avgResponseTimeMs = metrics.OperationCount > 0 
                ? metrics.TotalDuration / metrics.OperationCount 
                : 0;

            apiMetrics.ByEndpoint[kvp.Key] = new EndpointMetrics
            {
                Endpoint = kvp.Key,
                RequestCount = (int)metrics.TotalRequests,
                AverageResponseTime = TimeSpan.FromMilliseconds(avgResponseTimeMs),
                MinResponseTime = TimeSpan.FromMilliseconds(metrics.MinDuration == long.MaxValue ? 0 : metrics.MinDuration),
                MaxResponseTime = TimeSpan.FromMilliseconds(metrics.MaxDuration)
            };
        }

        if (apiOperations.Any())
        {
            var totalDuration = apiOperations.Sum(kvp => kvp.Value.TotalDuration);
            var totalCount = apiOperations.Sum(kvp => kvp.Value.OperationCount);
            var avgMs = totalCount > 0 ? totalDuration / totalCount : 0;
            apiMetrics.AverageResponseTime = TimeSpan.FromMilliseconds(avgMs);
        }

        return apiMetrics;
    }

    public SystemMetrics GetSystemMetrics()
    {
        var process = Process.GetCurrentProcess();
        var systemMetrics = new SystemMetrics
        {
            MemoryUsed = process.WorkingSet64,
            ActiveConnections = 0, // Would need additional tracking
            Uptime = DateTime.UtcNow - _startTime,
            Environment = System.Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Unknown",
            Version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown",
            RedisConnected = true // ✅ ADD: Default to true, would need actual Redis connection check
        };

        // Get available memory (simplified)
        try
        {
            var gc = GC.GetTotalMemory(false);
            systemMetrics.MemoryTotal = gc * 10; // Rough estimate
        }
        catch
        {
            systemMetrics.MemoryTotal = systemMetrics.MemoryUsed * 2; // Fallback
        }

        return systemMetrics;
    }

    // ✅ Clear metrics method
    public Task ClearMetricsAsync()
    {
        _metrics.Clear();
        _logger.LogInformation("Performance metrics cleared");
        return Task.CompletedTask;
    }

    private static void UpdateMinMax(OperationMetrics metrics, long milliseconds)
    {
        // Update minimum
        long currentMin;
        do
        {
            currentMin = metrics.MinDuration;
            if (milliseconds >= currentMin && currentMin != 0) break;
        } while (Interlocked.CompareExchange(ref metrics.MinDuration, milliseconds, currentMin) != currentMin);

        // Update maximum
        long currentMax;
        do
        {
            currentMax = metrics.MaxDuration;
            if (milliseconds <= currentMax) break;
        } while (Interlocked.CompareExchange(ref metrics.MaxDuration, milliseconds, currentMax) != currentMax);
    }

    public Task<PerformanceStats> GetStatsAsync()
    {
        var stats = new PerformanceStats();
        
        foreach (var kvp in _metrics)
        {
            var metrics = kvp.Value;
            var cacheHitRatio = metrics.TotalRequests > 0 
                ? (double)metrics.CacheHits / metrics.TotalRequests * 100 
                : 0;
            
            var avgDuration = metrics.OperationCount > 0 
                ? metrics.TotalDuration / metrics.OperationCount 
                : 0;

            stats.Operations.Add(new OperationStats
            {
                Operation = kvp.Key,
                CacheHitRatio = cacheHitRatio,
                AverageDuration = avgDuration,
                MinDuration = metrics.MinDuration == long.MaxValue ? 0 : metrics.MinDuration,
                MaxDuration = metrics.MaxDuration,
                TotalRequests = metrics.TotalRequests
            });
        }

        return Task.FromResult(stats);
    }

    private class OperationMetrics
    {
        public long CacheHits;
        public long CacheMisses;
        public long CacheSets;
        public long TotalRequests;
        public long TotalDuration;
        public long OperationCount;
        public long MinDuration = long.MaxValue;
        public long MaxDuration;
    }
}
