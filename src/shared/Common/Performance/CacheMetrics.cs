namespace Common.Performance;

public class CacheMetrics
{
    public long TotalHits { get; set; }
    public long TotalMisses { get; set; }
    public long TotalSets { get; set; }
    public int TotalRequests => (int)(TotalHits + TotalMisses);
    public double HitRatio => TotalHits + TotalMisses > 0 
        ? (double)TotalHits / (TotalHits + TotalMisses) * 100 
        : 0;
    public double AverageGetTime { get; set; }
    public double AverageSetTime { get; set; }
    
    // ✅ ADD: Properties expected by PerformanceController
    public TimeSpan AverageHitTime { get; set; }
    public TimeSpan AverageMissTime { get; set; }
    
    public long TotalOperations => TotalHits + TotalMisses + TotalSets;
    public Dictionary<string, long> OperationsByType { get; set; } = new();
}
