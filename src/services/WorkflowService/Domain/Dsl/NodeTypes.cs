namespace WorkflowService.Domain.Dsl;

/// <summary>
/// Constants for workflow node types
/// </summary>
public static class NodeTypes
{
    public const string Start = "Start";
    public const string End = "End";
    public const string HumanTask = "HumanTask";
    public const string Automatic = "Automatic";
    public const string Gateway = "Gateway";
    public const string Timer = "Timer";

    /// <summary>
    /// All supported node types in MVP
    /// </summary>
    public static readonly string[] SupportedTypes = 
    {
        Start, End, HumanTask, Automatic, Gateway, Timer
    };

    /// <summary>
    /// Check if a node type is supported
    /// </summary>
    public static bool IsSupported(string nodeType) => 
        SupportedTypes.Contains(nodeType, StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Gateway types
/// </summary>
public static class GatewayTypes
{
    public const string Exclusive = "Exclusive"; // XOR - only one path
    public const string Inclusive = "Inclusive"; // OR - multiple paths possible
    public const string Parallel = "Parallel";   // AND - all paths (future)
}

/// <summary>
/// Timer types
/// </summary>
public static class TimerTypes
{
    public const string Duration = "Duration";     // Wait for X duration
    public const string DueDate = "DueDate";      // Wait until specific date/time
    public const string Cron = "Cron";            // Wait for cron expression (future)
}

/// <summary>
/// Helper extensions for working with workflow nodes
/// </summary>
public static class WorkflowNodeExtensions
{
    public static bool IsStart(this WorkflowNode node) => node.IsType(NodeTypes.Start);
    public static bool IsEnd(this WorkflowNode node) => node.IsType(NodeTypes.End);
    public static bool IsHumanTask(this WorkflowNode node) => node.IsType(NodeTypes.HumanTask);
    public static bool IsAutomatic(this WorkflowNode node) => node.IsType(NodeTypes.Automatic);
    public static bool IsGateway(this WorkflowNode node) => node.IsType(NodeTypes.Gateway);
    public static bool IsTimer(this WorkflowNode node) => node.IsType(NodeTypes.Timer);

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
