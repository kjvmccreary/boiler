using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace UserService.PerformanceTests.TestUtilities;

public static class JwtTokenGenerator
{
    // ✅ FIX: Match the exact JWT configuration from UserService appsettings.json
    private static readonly string SecretKey = "your-super-secret-jwt-key-that-is-at-least-256-bits-long";
    private static readonly string Issuer = "AuthService";
    private static readonly string Audience = "StarterApp";

    public static string GenerateToken(int userId, string email, int tenantId, string[] roles, string[] permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey); // ✅ FIX: Use UTF8 encoding to match production
        
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            new("tenant_id", tenantId.ToString()),
            new("user_id", userId.ToString())
        };

        // Add roles as both 'role' and ClaimTypes.Role for compatibility
        foreach (var role in roles)
        {
            claims.Add(new Claim("role", role));
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            IssuedAt = DateTime.UtcNow,
            NotBefore = DateTime.UtcNow,
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateAdminToken(int tenantId = 1)
    {
        var permissions = new[]
        {
            "users.view", "users.create", "users.edit", "users.delete",
            "roles.view", "roles.create", "roles.edit", "roles.delete", "roles.assign",
            "permissions.view", "settings.view", "settings.edit", "reports.view"
        };

        return GenerateToken(1, $"admin@tenant{tenantId}.com", tenantId, new[] { "Admin" }, permissions);
    }

    public static string GenerateUserToken(int userId, int tenantId = 1)
    {
        var permissions = new[] { "users.view", "reports.view" };
        return GenerateToken(userId, $"user{userId}@tenant{tenantId}.com", tenantId, new[] { "User" }, permissions);
    }

    public static string GenerateSuperAdminToken()
    {
        var permissions = new[]
        {
            "users.view", "users.create", "users.edit", "users.delete",
            "roles.view", "roles.create", "roles.edit", "roles.delete", "roles.assign",
            "permissions.view", "settings.view", "settings.edit", "reports.view",
            "audit.view", "tenants.view", "tenants.create", "tenants.edit", "tenants.delete"
        };

        return GenerateToken(999, "superadmin@system.com", 1, new[] { "SuperAdmin" }, permissions);
    }
}
