using System.Security.Cryptography;
using System.Text;
using WorkflowService.Utilities;

namespace WorkflowService.Outbox;

public static class DeterministicOutboxKey
{
    // Forward to legacy helper to guarantee identical keys during transition.
    public static Guid Instance(int tenantId, int instanceId, string phase, int definitionVersion) =>
        OutboxIdempotency.CreateForWorkflow(tenantId, "instance", instanceId, phase, definitionVersion);

    public static Guid Task(int tenantId, int taskId, string phase) =>
        OutboxIdempotency.CreateForWorkflow(tenantId, "task", taskId, phase);

    public static Guid Definition(int tenantId, int definitionId, string phase, int version) =>
        OutboxIdempotency.CreateForWorkflow(tenantId, "definition", definitionId, phase, version);

    public static Guid DefinitionPublished(int tenantId, int definitionId, int version) =>
        Definition(tenantId, definitionId, "published", version);

    public static Guid DefinitionUnpublished(int tenantId, int definitionId, int version) =>
        Definition(tenantId, definitionId, "unpublished", version);

    public static Guid Custom(params object[] parts)
    {
        // Normalize like legacy helper does (lowercase, trim, colon-join)
        var normalized = parts?.Select(p => (p?.ToString() ?? "").Trim().ToLowerInvariant()).ToArray() ?? Array.Empty<string>();
        return OutboxIdempotency.CreateDeterministicGuid(normalized);
    }
}
