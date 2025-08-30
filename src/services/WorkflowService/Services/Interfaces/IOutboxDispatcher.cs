using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

public interface IOutboxDispatcher
{
    /// <summary>
    /// Dispatch a single outbox message (to broker, webhook, etc.).
    /// Return true if fully processed; false to retry.
    /// Throw only for unexpected (non-retriable) failures.
    /// </summary>
    Task<bool> DispatchAsync(OutboxMessage message, CancellationToken ct);
}
