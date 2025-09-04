using System.Text.Json.Nodes;

namespace WorkflowService.Engine.Pruning;

public interface IWorkflowContextPruner
{
    /// Prunes a gateway decision history array in-place (oldest first) according to options.
    /// Returns number of entries removed.
    int PruneGatewayHistory(JsonArray history);
}
