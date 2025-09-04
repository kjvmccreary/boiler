using Xunit;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using System;
using Microsoft.EntityFrameworkCore;
using DTOs.Workflow.Enums;

namespace WorkflowService.Tests.Isolation;

public class DefinitionIsolationTests
{
    [Fact]
    public async Task GetAll_Should_Not_Return_Other_Tenant_Definitions()
    {
        var dbName = nameof(GetAll_Should_Not_Return_Other_Tenant_Definitions);
        var ctxTenant1 = TenantFilteredDbContextFactory.Create(dbName, 1);
        var ctxTenant2 = TenantFilteredDbContextFactory.Create(dbName, 2);

        // Seed tenant 1 + tenant 2
        ctxTenant1.WorkflowDefinitions.Add(new WorkflowDefinition
        {
            TenantId = 1, Name = "T1Def", Version = 1, JSONDefinition = "{}", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        });
        await ctxTenant1.SaveChangesAsync();

        ctxTenant2.WorkflowDefinitions.Add(new WorkflowDefinition
        {
            TenantId = 2, Name = "T2Def", Version = 1, JSONDefinition = "{}", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        });
        await ctxTenant2.SaveChangesAsync();

        // Query as tenant 1
        var t1Names = await ctxTenant1.WorkflowDefinitions.Select(d => d.Name).ToListAsync();
        Assert.Contains("T1Def", t1Names);
        Assert.DoesNotContain("T2Def", t1Names);

        // Query as tenant 2
        var t2Names = await ctxTenant2.WorkflowDefinitions.Select(d => d.Name).ToListAsync();
        Assert.Contains("T2Def", t2Names);
        Assert.DoesNotContain("T1Def", t2Names);
    }
}
