using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Dsl;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Gateways;

public interface IGatewayStrategyRegistry
{
    IGatewayStrategy Get(string kind);
    IReadOnlyCollection<string> RegisteredKinds { get; }
}

public class GatewayStrategyRegistry : IGatewayStrategyRegistry
{
    private readonly ConcurrentDictionary<string, IGatewayStrategy> _map =
        new(StringComparer.OrdinalIgnoreCase);

    public GatewayStrategyRegistry(IEnumerable<IGatewayStrategy> strategies)
    {
        foreach (var s in strategies)
        {
            if (!string.IsNullOrWhiteSpace(s.Kind))
                _map.TryAdd(s.Kind.Trim(), s);
        }

        // Always ensure exclusive exists as fallback
        if (!_map.ContainsKey("exclusive"))
            _map.TryAdd("exclusive", new ExclusiveGatewayStrategy());
    }

    public IGatewayStrategy Get(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind)) return _map["exclusive"];
        return _map.TryGetValue(kind.Trim(), out var s) ? s : _map["exclusive"];
    }

    public IReadOnlyCollection<string> RegisteredKinds => _map.Keys.ToList().AsReadOnly();
}

/// <summary>
/// Exclusive strategy: single condition evaluation (legacy behavior).
/// Optional config: { "defaultTrue": true } to force true when condition blank.
/// </summary>
public class ExclusiveGatewayStrategy : IGatewayStrategy
{
    public string Kind => "exclusive";

    public async Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context)
    {
        var condition = ExtractCondition(context.Node);
        bool result = true;

        if (!string.IsNullOrWhiteSpace(condition) && !string.Equals(condition, "true", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                result = await context.ConditionEvaluator.EvaluateAsync(condition, context.CurrentContextJson);
            }
            catch
            {
                // Default to true on error (legacy behavior)
                result = true;
            }
        }

        return new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = result,
            // ChosenEdgeIds intentionally empty for now; runtime still does edge resolution
            ChosenEdgeIds = Array.Empty<string>(),
            ShouldWait = false
        };
    }

    private static string ExtractCondition(WorkflowNode node)
    {
        if (node.Properties.TryGetValue("condition", out var v))
        {
            if (v is JsonElement je && je.ValueKind == JsonValueKind.String)
                return je.GetString() ?? "true";
            if (v is string s2) return s2;
        }
        return node.GetProperty<string>("condition") ?? "true";
    }
}

/// <summary>
/// Parallel strategy: conceptually activates all outgoing edges (runtime adaptation pending).
/// Currently just records intent.
/// </summary>
public class ParallelGatewayStrategy : IGatewayStrategy
{
    public string Kind => "parallel";

    public Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context)
    {
        var decision = new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = true,
            ChosenEdgeIds = Array.Empty<string>(),
            ShouldWait = false,
            Notes = "Parallel strategy selected (all outgoing edges intended)."
        };
        return Task.FromResult(decision);
    }
}
