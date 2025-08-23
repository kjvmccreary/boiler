using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Common.Data;
using Contracts.Services;
using Common.Configuration;
using UserService.PerformanceTests.Utilities;
using Xunit;
using Common.Caching; // 🔧 ADD: For MemoryCacheService, MemoryPermissionCache, MemoryMetricsCollector
using UserService.Services; // 🔧 ADD: For NoOpCacheInvalidationService and ICacheInvalidationService
using Common.Monitoring; // 🔧 ADD: For IMetricsCollector

namespace UserService.PerformanceTests.Fixtures;

public class PerformanceTestFixture : WebApplicationFactory<Program>, IAsyncLifetime
{
    private static readonly string DatabaseName = $"PerformanceTestDb_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
    
    public PerformanceTestFixture()
    {
        // Use static database name to ensure all contexts use the same database
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Performance");

        builder.ConfigureServices(services =>
        {
            // Remove existing database services first
            var descriptors = services.Where(d => 
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ImplementationType?.Name.Contains("ApplicationDbContext") == true ||
                d.ServiceType.Name.Contains("DbContext")).ToList();
            
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // ⚡ CRITICAL: Remove ALL Redis-related services to ensure clean override
            services.RemoveAll<IDistributedCache>();
            services.RemoveAll<StackExchange.Redis.IConnectionMultiplexer>();
            services.RemoveAll<StackExchange.Redis.IDatabase>();
            services.RemoveAll<ICacheService>();
            services.RemoveAll<IPermissionCache>();
            services.RemoveAll<ICacheInvalidationService>();
            services.RemoveAll<IMetricsCollector>();
            
            // Add in-memory replacements
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddSingleton<IPermissionCache, MemoryPermissionCache>();
            services.AddScoped<ICacheInvalidationService, NoOpCacheInvalidationService>();
            services.AddSingleton<IMetricsCollector, MemoryMetricsCollector>();

            // CRITICAL: Use the same static database name for ALL contexts
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(DatabaseName); // Use static name
                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
                options.EnableServiceProviderCaching(true); // Enable for shared database
                
                options.ConfigureWarnings(warnings => 
                {
                    warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning);
                });
            });

            // Mock IHttpContextAccessor 
            services.RemoveAll<IHttpContextAccessor>();
            services.AddSingleton<IHttpContextAccessor>(provider =>
            {
                var accessor = new HttpContextAccessor();
                return accessor;
            });
            
            // Use simple tenant provider for performance tests
            services.RemoveAll<ITenantProvider>();
            services.AddScoped<ITenantProvider, SimplePerformanceTenantProvider>();

            // Configure tenant settings for performance tests
            services.RemoveAll<TenantSettings>();
            services.AddSingleton<TenantSettings>(new TenantSettings 
            {
                RequireTenantContext = true, // Keep validation but make it work
                ResolutionStrategy = TenantResolutionStrategy.Header,
                DefaultTenantId = "1",
                TenantHeaderName = "X-Tenant-ID",
                EnableRowLevelSecurity = false,
                AllowCrossTenantQueries = true
            });

            // Reduce logging
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Warning);
                builder.AddConsole();
            });
        });
    }

    public async Task InitializeAsync()
    {
        // CRITICAL: Seed data and ensure it persists for all requests
        using var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PerformanceTestFixture>>();
        
        // Ensure database is created
        await context.Database.EnsureCreatedAsync();
        
        // Seed data
        await PerformanceDataSeeder.SeedPerformanceDataAsync(context, logger);
        
        // CRITICAL: Verify the data is actually there
        var tenantCount = await context.Tenants.CountAsync();
        var userCount = await context.Users.CountAsync();
        
        logger.LogWarning("🔍 FIXTURE VERIFICATION: Created {TenantCount} tenants, {UserCount} users in database '{DatabaseName}'", 
            tenantCount, userCount, DatabaseName);
            
        if (tenantCount == 0 || userCount == 0)
        {
            throw new InvalidOperationException($"Data seeding failed! Tenants: {tenantCount}, Users: {userCount}");
        }
    }

    // 🔧 FIX: Implement IAsyncLifetime.DisposeAsync() (returns Task)
    Task IAsyncLifetime.DisposeAsync()
    {
        // Don't delete the database until all tests are done
        return Task.CompletedTask;
    }

    // 🔧 FIX: Override base class DisposeAsync() (returns ValueTask)
    public override async ValueTask DisposeAsync()
    {
        // Call the IAsyncLifetime implementation
        await ((IAsyncLifetime)this).DisposeAsync();
        
        // Call base class disposal
        await base.DisposeAsync();
    }
}

// Simplified tenant provider that works with the database
public class SimplePerformanceTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public SimplePerformanceTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<int?> GetCurrentTenantIdAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        
        // Try JWT token first
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out var tenantId))
            {
                return Task.FromResult<int?>(tenantId);
            }
        }
        
        // Try header
        if (httpContext?.Request?.Headers.TryGetValue("X-Tenant-ID", out var headerValues) == true)
        {
            if (int.TryParse(headerValues.FirstOrDefault(), out var headerTenantId))
            {
                return Task.FromResult<int?>(headerTenantId);
            }
        }
        
        // Default to tenant 1
        return Task.FromResult<int?>(1);
    }
    
    public Task<string?> GetCurrentTenantIdentifierAsync()
    {
        return Task.FromResult<string?>("1");
    }
    
    public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
    public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
    public Task ClearCurrentTenantAsync() => Task.CompletedTask;
    
    public bool HasTenantContext => true;
}
