// FILE: src/shared/DTOs/Common/ApiResponseDto.cs
namespace DTOs.Common;

public class ApiResponseDto<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<ErrorDto> Errors { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? TraceId { get; set; }

    public static ApiResponseDto<T> SuccessResult(T data, string message = "")
    {
        return new ApiResponseDto<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    public static ApiResponseDto<T> ErrorResult(string message, List<ErrorDto>? errors = null)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<ErrorDto>()
        };
    }

    public static ApiResponseDto<T> ErrorResult(List<ErrorDto> errors)
    {
        return new ApiResponseDto<T>
        {
            Success = false,
            Message = "Validation failed",
            Errors = errors
        };
    }
}
