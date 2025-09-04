using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Pruning;

public interface IGatewayPruningEventEmitter
{
    void EmitGatewayDecisionPruned(WorkflowInstance instance, string nodeId, int removed, int remaining);
}
