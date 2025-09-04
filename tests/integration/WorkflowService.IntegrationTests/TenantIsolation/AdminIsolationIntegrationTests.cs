using System.Net;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using WorkflowService.IntegrationTests.Infrastructure;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class AdminIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public AdminIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private TenantTestClient C() => new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact]
    public async Task T7_BulkCancel_CrossTenant_Definition_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        // Adjust route if different in your controller
        var resp = await client.PostJsonAsync("/api/workflow/admin/instances/bulk-cancel",
            new { workflowDefinitionId = d2.Id, reason = "cross tenant test" });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task T7_MoveInstance_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var inst2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.PostJsonAsync($"/api/workflow/admin/instances/{inst2.Id}/move",
            new { targetNodeId = "end", reason = "test" });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task T7_RetryInstance_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var inst2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        // Simulate failed state (if required)
        inst2.Status = DTOs.Workflow.Enums.InstanceStatus.Failed;
        inst2.UpdatedAt = DateTime.UtcNow;
        await db.SaveChangesAsync();

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.PostJsonAsync($"/api/workflow/admin/instances/{inst2.Id}/retry",
            new { retryReason = "x" });

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
