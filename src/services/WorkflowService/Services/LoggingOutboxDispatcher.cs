using WorkflowService.Domain.Models;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

public class LoggingOutboxDispatcher : IOutboxDispatcher
{
    private readonly ILogger<LoggingOutboxDispatcher> _logger;

    public LoggingOutboxDispatcher(ILogger<LoggingOutboxDispatcher> logger)
    {
        _logger = logger;
    }

    public Task<bool> DispatchAsync(OutboxMessage message, CancellationToken ct)
    {
        // MVP: just log. Future: publish to broker / webhook.
        var size = message.EventData?.Length ?? 0;

        _logger.LogInformation(
            "OUTBOX_DISPATCH EventType={EventType} Id={Id} Tenant={TenantId} Retry={Retry} PayloadSize={Size}",
            message.EventType,
            message.Id,
            message.TenantId,
            message.RetryCount,
            size);

        return Task.FromResult(true);
    }
}
