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
    /// Get all roles for the current tenant (requires roles.view permission)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<RoleDto>>>> GetRoles( // ✅ CHANGED: Return PagedResultDto
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            // Check for roles.view permission
            var hasRolesViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.view");
                
            if (!hasRolesViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to access roles list without roles.view permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<PagedResultDto<RoleDto>>.ErrorResult("You don't have permission to view roles"));
            }

            if (page <= 0)
            {
                return BadRequest(ApiResponseDto<PagedResultDto<RoleDto>>.ErrorResult("Page number must be greater than 0"));
            }
            
            if (pageSize <= 0 || pageSize > 100)
            {
                return BadRequest(ApiResponseDto<PagedResultDto<RoleDto>>.ErrorResult("Page size must be between 1 and 100"));
            }

            var roles = await _roleService.GetTenantRolesAsync();
            
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                roles = roles.Where(r => r.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                        r.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }

            var roleDtos = roles.Select(MapToRoleDto).ToList();
            
            // ✅ FIX: Use constructor instead of property assignment
            var totalCount = roleDtos.Count;
            var pagedItems = roleDtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            var pagedResult = new PagedResultDto<RoleDto>(
                items: pagedItems,
                totalCount: totalCount,
                pageNumber: page,
                pageSize: pageSize
            );
            // Note: TotalPages is automatically calculated by the property getter
            
            return Ok(ApiResponseDto<PagedResultDto<RoleDto>>.SuccessResult(pagedResult, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, ApiResponseDto<PagedResultDto<RoleDto>>.ErrorResult("An error occurred while retrieving roles"));
        }
    }

    /// <summary>
    /// Get a specific role by ID (requires roles.view permission)
    /// </summary>
    [HttpGet("{id:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRole(int id)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for roles.view permission instead of Admin role
            var hasRolesViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.view");
                
            if (!hasRolesViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view role {RoleId} without roles.view permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<RoleDto>.ErrorResult("You don't have permission to view roles"));
            }

            var role = await _roleService.GetRoleByIdAsync(id);
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
    /// Create a new role (requires roles.create permission)
    /// </summary>
    [HttpPost]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for roles.create permission instead of Admin role
            var hasRolesCreatePermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.create");
                
            if (!hasRolesCreatePermission)
            {
                _logger.LogWarning("User {UserId} attempted to create role without roles.create permission", 
                    GetCurrentUserId());
                return StatusCode(403, ApiResponseDto<RoleDto>.ErrorResult("You don't have permission to create roles"));
            }

            var role = await _roleService.CreateRoleAsync(createRoleDto.Name, createRoleDto.Description, createRoleDto.Permissions ?? new List<string>());
            var roleDto = MapToRoleDto(role);

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, 
                ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role created successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while creating the role"));
        }
    }

    /// <summary>
    /// Update an existing role (requires roles.edit permission)
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto updateRoleDto)
    {
        try
        {
            var hasRolesEditPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.edit");
                
            if (!hasRolesEditPermission)
            {
                _logger.LogWarning("User {UserId} attempted to update role {RoleId} without roles.edit permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<RoleDto>.ErrorResult("You don't have permission to edit roles"));
            }

            // 🔧 .NET 9 FIX: Add missing permissions parameter
            var success = await _roleService.UpdateRoleAsync(id, updateRoleDto.Name, updateRoleDto.Description, updateRoleDto.Permissions);
            if (!success)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            // Get updated role to return
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

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
    /// Delete a role (requires roles.delete permission)
    /// </summary>
    [HttpDelete("{id:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteRole(int id)
    {
        try
        {
            var hasRolesDeletePermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.delete");
                
            if (!hasRolesDeletePermission)
            {
                _logger.LogWarning("User {UserId} attempted to delete role {RoleId} without roles.delete permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to delete roles"));
            }

            // 🔧 .NET 9 FIX: Handle bool return type
            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role deleted successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while deleting the role"));
        }
    }

    /// <summary>
    /// Get permissions for a specific role (requires roles.view permission)
    /// </summary>
    [HttpGet("{id:int}/permissions")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetRolePermissions(int id)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for roles.view permission instead of Admin role
            var hasRolesViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.view");
                
            if (!hasRolesViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view permissions for role {RoleId} without roles.view permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<List<string>>.ErrorResult("You don't have permission to view role permissions"));
            }

            var role = await _roleService.GetRoleWithPermissionsAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<List<string>>.ErrorResult("Role not found"));
            }

            var permissions = role.Permissions.ToList();
            return Ok(ApiResponseDto<List<string>>.SuccessResult(permissions, "Role permissions retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving role permissions"));
        }
    }

    /// <summary>
    /// Update permissions for a specific role (requires roles.manage_permissions permission)
    /// </summary>
    [HttpPut("{id:int}/permissions")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateRolePermissions(int id, [FromBody] List<string> permissions)
    {
        try
        {
            var hasRolesManagePermissionsPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "roles.manage_permissions");
            
            if (!hasRolesManagePermissionsPermission)
            {
                _logger.LogWarning("User {UserId} attempted to update permissions for role {RoleId} without roles.manage_permissions permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to manage role permissions"));
            }

            // 🔧 .NET 9 FIX: Handle bool return type
            var success = await _roleService.UpdateRolePermissionsAsync(id, permissions);
            if (!success)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role permissions updated successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while updating role permissions"));
        }
    }

    /// <summary>
    /// Assign a role to a user (requires users.manage_roles permission)
    /// </summary>
    [HttpPost("assign")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.manage_roles permission instead of Admin role
            var hasUsersManageRolesPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.manage_roles");
                
            if (!hasUsersManageRolesPermission)
            {
                _logger.LogWarning("User {UserId} attempted to assign role {RoleId} to user {TargetUserId} without users.manage_roles permission", 
                    GetCurrentUserId(), assignRoleDto.RoleId, assignRoleDto.UserId);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to manage user roles"));
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
    /// Remove a role from a user (requires users.manage_roles permission)
    /// </summary>
    [HttpDelete("{roleId:int}/users/{userId:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> RemoveRoleFromUser(int roleId, int userId)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.manage_roles permission instead of Admin role
            var hasUsersManageRolesPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.manage_roles");
                
            if (!hasUsersManageRolesPermission)
            {
                _logger.LogWarning("User {UserId} attempted to remove role {RoleId} from user {TargetUserId} without users.manage_roles permission", 
                    GetCurrentUserId(), roleId, userId);
                return StatusCode(403, ApiResponseDto<bool>.ErrorResult("You don't have permission to manage user roles"));
            }

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
    /// Get all roles assigned to a user (requires users.view or own user permission)
    /// </summary>
    [HttpGet("users/{userId:int}")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<RoleDto>>>> GetUserRoles(int userId)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            
            // 🔧 .NET 9 FIX: Check permissions instead of roles
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");

            // Users can view their own roles, users with users.view permission can view any user's roles
            if (userId != currentUserId && !hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view roles for user {TargetUserId} without users.view permission", 
                    currentUserId, userId);
                return StatusCode(403, ApiResponseDto<List<RoleDto>>.ErrorResult("You don't have permission to view user roles"));
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
    /// Get users assigned to a specific role (requires users.view permission)
    /// </summary>
    [HttpGet("{id:int}/users")]
    [Authorize] // 🔧 .NET 9 FIX: Remove role requirement, use permission check
    public async Task<ActionResult<ApiResponseDto<List<UserInfo>>>> GetRoleUsers(int id)
    {
        try
        {
            // 🔧 .NET 9 FIX: Check for users.view permission instead of Admin role
            var hasUsersViewPermission = User.Claims.Any(c => 
                c.Type == "permission" && c.Value == "users.view");
                
            if (!hasUsersViewPermission)
            {
                _logger.LogWarning("User {UserId} attempted to view users for role {RoleId} without users.view permission", 
                    GetCurrentUserId(), id);
                return StatusCode(403, ApiResponseDto<List<UserInfo>>.ErrorResult("You don't have permission to view role users"));
            }

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

    private static RoleDto MapToRoleDto(RoleInfo role)
    {
        return new RoleDto
        {
            Id = role.Id, // 🔧 .NET 9 FIX: Use int directly, not ToString()
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsDefault = role.IsDefault,
            TenantId = role.TenantId, // 🔧 .NET 9 FIX: Use int? directly, not ToString()
            Permissions = role.Permissions.ToList(),
            CreatedAt = role.CreatedAt, // 🔧 .NET 9 FIX: Use DateTime directly, not ToString()
            UpdatedAt = role.UpdatedAt, // 🔧 .NET 9 FIX: Use DateTime directly, not ToString()
            UserCount = 0 // TODO: Calculate actual user count if needed
        };
    }

    #endregion
}
