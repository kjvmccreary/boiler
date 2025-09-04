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

internal sealed class WorkflowPublishValidator : IWorkflowPublishValidator
{
    private static readonly HashSet<string> AllowedFailurePolicies =
        new(StringComparer.OrdinalIgnoreCase) { "proceed", "suspend", "failInstance" };

    private readonly ILogger<WorkflowPublishValidator> _logger;

    public WorkflowPublishValidator(ILogger<WorkflowPublishValidator> logger)
    {
        _logger = logger;
    }

    public IReadOnlyList<string> Validate(WorkflowDefinition definition, IEnumerable<WorkflowNode> nodes)
    {
        var errors = new List<string>();
        int idx = 0;

        foreach (var n in nodes)
        {
            idx++;

            if (!string.Equals(n.Type, NodeTypes.Automatic, StringComparison.OrdinalIgnoreCase))
                continue;

            // Expect unified "action" object present as JsonElement/Object with required 'kind'
            if (!n.Properties.TryGetValue("action", out var actionRaw))
            {
                errors.Add(NodeErr(n, "Automatic node missing required 'action' object."));
                continue;
            }

            // Normalize to JsonObject / JsonNode
            JsonObject? actionObj = TryNormalizeToJsonObject(actionRaw);
            if (actionObj is null)
            {
                errors.Add(NodeErr(n, "'action' must be a JSON object."));
                continue;
            }

            if (actionObj.TryGetPropertyValue("kind", out var kindNode) &&
                kindNode is JsonValue kv &&
                kv.TryGetValue<string>(out var kindStr) &&
                !string.IsNullOrWhiteSpace(kindStr))
            {
                var kind = kindStr.Trim();

                // Reject legacy fields if present (force migration)
                if (n.Properties.ContainsKey("actionType") || n.Properties.ContainsKey("executorType"))
                {
                    errors.Add(NodeErr(n, "Legacy properties 'actionType' / 'executorType' are not allowed. Use 'action.kind'."));
                }

                // Optional: validate webhook specifics
                if (string.Equals(kind, "webhook", StringComparison.OrdinalIgnoreCase))
                {
                    if (actionObj.TryGetPropertyValue("config", out var cfgNode))
                    {
                        JsonObject? cfgObj = cfgNode as JsonObject
                            ?? (cfgNode is JsonValue v && v.TryGetValue<string>(out var cfgStr)
                                ? TryParseObject(cfgStr)
                                : null);

                        if (cfgObj is null)
                        {
                            errors.Add(NodeErr(n, "webhook action config must be an object."));
                        }
                        else
                        {
                            if (!cfgObj.TryGetPropertyValue("url", out var urlNode) ||
                                urlNode is not JsonValue uv ||
                                !uv.TryGetValue<string>(out var urlStr) ||
                                string.IsNullOrWhiteSpace(urlStr))
                            {
                                errors.Add(NodeErr(n, "webhook action requires config.url."));
                            }
                            else if (!urlStr.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                            {
                                errors.Add(NodeErr(n, $"webhook url must use https. Value='{urlStr}'."));
                            }
                            else
                            {
                                // (Optional) rudimentary URI validation
                                if (!Uri.TryCreate(urlStr, UriKind.Absolute, out var uri) ||
                                    (uri.Scheme != Uri.UriSchemeHttps))
                                {
                                    errors.Add(NodeErr(n, $"webhook url invalid or not https. Value='{urlStr}'."));
                                }
                            }
                        }
                    }
                    else
                    {
                        errors.Add(NodeErr(n, "webhook action requires 'config' with 'url'."));
                    }
                }

                // Failure policy validation (if present)
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
            else
            {
                errors.Add(NodeErr(n, "Automatic node 'action.kind' is required and must be a non-empty string."));
            }
        }

        if (errors.Count > 0)
        {
            _logger.LogWarning("Workflow publish validation failed: {Count} error(s). DefinitionId={DefId}", errors.Count, definition.Id);
        }

        return errors;
    }

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
}

public static class WorkflowPublishValidationServiceCollectionExtensions
{
    public static IServiceCollection AddWorkflowPublishValidation(this IServiceCollection services)
    {
        services.AddSingleton<IWorkflowPublishValidator, WorkflowPublishValidator>();
        return services;
    }
}
