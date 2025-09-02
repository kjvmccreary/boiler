using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using Contracts.Services;

// Enum aliases
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using InstanceStatusEnum = DTOs.Workflow.Enums.InstanceStatus;

namespace WorkflowService.Background;

public class TimerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TimerWorker> _logger;
    private readonly TimeSpan _interval;
    private readonly int _batchSize;

    public TimerWorker(
        IServiceProvider serviceProvider,
        ILogger<TimerWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;

        var seconds = configuration.GetValue<int?>("WorkflowSettings:Timer:IntervalSeconds")
                      ?? configuration.GetValue<int?>("WorkflowSettings:TimerWorkerIntervalSeconds")
                      ?? 30;

        _interval = TimeSpan.FromSeconds(Math.Clamp(seconds, 5, 300));
        _batchSize = Math.Clamp(configuration.GetValue<int?>("WorkflowSettings:Timer:BatchSize") ?? 100, 5, 500);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TIMER_WORKER_START Interval={IntervalSeconds}s BatchSize={BatchSize}",
            _interval.TotalSeconds, _batchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            var cycleStart = DateTime.UtcNow;
            try
            {
                await ProcessCycleAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TIMER_WORKER_LOOP_ERROR");
            }

            var elapsed = DateTime.UtcNow - cycleStart;
            var delay = _interval - elapsed;
            if (delay < TimeSpan.FromSeconds(1))
                delay = TimeSpan.FromSeconds(1);

            try { await Task.Delay(delay, stoppingToken); } catch { }
        }

        _logger.LogInformation("TIMER_WORKER_STOP");
    }

    private async Task ProcessCycleAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var runtime = scope.ServiceProvider.GetRequiredService<IWorkflowRuntime>();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();

        var now = DateTime.UtcNow;

        var tenantIds = await db.WorkflowTasks
            .IgnoreQueryFilters()
            .Where(t =>
                t.NodeType == "timer" &&
                t.DueDate.HasValue &&
                t.DueDate <= now &&
                (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned) &&
                t.WorkflowInstance.Status == InstanceStatusEnum.Running)
            .Select(t => t.TenantId)
            .Distinct()
            .OrderBy(t => t)
            .ToListAsync(ct);

        if (tenantIds.Count == 0)
        {
            _logger.LogDebug("TIMER_WORKER_NO_DUE");
            return;
        }

        int totalAcquired = 0;
        int totalCompleted = 0;
        var maxLagSeconds = 0.0;

        foreach (var tenantId in tenantIds)
        {
            if (ct.IsCancellationRequested) break;

            try
            {
                await EnsureTenantSessionAsync(db, tenantId, ct);
                try { await tenantProvider.SetCurrentTenantAsync(tenantId); } catch { }

                var acquired = await AcquireDueTimersForTenantAsync(db, tenantId, ct);
                if (acquired.Count == 0) continue;

                totalAcquired += acquired.Count;

                foreach (var row in acquired)
                {
                    if (row.DueDate.HasValue)
                    {
                        var lag = (DateTime.UtcNow - row.DueDate.Value).TotalSeconds;
                        if (lag > maxLagSeconds) maxLagSeconds = lag;
                    }
                    if (ct.IsCancellationRequested) break;

                    try
                    {
                        _logger.LogInformation(
                            "TIMER_WORKER_FIRE TaskId={TaskId} Instance={InstanceId} Tenant={TenantId} Due={Due}",
                            row.Id, row.WorkflowInstanceId, tenantId, row.DueDate);

                        await runtime.CompleteTaskAsync(row.Id, "{}", 0, ct, autoCommit: false);
                        totalCompleted++;
                    }
                    catch (Exception exTask)
                    {
                        _logger.LogError(exTask, "TIMER_WORKER_TASK_COMPLETE_ERROR TaskId={TaskId} Tenant={TenantId}", row.Id, tenantId);
                    }
                }

                try
                {
                    await db.SaveChangesAsync(ct);
                }
                catch (Exception exSave)
                {
                    _logger.LogError(exSave, "TIMER_WORKER_SAVE_ERROR Tenant={TenantId}", tenantId);
                }
            }
            catch (Exception exTenant)
            {
                _logger.LogError(exTenant, "TIMER_WORKER_TENANT_ERROR Tenant={TenantId}", tenantId);
            }
        }

        _logger.LogInformation(
            "TIMER_WORKER_DONE Tenants={TenantCount} Acquired={Acquired} Completed={Completed} MaxLagSec={MaxLag:F1}",
            tenantIds.Count, totalAcquired, totalCompleted, maxLagSeconds);
    }

    private sealed record AcquiredTimer(int Id, int WorkflowInstanceId, int TenantId, DateTime? DueDate);

    // ---- NEW (minimal) EF fallback for non-relational providers (tests / in-memory) ----
    private async Task<List<AcquiredTimer>> AcquireDueTimersForTenantEfAsync(
        WorkflowDbContext db,
        int tenantId,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var due = await db.WorkflowTasks
            .Where(t => t.TenantId == tenantId
                        && t.NodeType == "timer"
                        && t.DueDate.HasValue
                        && t.DueDate <= now
                        && (t.Status == WorkflowTaskStatus.Created || t.Status == WorkflowTaskStatus.Assigned)
                        && t.WorkflowInstance.Status == InstanceStatusEnum.Running)
            .OrderBy(t => t.DueDate)
            .Take(_batchSize)
            .ToListAsync(ct);

        if (!due.Any()) return new();

        foreach (var t in due)
        {
            t.Status = WorkflowTaskStatus.InProgress;
            t.UpdatedAt = DateTime.UtcNow;
        }

        return due
            .Select(t => new AcquiredTimer(t.Id, t.WorkflowInstanceId, t.TenantId, t.DueDate))
            .ToList();
    }
    // -------------------------------------------------------------------------------

    private async Task<List<AcquiredTimer>> AcquireDueTimersForTenantAsync(WorkflowDbContext db, int tenantId, CancellationToken ct)
    {
        // Use raw SKIP LOCKED only for relational + Npgsql; otherwise fallback
        if (!db.Database.IsRelational() ||
            (db.Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) != true))
        {
            return await AcquireDueTimersForTenantEfAsync(db, tenantId, ct);
        }

        var results = new List<AcquiredTimer>();
        var conn = (NpgsqlConnection)db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        using var cmd = conn.CreateCommand();
        cmd.CommandText = @"
WITH cte AS (
    SELECT t.""Id""
    FROM ""WorkflowTasks"" t
    JOIN ""WorkflowInstances"" i ON i.""Id"" = t.""WorkflowInstanceId""
    WHERE t.""NodeType"" = 'timer'
      AND t.""Status"" IN (@createdStatus, @assignedStatus)
      AND t.""DueDate"" <= now()
      AND t.""TenantId"" = @tenantId
      AND i.""Status"" = @runningInstance
    ORDER BY t.""DueDate""
    FOR UPDATE SKIP LOCKED
    LIMIT @batch
)
UPDATE ""WorkflowTasks"" wt
SET ""Status"" = @inProgressStatus,
    ""UpdatedAt"" = now()
FROM cte
WHERE wt.""Id"" = cte.""Id""
RETURNING wt.""Id"", wt.""WorkflowInstanceId"", wt.""TenantId"", wt.""DueDate"";";

        cmd.Parameters.Add(new NpgsqlParameter("createdStatus", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)WorkflowTaskStatus.Created });
        cmd.Parameters.Add(new NpgsqlParameter("assignedStatus", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)WorkflowTaskStatus.Assigned });
        cmd.Parameters.Add(new NpgsqlParameter("inProgressStatus", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)WorkflowTaskStatus.InProgress });
        cmd.Parameters.Add(new NpgsqlParameter("runningInstance", NpgsqlTypes.NpgsqlDbType.Integer) { Value = (int)InstanceStatusEnum.Running });
        cmd.Parameters.Add(new NpgsqlParameter("tenantId", NpgsqlTypes.NpgsqlDbType.Integer) { Value = tenantId });
        cmd.Parameters.Add(new NpgsqlParameter("batch", NpgsqlTypes.NpgsqlDbType.Integer) { Value = _batchSize });

        try
        {
            using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
            {
                results.Add(new AcquiredTimer(
                    reader.GetInt32(0),
                    reader.GetInt32(1),
                    reader.GetInt32(2),
                    reader.IsDBNull(3) ? null : reader.GetDateTime(3)
                ));
            }

            if (results.Count > 0)
            {
                _logger.LogInformation("TIMER_WORKER_ACQUIRE Tenant={TenantId} Count={Count}", tenantId, results.Count);
            }
        }
        catch (PostgresException pex)
        {
            _logger.LogError(pex, "TIMER_WORKER_ACQUIRE_PG_FAIL Tenant={TenantId}", tenantId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TIMER_WORKER_ACQUIRE_FAIL Tenant={TenantId}", tenantId);
        }

        return results;
    }

    private static async Task EnsureTenantSessionAsync(WorkflowDbContext db, int tenantId, CancellationToken ct)
    {
        // Skip for non-relational or non-Npgsql providers (avoids errors in unit tests / InMemory)
        if (!db.Database.IsRelational() ||
            (db.Database.ProviderName?.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) != true))
            return;

        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        await db.Database.ExecuteSqlInterpolatedAsync(
            $"SELECT set_config('app.tenant_id', {tenantId.ToString()}, false);",
            ct);
    }
}
