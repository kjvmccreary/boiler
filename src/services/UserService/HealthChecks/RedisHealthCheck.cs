using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace UserService.HealthChecks;

/// <summary>
/// Health check for Redis connectivity and performance
/// Tests both basic connectivity and response time
/// </summary>
public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisHealthCheck> _logger;

    public RedisHealthCheck(IConnectionMultiplexer redis, ILogger<RedisHealthCheck> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_redis.IsConnected)
            {
                return HealthCheckResult.Unhealthy("Redis is not connected");
            }

            var database = _redis.GetDatabase();
            var testKey = $"health_check_{Guid.NewGuid():N}";
            var testValue = DateTime.UtcNow.ToString("O");
            
            // Test write operation
            var writeStopwatch = System.Diagnostics.Stopwatch.StartNew();
            await database.StringSetAsync(testKey, testValue, TimeSpan.FromSeconds(5));
            writeStopwatch.Stop();
            
            // Test read operation
            var readStopwatch = System.Diagnostics.Stopwatch.StartNew();
            var retrievedValue = await database.StringGetAsync(testKey);
            readStopwatch.Stop();
            
            // Clean up test key
            await database.KeyDeleteAsync(testKey);
            
            if (retrievedValue != testValue)
            {
                return HealthCheckResult.Degraded("Redis read/write test failed - values don't match");
            }
            
            var totalResponseTime = writeStopwatch.ElapsedMilliseconds + readStopwatch.ElapsedMilliseconds;
            
            // Get Redis server info
            var endpoints = _redis.GetEndPoints();
            var serverInfo = new Dictionary<string, object>
            {
                ["endpoints"] = string.Join(", ", endpoints.Select(e => e.ToString())),
                ["is_connected"] = _redis.IsConnected,
                ["write_time_ms"] = writeStopwatch.ElapsedMilliseconds,
                ["read_time_ms"] = readStopwatch.ElapsedMilliseconds,
                ["total_response_time_ms"] = totalResponseTime
            };
            
            // ✅ FIXED: Simplified Redis server info retrieval
            try
            {
                if (endpoints.Length > 0)
                {
                    var server = _redis.GetServer(endpoints[0]);
                    
                    // Get basic server info
                    var info = await server.InfoAsync();
                    if (info != null && info.Any())
                    {
                        // Look for memory information in the server info
                        var memoryInfo = info.Where(group => group.Key.Contains("memory", StringComparison.OrdinalIgnoreCase))
                                            .SelectMany(group => group)
                                            .FirstOrDefault(kvp => kvp.Key.Contains("used_memory", StringComparison.OrdinalIgnoreCase));
                        
                        if (!string.IsNullOrEmpty(memoryInfo.Key))
                        {
                            serverInfo["used_memory"] = memoryInfo.Value;
                        }
                        
                        // Get connected clients count
                        var clientsInfo = info.Where(group => group.Key.Contains("clients", StringComparison.OrdinalIgnoreCase))
                                             .SelectMany(group => group)
                                             .FirstOrDefault(kvp => kvp.Key.Contains("connected_clients", StringComparison.OrdinalIgnoreCase));
                        
                        if (!string.IsNullOrEmpty(clientsInfo.Key))
                        {
                            serverInfo["connected_clients"] = clientsInfo.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Could not retrieve Redis server info (this is normal for some Redis configurations)");
                serverInfo["server_info_note"] = "Server info not available";
            }
            
            // Determine health status based on response time
            if (totalResponseTime > 1000) // > 1 second is concerning
            {
                return HealthCheckResult.Degraded(
                    $"Redis is responding slowly ({totalResponseTime}ms)",
                    data: serverInfo);
            }
            
            if (totalResponseTime > 100) // > 100ms is warning
            {
                return HealthCheckResult.Healthy(
                    $"Redis is healthy but response time is elevated ({totalResponseTime}ms)",
                    data: serverInfo);
            }
            
            return HealthCheckResult.Healthy(
                $"Redis is healthy (response time: {totalResponseTime}ms)",
                data: serverInfo);
        }
        catch (RedisConnectionException ex)
        {
            _logger.LogError(ex, "Redis connection failed during health check");
            return HealthCheckResult.Unhealthy(
                "Redis connection failed",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["is_connected"] = _redis.IsConnected
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Redis health check failed with unexpected error");
            return HealthCheckResult.Unhealthy(
                "Redis health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["error"] = ex.Message,
                    ["error_type"] = ex.GetType().Name
                });
        }
    }
}
