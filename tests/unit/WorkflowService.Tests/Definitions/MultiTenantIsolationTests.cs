using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace WorkflowService.Tests.Definitions;

public class MultiTenantIsolationTests
{
    private const string SimpleJson = """
{
  "nodes":[{"id":"start","type":"start"},{"id":"end","type":"end"}],
  "edges":[{"id":"e1","from":"start","to":"end"}]
}
""";

    [Fact]
    public async Task TenantA_Cannot_Unpublish_TenantB_Definition()
    {
        var dbName = nameof(TenantA_Cannot_Unpublish_TenantB_Definition) + "_" + Guid.NewGuid().ToString("N");

        // Tenant 1
        var builderA = new DefinitionServiceBuilder(dbName).WithTenant(1);
        var svcA = builderA.Build();
        var def = (await svcA.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="SharedFlow", JSONDefinition = SimpleJson })).Data!;
        await svcA.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        // Tenant 2 on same shared DB
        var builderB = new DefinitionServiceBuilder(dbName).WithTenant(2);
        var svcB = builderB.Build();

        var result = await svcB.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ForceTerminate_Does_Not_Cancel_OtherTenant_Instances()
    {
        // UNIQUE shared DB name for this test execution
        var sharedDbName = nameof(ForceTerminate_Does_Not_Cancel_OtherTenant_Instances) + "_" + Guid.NewGuid().ToString("N");

        // Tenant 1 setup
        var b1 = new DefinitionServiceBuilder(sharedDbName).WithTenant(1);
        var s1 = b1.Build();
        var def1 = (await s1.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow1", JSONDefinition = SimpleJson })).Data!;
        await s1.PublishAsync(def1.Id, new PublishDefinitionRequestDto());

        b1.Context.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def1.Id,
            DefinitionVersion = def1.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[\"start\"]",
            Context = "{}"
        });
        await b1.Context.SaveChangesAsync();

        // Tenant 2 setup (same DB name)
        var b2 = new DefinitionServiceBuilder(sharedDbName).WithTenant(2);
        var s2 = b2.Build();
        var def2 = (await s2.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow2", JSONDefinition = SimpleJson })).Data!;
        await s2.PublishAsync(def2.Id, new PublishDefinitionRequestDto());

        b2.Context.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 2,
            WorkflowDefinitionId = def2.Id,
            DefinitionVersion = def2.Version,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[\"start\"]",
            Context = "{}"
        });
        await b2.Context.SaveChangesAsync();

        // Tenant 2 force-terminates its own active instances
        var unpub2 = await s2.UnpublishAsync(def2.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true });

        // Enhanced assertion to surface diagnostic info if flaky again
        Assert.True(unpub2.Success,
            $"Expected force unpublish to succeed. Message={unpub2.Message}, Errors=[{string.Join(",",
                unpub2.Errors?.Select(e => e.Code+":"+e.Message) ?? Enumerable.Empty<string>())}]");

        // Verify tenant 1 instance remains Running
        var tenant1Instance = await b1.Context.WorkflowInstances
            .Where(i => i.TenantId == 1)
            .FirstAsync();

        Assert.Equal(InstanceStatus.Running, tenant1Instance.Status);
    }
}
