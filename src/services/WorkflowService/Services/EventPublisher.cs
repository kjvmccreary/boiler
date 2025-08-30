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
        await PublishEventAsync(
            instance.Id,
            "Instance",
            "Started",
            new
            {
                InstanceId = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                DefinitionVersion = instance.DefinitionVersion,
                StartedAt = instance.StartedAt,
                StartedByUserId = instance.StartedByUserId
            },
            instance.StartedByUserId,
            cancellationToken);

        await CreateOutboxMessage(
            "workflow.instance.started",
            new
            {
                InstanceId = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                TenantId = instance.TenantId,
                StartedAt = instance.StartedAt
            },
            instance.TenantId,
            cancellationToken);

        _logger.LogInformation("Published instance started event for workflow instance {InstanceId}", instance.Id);
    }

    public async Task PublishInstanceCompletedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            instance.Id,
            "Instance",
            "Completed",
            new
            {
                InstanceId = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                StartedAt = instance.StartedAt,
                CompletedAt = instance.CompletedAt,
                Duration = instance.CompletedAt.HasValue 
                    ? (instance.CompletedAt.Value - instance.StartedAt).TotalMinutes 
                    : 0
            },
            null,
            cancellationToken);

        await CreateOutboxMessage(
            "workflow.instance.completed",
            new
            {
                InstanceId = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                TenantId = instance.TenantId,
                CompletedAt = instance.CompletedAt,
                Duration = instance.CompletedAt.HasValue 
                    ? (instance.CompletedAt.Value - instance.StartedAt).TotalMinutes 
                    : 0
            },
            instance.TenantId,
            cancellationToken);

        _logger.LogInformation("Published instance completed event for workflow instance {InstanceId}", instance.Id);
    }

    public async Task PublishInstanceFailedAsync(WorkflowInstance instance, string errorMessage, CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            instance.Id,
            "Instance",
            "Failed",
            new
            {
                InstanceId = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                ErrorMessage = errorMessage,
                FailedAt = DateTime.UtcNow,
                Duration = (DateTime.UtcNow - instance.StartedAt).TotalMinutes
            },
            null,
            cancellationToken);

        await CreateOutboxMessage(
            "workflow.instance.failed",
            new
            {
                InstanceId = instance.Id,
                WorkflowDefinitionId = instance.WorkflowDefinitionId,
                TenantId = instance.TenantId,
                ErrorMessage = errorMessage,
                FailedAt = DateTime.UtcNow
            },
            instance.TenantId,
            cancellationToken);

        _logger.LogWarning("Published instance failed event for workflow instance {InstanceId}: {ErrorMessage}", 
            instance.Id, errorMessage);
    }

    public async Task PublishTaskCreatedAsync(WorkflowTask task, CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            task.WorkflowInstanceId,
            "Task",
            "Created",
            new
            {
                TaskId = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                NodeId = task.NodeId,
                TaskName = task.TaskName,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToRole = task.AssignedToRole,
                DueDate = task.DueDate,
                CreatedAt = task.CreatedAt
            },
            null,
            cancellationToken);

        await CreateOutboxMessage(
            "workflow.task.created",
            new
            {
                TaskId = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                TenantId = GetTenantIdFromTask(task),
                TaskName = task.TaskName,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToRole = task.AssignedToRole
            },
            GetTenantIdFromTask(task),
            cancellationToken);

        _logger.LogInformation("Published task created event for task {TaskId}", task.Id);
    }

    public async Task PublishTaskCompletedAsync(WorkflowTask task, int completedByUserId, CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            task.WorkflowInstanceId,
            "Task",
            "Completed",
            new
            {
                TaskId = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                NodeId = task.NodeId,
                TaskName = task.TaskName,
                CompletedByUserId = completedByUserId,
                CompletedAt = task.CompletedAt,
                Duration = task.CompletedAt.HasValue 
                    ? (task.CompletedAt.Value - task.CreatedAt).TotalMinutes 
                    : 0,
                CompletionData = task.CompletionData
            },
            completedByUserId,
            cancellationToken);

        await CreateOutboxMessage(
            "workflow.task.completed",
            new
            {
                TaskId = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                TenantId = GetTenantIdFromTask(task),
                CompletedByUserId = completedByUserId,
                CompletedAt = task.CompletedAt
            },
            GetTenantIdFromTask(task),
            cancellationToken);

        _logger.LogInformation("Published task completed event for task {TaskId} by user {UserId}", 
            task.Id, completedByUserId);
    }

    public async Task PublishTaskAssignedAsync(WorkflowTask task, int assignedToUserId, CancellationToken cancellationToken = default)
    {
        await PublishEventAsync(
            task.WorkflowInstanceId,
            "Task",
            "Assigned",
            new
            {
                TaskId = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                NodeId = task.NodeId,
                TaskName = task.TaskName,
                AssignedToUserId = assignedToUserId,
                AssignedAt = DateTime.UtcNow
            },
            null,
            cancellationToken);

        await CreateOutboxMessage(
            "workflow.task.assigned",
            new
            {
                TaskId = task.Id,
                WorkflowInstanceId = task.WorkflowInstanceId,
                TenantId = GetTenantIdFromTask(task),
                AssignedToUserId = assignedToUserId,
                TaskName = task.TaskName
            },
            GetTenantIdFromTask(task),
            cancellationToken);

        _logger.LogInformation("Published task assigned event for task {TaskId} to user {UserId}", 
            task.Id, assignedToUserId);
    }

    public async Task PublishDefinitionPublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        await CreateOutboxMessage(
            "workflow.definition.published",
            new
            {
                DefinitionId = definition.Id,
                Name = definition.Name,
                Version = definition.Version,
                TenantId = definition.TenantId,
                PublishedAt = definition.PublishedAt,
                PublishedByUserId = definition.PublishedByUserId
            },
            definition.TenantId,
            cancellationToken);

        _logger.LogInformation("Published definition published event for workflow definition {DefinitionId} version {Version}", 
            definition.Id, definition.Version);
    }

    public async Task PublishCustomEventAsync(string eventType, string eventName, object eventData, int tenantId, int? userId = null, CancellationToken cancellationToken = default)
    {
        // For custom events, we don't have a specific workflow instance, so we'll use 0 as a placeholder
        await PublishEventAsync(
            0, // No specific instance
            eventType,
            eventName,
            eventData,
            userId,
            cancellationToken);

        await CreateOutboxMessage(
            $"workflow.{eventType.ToLower()}.{eventName.ToLower()}",
            eventData,
            tenantId,
            cancellationToken);

        _logger.LogInformation("Published custom event {EventType}.{EventName} for tenant {TenantId}", 
            eventType, eventName, tenantId);
    }

    public async Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Get unprocessed outbox messages (limit to avoid memory issues)
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
                    // In a real implementation, you would:
                    // 1. Send to message queue (RabbitMQ, Azure Service Bus, etc.)
                    // 2. Send to external webhooks
                    // 3. Trigger other services
                    
                    // For now, we'll just mark as processed and log
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
                    
                    // Mark as failed after 5 retries
                    if (message.RetryCount >= 5)
                    {
                        message.IsProcessed = true; // Stop trying
                        message.ProcessedAt = DateTime.UtcNow;
                        _logger.LogError("Outbox message {MessageId} failed after {RetryCount} retries", 
                            message.Id, message.RetryCount);
                    }
                }
            }

            await _context.SaveChangesAsync(cancellationToken);
            
            if (messages.Any())
            {
                _logger.LogInformation("Completed processing {MessageCount} outbox messages", messages.Count);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing outbox messages");
        }
    }

    private async Task PublishEventAsync(int workflowInstanceId, string eventType, string eventName, object eventData, int? userId, CancellationToken cancellationToken)
    {
        var workflowEvent = new WorkflowEvent
        {
            WorkflowInstanceId = workflowInstanceId,
            Type = eventType,
            Name = eventName,
            Data = JsonSerializer.Serialize(eventData),
            OccurredAt = DateTime.UtcNow,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.WorkflowEvents.Add(workflowEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private async Task CreateOutboxMessage(string eventType, object eventData, int tenantId, CancellationToken cancellationToken)
    {
        var json = JsonSerializer.Serialize(eventData);

        var outboxMessage = new OutboxMessage
        {
            // Legacy canonical fields (required by current model configuration)
            Type = eventType,
            Payload = json,
            Processed = false,

            // New fields (keep in sync for now)
            EventType = eventType,
            EventData = json,
            IsProcessed = false,

            TenantId = tenantId,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.OutboxMessages.Add(outboxMessage);
        await _context.SaveChangesAsync(cancellationToken);
    }

    private int GetTenantIdFromTask(WorkflowTask task)
    {
        // Resolve tenant via instance (avoids schema change at this step).
        // NOTE: This adds a query per call; acceptable for MVP volume.
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
