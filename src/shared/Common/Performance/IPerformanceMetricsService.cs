using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Performance
{
    /// <summary>
    /// Service for collecting and retrieving performance metrics
    /// </summary>
    public interface IPerformanceMetricsService
    {
        // Cache Metrics
        Task RecordCacheHitAsync(string cacheKey, string cacheType, TimeSpan retrievalTime);
        Task RecordCacheMissAsync(string cacheKey, string cacheType, TimeSpan databaseTime);
        Task RecordCacheSetAsync(string cacheKey, string cacheType, TimeSpan setTime);
        Task<CacheMetrics> GetCacheMetricsAsync(TimeSpan? period = null);
        
        // Performance Metrics
        Task RecordApiCallAsync(string endpoint, string method, TimeSpan responseTime, int statusCode);
        Task<ApiMetrics> GetApiMetricsAsync(TimeSpan? period = null);
        
        // System Metrics
        Task<SystemMetrics> GetSystemMetricsAsync();
        
        // Reset/Clear
        Task ClearMetricsAsync();
    }

    public class CacheMetrics
    {
        public int TotalRequests { get; set; }
        public int CacheHits { get; set; }
        public int CacheMisses { get; set; }
        public double HitRatio => TotalRequests > 0 ? (double)CacheHits / TotalRequests * 100 : 0;
        public TimeSpan AverageHitTime { get; set; }
        public TimeSpan AverageMissTime { get; set; }
        public Dictionary<string, CacheTypeMetrics> ByType { get; set; } = new();
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class CacheTypeMetrics
    {
        public int Hits { get; set; }
        public int Misses { get; set; }
        public double HitRatio => (Hits + Misses) > 0 ? (double)Hits / (Hits + Misses) * 100 : 0;
        public TimeSpan AverageHitTime { get; set; }
        public TimeSpan AverageMissTime { get; set; }
    }

    public class ApiMetrics
    {
        public int TotalRequests { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public Dictionary<string, EndpointMetrics> ByEndpoint { get; set; } = new();
        public Dictionary<int, int> StatusCodes { get; set; } = new();
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }

    public class EndpointMetrics
    {
        public int RequestCount { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan MinResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
    }

    public class SystemMetrics
    {
        public bool RedisConnected { get; set; }
        public string RedisVersion { get; set; } = string.Empty;
        public long RedisUsedMemory { get; set; }
        public int RedisConnectedClients { get; set; }
        public int TotalCacheKeys { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public TimeSpan Uptime { get; set; }
    }
}
