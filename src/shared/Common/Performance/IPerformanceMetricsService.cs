using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Performance;

public interface IPerformanceMetricsService
{
    // ✅ Basic sync methods
    void RecordCacheHit(string operation);
    void RecordCacheMiss(string operation);
    void RecordCacheSet(string operation);
    void RecordOperationDuration(string operation, long milliseconds);
    
    // ✅ Detailed cache operation recording
    void RecordCacheHit(string key, string dataType, TimeSpan duration);
    void RecordCacheMiss(string key, string dataType, TimeSpan duration);
    void RecordCacheSet(string key, string dataType, TimeSpan duration);
    
    // ✅ FIXED: Methods that match controller expectations (with optional TimeSpan parameter)
    Task<CacheMetrics> GetCacheMetricsAsync(TimeSpan? period = null);
    Task<ApiMetrics> GetApiMetricsAsync(TimeSpan? period = null);
    Task<SystemMetrics> GetSystemMetricsAsync();
    
    // ✅ Legacy sync methods (for backward compatibility)
    CacheMetrics GetCacheMetrics();
    ApiMetrics GetApiMetrics();
    SystemMetrics GetSystemMetrics();
    
    // ✅ Clear metrics method
    Task ClearMetricsAsync();
    
    Task<PerformanceStats> GetStatsAsync();
}

public class PerformanceStats
{
    public List<OperationStats> Operations { get; set; } = new();
}

public class OperationStats
{
    public string Operation { get; set; } = string.Empty;
    public double CacheHitRatio { get; set; }
    public long AverageDuration { get; set; }
    public long MinDuration { get; set; }
    public long MaxDuration { get; set; }
    public long TotalRequests { get; set; }
}
