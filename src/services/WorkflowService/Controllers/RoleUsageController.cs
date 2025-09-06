using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization;
using Common.Constants;
using DTOs.Common;
using DTOs.Workflow;
using WorkflowService.Services;
using Contracts.Services;

namespace WorkflowService.Controllers;

/// <summary>
/// Controller for checking role usage in workflow definitions
/// </summary>
[ApiController]
[Route("api/workflow/[controller]")]
[Authorize]
public class RoleUsageController : ControllerBase
{
    private readonly IRoleWorkflowUsageService _roleWorkflowUsageService;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<RoleUsageController> _logger;

    public RoleUsageController(
        IRoleWorkflowUsageService roleWorkflowUsageService,
        ITenantProvider tenantProvider,
        ILogger<RoleUsageController> logger)
    {
        _roleWorkflowUsageService = roleWorkflowUsageService;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    /// <summary>
    /// Check if a role is used in workflow definitions
    /// </summary>
    [HttpPost("check-usage")]
    [RequiresPermission(Permissions.Roles.View)]
    public async Task<ActionResult<ApiResponseDto<RoleUsageInWorkflowsDto>>> CheckRoleUsage([FromBody] CheckRoleUsageRequestDto request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.RoleName))
            {
                return BadRequest(ApiResponseDto<RoleUsageInWorkflowsDto>.ErrorResult("Role name is required"));
            }

            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest(ApiResponseDto<RoleUsageInWorkflowsDto>.ErrorResult("Tenant context required"));
            }

            var usageInfo = await _roleWorkflowUsageService.CheckRoleUsageInWorkflowsAsync(request.RoleName, tenantId.Value);
            
            return Ok(ApiResponseDto<RoleUsageInWorkflowsDto>.SuccessResult(usageInfo, "Role workflow usage checked successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role workflow usage for {RoleName}", request.RoleName);
            return StatusCode(500, ApiResponseDto<RoleUsageInWorkflowsDto>.ErrorResult("Failed to check role workflow usage"));
        }
    }
}
