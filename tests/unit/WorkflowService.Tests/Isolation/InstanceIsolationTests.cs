using Xunit;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using System;
using Microsoft.EntityFrameworkCore;
using DTOs.Workflow.Enums;

namespace WorkflowService.Tests.Isolation;

public class InstanceIsolationTests
{
    [Fact]
    public async Task Instances_Query_Should_Filter_By_Tenant()
    {
        var dbName = nameof(Instances_Query_Should_Filter_By_Tenant);
        var ctx1 = TenantFilteredDbContextFactory.Create(dbName, 1);
        var ctx2 = TenantFilteredDbContextFactory.Create(dbName, 2);

        // Shared definition IDs (simulate same ID across tenants logically)
        ctx1.WorkflowDefinitions.Add(new WorkflowDefinition
        {
            TenantId = 1, Name = "Flow", Version = 1, JSONDefinition = "{}", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow
        });
        await ctx1.SaveChangesAsync();

        ctx2.WorkflowDefinitions.Add(new WorkflowDefinition
        {
            TenantId = 2, Name = "Flow", Version = 1, JSONDefinition = "{}", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow
        });
        await ctx2.SaveChangesAsync();

        var def1Id = await ctx1.WorkflowDefinitions.Select(d=>d.Id).FirstAsync();
        var def2Id = await ctx2.WorkflowDefinitions.Select(d=>d.Id).FirstAsync();

        ctx1.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 1,
            WorkflowDefinitionId = def1Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await ctx1.SaveChangesAsync();

        ctx2.WorkflowInstances.Add(new WorkflowInstance
        {
            TenantId = 2,
            WorkflowDefinitionId = def2Id,
            DefinitionVersion = 1,
            Status = InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });
        await ctx2.SaveChangesAsync();

        var tenant1Count = await ctx1.WorkflowInstances.CountAsync();
        var tenant2Count = await ctx2.WorkflowInstances.CountAsync();

        Assert.Equal(1, tenant1Count);
        Assert.Equal(1, tenant2Count);

        // Cross context should not see other tenant's instance
        Assert.Equal(0, await ctx1.WorkflowInstances.CountAsync(i => i.TenantId == 2));
        Assert.Equal(0, await ctx2.WorkflowInstances.CountAsync(i => i.TenantId == 1));
    }
}
