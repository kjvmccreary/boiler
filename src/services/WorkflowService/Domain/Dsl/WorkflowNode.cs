using System.Text.Json;
using System.Text.Json.Serialization;

namespace WorkflowService.Domain.Dsl;

// Allow parameterless construction (deserialization / legacy calls)
public record NodePosition(double X = 0, double Y = 0);

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

    public bool IsType(string nodeType) => Type.Equals(nodeType, StringComparison.OrdinalIgnoreCase);

    public T? GetProperty<T>(string key, T? defaultValue = default)
    {
        if (Properties.TryGetValue(key, out var value))
        {
            if (value is JsonElement el)
                return JsonSerializer.Deserialize<T>(el.GetRawText());
            return (T?)value;
        }
        return defaultValue;
    }

    public void SetProperty<T>(string key, T value) => Properties[key] = value!;
}
