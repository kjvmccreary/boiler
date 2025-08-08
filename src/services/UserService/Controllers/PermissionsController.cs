using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Contracts.Services;
using DTOs.Common;
using DTOs.Auth;
using System.Security.Claims;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PermissionsController : ControllerBase
{
    private readonly IPermissionService _permissionService;
    private readonly ILogger<PermissionsController> _logger;

    public PermissionsController(
        IPermissionService permissionService,
        ILogger<PermissionsController> logger)
    {
        _permissionService = permissionService;
        _logger = logger;
    }

    /// <summary>
    /// Get all available permissions in the system
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<PermissionDto>>>> GetAllPermissions()
    {
        try
        {
            var permissions = await _permissionService.GetAllAvailablePermissionsAsync();
            var permissionDtos = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description,
                IsActive = p.IsActive
            }).ToList();

            return Ok(ApiResponseDto<List<PermissionDto>>.SuccessResult(
                permissionDtos, 
                "Permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions");
            return StatusCode(500, ApiResponseDto<List<PermissionDto>>.ErrorResult(
                "An error occurred while retrieving permissions"));
        }
    }

    /// <summary>
    /// Get all permission categories
    /// </summary>
    [HttpGet("categories")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetPermissionCategories()
    {
        try
        {
            var categories = await _permissionService.GetPermissionCategoriesAsync();
            return Ok(ApiResponseDto<List<string>>.SuccessResult(categories, "Categories retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permission categories");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult(
                "An error occurred while retrieving categories"));
        }
    }

    /// <summary>
    /// Get permissions by category
    /// </summary>
    [HttpGet("categories/{category}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<PermissionDto>>>> GetPermissionsByCategory(string category)
    {
        try
        {
            var permissions = await _permissionService.GetPermissionsForCategoryAsync(category);
            var permissionDtos = permissions.Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Category = p.Category,
                Description = p.Description,
                IsActive = p.IsActive
            }).ToList();

            return Ok(ApiResponseDto<List<PermissionDto>>.SuccessResult(
                permissionDtos, 
                $"Permissions for category '{category}' retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for category {Category}", category);
            return StatusCode(500, ApiResponseDto<List<PermissionDto>>.ErrorResult(
                "An error occurred while retrieving permissions"));
        }
    }

    /// <summary>
    /// Get permissions grouped by category
    /// </summary>
    [HttpGet("grouped")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<Dictionary<string, List<PermissionDto>>>>> GetPermissionsGrouped()
    {
        try
        {
            var permissions = await _permissionService.GetPermissionsByCategoryAsync();
            var groupedPermissions = permissions.ToDictionary(
                g => g.Key,
                g => g.Value.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Description = p.Description,
                    IsActive = p.IsActive
                }).ToList()
            );

            return Ok(ApiResponseDto<Dictionary<string, List<PermissionDto>>>.SuccessResult(
                groupedPermissions, 
                "Grouped permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving grouped permissions");
            return StatusCode(500, ApiResponseDto<Dictionary<string, List<PermissionDto>>>.ErrorResult(
                "An error occurred while retrieving grouped permissions"));
        }
    }

    /// <summary>
    /// Get permissions for a specific user
    /// </summary>
    [HttpGet("users/{userId:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetUserPermissions(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("TenantAdmin");

            // Users can view their own permissions, admins can view any user's permissions
            if (userId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            return Ok(ApiResponseDto<List<string>>.SuccessResult(
                permissions.ToList(), 
                "User permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserId}", userId);
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult(
                "An error occurred while retrieving user permissions"));
        }
    }

    /// <summary>
    /// Check if a user has a specific permission
    /// </summary>
    [HttpGet("users/{userId:int}/check/{permission}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckUserPermission(int userId, string permission)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("TenantAdmin");

            // Users can check their own permissions, admins can check any user's permissions
            if (userId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var hasPermission = await _permissionService.UserHasPermissionAsync(userId, permission);
            return Ok(ApiResponseDto<bool>.SuccessResult(hasPermission, "Permission check completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permission {Permission} for user {UserId}", permission, userId);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult(
                "An error occurred while checking permission"));
        }
    }

    /// <summary>
    /// Check if a user has any of the specified permissions
    /// </summary>
    [HttpPost("users/{userId:int}/check-any")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckUserHasAnyPermissions(
        int userId, 
        [FromBody] List<string> permissions)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("TenantAdmin");

            if (userId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var hasAnyPermission = await _permissionService.UserHasAnyPermissionAsync(userId, permissions);
            return Ok(ApiResponseDto<bool>.SuccessResult(
                hasAnyPermission, 
                "Permission check completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking any permissions for user {UserId}", userId);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult(
                "An error occurred while checking permissions"));
        }
    }

    /// <summary>
    /// Check if a user has all of the specified permissions
    /// </summary>
    [HttpPost("users/{userId:int}/check-all")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckUserHasAllPermissions(
        int userId, 
        [FromBody] List<string> permissions)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("TenantAdmin");

            if (userId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var hasAllPermissions = await _permissionService.UserHasAllPermissionsAsync(userId, permissions);
            return Ok(ApiResponseDto<bool>.SuccessResult(
                hasAllPermissions, 
                "Permission check completed"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking all permissions for user {UserId}", userId);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult(
                "An error occurred while checking permissions"));
        }
    }

    /// <summary>
    /// Get current user's permissions (convenience endpoint)
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetMyPermissions()
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var permissions = await _permissionService.GetUserPermissionsAsync(currentUserId);
            
            return Ok(ApiResponseDto<List<string>>.SuccessResult(
                permissions.ToList(), 
                "Your permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving current user permissions");
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult(
                "An error occurred while retrieving your permissions"));
        }
    }

    #region Helper Methods

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(userIdClaim, out var userId))
        {
            return userId;
        }
        throw new UnauthorizedAccessException("User ID not found in token");
    }

    #endregion
}
