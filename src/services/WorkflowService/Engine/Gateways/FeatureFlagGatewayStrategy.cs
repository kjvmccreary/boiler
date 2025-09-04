using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Dsl;
using WorkflowService.Engine.FeatureFlags;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Gateways;

/// <summary>
/// Feature flag gateway strategy (A2).
/// Config shape:
/// {
///   "kind":"featureFlag",
///   "config":{
///      "flag":"newFlowX",
///      "onTarget":"nodeA",
///      "offTarget":"nodeB",
///      "required": true
///   }
/// }
/// Behavior:
///  - Evaluate provider.IsEnabledAsync(flag)
///  - If true -> onTarget ; else offTarget
///  - If provider throws & required=true => fallback to offTarget + emit FeatureFlagFallback event
/// Diagnostics:
///   flag, flagEnabled, required, providerError(bool), chosenTarget, evaluationMs, fallback(bool)
/// </summary>
public sealed class FeatureFlagGatewayStrategy : IGatewayStrategy
{
    public string Kind => "featureFlag";

    private readonly IFeatureFlagProvider _provider;
    private readonly IFeatureFlagFallbackEmitter _fallbackEmitter;
    private readonly ILogger<FeatureFlagGatewayStrategy> _logger;

    public FeatureFlagGatewayStrategy(
        IFeatureFlagProvider provider,
        IFeatureFlagFallbackEmitter fallbackEmitter,
        ILogger<FeatureFlagGatewayStrategy> logger)
    {
        _provider = provider;
        _fallbackEmitter = fallbackEmitter;
        _logger = logger;
    }

    public async Task<GatewayStrategyDecision> EvaluateAsync(GatewayStrategyContext context)
    {
        var cfg = TryGetConfig(context.Node);
        if (cfg == null)
        {
            return Decision("featureFlag config missing", context, null, null, providerError: true, fallback: true);
        }

        var flag = ReadString(cfg, "flag");
        var onTarget = ReadString(cfg, "onTarget");
        var offTarget = ReadString(cfg, "offTarget");
        var required = ReadBool(cfg, "required", defaultValue: true);

        if (string.IsNullOrWhiteSpace(flag))
        {
            return Decision("flag missing", context, null, null, providerError: true, fallback: true);
        }

        bool? enabled = null;
        bool providerError = false;
        bool fallback = false;
        string? errorReason = null;

        try
        {
            enabled = await _provider.IsEnabledAsync(flag, context.CancellationToken);
        }
        catch (Exception ex)
        {
            providerError = true;
            if (required)
            {
                fallback = true;
                errorReason = ex.Message;
                _fallbackEmitter.EmitFallback(context.Instance, context.Node.Id ?? "", flag, ex.Message, required);
                _logger.LogWarning(ex, "Feature flag provider error (flag={Flag}) - fallback to offTarget", flag);
            }
            else
            {
                enabled = false; // treat as off if not required
            }
        }

        string? chosenTarget = null;
        if (enabled == true)
            chosenTarget = onTarget;
        else
            chosenTarget = offTarget;

        // Build mapping to edges
        var chosenTargets = string.IsNullOrWhiteSpace(chosenTarget)
            ? Array.Empty<string>()
            : new[] { chosenTarget };

        var chosenEdges = context.OutgoingEdges
            .Where(e => !string.IsNullOrWhiteSpace(chosenTarget) &&
                        string.Equals(e.Target ?? e.To ?? e.EffectiveTarget, chosenTarget,
                            StringComparison.OrdinalIgnoreCase))
            .Select(e => e.Id ?? $"{e.Source ?? e.From}->{e.Target ?? e.To}")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList()
            .AsReadOnly();

        var diagnostics = new Dictionary<string, object>
        {
            ["flag"] = flag,
            ["required"] = required,
            ["flagEnabled"] = enabled,
            ["providerError"] = providerError,
            ["fallback"] = fallback,
            ["chosenTarget"] = chosenTarget
        };
        if (errorReason != null) diagnostics["errorReason"] = errorReason;

        return new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = true,
            SelectedTargetNodeIds = chosenTargets,
            ChosenEdgeIds = chosenEdges,
            ShouldWait = false,
            Diagnostics = diagnostics,
            Notes = fallback ? "featureFlag fallback path" : "featureFlag evaluation"
        };
    }

    #region Helpers

    private static JsonObject? TryGetConfig(WorkflowNode node)
    {
        if (!node.Properties.TryGetValue("strategy", out var raw)) return null;

        JsonObject? stratObj = raw switch
        {
            JsonObject jo => jo,
            JsonElement je when je.ValueKind == JsonValueKind.Object => JsonNode.Parse(je.GetRawText()) as JsonObject,
            string s => SafeParse(s),
            _ => null
        };
        if (stratObj == null) return null;
        if (!stratObj.TryGetPropertyValue("config", out var cfgNode) || cfgNode is not JsonObject cfg)
            return null;
        return cfg;
    }

    private static JsonObject? SafeParse(string json)
    {
        try { return JsonNode.Parse(json) as JsonObject; } catch { return null; }
    }

    private static string? ReadString(JsonObject obj, string prop)
    {
        if (!obj.TryGetPropertyValue(prop, out var v)) return null;
        if (v is JsonValue jv && jv.TryGetValue<string>(out var s) && !string.IsNullOrWhiteSpace(s))
            return s.Trim();
        return null;
    }

    private static bool ReadBool(JsonObject obj, string prop, bool defaultValue)
    {
        if (!obj.TryGetPropertyValue(prop, out var v)) return defaultValue;
        if (v is JsonValue jv)
        {
            if (jv.TryGetValue<bool>(out var b)) return b;
            if (jv.TryGetValue<string>(out var s))
            {
                if (bool.TryParse(s, out var pb)) return pb;
            }
        }
        return defaultValue;
    }

    private GatewayStrategyDecision Decision(
        string reason,
        GatewayStrategyContext ctx,
        string? chosenTarget,
        IReadOnlyList<string>? chosenEdges,
        bool providerError,
        bool fallback)
    {
        var diag = new Dictionary<string, object>
        {
            ["flag"] = null!,
            ["required"] = true,
            ["flagEnabled"] = null!,
            ["providerError"] = providerError,
            ["fallback"] = fallback,
            ["chosenTarget"] = chosenTarget,
            ["errorReason"] = reason
        };
        return new GatewayStrategyDecision
        {
            StrategyKind = Kind,
            ConditionResult = true,
            SelectedTargetNodeIds = chosenTarget == null ? Array.Empty<string>() : new[] { chosenTarget },
            ChosenEdgeIds = chosenEdges ?? Array.Empty<string>(),
            ShouldWait = false,
            Diagnostics = diag,
            Notes = "featureFlag evaluation error"
        };
    }

    #endregion
}
