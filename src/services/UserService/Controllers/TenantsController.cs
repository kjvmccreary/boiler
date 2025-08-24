using Contracts.Services;
using DTOs.Common;
using DTOs.Tenant;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Common.Authorization; // ✅ ADD
using System.Security.Claims;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly ITenantService _tenantService;
    private readonly IRoleTemplateService _roleTemplateService;
    private readonly ILogger<TenantsController> _logger;

    public TenantsController(
        ITenantService tenantService, 
        IRoleTemplateService roleTemplateService,
        ILogger<TenantsController> logger)
    {
        _tenantService = tenantService;
        _roleTemplateService = roleTemplateService;
        _logger = logger;
    }

    // 🔧 NEW: Dedicated endpoint for authenticated users to create additional tenants
    /// <summary>
    /// Create additional tenant for current authenticated user (consultant scenario)
    /// </summary>
    [HttpPost("create-additional")]
    [Authorize] // Only requires authentication, not specific permissions
    public async Task<ActionResult<ApiResponseDto<TenantDto>>> CreateAdditionalTenant(
        [FromBody] CreateAdditionalTenantDto request)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => new ErrorDto 
            { 
                Field = x.Key, 
                Message = e.ErrorMessage 
            })).ToList();
            
            return BadRequest(ApiResponseDto<TenantDto>.ErrorResult(errors));
        }

        try
        {
            var currentUserId = GetCurrentUserId();
            if (currentUserId <= 0)
            {
                return Unauthorized(ApiResponseDto<TenantDto>.ErrorResult("User not authenticated"));
            }

            _logger.LogInformation("User {UserId} creating additional tenant: {TenantName}", 
                currentUserId, request.TenantName);

            var result = await _tenantService.CreateTenantForExistingUserAsync(
                currentUserId, 
                request.TenantName, 
                request.TenantDomain);
            
            if (result.Success)
            {
                return CreatedAtAction(nameof(GetTenant), 
                    new { tenantId = result.Data!.Id }, result);
            }
            
            return BadRequest(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating additional tenant for user {UserId}", GetCurrentUserId());
            return StatusCode(500, ApiResponseDto<TenantDto>.ErrorResult("Failed to create tenant"));
        }
    }

    /// <summary>
    /// Create a new tenant with admin user - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost]
    [RequirePermission("tenants.create")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<TenantDto>>> CreateTenant([FromBody] CreateTenantDto createDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => new ErrorDto 
            { 
                Field = x.Key, 
                Message = e.ErrorMessage 
            })).ToList();
            
            return BadRequest(ApiResponseDto<TenantDto>.ErrorResult(errors));
        }

        var result = await _tenantService.CreateTenantAsync(createDto);
        
        if (result.Success)
        {
            return CreatedAtAction(nameof(GetTenant), new { tenantId = result.Data!.Id }, result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get tenant by ID - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet("{tenantId:int}")]
    [RequirePermission("tenants.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<TenantDto>>> GetTenant(int tenantId)
    {
        var result = await _tenantService.GetTenantAsync(tenantId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return NotFound(result);
    }

    /// <summary>
    /// Get all tenants (paginated) - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet]
    [RequirePermission("tenants.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<TenantDto>>>> GetTenants(
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 20)
    {
        if (page < 1 || pageSize < 1 || pageSize > 100)
        {
            return BadRequest(ApiResponseDto<PagedResultDto<TenantDto>>.ErrorResult(
                "Page must be >= 1 and pageSize must be between 1 and 100"));
        }

        var result = await _tenantService.GetTenantsAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Update tenant - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPut("{tenantId:int}")]
    [RequirePermission("tenants.edit")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<TenantDto>>> UpdateTenant(
        int tenantId, 
        [FromBody] UpdateTenantDto updateDto)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.SelectMany(x => x.Value!.Errors.Select(e => new ErrorDto 
            { 
                Field = x.Key, 
                Message = e.ErrorMessage 
            })).ToList();
            
            return BadRequest(ApiResponseDto<TenantDto>.ErrorResult(errors));
        }

        var result = await _tenantService.UpdateTenantAsync(tenantId, updateDto);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Delete tenant - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpDelete("{tenantId:int}")]
    [RequirePermission("tenants.delete")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteTenant(int tenantId)
    {
        var result = await _tenantService.DeleteTenantAsync(tenantId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Activate tenant - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost("{tenantId:int}/activate")]
    [RequirePermission("tenants.manage")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> ActivateTenant(int tenantId)
    {
        var result = await _tenantService.ActivateTenantAsync(tenantId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Deactivate tenant - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost("{tenantId:int}/deactivate")]
    [RequirePermission("tenants.manage")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> DeactivateTenant(int tenantId)
    {
        var result = await _tenantService.DeactivateTenantAsync(tenantId);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Initialize tenant - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost("{tenantId:int}/initialize")]
    [RequirePermission("tenants.initialize")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<TenantDto>>> InitializeTenant(
        int tenantId,
        [FromBody] TenantInitializationDto? initDto = null)
    {
        initDto ??= new TenantInitializationDto { TenantId = tenantId };
        initDto.TenantId = tenantId; // Ensure consistency

        var result = await _tenantService.InitializeTenantAsync(initDto);
        
        if (result.Success)
        {
            return Ok(result);
        }
        
        return BadRequest(result);
    }

    /// <summary>
    /// Get available role templates - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet("role-templates")]
    [RequirePermission("tenants.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<List<RoleTemplateDto>>>> GetRoleTemplates()
    {
        try
        {
            var templates = await _roleTemplateService.GetAvailableTemplatesAsync();
            return Ok(ApiResponseDto<List<RoleTemplateDto>>.SuccessResult(templates));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role templates");
            return StatusCode(500, ApiResponseDto<List<RoleTemplateDto>>.ErrorResult(
                "An error occurred while retrieving role templates"));
        }
    }

    // 🔧 HELPER: Get current user ID from JWT claims
    private int GetCurrentUserId()
    {
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value
                         ?? User.Claims.FirstOrDefault(c => c.Type == "user_id")?.Value;
        
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        
        return 0;
    }
}
