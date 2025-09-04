using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowService.Services.Interfaces;
using DTOs.Workflow.GatewayDiagnostics;

namespace WorkflowService.Controllers;

[ApiController]
[Route("workflow/instances")]
[Authorize(Policy = "workflow.read")]
public class DiagnosticsController : ControllerBase
{
    private readonly IWorkflowDiagnosticsService _diag;
    private readonly ILogger<DiagnosticsController> _logger;

    public DiagnosticsController(IWorkflowDiagnosticsService diag, ILogger<DiagnosticsController> logger)
    {
        _diag = diag;
        _logger = logger;
    }

    /// <summary>
    /// Get gateway decision history (all gateways) for a workflow instance.
    /// </summary>
    [HttpGet("{instanceId:int}/gateway-decisions")]
    [ProducesResponseType(typeof(GatewayDecisionHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGatewayDecisionHistory(int instanceId, CancellationToken ct)
    {
        var history = await _diag.GetGatewayDecisionHistoryAsync(instanceId, ct);
        if (history == null) return NotFound();
        return Ok(history);
    }

    /// <summary>
    /// Get decision history for a specific gateway node in an instance.
    /// </summary>
    [HttpGet("{instanceId:int}/gateway-decisions/{nodeId}")]
    [ProducesResponseType(typeof(GatewayNodeDecisionHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetGatewayNodeDecisionHistory(int instanceId, string nodeId, CancellationToken ct)
    {
        var node = await _diag.GetGatewayNodeDecisionHistoryAsync(instanceId, nodeId, ct);
        if (node == null) return NotFound();
        return Ok(node);
    }
}
