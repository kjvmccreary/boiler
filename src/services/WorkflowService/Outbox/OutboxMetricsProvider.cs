using Microsoft.EntityFrameworkCore;
using WorkflowService.Persistence;

namespace WorkflowService.Outbox;

public class OutboxMetricsProvider : IOutboxMetricsProvider
{
    private readonly IDbContextFactory<WorkflowDbContext> _dbFactory;
    private int _processedLast;
    private int _failedLast;
    private int _fetchedLast;
    private DateTime _lastCapture = DateTime.UtcNow;

    public OutboxMetricsProvider(IDbContextFactory<WorkflowDbContext> dbFactory)
    {
        _dbFactory = dbFactory;
    }

    public void RecordCycle(int fetched, int processed, int failed)
    {
        _processedLast = processed;
        _failedLast = failed;
        _fetchedLast = fetched;
        _lastCapture = DateTime.UtcNow;
    }

    public async Task<OutboxMetricsSnapshot> GetSnapshotAsync(CancellationToken ct = default)
    {
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var now = DateTime.UtcNow;

        var backlogQuery = db.OutboxMessages.Where(m => m.ProcessedAt == null);
        var backlogSize = await backlogQuery.CountAsync(ct);
        var failedPending = await backlogQuery.Where(m => m.RetryCount > 0).CountAsync(ct);

        var oldest = await backlogQuery
            .OrderBy(m => m.CreatedAt)
            .Select(m => m.CreatedAt)
            .FirstOrDefaultAsync(ct);

        double? oldestAge = oldest == default ? null : (now - oldest).TotalSeconds;

        return new OutboxMetricsSnapshot(
            now,
            backlogSize,
            failedPending,
            oldestAge,
            _processedLast,
            _failedLast,
            _fetchedLast);
    }
}
