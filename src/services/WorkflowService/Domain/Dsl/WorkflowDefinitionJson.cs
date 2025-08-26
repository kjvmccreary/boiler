using System.Text.Json;
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

    // ✅ ADD: Additional properties for workflow settings
    [JsonPropertyName("settings")]
    public Dictionary<string, object> Settings { get; set; } = new();

    // ✅ ADD: Serialization methods
    public string ToJson()
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Serialize(this, options);
    }

    public static WorkflowDefinitionJson FromJson(string json)
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        return JsonSerializer.Deserialize<WorkflowDefinitionJson>(json) ?? new WorkflowDefinitionJson();
    }

    // ✅ ADD: Validation method
    public ValidationResult Validate()
    {
        var result = new ValidationResult();

        // Must have at least one Start and one End node
        var startNodes = Nodes.Where(n => n.Type.Equals("Start", StringComparison.OrdinalIgnoreCase)).ToList();
        var endNodes = Nodes.Where(n => n.Type.Equals("End", StringComparison.OrdinalIgnoreCase)).ToList();

        if (!startNodes.Any())
            result.Errors.Add("Workflow must have at least one Start node");

        if (!endNodes.Any())
            result.Errors.Add("Workflow must have at least one End node");

        if (startNodes.Count > 1)
            result.Errors.Add("Workflow cannot have multiple Start nodes");

        // All nodes must have unique IDs
        var duplicateIds = Nodes.GroupBy(n => n.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);

        foreach (var duplicateId in duplicateIds)
        {
            result.Errors.Add($"Duplicate node ID: {duplicateId}");
        }

        // All edges must reference valid nodes
        var nodeIds = Nodes.Select(n => n.Id).ToHashSet();
        foreach (var edge in Edges)
        {
            if (!nodeIds.Contains(edge.Source))
                result.Errors.Add($"Edge references unknown source node: {edge.Source}");

            if (!nodeIds.Contains(edge.Target))
                result.Errors.Add($"Edge references unknown target node: {edge.Target}");
        }

        // Check reachability from Start node
        if (startNodes.Any())
        {
            var reachableNodes = GetReachableNodes(startNodes.First().Id);
            var unreachableNodes = Nodes.Where(n => !reachableNodes.Contains(n.Id) && 
                !n.Type.Equals("Start", StringComparison.OrdinalIgnoreCase));
            
            foreach (var node in unreachableNodes)
            {
                result.Warnings.Add($"Node '{node.Id}' is not reachable from Start node");
            }
        }

        return result;
    }

    private HashSet<string> GetReachableNodes(string startNodeId)
    {
        var visited = new HashSet<string>();
        var queue = new Queue<string>();
        queue.Enqueue(startNodeId);

        while (queue.Count > 0)
        {
            var currentNodeId = queue.Dequeue();
            if (visited.Contains(currentNodeId))
                continue;

            visited.Add(currentNodeId);

            // Add connected nodes
            var connectedNodes = Edges
                .Where(e => e.Source == currentNodeId)
                .Select(e => e.Target);

            foreach (var nodeId in connectedNodes)
            {
                if (!visited.Contains(nodeId))
                    queue.Enqueue(nodeId);
            }
        }

        return visited;
    }
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

    // ✅ ADD: Helper method to get outgoing connections
    public List<string> GetOutgoingConnections(List<WorkflowEdge> edges)
    {
        return edges.Where(e => e.Source == Id).Select(e => e.Target).ToList();
    }

    // ✅ ADD: Helper methods for specific node types
    public bool IsType(string nodeType) => Type.Equals(nodeType, StringComparison.OrdinalIgnoreCase);
    
    public T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
            {
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }
            return (T?)value;
        }
        return defaultValue;
    }

    public void SetProperty<T>(string key, T value)
    {
        Properties[key] = value ?? (object)string.Empty;
    }
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

    // ✅ ADD: Helper property for default conditions
    public bool IsDefault => GetProperty<bool>("isDefault", false);

    private T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
            {
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            }
            return (T?)value;
        }
        return defaultValue;
    }
}

public class NodePosition
{
    [JsonPropertyName("x")]
    public double X { get; set; }
    
    [JsonPropertyName("y")]
    public double Y { get; set; }
}

// ✅ ADD: Validation result class
public class ValidationResult
{
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool IsValid => !Errors.Any();
}
