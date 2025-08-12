// FILE: src/shared/DTOs/User/UpdateUserStatusDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

/// <summary>
/// DTO for updating user account status
/// </summary>
public class UpdateUserStatusDto
{
    [Required]
    public bool IsActive { get; set; }
    
    public string? Reason { get; set; }
}
