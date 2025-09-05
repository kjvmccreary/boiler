namespace WorkflowService.Outbox;

public class OutboxOptions
{
    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 50;
    public int MaxRetries { get; set; } = 5;
    public int BaseRetryDelaySeconds { get; set; } = 10;
    public bool UseExponentialBackoff { get; set; } = false;
    public double JitterRatio { get; set; } = 0.20; // 20% jitter
    public int MaxErrorTextLength { get; set; } = 2000;
    public bool EnableMetrics { get; set; } = true;
}
