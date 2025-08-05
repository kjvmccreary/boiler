// FILE: src/shared/DTOs/User/UserUpdateDto.cs
using System.ComponentModel.DataAnnotations;

namespace DTOs.User;

public class UserUpdateDto
{
    [Required]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    public List<string> Roles { get; set; } = new();
}
