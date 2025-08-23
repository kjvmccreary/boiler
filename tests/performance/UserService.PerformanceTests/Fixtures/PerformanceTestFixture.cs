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

namespace UserService.PerformanceTests.Fixtures;

public class PerformanceTestFixture : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    
    public PerformanceTestFixture()
    {
        _databaseName = $"PerformanceTestDb_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}";
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Critical: Set environment BEFORE ConfigureServices
        builder.UseEnvironment("Performance");

        builder.ConfigureServices(services =>
        {
            // Remove existing database services completely
            var descriptors = services.Where(d => 
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(ApplicationDbContext) ||
                d.ImplementationType?.Name.Contains("ApplicationDbContext") == true ||
                d.ServiceType.Name.Contains("DbContext")).ToList();
            
            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
            }

            // Critical: Add InMemory database BEFORE other services
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(_databaseName);
                options.EnableSensitiveDataLogging(false);
                options.EnableDetailedErrors(false);
                options.EnableServiceProviderCaching(false); // Disable for performance tests
                
                // Performance optimizations
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
            
            // Performance-optimized TenantProvider
            services.RemoveAll<ITenantProvider>();
            services.AddSingleton<ITenantProvider, PerformanceTenantProvider>();

            // Tenant settings optimized for performance
            services.RemoveAll<TenantSettings>();
            services.AddSingleton<TenantSettings>(new TenantSettings 
            {
                RequireTenantContext = false,
                ResolutionStrategy = TenantResolutionStrategy.Header,
                DefaultTenantId = "1",
                TenantHeaderName = "X-Tenant-ID",
                EnableRowLevelSecurity = false, // Disabled for performance testing
                AllowCrossTenantQueries = false
            });

            // CRITICAL: Completely disable Redis-dependent services
            services.RemoveAll<IDistributedCache>();
            services.RemoveAll<StackExchange.Redis.IConnectionMultiplexer>();
            services.RemoveAll<StackExchange.Redis.IDatabase>();
            
            // Replace with in-memory alternatives - use the proper .NET 9 way
            services.AddMemoryCache();
            services.AddDistributedMemoryCache(); // This is the correct .NET 9 method
            
            // Reduce logging for performance tests
            services.AddLogging(builder =>
            {
                builder.SetMinimumLevel(LogLevel.Warning);
                builder.AddConsole();
            });
        });
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
                // Ignore cleanup errors
            }
        }
        base.Dispose(disposing);
    }
}

// High-performance tenant provider for load testing
public class PerformanceTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly Dictionary<int, string> _tenantCache;
    
    public PerformanceTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantCache = new Dictionary<int, string>
        {
            { 1, "1" },
            { 2, "2" },
            { 3, "3" },
            { 4, "4" },
            { 5, "5" }
        };
    }

    public Task<int?> GetCurrentTenantIdAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out var tenantId))
            {
                return Task.FromResult<int?>(tenantId);
            }
        }
        
        // Performance: return cached tenant
        return Task.FromResult<int?>(1);
    }
    
    public Task<string?> GetCurrentTenantIdentifierAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null && _tenantCache.ContainsKey(int.Parse(tenantClaim.Value)))
            {
                return Task.FromResult<string?>(_tenantCache[int.Parse(tenantClaim.Value)]);
            }
        }
        
        return Task.FromResult<string?>("1");
    }
    
    public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
    public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
    public Task ClearCurrentTenantAsync() => Task.CompletedTask;
    
    public bool HasTenantContext => true;
}
