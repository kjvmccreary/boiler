using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Common.Data;
using DTOs.Entities;

namespace UserService.IntegrationTests.TestUtilities;

public static class AuthenticationHelper
{
    // FIXED: Use the EXACT same JWT settings as the UserService application
    private const string SecretKey = "your-super-secret-jwt-key-that-is-at-least-256-bits-long";
    private const string Issuer = "AuthService";
    private const string Audience = "StarterApp";

    public static async Task<string> GetValidTokenAsync(
        HttpClient client, 
        ApplicationDbContext dbContext, 
        string email, 
        string role)
    {
        // Find or create test user
        var user = await dbContext.Users
            .Include(u => u.PrimaryTenant)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            throw new InvalidOperationException($"Test user with email {email} not found. Ensure test data is seeded.");
        }

        var tenant = user.PrimaryTenant ?? throw new InvalidOperationException("User must have a primary tenant.");

        return GenerateJwtToken(user, tenant, role);
    }

    public static string GenerateJwtToken(User user, Tenant tenant, string role)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, role),
            new("tenant_id", tenant.Id.ToString()),
            new("tenant_name", tenant.Name),
            new("tenant_domain", tenant.Domain ?? "default.com"),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,      // Now matches UserService appsettings.json
            Audience = Audience,  // Now matches UserService appsettings.json
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
