using Common.Caching;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace UserService.IntegrationTests.TestUtilities;

/// <summary>
/// In-memory implementation of IPermissionCache for integration testing
/// Tracks cache hits/misses and performance metrics for testing
/// </summary>
public class InMemoryPermissionCache : IPermissionCache
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    private readonly ILogger<InMemoryPermissionCache> _logger;
    
    // Performance tracking for tests
    private long _cacheHits = 0;
    private long _cacheMisses = 0;
    private readonly List<long> _operationTimes = new();

    public InMemoryPermissionCache(ILogger<InMemoryPermissionCache> logger)
    {
        _logger = logger;
    }

    public Task<IEnumerable<string>?> GetUserPermissionsAsync(int userId, int tenantId)
    {
        var stopwatch = Stopwatch.StartNew();
        var key = GetUserPermissionKey(userId, tenantId);
        
        if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            Interlocked.Increment(ref _cacheHits);
            _logger.LogDebug("Cache hit for user {UserId} in tenant {TenantId}", userId, tenantId);
            
            stopwatch.Stop();
            lock (_operationTimes)
            {
                _operationTimes.Add(stopwatch.ElapsedMilliseconds);
            }
            
            return Task.FromResult<IEnumerable<string>?>(entry.Value as IEnumerable<string>);
        }
        
        Interlocked.Increment(ref _cacheMisses);
        _logger.LogDebug("Cache miss for user {UserId} in tenant {TenantId}", userId, tenantId);
        
        stopwatch.Stop();
        lock (_operationTimes)
        {
            _operationTimes.Add(stopwatch.ElapsedMilliseconds);
        }
        
        return Task.FromResult<IEnumerable<string>?>(null);
    }

    public Task SetUserPermissionsAsync(int userId, int tenantId, IEnumerable<string> permissions)
    {
        var stopwatch = Stopwatch.StartNew();
        var key = GetUserPermissionKey(userId, tenantId);
        var expiration = DateTime.UtcNow.AddMinutes(5); // 5 minute expiration like Redis
        
        _cache.AddOrUpdate(key, 
            new CacheEntry(permissions.ToList(), expiration),
            (_, _) => new CacheEntry(permissions.ToList(), expiration));
        
        _logger.LogDebug("Cached permissions for user {UserId} in tenant {TenantId}", userId, tenantId);
        
        stopwatch.Stop();
        lock (_operationTimes)
        {
            _operationTimes.Add(stopwatch.ElapsedMilliseconds);
        }
        
        return Task.CompletedTask;
    }

    public Task InvalidateUserPermissionsAsync(int userId, int tenantId)
    {
        var key = GetUserPermissionKey(userId, tenantId);
        _cache.TryRemove(key, out _);
        
        _logger.LogDebug("Invalidated cache for user {UserId} in tenant {TenantId}", userId, tenantId);
        return Task.CompletedTask;
    }

    public Task InvalidateTenantPermissionsAsync(int tenantId)
    {
        var pattern = $"permissions:tenant:{tenantId}:";
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith(pattern)).ToList();
        
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
        
        _logger.LogInformation("Invalidated all caches for tenant {TenantId}, removed {Count} entries", 
            tenantId, keysToRemove.Count);
        
        return Task.CompletedTask;
    }

    public Task<Dictionary<int, IEnumerable<string>>?> GetRolePermissionsAsync(int tenantId)
    {
        var key = GetRolePermissionKey(tenantId);
        
        if (_cache.TryGetValue(key, out var entry) && !entry.IsExpired)
        {
            Interlocked.Increment(ref _cacheHits);
            return Task.FromResult(entry.Value as Dictionary<int, IEnumerable<string>>);
        }
        
        Interlocked.Increment(ref _cacheMisses);
        return Task.FromResult<Dictionary<int, IEnumerable<string>>?>(null);
    }

    public Task SetRolePermissionsAsync(int tenantId, Dictionary<int, IEnumerable<string>> rolePermissions)
    {
        var key = GetRolePermissionKey(tenantId);
        var expiration = DateTime.UtcNow.AddMinutes(10); // 10 minute expiration like Redis
        
        _cache.AddOrUpdate(key,
            new CacheEntry(rolePermissions, expiration),
            (_, _) => new CacheEntry(rolePermissions, expiration));
        
        return Task.CompletedTask;
    }

    public Task InvalidateRolePermissionsAsync(int roleId, int tenantId)
    {
        // Invalidate role cache
        var roleKey = GetRolePermissionKey(tenantId);
        _cache.TryRemove(roleKey, out _);
        
        // Also invalidate all user caches for this tenant
        return InvalidateTenantPermissionsAsync(tenantId);
    }

    public Task RemoveByPatternAsync(string pattern)
    {
        // Convert Redis pattern to .NET pattern (simple implementation)
        var dotnetPattern = pattern.Replace("*", "");
        var keysToRemove = _cache.Keys.Where(k => k.StartsWith(dotnetPattern)).ToList();
        
        foreach (var key in keysToRemove)
        {
            _cache.TryRemove(key, out _);
        }
        
        _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", 
            keysToRemove.Count, pattern);
        
        return Task.CompletedTask;
    }

    #region Performance Metrics for Testing

    public long GetCacheHits() => _cacheHits;
    public long GetCacheMisses() => _cacheMisses;
    public double GetCacheHitRatio() => 
        _cacheHits + _cacheMisses == 0 ? 0 : (double)_cacheHits / (_cacheHits + _cacheMisses) * 100;

    public List<long> GetOperationTimes()
    {
        lock (_operationTimes)
        {
            return new List<long>(_operationTimes);
        }
    }

    public void ResetMetrics()
    {
        _cacheHits = 0;
        _cacheMisses = 0;
        lock (_operationTimes)
        {
            _operationTimes.Clear();
        }
    }

    public int GetCacheSize() => _cache.Count;

    #endregion

    #region Private Helpers

    private string GetUserPermissionKey(int userId, int tenantId)
    {
        return $"permissions:tenant:{tenantId}:user:{userId}";
    }

    private string GetRolePermissionKey(int tenantId)
    {
        return $"permissions:tenant:{tenantId}:roles";
    }

    private class CacheEntry
    {
        public object Value { get; }
        public DateTime ExpirationTime { get; }
        public bool IsExpired => DateTime.UtcNow > ExpirationTime;

        public CacheEntry(object value, DateTime expirationTime)
        {
            Value = value;
            ExpirationTime = expirationTime;
        }
    }

    #endregion
}
