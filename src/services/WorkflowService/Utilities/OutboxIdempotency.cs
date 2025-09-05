using System;
using System.Security.Cryptography;
using System.Text;

namespace WorkflowService.Utilities;

/// <summary>
/// LEGACY deterministic idempotency utilities.
/// Replaced by DeterministicOutboxKey (WorkflowService.Outbox namespace).
/// </summary>
[Obsolete("OutboxIdempotency is deprecated. Use DeterministicOutboxKey (Instance/Task/Definition/Create) instead. This will be removed in a future release.")]
public static class OutboxIdempotency
{
    /// <summary>
    /// Generic deterministic key creator from arbitrary parts (already normalized by caller if desired).
    /// </summary>
    public static Guid CreateDeterministicGuid(params string[] parts)
    {
        if (parts is null || parts.Length == 0)
            throw new ArgumentException("At least one part must be supplied", nameof(parts));

        var canonical = string.Join(':', parts.Select(p => p?.Trim().ToLowerInvariant() ?? ""));
        var bytes = Encoding.UTF8.GetBytes(canonical);

        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(bytes, hash);

        Span<byte> guidBytes = stackalloc byte[16];
        hash[..16].CopyTo(guidBytes);

        return new Guid(guidBytes);
    }

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
    {
        return CreateDeterministicGuid(
            tenantId.ToString(),
            category,
            entityId.ToString(),
            kind,
            version.ToString(),
            correlation ?? "");
    }
}
