namespace Common.Performance;

public class SystemMetrics
{
    public double CpuUsage { get; set; }
    public long MemoryUsed { get; set; }
    public long MemoryTotal { get; set; }
    public double MemoryUsagePercentage => MemoryTotal > 0 
        ? (double)MemoryUsed / MemoryTotal * 100 
        : 0;
    public long DiskUsed { get; set; }
    public long DiskTotal { get; set; }
    public double DiskUsagePercentage => DiskTotal > 0 
        ? (double)DiskUsed / DiskTotal * 100 
        : 0;
    public int ActiveConnections { get; set; }
    public TimeSpan Uptime { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    
    // ✅ ADD: Property expected by PerformanceController
    public bool RedisConnected { get; set; }
}
