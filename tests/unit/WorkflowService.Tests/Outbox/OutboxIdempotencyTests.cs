using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using WorkflowService.Persistence;
using WorkflowService.Domain.Models;
using Microsoft.AspNetCore.Http;
using Contracts.Services;
using WorkflowService.Utilities;
using WorkflowService.Services;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Tests.Outbox
{
    public class OutboxIdempotencyTests
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

        private WorkflowDbContext CreateContext(int tenantId)
        {
            var options = new DbContextOptionsBuilder<WorkflowDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new WorkflowDbContext(
                options,
                new HttpContextAccessor(),
                new FixedTenantProvider(tenantId),
                new NullLogger<WorkflowDbContext>());
        }

        [Fact]
        public void DeterministicKey_SameInputs_SameGuid()
        {
            var g1 = OutboxIdempotency.CreateForWorkflow(1, "instance", 123, "started", 2);
            var g2 = OutboxIdempotency.CreateForWorkflow(1, "instance", 123, "started", 2);
            Assert.Equal(g1, g2);
        }

        [Fact]
        public void DeterministicKey_DifferentInputs_DifferentGuid()
        {
            var g1 = OutboxIdempotency.CreateForWorkflow(1, "instance", 123, "started", 2);
            var g2 = OutboxIdempotency.CreateForWorkflow(1, "instance", 123, "completed", 2);
            Assert.NotEqual(g1, g2);
        }

        [Fact]
        public async Task PublishInstanceStarted_AssignsDeterministicKey()
        {
            using var ctx = CreateContext(tenantId: 5);

            var runtimeInstance = new WorkflowInstance
            {
                Id = 42,
                TenantId = 5,
                WorkflowDefinitionId = 7,
                DefinitionVersion = 3,
                Status = 0,
                StartedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                CurrentNodeIds = "[]",
                Context = "{}"
            };

            ctx.WorkflowInstances.Add(runtimeInstance);
            await ctx.SaveChangesAsync();

            IOutboxWriter outboxWriter = new OutboxWriter(ctx, new NullLogger<OutboxWriter>());
            var publisher = new EventPublisher(
                ctx,
                new NullLogger<EventPublisher>(),
                outboxWriter);

            await publisher.PublishInstanceStartedAsync(runtimeInstance);

            var outbox = ctx.OutboxMessages.Single(o => o.EventType == "workflow.instance.started");
            var expected = OutboxIdempotency.CreateForWorkflow(
                runtimeInstance.TenantId,
                "instance",
                runtimeInstance.Id,
                "started",
                runtimeInstance.DefinitionVersion);

            Assert.Equal(expected, outbox.IdempotencyKey);
            Assert.NotEqual(Guid.Empty, outbox.IdempotencyKey);
        }
    }
}
