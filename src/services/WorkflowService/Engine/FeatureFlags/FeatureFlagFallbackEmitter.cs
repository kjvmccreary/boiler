using System.Text.Json;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;

namespace WorkflowService.Engine.FeatureFlags;

/// <summary>
/// Persists a WorkflowEvent for feature flag fallback scenarios.
/// Defer SaveChanges to outer unit-of-work (runtime).
/// </summary>
public sealed class FeatureFlagFallbackEmitter : IFeatureFlagFallbackEmitter
{
    private readonly WorkflowDbContext _db;

    public FeatureFlagFallbackEmitter(WorkflowDbContext db)
    {
        _db = db;
    }

    public void EmitFallback(WorkflowInstance instance, string gatewayNodeId, string flag, string reason, bool required)
    {
        var evt = new WorkflowEvent
        {
            WorkflowInstanceId = instance.Id,
            Type = "FeatureFlag",
            Name = "FeatureFlagFallback",
            Data = JsonSerializer.Serialize(new
            {
                gatewayNodeId,
                flag,
                required,
                reason,
                occurredAtUtc = DateTime.UtcNow
            }),
            OccurredAt = DateTime.UtcNow
        };
        _db.WorkflowEvents.Add(evt);
    }
}
