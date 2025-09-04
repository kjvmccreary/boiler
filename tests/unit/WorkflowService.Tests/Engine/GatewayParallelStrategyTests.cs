using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services.Interfaces;
using Xunit;

namespace WorkflowService.Tests.Engine;

public class GatewayParallelStrategyTests : TestBase
{
    private readonly ILoggerFactory _loggerFactory;

    public GatewayParallelStrategyTests()
    {
        _loggerFactory = LoggerFactory.Create(b =>
        {
            b.SetMinimumLevel(LogLevel.Debug);
            b.AddFilter("Microsoft", LogLevel.Warning);
            b.AddFilter("System", LogLevel.Warning);
            b.AddConsole();
        });
    }

    private WorkflowRuntime CreateRuntime()
    {
        var condEval = new Mock<IConditionEvaluator>();
        condEval.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var mockPublisher = new Mock<IEventPublisher>();

        var diagOpts = Options.Create(new WorkflowDiagnosticsOptions
        {
            EnableAutomaticTrace = false,
            AutomaticBufferSize = 32
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

        var executors = new INodeExecutor[]
        {
            new StartEndExecutor(_loggerFactory.CreateLogger<StartEndExecutor>()),
            automaticExecutor,
            gatewayExecutor,
            new TimerExecutor(_loggerFactory.CreateLogger<TimerExecutor>())
        };

        var taskNotifier = new Mock<ITaskNotificationDispatcher>();

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
            Name = "ParallelGatewayTest",
            Version = 1,
            JSONDefinition = json,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();
        return def.Id;
    }

    private string BuildParallelGatewayDefinition() => """
{
  "nodes":[
    { "id":"start", "type":"start" },
    { "id":"gw", "type":"gateway", "properties": { "gatewayType": "parallel" } },
    { "id":"a", "type":"automatic", "properties": { "action": { "kind":"noop" } } },
    { "id":"b", "type":"automatic", "properties": { "action": { "kind":"noop" } } },
    { "id":"c", "type":"end" }
  ],
  "edges":[
    { "id":"e1", "from":"start", "to":"gw" },
    { "id":"e2", "from":"gw", "to":"a" },
    { "id":"e3", "from":"gw", "to":"b" },
    { "id":"e4", "from":"a", "to":"c" },
    { "id":"e5", "from":"b", "to":"c" }
  ]
}
""";

    [Fact]
    public async Task ParallelGateway_ShouldActivateAllOutgoingBranches()
    {
        // Arrange
        MockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);

        var json = BuildParallelGatewayDefinition();

        // Log raw definition JSON
        _loggerFactory.CreateLogger("Test").LogInformation("RAW_DEFINITION_JSON: {Json}", json);

        // (Optional) Quick sanity parse: ensure both e2 & e3 present as raw substring
        json.Contains("\"id\":\"e2\"").Should().BeTrue("definition must contain e2");
        json.Contains("\"id\":\"e3\"").Should().BeTrue("definition must contain e3");

        var defId = AddDefinition(json);
        var runtime = CreateRuntime();

        // Act
        var instance = await runtime.StartWorkflowAsync(defId, "{}", null, CancellationToken.None, autoCommit: true);

        // Reload instance
        DbContext.ChangeTracker.Clear();
        var reloaded = DbContext.WorkflowInstances.First(i => i.Id == instance.Id);

        // Dump all edge traversal events for diagnostics
        var allEdgeEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Edge" && e.Name == "EdgeTraversed")
            .OrderBy(e => e.Id)
            .Select(e => e.Data)
            .ToList();

        _loggerFactory.CreateLogger("Test").LogInformation("EDGE_EVENTS ({Count}): {Events}",
            allEdgeEvents.Count, string.Join(" | ", allEdgeEvents));

        // Show gateway evaluation events
        var gwEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == instance.Id && e.Type == "Gateway")
            .OrderBy(e => e.Id)
            .Select(e => e.Data)
            .ToList();

        _loggerFactory.CreateLogger("Test").LogInformation("GATEWAY_EVENTS ({Count}): {Events}",
            gwEvents.Count, string.Join(" | ", gwEvents));

        // Assert final status (expect workflow completed)
        reloaded.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Completed);

        // Assertions
        allEdgeEvents.Any(d => d.Contains("\"edgeId\":\"e2\"")).Should().BeTrue("edge e2 (gw->a) should have been traversed");
        allEdgeEvents.Any(d => d.Contains("\"edgeId\":\"e3\"")).Should().BeTrue(
            $"edge e3 (gw->b) should have been traversed. Captured: {string.Join(" | ", allEdgeEvents)}");
    }
}
