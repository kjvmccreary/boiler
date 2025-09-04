using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using DTOs.Workflow.Enums;

namespace WorkflowService.IntegrationTests.Infrastructure;

public static class MultiTenantWorkflowSeeder
{
    public static async Task<(WorkflowDefinition def1, WorkflowDefinition def2)> SeedDefinitionsAsync(
        WorkflowDbContext ctx, int tenant1, int tenant2, string baseName = "IsoFlow")
    {
        var d1 = new WorkflowDefinition
        {
            TenantId = tenant1,
            Name = $"{baseName}-T{tenant1}",
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[{"id":"start","type":"start"},{"id":"end","type":"end"}],
              "edges":[{"id":"e1","from":"start","to":"end"}]
            }
            """,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };
        var d2 = new WorkflowDefinition
        {
            TenantId = tenant2,
            Name = $"{baseName}-T{tenant2}",
            Version = 1,
            JSONDefinition = d1.JSONDefinition,
            IsPublished = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            PublishedAt = DateTime.UtcNow
        };
        ctx.WorkflowDefinitions.AddRange(d1, d2);
        await ctx.SaveChangesAsync();
        return (d1, d2);
    }

    public static async Task<WorkflowInstance> SeedInstanceAsync(
        WorkflowDbContext ctx, WorkflowDefinition def, int tenantId)
    {
        var inst = new WorkflowInstance
        {
            TenantId = tenantId,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            CurrentNodeIds = "[\"start\"]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ctx.WorkflowInstances.Add(inst);
        await ctx.SaveChangesAsync();
        return inst;
    }

    public static async Task<WorkflowTask> SeedTaskAsync(
        WorkflowDbContext ctx,
        WorkflowInstance inst,
        int tenantId,
        string taskName = "Human Task",
        TaskStatus status = TaskStatus.Created,
        int? assignedUserId = null)
    {
        var task = new WorkflowTask
        {
            TenantId = tenantId,
            WorkflowInstanceId = inst.Id,
            NodeId = "human1",
            TaskName = taskName,
            NodeType = "human",
            Status = status,
            AssignedToUserId = assignedUserId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        ctx.WorkflowTasks.Add(task);
        await ctx.SaveChangesAsync();
        return task;
    }

    public static async Task ForceCancelInstanceAsync(
        WorkflowDbContext ctx,
        WorkflowInstance inst)
    {
        inst.Status = InstanceStatus.Cancelled;
        inst.CompletedAt = DateTime.UtcNow;
        inst.UpdatedAt = DateTime.UtcNow;
        await ctx.SaveChangesAsync();
    }
}
