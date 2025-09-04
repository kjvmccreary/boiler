using System.Text.Json;
using FluentAssertions;
using WorkflowService.Engine;
using WorkflowService.Domain.Dsl;
using Xunit;

namespace WorkflowService.Tests.Parsing;

public class BuilderDefinitionAdapterFlattenTests
{
    [Fact]
    public void Parse_Should_Flatten_Nested_Properties_With_Strategy_Object()
    {
        // language=json
        var json = """
        {
          "nodes": [
            {
              "id": "gw1",
              "type": "gateway",
              "label": "Decision",
              "properties": {
                "strategy": { "kind": "parallel", "config": { "foo": 1 } },
                "gatewayType": "parallel",
                "customFlag": true
              }
            }
          ],
          "edges": []
        }
        """;

        var def = BuilderDefinitionAdapter.Parse(json);
        def.Nodes.Should().HaveCount(1);

        var gw = def.Nodes[0];
        gw.Properties.Should().NotContainKey("properties"); // flattened
        gw.Properties.Should().ContainKey("strategy");
        gw.Properties.Should().ContainKey("gatewayType");
        gw.Properties.Should().ContainKey("customFlag");

        var strategy = gw.Properties["strategy"];
        strategy.Should().BeOfType<JsonElement>();
        var el = (JsonElement)strategy;
        el.ValueKind.Should().Be(JsonValueKind.Object);
        el.TryGetProperty("kind", out var kindProp).Should().BeTrue();
        kindProp.GetString().Should().Be("parallel");
    }

    [Fact]
    public void Parse_Should_Flatten_Nested_GatewayType_Only()
    {
        // language=json
        var json = """
        {
          "nodes": [
            {
              "id": "gw2",
              "type": "gateway",
              "properties": {
                "gatewayType": "parallel"
              }
            }
          ],
          "edges": []
        }
        """;

        var def = BuilderDefinitionAdapter.Parse(json);
        var gw = def.Nodes.Single();
        gw.Properties.Should().ContainKey("gatewayType");
        gw.Properties["gatewayType"].Should().Be("parallel");
        gw.Properties.Should().NotContainKey("properties");
    }

    [Fact]
    public void Parse_Should_Not_Overwrite_Explicit_TopLevel_Fields()
    {
        // language=json
        var json = """
        {
          "nodes": [
            {
              "id": "gw3",
              "type": "gateway",
              "label": "TopLevelLabel",
              "properties": {
                "label": "NestedLabel",
                "strategy": { "kind": "parallel" }
              }
            }
          ],
          "edges": []
        }
        """;

        var def = BuilderDefinitionAdapter.Parse(json);
        var gw = def.Nodes.Single();

        // We copied label explicitly already; nested label should not override
        gw.Properties.Should().ContainKey("label");
        gw.Properties["label"].Should().Be("TopLevelLabel");

        // Strategy still present
        gw.Properties.Should().ContainKey("strategy");
    }
}
