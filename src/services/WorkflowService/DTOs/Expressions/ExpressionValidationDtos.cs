using System.Collections.Generic;

namespace DTOs.Workflow.Expressions;

public sealed class ExpressionValidationRequestDto
{
    public string Kind { get; set; } = "";          // "gateway" | "join"
    public string Expression { get; set; } = "";    // Raw JsonLogic JSON string
    public bool Strict { get; set; } = false;       // Future: stricter semantic checks
}

public sealed class ExpressionValidationResultDto
{
    public bool Success { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
    public object? Ast { get; set; }                // Optional: parsed form (can be trimmed later)

    public static ExpressionValidationResultDto Ok(object? ast = null, params string[] warnings)
        => new()
        {
            Success = true,
            Ast = ast,
            Warnings = warnings is { Length: > 0 } ? new List<string>(warnings) : new()
        };

    public static ExpressionValidationResultDto Fail(params string[] errors)
        => new()
        {
            Success = false,
            Errors = errors is { Length: > 0 } ? new List<string>(errors) : new()
        };
}
