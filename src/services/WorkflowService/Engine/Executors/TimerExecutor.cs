using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for timer nodes
/// </summary>
public class TimerExecutor : INodeExecutor
{
    private readonly ILogger<TimerExecutor> _logger;

    public string NodeType => NodeTypes.Timer;

    public TimerExecutor(ILogger<TimerExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => node.IsTimer();

    public async Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Executing timer node {NodeId} for instance {InstanceId}", node.Id, instance.Id);

            var timerType = GetTimerType(node);
            var dueDate = CalculateDueDate(node);

            _logger.LogInformation("Timer node {NodeId} type: {TimerType}, due date: {DueDate}",
                node.Id, timerType, dueDate);

            if (dueDate <= DateTime.UtcNow)
            {
                // Timer has already expired, continue immediately
                _logger.LogInformation("Timer node {NodeId} has already expired, continuing immediately", node.Id);

                return new NodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldWait = false,
                    NextNodeIds = new List<string>()
                };
            }
            else
            {
                // Timer is still running, create a timer record and wait
                _logger.LogInformation("Timer node {NodeId} will expire at {DueDate}, creating timer record",
                    node.Id, dueDate);

                // For MVP, we'll rely on the TimerWorker to advance this
                // In a full implementation, we might schedule a background job

                return new NodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldWait = true, // Wait for timer to expire
                    NextNodeIds = new List<string>(),
                    // Could add timer info to context for TimerWorker to find
                    UpdatedContext = UpdateContextWithTimerInfo(context, node.Id, dueDate)
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing timer node {NodeId} for instance {InstanceId}", node.Id, instance.Id);
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private string GetTimerType(WorkflowNode node)
    {
        return node.GetProperty<string>("timerType") ?? TimerTypes.Duration;
    }

    private DateTime CalculateDueDate(WorkflowNode node)
    {
        var timerType = GetTimerType(node);

        switch (timerType.ToLowerInvariant())
        {
            case "duration":
                var delayMinutes = GetDelayMinutes(node);
                return DateTime.UtcNow.AddMinutes(delayMinutes);

            case "duedate":
                var untilIso = GetUntilIso(node);
                if (DateTime.TryParse(untilIso, out var dueDate))
                {
                    return dueDate.ToUniversalTime();
                }
                _logger.LogWarning("Could not parse due date '{UntilIso}' for timer node, using 1 minute delay", untilIso);
                return DateTime.UtcNow.AddMinutes(1);

            default:
                _logger.LogWarning("Unknown timer type '{TimerType}', using 1 minute delay", timerType);
                return DateTime.UtcNow.AddMinutes(1);
        }
    }

    private int GetDelayMinutes(WorkflowNode node)
    {
        // Handle frontend JSON structure
        if (node.Properties.TryGetValue("delayMinutes", out var delayValue))
        {
            if (delayValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.Number)
            {
                if (jsonElement.TryGetInt32(out var minutes))
                {
                    return minutes;
                }
            }
        }

        return node.GetProperty<int>("durationMinutes", 1);
    }

    private string GetUntilIso(WorkflowNode node)
    {
        // Handle frontend JSON structure
        if (node.Properties.TryGetValue("untilIso", out var untilValue))
        {
            if (untilValue is JsonElement jsonElement && jsonElement.ValueKind == JsonValueKind.String)
            {
                return jsonElement.GetString() ?? string.Empty;
            }
        }

        return node.GetProperty<string>("dueDate") ?? string.Empty;
    }

    private string UpdateContextWithTimerInfo(string currentContext, string nodeId, DateTime dueDate)
    {
        try
        {
            var context = JsonSerializer.Deserialize<Dictionary<string, object>>(currentContext) ?? new();

            // Add timer info for TimerWorker to find
            context[$"timer_{nodeId}"] = new
            {
                NodeId = nodeId,
                DueDate = dueDate,
                CreatedAt = DateTime.UtcNow
            };

            return JsonSerializer.Serialize(context);
        }
        catch (Exception ex)
        {
            // If context update fails, just log and return original
            return currentContext;
        }
    }
}
