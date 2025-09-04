using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Gateways;

/// <summary>
/// Context passed to a gateway strategy. Contains the gateway node,
/// workflow instance, current workflow context JSON, and the outgoing edges.
/// </summary>
public sealed class GatewayStrategyContext
{
    public WorkflowNode Node { get; }
    public WorkflowInstance Instance { get; }
    public IReadOnlyList<WorkflowEdge> OutgoingEdges { get; }
    public string CurrentContextJson { get; }
    public IConditionEvaluator ConditionEvaluator { get; }
    public CancellationToken CancellationToken { get; }

    public GatewayStrategyContext(
        WorkflowNode node,
        WorkflowInstance instance,
        IReadOnlyList<WorkflowEdge> outgoingEdges,
        string currentContextJson,
        IConditionEvaluator conditionEvaluator,
        CancellationToken cancellationToken)
    {
        Node = node;
        Instance = instance;
        OutgoingEdges = outgoingEdges;
        CurrentContextJson = currentContextJson;
        ConditionEvaluator = conditionEvaluator;
        CancellationToken = cancellationToken;
    }
}

/// <summary>
/// Result returned by a gateway strategy. Includes core routing +
/// diagnostics metadata used for replay and audit.
/// </summary>
public sealed class GatewayStrategyDecision
{
    public string StrategyKind { get; init; } = "exclusive";
    public bool ConditionResult { get; init; } = true;
    public IReadOnlyList<string> ChosenEdgeIds { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SelectedTargetNodeIds { get; init; } = Array.Empty<string>();
    public bool ShouldWait { get; init; }
    public string? Notes { get; init; }

    // Diagnostics
    public double ElapsedMs { get; init; }
    public Guid DecisionId { get; init; } = Guid.NewGuid();
    public IDictionary<string, object>? Diagnostics { get; init; }
}

/// <summary>
/// Abstraction for gateway routing decisions.
/// </summary>
public interface IGatewayStrategy
{
    string Kind { get; }
    Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context);
}
