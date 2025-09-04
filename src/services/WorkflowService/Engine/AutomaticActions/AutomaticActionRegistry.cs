using System.Collections.Concurrent;

namespace WorkflowService.Engine.AutomaticActions;

/// <summary>
/// Registry that resolves executors by Kind (case-insensitive).
/// </summary>
public interface IAutomaticActionRegistry
{
    IAutomaticActionExecutor? Get(string kind);
    IReadOnlyCollection<string> RegisteredKinds { get; }
}

public class AutomaticActionRegistry : IAutomaticActionRegistry
{
    private readonly ConcurrentDictionary<string, IAutomaticActionExecutor> _map = new(StringComparer.OrdinalIgnoreCase);

    public AutomaticActionRegistry(IEnumerable<IAutomaticActionExecutor> executors)
    {
        foreach (var exec in executors)
        {
            if (string.IsNullOrWhiteSpace(exec.Kind)) continue;
            _map.TryAdd(exec.Kind.Trim(), exec);
        }
    }

    public IAutomaticActionExecutor? Get(string kind)
    {
        if (string.IsNullOrWhiteSpace(kind)) return null;
        _map.TryGetValue(kind.Trim(), out var exec);
        return exec;
    }

    public IReadOnlyCollection<string> RegisteredKinds => _map.Keys.ToList().AsReadOnly();
}
