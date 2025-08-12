// FILE: src/services/UserService/Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Contracts.User;
using DTOs.Common;
using DTOs.User;
using System.Security.Claims;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // All endpoints require authentication
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(
        IUserService userService,
        IUserProfileService userProfileService,
        ILogger<UsersController> logger)
    {
        _userService = userService;
        _userProfileService = userProfileService;
        _logger = logger;
    }

    /// <summary>
    /// Get current user's profile information
    /// </summary>
    [HttpGet("profile")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetCurrentUserProfile()
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userProfileService.GetUserProfileAsync(userId);
            
            if (!result.Success || result.Data == null)
            {
                return NotFound(ApiResponseDto<UserDto>.ErrorResult("User profile not found"));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user profile for user {UserId}", GetCurrentUserId());
            return StatusCode(500, ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving your profile"));
        }
    }

    /// <summary>
    /// Update current user's profile information
    /// </summary>
    [HttpPut("profile")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateCurrentUserProfile([FromBody] UserUpdateDto updateProfileDto)
    {
        try
        {
            var userId = GetCurrentUserId();
            var result = await _userProfileService.UpdateUserProfileAsync(userId, updateProfileDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile for user {UserId}", GetCurrentUserId());
            return StatusCode(500, ApiResponseDto<UserDto>.ErrorResult("An error occurred while updating your profile"));
        }
    }

    /// <summary>
    /// Get list of users (requires users.view permission)
    /// </summary>
    [HttpGet]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<UserDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.view permission instead of Admin role
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");
                
            if (!hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to access users list without users.view permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("You don't have permission to view users"));
            }

            // Validate parameters and map to DTO
            if (page <= 0)
            {
                return BadRequest(ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("Page number must be greater than 0"));
            }
            
            if (pageSize <= 0 || pageSize > 100)
            {
                return BadRequest(ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("Page size must be between 1 and 100"));
            }

            // Map query parameters to DTO
            var request = new PaginationRequestDto
            {
                PageNumber = page,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SortBy = sortBy,
                SortDirection = sortDirection
            };

            var result = await _userService.GetUsersAsync(request);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, ApiResponseDto<PagedResultDto<UserDto>>.ErrorResult("An error occurred while retrieving users"));
        }
    }

    /// <summary>
    /// Get specific user by ID (requires users.view permission or own user)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Use permission-based check instead of role-based
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            // Users can view their own profile, users with users.view permission can view any user in their tenant
            if (id != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view user {TargetUserId} without users.view permission", 
                    currentUserId, id);
                return StatusCode(403, ApiResponseDto<UserDto>.ErrorResult("You don't have permission to view this user"));
            }

            var result = await _userService.GetUserByIdAsync(id);
            
            if (!result.Success || result.Data == null)
            {
                return NotFound(ApiResponseDto<UserDto>.ErrorResult("User not found"));
            }

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user {UserId}", id);
            return StatusCode(500, ApiResponseDto<UserDto>.ErrorResult("An error occurred while retrieving the user"));
        }
    }

    /// <summary>
    /// Delete user (requires users.delete permission, cannot delete self)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            // Prevent users from deleting themselves
            if (id == currentUserId)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("You cannot delete your own account"));
            }

            // 🔧 .NET 9 FIX: Check for users.delete permission instead of Admin role
            var hasUsersDeletePermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.delete");
                
            if (!hasUsersDeletePermission)
            {
                _logger.LogWarning("User {UserId} attempted to delete user {TargetUserId} without users.delete permission", 
                    currentUserId, id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to delete users"));
            }

            var result = await _userService.DeleteUserAsync(id);
            
            // ✅ FIXED: Check if user was found and return appropriate status
            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while deleting the user"));
        }
    }

    /// <summary>
    /// Update user by ID (requires users.edit permission)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<UserDto>>> UpdateUser(int id, [FromBody] UserUpdateDto updateUserDto)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.edit permission instead of Admin role
            var hasUsersEditPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.edit");
                
            if (!hasUsersEditPermission)
            {
                _logger.LogWarning("User {UserId} attempted to update user {TargetUserId} without users.edit permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<UserDto>.ErrorResult("You don't have permission to edit users"));
            }

            var currentUserId = GetCurrentUserId();
            
            // Prevent admins from updating themselves via this endpoint (use profile endpoint instead)
            if (id == currentUserId)
            {
                return BadRequest(ApiResponseDto<UserDto>.ErrorResult("Use the profile endpoint to update your own profile"));
            }

            var result = await _userService.UpdateUserAsync(id, updateUserDto);
            
            if (!result.Success)
            {
                if (result.Message?.Contains("not found") == true)
                {
                    return NotFound(result);
                }
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user {UserId}", id);
            return StatusCode(500, ApiResponseDto<UserDto>.ErrorResult("An error occurred while updating the user"));
        }
    }

    /// <summary>
    /// Create a new user (requires users.create permission)
    /// </summary>
    [HttpPost]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.create permission instead of Admin role
            var hasUsersCreatePermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.create");
                
            if (!hasUsersCreatePermission)
            {
                _logger.LogWarning("User {UserId} attempted to create user without users.create permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<UserDto>.ErrorResult("You don't have permission to create users"));
            }

            var result = await _userService.CreateUserAsync(createUserDto);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return CreatedAtAction(nameof(GetUser), new { id = result.Data!.Id }, result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, ApiResponseDto<UserDto>.ErrorResult("An error occurred while creating the user"));
        }
    }

    /// <summary>
    /// Get user roles (requires users.view permission or own user)
    /// </summary>
    [HttpGet("{id:int}/roles")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetUserRoles(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Use permission-based check instead of role-based
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            // Users can view their own roles, users with users.view permission can view any user's roles
            if (id != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view roles for user {TargetUserId} without users.view permission", 
                    currentUserId, id);
                return StatusCode(403, ApiResponseDto<List<string>>.ErrorResult("You don't have permission to view user roles"));
            }

            var result = await _userService.GetUserRolesAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", id);
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving user roles"));
        }
    }

    /// <summary>
    /// Assign role to user (requires users.manage_roles permission)
    /// </summary>
    [HttpPost("{id:int}/roles")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> AssignRoleToUser(int id, [FromBody] AssignUserRoleDto assignRoleDto)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.manage_roles permission instead of Admin role
            var hasUsersManageRolesPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.manage_roles");
                
            if (!hasUsersManageRolesPermission)
            {
                _logger.LogWarning("User {UserId} attempted to assign role to user {TargetUserId} without users.manage_roles permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to manage user roles"));
            }

            var result = await _userService.AssignRoleToUserAsync(id, assignRoleDto.RoleId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role to user {UserId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while assigning the role"));
        }
    }

    /// <summary>
    /// Remove role from user (requires users.manage_roles permission)
    /// </summary>
    [HttpDelete("{id:int}/roles/{roleId:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> RemoveRoleFromUser(int id, int roleId)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.manage_roles permission instead of Admin role
            var hasUsersManageRolesPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.manage_roles");
                
            if (!hasUsersManageRolesPermission)
            {
                _logger.LogWarning("User {UserId} attempted to remove role from user {TargetUserId} without users.manage_roles permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to manage user roles"));
            }

            var result = await _userService.RemoveRoleFromUserAsync(id, roleId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role from user {UserId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while removing the role"));
        }
    }

    /// <summary>
    /// Get user permissions (requires users.view permission or own user)
    /// </summary>
    [HttpGet("{id:int}/permissions")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetUserPermissions(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Use permission-based check instead of role-based
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            // Users can view their own permissions, users with users.view permission can view any user's permissions
            if (id != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view permissions for user {TargetUserId} without users.view permission", 
                    currentUserId, id);
                return StatusCode(403, ApiResponseDto<List<string>>.ErrorResult("You don't have permission to view user permissions"));
            }

            var result = await _userService.GetUserPermissionsAsync(id);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for user {UserId}", id);
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving user permissions"));
        }
    }

    /// <summary>
    /// Enable or disable user account (requires users.manage permission)
    /// </summary>
    [HttpPut("{id:int}/status")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDto statusDto)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.manage permission instead of Admin role
            var hasUsersManagePermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.manage");
                
            if (!hasUsersManagePermission)
            {
                _logger.LogWarning("User {UserId} attempted to update status for user {TargetUserId} without users.manage permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to manage user status"));
            }

            var currentUserId = GetCurrentUserId();
            
            // Prevent users from disabling themselves
            if (id == currentUserId && !statusDto.IsActive)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("You cannot disable your own account"));
            }

            var result = await _userService.UpdateUserStatusAsync(id, statusDto.IsActive);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }
            
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for user {UserId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while updating user status"));
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
