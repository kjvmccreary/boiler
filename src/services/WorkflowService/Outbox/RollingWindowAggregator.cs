namespace WorkflowService.Outbox;

internal sealed class RollingWindowAggregator
{
    private readonly TimeSpan _window;
    private readonly object _lock = new();
    // Store (timestamp, fetched, processed, failed, giveup)
    private readonly LinkedList<(DateTime ts, int f, int p, int fl, int g)> _cycles = new();

    public RollingWindowAggregator(TimeSpan window) => _window = window;

    public void Add(DateTime timestampUtc, int fetched, int processed, int failed, int giveUp)
    {
        lock (_lock)
        {
            _cycles.AddLast((timestampUtc, fetched, processed, failed, giveUp));
            Trim(timestampUtc);
        }
    }

    private void Trim(DateTime nowUtc)
    {
        while (_cycles.First != null && (nowUtc - _cycles.First!.Value.ts) > _window)
            _cycles.RemoveFirst();
    }

    public (int processed, int failed, int giveUp, int fetched, double minutes) Snapshot()
    {
        lock (_lock)
        {
            if (_cycles.Count == 0) return (0, 0, 0, 0, 0);
            var first = _cycles.First!.Value.ts;
            var last = _cycles.Last!.Value.ts;
            double minutes = Math.Max(0.001, (last - first).TotalMinutes);
            int processed = 0, failed = 0, giveUp = 0, fetched = 0;
            foreach (var c in _cycles)
            {
                processed += c.p;
                failed += c.fl;
                giveUp += c.g;
                fetched += c.f;
            }
            return (processed, failed, giveUp, fetched, minutes);
        }
    }
}
