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
/// Background scanner for join timeouts (B2).
/// Looks for unsatisfied joins whose timeoutAtUtc has elapsed and applies
/// the configured onTimeout policy.
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

    private async Task ScanAsync(CancellationToken ct)
    {
        using var scope = _services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();

        // Heuristic filter: only contexts mentioning timeoutSeconds.
        var candidates = await db.WorkflowInstances
            .Where(i => i.Status == DTOs.Workflow.Enums.InstanceStatus.Running &&
                        i.Context.Contains("\"timeoutSeconds\"") &&
                        i.Context.Contains("\"timeoutAtUtc\"") &&
                        !string.IsNullOrEmpty(i.Context))
            .OrderBy(i => i.Id)
            .Take(_options.BatchSize)
            .ToListAsync(ct);

        if (candidates.Count == 0) return;

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

            if (root["_parallelGroups"] is not JsonObject groups) continue;

            bool changed = false;
            foreach (var kv in groups)
            {
                if (kv.Value is not JsonObject grp) continue;
                if (grp["join"] is not JsonObject joinMeta) continue;

                if (!TryGetBool(joinMeta, "satisfied", out var sat) || sat) continue;
                if (TryGetBool(joinMeta, "timeoutTriggered", out var trig) && trig) continue;

                if (!TryGetInt(joinMeta, "timeoutSeconds", out var timeoutSeconds) || timeoutSeconds <= 0) continue;
                if (!TryGetString(joinMeta, "timeoutAtUtc", out var timeoutAtStr) ||
                    !DateTime.TryParse(timeoutAtStr, out var timeoutAt)) continue;

                if (timeoutAt > now) continue; // not yet due

                var onTimeout = TryGetString(joinMeta, "onTimeout", out var ot) ? (ot?.ToLowerInvariant() ?? "force") : "force";
                var timeoutTarget = TryGetString(joinMeta, "timeoutTarget", out var tt) ? tt : null;

                // Mark triggered
                joinMeta["timeoutTriggered"] = true;
                joinMeta["timeoutAppliedAtUtc"] = now.ToString("O");

                _logger.LogInformation("JOIN_TIMEOUT Instance={InstanceId} JoinNode={JoinNode} Mode={Mode} Action={Action} Target={Target}",
                    inst.Id,
                    joinMeta.TryGetPropertyValue("nodeId", out var jn) ? jn?.GetValue<string>() : "?",
                    joinMeta.TryGetPropertyValue("mode", out var m) ? m?.GetValue<string>() : "?",
                    onTimeout,
                    timeoutTarget);

                // Action: fail | route | force
                if (onTimeout == "fail")
                {
                    inst.Status = DTOs.Workflow.Enums.InstanceStatus.Failed;
                    inst.ErrorMessage = "join-timeout";
                    inst.CompletedAt = now;
                    changed = true;
                    await AddEvent(db, inst.Id, "Parallel", "ParallelJoinTimeout",
                        new
                        {
                            joinNodeId = joinMeta["nodeId"]?.GetValue<string>(),
                            gatewayNodeId = grp["gatewayNodeId"]?.GetValue<string>(),
                            mode = joinMeta["mode"]?.GetValue<string>(),
                            action = "fail",
                            timeoutSeconds,
                            timeoutTarget,
                            reason = "timeout-failed"
                        });
                    continue; // no routing needed
                }

                // For route/force we mark satisfied so runtime can advance
                joinMeta["satisfied"] = true;
                joinMeta["satisfiedAtUtc"] = now.ToString("O");

                // Ensure join node is in active set or route directly
                var active = DeserializeActive(inst);
                var joinNodeId = joinMeta["nodeId"]?.GetValue<string>();
                if (onTimeout == "route" && !string.IsNullOrWhiteSpace(timeoutTarget))
                {
                    // Directly activate chosen timeout target (partial completion path)
                    if (!active.Contains(timeoutTarget!, StringComparer.OrdinalIgnoreCase))
                        active.Add(timeoutTarget!);
                    // Optionally still allow join node cleanup (do not add join node)
                }
                else
                {
                    // force: proceed with join node normal outgoing edges
                    if (!string.IsNullOrWhiteSpace(joinNodeId) &&
                        !active.Contains(joinNodeId!, StringComparer.OrdinalIgnoreCase))
                        active.Add(joinNodeId!);
                }

                inst.CurrentNodeIds = JsonSerializer.Serialize(active);
                changed = true;

                await AddEvent(db, inst.Id, "Parallel", "ParallelJoinTimeout",
                    new
                    {
                        joinNodeId,
                        gatewayNodeId = grp["gatewayNodeId"]?.GetValue<string>(),
                        mode = joinMeta["mode"]?.GetValue<string>(),
                        action = onTimeout,
                        timeoutSeconds,
                        timeoutTarget,
                        timeoutTriggered = true
                    });
            }

            if (changed)
            {
                inst.UpdatedAt = now;
                processed++;
            }
        }

        if (processed > 0)
            await db.SaveChangesAsync(ct);
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

    private static List<string> DeserializeActive(WorkflowInstance instance)
    {
        try
        {
            return JsonSerializer.Deserialize<List<string>>(instance.CurrentNodeIds) ?? new();
        }
        catch
        {
            return new List<string>();
        }
    }

    private static async Task AddEvent(WorkflowDbContext db, int instanceId, string type, string name, object payload)
    {
        var data = JsonSerializer.Serialize(payload);
        db.WorkflowEvents.Add(new WorkflowEvent
        {
            WorkflowInstanceId = instanceId,
            Type = type,
            Name = name,
            Data = data,
            OccurredAt = DateTime.UtcNow
        });
        db.OutboxMessages.Add(new OutboxMessage
        {
            EventType = $"workflow.{type}.{name}".ToLowerInvariant(),
            EventData = data,
            TenantId = db.WorkflowInstances.AsNoTracking().Where(i => i.Id == instanceId).Select(i => i.TenantId).FirstOrDefault()
        });
        await Task.CompletedTask;
    }
}
