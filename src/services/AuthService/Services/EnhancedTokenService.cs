using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Common.Configuration;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Common.Data;

namespace AuthService.Services;

public class EnhancedTokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IPermissionService _permissionService;
    private readonly ILogger<EnhancedTokenService> _logger;
    private readonly ApplicationDbContext _context; // 🔧 ADD: Need database access for UserRoles

    public EnhancedTokenService(
        JwtSettings jwtSettings, 
        IPermissionService permissionService,
        ApplicationDbContext context, // 🔧 ADD: Inject database context
        ILogger<EnhancedTokenService> logger)
    {
        _jwtSettings = jwtSettings;
        _permissionService = permissionService;
        _context = context; // 🔧 ADD: Initialize context
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
            new("tenant_id", tenant.Id.ToString()), // 🔧 FIX: Always use the target tenant ID, not user.TenantId
            new("tenant_name", tenant.Name),
            new("tenant_domain", tenant.Domain ?? string.Empty),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // 🔧 MULTI-ROLE FIX: Use UserRoles table instead of TenantUsers
        try
        {
            var userRoles = await _context.UserRoles
                .IgnoreQueryFilters() // ✅ ADD: Bypass global query filters
                .Where(ur => ur.UserId == user.Id && ur.TenantId == tenant.Id && ur.IsActive)
                .Include(ur => ur.Role)
                .ToListAsync();

            _logger.LogInformation("🔍 JWT: Found {RoleCount} active roles for user {UserId} in tenant {TenantId}", 
                userRoles.Count, user.Id, tenant.Id);

            // Add multiple role claims to JWT
            foreach (var userRole in userRoles)
            {
                if (userRole.Role != null && userRole.Role.IsActive)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
                    _logger.LogInformation("🔍 JWT: Added role claim '{RoleName}' for user {UserId}", 
                        userRole.Role.Name, user.Id);
                }
            }

            // 🔧 FALLBACK: If no UserRoles found, use TenantUsers (legacy support)
            if (!userRoles.Any() && user.TenantUsers != null && user.TenantUsers.Any())
            {
                _logger.LogWarning("🔍 JWT: No UserRoles found for user {UserId}, falling back to TenantUsers", user.Id);
                
                foreach (var tenantUser in user.TenantUsers.Where(tu => tu.IsActive && tu.TenantId == tenant.Id))
                {
                    claims.Add(new Claim(ClaimTypes.Role, tenantUser.Role));
                    _logger.LogInformation("🔍 JWT: Added legacy role claim '{RoleName}' from TenantUsers for user {UserId}", 
                        tenantUser.Role, user.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔍 JWT: Failed to load roles for user {UserId}, using TenantUsers fallback", user.Id);
            
            // Emergency fallback to TenantUsers
            if (user.TenantUsers != null && user.TenantUsers.Any())
            {
                foreach (var tenantUser in user.TenantUsers.Where(tu => tu.IsActive))
                {
                    claims.Add(new Claim(ClaimTypes.Role, tenantUser.Role));
                }
            }
        }

        // Include user permissions in JWT claims for better performance
        try
        {
            // 🔧 FIX: Use tenant-specific permission method instead of HTTP context-dependent one
            var userPermissions = await _permissionService.GetUserPermissionsForTenantAsync(user.Id, tenant.Id);
            foreach (var permission in userPermissions)
            {
                claims.Add(new Claim("permission", permission));
            }
            
            _logger.LogInformation("🔍 JWT: Added {PermissionCount} permissions to JWT for user {UserId} in tenant {TenantId}", 
                userPermissions.Count(), user.Id, tenant.Id);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "🔍 JWT: Failed to load permissions for user {UserId} during token generation", user.Id);
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

        var generatedToken = new JwtSecurityTokenHandler().WriteToken(token);
        
        _logger.LogInformation("🔍 JWT: Generated token for user {UserId} with {RoleCount} roles and {PermissionCount} permissions", 
            user.Id, claims.Count(c => c.Type == ClaimTypes.Role), claims.Count(c => c.Type == "permission"));

        return generatedToken;
    }

    // 🔧 NEW METHOD: Generate JWT without tenant context for initial login
    public Task<string> GenerateAccessTokenWithoutTenantAsync(User user)
    {
        _logger.LogInformation("🔍 TOKEN DEBUG: Generating tenant-less JWT for user {UserId}", user.Id);
        
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
            
            // 🔧 CRITICAL: NO tenant_id claim - this signals tenant selection needed
            // 🔧 CRITICAL: NO role claims yet - will be added after tenant selection
            // 🔧 CRITICAL: NO permission claims yet - will be added after tenant selection
        };

        _logger.LogInformation("🔍 TOKEN DEBUG: Tenant-less token will have {ClaimCount} claims (no tenant/roles/permissions)", claims.Count);
        
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15), // 🔧 Shorter expiry for tenant selection
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        _logger.LogInformation("🔍 TOKEN DEBUG: Tenant-less token generation completed for user {UserId}", user.Id);
        
        return Task.FromResult(tokenString);
    }

    // Keep existing methods unchanged...
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
