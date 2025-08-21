# Phase 10: Caching & Performance Optimization - Implementation Guide

## 🎯 Phase Overview

**Duration**: 2-3 sessions (8-12 hours)  
**Complexity**: Medium-High  
**Prerequisites**: Phase 9 (Advanced Multi-Tenancy with RBAC) completed

### Objectives
- Implement Redis caching for permissions and tenant data
- Optimize authorization performance to < 10ms response time
- Implement intelligent cache invalidation strategies
- Add batch loading for role/permission data
- Achieve > 95% cache hit ratio

### Success Criteria
✅ Permission checks respond in < 10ms (cached)  
✅ Cache hit ratio > 95%  
✅ Cache invalidation working correctly  
✅ Batch permission loading implemented  
✅ Query optimization completed  
✅ Performance metrics dashboard created

## 📚 Project Context for GitHub Copilot

### Current Architecture
```
Technology Stack:
- Backend: .NET 9 with ASP.NET Core Web API
- Frontend: React + Tailwind + TypeScript + Material UI + Vite
- Database: PostgreSQL with Entity Framework Core
- Authentication: JWT tokens with multi-role RBAC
- Multi-tenancy: Shared database, shared schema with RLS
- Caching: Redis (Docker container running)
- Containerization: Docker & Docker Compose
```

### Current RBAC Implementation
```csharp
// Key Tables:
- Users                   # Core user accounts
- Tenants                 # Tenant organizations
- TenantUsers             # Many-to-many relationship. Critical feature.
- Roles                   # Dynamic roles per tenant
- UserRoles               # Many-to-many user-role assignments
- Permissions             # System-wide permission definitions
- RolePermissions         # Role-permission mappings

// Current Authorization Flow:
1. User enters email / password
2. Backend authenticates and issues JWT not containing tenant ID.
3. User presented with list of tenants to which they have access
4. If only one tenant in list, that tenant autoselected.
5. If many tenants in list, user must select a tenant
6. Afer selection, refreshed JWT
7. JWT contains user ID and tenant ID
8. Database query to fetch user roles
9. Database query to fetch role permissions
10. Authorization handler evaluates permissions
11. Access granted/denied
```

### Performance Bottlenecks to Address
- Multiple database queries per authorization check
- No caching of permission data
- Repeated permission evaluations
- N+1 query problems in role/permission fetching
- No batch loading capabilities

---

## Session 1: Redis Infrastructure & Cache Abstractions (3-4 hours)

### 🎯 Session Goals
- Set up Redis connection and configuration
- Create cache abstraction interfaces
- Implement base caching services
- Add cache key management

### 📋 Implementation Tasks

#### Task 1.1: Add Redis NuGet Packages
```bash
# In UserService project
dotnet add package StackExchange.Redis
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

#### Task 1.2: Create Cache Abstractions
**File: `src/shared/Common/Caching/ICacheService.cs`**
```csharp
using System;
using System.Threading.Tasks;

namespace Common.Caching
{
    public interface ICacheService
    {
        Task<T> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
        Task<bool> ExistsAsync(string key);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    }
}
```

#### Task 1.3: Implement Redis Cache Service
**File: `src/shared/Common/Caching/RedisCacheService.cs`**
```csharp
using StackExchange.Redis;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Common.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        
        public RedisCacheService(
            IConnectionMultiplexer redis,
            ILogger<RedisCacheService> logger)
        {
            _redis = redis;
            _database = redis.GetDatabase();
            _logger = logger;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                if (value.IsNullOrEmpty)
                    return default(T);
                
                return JsonSerializer.Deserialize<T>(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting cache key: {Key}", key);
                return default(T);
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value);
                await _database.StringSetAsync(key, json, expiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key: {Key}", key);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            var server = _redis.GetServer(_redis.GetEndPoints()[0]);
            var keys = server.Keys(pattern: pattern);
            
            foreach (var key in keys)
            {
                await _database.KeyDeleteAsync(key);
            }
        }

        public async Task<T> GetOrSetAsync<T>(
            string key, 
            Func<Task<T>> factory, 
            TimeSpan? expiration = null)
        {
            var cached = await GetAsync<T>(key);
            if (cached != null)
                return cached;

            var value = await factory();
            await SetAsync(key, value, expiration);
            return value;
        }
    }
}
```

#### Task 1.4: Create Permission Cache Interface
**File: `src/shared/Common/Caching/IPermissionCache.cs`**
```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Caching
{
    public interface IPermissionCache
    {
        Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, Guid tenantId);
        Task SetUserPermissionsAsync(Guid userId, Guid tenantId, IEnumerable<string> permissions);
        Task InvalidateUserPermissionsAsync(Guid userId, Guid tenantId);
        Task InvalidateTenantPermissionsAsync(Guid tenantId);
        Task<Dictionary<Guid, IEnumerable<string>>> GetRolePermissionsAsync(Guid tenantId);
        Task SetRolePermissionsAsync(Guid tenantId, Dictionary<Guid, IEnumerable<string>> rolePermissions);
        Task InvalidateRolePermissionsAsync(Guid roleId, Guid tenantId);
    }
}
```

#### Task 1.5: Implement Permission Cache
**File: `src/services/UserService/Services/RedisPermissionCache.cs`**
```csharp
using Common.Caching;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UserService.Services
{
    public class RedisPermissionCache : IPermissionCache
    {
        private readonly ICacheService _cache;
        private readonly ILogger<RedisPermissionCache> _logger;
        private readonly TimeSpan _userPermissionExpiration = TimeSpan.FromMinutes(5);
        private readonly TimeSpan _rolePermissionExpiration = TimeSpan.FromMinutes(10);

        public RedisPermissionCache(
            ICacheService cache,
            ILogger<RedisPermissionCache> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId, Guid tenantId)
        {
            var key = GetUserPermissionKey(userId, tenantId);
            return await _cache.GetAsync<IEnumerable<string>>(key);
        }

        public async Task SetUserPermissionsAsync(
            Guid userId, 
            Guid tenantId, 
            IEnumerable<string> permissions)
        {
            var key = GetUserPermissionKey(userId, tenantId);
            await _cache.SetAsync(key, permissions, _userPermissionExpiration);
            _logger.LogDebug("Cached permissions for user {UserId} in tenant {TenantId}", 
                userId, tenantId);
        }

        public async Task InvalidateUserPermissionsAsync(Guid userId, Guid tenantId)
        {
            var key = GetUserPermissionKey(userId, tenantId);
            await _cache.RemoveAsync(key);
            _logger.LogDebug("Invalidated permission cache for user {UserId}", userId);
        }

        public async Task InvalidateTenantPermissionsAsync(Guid tenantId)
        {
            var pattern = $"permissions:tenant:{tenantId}:*";
            await _cache.RemoveByPatternAsync(pattern);
            _logger.LogDebug("Invalidated all permission caches for tenant {TenantId}", tenantId);
        }

        public async Task<Dictionary<Guid, IEnumerable<string>>> GetRolePermissionsAsync(Guid tenantId)
        {
            var key = GetRolePermissionKey(tenantId);
            return await _cache.GetAsync<Dictionary<Guid, IEnumerable<string>>>(key);
        }

        public async Task SetRolePermissionsAsync(
            Guid tenantId, 
            Dictionary<Guid, IEnumerable<string>> rolePermissions)
        {
            var key = GetRolePermissionKey(tenantId);
            await _cache.SetAsync(key, rolePermissions, _rolePermissionExpiration);
        }

        public async Task InvalidateRolePermissionsAsync(Guid roleId, Guid tenantId)
        {
            // Invalidate role cache
            var roleKey = GetRolePermissionKey(tenantId);
            await _cache.RemoveAsync(roleKey);
            
            // Invalidate all user caches for this tenant (since role changed)
            await InvalidateTenantPermissionsAsync(tenantId);
        }

        private string GetUserPermissionKey(Guid userId, Guid tenantId)
        {
            return $"permissions:tenant:{tenantId}:user:{userId}";
        }

        private string GetRolePermissionKey(Guid tenantId)
        {
            return $"permissions:tenant:{tenantId}:roles";
        }
    }
}
```

#### Task 1.6: Configure Redis in Startup
**File: `src/services/UserService/Program.cs`** (Add to ConfigureServices)
```csharp
// Add Redis
var redisConnectionString = builder.Configuration.GetConnectionString("Redis") 
    ?? "localhost:6379";

builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(redisConnectionString);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.ConnectionMultiplexerFactory = async () =>
    {
        var multiplexer = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IConnectionMultiplexer>();
        return await Task.FromResult(multiplexer);
    };
});

// Register cache services
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IPermissionCache, RedisPermissionCache>();
```

#### Task 1.7: Update Docker Compose
**File: `docker-compose.yml`** (Ensure Redis is configured)
```yaml
services:
  redis:
    image: redis:7-alpine
    container_name: starter-app-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
    command: redis-server --appendonly yes
    networks:
      - starter-app-network

volumes:
  redis-data:
```

### 🎓 GitHub Copilot Prompts for Session 1

```plaintext
"I'm implementing Redis caching for a multi-tenant RBAC system in .NET 9. 
Help me create a cache key strategy that:
1. Ensures tenant isolation
2. Supports efficient invalidation
3. Prevents key collisions
4. Uses hierarchical key patterns"
```

```plaintext
"Create a cache warming strategy for permission data that:
1. Loads permissions on first request
2. Pre-loads frequently accessed permissions
3. Handles cache misses gracefully
4. Implements circuit breaker pattern for Redis failures"
```

### ✅ Session 1 Checklist
- [ ] Redis packages installed
- [ ] Cache abstractions created
- [ ] Redis cache service implemented
- [ ] Permission cache interface defined
- [ ] Redis permission cache implemented
- [ ] Redis configured in startup
- [ ] Docker compose updated
- [ ] Basic cache operations tested

---

## Session 2: Optimized Permission Service & Batch Loading (4 hours)

### 🎯 Session Goals
- Refactor permission service to use caching
- Implement batch loading for permissions
- Optimize database queries
- Add cache warming strategies

### 📋 Implementation Tasks

#### Task 2.1: Create Cached Permission Service
**File: `src/services/UserService/Services/CachedPermissionService.cs`**
```csharp
using Common.Caching;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Data;
using UserService.Models;
using Microsoft.Extensions.Logging;

namespace UserService.Services
{
    public class CachedPermissionService : IPermissionService
    {
        private readonly UserDbContext _context;
        private readonly IPermissionCache _cache;
        private readonly ILogger<CachedPermissionService> _logger;

        public CachedPermissionService(
            UserDbContext context,
            IPermissionCache cache,
            ILogger<CachedPermissionService> logger)
        {
            _context = context;
            _cache = cache;
            _logger = logger;
        }

        public async Task<IEnumerable<string>> GetUserPermissionsAsync(
            Guid userId, 
            Guid tenantId)
        {
            // Try cache first
            var cached = await _cache.GetUserPermissionsAsync(userId, tenantId);
            if (cached != null)
            {
                _logger.LogDebug("Cache hit for user {UserId} permissions", userId);
                return cached;
            }

            _logger.LogDebug("Cache miss for user {UserId} permissions", userId);
            
            // Load from database with optimized query
            var permissions = await LoadUserPermissionsFromDatabaseAsync(userId, tenantId);
            
            // Cache the result
            await _cache.SetUserPermissionsAsync(userId, tenantId, permissions);
            
            return permissions;
        }

        private async Task<IEnumerable<string>> LoadUserPermissionsFromDatabaseAsync(
            Guid userId, 
            Guid tenantId)
        {
            // Single optimized query to get all permissions
            var permissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Select(rp => rp.Permission.Name)
                .Distinct()
                .ToListAsync();

            return permissions;
        }

        public async Task<Dictionary<Guid, IEnumerable<string>>> GetBatchUserPermissionsAsync(
            IEnumerable<Guid> userIds, 
            Guid tenantId)
        {
            var result = new Dictionary<Guid, IEnumerable<string>>();
            var uncachedUsers = new List<Guid>();

            // Check cache for each user
            foreach (var userId in userIds)
            {
                var cached = await _cache.GetUserPermissionsAsync(userId, tenantId);
                if (cached != null)
                {
                    result[userId] = cached;
                }
                else
                {
                    uncachedUsers.Add(userId);
                }
            }

            // Batch load uncached users
            if (uncachedUsers.Any())
            {
                var batchPermissions = await LoadBatchPermissionsFromDatabaseAsync(
                    uncachedUsers, 
                    tenantId);
                
                foreach (var kvp in batchPermissions)
                {
                    result[kvp.Key] = kvp.Value;
                    await _cache.SetUserPermissionsAsync(kvp.Key, tenantId, kvp.Value);
                }
            }

            return result;
        }

        private async Task<Dictionary<Guid, IEnumerable<string>>> LoadBatchPermissionsFromDatabaseAsync(
            IEnumerable<Guid> userIds, 
            Guid tenantId)
        {
            // Single query to load all permissions for multiple users
            var userPermissions = await _context.UserRoles
                .Where(ur => userIds.Contains(ur.UserId) && ur.TenantId == tenantId)
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
                .ToListAsync();

            var result = new Dictionary<Guid, IEnumerable<string>>();
            
            foreach (var userId in userIds)
            {
                var permissions = userPermissions
                    .Where(ur => ur.UserId == userId)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Select(rp => rp.Permission.Name)
                    .Distinct()
                    .ToList();
                
                result[userId] = permissions;
            }

            return result;
        }

        public async Task InvalidateUserCacheAsync(Guid userId, Guid tenantId)
        {
            await _cache.InvalidateUserPermissionsAsync(userId, tenantId);
        }

        public async Task InvalidateTenantCacheAsync(Guid tenantId)
        {
            await _cache.InvalidateTenantPermissionsAsync(tenantId);
        }

        public async Task WarmCacheForTenantAsync(Guid tenantId)
        {
            _logger.LogInformation("Warming permission cache for tenant {TenantId}", tenantId);
            
            // Load all active users for tenant
            var activeUsers = await _context.Users
                .Where(u => u.TenantUsers.Any(tu => tu.TenantId == tenantId && tu.IsActive))
                .Select(u => u.Id)
                .ToListAsync();

            // Batch load permissions
            await GetBatchUserPermissionsAsync(activeUsers, tenantId);
            
            _logger.LogInformation("Warmed cache for {Count} users in tenant {TenantId}", 
                activeUsers.Count, tenantId);
        }
    }
}
```

#### Task 2.2: Create Query Optimization Extensions
**File: `src/services/UserService/Data/QueryExtensions.cs`**
```csharp
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using UserService.Models;

namespace UserService.Data
{
    public static class QueryExtensions
    {
        public static IQueryable<UserRole> IncludePermissions(this IQueryable<UserRole> query)
        {
            return query
                .Include(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission);
        }

        public static IQueryable<Role> IncludePermissions(this IQueryable<Role> query)
        {
            return query
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
        }

        public static IQueryable<User> IncludeRolesAndPermissions(
            this IQueryable<User> query, 
            Guid tenantId)
        {
            return query
                .Include(u => u.UserRoles.Where(ur => ur.TenantId == tenantId))
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission);
        }

        // Compiled query for frequent permission checks
        public static readonly Func<UserDbContext, Guid, Guid, Task<bool>> HasPermissionCompiled =
            EF.CompileAsyncQuery((UserDbContext context, Guid userId, Guid tenantId) =>
                context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.TenantId == tenantId)
                    .SelectMany(ur => ur.Role.RolePermissions)
                    .Any());
    }
}
```

#### Task 2.3: Implement Cache Invalidation Service
**File: `src/services/UserService/Services/CacheInvalidationService.cs`**
```csharp
using Common.Caching;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace UserService.Services
{
    public interface ICacheInvalidationService
    {
        Task OnRoleUpdatedAsync(Guid roleId, Guid tenantId);
        Task OnUserRoleAssignedAsync(Guid userId, Guid roleId, Guid tenantId);
        Task OnUserRoleRevokedAsync(Guid userId, Guid roleId, Guid tenantId);
        Task OnPermissionChangedAsync(Guid permissionId);
        Task OnTenantDeactivatedAsync(Guid tenantId);
    }

    public class CacheInvalidationService : ICacheInvalidationService
    {
        private readonly IPermissionCache _cache;
        private readonly ILogger<CacheInvalidationService> _logger;

        public CacheInvalidationService(
            IPermissionCache cache,
            ILogger<CacheInvalidationService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task OnRoleUpdatedAsync(Guid roleId, Guid tenantId)
        {
            _logger.LogInformation("Invalidating cache for role {RoleId} update", roleId);
            await _cache.InvalidateRolePermissionsAsync(roleId, tenantId);
        }

        public async Task OnUserRoleAssignedAsync(Guid userId, Guid roleId, Guid tenantId)
        {
            _logger.LogInformation("Invalidating cache for user {UserId} role assignment", userId);
            await _cache.InvalidateUserPermissionsAsync(userId, tenantId);
        }

        public async Task OnUserRoleRevokedAsync(Guid userId, Guid roleId, Guid tenantId)
        {
            _logger.LogInformation("Invalidating cache for user {UserId} role revocation", userId);
            await _cache.InvalidateUserPermissionsAsync(userId, tenantId);
        }

        public async Task OnPermissionChangedAsync(Guid permissionId)
        {
            // This is a system-wide change, invalidate everything
            _logger.LogWarning("System-wide permission change, clearing all caches");
            await _cache.RemoveByPatternAsync("permissions:*");
        }

        public async Task OnTenantDeactivatedAsync(Guid tenantId)
        {
            _logger.LogInformation("Clearing all caches for deactivated tenant {TenantId}", tenantId);
            await _cache.InvalidateTenantPermissionsAsync(tenantId);
        }
    }
}
```

#### Task 2.4: Update Role Controller with Cache Invalidation
**File: `src/services/UserService/Controllers/RolesController.cs`** (Update methods)
```csharp
[HttpPut("{id}")]
[Authorize(Policy = "roles.edit")]
public async Task<IActionResult> UpdateRole(Guid id, UpdateRoleDto dto)
{
    var tenantId = GetTenantId();
    
    // Update role logic...
    var result = await _roleService.UpdateRoleAsync(id, dto, tenantId);
    
    // Invalidate cache
    await _cacheInvalidation.OnRoleUpdatedAsync(id, tenantId);
    
    return Ok(result);
}

[HttpPost("{roleId}/users/{userId}")]
[Authorize(Policy = "roles.assign")]
public async Task<IActionResult> AssignUserToRole(Guid roleId, Guid userId)
{
    var tenantId = GetTenantId();
    
    // Assign role logic...
    await _roleService.AssignUserToRoleAsync(userId, roleId, tenantId);
    
    // Invalidate cache
    await _cacheInvalidation.OnUserRoleAssignedAsync(userId, roleId, tenantId);
    
    return Ok();
}
```

#### Task 2.5: Add Database Query Optimization
**File: `src/services/UserService/Data/UserDbContext.cs`** (Add indexes)
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Existing configuration...

    // Add performance indexes
    modelBuilder.Entity<UserRole>()
        .HasIndex(ur => new { ur.UserId, ur.TenantId })
        .HasDatabaseName("IX_UserRole_UserId_TenantId");

    modelBuilder.Entity<UserRole>()
        .HasIndex(ur => new { ur.TenantId, ur.RoleId })
        .HasDatabaseName("IX_UserRole_TenantId_RoleId");

    modelBuilder.Entity<RolePermission>()
        .HasIndex(rp => rp.RoleId)
        .HasDatabaseName("IX_RolePermission_RoleId");

    modelBuilder.Entity<Role>()
        .HasIndex(r => new { r.TenantId, r.IsActive })
        .HasDatabaseName("IX_Role_TenantId_IsActive");
}
```

### 🎓 GitHub Copilot Prompts for Session 2

```plaintext
"Help me optimize this Entity Framework query for loading user permissions.
Current issues:
1. N+1 query problem
2. Loading unnecessary data
3. Multiple round trips to database
Requirements:
1. Single query if possible
2. Only load required fields
3. Support batch loading"
```

```plaintext
"Create a cache warming strategy that:
1. Pre-loads permissions for active users
2. Runs on application startup for small tenants
3. Uses background job for large tenants
4. Doesn't block application startup"
```

### ✅ Session 2 Checklist
- [ ] Cached permission service created
- [ ] Batch loading implemented
- [ ] Query optimization extensions added
- [ ] Cache invalidation service created
- [ ] Controllers updated with invalidation
- [ ] Database indexes added
- [ ] Performance improved to < 50ms

---

## Session 3: Performance Monitoring & Testing (4 hours)

### 🎯 Session Goals
- Add performance metrics collection
- Implement cache hit ratio monitoring
- Create performance tests
- Add health checks for Redis

### 📋 Implementation Tasks

#### Task 3.1: Create Performance Metrics Service
**File: `src/services/UserService/Services/PerformanceMetricsService.cs`**
```csharp
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UserService.Services
{
    public interface IPerformanceMetrics
    {
        void RecordCacheHit(string operation);
        void RecordCacheMiss(string operation);
        void RecordOperationDuration(string operation, long milliseconds);
        Task<PerformanceStats> GetStatsAsync();
    }

    public class PerformanceMetricsService : IPerformanceMetrics, IHostedService
    {
        private readonly ConcurrentDictionary<string, OperationMetrics> _metrics;
        private readonly ILogger<PerformanceMetricsService> _logger;
        private Timer _reportTimer;

        public PerformanceMetricsService(ILogger<PerformanceMetricsService> logger)
        {
            _metrics = new ConcurrentDictionary<string, OperationMetrics>();
            _logger = logger;
        }

        public void RecordCacheHit(string operation)
        {
            var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
            Interlocked.Increment(ref metrics.CacheHits);
            Interlocked.Increment(ref metrics.TotalRequests);
        }

        public void RecordCacheMiss(string operation)
        {
            var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
            Interlocked.Increment(ref metrics.CacheMisses);
            Interlocked.Increment(ref metrics.TotalRequests);
        }

        public void RecordOperationDuration(string operation, long milliseconds)
        {
            var metrics = _metrics.GetOrAdd(operation, _ => new OperationMetrics());
            Interlocked.Add(ref metrics.TotalDuration, milliseconds);
            Interlocked.Increment(ref metrics.OperationCount);
            
            // Update min/max
            UpdateMinMax(metrics, milliseconds);
        }

        private void UpdateMinMax(OperationMetrics metrics, long milliseconds)
        {
            // Update minimum
            long currentMin;
            do
            {
                currentMin = metrics.MinDuration;
                if (milliseconds >= currentMin && currentMin != 0) break;
            } while (Interlocked.CompareExchange(ref metrics.MinDuration, milliseconds, currentMin) != currentMin);

            // Update maximum
            long currentMax;
            do
            {
                currentMax = metrics.MaxDuration;
                if (milliseconds <= currentMax) break;
            } while (Interlocked.CompareExchange(ref metrics.MaxDuration, milliseconds, currentMax) != currentMax);
        }

        public Task<PerformanceStats> GetStatsAsync()
        {
            var stats = new PerformanceStats();
            
            foreach (var kvp in _metrics)
            {
                var metrics = kvp.Value;
                var cacheHitRatio = metrics.TotalRequests > 0 
                    ? (double)metrics.CacheHits / metrics.TotalRequests * 100 
                    : 0;
                
                var avgDuration = metrics.OperationCount > 0 
                    ? metrics.TotalDuration / metrics.OperationCount 
                    : 0;

                stats.Operations.Add(new OperationStats
                {
                    Operation = kvp.Key,
                    CacheHitRatio = cacheHitRatio,
                    AverageDuration = avgDuration,
                    MinDuration = metrics.MinDuration,
                    MaxDuration = metrics.MaxDuration,
                    TotalRequests = metrics.TotalRequests
                });
            }

            return Task.FromResult(stats);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _reportTimer = new Timer(ReportMetrics, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _reportTimer?.Dispose();
            return Task.CompletedTask;
        }

        private async void ReportMetrics(object state)
        {
            var stats = await GetStatsAsync();
            
            foreach (var op in stats.Operations)
            {
                _logger.LogInformation(
                    "Performance Metrics - Operation: {Operation}, " +
                    "Cache Hit Ratio: {HitRatio:F2}%, " +
                    "Avg Duration: {AvgDuration}ms, " +
                    "Min: {MinDuration}ms, Max: {MaxDuration}ms",
                    op.Operation, op.CacheHitRatio, op.AverageDuration, 
                    op.MinDuration, op.MaxDuration);
            }
        }

        private class OperationMetrics
        {
            public long CacheHits;
            public long CacheMisses;
            public long TotalRequests;
            public long TotalDuration;
            public long OperationCount;
            public long MinDuration = long.MaxValue;
            public long MaxDuration;
        }
    }

    public class PerformanceStats
    {
        public List<OperationStats> Operations { get; set; } = new List<OperationStats>();
    }

    public class OperationStats
    {
        public string Operation { get; set; }
        public double CacheHitRatio { get; set; }
        public long AverageDuration { get; set; }
        public long MinDuration { get; set; }
        public long MaxDuration { get; set; }
        public long TotalRequests { get; set; }
    }
}
```

#### Task 3.2: Add Performance Tracking to Permission Service
**File: `src/services/UserService/Services/CachedPermissionService.cs`** (Update with metrics)
```csharp
public async Task<IEnumerable<string>> GetUserPermissionsAsync(
    Guid userId, 
    Guid tenantId)
{
    var stopwatch = Stopwatch.StartNew();
    
    // Try cache first
    var cached = await _cache.GetUserPermissionsAsync(userId, tenantId);
    if (cached != null)
    {
        _metrics.RecordCacheHit("GetUserPermissions");
        _logger.LogDebug("Cache hit for user {UserId} permissions", userId);
        
        stopwatch.Stop();
        _metrics.RecordOperationDuration("GetUserPermissions", stopwatch.ElapsedMilliseconds);
        
        return cached;
    }

    _metrics.RecordCacheMiss("GetUserPermissions");
    _logger.LogDebug("Cache miss for user {UserId} permissions", userId);
    
    // Load from database with optimized query
    var permissions = await LoadUserPermissionsFromDatabaseAsync(userId, tenantId);
    
    // Cache the result
    await _cache.SetUserPermissionsAsync(userId, tenantId, permissions);
    
    stopwatch.Stop();
    _metrics.RecordOperationDuration("GetUserPermissions", stopwatch.ElapsedMilliseconds);
    
    return permissions;
}
```

#### Task 3.3: Create Redis Health Check
**File: `src/services/UserService/HealthChecks/RedisHealthCheck.cs`**
```csharp
using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.HealthChecks
{
    public class RedisHealthCheck : IHealthCheck
    {
        private readonly IConnectionMultiplexer _redis;

        public RedisHealthCheck(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var database = _redis.GetDatabase();
                var key = $"health_check_{Guid.NewGuid()}";
                
                await database.StringSetAsync(key, "test", TimeSpan.FromSeconds(1));
                var value = await database.StringGetAsync(key);
                
                if (value == "test")
                {
                    var endpoints = _redis.GetEndPoints();
                    var server = _redis.GetServer(endpoints[0]);
                    var info = await server.InfoAsync();
                    
                    return HealthCheckResult.Healthy(
                        "Redis is healthy",
                        new Dictionary<string, object>
                        {
                            ["endpoints"] = string.Join(", ", endpoints.Select(e => e.ToString())),
                            ["connected_clients"] = server.ClientList().Length
                        });
                }
                
                return HealthCheckResult.Unhealthy("Redis test failed");
            }
            catch (Exception ex)
            {
                return HealthCheckResult.Unhealthy(
                    "Redis connection failed",
                    ex);
            }
        }
    }
}
```

#### Task 3.4: Create Performance Controller
**File: `src/services/UserService/Controllers/PerformanceController.cs`**
```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UserService.Services;

namespace UserService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "SystemAdmin")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceMetrics _metrics;
        private readonly IPermissionCache _cache;

        public PerformanceController(
            IPerformanceMetrics metrics,
            IPermissionCache cache)
        {
            _metrics = metrics;
            _cache = cache;
        }

        [HttpGet("stats")]
        public async Task<IActionResult> GetPerformanceStats()
        {
            var stats = await _metrics.GetStatsAsync();
            return Ok(stats);
        }

        [HttpPost("cache/warm/{tenantId}")]
        public async Task<IActionResult> WarmCache(Guid tenantId)
        {
            // Implementation to warm cache for a specific tenant
            var service = HttpContext.RequestServices.GetRequiredService<CachedPermissionService>();
            await service.WarmCacheForTenantAsync(tenantId);
            
            return Ok(new { message = "Cache warming initiated for tenant", tenantId });
        }

        [HttpDelete("cache/tenant/{tenantId}")]
        public async Task<IActionResult> InvalidateTenantCache(Guid tenantId)
        {
            await _cache.InvalidateTenantPermissionsAsync(tenantId);
            return Ok(new { message = "Cache invalidated for tenant", tenantId });
        }
    }
}
```

#### Task 3.5: Create Integration Tests
**File: `tests/UserService.Tests/CachePerformanceTests.cs`**
```csharp
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace UserService.Tests
{
    public class CachePerformanceTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly IServiceProvider _serviceProvider;

        public CachePerformanceTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _serviceProvider = factory.Services;
        }

        [Fact]
        public async Task GetUserPermissions_WithCache_ShouldBeFasterThan10ms()
        {
            // Arrange
            var permissionService = _serviceProvider.GetRequiredService<IPermissionService>();
            var userId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            
            // Warm up cache
            await permissionService.GetUserPermissionsAsync(userId, tenantId);
            
            // Act
            var stopwatch = Stopwatch.StartNew();
            var permissions = await permissionService.GetUserPermissionsAsync(userId, tenantId);
            stopwatch.Stop();
            
            // Assert
            Assert.True(stopwatch.ElapsedMilliseconds < 10, 
                $"Permission check took {stopwatch.ElapsedMilliseconds}ms, expected < 10ms");
        }

        [Fact]
        public async Task CacheHitRatio_ShouldBeAbove95Percent()
        {
            // Arrange
            var permissionService = _serviceProvider.GetRequiredService<IPermissionService>();
            var metrics = _serviceProvider.GetRequiredService<IPerformanceMetrics>();
            var userId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            
            // Act - Make 100 requests
            for (int i = 0; i < 100; i++)
            {
                await permissionService.GetUserPermissionsAsync(userId, tenantId);
            }
            
            // Get metrics
            var stats = await metrics.GetStatsAsync();
            var permissionStats = stats.Operations
                .FirstOrDefault(o => o.Operation == "GetUserPermissions");
            
            // Assert
            Assert.NotNull(permissionStats);
            Assert.True(permissionStats.CacheHitRatio >= 95, 
                $"Cache hit ratio is {permissionStats.CacheHitRatio}%, expected >= 95%");
        }

        [Fact]
        public async Task BatchLoad_ShouldBeMoreEfficientThanIndividualLoads()
        {
            // Arrange
            var service = _serviceProvider.GetRequiredService<CachedPermissionService>();
            var tenantId = Guid.NewGuid();
            var userIds = Enumerable.Range(0, 50).Select(_ => Guid.NewGuid()).ToList();
            
            // Act - Individual loads
            var individualStopwatch = Stopwatch.StartNew();
            foreach (var userId in userIds)
            {
                await service.GetUserPermissionsAsync(userId, tenantId);
            }
            individualStopwatch.Stop();
            
            // Clear cache
            await service.InvalidateTenantCacheAsync(tenantId);
            
            // Act - Batch load
            var batchStopwatch = Stopwatch.StartNew();
            await service.GetBatchUserPermissionsAsync(userIds, tenantId);
            batchStopwatch.Stop();
            
            // Assert
            Assert.True(batchStopwatch.ElapsedMilliseconds < individualStopwatch.ElapsedMilliseconds / 2,
                $"Batch load took {batchStopwatch.ElapsedMilliseconds}ms, " +
                $"individual loads took {individualStopwatch.ElapsedMilliseconds}ms");
        }

        [Fact]
        public async Task CacheInvalidation_ShouldWorkCorrectly()
        {
            // Arrange
            var cache = _serviceProvider.GetRequiredService<IPermissionCache>();
            var userId = Guid.NewGuid();
            var tenantId = Guid.NewGuid();
            var permissions = new[] { "users.view", "users.edit" };
            
            // Act
            await cache.SetUserPermissionsAsync(userId, tenantId, permissions);
            var cached = await cache.GetUserPermissionsAsync(userId, tenantId);
            
            await cache.InvalidateUserPermissionsAsync(userId, tenantId);
            var afterInvalidation = await cache.GetUserPermissionsAsync(userId, tenantId);
            
            // Assert
            Assert.NotNull(cached);
            Assert.Equal(permissions, cached);
            Assert.Null(afterInvalidation);
        }
    }
}
```

#### Task 3.6: Configure Health Checks in Startup
**File: `src/services/UserService/Program.cs`** (Add health checks)
```csharp
// Add health checks
builder.Services.AddHealthChecks()
    .AddCheck<RedisHealthCheck>("redis", tags: new[] { "cache", "redis" })
    .AddDbContextCheck<UserDbContext>("database", tags: new[] { "db", "sql" });

// Register performance metrics
builder.Services.AddSingleton<IPerformanceMetrics, PerformanceMetricsService>();
builder.Services.AddHostedService<PerformanceMetricsService>();

// In Configure method
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/cache", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("cache"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
```

### 🎓 GitHub Copilot Prompts for Session 3

```plaintext
"Create a load test for the permission caching system that:
1. Simulates 1000 concurrent users
2. Tests cache hit ratio under load
3. Measures response times at percentiles (p50, p95, p99)
4. Identifies cache stampede scenarios"
```

```plaintext
"Help me create a dashboard query for monitoring cache performance:
1. Cache hit ratio over time
2. Average response time by operation
3. Cache evictions and memory usage
4. Failed cache operations"
```

### ✅ Session 3 Checklist
- [ ] Performance metrics service created
- [ ] Metrics integrated with permission service
- [ ] Redis health check implemented
- [ ] Performance controller added
- [ ] Integration tests created
- [ ] Health checks configured
- [ ] All tests passing
- [ ] Performance targets met (< 10ms, > 95% hit ratio)

---

## 📊 Performance Validation Checklist

### Required Performance Metrics
- [ ] **Authorization Check**: < 10ms for cached permissions
- [ ] **Cache Hit Ratio**: > 95% after warm-up
- [ ] **Database Query**: < 50ms for permission loading
- [ ] **Batch Loading**: 50+ users in < 200ms
- [ ] **Cache Invalidation**: < 100ms
- [ ] **Memory Usage**: < 100MB for 10,000 users

### Load Testing Scenarios
1. **Baseline Test**: 100 concurrent users, 5 minutes
2. **Stress Test**: 1000 concurrent users, 15 minutes
3. **Spike Test**: 0 to 500 users in 30 seconds
4. **Endurance Test**: 200 users for 1 hour

---

## 🚀 Deployment Considerations

### Environment Variables
```env
# Redis Configuration
REDIS_CONNECTION=redis:6379
REDIS_PASSWORD=
REDIS_SSL=false
REDIS_CONNECT_TIMEOUT=5000
REDIS_CONNECT_RETRY=3

# Cache Settings
CACHE_USER_PERMISSION_DURATION=300  # 5 minutes
CACHE_ROLE_PERMISSION_DURATION=600  # 10 minutes
CACHE_WARM_ON_STARTUP=true
CACHE_WARM_BATCH_SIZE=100
```

### Redis Configuration (production.conf)
```conf
# Persistence
save 900 1
save 300 10
save 60 10000

# Memory management
maxmemory 2gb
maxmemory-policy allkeys-lru

# Performance
tcp-keepalive 60
timeout 300
```

### Docker Compose Update
```yaml
services:
  redis:
    image: redis:7-alpine
    container_name: starter-app-redis
    ports:
      - "6379:6379"
    volumes:
      - redis-data:/data
      - ./redis.conf:/usr/local/etc/redis/redis.conf
    command: redis-server /usr/local/etc/redis/redis.conf
    networks:
      - starter-app-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 30s
      timeout: 3s
      retries: 3
```

---

## 🎯 Final Validation Steps

1. **Performance Testing**
   - Run all integration tests
   - Execute load testing scenarios
   - Verify performance metrics meet targets

2. **Cache Validation**
   - Verify tenant isolation in cache keys
   - Test cache invalidation scenarios
   - Confirm cache warming works

3. **Monitoring Setup**
   - Performance dashboard accessible
   - Health checks returning correct status
   - Metrics being collected

4. **Documentation**
   - Update API documentation
   - Document cache key patterns
   - Add troubleshooting guide

---

## 📚 Additional Resources

### Redis Best Practices
- Use connection pooling
- Implement circuit breaker pattern
- Set appropriate timeouts
- Monitor memory usage

### EF Core Optimization Tips
- Use compiled queries for hot paths
- Implement query result caching
- Use async operations throughout
- Add appropriate database indexes

### Cache Key Naming Conventions
```
permissions:tenant:{tenantId}:user:{userId}
permissions:tenant:{tenantId}:roles
permissions:tenant:{tenantId}:role:{roleId}
```

---

## 🏁 Phase 10 Completion Criteria

✅ **Phase is complete when:**
1. Redis caching fully implemented
2. Permission checks < 10ms (cached)
3. Cache hit ratio > 95%
4. Cache invalidation working correctly
5. Batch loading operational
6. Performance metrics dashboard available
7. All tests passing
8. Documentation updated

## 📝 Notes for Phase 11 (Security & Monitoring)

With caching complete, Phase 11 will focus on:
- Permission audit logging
- Security event monitoring
- Compliance features
- Advanced monitoring and alerting
- Performance analytics dashboard