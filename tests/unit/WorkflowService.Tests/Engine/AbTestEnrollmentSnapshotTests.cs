using FluentAssertions;
using Moq;
using System.Text.Json;
using System.Text.Json.Nodes;
using WorkflowService.Domain.Models;
using WorkflowService.Domain.Dsl;              // ADDED (WorkflowNode, WorkflowEdge)
using WorkflowService.Engine.Executors;
using WorkflowService.Engine.Gateways;
using WorkflowService.Engine.Interfaces;
using Contracts.Services;
using Xunit;
using WorkflowService.Engine.Pruning;
using Microsoft.Extensions.Logging;

namespace WorkflowService.Tests.Engine;

public class AbTestEnrollmentSnapshotTests : TestBase
{
    private readonly Mock<IDeterministicHasher> _hasher = new();
    private readonly Mock<IConditionEvaluator> _conditions = new();
    private readonly Mock<IWorkflowContextPruner> _pruner = new();
    private readonly Mock<IGatewayPruningEventEmitter> _pruneEmitter = new();
    private readonly ILogger<GatewayEvaluator> _gwLogger;
    private readonly ILogger<AbTestGatewayStrategy> _abLogger;

    public AbTestEnrollmentSnapshotTests()
    {
        _hasher.SetupGet(h => h.Algorithm).Returns("TestHasher");
        _hasher.SetupGet(h => h.Seed).Returns(1UL);
        _gwLogger = CreateMockLogger<GatewayEvaluator>().Object;
        _abLogger = CreateMockLogger<AbTestGatewayStrategy>().Object;
        _conditions.Setup(c => c.EvaluateAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);
        _pruner.Setup(p => p.PruneGatewayHistory(It.IsAny<JsonArray>())).Returns(0);
    }

    private (WorkflowInstance instance, WorkflowNode gateway, List<WorkflowEdge> edges) BuildModel(string jsonDef)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "A/B Snapshot",
            Version = 2,
            JSONDefinition = jsonDef,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowDefinitions.Add(def);
        DbContext.SaveChanges();

        var instance = new WorkflowInstance
        {
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            TenantId = 1,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            Context = "{}",
            CurrentNodeIds = "[]",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        DbContext.WorkflowInstances.Add(instance);
        DbContext.SaveChanges();

        var gwNode = new WorkflowNode
        {
            Id = "gw1",
            Type = "gateway",
            Properties = new Dictionary<string, object>
            {
                {
                    "strategy",
                    JsonDocument.Parse("""
                    {
                      "kind":"abTest",
                      "config":{
                        "keyPath":"user.id",
                        "variants":[
                          {"weight":70,"target":"A"},
                          {"weight":30,"target":"B"}
                        ]
                      }
                    }
                    """).RootElement
                }
            }
        };

        var edges = new List<WorkflowEdge>
        {
            new WorkflowEdge { Id="eA", Source="gw1", Target="A" },
            new WorkflowEdge { Id="eB", Source="gw1", Target="B" }
        };

        return (instance, gwNode, edges);
    }

    private GatewayEvaluator CreateEvaluator()
    {
        var strat = new AbTestGatewayStrategy(_hasher.Object, _abLogger);
        var registry = new GatewayStrategyRegistry(new IGatewayStrategy[]
        {
            strat,
            new ExclusiveGatewayStrategy(),
            new ParallelGatewayStrategy()
        });

        return new GatewayEvaluator(
            _conditions.Object,
            _gwLogger,
            registry,
            _hasher.Object,
            _pruner.Object,
            _pruneEmitter.Object);
    }

    private string DefinitionJson => """
    {
      "nodes":[
        {"id":"start","type":"start"},
        {"id":"gw1","type":"gateway","properties":{"strategy":{
          "kind":"abTest",
          "config":{"keyPath":"user.id","variants":[
            {"weight":70,"target":"A"},
            {"weight":30,"target":"B"}
          ]}}}},
        {"id":"A","type":"automatic","properties":{"action":{"kind":"noop"}}},
        {"id":"B","type":"automatic","properties":{"action":{"kind":"noop"}}},
        {"id":"end","type":"end"}
      ],
      "edges":[
        {"id":"s-g","from":"start","to":"gw1"},
        {"id":"g-A","from":"gw1","to":"A"},
        {"id":"g-B","from":"gw1","to":"B"},
        {"id":"A-e","from":"A","to":"end"},
        {"id":"B-e","from":"B","to":"end"}
      ]
    }
    """;

    [Fact]
    public async Task Snapshot_Should_Be_Persisted_And_Reused_When_Key_Changes()
    {
        // Arrange - forced hash sequence: first favors variant A, second would favor B if recomputed
        _hasher.SetupSequence(h => h.HashComposite(It.IsAny<string?[]>()))
            .Returns(100_000UL) // normalized small -> A
            .Returns(900_000UL); // large -> would map to B if recomputed

        var evaluator = CreateEvaluator();
        var (instance, gwNode, _) = BuildModel(DefinitionJson);

        // First evaluation with user.id = u1
        var context1 = """{"user":{"id":"u1"}}""";
        var result1 = await evaluator.ExecuteAsync(gwNode, instance, context1);
        result1.IsSuccess.Should().BeTrue();

        // Extract updated context (contains _experiments)
        var updated1 = JsonNode.Parse(result1.UpdatedContext!) as JsonObject;
        updated1!["_experiments"].Should().NotBeNull();
        var expObj = updated1!["_experiments"]!["gw1"]!.AsObject();
        expObj["variant"]!.GetValue<string>().Should().Be("A");

        // Second evaluation with CHANGED user.id (would hash to B if recomputed)
        var context2 = result1.UpdatedContext!.Replace("\"u1\"", "\"otherUser\"");
        var result2 = await evaluator.ExecuteAsync(gwNode, instance, context2);
        result2.IsSuccess.Should().BeTrue();

        // The variant should remain A due to snapshot reuse
        result2.NextNodeIds.Should().ContainSingle(v => v == "A");

        // Diagnostics history appended - ensure new decision added but variant stable
        var updated2 = JsonNode.Parse(result2.UpdatedContext!) as JsonObject;
        var decisions = updated2!["_gatewayDecisions"]!["gw1"]!.AsArray();
        decisions.Count.Should().BeGreaterThan(1);

        // Verify snapshot still same
        var expAfter = (updated2!["_experiments"]!["gw1"] as JsonObject)!;
        expAfter["variant"]!.GetValue<string>().Should().Be("A");
    }
}
