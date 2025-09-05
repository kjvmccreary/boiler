using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Helper responsible for preparing (tenant-scoped, idempotent) Outbox messages.
/// Does NOT call SaveChanges; caller batches persistence with other domain changes
/// so the Outbox pattern stays in the same transaction.
/// </summary>
public interface IOutboxWriter
{
    /// <summary>
    /// Enqueue a new OutboxMessage with a (possibly deterministic) IdempotencyKey.
    /// The entity is added to the DbContext but not yet saved.
    /// </summary>
    /// <param name="tenantId">Tenant scope</param>
    /// <param name="eventType">Normalized event type (e.g., workflow.instance.started)</param>
    /// <param name="payload">Arbitrary object (serialized to jsonb)</param>
    /// <param name="idempotencyKey">
    /// Optional deterministic key. If null a random Guid is used.
    /// </param>
    /// <returns>The (tracked) OutboxMessage instance.</returns>
    OutboxMessage Enqueue(int tenantId, string eventType, object payload, Guid? idempotencyKey = null);
}
