// FILE: src/shared/Common/Exceptions/ValidationException.cs
using DTOs.Common;

namespace Common.Exceptions;

public class ValidationException : Exception
{
    public List<ErrorDto> ValidationErrors { get; }

    public ValidationException(List<ErrorDto> validationErrors)
        : base("One or more validation errors occurred.")
    {
        ValidationErrors = validationErrors;
    }

    public ValidationException(string message)
        : base(message)
    {
        ValidationErrors = new List<ErrorDto> { new("VALIDATION_ERROR", message) };
    }

    public ValidationException(string field, string message)
        : base(message)
    {
        ValidationErrors = new List<ErrorDto> { new("VALIDATION_ERROR", message, field) };
    }
}
