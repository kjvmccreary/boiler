using System.Text.Json.Serialization;

namespace Common.Models
{
    /// <summary>
    /// Standardized API response wrapper for consistent JSON structure
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        /// <summary>
        /// The actual data payload (null if error)
        /// </summary>
        [JsonPropertyName("data")]
        public T? Data { get; set; }

        /// <summary>
        /// Error message (null if successful)
        /// </summary>
        [JsonPropertyName("message")]
        public string? Message { get; set; }

        /// <summary>
        /// Additional error details for debugging (only in development)
        /// </summary>
        [JsonPropertyName("errors")]
        public List<string>? Errors { get; set; }

        /// <summary>
        /// Pagination metadata (for paginated responses)
        /// </summary>
        [JsonPropertyName("pagination")]
        public PaginationMetadata? Pagination { get; set; }

        /// <summary>
        /// Request timestamp
        /// </summary>
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Create successful response
        /// </summary>
        public static ApiResponse<T> Ok(T data, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }

        /// <summary>
        /// Create successful response with pagination
        /// </summary>
        public static ApiResponse<T> Ok(T data, PaginationMetadata pagination, string? message = null)
        {
            return new ApiResponse<T>
            {
                Success = true,
                Data = data,
                Message = message,
                Pagination = pagination
            };
        }

        /// <summary>
        /// Create error response
        /// </summary>
        public static ApiResponse<T> Error(string message, List<string>? errors = null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message,
                Errors = errors
            };
        }

        /// <summary>
        /// Create validation error response
        /// </summary>
        public static ApiResponse<T> ValidationError(List<string> errors)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = "Validation failed",
                Errors = errors
            };
        }

        /// <summary>
        /// Create unauthorized response
        /// </summary>
        public static ApiResponse<T> Unauthorized(string message = "Unauthorized access")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message
            };
        }

        /// <summary>
        /// Create forbidden response
        /// </summary>
        public static ApiResponse<T> Forbidden(string message = "Access forbidden")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message
            };
        }

        /// <summary>
        /// Create not found response
        /// </summary>
        public static ApiResponse<T> NotFound(string message = "Resource not found")
        {
            return new ApiResponse<T>
            {
                Success = false,
                Data = default,
                Message = message
            };
        }
    }

    /// <summary>
    /// Non-generic version for responses without data
    /// </summary>
    public class ApiResponse : ApiResponse<object>
    {
        /// <summary>
        /// Create successful response without data
        /// </summary>
        public static ApiResponse Ok(string? message = null)
        {
            return new ApiResponse
            {
                Success = true,
                Message = message
            };
        }

        /// <summary>
        /// Create error response without data
        /// </summary>
        public new static ApiResponse Error(string message, List<string>? errors = null)
        {
            return new ApiResponse
            {
                Success = false,
                Message = message,
                Errors = errors
            };
        }
    }

    /// <summary>
    /// Pagination metadata for paginated API responses
    /// </summary>
    public class PaginationMetadata
    {
        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("totalPages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("totalItems")]
        public long TotalItems { get; set; }

        [JsonPropertyName("hasNext")]
        public bool HasNext => CurrentPage < TotalPages;

        [JsonPropertyName("hasPrevious")]
        public bool HasPrevious => CurrentPage > 1;

        public static PaginationMetadata Create(int currentPage, int pageSize, long totalItems)
        {
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            
            return new PaginationMetadata
            {
                CurrentPage = currentPage,
                PageSize = pageSize,
                TotalPages = totalPages,
                TotalItems = totalItems
            };
        }
    }
}
