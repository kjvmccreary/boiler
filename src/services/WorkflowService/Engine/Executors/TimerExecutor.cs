using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

public class TimerExecutor : INodeExecutor
{
    private readonly ILogger<TimerExecutor> _logger;

    // Interface-required node type identifier
    public string NodeType => "timer";

    public TimerExecutor(ILogger<TimerExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) =>
        node.Type.Equals("timer", StringComparison.OrdinalIgnoreCase);

    public async Task<NodeExecutionResult> ExecuteAsync(
        WorkflowNode node,
        WorkflowInstance instance,
        string currentContext,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var due = CalculateDueDate(node);

            var task = new WorkflowTask
            {
                TenantId = instance.TenantId,
                WorkflowInstanceId = instance.Id,
                NodeId = node.Id,
                NodeType = "timer",
                TaskName = node.GetProperty<string>("label") ?? "Timer",
                Status = DTOs.Workflow.Enums.TaskStatus.Created,
                DueDate = due,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Data = "{}"
            };

            _logger.LogInformation(
                "WF_TIMER_SCHEDULE Instance={InstanceId} Node={NodeId} DueUtc={DueUtc}",
                instance.Id, node.Id, due);

            return new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = true,      // Timer pauses progression until worker completes it
                CreatedTask = task,
                NextNodeIds = new List<string>(),
                UpdatedContext = currentContext
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "WF_TIMER_SCHEDULE_FAIL Instance={InstanceId} Node={NodeId}",
                instance.Id, node.Id);

            return new NodeExecutionResult
            {
                IsSuccess = false,
                ShouldWait = false,
                ErrorMessage = ex.Message,
                NextNodeIds = new List<string>()
            };
        }
    }

    private DateTime CalculateDueDate(WorkflowNode node)
    {
        // Priority: absolute untilIso/dueDate > delaySeconds > delayMinutes/durationMinutes > legacy int fallback
        var untilIso = GetString(node, "untilIso") ?? GetString(node, "dueDate");
        if (!string.IsNullOrWhiteSpace(untilIso) &&
            DateTime.TryParse(untilIso, out var parsed))
            return parsed.ToUniversalTime();

        if (TryGetDouble(node, "delaySeconds", out var delaySecondsRaw))
        {
            var seconds = CoerceRange(delaySecondsRaw, 0, 7 * 24 * 3600);
            return DateTime.UtcNow.AddSeconds(seconds);
        }

        if (TryGetDouble(node, "delayMinutes", out var delayMinutes) ||
            TryGetDouble(node, "durationMinutes", out delayMinutes))
        {
            var minutes = CoerceRange(delayMinutes, 0, 7 * 24 * 60);
            return DateTime.UtcNow.AddSeconds(minutes * 60);
        }

        var legacy = GetInt(node, "delayMinutes") ?? GetInt(node, "durationMinutes") ?? 1;
        if (legacy < 0) legacy = 0;
        return DateTime.UtcNow.AddMinutes(legacy);
    }

    private static double CoerceRange(double value, double min, double max)
    {
        if (double.IsNaN(value) || double.IsInfinity(value)) return min;
        if (value < min) return min;
        if (value > max) return max;
        return value;
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
        if (!node.Properties.TryGetValue(key, out var v)) return null;

        if (v is JsonElement el)
        {
            if (el.ValueKind == JsonValueKind.Number && el.TryGetInt32(out var n))
                return n;
            if (el.ValueKind == JsonValueKind.String &&
                int.TryParse(el.GetString(), out var ns))
                return ns;
        }
        else if (int.TryParse(v?.ToString(), out var parsed))
            return parsed;

        return null;
    }

    private static bool TryGetDouble(WorkflowNode node, string key, out double value)
    {
        value = 0;
        if (!node.Properties.TryGetValue(key, out var raw)) return false;

        if (raw is JsonElement el)
        {
            switch (el.ValueKind)
            {
                case JsonValueKind.Number:
                    if (el.TryGetDouble(out var n))
                    {
                        value = n;
                        return true;
                    }
                    break;
                case JsonValueKind.String:
                    if (double.TryParse(el.GetString(), out var ns))
                    {
                        value = ns;
                        return true;
                    }
                    break;
            }
        }
        else if (double.TryParse(raw?.ToString(), out var parsed))
        {
            value = parsed;
            return true;
        }
        return false;
    }
}
