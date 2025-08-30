using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;
using System.Text.Json;

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
            _logger.LogInformation("Executing gateway node {NodeId} for instance {InstanceId}", node.Id, instance.Id);

            var condition = GetCondition(node);
            var gatewayType = node.GetGatewayType();

            _logger.LogInformation("Gateway node {NodeId} type: {GatewayType}, condition: {Condition}", 
                node.Id, gatewayType, condition);

            // ✅ FIX: Evaluate condition using the proper condition evaluator
            bool conditionResult = true; // Default for MVP
            
            if (!string.IsNullOrEmpty(condition) && condition != "true")
            {
                try
                {
                    conditionResult = await _conditionEvaluator.EvaluateAsync(condition, context);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to evaluate condition '{Condition}' for gateway {NodeId}, defaulting to true", 
                        condition, node.Id);
                    conditionResult = true; // Default to true on evaluation failure
                }
            }

            _logger.LogInformation("Gateway node {NodeId} condition evaluated to: {Result}", node.Id, conditionResult);

            // ✅ FIX: Let the WorkflowRuntime handle next node calculation
            // The runtime has access to the full workflow definition with edges
            var result = new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = false, // Gateways evaluate immediately
                NextNodeIds = new List<string>(), // Let runtime calculate next nodes based on edges
                UpdatedContext = UpdateContextWithGatewayResult(context, node.Id, conditionResult)
            };

            return result;
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

    // ✅ ADD: Helper methods to handle frontend JSON structure
    private string GetCondition(WorkflowNode node)
    {
        // Handle frontend JSON structure
        if (node.Properties.TryGetValue("condition", out var conditionValue))
        {
            if (conditionValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
            {
                return jsonElement.GetString() ?? string.Empty;
            }
        }

        return node.GetProperty<string>("condition") ?? "true";
    }

    private string UpdateContextWithGatewayResult(string currentContext, string nodeId, bool conditionResult)
    {
        try
        {
            var context = JsonSerializer.Deserialize<Dictionary<string, object>>(currentContext) ?? new();
            
            // Add gateway result to context for future nodes to use
            context[$"gateway_{nodeId}"] = new
            {
                NodeId = nodeId,
                ConditionResult = conditionResult,
                EvaluatedAt = DateTime.UtcNow
            };

            return JsonSerializer.Serialize(context);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to update context with gateway result for node {NodeId}", nodeId);
            return currentContext;
        }
    }

    // NOTE: Edge selection & condition branching now handled centrally in WorkflowRuntime.
    // This executor only records evaluation success/failure context
}
