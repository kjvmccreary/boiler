using FluentAssertions;
using Moq;
using System.Text.Json;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using Contracts.Services;
using Xunit;
using Microsoft.Extensions.Logging;

namespace WorkflowService.Tests.Engine;

public class AbTestGatewayStrategyTests
{
    private readonly Mock<IDeterministicHasher> _hasher = new();
    private readonly Mock<ILogger<AbTestGatewayStrategy>> _logger = new();

    public AbTestGatewayStrategyTests()
    {
        _hasher.SetupGet(h => h.Algorithm).Returns("TestHasher");
        _hasher.SetupGet(h => h.Seed).Returns(1UL);
    }

    private GatewayStrategyContext CreateContext(string contextJson, WorkflowNode node, List<WorkflowEdge> edges, ulong forcedHash)
    {
        // Force hash output deterministically
        _hasher.Setup(h => h.HashComposite(It.IsAny<string?[]>()))
            .Returns(forcedHash);

        return new GatewayStrategyContext(
            node,
            new WorkflowInstance
            {
                Id = 101,
                DefinitionVersion = 3,
                WorkflowDefinitionId = 200,
                Context = contextJson,
                CurrentNodeIds = "[]"
            },
            edges,
            contextJson,
            new Mock<IConditionEvaluator>().Object,
            CancellationToken.None);
    }

    private WorkflowNode MakeNode(string gatewayId, string keyPath = "user.id") =>
        new()
        {
            Id = gatewayId,
            Type = "gateway",
            Properties = new Dictionary<string, object>
            {
                {
                    "strategy",
                    JsonDocument.Parse($$"""
                    {
                      "kind":"abTest",
                      "config":{
                        "keyPath":"{{keyPath}}",
                        "variants":[
                          {"weight":70,"target":"pathA"},
                          {"weight":30,"target":"pathB"}
                        ]
                      }
                    }
                    """).RootElement
                }
            }
        };

    private List<WorkflowEdge> MakeEdges(string gatewayId) => new()
    {
        new WorkflowEdge { Id="eA", Source=gatewayId, Target="pathA" },
        new WorkflowEdge { Id="eB", Source=gatewayId, Target="pathB" }
    };

    [Fact]
    public async Task AbTest_Should_Select_FirstVariant_When_Roll_In_FirstRange()
    {
        var node = MakeNode("gw1");
        var edges = MakeEdges("gw1");

        // Force hash -> normalized roll < 70 (simulate bucket producing small value)
        var ctx = CreateContext("""{"user":{"id":"u1"}}""", node, edges, forcedHash: 10UL);

        var strat = new AbTestGatewayStrategy(_hasher.Object, _logger.Object);
        var decision = await strat.EvaluateAsync(ctx);

        decision.SelectedTargetNodeIds.Should().ContainSingle(t => t == "pathA");
        decision.ChosenEdgeIds.Should().ContainSingle(e => e == "eA");
        decision.Diagnostics.Should().ContainKey("variantTarget").WhoseValue.Should().Be("pathA");
    }

    [Fact]
    public async Task AbTest_Should_Select_SecondVariant_When_Roll_In_SecondRange()
    {
        var node = MakeNode("gw2");
        var edges = MakeEdges("gw2");

        // Force hash high enough to land in second variant (e.g., normalized >= 70)
        // Choose a large forcedHash to produce big normalized.
        var ctx = CreateContext("""{"user":{"id":"u2"}}""", node, edges, forcedHash: 900_000UL);

        var strat = new AbTestGatewayStrategy(_hasher.Object, _logger.Object);
        var decision = await strat.EvaluateAsync(ctx);

        decision.SelectedTargetNodeIds.Should().ContainSingle(t => t == "pathB");
        decision.ChosenEdgeIds.Should().ContainSingle(e => e == "eB");
        decision.Diagnostics.Should().ContainKey("variantTarget").WhoseValue.Should().Be("pathB");
    }

    [Fact]
    public async Task AbTest_SameInputs_Should_Be_Stable()
    {
        var node = MakeNode("gwStable");
        var edges = MakeEdges("gwStable");

        // For stable test, let hash be constant for repeated calls
        var ctx1 = CreateContext("""{"user":{"id":"stable"}}""", node, edges, forcedHash: 123456UL);
        var ctx2 = CreateContext("""{"user":{"id":"stable"}}""", node, edges, forcedHash: 123456UL);

        var strat = new AbTestGatewayStrategy(_hasher.Object, _logger.Object);
        var d1 = await strat.EvaluateAsync(ctx1);
        var d2 = await strat.EvaluateAsync(ctx2);

        d1.SelectedTargetNodeIds.Should().Equal(d2.SelectedTargetNodeIds);
        d1.Diagnostics!["variantTarget"].Should().Be(d2.Diagnostics!["variantTarget"]);
    }
}
