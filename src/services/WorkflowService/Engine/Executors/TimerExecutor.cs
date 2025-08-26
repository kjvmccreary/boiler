using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Interfaces;

namespace WorkflowService.Engine.Executors;

/// <summary>
/// Executor for timer nodes
/// </summary>
public class TimerExecutor : INodeExecutor
{
    private readonly ILogger<TimerExecutor> _logger;

    public string NodeType => NodeTypes.Timer;

    public TimerExecutor(ILogger<TimerExecutor> logger)
    {
        _logger = logger;
    }

    public bool CanExecute(WorkflowNode node) => node.IsTimer();

    public Task<NodeExecutionResult> ExecuteAsync(WorkflowNode node, WorkflowInstance instance, string context, CancellationToken cancellationToken = default)
    {
        try
        {
            var waitUntil = CalculateWaitTime(node);

            if (waitUntil <= DateTime.UtcNow)
            {
                // Timer has already expired, continue immediately
                _logger.LogInformation("Timer {NodeId} has already expired, continuing workflow", node.Id);
                
                var result = new NodeExecutionResult
                {
                    IsSuccess = true,
                    NextNodeIds = node.GetOutgoingConnections(new List<WorkflowEdge>())
                };
                return Task.FromResult(result);
            }
            else
            {
                // Timer needs to wait
                _logger.LogInformation("Timer {NodeId} will wait until {WaitUntil}", node.Id, waitUntil);
                
                var result = new NodeExecutionResult
                {
                    IsSuccess = true,
                    ShouldWait = true,
                    WaitUntil = waitUntil,
                    NextNodeIds = node.GetOutgoingConnections(new List<WorkflowEdge>())
                };
                return Task.FromResult(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing timer node {NodeId}", node.Id);
            var result = new NodeExecutionResult
            {
                IsSuccess = false,
                ErrorMessage = ex.Message
            };
            return Task.FromResult(result);
        }
    }

    private DateTime CalculateWaitTime(WorkflowNode timer)
    {
        var timerType = timer.GetTimerType();
        
        return timerType switch
        {
            TimerTypes.Duration => DateTime.UtcNow.Add(timer.GetDuration() ?? TimeSpan.Zero),
            TimerTypes.DueDate => timer.GetDueDate() ?? DateTime.UtcNow,
            TimerTypes.Cron => throw new NotImplementedException("Cron timers not implemented in MVP"),
            _ => throw new ArgumentException($"Unknown timer type: {timerType}")
        };
    }
}
