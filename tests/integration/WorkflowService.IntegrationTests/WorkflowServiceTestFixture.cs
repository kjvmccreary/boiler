using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkflowService.Persistence;
using Microsoft.EntityFrameworkCore;

namespace WorkflowService.IntegrationTests;

public class WorkflowServiceTestFixture : IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    public IServiceProvider Services => _factory.Services;

    public WorkflowServiceTestFixture()
    {
        // Enable tenant query filters in tests (our DbContext checks this flag)
        Environment.SetEnvironmentVariable(
            "ENABLE_TENANT_FILTERS_IN_TESTS",
            "true" // Ensures DbContext applies tenant filters so isolation tests reflect prod behavior
        );

        // (Optional) also mark test environment explicitly if your code branches on it
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Optionally ensure fresh InMemory DB per run (if not already)
                    var descriptor = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<WorkflowDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    var dbName = $"WorkflowServiceIntegrationDB_{Guid.NewGuid():N}";
                    services.AddDbContext<WorkflowDbContext>(o =>
                        o.UseInMemoryDatabase(dbName).EnableSensitiveDataLogging());

                    // Seed hook can be added here if needed
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
                    db.Database.EnsureCreated();
                });
            });
    }

    public HttpClient CreateClient() => 
        _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

    // Example token helper (adjust to your auth flow)
    public async Task<string> GetTenantTokenAsync(string email, string password, int tenantId)
    {
        // If you already have a helper elsewhere, just forward to it.
        // Placeholder: return a pre-seeded JWT or call your auth endpoint.
        // For now, assume a seeded static token provider is available.
        var client = CreateClient();
        var loginResp = await client.PostAsJsonAsync("/api/auth/login", new
        {
            Email = email,
            Password = password
        });
        loginResp.EnsureSuccessStatusCode();
        var loginData = await loginResp.Content.ReadFromJsonAsync<LoginPayload>();

        // Phase 2 tenant switch if your API requires it:
        var switchResp = await client.PostAsJsonAsync("/api/auth/switch-tenant", new { TenantId = tenantId });
        switchResp.EnsureSuccessStatusCode();
        var switchData = await switchResp.Content.ReadFromJsonAsync<LoginPayload>();

        return switchData!.Data.AccessToken;
    }

    // Lightweight DTO matching your auth response
    private sealed class LoginPayload
    {
        public bool Success { get; set; }
        public TokenData Data { get; set; } = new();
    }

    private sealed class TokenData
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task WithDbContextAsync(Func<WorkflowDbContext, Task> work)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WorkflowDbContext>();
        await work(db);
    }
}

// Collection definition so multiple test classes reuse this fixture
[CollectionDefinition("WorkflowServiceIntegration")]
public class WorkflowServiceIntegrationCollection : ICollectionFixture<WorkflowServiceTestFixture> { }
