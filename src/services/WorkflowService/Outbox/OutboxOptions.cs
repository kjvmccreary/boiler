namespace WorkflowService.Outbox;

public class OutboxOptions
{
    public int PollIntervalSeconds { get; set; } = 5;
    public int BatchSize { get; set; } = 50;
    public int MaxRetries { get; set; } = 5;
    public int BaseRetryDelaySeconds { get; set; } = 10;
    public bool UseExponentialBackoff { get; set; } = false;
    public double JitterRatio { get; set; } = 0.20;
    public int MaxErrorTextLength { get; set; } = 2000;

    public bool EnableMetrics { get; set; } = true;
    public bool EnablePrometheus { get; set; } = true;
    public int RollingWindowMinutes { get; set; } = 5;

    // NEW: Emit terminal failures as DeadLetter instead of “give up” normal processed.
    public bool UseDeadLetterOnGiveUp { get; set; } = true;

    public OutboxHealthThresholds Health { get; set; } = new();
}

public class OutboxHealthThresholds
{
    public int BacklogWarn { get; set; } = 500;
    public int BacklogUnhealthy { get; set; } = 2000;
    public double FailedPendingRatioWarn { get; set; } = 0.20;
    public double FailedPendingRatioUnhealthy { get; set; } = 0.50;
    public int OldestAgeWarnSeconds { get; set; } = 300;
    public int OldestAgeUnhealthySeconds { get; set; } = 900;
}
