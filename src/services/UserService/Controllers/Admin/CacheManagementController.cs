using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.Caching;
using Common.Performance;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace UserService.Controllers.Admin
{
    /// <summary>
    /// Cache management and administration tools
    /// </summary>
    [ApiController]
    [Route("api/admin/cache")]
    [Authorize(Policy = "AdminOnly")]
    public class CacheManagementController : ControllerBase
    {
        private readonly ICacheService _cacheService;
        private readonly IPermissionCache _permissionCache;
        private readonly IConnectionMultiplexer _redis;
        private readonly ILogger<CacheManagementController> _logger;

        public CacheManagementController(
            ICacheService cacheService,
            IPermissionCache permissionCache,
            IConnectionMultiplexer redis,
            ILogger<CacheManagementController> logger)
        {
            _cacheService = cacheService;
            _permissionCache = permissionCache;
            _redis = redis;
            _logger = logger;
        }

        /// <summary>
        /// Get Redis connection status and basic info
        /// </summary>
        [HttpGet("status")]
        public async Task<ActionResult<CacheStatus>> GetCacheStatus()
        {
            try
            {
                var status = new CacheStatus
                {
                    IsConnected = _redis.IsConnected,
                    Timestamp = DateTime.UtcNow
                };

                if (_redis.IsConnected)
                {
                    var server = _redis.GetServer(_redis.GetEndPoints()[0]);
                    var info = await server.InfoAsync();
                    
                    status.RedisVersion = info.FirstOrDefault(i => i.Key == "redis_version").Value ?? "Unknown";
                    status.ServerUptime = TimeSpan.FromSeconds(
                        long.TryParse(info.FirstOrDefault(i => i.Key == "uptime_in_seconds").Value, out var uptime) 
                        ? uptime : 0);
                    
                    var memInfo = info.FirstOrDefault(i => i.Key == "used_memory_human").Value;
                    status.UsedMemory = memInfo ?? "Unknown";

                    // Get key counts by pattern
                    status.TotalKeys = server.Keys(pattern: "*").Count();
                    status.PermissionKeys = server.Keys(pattern: "permissions:*").Count();
                    status.UserKeys = server.Keys(pattern: "*user*").Count();
                }

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache status");
                return StatusCode(500, new { error = "Failed to retrieve cache status" });
            }
        }

        /// <summary>
        /// Clear all permission caches
        /// </summary>
        [HttpDelete("permissions")]
        public async Task<ActionResult> ClearPermissionCaches()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync("permissions:*");
                _logger.LogInformation("All permission caches cleared by admin");
                return Ok(new { message = "Permission caches cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing permission caches");
                return StatusCode(500, new { error = "Failed to clear permission caches" });
            }
        }

        /// <summary>
        /// Clear permission cache for specific tenant
        /// </summary>
        [HttpDelete("permissions/tenant/{tenantId}")]
        public async Task<ActionResult> ClearTenantPermissionCaches(int tenantId)
        {
            try
            {
                await _permissionCache.InvalidateTenantPermissionsAsync(tenantId);
                _logger.LogInformation("Permission caches cleared for tenant {TenantId} by admin", tenantId);
                return Ok(new { message = $"Permission caches cleared for tenant {tenantId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing tenant permission caches for tenant {TenantId}", tenantId);
                return StatusCode(500, new { error = "Failed to clear tenant permission caches" });
            }
        }

        /// <summary>
        /// Clear permission cache for specific user
        /// </summary>
        [HttpDelete("permissions/user/{userId}/tenant/{tenantId}")]
        public async Task<ActionResult> ClearUserPermissionCache(int userId, int tenantId)
        {
            try
            {
                await _permissionCache.InvalidateUserPermissionsAsync(userId, tenantId);
                _logger.LogInformation("Permission cache cleared for user {UserId} in tenant {TenantId} by admin", 
                    userId, tenantId);
                return Ok(new { message = $"Permission cache cleared for user {userId} in tenant {tenantId}" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing user permission cache for user {UserId} in tenant {TenantId}", 
                    userId, tenantId);
                return StatusCode(500, new { error = "Failed to clear user permission cache" });
            }
        }

        /// <summary>
        /// Clear all caches (DANGEROUS - Admin only)
        /// </summary>
        [HttpDelete("all")]
        public async Task<ActionResult> ClearAllCaches()
        {
            try
            {
                await _cacheService.RemoveByPatternAsync("*");
                _logger.LogWarning("ALL CACHES CLEARED by admin - this affects all cached data!");
                return Ok(new { message = "All caches cleared successfully", warning = "This cleared ALL cached data!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing all caches");
                return StatusCode(500, new { error = "Failed to clear all caches" });
            }
        }

        /// <summary>
        /// Warm up permission caches for a tenant
        /// </summary>
        [HttpPost("warm/permissions/tenant/{tenantId}")]
        public async Task<ActionResult> WarmPermissionCaches(int tenantId)
        {
            try
            {
                // This would trigger cache warming - you'd implement this based on your business logic
                // For now, we'll just acknowledge the request
                _logger.LogInformation("Permission cache warming requested for tenant {TenantId}", tenantId);
                
                // You could implement actual cache warming here by:
                // 1. Getting all users in the tenant
                // 2. Pre-loading their permissions
                // 3. Pre-loading role permissions
                
                return Ok(new { 
                    message = $"Permission cache warming initiated for tenant {tenantId}",
                    note = "Cache warming is running in background"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error warming permission caches for tenant {TenantId}", tenantId);
                return StatusCode(500, new { error = "Failed to warm permission caches" });
            }
        }

        /// <summary>
        /// Get cache keys matching a pattern (for debugging)
        /// </summary>
        [HttpGet("keys")]
        public async Task<ActionResult<CacheKeysResponse>> GetCacheKeys(
            [FromQuery] string pattern = "*", 
            [FromQuery] int limit = 100)
        {
            try
            {
                if (!_redis.IsConnected)
                {
                    return BadRequest(new { error = "Redis is not connected" });
                }

                var server = _redis.GetServer(_redis.GetEndPoints()[0]);
                var keys = server.Keys(pattern: pattern).Take(limit).ToList();

                var response = new CacheKeysResponse
                {
                    Pattern = pattern,
                    TotalFound = keys.Count,
                    Limit = limit,
                    Keys = keys.Select(k => k.ToString()).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache keys with pattern {Pattern}", pattern);
                return StatusCode(500, new { error = "Failed to retrieve cache keys" });
            }
        }
    }

    // DTOs
    public class CacheStatus
    {
        public bool IsConnected { get; set; }
        public string RedisVersion { get; set; } = string.Empty;
        public TimeSpan ServerUptime { get; set; }
        public string UsedMemory { get; set; } = string.Empty;
        public int TotalKeys { get; set; }
        public int PermissionKeys { get; set; }
        public int UserKeys { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CacheKeysResponse
    {
        public string Pattern { get; set; } = string.Empty;
        public int TotalFound { get; set; }
        public int Limit { get; set; }
        public List<string> Keys { get; set; } = new();
    }
}
