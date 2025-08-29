using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Nodes;

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

    [JsonPropertyName("settings")]
    public Dictionary<string, object> Settings { get; set; } = new();

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
        var normalized = Normalize(json);
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var def = JsonSerializer.Deserialize<WorkflowDefinitionJson>(normalized, options) ?? new WorkflowDefinitionJson();

        // Edge repair (in case normalization still failed and we fell back)
        if (def.Edges.Any() && def.Edges.All(e => string.IsNullOrWhiteSpace(e.Source) || string.IsNullOrWhiteSpace(e.Target)))
        {
            try
            {
                var root = JsonNode.Parse(json) as JsonObject;
                if (root != null && root.TryGetPropertyValue("edges", out var edgesNode) && edgesNode is JsonArray arr)
                {
                    int repaired = 0;
                    foreach (var (edgeObj, modelEdge) in arr.OfType<JsonObject>().Zip(def.Edges, (o, m) => (o, m)))
                    {
                        if (string.IsNullOrWhiteSpace(modelEdge.Source) && edgeObj.TryGetPropertyValue("from", out var fromNode))
                        {
                            modelEdge.Source = fromNode?.GetValue<string>() ?? "";
                            repaired++;
                        }
                        if (string.IsNullOrWhiteSpace(modelEdge.Target) && edgeObj.TryGetPropertyValue("to", out var toNode))
                        {
                            modelEdge.Target = toNode?.GetValue<string>() ?? "";
                            repaired++;
                        }
                    }
                    if (repaired > 0)
                        Console.WriteLine($"[NormalizeRepair] Repaired {repaired} edge endpoint values from builder 'from/to'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[NormalizeRepair] Failed edge repair: {ex.Message}");
            }
        }

        return def;
    }

    public static string Normalize(string rawJson)
    {
        try
        {
            var doc = JsonNode.Parse(rawJson) as JsonObject;
            if (doc == null) return rawJson;

            var result = new JsonObject
            {
                ["id"] = doc.TryGetPropertyValue("id", out var rid) ? rid?.DeepClone() ?? "" : "",
                ["name"] = doc.TryGetPropertyValue("name", out var rname) ? rname?.DeepClone() :
                           (doc.TryGetPropertyValue("key", out var keyVal) ? keyVal?.DeepClone() : JsonValue.Create(""))!,
                ["description"] = doc.TryGetPropertyValue("description", out var desc) ? desc?.DeepClone() : null,
                ["version"] = doc.TryGetPropertyValue("version", out var ver) ? ver?.DeepClone() : JsonValue.Create(1)
            };

            var nodesArray = new JsonArray();
            if (doc.TryGetPropertyValue("nodes", out var nodesNode) && nodesNode is JsonArray rawNodes)
            {
                foreach (var n in rawNodes.OfType<JsonObject>())
                {
                    var id = n.TryGetPropertyValue("id", out var nid) ? nid?.GetValue<string>() ?? "" : "";
                    var type = n.TryGetPropertyValue("type", out var ntype) ? ntype?.GetValue<string>() ?? "" : "";
                    var label = n.TryGetPropertyValue("label", out var nlabel) ? nlabel?.GetValue<string>() : null;
                    var name = n.TryGetPropertyValue("name", out var nname) ? nname?.GetValue<string>() : label ?? type;

                    var pos = new JsonObject();
                    if (n.TryGetPropertyValue("x", out var xVal)) pos["x"] = xVal?.DeepClone() ?? JsonValue.Create(0);
                    if (n.TryGetPropertyValue("y", out var yVal)) pos["y"] = yVal?.DeepClone() ?? JsonValue.Create(0);
                    if (!pos.Any()) pos = null;

                    var properties = new JsonObject();
                    foreach (var kvp in n)
                    {
                        if (kvp.Key is "id" or "type" or "label" or "name" or "x" or "y") continue;
                        // DeepClone each value to avoid parent conflict
                        properties[kvp.Key] = kvp.Value?.DeepClone();
                    }

                    var canonicalNode = new JsonObject
                    {
                        ["id"] = id,
                        ["type"] = type,
                        ["name"] = name ?? ""
                    };
                    canonicalNode["properties"] = properties;
                    if (pos != null) canonicalNode["position"] = pos;
                    nodesArray.Add(canonicalNode);
                }
            }
            result["nodes"] = nodesArray;

            var edgesArray = new JsonArray();
            if (doc.TryGetPropertyValue("edges", out var edgesNode) && edgesNode is JsonArray rawEdges)
            {
                foreach (var e in rawEdges.OfType<JsonObject>())
                {
                    var id = e.TryGetPropertyValue("id", out var eid) ? eid?.GetValue<string>() ?? "" : "";
                    var source = e.TryGetPropertyValue("source", out var srcVal)
                        ? srcVal?.GetValue<string>() ?? ""
                        : (e.TryGetPropertyValue("from", out var fromVal) ? fromVal?.GetValue<string>() ?? "" : "");
                    var target = e.TryGetPropertyValue("target", out var tgtVal)
                        ? tgtVal?.GetValue<string>() ?? ""
                        : (e.TryGetPropertyValue("to", out var toVal) ? toVal?.GetValue<string>() ?? "" : "");
                    var label = e.TryGetPropertyValue("label", out var lblVal) ? lblVal?.GetValue<string>() : null;

                    var edgeObj = new JsonObject
                    {
                        ["id"] = id,
                        ["source"] = source,
                        ["target"] = target
                    };
                    if (!string.IsNullOrWhiteSpace(label))
                        edgeObj["label"] = label;

                    edgesArray.Add(edgeObj);
                }
            }
            result["edges"] = edgesArray;

            if (doc.TryGetPropertyValue("variables", out var varsNode))
                result["variables"] = varsNode?.DeepClone();
            if (doc.TryGetPropertyValue("settings", out var settingsNode))
                result["settings"] = settingsNode?.DeepClone();

            return result.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Normalize] Failed to normalize workflow JSON: {ex.Message}");
            return rawJson;
        }
    }

    public ValidationResult Validate()
    {
        var result = new ValidationResult();

        var startNodes = Nodes.Where(n => n.Type.Equals("Start", StringComparison.OrdinalIgnoreCase)).ToList();
        var endNodes = Nodes.Where(n => n.Type.Equals("End", StringComparison.OrdinalIgnoreCase)).ToList();

        if (!startNodes.Any())
            result.Errors.Add("Workflow must have at least one Start node");
        if (!endNodes.Any())
            result.Errors.Add("Workflow must have at least one End node");
        if (startNodes.Count > 1)
            result.Errors.Add("Workflow cannot have multiple Start nodes");

        var duplicateIds = Nodes.GroupBy(n => n.Id)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        foreach (var duplicateId in duplicateIds)
            result.Errors.Add($"Duplicate node ID: {duplicateId}");

        var nodeIds = Nodes.Select(n => n.Id).ToHashSet();
        foreach (var edge in Edges)
        {
            if (!nodeIds.Contains(edge.Source))
                result.Errors.Add($"Edge references unknown source node: {edge.Source}");
            if (!nodeIds.Contains(edge.Target))
                result.Errors.Add($"Edge references unknown target node: {edge.Target}");
        }

        if (startNodes.Any())
        {
            var reachableNodes = GetReachableNodes(startNodes.First().Id);
            var unreachableNodes = Nodes.Where(n => !reachableNodes.Contains(n.Id) &&
                !n.Type.Equals("Start", StringComparison.OrdinalIgnoreCase));
            foreach (var node in unreachableNodes)
                result.Warnings.Add($"Node '{node.Id}' is not reachable from Start node");
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

            var connectedNodes = Edges
                .Where(e => e.Source == currentNodeId)
                .Select(e => e.Target);

            foreach (var nodeId in connectedNodes)
                if (!visited.Contains(nodeId))
                    queue.Enqueue(nodeId);
        }

        return visited;
    }
}

public class WorkflowNode
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;
    
    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
    
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();
    
    [JsonPropertyName("position")]
    public NodePosition? Position { get; set; }

    public List<string> GetOutgoingConnections(List<WorkflowEdge> edges) =>
        edges.Where(e => e.Source == Id).Select(e => e.Target).ToList();

    public bool IsType(string nodeType) => Type.Equals(nodeType, StringComparison.OrdinalIgnoreCase);
    
    public T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
            return (T?)value;
        }
        return defaultValue;
    }

    public void SetProperty<T>(string key, T value) => Properties[key] = value ?? (object)string.Empty;
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
    public string? Condition { get; set; }
    
    [JsonPropertyName("label")]
    public string? Label { get; set; }
    
    [JsonPropertyName("properties")]
    public Dictionary<string, object> Properties { get; set; } = new();

    public bool IsDefault => GetProperty<bool>("isDefault", false);

    private T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value))
        {
            if (value is JsonElement jsonElement)
                return JsonSerializer.Deserialize<T>(jsonElement.GetRawText());
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

public class ValidationResult
{
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public bool IsValid => !Errors.Any();
}
