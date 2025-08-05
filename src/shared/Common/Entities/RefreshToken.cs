// FILE: src/shared/Common/Entities/RefreshToken.cs
namespace Common.Entities;

public class RefreshToken : TenantEntity
{
    public int UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiryDate { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public string? DeviceInfo { get; set; }

    public bool IsExpired => DateTime.UtcNow >= ExpiryDate;
    public bool IsActive => !IsRevoked && !IsExpired;

    // Navigation properties
    public User User { get; set; } = null!;
    public Tenant Tenant { get; set; } = null!;
}
