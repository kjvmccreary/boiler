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

    public bool UseDeadLetterOnGiveUp { get; set; } = true;

    public OutboxHealthThresholds Health { get; set; } = new();

    // NEW: Poison / failure classification tuning
    public OutboxPoisonOptions Poison { get; set; } = new();
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

// NEW
public class OutboxPoisonOptions
{
    // Optional early dead-letter threshold (if set < MaxRetries). 0 = disabled
    public int EarlyDeadLetterRetries { get; set; } = 0;

    // Regex patterns (substring match for simplicity) that mark message as non‑transient (dead-letter immediately)
    public string[] NonTransientErrorMarkers { get; set; } = new[]
    {
        "validation", "schema", "deserializ", "permission", "unauthorized"
    };

    // Patterns that should stay transient (never early dead-letter) – evaluated before non-transient
    public string[] AlwaysTransientMarkers { get; set; } = new[]
    {
        "timeout", "temporar", "rate limit", "connection", "unavailable"
    };

    // If true, any exception WITHOUT a marker but reaching MaxRetries becomes dead-letter (same as current)
    public bool DeadLetterOnMaxRetries { get; set; } = true;

    // If true, truncate errors strictly at MaxErrorTextLength, else soft (only if > length*2)
    public bool StrictErrorTruncation { get; set; } = true;
}
