using System.Net;
using System.Text;
using FluentAssertions;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using WorkflowService.Tests.Infrastructure; // Added: resolves WorkflowServiceTestFixture

namespace WorkflowService.Tests.Isolation;

// Skipped until supporting helpers (TenantTestClient, MultiTenantWorkflowSeeder, auth flows) are ported.
[Collection("Workflow Integration")]
public class AdminIsolationIntegrationTests : IClassFixture<WorkflowServiceTestFixture>
{
    private readonly WorkflowServiceTestFixture _fx;
    private readonly IServiceScopeFactory _scopeFactory;

    public AdminIsolationIntegrationTests(WorkflowServiceTestFixture fx)
    {
        _fx = fx;
        _scopeFactory = fx.Services.GetRequiredService<IServiceScopeFactory>();
    }

    private HttpClient BasicClient() => _fx.CreateClient();

    [Fact(Skip = "Pending port of MultiTenantWorkflowSeeder & auth/TenantTestClient helpers")]
    public async Task T7_BulkCancel_CrossTenant_Definition_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var client = BasicClient();
        var resp = await client.PostAsync("/api/workflow/admin/instances/bulk-cancel", new StringContent("{}", Encoding.UTF8, "application/json"));
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Pending port of MultiTenantWorkflowSeeder & auth/TenantTestClient helpers")]
    public async Task T7_MoveInstance_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var client = BasicClient();
        var resp = await client.PostAsync("/api/workflow/admin/instances/999/move", new StringContent("{\"targetNodeId\":\"end\"}", Encoding.UTF8, "application/json"));
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact(Skip = "Pending port of MultiTenantWorkflowSeeder & auth/TenantTestClient helpers")]
    public async Task T7_RetryInstance_CrossTenant_Should404()
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        var client = BasicClient();
        var resp = await client.PostAsync("/api/workflow/admin/instances/999/retry", new StringContent("{\"retryReason\":\"x\"}", Encoding.UTF8, "application/json"));
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
