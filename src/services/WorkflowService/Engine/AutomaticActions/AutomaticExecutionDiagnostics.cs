using System.Collections.Concurrent;

namespace WorkflowService.Engine.AutomaticActions;

/// <summary>
/// In-memory diagnostics to verify AutomaticExecutor invocation during tests.
/// Not intended for production telemetry persistence.
/// </summary>
public static class AutomaticExecutionDiagnostics
{
    private static readonly ConcurrentDictionary<int, List<string>> _starts = new();
    private static readonly ConcurrentDictionary<int, List<string>> _completes = new();

    public static void RecordStart(int instanceId, string nodeId)
    {
        var list = _starts.GetOrAdd(instanceId, _ => new List<string>());
        lock (list) list.Add(nodeId);
    }

    public static void RecordComplete(int instanceId, string nodeId)
    {
        var list = _completes.GetOrAdd(instanceId, _ => new List<string>());
        lock (list) list.Add(nodeId);
    }

    public static IReadOnlyList<string> GetStarts(int instanceId) =>
        _starts.TryGetValue(instanceId, out var list) ? list.AsReadOnly() : Array.Empty<string>();

    public static IReadOnlyList<string> GetCompletes(int instanceId) =>
        _completes.TryGetValue(instanceId, out var list) ? list.AsReadOnly() : Array.Empty<string>();

    public static string Dump()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("=== AutomaticExecutionDiagnostics ===");
        foreach (var kv in _starts.OrderBy(k => k.Key))
        {
            var completes = GetCompletes(kv.Key);
            sb.AppendLine($"Instance {kv.Key} | Starts: [{string.Join(",", kv.Value)}] | Completes: [{string.Join(",", completes)}]");
        }
        return sb.ToString();
    }

    public static void Reset()
    {
        _starts.Clear();
        _completes.Clear();
    }
}
