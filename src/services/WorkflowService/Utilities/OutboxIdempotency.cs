using System;
using System.Security.Cryptography;
using System.Text;
using WorkflowService.Outbox;

namespace WorkflowService.Utilities;

/// <summary>
/// LEGACY deterministic idempotency utilities.
/// Forwarding to DeterministicOutboxKey; will be removed in a future release.
/// </summary>
[Obsolete("OutboxIdempotency is deprecated. Use DeterministicOutboxKey (Instance/Task/Definition/Custom) instead. This will be removed in a future release.")]
public static class OutboxIdempotency
{
    /// <summary>
    /// Generic deterministic key creator from arbitrary parts (normalized).
    /// </summary>
    public static Guid CreateDeterministicGuid(params string[] parts) =>
        DeterministicOutboxKey.Custom(parts ?? Array.Empty<string>());

    /// <summary>
    /// Workflow-centric helper (tenant + category + entityId + kind + version + optional correlation).
    /// Category examples: instance | task | definition.
    /// Kind examples: started | completed | failed | created | published | unpublished.
    /// </summary>
    public static Guid CreateForWorkflow(
        int tenantId,
        string category,
        long entityId,
        string kind,
        int version = 0,
        string? correlation = null)
        => DeterministicOutboxKey.Custom(
            tenantId,
            category,
            entityId,
            kind,
            version,
            correlation ?? "");
}
