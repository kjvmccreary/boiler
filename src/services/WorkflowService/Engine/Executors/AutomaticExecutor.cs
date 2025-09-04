using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Services.Interfaces;
using Microsoft.Extensions.Options;
using WorkflowService.Engine.Diagnostics;

namespace WorkflowService.Engine.Executors;

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
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["WorkflowInstanceId"] = instance.Id,
            ["WorkflowNodeId"] = node.Id
        });

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

        return new NodeExecutionResult
        {
            IsSuccess = true,
            ShouldWait = result.ShouldHaltTraversal,
            NextNodeIds = new List<string>(),
            UpdatedContext = context
        };
    }

    private (string kind, string? cfg, string? policy) ExtractAction(WorkflowNode node)
    {
        // 1. Direct (preferred) location
        if (TryExtractActionFromElement(node.Properties, out var direct))
            return direct;

        // 2. Nested legacy "properties" object (defensive)
        if (node.Properties.TryGetValue("properties", out var nested) && nested is JsonElement propsEl &&
            propsEl.ValueKind == JsonValueKind.Object)
        {
            if (TryExtractActionFromObject(propsEl, out var nestedResult))
                return nestedResult;
        }

        // 3. Legacy flat fields
        var legacyKind = node.GetProperty<string>("actionType")
                        ?? node.GetProperty<string>("executorType")
                        ?? "noop";
        var legacyCfg = node.GetProperty<string>("configuration");
        var legacyPol = node.GetProperty<string>("onFailure");
        return (legacyKind, legacyCfg, legacyPol);
    }

    private bool TryExtractActionFromElement(
        IDictionary<string, object> bag,
        out (string kind, string? cfg, string? policy) result)
    {
        result = default;
        if (!bag.TryGetValue("action", out var raw) || raw is not JsonElement el || el.ValueKind != JsonValueKind.Object)
            return false;

        if (TryExtractActionFromObject(el, out result))
            return true;

        return false;
    }

    private bool TryExtractActionFromObject(
        JsonElement el,
        out (string kind, string? cfg, string? policy) result)
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

        result = (kind, cfg, pol);
        return true;
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
            return new NodeExecutionResult { IsSuccess = true };
        }

        if (suspend)
        {
            _logger.LogWarning("WF_AUTO_EXEC_POLICY_SUSPEND Node={NodeId} Error={Error}", node.Id, error);
            RecordDiag(new { phase = "policy-suspend", instanceId = instance.Id, nodeId = node.Id, error });
        }

        if (fail)
        {
            _logger.LogError("WF_AUTO_EXEC_POLICY_FAIL Node={NodeId} Error={Error}", node.Id, error);
            RecordDiag(new { phase = "policy-fail", instanceId = instance.Id, nodeId = node.Id, error });
        }

        return new NodeExecutionResult
        {
            IsSuccess = false,
            ErrorMessage = error
        };
    }

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
}
