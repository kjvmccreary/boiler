namespace WorkflowService.Domain.Dsl;

public static class NodeTypes
{
    public const string Start = "Start";
    public const string End = "End";
    public const string HumanTask = "HumanTask";
    public const string Automatic = "Automatic";
    public const string Gateway = "Gateway";
    public const string Timer = "Timer";
    
    public static readonly string[] AllowedTypes = 
    {
        Start, End, HumanTask, Automatic, Gateway, Timer
    };
    
    public static bool IsValid(string nodeType)
    {
        return AllowedTypes.Contains(nodeType);
    }
}

// Node-specific property classes for type safety
public class HumanTaskProperties
{
    public string? AssignedToRole { get; set; }
    public int? AssignedToUserId { get; set; }
    public int? TimeoutMinutes { get; set; }
    public string? FormSchema { get; set; } // JSON schema for task form
    public Dictionary<string, object> DefaultValues { get; set; } = new();
}

public class AutomaticProperties
{
    public string? ServiceUrl { get; set; }
    public string? Method { get; set; } = "POST";
    public Dictionary<string, string> Headers { get; set; } = new();
    public string? PayloadTemplate { get; set; } // JSON template
    public int? TimeoutSeconds { get; set; } = 30;
}

public class TimerProperties
{
    public int? DelayMinutes { get; set; }
    public DateTime? DueDate { get; set; }
    public string? CronExpression { get; set; }
}

public class GatewayProperties
{
    public string Type { get; set; } = "Exclusive"; // Exclusive, Inclusive, Parallel
    public string? DefaultPath { get; set; } // Fallback edge ID
}
