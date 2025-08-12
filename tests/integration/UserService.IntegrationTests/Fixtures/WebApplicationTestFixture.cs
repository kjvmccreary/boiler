using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Common.Data;

namespace UserService.IntegrationTests.Fixtures;

public class WebApplicationTestFixture : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    
    public WebApplicationTestFixture()
    {
        // ✅ FIX: Create unique database per test run to prevent data mutation
        _databaseName = $"IntegrationTestDb_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}";
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove existing database services
            var descriptors = services.Where(d => 
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ImplementationType?.Name.Contains("ApplicationDbContext") == true ||
                d.ServiceType.Name.Contains("DbContext")).ToList();
            
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // ✅ FIX: Use completely unique database per fixture
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
                
                options.ConfigureWarnings(warnings => 
                {
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning);
                });
            });
        });

        builder.UseEnvironment("Testing");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            try
            {
                using var scope = Services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                context.Database.EnsureDeleted();
            }
            catch
            {
                // Ignore cleanup errors during disposal
            }
        }
        base.Dispose(disposing);
    }
}
