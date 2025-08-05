// FILE: src/shared/Common/Exceptions/BusinessException.cs
namespace Common.Exceptions;

public class BusinessException : Exception
{
    public string Code { get; }
    public object? Details { get; }

    public BusinessException(string code, string message, object? details = null)
        : base(message)
    {
        Code = code;
        Details = details;
    }

    public BusinessException(string code, string message, Exception innerException, object? details = null)
        : base(message, innerException)
    {
        Code = code;
        Details = details;
    }
}

// Common business exceptions
public class EntityNotFoundException : BusinessException
{
    public EntityNotFoundException(string entityType, object id)
        : base("ENTITY_NOT_FOUND", $"{entityType} with ID '{id}' was not found.", new { EntityType = entityType, Id = id })
    {
    }
}
