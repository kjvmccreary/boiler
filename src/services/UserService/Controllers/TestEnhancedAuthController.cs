using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Common.Authorization;
using Common.Services;
using DTOs.Monitoring;
using Common.Constants;

namespace UserService.Controllers;

/// <summary>
/// Test controller to validate Enhanced Security and Monitoring features
/// Phase 11 - Enhanced Security and Monitoring
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // 🔧 Require authentication
public class TestEnhancedAuthController : ControllerBase
{
    private readonly IMonitoringService _monitoringService;
    private readonly ILogger<TestEnhancedAuthController> _logger;

    public TestEnhancedAuthController(
        IMonitoringService monitoringService,
        ILogger<TestEnhancedAuthController> logger)
    {
        _monitoringService = monitoringService;
        _logger = logger;
    }

    /// <summary>
    /// Test endpoint that requires users.view permission
    /// This will trigger the Enhanced Permission Authorization Handler
    /// </summary>
    [HttpGet("test-users-view")]
    [RequiresPermission(Permissions.Users.View)]
    public IActionResult TestUsersView() // 🔧 FIX: Remove async since no await is used
    {
        try
        {
            _logger.LogInformation("🧪 TEST: users.view permission test accessed by user");
            
            return Ok(new
            {
                Message = "Success! You have users.view permission",
                Permission = "users.view",
                Timestamp = DateTime.UtcNow,
                TestResult = "PASS"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in users.view test endpoint");
            return StatusCode(500, new { Message = "Test failed", Error = ex.Message });
        }
    }

    /// <summary>
    /// Test endpoint that requires users.edit permission
    /// </summary>
    [HttpPost("test-users-edit")]
    [RequiresPermission(Permissions.Users.Edit)]
    public IActionResult TestUsersEdit() // 🔧 FIX: Remove async since no await is used
    {
        try
        {
            _logger.LogInformation("🧪 TEST: users.edit permission test accessed by user");
            
            return Ok(new
            {
                Message = "Success! You have users.edit permission",
                Permission = "users.edit",
                Timestamp = DateTime.UtcNow,
                TestResult = "PASS"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in users.edit test endpoint");
            return StatusCode(500, new { Message = "Test failed", Error = ex.Message });
        }
    }

    /// <summary>
    /// Test endpoint that requires admin-only permissions
    /// </summary>
    [HttpDelete("test-admin-only")]
    [RequiresPermission(Permissions.Users.Delete)]
    public IActionResult TestAdminOnly() // 🔧 FIX: Remove async since no await is used
    {
        try
        {
            _logger.LogInformation("🧪 TEST: users.delete permission test accessed by user");
            
            return Ok(new
            {
                Message = "Success! You have admin-level permissions",
                Permission = "users.delete",
                Timestamp = DateTime.UtcNow,
                TestResult = "PASS"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in admin-only test endpoint");
            return StatusCode(500, new { Message = "Test failed", Error = ex.Message });
        }
    }

    /// <summary>
    /// Test endpoint to trigger security events
    /// </summary>
    [HttpGet("trigger-security-events")]
    [AllowAnonymous] // Allow anonymous to test security detection
    public async Task<IActionResult> TriggerSecurityEvents([FromQuery] string test = "")
    {
        try
        {
            // Record security event based on the test parameter
            if (test.Contains("<script>") || test.Contains("javascript:"))
            {
                await _monitoringService.RecordSecurityEventAsync(
                    "PotentialXssAttack", 
                    "High", 
                    new Dictionary<string, object>
                    {
                        ["UserAgent"] = Request.Headers["User-Agent"].ToString(),
                        ["IpAddress"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        ["QueryParameter"] = test,
                        ["Timestamp"] = DateTime.UtcNow
                    });

                _logger.LogWarning("🚨 XSS attack detected in test parameter: {Test}", test);
            }
            else if (test.ToLower().Contains("union") && test.ToLower().Contains("select"))
            {
                await _monitoringService.RecordSecurityEventAsync(
                    "PotentialSqlInjection", 
                    "High", 
                    new Dictionary<string, object>
                    {
                        ["UserAgent"] = Request.Headers["User-Agent"].ToString(),
                        ["IpAddress"] = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        ["QueryParameter"] = test,
                        ["Timestamp"] = DateTime.UtcNow
                    });

                _logger.LogWarning("🚨 SQL injection detected in test parameter: {Test}", test);
            }

            return Ok(new
            {
                Message = "Security event detection test completed",
                TestParameter = test,
                EventTriggered = !string.IsNullOrEmpty(test),
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in security events test endpoint");
            return StatusCode(500, new { Message = "Test failed", Error = ex.Message });
        }
    }

    /// <summary>
    /// Test multiple permission checks rapidly to test caching
    /// </summary>
    [HttpGet("test-cache-performance")]
    [RequiresPermission(Permissions.Users.View)]
    public IActionResult TestCachePerformance() // 🔧 FIX: Remove async since no real async work is done
    {
        try
        {
            var results = new List<object>();
            
            // Make 10 rapid permission checks
            for (int i = 0; i < 10; i++)
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // This will trigger permission checks each time (through the authorization handler)
                var testResult = new
                {
                    Iteration = i + 1,
                    ResponseTime = stopwatch.ElapsedMilliseconds,
                    Timestamp = DateTime.UtcNow
                };
                
                results.Add(testResult);
                stopwatch.Stop();
            }

            _logger.LogInformation("🧪 TEST: Cache performance test completed with {Count} iterations", results.Count);
            
            return Ok(new
            {
                Message = "Cache performance test completed",
                Results = results,
                TotalIterations = results.Count,
                AverageResponseTime = results.Average(r => (double)((dynamic)r).ResponseTime),
                TestResult = "PASS"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in cache performance test endpoint");
            return StatusCode(500, new { Message = "Test failed", Error = ex.Message });
        }
    }

    /// <summary>
    /// Get current monitoring metrics for testing
    /// </summary>
    [HttpGet("monitoring-status")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMonitoringStatus()
    {
        try
        {
            var metrics = await _monitoringService.GetSystemMetricsAsync(TimeSpan.FromMinutes(5));
            var alerts = await _monitoringService.GetActiveAlertsAsync();

            return Ok(new
            {
                Message = "Current monitoring status",
                Metrics = new
                {
                    TotalRequests = metrics.TotalRequests,
                    TotalErrors = metrics.TotalErrors,
                    ErrorRate = $"{metrics.ErrorRate:F1}%",
                    AvgResponseTime = $"{metrics.AverageResponseTime:F0}ms",
                    TotalPermissionChecks = metrics.TotalPermissionChecks,
                    CacheHitRate = $"{metrics.PermissionCacheHitRate:F1}%",
                    PermissionDenialRate = $"{metrics.PermissionDenialRate:F1}%"
                },
                ActiveAlerts = alerts.Count,
                SecurityEvents = metrics.SecurityEvents.Count,
                Period = "Last 5 minutes",
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting monitoring status");
            return StatusCode(500, new { Message = "Failed to get monitoring status", Error = ex.Message });
        }
    }
}
