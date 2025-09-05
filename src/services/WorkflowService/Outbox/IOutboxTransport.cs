using WorkflowService.Domain.Models;

namespace WorkflowService.Outbox;

public interface IOutboxTransport
{
    Task DeliverAsync(OutboxMessage message, CancellationToken ct);
}
