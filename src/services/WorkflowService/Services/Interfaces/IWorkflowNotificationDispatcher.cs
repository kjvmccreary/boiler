using DTOs.Workflow.Enums;
using WorkflowService.Domain.Models;

namespace WorkflowService.Services.Interfaces;

public interface IWorkflowNotificationDispatcher
{
    Task NotifyInstanceAsync(WorkflowInstance instance, CancellationToken ct = default);
    Task NotifyInstanceAsync(int tenantId, int instanceId, InstanceStatus status, string currentNodeIds,
        DateTime? completedAt, string? errorMessage, CancellationToken ct = default);
    Task NotifyInstancesChangedAsync(int tenantId, CancellationToken ct = default);
    Task NotifyInstanceProgressAsync(int tenantId, int instanceId, int percentage, int visitedCount, int totalNodes,
        string status, IEnumerable<string> activeNodeIds, CancellationToken ct = default);
}

public sealed class NullWorkflowNotificationDispatcher : IWorkflowNotificationDispatcher
{
    public Task NotifyInstanceAsync(WorkflowInstance instance, CancellationToken ct = default) => Task.CompletedTask;
    public Task NotifyInstanceAsync(int tenantId, int instanceId, InstanceStatus status, string currentNodeIds,
        DateTime? completedAt, string? errorMessage, CancellationToken ct = default) => Task.CompletedTask;
    public Task NotifyInstancesChangedAsync(int tenantId, CancellationToken ct = default) => Task.CompletedTask;
    public Task NotifyInstanceProgressAsync(int tenantId, int instanceId, int percentage, int visitedCount, int totalNodes,
        string status, IEnumerable<string> activeNodeIds, CancellationToken ct = default) => Task.CompletedTask;
}
