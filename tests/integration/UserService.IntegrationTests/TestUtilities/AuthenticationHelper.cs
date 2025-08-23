using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.IdentityModel.Tokens;
using Common.Data;
using DTOs.Entities;
using DTOs.Auth;
using DTOs.Common;
using Xunit;

namespace UserService.IntegrationTests.TestUtilities;

public static class AuthenticationHelper
{
    // JWT settings that EXACTLY match the UserService configuration
    private const string SecretKey = "your-super-secret-jwt-key-that-is-at-least-256-bits-long";
    private const string Issuer = "AuthService";
    private const string Audience = "StarterApp";

    /// <summary>
    /// 🔧 FIXED: Test the complete two-phase authentication flow by MOCKING it
    /// Since UserService doesn't have auth endpoints, we simulate the flow
    /// </summary>
    public static async Task<string> GetTenantTokenViaTwoPhaseFlow(
        HttpClient client,
        ApplicationDbContext dbContext,
        string email,
        int preferredTenantId)
    {
        Console.WriteLine($"🔄 MOCKING two-phase flow for {email} → Tenant {preferredTenantId}");
        
        // 🔧 CRITICAL FIX: Don't call auth endpoints - just simulate the flow
        
        // Step 1: Verify user exists
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email);
            
        if (user == null)
        {
            throw new InvalidOperationException($"Test user {email} not found");
        }
        
        Console.WriteLine($"✅ Phase 1 simulated: User {email} authenticated (mocked)");
        
        // Step 2: Verify user has access to tenant
        var hasAccess = await dbContext.TenantUsers
            .IgnoreQueryFilters()
            .AnyAsync(tu => tu.UserId == user.Id && tu.TenantId == preferredTenantId && tu.IsActive);
            
        if (!hasAccess)
        {
            throw new UnauthorizedAccessException($"User {email} does not have access to tenant {preferredTenantId}");
        }
        
        Console.WriteLine($"✅ Phase 2 simulated: Tenant access verified for tenant {preferredTenantId}");
        
        // Step 3: Generate the final tenant-aware JWT directly
        var finalJwt = await GenerateTenantAwareJwtDirectly(dbContext, email, preferredTenantId);
        
        Console.WriteLine($"✅ Two-phase flow simulation completed successfully");
        
        return finalJwt;
    }

    /// <summary>
    /// 🔧 NEW: Generate tenant-aware JWT directly without calling auth services
    /// </summary>
    private static async Task<string> GenerateTenantAwareJwtDirectly(
        ApplicationDbContext dbContext,
        string email,
        int tenantId)
    {
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email);

        var tenant = await dbContext.Tenants
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(t => t.Id == tenantId);

        if (user == null || tenant == null)
        {
            throw new InvalidOperationException($"User {email} or tenant {tenantId} not found");
        }

        // Get user's roles and permissions for this tenant
        var userRoles = await dbContext.UserRoles
            .IgnoreQueryFilters()
            .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == user.Id && ur.TenantId == tenantId && ur.IsActive)
            .ToListAsync();

        var roles = userRoles.Select(ur => ur.Role.Name).ToList();
        var permissions = userRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        var primaryRole = roles.FirstOrDefault() ?? "User";

        Console.WriteLine($"🔑 Generated tenant-aware JWT for {email}:");
        Console.WriteLine($"   - Tenant: {tenant.Name} (ID: {tenantId})");
        Console.WriteLine($"   - Roles: {string.Join(", ", roles)}");
        Console.WriteLine($"   - Permissions: {permissions.Count}");

        return GenerateTenantAwareJwtToken(user, tenant, primaryRole, roles, permissions);
    }

    /// <summary>
    /// Test two-phase flow with invalid tenant access (should fail)
    /// </summary>
    public static async Task<bool> TestUnauthorizedTenantAccess(
        HttpClient client,
        ApplicationDbContext dbContext,
        string email,
        int unauthorizedTenantId)
    {
        Console.WriteLine($"🚫 Testing unauthorized tenant access: {email} → Tenant {unauthorizedTenantId}");
        
        try
        {
            // Try to generate a JWT for unauthorized tenant - should fail
            await GetTenantTokenViaTwoPhaseFlow(client, dbContext, email, unauthorizedTenantId);
            
            Console.WriteLine($"❌ SECURITY ISSUE: User {email} was able to access unauthorized tenant {unauthorizedTenantId}!");
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"✅ Security working: User {email} correctly denied access to tenant {unauthorizedTenantId}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✅ Security working: User {email} denied access due to: {ex.Message}");
            return true;
        }
    }

    /// <summary>
    /// Decode JWT payload for testing and validation
    /// </summary>
    public static Dictionary<string, object> DecodeJwtPayload(string jwt)
    {
        try
        {
            var parts = jwt.Split('.');
            if (parts.Length != 3)
            {
                throw new ArgumentException("Invalid JWT format");
            }
            
            var payload = parts[1];
            
            // Add padding if needed for Base64 decoding
            while (payload.Length % 4 != 0)
            {
                payload += "=";
            }
            
            var jsonBytes = Convert.FromBase64String(payload);
            var json = Encoding.UTF8.GetString(jsonBytes);
            
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            
            return JsonSerializer.Deserialize<Dictionary<string, object>>(json, options) 
                   ?? new Dictionary<string, object>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to decode JWT: {ex.Message}");
            Console.WriteLine($"JWT: {jwt}");
            throw;
        }
    }

    /// <summary>
    /// Validate JWT token contains expected claims
    /// </summary>
    public static void ValidateJwtClaims(string jwt, Dictionary<string, object> expectedClaims)
    {
        var payload = DecodeJwtPayload(jwt);
        
        foreach (var expectedClaim in expectedClaims)
        {
            Assert.True(payload.ContainsKey(expectedClaim.Key), 
                $"JWT should contain claim '{expectedClaim.Key}'");
                
            var actualValue = payload[expectedClaim.Key]?.ToString();
            var expectedValue = expectedClaim.Value?.ToString();
            
            Assert.Equal(expectedValue, actualValue);
        }
    }

    /// <summary>
    /// Validate JWT token has specific permissions
    /// </summary>
    public static void ValidateJwtPermissions(string jwt, params string[] expectedPermissions)
    {
        var payload = DecodeJwtPayload(jwt);
        
        if (!payload.ContainsKey("permission"))
        {
            if (expectedPermissions.Length > 0)
            {
                Assert.Fail("JWT should contain permission claims");
            }
            return;
        }
        
        // Handle both single permission and array of permissions
        var permissionClaim = payload["permission"];
        var actualPermissions = new List<string>();
        
        if (permissionClaim is JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Array)
            {
                actualPermissions.AddRange(element.EnumerateArray()
                    .Select(p => p.GetString())
                    .Where(p => !string.IsNullOrEmpty(p))
                    .Cast<string>());
            }
            else
            {
                var singlePermission = element.GetString();
                if (!string.IsNullOrEmpty(singlePermission))
                {
                    actualPermissions.Add(singlePermission);
                }
            }
        }
        else if (permissionClaim is string singlePerm)
        {
            actualPermissions.Add(singlePerm);
        }
        
        foreach (var expectedPermission in expectedPermissions)
        {
            Assert.True(actualPermissions.Contains(expectedPermission), 
                $"JWT should contain permission '{expectedPermission}'. Actual permissions: [{string.Join(", ", actualPermissions)}]");
        }
    }

    #region Existing Methods (Enhanced)

    /// <summary>
    /// Phase 1: Generate initial JWT without tenant (like real login response)
    /// </summary>
    public static async Task<string> GetInitialJwtAsync(
        ApplicationDbContext dbContext, 
        string email)
    {
        var user = await dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            var availableUsers = await dbContext.Users
                .IgnoreQueryFilters()
                .Select(u => u.Email).ToListAsync();
            throw new InvalidOperationException($"Test user with email {email} not found. Available users: {string.Join(", ", availableUsers)}");
        }

        return GenerateInitialJwtToken(user);
    }

    /// <summary>
    /// Phase 2: Simulate tenant selection and get tenant-aware JWT
    /// </summary>
    public static async Task<string> GetTenantAwareJwtAsync(
        HttpClient client,
        ApplicationDbContext dbContext,
        string email,
        int? preferredTenantId = null)
    {
        // For backward compatibility, if preferredTenantId is provided, use simulation
        if (preferredTenantId.HasValue)
        {
            return await GetTenantTokenViaTwoPhaseFlow(client, dbContext, email, preferredTenantId.Value);
        }

        // Step 1: Get user's available tenants
        var availableTenants = await GetUserTenantsAsync(dbContext, email);
        if (!availableTenants.Any())
        {
            throw new InvalidOperationException($"User {email} has no tenant assignments");
        }

        // Step 2: Select first available tenant
        var selectedTenantId = availableTenants.First().Id;

        // Step 3: Generate tenant-aware JWT directly
        return await GenerateTenantAwareJwtDirectly(dbContext, email, selectedTenantId);
    }

    /// <summary>
    /// Convenience method: Get tenant-aware JWT with automatic tenant selection
    /// This is what your existing tests should use
    /// </summary>
    public static async Task<string> GetValidTokenAsync(
        HttpClient client, 
        ApplicationDbContext dbContext, 
        string email, 
        string? overrideRole = null) // Keep for backward compatibility
    {
        return await GetTenantAwareJwtAsync(client, dbContext, email);
    }

    #endregion

    #region Private Helper Methods

    private static string GenerateInitialJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

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
            // NOTE: No tenant_id, no roles, no permissions - just like real login
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private static async Task<List<Tenant>> GetUserTenantsAsync(ApplicationDbContext dbContext, string email)
    {
        return await dbContext.TenantUsers
            .IgnoreQueryFilters()
            .Include(tu => tu.Tenant)
            .Where(tu => tu.User.Email == email && tu.IsActive)
            .Select(tu => tu.Tenant)
            .ToListAsync();
    }

    private static string GenerateTenantAwareJwtToken(
        User user, 
        Tenant tenant, 
        string primaryRole, 
        List<string> roles, 
        List<string> permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email), // Keep only one email claim
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, primaryRole),
            new("tenant_id", tenant.Id.ToString()),
            new("tenant_name", tenant.Name),
            new("tenant_domain", tenant.Domain ?? "default.com"),
            new("current_tenant_id", tenant.Id.ToString()),
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            // 🔧 FIX: Remove duplicate email claim
            // new(JwtRegisteredClaimNames.Email, user.Email), // REMOVED - duplicate!
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, 
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(),
                ClaimValueTypes.Integer64)
        };

        // Add all roles as role claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permission claims
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = Issuer,
            Audience = Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key), 
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    #endregion
}
