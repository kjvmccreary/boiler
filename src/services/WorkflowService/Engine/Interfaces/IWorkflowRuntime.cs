using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Interfaces;

/// <summary>
/// Main workflow runtime engine
/// </summary>
public interface IWorkflowRuntime
{
    /// <summary>
    /// Start a new workflow instance.
    /// </summary>
    Task<WorkflowInstance> StartWorkflowAsync(
        int definitionId,
        string initialContext,
        int? startedByUserId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    /// <summary>
    /// Continue workflow execution from current state.
    /// </summary>
    Task ContinueWorkflowAsync(
        int instanceId,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    /// <summary>
    /// Signal a workflow instance with external event.
    /// </summary>
    Task SignalWorkflowAsync(
        int instanceId,
        string signalName,
        string signalData,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    /// <summary>
    /// Complete a task (human or timer) and advance workflow.
    /// </summary>
    Task CompleteTaskAsync(
        int taskId,
        string completionData,
        int completedByUserId,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    /// <summary>
    /// Cancel a workflow instance.
    /// </summary>
    Task CancelWorkflowAsync(
        int instanceId,
        string reason,
        int? cancelledByUserId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    /// <summary>
    /// Retry a failed workflow instance.
    /// </summary>
    Task RetryWorkflowAsync(
        int instanceId,
        string? resetToNodeId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);
}
