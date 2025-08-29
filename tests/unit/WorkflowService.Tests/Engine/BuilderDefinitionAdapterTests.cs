using FluentAssertions;
using WorkflowService.Engine;
using Xunit;

namespace WorkflowService.Tests.Engine;

public class BuilderDefinitionAdapterTests
{
    [Fact]
    public void Parse_Should_Map_FromToEdges_And_NodeProperties()
    {
        var json = """
        {
          "key": "take-two",
          "edges": [
            { "id": "e1", "from": "start-1", "to": "task-1" },
            { "id": "e2", "from": "task-1", "to": "end-1" }
          ],
          "nodes": [
            { "id": "start-1", "type": "start", "label": "Start", "x": 10, "y": 20 },
            { "id": "task-1", "type": "humanTask", "label": "Human Task", "assigneeRoles": ["Admin"], "dueInMinutes": 5 },
            { "id": "end-1", "type": "end", "label": "End" }
          ]
        }
        """;

        var def = BuilderDefinitionAdapter.Parse(json);

        def.Nodes.Should().HaveCount(3);
        def.Edges.Should().HaveCount(2);

        def.Edges.Select(e => e.Source + "->" + e.Target)
            .Should().BeEquivalentTo(new[]
            {
                "start-1->task-1",
                "task-1->end-1"
            });

        var human = def.Nodes.Single(n => n.Id == "task-1");
        human.Properties.Should().ContainKey("assigneeRoles");
        human.Properties.Should().ContainKey("dueInMinutes");
    }

    [Fact]
    public void Parse_InvalidJson_ReturnsEmptyDefinition()
    {
        var def = BuilderDefinitionAdapter.Parse("<<< not json >>>");
        def.Nodes.Should().BeEmpty();
        def.Edges.Should().BeEmpty();
    }
}
