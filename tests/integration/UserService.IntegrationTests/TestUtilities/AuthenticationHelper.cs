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
    // JWT settings that EXACTLY match the UserService configuration
    private const string SecretKey = "your-super-secret-jwt-key-that-is-at-least-256-bits-long";
    private const string Issuer = "AuthService";
    private const string Audience = "StarterApp";

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
        // Step 1: Get initial JWT (no tenant)
        var initialJwt = await GetInitialJwtAsync(dbContext, email);

        // Step 2: Get user's available tenants
        var availableTenants = await GetUserTenantsAsync(dbContext, email);
        if (!availableTenants.Any())
        {
            throw new InvalidOperationException($"User {email} has no tenant assignments");
        }

        // Step 3: Select tenant (auto-select if only one, or use preferred)
        var selectedTenantId = preferredTenantId ?? availableTenants.First().Id;
        if (!availableTenants.Any(t => t.Id == selectedTenantId))
        {
            throw new InvalidOperationException($"User {email} does not have access to tenant {selectedTenantId}");
        }

        // Step 4: Simulate calling /api/auth/select-tenant
        return await SimulateTenantSelectionAsync(client, dbContext, initialJwt, email, selectedTenantId);
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

    private static async Task<string> SimulateTenantSelectionAsync(
        HttpClient client,
        ApplicationDbContext dbContext,
        string initialJwt,
        string email,
        int tenantId)
    {
        // Since we're testing UserService, not AuthService, we'll generate the tenant-aware JWT directly
        // In a full E2E test, you would actually call the AuthService endpoint
        
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
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, primaryRole),
            new("tenant_id", tenant.Id.ToString()),
            new("tenant_name", tenant.Name),
            new("tenant_domain", tenant.Domain ?? "default.com"),
            new("current_tenant_id", tenant.Id.ToString()), // 🔧 ADD: Extra claim for tenant context
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email),
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
