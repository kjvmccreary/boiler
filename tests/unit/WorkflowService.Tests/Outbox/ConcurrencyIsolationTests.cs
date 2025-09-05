using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using WorkflowService.Domain.Models;
using WorkflowService.Outbox;
using WorkflowService.Persistence;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;
using Xunit;

namespace WorkflowService.Tests.Outbox;

public class ConcurrencyIsolationTests
{
    private sealed class FixedTenantProvider : Contracts.Services.ITenantProvider
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

    private static WorkflowDbContext CreateSqliteContext(string dbFilePath, int tenantId)
    {
        var connString = new SqliteConnectionStringBuilder
        {
            DataSource = dbFilePath,
            Mode = SqliteOpenMode.ReadWriteCreate,
            Cache = SqliteCacheMode.Default
        }.ToString();

        var options = new DbContextOptionsBuilder<WorkflowDbContext>()
            .UseSqlite(connString)
            .EnableSensitiveDataLogging()
            .Options;

        var ctx = new WorkflowDbContext(
            options,
            new HttpContextAccessorStub(),
            new FixedTenantProvider(tenantId),
            new NullLogger<WorkflowDbContext>());

        // Ensure schema (first caller creates, others are no-ops)
        ctx.Database.EnsureCreated();
        return ctx;
    }

    private static (string dbFilePath, Action cleanup) CreateTempDbFile()
    {
        var path = Path.Combine(Path.GetTempPath(), $"wf_outbox_{Guid.NewGuid():N}.db");
        return (path, () =>
        {
            try { if (File.Exists(path)) File.Delete(path); } catch { }
        });
    }

    [Fact]
    public async Task Concurrency_IdempotentInsert_Race_Produces_SingleRow()
    {
        var (dbPath, cleanup) = CreateTempDbFile();
        try
        {
            const int tenantId = 10;
            var key = DeterministicOutboxKey.DefinitionPublished(tenantId, definitionId: 77, version: 3);
            var eventType = "workflow.definition.published";
            var payload = new { DefinitionId = 77, Version = 3 };

            // Warm up DB (create schema)
            using (var warm = CreateSqliteContext(dbPath, tenantId))
            {
                warm.Database.EnsureCreated();
            }

            // Run N parallel inserts using separate DbContexts to avoid DbContext threading issues.
            const int N = 40;
            var tasks = Enumerable.Range(0, N).Select(async _ =>
            {
                using var ctx = CreateSqliteContext(dbPath, tenantId);
                IOutboxWriter writer = new OutboxWriter(ctx, new NullLogger<OutboxWriter>());
                var result = await writer.TryAddAsync(tenantId, eventType, payload, key);
                return result.AlreadyExisted;
            }).ToArray();

            var results = await Task.WhenAll(tasks);

            // Assert: exactly one row exists; the rest observed idempotent duplicate
            using var verify = CreateSqliteContext(dbPath, tenantId);
            var rows = await verify.OutboxMessages
                .Where(o => o.TenantId == tenantId && o.IdempotencyKey == key)
                .ToListAsync();

            Assert.Single(rows);
            Assert.Contains(results, r => r == false); // at least one insert actually performed
            Assert.True(results.Count(r => r) >= N - 1); // most others were idempotent duplicates
        }
        finally { cleanup(); }
    }

    [Fact]
    public async Task Concurrency_SameKey_DifferentTenants_Produces_TwoRows()
    {
        var (dbPath, cleanup) = CreateTempDbFile();
        try
        {
            var eventType = "workflow.instance.completed";
            var payload = new { InstanceId = 500, Version = 1 };

            // Same semantic event across two tenants -> same deterministic part except tenant.
            var keyT1 = DeterministicOutboxKey.Instance(1, instanceId: 500, phase: "completed", definitionVersion: 1);
            var keyT2 = DeterministicOutboxKey.Instance(2, instanceId: 500, phase: "completed", definitionVersion: 1);

            // Parallel add for tenant 1 and tenant 2
            using var ctx1 = CreateSqliteContext(dbPath, tenantId: 1);
            using var ctx2 = CreateSqliteContext(dbPath, tenantId: 2);
            IOutboxWriter w1 = new OutboxWriter(ctx1, new NullLogger<OutboxWriter>());
            IOutboxWriter w2 = new OutboxWriter(ctx2, new NullLogger<OutboxWriter>());

            var t1 = w1.TryAddAsync(1, eventType, payload, keyT1);
            var t2 = w2.TryAddAsync(2, eventType, payload, keyT2);
            await Task.WhenAll(t1, t2);

            using var verify = CreateSqliteContext(dbPath, tenantId: 1); // any tenant for verification, we query both
            var all = await verify.OutboxMessages
                .Where(o => o.EventType == eventType)
                .ToListAsync();

            Assert.Equal(2, all.Count);
            Assert.Contains(all, m => m.TenantId == 1 && m.IdempotencyKey == keyT1);
            Assert.Contains(all, m => m.TenantId == 2 && m.IdempotencyKey == keyT2);
        }
        finally { cleanup(); }
    }
}
