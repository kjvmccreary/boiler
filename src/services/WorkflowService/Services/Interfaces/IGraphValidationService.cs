using DTOs.Workflow;

namespace WorkflowService.Services.Interfaces;

public interface IGraphValidationService
{
    ValidationResultDto Validate(string jsonDefinition, bool strict);
}
