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
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;

    public RolesController(
        IRoleService roleService,
        ILogger<RolesController> logger)
    {
        _roleService = roleService;
        _logger = logger;
    }

    /// <summary>
    /// Get all roles for the current tenant (tenant-scoped)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<RoleDto>>>> GetRoles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            if (page <= 0)
            {
                return BadRequest(ApiResponseDto<List<RoleDto>>.ErrorResult("Page number must be greater than 0"));
            }
            
            if (pageSize <= 0 || pageSize > 100)
            {
                return BadRequest(ApiResponseDto<List<RoleDto>>.ErrorResult("Page size must be between 1 and 100"));
            }

            // For now, get all roles (you can enhance with pagination later)
            var roles = await _roleService.GetTenantRolesAsync();
            
            // Apply search filter if provided
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                roles = roles.Where(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                        r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            // Convert to DTOs
            var roleDtos = roles.Select(MapToRoleDto).ToList();
            
            return Ok(ApiResponseDto<List<RoleDto>>.SuccessResult(roleDtos, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, ApiResponseDto<List<RoleDto>>.ErrorResult("An error occurred while retrieving roles"));
        }
    }

    /// <summary>
    /// Get a specific role by ID
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRole(int id)
    {
        try
        {
            var role = await _roleService.GetRoleWithPermissionsAsync(id);
            
            if (role == null)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            var roleDto = MapToRoleDto(role);
            return Ok(ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while retrieving the role"));
        }
    }

    /// <summary>
    /// Create a new role
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            // Check if role name is available
            var isNameAvailable = await _roleService.IsRoleNameAvailableAsync(createRoleDto.Name);
            if (!isNameAvailable)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult($"Role name '{createRoleDto.Name}' is already taken"));
            }

            var role = await _roleService.CreateRoleAsync(
                createRoleDto.Name,
                createRoleDto.Description,
                createRoleDto.Permissions);

            var roleDto = MapToRoleDto(role);
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, 
                ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role {RoleName}", createRoleDto.Name);
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while creating the role"));
        }
    }

    /// <summary>
    /// Update an existing role
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            // Check if role exists
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            // Check if the role is a system role (cannot be modified)
            if (existingRole.IsSystemRole)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("System roles cannot be modified"));
            }

            // Check if name is available (excluding current role)
            var isNameAvailable = await _roleService.IsRoleNameAvailableAsync(updateRoleDto.Name, id);
            if (!isNameAvailable)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult($"Role name '{updateRoleDto.Name}' is already taken"));
            }

            var role = await _roleService.UpdateRoleAsync(
                id,
                updateRoleDto.Name,
                updateRoleDto.Description,
                updateRoleDto.Permissions);

            var roleDto = MapToRoleDto(role);
            return Ok(ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while updating the role"));
        }
    }

    /// <summary>
    /// Delete a role
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteRole(int id)
    {
        try
        {
            // Check if role exists
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            // Check if the role is a system role (cannot be deleted)
            if (existingRole.IsSystemRole)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("System roles cannot be deleted"));
            }

            // Check if role has users assigned
            var usersInRole = await _roleService.GetUsersInRoleAsync(id);
            if (usersInRole.Any())
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Cannot delete role that has users assigned to it"));
            }

            await _roleService.DeleteRoleAsync(id);
            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while deleting the role"));
        }
    }

    /// <summary>
    /// Get permissions for a specific role
    /// </summary>
    [HttpGet("{id:int}/permissions")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetRolePermissions(int id)
    {
        try
        {
            var role = await _roleService.GetRoleWithPermissionsAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<List<string>>.ErrorResult("Role not found"));
            }

            return Ok(ApiResponseDto<List<string>>.SuccessResult(role.Permissions, "Role permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving role permissions"));
        }
    }

    /// <summary>
    /// Update permissions for a role
    /// </summary>
    [HttpPut("{id:int}/permissions")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateRolePermissions(int id, [FromBody] RolePermissionUpdateDto permissionDto)
    {
        try
        {
            // Check if role exists
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            // Check if the role is a system role (cannot be modified)
            if (existingRole.IsSystemRole)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("System role permissions cannot be modified"));
            }

            // Update the role with new permissions
            await _roleService.UpdateRoleAsync(
                id,
                existingRole.Name,
                existingRole.Description,
                permissionDto.Permissions);

            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role permissions updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while updating role permissions"));
        }
    }

    /// <summary>
    /// Assign a role to a user
    /// </summary>
    [HttpPost("assign")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            // Check if role exists
            var role = await _roleService.GetRoleByIdAsync(assignRoleDto.RoleId);
            if (role == null)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            await _roleService.AssignRoleToUserAsync(assignRoleDto.UserId, assignRoleDto.RoleId);
            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role assigned to user successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleId} to user {UserId}", assignRoleDto.RoleId, assignRoleDto.UserId);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while assigning the role"));
        }
    }

    /// <summary>
    /// Remove a role from a user
    /// </summary>
    [HttpDelete("{roleId:int}/users/{userId:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<bool>>> RemoveRoleFromUser(int roleId, int userId)
    {
        try
        {
            await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role removed from user successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing role {RoleId} from user {UserId}", roleId, userId);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while removing the role"));
        }
    }

    /// <summary>
    /// Get all roles assigned to a user
    /// </summary>
    [HttpGet("users/{userId:int}")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<RoleDto>>>> GetUserRoles(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("TenantAdmin");

            // Users can view their own roles, admins can view any user's roles
            if (userId != currentUserId && !isAdmin)
            {
                return Forbid();
            }

            var roles = await _roleService.GetUserRolesAsync(userId);
            var roleDtos = roles.Select(MapToRoleDto).ToList();

            return Ok(ApiResponseDto<List<RoleDto>>.SuccessResult(roleDtos, "User roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles for user {UserId}", userId);
            return StatusCode(500, ApiResponseDto<List<RoleDto>>.ErrorResult("An error occurred while retrieving user roles"));
        }
    }

    /// <summary>
    /// Get users assigned to a specific role
    /// </summary>
    [HttpGet("{id:int}/users")]
    [Authorize(Roles = "Admin,SuperAdmin,TenantAdmin")]
    public async Task<ActionResult<ApiResponseDto<List<UserInfo>>>> GetRoleUsers(int id)
    {
        try
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<List<UserInfo>>.ErrorResult("Role not found"));
            }

            var users = await _roleService.GetUsersInRoleAsync(id);
            return Ok(ApiResponseDto<List<UserInfo>>.SuccessResult(users.ToList(), "Role users retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<List<UserInfo>>.ErrorResult("An error occurred while retrieving role users"));
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

    private static RoleDto MapToRoleDto(RoleInfo roleInfo)
    {
        return new RoleDto
        {
            Id = roleInfo.Id,
            TenantId = roleInfo.TenantId,
            Name = roleInfo.Name,
            Description = roleInfo.Description,
            IsSystemRole = roleInfo.IsSystemRole,
            IsDefault = roleInfo.IsDefault,
            Permissions = roleInfo.Permissions,
            CreatedAt = roleInfo.CreatedAt,
            UpdatedAt = roleInfo.UpdatedAt,
            UserCount = 0 // You can enhance this later if needed
        };
    }

    #endregion
}
