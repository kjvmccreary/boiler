using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Interfaces;

/// <summary>
/// Base interface for node executors
/// </summary>
public interface INodeExecutor
{
    string NodeType { get; }
    Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default);
    bool CanExecute(WorkflowNode node);
}

/// <summary>
/// Additional instruction for the runtime when a node execution fails.
/// </summary>
public enum NodeFailureAction
{
    None = 0,          // Treat as standard failure (fail instance)
    FailInstance = 1,  // Explicitly fail instance (default)
    SuspendInstance = 2 // Suspend / pause instance
}

/// <summary>
/// Result of node execution
/// </summary>
public class NodeExecutionResult
{
    public bool IsSuccess { get; set; }
    public string? ErrorMessage { get; set; }
    public string? UpdatedContext { get; set; }
    public List<string> NextNodeIds { get; set; } = new();
    public WorkflowTask? CreatedTask { get; set; }
    public bool ShouldWait { get; set; } = false;
    public DateTime? WaitUntil { get; set; }

    /// <summary>
    /// Runtime directive on failure (only meaningful when IsSuccess == false).
    /// </summary>
    public NodeFailureAction FailureAction { get; set; } = NodeFailureAction.None;
}
