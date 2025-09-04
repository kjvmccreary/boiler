using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executes an activated join node (already satisfied).
/// Simply advances along its outgoing linear edges.
/// </summary>
public class JoinExecutor : INodeExecutor
{
    private readonly ILogger<JoinExecutor> _logger;

    public string NodeType => NodeTypes.Join;

    public JoinExecutor(ILogger<JoinExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => node.IsJoin();

    public Task<NodeExecutionResult> ExecuteAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        string context,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("WF_JOIN_EXEC Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);

        // Outgoing nodes resolved by runtime (linear)
        return Task.FromResult(new NodeExecutionResult
        {
            IsSuccess = true,
            ShouldWait = false,
            NextNodeIds = new List<string>() // runtime will discover linear edges
        });
    }
}
