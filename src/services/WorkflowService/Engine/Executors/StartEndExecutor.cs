using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for Start and End nodes
/// </summary>
public class StartEndExecutor : INodeExecutor
{
    private readonly ILogger<StartEndExecutor> _logger;

    public string NodeType => NodeTypes.Start; // Handles both Start and End

    public StartEndExecutor(ILogger<StartEndExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => 
        node.IsStart() || node.IsEnd();

    public Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            if (node.IsStart())
            {
                _logger.LogInformation("Executing start node {NodeId} for instance {InstanceId}", 
                    node.Id, instance.Id);

                // ✅ FIX: This executor doesn't have access to the workflow definition edges
                // We need to let the WorkflowRuntime handle the next node calculation
                var result = new NodeExecutionResult
                {
                    IsSuccess = true,
                    NextNodeIds = new List<string>() // Let runtime calculate next nodes
                };
                return Task.FromResult(result);
            }
            else if (node.IsEnd())
            {
                _logger.LogInformation("Executing end node {NodeId} for instance {InstanceId}", 
                    node.Id, instance.Id);

                // End nodes terminate the workflow
                var result = new NodeExecutionResult
                {
                    IsSuccess = true,
                    NextNodeIds = new List<string>() // No next nodes - workflow ends
                };
                return Task.FromResult(result);
            }

            var errorResult = new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = $"Unsupported node type: {node.Type}"
            };
            return Task.FromResult(errorResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing {NodeType} node {NodeId}", node.Type, node.Id);
            var result = new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
            return Task.FromResult(result);
        }
    }
}
