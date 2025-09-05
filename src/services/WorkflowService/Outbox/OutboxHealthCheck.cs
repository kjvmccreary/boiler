using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WorkflowService.Outbox;

public class OutboxHealthCheck : IHealthCheck
{
    private readonly IOutboxMetricsProvider _metrics;
    private readonly OutboxOptions _options;

    public OutboxHealthCheck(IOutboxMetricsProvider metrics, Microsoft.Extensions.Options.IOptions<OutboxOptions> options)
    {
        _metrics = metrics;
        _options = options.Value;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        var snap = await _metrics.GetSnapshotAsync(cancellationToken);
        var h = _options.Health;

        double failedPendingRatio = snap.BacklogSize == 0 ? 0 : (double)snap.FailedPending / snap.BacklogSize;
        double oldest = snap.OldestAgeSeconds ?? 0;

        var data = new Dictionary<string, object?>
        {
            ["backlogSize"] = snap.BacklogSize,
            ["failedPending"] = snap.FailedPending,
            ["failedPendingRatio"] = failedPendingRatio,
            ["oldestAgeSeconds"] = snap.OldestAgeSeconds,
            ["throughputPerMinuteWindow"] = snap.ThroughputPerMinuteWindow,
            ["failureRatioWindow"] = snap.FailureRatioWindow
        };

        // Determine status
        var status = HealthStatus.Healthy;

        if (snap.BacklogSize >= h.BacklogUnhealthy ||
            failedPendingRatio >= h.FailedPendingRatioUnhealthy ||
            oldest >= h.OldestAgeUnhealthySeconds)
        {
            status = HealthStatus.Unhealthy;
        }
        else if (snap.BacklogSize >= h.BacklogWarn ||
                 failedPendingRatio >= h.FailedPendingRatioWarn ||
                 oldest >= h.OldestAgeWarnSeconds)
        {
            status = HealthStatus.Degraded;
        }

        var desc = $"Outbox health status: {status}";
        return new HealthCheckResult(status, desc, null, data);
    }
}
