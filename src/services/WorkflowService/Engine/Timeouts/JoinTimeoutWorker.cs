using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;

namespace WorkflowService.Engine.Timeouts;

public sealed class JoinTimeoutOptions
{
    public int ScanIntervalSeconds { get; set; } = 30;
    public int BatchSize { get; set; } = 100;
}

/// <summary>
/// Background scanner for join timeouts.
/// </summary>
public sealed class JoinTimeoutWorker : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<JoinTimeoutWorker> _logger;
    private readonly JoinTimeoutOptions _options;

    public JoinTimeoutWorker(
        IServiceProvider services,
        IOptions<JoinTimeoutOptions> options,
        ILogger<JoinTimeoutWorker> logger)
    {
        _services = services;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("JoinTimeoutWorker started (interval={Interval}s)", _options.ScanIntervalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScanAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "JoinTimeoutWorker scan failure");
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.ScanIntervalSeconds), stoppingToken);
            }
            catch (TaskCanceledException) { }
        }
    }

    // Exposed for tests (Option A reinstatement). Returns number of instances updated / processed.
    public async Task<int> ScanOnceAsync(CancellationToken ct = default) => await ScanAsync(ct);

    private async Task<int> ScanAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        // Only need a heuristic marker that timeouts exist
        // NOTE: Relaxed filter for reliability (in-memory tests + varied JSON layout).
        // We pull a bounded set of running instances, then cheap pre-filter in-memory.
        var candidates = await db.WorkflowInstances
            .Where(i => i.Status == DTOs.Workflow.Enums.InstanceStatus.Running &&
                        !string.IsNullOrEmpty(i.Context))
            .OrderBy(i => i.Id)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        // Narrow to those that appear to have join timeout markers.
        candidates = candidates
            .Where(i => i.Context!.Contains("\"timeoutSeconds\"", StringComparison.Ordinal))
            .ToList();

        if (candidates.Count == 0) return 0;

        var now = DateTime.UtcNow;
        int processed = 0;

        foreach (var inst in candidates)
        {
            if (ct.IsCancellationRequested) break;

            JsonObject root;
            try
            {
                root = JsonNode.Parse(inst.Context) as JsonObject ?? new JsonObject();
            }
            catch
            {
                continue;
            }

            if (root["_parallelGroups"] is not JsonObject groups)
                continue;

            // Load definition to enable scaffolding
            string? defJson = await db.WorkflowDefinitions
                .AsNoTracking()
                .Where(d => d.Id == inst.WorkflowDefinitionId && d.Version == inst.DefinitionVersion)
                .Select(d => d.JSONDefinition)
                .FirstOrDefaultAsync(ct);

            WorkflowService.Domain.Dsl.WorkflowDefinitionJson? defParsed = null;
            if (!string.IsNullOrWhiteSpace(defJson))
            {
                try { defParsed = BuilderDefinitionAdapter.Parse(defJson); }
                catch { /* ignore parse errors */ }
            }

            var active = DeserializeActive(inst);
            bool changed = false;

            // ---- Scaffolding for joins with timeout (if join meta absent) ----
            if (defParsed != null)
            {
                foreach (var joinNode in defParsed.Nodes.Where(n =>
                           n.Type != null &&
                           n.Type.Equals("join", StringComparison.OrdinalIgnoreCase)))
                {
                    if (!joinNode.Properties.TryGetValue("timeout", out var timeoutRaw))
                        continue;

                    JsonElement timeoutEl;
                    if (timeoutRaw is JsonElement je && je.ValueKind == JsonValueKind.Object)
                        timeoutEl = je;
                    else
                        continue;

                    int seconds = 0;
                    string? onTimeout = null;
                    string? target = null;
                    if (timeoutEl.TryGetProperty("seconds", out var secEl) && secEl.ValueKind == JsonValueKind.Number)
                        seconds = secEl.GetInt32();
                    if (seconds <= 0) continue;

                    if (timeoutEl.TryGetProperty("onTimeout", out var otEl) && otEl.ValueKind == JsonValueKind.String)
                        onTimeout = otEl.GetString();
                    if (timeoutEl.TryGetProperty("target", out var tgtEl) && tgtEl.ValueKind == JsonValueKind.String)
                        target = tgtEl.GetString();

                    // gatewayId retrieval (robust)
                    string? gatewayId = null;
                    if (joinNode.Properties.TryGetValue("gatewayId", out var gwRaw))
                    {
                        if (gwRaw is string s) gatewayId = s;
                        else if (gwRaw is JsonElement gj && gj.ValueKind == JsonValueKind.String)
                            gatewayId = gj.GetString();
                    }
                    if (string.IsNullOrWhiteSpace(gatewayId)) continue;

                    if (groups[gatewayId] is not JsonObject groupObj) continue;
                    if (groupObj["join"] is JsonObject) continue; // already scaffolded

                    var seed = inst.StartedAt == default ? inst.CreatedAt : inst.StartedAt;
                    if (seed == default) seed = now;
                    var timeoutAt = seed.AddSeconds(seconds);

                    var joinMeta = new JsonObject
                    {
                        ["nodeId"] = joinNode.Id,
                        ["mode"] = (joinNode.Properties.TryGetValue("mode", out var mRaw) && mRaw is string ms)
                            ? ms.ToLowerInvariant()
                            : "all",
                        ["cancelRemaining"] = (joinNode.Properties.TryGetValue("cancelRemaining", out var crRaw) &&
                                               crRaw is bool cb) ? cb : false,
                        ["count"] = (joinNode.Properties.TryGetValue("count", out var cRaw) &&
                                     cRaw is JsonElement ce && ce.ValueKind == JsonValueKind.Number)
                            ? ce.GetInt32() : 0,
                        ["arrivals"] = new JsonArray(),
                        ["satisfied"] = false,
                        ["satisfiedAtUtc"] = null,
                        ["timeoutSeconds"] = seconds,
                        ["timeoutAtUtc"] = timeoutAt.ToString("O"),
                        ["onTimeout"] = (onTimeout ?? "force").Trim().ToLowerInvariant(),
                        ["timeoutTarget"] = string.IsNullOrWhiteSpace(target) ? null : target,
                        ["timeoutTriggered"] = false
                    };

                    groupObj["join"] = joinMeta;
                    changed = true;

                    _logger.LogDebug("JOIN_TIMEOUT_SCAFFOLD Instance={InstanceId} JoinNode={JoinNode} Seconds={Seconds}",
                        inst.Id, joinNode.Id, seconds);
                }
            }

            if (changed)
            {
                inst.Context = root.ToJsonString();
                inst.UpdatedAt = now;
                processed++;
                await db.SaveChangesAsync(ct);
                // Rehydrate root to include any EF formatting changes
                try { root = JsonNode.Parse(inst.Context) as JsonObject ?? root; }
                catch { }
            }

            // Ensure groups reference still valid
            groups = root["_parallelGroups"] as JsonObject ?? groups;

            // ---- Timeout evaluation & triggering ----
            foreach (var kv in groups)
            {
                if (kv.Value is not JsonObject grp) continue;
                if (grp["join"] is not JsonObject joinMeta) continue;

                if (TryGetBool(joinMeta, "satisfied", out var sat) && sat) continue;
                if (TryGetBool(joinMeta, "timeoutTriggered", out var trig) && trig) continue;
                if (!TryGetInt(joinMeta, "timeoutSeconds", out var timeoutSeconds) || timeoutSeconds <= 0) continue;

                DateTime timeoutAt;
                if (TryGetString(joinMeta, "timeoutAtUtc", out var timeoutAtStr) &&
                    DateTime.TryParse(timeoutAtStr, out var parsedTimeoutAt))
                {
                    timeoutAt = parsedTimeoutAt;
                }
                else
                {
                    var seed = inst.StartedAt == default ? inst.CreatedAt : inst.StartedAt;
                    if (seed == default) seed = now;
                    timeoutAt = seed.AddSeconds(timeoutSeconds);
                    joinMeta["timeoutAtUtc"] = timeoutAt.ToString("O");
                    changed = true;
                }

                // 100ms tolerance
                if (timeoutAt > now.AddMilliseconds(-100)) continue;

                var onTimeout = TryGetString(joinMeta, "onTimeout", out var otVal)
                    ? (otVal?.Trim().ToLowerInvariant() ?? "force")
                    : "force";
                var timeoutTarget = TryGetString(joinMeta, "timeoutTarget", out var tgtVal) ? tgtVal : null;
                var joinNodeId = joinMeta.TryGetPropertyValue("nodeId", out var jNodeVal) ? jNodeVal?.GetValue<string>() : null;
                var gatewayNodeId = grp.TryGetPropertyValue("gatewayNodeId", out var gVal) ? gVal?.GetValue<string>() : null;

                joinMeta["timeoutTriggered"] = true;
                joinMeta["timeoutAppliedAtUtc"] = now.ToString("O");
                changed = true;

                _logger.LogInformation("JOIN_TIMEOUT_TRIGGER Instance={InstanceId} JoinNode={JoinNode} Action={Action} Target={Target}",
                    inst.Id, joinNodeId ?? "?", onTimeout, timeoutTarget);

                if (onTimeout == "fail")
                {
                    inst.Status = DTOs.Workflow.Enums.InstanceStatus.Failed;
                    inst.ErrorMessage = "join-timeout";
                    inst.CompletedAt = now;
                    await AddEvent(db, inst.Id, "Parallel", "ParallelJoinTimeout", new
                    {
                        joinNodeId,
                        gatewayNodeId,
                        mode = joinMeta.TryGetPropertyValue("mode", out var m) ? m?.GetValue<string>() : null,
                        action = "fail",
                        timeoutSeconds,
                        timeoutTarget,
                        reason = "timeout-failed"
                    });
                    continue;
                }

                // route / force
                joinMeta["satisfied"] = true;
                joinMeta["satisfiedAtUtc"] = now.ToString("O");
                changed = true;

                if (onTimeout == "route" && !string.IsNullOrWhiteSpace(timeoutTarget))
                {
                    if (!active.Contains(timeoutTarget, StringComparer.OrdinalIgnoreCase))
                        active.Add(timeoutTarget);
                }
                else if (!string.IsNullOrWhiteSpace(joinNodeId))
                {
                    if (!active.Contains(joinNodeId, StringComparer.OrdinalIgnoreCase))
                        active.Add(joinNodeId);
                }

                inst.CurrentNodeIds = JsonSerializer.Serialize(active);

                await AddEvent(db, inst.Id, "Parallel", "ParallelJoinTimeout", new
                {
                    joinNodeId,
                    gatewayNodeId,
                    mode = joinMeta.TryGetPropertyValue("mode", out var modeVal) ? modeVal?.GetValue<string>() : null,
                    action = onTimeout,
                    timeoutSeconds,
                    timeoutTarget,
                    timeoutTriggered = true
                });
            }

            if (changed)
            {
                inst.UpdatedAt = now;
                inst.Context = root.ToJsonString();
                processed++;
            }
        }

        if (processed > 0)
            await db.SaveChangesAsync(ct);

        return processed;
    }

    #region Helpers

    private static List<string> DeserializeActive(WorkflowInstance instance)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(instance.CurrentNodeIds) ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static async Task AddEvent(WorkflowDbContext db, int instanceId, string type, string name, object payload)
    {
        var data = JsonSerializer.Serialize(payload);
        var tenantId = await db.WorkflowInstances
            .AsNoTracking()
            .Where(i => i.Id == instanceId)
            .Select(i => i.TenantId)
            .FirstOrDefaultAsync();

        db.WorkflowEvents.Add(new WorkflowEvent
        {
            WorkflowInstanceId = instanceId,
            TenantId = tenantId,
            Type = type,
            Name = name,
            Data = data,
            OccurredAt = DateTime.UtcNow
        });
        db.OutboxMessages.Add(new OutboxMessage
        {
            EventType = $"workflow.{type}.{name}".ToLowerInvariant(),
            EventData = data,
            TenantId = tenantId
        });
    }

    private static bool TryGetBool(JsonObject o, string prop, out bool value)
    {
        value = false;
        if (!o.TryGetPropertyValue(prop, out var v) || v is not JsonValue jv) return false;
        return jv.TryGetValue(out value);
    }

    private static bool TryGetInt(JsonObject o, string prop, out int value)
    {
        value = 0;
        if (!o.TryGetPropertyValue(prop, out var v) || v is not JsonValue jv) return false;
        return jv.TryGetValue(out value);
    }

    private static bool TryGetString(JsonObject o, string prop, out string? value)
    {
        value = null;
        if (!o.TryGetPropertyValue(prop, out var v) || v is not JsonValue jv) return false;
        return jv.TryGetValue(out value) && value != null;
    }

    #endregion
}
