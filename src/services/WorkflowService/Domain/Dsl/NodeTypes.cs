using System;

namespace WorkflowService.Domain.Dsl;

/// <summary>
/// Constants for workflow node types
/// </summary>
public static partial class NodeTypes
{
    public const string Start      = "Start";
    public const string End        = "End";
    public const string HumanTask  = "HumanTask";
    public const string Automatic  = "Automatic";
    public const string Gateway    = "Gateway";
    public const string Timer      = "Timer";
    // Join constant lives in partial (NodeTypes.Join.cs) but we still reference it here.
    // Ensure SupportedTypes includes it (the partial provides the actual constant).

    /// <summary>
    /// All supported node types in MVP (extended with Join via partial)
    /// </summary>
    public static readonly string[] SupportedTypes =
    {
        Start, End, HumanTask, Automatic, Gateway, Timer, Join  // Join from partial
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
/// Helper extensions for working with workflow nodes (case-insensitive).
/// </summary>
public static partial class WorkflowNodeExtensions
{
    private static string CleanType(string? raw)
    {
        if (string.IsNullOrEmpty(raw)) return string.Empty;
        Span<char> buf = stackalloc char[raw.Length];
        var i = 0;
        foreach (var c in raw)
        {
            if (c is '\u200B' // zero width space
                or '\u200C'   // zero width non-joiner
                or '\u200D'   // zero width joiner
                or '\u2060'   // word joiner
                or '\uFEFF'   // BOM
                or '\u00A0')  // NBSP
                continue;
            buf[i++] = c;
        }
        return new string(buf[..i]).Trim();
    }

    private static bool Eq(string? a, string b) =>
        CleanType(a).Equals(b, StringComparison.OrdinalIgnoreCase);

    public static bool IsStart(this WorkflowNode node) => Eq(node.Type, NodeTypes.Start);
    public static bool IsEnd(this WorkflowNode node) => Eq(node.Type, NodeTypes.End);

    public static bool IsHumanTask(this WorkflowNode node) =>
        Eq(node.Type, NodeTypes.HumanTask) || Eq(node.Type, "humantask");

    public static bool IsAutomatic(this WorkflowNode node) =>
        Eq(node.Type, NodeTypes.Automatic) || Eq(node.Type, "automatic");

    public static bool IsGateway(this WorkflowNode node) => Eq(node.Type, NodeTypes.Gateway);
    public static bool IsTimer(this WorkflowNode node) => Eq(node.Type, NodeTypes.Timer);

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
