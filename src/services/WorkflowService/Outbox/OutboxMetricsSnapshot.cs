namespace WorkflowService.Outbox;

public sealed record OutboxMetricsSnapshot(
    DateTime CapturedAtUtc,
    int BacklogSize,
    int FailedPending,
    int DeadLetterUnprocessed,          // NEW (DeadLetter && ProcessedAt NULL)
    double? OldestAgeSeconds,
    int FetchedLastCycle,
    int ProcessedLastCycle,
    int FailedLastCycle,
    int GiveUpLastCycle,
    int DeadLetterLastCycle,            // NEW
    long ProcessedTotal,
    long FailedTotal,
    long GiveUpTotal,
    long DeadLetterTotal,               // NEW
    double FailureRatioLastCycle,
    double ThroughputPerMinuteWindow,
    double FailureRatioWindow);
