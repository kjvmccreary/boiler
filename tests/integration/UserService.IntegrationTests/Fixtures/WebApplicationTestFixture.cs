using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Common.Data;
using Microsoft.AspNetCore.Http;
using Contracts.Services;
using Common.Configuration;
using Common.Caching;
using UserService.IntegrationTests.TestUtilities;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Moq;

namespace UserService.IntegrationTests.Fixtures;

public class WebApplicationTestFixture : WebApplicationFactory<Program>
{
    private readonly string _databaseName;
    
    public WebApplicationTestFixture()
    {
        _databaseName = $"IntegrationTestDb_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}_{Guid.NewGuid():N}";
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // 🔧 CRITICAL FIX: Configure test-specific settings to disable rate limiting
        builder.ConfigureAppConfiguration((context, config) =>
        {
            // 🔧 FIX: Cast to IEnumerable<KeyValuePair<string, string?>> to fix nullability warning
            var testConfiguration = new KeyValuePair<string, string?>[]
            {
                // Disable rate limiting for tests
                new("RateLimiting:Enabled", "false"),
                new("RateLimiting:GlobalOptions:PermitLimit", "10000"),
                new("RateLimiting:GlobalOptions:Window", "01:00:00"),
                new("RateLimiting:GlobalOptions:QueueLimit", "1000"),
                new("RateLimiting:AuthOptions:PermitLimit", "10000"),
                new("RateLimiting:AuthOptions:Window", "01:00:00"),
                
                // Ensure testing environment
                new("ASPNETCORE_ENVIRONMENT", "Testing"),
                
                // Disable HTTPS redirection warnings
                new("ASPNETCORE_HTTPS_PORT", ""),
            };
            
            config.AddInMemoryCollection(testConfiguration);
        });
        
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

            // ✅ Mock IHttpContextAccessor 
            services.RemoveAll<IHttpContextAccessor>();
            services.AddSingleton<IHttpContextAccessor>(provider =>
            {
                var accessor = new HttpContextAccessor();
                return accessor;
            });

            // ✅ CRITICAL FIX: Use dynamic TestTenantProvider that reads from JWT claims
            services.RemoveAll<ITenantProvider>();
            services.AddSingleton<ITenantProvider>(provider =>
            {
                var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
                return new TestTenantProvider(httpContextAccessor);
            });

            // ✅ CRITICAL: Add TenantSettings that completely disables tenant requirements
            services.RemoveAll<TenantSettings>();
            services.AddSingleton<TenantSettings>(new TenantSettings
            {
                RequireTenantContext = false, // ✅ Don't require tenant context in tests
                ResolutionStrategy = TenantResolutionStrategy.Header,
                DefaultTenantId = "1",  // ✅ Default to tenant 1
                TenantHeaderName = "X-Tenant-ID",
                EnableRowLevelSecurity = false, // ✅ Disable RLS in tests
                AllowCrossTenantQueries = true // ✅ Allow cross-tenant queries in tests
            });

            // ✅ SIMPLIFIED: Use Moq to create Redis mocks
            services.RemoveAll<StackExchange.Redis.IConnectionMultiplexer>();
            services.RemoveAll<IPermissionCache>();
            services.RemoveAll<Common.Caching.ICacheService>();

            // Create mock Redis connection using Moq
            var mockConnectionMultiplexer = new Mock<StackExchange.Redis.IConnectionMultiplexer>();
            var mockDatabase = new Mock<StackExchange.Redis.IDatabase>();
            
            mockConnectionMultiplexer.Setup(x => x.IsConnected).Returns(true);
            mockConnectionMultiplexer.Setup(x => x.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
                                   .Returns(mockDatabase.Object);
            
            // Setup basic Redis operations that might be called
            mockDatabase.Setup(x => x.StringSetAsync(It.IsAny<StackExchange.Redis.RedisKey>(), 
                                                   It.IsAny<StackExchange.Redis.RedisValue>(), 
                                                   It.IsAny<TimeSpan?>(), 
                                                   It.IsAny<StackExchange.Redis.When>(), 
                                                   It.IsAny<StackExchange.Redis.CommandFlags>()))
                       .ReturnsAsync(true);
            
            mockDatabase.Setup(x => x.StringGetAsync(It.IsAny<StackExchange.Redis.RedisKey>(), 
                                                    It.IsAny<StackExchange.Redis.CommandFlags>()))
                       .ReturnsAsync(DateTime.UtcNow.ToString()); // Return a test value
            
            services.AddSingleton(mockConnectionMultiplexer.Object);

            // Add in-memory permission cache for testing
            services.AddSingleton<IPermissionCache, InMemoryPermissionCache>();

            // Add mock cache service if needed by other components
            services.AddSingleton<Common.Caching.ICacheService, MockCacheService>();

            // ✅ Use regular ApplicationDbContext with proper test configuration
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

        // ✅ CRITICAL: Use Testing environment
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

// ✅ CRITICAL FIX: Dynamic Test TenantProvider that reads tenant from JWT claims
public class TestTenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public TestTenantProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<int?> GetCurrentTenantIdAsync()
    {
        // ✅ CRITICAL: Read tenant from JWT claims if available
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null && int.TryParse(tenantClaim.Value, out var tenantId))
            {
                return Task.FromResult<int?>(tenantId);
            }
        }

        // ✅ Fallback to tenant 1 if no claims found (for data seeding)
        return Task.FromResult<int?>(1);
    }

    public Task<string?> GetCurrentTenantIdentifierAsync()
    {
        // ✅ Read from claims or fallback
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = httpContext.User.FindFirst("tenant_id");
            if (tenantClaim != null)
            {
                return Task.FromResult<string?>(tenantClaim.Value);
            }
        }

        return Task.FromResult<string?>("1");
    }

    public Task SetCurrentTenantAsync(int tenantId) => Task.CompletedTask;
    public Task SetCurrentTenantAsync(string tenantIdentifier) => Task.CompletedTask;
    public Task ClearCurrentTenantAsync() => Task.CompletedTask;

    // ✅ Return true if we have tenant context from JWT or fallback
    public bool HasTenantContext
    {
        get
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.User?.Identity?.IsAuthenticated == true ||
                   httpContext?.Request != null; // Always have context during tests
        }
    }
}

// 🔧 FIX: Correct return type to match interface
public class MockCacheService : Common.Caching.ICacheService
{
    private readonly ConcurrentDictionary<string, object?> _cache = new();

    public Task<T?> GetAsync<T>(string key)
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult((T?)value);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        _cache.AddOrUpdate(key, value, (_, _) => value);
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string key)
    {
        return Task.FromResult(_cache.ContainsKey(key));
    }

    public Task RemoveAsync(string key)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        var dotnetPattern = pattern.Replace("*", "");
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith(dotnetPattern)).ToList();
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
        return Task.CompletedTask;
    }

    // 🔧 FIX: Change return type to Task<T> to match interface
    public async Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
    {
        if (_cache.TryGetValue(key, out var existing) && existing is T existingT)
        {
            return existingT;
        }

        var value = await factory();
        await SetAsync(key, value, expiration);
        return value;
    }
}
