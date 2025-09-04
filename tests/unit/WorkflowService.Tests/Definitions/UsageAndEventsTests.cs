using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using System.Linq;

namespace WorkflowService.Tests.Definitions;

public class UsageAndEventsTests
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
    public async Task Usage_Returns_Correct_Counts()
    {
        var b = new DefinitionServiceBuilder(nameof(Usage_Returns_Correct_Counts));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        ctx.WorkflowInstances.AddRange(
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=def.Id, DefinitionVersion=def.Version, Status=InstanceStatus.Running, CurrentNodeIds="[\"start\"]", Context="{}" },
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=def.Id, DefinitionVersion=def.Version, Status=InstanceStatus.Suspended, CurrentNodeIds="[\"start\"]", Context="{}" },
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=def.Id, DefinitionVersion=def.Version, Status=InstanceStatus.Completed, CurrentNodeIds="[]", Context="{}" }
        );
        await ctx.SaveChangesAsync();

        var usage = await svc.GetUsageAsync(def.Id);
        Assert.True(usage.Success);
        Assert.Equal(1, usage.Data!.RunningCount);
        Assert.Equal(1, usage.Data!.SuspendedCount);
        Assert.Equal(1, usage.Data!.CompletedCount);
        Assert.Equal(2, usage.Data!.ActiveInstanceCount);
    }

    [Fact]
    public async Task Usage_Reflects_Zero_After_ForceTerminateAndUnpublish()
    {
        var b = new DefinitionServiceBuilder(nameof(Usage_Reflects_Zero_After_ForceTerminateAndUnpublish));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        ctx.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId=1,
            WorkflowDefinitionId=def.Id,
            DefinitionVersion=def.Version,
            Status=InstanceStatus.Running,
            CurrentNodeIds="[\"start\"]",
            Context="{}"
        });
        await ctx.SaveChangesAsync();

        await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true });

        var usage = await svc.GetUsageAsync(def.Id);
        Assert.True(usage.Success);
        Assert.Equal(0, usage.Data!.RunningCount);
        Assert.Equal(0, usage.Data.SuspendedCount);
        Assert.Equal(0, usage.Data.ActiveInstanceCount);
    }

    [Fact]
    public async Task Publish_And_Unpublish_Emit_Outbox_Event_Types()
    {
        var b = new DefinitionServiceBuilder(nameof(Publish_And_Unpublish_Emit_Outbox_Event_Types));
        var svc = b.Build();

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        Assert.Contains("workflow.definition.published", b.Publisher.OutboxTypes);

        await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());

        Assert.Contains("workflow.definition.unpublished", b.Publisher.OutboxTypes);
    }

    [Fact]
    public async Task ForceTerminateUnpublish_Emits_InstanceCancellation_Events()
    {
        var b = new DefinitionServiceBuilder(nameof(ForceTerminateUnpublish_Emits_InstanceCancellation_Events));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        ctx.WorkflowInstances.AddRange(
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=def.Id, DefinitionVersion=def.Version, Status=InstanceStatus.Running, CurrentNodeIds="[\"start\"]", Context="{}" },
            new WorkflowInstance { TenantId=1, WorkflowDefinitionId=def.Id, DefinitionVersion=def.Version, Status=InstanceStatus.Suspended, CurrentNodeIds="[\"start\"]", Context="{}" }
        );
        await ctx.SaveChangesAsync();

        await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true });

        Assert.Equal(2, b.Publisher.ForceCancelledInstances);
        Assert.Contains("workflow.instance.force_cancelled", b.Publisher.OutboxTypes);
    }
}
