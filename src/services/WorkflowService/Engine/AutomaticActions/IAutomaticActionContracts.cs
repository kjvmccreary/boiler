using System.Text.Json;
using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.AutomaticActions;

/// <summary>
/// Runtime context passed to an automatic action executor.
/// </summary>
public interface IAutomaticActionContext
{
    WorkflowInstance Instance { get; }
    string NodeId { get; }
    JsonDocument? NodeJson { get; }
    JsonDocument? ActionConfig { get; }
    string CurrentContextJson { get; }
    CancellationToken CancellationToken { get; }
}

/// <summary>
/// Result from an automatic action execution.
/// </summary>
public interface IAutomaticActionResult
{
    bool Success { get; }
    string? Error { get; }
    object? Output { get; }
    bool ShouldHaltTraversal { get; }
}

/// <summary>
/// Implemented per action kind (e.g. noop, webhook, billing.generateInvoice).
/// </summary>
public interface IAutomaticActionExecutor
{
    /// <summary>Unique, case-insensitive action kind key.</summary>
    string Kind { get; }

    Task<IAutomaticActionResult> ExecuteAsync(IAutomaticActionContext ctx);
}

internal sealed class AutomaticActionContext : IAutomaticActionContext
{
    public WorkflowInstance Instance { get; }
    public string NodeId { get; }
    public JsonDocument? NodeJson { get; }
    public JsonDocument? ActionConfig { get; }
    public string CurrentContextJson { get; }
    public CancellationToken CancellationToken { get; }

    public AutomaticActionContext(
        WorkflowInstance instance,
        string nodeId,
        JsonDocument? nodeJson,
        JsonDocument? actionConfig,
        string currentContextJson,
        CancellationToken ct)
    {
        Instance = instance;
        NodeId = nodeId;
        NodeJson = nodeJson;
        ActionConfig = actionConfig;
        CurrentContextJson = currentContextJson;
        CancellationToken = ct;
    }
}

internal sealed class AutomaticActionResult : IAutomaticActionResult
{
    public bool Success { get; init; }
    public string? Error { get; init; }
    public object? Output { get; init; }
    public bool ShouldHaltTraversal { get; init; }

    public static AutomaticActionResult Ok(object? output = null, bool halt = false) =>
        new() { Success = true, Output = output, ShouldHaltTraversal = halt };

    public static AutomaticActionResult Fail(string? error, bool halt = false) =>
        new() { Success = false, Error = error, ShouldHaltTraversal = halt };
}
