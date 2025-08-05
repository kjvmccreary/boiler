// FILE: src/shared/DTOs/Common/ErrorDto.cs
namespace DTOs.Common;

public class ErrorDto
{
    public string Code { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string? Field { get; set; }
    public object? Value { get; set; }

    public ErrorDto() { }

    public ErrorDto(string code, string message, string? field = null, object? value = null)
    {
        Code = code;
        Message = message;
        Field = field;
        Value = value;
    }
}
