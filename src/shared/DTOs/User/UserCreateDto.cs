//// FILE: src/shared/DTOs/User/UserCreateDto.cs
//using System.ComponentModel.DataAnnotations;

//namespace DTOs.User;

//public class UserCreateDto
//{
//    [Required]
//    [EmailAddress]
//    [StringLength(255)]
//    public string Email { get; set; } = string.Empty;

//    [Required]
//    [StringLength(100)]
//    public string FirstName { get; set; } = string.Empty;

//    [Required]
//    [StringLength(100)]
//    public string LastName { get; set; } = string.Empty;

//    [Required]
//    [StringLength(100, MinimumLength = 8)]
//    public string Password { get; set; } = string.Empty;

//    [Required]
//    [Compare("Password")]
//    public string ConfirmPassword { get; set; } = string.Empty;

//    public List<string> Roles { get; set; } = new();
//}
