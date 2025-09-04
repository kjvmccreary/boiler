using System.Net;
using System.Text.Json;
using Xunit;
using WorkflowService.Persistence;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.IntegrationTests.Infrastructure;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class DefinitionIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public DefinitionIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
        Environment.SetEnvironmentVariable("ENABLE_TENANT_FILTERS_IN_TESTS", "true");
    }

    private TenantTestClient CreateClient() =>
        new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact]
    public async Task T2_DefinitionList_ShouldExclude_OtherTenant()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (def1, def2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = CreateClient();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync("/api/workflow/definitions?pageSize=50");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var items = payload.RootElement.GetProperty("data").GetProperty("items");
        items.EnumerateArray().Should().OnlyContain(e =>
            e.GetProperty("name").GetString()!.Contains("-T1"));
    }

    [Fact]
    public async Task T3_Instance_Retrieve_OtherTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (d1, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var inst2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        var client = CreateClient();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var resp = await client.GetAsync($"/api/workflow/instances/{inst2.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task T4_StartInstance_WithForeignDefinition_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = CreateClient();
        await client.AuthorizeAsync("admin@tenant1.com", "password123", 1);

        var startResp = await client.PostJsonAsync("/api/workflow/instances",
            new { workflowDefinitionId = d2.Id });

        startResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
