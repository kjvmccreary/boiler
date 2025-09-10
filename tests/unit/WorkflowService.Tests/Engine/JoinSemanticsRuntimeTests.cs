using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using Contracts.Services;
using DTOs.Workflow.Enums;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Xunit;

namespace WorkflowService.Tests.Engine;

/// <summary>
/// Backend parity tests for join-mode semantics (runtime) matching Simulation PR3.
/// Covers: all | any(cancelRemaining true/false) | count | quorum(percent) | expression.
/// </summary>
public class JoinSemanticsRuntimeTests
{
    #region Test Stubs

    private sealed class FixedTenantProvider : ITenantProvider
    {
        public Task<int?> GetCurrentTenantIdAsync() => Task.FromResult<int?>(1);
        public Task<string?> GetCurrentTenantIdentifierAsync() => Task.FromResult<string?>("tenant-1");
        public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
        public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
        public Task ClearCurrentTenantAsync() => Task.CompletedTask;
        public bool HasTenantContext => true;
    }

    /// <summary>
    /// Minimal condition evaluator:
    ///  - Empty => true
    ///  - Supports a single JsonLogic form: {">=":[{"var":"_joinEval.arrived"},X]}
    ///    Sufficient for expression join test without pulling full engine.
    /// </summary>
    private sealed class MiniConditionEvaluator : IConditionEvaluator
    {
        public Task<bool> EvaluateAsync(string condition, string contextJson)
        {
            if (string.IsNullOrWhiteSpace(condition)) return Task.FromResult(true);
            try
            {
                using var rule = JsonDocument.Parse(condition);
                using var ctx = JsonDocument.Parse(string.IsNullOrWhiteSpace(contextJson) ? "{}" : contextJson);
                if (rule.RootElement.TryGetProperty(">=", out var arr) &&
                    arr.ValueKind == JsonValueKind.Array &&
                    arr.GetArrayLength() == 2 &&
                    arr[0].ValueKind == JsonValueKind.Object &&
                    arr[0].TryGetProperty("var", out var varEl) &&
                    varEl.GetString() == "_joinEval.arrived")
                {
                    var thresholdEl = arr[1];
                    if (thresholdEl.ValueKind == JsonValueKind.Number &&
                        ctx.RootElement.TryGetProperty("_joinEval", out var je) &&
                        je.TryGetProperty("arrived", out var arrivedEl) &&
                        arrivedEl.ValueKind == JsonValueKind.Number)
                    {
                        return Task.FromResult(arrivedEl.GetInt32() >= thresholdEl.GetInt32());
                    }
                }
            }
            catch
            {
                return Task.FromResult(false);
            }
            return Task.FromResult(false);
        }

        public Task<object?> EvaluateExpressionAsync(string expression, string contextData) =>
            Task.FromResult<object?>(null);

        public bool ValidateCondition(string condition) => true;
    }

    private sealed class NullTaskNotifier : ITaskNotificationDispatcher
    {
        public Task NotifyTenantAsync(int tenantId, CancellationToken ct = default) => Task.CompletedTask;
        public Task NotifyUserAsync(int tenantId, int userId, CancellationToken ct = default) => Task.CompletedTask;
    }

    private sealed class NullInstanceNotifier : IWorkflowNotificationDispatcher
    {
        public Task NotifyInstanceAsync(WorkflowInstance instance, CancellationToken ct = default) => Task.CompletedTask;
        public Task NotifyInstanceAsync(int tenantId, int instanceId, InstanceStatus status, string currentNodeIds,
            DateTime? completedAt, string? errorMessage, CancellationToken ct = default) => Task.CompletedTask;
        public Task NotifyInstancesChangedAsync(int tenantId, CancellationToken ct = default) => Task.CompletedTask;
        public Task NotifyInstanceProgressAsync(int tenantId, int instanceId, int percentage, int visitedCount,
            int totalNodes, string status, IEnumerable<string> activeNodeIds, CancellationToken ct = default) => Task.CompletedTask;
    }

    /// <summary>
    /// Automatic executor for nodes of type 'Automatic'.
    /// </summary>
    private sealed class AutomaticNodeExecutor : INodeExecutor
    {
        public string NodeType => "Automatic";
        public bool CanExecute(WorkflowService.Domain.Dsl.WorkflowNode node) =>
            node.Type.Equals("Automatic", StringComparison.OrdinalIgnoreCase);

        public Task<NodeExecutionResult> ExecuteAsync(
            WorkflowService.Domain.Dsl.WorkflowNode node,
            WorkflowInstance instance,
            string context,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(new NodeExecutionResult
            {
                IsSuccess = true,
                ShouldWait = false
            });
    }

    #endregion

    #region Harness

    private WorkflowDbContext BuildDb(out WorkflowRuntime runtime)
    {
        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .EnableSensitiveDataLogging()
            .Options;

        var http = new HttpContextAccessor { HttpContext = new DefaultHttpContext() };
        var tenantProvider = new FixedTenantProvider();
        var dbLogger = NullLogger<WorkflowDbContext>.Instance;

        var db = new WorkflowDbContext(options, http, tenantProvider, dbLogger);

        runtime = new WorkflowRuntime(
            db,
            new List<INodeExecutor> { new AutomaticNodeExecutor() },
            tenantProvider,
            new MiniConditionEvaluator(),
            new NullTaskNotifier(),
            NullLogger<WorkflowRuntime>.Instance,
            new NullInstanceNotifier());

        return db;
    }

    private WorkflowDefinition AddDefinition(WorkflowDbContext db, string jsonDsl, string name = "JoinTest")
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = name,
            Version = 1,
            JSONDefinition = jsonDsl,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        db.WorkflowDefinitions.Add(def);
        db.SaveChanges();
        return def;
    }

    private static string ParallelJoinTemplate(
        int branchCount,
        string joinMode,
        bool cancelRemaining = false,
        int? count = null,
        int? thresholdCount = null,
        double? thresholdPercent = null,
        string? expression = null)
    {
        var nodes = new JsonArray
        {
            Node("start","Start"),
            Gateway("gw","parallel")
        };

        for (int i = 1; i <= branchCount; i++)
            nodes.Add(Node($"b{i}", "Automatic"));

        var joinObj = new JsonObject
        {
            ["id"] = "j",
            ["type"] = "Join",
            ["gatewayId"] = "gw",
            ["mode"] = joinMode,
            ["cancelRemaining"] = cancelRemaining
        };
        if (count.HasValue) joinObj["count"] = count.Value;
        if (thresholdCount.HasValue) joinObj["thresholdCount"] = thresholdCount.Value;
        if (thresholdPercent.HasValue) joinObj["thresholdPercent"] = thresholdPercent.Value;
        if (!string.IsNullOrWhiteSpace(expression)) joinObj["expression"] = expression;
        nodes.Add(joinObj);
        nodes.Add(Node("end","End"));

        var edges = new JsonArray { Edge("start","gw") };
        for (int i = 1; i <= branchCount; i++)
        {
            edges.Add(Edge("gw", $"b{i}"));
            edges.Add(Edge($"b{i}", "j"));
        }
        edges.Add(Edge("j","end"));

        return new JsonObject
        {
            ["nodes"] = nodes,
            ["edges"] = edges
        }.ToJsonString();

        static JsonObject Node(string id, string type) => new() { ["id"] = id, ["type"] = type };
        static JsonObject Gateway(string id, string strategy) => new() { ["id"] = id, ["type"] = "Gateway", ["strategy"] = strategy };
        static JsonObject Edge(string from, string to) => new() { ["id"] = $"{from}->{to}", ["from"] = from, ["to"] = to };
    }

    private Task<WorkflowInstance> StartAsync(WorkflowRuntime runtime, int defId) =>
        runtime.StartWorkflowAsync(defId, "{}", startedByUserId: 99, CancellationToken.None);

    private static List<WorkflowEvent> EventsFor(WorkflowDbContext db, int instanceId, string type, string name) =>
        db.WorkflowEvents
          .Where(e => e.WorkflowInstanceId == instanceId && e.Type == type && e.Name == name)
          .ToList();

    private static JsonDocument Parse(string json) =>
        JsonDocument.Parse(string.IsNullOrWhiteSpace(json) ? "{}" : json);

    #endregion

    #region Tests

    [Fact]
    public async Task Join_All_Mode_ShouldRequireAllBranches()
    {
        var db = BuildDb(out var runtime);
        var def = AddDefinition(db, ParallelJoinTemplate(3, "all"));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        var satisfied = EventsFor(db, instance.Id, "Parallel", "ParallelJoinSatisfied").Single();
        using var doc = Parse(satisfied.Data);
        doc.RootElement.GetProperty("mode").GetString().Should().Be("all");
        doc.RootElement.GetProperty("arrivals").EnumerateArray().Count().Should().Be(3);
    }

    [Fact]
    public async Task Join_Any_CancelRemaining_ShouldFireAfterFirstArrival_AndCancelOthers()
    {
        var db = BuildDb(out var runtime);
        var def = AddDefinition(db, ParallelJoinTemplate(3, "any", cancelRemaining: true));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        var satisfied = EventsFor(db, instance.Id, "Parallel", "ParallelJoinSatisfied").Single();
        using var doc = Parse(satisfied.Data);
        doc.RootElement.GetProperty("mode").GetString().Should().Be("any");
        doc.RootElement.GetProperty("arrivals").EnumerateArray().Count().Should().Be(1);

        EventsFor(db, instance.Id, "Parallel", "ParallelJoinBranchCancelled")
            .Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Join_Count_Mode_ShouldSatisfyAtThreshold()
    {
        var db = BuildDb(out var runtime);
        var def = AddDefinition(db, ParallelJoinTemplate(3, "count", cancelRemaining: true, count: 2));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        var satisfied = EventsFor(db, instance.Id, "Parallel", "ParallelJoinSatisfied").Single();
        using var doc = Parse(satisfied.Data);
        doc.RootElement.GetProperty("mode").GetString().Should().Be("count");
        doc.RootElement.GetProperty("arrivals").EnumerateArray().Count().Should().Be(2);

        EventsFor(db, instance.Id, "Parallel", "ParallelJoinBranchCancelled")
            .Count.Should().BeGreaterThanOrEqualTo(1);
    }

    [Fact]
    public async Task Join_Quorum_Percent_ShouldComputeEffectiveThreshold()
    {
        var db = BuildDb(out var runtime);
        var def = AddDefinition(db, ParallelJoinTemplate(4, "quorum", cancelRemaining: true, thresholdPercent: 50));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        var satisfied = EventsFor(db, instance.Id, "Parallel", "ParallelJoinSatisfied").Single();
        using var doc = Parse(satisfied.Data);
        doc.RootElement.GetProperty("mode").GetString().Should().Be("quorum");
        doc.RootElement.GetProperty("arrivals").EnumerateArray().Count().Should().Be(2);
        doc.RootElement.TryGetProperty("quorumThresholdCount", out var qc).Should().BeTrue();
        qc.GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task Join_Expression_Mode_ShouldUseJoinEvalOverlay()
    {
        var db = BuildDb(out var runtime);
        var expr = "{\">=\":[{\"var\":\"_joinEval.arrived\"},2]}";
        var def = AddDefinition(db, ParallelJoinTemplate(3, "expression", cancelRemaining: true, expression: expr));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        var satisfied = EventsFor(db, instance.Id, "Parallel", "ParallelJoinSatisfied").Single();
        using var doc = Parse(satisfied.Data);
        doc.RootElement.GetProperty("mode").GetString().Should().Be("expression");
        doc.RootElement.GetProperty("arrivals").EnumerateArray().Count().Should().Be(2);
    }

    [Fact]
    public async Task Join_All_Mode_NoCancelRemaining_ShouldNotEmitBranchCancelledEvents()
    {
        var db = BuildDb(out var runtime);
        var def = AddDefinition(db, ParallelJoinTemplate(2, "all", cancelRemaining: false));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        EventsFor(db, instance.Id, "Parallel", "ParallelJoinBranchCancelled").Should().BeEmpty();
    }

    [Fact]
    public async Task Join_Any_NoCancelRemaining_ShouldNotCancelOtherBranches()
    {
        var db = BuildDb(out var runtime);
        var def = AddDefinition(db, ParallelJoinTemplate(3, "any", cancelRemaining: false));
        var instance = await StartAsync(runtime, def.Id);

        instance.Status.Should().Be(InstanceStatus.Completed);
        EventsFor(db, instance.Id, "Parallel", "ParallelJoinBranchCancelled").Should().BeEmpty();
    }

    #endregion
}
