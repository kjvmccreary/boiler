using System.Text.Json;
using Microsoft.Extensions.Logging;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;

namespace WorkflowService.Engine.Pruning;

public sealed class GatewayPruningEventEmitter : IGatewayPruningEventEmitter
{
    private readonly WorkflowDbContext _db;
    private readonly ILogger<GatewayPruningEventEmitter> _logger;

    public GatewayPruningEventEmitter(
        WorkflowDbContext db,
        ILogger<GatewayPruningEventEmitter> logger)
    {
        _db = db;
        _logger = logger;
    }

    public void EmitGatewayDecisionPruned(WorkflowInstance instance, string nodeId, int removed, int remaining)
    {
        try
        {
            var evt = new WorkflowEvent
            {
                WorkflowInstanceId = instance.Id,
                Type = "Prune",
                Name = "GatewayDecisionPruned",
                Data = JsonSerializer.Serialize(new
                {
                    nodeId,
                    removed,
                    remaining,
                    prunedAtUtc = DateTime.UtcNow
                }),
                OccurredAt = DateTime.UtcNow
            };
            _db.WorkflowEvents.Add(evt);
            // Intentionally not saving here; caller (runtime execution) will save in same unit of work.
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to emit GatewayDecisionPruned event for Instance={InstanceId} Node={NodeId}", instance.Id, nodeId);
        }
    }
}
