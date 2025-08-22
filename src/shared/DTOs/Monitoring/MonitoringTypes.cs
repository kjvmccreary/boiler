namespace DTOs.Monitoring;

/// <summary>
/// Metrics for individual HTTP requests
/// </summary>
public class RequestMetrics
{
    public string Path { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public double ResponseTimeMs { get; set; }
    public int TenantId { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Metrics for permission check operations
/// </summary>
public class PermissionCheckMetrics
{
    public string Permission { get; set; } = string.Empty;
    public bool Granted { get; set; }
    public double CheckDurationMs { get; set; }
    public bool CacheHit { get; set; }
    public int TenantId { get; set; }
    public DateTime Timestamp { get; set; }
}

/// <summary>
/// Aggregated system metrics for a time period
/// </summary>
public class SystemMetrics
{
    public TimeSpan Period { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int TotalRequests { get; set; }
    public int TotalErrors { get; set; }
    public double AverageResponseTime { get; set; }
    public double ErrorRate { get; set; }
    public int TotalPermissionChecks { get; set; }
    public double PermissionCacheHitRate { get; set; }
    public double PermissionDenialRate { get; set; }
    public Dictionary<string, object> SecurityEvents { get; set; } = new();
}

/// <summary>
/// Performance alert when thresholds are exceeded
/// </summary>
public class PerformanceAlert
{
    public string Id { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public double Value { get; set; }
    public double Threshold { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public bool IsResolved => ResolvedAt.HasValue;
    public Dictionary<string, object> Details { get; set; } = new();
}

/// <summary>
/// Internal metrics for aggregation (not exposed in interface)
/// </summary>
public class HourlyRequestMetrics
{
    public int TotalRequests { get; set; }
    public int ErrorCount { get; set; }
    public int SlowRequestCount { get; set; }
    public double TotalResponseTime { get; set; }
    public double AverageResponseTime { get; set; }
}

/// <summary>
/// Internal permission metrics for aggregation (not exposed in interface)
/// </summary>
public class HourlyPermissionMetrics
{
    public int TotalChecks { get; set; }
    public int CacheHits { get; set; }
    public int DeniedChecks { get; set; }
    public double TotalCheckTime { get; set; }
    public double AverageCheckTime { get; set; }
    public double CacheHitRate { get; set; }
}

/// <summary>
/// Internal security metrics for aggregation (not exposed in interface)
/// </summary>
public class HourlySecurityMetrics
{
    public int TotalEvents { get; set; }
    public int LowSeverityEvents { get; set; }
    public int MediumSeverityEvents { get; set; }
    public int HighSeverityEvents { get; set; }
    public int CriticalSeverityEvents { get; set; }
    public Dictionary<string, int> EventTypes { get; set; } = new();
}
