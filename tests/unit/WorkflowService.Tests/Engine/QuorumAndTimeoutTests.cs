using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.Pruning;
using WorkflowService.Engine.Timeouts;
using WorkflowService.Persistence;                  // ADDED
using WorkflowService.Services.Interfaces;
using Contracts.Services;
using Xunit;

namespace WorkflowService.Tests.Engine;

/// <summary>
/// Tests quorum + timeout coordination behavior.
/// </summary>
/// <remarks>
/// Covers:
/// - D3 Quorum join behavior (below / exact threshold & late arrivals)
/// - D4 Timeout path (force | route | fail)
/// </remarks>
public class QuorumAndTimeoutTests : TestBase
{
    private WorkflowRuntime CreateRuntime(IEnumerable<INodeExecutor>? injected = null)
    {
        var cond = new Mock<IConditionEvaluator>();
        cond.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var publisher = new Mock<IEventPublisher>();
        var diagOpts = Options.Create(new WorkflowDiagnosticsOptions
        {
            EnableAutomaticTrace = false,
            AutomaticBufferSize = 8
        });
        IAutomaticDiagnosticsBuffer diagBuffer = new AutomaticDiagnosticsBuffer(diagOpts);

        var autoReg = new AutomaticActionRegistry(new IAutomaticActionExecutor[]
        {
            new NoopAutomaticActionExecutor()
        });

        var automaticExecutor = new AutomaticExecutor(
            CreateMockLogger<AutomaticExecutor>().Object,
            autoReg,
            publisher.Object,
            diagBuffer,
            diagOpts);

        var hasher = new Mock<IDeterministicHasher>();
        hasher.SetupGet(h => h.Algorithm).Returns("TestHasher");
        hasher.SetupGet(h => h.Seed).Returns(1UL);
        hasher.Setup(h => h.Hash(It.IsAny<string?>())).Returns(123UL);
        hasher.Setup(h => h.HashComposite(It.IsAny<string?[]>())).Returns(123UL);
        hasher.Setup(h => h.ToBucket(It.IsAny<ulong>(), It.IsAny<int>()))
              .Returns<ulong, int>((value, buckets) => (int)(value % (uint)buckets));

        var pruner = new Mock<IWorkflowContextPruner>();
        pruner.Setup(p => p.PruneGatewayHistory(It.IsAny<JsonArray>())).Returns(0);
        var prunerEvents = new Mock<IGatewayPruningEventEmitter>();
        var expEmitter = new Mock<IExperimentAssignmentEmitter>();

        IGatewayStrategyRegistry stratReg = new GatewayStrategyRegistry(new IGatewayStrategy[]
        {
            new ExclusiveGatewayStrategy(),
            new ParallelGatewayStrategy()
        });

        var gateway = new GatewayEvaluator(
            cond.Object,
            CreateMockLogger<GatewayEvaluator>().Object,
            stratReg,
            hasher.Object,
            pruner.Object,
            prunerEvents.Object,
            expEmitter.Object);

        var executors = injected?.ToList() ?? new List<INodeExecutor>
        {
            new StartEndExecutor(CreateMockLogger<StartEndExecutor>().Object),
            automaticExecutor,
            gateway,
            new TimerExecutor(CreateMockLogger<TimerExecutor>().Object),
            new JoinExecutor(CreateMockLogger<JoinExecutor>().Object)
        };

        var notifier = new Mock<ITaskNotificationDispatcher>();
        var roleService = new Mock<IRoleService>(); // for HumanTaskExecutor

        var humanExecutor = new HumanTaskExecutor(
            CreateMockLogger<HumanTaskExecutor>().Object,
            roleService.Object,
            notifier.Object,
            DbContext);

        // Insert human executor into pipeline
        if (executors.All(e => e.GetType() != typeof(HumanTaskExecutor)))
            executors.Insert(2, humanExecutor);

        return new WorkflowRuntime(
            DbContext,
            executors,
            MockTenantProvider.Object,
            cond.Object,
            notifier.Object,
            CreateMockLogger<WorkflowRuntime>().Object);
    }

    private WorkflowDefinition SeedDefinition(string json)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "QuorumDef",
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

    // Parallel gateway with 4 branches: auto1/auto2 automatic, h1/h2 human -> join
    private string QuorumWorkflowJson(double thresholdPercent = 50) =>
$@"{{
  ""nodes"": [
    {{""id"":""start"",""type"":""start""}},
    {{""id"":""gw"",""type"":""gateway"",""properties"":{{""strategy"":""parallel""}}}},
    {{""id"":""auto1"",""type"":""automatic"",""properties"":{{""action"":{{""kind"":""noop""}}}}}},
    {{""id"":""auto2"",""type"":""automatic"",""properties"":{{""action"":{{""kind"":""noop""}}}}}},
    {{""id"":""h1"",""type"":""humanTask"",""properties"":{{""label"":""H1""}}}},
    {{""id"":""h2"",""type"":""humanTask"",""properties"":{{""label"":""H2""}}}},
    {{""id"":""join"",""type"":""join"",""properties"":{{
        ""gatewayId"":""gw"",
        ""mode"":""quorum"",
        ""thresholdPercent"": {thresholdPercent},
        ""cancelRemaining"": false
    }}}},
    {{""id"":""end"",""type"":""end""}}
  ],
  ""edges"": [
    {{""id"":""e1"",""from"":""start"",""to"":""gw""}},
    {{""id"":""e2"",""from"":""gw"",""to"":""auto1""}},
    {{""id"":""e3"",""from"":""gw"",""to"":""auto2""}},
    {{""id"":""e4"",""from"":""gw"",""to"":""h1""}},
    {{""id"":""e5"",""from"":""gw"",""to"":""h2""}},
    {{""id"":""e6"",""from"":""auto1"",""to"":""join""}},
    {{""id"":""e7"",""from"":""auto2"",""to"":""join""}},
    {{""id"":""e8"",""from"":""h1"",""to"":""join""}},
    {{""id"":""e9"",""from"":""h2"",""to"":""join""}},
    {{""id"":""e10"",""from"":""join"",""to"":""end""}}
  ]
}}";

    [Fact]
    public async Task D3_Quorum_ExactThreshold_SatisfiedAfterAutos()
    {
        MockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        var def = SeedDefinition(QuorumWorkflowJson(50)); // ceil(4 * 0.5)=2
        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        DbContext.ChangeTracker.Clear();
        var events = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Parallel")
            .ToList();

        events.Any(e => e.Name == "ParallelJoinSatisfied").Should().BeTrue();

        // Complete remaining human tasks
        var humanTasks = DbContext.WorkflowTasks.Where(t => t.WorkflowInstanceId == inst.Id).ToList();
        foreach (var ht in humanTasks)
        {
            ht.Status = DTOs.Workflow.Enums.TaskStatus.Claimed;
            DbContext.SaveChanges();
            await rt.CompleteTaskAsync(ht.Id, "{}", 123, CancellationToken.None);
        }

        var satisfiedEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Name == "ParallelJoinSatisfied")
            .ToList();
        satisfiedEvents.Count.Should().Be(1);
    }

    [Fact]
    public async Task D3_Quorum_BelowThreshold_NotSatisfiedUntilHumanCompletes()
    {
        MockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        var def = SeedDefinition(QuorumWorkflowJson(75)); // ceil(4 * .75)=3
        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");

        DbContext.ChangeTracker.Clear();
        DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Name == "ParallelJoinSatisfied")
            .Should().BeEmpty();

        // Complete one human (3 arrivals)
        var human = DbContext.WorkflowTasks.First(t => t.NodeId == "h1");
        human.Status = DTOs.Workflow.Enums.TaskStatus.Claimed;
        DbContext.SaveChanges();
        await rt.CompleteTaskAsync(human.Id, "{}", 99, CancellationToken.None);

        var satisfied = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Name == "ParallelJoinSatisfied")
            .ToList();
        satisfied.Count.Should().Be(1);
    }

    private string TimeoutWorkflowJson(string action) =>
$@"{{
  ""nodes"": [
    {{""id"":""start"",""type"":""start""}},
    {{""id"":""gw"",""type"":""gateway"",""properties"":{{""strategy"":""parallel""}}}},
    {{""id"":""slow"",""type"":""humanTask"",""properties"":{{""label"":""Slow""}}}},
    {{""id"":""fast"",""type"":""automatic"",""properties"":{{""action"":{{""kind"":""noop""}}}}}},
    {{""id"":""join"",""type"":""join"",""properties"":{{
      ""gatewayId"":""gw"",
      ""mode"":""all"",
      ""timeout"":{{""seconds"":2,""onTimeout"":""{action}"",""target"":""timeoutHandler""}}
    }}}},
    {{""id"":""timeoutHandler"",""type"":""automatic"",""properties"":{{""action"":{{""kind"":""noop""}}}}}},
    {{""id"":""end"",""type"":""end""}}
  ],
  ""edges"": [
    {{""id"":""e1"",""from"":""start"",""to"":""gw""}},
    {{""id"":""e2"",""from"":""gw"",""to"":""slow""}},
    {{""id"":""e3"",""from"":""gw"",""to"":""fast""}},
    {{""id"":""e4"",""from"":""slow"",""to"":""join""}},
    {{""id"":""e5"",""from"":""fast"",""to"":""join""}},
    {{""id"":""e6"",""from"":""join"",""to"":""end""}},
    {{""id"":""e7"",""from"":""timeoutHandler"",""to"":""end""}}
  ]
}}";

    private async Task<(WorkflowRuntime rt, WorkflowInstance inst)> StartTimeoutScenario(string action)
    {
        MockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
        var def = SeedDefinition(TimeoutWorkflowJson(action));
        var rt = CreateRuntime();
        var inst = await rt.StartWorkflowAsync(def.Id, "{}");
        return (rt, inst);
    }

    [Fact]
    public async Task D4_Timeout_Force_CompletesJoin()
    {
        var (rt, inst) = await StartTimeoutScenario("force");
        await Task.Delay(2500);

        var opts = Options.Create(new JoinTimeoutOptions { ScanIntervalSeconds = 60, BatchSize = 50 });
        var worker = new JoinTimeoutWorker(new TestServiceProvider(DbContext), opts, CreateMockLogger<JoinTimeoutWorker>().Object);
        await InvokeWorkerScanAsync(worker);

        DbContext.ChangeTracker.Clear();
        DbContext.WorkflowEvents.Any(e => e.WorkflowInstanceId == inst.Id && e.Name == "ParallelJoinTimeout")
            .Should().BeTrue();
    }

    [Fact]
    public async Task D4_Timeout_Fail_FailsInstance()
    {
        var (rt, inst) = await StartTimeoutScenario("fail");
        await Task.Delay(2500);

        var worker = new JoinTimeoutWorker(new TestServiceProvider(DbContext),
            Options.Create(new JoinTimeoutOptions()),
            CreateMockLogger<JoinTimeoutWorker>().Object);
        await InvokeWorkerScanAsync(worker);

        DbContext.ChangeTracker.Clear();
        DbContext.WorkflowInstances.First(i => i.Id == inst.Id).Status
            .Should().Be(DTOs.Workflow.Enums.InstanceStatus.Failed);
    }

    [Fact]
    public async Task D4_Timeout_Route_ActivatesTimeoutHandler()
    {
        var (rt, inst) = await StartTimeoutScenario("route");
        await Task.Delay(2500);

        var worker = new JoinTimeoutWorker(new TestServiceProvider(DbContext),
            Options.Create(new JoinTimeoutOptions()),
            CreateMockLogger<JoinTimeoutWorker>().Object);
        await InvokeWorkerScanAsync(worker);

        DbContext.ChangeTracker.Clear();
        DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Parallel" && e.Name == "ParallelJoinTimeout")
            .Any(e => e.Data.Contains(@"""action"":""route""")).Should().BeTrue();
    }

    private static async Task InvokeWorkerScanAsync(JoinTimeoutWorker worker)
    {
        var m = typeof(JoinTimeoutWorker).GetMethod("ScanAsync",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (m == null) throw new InvalidOperationException("ScanAsync not found.");
        var task = (Task)m.Invoke(worker, new object[] { CancellationToken.None })!;
        await task;
    }

    private sealed class TestServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope
    {
        private readonly WorkflowDbContext _db;
        public TestServiceProvider(WorkflowDbContext db) => _db = db;

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(WorkflowDbContext)) return _db;
            if (serviceType == typeof(IServiceScopeFactory)) return this;
            if (serviceType == typeof(IServiceScope)) return this;
            return null;
        }

        public IServiceScope CreateScope() => this;
        public IServiceProvider ServiceProvider => this;
        public void Dispose() { }
    }
}
