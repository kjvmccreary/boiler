using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Gateway node executor.
/// Delegates branch selection to strategies and enriches workflow context with
/// a detailed, append-only decision history for diagnostics & replay.
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
            var (strategyKind, cfgNode) = ExtractStrategy(node);
            var strategy = _strategyRegistry.Get(strategyKind);

            var workflowJson = instance.WorkflowDefinition?.JSONDefinition;
            if (string.IsNullOrWhiteSpace(workflowJson))
                throw new InvalidOperationException("Workflow definition JSON not available in instance.");

            var def = BuilderDefinitionAdapter.Parse(workflowJson);
            var outgoingEdges = def.Edges
                .Where(e =>
                    (e.Source?.Equals(node.Id, StringComparison.OrdinalIgnoreCase) == true) ||
                    (e.From?.Equals(node.Id, StringComparison.OrdinalIgnoreCase) == true) ||
                    (e.EffectiveSource?.Equals(node.Id, StringComparison.OrdinalIgnoreCase) == true))
                .ToList();

            var gwCtx = new GatewayStrategyContext(
                node,
                instance,
                outgoingEdges,
                context,
                _conditionEvaluator,
                cancellationToken);

            _logger.LogInformation("GATEWAY_EVAL_START Node={NodeId} Strategy={Strategy} Outgoing={Count}",
                node.Id, strategy.Kind, outgoingEdges.Count);

            var sw = Stopwatch.StartNew();
            var decision = await strategy.EvaluateAsync(gwCtx);
            sw.Stop();

            // If strategy did not set elapsed we record wrapper time
            if (decision.ElapsedMs <= 0 && decision.Diagnostics is IDictionary<string, object> diag)
            {
                diag["wrapperElapsedMs"] = sw.Elapsed.TotalMilliseconds;
            }

            var updatedContext = RecordDecision(context, node.Id, decision);

            _logger.LogInformation("GATEWAY_EVAL_DONE Node={NodeId} Strategy={Strategy} Targets=[{Targets}] DecisionId={DecisionId}",
                node.Id, decision.StrategyKind, string.Join(",", decision.SelectedTargetNodeIds), decision.DecisionId);

            return new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = decision.ShouldWait,
                NextNodeIds = decision.SelectedTargetNodeIds.ToList(),
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

    #region Strategy Extraction

    private (string strategyKind, JsonObject? cfg) ExtractStrategy(WorkflowNode node)
    {
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
                        return (kind, cfg);
                    }
                }
                if (raw is IDictionary<string, object> dict)
                {
                    var (kind, cfgObj) = FromDictionary(dict);
                    return (kind, cfgObj);
                }
                if (raw is JsonObject jo)
                {
                    var (kind, cfgObj) = FromJsonObject(jo);
                    return (kind, cfgObj);
                }
                if (raw is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return (s.Trim(), null);
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "GATEWAY_STRATEGY_PARSE_FAIL Node={NodeId}", node.Id);
            }
        }

        // Legacy
        var legacy = node.GetProperty<string>("gatewayType");
        if (!string.IsNullOrWhiteSpace(legacy))
            return (legacy!, null);

        return ("exclusive", null);
    }

    private static bool TryGetRawStrategyObject(
        IDictionary<string, object> props,
        out object? raw)
    {
        raw = null;
        var direct = props
            .FirstOrDefault(kv => kv.Key.Equals("strategy", StringComparison.OrdinalIgnoreCase));
        if (!string.IsNullOrEmpty(direct.Key))
        {
            raw = direct.Value;
            return true;
        }

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

    #endregion

    #region Context Recording (Enriched History)

    private string RecordDecision(
        string currentContextJson,
        string nodeId,
        GatewayStrategyDecision decision)
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

        // Gateway decisions stored as array (append-only). Support legacy object -> convert to array.
        if (decisionsObj[nodeId] is JsonObject legacyObj)
        {
            var arr = new JsonArray { legacyObj };
            decisionsObj[nodeId] = arr;
        }

        if (decisionsObj[nodeId] is not JsonArray history)
        {
            history = new JsonArray();
            decisionsObj[nodeId] = history;
        }

        var decisionObj = new JsonObject
        {
            ["decisionId"] = decision.DecisionId.ToString(),
            ["strategy"] = decision.StrategyKind,
            ["conditionResult"] = decision.ConditionResult,
            ["chosenEdgeIds"] = new JsonArray(decision.ChosenEdgeIds.Select(e => (JsonNode?)e).ToArray()),
            ["selectedTargets"] = new JsonArray(decision.SelectedTargetNodeIds.Select(t => (JsonNode?)t).ToArray()),
            ["shouldWait"] = decision.ShouldWait,
            ["elapsedMs"] = decision.ElapsedMs,
            ["notes"] = decision.Notes,
            ["evaluatedAtUtc"] = DateTime.UtcNow.ToString("O")
        };

        if (decision.Diagnostics is { Count: > 0 })
        {
            var diag = new JsonObject();
            foreach (var kv in decision.Diagnostics)
            {
                diag[kv.Key] = kv.Value switch
                {
                    null => null,
                    string s => s,
                    Guid g => g.ToString(),
                    int i => i,
                    long l => l,
                    double d => d,
                    bool b => b,
                    IEnumerable<string> list => new JsonArray(list.Select(x => (JsonNode?)x).ToArray()),
                    IEnumerable<object> objList => new JsonArray(objList.Select(o => JsonValue.Create(o?.ToString())).ToArray()),
                    _ => JsonValue.Create(kv.Value.ToString())
                };
            }
            decisionObj["diagnostics"] = diag;
        }

        history.Add(decisionObj);

        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = false });
    }

    #endregion
}
