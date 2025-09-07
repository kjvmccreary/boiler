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
    private const string SkipAll = "Temporarily skipped (tenant isolation integration harness under repair)";
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public DefinitionIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
        Environment.SetEnvironmentVariable("ENABLE_TENANT_FILTERS_IN_TESTS", "true");
    }

    private TenantTestClient CreateClient() => new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T2_DefinitionList_ShouldExclude_OtherTenant() { }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T3_Instance_Retrieve_OtherTenant_Should404() { }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T4_StartInstance_WithForeignDefinition_Should404() { }
}
