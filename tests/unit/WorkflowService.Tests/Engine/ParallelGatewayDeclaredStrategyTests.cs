using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using DTOs.Workflow.Enums;
using Xunit;

namespace WorkflowService.Tests.Engine;

public class ParallelGatewayDeclaredStrategyTests : TestBase
{
    private readonly Mock<ITenantProvider> _tenant = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly Mock<ITaskNotificationDispatcher> _notifier = new();
    private readonly List<INodeExecutor> _executors = new();

    private WorkflowRuntime CreateRuntime() => new(
        DbContext,
        _executors,
        _tenant.Object,
        _conditions.Object,
        _notifier.Object,
        CreateMockLogger<WorkflowRuntime>().Object);

    public ParallelGatewayDeclaredStrategyTests()
    {
        _tenant.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    private WorkflowDefinition SeedDefinition(string json)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "GW_Parallel",
            Version = 1,
            JSONDefinition = json,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def;
    }

    private void ClearExecutors() => _executors.Clear();

    private void AddSimpleExecutors()
    {
        var startExec = new Mock<INodeExecutor>();
        startExec.Setup(e => e.CanExecute(It.IsAny<WorkflowNode>()))
            .Returns<WorkflowNode>(n => n.Type.Equals("Start", StringComparison.OrdinalIgnoreCase));
        startExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });
        _executors.Add(startExec.Object);

        var gwExec = new Mock<INodeExecutor>();
        gwExec.Setup(e => e.CanExecute(It.IsAny<WorkflowNode>()))
            .Returns<WorkflowNode>(n => n.Type.Equals("Gateway", StringComparison.OrdinalIgnoreCase));
        gwExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });
        _executors.Add(gwExec.Object);

        var taskExec = new Mock<INodeExecutor>();
        taskExec.Setup(e => e.CanExecute(It.IsAny<WorkflowNode>()))
            .Returns<WorkflowNode>(n =>
                n.Type.Equals("Task", StringComparison.OrdinalIgnoreCase) ||
                n.Type.Equals("End", StringComparison.OrdinalIgnoreCase));
        taskExec.Setup(e => e.ExecuteAsync(
                It.IsAny<WorkflowNode>(),
                It.IsAny<WorkflowInstance>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NodeExecutionResult { IsSuccess = true, ShouldWait = false });
        _executors.Add(taskExec.Object);
    }

    private static string ParallelDeclaredJson => """
    {
      "nodes": [
        { "id": "start", "type": "Start" },
        { "id": "gw", "type": "Gateway", "properties": { "strategy": { "kind":"parallel" } } },
        { "id": "a", "type": "Task" },
        { "id": "b", "type": "Task" },
        { "id": "endA", "type": "End" },
        { "id": "endB", "type": "End" }
      ],
      "edges": [
        { "id": "e1", "source": "start", "target": "gw" },
        { "id": "e2", "source": "gw", "target": "a" },
        { "id": "e3", "source": "gw", "target": "b" },
        { "id": "e4", "source": "a", "target": "endA" },
        { "id": "e5", "source": "b", "target": "endB" }
      ]
    }
    """;

    [Fact]
    public async Task DeclaredParallelGateway_Should_Emit_GatewayEvaluated_With_All_Branches_And_EdgeTraversed_Events()
    {
        var def = SeedDefinition(ParallelDeclaredJson);
        ClearExecutors();
        AddSimpleExecutors();

        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        var events = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id)
            .ToList();

        var gwEval = events.FirstOrDefault(e => e.Name == "GatewayEvaluated");
        gwEval.Should().NotBeNull();

        using var doc = JsonDocument.Parse(gwEval!.Data);
        var selectedTargets = doc.RootElement.GetProperty("selected")
            .EnumerateArray().Select(x => x.GetString()).ToList();
        selectedTargets.Should().BeEquivalentTo(new[] { "a", "b" });

        var traversedTargets = events.Where(e => e.Name == "EdgeTraversed")
            .Select(e =>
            {
                using var d = JsonDocument.Parse(e.Data);
                return d.RootElement.TryGetProperty("to", out var toEl) ? toEl.GetString() : null;
            })
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        traversedTargets.Should().Contain("a");
        traversedTargets.Should().Contain("b");

        var parallelEdgeModes = events.Where(e => e.Name == "EdgeTraversed")
            .Select(e =>
            {
                using var d = JsonDocument.Parse(e.Data);
                return d.RootElement.TryGetProperty("mode", out var m) ? m.GetString() : "";
            })
            .Distinct()
            .ToList();

        parallelEdgeModes.Should().ContainSingle(m => m == "AutoAdvanceParallel");
    }

    private static string ParallelGatewayTypeJson => """
    {
      "nodes": [
        { "id": "start", "type": "Start" },
        { "id": "gw", "type": "Gateway", "properties": { "gatewayType":"parallel" } },
        { "id": "t1", "type": "Task" },
        { "id": "t2", "type": "Task" },
        { "id": "e1", "type": "End" },
        { "id": "e2", "type": "End" }
      ],
      "edges": [
        { "id": "s-g", "source": "start", "target": "gw" },
        { "id": "g-t1", "source": "gw", "target": "t1" },
        { "id": "g-t2", "source": "gw", "target": "t2" },
        { "id": "t1-e1", "source": "t1", "target": "e1" },
        { "id": "t2-e2", "source": "t2", "target": "e2" }
      ]
    }
    """;

    [Fact]
    public async Task DeclaredParallelGateway_Using_GatewayType_Should_Emit_All_Branch_Events()
    {
        var def = SeedDefinition(ParallelGatewayTypeJson);
        ClearExecutors();
        AddSimpleExecutors();

        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        var events = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id)
            .ToList();

        var gwEval = events.SingleOrDefault(e => e.Name == "GatewayEvaluated");
        gwEval.Should().NotBeNull("gatewayType=parallel must produce evaluation event");

        using var doc = JsonDocument.Parse(gwEval!.Data);
        var selected = doc.RootElement.GetProperty("selected")
            .EnumerateArray().Select(x => x.GetString()).ToList();
        selected.Should().BeEquivalentTo(new[] { "t1", "t2" });

        var traversedTargets = events.Where(e => e.Name == "EdgeTraversed")
            .Select(e =>
            {
                using var d = JsonDocument.Parse(e.Data);
                return d.RootElement.TryGetProperty("to", out var toEl) ? toEl.GetString() : null;
            })
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();
        traversedTargets.Should().Contain(new[] { "t1", "t2" });

        events.Where(e => e.Name == "EdgeTraversed")
            .Select(e =>
            {
                using var d = JsonDocument.Parse(e.Data);
                return d.RootElement.TryGetProperty("mode", out var m) ? m.GetString() : "";
            })
            .Distinct()
            .Should().ContainSingle(m => m == "AutoAdvanceParallel");
    }

    private static string ParallelStringStrategyJson => """
    {
      "nodes": [
        { "id": "start", "type": "Start" },
        { "id": "gw", "type": "Gateway", "properties": { "strategy":"parallel" } },
        { "id": "x", "type": "Task" },
        { "id": "y", "type": "Task" },
        { "id": "endX", "type": "End" },
        { "id": "endY", "type": "End" }
      ],
      "edges": [
        { "id": "sg", "source": "start", "target": "gw" },
        { "id": "g-x", "source": "gw", "target": "x" },
        { "id": "g-y", "source": "gw", "target": "y" },
        { "id": "x-end", "source": "x", "target": "endX" },
        { "id": "y-end", "source": "y", "target": "endY" }
      ]
    }
    """;

    [Fact]
    public async Task DeclaredParallelGateway_Using_StringStrategy_Should_FanOut_All_Branches()
    {
        var def = SeedDefinition(ParallelStringStrategyJson);
        ClearExecutors();
        AddSimpleExecutors();

        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        var events = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id)
            .ToList();

        var gwEval = events.SingleOrDefault(e => e.Name == "GatewayEvaluated");
        gwEval.Should().NotBeNull();

        using var doc = JsonDocument.Parse(gwEval!.Data);
        doc.RootElement.GetProperty("selected")
            .EnumerateArray().Select(e => e.GetString())
            .Should().BeEquivalentTo(new[] { "x", "y" });

        var modes = events.Where(e => e.Name == "EdgeTraversed")
            .Select(e =>
            {
                using var d = JsonDocument.Parse(e.Data);
                return d.RootElement.TryGetProperty("mode", out var m) ? m.GetString() : "";
            })
            .Distinct()
            .ToList();
        modes.Should().ContainSingle(m => m == "AutoAdvanceParallel");
    }
}
