using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using WorkflowService.Outbox;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using Xunit;
using Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.Outbox;

public class OutboxMaxRetryTests
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

    private static OutboxMessage NewMessage(string type) => new()
    {
        TenantId = 1,
        EventType = type,
        EventData = "{}",
        IdempotencyKey = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    private class AlwaysFailTransport : IOutboxTransport
    {
        public Task DeliverAsync(OutboxMessage message, System.Threading.CancellationToken ct)
            => throw new InvalidOperationException("boom");
    }

    private OutboxDispatcher CreateDispatcher(WorkflowDbContext ctx, int maxRetries)
    {
        var options = Options.Create(new OutboxOptions
        {
            BatchSize = 10,
            BaseRetryDelaySeconds = 1,
            MaxRetries = maxRetries,
            UseExponentialBackoff = false,
            EnableMetrics = false
        });

        return new OutboxDispatcher(
            ctx,
            new AlwaysFailTransport(),
            options,
            new NullLogger<OutboxDispatcher>());
    }

    [Fact]
    public async Task Dispatcher_Retry_Until_MaxRetries_Reached()
    {
        using var ctx = CreateContext();
        var msg = NewMessage("workflow.instance.completed");
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        int max = 3;
        var dispatcher = CreateDispatcher(ctx, max);

        // Perform attempts until terminal
        for (int attempt = 1; attempt <= max; attempt++)
        {
            // Adjust NextRetryAt so dispatcher picks it up
            msg.NextRetryAt = DateTime.UtcNow.AddSeconds(-1);
            ctx.SaveChanges();

            await dispatcher.ProcessBatchAsync();
            var re = ctx.OutboxMessages.Single();

            Assert.Equal(attempt, re.RetryCount);
            if (attempt < max)
            {
                Assert.False(re.IsProcessed);
                Assert.Null(re.ProcessedAt);
                Assert.NotNull(re.NextRetryAt);
            }
            else
            {
                // terminal
                Assert.True(re.IsProcessed);
                Assert.NotNull(re.ProcessedAt);
                Assert.Null(re.NextRetryAt);
            }
        }
    }

    [Fact]
    public async Task Dispatcher_NoRetry_After_MaxRetries()
    {
        using var ctx = CreateContext();
        var msg = NewMessage("workflow.instance.failed");
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var dispatcher = CreateDispatcher(ctx, maxRetries: 2);

        // First failure
        await dispatcher.ProcessBatchAsync();
        msg.NextRetryAt = DateTime.UtcNow.AddSeconds(-1);
        ctx.SaveChanges();

        // Second (terminal) failure
        await dispatcher.ProcessBatchAsync();

        var terminal = ctx.OutboxMessages.Single();
        Assert.True(terminal.IsProcessed);
        Assert.NotNull(terminal.ProcessedAt);
        var processedAt = terminal.ProcessedAt!.Value;
        var retryCount = terminal.RetryCount;

        // Force NextRetryAt artificially earlier; dispatcher must still skip due to ProcessedAt
        terminal.NextRetryAt = DateTime.UtcNow.AddSeconds(-5);
        ctx.SaveChanges();

        await dispatcher.ProcessBatchAsync();

        var after = ctx.OutboxMessages.Single();
        Assert.Equal(retryCount, after.RetryCount);
        Assert.Equal(processedAt, after.ProcessedAt); // unchanged
    }
}
