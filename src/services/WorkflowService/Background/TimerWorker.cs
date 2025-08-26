using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Dsl;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using DTOs.Workflow.Enums;

namespace WorkflowService.Background;

/// <summary>
/// Background service that processes workflow timers
/// </summary>
public class TimerWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TimerWorker> _logger;
    private readonly TimeSpan _checkInterval;

    public TimerWorker(
        IServiceProvider serviceProvider,
        ILogger<TimerWorker> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        var intervalSeconds = configuration.GetValue<int>("WorkflowSettings:TimerWorkerIntervalSeconds", 30);
        _checkInterval = TimeSpan.FromSeconds(intervalSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("TimerWorker started with check interval {Interval}", _checkInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessDueTimers(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing due timers");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("TimerWorker stopped");
    }

    private async Task ProcessDueTimers(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var workflowRuntime = scope.ServiceProvider.GetRequiredService<IWorkflowRuntime>();

        // Find workflow instances that are waiting on timer nodes
        var now = DateTime.UtcNow;
        
        var waitingInstances = await context.WorkflowInstances
            .Include(i => i.WorkflowDefinition)
            .Where(i => i.Status == InstanceStatus.Running)
            .ToListAsync(cancellationToken);

        foreach (var instance in waitingInstances)
        {
            try
            {
                await ProcessInstanceTimers(instance, workflowRuntime, now, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing timers for instance {InstanceId}", instance.Id);
            }
        }
    }

    private async Task ProcessInstanceTimers(
        WorkflowService.Domain.Models.WorkflowInstance instance, 
        IWorkflowRuntime workflowRuntime, 
        DateTime now, 
        CancellationToken cancellationToken)
    {
        var workflowDef = WorkflowDefinitionJson.FromJson(instance.WorkflowDefinition.JSONDefinition);
        var currentNodeIds = JsonSerializer.Deserialize<string[]>(instance.CurrentNodeIds) ?? Array.Empty<string>();

        var dueTimers = new List<string>();

        foreach (var nodeId in currentNodeIds)
        {
            var node = workflowDef.Nodes.FirstOrDefault(n => n.Id == nodeId);
            if (node != null && node.IsTimer())
            {
                var dueTime = CalculateTimerDueTime(node);
                if (dueTime <= now)
                {
                    dueTimers.Add(nodeId);
                    _logger.LogInformation("Timer {NodeId} is due for instance {InstanceId}", 
                        nodeId, instance.Id);
                }
            }
        }

        // If any timers are due, continue the workflow
        if (dueTimers.Any())
        {
            await workflowRuntime.ContinueWorkflowAsync(instance.Id, cancellationToken);
        }
    }

    private DateTime CalculateTimerDueTime(WorkflowNode timer)
    {
        var timerType = timer.GetTimerType();
        
        return timerType switch
        {
            TimerTypes.Duration => DateTime.UtcNow.Add(timer.GetDuration() ?? TimeSpan.Zero),
            TimerTypes.DueDate => timer.GetDueDate() ?? DateTime.UtcNow,
            TimerTypes.Cron => throw new NotImplementedException("Cron timers not implemented"),
            _ => DateTime.UtcNow
        };
    }
}
