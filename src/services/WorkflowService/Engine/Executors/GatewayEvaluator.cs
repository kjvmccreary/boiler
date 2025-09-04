using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Engine.Gateways;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Delegating gateway executor using strategy abstraction.
/// </summary>
public class GatewayEvaluator : INodeExecutor
{
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ILogger<GatewayEvaluator> _logger;
    private readonly IGatewayStrategyRegistry _strategyRegistry;

    public string NodeType => NodeTypes.Gateway;

    public GatewayEvaluator(
        IConditionEvaluator conditionEvaluator,
        ILogger<GatewayEvaluator> logger,
        IGatewayStrategyRegistry strategyRegistry)
    {
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
        _strategyRegistry = strategyRegistry;
    }

    public bool CanExecute(WorkflowNode node) => node.IsGateway();

    public async Task<NodeExecutionResult> ExecuteAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        string context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var (kind, cfgNode) = ExtractStrategy(node);
            var strategy = _strategyRegistry.Get(kind);

            _logger.LogInformation("GATEWAY_EXEC Node={NodeId} Strategy={Strategy}", node.Id, strategy.Kind);

            var gCtx = new GatewayStrategyContext(
                node,
                instance,
                context,
                cfgNode,
                _conditionEvaluator,
                cancellationToken);

            var decision = await strategy.EvaluateAsync(gCtx);

            var updatedContext = RecordDecision(context, node.Id, decision);

            return new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = decision.ShouldWait,
                // Leave traversal to runtime for now; future: use decision.ChosenEdgeIds
                NextNodeIds = new List<string>(),
                UpdatedContext = updatedContext
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GATEWAY_EXEC_FAIL Node={NodeId}", node.Id);
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private (string strategyKind, JsonObject? cfg) ExtractStrategy(WorkflowNode node)
    {
        // Expected: properties.strategy = { kind: "...", config:{...} }
        // But be defensive: case-insensitive key, dictionary forms, JsonObject, fallback scan.
        if (TryGetRawStrategyObject(node.Properties, out var raw))
        {
            try
            {
                if (raw is JsonElement el && el.ValueKind == JsonValueKind.Object)
                {
                    var obj = JsonNode.Parse(el.GetRawText()) as JsonObject;
                    if (obj != null)
                    {
                        var kind = obj.TryGetPropertyValue("kind", out var kNode) &&
                                   kNode is JsonValue kv &&
                                   kv.TryGetValue<string>(out var kStr) &&
                                   !string.IsNullOrWhiteSpace(kStr)
                            ? kStr.Trim()
                            : "exclusive";

                        JsonObject? cfg = null;
                        if (obj.TryGetPropertyValue("config", out var cfgNode) && cfgNode is JsonObject cObj)
                            cfg = cObj;
                        _logger.LogDebug("GATEWAY_STRATEGY_DETECTED Node={NodeId} Kind={Kind} (JsonElement)", node.Id, kind);
                        return (kind, cfg);
                    }
                }
                if (raw is IDictionary<string, object> dict)
                {
                    var (kind, cfgObj) = FromDictionary(dict);
                    _logger.LogDebug("GATEWAY_STRATEGY_DETECTED Node={NodeId} Kind={Kind} (Dictionary)", node.Id, kind);
                    return (kind, cfgObj);
                }
                if (raw is JsonObject jo)
                {
                    var (kind, cfgObj) = FromJsonObject(jo);
                    _logger.LogDebug("GATEWAY_STRATEGY_DETECTED Node={NodeId} Kind={Kind} (JsonObject)", node.Id, kind);
                    return (kind, cfgObj);
                }
                if (raw is string s && !string.IsNullOrWhiteSpace(s))
                {
                    _logger.LogDebug("GATEWAY_STRATEGY_DETECTED Node={NodeId} Kind={Kind} (String)", node.Id, s.Trim());
                    return (s.Trim(), null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "GATEWAY_STRATEGY_PARSE_FAIL Node={NodeId}", node.Id);
            }
        }

        // Legacy gatewayType fallback -> exclusive
        var legacy = node.GetProperty<string>("gatewayType");
        if (!string.IsNullOrWhiteSpace(legacy))
        {
            _logger.LogDebug("GATEWAY_STRATEGY_FALLBACK_LEGACY Node={NodeId} Kind={Kind}", node.Id, legacy);
            return (legacy!, null);
        }

        _logger.LogDebug("GATEWAY_STRATEGY_DEFAULT Node={NodeId} Kind=exclusive", node.Id);
        return ("exclusive", null);
    }

    private static bool TryGetRawStrategyObject(
        IDictionary<string, object> props,
        out object? raw)
    {
        // Direct, case-insensitive
        raw = null;
        var direct = props
            .FirstOrDefault(kv => kv.Key.Equals("strategy", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(direct.Key))
        {
            raw = direct.Value;
            return true;
        }

        // Fallback scan: any property whose value is object/dictionary with a "kind" key
        foreach (var kv in props)
        {
            if (kv.Value is IDictionary<string, object> dict &&
                dict.Keys.Any(k => k.Equals("kind", StringComparison.OrdinalIgnoreCase)))
            {
                raw = kv.Value;
                return true;
            }
            if (kv.Value is JsonElement je && je.ValueKind == JsonValueKind.Object)
            {
                try
                {
                    using var doc = JsonDocument.Parse(je.GetRawText());
                    if (doc.RootElement.TryGetProperty("kind", out _))
                    {
                        raw = kv.Value;
                        return true;
                    }
                }
                catch { }
            }
        }
        return false;
    }

    private static (string kind, JsonObject? cfg) FromDictionary(IDictionary<string, object> dict)
    {
        var kind = dict.TryGetValue("kind", out var kv) && kv is string ks && !string.IsNullOrWhiteSpace(ks)
            ? ks.Trim()
            : "exclusive";
        JsonObject? cfgObj = null;
        if (dict.TryGetValue("config", out var cv) && cv is IDictionary<string, object> cfgDict)
        {
            cfgObj = new JsonObject();
            foreach (var c in cfgDict)
                cfgObj[c.Key] = c.Value is null ? null : JsonValue.Create(c.Value);
        }
        return (kind, cfgObj);
    }

    private static (string kind, JsonObject? cfg) FromJsonObject(JsonObject jo)
    {
        string kind = "exclusive";
        if (jo.TryGetPropertyValue("kind", out var kNode) &&
            kNode is JsonValue kv &&
            kv.TryGetValue<string>(out var ks) &&
            !string.IsNullOrWhiteSpace(ks))
            kind = ks.Trim();
        JsonObject? cfg = null;
        if (jo.TryGetPropertyValue("config", out var cNode) && cNode is JsonObject cJo)
            cfg = cJo;
        return (kind, cfg);
    }

    private string RecordDecision(string currentContextJson, string nodeId, GatewayStrategyDecision decision)
    {
        JsonObject root;
        try
        {
            root = JsonNode.Parse(currentContextJson) as JsonObject ?? new JsonObject();
        }
        catch
        {
            root = new JsonObject();
        }

        if (root["_gatewayDecisions"] is not JsonObject decisionsObj)
        {
            decisionsObj = new JsonObject();
            root["_gatewayDecisions"] = decisionsObj;
        }

        decisionsObj[nodeId] = new JsonObject
        {
            ["strategy"] = decision.StrategyKind,
            ["conditionResult"] = decision.ConditionResult,
            ["chosenEdgeIds"] = new JsonArray(decision.ChosenEdgeIds.Select(e => (JsonNode?)e).ToArray()),
            ["shouldWait"] = decision.ShouldWait,
            ["notes"] = decision.Notes,
            ["evaluatedAtUtc"] = DateTime.UtcNow.ToString("O")
        };

        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    }
}
