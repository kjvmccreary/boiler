using System.Text.Json;
using System.Text.Json.Nodes;

namespace WorkflowService.Domain.Dsl;

public static class WorkflowDefinitionJsonExtensions
{
    public static string BackfillGatewayHandles(this string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson)) return rawJson;
        JsonNode? node;
        try { node = JsonNode.Parse(rawJson); } catch { return rawJson; }
        if (node is not JsonObject root) return rawJson;
        if (root["edges"] is not JsonArray edges) return rawJson;

        var groups = edges
            .OfType<JsonObject>()
            .GroupBy(e => e["from"]?.GetValue<string>() ?? e["source"]?.GetValue<string>() ?? "")
            .Where(g => !string.IsNullOrEmpty(g.Key));

        foreach (var g in groups)
        {
            var list = g.ToList();

            var trueList = new List<JsonObject>();
            var falseList = new List<JsonObject>();
            var elseList = new List<JsonObject>();
            var unlabeled = new List<JsonObject>();

            foreach (var e in list)
            {
                string? label = e["label"]?.GetValue<string>();
                string? handle = e["fromHandle"]?.GetValue<string>() ?? e["sourceHandle"]?.GetValue<string>();
                string? eff = label ?? handle;

                if (string.IsNullOrWhiteSpace(eff))
                {
                    var idLower = e["id"]?.GetValue<string>()?.ToLowerInvariant() ?? "";
                    if (idLower.Contains("true")) eff = "true";
                    else if (idLower.Contains("false")) eff = "false";
                    else if (idLower.Contains("else")) eff = "else";
                }

                switch (eff)
                {
                    case "true": trueList.Add(e); break;
                    case "false": falseList.Add(e); break;
                    case "else": elseList.Add(e); break;
                    default: unlabeled.Add(e); break;
                }
            }

            if ((trueList.Count > 0 || falseList.Count > 0) && elseList.Count == 0 && unlabeled.Count == 1)
            {
                var fallback = unlabeled[0];
                fallback["fromHandle"] = "else";
                fallback["label"] = "else";
            }

            // Symmetry
            foreach (var e in list)
            {
                if (e["fromHandle"] is null && e["label"] is not null)
                    e["fromHandle"] = e["label"]!.GetValue<string>();
                if (e["label"] is null && e["fromHandle"] is not null)
                    e["label"] = e["fromHandle"]!.GetValue<string>();
            }
        }

        return root.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
}
