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
        _logger.LogInformation("OUTBOX_DISPATCH EventType={EventType} Id={Id} Tenant={TenantId} Retry={Retry} PayloadSize={Size}",
            message.EventType ?? message.Type,
            message.Id,
            message.TenantId,
            message.RetryCount,
            message.EventData?.Length ?? message.Payload?.Length ?? 0);

        return Task.FromResult(true);
    }
}
