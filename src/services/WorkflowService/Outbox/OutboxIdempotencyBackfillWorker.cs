using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;

namespace WorkflowService.Outbox;

/// <summary>
/// One-shot background worker that assigns IdempotencyKey for any legacy
/// OutboxMessages where the key is NULL or Guid.Empty (post-migration safety).
/// Safe to run concurrently (idempotent).
/// </summary>
public class OutboxIdempotencyBackfillWorker : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly ILogger<OutboxIdempotencyBackfillWorker> _logger;
    private readonly OutboxBackfillOptions _options;

    public OutboxIdempotencyBackfillWorker(
        IServiceProvider sp,
        IOptions<OutboxBackfillOptions> opts,
        ILogger<OutboxIdempotencyBackfillWorker> logger)
    {
        _sp = sp;
        _logger = logger;
        _options = opts.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("OUTBOX_BACKFILL_SKIP (disabled)");
            return;
        }

        _ = Task.Run(() => RunAsync(cancellationToken), cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private async Task RunAsync(CancellationToken ct)
    {
        try
        {
            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

            int total = 0;
            var batchSize = Math.Clamp(_options.BatchSize, 50, 5000);

            while (true)
            {
                ct.ThrowIfCancellationRequested();

                var batch = await db.OutboxMessages
                    .Where(m => m.IdempotencyKey == Guid.Empty)
                    .OrderBy(m => m.Id)
                    .Take(batchSize)
                    .ToListAsync(ct);

                if (batch.Count == 0)
                {
                    _logger.LogInformation("OUTBOX_BACKFILL_DONE totalUpdated={Total}", total);
                    break;
                }

                foreach (var msg in batch)
                {
                    msg.IdempotencyKey = Guid.NewGuid();
                }

                await db.SaveChangesAsync(ct);
                total += batch.Count;

                _logger.LogInformation("OUTBOX_BACKFILL_BATCH updated={Count} total={Total}", batch.Count, total);

                if (_options.MaxTotal > 0 && total >= _options.MaxTotal)
                {
                    _logger.LogWarning("OUTBOX_BACKFILL_STOP maxTotalReached={Max}", _options.MaxTotal);
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "OUTBOX_BACKFILL_ERROR");
        }
    }
}
