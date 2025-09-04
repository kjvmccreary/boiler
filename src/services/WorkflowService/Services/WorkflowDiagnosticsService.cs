using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow.GatewayDiagnostics;

namespace WorkflowService.Services;

public class WorkflowDiagnosticsService : IWorkflowDiagnosticsService
{
    private readonly WorkflowDbContext _db;
    private readonly ILogger<WorkflowDiagnosticsService> _logger;

    public WorkflowDiagnosticsService(WorkflowDbContext db, ILogger<WorkflowDiagnosticsService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<GatewayDecisionHistoryDto?> GetGatewayDecisionHistoryAsync(int instanceId, CancellationToken ct = default)
    {
        var instance = await _db.WorkflowInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == instanceId, ct);

        if (instance == null) return null;

        var root = ParseContext(instance.Context);
        if (root["_gatewayDecisions"] is not JsonObject gwObj)
            return new GatewayDecisionHistoryDto { InstanceId = instanceId };

        var result = new GatewayDecisionHistoryDto { InstanceId = instanceId };

        foreach (var kv in gwObj)
        {
            var nodeHistory = ParseNodeHistory(kv.Key, kv.Value);
            if (nodeHistory != null)
                result.Gateways.Add(nodeHistory);
        }

        return result;
    }

    public async Task<GatewayNodeDecisionHistoryDto?> GetGatewayNodeDecisionHistoryAsync(
        int instanceId,
        string nodeId,
        CancellationToken ct = default)
    {
        var instance = await _db.WorkflowInstances
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == instanceId, ct);

        if (instance == null) return null;

        var root = ParseContext(instance.Context);
        if (root["_gatewayDecisions"] is not JsonObject gwObj) return null;
        if (!gwObj.TryGetPropertyValue(nodeId, out var nodeVal))
            return null;

        return ParseNodeHistory(nodeId, nodeVal);
    }

    private static JsonObject ParseContext(string json)
    {
        try
        {
            return JsonNode.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json) as JsonObject
                   ?? new JsonObject();
        }
        catch
        {
            return new JsonObject();
        }
    }

    private GatewayNodeDecisionHistoryDto? ParseNodeHistory(string nodeId, JsonNode? nodeVal)
    {
        if (nodeVal == null) return null;

        var container = new List<JsonObject>();

        if (nodeVal is JsonArray arr)
        {
            foreach (var n in arr.OfType<JsonObject>())
                container.Add(n);
        }
        else if (nodeVal is JsonObject legacy)
        {
            container.Add(legacy);
        }
        else
        {
            return null;
        }

        var dto = new GatewayNodeDecisionHistoryDto { NodeId = nodeId };

        foreach (var jo in container)
        {
            try
            {
                var entry = new GatewayDecisionEntryDto
                {
                    DecisionId = jo.TryGetPropertyValue("decisionId", out var did) ? did?.GetValue<string>() ?? "" : "",
                    Strategy = jo.TryGetPropertyValue("strategy", out var st) ? st?.GetValue<string>() ?? "" : "",
                    ConditionResult = jo.TryGetPropertyValue("conditionResult", out var cr) && cr is JsonValue vCr && vCr.TryGetValue<bool>(out var bCr) && bCr,
                    ShouldWait = jo.TryGetPropertyValue("shouldWait", out var sw) && sw is JsonValue vSw && vSw.TryGetValue<bool>(out var bSw) && bSw,
                    ElapsedMs = jo.TryGetPropertyValue("elapsedMs", out var ems) && ems is JsonValue vMs && vMs.TryGetValue<double>(out var dMs) ? dMs : 0,
                    Notes = jo.TryGetPropertyValue("notes", out var nts) ? nts?.GetValue<string>() : null,
                    EvaluatedAtUtc = ParseDate(jo, "evaluatedAtUtc")
                };

                if (jo.TryGetPropertyValue("chosenEdgeIds", out var ce) && ce is JsonArray ceArr)
                    entry.ChosenEdgeIds = ceArr.OfType<JsonValue>().Select(v => v.TryGetValue<string>(out var s) ? s : null).Where(s => !string.IsNullOrWhiteSpace(s)).ToList()!;

                if (jo.TryGetPropertyValue("selectedTargets", out var stn) && stn is JsonArray stArr)
                    entry.SelectedTargets = stArr.OfType<JsonValue>().Select(v => v.TryGetValue<string>(out var s) ? s : null).Where(s => !string.IsNullOrWhiteSpace(s)).ToList()!;

                if (jo.TryGetPropertyValue("diagnostics", out var diag) && diag is JsonObject dObj)
                {
                    if (dObj.TryGetPropertyValue("outgoingEdgeCount", out var oec) && oec is JsonValue vOec && vOec.TryGetValue<int>(out var iOec))
                        entry.OutgoingEdgeCount = iOec;

                    if (dObj.TryGetPropertyValue("classification", out var cls) && cls is JsonObject cObj)
                    {
                        entry.Classification = new Dictionary<string, string[]>();
                        foreach (var bk in cObj)
                        {
                            if (bk.Value is JsonArray arr2)
                            {
                                entry.Classification[bk.Key] = arr2
                                    .OfType<JsonValue>()
                                    .Select(v => v.TryGetValue<string>(out var s) ? s : null)
                                    .Where(s => s != null)
                                    .Cast<string>()
                                    .ToArray();
                            }
                        }
                    }

                    if (dObj.TryGetPropertyValue("outgoingSnapshot", out var snap) && snap is JsonArray snapArr)
                    {
                        entry.OutgoingSnapshot = new List<GatewayEdgeSnapshotDto>();
                        foreach (var row in snapArr.OfType<JsonObject>())
                        {
                            entry.OutgoingSnapshot.Add(new GatewayEdgeSnapshotDto
                            {
                                EdgeId = row.TryGetPropertyValue("edgeId", out var eId) ? eId?.GetValue<string>() ?? "" : "",
                                From = row.TryGetPropertyValue("from", out var fr) ? fr?.GetValue<string>() : null,
                                To = row.TryGetPropertyValue("to", out var to) ? to?.GetValue<string>() : null,
                                InferredLabel = row.TryGetPropertyValue("inferredLabel", out var il) ? il?.GetValue<string>() : null
                            });
                        }
                    }

                    if (dObj.TryGetPropertyValue("skippedEdgeIds", out var sk) && sk is JsonArray skArr)
                        entry.SkippedEdgeIds = skArr.OfType<JsonValue>()
                            .Select(v => v.TryGetValue<string>(out var s) ? s : null)
                            .Where(s => s != null)
                            .Cast<string>()
                            .ToArray();

                    // Capture any extras not already mapped
                    var usedKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                    {
                        "outgoingEdgeCount","classification","outgoingSnapshot","chosenEdgeIds","skippedEdgeIds"
                    };
                    var extras = new Dictionary<string, object>();
                    foreach (var kvp in dObj)
                    {
                        var k = kvp.Key;
                        if (usedKeys.Contains(k)) continue;
                        var raw = kvp.Value;
                        extras[k] = raw switch
                        {
                            null => "null",
                            JsonValue jv when jv.TryGetValue<string>(out var sVal) => sVal,
                            JsonValue jv2 when jv2.TryGetValue<double>(out var dVal) => dVal,
                            JsonValue jv3 when jv3.TryGetValue<long>(out var lVal) => lVal,
                            JsonValue jv4 when jv4.TryGetValue<bool>(out var bVal) => bVal,
                            JsonArray arr3 => arr3.ToJsonString(),
                            JsonObject obj3 => obj3.ToJsonString(),
                            _ => raw.ToJsonString()
                        };
                    }
                    if (extras.Count > 0)
                        entry.Extra = extras;
                }

                dto.Decisions.Add(entry);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to parse gateway decision node={NodeId}", nodeId);
            }
        }

        return dto;
    }

    private static DateTime ParseDate(JsonObject jo, string prop)
    {
        if (jo.TryGetPropertyValue(prop, out var val) && val is JsonValue v && v.TryGetValue<string>(out var s))
        {
            if (DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var dt))
                return dt;
        }
        return DateTime.MinValue;
    }
}
