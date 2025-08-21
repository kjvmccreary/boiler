using System;
using System.Threading.Tasks;

namespace Common.Caching
{
    /// <summary>
    /// Generic cache service interface for basic caching operations
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Get a cached value by key
        /// </summary>
        /// <typeparam name="T">Type of cached value</typeparam>
        /// <param name="key">Cache key</param>
        /// <returns>Cached value or default(T) if not found</returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Set a value in cache with optional expiration
        /// </summary>
        /// <typeparam name="T">Type of value to cache</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="value">Value to cache</param>
        /// <param name="expiration">Optional expiration time</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);

        /// <summary>
        /// Check if a key exists in cache
        /// </summary>
        /// <param name="key">Cache key</param>
        /// <returns>True if key exists</returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Remove a key from cache
        /// </summary>
        /// <param name="key">Cache key</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Remove multiple keys matching a pattern
        /// </summary>
        /// <param name="pattern">Pattern to match (Redis pattern syntax)</param>
        Task RemoveByPatternAsync(string pattern);

        /// <summary>
        /// Get value from cache or set it using factory function if not found
        /// </summary>
        /// <typeparam name="T">Type of value</typeparam>
        /// <param name="key">Cache key</param>
        /// <param name="factory">Function to create value if not cached</param>
        /// <param name="expiration">Optional expiration time</param>
        /// <returns>Cached or newly created value</returns>
        Task<T> GetOrSetAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null);
    }
}
