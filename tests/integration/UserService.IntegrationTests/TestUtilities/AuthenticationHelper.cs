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
        var totalUserRoles = await dbContext.UserRoles.IgnoreQueryFilters().CountAsync(); // ✅ FIXED
        var userRolesForEmail = await dbContext.UserRoles
            .IgnoreQueryFilters() // ✅ FIXED
            .Where(ur => ur.User.Email == email)
            .CountAsync();
        var activeUserRolesForEmail = await dbContext.UserRoles
            .IgnoreQueryFilters() // ✅ FIXED
            .Where(ur => ur.User.Email == email && ur.IsActive)
            .CountAsync();

        Console.WriteLine($"🔍 DEBUGGING UserRole Query for {email}:");
        Console.WriteLine($"   - Total UserRoles in DB: {totalUserRoles}");
        Console.WriteLine($"   - UserRoles for this email: {userRolesForEmail}");
        Console.WriteLine($"   - Active UserRoles for this email: {activeUserRolesForEmail}");

        // ✅ CRITICAL FIX: Use IgnoreQueryFilters to access ALL users regardless of tenant context
        var user = await dbContext.Users
            .IgnoreQueryFilters() // ✅ CRITICAL FIX
            .Include(u => u.PrimaryTenant)
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            var availableUsers = await dbContext.Users
                .IgnoreQueryFilters() // ✅ CRITICAL FIX
                .Select(u => u.Email).ToListAsync();
            throw new InvalidOperationException($"Test user with email {email} not found. Available users: {string.Join(", ", availableUsers)}");
        }

        // ✅ CRITICAL FIX: Use IgnoreQueryFilters for UserRoles lookup
        var userRoles = await dbContext.UserRoles
            .IgnoreQueryFilters() // ✅ CRITICAL FIX
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

        // ✅ CRITICAL FIX: Use IgnoreQueryFilters for tenant lookup
        var tenant = user.PrimaryTenant;
        if (tenant == null && user.TenantId.HasValue)
        {
            tenant = await dbContext.Tenants
                .IgnoreQueryFilters() // ✅ CRITICAL FIX (though Tenants probably don't have query filters)
                .FirstOrDefaultAsync(t => t.Id == user.TenantId.Value);
        }
        
        if (tenant == null)
        {
            throw new InvalidOperationException($"User {email} must have a primary tenant. User.TenantId={user.TenantId}");
        }

        Console.WriteLine($"🔍 TENANT DEBUG:");
        Console.WriteLine($"   - Tenant ID: {tenant.Id}");
        Console.WriteLine($"   - Tenant Name: {tenant.Name}");

        // ✅ CRITICAL FIX: Filter UserRoles by tenant (but we now have access to all user roles)
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

        // ✅ ENHANCED DEBUG: Special logging for Tenant 2 Admin
        if (email == "admin@tenant2.com")
        {
            Console.WriteLine($"🔍🔍🔍 SPECIAL DEBUG FOR TENANT 2 ADMIN 🔍🔍🔍");
            
            // Check if Tenant 2 Admin user exists
            var t2Admin = await dbContext.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Email == "admin@tenant2.com");
            Console.WriteLine($"   - Tenant 2 Admin User Exists: {t2Admin != null}");
            Console.WriteLine($"   - User TenantId: {t2Admin?.TenantId}");
            
            // Check Tenant 2 Admin role assignments
            var t2AdminRoles = await dbContext.UserRoles
                .IgnoreQueryFilters()
                .Include(ur => ur.Role)
                .Where(ur => ur.UserId == t2Admin.Id)
                .ToListAsync();
            Console.WriteLine($"   - Total UserRole assignments: {t2AdminRoles.Count}");
            foreach (var ur in t2AdminRoles)
            {
                Console.WriteLine($"     * RoleId={ur.RoleId}, RoleName={ur.Role?.Name}, TenantId={ur.TenantId}, IsActive={ur.IsActive}");
            }
            
            // Check Tenant 2 Admin role permissions
            var t2AdminRolePermissions = await dbContext.RolePermissions
                .IgnoreQueryFilters()
                .Include(rp => rp.Permission)
                .Include(rp => rp.Role)
                .Where(rp => t2AdminRoles.Select(ur => ur.RoleId).Contains(rp.RoleId))
                .ToListAsync();
            Console.WriteLine($"   - Total Role Permissions: {t2AdminRolePermissions.Count}");
            foreach (var rp in t2AdminRolePermissions.Take(10)) // Only show first 10 to avoid spam
            {
                Console.WriteLine($"     * Permission: {rp.Permission.Name}, Role: {rp.Role.Name}");
            }
        }

        // ✅ ENHANCED DEBUG: After generating permissions list
        if (email == "admin@tenant2.com")
        {
            Console.WriteLine($"🔑 FINAL TENANT 2 ADMIN TOKEN CLAIMS:");
            Console.WriteLine($"   - Final Role: {roleToUse}");
            Console.WriteLine($"   - Final Tenant: {tenant.Name} (ID: {tenant.Id})");
            Console.WriteLine($"   - Final Permissions ({permissions.Count}): [{string.Join(", ", permissions)}]");
            Console.WriteLine($"   - Has 'roles.view': {permissions.Contains("roles.view")}");
            Console.WriteLine($"   - Has 'users.view': {permissions.Contains("users.view")}");
            Console.WriteLine($"   - Has 'tenants.initialize': {permissions.Contains("tenants.initialize")}");
            Console.WriteLine($"🔍🔍🔍 END TENANT 2 ADMIN DEBUG 🔍🔍🔍");
        }
        
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
