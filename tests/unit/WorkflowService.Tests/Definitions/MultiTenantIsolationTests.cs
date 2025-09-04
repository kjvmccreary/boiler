using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
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
        // Create definition under tenant 1
        var builderA = new DefinitionServiceBuilder(nameof(TenantA_Cannot_Unpublish_TenantB_Definition)).WithTenant(1);
        var svcA = builderA.Build();
        var def = (await svcA.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="SharedFlow", JSONDefinition = SimpleJson })).Data!;
        await svcA.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        // Switch to tenant 2 using a new service on same DB (different tenant id)
        var builderB = new DefinitionServiceBuilder(nameof(TenantA_Cannot_Unpublish_TenantB_Definition)).WithTenant(2);
        // NOTE: new builder uses fresh in‑memory DB; reuse same db name so memory DB is shared
        var svcB = builderB.Build();

        // Attempt unpublish under tenant 2 (should not find it)
        var result = await svcB.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message!, System.StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ForceTerminate_Does_Not_Cancel_OtherTenant_Instances()
    {
        var dbName = nameof(ForceTerminate_Does_Not_Cancel_OtherTenant_Instances);

        // Tenant 1 setup
        var b1 = new DefinitionServiceBuilder(dbName).WithTenant(1);
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

        // Tenant 2 setup in same DB name (shared in-memory store) - separate definition
        var b2 = new DefinitionServiceBuilder(dbName).WithTenant(2);
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

        // Tenant 2 force terminates its own
        var unpub2 = await s2.UnpublishAsync(def2.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true });
        Assert.True(unpub2.Success);

        // Verify tenant 1 instance still running
        var tenant1Instance = await b1.Context.WorkflowInstances
            .Where(i => i.TenantId == 1)
            .FirstAsync();
        Assert.Equal(InstanceStatus.Running, tenant1Instance.Status);
    }
}
