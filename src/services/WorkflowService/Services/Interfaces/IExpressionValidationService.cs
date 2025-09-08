using DTOs.Workflow.Expressions;

namespace WorkflowService.Services.Interfaces;

public interface IExpressionValidationService
{
    ExpressionValidationResultDto Validate(ExpressionValidationRequestDto request);
}
