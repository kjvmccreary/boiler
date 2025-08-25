using System.Text.Json.Serialization;

namespace WorkflowService.Domain.Dsl;

public class WorkflowDefinitionJson
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("version")]
    public int Version { get; set; } = 1;
    
    [JsonPropertyName("nodes")]
    public List<WorkflowNode> Nodes { get; set; } = new();
    
    [JsonPropertyName("edges")]
    public List<WorkflowEdge> Edges { get; set; } = new();
    
    [JsonPropertyName("variables")]
    public Dictionary<string, object> Variables { get; set; } = new();
}

public class WorkflowNode
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty; // Start, End, HumanTask, Automatic, Gateway, Timer
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
    
    [JsonPropertyName("position")]
    public NodePosition? Position { get; set; } // For UI layout
}

public class WorkflowEdge
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("source")]
    public string Source { get; set; } = string.Empty;
    
    [JsonPropertyName("target")]
    public string Target { get; set; } = string.Empty;
    
    [JsonPropertyName("condition")]
    public string? Condition { get; set; } // JsonLogic expression
    
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
}

public class NodePosition
{
    [JsonPropertyName("x")]
    public double X { get; set; }
    
    [JsonPropertyName("y")]
    public double Y { get; set; }
}
