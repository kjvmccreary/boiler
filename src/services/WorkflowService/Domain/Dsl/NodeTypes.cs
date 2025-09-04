using System;

namespace WorkflowService.Domain.Dsl;

/// <summary>
/// Constants for workflow node types
/// </summary>
public static class NodeTypes
{
    public const string Start      = "Start";
    public const string End        = "End";
    public const string HumanTask  = "HumanTask";
    public const string Automatic  = "Automatic";
    public const string Gateway    = "Gateway";
    public const string Timer      = "Timer";

    /// <summary>
    /// All supported node types in MVP
    /// </summary>
    public static readonly string[] SupportedTypes =
    {
        Start, End, HumanTask, Automatic, Gateway, Timer
    };

    public static bool IsSupported(string nodeType) =>
        SupportedTypes.Contains(nodeType, StringComparer.OrdinalIgnoreCase);
}

public static class GatewayTypes
{
    public const string Exclusive = "Exclusive";
    public const string Inclusive = "Inclusive";
    public const string Parallel  = "Parallel";
}

public static class TimerTypes
{
    public const string Duration = "Duration";
    public const string DueDate  = "DueDate";
    public const string Cron     = "Cron";
}

/// <summary>
/// Helper extensions for working with workflow nodes (now all case-insensitive).
/// Frontend supplies lowercase types (e.g. "automatic", "humanTask") while
/// backend constants are PascalCase. These helpers normalize that mismatch.
/// </summary>
public static class WorkflowNodeExtensions
{
    public static bool IsStart(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.Start, StringComparison.OrdinalIgnoreCase);

    public static bool IsEnd(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.End, StringComparison.OrdinalIgnoreCase);

    public static bool IsHumanTask(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.HumanTask, StringComparison.OrdinalIgnoreCase)
        || node.Type.Equals("humantask", StringComparison.OrdinalIgnoreCase);

    public static bool IsAutomatic(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.Automatic, StringComparison.OrdinalIgnoreCase)
        || node.Type.Equals("automatic", StringComparison.OrdinalIgnoreCase);

    public static bool IsGateway(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.Gateway, StringComparison.OrdinalIgnoreCase);

    public static bool IsTimer(this WorkflowNode node) =>
        node.Type.Equals(NodeTypes.Timer, StringComparison.OrdinalIgnoreCase);

    // Human Task helpers
    public static string GetTaskName(this WorkflowNode node) =>
        node.GetProperty<string>("taskName") ?? node.Name;

    public static string? GetAssignedToUserId(this WorkflowNode node) =>
        node.GetProperty<string>("assignedToUserId");

    public static string? GetAssignedToRole(this WorkflowNode node) =>
        node.GetProperty<string>("assignedToRole");

    public static string GetFormSchema(this WorkflowNode node) =>
        node.GetProperty<string>("formSchema") ?? "{}";

    public static TimeSpan? GetTimeoutDuration(this WorkflowNode node)
    {
        var timeoutMinutes = node.GetProperty<int?>("timeoutMinutes");
        return timeoutMinutes.HasValue ? TimeSpan.FromMinutes(timeoutMinutes.Value) : null;
    }

    // Gateway helpers
    public static string GetGatewayType(this WorkflowNode node) =>
        node.GetProperty<string>("gatewayType") ?? GatewayTypes.Exclusive;

    // Timer helpers
    public static string GetTimerType(this WorkflowNode node) =>
        node.GetProperty<string>("timerType") ?? TimerTypes.Duration;

    public static TimeSpan? GetDuration(this WorkflowNode node)
    {
        var durationMinutes = node.GetProperty<int?>("durationMinutes");
        return durationMinutes.HasValue ? TimeSpan.FromMinutes(durationMinutes.Value) : null;
    }

    public static DateTime? GetDueDate(this WorkflowNode node)
    {
        var dueDateStr = node.GetProperty<string>("dueDate");
        return DateTime.TryParse(dueDateStr, out var dueDate) ? dueDate : null;
    }

    // Automatic helpers
    public static string GetExecutorType(this WorkflowNode node) =>
        node.GetProperty<string>("executorType") ?? "default";

    public static string GetConfiguration(this WorkflowNode node) =>
        node.GetProperty<string>("configuration") ?? "{}";
}
