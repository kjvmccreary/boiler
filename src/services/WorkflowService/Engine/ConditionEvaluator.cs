using System.Text.Json;
using JsonLogic.Net;
using WorkflowService.Engine.Interfaces;
using Newtonsoft.Json.Linq;

namespace WorkflowService.Engine;

/// <summary>
/// JsonLogic-based condition evaluator
/// </summary>
public class JsonLogicConditionEvaluator : IConditionEvaluator
{
    private readonly ILogger<JsonLogicConditionEvaluator> _logger;
    private readonly JsonLogicEvaluator _evaluator;

    public JsonLogicConditionEvaluator(ILogger<JsonLogicConditionEvaluator> logger)
    {
        _logger = logger;
        _evaluator = new JsonLogicEvaluator(EvaluateOperators.Default);
    }

    public async Task<bool> EvaluateAsync(string condition, string contextData)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(condition))
                return true; // Empty condition is always true

            var result = await EvaluateExpressionAsync(condition, contextData);
            
            // Convert result to boolean
            return result switch
            {
                bool b => b,
                null => false,
                string s => !string.IsNullOrEmpty(s),
                int i => i != 0,
                double d => d != 0.0,
                _ => true
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating condition: {Condition}", condition);
            return false;
        }
    }

    public async Task<object?> EvaluateExpressionAsync(string expression, string contextData)
    {
        try
        {
            // Parse context data as JToken (Newtonsoft.Json for JsonLogic compatibility)
            var context = JToken.Parse(contextData);
            
            // Parse and evaluate expression as JToken
            var rule = JToken.Parse(expression);
            
            return await Task.Run(() => _evaluator.Apply(rule, context));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error evaluating expression: {Expression}", expression);
            throw;
        }
    }

    public bool ValidateCondition(string condition)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(condition))
                return true;

            // Try to parse as JSON using Newtonsoft.Json (compatible with JsonLogic)
            JToken.Parse(condition);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
