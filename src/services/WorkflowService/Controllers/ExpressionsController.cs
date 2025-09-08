using DTOs.Common;
using DTOs.Workflow.Expressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/expressions")]
[Authorize(Policy = "workflow.read")]
public class ExpressionsController : ControllerBase
{
    private readonly IExpressionValidationService _svc;

    public ExpressionsController(IExpressionValidationService svc)
    {
        _svc = svc;
    }

    [HttpPost("validate")]
    public ActionResult<ApiResponseDto<ExpressionValidationResultDto>> ValidateExpression(
        [FromBody] ExpressionValidationRequestDto request)
    {
        // Normalize kind
        request.Kind = (request.Kind ?? "").Trim().ToLowerInvariant();
        if (request.Kind is not ("gateway" or "join"))
        {
            return BadRequest(ApiResponseDto<ExpressionValidationResultDto>.ErrorResult(
                "Kind must be 'gateway' or 'join'"));
        }

        var result = _svc.Validate(request);
        if (!result.Success)
        {
            return Ok(ApiResponseDto<ExpressionValidationResultDto>.SuccessResult(result, "Invalid expression"));
        }

        return Ok(ApiResponseDto<ExpressionValidationResultDto>.SuccessResult(result));
    }
}
