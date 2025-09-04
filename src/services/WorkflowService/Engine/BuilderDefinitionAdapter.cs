using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using WorkflowService.Domain.Dsl;

namespace WorkflowService.Engine;

/// <summary>
/// Adapter from front-end builder JSON to internal runtime definition.
/// - Canonicalizes node types
/// - Supports edge key synonyms: from/to or source/target
/// - Flattens nested "properties" object (if present) into node.Properties
/// </summary>
public static class BuilderDefinitionAdapter
{
    private static readonly JsonSerializerOptions RawOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip
    };

    private static readonly TextInfo TI = CultureInfo.InvariantCulture.TextInfo;

    public static WorkflowDefinitionJson Parse(string builderJson)
    {
        if (string.IsNullOrWhiteSpace(builderJson))
            return new WorkflowDefinitionJson();

        BuilderRoot? root;
        try
        {
            root = JsonSerializer.Deserialize<BuilderRoot>(builderJson, RawOptions);
        }
        catch
        {
            // Fallback: attempt to parse directly as WorkflowDefinitionJson DSL form
            try { return WorkflowDefinitionJson.FromJson(builderJson); }
            catch { return new WorkflowDefinitionJson(); }
        }

        if (root == null) return new WorkflowDefinitionJson();

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

                var canonicalType = CanonicalizeType(n.Type);

                var node = new WorkflowNode
                {
                    Id = n.Id!,
                    Type = canonicalType,
                    Name = n.Label ?? canonicalType ?? n.Id!,
                    Position = (n.X.HasValue && n.Y.HasValue)
                        ? new NodePosition { X = n.X.Value, Y = n.Y.Value }
                        : null,
                    Properties = new Dictionary<string, object>()
                };

                if (!string.Equals(canonicalType, n.Type, StringComparison.Ordinal))
                    node.Properties["originalType"] = n.Type!;

                // Direct mapped scalar / array / object style fields
                Copy(node.Properties, "label", n.Label);
                Copy(node.Properties, "assigneeRoles", n.AssigneeRoles);
                Copy(node.Properties, "dueInMinutes", n.DueInMinutes);
                Copy(node.Properties, "formSchema", n.FormSchema);
                Copy(node.Properties, "action", n.Action);
                Copy(node.Properties, "condition", n.Condition);
                Copy(node.Properties, "delayMinutes", n.DelayMinutes);
                Copy(node.Properties, "untilIso", n.UntilIso);
                Copy(node.Properties, "delaySeconds", n.DelaySeconds);

                // Extension data (unknown fields)
                if (n.Extra != null)
                {
                    foreach (var kv in n.Extra)
                    {
                        if (kv.Value.ValueKind == JsonValueKind.Null ||
                            kv.Value.ValueKind == JsonValueKind.Undefined) continue;

                        // Skip if a direct property already populated it (precedence for explicit mapped fields)
                        if (!node.Properties.ContainsKey(kv.Key))
                            node.Properties[kv.Key] = JsonElementToObject(kv.Value);
                    }
                }

                // NEW: Flatten nested "properties" object if present in extension data
                // This supports builder output like:
                // { "id":"gw", "type":"gateway", "properties": { "strategy": { "kind":"parallel" }, "gatewayType":"parallel" } }
                FlattenNestedProperties(node);

                def.Nodes.Add(node);
            }
        }

        // Edges (with synonyms)
        if (root.Edges != null)
        {
            foreach (var e in root.Edges)
            {
                string? from = e.From;
                string? to = e.To;

                if ((from == null || to == null) && e.Extra != null)
                {
                    if (from == null &&
                        e.Extra.TryGetValue("source", out var srcEl) &&
                        srcEl.ValueKind == JsonValueKind.String)
                        from = srcEl.GetString();

                    if (to == null &&
                        e.Extra.TryGetValue("target", out var tgtEl) &&
                        tgtEl.ValueKind == JsonValueKind.String)
                        to = tgtEl.GetString();
                }

                if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
                    continue;

                def.Edges.Add(new WorkflowEdge
                {
                    Id = e.Id ?? Guid.NewGuid().ToString("N"),
                    Source = from!,
                    Target = to!,
                    Label = e.Label
                });
            }
        }

        return def;
    }

    /// <summary>
    /// Flatten a nested "properties" JsonElement object (if present) into the node's top-level Properties dictionary.
    /// Does not overwrite existing keys. After flattening removes the original "properties" entry.
    /// This enables strategy / gatewayType values to be detected by downstream GatewayEvaluator & runtime logic.
    /// </summary>
    private static void FlattenNestedProperties(WorkflowNode node)
    {
        if (!node.Properties.TryGetValue("properties", out var nestedRaw))
            return;

        try
        {
            if (nestedRaw is JsonElement je && je.ValueKind == JsonValueKind.Object)
            {
                using var doc = JsonDocument.Parse(je.GetRawText());
                foreach (var prop in doc.RootElement.EnumerateObject())
                {
                    var key = prop.Name;

                    // Never overwrite an already populated key
                    if (node.Properties.ContainsKey(key))
                        continue;

                    // Preserve strategy object as JsonElement (object) so GatewayEvaluator can parse it
                    if (prop.Value.ValueKind == JsonValueKind.Object)
                    {
                        // Re-serialize + parse to keep a compact JsonElement clone
                        using var inner = JsonDocument.Parse(prop.Value.GetRawText());
                        node.Properties[key] = inner.RootElement.Clone();
                    }
                    else
                    {
                        node.Properties[key] = JsonElementToObject(prop.Value);
                    }
                }
            }
        }
        catch
        {
            // Swallow – flattening is best-effort
        }
        finally
        {
            // Remove the container key to avoid confusion
            node.Properties.Remove("properties");
        }
    }

    private static string CanonicalizeType(string raw)
    {
        var trimmed = SanitizeType(raw);
        if (trimmed.Length == 0) return trimmed;

        return trimmed.ToLowerInvariant() switch
        {
            "start" => NodeTypes.Start,
            "end" => NodeTypes.End,
            "humantask" or "human_task" or "human-task" or "human task" => NodeTypes.HumanTask,
            "automatic" or "automated" => NodeTypes.Automatic,
            "gateway" => NodeTypes.Gateway,
            "timer" => NodeTypes.Timer,
            _ => TI.ToTitleCase(trimmed)
        };
    }

    /// <summary>
    /// Strips common invisible / non-breaking whitespace characters that can
    /// silently break type comparisons (e.g. "Start\u200B").
    /// </summary>
    private static string SanitizeType(string raw)
    {
        if (string.IsNullOrEmpty(raw)) return raw;
        Span<char> buf = stackalloc char[raw.Length];
        var i = 0;
        foreach (var c in raw)
        {
            // Zero-width & non-breaking space variants
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

    private static void Copy(IDictionary<string, object> bag, string key, object? value)
    {
        if (value != null)
            bag[key] = value;
    }

    private static object JsonElementToObject(JsonElement el) =>
        el.ValueKind switch
        {
            JsonValueKind.String => el.GetString()!,
            JsonValueKind.Number => el.TryGetInt64(out var l) ? l :
                                    el.TryGetDouble(out var d) ? d : el.GetRawText(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null!,
            // For objects / arrays we keep the JsonElement so downstream code (GatewayEvaluator, etc.)
            // can inspect raw JSON (e.g., strategy.kind)
            _ => el
        };

    // DTOs
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
        [JsonPropertyName("delaySeconds")] public double? DelaySeconds { get; set; }
        [JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }
    }

    private sealed class BuilderEdge
    {
        [JsonPropertyName("id")] public string? Id { get; set; }
        [JsonPropertyName("from")] public string? From { get; set; }
        [JsonPropertyName("to")] public string? To { get; set; }
        [JsonPropertyName("label")] public string? Label { get; set; }
        [JsonExtensionData] public Dictionary<string, JsonElement>? Extra { get; set; }
    }
}
