namespace WorkflowService.Outbox;

public class OutboxOptions
{
    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 50;
    public int MaxRetries { get; set; } = 5;
    public int BaseRetryDelaySeconds { get; set; } = 10;
    public bool UseExponentialBackoff { get; set; } = false;
    public int MaxErrorTextLength { get; set; } = 2000;
}
