using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using System.Text.Json;
using WorkflowService.Utilities;
using Npgsql;

namespace WorkflowService.Services;

public class EventPublisher : IEventPublisher
{
    private readonly WorkflowDbContext _context;
    private readonly ILogger<EventPublisher> _logger;
    private readonly IOutboxWriter _outboxWriter;

    public EventPublisher(
        WorkflowDbContext context,
        ILogger<EventPublisher> logger,
        IOutboxWriter outboxWriter)
    {
        _context = context;
        _logger = logger;
        _outboxWriter = outboxWriter;
    }

    #region Interface Implementations

    public Task PublishInstanceStartedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
        => PublishInstanceLifecycleAsync(instance, "started", null, cancellationToken);

    public Task PublishInstanceCompletedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
        => PublishInstanceLifecycleAsync(instance, "completed", null, cancellationToken);

    public Task PublishInstanceFailedAsync(WorkflowInstance instance, string errorMessage, CancellationToken cancellationToken = default)
        => PublishInstanceFailureAsync(instance, errorMessage, null, cancellationToken);

    public Task PublishInstanceForceCancelledAsync(WorkflowInstance instance, string reason, CancellationToken cancellationToken = default)
        => PublishInstanceForceCancelledInternalAsync(instance, reason, null, cancellationToken);

    public Task PublishTaskCreatedAsync(WorkflowTask task, CancellationToken cancellationToken = default)
        => PublishTaskEventAsync(task, "created", null, cancellationToken);

    public Task PublishTaskCompletedAsync(WorkflowTask task, int completedByUserId, CancellationToken cancellationToken = default)
        => PublishTaskCompletedInternalAsync(task, completedByUserId, null, cancellationToken);

    public Task PublishTaskAssignedAsync(WorkflowTask task, int assignedToUserId, CancellationToken cancellationToken = default)
        => PublishTaskAssignedInternalAsync(task, assignedToUserId, null, cancellationToken);

    public Task PublishDefinitionPublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        => PublishDefinitionLifecycleAsync(definition, "published", null, cancellationToken);

    public Task PublishDefinitionUnpublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
        => PublishDefinitionLifecycleAsync(definition, "unpublished", null, cancellationToken);

    public Task PublishCustomEventAsync(
        string eventType,
        string eventName,
        object eventData,
        int tenantId,
        int? userId = null,
        int? workflowInstanceId = null,
        CancellationToken cancellationToken = default)
        => PublishCustomEventInternalAsync(eventType, eventName, eventData, tenantId, userId, workflowInstanceId, null, cancellationToken);

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
                    message.Error = null;
                    message.UpdatedAt = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;
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

    #endregion

    #region Optional Override Helpers

    public Task PublishInstanceStartedWithKeyAsync(WorkflowInstance instance, Guid key, CancellationToken ct = default)
        => PublishInstanceLifecycleAsync(instance, "started", key, ct);
    public Task PublishInstanceCompletedWithKeyAsync(WorkflowInstance instance, Guid key, CancellationToken ct = default)
        => PublishInstanceLifecycleAsync(instance, "completed", key, ct);
    public Task PublishInstanceFailedWithKeyAsync(WorkflowInstance instance, string errorMessage, Guid key, CancellationToken ct = default)
        => PublishInstanceFailureAsync(instance, errorMessage, key, ct);
    public Task PublishInstanceForceCancelledWithKeyAsync(WorkflowInstance instance, string reason, Guid key, CancellationToken ct = default)
        => PublishInstanceForceCancelledInternalAsync(instance, reason, key, ct);
    public Task PublishTaskCreatedWithKeyAsync(WorkflowTask task, Guid key, CancellationToken ct = default)
        => PublishTaskEventAsync(task, "created", key, ct);
    public Task PublishTaskCompletedWithKeyAsync(WorkflowTask task, int completedByUserId, Guid key, CancellationToken ct = default)
        => PublishTaskCompletedInternalAsync(task, completedByUserId, key, ct);
    public Task PublishTaskAssignedWithKeyAsync(WorkflowTask task, int assignedToUserId, Guid key, CancellationToken ct = default)
        => PublishTaskAssignedInternalAsync(task, assignedToUserId, key, ct);
    public Task PublishDefinitionPublishedWithKeyAsync(WorkflowDefinition definition, Guid key, CancellationToken ct = default)
        => PublishDefinitionLifecycleAsync(definition, "published", key, ct);
    public Task PublishDefinitionUnpublishedWithKeyAsync(WorkflowDefinition definition, Guid key, CancellationToken ct = default)
        => PublishDefinitionLifecycleAsync(definition, "unpublished", key, ct);
    public Task PublishCustomEventWithKeyAsync(
        string eventType,
        string eventName,
        object eventData,
        int tenantId,
        Guid key,
        int? userId = null,
        int? workflowInstanceId = null,
        CancellationToken ct = default)
        => PublishCustomEventInternalAsync(eventType, eventName, eventData, tenantId, userId, workflowInstanceId, key, ct);

    #endregion

    #region Internal Implementations (Refactored for centralized idempotent insert)

    private async Task PublishCustomEventInternalAsync(
        string eventType,
        string eventName,
        object eventData,
        int tenantId,
        int? userId,
        int? workflowInstanceId,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var normalized = $"workflow.{eventType.ToLowerInvariant()}.{eventName.ToLowerInvariant()}";

        // Random (or supplied) key; custom events usually not deterministic unless forced.
        var result = await _outboxWriter.TryAddAsync(
            tenantId,
            normalized,
            eventData,
            keyOverride,
            ct);

        if (!result.AlreadyExisted)
        {
            AddWorkflowEvent(workflowInstanceId ?? 0, tenantId, eventType, Capitalize(eventName), eventData, userId);
            await _context.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Published custom event {Event} duplicate={Dup}", normalized, result.AlreadyExisted);
    }

    private async Task PublishInstanceLifecycleAsync(
        WorkflowInstance instance,
        string phase,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = instance.TenantId;
        var deterministic = OutboxIdempotency.CreateForWorkflow(
            tenantId, "instance", instance.Id, phase, instance.DefinitionVersion);
        var key = keyOverride ?? deterministic;

        var outboxPayload = new
        {
            InstanceId = instance.Id,
            instance.WorkflowDefinitionId,
            TenantId = tenantId,
            instance.StartedAt,
            instance.CompletedAt
        };

        var result = await _outboxWriter.TryAddAsync(
            tenantId,
            $"workflow.instance.{phase}",
            outboxPayload,
            key,
            ct);

        if (!result.AlreadyExisted)
        {
            var wfPayload = new
            {
                InstanceId = instance.Id,
                instance.WorkflowDefinitionId,
                instance.DefinitionVersion,
                instance.StartedAt,
                instance.CompletedAt,
                Status = phase
            };
            AddWorkflowEvent(instance.Id, tenantId, "Instance", Capitalize(phase), wfPayload, instance.StartedByUserId);
            await _context.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Published instance {Phase} instanceId={Id} duplicate={Dup}",
            phase, instance.Id, result.AlreadyExisted);
    }

    private async Task PublishInstanceFailureAsync(
        WorkflowInstance instance,
        string errorMessage,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var deterministic = OutboxIdempotency.CreateForWorkflow(
            instance.TenantId, "instance", instance.Id, "failed", instance.DefinitionVersion);

        var key = keyOverride ?? deterministic;

        var outboxPayload = new
        {
            InstanceId = instance.Id,
            instance.WorkflowDefinitionId,
            instance.TenantId,
            ErrorMessage = errorMessage,
            FailedAt = DateTime.UtcNow
        };

        var result = await _outboxWriter.TryAddAsync(
            instance.TenantId,
            "workflow.instance.failed",
            outboxPayload,
            key,
            ct);

        if (!result.AlreadyExisted)
        {
            var failedAt = outboxPayload.FailedAt;
            AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "Failed", new
            {
                InstanceId = instance.Id,
                instance.WorkflowDefinitionId,
                ErrorMessage = errorMessage,
                FailedAt = failedAt,
                DurationMinutes = (failedAt - instance.StartedAt).TotalMinutes
            }, null);
            await _context.SaveChangesAsync(ct);
        }

        _logger.LogWarning("Published instance failed instanceId={Id} duplicate={Dup}",
            instance.Id, result.AlreadyExisted);
    }

    private async Task PublishInstanceForceCancelledInternalAsync(
        WorkflowInstance instance,
        string reason,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var deterministic = OutboxIdempotency.CreateForWorkflow(
            instance.TenantId, "instance", instance.Id, "force_cancelled", instance.DefinitionVersion);
        var key = keyOverride ?? deterministic;

        var outboxPayload = new
        {
            InstanceId = instance.Id,
            instance.WorkflowDefinitionId,
            instance.TenantId,
            Reason = reason
        };

        var result = await _outboxWriter.TryAddAsync(
            instance.TenantId,
            "workflow.instance.force_cancelled",
            outboxPayload,
            key,
            ct);

        if (!result.AlreadyExisted)
        {
            AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "ForceCancelled", new
            {
                InstanceId = instance.Id,
                instance.WorkflowDefinitionId,
                instance.DefinitionVersion,
                CancelledAt = DateTime.UtcNow,
                Reason = reason
            }, null);
            await _context.SaveChangesAsync(ct);
        }
    }

    private async Task PublishTaskEventAsync(
        WorkflowTask task,
        string phase,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = GetTenantIdFromTask(task);

        Guid? deterministic = phase == "created"
            ? OutboxIdempotency.CreateForWorkflow(tenantId, "task", task.Id, "created")
            : phase == "completed"
                ? OutboxIdempotency.CreateForWorkflow(tenantId, "task", task.Id, "completed")
                : null;

        var key = keyOverride ?? deterministic;

        var outboxPayload = new
        {
            TaskId = task.Id,
            task.WorkflowInstanceId,
            TenantId = tenantId,
            task.TaskName,
            task.AssignedToUserId,
            task.AssignedToRole
        };

        var result = await _outboxWriter.TryAddAsync(
            tenantId,
            $"workflow.task.{phase}",
            outboxPayload,
            key,
            ct);

        if (!result.AlreadyExisted)
        {
            AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", Capitalize(phase), new
            {
                TaskId = task.Id,
                task.WorkflowInstanceId,
                task.NodeId,
                task.TaskName,
                task.AssignedToUserId,
                task.AssignedToRole,
                task.DueDate,
                CreatedAt = task.CreatedAt
            }, null);
            await _context.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Published task {Phase} taskId={TaskId} duplicate={Dup}",
            phase, task.Id, result.AlreadyExisted);
    }

    private async Task PublishTaskCompletedInternalAsync(
        WorkflowTask task,
        int completedByUserId,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = GetTenantIdFromTask(task);
        var deterministic = OutboxIdempotency.CreateForWorkflow(tenantId, "task", task.Id, "completed");
        var key = keyOverride ?? deterministic;

        var outboxPayload = new
        {
            TaskId = task.Id,
            task.WorkflowInstanceId,
            TenantId = tenantId,
            CompletedByUserId = completedByUserId,
            task.CompletedAt
        };

        var result = await _outboxWriter.TryAddAsync(
            tenantId,
            "workflow.task.completed",
            outboxPayload,
            key,
            ct);

        if (!result.AlreadyExisted)
        {
            var duration = task.CompletedAt.HasValue
                ? (task.CompletedAt.Value - task.CreatedAt).TotalMinutes
                : 0;

            AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", "Completed", new
            {
                TaskId = task.Id,
                task.WorkflowInstanceId,
                task.NodeId,
                task.TaskName,
                CompletedByUserId = completedByUserId,
                task.CompletedAt,
                DurationMinutes = duration,
                task.CompletionData
            }, completedByUserId);

            await _context.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Published task completed taskId={TaskId} duplicate={Dup}",
            task.Id, result.AlreadyExisted);
    }

    private async Task PublishTaskAssignedInternalAsync(
        WorkflowTask task,
        int assignedToUserId,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = GetTenantIdFromTask(task);

        // Assignment events intentionally not deterministic unless override provided
        var outboxPayload = new
        {
            TaskId = task.Id,
            task.WorkflowInstanceId,
            TenantId = tenantId,
            AssignedToUserId = assignedToUserId,
            task.TaskName
        };

        var result = await _outboxWriter.TryAddAsync(
            tenantId,
            "workflow.task.assigned",
            outboxPayload,
            keyOverride,
            ct);

        if (!result.AlreadyExisted)
        {
            AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", "Assigned", new
            {
                TaskId = task.Id,
                task.WorkflowInstanceId,
                task.NodeId,
                task.TaskName,
                AssignedToUserId = assignedToUserId,
                AssignedAt = DateTime.UtcNow
            }, null);
            await _context.SaveChangesAsync(ct);
        }

        _logger.LogInformation("Published task assigned taskId={TaskId} duplicate={Dup}",
            task.Id, result.AlreadyExisted);
    }

    private async Task PublishDefinitionLifecycleAsync(
        WorkflowDefinition definition,
        string phase,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var deterministic = OutboxIdempotency.CreateForWorkflow(
            definition.TenantId, "definition", definition.Id, phase, definition.Version);
        var key = keyOverride ?? deterministic;

        var outboxPayload = new
        {
            DefinitionId = definition.Id,
            definition.Name,
            definition.Version,
            definition.TenantId,
            Timestamp = DateTime.UtcNow
        };

        var result = await _outboxWriter.TryAddAsync(
            definition.TenantId,
            $"workflow.definition.{phase}",
            outboxPayload,
            key,
            ct);

        // (Previous design: no internal WorkflowEvent for publish/unpublish; preserve that.)
        _logger.LogInformation("Published definition {Phase} defId={Id} duplicate={Dup}",
            phase, definition.Id, result.AlreadyExisted);
    }

    #endregion

    #region Helpers

    private string Capitalize(string s) => string.IsNullOrEmpty(s) ? s : char.ToUpperInvariant(s[0]) + s[1..];

    private void AddWorkflowEvent(
        int workflowInstanceId,
        int tenantId,
        string eventType,
        string eventName,
        object eventData,
        int? userId)
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
        catch
        {
            return 0;
        }
    }

    #endregion
}
