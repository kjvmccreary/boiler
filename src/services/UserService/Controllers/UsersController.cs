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
    /// Get list of users (Admin and SuperAdmin only, tenant-scoped)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<UserDto>>>> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string sortDirection = "asc")
    {
        try
        {
            // ✅ FIXED: Validate parameters and map to DTO
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
    /// Get specific user by ID (with permission checks)
    /// </summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ApiResponseDto<UserDto>>> GetUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            // FIXED: Check for both Admin and SuperAdmin roles
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin");

            // Users can view their own profile, admins can view any user in their tenant
            if (id != currentUserId && !isAdmin)
            {
                return Forbid();
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
    /// Delete user (Admin and SuperAdmin only, cannot delete self)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteUser(int id)
    {
        try
        {
            var currentUserId = GetCurrentUserId();

            // Prevent admins from deleting themselves
            if (id == currentUserId)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("You cannot delete your own account"));
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
