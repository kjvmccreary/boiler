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
    public int Version { get; set; }

    [JsonPropertyName("nodes")]
    public List<WorkflowNode> Nodes { get; set; } = new();

    [JsonPropertyName("edges")]
    public List<WorkflowEdge> Edges { get; set; } = new();

    [JsonPropertyName("variables")]
    public Dictionary<string, object> Variables { get; set; } = new();

    [JsonPropertyName("settings")]
    public Dictionary<string, object> Settings { get; set; } = new();

    public static WorkflowDefinitionJson FromJson(string json)
    {
        var doc = JsonSerializer.Deserialize<WorkflowDefinitionJson>(json, SerializerOptions())
                  ?? throw new InvalidOperationException("Invalid workflow definition JSON");
        doc.NormalizeEdges();
        return doc;
    }

    public string ToJson()
    {
        NormalizeEdges(); // ensure we commit normalized edges
        return JsonSerializer.Serialize(this, SerializerOptions());
    }

    /// <summary>
    /// Normalize edges to ensure Source/Target present and label inferred from handles.
    /// </summary>
    public void NormalizeEdges()
    {
        foreach (var e in Edges)
        {
            if (string.IsNullOrWhiteSpace(e.Source) && !string.IsNullOrWhiteSpace(e.From))
                e.Source = e.From;
            if (string.IsNullOrWhiteSpace(e.Target) && !string.IsNullOrWhiteSpace(e.To))
                e.Target = e.To;

            e.InferLabelIfMissing();
        }
    }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();

        var startNodes = Nodes.Where(n => n.Type.Equals("start", StringComparison.OrdinalIgnoreCase)).ToList();
        if (!startNodes.Any())
            result.Errors.Add("A Start node is required.");
        if (Nodes.All(n => !n.Type.Equals("end", StringComparison.OrdinalIgnoreCase)))
            result.Errors.Add("An End node is required.");

        var nodeIds = Nodes.Select(n => n.Id).ToHashSet();

        // Ensure edges normalized before validation
        NormalizeEdges();

        foreach (var edge in Edges)
        {
            if (!nodeIds.Contains(edge.EffectiveSource))
                result.Errors.Add($"Edge {edge.Id} references unknown source node: {edge.EffectiveSource}");
            if (!nodeIds.Contains(edge.EffectiveTarget))
                result.Errors.Add($"Edge {edge.Id} references unknown target node: {edge.EffectiveTarget}");
        }

        if (startNodes.Any())
        {
            var reachable = GetReachableNodes(startNodes.First().Id);
            foreach (var n in Nodes.Where(n => !reachable.Contains(n.Id) && !n.Type.Equals("start", StringComparison.OrdinalIgnoreCase)))
                result.Warnings.Add($"Node '{n.Id}' is not reachable from Start node");
        }

        // Gateway specific helpful warnings
        foreach (var gateway in Nodes.Where(n => n.Type.Equals("gateway", StringComparison.OrdinalIgnoreCase)))
        {
            var outEdges = Edges.Where(e => e.EffectiveSource == gateway.Id).ToList();
            if (outEdges.Count > 1 && outEdges.Count(e => (e.Label ?? "").Length > 0) == 0)
            {
                result.Warnings.Add($"Gateway '{gateway.Id}' has multiple unlabeled edges. Add labels/handles (true/false/else) for clarity.");
            }
        }

        // After existing gateway warnings in Validate()
        foreach (var gw in Nodes.Where(n => n.Type.Equals("gateway", StringComparison.OrdinalIgnoreCase)))
        {
            var outs = Edges.Where(e => e.EffectiveSource == gw.Id).ToList();
            if (outs.Count > 1)
            {
                bool anyHandle = outs.Any(e =>
                    (e.FromHandle ?? e.SourceHandle ?? e.Label)?.ToLowerInvariant() is "true" or "false" or "else");
                if (!anyHandle)
                    result.Warnings.Add($"Gateway '{gw.Id}' has multiple outgoing edges but no branch handles/labels; edges may collapse visually.");
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
            var current = queue.Dequeue();
            if (!visited.Add(current)) continue;

            var next = Edges
                .Where(e => e.EffectiveSource == current)
                .Select(e => e.EffectiveTarget);

            foreach (var id in next)
                if (!visited.Contains(id))
                    queue.Enqueue(id);
        }

        return visited;
    }

    private static JsonSerializerOptions SerializerOptions() => new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
}

public class ValidationResult
{
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool IsValid => Errors.Count == 0;
}
