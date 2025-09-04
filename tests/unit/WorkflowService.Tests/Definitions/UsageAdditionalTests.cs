using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;

namespace WorkflowService.Tests.Definitions;

public class UsageAdditionalTests
{
    private const string SimpleJson = """
    {
      "nodes":[
        {"id":"start","type":"start"},
        {"id":"end","type":"end"}
      ],
      "edges":[{"id":"e1","from":"start","to":"end"}]
    }
    """;

    [Fact]
    public async Task Usage_Reflects_New_Instance_After_Start()
    {
        var b = new DefinitionServiceBuilder(nameof(Usage_Reflects_New_Instance_After_Start));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        var usageBefore = await svc.GetUsageAsync(def.Id);
        Assert.True(usageBefore.Success);
        Assert.Equal(0, usageBefore.Data!.ActiveInstanceCount);

        ctx.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[\"start\"]",
            Context = "{}"
        });
        await ctx.SaveChangesAsync();

        var usageAfter = await svc.GetUsageAsync(def.Id);
        Assert.Equal(1, usageAfter.Data!.RunningCount);
        Assert.Equal(1, usageAfter.Data.ActiveInstanceCount);
    }
}
