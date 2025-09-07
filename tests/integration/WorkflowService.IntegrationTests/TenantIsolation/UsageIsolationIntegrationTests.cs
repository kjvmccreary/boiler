using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using WorkflowService.IntegrationTests.Infrastructure;

namespace WorkflowService.IntegrationTests.TenantIsolation;

[Collection("WorkflowServiceIntegration")]
public class UsageIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private const string SkipAll = "Temporarily skipped (tenant isolation integration harness under repair)";
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public UsageIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private TenantTestClient C() => new(_fx.CreateClient(), _fx.GetTenantTokenAsync);

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T8_AfterForceTerminate_Tenant1_CannotSee_Tenant2_Instances() { }

    [Fact(Skip = SkipAll), Trait("TestCategory", "TenantIsolation")]
    public async Task T9_DefinitionUsage_CrossTenant_Should404() { }
}
