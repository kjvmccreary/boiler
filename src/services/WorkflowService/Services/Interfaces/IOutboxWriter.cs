using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Helper responsible for preparing (tenant-scoped, idempotent) Outbox messages.
/// </summary>
public interface IOutboxWriter
{
    /// <summary>
    /// Enqueue a new OutboxMessage with a (possibly deterministic) IdempotencyKey
    /// WITHOUT saving (legacy method – caller will SaveChanges and must handle uniqueness).
    /// </summary>
    OutboxMessage Enqueue(int tenantId, string eventType, object payload, Guid? idempotencyKey = null);

    /// <summary>
    /// Tries to insert an OutboxMessage idempotently (saves immediately).
    /// On unique violation (TenantId, IdempotencyKey) it loads the existing row
    /// and returns (AlreadyExisted = true).
    /// </summary>
    /// <param name="tenantId">Tenant scope</param>
    /// <param name="eventType">Normalized event type</param>
    /// <param name="payload">Serializable object or raw JSON string</param>
    /// <param name="idempotencyKey">Deterministic key (if null a new Guid is generated)</param>
    /// <returns>Result with created or existing message and AlreadyExisted flag</returns>
    Task<OutboxEnqueueResult> TryAddAsync(
        int tenantId,
        string eventType,
        object payload,
        Guid? idempotencyKey = null,
        CancellationToken ct = default);
}

/// <summary>
/// Result of an idempotent Outbox insert attempt.
/// </summary>
public sealed record OutboxEnqueueResult(OutboxMessage Message, bool AlreadyExisted);
