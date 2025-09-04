// Only dictionary value nullability adjustments (remove warning CS8619)
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.Json;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Domain.Dsl;

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

        if (!_map.ContainsKey("exclusive"))
            _map.TryAdd("exclusive", new ExclusiveGatewayStrategy());

        if (!_map.ContainsKey("parallel"))
            _map.TryAdd("parallel", new ParallelGatewayStrategy());
    }

    public IGatewayStrategy Get(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind)) return _map["exclusive"];
        return _map.TryGetValue(kind.Trim(), out var s) ? s : _map["exclusive"];
    }

    public IReadOnlyCollection<string> RegisteredKinds => _map.Keys.ToList().AsReadOnly();
}

public class ExclusiveGatewayStrategy : IGatewayStrategy
{
    public string Kind => "exclusive";

    public async Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context)
    {
        var sw = Stopwatch.StartNew();
        var edges = context.OutgoingEdges;
        if (edges.Count == 0)
        {
            sw.Stop();
            return new GatewayStrategyDecision
            {
                StrategyKind = Kind,
                ConditionResult = true,
                Diagnostics = new Dictionary<string, object>
                {
                    ["edgeCount"] = 0,
                    ["classification"] = new Dictionary<string, object>
                    {
                        ["true"] = Array.Empty<string>(),
                        ["false"] = Array.Empty<string>(),
                        ["else"] = Array.Empty<string>(),
                        ["unlabeled"] = Array.Empty<string>()
                    }
                },
                ElapsedMs = sw.Elapsed.TotalMilliseconds
            };
        }

        var conditionJson = ExtractCondition(context.Node);
        bool conditionResult = true;

        if (!string.IsNullOrWhiteSpace(conditionJson) &&
            !string.Equals(conditionJson, "true", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                conditionResult = await context.ConditionEvaluator
                    .EvaluateAsync(conditionJson, context.CurrentContextJson);
            }
            catch
            {
                conditionResult = true;
            }
        }

        foreach (var e in edges) e.InferLabelIfMissing();

        string Normalize(string? raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
            raw = raw.Trim().ToLowerInvariant();
            return raw switch
            {
                "yes" => "true",
                "no" => "false",
                "default" => "else",
                _ => raw
            };
        }

        var classified = edges.Select(e =>
        {
            var label = Normalize(e.Label);
            if (string.IsNullOrEmpty(label))
            {
                var idLower = e.Id?.ToLowerInvariant() ?? "";
                if (idLower.Contains("true")) label = "true";
                else if (idLower.Contains("false")) label = "false";
                else if (idLower.Contains("else")) label = "else";
            }
            return new
            {
                Edge = e,
                Label = label,
                Target = (e.Target ?? e.To ?? e.EffectiveTarget ?? "").Trim(),
                EdgeId = e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}"
            };
        }).ToList();

        var trueEdges = classified.Where(c => c.Label == "true").ToList();
        var falseEdges = classified.Where(c => c.Label == "false").ToList();
        var elseEdges = classified.Where(c => c.Label == "else").ToList();
        var unlabeledEdges = classified.Where(c => string.IsNullOrEmpty(c.Label)).ToList();

        List<WorkflowEdge> chosen;
        if (conditionResult)
        {
            if (trueEdges.Any()) chosen = trueEdges.Select(c => c.Edge).ToList();
            else if (elseEdges.Any()) chosen = elseEdges.Select(c => c.Edge).ToList();
            else if (unlabeledEdges.Any()) chosen = new List<WorkflowEdge> { unlabeledEdges[0].Edge };
            else chosen = new List<WorkflowEdge>();
        }
        else
        {
            if (falseEdges.Any()) chosen = falseEdges.Select(c => c.Edge).ToList();
            else if (elseEdges.Any()) chosen = elseEdges.Select(c => c.Edge).ToList();
            else if (unlabeledEdges.Count >= 2) chosen = new List<WorkflowEdge> { unlabeledEdges[1].Edge };
            else chosen = new List<WorkflowEdge>();
        }

        var chosenIds = chosen
            .Select(e => e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var targets = chosen
            .Select(e => (e.Target ?? e.To ?? e.EffectiveTarget ?? "").Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var classification = new Dictionary<string, object>
        {
            ["true"] = trueEdges.Select(c => c.EdgeId).ToArray(),
            ["false"] = falseEdges.Select(c => c.EdgeId).ToArray(),
            ["else"] = elseEdges.Select(c => c.EdgeId).ToArray(),
            ["unlabeled"] = unlabeledEdges.Select(c => c.EdgeId).ToArray()
        };

        var outgoingSnapshot = edges.Select(e => new
        {
            edgeId = e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}",
            from = e.Source ?? e.From ?? e.EffectiveSource,
            to = e.Target ?? e.To ?? e.EffectiveTarget,
            inferredLabel = classified.First(c => c.Edge == e).Label
        }).ToList();

        sw.Stop();

        return new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = conditionResult,
            ChosenEdgeIds = chosenIds,
            SelectedTargetNodeIds = targets,
            Diagnostics = new Dictionary<string, object>
            {
                ["condition"] = conditionJson,
                ["outgoingEdgeCount"] = edges.Count,
                ["classification"] = classification,
                ["outgoingSnapshot"] = outgoingSnapshot,
                ["chosenEdgeIds"] = chosenIds,
                ["skippedEdgeIds"] = outgoingSnapshot.Select(o => (string)o.edgeId)
                    .Where(id => !chosenIds.Contains(id, StringComparer.OrdinalIgnoreCase))
                    .ToArray()
            },
            ElapsedMs = sw.Elapsed.TotalMilliseconds
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

public class ParallelGatewayStrategy : IGatewayStrategy
{
    public string Kind => "parallel";

    public Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context)
    {
        var edges = context.OutgoingEdges;

        var edgeIds = edges
            .Select(e => e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var targets = edges
            .Select(e => (e.Target ?? e.To ?? e.EffectiveTarget ?? "").Trim())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var snapshot = edges.Select(e => new
        {
            edgeId = e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}",
            from = e.Source ?? e.From ?? e.EffectiveSource,
            to = e.Target ?? e.To ?? e.EffectiveTarget
        }).ToList();

        var decision = new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = true,
            ChosenEdgeIds = edgeIds,
            SelectedTargetNodeIds = targets,
            Notes = "Parallel strategy selected",
            Diagnostics = new Dictionary<string, object>
            {
                ["outgoingEdgeCount"] = edges.Count,
                ["outgoingSnapshot"] = snapshot,
                ["chosenEdgeIds"] = edgeIds,
                ["skippedEdgeIds"] = Array.Empty<string>(),
                ["classification"] = new Dictionary<string, object>
                {
                    ["true"] = Array.Empty<string>(),
                    ["false"] = Array.Empty<string>(),
                    ["else"] = Array.Empty<string>(),
                    ["unlabeled"] = edgeIds.ToArray()
                }
            },
            ElapsedMs = 0
        };

        return Task.FromResult(decision);
    }
}
