using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using Contracts.Services;

// Enum aliases
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using InstanceStatusEnum = DTOs.Workflow.Enums.InstanceStatus;

namespace WorkflowService.Background;

/// <summary>
/// Simplified, resilient timer worker:
/// - Discovers due timer tasks ignoring filters
/// - Groups by tenant
/// - For each tenant: opens connection, SESSION scope SET app.tenant_id
/// - Sets ITenantProvider (best effort)
/// - Calls runtime.CompleteTaskAsync directly (system user 0)
/// - Saves after each tenant batch
/// No pre-status flip; no ExecuteUpdateAsync; avoids tenant trigger failures.
/// </summary>
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

        _interval = TimeSpan.FromSeconds(Math.Max(5, seconds));
        _batchSize = Math.Clamp(configuration.GetValue<int?>("WorkflowSettings:Timer:BatchSize") ?? 100, 5, 500);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TIMER_WORKER_START Interval={Interval}s BatchSize={BatchSize}",
            _interval.TotalSeconds, _batchSize);

        while (!stoppingToken.IsCancellationRequested)
        {
            var started = DateTime.UtcNow;
            try
            {
                await ProcessAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TIMER_WORKER_UNHANDLED");
            }

            var elapsed = DateTime.UtcNow - started;
            var delay = _interval - elapsed;
            if (delay < TimeSpan.FromSeconds(1)) delay = TimeSpan.FromSeconds(1);

            try { await Task.Delay(delay, stoppingToken); } catch { }
        }

        _logger.LogInformation("TIMER_WORKER_STOP");
    }

    private async Task ProcessAsync(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var runtime = scope.ServiceProvider.GetRequiredService<IWorkflowRuntime>();
        var tenantProvider = scope.ServiceProvider.GetRequiredService<ITenantProvider>();

        var now = DateTime.UtcNow;

        // Discover due timers
        var due = await db.WorkflowTasks
            .IgnoreQueryFilters()
            .Where(t =>
                t.NodeType == "timer" &&
                t.DueDate.HasValue &&
                t.DueDate <= now &&
                (t.Status == WorkflowTaskStatus.Created ||
                 t.Status == WorkflowTaskStatus.Assigned ||
                 t.Status == WorkflowTaskStatus.InProgress) &&
                t.WorkflowInstance.Status == InstanceStatusEnum.Running)
            .OrderBy(t => t.DueDate)
            .Select(t => new
            {
                t.Id,
                t.TenantId,
                t.WorkflowInstanceId,
                t.Status,
                t.DueDate
            })
            .Take(_batchSize)
            .ToListAsync(ct);

        if (due.Count == 0)
        {
            _logger.LogDebug("TIMER_WORKER_NO_DUE");
            return;
        }

        _logger.LogInformation("TIMER_WORKER_CANDIDATES Count={Count}", due.Count);
        int processed = 0;

        foreach (var tenantGroup in due.GroupBy(x => x.TenantId))
        {
            if (ct.IsCancellationRequested) break;
            var tenantId = tenantGroup.Key;

            await EnsureTenantSessionAsync(db, tenantId, ct);
            try { await tenantProvider.SetCurrentTenantAsync(tenantId); } catch { }

            foreach (var taskInfo in tenantGroup)
            {
                if (ct.IsCancellationRequested) break;
                try
                {
                    _logger.LogInformation(
                        "TIMER_WORKER_FIRE TaskId={TaskId} Instance={InstanceId} Tenant={TenantId} Due={Due} PrevStatus={Status}",
                        taskInfo.Id, taskInfo.WorkflowInstanceId, tenantId, taskInfo.DueDate, taskInfo.Status);

                    await runtime.CompleteTaskAsync(taskInfo.Id, "{}", 0, ct, autoCommit: false);
                    processed++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "TIMER_WORKER_COMPLETE_ERROR TaskId={TaskId} Tenant={TenantId}", taskInfo.Id, tenantId);
                }
            }

            try
            {
                await db.SaveChangesAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "TIMER_WORKER_SAVE_TENANT_ERROR Tenant={TenantId}", tenantId);
            }
        }

        _logger.LogInformation("TIMER_WORKER_DONE Processed={Processed} TotalCandidates={Total}", processed, due.Count);
    }

    private static async Task EnsureTenantSessionAsync(WorkflowDbContext db, int tenantId, CancellationToken ct)
    {
        var conn = db.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
            await conn.OpenAsync(ct);

        // SESSION scope
        await db.Database.ExecuteSqlRawAsync(
            $"SET app.tenant_id = '{tenantId}';", cancellationToken: ct);
    }
}
