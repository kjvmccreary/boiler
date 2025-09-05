using Microsoft.EntityFrameworkCore;
using WorkflowService.Persistence;
using Microsoft.Extensions.Options;

namespace WorkflowService.Outbox;

public class OutboxDispatcher : IOutboxDispatcher
{
    private readonly WorkflowDbContext _db;
    private readonly IOutboxTransport _transport;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxDispatcher> _logger;
    private readonly IOutboxMetricsProvider? _metrics;

    public OutboxDispatcher(
        WorkflowDbContext db,
        IOutboxTransport transport,
        IOptions<OutboxOptions> opts,
        ILogger<OutboxDispatcher> logger,
        IOutboxMetricsProvider? metrics = null)
    {
        _db = db;
        _transport = transport;
        _logger = logger;
        _options = opts.Value;
        _metrics = metrics;
    }

    public async Task<(int processed, int failed)> ProcessBatchAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;

        var pending = await _db.OutboxMessages
            .Where(m => m.ProcessedAt == null &&
                        (m.NextRetryAt == null || m.NextRetryAt <= now))
            .OrderBy(m => m.CreatedAt)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        if (pending.Count == 0)
        {
            _logger.LogDebug("OUTBOX_WORKER_FETCH count=0");
            _metrics?.RecordCycle(0, 0, 0);
            return (0, 0);
        }

        _logger.LogInformation("OUTBOX_WORKER_FETCH count={Count}", pending.Count);

        int processed = 0, failed = 0;

        foreach (var msg in pending)
        {
            if (ct.IsCancellationRequested) break;

            if (msg.ProcessedAt != null)
            {
                _logger.LogDebug("OUTBOX_WORKER_SKIP_PROCESSED id={Id}", msg.Id);
                continue;
            }

            try
            {
                await _transport.DeliverAsync(msg, ct);

                msg.ProcessedAt = DateTime.UtcNow;
                msg.IsProcessed = true;
                msg.Error = null;
                msg.NextRetryAt = null;
                msg.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync(ct);

                _logger.LogInformation(
                    "OUTBOX_WORKER_DISPATCH_SUCCESS id={Id} key={Key} type={Type}",
                    msg.Id, msg.IdempotencyKey, msg.EventType);

                processed++;
            }
            catch (Exception ex)
            {
                msg.RetryCount += 1;
                msg.Error = TruncateError(ex.Message);
                msg.UpdatedAt = DateTime.UtcNow;

                var terminal = msg.RetryCount >= _options.MaxRetries;

                if (terminal)
                {
                    msg.ProcessedAt = DateTime.UtcNow;
                    msg.IsProcessed = true;
                    msg.NextRetryAt = null;
                    _logger.LogWarning(
                        "OUTBOX_WORKER_GIVEUP id={Id} key={Key} retry={Retry} max={Max} error=\"{Err}\"",
                        msg.Id, msg.IdempotencyKey, msg.RetryCount, _options.MaxRetries, msg.Error);
                }
                else
                {
                    var delay = OutboxRetryPolicy.ComputeDelay(msg.RetryCount, _options);
                    msg.NextRetryAt = DateTime.UtcNow + delay;
                    _logger.LogWarning(
                        "OUTBOX_WORKER_DISPATCH_FAIL id={Id} key={Key} retry={Retry} max={Max} nextRetryAt={Next:o} delaySeconds={Delay} error=\"{Err}\"",
                        msg.Id, msg.IdempotencyKey, msg.RetryCount, _options.MaxRetries,
                        msg.NextRetryAt, (int)delay.TotalSeconds, msg.Error);
                }

                await _db.SaveChangesAsync(ct);
                failed++;
            }
        }

        // Metrics
        if (_options.EnableMetrics)
        {
            try
            {
                _metrics?.RecordCycle(pending.Count, processed, failed);

                // Optional summary
                var backlog = await _db.OutboxMessages.CountAsync(m => m.ProcessedAt == null, ct);
                var oldestCreated = await _db.OutboxMessages
                    .Where(m => m.ProcessedAt == null)
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => m.CreatedAt)
                    .FirstOrDefaultAsync(ct);

                double oldestAgeSeconds = oldestCreated == default
                    ? 0
                    : (DateTime.UtcNow - oldestCreated).TotalSeconds;

                _logger.LogInformation(
                    "OUTBOX_WORKER_CYCLE fetched={Fetched} processed={Processed} failed={Failed} backlog={Backlog} oldestAgeSeconds={OldestAge}",
                    pending.Count, processed, failed, backlog, (int)oldestAgeSeconds);
            }
            catch (Exception e)
            {
                _logger.LogDebug(e, "OUTBOX_WORKER_METRICS_ERROR");
            }
        }

        return (processed, failed);
    }

    private string? TruncateError(string? err)
    {
        if (err == null) return null;
        if (err.Length <= _options.MaxErrorTextLength) return err;
        return err.Substring(0, _options.MaxErrorTextLength);
    }
}
