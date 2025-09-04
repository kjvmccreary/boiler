using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using System.Text.Json;
using DTOs.Workflow.Enums;

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

    private static Guid NewKey() => Guid.NewGuid();

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
        _logger.LogInformation("Published instance started event {InstanceId}", instance.Id);
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
        _logger.LogInformation("Published instance completed event {InstanceId}", instance.Id);
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
        _logger.LogWarning("Published instance failed event {InstanceId}: {ErrorMessage}", instance.Id, errorMessage);
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
        _logger.LogInformation("Published task created {TaskId}", task.Id);
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
        _logger.LogInformation("Published task completed {TaskId}", task.Id);
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
        _logger.LogInformation("Published task assigned {TaskId}", task.Id);
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
        _logger.LogInformation("Published definition published {DefinitionId}", definition.Id);
    }

    // NEW: definition unpublished
    public async Task PublishDefinitionUnpublishedAsync(WorkflowDefinition definition, CancellationToken ct = default)
    {
        AddOutboxMessage("workflow.definition.unpublished", new
        {
            DefinitionId = definition.Id,
            Name = definition.Name,
            Version = definition.Version,
            TenantId = definition.TenantId,
            UnpublishedAt = DateTime.UtcNow
        }, definition.TenantId);

        await _context.SaveChangesAsync(ct);
        _logger.LogInformation("Published definition unpublished {DefinitionId}", definition.Id);
    }

    // NEW: instance force cancelled (reason: unpublish)
    public async Task PublishInstanceForceCancelledAsync(WorkflowInstance instance, string reason, CancellationToken ct = default)
    {
        AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "ForceCancelled", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            DefinitionVersion = instance.DefinitionVersion,
            CancelledAt = DateTime.UtcNow,
            Reason = reason
        }, null);

        AddOutboxMessage("workflow.instance.force_cancelled", new
        {
            InstanceId = instance.Id,
            WorkflowDefinitionId = instance.WorkflowDefinitionId,
            TenantId = instance.TenantId,
            Reason = reason
        }, instance.TenantId);

        await _context.SaveChangesAsync(ct);
    }

    public async Task PublishCustomEventAsync(
        string eventType,
        string eventName,
        object eventData,
        int tenantId,
        int? userId = null,
        int? workflowInstanceId = null,
        CancellationToken cancellationToken = default)
    {
        var normalized = $"workflow.{eventType.ToLowerInvariant()}.{eventName.ToLowerInvariant()}";
        AddWorkflowEvent(workflowInstanceId ?? 0, tenantId, eventType, eventName, eventData, userId);
        AddOutboxMessage(normalized, eventData, tenantId);

        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Published custom event {Event}", normalized);
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

            if (messages.Count == 0) return;

            foreach (var message in messages)
            {
                try
                {
                    message.IsProcessed = true;
                    message.ProcessedAt = DateTime.UtcNow;
                    message.UpdatedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.LastError = ex.Message;
                    message.UpdatedAt = DateTime.UtcNow;
                    if (message.RetryCount >= 5)
                    {
                        message.IsProcessed = true;
                        message.ProcessedAt = DateTime.UtcNow;
                    }
                }
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Outbox processing failed");
        }
    }

    private void AddWorkflowEvent(int workflowInstanceId, int tenantId, string eventType, string eventName, object eventData, int? userId)
    {
        _context.WorkflowEvents.Add(new WorkflowEvent
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
        });
    }

    private void AddOutboxMessage(string eventType, object eventData, int tenantId)
    {
        _context.OutboxMessages.Add(new OutboxMessage
        {
            EventType = eventType,
            EventData = JsonSerializer.Serialize(eventData),
            IsProcessed = false,
            TenantId = tenantId,
            RetryCount = 0,
            IdempotencyKey = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
    }

    private int GetTenantIdFromTask(WorkflowTask task)
    {
        try
        {
            return _context.WorkflowInstances
                .AsNoTracking()
                .Where(i => i.Id == task.WorkflowInstanceId)
                .Select(i => i.TenantId)
                .FirstOrDefault();
        }
        catch { return 0; }
    }
}
