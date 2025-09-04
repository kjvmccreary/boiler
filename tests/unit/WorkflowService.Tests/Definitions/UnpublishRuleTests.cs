using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using System.Linq;
using System;
using Microsoft.EntityFrameworkCore;

namespace WorkflowService.Tests.Definitions;

public class UnpublishRuleTests
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
    public async Task Unpublish_Succeeds_When_No_Running_Or_Suspended_Instances()
    {
        var b = new DefinitionServiceBuilder(nameof(Unpublish_Succeeds_When_No_Running_Or_Suspended_Instances));
        var svc = b.Build();
        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        var result = await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());

        Assert.True(result.Success);
        Assert.False(result.Data!.IsPublished);
    }

    [Fact]
    public async Task Unpublish_Fails_When_RunningInstances_Exist()
    {
        var b = new DefinitionServiceBuilder(nameof(Unpublish_Fails_When_RunningInstances_Exist));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

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

        var result = await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());
        Assert.False(result.Success);
        Assert.Contains("Active instances", result.Message!);
    }

    [Fact]
    public async Task ForceTerminateAndUnpublish_Cancels_Running_Then_Unpublishes()
    {
        var b = new DefinitionServiceBuilder(nameof(ForceTerminateAndUnpublish_Cancels_Running_Then_Unpublishes));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        ctx.WorkflowInstances.AddRange(
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = def.Id,
                DefinitionVersion = def.Version,
                Status = InstanceStatus.Running,
                CurrentNodeIds = "[\"start\"]",
                Context = "{}"
            },
            new WorkflowInstance
            {
                TenantId = 1,
                WorkflowDefinitionId = def.Id,
                DefinitionVersion = def.Version,
                Status = InstanceStatus.Suspended,
                CurrentNodeIds = "[\"start\"]",
                Context = "{}"
            });
        await ctx.SaveChangesAsync();

        var result = await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true });

        Assert.True(result.Success);
        Assert.False(result.Data!.IsPublished);
        var statuses = ctx.WorkflowInstances.Select(i => i.Status).ToList();
        Assert.All(statuses, s => Assert.Equal(InstanceStatus.Cancelled, s));
        Assert.Equal(2, b.Publisher.ForceCancelledInstances);
        Assert.Equal(1, b.Publisher.UnpublishedDefinitions);
    }

    [Fact]
    public async Task ForceTerminate_PartialFailure_IsAtomic()
    {
        var b = new DefinitionServiceBuilder(nameof(ForceTerminate_PartialFailure_IsAtomic));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

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

        // Simulate failure during event publishing
        b.Publisher.ThrowOnForceCancel = true;

        // Accept either Exception or InvalidOperationException (implementation detail)
        var ex = await Assert.ThrowsAnyAsync<Exception>(async () =>
            await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true }));

        // Ensure rollback: instance still running, definition still published
        var instance = await ctx.WorkflowInstances.FirstAsync();
        Assert.Equal(InstanceStatus.Running, instance.Status);

        var definition = await ctx.WorkflowDefinitions.FirstAsync(d => d.Id == def.Id);
        Assert.True(definition.IsPublished);

        // Ensure cancel events NOT emitted (no partial side-effects)
        Assert.Equal(0, b.Publisher.ForceCancelledInstances);

        // Exception message sanity (optional)
        Assert.Contains("failure", ex.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task Unpublish_After_Instances_Complete_Succeeds()
    {
        var b = new DefinitionServiceBuilder(nameof(Unpublish_After_Instances_Complete_Succeeds));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        ctx.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = InstanceStatus.Completed,
            CurrentNodeIds = "[]",
            Context = "{}",
            CompletedAt = DateTime.UtcNow
        });
        await ctx.SaveChangesAsync();

        var result = await svc.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());

        Assert.True(result.Success);
        Assert.False(result.Data!.IsPublished);
    }
}
