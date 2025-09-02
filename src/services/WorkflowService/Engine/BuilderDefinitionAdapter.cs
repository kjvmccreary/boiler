using System.Text.Json;
using System.Text.Json.Serialization;
using WorkflowService.Domain.Dsl;

namespace WorkflowService.Engine;

/// <summary>
/// Adapts raw front-end builder JSON (lowercase types, from/to edges, label/x/y, ad-hoc fields)
/// into the internal runtime DSL (WorkflowDefinitionJson).
/// This DOES NOT mutate or normalize the stored JSON; it is a read-time adapter only.
/// </summary>
public static class BuilderDefinitionAdapter
{
    private static readonly JsonSerializerOptions RawOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    /// <summary>
    /// Parse builder JSON into WorkflowDefinitionJson.
    /// Safe: returns empty definition on any failure.
    /// </summary>
    public static WorkflowDefinitionJson Parse(string builderJson)
    {
        if (string.IsNullOrWhiteSpace(builderJson))
            return new WorkflowDefinitionJson();

        BuilderRoot? root;
        try
        {
            root = JsonSerializer.Deserialize<BuilderRoot>(builderJson, RawOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BuilderDefinitionAdapter] Failed primary deserialize, falling back. {ex.Message}");
            // As a fallback, attempt to parse assuming already canonical
            try
            {
                return WorkflowDefinitionJson.FromJson(builderJson);
            }
            catch
            {
                return new WorkflowDefinitionJson();
            }
        }

        if (root == null)
            return new WorkflowDefinitionJson();

        var def = new WorkflowDefinitionJson
        {
            Id = root.Id ?? root.Key ?? string.Empty,
            Name = root.Key ?? root.Id ?? "workflow",
            Version = root.Version ?? 1
        };

        // Nodes
        if (root.Nodes != null)
        {
            foreach (var n in root.Nodes)
            {
                if (string.IsNullOrWhiteSpace(n.Id) || string.IsNullOrWhiteSpace(n.Type))
                    continue;

                var node = new WorkflowNode
                {
                    Id = n.Id!,
                    Type = n.Type!, // keep case; IsType is already case-insensitive
                    Name = n.Label ?? n.Type ?? n.Id!,
                    Position = (n.X.HasValue && n.Y.HasValue)
                        ? new NodePosition { X = n.X.Value, Y = n.Y.Value }
                        : null,
                    Properties = new Dictionary<string, object>()
                };

                // Copy known optional semantic fields into Properties for executors
                CopyIfSet(node.Properties, "label", n.Label);
                CopyIfSet(node.Properties, "assigneeRoles", n.AssigneeRoles);
                CopyIfSet(node.Properties, "dueInMinutes", n.DueInMinutes);
                CopyIfSet(node.Properties, "formSchema", n.FormSchema);
                CopyIfSet(node.Properties, "action", n.Action);
                CopyIfSet(node.Properties, "condition", n.Condition);
                CopyIfSet(node.Properties, "delayMinutes", n.DelayMinutes);
                CopyIfSet(node.Properties, "untilIso", n.UntilIso);
                CopyIfSet(node.Properties, "delaySeconds", n.DelaySeconds); // <<< NEW

                // Copy any remaining ad-hoc fields (extension data)
                if (n.Extra != null)
                {
                    foreach (var kv in n.Extra)
                    {
                        if (kv.Value is null) continue;
                        if (!node.Properties.ContainsKey(kv.Key))
                            node.Properties[kv.Key] = kv.Value;
                    }
                }

                def.Nodes.Add(node);
            }
        }

        // Edges
        if (root.Edges != null)
        {
            foreach (var e in root.Edges)
            {
                if (string.IsNullOrWhiteSpace(e.From) || string.IsNullOrWhiteSpace(e.To))
                    continue;

                def.Edges.Add(new WorkflowEdge
                {
                    Id = e.Id ?? Guid.NewGuid().ToString("N"),
                    Source = e.From!,
                    Target = e.To!,
                    Label = e.Label
                });
            }
        }

        return def;
    }

    private static void CopyIfSet(IDictionary<string, object> bag, string key, object? value)
    {
        if (value != null)
            bag[key] = value;
    }

    // Builder DTOs
    private sealed class BuilderRoot
    {
        [JsonPropertyName("key")] public string? Key { get; set; }
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("version")] public int? Version { get; set; }
        [JsonPropertyName("nodes")] public List<BuilderNode>? Nodes { get; set; }
        [JsonPropertyName("edges")] public List<BuilderEdge>? Edges { get; set; }
    }

    private sealed class BuilderNode
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("type")] public string? Type { get; set; }
        [JsonPropertyName("label")] public string? Label { get; set; }
        [JsonPropertyName("x")] public double? X { get; set; }
        [JsonPropertyName("y")] public double? Y { get; set; }

        [JsonPropertyName("assigneeRoles")] public string[]? AssigneeRoles { get; set; }
        [JsonPropertyName("dueInMinutes")] public int? DueInMinutes { get; set; }
        [JsonPropertyName("formSchema")] public object? FormSchema { get; set; }
        [JsonPropertyName("action")] public object? Action { get; set; }
        [JsonPropertyName("condition")] public string? Condition { get; set; }
        [JsonPropertyName("delayMinutes")] public double? DelayMinutes { get; set; }
        [JsonPropertyName("untilIso")] public string? UntilIso { get; set; }
        [JsonPropertyName("delaySeconds")] public double? DelaySeconds { get; set; } // <<< NEW
        [JsonExtensionData] public Dictionary<string, object>? Extra { get; set; }
    }

    private sealed class BuilderEdge
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("from")] public string? From { get; set; }
        [JsonPropertyName("to")] public string? To { get; set; }
        [JsonPropertyName("label")] public string? Label { get; set; }
    }
}
