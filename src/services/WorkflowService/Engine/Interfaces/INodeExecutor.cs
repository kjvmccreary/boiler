using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;

namespace WorkflowService.Engine.Interfaces;

/// <summary>
/// Base interface for node executors
/// </summary>
public interface INodeExecutor
{
    /// <summary>
    /// Node type this executor can handle
    /// </summary>
    string NodeType { get; }

    /// <summary>
    /// Execute a node in the workflow
    /// </summary>
    Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if this executor can handle the given node
    /// </summary>
    bool CanExecute(WorkflowNode node);
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
}
