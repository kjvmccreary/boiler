using System.Text.Json;
using System.Text.Json.Nodes;

namespace WorkflowService.Domain.Dsl;

public static class WorkflowDefinitionJsonBinaryGatewayCleanup
{
    public static string NormalizeBinaryGateways(this string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return raw;
        JsonNode? root;
        try { root = JsonNode.Parse(raw); } catch { return raw; }
        if (root is not JsonObject o || o["edges"] is not JsonArray arr) return raw;

        var edges = arr.OfType<JsonObject>().ToList();
        var groups = edges.GroupBy(e => e["from"]?.GetValue<string>() ?? "").Where(g => !string.IsNullOrEmpty(g.Key));

        foreach (var g in groups)
        {
            var list = g.ToList();
            var trueEdges  = list.Where(e => IsLabel(e, "true")).ToList();
            var falseEdges = list.Where(e => IsLabel(e, "false")).ToList();
            var elseEdges  = list.Where(e => IsLabel(e, "else")).ToList();

            if (elseEdges.Count > 0)
            {
                if (trueEdges.Count == 0 && falseEdges.Count == 0)
                {
                    // Convert single fallback to false
                    foreach (var e in elseEdges)
                    {
                        e["label"] = "false";
                        e["fromHandle"] = "false";
                    }
                }
                else
                {
                    // Remove else
                    foreach (var e in elseEdges) arr.Remove(e);
                }
            }

            // Keep only first true / first false
            foreach (var dup in trueEdges.Skip(1)) arr.Remove(dup);
            foreach (var dup in falseEdges.Skip(1)) arr.Remove(dup);
        }

        return o.ToJsonString(new JsonSerializerOptions { WriteIndented = true });

        static bool IsLabel(JsonObject e, string v)
        {
            var l = e["label"]?.GetValue<string>()?.ToLowerInvariant();
            var h = e["fromHandle"]?.GetValue<string>()?.ToLowerInvariant();
            return l == v || h == v;
        }
    }
}
