using System.Net;
using System.Net.Http;
using FluentAssertions;
using Moq;
using Microsoft.Extensions.Options;
using WorkflowService.Domain.Models;
using WorkflowService.Engine;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using Xunit;

namespace WorkflowService.Tests.Engine;

public class AutomaticActionsTests : TestBase
{
    private WorkflowRuntime CreateRuntime(INodeExecutor automaticExecutor)
    {
        MockTenantProvider.Setup(t => t.GetCurrentTenantIdAsync()).ReturnsAsync(1);

        var condition = new Mock<IConditionEvaluator>();
        condition.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
                 .ReturnsAsync(true);

        var taskNotify = new Mock<ITaskNotificationDispatcher>();
        var runtimeLogger = CreateMockLogger<WorkflowRuntime>();
        var startEndLogger = CreateMockLogger<StartEndExecutor>();

        var executors = new List<INodeExecutor>
        {
            new StartEndExecutor(startEndLogger.Object),
            automaticExecutor
        };

        return new WorkflowRuntime(
            DbContext,
            executors,
            MockTenantProvider.Object,
            condition.Object,
            taskNotify.Object,
            runtimeLogger.Object);
    }

    private int AddDefinition(string json)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "AutoTest",
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

    // Top-level action placement (no nested "properties")
    private static string BuildDefinition(string actionFragment) =>
$@"{{
  ""nodes"":[
    {{""id"":""start"",""type"":""start""}},
    {{""id"":""auto1"",""type"":""automatic"",{actionFragment}}},
    {{""id"":""end"",""type"":""end""}}
  ],
  ""edges"":[
    {{""id"":""e1"",""from"":""start"",""to"":""auto1""}},
    {{""id"":""e2"",""from"":""auto1"",""to"":""end""}}
  ]
}}";

    private class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        public StubHttpClientFactory(HttpMessageHandler handler) => _handler = handler;
        public HttpClient CreateClient(string name) => new HttpClient(_handler, disposeHandler: false);
    }

    private class StaticResponseHandler : HttpMessageHandler
    {
        private readonly HttpStatusCode _code;
        private readonly string _body;
        public StaticResponseHandler(HttpStatusCode code, string body = "{}")
        {
            _code = code;
            _body = body;
        }
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var resp = new HttpResponseMessage(_code) { Content = new StringContent(_body) };
            return Task.FromResult(resp);
        }
    }

    private AutomaticExecutor CreateAutomaticExecutor(IEnumerable<IAutomaticActionExecutor> actionExecutors, bool trace = false)
    {
        var registry = new AutomaticActionRegistry(actionExecutors);
        var loggerAuto = CreateMockLogger<AutomaticExecutor>();
        var publisherLogger = CreateMockLogger<EventPublisher>();
        IEventPublisher publisher = new EventPublisher(DbContext, publisherLogger.Object);

        // Diagnostics dependencies (new constructor params)
        var opts = Options.Create(new WorkflowDiagnosticsOptions
        {
            EnableAutomaticTrace = trace,
            AutomaticBufferSize = 50
        });
        IAutomaticDiagnosticsBuffer diagBuffer = new AutomaticDiagnosticsBuffer(opts);

        return new AutomaticExecutor(
            loggerAuto.Object,
            registry,
            publisher,
            diagBuffer,
            opts);
    }

    private void Preflight(string json)
    {
        var parsed = BuilderDefinitionAdapter.Parse(json);
        parsed.Nodes.Count.Should().Be(3);
        parsed.Edges.Count.Should().Be(2);
        parsed.Edges.Select(e => $"{e.Source}->{e.Target}")
            .Should().Contain("start->auto1");
    }

    [Fact]
    public async Task Automatic_UnknownExecutor_Should_Fail_Instance()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""mystery""}");
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var autoExec = CreateAutomaticExecutor(new[] { new NoopAutomaticActionExecutor() });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", null, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Failed);
        var automaticEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Automatic")
            .Select(e => e.Name)
            .ToList();

        automaticEvents.Should().Contain("AutomaticActionExecutorMissing");
        automaticEvents.Should().NotContain("AutomaticActionCompleted");
    }

    [Fact]
    public async Task Automatic_NoOp_Should_Complete_Instance()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""noop""}");
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var autoExec = CreateAutomaticExecutor(new[] { new NoopAutomaticActionExecutor() });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", 11, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Completed);

        var events = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Automatic")
            .Select(e => e.Name)
            .ToList();

        events.Should().Contain("AutomaticActionStarted");
        events.Should().Contain("AutomaticActionCompleted");
    }

    [Fact]
    public async Task Automatic_UnknownExecutor_With_ProceedPolicy_Should_Complete_Instance()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""missingKind"",""onFailure"":""proceed""}");
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var autoExec = CreateAutomaticExecutor(new[] { new NoopAutomaticActionExecutor() });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", 7, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Completed);

        var automaticEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Automatic")
            .Select(e => e.Name)
            .ToList();

        automaticEvents.Should().Contain("AutomaticActionExecutorMissing");
        automaticEvents.Should().NotContain("AutomaticActionFailed"); // proceed path converts to success
    }

    [Fact]
    public async Task Automatic_Webhook_Success_Should_Complete_Instance()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""webhook"",""config"":{""url"":""https://example.com/hook"",""method"":""POST""}}");
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var handler = new StaticResponseHandler(HttpStatusCode.OK, "{\"ok\":true}");
        var httpFactory = new StubHttpClientFactory(handler);
        var webhookExec = new WebhookAutomaticActionExecutor(httpFactory, CreateMockLogger<WebhookAutomaticActionExecutor>().Object);

        var autoExec = CreateAutomaticExecutor(new IAutomaticActionExecutor[]
        {
            new NoopAutomaticActionExecutor(),
            webhookExec
        });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", null, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Completed);

        var automaticEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Automatic")
            .Select(e => e.Name)
            .ToList();

        automaticEvents.Should().Contain("AutomaticActionCompleted");
    }

    [Fact]
    public async Task Automatic_Webhook_NonSuccess_With_AllowNonSuccess_Should_Complete_Instance()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""webhook"",""config"":{""url"":""https://example.com/hook"",""method"":""GET"",""allowNonSuccess"":true}}");
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var handler = new StaticResponseHandler(HttpStatusCode.InternalServerError, "{\"error\":\"boom\"}");
        var httpFactory = new StubHttpClientFactory(handler);
        var webhookExec = new WebhookAutomaticActionExecutor(httpFactory, CreateMockLogger<WebhookAutomaticActionExecutor>().Object);

        var autoExec = CreateAutomaticExecutor(new IAutomaticActionExecutor[]
        {
            new NoopAutomaticActionExecutor(),
            webhookExec
        });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", null, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Completed);

        var automaticEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Automatic")
            .Select(e => e.Name)
            .ToList();

        automaticEvents.Should().Contain("AutomaticActionCompleted");
    }

    [Fact]
    public async Task Automatic_UnknownExecutor_With_SuspendPolicy_Should_Suspend_Instance()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""missingKind"",""onFailure"":""suspend""}");
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var autoExec = CreateAutomaticExecutor(new[] { new NoopAutomaticActionExecutor() });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", 42, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Suspended, "suspend policy should pause instance");
        inst.ErrorMessage.Should().NotBeNullOrEmpty();

        var automaticEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Automatic")
            .Select(e => e.Name)
            .ToList();

        automaticEvents.Should().Contain("AutomaticActionExecutorMissing");
        automaticEvents.Should().NotContain("AutomaticActionCompleted");

        var instanceEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Instance")
            .Select(e => e.Name)
            .ToList();

        instanceEvents.Should().Contain("Suspended");
    }

    // NEW: Explicit test verifying Instance Failed event emitted for failInstance/default policy
    [Fact]
    public async Task Automatic_UnknownExecutor_Should_Emit_InstanceFailed_Event()
    {
        var defJson = BuildDefinition(@"""action"":{""kind"":""notRegistered""}"); // default policy = failInstance
        Preflight(defJson);
        var defId = AddDefinition(defJson);

        var autoExec = CreateAutomaticExecutor(new[] { new NoopAutomaticActionExecutor() });
        var runtime = CreateRuntime(autoExec);

        var inst = await runtime.StartWorkflowAsync(defId, "{}", null, autoCommit: true);

        inst.Status.Should().Be(DTOs.Workflow.Enums.InstanceStatus.Failed);

        var instanceEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Instance")
            .Select(e => e.Name)
            .ToList();

        instanceEvents.Should().Contain("Failed", "a failing automatic action with failInstance policy must emit Instance Failed event");

        var nodeFailEvents = DbContext.WorkflowEvents
            .Where(e => e.WorkflowInstanceId == inst.Id && e.Type == "Node" && e.Name == "Failed")
            .ToList();

        nodeFailEvents.Should().HaveCountGreaterThan(0, "node-level failure should also be recorded");
    }
}
