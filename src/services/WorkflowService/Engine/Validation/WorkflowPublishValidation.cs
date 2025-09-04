using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Domain.Models;
using WorkflowService.Domain.Dsl;

namespace WorkflowService.Engine.Validation;

public interface IWorkflowPublishValidator
{
    /// <summary>
    /// Returns list of validation error messages. Empty list means success.
    /// </summary>
    IReadOnlyList<string> Validate(WorkflowDefinition definition, IEnumerable<WorkflowNode> nodes);
}

/// NOTE:
/// Made class public (was internal) so unit test project can instantiate it directly.
/// Alternative would be InternalsVisibleTo("WorkflowService.Tests"), but public keeps it simple.
/// No behavioral changes.
public sealed class WorkflowPublishValidator : IWorkflowPublishValidator
{
    private static readonly HashSet<string> AllowedFailurePolicies =
        new(StringComparer.OrdinalIgnoreCase) { "proceed", "suspend", "failInstance" };

    // Strategy validation parameters (for abTest)
    private const double AbTestExpectedTotal = 100.0;
    private const double AbTestWeightTolerance = 0.0001; // Accept 100 ± 0.0001
    private const int AbTestMinVariants = 2;

    private readonly ILogger<WorkflowPublishValidator> _logger;

    public WorkflowPublishValidator(ILogger<WorkflowPublishValidator> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<string> Validate(WorkflowDefinition definition, IEnumerable<WorkflowNode> nodes)
    {
        var errors = new List<string>();

        foreach (var n in nodes)
        {
            // AUTOMATIC NODE VALIDATION
            if (string.Equals(n.Type, NodeTypes.Automatic, StringComparison.OrdinalIgnoreCase))
            {
                ValidateAutomaticNode(n, errors);
            }

            // GATEWAY STRATEGY VALIDATION
            if (n.IsGateway())
            {
                TryValidateGatewayStrategies(n, errors);
            }
        }

        if (errors.Count > 0)
        {
            _logger.LogWarning("Workflow publish validation failed: {Count} error(s). DefinitionId={DefId}", errors.Count, definition.Id);
        }

        return errors;
    }

    #region Automatic Node Validation

    private void ValidateAutomaticNode(WorkflowNode n, List<string> errors)
    {
        if (!n.Properties.TryGetValue("action", out var actionRaw))
        {
            errors.Add(NodeErr(n, "Automatic node missing required 'action' object."));
            return;
        }

        JsonObject? actionObj = TryNormalizeToJsonObject(actionRaw);
        if (actionObj is null)
        {
            errors.Add(NodeErr(n, "'action' must be a JSON object."));
            return;
        }

        if (!(actionObj.TryGetPropertyValue("kind", out var kindNode) &&
              kindNode is JsonValue kv &&
              kv.TryGetValue<string>(out var kindStr) &&
              !string.IsNullOrWhiteSpace(kindStr)))
        {
            errors.Add(NodeErr(n, "Automatic node 'action.kind' is required and must be a non-empty string."));
            return;
        }

        var kind = kindStr.Trim();

        if (n.Properties.ContainsKey("actionType") || n.Properties.ContainsKey("executorType"))
        {
            errors.Add(NodeErr(n, "Legacy properties 'actionType' / 'executorType' are not allowed. Use 'action.kind'."));
        }

        if (string.Equals(kind, "webhook", StringComparison.OrdinalIgnoreCase))
        {
            ValidateWebhookConfig(n, actionObj, errors);
        }

        if (actionObj.TryGetPropertyValue("onFailure", out var polNode) &&
            polNode is JsonValue pv &&
            pv.TryGetValue<string>(out var polStr) &&
            !string.IsNullOrWhiteSpace(polStr))
        {
            if (!AllowedFailurePolicies.Contains(polStr.Trim()))
            {
                errors.Add(NodeErr(n, $"Unknown failure policy '{polStr}'. Allowed: proceed|suspend|failInstance"));
            }
        }
    }

    private void ValidateWebhookConfig(WorkflowNode n, JsonObject actionObj, List<string> errors)
    {
        if (!actionObj.TryGetPropertyValue("config", out var cfgNode))
        {
            errors.Add(NodeErr(n, "webhook action requires 'config' with 'url'."));
            return;
        }

        JsonObject? cfgObj = cfgNode as JsonObject
                              ?? (cfgNode is JsonValue v && v.TryGetValue<string>(out var cfgStr)
                                  ? TryParseObject(cfgStr)
                                  : null);

        if (cfgObj is null)
        {
            errors.Add(NodeErr(n, "webhook action config must be an object."));
            return;
        }

        if (!cfgObj.TryGetPropertyValue("url", out var urlNode) ||
            urlNode is not JsonValue uv ||
            !uv.TryGetValue<string>(out var urlStr) ||
            string.IsNullOrWhiteSpace(urlStr))
        {
            errors.Add(NodeErr(n, "webhook action requires config.url."));
            return;
        }

        if (!urlStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            errors.Add(NodeErr(n, $"webhook url must use https. Value='{urlStr}'."));
            return;
        }

        if (!Uri.TryCreate(urlStr, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps)
        {
            errors.Add(NodeErr(n, $"webhook url invalid or not https. Value='{urlStr}'."));
        }
    }

    #endregion

    #region Gateway Strategy Validation (abTest)

    private void TryValidateGatewayStrategies(WorkflowNode node, List<string> errors)
    {
        if (!TryExtractStrategy(node, out var strategyObj))
            return; // No strategy → skip

        if (!strategyObj.TryGetPropertyValue("kind", out var kindNode) ||
            kindNode is not JsonValue kv ||
            !kv.TryGetValue<string>(out var kindRaw) ||
            string.IsNullOrWhiteSpace(kindRaw))
        {
            errors.Add(NodeErr(node, "Gateway strategy object missing 'kind'."));
            return;
        }

        var kind = kindRaw.Trim();

        if (string.Equals(kind, "abTest", StringComparison.OrdinalIgnoreCase))
        {
            ValidateAbTestStrategy(node, strategyObj, errors);
        }
    }

    private void ValidateAbTestStrategy(WorkflowNode node, JsonObject strategyObj, List<string> errors)
    {
        if (!strategyObj.TryGetPropertyValue("config", out var cfgNode) || cfgNode is not JsonObject cfg)
        {
            errors.Add(NodeErr(node, "abTest strategy requires 'config' object."));
            return;
        }

        if (!cfg.TryGetPropertyValue("keyPath", out var kpNode) ||
            kpNode is not JsonValue kpVal ||
            !kpVal.TryGetValue<string>(out var keyPath) ||
            string.IsNullOrWhiteSpace(keyPath))
        {
            errors.Add(NodeErr(node, "abTest config.keyPath is required (JSON path to stable identity)."));
        }

        if (!cfg.TryGetPropertyValue("variants", out var variantsNode) ||
            variantsNode is not JsonArray variantsArr)
        {
            errors.Add(NodeErr(node, "abTest config.variants must be an array."));
            return;
        }

        if (variantsArr.Count < AbTestMinVariants)
        {
            errors.Add(NodeErr(node, $"abTest requires at least {AbTestMinVariants} variants."));
        }

        var seenTargets = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        double totalWeight = 0;
        var index = -1;

        foreach (var item in variantsArr)
        {
            index++;
            if (item is not JsonObject vObj)
            {
                errors.Add(NodeErr(node, $"abTest variants[{index}] must be an object."));
                continue;
            }

            if (!vObj.TryGetPropertyValue("target", out var tgtNode) ||
                tgtNode is not JsonValue tVal ||
                !tVal.TryGetValue<string>(out var target) ||
                string.IsNullOrWhiteSpace(target))
            {
                errors.Add(NodeErr(node, $"abTest variants[{index}].target is required."));
            }
            else
            {
                if (!seenTargets.Add(target.Trim()))
                {
                    errors.Add(NodeErr(node, $"abTest duplicate variant target '{target}' at index {index}."));
                }
            }

            if (!vObj.TryGetPropertyValue("weight", out var wNode))
            {
                errors.Add(NodeErr(node, $"abTest variants[{index}].weight is required."));
                continue;
            }

            double weightVal;
            if (wNode is JsonValue wVal)
            {
                if (wVal.TryGetValue<double>(out var d))
                    weightVal = d;
                else if (wVal.TryGetValue<int>(out var i))
                    weightVal = i;
                else if (wVal.TryGetValue<long>(out var l))
                    weightVal = l;
                else
                {
                    errors.Add(NodeErr(node, $"abTest variants[{index}].weight must be numeric."));
                    continue;
                }
            }
            else
            {
                errors.Add(NodeErr(node, $"abTest variants[{index}].weight must be numeric."));
                continue;
            }

            if (weightVal <= 0)
            {
                errors.Add(NodeErr(node, $"abTest variants[{index}].weight must be > 0."));
            }
            totalWeight += weightVal;
        }

        if (variantsArr.Count >= AbTestMinVariants)
        {
            if (Math.Abs(totalWeight - AbTestExpectedTotal) > AbTestWeightTolerance)
            {
                errors.Add(NodeErr(node,
                    $"abTest variant weights must sum to {AbTestExpectedTotal} (±{AbTestWeightTolerance}). Actual={totalWeight:0.####}."));
            }
        }
    }

    private bool TryExtractStrategy(WorkflowNode node, out JsonObject strategyObj)
    {
        strategyObj = null;

        if (node.Properties.TryGetValue("strategy", out var raw))
        {
            strategyObj = TryNormalizeToJsonObject(raw);
            return strategyObj != null;
        }

        foreach (var kv in node.Properties)
        {
            var obj = TryNormalizeToJsonObject(kv.Value);
            if (obj != null &&
                obj.TryGetPropertyValue("kind", out var kNode) &&
                kNode is JsonValue kvv &&
                kvv.TryGetValue<string>(out var kindStr) &&
                !string.IsNullOrWhiteSpace(kindStr))
            {
                strategyObj = obj;
                return true;
            }
        }

        return false;
    }

    #endregion

    #region Helpers

    private static string NodeErr(WorkflowNode n, string msg) =>
        $"Node '{n.Id}' ({n.Name ?? n.Type}): {msg}";

    private static JsonObject? TryNormalizeToJsonObject(object raw)
    {
        switch (raw)
        {
            case JsonObject jo:
                return jo;
            case JsonElement je when je.ValueKind == JsonValueKind.Object:
                return JsonNode.Parse(je.GetRawText()) as JsonObject;
            case string s:
                return TryParseObject(s);
            case IDictionary<string, object> dict:
            {
                var o = new JsonObject();
                foreach (var kv in dict)
                    o[kv.Key] = kv.Value is null ? null : JsonValue.Create(kv.Value);
                return o;
            }
            default:
                return null;
        }
    }

    private static JsonObject? TryParseObject(string json)
    {
        try
        {
            var node = JsonNode.Parse(json);
            return node as JsonObject;
        }
        catch
        {
            return null;
        }
    }

    #endregion
}

public static class WorkflowPublishValidationServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowPublishValidation(this IServiceCollection services)
    {
        services.AddSingleton<IWorkflowPublishValidator, WorkflowPublishValidator>();
        return services;
    }
}
