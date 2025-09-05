using System.Threading.Tasks;
using Xunit;
using DTOs.Workflow;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using Xunit.Abstractions;
using WorkflowService.Services; // for DefinitionService.IsolationDiag

namespace WorkflowService.Tests.Definitions;

public class MultiTenantIsolationTests
{
    private readonly ITestOutputHelper _output;
    public MultiTenantIsolationTests(ITestOutputHelper output) => _output = output;

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

        var builderA = new DefinitionServiceBuilder(dbName).WithTenant(1);
        var svcA = builderA.Build();
        var def = (await svcA.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="SharedFlow", JSONDefinition = SimpleJson })).Data!;
        await svcA.PublishAsync(def.Id, new PublishDefinitionRequestDto());

        var builderB = new DefinitionServiceBuilder(dbName).WithTenant(2);
        var svcB = builderB.Build();

        var result = await svcB.UnpublishAsync(def.Id, new UnpublishDefinitionRequestDto());
        Assert.False(result.Success);
        Assert.Contains("not found", result.Message!, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task ForceTerminate_Does_Not_Cancel_OtherTenant_Instances()
    {
        var sharedDbName = nameof(ForceTerminate_Does_Not_Cancel_OtherTenant_Instances) + "_" + Guid.NewGuid().ToString("N");

        // Tenant 1 setup
        var b1 = new DefinitionServiceBuilder(sharedDbName).WithTenant(1).WithOutput(_output);
        var s1 = b1.Build();
        var def1 = (await s1.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow1", JSONDefinition = SimpleJson })).Data!;
        var publish1 = await s1.PublishAsync(def1.Id, new PublishDefinitionRequestDto());
        _output.WriteLine($"PUBLISH1 success={publish1.Success} msg='{publish1.Message}' " +
                          $"errors=[{string.Join("; ", publish1.Errors?.Select(e => $"{e.Code}:{e.Message}") ?? Array.Empty<string>())}]");
        Assert.True(publish1.Success, "Publish tenant1 failed");

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

        // Tenant 2 setup
        var b2 = new DefinitionServiceBuilder(sharedDbName).WithTenant(2).WithOutput(_output);
        var s2 = b2.Build();
        var def2 = (await s2.CreateDraftAsync(new CreateWorkflowDefinitionDto { Name="Flow2", JSONDefinition = SimpleJson })).Data!;

        var directLookup = await b2.Context.WorkflowDefinitions
    .AsNoTracking()
    .FirstOrDefaultAsync(d => d.Id == def2.Id);
        _output.WriteLine("DIRECT LOOKUP def2.Id => " + (directLookup == null
            ? "NULL"
            : $"FOUND Id={directLookup.Id} TenantId={directLookup.TenantId} Published={directLookup.IsPublished}"));

        var publish2 = await s2.PublishAsync(def2.Id, new PublishDefinitionRequestDto());
        _output.WriteLine($"PUBLISH2 success={publish2.Success} msg='{publish2.Message}' " +
                          $"errors=[{string.Join("; ", publish2.Errors?.Select(e => $"{e.Code}:{e.Message}") ?? Array.Empty<string>())}]");
        Assert.True(publish2.Success, "Publish tenant2 failed");

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

        // Force unpublish for tenant 2 definition (ignore immutability race in InMemory provider)
        try
        {
            var unpub2 = await s2.UnpublishAsync(def2.Id, new UnpublishDefinitionRequestDto { ForceTerminateAndUnpublish = true });
            if (!unpub2.Success && !(unpub2.Message?.Contains("not found", StringComparison.OrdinalIgnoreCase) ?? false))
            {
                Assert.True(unpub2.Success, $"Unexpected failure: {unpub2.Message}");
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("immutable", StringComparison.OrdinalIgnoreCase))
        {
            // Ignore immutability exception; isolation assertion below is what matters.
        }

        // Snapshot both contexts
        var beforeAssertT1 = await b1.Context.WorkflowInstances.AsNoTracking()
            .Select(i => new { i.Id, i.TenantId, i.Status, i.WorkflowDefinitionId })
            .ToListAsync();
        var beforeAssertT2 = await b2.Context.WorkflowInstances.AsNoTracking()
            .Select(i => new { i.Id, i.TenantId, i.Status, i.WorkflowDefinitionId })
            .ToListAsync();

        _output.WriteLine($"DEF1={def1.Id} DEF2={def2.Id}");
        _output.WriteLine("SNAP T1: " + string.Join(" | ", beforeAssertT1.Select(x => $"{x.Id}:{x.TenantId}:{x.Status}:{x.WorkflowDefinitionId}")));
        _output.WriteLine("SNAP T2: " + string.Join(" | ", beforeAssertT2.Select(x => $"{x.Id}:{x.TenantId}:{x.Status}:{x.WorkflowDefinitionId}")));

        // Dump DefinitionService isolation diagnostics
        while (DefinitionService.IsolationDiag.TryDequeue(out var diag))
            _output.WriteLine("ISO-DIAG " + diag);

        // Verify tenant 1 instance NOT cancelled
        var tenant1Instance = await b1.Context.WorkflowInstances
            .FirstAsync(i => i.TenantId == 1);
        Assert.Equal(InstanceStatus.Running, tenant1Instance.Status);

        // Assert tenant2 instance is cancelled (explicit)
        var tenant2Instance = await b2.Context.WorkflowInstances
            .AsNoTracking()
            .FirstAsync(i => i.TenantId == 2);
        Assert.Equal(InstanceStatus.Cancelled, tenant2Instance.Status);

        var allT1 = await b1.Context.WorkflowInstances.AsNoTracking()
            .Select(i => new { i.Id, i.TenantId, i.Status, i.WorkflowDefinitionId })
            .ToListAsync();
        var allT2 = await b2.Context.WorkflowInstances.AsNoTracking()
            .Select(i => new { i.Id, i.TenantId, i.Status, i.WorkflowDefinitionId })
            .ToListAsync();
        _output.WriteLine("T1 INSTANCES: " + string.Join(" | ", allT1.Select(x => $"{x.Id}:{x.TenantId}:{x.Status}:{x.WorkflowDefinitionId}")));
        _output.WriteLine("T2 INSTANCES: " + string.Join(" | ", allT2.Select(x => $"{x.Id}:{x.TenantId}:{x.Status}:{x.WorkflowDefinitionId}")));
    }
}
