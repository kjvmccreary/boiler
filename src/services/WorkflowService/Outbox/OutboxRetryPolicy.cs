namespace WorkflowService.Outbox;

// Public so tests can access ComputeDelay
public static class OutboxRetryPolicy
{
    private static readonly Random _rng = new();

    /// <summary>
    /// Computes the retry delay (1-based attempt index) applying fixed or exponential backoff and optional +/- jitter.
    /// </summary>
    public static TimeSpan ComputeDelay(int currentRetryCount, OutboxOptions options)
    {
        double baseSeconds = options.BaseRetryDelaySeconds <= 0 ? 5 : options.BaseRetryDelaySeconds;

        double raw = options.UseExponentialBackoff
            ? baseSeconds * Math.Pow(2, Math.Max(0, currentRetryCount - 1))
            : baseSeconds;

        var jitterRatio = options.JitterRatio is > 0 and < 1 ? options.JitterRatio : 0;
        if (jitterRatio > 0)
        {
            var delta = raw * jitterRatio;
            var offset = (_rng.NextDouble() * 2 - 1) * delta; // [-delta, +delta]
            raw += offset;
        }

        raw = Math.Min(raw, 3600); // cap at 1 hour
        return TimeSpan.FromSeconds(Math.Max(1, raw));
    }
}
