using Microsoft.EntityFrameworkCore;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// Alias enums to avoid ambiguity with System.Threading.Tasks.TaskStatus
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus;
using InstanceStatusEnum = DTOs.Workflow.Enums.InstanceStatus;

namespace WorkflowService.Background;

/// <summary>
/// Background service that advances due timer tasks.
/// </summary>
public class TimerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TimerWorker> _logger;
    private readonly TimeSpan _interval;

    public TimerWorker(
        IServiceProvider serviceProvider,
        ILogger<TimerWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        var seconds = configuration.GetValue<int>("WorkflowSettings:TimerWorkerIntervalSeconds", 30);
        _interval = TimeSpan.FromSeconds(Math.Max(5, seconds));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("WF_TIMER_WORKER_START IntervalSeconds={Interval}", _interval.TotalSeconds);

        while (!stoppingToken.IsCancellationRequested)
        {
            var started = DateTime.UtcNow;
            try
            {
                await ProcessDueTimers(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WF_TIMER_WORKER_ERROR");
            }

            var elapsed = DateTime.UtcNow - started;
            var delay = _interval - elapsed;
            if (delay < TimeSpan.FromSeconds(1)) delay = TimeSpan.FromSeconds(1);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException) { }
        }

        _logger.LogInformation("WF_TIMER_WORKER_STOP");
    }

    private async Task ProcessDueTimers(CancellationToken ct)
    {
        using var scope = _serviceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var runtime = scope.ServiceProvider.GetRequiredService<IWorkflowRuntime>();

        var now = DateTime.UtcNow;

        // Fetch a reasonable batch to avoid long locks
        var dueTasks = await db.WorkflowTasks
            .Include(t => t.WorkflowInstance)
            .Where(t =>
                t.Status == WorkflowTaskStatus.Created &&
                t.DueDate.HasValue &&
                t.DueDate <= now &&
                t.WorkflowInstance.Status == InstanceStatusEnum.Running)
            .OrderBy(t => t.DueDate)
            .Take(100)
            .ToListAsync(ct);

        if (!dueTasks.Any())
        {
            _logger.LogDebug("WF_TIMER_WORKER_NO_DUE");
            return;
        }

        _logger.LogInformation("WF_TIMER_WORKER_DUE_COUNT Count={Count}", dueTasks.Count);

        foreach (var task in dueTasks)
        {
            try
            {
                _logger.LogInformation("WF_TIMER_FIRE Instance={InstanceId} Task={TaskId} Node={NodeId} Due={Due}",
                    task.WorkflowInstanceId, task.Id, task.NodeId, task.DueDate);

                // Use 0 as system user id sentinel
                await runtime.CompleteTaskAsync(task.Id, "{}", 0, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WF_TIMER_FIRE_ERROR Task={TaskId} Instance={InstanceId}",
                    task.Id, task.WorkflowInstanceId);
            }
        }
    }
}
