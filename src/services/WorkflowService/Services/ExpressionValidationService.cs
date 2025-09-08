using System.Text.Json;
using DTOs.Workflow.Expressions;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

public class ExpressionValidationService : IExpressionValidationService
{
    private static readonly HashSet<string> AllowedTopLevelOps = new()
    {
        "==","!=",">",">=","<","<=","and","or","!","+","-","*","/","var","in","missing","missing_some"
    };

    public ExpressionValidationResultDto Validate(ExpressionValidationRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Expression))
            return ExpressionValidationResultDto.Fail("Expression is required");

        JsonDocument doc;
        try
        {
            doc = JsonDocument.Parse(request.Expression);
        }
        catch (JsonException jx)
        {
            return ExpressionValidationResultDto.Fail($"Invalid JSON: {jx.Message}");
        }

        // Basic shape checks (JsonLogic normally uses an object with single key operator)
        if (doc.RootElement.ValueKind != JsonValueKind.Object)
            return ExpressionValidationResultDto.Fail("Top-level expression must be a JSON object");

        using (doc)
        {
            var obj = doc.RootElement;
            if (!obj.EnumerateObject().MoveNext())
                return ExpressionValidationResultDto.Fail("Expression object is empty");

            var firstProp = obj.EnumerateObject().First();
            var op = firstProp.Name;

            if (!AllowedTopLevelOps.Contains(op))
            {
                return ExpressionValidationResultDto.Fail($"Operator '{op}' is not in allowed set");
            }

            // Gateway vs Join advisory warnings
            var warnings = new List<string>();
            if (request.Kind == "gateway" && (op == "missing" || op == "missing_some"))
            {
                warnings.Add("Using 'missing' operators in gateway conditions may reduce clarity.");
            }
            if (request.Kind == "join" && op == "var")
            {
                warnings.Add("Join expression root is a 'var'; verify this derives arrivals/context as intended.");
            }

            return ExpressionValidationResultDto.Ok(ast: new
            {
                operatorName = op,
                arity = firstProp.Value.ValueKind == JsonValueKind.Array
                    ? firstProp.Value.GetArrayLength()
                    : 1
            }, warnings: warnings.ToArray());
        }
    }
}
