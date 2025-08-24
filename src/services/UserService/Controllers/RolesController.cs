using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Contracts.Services;
using DTOs.Common;
using DTOs.Auth;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Authorization; // ✅ ADD: Import our custom authorization
using System.ComponentModel.DataAnnotations;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Base authentication requirement
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    private readonly ILogger<RolesController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly ITenantProvider _tenantProvider;

    public RolesController(
        IRoleService roleService,
        ILogger<RolesController> logger,
        ApplicationDbContext context,
        ITenantProvider tenantProvider)
    {
        _roleService = roleService;
        _logger = logger;
        _context = context;
        _tenantProvider = tenantProvider;
    }

    /// <summary>
    /// Get all roles for the current tenant - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet]
    [RequirePermission("roles.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<PagedResultDto<RoleDto>>>> GetRoles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            // ✅ REMOVED: Manual permission checking - now handled by framework
            // var hasRolesViewPermission = User.Claims.Any(c => 
            //     c.Type == "permission" && c.Value == "roles.view");
            // if (!hasRolesViewPermission) { ... }
            
            // 🔧 ADD: Input validation
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

            // Get user counts efficiently
            var roleIds = roles.Select(r => r.Id).ToList();
            var userCounts = await GetUserCountsForRoles(roleIds);

            var roleDtos = roles.Select(role => {
                var roleDto = MapToRoleDto(role);
                roleDto.UserCount = userCounts.GetValueOrDefault(role.Id, 0);
                return roleDto;
            }).ToList();
            
            var totalCount = roleDtos.Count;
            var pagedItems = roleDtos.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            var pagedResult = new PagedResultDto<RoleDto>(
                items: pagedItems,
                totalCount: totalCount,
                pageNumber: page,
                pageSize: pageSize
            );
            
            return Ok(ApiResponseDto<PagedResultDto<RoleDto>>.SuccessResult(pagedResult, "Roles retrieved successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving roles");
            return StatusCode(500, ApiResponseDto<PagedResultDto<RoleDto>>.ErrorResult("An error occurred while retrieving roles"));
        }
    }

    /// <summary>
    /// Get a specific role by ID - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet("{id:int}")]
    [RequirePermission("roles.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> GetRole(int id)
    {
        try
        {
            // ✅ REMOVED: Manual permission checking
            // var hasRolesViewPermission = User.Claims.Any(...);
            
            if (id <= 0)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            var roleDto = MapToRoleDto(role);
            return Ok(ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role retrieved successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while retrieving the role"));
        }
    }

    /// <summary>
    /// Create a new role - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost]
    [RequirePermission("roles.create")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> CreateRole([FromBody] CreateRoleDto createRoleDto)
    {
        try
        {
            // ✅ REMOVED: Manual permission checking
            // var hasRolesCreatePermission = User.Claims.Any(...);
            
            if (createRoleDto == null)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role data is required"));
            }

            if (string.IsNullOrWhiteSpace(createRoleDto.Name))
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role name is required"));
            }

            if (createRoleDto.Name.Length > 100)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role name cannot exceed 100 characters"));
            }

            if (!string.IsNullOrEmpty(createRoleDto.Description) && createRoleDto.Description.Length > 500)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role description cannot exceed 500 characters"));
            }

            // 🔧 FIX: Sanitize input to prevent XSS
            createRoleDto.Name = System.Net.WebUtility.HtmlEncode(createRoleDto.Name);
            if (!string.IsNullOrEmpty(createRoleDto.Description))
            {
                createRoleDto.Description = System.Net.WebUtility.HtmlEncode(createRoleDto.Description);
            }

            var role = await _roleService.CreateRoleAsync(createRoleDto.Name, createRoleDto.Description ?? "", createRoleDto.Permissions ?? new List<string>());
            var roleDto = MapToRoleDto(role);

            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, 
                ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role created successfully"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            return Conflict(ApiResponseDto<RoleDto>.ErrorResult("A role with this name already exists"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating role");
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while creating the role"));
        }
    }

    /// <summary>
    /// Update a specific role - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPut("{id:int}")]
    [RequirePermission("roles.edit")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<RoleDto>>> UpdateRole(int id, [FromBody] UpdateRoleDto request)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role ID must be greater than 0"));
            }

            if (request == null)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role data is required"));
            }

            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role name is required"));
            }

            if (request.Name.Length > 100)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role name cannot exceed 100 characters"));
            }

            if (!string.IsNullOrEmpty(request.Description) && request.Description.Length > 500)
            {
                return BadRequest(ApiResponseDto<RoleDto>.ErrorResult("Role description cannot exceed 500 characters"));
            }

            // 🔧 FIX: Sanitize input
            request.Name = System.Net.WebUtility.HtmlEncode(request.Name);
            if (!string.IsNullOrEmpty(request.Description))
            {
                request.Description = System.Net.WebUtility.HtmlEncode(request.Description);
            }

            // 🔧 FIX: Check for system role BEFORE tenant access check
            var existingRole = await _roleService.GetRoleByIdAsync(id);
            if (existingRole == null)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            // 🔧 FIX: System role check should come first and return 500
            if (existingRole.IsSystemRole)
            {
                return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("System roles cannot be modified"));
            }

            // Now check tenant access
            if (existingRole.TenantId != await _tenantProvider.GetCurrentTenantIdAsync())
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
            }

            var success = await _roleService.UpdateRoleAsync(id, request.Name, request.Description, request.Permissions);
            if (!success)
            {
                return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found or could not be updated"));
            }

            var updatedRole = await _roleService.GetRoleByIdAsync(id);
            var roleDto = MapToRoleDto(updatedRole!);
            return Ok(ApiResponseDto<RoleDto>.SuccessResult(roleDto, "Role updated successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(ApiResponseDto<RoleDto>.ErrorResult("Role not found"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("System role"))
        {
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("System roles cannot be modified"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
        {
            // 🔧 FIX: Return 409 Conflict for duplicate names instead of 500
            return Conflict(ApiResponseDto<RoleDto>.ErrorResult("A role with this name already exists"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<RoleDto>.ErrorResult("An error occurred while updating the role"));
        }
    }

    /// <summary>
    /// Delete a role - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpDelete("{id:int}")]
    [RequirePermission("roles.delete")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> DeleteRole(int id)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Role ID must be greater than 0"));
            }

            var success = await _roleService.DeleteRoleAsync(id);
            if (!success)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role deleted successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            // 🔧 FIX: Return 404 for cross-tenant attempts
            return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("System roles cannot be deleted"))
        {
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("System roles cannot be deleted"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("has users assigned"))
        {
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Cannot delete role that has users assigned to it"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while deleting the role"));
        }
    }

    /// <summary>
    /// Get permissions for a specific role - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpGet("{id:int}/permissions")]
    [RequirePermission("roles.view")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<List<string>>>> GetRolePermissions(int id)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<List<string>>.ErrorResult("Role ID must be greater than 0"));
            }

            var role = await _roleService.GetRoleWithPermissionsAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<List<string>>.ErrorResult("Role not found"));
            }

            var permissions = role.Permissions.ToList();
            return Ok(ApiResponseDto<List<string>>.SuccessResult(permissions, "Role permissions retrieved successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            // 🔧 FIX: Return 404 for cross-tenant attempts
            return NotFound(ApiResponseDto<List<string>>.ErrorResult("Role not found"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving permissions for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<List<string>>.ErrorResult("An error occurred while retrieving role permissions"));
        }
    }

    /// <summary>
    /// Update permissions for a specific role - SECURE: Multiple permissions required
    /// </summary>
    [HttpPut("{id:int}/permissions")]
    [RequirePermission("roles.edit")]
    [RequirePermission("roles.manage_permissions")] // ✅ SECURE: Can stack multiple requirements
    public async Task<ActionResult<ApiResponseDto<bool>>> UpdateRolePermissions(int id, [FromBody] List<string> permissions)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Role ID must be greater than 0"));
            }

            if (permissions == null)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Permissions list is required"));
            }

            // 🔧 FIX: Check if role exists and belongs to tenant before updating
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            // 🔧 FIX: Check if it's a system role with consistent message
            if (role.IsSystemRole)
            {
                return StatusCode(500, ApiResponseDto<bool>.ErrorResult("System roles cannot be modified"));
            }

            var success = await _roleService.UpdateRolePermissionsAsync(id, permissions);
            if (!success)
            {
                return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
            }

            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role permissions updated successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            return NotFound(ApiResponseDto<bool>.ErrorResult("Role not found"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("System role permissions cannot be modified"))
        {
            // 🔧 FIX: Convert service message to consistent controller message
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("System roles cannot be modified"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating permissions for role {RoleId}", id);
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("An error occurred while updating role permissions"));
        }
    }

    /// <summary>
    /// Assign a role to a user - SECURE: Framework-enforced permission check
    /// </summary>
    [HttpPost("assign")]
    [RequirePermission("users.manage_roles")] // ✅ SECURE: Framework-enforced permission check
    public async Task<ActionResult<ApiResponseDto<bool>>> AssignRoleToUser([FromBody] AssignRoleDto assignRoleDto)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (assignRoleDto == null)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Assignment data is required"));
            }

            if (assignRoleDto.UserId <= 0)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("User ID must be greater than 0"));
            }

            if (assignRoleDto.RoleId <= 0)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Role ID must be greater than 0"));
            }

            await _roleService.AssignRoleToUserAsync(assignRoleDto.UserId, assignRoleDto.RoleId);
            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role assigned to user successfully"));
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("Role not found"))
        {
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Role not found or access denied"));
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
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<bool>>> RemoveRoleFromUser(int roleId, int userId)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (roleId <= 0)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("Role ID must be greater than 0"));
            }

            if (userId <= 0)
            {
                return BadRequest(ApiResponseDto<bool>.ErrorResult("User ID must be greater than 0"));
            }

            // 🔧 NEW: Add cross-tenant security check BEFORE calling service
            var role = await _roleService.GetRoleByIdAsync(roleId);
            if (role == null)
            {
                return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Role not found or access denied"));
            }

            await _roleService.RemoveRoleFromUserAsync(userId, roleId);
            return Ok(ApiResponseDto<bool>.SuccessResult(true, "Role removed from user successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            // 🔧 NEW: Handle cross-tenant access attempts
            return StatusCode(500, ApiResponseDto<bool>.ErrorResult("Role not found or access denied"));
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
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<List<RoleDto>>>> GetUserRoles(int userId)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (userId <= 0)
            {
                return BadRequest(ApiResponseDto<List<RoleDto>>.ErrorResult("User ID must be greater than 0"));
            }

            var currentUserId = GetCurrentUserId();
            
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
    [Authorize]
    public async Task<ActionResult<ApiResponseDto<List<UserInfo>>>> GetRoleUsers(int id)
    {
        try
        {
            // 🔧 FIX: Add input validation
            if (id <= 0)
            {
                return BadRequest(ApiResponseDto<List<UserInfo>>.ErrorResult("Role ID must be greater than 0"));
            }

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
        catch (UnauthorizedAccessException)
        {
            // 🔧 FIX: Return 404 for cross-tenant attempts
            return NotFound(ApiResponseDto<List<UserInfo>>.ErrorResult("Role not found"));
        }
        catch (ArgumentException)
        {
            // 🔧 FIX: Return 404 for non-existent roles
            return NotFound(ApiResponseDto<List<UserInfo>>.ErrorResult("Role not found"));
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

    private async Task<Dictionary<int, int>> GetUserCountsForRoles(List<int> roleIds)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return new Dictionary<int, int>();
            }

            var userCounts = await _context.UserRoles
                .Where(ur => roleIds.Contains(ur.RoleId) && ur.TenantId == tenantId.Value && ur.IsActive)
                .GroupBy(ur => ur.RoleId)
                .Select(g => new { RoleId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.RoleId, x => x.Count);

            return userCounts;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user counts for roles");
            return new Dictionary<int, int>();
        }
    }

    private static RoleDto MapToRoleDto(RoleInfo role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name,
            Description = role.Description,
            IsSystemRole = role.IsSystemRole,
            IsDefault = role.IsDefault,
            TenantId = role.TenantId,
            Permissions = role.Permissions.ToList(),
            CreatedAt = role.CreatedAt,
            UpdatedAt = role.UpdatedAt,
            UserCount = 0
        };
    }

    #endregion
}
