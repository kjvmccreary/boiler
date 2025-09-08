using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using Xunit;
using Moq;
using DTOs.Workflow.Enums;
using Contracts.Services;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Tests.Engine;

public class WorkflowRuntimeProgressDedupeTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly Mock<ITaskNotificationDispatcher> _taskNotifier = new();
    private readonly Mock<IWorkflowNotificationDispatcher> _instanceNotifier = new();
    private readonly System.Collections.Generic.List<INodeExecutor> _executors = new();

    public WorkflowRuntimeProgressDedupeTests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        // Start node executor (auto) - match by Type (avoid extension method dependency)
        var startExec = new Mock<INodeExecutor>();
        startExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n =>
                string.Equals(n.Type, "Start", StringComparison.OrdinalIgnoreCase));
        startExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        // End node executor (auto)
        var endExec = new Mock<INodeExecutor>();
        endExec.Setup(e => e.CanExecute(It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>()))
            .Returns<WorkflowService.Domain.Dsl.WorkflowNode>(n =>
                string.Equals(n.Type, "End", StringComparison.OrdinalIgnoreCase));
        endExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowService.Domain.Dsl.WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });

        _executors.Add(startExec.Object);
        _executors.Add(endExec.Object);
    }

    private WorkflowRuntime CreateRuntime()
    {
        return new WorkflowRuntime(
            DbContext,
            _executors,
            _tenant.Object,
            _conditions.Object,
            _taskNotifier.Object,
            CreateMockLogger<WorkflowRuntime>().Object,
            _instanceNotifier.Object);
    }

    [Fact]
    public async Task Linear_Auto_Flow_Should_Emit_Single_Terminal_Progress_100()
    {
        // Definition: Start -> End
        var defJson = JsonSerializer.Serialize(new
        {
            nodes = new[]
            {
                new { id = "start", type = "Start" },
                new { id = "end", type = "End" }
            },
            edges = new[]
            {
                new { id = "e1", source = "start", target = "end" }
            }
        });

        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Linear",
            Version = 1,
            JSONDefinition = defJson,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        await DbContext.SaveChangesAsync();

        var runtime = CreateRuntime();

        var instance = await runtime.StartWorkflowAsync(def.Id, "{}");
        instance.Status.Should().Be(InstanceStatus.Completed);

        // Gather progress events
        var progressEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id &&
                        e.Type == "Instance" &&
                        e.Name == "Progress")
            .OrderBy(e => e.OccurredAt)
            .ToList();

        // There should be at least one (initial) + final; dedupe ensures only one 100%
        progressEvents.Should().NotBeEmpty();

        var terminalHundreds = progressEvents
            .Count(e => (e.Data ?? string.Empty).Contains("\"percentage\":100", StringComparison.OrdinalIgnoreCase));

        terminalHundreds.Should().Be(1, "duplicate terminal 100% progress events should be suppressed");
    }
}
