using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for automatic/system task nodes
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
            _logger.LogInformation("Executing automatic node {NodeId} for instance {InstanceId}", node.Id, instance.Id);

            var actionType = GetActionType(node);
            var config = GetConfiguration(node);

            _logger.LogInformation("Automatic node {NodeId} action type: {ActionType}", node.Id, actionType);

            // For MVP, just simulate the action
            switch (actionType.ToLowerInvariant())
            {
                case "webhook":
                    await ExecuteWebhookAction(node, instance, config, cancellationToken);
                    break;
                case "noop":
                default:
                    _logger.LogInformation("Executing no-op action for node {NodeId}", node.Id);
                    break;
            }

            var result = new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = false, // Automatic nodes complete immediately
                NextNodeIds = new List<string>() // Let runtime calculate next nodes
            };

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing automatic node {NodeId} for instance {InstanceId}", node.Id, instance.Id);
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private string GetActionType(WorkflowNode node)
    {
        // Handle frontend JSON structure
        if (node.Properties.TryGetValue("action", out var actionValue))
        {
            if (actionValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
            {
                var action = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
                if (action?.TryGetValue("kind", out var kindValue) == true)
                {
                    return kindValue?.ToString() ?? "noop";
                }
            }
        }

        return node.GetProperty<string>("actionType") ?? 
               node.GetProperty<string>("executorType") ?? 
               "noop";
    }

    private string GetConfiguration(WorkflowNode node)
    {
        // Handle frontend JSON structure
        if (node.Properties.TryGetValue("action", out var actionValue))
        {
            if (actionValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Object)
            {
                var action = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonElement.GetRawText());
                if (action?.TryGetValue("config", out var configValue) == true)
                {
                    return JsonSerializer.Serialize(configValue);
                }
            }
        }

        return node.GetProperty<string>("configuration") ?? "{}";
    }

    private async Task ExecuteWebhookAction(WorkflowNode node, WorkflowInstance instance, string config, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Simulating webhook action for node {NodeId} (MVP - not implemented)", node.Id);
        
        // For MVP, just log that we would call a webhook
        // In a full implementation, this would:
        // 1. Parse the config to get URL, headers, payload template
        // 2. Make HTTP request with workflow context data
        // 3. Handle response and update workflow context
        
        await Task.Delay(100, cancellationToken); // Simulate processing time
    }
}
