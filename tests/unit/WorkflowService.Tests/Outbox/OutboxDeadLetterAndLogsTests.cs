using System;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkflowService.Persistence;
using WorkflowService.Outbox;
using WorkflowService.Domain.Models;
using Xunit;
using Contracts.Services;
using Microsoft.AspNetCore.Http;

namespace WorkflowService.Tests.Outbox;

public class OutboxDeadLetterAndLogsTests
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
            new LoggerFactory().CreateLogger<WorkflowDbContext>());
    }

    private class AlwaysFailTransport : IOutboxTransport
    {
        public Task DeliverAsync(OutboxMessage message, System.Threading.CancellationToken ct)
            => throw new InvalidOperationException("boom");
    }

    private sealed class TestLogger<T> : ILogger<T>, IDisposable
    {
        public ConcurrentQueue<string> Lines { get; } = new();
        public IDisposable BeginScope<TState>(TState state) => this;
        public void Dispose() { }
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Lines.Enqueue(formatter(state, exception));
        }
    }

    private OutboxDispatcher CreateDispatcher(WorkflowDbContext ctx, TestLogger<OutboxDispatcher> logger, bool deadLetter, int maxRetries = 2)
    {
        var options = Options.Create(new OutboxOptions
        {
            BatchSize = 10,
            BaseRetryDelaySeconds = 1,
            MaxRetries = maxRetries,
            UseExponentialBackoff = false,
            EnableMetrics = false,
            UseDeadLetterOnGiveUp = deadLetter
        });

        return new OutboxDispatcher(
            ctx,
            new AlwaysFailTransport(),
            options,
            logger);
    }

    private OutboxMessage NewMessage() => new()
    {
        TenantId = 1,
        EventType = "workflow.instance.test",
        EventData = "{}",
        IdempotencyKey = Guid.NewGuid(),
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow
    };

    [Fact]
    public async Task DeadLetter_Path_Sets_Flag_And_Logs()
    {
        using var ctx = CreateContext();
        var msg = NewMessage();
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var logger = new TestLogger<OutboxDispatcher>();
        var dispatcher = CreateDispatcher(ctx, logger, deadLetter: true, maxRetries: 1);

        await dispatcher.ProcessBatchAsync();

        var re = ctx.OutboxMessages.Single();
        Assert.True(re.DeadLetter);
        Assert.True(re.IsProcessed);
        Assert.NotNull(re.ProcessedAt);

        var joined = string.Join(Environment.NewLine, logger.Lines);
        Assert.Contains("OUTBOX_WORKER_DEADLETTER", joined);
    }

    [Fact]
    public async Task Log_Flow_Contains_Fail_Then_GiveUp_When_Not_DeadLetter_Mode()
    {
        using var ctx = CreateContext();
        var msg = NewMessage();
        ctx.OutboxMessages.Add(msg);
        ctx.SaveChanges();

        var logger = new TestLogger<OutboxDispatcher>();
        var dispatcher = CreateDispatcher(ctx, logger, deadLetter: false, maxRetries: 1);

        await dispatcher.ProcessBatchAsync();

        var text = string.Join(Environment.NewLine, logger.Lines);
        Assert.Contains("OUTBOX_WORKER_GIVEUP", text);
        Assert.DoesNotContain("OUTBOX_WORKER_DEADLETTER", text);
    }
}
