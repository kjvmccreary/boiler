using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Service for publishing workflow events
/// </summary>
public interface IEventPublisher
{
    Task PublishInstanceStartedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);
    Task PublishInstanceCompletedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);
    Task PublishInstanceFailedAsync(WorkflowInstance instance, string errorMessage, CancellationToken cancellationToken = default);
    Task PublishTaskCreatedAsync(WorkflowTask task, CancellationToken cancellationToken = default);
    Task PublishTaskCompletedAsync(WorkflowTask task, int completedByUserId, CancellationToken cancellationToken = default);
    Task PublishTaskAssignedAsync(WorkflowTask task, int assignedToUserId, CancellationToken cancellationToken = default);
    Task PublishDefinitionPublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish custom workflow event (instance-scoped if workflowInstanceId provided).
    /// </summary>
    Task PublishCustomEventAsync(
        string eventType,
        string eventName,
        object eventData,
        int tenantId,
        int? userId = null,
        int? workflowInstanceId = null,
        CancellationToken cancellationToken = default);

    Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default);
}
