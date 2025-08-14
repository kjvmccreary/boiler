using Common.Data;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace UserService.IntegrationTests.TestUtilities;

public static class TestDataSeeder
{
    public static async Task SeedTestDataAsync(ApplicationDbContext dbContext)
    {
        try
        {
            // ✅ CRITICAL FIX: Ensure database is completely clean before seeding
            await EnsureCleanDatabaseAsync(dbContext);
            
            Console.WriteLine("🚀 TestDataSeeder: Starting test data seeding");

            // Create all test data in correct order
            await CreateTenantsAsync(dbContext);
            await CreatePermissionsAsync(dbContext);
            await CreateRolesAsync(dbContext);
            await CreateRolePermissionsAsync(dbContext);
            await CreateUsersAsync(dbContext); // ✅ FIX: Added missing opening parenthesis
            await CreateUserRoleAssignmentsAsync(dbContext);
            await CreateLegacyTenantUsersAsync(dbContext);

            // ✅ CRITICAL: Verify data was created correctly
            await VerifyTestDataAsync(dbContext);

            Console.WriteLine("✅ TestDataSeeder: Test data seeding completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ TestDataSeeder: Failed to seed test data: {ex.Message}");
            Console.WriteLine($"❌ TestDataSeeder: Stack trace: {ex.StackTrace}");
            Console.WriteLine($"❌ TestDataSeeder: Inner exception: {ex.InnerException?.Message}");
            throw;
        }
    }

    private static async Task EnsureCleanDatabaseAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🧹 Ensuring clean database state...");
            
            // ✅ IMPROVED FIX: Delete and recreate database to ensure clean state
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.EnsureCreatedAsync();
            
            Console.WriteLine("✅ Database recreated with clean state");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️ Warning: Could not recreate database: {ex.Message}");
            
            // Fallback: Try to clear data manually
            try
            {
                Console.WriteLine("🧹 Attempting manual data clear...");
                
                // Use RemoveRange for all entities
                var tenantUsers = await dbContext.TenantUsers.ToListAsync();
                if (tenantUsers.Any()) 
                {
                    dbContext.TenantUsers.RemoveRange(tenantUsers);
                    await dbContext.SaveChangesAsync();
                }
                
                var userRoles = await dbContext.UserRoles.ToListAsync();
                if (userRoles.Any()) 
                {
                    dbContext.UserRoles.RemoveRange(userRoles);
                    await dbContext.SaveChangesAsync();
                }
                
                var rolePermissions = await dbContext.RolePermissions.ToListAsync();
                if (rolePermissions.Any()) 
                {
                    dbContext.RolePermissions.RemoveRange(rolePermissions);
                    await dbContext.SaveChangesAsync();
                }
                
                var users = await dbContext.Users.ToListAsync();
                if (users.Any()) 
                {
                    dbContext.Users.RemoveRange(users);
                    await dbContext.SaveChangesAsync();
                }
                
                var roles = await dbContext.Roles.ToListAsync();
                if (roles.Any()) 
                {
                    dbContext.Roles.RemoveRange(roles);
                    await dbContext.SaveChangesAsync();
                }
                
                var permissions = await dbContext.Permissions.ToListAsync();
                if (permissions.Any()) 
                {
                    dbContext.Permissions.RemoveRange(permissions);
                    await dbContext.SaveChangesAsync();
                }
                
                var tenants = await dbContext.Tenants.ToListAsync();
                if (tenants.Any()) 
                {
                    dbContext.Tenants.RemoveRange(tenants);
                    await dbContext.SaveChangesAsync();
                }
                
                Console.WriteLine("✅ Manual data clear completed");
            }
            catch (Exception clearEx)
            {
                Console.WriteLine($"❌ Manual clear also failed: {clearEx.Message}");
                throw;
            }
        }
    }

    private static async Task CreateTenantsAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🏢 Creating tenants...");

            // ✅ Double-check we have clean state
            var existingCount = await dbContext.Tenants.CountAsync();
            if (existingCount > 0)
            {
                throw new InvalidOperationException($"Database is not clean! Found {existingCount} existing tenants before seeding.");
            }

            var tenants = new[]
            {
                new Tenant 
                { 
                    Name = "Test Tenant 1", 
                    Domain = "tenant1.test.com", 
                    SubscriptionPlan = "Premium", 
                    Settings = "{\"theme\":\"blue\"}", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                },
                new Tenant 
                { 
                    Name = "Test Tenant 2", 
                    Domain = "tenant2.test.com", 
                    SubscriptionPlan = "Basic", 
                    Settings = "{\"theme\":\"green\"}", 
                    IsActive = true, 
                    CreatedAt = DateTime.UtcNow, 
                    UpdatedAt = DateTime.UtcNow 
                }
            };

            dbContext.Tenants.AddRange(tenants);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.Tenants.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} tenants");
            
            if (createdCount != 2)
            {
                throw new InvalidOperationException($"Expected 2 tenants, but created {createdCount}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create tenants: {ex.Message}");
            throw;
        }
    }

    private static async Task CreatePermissionsAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🔑 Creating permissions...");

            var permissions = new[]
            {
                // Users permissions
                new Permission { Name = "users.view", Category = "Users", Description = "View users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "users.edit", Category = "Users", Description = "Edit users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "users.create", Category = "Users", Description = "Create users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "users.delete", Category = "Users", Description = "Delete users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "users.manage", Category = "Users", Description = "Manage users", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "users.manage_roles", Category = "Users", Description = "Manage user roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                // Roles permissions
                new Permission { Name = "roles.view", Category = "Roles", Description = "View roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "roles.create", Category = "Roles", Description = "Create roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "roles.edit", Category = "Roles", Description = "Edit roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "roles.delete", Category = "Roles", Description = "Delete roles", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "roles.manage_permissions", Category = "Roles", Description = "Manage role permissions", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                // Tenants permissions
                new Permission { Name = "tenants.view", Category = "Tenants", Description = "View tenants", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "tenants.edit", Category = "Tenants", Description = "Edit tenants", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                // Reports permissions
                new Permission { Name = "reports.view", Category = "Reports", Description = "View reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "reports.create", Category = "Reports", Description = "Create reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "reports.export", Category = "Reports", Description = "Export reports", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Permissions category permissions
                new Permission { Name = "permissions.view", Category = "Permissions", Description = "View permissions", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "permissions.manage", Category = "Permissions", Description = "Manage permissions", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                // Additional permissions for comprehensive testing
                new Permission { Name = "audit.view", Category = "Audit", Description = "View audit logs", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Permission { Name = "system.admin", Category = "System", Description = "System administration", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            };

            dbContext.Permissions.AddRange(permissions);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.Permissions.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} permissions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create permissions: {ex.Message}");
            throw;
        }
    }

    private static async Task CreateUsersAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("👤 Creating users...");

            var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
            var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");

            Console.WriteLine($"🔍 Found Tenant 1: ID={tenant1.Id}, Name={tenant1.Name}");
            Console.WriteLine($"🔍 Found Tenant 2: ID={tenant2.Id}, Name={tenant2.Name}");

            var users = new[]
            {
                // Tenant 1 users
                new User
                {
                    TenantId = tenant1.Id,
                    Email = "admin@tenant1.com",
                    FirstName = "Admin",
                    LastName = "User1",
                    PasswordHash = CreateSimpleHash("password123"), // ✅ FIX: Use simple hash instead of BCrypt
                    EmailConfirmed = true,
                    IsActive = true,
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    TenantId = tenant1.Id,
                    Email = "user@tenant1.com",
                    FirstName = "Regular",
                    LastName = "User1",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    TenantId = tenant1.Id,
                    Email = "manager@tenant1.com",
                    FirstName = "Manager",
                    LastName = "User1",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    TenantId = tenant1.Id,
                    Email = "viewer@tenant1.com",
                    FirstName = "View",
                    LastName = "Only",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    TenantId = tenant1.Id,
                    Email = "editor@tenant1.com",
                    FirstName = "Content",
                    LastName = "Editor",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                
                // Tenant 2 users
                new User
                {
                    TenantId = tenant2.Id,
                    Email = "admin@tenant2.com",
                    FirstName = "Admin",
                    LastName = "User2",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    TenantId = tenant2.Id,
                    Email = "user@tenant2.com",
                    FirstName = "Regular",
                    LastName = "User2",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            };

            Console.WriteLine($"🔍 Adding {users.Length} users to database...");
            dbContext.Users.AddRange(users);
            
            Console.WriteLine("🔍 Saving users to database...");
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.Users.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} users");
            
            if (createdCount != users.Length)
            {
                throw new InvalidOperationException($"Expected {users.Length} users, but created {createdCount}");
            }

            // ✅ CRITICAL: Verify users were created with correct tenant associations
            var tenant1Users = await dbContext.Users.Where(u => u.TenantId == tenant1.Id).CountAsync();
            var tenant2Users = await dbContext.Users.Where(u => u.TenantId == tenant2.Id).CountAsync();
            
            Console.WriteLine($"🔍 Tenant 1 users: {tenant1Users}");
            Console.WriteLine($"🔍 Tenant 2 users: {tenant2Users}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create users: {ex.Message}");
            Console.WriteLine($"❌ User creation error details: {ex.InnerException?.Message}");
            throw;
        }
    }

    // ✅ CRITICAL FIX: Simple hash function for testing (replace BCrypt dependency)
    private static string CreateSimpleHash(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"salt_{password}"));
        return Convert.ToBase64String(hashedBytes);
    }

    private static async Task CreateRolesAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("👥 Creating roles...");

            var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
            var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");

            var roles = new[]
            {
                // System roles
                new Role { TenantId = null, Name = "SuperAdmin", Description = "System super admin", IsSystemRole = true, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                // Tenant 1 roles
                new Role { TenantId = tenant1.Id, Name = "Admin", Description = "Tenant admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { TenantId = tenant1.Id, Name = "User", Description = "Regular user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { TenantId = tenant1.Id, Name = "Manager", Description = "Department manager", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { TenantId = tenant1.Id, Name = "Viewer", Description = "Read-only access", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { TenantId = tenant1.Id, Name = "Editor", Description = "Content editor", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                // Tenant 2 roles
                new Role { TenantId = tenant2.Id, Name = "Admin", Description = "Tenant 2 admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { TenantId = tenant2.Id, Name = "User", Description = "Tenant 2 user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            };

            dbContext.Roles.AddRange(roles);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.Roles.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} roles");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create roles: {ex.Message}");
            throw;
        }
    }

    private static async Task CreateRolePermissionsAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🔗 Creating role permissions...");

            var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
            var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");
            var permissions = await dbContext.Permissions.ToListAsync();
            var roles = await dbContext.Roles.ToListAsync();

            var rolePermissions = new List<RolePermission>();

            // SuperAdmin gets ALL permissions
            var superAdminRole = roles.First(r => r.Name == "SuperAdmin");
            foreach (var permission in permissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = superAdminRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Tenant 1 Admin gets comprehensive permissions
            var tenant1AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
            var adminPermissions = permissions.Where(p => 
                p.Category == "Users" || 
                p.Category == "Roles" || 
                p.Category == "Tenants" ||
                p.Category == "Reports" ||
                p.Category == "Permissions").ToList();

            foreach (var permission in adminPermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = tenant1AdminRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Tenant 1 User gets basic permissions
            var tenant1UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant1.Id);
            var userPermissions = permissions.Where(p => 
                p.Name == "roles.view").ToList(); // Users can view roles but not users

            foreach (var permission in userPermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = tenant1UserRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Tenant 1 Manager gets intermediate permissions
            var tenant1ManagerRole = roles.First(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
            var managerPermissions = permissions.Where(p => 
                p.Name == "users.view" || 
                p.Name == "users.edit" ||
                p.Name == "roles.view" ||
                p.Name == "reports.view").ToList();

            foreach (var permission in managerPermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = tenant1ManagerRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Viewer role permissions
            var viewerRole = roles.First(r => r.Name == "Viewer" && r.TenantId == tenant1.Id);
            var viewerPermissions = permissions.Where(p => 
                p.Name == "roles.view" ||
                p.Name == "reports.view").ToList();

            foreach (var permission in viewerPermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = viewerRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Tenant 2 Admin gets admin permissions
            var tenant2AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant2.Id);
            foreach (var permission in adminPermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = tenant2AdminRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            // Tenant 2 User gets basic permissions
            var tenant2UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant2.Id);
            foreach (var permission in userPermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = tenant2UserRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "System",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            dbContext.RolePermissions.AddRange(rolePermissions);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.RolePermissions.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} role permissions");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create role permissions: {ex.Message}");
            throw;
        }
    }

    private static async Task CreateUserRoleAssignmentsAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🔗 Creating user role assignments...");

            var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
            var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");
            var users = await dbContext.Users.ToListAsync();
            var roles = await dbContext.Roles.ToListAsync();

            Console.WriteLine($"🔍 Found {users.Count} users and {roles.Count} roles for assignment");

            var adminUser1 = users.First(u => u.Email == "admin@tenant1.com");
            var regularUser1 = users.First(u => u.Email == "user@tenant1.com");
            var managerUser1 = users.First(u => u.Email == "manager@tenant1.com");
            var viewerUser1 = users.First(u => u.Email == "viewer@tenant1.com");
            var editorUser1 = users.First(u => u.Email == "editor@tenant1.com");
            var adminUser2 = users.First(u => u.Email == "admin@tenant2.com");
            var regularUser2 = users.First(u => u.Email == "user@tenant2.com");

            var tenant1AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
            var tenant1UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant1.Id);
            var tenant1ManagerRole = roles.First(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
            var tenant1ViewerRole = roles.First(r => r.Name == "Viewer" && r.TenantId == tenant1.Id);
            var tenant1EditorRole = roles.First(r => r.Name == "Editor" && r.TenantId == tenant1.Id);
            var tenant2AdminRole = roles.First(r => r.Name == "Admin" && r.TenantId == tenant2.Id);
            var tenant2UserRole = roles.First(r => r.Name == "User" && r.TenantId == tenant2.Id);

            var userRoles = new[]
            {
                // Primary role assignments
                new UserRole { UserId = adminUser1.Id, RoleId = tenant1AdminRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UserRole { UserId = regularUser1.Id, RoleId = tenant1UserRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UserRole { UserId = managerUser1.Id, RoleId = tenant1ManagerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UserRole { UserId = viewerUser1.Id, RoleId = tenant1ViewerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UserRole { UserId = editorUser1.Id, RoleId = tenant1EditorRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UserRole { UserId = adminUser2.Id, RoleId = tenant2AdminRole.Id, TenantId = tenant2.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new UserRole { UserId = regularUser2.Id, RoleId = tenant2UserRole.Id, TenantId = tenant2.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                
                // Additional role assignments for multi-role scenarios
                new UserRole { UserId = editorUser1.Id, RoleId = tenant1ViewerRole.Id, TenantId = tenant1.Id, AssignedAt = DateTime.UtcNow, AssignedBy = "System", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            };

            Console.WriteLine($"🔍 Adding {userRoles.Length} user role assignments...");
            dbContext.UserRoles.AddRange(userRoles);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.UserRoles.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} user role assignments");
            
            if (createdCount != userRoles.Length)
            {
                throw new InvalidOperationException($"Expected {userRoles.Length} user roles, but created {createdCount}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create user role assignments: {ex.Message}");
            throw;
        }
    }

    private static async Task CreateLegacyTenantUsersAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🔄 Creating legacy tenant users...");

            var users = await dbContext.Users.ToListAsync();

            var legacyTenantUsers = users.Select(user => new TenantUser
            {
                UserId = user.Id,
                TenantId = user.TenantId!.Value,
                Role = user.Email.Contains("admin") ? "Admin" : 
                       user.Email.Contains("manager") ? "Manager" : 
                       user.Email.Contains("viewer") ? "Viewer" :
                       user.Email.Contains("editor") ? "Editor" : "User",
                IsActive = true,
                JoinedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToArray();

            dbContext.TenantUsers.AddRange(legacyTenantUsers);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.TenantUsers.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} legacy tenant users");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create legacy tenant users: {ex.Message}");
            throw;
        }
    }

    private static async Task VerifyTestDataAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🔍 Verifying test data...");

            var tenantCount = await dbContext.Tenants.CountAsync();
            var userCount = await dbContext.Users.CountAsync();
            var roleCount = await dbContext.Roles.CountAsync();
            var permissionCount = await dbContext.Permissions.CountAsync();
            var userRoleCount = await dbContext.UserRoles.CountAsync();
            var rolePermissionCount = await dbContext.RolePermissions.CountAsync();

            Console.WriteLine($"📊 Final counts: Tenants={tenantCount}, Users={userCount}, Roles={roleCount}, Permissions={permissionCount}, UserRoles={userRoleCount}, RolePermissions={rolePermissionCount}");

            // ✅ CRITICAL VERIFICATION: Ensure test users can be found
            var testUsers = new[] { "admin@tenant1.com", "user@tenant1.com", "admin@tenant2.com" };
            foreach (var email in testUsers)
            {
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    throw new InvalidOperationException($"Critical test user {email} was not created!");
                }
                Console.WriteLine($"✅ Verified user: {email} (ID: {user.Id}, TenantId: {user.TenantId})");
            }

            // ✅ Verify user role assignments
            var adminUserRoles = await dbContext.UserRoles
                .Where(ur => ur.User.Email == "admin@tenant1.com")
                .CountAsync();
            
            if (adminUserRoles == 0)
            {
                throw new InvalidOperationException("Critical: admin@tenant1.com has no role assignments!");
            }
            
            Console.WriteLine($"✅ admin@tenant1.com has {adminUserRoles} role assignments");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Test data verification failed: {ex.Message}");
            throw;
        }
    }
}
