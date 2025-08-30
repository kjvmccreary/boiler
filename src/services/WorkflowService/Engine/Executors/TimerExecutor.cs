using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for timer nodes (schedules a waiting task).
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
            var dueDate = CalculateDueDate(node);

            _logger.LogInformation("WF_TIMER_SCHEDULE Instance={InstanceId} Node={NodeId} DueUtc={DueUtc}",
                instance.Id, node.Id, dueDate);

            var task = new WorkflowTask
            {
                WorkflowInstanceId = instance.Id,
                NodeId = node.Id,
                TaskName = node.GetProperty<string>("label") ?? "Timer",
                Status = DTOs.Workflow.Enums.TaskStatus.Created,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DueDate = dueDate
            };

            return new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = true,
                CreatedTask = task,
                UpdatedContext = context
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_TIMER_ERROR Instance={InstanceId} Node={NodeId}", instance.Id, node.Id);
            return new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private DateTime CalculateDueDate(WorkflowNode node)
    {
        // Priority: untilIso > delayMinutes > default 1 minute
        var untilIso = GetString(node, "untilIso") ?? GetString(node, "dueDate");
        if (!string.IsNullOrWhiteSpace(untilIso) && DateTime.TryParse(untilIso, out var until))
            return until.ToUniversalTime();

        var delayMinutes = GetInt(node, "delayMinutes") ?? GetInt(node, "durationMinutes") ?? 1;
        return DateTime.UtcNow.AddMinutes(delayMinutes);
    }

    private static string? GetString(WorkflowNode node, string key)
    {
        if (node.Properties.TryGetValue(key, out var v))
        {
            if (v is JsonElement el && el.ValueKind == JsonValueKind.String)
                return el.GetString();
            return v?.ToString();
        }
        return null;
    }

    private static int? GetInt(WorkflowNode node, string key)
    {
        if (node.Properties.TryGetValue(key, out var v))
        {
            if (v is JsonElement el && el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var n))
                return n;
            if (int.TryParse(v?.ToString(), out var parsed))
                return parsed;
        }
        return null;
    }
}
