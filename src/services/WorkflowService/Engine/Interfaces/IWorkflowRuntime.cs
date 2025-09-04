using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Interfaces;

public interface IWorkflowRuntime
{
    Task<WorkflowInstance> StartWorkflowAsync(
        int definitionId,
        string initialContext,
        int? startedByUserId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task ContinueWorkflowAsync(
        int instanceId,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task SignalWorkflowAsync(
        int instanceId,
        string signalName,
        string signalData,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task CompleteTaskAsync(
        int taskId,
        string completionData,
        int completedByUserId,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task CancelWorkflowAsync(
        int instanceId,
        string reason,
        int? cancelledByUserId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task RetryWorkflowAsync(
        int instanceId,
        string? resetToNodeId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task SuspendWorkflowAsync(
        int instanceId,
        string reason,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);

    Task ResumeWorkflowAsync(
        int instanceId,
        int? userId = null,
        CancellationToken cancellationToken = default,
        bool autoCommit = true);
}
