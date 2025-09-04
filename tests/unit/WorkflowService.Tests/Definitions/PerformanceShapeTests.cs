using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using System.Linq;
using System.Collections.Generic;

namespace WorkflowService.Tests.Definitions;

/// <summary>
/// Lightweight shape tests to ensure ActiveInstanceCount is computed and does not require per-definition N+1 queries.
/// (In-memory EF does not expose query count easily; we assert projection correctness across multiple definitions.)
/// </summary>
public class PerformanceShapeTests
{
    private const string SimpleJson = """
{
  "nodes":[{"id":"start","type":"start"},{"id":"end","type":"end"}],
  "edges":[{"id":"e1","from":"start","to":"end"}]
}
""";

    [Fact]
    public async Task List_Definitions_Includes_Correct_ActiveInstanceCount_For_Many()
    {
        var b = new DefinitionServiceBuilder(nameof(List_Definitions_Includes_Correct_ActiveInstanceCount_For_Many));
        var svc = b.Build();
        var ctx = b.Context;

        // Create & publish 3 definitions
        var defs = new List<int>();
        for (int i = 1; i <= 3; i++)
        {
            var d = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = $"Flow{i}", JSONDefinition = SimpleJson })).Data!;
            await svc.PublishAsync(d.Id, new PublishDefinitionRequestDto());
            defs.Add(d.Id);
        }

        // Add instances: Flow1 -> 2 running, Flow2 -> 1 suspended, Flow3 -> none
        var flow1Id = defs[0];
        var flow2Id = defs[1];

        ctx.WorkflowInstances.AddRange(
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=flow1Id, DefinitionVersion=1, Status=InstanceStatus.Running, CurrentNodeIds="[\"start\"]", Context="{}" },
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=flow1Id, DefinitionVersion=1, Status=InstanceStatus.Running, CurrentNodeIds="[\"start\"]", Context="{}" },
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=flow2Id, DefinitionVersion=1, Status=InstanceStatus.Suspended, CurrentNodeIds="[\"start\"]", Context="{}" }
        );
        await ctx.SaveChangesAsync();

        var list = await svc.GetAllAsync(new GetWorkflowDefinitionsRequestDto { PageSize = 10 });
        Assert.True(list.Success);
        var items = list.Data!.Items;

        var flow1Dto = items.Single(x => x.Id == flow1Id);
        var flow2Dto = items.Single(x => x.Id == flow2Id);
        var flow3Dto = items.Single(x => x.Name == "Flow3");

        Assert.Equal(2, flow1Dto.ActiveInstanceCount);
        Assert.Equal(1, flow2Dto.ActiveInstanceCount);
        Assert.Equal(0, flow3Dto.ActiveInstanceCount);
    }
}
