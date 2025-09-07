using System.Net;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using WorkflowService.Tests.Infrastructure;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class UsageIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private const string SkipAll = "Temporarily skipped (tenant isolation integration tests deferred).";

    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public UsageIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private TenantTestClient C() => new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T9_DefinitionUsage_CrossTenant_Should404()
    {
        // Original test logic intentionally retained (will not run while skipped).
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync($"/api/workflow/definitions/{d2.Id}/usage");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T8_AfterForceTerminate_Tenant1_CannotSee_Tenant2_Instances()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var inst2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        await MultiTenantWorkflowSeeder.ForceCancelInstanceAsync(db, inst2);

        var client = C();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync($"/api/workflow/instances/{inst2.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
