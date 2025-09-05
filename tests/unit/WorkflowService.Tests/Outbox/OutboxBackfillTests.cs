using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection; // <-- ADDED (IServiceScopeFactory, IServiceScope)
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using WorkflowService.Outbox;
using Xunit;
using Contracts.Services;

namespace WorkflowService.Tests.Outbox;

public class OutboxBackfillTests
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

    [Fact]
    public async Task LegacyRow_SaveChanges_Assigns_IdempotencyKey()
    {
        using var ctx = CreateContext();

        ctx.OutboxMessages.Add(new OutboxMessage
        {
            TenantId = 1,
            EventType = "legacy.event",
            EventData = "{}",
            IdempotencyKey = Guid.Empty,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        });

        await ctx.SaveChangesAsync();

        var row = ctx.OutboxMessages.Single();
        Assert.NotEqual(Guid.Empty, row.IdempotencyKey);
    }

    [Fact]
    public async Task BackfillWorker_Assigns_Multiple_Missing_Keys()
    {
        using var ctx = CreateContext();

        for (int i = 0; i < 5; i++)
        {
            ctx.OutboxMessages.Add(new OutboxMessage
            {
                TenantId = 1,
                EventType = $"legacy.batch.{i}",
                EventData = "{}",
                IdempotencyKey = Guid.Empty,
                CreatedAt = DateTime.UtcNow.AddMinutes(-i),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-i)
            });
        }
        await ctx.SaveChangesAsync();

        foreach (var msg in ctx.OutboxMessages.Take(3))
        {
            msg.IdempotencyKey = Guid.Empty;
        }
        await ctx.SaveChangesAsync();

        var opts = Options.Create(new OutboxBackfillOptions
        {
            Enabled = true,
            BatchSize = 10
        });

        var sp = new TestServiceProvider(ctx, opts);
        var logger = new NullLogger<OutboxIdempotencyBackfillWorker>();
        var worker = new OutboxIdempotencyBackfillWorker(sp, opts, logger);

        await worker.StartAsync(default);
        await Task.Delay(100);

        foreach (var row in ctx.OutboxMessages)
        {
            Assert.NotEqual(Guid.Empty, row.IdempotencyKey);
        }
    }

    private sealed class TestServiceProvider : IServiceProvider, IServiceScopeFactory, IServiceScope
    {
        private readonly WorkflowDbContext _db;
        private readonly IOptions<OutboxBackfillOptions> _backfillOptions;

        public TestServiceProvider(WorkflowDbContext db, IOptions<OutboxBackfillOptions> opts)
        {
            _db = db;
            _backfillOptions = opts;
        }

        public object? GetService(Type serviceType)
        {
            if (serviceType == typeof(WorkflowDbContext)) return _db;
            if (serviceType == typeof(IServiceScopeFactory)) return this;
            if (serviceType == typeof(IServiceScope)) return this;
            if (serviceType == typeof(IOptions<OutboxBackfillOptions>)) return _backfillOptions;
            return null;
        }

        public IServiceScope CreateScope() => this;
        public IServiceProvider ServiceProvider => this;
        public void Dispose() { }
    }
}
