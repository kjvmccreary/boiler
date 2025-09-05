using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Utilities;
using Xunit;

namespace WorkflowService.Tests.Outbox;

public class EventPublisherOutboxTests
{
    private sealed class NoopOutboxWriterLogger<T> : NullLogger<T> { }

    private static WorkflowDbContext CreateSqliteContext()
    {
        // Use SQLite in-memory so unique index constraints are enforced (unlike EF InMemory provider)
        var conn = new SqliteConnection("DataSource=:memory:");
        conn.Open();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(conn)
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new WorkflowDbContext(
            options,
            new HttpContextAccessorStub(),
            new FixedTenantProvider(tenantId: 1),
            new NullLogger<WorkflowDbContext>());

        ctx.Database.EnsureCreated();
        return ctx;
    }

    #region Stubs / Helpers

    private class FixedTenantProvider : Contracts.Services.ITenantProvider
    {
        private readonly int _tenantId;
        public FixedTenantProvider(int tenantId) => _tenantId = tenantId;
        public Task<int?> GetCurrentTenantIdAsync() => Task.FromResult<int?>(_tenantId);
        public Task<string?> GetCurrentTenantIdentifierAsync() => Task.FromResult<string?>(_tenantId.ToString());
        public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
        public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
        public Task ClearCurrentTenantAsync() => Task.CompletedTask;
        public bool HasTenantContext => true;
    }

    private sealed class HttpContextAccessorStub : Microsoft.AspNetCore.Http.IHttpContextAccessor
    {
        public Microsoft.AspNetCore.Http.HttpContext? HttpContext { get; set; }
    }

    private static WorkflowDefinition SeedDefinition(WorkflowDbContext ctx, int idSeed = 0)
    {
        var def = new WorkflowDefinition
        {
            TenantId = 1,
            Name = "Definition-" + idSeed,
            Version = 1,
            JSONDefinition = """
            {
              "nodes":[
                {"id":"start","type":"Start","properties":{}},
                {"id":"end","type":"End","properties":{}}
              ],
              "edges":[{"id":"e1","source":"start","target":"end"}]
            }
            """,
            IsPublished = true,
            PublishedAt = DateTime.UtcNow.AddMinutes(-5),
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        ctx.WorkflowDefinitions.Add(def);
        ctx.SaveChanges();
        return def;
    }

    private static WorkflowInstance SeedInstance(WorkflowDbContext ctx, WorkflowDefinition def, int idSeed = 0)
    {
        var inst = new WorkflowInstance
        {
            TenantId = def.TenantId,
            WorkflowDefinitionId = def.Id,
            DefinitionVersion = def.Version,
            Status = DTOs.Workflow.Enums.InstanceStatus.Running,
            CurrentNodeIds = "[]",
            Context = "{}",
            StartedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            StartedByUserId = 7
        };
        ctx.WorkflowInstances.Add(inst);
        ctx.SaveChanges();
        return inst;
    }

    private static WorkflowTask SeedTask(WorkflowDbContext ctx, WorkflowInstance inst, string nodeId = "n1")
    {
        var task = new WorkflowTask
        {
            TenantId = inst.TenantId,
            WorkflowInstanceId = inst.Id,
            NodeId = nodeId,
            TaskName = "Task-" + nodeId,
            NodeType = "human",
            Status = DTOs.Workflow.Enums.TaskStatus.Created,
            Data = "{}",
            CreatedAt = DateTime.UtcNow.AddMinutes(-20),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5)
        };
        ctx.WorkflowTasks.Add(task);
        ctx.SaveChanges();
        return task;
    }

    private static EventPublisher CreatePublisher(WorkflowDbContext ctx)
    {
        IOutboxWriter writer = new OutboxWriter(ctx, new NullLogger<OutboxWriter>());
        return new EventPublisher(ctx, new NullLogger<EventPublisher>(), writer);
    }

    #endregion

    [Fact]
    public async Task InstanceStarted_Should_Create_Outbox_With_Deterministic_Key()
    {
        using var ctx = CreateSqliteContext();
        var def = SeedDefinition(ctx);
        var inst = SeedInstance(ctx, def);

        var publisher = CreatePublisher(ctx);

        await publisher.PublishInstanceStartedAsync(inst);

        var msg = ctx.OutboxMessages.Single();
        var expected = OutboxIdempotency.CreateForWorkflow(inst.TenantId, "instance", inst.Id, "started", inst.DefinitionVersion);

        Assert.Equal("workflow.instance.started", msg.EventType);
        Assert.Equal(expected, msg.IdempotencyKey);
        Assert.False(msg.IsProcessed);
    }

    [Fact]
    public async Task DefinitionPublished_Twice_Should_Remain_Single_Row_Idempotent()
    {
        using var ctx = CreateSqliteContext();
        var def = SeedDefinition(ctx);
        var publisher = CreatePublisher(ctx);

        // First publish
        await publisher.PublishDefinitionPublishedAsync(def);
        var first = ctx.OutboxMessages.Single();
        var expectedKey = OutboxIdempotency.CreateForWorkflow(def.TenantId, "definition", def.Id, "published", def.Version);
        Assert.Equal(expectedKey, first.IdempotencyKey);

        // Second publish (same logical event) – should not create duplicate row
        await publisher.PublishDefinitionPublishedAsync(def);

        // Re-query to ensure only 1 row
        var all = ctx.OutboxMessages.Where(o => o.EventType == "workflow.definition.published").ToList();
        Assert.Single(all);
        Assert.Equal(expectedKey, all[0].IdempotencyKey);
    }

    [Fact]
    public async Task TaskCreated_Should_Use_Deterministic_Key()
    {
        using var ctx = CreateSqliteContext();
        var def = SeedDefinition(ctx);
        var inst = SeedInstance(ctx, def);
        var task = SeedTask(ctx, inst);

        var publisher = CreatePublisher(ctx);

        await publisher.PublishTaskCreatedAsync(task);

        var msg = ctx.OutboxMessages.Single(o => o.EventType == "workflow.task.created");
        var expected = OutboxIdempotency.CreateForWorkflow(task.TenantId, "task", task.Id, "created");
        Assert.Equal(expected, msg.IdempotencyKey);
    }

    [Fact]
    public async Task TaskAssigned_Twice_NoOverride_Should_Produce_Different_Keys()
    {
        using var ctx = CreateSqliteContext();
        var def = SeedDefinition(ctx);
        var inst = SeedInstance(ctx, def);
        var task = SeedTask(ctx, inst);

        var publisher = CreatePublisher(ctx);

        await publisher.PublishTaskAssignedAsync(task, assignedToUserId: 25);
        await publisher.PublishTaskAssignedAsync(task, assignedToUserId: 25);

        var assignedMsgs = ctx.OutboxMessages
            .Where(o => o.EventType == "workflow.task.assigned")
            .OrderBy(o => o.CreatedAt)
            .ToList();

        Assert.Equal(2, assignedMsgs.Count);
        Assert.NotEqual(assignedMsgs[0].IdempotencyKey, assignedMsgs[1].IdempotencyKey);
    }

    [Fact]
    public async Task Override_Key_Should_Be_Used_Instead_Of_Deterministic()
    {
        using var ctx = CreateSqliteContext();
        var def = SeedDefinition(ctx);
        var inst = SeedInstance(ctx, def);
        var publisher = CreatePublisher(ctx);

        var customKey = Guid.NewGuid();

        // Use extended overload with override
        await publisher.PublishInstanceStartedWithKeyAsync(inst, customKey);

        var msg = ctx.OutboxMessages.Single();
        Assert.Equal(customKey, msg.IdempotencyKey);

        // Deterministic would differ
        var deterministic = OutboxIdempotency.CreateForWorkflow(inst.TenantId, "instance", inst.Id, "started", inst.DefinitionVersion);
        Assert.NotEqual(deterministic, customKey);
    }

    [Fact]
    public async Task InstanceCompleted_Deterministic_Key_Stable()
    {
        using var ctx = CreateSqliteContext();
        var def = SeedDefinition(ctx);
        var inst = SeedInstance(ctx, def);
        inst.CompletedAt = DateTime.UtcNow;
        ctx.SaveChanges();

        var publisher = CreatePublisher(ctx);

        await publisher.PublishInstanceCompletedAsync(inst);
        var key1 = ctx.OutboxMessages.Single().IdempotencyKey;

        // Simulate second attempt (e.g., retry path) — should still be idempotent
        await publisher.PublishInstanceCompletedAsync(inst);
        var rows = ctx.OutboxMessages.Where(o => o.EventType == "workflow.instance.completed").ToList();

        Assert.Single(rows);
        var expected = OutboxIdempotency.CreateForWorkflow(inst.TenantId, "instance", inst.Id, "completed", inst.DefinitionVersion);
        Assert.Equal(expected, key1);
        Assert.Equal(expected, rows[0].IdempotencyKey);
    }
}
