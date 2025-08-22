using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.Performance;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Common.Constants; // ✅ add

namespace UserService.Controllers.Admin
{
    /// <summary>
    /// Performance monitoring and metrics dashboard
    /// </summary>
    [ApiController]
    [Route("api/admin/performance")]
    [Authorize(Policy = "RedisMonitoring")]
    public class PerformanceController : ControllerBase
    {
        private readonly IPerformanceMetricsService _metricsService;
        private readonly ILogger<PerformanceController> _logger;

        public PerformanceController(
            IPerformanceMetricsService metricsService,
            ILogger<PerformanceController> logger)
        {
            _metricsService = metricsService;
            _logger = logger;
        }

        /// <summary>
        /// Get comprehensive performance dashboard data
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<PerformanceDashboard>> GetDashboard([FromQuery] int? periodHours = null)
        {
            try
            {
                TimeSpan? period = periodHours.HasValue ? TimeSpan.FromHours(periodHours.Value) : null;

                var dashboard = new PerformanceDashboard
                {
                    CacheMetrics = await _metricsService.GetCacheMetricsAsync(period),
                    ApiMetrics = await _metricsService.GetApiMetricsAsync(period),
                    SystemMetrics = await _metricsService.GetSystemMetricsAsync(),
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance dashboard");
                return StatusCode(500, new { error = "Failed to retrieve performance data" });
            }
        }

        /// <summary>
        /// Get cache-specific metrics
        /// </summary>
        [HttpGet("cache")]
        public async Task<ActionResult<CacheMetrics>> GetCacheMetrics([FromQuery] int? periodHours = null)
        {
            try
            {
                TimeSpan? period = periodHours.HasValue ? TimeSpan.FromHours(periodHours.Value) : null;
                var metrics = await _metricsService.GetCacheMetricsAsync(period);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cache metrics");
                return StatusCode(500, new { error = "Failed to retrieve cache metrics" });
            }
        }

        /// <summary>
        /// Get API performance metrics
        /// </summary>
        [HttpGet("api")]
        public async Task<ActionResult<ApiMetrics>> GetApiMetrics([FromQuery] int? periodHours = null)
        {
            try
            {
                TimeSpan? period = periodHours.HasValue ? TimeSpan.FromHours(periodHours.Value) : null;
                var metrics = await _metricsService.GetApiMetricsAsync(period);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving API metrics");
                return StatusCode(500, new { error = "Failed to retrieve API metrics" });
            }
        }

        /// <summary>
        /// Get system health and Redis metrics
        /// </summary>
        [HttpGet("system")]
        public async Task<ActionResult<SystemMetrics>> GetSystemMetrics()
        {
            try
            {
                var metrics = await _metricsService.GetSystemMetricsAsync();
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system metrics");
                return StatusCode(500, new { error = "Failed to retrieve system metrics" });
            }
        }

        /// <summary>
        /// Get performance summary statistics
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<PerformanceSummary>> GetSummary([FromQuery] int? periodHours = null)
        {
            try
            {
                TimeSpan? period = periodHours.HasValue ? TimeSpan.FromHours(periodHours.Value) : null;
                
                var cacheMetrics = await _metricsService.GetCacheMetricsAsync(period);
                var apiMetrics = await _metricsService.GetApiMetricsAsync(period);
                var systemMetrics = await _metricsService.GetSystemMetricsAsync();

                var summary = new PerformanceSummary
                {
                    CacheHitRatio = cacheMetrics.HitRatio,
                    TotalCacheRequests = cacheMetrics.TotalRequests,
                    AverageCacheHitTime = cacheMetrics.AverageHitTime,
                    AverageCacheMissTime = cacheMetrics.AverageMissTime,
                    TotalApiRequests = apiMetrics.TotalRequests,
                    AverageApiResponseTime = apiMetrics.AverageResponseTime,
                    RedisConnected = systemMetrics.RedisConnected,
                    SystemUptime = systemMetrics.Uptime,
                    Period = period,
                    GeneratedAt = DateTime.UtcNow
                };

                return Ok(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving performance summary");
                return StatusCode(500, new { error = "Failed to retrieve performance summary" });
            }
        }

        /// <summary>
        /// Clear all performance metrics (Admin only)
        /// </summary>
        [HttpDelete("clear")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<ActionResult> ClearMetrics()
        {
            try
            {
                await _metricsService.ClearMetricsAsync();
                _logger.LogInformation("Performance metrics cleared by admin");
                return Ok(new { message = "Performance metrics cleared successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clearing performance metrics");
                return StatusCode(500, new { error = "Failed to clear performance metrics" });
            }
        }

        /// <summary>
        /// Get top performing and underperforming endpoints
        /// </summary>
        [HttpGet("endpoints/analysis")]
        public async Task<ActionResult<EndpointAnalysis>> GetEndpointAnalysis([FromQuery] int? periodHours = null)
        {
            try
            {
                TimeSpan? period = periodHours.HasValue ? TimeSpan.FromHours(periodHours.Value) : null;
                var apiMetrics = await _metricsService.GetApiMetricsAsync(period);

                var analysis = new EndpointAnalysis();

                foreach (var endpoint in apiMetrics.ByEndpoint)
                {
                    var endpointData = new EndpointPerformanceData
                    {
                        Endpoint = endpoint.Key,
                        RequestCount = endpoint.Value.RequestCount,
                        AverageResponseTime = endpoint.Value.AverageResponseTime,
                        MinResponseTime = endpoint.Value.MinResponseTime,
                        MaxResponseTime = endpoint.Value.MaxResponseTime
                    };

                    analysis.AllEndpoints.Add(endpointData);

                    // Categorize endpoints
                    if (endpoint.Value.AverageResponseTime.TotalMilliseconds < 50)
                        analysis.FastEndpoints.Add(endpointData);
                    else if (endpoint.Value.AverageResponseTime.TotalMilliseconds > 500)
                        analysis.SlowEndpoints.Add(endpointData);

                    if (endpoint.Value.RequestCount > 100)
                        analysis.HighTrafficEndpoints.Add(endpointData);
                }

                // Sort lists
                analysis.FastEndpoints.Sort((a, b) => a.AverageResponseTime.CompareTo(b.AverageResponseTime));
                analysis.SlowEndpoints.Sort((a, b) => b.AverageResponseTime.CompareTo(a.AverageResponseTime));
                analysis.HighTrafficEndpoints.Sort((a, b) => b.RequestCount.CompareTo(a.RequestCount));

                return Ok(analysis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error performing endpoint analysis");
                return StatusCode(500, new { error = "Failed to analyze endpoint performance" });
            }
        }
    }

    // DTO Classes
    public class PerformanceDashboard
    {
        public CacheMetrics CacheMetrics { get; set; } = new();
        public ApiMetrics ApiMetrics { get; set; } = new();
        public SystemMetrics SystemMetrics { get; set; } = new();
        public DateTime GeneratedAt { get; set; }
    }

    public class PerformanceSummary
    {
        public double CacheHitRatio { get; set; }
        public int TotalCacheRequests { get; set; }
        public TimeSpan AverageCacheHitTime { get; set; }
        public TimeSpan AverageCacheMissTime { get; set; }
        public int TotalApiRequests { get; set; }
        public TimeSpan AverageApiResponseTime { get; set; }
        public bool RedisConnected { get; set; }
        public TimeSpan SystemUptime { get; set; }
        public TimeSpan? Period { get; set; }
        public DateTime GeneratedAt { get; set; }
    }

    public class EndpointAnalysis
    {
        public List<EndpointPerformanceData> AllEndpoints { get; set; } = new();
        public List<EndpointPerformanceData> FastEndpoints { get; set; } = new();
        public List<EndpointPerformanceData> SlowEndpoints { get; set; } = new();
        public List<EndpointPerformanceData> HighTrafficEndpoints { get; set; } = new();
    }

    public class EndpointPerformanceData
    {
        public string Endpoint { get; set; } = string.Empty;
        public int RequestCount { get; set; }
        public TimeSpan AverageResponseTime { get; set; }
        public TimeSpan MinResponseTime { get; set; }
        public TimeSpan MaxResponseTime { get; set; }
    }
}
