using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Diagnostics;

public interface IExperimentAssignmentEmitter
{
    void EmitAssigned(
        WorkflowInstance instance,
        string gatewayNodeId,
        string variant,
        string? hash,
        bool overrideApplied,
        bool snapshotReuse);
}
