using System;
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

public class OutboxHealthAndPrometheusTests
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
    public async Task PrometheusFormatter_Contains_CoreMetrics()
    {
        using var ctx = CreateContext();
        ctx.OutboxMessages.Add(new OutboxMessage
        {
            TenantId = 1,
            EventType = "workflow.instance.started",
            EventData = "{}",
            IdempotencyKey = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow.AddMinutes(-2),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-2)
        });
        ctx.SaveChanges();

        var opts = Options.Create(new OutboxOptions { EnableMetrics = true, EnablePrometheus = true });
        var provider = new OutboxMetricsProvider(new DummyScopeFactory(ctx), opts);
        provider.RecordCycle(1, 0, 1, 0);

        var snap = await provider.GetSnapshotAsync();
        var text = PrometheusFormatter.Format(snap);

        Assert.Contains("outbox_backlog_size", text);
        Assert.Contains("outbox_messages_failed_total", text);
        Assert.Contains("outbox_failure_ratio_window", text);
    }

    [Fact]
    public async Task HealthCheck_Backlog_Unhealthy()
    {
        using var ctx = CreateContext();
        for (int i = 0; i < 10; i++)
        {
            ctx.OutboxMessages.Add(new OutboxMessage
            {
                TenantId = 1,
                EventType = "workflow.task.created",
                EventData = "{}",
                IdempotencyKey = Guid.NewGuid(),
                CreatedAt = DateTime.UtcNow.AddMinutes(-30),
                UpdatedAt = DateTime.UtcNow.AddMinutes(-30),
                RetryCount = i % 2
            });
        }
        ctx.SaveChanges();

        var opts = Options.Create(new OutboxOptions
        {
            Health = new OutboxHealthThresholds
            {
                BacklogWarn = 5,
                BacklogUnhealthy = 8,
                FailedPendingRatioWarn = 0.1,
                FailedPendingRatioUnhealthy = 0.2,
                OldestAgeWarnSeconds = 60,
                OldestAgeUnhealthySeconds = 120
            },
            EnableMetrics = true
        });

        var metrics = new OutboxMetricsProvider(new DummyScopeFactory(ctx), opts);
        metrics.RecordCycle(10, 0, 2, 0);

        var hc = new OutboxHealthCheck(metrics, opts);
        var result = await hc.CheckHealthAsync(new Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckContext());

        Assert.Equal(Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Unhealthy, result.Status);
    }

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
