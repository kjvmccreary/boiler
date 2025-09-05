namespace WorkflowService.Outbox;

public interface IOutboxMetricsProvider
{
    /// <summary>
    /// Records a dispatcher cycle. Any of giveUp or deadLetter can be omitted (defaults = 0).
    /// </summary>
    void RecordCycle(int fetched, int processed, int failed, int giveUp = 0, int deadLetter = 0);

    Task<OutboxMetricsSnapshot> GetSnapshotAsync(CancellationToken ct = default);
}
