using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using System.Text.Json;

namespace WorkflowService.Services;

public class EventPublisher : IEventPublisher
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(WorkflowDbContext context, ILogger<EventPublisher> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task PublishInstanceStartedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "Started", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            DefinitionVersion = instance.DefinitionVersion,
            StartedAt = instance.StartedAt,
            StartedByUserId = instance.StartedByUserId
        }, instance.StartedByUserId);

        AddOutboxMessage("workflow.instance.started", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            TenantId = instance.TenantId,
            StartedAt = instance.StartedAt
        }, instance.TenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published instance started event for workflow instance {InstanceId}", instance.Id);
    }

    public async Task PublishInstanceCompletedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        var duration = instance.CompletedAt.HasValue
            ? (instance.CompletedAt.Value - instance.StartedAt).TotalMinutes
            : 0;

        AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "Completed", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            StartedAt = instance.StartedAt,
            CompletedAt = instance.CompletedAt,
            Duration = duration
        }, null);

        AddOutboxMessage("workflow.instance.completed", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            TenantId = instance.TenantId,
            CompletedAt = instance.CompletedAt,
            Duration = duration
        }, instance.TenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published instance completed event for workflow instance {InstanceId}", instance.Id);
    }

    public async Task PublishInstanceFailedAsync(WorkflowInstance instance, string errorMessage, CancellationToken cancellationToken = default)
    {
        var failedAt = DateTime.UtcNow;

        AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "Failed", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            ErrorMessage = errorMessage,
            FailedAt = failedAt,
            Duration = (failedAt - instance.StartedAt).TotalMinutes
        }, null);

        AddOutboxMessage("workflow.instance.failed", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            TenantId = instance.TenantId,
            ErrorMessage = errorMessage,
            FailedAt = failedAt
        }, instance.TenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogWarning("Published instance failed event for workflow instance {InstanceId}: {ErrorMessage}",
            instance.Id, errorMessage);
    }

    public async Task PublishTaskCreatedAsync(WorkflowTask task, CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantIdFromTask(task);

        AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", "Created", new
        {
            TaskId = task.Id,
            WorkflowInstanceId = task.WorkflowInstanceId,
            NodeId = task.NodeId,
            TaskName = task.TaskName,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToRole = task.AssignedToRole,
            DueDate = task.DueDate,
            CreatedAt = task.CreatedAt
        }, null);

        AddOutboxMessage("workflow.task.created", new
        {
            TaskId = task.Id,
            WorkflowInstanceId = task.WorkflowInstanceId,
            TenantId = tenantId,
            TaskName = task.TaskName,
            AssignedToUserId = task.AssignedToUserId,
            AssignedToRole = task.AssignedToRole
        }, tenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published task created event for task {TaskId}", task.Id);
    }

    public async Task PublishTaskCompletedAsync(WorkflowTask task, int completedByUserId, CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantIdFromTask(task);
        var duration = task.CompletedAt.HasValue
            ? (task.CompletedAt.Value - task.CreatedAt).TotalMinutes
            : 0;

        AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", "Completed", new
        {
            TaskId = task.Id,
            WorkflowInstanceId = task.WorkflowInstanceId,
            NodeId = task.NodeId,
            TaskName = task.TaskName,
            CompletedByUserId = completedByUserId,
            CompletedAt = task.CompletedAt,
            Duration = duration,
            CompletionData = task.CompletionData
        }, completedByUserId);

        AddOutboxMessage("workflow.task.completed", new
        {
            TaskId = task.Id,
            WorkflowInstanceId = task.WorkflowInstanceId,
            TenantId = tenantId,
            CompletedByUserId = completedByUserId,
            CompletedAt = task.CompletedAt
        }, tenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published task completed event for task {TaskId} by user {UserId}",
            task.Id, completedByUserId);
    }

    public async Task PublishTaskAssignedAsync(WorkflowTask task, int assignedToUserId, CancellationToken cancellationToken = default)
    {
        var tenantId = GetTenantIdFromTask(task);

        AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", "Assigned", new
        {
            TaskId = task.Id,
            WorkflowInstanceId = task.WorkflowInstanceId,
            NodeId = task.NodeId,
            TaskName = task.TaskName,
            AssignedToUserId = assignedToUserId,
            AssignedAt = DateTime.UtcNow
        }, null);

        AddOutboxMessage("workflow.task.assigned", new
        {
            TaskId = task.Id,
            WorkflowInstanceId = task.WorkflowInstanceId,
            TenantId = tenantId,
            AssignedToUserId = assignedToUserId,
            TaskName = task.TaskName
        }, tenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published task assigned event for task {TaskId} to user {UserId}",
            task.Id, assignedToUserId);
    }

    public async Task PublishDefinitionPublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        AddOutboxMessage("workflow.definition.published", new
        {
            DefinitionId = definition.Id,
            Name = definition.Name,
            Version = definition.Version,
            TenantId = definition.TenantId,
            PublishedAt = definition.PublishedAt,
            PublishedByUserId = definition.PublishedByUserId
        }, definition.TenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published definition published event for workflow definition {DefinitionId} version {Version}",
            definition.Id, definition.Version);
    }

    public async Task PublishCustomEventAsync(string eventType, string eventName, object eventData, int tenantId, int? userId = null, CancellationToken cancellationToken = default)
    {
        var normalized = $"workflow.{eventType.ToLowerInvariant()}.{eventName.ToLowerInvariant()}";

        AddWorkflowEvent(0, tenantId, eventType, eventName, eventData, userId);
        AddOutboxMessage(normalized, eventData, tenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published custom event {EventType}.{EventName} for tenant {TenantId}",
            eventType, eventName, tenantId);
    }

    public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var messages = await _context.OutboxMessages
                .Where(m => !m.IsProcessed)
                .OrderBy(m => m.CreatedAt)
                .Take(100)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Processing {MessageCount} outbox messages", messages.Count);

            foreach (var message in messages)
            {
                try
                {
                    _logger.LogDebug("Processing outbox message {MessageId}: {EventType}",
                        message.Id, message.EventType);

                    message.IsProcessed = true;
                    message.ProcessedAt = DateTime.UtcNow;
                    message.UpdatedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process outbox message {MessageId}", message.Id);

                    message.RetryCount++;
                    message.LastError = ex.Message;
                    message.UpdatedAt = DateTime.UtcNow;

                    if (message.RetryCount >= 5)
                    {
                        message.IsProcessed = true;
                        message.ProcessedAt = DateTime.UtcNow;
                        _logger.LogError("Outbox message {MessageId} failed after {RetryCount} retries",
                            message.Id, message.RetryCount);
                    }
                }
            }

            if (messages.Count > 0)
            {
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Completed processing {MessageCount} outbox messages", messages.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing outbox messages");
        }
    }

    private void AddWorkflowEvent(int workflowInstanceId, int tenantId, string eventType, string eventName, object eventData, int? userId)
    {
        var workflowEvent = new WorkflowEvent
        {
            WorkflowInstanceId = workflowInstanceId,
            TenantId = tenantId,
            Type = eventType,
            Name = eventName,
            Data = JsonSerializer.Serialize(eventData),
            OccurredAt = DateTime.UtcNow,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.WorkflowEvents.Add(workflowEvent);
    }

    private void AddOutboxMessage(string eventType, object eventData, int tenantId)
    {
        var json = JsonSerializer.Serialize(eventData);
        var msg = new OutboxMessage
        {
            EventType = eventType,
            EventData = json,
            IsProcessed = false,
            TenantId = tenantId,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _context.OutboxMessages.Add(msg);
    }

    private int GetTenantIdFromTask(WorkflowTask task)
    {
        try
        {
            var instance = _context.WorkflowInstances
                .AsNoTracking()
                .Where(i => i.Id == task.WorkflowInstanceId)
                .Select(i => new { i.TenantId })
                .FirstOrDefault();
            return instance?.TenantId ?? 0;
        }
        catch
        {
            return 0;
        }
    }
}
