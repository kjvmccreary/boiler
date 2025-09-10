namespace WorkflowService.Services.Interfaces;

public interface IExpressionService
{
    Task<(IEnumerable<string> variables, bool fromCache)> GetVariablesAsync(string kind, CancellationToken ct);
    Task<(bool success, List<string> errors, List<string> warnings, TimeSpan duration)> ValidateAsync(string kind, string expression, CancellationToken ct);
}
