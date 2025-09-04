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
        // Expected: properties.strategy = { kind: "exclusive", config: {...} }
        if (node.Properties.TryGetValue("strategy", out var raw))
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
                else if (raw is string s && !string.IsNullOrWhiteSpace(s))
                {
                    return (s.Trim(), null);
                }
            }
            catch { /* fall through */ }
        }

        // Legacy gatewayType fallback -> exclusive
        var legacy = node.GetProperty<string>("gatewayType");
        if (!string.IsNullOrWhiteSpace(legacy))
            return (legacy!, null);

        return ("exclusive", null);
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
