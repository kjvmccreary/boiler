using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.Diagnostics;
// If you have a custom permission attribute, keep it; otherwise remove the RequiresPermission line.
using Common.Authorization;
using Common.Constants;

namespace WorkflowService.Controllers;

[ApiController]
[Route("api/workflow/admin/diagnostics")]
[Authorize]
[RequiresPermission(Permissions.Workflow.Admin)]
public class WorkflowDiagnosticsController : ControllerBase
{
    private readonly IAutomaticDiagnosticsBuffer _buffer;
    private readonly WorkflowDiagnosticsOptions _opts;

    public WorkflowDiagnosticsController(
        IAutomaticDiagnosticsBuffer buffer,
        IOptions<WorkflowDiagnosticsOptions> opts)
    {
        _buffer = buffer;
        _opts = opts.Value;
    }

    [HttpGet("automatic")]
    public IActionResult GetAutomatic()
    {
        if (!_opts.EnableAutomaticTrace)
            return Ok(new { enabled = false, message = "EnableAutomaticTrace=false" });

        return Ok(new
        {
            enabled = true,
            entries = _buffer.Snapshot()
        });
    }

    [HttpDelete("automatic")]
    public IActionResult ClearAutomatic()
    {
        if (!_opts.EnableAutomaticTrace)
            return Ok(new { cleared = false, reason = "Tracing disabled" });

        _buffer.Clear();
        return Ok(new { cleared = true });
    }
}
