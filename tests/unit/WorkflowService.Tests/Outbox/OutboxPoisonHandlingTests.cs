using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using WorkflowService.Outbox;
using Xunit;
using Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.Outbox;

public class OutboxPoisonHandlingTests
{
    private class FixedTenantProvider : ITenantProvider
    {
        public Task<int?> GetCurrentTenantIdAsync() => Task.FromResult<int?>(1);
        public Task<string?> GetCurrentTenantIdentifierAsync() => Task.FromResult<string?>("1");
        public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
        public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
        public Task ClearCurrentTenantAsync() => Task.CompletedTask;
        public bool HasTenantContext => true;
    }

    private WorkflowDbContext CreateCtx()
    {
        var opts = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new WorkflowDbContext(
            opts,
            new HttpContextAccessor(),
            new FixedTenantProvider(),
            new NullLogger<WorkflowDbContext>());
    }

    private class FailingTransport : IOutboxTransport
    {
        private readonly string _msg;
        public FailingTransport(string msg) => _msg = msg;
        public Task DeliverAsync(OutboxMessage message, System.Threading.CancellationToken ct)
            => throw new InvalidOperationException(_msg);
    }

    private OutboxDispatcher Dispatcher(WorkflowDbContext ctx, string error, Action<OutboxOptions>? cfg = null)
    {
        var options = new OutboxOptions
        {
            BatchSize = 10,
            MaxRetries = 5,
            UseDeadLetterOnGiveUp = true,
            Poison = new OutboxPoisonOptions
            {
                EarlyDeadLetterRetries = 0,
                NonTransientErrorMarkers = new[] { "validation", "schema" },
                AlwaysTransientMarkers = new[] { "timeout" },
                DeadLetterOnMaxRetries = true,
                StrictErrorTruncation = true
            }
        };
        cfg?.Invoke(options);
        return new OutboxDispatcher(
            ctx,
            new FailingTransport(error),
            Options.Create(options),
            new NullLogger<OutboxDispatcher>());
    }

    private OutboxMessage NewMsg(string et) => new()
    {
        TenantId = 1,
        EventType = et,
        EventData = "{}",
        IdempotencyKey = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task NonTransientMarker_Immediate_DeadLetter()
    {
        using var ctx = CreateCtx();
        ctx.OutboxMessages.Add(NewMsg("evt.a"));
        ctx.SaveChanges();

        var d = Dispatcher(ctx, "validation failed because X");
        await d.ProcessBatchAsync();

        var row = ctx.OutboxMessages.Single();
        Assert.True(row.DeadLetter);
        Assert.NotNull(row.ProcessedAt);
        Assert.Equal(1, row.RetryCount); // counted once
    }

    [Fact]
    public async Task AlwaysTransient_Ignores_EarlyDeadLetter()
    {
        using var ctx = CreateCtx();
        ctx.OutboxMessages.Add(NewMsg("evt.b"));
        ctx.SaveChanges();

        var d = Dispatcher(ctx, "timeout contacting service", o =>
        {
            o.Poison.EarlyDeadLetterRetries = 1;
        });

        // First attempt -> transient
        await d.ProcessBatchAsync();
        var row = ctx.OutboxMessages.Single();
        Assert.False(row.DeadLetter);
        Assert.Null(row.ProcessedAt);
        Assert.Equal(1, row.RetryCount);
        Assert.NotNull(row.NextRetryAt);
    }

    [Fact]
    public async Task EarlyDeadLetterThreshold_Applied()
    {
        using var ctx = CreateCtx();
        ctx.OutboxMessages.Add(NewMsg("evt.c"));
        ctx.SaveChanges();

        var d = Dispatcher(ctx, "random unexpected serialization blob", o =>
        {
            o.Poison.EarlyDeadLetterRetries = 2;
            o.MaxRetries = 10;
        });

        // attempt 1 - fail transient
        await d.ProcessBatchAsync();
        var row1 = ctx.OutboxMessages.Single();
        row1.NextRetryAt = DateTime.UtcNow.AddSeconds(-1);
        ctx.SaveChanges();

        // attempt 2 - should early dead-letter
        await d.ProcessBatchAsync();
        var final = ctx.OutboxMessages.Single();
        Assert.True(final.DeadLetter);
        Assert.NotNull(final.ProcessedAt);
        Assert.Equal(2, final.RetryCount);
    }

    [Fact]
    public async Task Error_Truncated_When_TooLong()
    {
        using var ctx = CreateCtx();
        ctx.OutboxMessages.Add(NewMsg("evt.long"));
        ctx.SaveChanges();

        var longErr = new string('X', 5000);
        var d = Dispatcher(ctx, longErr, o =>
        {
            o.MaxErrorTextLength = 1000;
            o.Poison.StrictErrorTruncation = true;
        });

        await d.ProcessBatchAsync();
        var row = ctx.OutboxMessages.Single();
        Assert.True(row.Error!.Length <= 1000);
    }
}
