using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using Microsoft.Extensions.Options;

namespace WorkflowService.Outbox;

public class OutboxDispatcher : IOutboxDispatcher
{
    private readonly WorkflowDbContext _db;
    private readonly IOutboxTransport _transport;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxDispatcher> _logger;

    public OutboxDispatcher(
        WorkflowDbContext db,
        IOutboxTransport transport,
        IOptions<OutboxOptions> opts,
        ILogger<OutboxDispatcher> logger)
    {
        _db = db;
        _transport = transport;
        _logger = logger;
        _options = opts.Value;
    }

    public async Task<(int processed, int failed)> ProcessBatchAsync(CancellationToken ct = default)
    {
        var pending = await _db.OutboxMessages
            .Where(m => m.ProcessedAt == null)
            .OrderBy(m => m.CreatedAt)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        if (pending.Count == 0)
        {
            _logger.LogDebug("OUTBOX_WORKER_FETCH count=0");
            return (0, 0);
        }

        _logger.LogInformation("OUTBOX_WORKER_FETCH count={Count}", pending.Count);

        int processed = 0, failed = 0;
        var now = DateTime.UtcNow;

        foreach (var msg in pending)
        {
            // Legacy guard if already processed concurrently
            if (msg.ProcessedAt != null)
            {
                _logger.LogDebug("OUTBOX_WORKER_SKIP_PROCESSED id={Id}", msg.Id);
                continue;
            }

            try
            {
                await _transport.DeliverAsync(msg, ct);

                msg.ProcessedAt = DateTime.UtcNow;
                msg.IsProcessed = true;              // legacy flag for backward compatibility
                msg.Error = null;
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

                // Do not set ProcessedAt; message remains eligible for future retry.
                await _db.SaveChangesAsync(ct);

                _logger.LogWarning(
                    "OUTBOX_WORKER_DISPATCH_FAIL id={Id} key={Key} retry={Retry} max={Max} error=\"{Err}\"",
                    msg.Id, msg.IdempotencyKey, msg.RetryCount, _options.MaxRetries, msg.Error);

                failed++;
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
