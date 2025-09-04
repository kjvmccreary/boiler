using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;
using Microsoft.Extensions.Logging;
using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Engine.Validation;
using WorkflowService.Domain.Dsl;

namespace WorkflowService.Tests.TestSupport;

public class FakeEventPublisher : IEventPublisher
{
    public int PublishedDefinitions { get; private set; }
    public int UnpublishedDefinitions { get; private set; }
    public int ForceCancelledInstances { get; private set; }
    public int InstanceStarted { get; private set; }
    public int InstanceCompleted { get; private set; }
    public int InstanceFailed { get; private set; }
    public bool ThrowOnForceCancel { get; set; }

    public List<string> OutboxTypes = new();

    public Task PublishInstanceStartedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        InstanceStarted++; return Task.CompletedTask;
    }
    public Task PublishInstanceCompletedAsync(WorkflowInstance instance, CancellationToken cancellationToken = default)
    {
        InstanceCompleted++; return Task.CompletedTask;
    }
    public Task PublishInstanceFailedAsync(WorkflowInstance instance, string errorMessage, CancellationToken cancellationToken = default)
    {
        InstanceFailed++; return Task.CompletedTask;
    }
    public Task PublishTaskCreatedAsync(WorkflowTask task, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task PublishTaskCompletedAsync(WorkflowTask task, int completedByUserId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task PublishTaskAssignedAsync(WorkflowTask task, int assignedToUserId, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task PublishDefinitionPublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        PublishedDefinitions++; OutboxTypes.Add("workflow.definition.published"); return Task.CompletedTask;
    }
    public Task PublishDefinitionUnpublishedAsync(WorkflowDefinition definition, CancellationToken cancellationToken = default)
    {
        UnpublishedDefinitions++; OutboxTypes.Add("workflow.definition.unpublished"); return Task.CompletedTask;
    }
    public Task PublishInstanceForceCancelledAsync(WorkflowInstance instance, string reason, CancellationToken cancellationToken = default)
    {
        if (ThrowOnForceCancel)
            throw new InvalidOperationException("Simulated failure during force cancel event publish");
        ForceCancelledInstances++; OutboxTypes.Add("workflow.instance.force_cancelled"); return Task.CompletedTask;
    }
    public Task PublishCustomEventAsync(string eventType, string eventName, object eventData, int tenantId, int? userId = null, int? workflowInstanceId = null, CancellationToken cancellationToken = default)
    {
        OutboxTypes.Add($"workflow.{eventType}.{eventName}");
        return Task.CompletedTask;
    }
    public Task ProcessOutboxMessagesAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;
}

public class FakeGraphValidationService : IGraphValidationService
{
    public ValidationResultDto Validate(string json, bool strict = false) =>
        new() { IsValid = true };
}

public class FakeWorkflowPublishValidator : IWorkflowPublishValidator
{
    public IReadOnlyList<string> Validate(WorkflowDefinition definition, IEnumerable<WorkflowNode> nodes) =>
        Array.Empty<string>();
}
