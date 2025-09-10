using WorkflowService.Services.Interfaces;
using System.Text.Json;

namespace WorkflowService.Services;

public class ExpressionService : IExpressionService
{
    // Simple in‑memory examples; extend later
    private static readonly Dictionary<string, string[]> _kindVars = new(StringComparer.OrdinalIgnoreCase)
    {
        { "gateway", new[] { "user.id", "tenant.id", "task.status", "instance.status" } },
        { "join", new[] { "branch.count", "arrivals", "threshold" } },
        { "task-assignment", new[] { "user.roleCodes", "candidate.users", "candidate.roles" } }
    };

    public Task<(IEnumerable<string> variables, bool fromCache)> GetVariablesAsync(string kind, CancellationToken ct)
    {
        if (!_kindVars.TryGetValue(kind ?? "gateway", out var vars))
            vars = _kindVars["gateway"];
        return Task.FromResult(((IEnumerable<string>)vars, false));
    }

    public Task<(bool success, List<string> errors, List<string> warnings, TimeSpan duration)> ValidateAsync(
        string kind,
        string expression,
        CancellationToken ct)
    {
        var started = DateTime.UtcNow;

        if (string.IsNullOrWhiteSpace(expression))
            return Task.FromResult((true, new List<string>(), new List<string>(), DateTime.UtcNow - started));

        // Lightweight JSON parse check (since editor sends JsonLogic JSON)
        try
        {
            using var doc = JsonDocument.Parse(expression);
            // Example heuristic: Disallow huge root object
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
                return Task.FromResult((false, new List<string> { "Expression must be a JSON object" }, new List<string>(), DateTime.UtcNow - started));
        }
        catch (Exception ex)
        {
            return Task.FromResult((false, new List<string> { $"Invalid JSON: {ex.Message}" }, new List<string>(), DateTime.UtcNow - started));
        }

        return Task.FromResult((true, new List<string>(), new List<string>(), DateTime.UtcNow - started));
    }
}
