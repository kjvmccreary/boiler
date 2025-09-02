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

    public ValidationResult ValidateForPublish()
    {
        var baseResult = Validate(); // reuse existing structural checks (start presence, edge refs, gateway hints)
        var result = new ValidationResult
        {
            Errors = new List<string>(baseResult.Errors),
            Warnings = new List<string>(baseResult.Warnings)
        };

        // 1. Exactly one Start
        var starts = Nodes.Where(n => n.Type.Equals("start", StringComparison.OrdinalIgnoreCase)).ToList();
        if (starts.Count == 0)
            result.Errors.Add("Exactly one Start node is required (found 0).");
        else if (starts.Count > 1)
            result.Errors.Add($"Exactly one Start node is required (found {starts.Count}).");

        // 2. At least one End already partially enforced, but ensure we list count
        var ends = Nodes.Where(n => n.Type.Equals("end", StringComparison.OrdinalIgnoreCase)).ToList();
        if (ends.Count == 0)
            result.Errors.Add("At least one End node is required (found 0).");

        // Short circuit if no start
        if (starts.Count == 1)
        {
            var startId = starts[0].Id;
            var reachable = GetReachableNodes(startId);

            // 3. Unreachable nodes (make them Errors for publish)
            var unreachable = Nodes.Where(n => !reachable.Contains(n.Id)).Select(n => n.Id).ToList();
            if (unreachable.Count > 0)
                result.Errors.Add($"Unreachable node(s) from Start: {string.Join(", ", unreachable)}");

            // 4. Unreachable Ends (already included above, but call out)
            var unreachableEnds = ends.Where(e => !reachable.Contains(e.Id)).Select(e => e.Id).ToList();
            if (unreachableEnds.Count > 0)
                result.Errors.Add($"End node(s) unreachable from Start: {string.Join(", ", unreachableEnds)}");

            // 5/6. Isolated islands (connected components not containing Start)
            var islandComponents = GetUndirectedComponents();
            foreach (var comp in islandComponents)
            {
                if (!comp.Contains(startId))
                    result.Errors.Add($"Isolated component (no path to Start) contains: {string.Join(", ", comp)}");
            }
        }

        // 7. Duplicate IDs
        var duplicates = Nodes
            .GroupBy(n => n.Id, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .Select(g => $"{g.Key} (x{g.Count()})")
            .ToList();
        if (duplicates.Count > 0)
            result.Errors.Add($"Duplicate node id(s): {string.Join(", ", duplicates)}");

        return result;
    }

    public ValidationResult ValidateForDraft()
    {
        // Start from the existing relaxed Validate (which already produces warnings for reachability)
        var baseResult = Validate();

        // Clone so we can manipulate
        var draftResult = new ValidationResult
        {
            Errors = new List<string>(baseResult.Errors),
            Warnings = new List<string>(baseResult.Warnings)
        };

        // If "An End node is required." exists, downgrade to warning
        const string endError = "An End node is required.";
        if (draftResult.Errors.Remove(endError))
            draftResult.Warnings.Add("End node missing (required before publish).");

        // If you later add a single‑start enforcement to Validate(), you can similarly downgrade “multiple starts”
        // Leave missing Start as a hard error: drafts still need a start to be meaningful
        // Leave unknown edge endpoints & duplicate IDs as hard errors (they indicate corruption or ID collisions)

        return draftResult;
    }

    // Helper: build undirected connected components
    private List<HashSet<string>> GetUndirectedComponents()
    {
        var adj = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
        foreach (var n in Nodes)
            adj[n.Id] = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var e in Edges)
        {
            if (!adj.ContainsKey(e.EffectiveSource) || !adj.ContainsKey(e.EffectiveTarget))
                continue;
            adj[e.EffectiveSource].Add(e.EffectiveTarget);
            adj[e.EffectiveTarget].Add(e.EffectiveSource);
        }

        var visited = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var comps = new List<HashSet<string>>();

        foreach (var id in adj.Keys)
        {
            if (visited.Contains(id)) continue;
            var stack = new Stack<string>();
            var comp = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            stack.Push(id);
            while (stack.Count > 0)
            {
                var cur = stack.Pop();
                if (!visited.Add(cur)) continue;
                comp.Add(cur);
                foreach (var nxt in adj[cur])
                    if (!visited.Contains(nxt)) stack.Push(nxt);
            }
            comps.Add(comp);
        }
        return comps;
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
