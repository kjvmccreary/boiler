using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Contracts.Services;
using DTOs.Entities;
using DTOs.Monitoring;

namespace Common.Services;

/// <summary>
/// Service for collecting and analyzing system performance metrics
/// Phase 11 - Enhanced Security and Monitoring
/// </summary>
public interface IMonitoringService
{
    Task RecordRequestMetricsAsync(RequestMetrics metrics);
    Task RecordPermissionCheckMetricsAsync(PermissionCheckMetrics metrics);
    Task<SystemMetrics> GetSystemMetricsAsync(TimeSpan period);
    Task<List<PerformanceAlert>> GetActiveAlertsAsync();
    Task RecordSecurityEventAsync(string eventType, string severity, Dictionary<string, object> details);
}

public class MonitoringService : IMonitoringService
{
    private readonly IDistributedCache _cache;
    private readonly IEnhancedAuditService _auditService;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(
        IDistributedCache cache,
        IEnhancedAuditService auditService,
        ILogger<MonitoringService> logger)
    {
        _cache = cache;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task RecordRequestMetricsAsync(RequestMetrics metrics)
    {
        try
        {
            var key = $"metrics:requests:{DateTime.UtcNow:yyyyMMddHH}";
            var existingData = await _cache.GetStringAsync(key);
            
            var hourlyMetrics = string.IsNullOrEmpty(existingData) 
                ? new HourlyRequestMetrics() 
                : JsonSerializer.Deserialize<HourlyRequestMetrics>(existingData)!;

            hourlyMetrics.TotalRequests++;
            hourlyMetrics.TotalResponseTime += metrics.ResponseTimeMs;
            hourlyMetrics.AverageResponseTime = hourlyMetrics.TotalResponseTime / hourlyMetrics.TotalRequests;

            if (metrics.StatusCode >= 400)
            {
                hourlyMetrics.ErrorCount++;
            }

            if (metrics.ResponseTimeMs > 1000) // Slow request threshold
            {
                hourlyMetrics.SlowRequestCount++;
            }

            // Update cache with 25-hour expiration
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(hourlyMetrics),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(25)
                });

            // Check for performance alerts
            await CheckPerformanceAlertsAsync(hourlyMetrics, metrics);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record request metrics");
        }
    }

    public async Task RecordPermissionCheckMetricsAsync(PermissionCheckMetrics metrics)
    {
        try
        {
            var key = $"metrics:permissions:{DateTime.UtcNow:yyyyMMddHH}";
            var existingData = await _cache.GetStringAsync(key);
            
            var hourlyMetrics = string.IsNullOrEmpty(existingData) 
                ? new HourlyPermissionMetrics() 
                : JsonSerializer.Deserialize<HourlyPermissionMetrics>(existingData)!;

            hourlyMetrics.TotalChecks++;
            hourlyMetrics.TotalCheckTime += metrics.CheckDurationMs;
            hourlyMetrics.AverageCheckTime = hourlyMetrics.TotalCheckTime / hourlyMetrics.TotalChecks;

            if (metrics.CacheHit)
            {
                hourlyMetrics.CacheHits++;
            }

            if (!metrics.Granted)
            {
                hourlyMetrics.DeniedChecks++;
            }

            hourlyMetrics.CacheHitRate = (double)hourlyMetrics.CacheHits / hourlyMetrics.TotalChecks * 100;

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(hourlyMetrics),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(25)
                });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record permission check metrics");
        }
    }

    public async Task<SystemMetrics> GetSystemMetricsAsync(TimeSpan period)
    {
        var endTime = DateTime.UtcNow;
        var startTime = endTime - period;
        var systemMetrics = new SystemMetrics
        {
            Period = period,
            StartTime = startTime,
            EndTime = endTime
        };

        try
        {
            // Collect hourly metrics for the period
            var hours = (int)Math.Ceiling(period.TotalHours);
            var requestMetrics = new List<HourlyRequestMetrics>();
            var permissionMetrics = new List<HourlyPermissionMetrics>();

            for (int i = 0; i < hours; i++)
            {
                var hour = endTime.AddHours(-i);
                var hourKey = hour.ToString("yyyyMMddHH");

                var requestKey = $"metrics:requests:{hourKey}";
                var requestData = await _cache.GetStringAsync(requestKey);
                if (!string.IsNullOrEmpty(requestData))
                {
                    requestMetrics.Add(JsonSerializer.Deserialize<HourlyRequestMetrics>(requestData)!);
                }

                var permissionKey = $"metrics:permissions:{hourKey}";
                var permissionData = await _cache.GetStringAsync(permissionKey);
                if (!string.IsNullOrEmpty(permissionData))
                {
                    permissionMetrics.Add(JsonSerializer.Deserialize<HourlyPermissionMetrics>(permissionData)!);
                }
            }

            // Aggregate metrics
            systemMetrics.TotalRequests = requestMetrics.Sum(rm => rm.TotalRequests);
            systemMetrics.TotalErrors = requestMetrics.Sum(rm => rm.ErrorCount);
            systemMetrics.AverageResponseTime = requestMetrics.Any() 
                ? requestMetrics.Average(rm => rm.AverageResponseTime) 
                : 0;
            systemMetrics.ErrorRate = systemMetrics.TotalRequests > 0 
                ? (double)systemMetrics.TotalErrors / systemMetrics.TotalRequests * 100 
                : 0;

            systemMetrics.TotalPermissionChecks = permissionMetrics.Sum(pm => pm.TotalChecks);
            systemMetrics.PermissionCacheHitRate = permissionMetrics.Any() 
                ? permissionMetrics.Average(pm => pm.CacheHitRate) 
                : 0;
            systemMetrics.PermissionDenialRate = permissionMetrics.Any() && systemMetrics.TotalPermissionChecks > 0
                ? (double)permissionMetrics.Sum(pm => pm.DeniedChecks) / systemMetrics.TotalPermissionChecks * 100
                : 0;

            // Get security metrics from audit service
            var securityMetrics = await _auditService.GetSecurityMetricsAsync(period);
            systemMetrics.SecurityEvents = securityMetrics;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get system metrics for period {Period}", period);
        }

        return systemMetrics;
    }

    public async Task<List<PerformanceAlert>> GetActiveAlertsAsync()
    {
        var alerts = new List<PerformanceAlert>();

        try
        {
            var alertKeys = GetAlertKeys();
            
            foreach (var key in alertKeys)
            {
                var alertData = await _cache.GetStringAsync(key);
                if (!string.IsNullOrEmpty(alertData))
                {
                    var alert = JsonSerializer.Deserialize<PerformanceAlert>(alertData);
                    if (alert != null && !alert.IsResolved)
                    {
                        alerts.Add(alert);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get active alerts");
        }

        return alerts.OrderByDescending(a => a.CreatedAt).ToList();
    }

    public async Task RecordSecurityEventAsync(string eventType, string severity, Dictionary<string, object> details)
    {
        try
        {
            // Record in monitoring metrics
            var key = $"metrics:security:{DateTime.UtcNow:yyyyMMddHH}";
            var existingData = await _cache.GetStringAsync(key);
            
            var hourlyMetrics = string.IsNullOrEmpty(existingData) 
                ? new HourlySecurityMetrics() 
                : JsonSerializer.Deserialize<HourlySecurityMetrics>(existingData)!;

            hourlyMetrics.TotalEvents++;
            
            switch (severity.ToLower())
            {
                case "low":
                    hourlyMetrics.LowSeverityEvents++;
                    break;
                case "medium":
                    hourlyMetrics.MediumSeverityEvents++;
                    break;
                case "high":
                    hourlyMetrics.HighSeverityEvents++;
                    break;
                case "critical":
                    hourlyMetrics.CriticalSeverityEvents++;
                    break;
            }

            if (!hourlyMetrics.EventTypes.ContainsKey(eventType))
            {
                hourlyMetrics.EventTypes[eventType] = 0;
            }
            hourlyMetrics.EventTypes[eventType]++;

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(hourlyMetrics),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(25)
                });

            // Create alert for high/critical severity events
            if (severity.ToLower() is "high" or "critical")
            {
                await CreateSecurityAlertAsync(eventType, severity, details);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to record security event metrics");
        }
    }

    private async Task CheckPerformanceAlertsAsync(HourlyRequestMetrics metrics, RequestMetrics currentRequest)
    {
        var alerts = new List<PerformanceAlert>();

        // High error rate alert
        var errorRate = (double)metrics.ErrorCount / metrics.TotalRequests * 100;
        if (errorRate > 10) // 10% error rate threshold
        {
            alerts.Add(new PerformanceAlert
            {
                Id = Guid.NewGuid().ToString(),
                Type = "HighErrorRate",
                Severity = "High",
                Message = $"Error rate is {errorRate:F1}% (threshold: 10%)",
                Value = errorRate,
                Threshold = 10,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Slow average response time alert
        if (metrics.AverageResponseTime > 2000) // 2 second threshold
        {
            alerts.Add(new PerformanceAlert
            {
                Id = Guid.NewGuid().ToString(),
                Type = "SlowResponseTime",
                Severity = "Medium",
                Message = $"Average response time is {metrics.AverageResponseTime:F0}ms (threshold: 2000ms)",
                Value = metrics.AverageResponseTime,
                Threshold = 2000,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Store alerts
        foreach (var alert in alerts)
        {
            var alertKey = $"alert:{alert.Type}:{DateTime.UtcNow:yyyyMMddHH}";
            await _cache.SetStringAsync(alertKey, JsonSerializer.Serialize(alert),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2)
                });
        }
    }

    private async Task CreateSecurityAlertAsync(string eventType, string severity, Dictionary<string, object> details)
    {
        var alert = new PerformanceAlert
        {
            Id = Guid.NewGuid().ToString(),
            Type = "SecurityEvent",
            Severity = severity,
            Message = $"Security event detected: {eventType}",
            CreatedAt = DateTime.UtcNow,
            Details = details
        };

        var alertKey = $"alert:security:{alert.Id}";
        await _cache.SetStringAsync(alertKey, JsonSerializer.Serialize(alert),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24)
            });
    }

    private List<string> GetAlertKeys()
    {
        // This is a simplified implementation
        // In production, you might use Redis SCAN or maintain an index
        var keys = new List<string>();
        var currentHour = DateTime.UtcNow.ToString("yyyyMMddHH");
        var previousHour = DateTime.UtcNow.AddHours(-1).ToString("yyyyMMddHH");

        keys.Add($"alert:HighErrorRate:{currentHour}");
        keys.Add($"alert:HighErrorRate:{previousHour}");
        keys.Add($"alert:SlowResponseTime:{currentHour}");
        keys.Add($"alert:SlowResponseTime:{previousHour}");

        return keys;
    }
}
