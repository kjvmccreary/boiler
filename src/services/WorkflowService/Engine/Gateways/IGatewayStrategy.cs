using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Gateways;

public sealed class GatewayStrategyContext
{
    public WorkflowNode Node { get; }
    public WorkflowInstance Instance { get; }
    public string CurrentContextJson { get; }
    public JsonObject? StrategyConfig { get; }
    public IConditionEvaluator ConditionEvaluator { get; }
    public CancellationToken CancellationToken { get; }

    public GatewayStrategyContext(
        WorkflowNode node,
        WorkflowInstance instance,
        string currentContextJson,
        JsonObject? strategyConfig,
        IConditionEvaluator conditionEvaluator,
        CancellationToken ct)
    {
        Node = node;
        Instance = instance;
        CurrentContextJson = currentContextJson;
        StrategyConfig = strategyConfig;
        ConditionEvaluator = conditionEvaluator;
        CancellationToken = ct;
    }
}

public sealed class GatewayStrategyDecision
{
    public string StrategyKind { get; init; } = "exclusive";
    public bool ConditionResult { get; init; } = true;
    public IReadOnlyList<string> ChosenEdgeIds { get; init; } = Array.Empty<string>();
    public bool ShouldWait { get; init; } = false;
    public string? Notes { get; init; }
}

/// <summary>
/// Abstraction for gateway routing decisions.
/// </summary>
public interface IGatewayStrategy
{
    string Kind { get; }
    Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context);
}
