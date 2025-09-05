using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WorkflowService.Persistence;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using WorkflowService.Domain.Models;
using Xunit;
using Microsoft.AspNetCore.Http;
using Contracts.Services;

namespace WorkflowService.Tests.Outbox;

public class OutboxWriterIdempotentTests
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

    private static string NewDatabasePath()
    {
        var dir = Path.Combine(Path.GetTempPath(), "wf-outbox-tests");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"outbox-{Guid.NewGuid():N}.db");
    }

    private WorkflowDbContext CreateSqliteContext(string dbPath, int tenant = 1)
    {
        // Do NOT pre-create file; SQLite will create it
        var conn = new SqliteConnection($"Data Source={dbPath};Cache=Shared;Mode=ReadWriteCreate");
        conn.Open();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(conn)
            .Options;

        var ctx = new WorkflowDbContext(
            options,
            new HttpContextAccessor(),
            new FixedTenantProvider(tenant),
            new NullLogger<WorkflowDbContext>());

        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static void TryDelete(string path)
    {
        if (string.IsNullOrWhiteSpace(path)) return;
        for (int i = 0; i < 5; i++)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
                return;
            }
            catch (IOException)
            {
                Thread.Sleep(40); // brief backoff
            }
            catch
            {
                return;
            }
        }
    }

    [Fact]
    public async Task TryAddOutbox_Twice_SameKey_SingleRowExists()
    {
        var path = NewDatabasePath();
        try
        {
            await using var ctx = CreateSqliteContext(path);
            IOutboxWriter writer = new OutboxWriter(ctx, new NullLogger<OutboxWriter>());

            var key = Guid.NewGuid();

            var r1 = await writer.TryAddAsync(1, "workflow.definition.published", new { A = 1 }, key);
            var r2 = await writer.TryAddAsync(1, "workflow.definition.published", new { A = 1 }, key);

            Assert.False(r1.AlreadyExisted);
            Assert.True(r2.AlreadyExisted);
            Assert.Equal(r1.Message.Id, r2.Message.Id);

            var all = ctx.OutboxMessages.Where(m => m.TenantId == 1).ToList();
            Assert.Single(all);
            Assert.Equal(key, all[0].IdempotencyKey);
        }
        finally
        {
            TryDelete(path);
        }
    }

    [Fact]
    public async Task TryAddOutbox_Parallel_Inserts_SameKey_SingleRow()
    {
        var path = NewDatabasePath();
        try
        {
            var key = Guid.NewGuid();

            // Parallel contexts, each its own connection
            var tasks = Enumerable.Range(0, 8).Select(async _ =>
            {
                await using var ctx = CreateSqliteContext(path);
                IOutboxWriter writer = new OutboxWriter(ctx, new NullLogger<OutboxWriter>());
                return await writer.TryAddAsync(1, "workflow.instance.completed", new { V = 9 }, key);
            });

            var results = await Task.WhenAll(tasks);

            var createdCount = results.Count(r => !r.AlreadyExisted);
            var duplicateCount = results.Count(r => r.AlreadyExisted);

            Assert.Equal(1, createdCount);
            Assert.Equal(results.Length - 1, duplicateCount);

            await using var finalCtx = CreateSqliteContext(path);
            var rows = finalCtx.OutboxMessages.Where(m => m.TenantId == 1).ToList();
            Assert.Single(rows);
            Assert.Equal(key, rows[0].IdempotencyKey);
        }
        finally
        {
            TryDelete(path);
        }
    }
}
