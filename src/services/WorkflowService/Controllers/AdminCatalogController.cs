using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using Common.Constants;
using WorkflowService.Engine.AutomaticActions;

namespace WorkflowService.Controllers;

/// <summary>
/// Administrative catalog endpoints (automatic action kinds, etc.)
/// </summary>
[ApiController]
[Route("api/workflow/admin")]
public class AdminCatalogController : ControllerBase
{
    private readonly IAutomaticActionRegistry _actionRegistry;
    private readonly ILogger<AdminCatalogController> _logger;

    public AdminCatalogController(
        IAutomaticActionRegistry actionRegistry,
        ILogger<AdminCatalogController> logger)
    {
        _actionRegistry = actionRegistry;
        _logger = logger;
    }

    /// <summary>
    /// Returns all registered automatic action kinds.
    /// </summary>
    /// GET /api/workflow/admin/action-catalog
    [HttpGet("action-catalog")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)] // TODO: Adjust to a more specific admin permission if desired.
    public ActionResult<object> GetActionCatalog()
    {
        var kinds = _actionRegistry.RegisteredKinds
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _logger.LogDebug("ACTION_CATALOG_REQUEST Count={Count}", kinds.Length);

        return Ok(new
        {
            actionKinds = kinds,
            count = kinds.Length
        });
    }
}
