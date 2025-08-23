using DTOs.Monitoring;

namespace Common.Monitoring;

/// <summary>
/// Interface for collecting and exposing performance metrics
/// Phase 11 Session 2 - Monitoring Infrastructure
/// </summary>
public interface IMetricsCollector
{
    // Permission & Authorization Metrics
    void RecordPermissionCheck(string permission, bool granted, TimeSpan duration, bool cacheHit = false, int tenantId = 0);
    void RecordRoleChange(string changeType, int roleId, int tenantId);
    void RecordAuthorizationPerformance(string operation, TimeSpan duration, int tenantId);
    
    // Caching Metrics
    void RecordCacheHit(string cacheKey, bool hit, TimeSpan? duration = null);
    void RecordCacheOperation(string operation, string key, TimeSpan duration, bool success);
    
    // Security Metrics
    void RecordSecurityEvent(string eventType, string severity, int tenantId = 0);
    void RecordFailedLogin(string reason, string ipAddress, int tenantId = 0);
    void RecordSuspiciousActivity(string activityType, string details, int tenantId = 0);
    
    // Request Metrics
    void RecordHttpRequest(string method, string path, int statusCode, TimeSpan duration, int tenantId = 0);
    void RecordApiCall(string endpoint, bool success, TimeSpan duration, int tenantId = 0);
    
    // System Metrics
    void RecordDatabaseQuery(string operation, TimeSpan duration, bool success);
    void RecordRedisOperation(string operation, TimeSpan duration, bool success);
    
    // Reporting
    Task<MetricsSummary> GetSummaryAsync(TimeSpan period);
    Task<MetricsSummary> GetSystemMetricsAsync(TimeSpan period); // 🔧 ADD: Missing method
    Task<Dictionary<string, object>> GetRealTimeMetricsAsync();
    Task<List<MetricTrend>> GetTrendsAsync(string metricName, TimeSpan period);
    
    // Health & Status
    Task<Common.Monitoring.HealthStatus> GetHealthStatusAsync(); // 🔧 FIX: Fully qualified type
    void RecordHealthCheck(string checkName, bool healthy, TimeSpan duration);
}

/// <summary>
/// Metrics summary for reporting
/// </summary>
public class MetricsSummary
{
    public TimeSpan Period { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    
    // Request Metrics
    public long TotalRequests { get; set; }
    public long TotalErrors { get; set; }
    public double ErrorRate { get; set; }
    public double AverageResponseTime { get; set; }
    public double P95ResponseTime { get; set; }
    public double RequestsPerSecond { get; set; }
    
    // Permission Metrics
    public long TotalPermissionChecks { get; set; }
    public long GrantedPermissions { get; set; }
    public long DeniedPermissions { get; set; }
    public double PermissionGrantRate { get; set; }
    public double AveragePermissionCheckTime { get; set; }
    public double PermissionCacheHitRate { get; set; } // 🔧 ADD: Missing property
    
    // Cache Metrics
    public long TotalCacheOperations { get; set; }
    public long CacheHits { get; set; }
    public long CacheMisses { get; set; }
    public double CacheHitRate { get; set; }
    public double AverageCacheResponseTime { get; set; }
    
    // Security Metrics
    public long TotalSecurityEvents { get; set; }
    public Dictionary<string, long> SecurityEventsByType { get; set; } = new();
    public Dictionary<string, long> SecurityEventsBySeverity { get; set; } = new();
    public long FailedLogins { get; set; }
    public long SuspiciousActivities { get; set; }
    
    // System Metrics
    public long DatabaseQueries { get; set; }
    public double AverageDatabaseQueryTime { get; set; }
    public long RedisOperations { get; set; }
    public double AverageRedisResponseTime { get; set; }
    
    // Tenant Metrics
    public Dictionary<int, TenantMetrics> TenantBreakdown { get; set; } = new();
    
    // Top Lists
    public List<string> TopDeniedPermissions { get; set; } = new();
    public List<string> SlowestEndpoints { get; set; } = new();
    public List<string> MostActiveUsers { get; set; } = new();
}

public class TenantMetrics
{
    public int TenantId { get; set; }
    public long Requests { get; set; }
    public long PermissionChecks { get; set; }
    public long SecurityEvents { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
}

public class MetricTrend
{
    public DateTime Timestamp { get; set; }
    public string MetricName { get; set; }
    public double Value { get; set; }
    public Dictionary<string, string> Labels { get; set; } = new();
}

public class HealthStatus
{
    public string Status { get; set; } = "Unknown"; // Healthy, Degraded, Unhealthy
    public DateTime Timestamp { get; set; }
    public Dictionary<string, HealthCheckResult> Checks { get; set; } = new();
    public TimeSpan TotalDuration { get; set; }
    
    public class HealthCheckResult
    {
        public bool IsHealthy { get; set; }
        public string Status { get; set; } = string.Empty;
        public TimeSpan Duration { get; set; }
        public string? Description { get; set; }
        public Dictionary<string, object> Data { get; set; } = new();
    }
}
