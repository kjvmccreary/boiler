// FILE: src/shared/DTOs/User/AssignUserRoleDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

/// <summary>
/// DTO for assigning a role to a user (used by UsersController)
/// This is different from AssignRoleDto which is used by RolesController
/// </summary>
public class AssignUserRoleDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Role ID must be a positive integer")]
    public int RoleId { get; set; }
    
    public DateTime? ExpiresAt { get; set; }
    public string? AssignedBy { get; set; }
}
