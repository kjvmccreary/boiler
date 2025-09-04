using System.Net;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using WorkflowService.IntegrationTests.Infrastructure;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class UsageIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public UsageIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private TenantTestClient C() => new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact]
    public async Task T9_DefinitionUsage_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync($"/api/workflow/definitions/{d2.Id}/usage");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task T8_AfterForceTerminate_Tenant1_CannotSee_Tenant2_Instances()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var inst2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        // Force terminate via direct mutation to simulate other tenant unpublish path
        await MultiTenantWorkflowSeeder.ForceCancelInstanceAsync(db, inst2);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync($"/api/workflow/instances/{inst2.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
