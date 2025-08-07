using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Configuration;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public class EnhancedTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<EnhancedTokenService> _logger;

    public EnhancedTokenService(
        JwtSettings jwtSettings, 
        IPermissionService permissionService,
        ILogger<EnhancedTokenService> logger)
    {
        _jwtSettings = jwtSettings;
        _permissionService = permissionService;
        _logger = logger;
    }

    public async Task<string> GenerateAccessTokenAsync(User user, Tenant tenant)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("tenant_id", user.TenantId?.ToString() ?? tenant.Id.ToString()),
            new("tenant_name", tenant.Name),
            new("tenant_domain", tenant.Domain ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // NEW: Include user roles from TenantUsers relationship
        if (user.TenantUsers != null && user.TenantUsers.Any())
        {
            foreach (var tenantUser in user.TenantUsers.Where(tu => tu.IsActive))
            {
                claims.Add(new Claim(ClaimTypes.Role, tenantUser.Role));
            }
        }

        // NEW: Include user permissions in JWT claims for better performance
        try
        {
            var userPermissions = await _permissionService.GetUserPermissionsAsync(user.Id);
            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim("permission", permission));
            }
            
            _logger.LogDebug("Added {PermissionCount} permissions to JWT for user {UserId}", userPermissions.Count(), user.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to load permissions for user {UserId} during token generation", user.Id);
            // Continue without permissions - they can be loaded dynamically if needed
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Keep existing methods unchanged
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = _jwtSettings.ValidateAudience,
                ValidateIssuer = _jwtSettings.ValidateIssuer,
                ValidateIssuerSigningKey = _jwtSettings.ValidateIssuerSigningKey,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey)),
                ValidateLifetime = false,
                ValidIssuer = _jwtSettings.Issuer,
                ValidAudience = _jwtSettings.Audience
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to validate expired token");
            return null;
        }
    }

    public Task<RefreshToken> CreateRefreshTokenAsync(User user)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false
        };

        return Task.FromResult(refreshToken);
    }
}
