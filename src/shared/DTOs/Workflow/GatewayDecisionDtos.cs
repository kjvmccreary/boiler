using System.Text.Json.Serialization;

namespace DTOs.Workflow.GatewayDiagnostics;

public class GatewayDecisionHistoryDto
{
    public int InstanceId { get; set; }
    public List<GatewayNodeDecisionHistoryDto> Gateways { get; set; } = new();
}

public class GatewayNodeDecisionHistoryDto
{
    public string NodeId { get; set; } = string.Empty;
    public List<GatewayDecisionEntryDto> Decisions { get; set; } = new();
}

public class GatewayDecisionEntryDto
{
    public string DecisionId { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public bool ConditionResult { get; set; }
    public List<string> ChosenEdgeIds { get; set; } = new();
    public List<string> SelectedTargets { get; set; } = new();
    public bool ShouldWait { get; set; }
    public double ElapsedMs { get; set; }
    public string? Notes { get; set; }
    public DateTime EvaluatedAtUtc { get; set; }

    // Diagnostics
    public int? OutgoingEdgeCount { get; set; }
    public Dictionary<string, string[]>? Classification { get; set; }
    public List<GatewayEdgeSnapshotDto>? OutgoingSnapshot { get; set; }
    public string[]? SkippedEdgeIds { get; set; }
    public Dictionary<string, object>? Extra { get; set; }
}

public class GatewayEdgeSnapshotDto
{
    public string EdgeId { get; set; } = string.Empty;
    public string? From { get; set; }
    public string? To { get; set; }
    public string? InferredLabel { get; set; }
}
