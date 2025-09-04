using System.Text.Json;
using System.Text.Json.Nodes;
using Contracts.Services;
using WorkflowService.Domain.Dsl;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Gateways;

public sealed class AbTestGatewayStrategy : IGatewayStrategy
{
    public string Kind => "abTest";

    private readonly IDeterministicHasher _hasher;
    private readonly ILogger<AbTestGatewayStrategy> _logger;

    public AbTestGatewayStrategy(
        IDeterministicHasher hasher,
        ILogger<AbTestGatewayStrategy> logger)
    {
        _hasher = hasher;
        _logger = logger;
    }

    public Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context)
    {
        // 1. Attempt snapshot reuse BEFORE any hashing
        if (TryReuseSnapshot(context, out var reusedDecision))
        {
            return Task.FromResult(reusedDecision);
        }

        // 2. Normal fresh evaluation
        var configObj = TryGetStrategyConfig(context.Node);
        if (configObj is null)
            return Task.FromResult(Fallback("Missing config", context));

        var keyPath = ReadString(configObj, "keyPath") ?? "id";
        var variantsArr = configObj["variants"] as JsonArray;
        if (variantsArr == null || variantsArr.Count == 0)
            return Task.FromResult(Fallback("No variants", context));

        var variants = new List<VariantRow>(variantsArr.Count);
        double totalWeightRaw = 0;
        foreach (var v in variantsArr)
        {
            if (v is not JsonObject vo) continue;
            var target = ReadString(vo, "target");
            var weight = ReadDouble(vo, "weight");
            if (string.IsNullOrWhiteSpace(target) || weight <= 0) continue;
            totalWeightRaw += weight;
            variants.Add(new VariantRow(target.Trim(), weight));
        }
        if (variants.Count == 0)
            return Task.FromResult(Fallback("All variants invalid", context));

        double cumulative = 0;
        foreach (var vr in variants)
        {
            cumulative += vr.Weight;
            vr.Cumulative = cumulative;
        }

        var keyValue = ExtractKeyValue(context.CurrentContextJson, keyPath) ?? "∅";

        var compositeHash = _hasher.HashComposite(
            keyValue,
            context.Node.Id ?? "",
            context.Instance.DefinitionVersion.ToString());

        const int HighResolution = 1_000_000;
        var bucket = (int)(compositeHash % (ulong)HighResolution);
        var normalized = (bucket / (double)HighResolution) * totalWeightRaw;

        VariantRow? chosen = null;
        for (int i = 0; i < variants.Count; i++)
        {
            if (normalized < variants[i].Cumulative)
            {
                chosen = variants[i];
                break;
            }
        }
        chosen ??= variants[^1];

        var chosenTargets = new List<string> { chosen.Target };
        var chosenEdges = context.OutgoingEdges
            .Where(e => string.Equals(e.Target ?? e.To ?? e.EffectiveTarget,
                                      chosen.Target, StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var table = variants.Select(v => new
        {
            target = v.Target,
            weight = v.Weight,
            cumulative = v.Cumulative
        }).ToList();

        var snapshotObj = new
        {
            variant = chosen.Target,
            assignedHash = compositeHash.ToString("X16"),
            keyValue
        };

        var diagnostics = new Dictionary<string, object>
        {
            ["keyPath"] = keyPath,
            ["keyValue"] = keyValue,
            ["hash"] = compositeHash,
            ["normalizedRoll"] = normalized,
            ["variantTarget"] = chosen.Target,
            ["variantWeight"] = chosen.Weight,
            ["variantTable"] = table,
            ["selectionIndex"] = variants.IndexOf(chosen),
            ["experimentSnapshot"] = snapshotObj
        };

        return Task.FromResult(new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = true,
            ChosenEdgeIds = chosenEdges,
            SelectedTargetNodeIds = chosenTargets,
            ShouldWait = false,
            Diagnostics = diagnostics,
            Notes = "abTest selection",
            ElapsedMs = 0
        });
    }

    #region Snapshot Reuse

    private bool TryReuseSnapshot(GatewayStrategyContext context, out GatewayStrategyDecision decision)
    {
        decision = default!;
        try
        {
            var root = JsonNode.Parse(context.CurrentContextJson) as JsonObject;
            if (root?["_experiments"] is not JsonObject exps)
                return false;

            var gwId = context.Node.Id;
            if (gwId == null || !exps.TryGetPropertyValue(gwId, out var gwNode) || gwNode is not JsonObject snap)
                return false;

            if (!snap.TryGetPropertyValue("variant", out var vNode) ||
                vNode is not JsonValue vv ||
                !vv.TryGetValue<string>(out var variant) ||
                string.IsNullOrWhiteSpace(variant))
                return false;

            // Ensure variant still maps to at least one outgoing edge target; else recompute
            var targetExists = context.OutgoingEdges.Any(e =>
                string.Equals(e.Target ?? e.To ?? e.EffectiveTarget,
                              variant, StringComparison.OrdinalIgnoreCase));

            if (!targetExists)
                return false;

            // Collect edges for this variant
            var chosenEdges = context.OutgoingEdges
                .Where(e => string.Equals(e.Target ?? e.To ?? e.EffectiveTarget,
                                          variant, StringComparison.OrdinalIgnoreCase))
                .Select(e => e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            var chosenTargets = new List<string> { variant };

            var diagnostics = new Dictionary<string, object>
            {
                ["snapshotReuse"] = true,
                ["variantTarget"] = variant
            };

            if (snap.TryGetPropertyValue("assignedHash", out var hNode) &&
                hNode is JsonValue hv &&
                hv.TryGetValue<string>(out var hStr))
            {
                diagnostics["hash"] = hStr;
            }
            if (snap.TryGetPropertyValue("keyValue", out var kNode) &&
                kNode is JsonValue kv &&
                kv.TryGetValue<string>(out var keyVal))
            {
                diagnostics["keyValue"] = keyVal;
            }

            decision = new GatewayStrategyDecision
            {
                StrategyKind = Kind,
                ConditionResult = true,
                SelectedTargetNodeIds = chosenTargets,
                ChosenEdgeIds = chosenEdges,
                ShouldWait = false,
                Diagnostics = diagnostics,
                Notes = "abTest snapshot reuse"
            };
            return true;
        }
        catch
        {
            return false;
        }
    }

    #endregion

    #region Helpers

    private static JsonObject? TryGetStrategyConfig(WorkflowNode node)
    {
        if (!node.Properties.TryGetValue("strategy", out var raw)) return null;
        JsonObject? stratObj = raw switch
        {
            JsonObject jo => jo,
            JsonElement je when je.ValueKind == JsonValueKind.Object => JsonNode.Parse(je.GetRawText()) as JsonObject,
            string s => SafeParseObj(s),
            _ => null
        };
        if (stratObj == null) return null;
        if (!stratObj.TryGetPropertyValue("config", out var cfg) || cfg is not JsonObject cfgObj)
            return null;
        return cfgObj;
    }

    private static JsonObject? SafeParseObj(string json)
    {
        try { return JsonNode.Parse(json) as JsonObject; }
        catch { return null; }
    }

    private static string? ReadString(JsonObject obj, string prop)
    {
        if (!obj.TryGetPropertyValue(prop, out var val)) return null;
        if (val is JsonValue jv && jv.TryGetValue<string>(out var s) && !string.IsNullOrWhiteSpace(s))
            return s;
        return null;
    }

    private static double ReadDouble(JsonObject obj, string prop)
    {
        if (!obj.TryGetPropertyValue(prop, out var val)) return 0;
        if (val is JsonValue jv)
        {
            if (jv.TryGetValue<double>(out var d)) return d;
            if (jv.TryGetValue<int>(out var i)) return i;
            if (jv.TryGetValue<long>(out var l)) return l;
        }
        return 0;
    }

    private static string? ExtractKeyValue(string contextJson, string keyPath)
    {
        if (string.IsNullOrWhiteSpace(contextJson) || string.IsNullOrWhiteSpace(keyPath))
            return null;

        try
        {
            var node = JsonNode.Parse(contextJson) as JsonObject;
            if (node == null) return null;

            var segments = keyPath.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            JsonNode? current = node;
            foreach (var seg in segments)
            {
                if (current is JsonObject o)
                {
                    if (!o.TryGetPropertyValue(seg, out current))
                        return null;
                }
                else return null;
            }

            if (current is JsonValue v)
            {
                if (v.TryGetValue<string>(out var s)) return s;
                if (v.TryGetValue<long>(out var ln)) return ln.ToString();
                if (v.TryGetValue<double>(out var d)) return d.ToString("R", System.Globalization.CultureInfo.InvariantCulture);
                if (v.TryGetValue<bool>(out var b)) return b ? "true" : "false";
            }
            else if (current != null)
            {
                return current.ToJsonString();
            }
            return null;
        }
        catch { return null; }
    }

    private GatewayStrategyDecision Fallback(string reason, GatewayStrategyContext ctx)
    {
        _logger.LogDebug("abTest fallback for node {NodeId}: {Reason}", ctx.Node.Id, reason);
        return new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = true,
            SelectedTargetNodeIds = Array.Empty<string>(),
            ChosenEdgeIds = Array.Empty<string>(),
            Diagnostics = new Dictionary<string, object>
            {
                ["fallback"] = true,
                ["reason"] = reason
            },
            Notes = "abTest fallback"
        };
    }

    private sealed class VariantRow
    {
        public string Target { get; }
        public double Weight { get; }
        public double Cumulative { get; set; }
        public VariantRow(string target, double weight) { Target = target; Weight = weight; }
    }

    #endregion
}
