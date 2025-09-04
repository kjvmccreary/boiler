using FluentAssertions;
using Moq;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.FeatureFlags;
using WorkflowService.Domain.Dsl;
using Xunit;
using Microsoft.Extensions.Logging;

namespace WorkflowService.Tests.Engine;

public class FeatureFlagGatewayStrategyTests
{
    private readonly Mock<IFeatureFlagProvider> _provider = new();
    private readonly Mock<IFeatureFlagFallbackEmitter> _fallbackEmitter = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly ILogger<FeatureFlagGatewayStrategy> _logger;

    public FeatureFlagGatewayStrategyTests()
    {
        _logger = new Moq.Mock<ILogger<FeatureFlagGatewayStrategy>>().Object;
        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);
    }

    private GatewayStrategyContext CreateContext(string strategyJson, string ctxJson = "{}")
    {
        var node = new WorkflowNode
        {
            Id = "gwFlag",
            Type = "gateway",
            Properties = new Dictionary<string, object>
            {
                { "strategy", System.Text.Json.JsonDocument.Parse(strategyJson).RootElement }
            }
        };

        var def = new WorkflowDefinition
        {
            Id = 1,
            Version = 1,
            JSONDefinition = "{}",
            Name = "FlagDef"
        };

        var instance = new WorkflowInstance
        {
            Id = 10,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = ctxJson,
            CurrentNodeIds = "[]"
        };

        var edges = new List<WorkflowEdge>
        {
            new WorkflowEdge { Id="eOn",  Source="gwFlag", Target="onPath" },
            new WorkflowEdge { Id="eOff", Source="gwFlag", Target="offPath" }
        };

        return new GatewayStrategyContext(
            node,
            instance,
            edges,
            ctxJson,
            _conditions.Object,
            CancellationToken.None);
    }

    private FeatureFlagGatewayStrategy CreateStrategy() =>
        new FeatureFlagGatewayStrategy(_provider.Object, _fallbackEmitter.Object, _logger);

    [Fact]
    public async Task Flag_On_Should_Select_OnTarget()
    {
        _provider.Setup(p => p.IsEnabledAsync("flagX", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var ctx = CreateContext("""
        {
          "kind":"featureFlag",
          "config":{"flag":"flagX","onTarget":"onPath","offTarget":"offPath","required":true}
        }
        """);

        var strat = CreateStrategy();
        var decision = await strat.EvaluateAsync(ctx);

        decision.SelectedTargetNodeIds.Should().ContainSingle(v => v == "onPath");
        decision.ChosenEdgeIds.Should().ContainSingle(e => e == "eOn");
        decision.Diagnostics.Should().ContainKey("flagEnabled").WhoseValue.Should().Be(true);
    }

    [Fact]
    public async Task Flag_Off_Should_Select_OffTarget()
    {
        _provider.Setup(p => p.IsEnabledAsync("flagY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var ctx = CreateContext("""
        {
          "kind":"featureFlag",
          "config":{"flag":"flagY","onTarget":"onPath","offTarget":"offPath","required":false}
        }
        """);

        var strat = CreateStrategy();
        var decision = await strat.EvaluateAsync(ctx);

        decision.SelectedTargetNodeIds.Should().ContainSingle(v => v == "offPath");
        decision.ChosenEdgeIds.Should().ContainSingle(e => e == "eOff");
        decision.Diagnostics!["flagEnabled"].Should().Be(false);
        _fallbackEmitter.Verify(f => f.EmitFallback(It.IsAny<WorkflowInstance>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
    }

    [Fact]
    public async Task Provider_Error_Required_Should_Emit_Fallback_And_Select_OffTarget()
    {
        _provider.Setup(p => p.IsEnabledAsync("flagZ", It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var ctx = CreateContext("""
        {
          "kind":"featureFlag",
          "config":{"flag":"flagZ","onTarget":"onPath","offTarget":"offPath","required":true}
        }
        """);

        var strat = CreateStrategy();
        var decision = await strat.EvaluateAsync(ctx);

        decision.SelectedTargetNodeIds.Should().ContainSingle(v => v == "offPath");
        decision.Diagnostics!["providerError"].Should().Be(true);
        decision.Diagnostics!["fallback"].Should().Be(true);
        _fallbackEmitter.Verify(f => f.EmitFallback(
            ctx.Instance, "gwFlag", "flagZ", It.IsAny<string>(), true), Times.Once);
    }
}
