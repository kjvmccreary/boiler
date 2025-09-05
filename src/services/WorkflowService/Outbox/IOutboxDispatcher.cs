namespace WorkflowService.Outbox;

public interface IOutboxDispatcher
{
    /// <summary>
    /// Processes a single batch (non-looping) of outbox messages.
    /// Returns (processedCount, failedCount).
    /// </summary>
    Task<(int processed, int failed)> ProcessBatchAsync(CancellationToken ct = default);
}
