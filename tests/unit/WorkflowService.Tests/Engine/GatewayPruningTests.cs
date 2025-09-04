using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;
using Moq;
using WorkflowService.Domain.Dsl;
using WorkflowService.Domain.Models;
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using WorkflowService.Engine.Pruning;
using WorkflowService.Engine.Diagnostics;
using Contracts.Services;
using Xunit;

namespace WorkflowService.Tests.Engine;

/// Covers D5 Context Prune Test & diagnosticsVersion presence (C2)
public class GatewayPruningTests : TestBase
{
    [Fact]
    public async Task D5_PrunesOldDecisionsAndEmitsEvent()
    {
        var hasher = new Mock<IDeterministicHasher>();
        hasher.SetupGet(h => h.Algorithm).Returns("TestHasher");
        hasher.SetupGet(h => h.Seed).Returns(1UL);
        hasher.Setup(h => h.Hash(It.IsAny<string?>())).Returns(5555UL);
        hasher.Setup(h => h.HashComposite(It.IsAny<string?[]>())).Returns(5555UL);

        var cond = new Mock<IConditionEvaluator>();
        cond.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        var prunerOptions = Microsoft.Extensions.Options.Options.Create(new WorkflowPruningOptions
        {
            MaxGatewayDecisionsPerNode = 2
        });
        var pruner = new WorkflowContextPruner(
            CreateMockLogger<WorkflowContextPruner>().Object,
            prunerOptions);

        var pruneEmitter = new Mock<IGatewayPruningEventEmitter>();
        var expEmitter = new Mock<IExperimentAssignmentEmitter>();

        var stratRegistry = new GatewayStrategyRegistry(new IGatewayStrategy[]
        {
            new ExclusiveGatewayStrategy(),
            new ParallelGatewayStrategy()
        });

        var evaluator = new GatewayEvaluator(
            cond.Object,
            CreateMockLogger<GatewayEvaluator>().Object,
            stratRegistry,
            hasher.Object,
            pruner,
            pruneEmitter.Object,
            expEmitter.Object);

        var node = new WorkflowNode
        {
            Id = "gw",
            Type = "gateway",
            Properties = new Dictionary<string, object>
            {
                { "strategy", "exclusive" }
            }
        };

        var instance = new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = 1,
            DefinitionVersion = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = "{}",
            CurrentNodeIds = "[]",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            WorkflowDefinition = new WorkflowDefinition
            {
                Id = 1,
                Name = "PruneTest",
                Version = 1,
                JSONDefinition = "{\"nodes\":[],\"edges\":[]}",
                IsPublished = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        // Fake strategy outcome via registry by just using exclusive default -> no targets
        string context = "{}";

        for (int i = 0; i < 3; i++)
        {
            var res = await evaluator.ExecuteAsync(node, instance, context);
            res.IsSuccess.Should().BeTrue();
            context = res.UpdatedContext!;
        }

        var root = JsonNode.Parse(context)!.AsObject();
        var history = (root["_gatewayDecisions"]!["gw"] as JsonArray)!;
        history.Count.Should().Be(2, "History pruned to max 2");
        // Expect emitter called once (after 3rd add)
        pruneEmitter.Verify(p => p.EmitGatewayDecisionPruned(instance, "gw", 1, 2), Times.Once);

        // Verify diagnosticsVersion present (C2)
        foreach (var d in history)
        {
            var obj = d!.AsObject();
            obj["diagnosticsVersion"]!.GetValue<int>().Should().BeGreaterOrEqualTo(2);
        }
    }
}
