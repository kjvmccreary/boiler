using System.ComponentModel.DataAnnotations;

namespace DTOs.Auth;

public class SwitchTenantDto
{
    [Required]
    public int TenantId { get; set; }
}
