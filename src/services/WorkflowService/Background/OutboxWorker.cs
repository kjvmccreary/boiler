using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;

namespace WorkflowService.Background;

/// <summary>
/// Periodically scans OutboxMessages and dispatches them via IOutboxDispatcher.
/// Simple retry policy (linear/exponential can be added later).
/// </summary>
public class OutboxWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxWorker> _logger;
    private readonly TimeSpan _interval;
    private readonly int _batchSize;
    private readonly int _maxRetries;

    public OutboxWorker(
        IServiceProvider serviceProvider,
        IConfiguration configuration,
        ILogger<OutboxWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var cfg = configuration.GetSection("WorkflowSettings:Outbox");
        _interval = TimeSpan.FromSeconds(Math.Max(5, cfg.GetValue<int?>("IntervalSeconds") ?? 15));
        _batchSize = Math.Clamp(cfg.GetValue<int?>("BatchSize") ?? 50, 1, 500);
        _maxRetries = Math.Clamp(cfg.GetValue<int?>("MaxRetries") ?? 5, 1, 20);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OUTBOX_WORKER_START Interval={Interval}s BatchSize={Batch} MaxRetries={MaxRetries}",
            _interval.TotalSeconds, _batchSize, _maxRetries);

        while (!stoppingToken.IsCancellationRequested)
        {
            var started = DateTime.UtcNow;
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OUTBOX_WORKER_LOOP_ERROR");
            }

            var elapsed = DateTime.UtcNow - started;
            var delay = _interval - elapsed;
            if (delay < TimeSpan.FromSeconds(1))
                delay = TimeSpan.FromSeconds(1);

            try { await Task.Delay(delay, stoppingToken); }
            catch (TaskCanceledException) { }
        }

        _logger.LogInformation("OUTBOX_WORKER_STOP");
    }

    private async Task ProcessBatchAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var dispatcher = scope.ServiceProvider.GetRequiredService<IOutboxDispatcher>();

        // Fetch unprocessed messages
        var messages = await db.OutboxMessages
            .Where(m => !m.IsProcessed)
            .OrderBy(m => m.CreatedAt)
            .Take(_batchSize)
            .ToListAsync(ct);

        if (messages.Count == 0)
        {
            _logger.LogDebug("OUTBOX_EMPTY");
            return;
        }

        _logger.LogInformation("OUTBOX_BATCH Size={Count}", messages.Count);

        foreach (var msg in messages)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                var ok = await dispatcher.DispatchAsync(msg, ct);
                if (ok)
                {
                    msg.IsProcessed = true;
                    msg.ProcessedAt = DateTime.UtcNow;
                    msg.UpdatedAt = DateTime.UtcNow;
                }
                else
                {
                    HandleRetry(msg, transient: true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OUTBOX_DISPATCH_ERROR Id={Id} Retry={Retry}", msg.Id, msg.RetryCount);
                HandleRetry(msg, transient: true, ex.Message);
            }
        }

        await db.SaveChangesAsync(ct);
    }

    private void HandleRetry(OutboxMessage msg, bool transient, string? error = null)
    {
        msg.RetryCount++;
        msg.LastError = error;
        msg.UpdatedAt = DateTime.UtcNow;

        if (msg.RetryCount >= _maxRetries || !transient)
        {
            msg.IsProcessed = true;           // Give up
            msg.ProcessedAt = DateTime.UtcNow;
            _logger.LogWarning("OUTBOX_GIVEUP Id={Id} Retries={Retries}", msg.Id, msg.RetryCount);
            return;
        }

        // (Optional) exponential delay – for now we just rely on interval scanning.
        _logger.LogInformation("OUTBOX_RETRY_SCHEDULED Id={Id} NextAttempt=+{Interval}s Retry={Retry}",
            msg.Id, _interval.TotalSeconds, msg.RetryCount);
    }
}
