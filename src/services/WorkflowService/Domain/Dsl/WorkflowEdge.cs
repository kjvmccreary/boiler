using System.Text.Json.Serialization;

namespace WorkflowService.Domain.Dsl;

public class WorkflowEdge
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    // Primary
    [JsonPropertyName("source")]
    public string? Source { get; set; }
    [JsonPropertyName("target")]
    public string? Target { get; set; }

    // Legacy synonyms
    [JsonPropertyName("from")]
    public string? From { get; set; }
    [JsonPropertyName("to")]
    public string? To { get; set; }

    [JsonPropertyName("label")]
    public string? Label { get; set; }

    [JsonPropertyName("fromHandle")]
    public string? FromHandle { get; set; }

    [JsonPropertyName("sourceHandle")]
    public string? SourceHandle { get; set; }

    [JsonPropertyName("properties")]
    public Dictionary<string, object>? Properties { get; set; }

    [JsonIgnore]
    public string EffectiveSource => Source ?? From ?? throw new InvalidOperationException($"Edge {Id} missing source");
    [JsonIgnore]
    public string EffectiveTarget => Target ?? To ?? throw new InvalidOperationException($"Edge {Id} missing target");
    [JsonIgnore]
    public string? EffectiveHandle => FromHandle ?? SourceHandle;

    public void InferLabelIfMissing()
    {
        if (!string.IsNullOrWhiteSpace(Label)) return;
        var c = EffectiveHandle;
        if (string.IsNullOrWhiteSpace(c))
        {
            var idl = Id.ToLowerInvariant();
            if (idl.Contains("true")) c = "true";
            else if (idl.Contains("false")) c = "false";
            else if (idl.Contains("else")) c = "else";
        }
        if (!string.IsNullOrWhiteSpace(c))
        {
            c = c.Trim().ToLowerInvariant();
            if (c is "yes") c = "true";
            else if (c is "no") c = "false";
            else if (c is "default") c = "else";
            if (c is "true" or "false" or "else")
                Label = c;
        }
    }
}
