namespace Common.Performance;

public class ApiMetrics
{
    public int TotalRequests { get; set; }
    public long TotalErrors { get; set; }
    public TimeSpan AverageResponseTime { get; set; }
    public double ErrorRate => TotalRequests > 0 
        ? (double)TotalErrors / TotalRequests * 100 
        : 0;
    
    // ✅ FIXED: Property name expected by controller
    public Dictionary<string, EndpointMetrics> ByEndpoint { get; set; } = new();
    
    // ✅ Keep legacy property for backward compatibility
    public Dictionary<string, EndpointMetrics> EndpointMetrics => ByEndpoint;
    
    public Dictionary<int, long> StatusCodes { get; set; } = new();
}

public class EndpointMetrics
{
    public string Endpoint { get; set; } = string.Empty;
    public int RequestCount { get; set; }
    public long ErrorCount { get; set; }
    
    // ✅ FIXED: Change to TimeSpan as expected by controller
    public TimeSpan AverageResponseTime { get; set; }
    public TimeSpan MinResponseTime { get; set; }
    public TimeSpan MaxResponseTime { get; set; }
}
