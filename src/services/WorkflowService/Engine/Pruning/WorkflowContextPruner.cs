using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkflowService.Engine.Pruning;

public sealed class WorkflowContextPruner : IWorkflowContextPruner
{
    private readonly ILogger<WorkflowContextPruner> _logger;
    private readonly WorkflowPruningOptions _options;

    public WorkflowContextPruner(
        ILogger<WorkflowContextPruner> logger,
        IOptions<WorkflowPruningOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public int PruneGatewayHistory(JsonArray history)
    {
        if (history == null) return 0;
        var max = _options.MaxGatewayDecisionsPerNode;
        if (max <= 0) return 0;
        var removed = 0;

        // Never remove the most recent entry; prune strictly from the oldest (index 0).
        while (history.Count > max)
        {
            var oldest = history[0];
            history.RemoveAt(0);
            removed++;

            // Best-effort add tombstone metadata to newly first element (optional)
            if (history.Count > 0 &&
                history[0] is JsonObject first &&
                !first.ContainsKey("_pruneInfo"))
            {
                first["_pruneInfo"] = new JsonObject
                {
                    ["prunedCountBefore"] = removed,
                    ["lastPruneAtUtc"] = DateTime.UtcNow.ToString("O")
                };
            }
        }

        if (removed > 0)
        {
            _logger.LogDebug("Gateway history pruned: Removed={Removed} Remaining={Remain} Max={Max}",
                removed, history.Count, max);
        }
        return removed;
    }
}
