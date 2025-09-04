using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
using System;

namespace WorkflowService.Tests.Definitions;

public class DefinitionImmutabilityTests
{
    private const string SimpleJson = """
{
  "nodes":[
    {"id":"start","type":"start","name":"Start"},
    {"id":"end","type":"end","name":"End"}
  ],
  "edges":[{"id":"e1","from":"start","to":"end"}]
}
""";

    [Fact]
    public async Task Should_UpdateDraft_When_NotPublished()
    {
        var b = new DefinitionServiceBuilder(nameof(Should_UpdateDraft_When_NotPublished));
        var svc = b.Build();

        var create = await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto
        {
            Name = "Flow A",
            JSONDefinition = SimpleJson
        });

        var update = await svc.UpdateDraftAsync(create.Data!.Id, new UpdateWorkflowDefinitionDto
        {
            Name = "Flow A2",
            JSONDefinition = SimpleJson.Replace("End", "End2")
        });

        Assert.True(update.Success);
        Assert.Contains("Draft updated", update.Message);
        Assert.Equal("Flow A2", update.Data!.Name);
    }

    [Fact]
    public async Task Should_Fail_UpdateDraft_When_Published()
    {
        var b = new DefinitionServiceBuilder(nameof(Should_Fail_UpdateDraft_When_Published));
        var svc = b.Build();

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson
        })).Data!;

        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        var update = await svc.UpdateDraftAsync(def.Id, new UpdateWorkflowDefinitionDto
        {
            JSONDefinition = SimpleJson
        });

        Assert.False(update.Success);
        Assert.Contains("Cannot modify a published", update.Message);
    }

    [Fact]
    public async Task Should_Throw_On_Attempted_JSONDefinition_Modification_PostPublish()
    {
        var b = new DefinitionServiceBuilder(nameof(Should_Throw_On_Attempted_JSONDefinition_Modification_PostPublish));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson
        })).Data!;

        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        // Directly modify JSONDefinition after publish -> should throw on SaveChanges
        var entity = await ctx.WorkflowDefinitions.FirstAsync(d => d.Id == def.Id);
        entity.JSONDefinition = entity.JSONDefinition.Replace("End", "EndX");

        await Assert.ThrowsAsync<InvalidOperationException>(() => ctx.SaveChangesAsync());
    }

    [Fact]
    public async Task Should_Allow_Metadata_Update_When_Published()
    {
        var b = new DefinitionServiceBuilder(nameof(Should_Allow_Metadata_Update_When_Published));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson,
            Description = "v1"
        })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        var entity = await ctx.WorkflowDefinitions.FirstAsync(d => d.Id == def.Id);
        var originalJson = entity.JSONDefinition;
        entity.Description = "updated description";
        await ctx.SaveChangesAsync();

        var reloaded = await ctx.WorkflowDefinitions.FirstAsync(d => d.Id == def.Id);
        Assert.Equal("updated description", reloaded.Description);
        Assert.Equal(originalJson, reloaded.JSONDefinition);
    }

    [Fact]
    public async Task ForcePublish_OnAlreadyPublished_ReturnsSuccess_Idempotent()
    {
        var b = new DefinitionServiceBuilder(nameof(ForcePublish_OnAlreadyPublished_ReturnsSuccess_Idempotent));
        var svc = b.Build();

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson
        })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        var again = await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto { ForcePublish = true });

        Assert.True(again.Success);
        Assert.Equal(def.Id, again.Data!.Id);
        Assert.Equal(1, b.Publisher.PublishedDefinitions);
    }

    [Fact]
    public async Task Should_Prevent_ForcePublish_When_JSONDefinition_Changed()
    {
        var b = new DefinitionServiceBuilder(nameof(Should_Prevent_ForcePublish_When_JSONDefinition_Changed));
        var svc = b.Build();
        var ctx = b.Context;

        var def = (await svc.CreateDraftAsync(new CreateWorkflowDefinitionDto
        {
            Name = "Flow",
            JSONDefinition = SimpleJson
        })).Data!;
        await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        // Simulate an (invalid) attempt to mutate JSON after publish
        var entity = await ctx.WorkflowDefinitions.FirstAsync(d => d.Id == def.Id);
        entity.JSONDefinition = entity.JSONDefinition.Replace("End", "EndZZ"); // mark modified (EntityState.Modified)

        // Call Publish with ForcePublish=true -> should be blocked (no SaveChanges needed to detect)
        var attempt = await svc.PublishAsync(def.Id, new PublishDefinitionRequestDto { ForcePublish = true });

        Assert.False(attempt.Success);
        Assert.Contains("Force publish blocked", attempt.Message);
        // Ensure no new publish events
        Assert.Equal(1, b.Publisher.PublishedDefinitions);
    }
}
