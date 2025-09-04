using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using Microsoft.Extensions.Options;
using Moq;
using WorkflowService.Services.Interfaces;          // For IEventPublisher, ITaskNotificationDispatcher
using Contracts.Services;                          // ADDED: For IRoleService
using DTOs.Workflow.Enums;
using WorkflowService.Persistence;

// Alias to disambiguate from System.Threading.Tasks.TaskStatus
using TaskStatusEnum = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Tests.Engine;

public class ParallelTrueCompletionTests : TestBase
{
    private readonly ILoggerFactory _loggerFactory;

    public ParallelTrueCompletionTests()
    {
        _loggerFactory = LoggerFactory.Create(b =>
        {
            b.SetMinimumLevel(LogLevel.Warning);
            b.AddConsole();
        });

        MockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);
    }

    private WorkflowRuntime CreateRuntime()
    {
        // Mocks
        var condEval      = new Mock<IConditionEvaluator>();
        var mockPublisher = new Mock<IEventPublisher>();
        var roleService   = new Mock<IRoleService>();
        var taskNotifier  = new Mock<ITaskNotificationDispatcher>();

        condEval.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var diagOpts = Options.Create(new WorkflowDiagnosticsOptions
        {
            EnableAutomaticTrace = false,
            AutomaticBufferSize = 16
        });
        IAutomaticDiagnosticsBuffer diagBuffer = new AutomaticDiagnosticsBuffer(diagOpts);

        var autoRegistry = new AutomaticActionRegistry(new IAutomaticActionExecutor[]
        {
            new NoopAutomaticActionExecutor()
        });

        var automaticExecutor = new AutomaticExecutor(
            _loggerFactory.CreateLogger<AutomaticExecutor>(),
            autoRegistry,
            mockPublisher.Object,
            diagBuffer,
            diagOpts);

        IGatewayStrategyRegistry gatewayStrategies = new GatewayStrategyRegistry(
            new IGatewayStrategy[] { new ExclusiveGatewayStrategy(), new ParallelGatewayStrategy() });

        var gatewayExecutor = new GatewayEvaluator(
            condEval.Object,
            _loggerFactory.CreateLogger<GatewayEvaluator>(),
            gatewayStrategies);

        // HumanTaskExecutor requires: ILogger, IRoleService, ITaskNotificationDispatcher, WorkflowDbContext
        var humanExecutor = new HumanTaskExecutor(
            _loggerFactory.CreateLogger<HumanTaskExecutor>(),
            roleService.Object,
            taskNotifier.Object,
            DbContext);

        var executors = new INodeExecutor[]
        {
            new StartEndExecutor(_loggerFactory.CreateLogger<StartEndExecutor>()),
            automaticExecutor,
            gatewayExecutor,
            new TimerExecutor(_loggerFactory.CreateLogger<TimerExecutor>()),
            humanExecutor
        };

        return new WorkflowRuntime(
            DbContext,
            executors,
            MockTenantProvider.Object,
            condEval.Object,
            taskNotifier.Object,
            _loggerFactory.CreateLogger<WorkflowRuntime>());
    }

    private int AddDefinition(string json)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "ParallelTrueCompletion",
            Version = 1,
            JSONDefinition = json,
            IsPublished = true,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def.Id;
    }

    private string ParallelHumanAutomatic() => """
{
  "nodes":[
    { "id":"start", "type":"start" },
    { "id":"gw", "type":"gateway", "properties": { "strategy":"parallel" } },
    { "id":"human", "type":"humanTask", "properties": { "label":"Approval" } },
    { "id":"auto", "type":"automatic", "properties": { "action": { "kind":"noop" } } },
    { "id":"endA", "type":"end" },
    { "id":"endB", "type":"end" }
  ],
  "edges":[
    { "id":"e1", "from":"start", "to":"gw" },
    { "id":"e2", "from":"gw", "to":"human" },
    { "id":"e3", "from":"gw", "to":"auto" },
    { "id":"e4", "from":"human", "to":"endA" },
    { "id":"e5", "from":"auto", "to":"endB" }
  ]
}
""";

    [Fact]
    public async Task ParallelTrueCompletion_MixedHumanAndAutomatic_WaitsForHuman()
    {
        var runtime = CreateRuntime();
        var defId = AddDefinition(ParallelHumanAutomatic());

        var instance = await runtime.StartWorkflowAsync(defId, "{}", null, CancellationToken.None, autoCommit: true);

        // After start, automatic branch should finish; instance should still be running (waiting on human)
        DbContext.ChangeTracker.Clear();
        var reloaded = DbContext.WorkflowInstances.First(i => i.Id == instance.Id);
        reloaded.Status.Should().Be(InstanceStatus.Running);

        var activeNodes = System.Text.Json.JsonSerializer.Deserialize<string[]>(reloaded.CurrentNodeIds)!;
        activeNodes.Should().ContainSingle(a => a.Equals("human", System.StringComparison.OrdinalIgnoreCase));

        // Human task should exist
        var humanTask = DbContext.WorkflowTasks
            .FirstOrDefault(t => t.WorkflowInstanceId == instance.Id && t.NodeId == "human");
        humanTask.Should().NotBeNull();
        humanTask!.Status.Should().Be(TaskStatusEnum.Created);

        // Simulate claim then complete
        humanTask.Status = TaskStatusEnum.Claimed;
        DbContext.SaveChanges();

        await runtime.CompleteTaskAsync(humanTask.Id, "{}", completedByUserId: 123, CancellationToken.None, autoCommit: true);

        DbContext.ChangeTracker.Clear();
        var finalInstance = DbContext.WorkflowInstances.First(i => i.Id == instance.Id);
        finalInstance.Status.Should().Be(InstanceStatus.Completed);

        // Ensure only one completion event
        var completionEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Name == "Completed" && e.Type == "Instance")
            .ToList();
        completionEvents.Count.Should().Be(1);
    }

    private string ParallelBothAutomatic() => """
{
  "nodes":[
    { "id":"start", "type":"start" },
    { "id":"gw", "type":"gateway", "properties": { "strategy": { "kind":"parallel" } } },
    { "id":"a1", "type":"automatic", "properties": { "action": { "kind":"noop" } } },
    { "id":"a2", "type":"automatic", "properties": { "action": { "kind":"noop" } } },
    { "id":"end1", "type":"end" },
    { "id":"end2", "type":"end" }
  ],
  "edges":[
    { "id":"e1", "from":"start", "to":"gw" },
    { "id":"e2", "from":"gw", "to":"a1" },
    { "id":"e3", "from":"gw", "to":"a2" },
    { "id":"e4", "from":"a1", "to":"end1" },
    { "id":"e5", "from":"a2", "to":"end2" }
  ]
}
""";

    [Fact]
    public async Task ParallelTrueCompletion_ShouldWaitForAllBranches()
    {
        var runtime = CreateRuntime();
        var defId = AddDefinition(ParallelBothAutomatic());

        var instance = await runtime.StartWorkflowAsync(defId, "{}", null, CancellationToken.None, autoCommit: true);

        DbContext.ChangeTracker.Clear();
        var finalInstance = DbContext.WorkflowInstances.First(i => i.Id == instance.Id);
        finalInstance.Status.Should().Be(InstanceStatus.Completed);

        var edgeEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Name == "EdgeTraversed")
            .Select(e => e.Data)
            .ToList();

        edgeEvents.Any(d => d.Contains("\"edgeId\":\"e4\"")).Should().BeTrue();
        edgeEvents.Any(d => d.Contains("\"edgeId\":\"e5\"")).Should().BeTrue();
    }
}
