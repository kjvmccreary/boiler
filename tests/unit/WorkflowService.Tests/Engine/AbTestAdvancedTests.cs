using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using Contracts.Services;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Diagnostics;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.Pruning;
using Xunit;

namespace WorkflowService.Tests.Engine;

/// Covers:
/// - D1 Deterministic Variant Stability
/// - A4 Override behavior
/// - A5 ExperimentAssigned event emission
/// Stand‑alone unit test (no TestBase / runtime).
public class AbTestAdvancedTests
{
    private readonly Mock<IDeterministicHasher> _hasher = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly Mock<IWorkflowContextPruner> _pruner = new();
    private readonly Mock<IGatewayPruningEventEmitter> _pruneEmitter = new();
    private readonly Mock<IExperimentAssignmentEmitter> _expEmitter = new();
    private readonly AbTestGatewayStrategy _strategy;
    private readonly GatewayEvaluator _evaluator;

    // Minimal workflow definition JSON (classic string, no raw literal to avoid lang version issues)
    private const string MinimalDefinitionJson =
@"{
  ""nodes"": [
    { ""id"": ""gw"", ""type"": ""gateway"" },
    { ""id"": ""A"", ""type"": ""automatic"", ""properties"": { ""action"": { ""kind"": ""noop"" } } },
    { ""id"": ""B"", ""type"": ""automatic"", ""properties"": { ""action"": { ""kind"": ""noop"" } } }
  ],
  ""edges"": [
    { ""id"": ""eA"", ""from"": ""gw"", ""to"": ""A"" },
    { ""id"": ""eB"", ""from"": ""gw"", ""to"": ""B"" }
  ]
}";

    public AbTestAdvancedTests()
    {
        _hasher.SetupGet(h => h.Algorithm).Returns("TestHasher");
        _hasher.SetupGet(h => h.Seed).Returns(1UL);
        _hasher.Setup(h => h.HashComposite(It.IsAny<string?[]>()))
            .Returns<string?[]>(parts =>
            {
                unchecked
                {
                    ulong acc = 1469598103934665603;
                    foreach (var p in parts)
                    {
                        var s = p ?? "";
                        foreach (var c in s)
                            acc = (acc ^ c) * 1099511628211;
                    }
                    return acc;
                }
            });

        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _pruner.Setup(p => p.PruneGatewayHistory(It.IsAny<JsonArray>())).Returns(0);

        _strategy = new AbTestGatewayStrategy(_hasher.Object, MockLogger<AbTestGatewayStrategy>());

        var registry = new GatewayStrategyRegistry(new IGatewayStrategy[]
        {
            _strategy,
            new ExclusiveGatewayStrategy(),
            new ParallelGatewayStrategy()
        });

        _evaluator = new GatewayEvaluator(
            _conditions.Object,
            MockLogger<GatewayEvaluator>(),
            registry,
            _hasher.Object,
            _pruner.Object,
            _pruneEmitter.Object,
            _expEmitter.Object);
    }

    private static ILogger<T> MockLogger<T>() => new Mock<ILogger<T>>().Object;

    private WorkflowNode MakeNode(string id = "gw") => new()
    {
        Id = id,
        Type = "gateway",
        Properties = new Dictionary<string, object>
        {
            {
                "strategy",
                JsonDocument.Parse(
@"{ ""kind"":""abTest"",
   ""config"":{ ""keyPath"":""user.id"",""variants"":[
      { ""weight"":70,""target"":""A"" },
      { ""weight"":30,""target"":""B"" }
   ]}}").RootElement
            }
        }
    };

    private WorkflowInstance MakeInstance(int defVersion = 1) => new()
    {
        WorkflowDefinitionId = 1,
        DefinitionVersion = defVersion,
        TenantId = 1,
        Status = DTOs.Workflow.Enums.InstanceStatus.Running,
        Context = "{}",
        CurrentNodeIds = "[]",
        StartedAt = System.DateTime.UtcNow,
        CreatedAt = System.DateTime.UtcNow,
        UpdatedAt = System.DateTime.UtcNow,
        WorkflowDefinition = new WorkflowDefinition
        {
            Id = 1,
            Name = "TestDef",
            Version = defVersion,
            JSONDefinition = MinimalDefinitionJson,
            IsPublished = true,
            CreatedAt = System.DateTime.UtcNow,
            UpdatedAt = System.DateTime.UtcNow
        }
    };

    [Fact]
    public async Task D1_DeterministicVariant_ShouldRemainStableOverMultipleEvaluations()
    {
        var node = MakeNode();
        var instance = MakeInstance();
        var context = "{\"user\":{\"id\":\"user-123\"}}";

        string? firstVariant = null;
        for (int i = 0; i < 10; i++)
        {
            var res = await _evaluator.ExecuteAsync(node, instance, context);
            res.IsSuccess.Should().BeTrue();
            firstVariant ??= res.NextNodeIds.Single();
            res.NextNodeIds.Single().Should().Be(firstVariant);
            context = res.UpdatedContext!;
        }

        var fresh = MakeInstance();
        var repeat = await _evaluator.ExecuteAsync(node, fresh, "{\"user\":{\"id\":\"user-123\"}}");
        repeat.NextNodeIds.Single().Should().Be(firstVariant);
    }

    [Fact]
    public async Task A4_Override_ShouldForceSpecificVariantAndMarkDiagnostics()
    {
        var node = MakeNode();
        var instance = MakeInstance();
        var context =
@"{
  ""user"":{""id"":""anyUser""},
  ""_overrides"":{""gateway"":{""gw"":""B""}}
}";

        var res = await _evaluator.ExecuteAsync(node, instance, context);
        res.IsSuccess.Should().BeTrue();
        res.NextNodeIds.Should().ContainSingle(v => v == "B");

        var root = JsonNode.Parse(res.UpdatedContext!)!.AsObject();
        var history = (root["_gatewayDecisions"]!["gw"] as JsonArray)!;
        var latest = history[^1]!.AsObject();
        latest["diagnostics"]!["overrideApplied"]!.GetValue<bool>().Should().BeTrue();
        latest["selectedTargets"]!.AsArray().Single()!.GetValue<string>().Should().Be("B");
    }

    [Fact]
    public async Task A5_ExperimentAssignedEvent_EmittedOnFirstAndOverride_NotOnReuse()
    {
        var node = MakeNode();
        var instance = MakeInstance();
        var initial = "{\"user\":{\"id\":\"seedUser\"}}";

        var first = await _evaluator.ExecuteAsync(node, instance, initial);
        first.IsSuccess.Should().BeTrue();
        _expEmitter.Verify(e => e.EmitAssigned(
            instance, "gw",
            It.IsAny<string>(), It.IsAny<string?>(),
            false, false), Times.Once);

        var reuse = await _evaluator.ExecuteAsync(node, instance, first.UpdatedContext!);
        reuse.IsSuccess.Should().BeTrue();
        _expEmitter.VerifyNoOtherCalls();

        var ctxObj = JsonNode.Parse(first.UpdatedContext!)!.AsObject();
        ctxObj["_overrides"] = new JsonObject
        {
            ["gateway"] = new JsonObject { ["gw"] = "B" }
        };
        var overridden = await _evaluator.ExecuteAsync(node, instance, ctxObj.ToJsonString());
        overridden.IsSuccess.Should().BeTrue();
        _expEmitter.Verify(e => e.EmitAssigned(
            instance, "gw",
            "B", It.IsAny<string?>(),
            true, false), Times.Once);
    }
}
