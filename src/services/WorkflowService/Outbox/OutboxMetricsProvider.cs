using Microsoft.EntityFrameworkCore;
using WorkflowService.Persistence;
using Microsoft.Extensions.Options;

namespace WorkflowService.Outbox;

public class OutboxMetricsProvider : IOutboxMetricsProvider
{
    private readonly IDbContextFactory<WorkflowDbContext> _dbFactory;
    private readonly OutboxOptions _options;

    private long _processedTotal;
    private long _failedTotal;
    private long _giveUpTotal;
    private long _deadLetterTotal;

    private int _fetchedLast;
    private int _processedLast;
    private int _failedLast;
    private int _giveUpLast;
    private int _deadLetterLast;
    private DateTime _lastCycleAt = DateTime.MinValue;

    private readonly RollingWindowAggregator _window;

    public OutboxMetricsProvider(
        IDbContextFactory<WorkflowDbContext> dbFactory,
        IOptions<OutboxOptions> options)
    {
        _dbFactory = dbFactory;
        _options = options.Value;
        _window = new RollingWindowAggregator(TimeSpan.FromMinutes(
            Math.Clamp(_options.RollingWindowMinutes <= 0 ? 5 : _options.RollingWindowMinutes, 1, 120)));
    }

    public void RecordCycle(int fetched, int processed, int failed, int giveUp = 0, int deadLetter = 0)
    {
        _fetchedLast = fetched;
        _processedLast = processed;
        _failedLast = failed;
        _giveUpLast = giveUp;
        _deadLetterLast = deadLetter;

        _processedTotal += processed;
        _failedTotal += failed;
        _giveUpTotal += giveUp;
        _deadLetterTotal += deadLetter;

        _lastCycleAt = DateTime.UtcNow;
        // For window we treat failures as (failed + deadLetter) and giveUp aggregated with deadLetter just for a generic failure curve
        _window.Add(_lastCycleAt, fetched, processed, failed + deadLetter, giveUp + deadLetter);
    }

    public async Task<OutboxMetricsSnapshot> GetSnapshotAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        await using var db = await _dbFactory.CreateDbContextAsync(ct);

        var backlogQuery = db.OutboxMessages.Where(m => m.ProcessedAt == null);
        var backlogSize = await backlogQuery.CountAsync(ct);
        var failedPending = await backlogQuery.Where(m => m.RetryCount > 0 && !m.DeadLetter).CountAsync(ct);
        var deadLetterUnprocessed = await backlogQuery.Where(m => m.DeadLetter).CountAsync(ct);

        var oldest = await backlogQuery
            .OrderBy(m => m.CreatedAt)
            .Select(m => m.CreatedAt)
            .FirstOrDefaultAsync(ct);

        double? oldestAge = oldest == default ? null : (now - oldest).TotalSeconds;

        double failureRatioLast = _fetchedLast == 0 ? 0 : (double)_failedLast / _fetchedLast;

        var (pWin, fWin, gWin, fetchedWin, minutesWin) = _window.Snapshot();
        double throughputWin = minutesWin <= 0 ? 0 : pWin / minutesWin;
        double failureRatioWin = fetchedWin == 0 ? 0 : (double)fWin / fetchedWin;

        return new OutboxMetricsSnapshot(
            now,
            backlogSize,
            failedPending,
            deadLetterUnprocessed,
            oldestAge,
            _fetchedLast,
            _processedLast,
            _failedLast,
            _giveUpLast,
            _deadLetterLast,
            _processedTotal,
            _failedTotal,
            _giveUpTotal,
            _deadLetterTotal,
            failureRatioLast,
            throughputWin,
            failureRatioWin);
    }
}
