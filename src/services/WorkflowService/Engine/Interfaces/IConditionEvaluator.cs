namespace WorkflowService.Engine.Interfaces;

/// <summary>
/// Evaluates conditions using JsonLogic
/// </summary>
public interface IConditionEvaluator
{
    /// <summary>
    /// Evaluate a JsonLogic condition against context data
    /// </summary>
    Task<bool> EvaluateAsync(string condition, string contextData);

    /// <summary>
    /// Evaluate a condition and return the result value
    /// </summary>
    Task<object?> EvaluateExpressionAsync(string expression, string contextData);

    /// <summary>
    /// Validate that a condition string is valid JsonLogic
    /// </summary>
    bool ValidateCondition(string condition);
}
