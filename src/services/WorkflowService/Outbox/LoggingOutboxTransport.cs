using WorkflowService.Domain.Models;

namespace WorkflowService.Outbox;

public class LoggingOutboxTransport : IOutboxTransport
{
    private readonly ILogger<LoggingOutboxTransport> _logger;
    public LoggingOutboxTransport(ILogger<LoggingOutboxTransport> logger) => _logger = logger;

    public Task DeliverAsync(OutboxMessage message, CancellationToken ct)
    {
        // MVP: just log; real implementation would push to broker / HTTP endpoint.
        _logger.LogInformation("OUTBOX_TRANSPORT_DELIVER eventType={EventType} id={Id} tenant={Tenant}",
            message.EventType, message.Id, message.TenantId);
        return Task.CompletedTask;
    }
}
