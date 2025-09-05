using System.Security.Cryptography;
using System.Text;

namespace WorkflowService.Outbox;

/// <summary>
/// Central deterministic idempotency key factory for Outbox events.
/// Stable: same canonical inputs -> same Guid, across processes / restarts.
/// Implementation: SHA-256 over normalized (lowercase, trimmed) colon-joined parts; first 16 bytes -> Guid.
/// </summary>
public static class DeterministicOutboxKey
{
    public static Guid Instance(int tenantId, int instanceId, string phase, int definitionVersion) =>
        FromParts(tenantId, "instance", instanceId, phase, definitionVersion);

    public static Guid Task(int tenantId, int taskId, string phase) =>
        FromParts(tenantId, "task", taskId, phase);

    public static Guid Definition(int tenantId, int definitionId, string phase, int version) =>
        FromParts(tenantId, "definition", definitionId, phase, version);

    public static Guid DefinitionPublished(int tenantId, int definitionId, int version) =>
        Definition(tenantId, definitionId, "published", version);

    public static Guid DefinitionUnpublished(int tenantId, int definitionId, int version) =>
        Definition(tenantId, definitionId, "unpublished", version);

    /// <summary>
    /// Generic builder for custom categories when no strongly-typed overload exists.
    /// </summary>
    public static Guid Custom(params object[] parts) => FromParts(parts);

    private static Guid FromParts(params object[] rawParts)
    {
        if (rawParts == null || rawParts.Length == 0)
            throw new ArgumentException("At least one part required", nameof(rawParts));

        var sb = new StringBuilder();
        for (int i = 0; i < rawParts.Length; i++)
        {
            if (i > 0) sb.Append(':');
            var p = rawParts[i]?.ToString() ?? "";
            sb.Append(p.Trim().ToLowerInvariant());
        }

        Span<byte> hash = stackalloc byte[32];
        SHA256.HashData(Encoding.UTF8.GetBytes(sb.ToString()), hash);

        Span<byte> guidBytes = stackalloc byte[16];
        hash[..16].CopyTo(guidBytes);
        return new Guid(guidBytes);
    }
}
