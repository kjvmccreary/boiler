using WorkflowService.Domain.Dsl;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services.Validation;

public interface IWorkflowGraphValidator
{
    ValidationResult ValidateForPublish(WorkflowDefinitionJson definition);
}

public class WorkflowGraphValidator : IWorkflowGraphValidator
{
    public ValidationResult ValidateForPublish(WorkflowDefinitionJson definition)
        => definition.ValidateForPublish();
}
