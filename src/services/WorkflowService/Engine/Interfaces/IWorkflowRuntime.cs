using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Interfaces;

/// <summary>
/// Main workflow runtime engine
/// </summary>
public interface IWorkflowRuntime
{
    /// <summary>
    /// Start a new workflow instance
    /// </summary>
    Task<WorkflowInstance> StartWorkflowAsync(int definitionId, string initialContext, int? startedByUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Continue workflow execution from current state
    /// </summary>
    Task ContinueWorkflowAsync(int instanceId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Signal a workflow instance with external event
    /// </summary>
    Task SignalWorkflowAsync(int instanceId, string signalName, string signalData, int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Complete a human task and continue workflow
    /// </summary>
    Task CompleteTaskAsync(int taskId, string completionData, int completedByUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancel a workflow instance
    /// </summary>
    Task CancelWorkflowAsync(int instanceId, string reason, int? cancelledByUserId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retry a failed workflow instance
    /// </summary>
    Task RetryWorkflowAsync(int instanceId, string? resetToNodeId = null, CancellationToken cancellationToken = default);
}
