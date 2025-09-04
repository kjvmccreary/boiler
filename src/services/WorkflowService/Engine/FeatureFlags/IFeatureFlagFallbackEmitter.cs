using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.FeatureFlags;

public interface IFeatureFlagFallbackEmitter
{
    void EmitFallback(WorkflowInstance instance, string gatewayNodeId, string flag, string reason, bool required);
}
