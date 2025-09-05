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

    #region Internal Implementations

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
        AddWorkflowEvent(workflowInstanceId ?? 0, tenantId, eventType, eventName, eventData, userId);
        _outboxWriter.Enqueue(tenantId, normalized, eventData, keyOverride);
        await CommitWithIdempotentProtectionAsync(normalized, tenantId, keyOverride, ct);
        _logger.LogInformation("Published custom event {Event}", normalized);
    }

    private async Task PublishInstanceLifecycleAsync(
        WorkflowInstance instance,
        string phase,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = instance.TenantId;

        AddWorkflowEvent(instance.Id, tenantId, "Instance", Capitalize(phase), new
        {
            InstanceId = instance.Id,
            instance.WorkflowDefinitionId,
            instance.DefinitionVersion,
            instance.StartedAt,
            instance.CompletedAt,
            Status = phase
        }, instance.StartedByUserId);

        var deterministic = OutboxIdempotency.CreateForWorkflow(
            tenantId, "instance", instance.Id, phase, instance.DefinitionVersion);

        var finalKey = keyOverride ?? deterministic;

        _outboxWriter.Enqueue(tenantId,
            $"workflow.instance.{phase}",
            new
            {
                InstanceId = instance.Id,
                instance.WorkflowDefinitionId,
                TenantId = tenantId,
                instance.StartedAt,
                instance.CompletedAt
            },
            finalKey);

        await CommitWithIdempotentProtectionAsync($"workflow.instance.{phase}", tenantId, finalKey, ct);
        _logger.LogInformation("Published instance {Phase} {InstanceId}", phase, instance.Id);
    }

    private async Task PublishInstanceFailureAsync(
        WorkflowInstance instance,
        string errorMessage,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var failedAt = DateTime.UtcNow;

        AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "Failed", new
        {
            InstanceId = instance.Id,
            instance.WorkflowDefinitionId,
            ErrorMessage = errorMessage,
            FailedAt = failedAt,
            DurationMinutes = (failedAt - instance.StartedAt).TotalMinutes
        }, null);

        var deterministic = OutboxIdempotency.CreateForWorkflow(
            instance.TenantId, "instance", instance.Id, "failed", instance.DefinitionVersion);

        _outboxWriter.Enqueue(instance.TenantId,
            "workflow.instance.failed",
            new
            {
                InstanceId = instance.Id,
                instance.WorkflowDefinitionId,
                instance.TenantId,
                ErrorMessage = errorMessage,
                FailedAt = failedAt
            },
            keyOverride ?? deterministic);

        await CommitWithIdempotentProtectionAsync("workflow.instance.failed", instance.TenantId, keyOverride ?? deterministic, ct);
        _logger.LogWarning("Published instance failed {InstanceId}", instance.Id);
    }

    private async Task PublishInstanceForceCancelledInternalAsync(
        WorkflowInstance instance,
        string reason,
        Guid? keyOverride,
        CancellationToken ct)
    {
        AddWorkflowEvent(instance.Id, instance.TenantId, "Instance", "ForceCancelled", new
        {
            InstanceId = instance.Id,
            instance.WorkflowDefinitionId,
            instance.DefinitionVersion,
            CancelledAt = DateTime.UtcNow,
            Reason = reason
        }, null);

        var deterministic = OutboxIdempotency.CreateForWorkflow(
            instance.TenantId, "instance", instance.Id, "force_cancelled", instance.DefinitionVersion);

        _outboxWriter.Enqueue(instance.TenantId,
            "workflow.instance.force_cancelled",
            new
            {
                InstanceId = instance.Id,
                instance.WorkflowDefinitionId,
                instance.TenantId,
                Reason = reason
            },
            keyOverride ?? deterministic);

        await CommitWithIdempotentProtectionAsync("workflow.instance.force_cancelled", instance.TenantId, keyOverride ?? deterministic, ct);
    }

    private async Task PublishTaskEventAsync(
        WorkflowTask task,
        string phase,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = GetTenantIdFromTask(task);

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

        Guid? deterministic = phase == "created"
            ? OutboxIdempotency.CreateForWorkflow(tenantId, "task", task.Id, "created")
            : null;

        _outboxWriter.Enqueue(tenantId,
            $"workflow.task.{phase}",
            new
            {
                TaskId = task.Id,
                task.WorkflowInstanceId,
                TenantId = tenantId,
                task.TaskName,
                task.AssignedToUserId,
                task.AssignedToRole
            },
            keyOverride ?? deterministic);

        await CommitWithIdempotentProtectionAsync($"workflow.task.{phase}", tenantId, keyOverride ?? deterministic, ct);
        _logger.LogInformation("Published task {Phase} {TaskId}", phase, task.Id);
    }

    private async Task PublishTaskCompletedInternalAsync(
        WorkflowTask task,
        int completedByUserId,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = GetTenantIdFromTask(task);
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

        var deterministic = OutboxIdempotency.CreateForWorkflow(tenantId, "task", task.Id, "completed");

        _outboxWriter.Enqueue(tenantId,
            "workflow.task.completed",
            new
            {
                TaskId = task.Id,
                task.WorkflowInstanceId,
                TenantId = tenantId,
                CompletedByUserId = completedByUserId,
                task.CompletedAt
            },
            keyOverride ?? deterministic);

        await CommitWithIdempotentProtectionAsync("workflow.task.completed", tenantId, keyOverride ?? deterministic, ct);
        _logger.LogInformation("Published task completed {TaskId}", task.Id);
    }

    private async Task PublishTaskAssignedInternalAsync(
        WorkflowTask task,
        int assignedToUserId,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var tenantId = GetTenantIdFromTask(task);

        AddWorkflowEvent(task.WorkflowInstanceId, tenantId, "Task", "Assigned", new
        {
            TaskId = task.Id,
            task.WorkflowInstanceId,
            task.NodeId,
            task.TaskName,
            AssignedToUserId = assignedToUserId,
            AssignedAt = DateTime.UtcNow
        }, null);

        _outboxWriter.Enqueue(tenantId,
            "workflow.task.assigned",
            new
            {
                TaskId = task.Id,
                task.WorkflowInstanceId,
                TenantId = tenantId,
                AssignedToUserId = assignedToUserId,
                task.TaskName
            },
            keyOverride);

        await CommitWithIdempotentProtectionAsync("workflow.task.assigned", tenantId, keyOverride, ct);
        _logger.LogInformation("Published task assigned {TaskId}", task.Id);
    }

    private async Task PublishDefinitionLifecycleAsync(
        WorkflowDefinition definition,
        string phase,
        Guid? keyOverride,
        CancellationToken ct)
    {
        var deterministic = OutboxIdempotency.CreateForWorkflow(
            definition.TenantId, "definition", definition.Id, phase, definition.Version);

        _outboxWriter.Enqueue(definition.TenantId,
            $"workflow.definition.{phase}",
            new
            {
                DefinitionId = definition.Id,
                definition.Name,
                definition.Version,
                definition.TenantId,
                Timestamp = DateTime.UtcNow
            },
            keyOverride ?? deterministic);

        await CommitWithIdempotentProtectionAsync($"workflow.definition.{phase}", definition.TenantId, keyOverride ?? deterministic, ct);
        _logger.LogInformation("Published definition {Phase} {DefinitionId}", phase, definition.Id);
    }

    #endregion

    #region Persistence Helpers

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

    private async Task CommitWithIdempotentProtectionAsync(
        string eventType,
        int tenantId,
        Guid? idempotencyKey,
        CancellationToken ct)
    {
        try
        {
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex) && idempotencyKey.HasValue)
        {
            var existing = await _context.OutboxMessages
                .AsNoTracking()
                .Where(o => o.TenantId == tenantId && o.IdempotencyKey == idempotencyKey.Value)
                .FirstOrDefaultAsync(ct);

            if (existing != null)
            {
                _logger.LogInformation(
                    "OUTBOX_IDEMPOTENT_DUPLICATE Tenant={TenantId} Key={Key} EventType={EventType}",
                    tenantId, idempotencyKey, eventType);

                _context.ChangeTracker.Clear();
                return;
            }

            _logger.LogWarning(ex,
                "OUTBOX_DUPLICATE_UNEXPECTED Tenant={TenantId} Key={Key}",
                tenantId, idempotencyKey);
            throw;
        }
    }

    private bool IsUniqueViolation(DbUpdateException ex)
    {
        // PostgreSQL
        if (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            return true;

        // SQLite (reflection; no compile-time dependency required)
        var inner = ex.InnerException;
        if (inner != null && inner.GetType().FullName == "Microsoft.Data.Sqlite.SqliteException")
        {
            var codeProp = inner.GetType().GetProperty("SqliteErrorCode");
            if (codeProp != null)
            {
                var codeVal = (int)codeProp.GetValue(inner)!;
                if (codeVal == 19 && inner.Message.IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0)
                    return true;
            }
        }

        return false;
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
