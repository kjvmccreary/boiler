using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for gateway nodes (conditional branching)
/// </summary>
public class GatewayEvaluator : INodeExecutor
{
    private readonly IConditionEvaluator _conditionEvaluator;
    private readonly ILogger<GatewayEvaluator> _logger;

    public string NodeType => NodeTypes.Gateway;

    public GatewayEvaluator(
        IConditionEvaluator conditionEvaluator,
        ILogger<GatewayEvaluator> logger)
    {
        _conditionEvaluator = conditionEvaluator;
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => node.IsGateway();

    public async Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            var nextNodeIds = await EvaluateGatewayConditions(node, context);

            _logger.LogInformation("Gateway {NodeId} evaluated to next nodes: {NextNodes}", 
                node.Id, string.Join(", ", nextNodeIds));

            return new NodeExecutionResult
            {
                IsSuccess = true,
                NextNodeIds = nextNodeIds
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating gateway node {NodeId}", node.Id);
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private Task<List<string>> EvaluateGatewayConditions(WorkflowNode gateway, string context)
    {
        var nextNodeIds = new List<string>();
        var gatewayType = gateway.GetGatewayType();

        // For MVP, we'll use a simplified condition evaluation
        // In a real implementation, you'd parse the workflow definition to get edges and conditions
        
        switch (gatewayType)
        {
            case GatewayTypes.Exclusive:
                // XOR - only one path can be taken
                // For MVP, just take the first outgoing connection
                var connections = gateway.GetOutgoingConnections(new List<WorkflowEdge>());
                if (connections.Any())
                {
                    nextNodeIds.Add(connections.First());
                }
                break;

            case GatewayTypes.Inclusive:
                // OR - multiple paths can be taken
                // For MVP, take all outgoing connections
                nextNodeIds.AddRange(gateway.GetOutgoingConnections(new List<WorkflowEdge>()));
                break;

            case GatewayTypes.Parallel:
                // AND - all paths are taken (not implemented in MVP)
                throw new NotImplementedException("Parallel gateways are not implemented in MVP");

            default:
                throw new ArgumentException($"Unknown gateway type: {gatewayType}");
        }

        return Task.FromResult(nextNodeIds);
    }
}
