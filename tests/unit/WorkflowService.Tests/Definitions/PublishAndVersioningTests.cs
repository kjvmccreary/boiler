using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using DTOs.Workflow.Enums;
using System.Linq;

namespace WorkflowService.Tests.Definitions;

public class PublishAndVersioningTests
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
    public async Task Publish_Sets_IsPublished_And_PublishedAt()
    {
        var b = new DefinitionServiceBuilder(nameof(Publish_Sets_IsPublished_And_PublishedAt));
        var svc = b.Build();

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "Flow", JSONDefinition = SimpleJson })).Data!;
        var published = await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        Assert.True(published.Success);
        Assert.True(published.Data!.IsPublished);
        Assert.NotNull(published.Data.PublishedAt);
    }

    [Fact]
    public async Task CreateNewVersion_Increments_Version_And_IsDraft()
    {
        var b = new DefinitionServiceBuilder(nameof(CreateNewVersion_Increments_Version_And_IsDraft));
        var svc = b.Build();

        var v1 = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(v1.Id, new PublishDefinitionRequestDto());

        var v2 = await svc.CreateNewVersionAsync(v1.Id, new CreateNewVersionRequestDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson.Replace("end", "end2")
        });

        Assert.True(v2.Success);
        Assert.Equal(2, v2.Data!.Version);
        Assert.False(v2.Data.IsPublished);
    }

    [Fact]
    public async Task ExistingInstances_Retain_OriginalVersion_After_NewVersion_Published()
    {
        var b = new DefinitionServiceBuilder(nameof(ExistingInstances_Retain_OriginalVersion_After_NewVersion_Published));
        var svc = b.Build();
        var ctx = b.Context;

        var v1 = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name = "Flow", JSONDefinition = SimpleJson })).Data!;
        await svc.PublishAsync(v1.Id, new PublishDefinitionRequestDto());

        // Simulate instance of version 1
        ctx.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = v1.Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[\"start\"]",
            Context = "{}"
        });
        await ctx.SaveChangesAsync();

        // New version
        await svc.CreateNewVersionAsync(v1.Id, new CreateNewVersionRequestDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson
        });

        var inst = ctx.WorkflowInstances.First();
        Assert.Equal(1, inst.DefinitionVersion);
    }
}
