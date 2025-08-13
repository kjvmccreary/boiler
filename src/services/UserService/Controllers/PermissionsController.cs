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
    /// Get all available permissions in the system (requires permissions.view permission)
    /// </summary>
    [HttpGet]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<PermissionDto>>>> GetAllPermissions()
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for permissions.view permission instead of Admin role
            var hasPermissionsViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "permissions.view");
                
            if (!hasPermissionsViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view permissions without permissions.view permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<List<PermissionDto>>.ErrorResult("You don't have permission to view permissions"));
            }

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
    /// Get all permission categories (requires permissions.view permission)
    /// </summary>
    [HttpGet("categories")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetPermissionCategories()
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for permissions.view permission instead of Admin role
            var hasPermissionsViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "permissions.view");
                
            if (!hasPermissionsViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view permission categories without permissions.view permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<List<string>>.ErrorResult("You don't have permission to view permission categories"));
            }

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
    /// Get permissions by category (requires permissions.view permission)
    /// </summary>
    [HttpGet("categories/{category}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<PermissionDto>>>> GetPermissionsByCategory(string category)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for permissions.view permission instead of Admin role
            var hasPermissionsViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "permissions.view");
                
            if (!hasPermissionsViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view permissions by category without permissions.view permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<List<PermissionDto>>.ErrorResult("You don't have permission to view permissions"));
            }

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
    /// Get permissions grouped by category (requires permissions.view permission)
    /// </summary>
    [HttpGet("grouped")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<Dictionary<string, List<PermissionDto>>>>> GetPermissionsGrouped()
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for permissions.view permission instead of Admin role
            var hasPermissionsViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "permissions.view");
                
            if (!hasPermissionsViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view grouped permissions without permissions.view permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<Dictionary<string, List<PermissionDto>>>.ErrorResult("You don't have permission to view permissions"));
            }

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
    /// Get permissions for a specific user (requires users.view permission or own user)
    /// </summary>
    [HttpGet("users/{userId:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetUserPermissions(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Check permissions instead of roles
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            // Users can view their own permissions, users with users.view permission can view any user's permissions
            if (userId != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view permissions for user {TargetUserId} without users.view permission", 
                    currentUserId, userId);
                return Forbid("You don't have permission to view user permissions");
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
    /// Check if a user has a specific permission (requires users.view permission or own user)
    /// </summary>
    [HttpGet("users/{userId:int}/check/{permission}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckUserPermission(int userId, string permission)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Check permissions instead of roles
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            // Users can check their own permissions, users with users.view permission can check any user's permissions
            if (userId != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to check permission for user {TargetUserId} without users.view permission", 
                    currentUserId, userId);
                return Forbid("You don't have permission to check user permissions");
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
    /// Check if a user has any of the specified permissions (requires users.view permission or own user)
    /// </summary>
    [HttpPost("users/{userId:int}/check-any")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckUserHasAnyPermissions(
        int userId, 
        [FromBody] List<string> permissions)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Check permissions instead of roles
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            if (userId != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to check permissions for user {TargetUserId} without users.view permission", 
                    currentUserId, userId);
                return Forbid("You don't have permission to check user permissions");
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
    /// Check if a user has all of the specified permissions (requires users.view permission or own user)
    /// </summary>
    [HttpPost("users/{userId:int}/check-all")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> CheckUserHasAllPermissions(
        int userId, 
        [FromBody] List<string> permissions)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Check permissions instead of roles
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            if (userId != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to check permissions for user {TargetUserId} without users.view permission", 
                    currentUserId, userId);
                return Forbid("You don't have permission to check user permissions");
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
