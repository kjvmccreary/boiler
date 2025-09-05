using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using WorkflowService.Controllers;
using WorkflowService.Domain.Models;
using WorkflowService.Outbox;
using WorkflowService.Persistence;
using Xunit;
using Contracts.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace WorkflowService.Tests.Outbox;

public class OutboxAdminEndpointTests
{
    private class FixedTenantProvider : ITenantProvider
    {
        private readonly int _tenant;
        public FixedTenantProvider(int tenant) => _tenant = tenant;
        public Task<int?> GetCurrentTenantIdAsync() => Task.FromResult<int?>(_tenant);
        public Task<string?> GetCurrentTenantIdentifierAsync() => Task.FromResult<string?>(_tenant.ToString());
        public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
        public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
        public Task ClearCurrentTenantAsync() => Task.CompletedTask;
        public bool HasTenantContext => true;
    }

    private WorkflowDbContext CreateContext()
    {
        var opts = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new WorkflowDbContext(
            opts,
            new HttpContextAccessor(),
            new FixedTenantProvider(1),
            new NullLogger<WorkflowDbContext>());
    }

    private static OutboxMessage Build(
        string type,
        bool processed = false,
        bool failed = false,
        bool dead = false)
        => new()
        {
            TenantId = 1,
            EventType = type,
            EventData = "{}",
            IdempotencyKey = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10),
            RetryCount = failed ? 2 : 0,
            ProcessedAt = processed ? DateTime.UtcNow.AddMinutes(-1) : null,
            IsProcessed = processed,
            DeadLetter = dead
        };

    [Fact]
    public async Task Admin_List_Pending_Returns_Unprocessed_NonFailed()
    {
        using var ctx = CreateContext();
        ctx.OutboxMessages.AddRange(
            Build("a.pending"),
            Build("b.failed", failed: true),
            Build("c.processed", processed: true),
            Build("d.dead", dead: true));
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));
        var result = await controller.GetAsync(status: "pending");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.All(page.Items, i =>
        {
            Assert.Null(i.ProcessedAt);
            Assert.False(i.DeadLetter);
        });
        Assert.DoesNotContain(page.Items, i => i.RetryCount > 0);
    }

    [Fact]
    public async Task Admin_Filter_DeadLetter_Returns_Only_DeadLetters()
    {
        using var ctx = CreateContext();
        ctx.OutboxMessages.AddRange(
            Build("x1.dead", dead: true),
            Build("x2.dead", dead: true),
            Build("y.pending"),
            Build("z.failed", failed: true));
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));
        var result = await controller.GetAsync(status: "deadletter", pageSize: 10);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.True(page.Items.Count >= 2);
        Assert.All(page.Items, i => Assert.True(i.DeadLetter));
    }

    [Fact]
    public async Task Admin_EventType_Prefix_Filter_Works()
    {
        using var ctx = CreateContext();
        ctx.OutboxMessages.AddRange(
            Build("workflow.task.created"),
            Build("workflow.task.completed", processed: true),
            Build("workflow.instance.started"));
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));
        var result = await controller.GetAsync(status: "all", eventType: "workflow.task.*");
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.All(page.Items, i => Assert.StartsWith("workflow.task.", i.EventType));
        Assert.DoesNotContain(page.Items, i => i.EventType == "workflow.instance.started");
    }

    [Fact]
    public async Task Admin_Pagination_Works()
    {
        using var ctx = CreateContext();
        for (int i = 0; i < 75; i++)
            ctx.OutboxMessages.Add(Build($"evt.{i}"));
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));
        var result = await controller.GetAsync(status: "pending", page: 2, pageSize: 25);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.Equal(2, page.Page);
        Assert.Equal(25, page.PageSize);
        Assert.Equal(75, page.TotalCount);
        Assert.Equal(3, page.TotalPages);
        Assert.Equal(25, page.Items.Count);
    }

    [Fact]
    public async Task Admin_DateRange_And_Retry_Filters()
    {
        using var ctx = CreateContext();
        var now = DateTime.UtcNow;

        var m1 = Build("t.range1");
        m1.CreatedAt = now.AddHours(-5);
        m1.UpdatedAt = m1.CreatedAt;
        m1.RetryCount = 0;

        var m2 = Build("t.range2");
        m2.CreatedAt = now.AddHours(-2);
        m2.UpdatedAt = m2.CreatedAt;
        m2.RetryCount = 3;

        var m3 = Build("t.range3");
        m3.CreatedAt = now.AddMinutes(-30);
        m3.UpdatedAt = m3.CreatedAt;
        m3.RetryCount = 5;

        ctx.OutboxMessages.AddRange(m1, m2, m3);
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));

        var min = now.AddHours(-3);
        var max = now.AddMinutes(-10);

        var result = await controller.GetAsync(
            status: "all",
            minCreatedUtc: min,
            maxCreatedUtc: max,
            minRetry: 2,
            maxRetry: 4);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.Single(page.Items);
        Assert.InRange(page.Items[0].RetryCount, 2, 4);
        Assert.True(page.Items[0].CreatedAt >= min && page.Items[0].CreatedAt <= max);
    }

    [Fact]
    public async Task Admin_StaleSeconds_Filters_Unprocessed_Older_Than_Cutoff()
    {
        using var ctx = CreateContext();
        var oldMsg = Build("stale.old");
        oldMsg.CreatedAt = DateTime.UtcNow.AddMinutes(-30);
        oldMsg.UpdatedAt = oldMsg.CreatedAt;

        var freshMsg = Build("stale.fresh");
        freshMsg.CreatedAt = DateTime.UtcNow.AddMinutes(-1);
        freshMsg.UpdatedAt = freshMsg.CreatedAt;

        ctx.OutboxMessages.Add(oldMsg);
        ctx.OutboxMessages.Add(freshMsg);
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));
        var result = await controller.GetAsync(status: "pending", staleSeconds: 600);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.Single(page.Items);
        Assert.Equal("stale.old", page.Items[0].EventType);
    }

    [Fact]
    public async Task Admin_HasError_Filter_Works()
    {
        using var ctx = CreateContext();
        var err = Build("err.with");
        err.Error = "Boom";
        err.RetryCount = 1;

        var noErr = Build("err.without");

        ctx.OutboxMessages.AddRange(err, noErr);
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));

        var withErr = await controller.GetAsync(status: "all", hasError: true);
        var ok1 = Assert.IsType<OkObjectResult>(withErr.Result);
        var page1 = Assert.IsType<OutboxAdminPageDto>(ok1.Value);
        Assert.Single(page1.Items);
        Assert.Equal("err.with", page1.Items[0].EventType);

        var withoutErr = await controller.GetAsync(status: "all", hasError: false);
        var ok2 = Assert.IsType<OkObjectResult>(withoutErr.Result);
        var page2 = Assert.IsType<OutboxAdminPageDto>(ok2.Value);
        Assert.Contains(page2.Items, i => i.EventType == "err.without");
        Assert.DoesNotContain(page2.Items, i => i.EventType == "err.with");
    }

    [Fact]
    public async Task Admin_Filter_By_IdempotencyKey()
    {
        using var ctx = CreateContext();
        var m1 = Build("k.one");
        var m2 = Build("k.two");
        ctx.OutboxMessages.AddRange(m1, m2);
        ctx.SaveChanges();

        var controller = new OutboxAdminController(ctx, Options.Create(new OutboxOptions()));
        var result = await controller.GetAsync(status: "all", idempotencyKey: m2.IdempotencyKey);
        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var page = Assert.IsType<OutboxAdminPageDto>(ok.Value);

        Assert.Single(page.Items);
        Assert.Equal(m2.IdempotencyKey, page.Items[0].IdempotencyKey);
    }
}
