using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Options;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Delegating executor for Automatic nodes with suspend / proceed / fail policies.
/// </summary>
public class AutomaticExecutor : INodeExecutor
{
    private readonly ILogger<AutomaticExecutor> _logger;
    private readonly IAutomaticActionRegistry _registry;
    private readonly IEventPublisher _events;
    private readonly IAutomaticDiagnosticsBuffer _diagBuffer;
    private readonly WorkflowDiagnosticsOptions _diagOptions;

    public string NodeType => NodeTypes.Automatic;

    public AutomaticExecutor(
        ILogger<AutomaticExecutor> logger,
        IAutomaticActionRegistry registry,
        IEventPublisher events,
        IAutomaticDiagnosticsBuffer diagBuffer,
        IOptions<WorkflowDiagnosticsOptions> diagOptions)
    {
        _logger = logger;
        _registry = registry;
        _events = events;
        _diagBuffer = diagBuffer;
        _diagOptions = diagOptions.Value;
    }

    public bool CanExecute(WorkflowNode node) =>
        node.Type.Equals(NodeTypes.Automatic, StringComparison.OrdinalIgnoreCase);

    public async Task<NodeExecutionResult> ExecuteAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        string context,
        CancellationToken cancellationToken = default)
    {
        RecordDiag(new { phase = "enter", instanceId = instance.Id, nodeId = node.Id });

        var (kind, actionConfigJson, onFailure) = ExtractAction(node);

        _logger.LogInformation("WF_AUTO_EXEC_BEGIN Instance={InstanceId} Node={NodeId} Kind={Kind} Policy={Policy}",
            instance.Id, node.Id, kind, onFailure ?? "(default)");

        var exec = _registry.Get(kind);
        if (exec == null)
        {
            await EmitAsync("AutomaticActionExecutorMissing",
                new { instanceId = instance.Id, nodeId = node.Id, kind }, cancellationToken);

            var (fail, suspend, proceed) = InterpretPolicy(onFailure);
            return BuildPolicyResult(instance, node, $"Executor '{kind}' not registered", fail, suspend, proceed);
        }

        JsonDocument? cfgDoc = null;
        try
        {
            if (actionConfigJson != null)
                cfgDoc = JsonDocument.Parse(actionConfigJson);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "WF_AUTO_EXEC_CFG_PARSE_FAIL Node={NodeId}", node.Id);
            RecordDiag(new { phase = "config-parse-fail", instanceId = instance.Id, nodeId = node.Id, error = ex.Message });
        }

        await EmitAsync("AutomaticActionStarted",
            new { instanceId = instance.Id, nodeId = node.Id, kind }, cancellationToken);

        IAutomaticActionResult result;
        try
        {
            var actCtx = new AutomaticActionContext(instance, node.Id, null, cfgDoc, context, cancellationToken);
            result = await exec.ExecuteAsync(actCtx);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_AUTO_EXEC_UNHANDLED Instance={InstanceId} Node={NodeId} Kind={Kind}",
                instance.Id, node.Id, kind);
            result = Engine.AutomaticActions.AutomaticActionResult.Fail(ex.Message);
        }

        if (!result.Success)
        {
            await EmitAsync("AutomaticActionFailed",
                new { instanceId = instance.Id, nodeId = node.Id, kind, error = result.Error }, cancellationToken);

            var (fail, suspend, proceed) = InterpretPolicy(onFailure);
            return BuildPolicyResult(instance, node, result.Error ?? "Action failed", fail, suspend, proceed);
        }

        await EmitAsync("AutomaticActionCompleted",
            new { instanceId = instance.Id, nodeId = node.Id, kind }, cancellationToken);

        // Optional: persist output into workflow context
        var updatedContext = context;
        if (result.Output is not null)
        {
            try
            {
                updatedContext = MergeAutomaticOutput(context, node.Id, result.Output);
                RecordDiag(new
                {
                    phase = "output-persisted",
                    instanceId = instance.Id,
                    nodeId = node.Id,
                    hasOutput = true
                });
                _logger.LogDebug("WF_AUTO_OUTPUT_PERSISTED Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "WF_AUTO_OUTPUT_PERSIST_FAIL Instance={InstanceId} Node={NodeId} - leaving context unchanged",
                    instance.Id, node.Id);
                RecordDiag(new
                {
                    phase = "output-persist-fail",
                    instanceId = instance.Id,
                    nodeId = node.Id,
                    error = ex.Message
                });
            }
        }

        return new NodeExecutionResult
        {
            IsSuccess = true,
            ShouldWait = result.ShouldHaltTraversal,
            NextNodeIds = new List<string>(),
            UpdatedContext = updatedContext
        };
    }

    #region Output Persistence

    /// <summary>
    /// Merge an executor's output into the workflow context JSON at _autoOutputs[nodeId].
    /// If context is invalid/missing, a new root object is created.
    /// </summary>
    private static string MergeAutomaticOutput(string contextJson, string nodeId, object output)
    {
        JsonObject root;

        // Parse or create root object
        if (string.IsNullOrWhiteSpace(contextJson))
        {
            root = new JsonObject();
        }
        else
        {
            try
            {
                var parsed = JsonNode.Parse(contextJson);
                root = parsed as JsonObject ?? new JsonObject();
            }
            catch
            {
                root = new JsonObject();
            }
        }

        // Ensure _autoOutputs object
        if (root["_autoOutputs"] is not JsonObject autoOutputsObj)
        {
            autoOutputsObj = new JsonObject();
            root["_autoOutputs"] = autoOutputsObj;
        }

        // Serialize output to JsonNode
        JsonNode? outputNode;
        try
        {
            outputNode = JsonSerializer.SerializeToNode(output, output.GetType()) ?? JsonValue.Create((string?)null);
        }
        catch
        {
            // Fallback: store as string if serialization fails
            outputNode = JsonValue.Create(output.ToString());
        }

        autoOutputsObj[nodeId] = outputNode;

        // Return compact JSON (avoid indentation to minimize storage size)
        return root.ToJsonString(new JsonSerializerOptions
        {
            WriteIndented = false,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.Never
        });
    }

    #endregion

    #region Action Extraction / Policies

    private (string kind, string? cfg, string? policy) ExtractAction(WorkflowNode node)
    {
        if (node.Properties.TryGetValue("action", out var v) &&
            v is JsonElement el &&
            el.ValueKind == JsonValueKind.Object)
        {
            string kind = "noop";
            string? cfg = null;
            string? pol = null;

            if (el.TryGetProperty("kind", out var kEl) && kEl.ValueKind == JsonValueKind.String)
                kind = kEl.GetString() ?? "noop";
            if (el.TryGetProperty("config", out var cEl))
                cfg = cEl.GetRawText();
            if (el.TryGetProperty("onFailure", out var pEl) && pEl.ValueKind == JsonValueKind.String)
                pol = pEl.GetString();
            return (kind, cfg, pol);
        }

        var legacyKind = node.GetProperty<string>("actionType")
                        ?? node.GetProperty<string>("executorType")
                        ?? "noop";
        var legacyCfg = node.GetProperty<string>("configuration");
        var legacyPolicy = node.GetProperty<string>("onFailure");
        return (legacyKind, legacyCfg, legacyPolicy);
    }

    private static (bool fail, bool suspend, bool proceed) InterpretPolicy(string? policy)
    {
        if (string.IsNullOrWhiteSpace(policy)) return (true, false, false);
        return policy.Trim().ToLowerInvariant() switch
        {
            "proceed" => (false, false, true),
            "suspend" => (false, true, false),
            "failinstance" => (true, false, false),
            _ => (true, false, false)
        };
    }

    private NodeExecutionResult BuildPolicyResult(
        WorkflowInstance instance,
        WorkflowNode node,
        string error,
        bool fail,
        bool suspend,
        bool proceed)
    {
        if (proceed)
        {
            _logger.LogWarning("WF_AUTO_EXEC_POLICY_PROCEED Node={NodeId} Error={Error}", node.Id, error);
            RecordDiag(new { phase = "policy-proceed", instanceId = instance.Id, nodeId = node.Id, error });
            return new NodeExecutionResult
            {
                IsSuccess = true
            };
        }

        if (suspend)
        {
            _logger.LogWarning("WF_AUTO_EXEC_POLICY_SUSPEND Node={NodeId} Error={Error}", node.Id, error);
            RecordDiag(new { phase = "policy-suspend", instanceId = instance.Id, nodeId = node.Id, error });
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = error,
                FailureAction = NodeFailureAction.SuspendInstance
            };
        }

        // default fail
        if (fail)
        {
            _logger.LogError("WF_AUTO_EXEC_POLICY_FAIL Node={NodeId} Error={Error}", node.Id, error);
            RecordDiag(new { phase = "policy-fail", instanceId = instance.Id, nodeId = node.Id, error });
        }

        return new NodeExecutionResult
        {
            IsSuccess = false,
            ErrorMessage = error,
            FailureAction = NodeFailureAction.FailInstance
        };
    }

    #endregion

    #region Event + Diagnostics

    private async Task EmitAsync(string name, object payload, CancellationToken ct)
    {
        try
        {
            await _events.PublishCustomEventAsync(
                "Automatic",
                name,
                payload,
                tenantId: 0,
                workflowInstanceId: (int?)payload.GetType().GetProperty("instanceId")?.GetValue(payload),
                cancellationToken: ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "WF_AUTO_EXEC_EVENT_EMIT_FAIL Event={Event}", name);
            RecordDiag(new { phase = "event-emit-fail", eventName = name, error = ex.Message });
        }
    }

    private void RecordDiag(object obj)
    {
        if (_diagOptions.EnableAutomaticTrace)
            _diagBuffer.Record(obj);
    }

    #endregion
}
