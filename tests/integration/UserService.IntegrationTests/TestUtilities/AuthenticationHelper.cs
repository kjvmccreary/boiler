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

    public static async Task<string> GetValidTokenAsync(
        HttpClient client, 
        ApplicationDbContext dbContext, 
        string email, 
        string? overrideRole = null) // ✅ RBAC FIX: Make role optional
    {
        // ✅ CRITICAL DEBUGGING: First check if UserRoles exist at all
        var totalUserRoles = await dbContext.UserRoles.CountAsync();
        var userRolesForEmail = await dbContext.UserRoles
            .Where(ur => ur.User.Email == email)
            .CountAsync();
        var activeUserRolesForEmail = await dbContext.UserRoles
            .Where(ur => ur.User.Email == email && ur.IsActive)
            .CountAsync();

        Console.WriteLine($"🔍 DEBUGGING UserRole Query for {email}:");
        Console.WriteLine($"   - Total UserRoles in DB: {totalUserRoles}");
        Console.WriteLine($"   - UserRoles for this email: {userRolesForEmail}");
        Console.WriteLine($"   - Active UserRoles for this email: {activeUserRolesForEmail}");

        // ✅ CRITICAL FIX: Load user first, then load UserRoles separately to avoid Include() filtering issues
        var user = await dbContext.Users
            .Include(u => u.PrimaryTenant)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            var availableUsers = await dbContext.Users.Select(u => u.Email).ToListAsync();
            throw new InvalidOperationException($"Test user with email {email} not found. Available users: {string.Join(", ", availableUsers)}");
        }

        // ✅ CRITICAL FIX: Load UserRoles separately to avoid EF Core Include filtering issues
        var userRoles = await dbContext.UserRoles
            .Include(ur => ur.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
            .Where(ur => ur.UserId == user.Id && ur.IsActive)
            .ToListAsync();

        Console.WriteLine($"🔍 USER ENTITY DEBUG for {email}:");
        Console.WriteLine($"   - User ID: {user.Id}");
        Console.WriteLine($"   - User TenantId: {user.TenantId}");
        Console.WriteLine($"   - Separately loaded UserRoles count: {userRoles.Count}");
        Console.WriteLine($"   - UserRoles details: {string.Join(", ", userRoles.Select(ur => $"RoleId={ur.RoleId}, TenantId={ur.TenantId}, Active={ur.IsActive}"))}");

        // ✅ CRITICAL FIX: Ensure tenant is properly loaded
        var tenant = user.PrimaryTenant;
        if (tenant == null && user.TenantId.HasValue)
        {
            tenant = await dbContext.Tenants
                .FirstOrDefaultAsync(t => t.Id == user.TenantId.Value);
        }
        
        if (tenant == null)
        {
            throw new InvalidOperationException($"User {email} must have a primary tenant. User.TenantId={user.TenantId}");
        }

        Console.WriteLine($"🔍 TENANT DEBUG:");
        Console.WriteLine($"   - Tenant ID: {tenant.Id}");
        Console.WriteLine($"   - Tenant Name: {tenant.Name}");

        // ✅ CRITICAL FIX: Filter UserRoles by tenant
        var relevantUserRoles = userRoles
            .Where(ur => ur.TenantId == tenant.Id)
            .ToList();

        Console.WriteLine($"🔍 FILTERING DEBUG:");
        Console.WriteLine($"   - Total loaded UserRoles: {userRoles.Count}");
        Console.WriteLine($"   - UserRoles after TenantId filter: {relevantUserRoles.Count}");
        Console.WriteLine($"   - Relevant UserRoles: {string.Join(", ", relevantUserRoles.Select(ur => $"RoleId={ur.RoleId}, RoleName={ur.Role?.Name}"))}");

        var permissions = relevantUserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToList();

        Console.WriteLine($"🔍 USER {email} in TENANT {tenant.Id}:");
        Console.WriteLine($"   - UserRoles: {relevantUserRoles.Count}");
        Console.WriteLine($"   - Permissions: {permissions.Count} [{string.Join(", ", permissions)}]");

        // ✅ RBAC FIX: Use actual roles from RBAC system
        var actualRoles = relevantUserRoles.Select(ur => ur.Role.Name).ToList();
        var primaryRole = actualRoles.FirstOrDefault() ?? "User";
        
        // ✅ RBAC FIX: Only override role for specific test scenarios
        var roleToUse = overrideRole ?? primaryRole;
        
        // ✅ RBAC FIX: Warn if overriding role for debugging
        if (overrideRole != null && !actualRoles.Contains(overrideRole))
        {
            Console.WriteLine($"⚠️ WARNING: Overriding role to {overrideRole} but user actually has: {string.Join(", ", actualRoles)}");
        }

        Console.WriteLine($"🔑 JWT CLAIMS for {email}:");
        Console.WriteLine($"   - Role: {roleToUse} (Actual: {string.Join(", ", actualRoles)})");
        Console.WriteLine($"   - Tenant: {tenant.Name} (ID: {tenant.Id})");
        Console.WriteLine($"   - Permissions: {permissions.Count}");

        return GenerateJwtToken(user, tenant, roleToUse, permissions);
    }

    // ✅ FIX: Also support multiple roles (as per RBAC migration document)
    public static string GenerateJwtToken(User user, Tenant tenant, string primaryRole, List<string> permissions)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(SecretKey);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.GivenName, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.Role, primaryRole), // Primary role for compatibility
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

        // ✅ CRITICAL: Add permission claims that controllers expect
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        Console.WriteLine($"🔑 JWT CLAIMS for {user.Email}:");
        Console.WriteLine($"   - Role: {primaryRole}");
        Console.WriteLine($"   - Tenant: {tenant.Name} (ID: {tenant.Id})");
        Console.WriteLine($"   - Permissions: {permissions.Count}");

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
}
