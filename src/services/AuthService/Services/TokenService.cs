// FILE: src/services/AuthService/Services/TokenService.cs
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Configuration;
using Common.Data; // 🔧 .NET 9 FIX: Add missing using directive
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore; // 🔧 .NET 9 FIX: Add missing using directive
using Microsoft.Extensions.DependencyInjection; // 🔧 .NET 9 FIX: Add missing using directive
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(User user, Tenant tenant);
    string GenerateRefreshToken();
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    Task<RefreshToken> CreateRefreshTokenAsync(User user);
}

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        JwtSettings jwtSettings, 
        IServiceProvider serviceProvider,
        ILogger<TokenService> logger)
    {
        _jwtSettings = jwtSettings;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task<string> GenerateAccessTokenAsync(User user, Tenant tenant)
    {
        _logger.LogInformation("🔍 TOKEN DEBUG: Starting token generation for user {UserId} in tenant {TenantId}", user.Id, tenant.Id);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new("tenant_id", tenant.Id.ToString()), // 🔧 FIX: Always use the passed tenant ID
            new("tenant_name", tenant.Name),
            new("tenant_domain", tenant.Domain ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // 🔧 .NET 9 FIX: Enhanced role inclusion with debugging
        var rolesAdded = 0;
        if (user.TenantUsers != null && user.TenantUsers.Any())
        {
            foreach (var tenantUser in user.TenantUsers.Where(tu => tu.IsActive))
            {
                claims.Add(new Claim(ClaimTypes.Role, tenantUser.Role));
                rolesAdded++;
                _logger.LogInformation("🔍 TOKEN DEBUG: Added role claim '{Role}' for user {UserId}", tenantUser.Role, user.Id);
            }
        }
        else
        {
            _logger.LogWarning("🔍 TOKEN DEBUG: No TenantUsers found for user {UserId}", user.Id);
            
            // 🔧 .NET 9 FIX: Alternative - try to load roles from UserRoles table directly
            _logger.LogInformation("🔍 TOKEN DEBUG: Attempting to load roles from UserRoles table as fallback...");
            
            // This is a fallback - ideally the TenantUsers should be loaded
            // But let's see if we can get roles from UserRoles table
            try
            {
                // 🔧 .NET 9 FIX: Proper service resolution
                using var scope = _serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
                if (context != null)
                {
                    var userRoles = await context.UserRoles
                        .Where(ur => ur.UserId == user.Id && ur.IsActive && ur.TenantId == tenant.Id)
                        .Include(ur => ur.Role)
                        .ToListAsync();
                    
                    _logger.LogInformation("🔍 TOKEN DEBUG: Found {Count} UserRoles for user {UserId}", userRoles.Count, user.Id);
                    
                    foreach (var userRole in userRoles)
                    {
                        if (userRole.Role != null)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                            rolesAdded++;
                            _logger.LogInformation("🔍 TOKEN DEBUG: Added role claim '{Role}' from UserRoles for user {UserId}", userRole.Role.Name, user.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "🔍 TOKEN DEBUG: Failed to load roles from UserRoles table");
            }
        }
        
        _logger.LogInformation("🔍 TOKEN DEBUG: Total roles added to token: {RolesCount}", rolesAdded);

        // NEW: Include user permissions in JWT claims for Enhanced Phase 4
        try
        {
            _logger.LogInformation("🔍 TOKEN DEBUG: Attempting to resolve IPermissionService for user {UserId}", user.Id);
            
            // 🔧 .NET 9 FIX: Proper service resolution with scope
            using var scope = _serviceProvider.CreateScope();
            var permissionService = scope.ServiceProvider.GetService<IPermissionService>();
            if (permissionService != null)
            {
                _logger.LogInformation("🔍 TOKEN DEBUG: IPermissionService resolved successfully, getting permissions for user {UserId} in tenant {TenantId}", user.Id, tenant.Id);
                
                // FIXED: Use the tenant-specific method instead of the tenant-provider dependent method
                var userPermissions = await permissionService.GetUserPermissionsForTenantAsync(user.Id, tenant.Id);
                _logger.LogInformation("🔍 TOKEN DEBUG: Retrieved {PermissionCount} permissions for user {UserId}: {Permissions}", 
                    userPermissions.Count(), user.Id, string.Join(", ", userPermissions.Take(10))); // Show first 10 permissions
                
                foreach (var permission in userPermissions)
                {
                    claims.Add(new Claim("permission", permission));
                }
                
                _logger.LogInformation("🔍 TOKEN DEBUG: Successfully added {PermissionCount} permission claims to JWT for user {UserId}", userPermissions.Count(), user.Id);
            }
            else
            {
                _logger.LogWarning("🔍 TOKEN DEBUG: IPermissionService is NULL - service not available during token generation for user {UserId}", user.Id);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔍 TOKEN DEBUG: FAILED to load permissions for user {UserId} during token generation", user.Id);
        }

        _logger.LogInformation("🔍 TOKEN DEBUG: Total claims count before token generation: {ClaimCount}", claims.Count);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("🔍 TOKEN DEBUG: Token generation completed for user {UserId}", user.Id);
        
        return tokenString;
    }

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

    public async Task<RefreshToken> CreateRefreshTokenAsync(User user)
    {
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = GenerateRefreshToken(),
            ExpiryDate = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays),
            IsRevoked = false
        };

        return await Task.FromResult(refreshToken); // 🔧 .NET 9 FIX: Add await to remove warning
    }
}
