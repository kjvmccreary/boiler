using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

/// <summary>
/// Service for publishing workflow events
/// </summary>
public interface IEventPublisher
{
    /// <summary>
    /// Publish workflow instance started event
    /// </summary>
    Task PublishInstanceStartedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish workflow instance completed event
    /// </summary>
    Task PublishInstanceCompletedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish workflow instance failed event
    /// </summary>
    Task PublishInstanceFailedAsync(WorkflowInstance instance, string errorMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish task created event
    /// </summary>
    Task PublishTaskCreatedAsync(WorkflowTask task, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish task completed event
    /// </summary>
    Task PublishTaskCompletedAsync(WorkflowTask task, int completedByUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish task assigned event
    /// </summary>
    Task PublishTaskAssignedAsync(WorkflowTask task, int assignedToUserId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish definition published event
    /// </summary>
    Task PublishDefinitionPublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publish custom workflow event
    /// </summary>
    Task PublishCustomEventAsync(string eventType, string eventName, object eventData, int tenantId, int? userId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Process outbox messages (for transactional outbox pattern)
    /// </summary>
    Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default);
}
