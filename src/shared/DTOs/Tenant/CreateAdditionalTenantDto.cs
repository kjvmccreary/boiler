using System.ComponentModel.DataAnnotations;

namespace DTOs.Tenant;

/// <summary>
/// DTO for authenticated users creating additional tenants
/// </summary>
public class CreateAdditionalTenantDto
{
    [Required(ErrorMessage = "Organization name is required")]
    [StringLength(255, ErrorMessage = "Organization name must not exceed 255 characters")]
    public required string TenantName { get; set; }
    
    [StringLength(255, ErrorMessage = "Domain must not exceed 255 characters")]
    public string? TenantDomain { get; set; }
}
