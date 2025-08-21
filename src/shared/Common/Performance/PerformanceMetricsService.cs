using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Common.Performance
{
    /// <summary>
    /// In-memory performance metrics collection service
    /// </summary>
    public class PerformanceMetricsService : IPerformanceMetricsService
    {
        private readonly ILogger<PerformanceMetricsService> _logger;
        private readonly IConnectionMultiplexer? _redis;
        
        // Thread-safe collections for metrics
        private readonly ConcurrentBag<CacheMetricEntry> _cacheMetrics = new();
        private readonly ConcurrentBag<ApiMetricEntry> _apiMetrics = new();
        private readonly DateTime _startTime = DateTime.UtcNow;

        public PerformanceMetricsService(
            ILogger<PerformanceMetricsService> logger,
            IConnectionMultiplexer? redis = null)
        {
            _logger = logger;
            _redis = redis;
        }

        public async Task RecordCacheHitAsync(string cacheKey, string cacheType, TimeSpan retrievalTime)
        {
            _cacheMetrics.Add(new CacheMetricEntry
            {
                CacheKey = cacheKey,
                CacheType = cacheType,
                IsHit = true,
                ResponseTime = retrievalTime,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogDebug("Cache HIT: {CacheType}/{CacheKey} in {ResponseTime}ms", 
                cacheType, cacheKey, retrievalTime.TotalMilliseconds);
            
            await Task.CompletedTask;
        }

        public async Task RecordCacheMissAsync(string cacheKey, string cacheType, TimeSpan databaseTime)
        {
            _cacheMetrics.Add(new CacheMetricEntry
            {
                CacheKey = cacheKey,
                CacheType = cacheType,
                IsHit = false,
                ResponseTime = databaseTime,
                Timestamp = DateTime.UtcNow
            });

            _logger.LogDebug("Cache MISS: {CacheType}/{CacheKey} in {ResponseTime}ms", 
                cacheType, cacheKey, databaseTime.TotalMilliseconds);
            
            await Task.CompletedTask;
        }

        public async Task RecordCacheSetAsync(string cacheKey, string cacheType, TimeSpan setTime)
        {
            _logger.LogDebug("Cache SET: {CacheType}/{CacheKey} in {SetTime}ms", 
                cacheType, cacheKey, setTime.TotalMilliseconds);
            
            await Task.CompletedTask;
        }

        public async Task<CacheMetrics> GetCacheMetricsAsync(TimeSpan? period = null)
        {
            var cutoff = period.HasValue ? DateTime.UtcNow - period.Value : _startTime;
            var relevantMetrics = _cacheMetrics.Where(m => m.Timestamp >= cutoff).ToList();

            var metrics = new CacheMetrics
            {
                TotalRequests = relevantMetrics.Count,
                CacheHits = relevantMetrics.Count(m => m.IsHit),
                CacheMisses = relevantMetrics.Count(m => !m.IsHit),
                PeriodStart = cutoff,
                PeriodEnd = DateTime.UtcNow
            };

            if (relevantMetrics.Any())
            {
                var hits = relevantMetrics.Where(m => m.IsHit);
                var misses = relevantMetrics.Where(m => !m.IsHit);

                metrics.AverageHitTime = hits.Any() 
                    ? TimeSpan.FromTicks((long)hits.Average(h => h.ResponseTime.Ticks))
                    : TimeSpan.Zero;

                metrics.AverageMissTime = misses.Any()
                    ? TimeSpan.FromTicks((long)misses.Average(m => m.ResponseTime.Ticks))
                    : TimeSpan.Zero;

                // Group by cache type
                foreach (var typeGroup in relevantMetrics.GroupBy(m => m.CacheType))
                {
                    var typeMetrics = typeGroup.ToList();
                    var typeHits = typeMetrics.Where(m => m.IsHit);
                    var typeMisses = typeMetrics.Where(m => !m.IsHit);

                    metrics.ByType[typeGroup.Key] = new CacheTypeMetrics
                    {
                        Hits = typeHits.Count(),
                        Misses = typeMisses.Count(),
                        AverageHitTime = typeHits.Any()
                            ? TimeSpan.FromTicks((long)typeHits.Average(h => h.ResponseTime.Ticks))
                            : TimeSpan.Zero,
                        AverageMissTime = typeMisses.Any()
                            ? TimeSpan.FromTicks((long)typeMisses.Average(m => m.ResponseTime.Ticks))
                            : TimeSpan.Zero
                    };
                }
            }

            return await Task.FromResult(metrics);
        }

        public async Task RecordApiCallAsync(string endpoint, string method, TimeSpan responseTime, int statusCode)
        {
            _apiMetrics.Add(new ApiMetricEntry
            {
                Endpoint = endpoint,
                Method = method,
                ResponseTime = responseTime,
                StatusCode = statusCode,
                Timestamp = DateTime.UtcNow
            });

            await Task.CompletedTask;
        }

        public async Task<ApiMetrics> GetApiMetricsAsync(TimeSpan? period = null)
        {
            var cutoff = period.HasValue ? DateTime.UtcNow - period.Value : _startTime;
            var relevantMetrics = _apiMetrics.Where(m => m.Timestamp >= cutoff).ToList();

            var metrics = new ApiMetrics
            {
                TotalRequests = relevantMetrics.Count,
                PeriodStart = cutoff,
                PeriodEnd = DateTime.UtcNow
            };

            if (relevantMetrics.Any())
            {
                metrics.AverageResponseTime = TimeSpan.FromTicks(
                    (long)relevantMetrics.Average(m => m.ResponseTime.Ticks));

                // Group by endpoint
                foreach (var endpointGroup in relevantMetrics.GroupBy(m => $"{m.Method} {m.Endpoint}"))
                {
                    var endpointMetrics = endpointGroup.ToList();
                    metrics.ByEndpoint[endpointGroup.Key] = new EndpointMetrics
                    {
                        RequestCount = endpointMetrics.Count,
                        AverageResponseTime = TimeSpan.FromTicks(
                            (long)endpointMetrics.Average(m => m.ResponseTime.Ticks)),
                        MinResponseTime = TimeSpan.FromTicks(endpointMetrics.Min(m => m.ResponseTime.Ticks)),
                        MaxResponseTime = TimeSpan.FromTicks(endpointMetrics.Max(m => m.ResponseTime.Ticks))
                    };
                }

                // Group by status code
                foreach (var statusGroup in relevantMetrics.GroupBy(m => m.StatusCode))
                {
                    metrics.StatusCodes[statusGroup.Key] = statusGroup.Count();
                }
            }

            return await Task.FromResult(metrics);
        }

        public async Task<SystemMetrics> GetSystemMetricsAsync()
        {
            var metrics = new SystemMetrics
            {
                Uptime = DateTime.UtcNow - _startTime,
                RedisConnected = _redis?.IsConnected ?? false
            };

            if (_redis?.IsConnected == true)
            {
                try
                {
                    var server = _redis.GetServer(_redis.GetEndPoints().First());
                    var info = await server.InfoAsync();
                    
                    // Flatten the info groupings into a single sequence of key-value pairs
                    var flatInfo = info.SelectMany(g => g);

                    // Now you can search for the key as expected
                    metrics.RedisVersion = flatInfo.FirstOrDefault(i => i.Key == "redis_version").Value ?? "Unknown";
                    
                    var memoryInfo = flatInfo.FirstOrDefault(i => i.Key == "used_memory");
                    if (long.TryParse(memoryInfo.Value, out var memory))
                        metrics.RedisUsedMemory = memory;

                    var clientsInfo = flatInfo.FirstOrDefault(i => i.Key == "connected_clients");
                    if (int.TryParse(clientsInfo.Value, out var clients))
                        metrics.RedisConnectedClients = clients;

                    // Count total keys (approximate)
                    try
                    {
                        var keys = server.Keys(pattern: "*");
                        metrics.TotalCacheKeys = keys.Count();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogWarning(ex, "Could not count Redis keys");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not retrieve Redis system info");
                }
            }

            return metrics;
        }

        public async Task ClearMetricsAsync()
        {
            _cacheMetrics.Clear();
            _apiMetrics.Clear();
            _logger.LogInformation("Performance metrics cleared");
            await Task.CompletedTask;
        }

        // Internal metric entry classes
        private class CacheMetricEntry
        {
            public string CacheKey { get; set; } = string.Empty;
            public string CacheType { get; set; } = string.Empty;
            public bool IsHit { get; set; }
            public TimeSpan ResponseTime { get; set; }
            public DateTime Timestamp { get; set; }
        }

        private class ApiMetricEntry
        {
            public string Endpoint { get; set; } = string.Empty;
            public string Method { get; set; } = string.Empty;
            public TimeSpan ResponseTime { get; set; }
            public int StatusCode { get; set; }
            public DateTime Timestamp { get; set; }
        }
    }
}
