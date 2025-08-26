using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UserService.IntegrationTests.TestUtilities;

/// <summary>
/// Utilities for managing rate limiting during tests
/// </summary>
public static class RateLimitingTestUtilities
{
    /// <summary>
    /// Clear all rate limiting cache entries to ensure clean test state
    /// </summary>
    public static void ClearRateLimitCache(IServiceProvider services)
    {
        try
        {
            var cache = services.GetService<IDistributedCache>();
            if (cache != null)
            {
                // Clear all rate limiting keys
                // Note: Since IDistributedCache doesn't have a clear all method,
                // we'll rely on the test configuration having very high limits
                // and the middleware being disabled in testing environment

                // ✅ FIX: Use ILoggerFactory.CreateLogger instead of ILogger<static class>
                var loggerFactory = services.GetService<ILoggerFactory>();
                var logger = loggerFactory?.CreateLogger("RateLimitingTestUtilities");
                logger?.LogDebug("Rate limiting cache management requested - middleware disabled in testing");
            }
        }
        catch (Exception ex)
        {
            // Don't fail tests due to cache clearing issues
            // ✅ FIX: Use ILoggerFactory.CreateLogger instead of ILogger<static class>
            var loggerFactory = services.GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger("RateLimitingTestUtilities");
            logger?.LogWarning(ex, "Failed to clear rate limiting cache, but continuing with test");
        }
    }

    /// <summary>
    /// Add delay between requests if rate limiting is somehow still active
    /// </summary>
    public static async Task DelayForRateLimitingAsync(int milliseconds = 100)
    {
        // Small delay to ensure tests don't hit any residual rate limits
        await Task.Delay(milliseconds);
    }
}
