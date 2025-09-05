using Microsoft.Extensions.Options;

namespace WorkflowService.Outbox;

public class OutboxBackgroundWorker : BackgroundService
{
    private readonly IOutboxDispatcher _dispatcher;
    private readonly OutboxOptions _options;
    private readonly ILogger<OutboxBackgroundWorker> _logger;

    public OutboxBackgroundWorker(
        IOutboxDispatcher dispatcher,
        IOptions<OutboxOptions> options,
        ILogger<OutboxBackgroundWorker> logger)
    {
        _dispatcher = dispatcher;
        _options = options.Value;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OUTBOX_WORKER_START pollSeconds={Poll} batchSize={Batch}",
            _options.PollIntervalSeconds, _options.BatchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await _dispatcher.ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "OUTBOX_WORKER_LOOP_ERROR");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.PollIntervalSeconds), stoppingToken);
            }
            catch (TaskCanceledException) { }
        }

        _logger.LogInformation("OUTBOX_WORKER_STOP");
    }
}
