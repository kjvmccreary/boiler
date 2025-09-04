using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Options;
using WorkflowService.Engine.Diagnostics;

namespace WorkflowService.Engine.AutomaticActions;

/// <summary>
/// Ring buffer for recent Automatic executor diagnostic entries.
/// </summary>
public interface IAutomaticDiagnosticsBuffer
{
    void Record(object payload);
    IReadOnlyList<string> Snapshot();
    void Clear();
}

public class AutomaticDiagnosticsBuffer : IAutomaticDiagnosticsBuffer
{
    private readonly int _capacity;
    private readonly ConcurrentQueue<string> _queue = new();

    public AutomaticDiagnosticsBuffer(IOptions<WorkflowDiagnosticsOptions> opts)
    {
        _capacity = Math.Max(10, opts.Value.AutomaticBufferSize);
    }

    public void Record(object payload)
    {
        var json = JsonSerializer.Serialize(new
        {
            at = DateTime.UtcNow,
            type = "automatic",
            data = payload
        });

        _queue.Enqueue(json);
        while (_queue.Count > _capacity && _queue.TryDequeue(out _)) { }
    }

    public IReadOnlyList<string> Snapshot() => _queue.ToList().AsReadOnly();

    public void Clear()
    {
        while (_queue.TryDequeue(out _)) { }
    }
}
