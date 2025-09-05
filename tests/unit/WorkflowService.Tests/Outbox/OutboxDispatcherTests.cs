using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WorkflowService.Domain.Models;
using WorkflowService.Outbox;
using WorkflowService.Persistence;
using Xunit;
using Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.Outbox;

public class OutboxDispatcherTests
{
    private class FixedTenantProvider : ITenantProvider
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

    private static OutboxMessage NewMessage(int tenantId, string type, bool processed = false)
        => new()
        {
            TenantId = tenantId,
            EventType = type,
            EventData = "{}",
            IdempotencyKey = Guid.NewGuid(),
            IsProcessed = processed,
            ProcessedAt = processed ? DateTime.UtcNow.AddMinutes(-1) : null,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow.AddMinutes(-10),
            UpdatedAt = DateTime.UtcNow.AddMinutes(-10)
        };

    private IOutboxDispatcher CreateDispatcher(WorkflowDbContext ctx, IOutboxTransport transport, Action<OutboxOptions>? configure = null)
    {
        var options = new OutboxOptions();
        configure?.Invoke(options);
        return new OutboxDispatcher(
            ctx,
            transport,
            Options.Create(options),
            new NullLogger<OutboxDispatcher>());
    }

    private class TestTransport : IOutboxTransport
    {
        private readonly bool _fail;
        public int Delivered { get; private set; }
        public TestTransport(bool fail) => _fail = fail;

        public Task DeliverAsync(OutboxMessage message, CancellationToken ct)
        {
            if (_fail) throw new InvalidOperationException("boom");
            Delivered++;
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task Dispatcher_Sets_ProcessedAt_OnSuccess()
    {
        using var ctx = CreateContext();
        var msg = NewMessage(1, "workflow.instance.started");
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var transport = new TestTransport(fail: false);
        var dispatcher = CreateDispatcher(ctx, transport);

        var (processed, failed) = await dispatcher.ProcessBatchAsync();

        processed.Should().Be(1);
        failed.Should().Be(0);

        var reloaded = ctx.OutboxMessages.Single();
        reloaded.ProcessedAt.Should().NotBeNull();
        reloaded.Error.Should().BeNull();
        reloaded.IsProcessed.Should().BeTrue();
    }

    [Fact]
    public async Task Dispatcher_Sets_Error_And_Retry_OnFailure()
    {
        using var ctx = CreateContext();
        var msg = NewMessage(1, "workflow.instance.completed");
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var transport = new TestTransport(fail: true);
        var dispatcher = CreateDispatcher(ctx, transport, o => o.MaxRetries = 5);

        var (processed, failed) = await dispatcher.ProcessBatchAsync();

        processed.Should().Be(0);
        failed.Should().Be(1);

        var reloaded = ctx.OutboxMessages.Single();
        reloaded.ProcessedAt.Should().BeNull();
        reloaded.RetryCount.Should().Be(1);
        reloaded.Error.Should().NotBeNullOrEmpty();
        reloaded.IsProcessed.Should().BeFalse();
    }

    [Fact]
    public async Task Dispatcher_Skips_AlreadyProcessed()
    {
        using var ctx = CreateContext();
        var processedMsg = NewMessage(1, "workflow.definition.published", processed: true);
        ctx.OutboxMessages.Add(processedMsg);
        ctx.SaveChanges();

        var transport = new TestTransport(fail: false);
        var dispatcher = CreateDispatcher(ctx, transport);

        var (processed, failed) = await dispatcher.ProcessBatchAsync();

        processed.Should().Be(0);
        failed.Should().Be(0);
        transport.Delivered.Should().Be(0);
    }
}
