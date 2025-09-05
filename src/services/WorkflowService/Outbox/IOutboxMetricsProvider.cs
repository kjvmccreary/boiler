namespace WorkflowService.Outbox;

public interface IOutboxMetricsProvider
{
    void RecordCycle(int fetched, int processed, int failed);
    Task<OutboxMetricsSnapshot> GetSnapshotAsync(CancellationToken ct = default);
}
