// FILE: src/services/UserService/Controllers/UsersController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Contracts.User;
using DTOs.Common;
using DTOs.User;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using DTOs.Tenant;
using Common.Authorization; // ✅ ADD

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IUserProfileService _userProfileService;
    private readonly ILogger<UsersController> _logger;
    private readonly ApplicationDbContext _context;  // Add this

    public UsersController(
        IUserService userService,
        IUserProfileService userProfileService,
        ILogger<UsersController> logger,
        ApplicationDbContext context)  // Add this parameter
    {
        _userService = userService;
        _userProfileService = userProfileService;
        _logger = logger;
        _context = context;  // Add this
    }

    /// <summary>
    /// Get current user's profile information - No additional permission needed (own profile)
    /// </summary>
    [HttpGet("profile")]
    // ✅ NO PERMISSION ATTRIBUTE: Users can always access their own profile
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
    /// Get list of users - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet]
    [RequirePermission("users.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<UserDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
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
    /// Get specific user by ID - SECURE: Mixed permission logic
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // ✅ KEEP: This permission logic - users can view their own profile OR need users.view permission
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

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
    /// Delete user - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpDelete("{id:int}")]
    [RequirePermission("users.delete")] // ✅ SECURE: Framework-enforced permission check
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

            var result = await _userService.DeleteUserAsync(id);
            
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
    /// Create a new user - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost]
    [RequirePermission("users.create")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<UserDto>>> CreateUser([FromBody] CreateUserDto createUserDto)
    {
        try
        {
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
    /// Assign role to user - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost("{id:int}/roles")]
    [RequirePermission("users.manage_roles")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> AssignRoleToUser(int id, [FromBody] AssignUserRoleDto assignRoleDto)
    {
        try
        {
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
    /// Remove role from user - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpDelete("{id:int}/roles/{roleId:int}")]
    [RequirePermission("users.manage_roles")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> RemoveRoleFromUser(int id, int roleId)
    {
        try
        {
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
    /// Enable or disable user account (requires users.edit permission)
    /// </summary>
    [HttpPut("{id:int}/status")]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateUserStatus(int id, [FromBody] UpdateUserStatusDto statusDto)
    {
        try
        {
            // ✅ FIX: Check for users.edit permission instead of users.manage
            var hasUsersEditPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.edit");
            
            if (!hasUsersEditPermission)
            {
                _logger.LogWarning("User {UserId} attempted to update status for user {TargetUserId} without users.edit permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to edit users"));
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

    /// <summary>
    /// Get tenants that a user has access to (requires users.view permission or own user)
    /// </summary>
    [HttpGet("{id:int}/tenants")]
    public async Task<ActionResult<ApiResponseDto<List<TenantDto>>>> GetUserTenants(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // Users can view their own tenants, users with users.view permission can view any user's tenants
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            if (id != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view tenants for user {TargetUserId} without users.view permission", 
                    currentUserId, id);
                return StatusCode(403, ApiResponseDto<List<TenantDto>>.ErrorResult("You don't have permission to view user tenants"));
            }

            // 🔧 CRITICAL FIX: Use IgnoreQueryFilters() to bypass global tenant filtering
            // We want ALL tenants this user has access to, not just current tenant
            var userTenants = await _context.TenantUsers
                .IgnoreQueryFilters() // 🔧 ADD: This bypasses the global tenant filter
                .Where(tu => tu.UserId == id && tu.IsActive)
                .Include(tu => tu.Tenant)
                .Where(tu => tu.Tenant.IsActive)
                .Select(tu => new TenantDto
                {
                    Id = tu.Tenant.Id,
                    Name = tu.Tenant.Name,
                    Domain = tu.Tenant.Domain,
                    SubscriptionPlan = tu.Tenant.SubscriptionPlan,
                    IsActive = tu.Tenant.IsActive,
                    CreatedAt = tu.Tenant.CreatedAt,
                    UpdatedAt = tu.Tenant.UpdatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Found {Count} tenants for user {UserId} (unfiltered by tenant context)", userTenants.Count, id);

            return Ok(ApiResponseDto<List<TenantDto>>.SuccessResult(userTenants, "User tenants retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenants for user {UserId}", id);
            return StatusCode(500, ApiResponseDto<List<TenantDto>>.ErrorResult("An error occurred while retrieving user tenants"));
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
