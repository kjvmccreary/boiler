using Microsoft.EntityFrameworkCore;
using WorkflowService.Persistence;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace WorkflowService.Outbox;

internal static class OutboxActivity
{
    public static readonly ActivitySource Source = new("WorkflowService.Outbox");
}

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
        using var batchActivity = OutboxActivity.Source.StartActivity("OutboxDispatchBatch");

        var now = DateTime.UtcNow;
        var pending = await _db.OutboxMessages
            .Where(m => m.ProcessedAt == null &&
                        (m.NextRetryAt == null || m.NextRetryAt <= now) &&
                        !m.DeadLetter)  // do not re-pick dead letters
            .OrderBy(m => m.CreatedAt)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        if (pending.Count == 0)
        {
            _logger.LogDebug("OUTBOX_WORKER_FETCH count=0");
            _metrics?.RecordCycle(0, 0, 0, 0, 0);
            return (0, 0);
        }

        _logger.LogInformation("OUTBOX_WORKER_FETCH count={Count}", pending.Count);

        int processed = 0, failed = 0, giveUp = 0, deadLetter = 0;

        foreach (var msg in pending)
        {
            if (ct.IsCancellationRequested) break;

            using var msgActivity = OutboxActivity.Source.StartActivity("OutboxDispatchMessage");
            msgActivity?.AddTag("outbox.id", msg.Id);
            msgActivity?.AddTag("outbox.event_type", msg.EventType);
            msgActivity?.AddTag("outbox.retry", msg.RetryCount);

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

                msgActivity?.AddTag("outbox.status", "success");

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
                    if (_options.UseDeadLetterOnGiveUp)
                    {
                        msg.DeadLetter = true;
                        msg.ProcessedAt = DateTime.UtcNow; // mark terminal timestamp
                        msg.IsProcessed = true;
                        msg.NextRetryAt = null;
                        deadLetter++;
                        _logger.LogWarning(
                            "OUTBOX_WORKER_DEADLETTER id={Id} key={Key} retry={Retry} max={Max} error=\"{Err}\"",
                            msg.Id, msg.IdempotencyKey, msg.RetryCount, _options.MaxRetries, msg.Error);
                        msgActivity?.AddTag("outbox.status", "deadletter");
                    }
                    else
                    {
                        msg.ProcessedAt = DateTime.UtcNow;
                        msg.IsProcessed = true;
                        msg.NextRetryAt = null;
                        giveUp++;
                        _logger.LogWarning(
                            "OUTBOX_WORKER_GIVEUP id={Id} key={Key} retry={Retry} max={Max} error=\"{Err}\"",
                            msg.Id, msg.IdempotencyKey, msg.RetryCount, _options.MaxRetries, msg.Error);
                        msgActivity?.AddTag("outbox.status", "giveup");
                    }
                }
                else
                {
                    var delay = OutboxRetryPolicy.ComputeDelay(msg.RetryCount, _options);
                    msg.NextRetryAt = DateTime.UtcNow + delay;
                    _logger.LogWarning(
                        "OUTBOX_WORKER_DISPATCH_FAIL id={Id} key={Key} retry={Retry} max={Max} nextRetryAt={Next:o} delaySeconds={Delay} error=\"{Err}\"",
                        msg.Id, msg.IdempotencyKey, msg.RetryCount, _options.MaxRetries,
                        msg.NextRetryAt, (int)delay.TotalSeconds, msg.Error);
                    msgActivity?.AddTag("outbox.status", "retry");
                    failed++;
                }

                await _db.SaveChangesAsync(ct);
            }
        }

        if (_options.EnableMetrics)
        {
            try
            {
                _metrics?.RecordCycle(pending.Count, processed, failed, giveUp, deadLetter);
                var backlog = await _db.OutboxMessages.CountAsync(m => m.ProcessedAt == null && !m.DeadLetter, ct);
                var oldestCreated = await _db.OutboxMessages
                    .Where(m => m.ProcessedAt == null && !m.DeadLetter)
                    .OrderBy(m => m.CreatedAt)
                    .Select(m => m.CreatedAt)
                    .FirstOrDefaultAsync(ct);

                double oldestAgeSeconds = oldestCreated == default
                    ? 0
                    : (DateTime.UtcNow - oldestCreated).TotalSeconds;

                _logger.LogInformation(
                    "OUTBOX_WORKER_CYCLE fetched={Fetched} processed={Processed} failed={Failed} giveup={GiveUp} deadletter={Dead} backlog={Backlog} oldestAgeSeconds={OldestAge}",
                    pending.Count, processed, failed, giveUp, deadLetter, backlog, (int)oldestAgeSeconds);
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
