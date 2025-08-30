using System.Text.Json;
using System.Text.Json.Nodes;

namespace WorkflowService.Domain.Dsl;

public static class WorkflowDefinitionJsonEnrichment
{
    public static string EnrichEdgesForGateway(this string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return raw;
        JsonNode? root;
        try { root = JsonNode.Parse(raw); } catch { return raw; }
        if (root is not JsonObject o) return raw;
        if (o["edges"] is not JsonArray edges) return raw;

        var groups = edges.OfType<JsonObject>()
            .GroupBy(e => e["from"]?.GetValue<string>() ?? e["source"]?.GetValue<string>() ?? "")
            .Where(g => !string.IsNullOrEmpty(g.Key));

        foreach (var g in groups)
        {
            var list = g.ToList();
            var trueEdges = new List<JsonObject>();
            var falseEdges = new List<JsonObject>();
            var elseEdges = new List<JsonObject>();
            var unlabeled = new List<JsonObject>();

            foreach (var e in list)
            {
                string? label = e["label"]?.GetValue<string>();
                string? handle = e["fromHandle"]?.GetValue<string>() ?? e["sourceHandle"]?.GetValue<string>();
                string? eff = label ?? handle;

                if (string.IsNullOrEmpty(eff))
                {
                    var idLow = e["id"]?.GetValue<string>()?.ToLowerInvariant() ?? "";
                    if (idLow.Contains("true")) eff = "true";
                    else if (idLow.Contains("false")) eff = "false";
                    else if (idLow.Contains("else")) eff = "else";
                }

                switch (eff)
                {
                    case "true": trueEdges.Add(e); break;
                    case "false": falseEdges.Add(e); break;
                    case "else": elseEdges.Add(e); break;
                    default: unlabeled.Add(e); break;
                }

                if (!string.IsNullOrEmpty(eff))
                {
                    e["label"] = eff;
                    e["fromHandle"] = eff;
                }
            }

            if ((trueEdges.Count > 0 || falseEdges.Count > 0) && elseEdges.Count == 0 && unlabeled.Count == 1)
            {
                var candidate = unlabeled[0];
                candidate["label"] = "else";
                candidate["fromHandle"] = "else";
            }

            foreach (var e in list)
            {
                if (e["fromHandle"] is null && e["label"] is not null)
                    e["fromHandle"] = e["label"]!.GetValue<string>();
                if (e["label"] is null && e["fromHandle"] is not null)
                    e["label"] = e["fromHandle"]!.GetValue<string>();
            }
        }

        return o.ToJsonString(new JsonSerializerOptions { WriteIndented = true });
    }
}
