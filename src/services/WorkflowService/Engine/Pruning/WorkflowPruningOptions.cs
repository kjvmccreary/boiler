namespace WorkflowService.Engine.Pruning;

public class WorkflowPruningOptions
{
    /// Maximum number of decisions retained per gateway node (oldest pruned first).
    public int MaxGatewayDecisionsPerNode { get; set; } = 25;

    /// (Reserved for future) Whether to enable total-cap pruning across all gateways.
    public bool EnableTotalCap { get; set; } = false;

    /// (Reserved) Total cap value if EnableTotalCap = true.
    public int MaxGatewayDecisionsTotal { get; set; } = 500;
}
