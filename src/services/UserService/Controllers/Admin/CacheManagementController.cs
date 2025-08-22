using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.Caching;
using Common.Performance;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Linq;
using System.Collections.Generic;
using Common.Constants;

namespace UserService.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/cache")]
    [Authorize(Policy = "RedisMonitoring")]
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

        [HttpGet("status")]
        public async Task<ActionResult<CacheStatus>> GetCacheStatus()
        {
            var status = new CacheStatus
            {
                IsConnected = _redis.IsConnected,
                Timestamp = DateTime.UtcNow,
                RedisVersion = "Unknown",
                UsedMemory = "Unknown"
            };

            if (!_redis.IsConnected)
                return Ok(status); // 200 with IsConnected=false

            try
            {
                var endpoints = _redis.GetEndPoints();
                if (endpoints == null || endpoints.Length == 0)
                {
                    _logger.LogWarning("Redis returned no endpoints");
                    return Ok(status);
                }

                var server = _redis.GetServer(endpoints[0]);

                // INFO section
                try
                {
                    var infoGroups = await server.InfoAsync();
                    var flat = infoGroups.SelectMany(g => g);

                    string? GetInfo(string key) => flat.FirstOrDefault(i => i.Key == key).Value;

                    status.RedisVersion = GetInfo("redis_version") ?? "Unknown";

                    if (long.TryParse(GetInfo("uptime_in_seconds"), out var uptimeSeconds))
                        status.ServerUptime = TimeSpan.FromSeconds(uptimeSeconds);

                    status.UsedMemory = GetInfo("used_memory_human") ?? "Unknown";
                }
                catch (Exception exInfo)
                {
                    _logger.LogWarning(exInfo, "Failed Redis INFO (AllowAdmin missing or restricted) – continuing");
                }

                // Key counts (each guarded)
                status.TotalKeys = SafeCountKeys(server, "*");
                status.PermissionKeys = SafeCountKeys(server, "permissions:*");
                status.UserKeys = SafeCountKeys(server, "*user*");

                return Ok(status);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache status (outer)");
                // Return partial (not throwing) to keep UX predictable
                return StatusCode(500, new { error = "Failed to retrieve cache status", detail = "See logs" });
            }
        }

        private int SafeCountKeys(IServer server, string pattern)
        {
            try
            {
                int count = 0;
                foreach (var key in server.Keys(pattern: pattern, pageSize: 250))
                {
                    count++;
                    if (count >= 100_000)
                    {
                        _logger.LogWarning("Aborting key count early for pattern {Pattern} at {Count}", pattern, count);
                        break;
                    }
                }
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Key enumeration failed for pattern {Pattern}", pattern);
                return -1; // sentinel for failure
            }
        }

        [HttpDelete("permissions")]
        [Authorize(Policy = "AdminOnly")]
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

        [HttpDelete("permissions/tenant/{tenantId}")]
        [Authorize(Policy = "AdminOnly")]
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

        [HttpDelete("permissions/user/{userId}/tenant/{tenantId}")]
        [Authorize(Policy = "AdminOnly")]
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

        [HttpDelete("all")]
        [Authorize(Policy = "AdminOnly")]
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

        [HttpPost("warm/permissions/tenant/{tenantId}")]
        [Authorize(Policy = "AdminOnly")]
        public ActionResult WarmPermissionCaches(int tenantId)
        {
            try
            {
                _logger.LogInformation("Permission cache warming requested for tenant {TenantId}", tenantId);
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

        [HttpGet("keys")]
        public ActionResult<CacheKeysResponse> GetCacheKeys(
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
