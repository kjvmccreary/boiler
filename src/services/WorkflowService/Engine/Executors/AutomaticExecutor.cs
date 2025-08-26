using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for automatic nodes
/// </summary>
public class AutomaticExecutor : INodeExecutor
{
    private readonly ILogger<AutomaticExecutor> _logger;

    public string NodeType => NodeTypes.Automatic;

    public AutomaticExecutor(ILogger<AutomaticExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => node.IsAutomatic();

    public async Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Executing automatic node {NodeId} for instance {InstanceId}", 
                node.Id, instance.Id);

            // For MVP, automatic nodes just pass through
            // In the future, this would execute actual logic based on ExecutorType
            var result = await ExecuteAutomaticLogic(node, context, cancellationToken);

            return new NodeExecutionResult
            {
                IsSuccess = true,
                UpdatedContext = result.UpdatedContext,
                NextNodeIds = node.GetOutgoingConnections(GetEdgesFromInstance(instance))
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing automatic node {NodeId}", node.Id);
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private async Task<AutomaticExecutionResult> ExecuteAutomaticLogic(WorkflowNode node, string context, CancellationToken cancellationToken)
    {
        // For MVP, just add execution timestamp to context
        var contextObj = JsonSerializer.Deserialize<Dictionary<string, object>>(context) ?? new();
        contextObj[$"automatic_{node.Id}_executed_at"] = DateTime.UtcNow.ToString("O");

        var updatedContext = JsonSerializer.Serialize(contextObj);

        // Simulate some processing time
        await Task.Delay(100, cancellationToken);

        return new AutomaticExecutionResult
        {
            IsSuccess = true,
            UpdatedContext = updatedContext
        };
    }

    private List<WorkflowEdge> GetEdgesFromInstance(WorkflowInstance instance)
    {
        // This is a placeholder - in reality, you'd get this from the workflow definition
        // For now, return empty list to avoid null reference
        return new List<WorkflowEdge>();
    }

    private class AutomaticExecutionResult
    {
        public bool IsSuccess { get; set; }
        public string? UpdatedContext { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
