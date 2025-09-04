using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using Common.Constants;
using WorkflowService.Engine.AutomaticActions;
using WorkflowService.Engine.Gateways;

namespace WorkflowService.Controllers;

/// <summary>
/// Administrative catalog endpoints (automatic action kinds, gateway strategy kinds, etc.)
/// </summary>
[ApiController]
[Route("api/workflow/admin")]
public class AdminCatalogController : ControllerBase
{
    private readonly IAutomaticActionRegistry _actionRegistry;
    private readonly IGatewayStrategyRegistry _gatewayStrategyRegistry;
    private readonly ILogger<AdminCatalogController> _logger;

    public AdminCatalogController(
        IAutomaticActionRegistry actionRegistry,
        IGatewayStrategyRegistry gatewayStrategyRegistry,
        ILogger<AdminCatalogController> logger)
    {
        _actionRegistry = actionRegistry;
        _gatewayStrategyRegistry = gatewayStrategyRegistry;
        _logger = logger;
    }

    /// <summary>
    /// Returns all registered automatic action kinds.
    /// </summary>
    /// GET /api/workflow/admin/action-catalog
    [HttpGet("action-catalog")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)] // Adjust permission if needed
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

    /// <summary>
    /// Returns all registered gateway strategy kinds.
    /// </summary>
    /// GET /api/workflow/admin/gateway-strategy-catalog
    [HttpGet("gateway-strategy-catalog")]
    [RequiresPermission(Permissions.Workflow.ViewDefinitions)] // Adjust permission if needed
    public ActionResult<object> GetGatewayStrategyCatalog()
    {
        var kinds = _gatewayStrategyRegistry.RegisteredKinds
            .OrderBy(k => k, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        _logger.LogDebug("GATEWAY_STRATEGY_CATALOG_REQUEST Count={Count}", kinds.Length);

        return Ok(new
        {
            strategyKinds = kinds,
            count = kinds.Length
        });
    }
}
