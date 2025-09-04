using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.AutomaticActions.Executors;
using Xunit;

namespace WorkflowService.Tests.Engine;

public class WebhookAutomaticActionExecutorTokenTests
{
    private sealed class CapturingHandler : HttpMessageHandler
    {
        public HttpRequestMessage? LastRequest { get; private set; }
        private readonly HttpStatusCode _status;
        private readonly string _response;

        public CapturingHandler(HttpStatusCode status = HttpStatusCode.OK, string response = "{\"ok\":true}")
        {
            _status = status;
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            LastRequest = request;
            var msg = new HttpResponseMessage(_status)
            {
                Content = new StringContent(_response, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(msg);
        }
    }

    private sealed class StubHttpClientFactory : IHttpClientFactory
    {
        private readonly HttpMessageHandler _handler;
        public CapturingHandler Capture => (CapturingHandler)_handler; // retained (unused now)
        public StubHttpClientFactory(CapturingHandler handler) => _handler = handler;
        public HttpClient CreateClient(string name) => new HttpClient(_handler, disposeHandler: false);
    }

    private static WebhookAutomaticActionExecutor CreateExecutor(StubHttpClientFactory factory)
    {
        var logger = new Mock<ILogger<WebhookAutomaticActionExecutor>>();
        return new WebhookAutomaticActionExecutor(factory, logger.Object);
    }

    private static WorkflowInstance MakeInstance(int id = 101) => new()
    {
        Id = id,
        TenantId = 1,
        WorkflowDefinitionId = 55,
        DefinitionVersion = 3,
        Status = DTOs.Workflow.Enums.InstanceStatus.Running,
        CurrentNodeIds = "[]",
        Context = "{}",
        StartedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        CreatedAt = DateTime.UtcNow
    };

    private sealed class TestActionContext : IAutomaticActionContext
    {
        public WorkflowInstance Instance { get; }
        public string NodeId { get; }
        public JsonDocument? NodeJson { get; }
        public JsonDocument? ActionConfig { get; }
        public string CurrentContextJson { get; }
        public CancellationToken CancellationToken { get; }

        public TestActionContext(
            WorkflowInstance instance,
            string nodeId,
            JsonDocument actionConfig,
            string currentContextJson = "{}",
            CancellationToken ct = default)
        {
            Instance = instance;
            NodeId = nodeId;
            ActionConfig = actionConfig;
            CurrentContextJson = currentContextJson;
            CancellationToken = ct;
        }
    }

    private static JsonDocument BuildConfig(object anon) =>
        JsonDocument.Parse(JsonSerializer.Serialize(anon, new JsonSerializerOptions { PropertyNamingPolicy = null }));

    [Fact]
    public async Task Headers_TokenExpansion_ReplacesInstanceAndContext()
    {
        var handler = new CapturingHandler();
        var factory = new StubHttpClientFactory(handler);
        var exec = CreateExecutor(factory);

        var cfg = BuildConfig(new
        {
            url = "https://example.com/h",
            method = "GET",
            headers = new
            {
                X_Instance = "{{instance.id}}",
                X_Customer = "{{context.customerId}}"
            }
        });

        var ctxJson = new JsonObject
        {
            ["customerId"] = "C-789"
        }.ToJsonString();

        var ctx = new TestActionContext(MakeInstance(222), "auto1", cfg, ctxJson);

        var result = await exec.ExecuteAsync(ctx);

        result.Success.Should().BeTrue();
        handler.LastRequest.Should().NotBeNull();
        var req = handler.LastRequest!;
        req.Headers.TryGetValues("X_Instance", out var instVals).Should().BeTrue();
        instVals!.First().Should().Be("222");
        req.Headers.TryGetValues("X_Customer", out var custVals).Should().BeTrue();
        custVals!.First().Should().Be("C-789");
    }

    [Fact]
    public async Task BodyRaw_TokenExpansion_MultipleTokens()
    {
        var handler = new CapturingHandler();
        var factory = new StubHttpClientFactory(handler);
        var exec = CreateExecutor(factory);

        var cfg = BuildConfig(new
        {
            url = "https://example.com/raw",
            method = "POST",
            bodyRaw = "{\"id\":{{instance.id}},\"cust\":\"{{context.customerId}}\",\"missing\":\"{{context.nope}}\"}"
        });

        var ctxJson = new JsonObject
        {
            ["customerId"] = "AB-12"
        }.ToJsonString();

        var ctx = new TestActionContext(MakeInstance(9001), "auto2", cfg, ctxJson);

        var result = await exec.ExecuteAsync(ctx);
        result.Success.Should().BeTrue();

        var sent = await handler.LastRequest!.Content!.ReadAsStringAsync();
        sent.Should().Contain("\"id\":9001");
        sent.Should().Contain("\"cust\":\"AB-12\"");
        sent.Should().Contain("{{context.nope}}");
    }

    [Fact]
    public async Task BodyObject_TokenExpansion_NestedPaths()
    {
        var handler = new CapturingHandler();
        var factory = new StubHttpClientFactory(handler);
        var exec = CreateExecutor(factory);

        var cfg = BuildConfig(new
        {
            url = "https://example.com/body",
            method = "POST",
            body = new
            {
                meta = new
                {
                    inst = "{{instance.id}}",
                    status = "{{instance.status}}"
                },
                payload = new
                {
                    orderId = "{{context.order.id}}",
                    total = "{{context.order.total}}",
                    customerName = "{{context.customer.name}}"
                }
            }
        });

        var ctxJson = new JsonObject
        {
            ["order"] = new JsonObject
            {
                ["id"] = 4567,
                ["total"] = 123.45
            },
            ["customer"] = new JsonObject
            {
                ["name"] = "Jane"
            }
        }.ToJsonString();

        var ctx = new TestActionContext(MakeInstance(17), "auto3", cfg, ctxJson);

        var result = await exec.ExecuteAsync(ctx);
        result.Success.Should().BeTrue();

        var body = await handler.LastRequest!.Content!.ReadAsStringAsync();
        body.Should().Contain("\"inst\":\"17\"");
        body.Should().Contain("\"status\":\"Running\"");
        body.Should().Contain("\"orderId\":\"4567\"");
        body.Should().Contain("\"total\":\"123.45\"");
        body.Should().Contain("\"customerName\":\"Jane\"");
    }

    [Fact]
    public async Task Context_Array_Index_Path_Supported()
    {
        var handler = new CapturingHandler();
        var factory = new StubHttpClientFactory(handler);
        var exec = CreateExecutor(factory);

        var cfg = BuildConfig(new
        {
            url = "https://example.com/arr",
            method = "POST",
            body = new
            {
                firstSku = "{{context.lines.0.sku}}",
                secondQty = "{{context.lines.1.qty}}"
            }
        });

        var ctxJson = new JsonObject
        {
            ["lines"] = new JsonArray(
                new JsonObject { ["sku"] = "SKU-A", ["qty"] = 2 },
                new JsonObject { ["sku"] = "SKU-B", ["qty"] = 9 }
            )
        }.ToJsonString();

        var ctx = new TestActionContext(MakeInstance(3), "auto4", cfg, ctxJson);
        var result = await exec.ExecuteAsync(ctx);
        result.Success.Should().BeTrue();

        var body = await handler.LastRequest!.Content!.ReadAsStringAsync();
        body.Should().Contain("\"firstSku\":\"SKU-A\"");
        body.Should().Contain("\"secondQty\":\"9\"");
    }

    [Fact]
    public async Task Unknown_Instance_Field_Left_Unchanged()
    {
        var handler = new CapturingHandler();
        var factory = new StubHttpClientFactory(handler);
        var exec = CreateExecutor(factory);

        var cfg = BuildConfig(new
        {
            url = "https://example.com/u",
            method = "POST",
            bodyRaw = "Value={{instance.unknownField}}"
        });

        var ctx = new TestActionContext(MakeInstance(1), "auto5", cfg, "{}");
        var result = await exec.ExecuteAsync(ctx);
        result.Success.Should().BeTrue();

        var body = await handler.LastRequest!.Content!.ReadAsStringAsync();
        body.Should().Contain("{{instance.unknownField}}");
    }
}
