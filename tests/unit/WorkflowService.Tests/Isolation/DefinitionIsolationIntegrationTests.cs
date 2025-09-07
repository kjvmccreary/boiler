using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using WorkflowService.Tests.Infrastructure;
using WorkflowService.Persistence;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class DefinitionIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private const string SkipAll = "Temporarily skipped (tenant isolation integration tests deferred).";

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

    private async Task<TenantTestClient> CreateAuthedTenant1Async()
    {
        var c = CreateClient();
        await c.AuthorizeAsync("admin@tenant1.com", "password123", 1);
        return c;
    }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task Get_Definitions_Default_ExcludesArchived()
    {
        await _fx.SeedAsync(db =>
        {
            db.WorkflowDefinitions.AddRange(
                new WorkflowService.Domain.Models.WorkflowDefinition
                {
                    TenantId = 1,
                    Name = "Active",
                    Version = 1,
                    JSONDefinition = "{}",
                    IsPublished = false
                },
                new WorkflowService.Domain.Models.WorkflowDefinition
                {
                    TenantId = 1,
                    Name = "Archived",
                    Version = 1,
                    JSONDefinition = "{}",
                    IsPublished = false,
                    IsArchived = true
                }
            );
        });

        var client = await CreateAuthedTenant1Async();
        var resp = await client.GetAsync("/api/workflow/definitions");
        resp.EnsureSuccessStatusCode();
        var envelope = await resp.Content.ReadFromJsonAsync<DTOs.Common
            .ApiResponseDto<System.Collections.Generic.List<DTOs.Workflow.WorkflowDefinitionDto>>>();

        envelope!.Success.Should().BeTrue();
        envelope.Data.Should().Contain(d => d.Name == "Active");
        envelope.Data.Should().NotContain(d => d.Name == "Archived");
    }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T2_DefinitionList_ShouldExclude_OtherTenant()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (def1, def2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = await CreateAuthedTenant1Async();

        var resp = await client.GetAsync("/api/workflow/definitions?pageSize=50");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        var items = payload.RootElement.GetProperty("data").GetProperty("items");
        items.EnumerateArray().Should().OnlyContain(e =>
            e.GetProperty("name").GetString()!.Contains("-T1"));
    }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T3_Instance_Retrieve_OtherTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (d1, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);
        var inst2 = await MultiTenantWorkflowSeeder.SeedInstanceAsync(db, d2, 2);

        var client = await CreateAuthedTenant1Async();
        var resp = await client.GetAsync($"/api/workflow/instances/{inst2.Id}");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T4_StartInstance_WithForeignDefinition_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var (_, d2) = await MultiTenantWorkflowSeeder.SeedDefinitionsAsync(db, 1, 2);

        var client = await CreateAuthedTenant1Async();

        var startResp = await client.PostJsonAsync("/api/workflow/instances",
            new { workflowDefinitionId = d2.Id });

        startResp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
