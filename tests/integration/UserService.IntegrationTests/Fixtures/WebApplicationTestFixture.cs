using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Common.Data;
using Contracts.Services;
using Common.Services;
using Microsoft.AspNetCore.Http;

namespace UserService.IntegrationTests.Fixtures;

public class WebApplicationTestFixture : WebApplicationFactory<Program>
{
    // ✅ FIXED: Create a shared database name for all tests
    private static readonly string SharedDatabaseName = $"TestDb_{Guid.NewGuid()}";
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // FIXED: Add configuration that matches UserService appsettings.json
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new[]
            {
                // JWT Settings - Must match UserService appsettings.json exactly
                new KeyValuePair<string, string?>("JwtSettings:SecretKey", "your-super-secret-jwt-key-that-is-at-least-256-bits-long"),
                new KeyValuePair<string, string?>("JwtSettings:Issuer", "AuthService"),
                new KeyValuePair<string, string?>("JwtSettings:Audience", "StarterApp"),
                new KeyValuePair<string, string?>("JwtSettings:ExpiryMinutes", "60"),
                new KeyValuePair<string, string?>("JwtSettings:RefreshTokenExpiryDays", "7"),
                new KeyValuePair<string, string?>("JwtSettings:ValidateIssuer", "true"),
                new KeyValuePair<string, string?>("JwtSettings:ValidateAudience", "true"),
                new KeyValuePair<string, string?>("JwtSettings:ValidateLifetime", "true"),
                new KeyValuePair<string, string?>("JwtSettings:ValidateIssuerSigningKey", "true"),
                
                // Tenant Settings
                new KeyValuePair<string, string?>("TenantSettings:DefaultTenantId", "1"),
                new KeyValuePair<string, string?>("TenantSettings:ResolutionStrategy", "Domain"),
                new KeyValuePair<string, string?>("TenantSettings:EnableRowLevelSecurity", "true"),
                new KeyValuePair<string, string?>("TenantSettings:AllowCrossTenantQueries", "false"),
                new KeyValuePair<string, string?>("TenantSettings:TenantHeaderName", "X-Tenant-ID")
            });
        });

        builder.ConfigureServices(services =>
        {
            // STEP 1: Remove ALL Entity Framework Core registrations
            services.RemoveAll(typeof(ApplicationDbContext));
            services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            
            // Remove any EF Core internal services that might cause conflicts
            var efCoreServices = services.Where(s => 
                s.ServiceType.FullName?.Contains("EntityFrameworkCore") == true ||
                s.ServiceType.FullName?.Contains("Npgsql") == true)
                .ToList();
            
            foreach (var service in efCoreServices)
            {
                services.Remove(service);
            }

            // ✅ FIXED: Use the SAME database name for all contexts
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(SharedDatabaseName)
                       .EnableSensitiveDataLogging()
                       .EnableDetailedErrors();
            }, ServiceLifetime.Scoped);

            // STEP 3: Ensure supporting services are available
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddScoped<ITenantProvider, TenantProvider>();

            // STEP 4: Configure logging for tests
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
                builder.SetMinimumLevel(LogLevel.Warning);
            });
        });

        // Use Testing environment
        builder.UseEnvironment("Testing");
    }
}
