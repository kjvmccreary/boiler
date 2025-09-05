namespace WorkflowService.Outbox;

public sealed record OutboxMetricsSnapshot(
    DateTime CapturedAtUtc,
    int BacklogSize,
    int FailedPending,
    double? OldestAgeSeconds,
    int ProcessedLastCycle,
    int FailedLastCycle,
    int FetchedLastCycle);
