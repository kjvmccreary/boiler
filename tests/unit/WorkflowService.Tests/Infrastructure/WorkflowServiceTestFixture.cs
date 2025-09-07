using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using WorkflowService.Persistence;
using Xunit;

namespace WorkflowService.Tests.Infrastructure;

public class WorkflowServiceTestFixture : IAsyncLifetime
{
    public TestWebApplicationFactory Factory { get; private set; } = default!;
    public HttpClient Client { get; private set; } = default!;
    public IServiceProvider Services => Factory.Services;

    public HttpClient CreateClient() =>
        Factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    public Task InitializeAsync()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        Environment.SetEnvironmentVariable("ENABLE_TENANT_FILTERS_IN_TESTS", "true");

        Factory = new TestWebApplicationFactory();
        Client = CreateClient();
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Client.Dispose();
        Factory.Dispose();
        return Task.CompletedTask;
    }

    public async Task SeedAsync(Action<WorkflowDbContext> seed)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        seed(db);
        await db.SaveChangesAsync();
    }

    // Legacy call support for tests still invoking token acquisition.
    // Returns a dummy token; real auth occurs via TestAuthHandler.
    public Task<string> GetTenantTokenAsync(string email, string password, int tenantId)
        => Task.FromResult($"dummy-token-{tenantId}");
}

[CollectionDefinition("Workflow Integration")]
public class WorkflowIntegrationCollection : ICollectionFixture<WorkflowServiceTestFixture> { }
