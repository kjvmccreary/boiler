using Xunit;
using WorkflowService.Tests.TestSupport;
using WorkflowService.Domain.Models;
using System;
using Microsoft.EntityFrameworkCore;
using DTOs.Workflow.Enums;
using WorkflowTaskStatus = DTOs.Workflow.Enums.TaskStatus; // Alias to avoid ambiguity with System.Threading.Tasks.TaskStatus

namespace WorkflowService.Tests.Isolation;

public class TaskIsolationTests
{
    [Fact]
    public async Task Tasks_Query_Should_Filter_By_Tenant()
    {
        var dbName = nameof(Tasks_Query_Should_Filter_By_Tenant);
        var ctx1 = TenantFilteredDbContextFactory.Create(dbName, 1);
        var ctx2 = TenantFilteredDbContextFactory.Create(dbName, 2);

        var def1 = new WorkflowDefinition { TenantId=1, Name="D1", Version=1, JSONDefinition="{}", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow };
        var def2 = new WorkflowDefinition { TenantId=2, Name="D2", Version=1, JSONDefinition="{}", CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow };
        ctx1.WorkflowDefinitions.Add(def1);
        await ctx1.SaveChangesAsync();
        ctx2.WorkflowDefinitions.Add(def2);
        await ctx2.SaveChangesAsync();

        var inst1 = new WorkflowInstance
        {
            TenantId=1, WorkflowDefinitionId=def1.Id, DefinitionVersion=1,
            Status=InstanceStatus.Running, CurrentNodeIds="[]", Context="{}",
            StartedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        ctx1.WorkflowInstances.Add(inst1);
        await ctx1.SaveChangesAsync();

        var inst2 = new WorkflowInstance
        {
            TenantId=2, WorkflowDefinitionId=def2.Id, DefinitionVersion=1,
            Status=InstanceStatus.Running, CurrentNodeIds="[]", Context="{}",
            StartedAt = DateTime.UtcNow, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow
        };
        ctx2.WorkflowInstances.Add(inst2);
        await ctx2.SaveChangesAsync();

        ctx1.WorkflowTasks.Add(new WorkflowTask
        {
            TenantId=1, WorkflowInstanceId=inst1.Id, NodeId="n1",
            TaskName="T1", NodeType="human", Status=WorkflowTaskStatus.Created,
            CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow
        });
        await ctx1.SaveChangesAsync();

        ctx2.WorkflowTasks.Add(new WorkflowTask
        {
            TenantId=2, WorkflowInstanceId=inst2.Id, NodeId="n1",
            TaskName="T2", NodeType="human", Status=WorkflowTaskStatus.Created,
            CreatedAt=DateTime.UtcNow, UpdatedAt=DateTime.UtcNow
        });
        await ctx2.SaveChangesAsync();

        var t1Tasks = await ctx1.WorkflowTasks.ToListAsync();
        var t2Tasks = await ctx2.WorkflowTasks.ToListAsync();

        Assert.Single(t1Tasks);
        Assert.Single(t2Tasks);

        Assert.DoesNotContain(t1Tasks, t => t.TenantId == 2);
        Assert.DoesNotContain(t2Tasks, t => t.TenantId == 1);
    }
}
