using WorkflowService.Domain.Dsl;
using FluentAssertions;
using Xunit;

public class WorkflowDefinitionJsonTests
{
    [Fact]
    public void Normalize_Should_Map_From_To_Source_Target()
    {
        var raw = """
        {
          "key": "take-two",
          "edges": [
            {"id":"e1","to":"humanTask-1756413788261","from":"start-1756413793662"},
            {"id":"e2","to":"end-1756413782420","from":"humanTask-1756413788261"}
          ],
          "nodes": [
            {"id":"end-1756413782420","type":"end","label":"End"},
            {"id":"humanTask-1756413788261","type":"humanTask","label":"Human Task","dueInMinutes":10,"assigneeRoles":["Admin"]},
            {"id":"start-1756413793662","type":"start","label":"Start"}
          ]
        }
        """;

        var parsed = WorkflowDefinitionJson.FromJson(raw);
        parsed.Edges.Should().HaveCount(2);
        parsed.Edges.Select(e => e.Source + "->" + e.Target)
            .Should().BeEquivalentTo(new[]
            {
                "start-1756413793662->humanTask-1756413788261",
                "humanTask-1756413788261->end-1756413782420"
            });
    }
}
