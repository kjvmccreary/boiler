using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using System;
using Microsoft.EntityFrameworkCore;

namespace WorkflowService.Tests.Definitions;

public class ConcurrencyTests
{
    private const string SimpleJson = """
{
  "nodes":[{"id":"start","type":"start"},{"id":"end","type":"end"}],
  "edges":[{"id":"e1","from":"start","to":"end"}]
}
""";

    [Fact]
    public async Task Concurrent_Unpublish_And_NewInstance_Start_Unpublish_Fails_When_Instance_Running()
    {
        var b = new DefinitionServiceBuilder(nameof(Concurrent_Unpublish_And_NewInstance_Start_Unpublish_Fails_When_Instance_Running));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        // Simulate "start instance" and "unpublish" racing. We insert instance first then call unpublish.
        var startTask = Task.Run(async () =>
        {
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
        });

        await startTask; // ensure instance persisted before unpublish call in this simplified race model

        var unpublish = await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());
        Assert.False(unpublish.Success);
        Assert.Contains("Active instances", unpublish.Message!);
    }
}
