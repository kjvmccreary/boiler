using Xunit;
using WorkflowService.Tests.TestSupport;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using System;

namespace WorkflowService.Tests.Isolation;

public class InstanceServiceIsolationTests
{
    [Fact]
    public async Task GetAllAsync_Should_Return_Only_Current_Tenant_Instances()
    {
        var dbName = nameof(GetAllAsync_Should_Return_Only_Current_Tenant_Instances) + "_" + Guid.NewGuid().ToString("N");

        // Tenant 1 context & seeding
        var b1 = new InstanceServiceBuilder(dbName, 1)
            .SeedDefinition(1, "Flow-A")
            .SeedInstance(1, "Flow-A");

        // Tenant 2 context & seeding (separate builder, same DB name)
        var b2 = new InstanceServiceBuilder(dbName, 2)
            .SeedDefinition(2, "Flow-B")
            .SeedInstance(2, "Flow-B");

        var svc1 = b1.Build();

        var list = await svc1.GetAllAsync(new GetInstancesRequestDto
        {
            Page = 1,
            PageSize = 50
        });

        Assert.True(list.Success);
        Assert.Single(list.Data!.Items);
        Assert.Equal("Flow-A", list.Data.Items[0].WorkflowDefinitionName);
    }

    [Fact]
    public async Task GetByIdAsync_ForeignInstance_ShouldReturnNotFound()
    {
        var dbName = nameof(GetByIdAsync_ForeignInstance_ShouldReturnNotFound) + "_" + Guid.NewGuid().ToString("N");

        var b1 = new InstanceServiceBuilder(dbName, 1)
            .SeedDefinition(1, "F1")
            .SeedInstance(1, "F1");

        var b2 = new InstanceServiceBuilder(dbName, 2)
            .SeedDefinition(2, "F2")
            .SeedInstance(2, "F2");

        var svc1 = b1.Build();
        var foreignInstanceId = b2.GetForeignInstanceId(2);

        var resp = await svc1.GetByIdAsync(foreignInstanceId);

        Assert.False(resp.Success);
        Assert.Contains("not found", resp.Message!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task StartInstanceAsync_Should_Fail_For_Foreign_Definition()
    {
        var dbName = nameof(StartInstanceAsync_Should_Fail_For_Foreign_Definition) + "_" + Guid.NewGuid().ToString("N");

        var b1 = new InstanceServiceBuilder(dbName, 1)
            .SeedDefinition(1, "OwnDef");

        var b2 = new InstanceServiceBuilder(dbName, 2)
            .SeedDefinition(2, "OtherDef");

        var svc1 = b1.Build();
        var foreignDefId = b2.GetDefinitionId("OtherDef", 2);

        var resp = await svc1.StartInstanceAsync(new StartInstanceRequestDto
        {
            WorkflowDefinitionId = foreignDefId
        });

        Assert.False(resp.Success);
        Assert.Contains("not found", resp.Message!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetAllAsync_With_Filter_Should_Not_Leak_Foreign_Definition()
    {
        var dbName = nameof(GetAllAsync_With_Filter_Should_Not_Leak_Foreign_Definition) + "_" + Guid.NewGuid().ToString("N");

        var b1 = new InstanceServiceBuilder(dbName, 1)
            .SeedDefinition(1, "Alpha")
            .SeedInstance(1, "Alpha");

        var b2 = new InstanceServiceBuilder(dbName, 2)
            .SeedDefinition(2, "Beta")
            .SeedInstance(2, "Beta");

        var svc = b1.Build();

        var localDefId = b1.GetDefinitionId("Alpha", 1);
        var list = await svc.GetAllAsync(new GetInstancesRequestDto
        {
            Page = 1,
            PageSize = 20,
            WorkflowDefinitionId = localDefId
        });

        Assert.True(list.Success);
        Assert.All(list.Data!.Items, i => Assert.Equal("Alpha", i.WorkflowDefinitionName));
        Assert.DoesNotContain(list.Data.Items, i => i.WorkflowDefinitionName == "Beta");
    }
}
