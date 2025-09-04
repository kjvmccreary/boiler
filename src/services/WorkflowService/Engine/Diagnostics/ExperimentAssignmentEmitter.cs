using System.Text.Json;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;

namespace WorkflowService.Engine.Diagnostics;

public sealed class ExperimentAssignmentEmitter : IExperimentAssignmentEmitter
{
    private readonly WorkflowDbContext _db;

    public ExperimentAssignmentEmitter(WorkflowDbContext db)
    {
        _db = db;
    }

    public void EmitAssigned(
        WorkflowInstance instance,
        string gatewayNodeId,
        string variant,
        string? hash,
        bool overrideApplied,
        bool snapshotReuse)
    {
        var payload = new
        {
            gatewayNodeId,
            variant,
            definitionId = instance.WorkflowDefinitionId,
            definitionVersion = instance.DefinitionVersion,
            instanceId = instance.Id,
            tenantId = instance.TenantId,
            hash,
            overrideApplied,
            snapshotReuse,
            occurredAtUtc = DateTime.UtcNow
        };

        var data = JsonSerializer.Serialize(payload);

        _db.WorkflowEvents.Add(new WorkflowEvent
        {
            WorkflowInstanceId = instance.Id,
            TenantId = instance.TenantId,
            Type = "Experiment",
            Name = "ExperimentAssigned",
            Data = data,
            OccurredAt = DateTime.UtcNow
        });

        _db.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "workflow.experiment.assigned",
            EventData = data,
            TenantId = instance.TenantId,
            IsProcessed = false,
            RetryCount = 0
        });
    }
}
