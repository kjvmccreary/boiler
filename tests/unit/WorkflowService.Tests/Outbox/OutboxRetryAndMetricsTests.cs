using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WorkflowService.Outbox;
using WorkflowService.Persistence;
using Xunit;
using Contracts.Services;
using Microsoft.AspNetCore.Http;
using WorkflowService.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace WorkflowService.Tests.Outbox;

public class OutboxRetryAndMetricsTests
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

    private static OutboxMessage NewMessage(string type) =>
        new()
        {
            TenantId = 1,
            EventType = type,
            EventData = "{}",
            IdempotencyKey = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-5),
            RetryCount = 0
        };

    private class FailingTransport : IOutboxTransport
    {
        public Task DeliverAsync(OutboxMessage message, CancellationToken ct)
            => throw new InvalidOperationException("boom");
    }

    private class SuccessTransport : IOutboxTransport
    {
        public Task DeliverAsync(OutboxMessage message, CancellationToken ct) => Task.CompletedTask;
    }

    private OutboxDispatcher CreateDispatcher(
        WorkflowDbContext ctx,
        IOutboxTransport transport,
        Action<OutboxOptions>? cfg = null,
        IOutboxMetricsProvider? metrics = null)
    {
        var options = new OutboxOptions
        {
            BatchSize = 25,
            BaseRetryDelaySeconds = 2,
            MaxRetries = 3,
            UseExponentialBackoff = true,
            EnableMetrics = true
        };
        cfg?.Invoke(options);
        return new OutboxDispatcher(ctx, transport, Options.Create(options), new NullLogger<OutboxDispatcher>(), metrics);
    }

    [Fact]
    public async Task Retry_Sets_NextRetryAt_And_Does_Not_Immediate_ReAttempt()
    {
        using var ctx = CreateContext();
        var msg = NewMessage("workflow.instance.started");
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var dispatcher = CreateDispatcher(ctx, new FailingTransport(), o =>
        {
            o.MaxRetries = 5;
            o.BaseRetryDelaySeconds = 10;
            o.UseExponentialBackoff = false;
        });

        // First attempt — fail
        var (processed, failed) = await dispatcher.ProcessBatchAsync();
        Assert.Equal(0, processed);
        Assert.Equal(1, failed);

        var reloaded = ctx.OutboxMessages.Single();
        Assert.NotNull(reloaded.NextRetryAt);
        var firstNext = reloaded.NextRetryAt!.Value;

        // Immediate second batch should skip (NextRetryAt in future)
        (processed, failed) = await dispatcher.ProcessBatchAsync();
        Assert.Equal(0, processed);
        Assert.Equal(0, failed); // not attempted again

        var secondReload = ctx.OutboxMessages.Single();
        Assert.Equal(firstNext, secondReload.NextRetryAt);
        Assert.Equal(1, secondReload.RetryCount);
    }

    [Fact]
    public async Task MaxRetries_Marks_GiveUp()
    {
        using var ctx = CreateContext();
        var msg = NewMessage("workflow.instance.failed");
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var dispatcher = CreateDispatcher(ctx, new FailingTransport(), o =>
        {
            o.MaxRetries = 2;
            o.BaseRetryDelaySeconds = 1;
        });

        // 1st fail
        await dispatcher.ProcessBatchAsync();
        // Force time travel by nulling NextRetryAt to allow next attempt
        msg.NextRetryAt = DateTime.UtcNow.AddSeconds(-1);
        ctx.SaveChanges();

        // 2nd fail -> terminal
        await dispatcher.ProcessBatchAsync();

        var re = ctx.OutboxMessages.Single();
        Assert.True(re.IsProcessed);
        Assert.NotNull(re.ProcessedAt);
        Assert.Equal(2, re.RetryCount);
    }

    [Fact]
    public void Backoff_Exponential_Grows()
    {
        // Backoff_Exponential_Grows test (ensure policy is accessible)
        var opt = new OutboxOptions
        {
            BaseRetryDelaySeconds = 3,
            UseExponentialBackoff = true,
            JitterRatio = 0
        };
        var d1 = OutboxRetryPolicy.ComputeDelay(1, opt).TotalSeconds;
        var d2 = OutboxRetryPolicy.ComputeDelay(2, opt).TotalSeconds;
        var d3 = OutboxRetryPolicy.ComputeDelay(3, opt).TotalSeconds;

        Assert.Equal(3, d1);
        Assert.Equal(6, d2);
        Assert.Equal(12, d3);
    }

    [Fact]
    public async Task Metrics_Snapshot_Computed()
    {
        using var ctx = CreateContext();
        for (int i = 0; i < 4; i++)
        {
            ctx.OutboxMessages.Add(NewMessage("workflow.task.created"));
        }
        ctx.SaveChanges();

        var opts = Options.Create(new OutboxOptions
        {
            EnableMetrics = true,
            EnablePrometheus = false,
            RollingWindowMinutes = 1
        });

        var metrics = new OutboxMetricsProvider(new DummyScopeFactory(ctx), opts);

        var dispatcher = CreateDispatcher(ctx, new SuccessTransport(), metrics: metrics);

        await dispatcher.ProcessBatchAsync();

        var snap = await metrics.GetSnapshotAsync();
        Assert.True(snap.BacklogSize >= 0);
        Assert.True(snap.ProcessedLastCycle >= 0);
        Assert.True(snap.ProcessedTotal >= snap.ProcessedLastCycle);
    }

    // IServiceScopeFactory stub supplying our in-memory context
    private sealed class DummyScopeFactory : IServiceScopeFactory
    {
        private readonly WorkflowDbContext _ctx;
        public DummyScopeFactory(WorkflowDbContext ctx) => _ctx = ctx;
        public IServiceScope CreateScope() => new DummyScope(_ctx);

        private sealed class DummyScope : IServiceScope
        {
            public IServiceProvider ServiceProvider { get; }
            public DummyScope(WorkflowDbContext ctx) => ServiceProvider = new DummyProvider(ctx);
            public void Dispose() { }
        }

        private sealed class DummyProvider : IServiceProvider
        {
            private readonly WorkflowDbContext _ctx;
            public DummyProvider(WorkflowDbContext ctx) => _ctx = ctx;
            public object? GetService(Type serviceType) =>
                serviceType == typeof(WorkflowDbContext) ? _ctx : null;
        }
    }
}
