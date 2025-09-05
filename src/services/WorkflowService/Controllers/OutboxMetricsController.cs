using Microsoft.AspNetCore.Mvc;
using WorkflowService.Outbox;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/outbox/metrics")]
public class OutboxMetricsController : ControllerBase
{
    private readonly IOutboxMetricsProvider _metrics;
    private readonly OutboxOptions _options;

    public OutboxMetricsController(IOutboxMetricsProvider metrics,
        Microsoft.Extensions.Options.IOptions<OutboxOptions> options)
    {
        _metrics = metrics;
        _options = options.Value;
    }

    [HttpGet]
    public async Task<ActionResult<OutboxMetricsSnapshot>> GetAsync(CancellationToken ct)
    {
        if (!_options.EnableMetrics)
            return StatusCode(503, "Metrics disabled");
        var snap = await _metrics.GetSnapshotAsync(ct);
        return Ok(snap);
    }
}
