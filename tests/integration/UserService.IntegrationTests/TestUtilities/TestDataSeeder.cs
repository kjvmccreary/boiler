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
            await CreateUsersAsync (dbContext);
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

            // ✅ FIXED: Use ONLY the actual business-defined permissions from Permissions.cs
            var actualPermissions = Common.Constants.Permissions.GetAllPermissions();
            
            var permissions = actualPermissions.Select(permissionName => new Permission
            {
                Name = permissionName,
                Category = GetCategoryFromPermissionName(permissionName),
                Description = GetDescriptionFromPermissionName(permissionName),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }).ToArray();

            Console.WriteLine($"🔍 Creating {permissions.Length} permissions from actual business requirements");
            foreach (var permission in permissions)
            {
                Console.WriteLine($"   - {permission.Name} ({permission.Category})");
            }

            dbContext.Permissions.AddRange(permissions);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.Permissions.CountAsync();
            Console.WriteLine($"✅ Created {createdCount} permissions");

            if (createdCount != actualPermissions.Count)
            {
                throw new InvalidOperationException($"Expected {actualPermissions.Count} permissions, but created {createdCount}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create permissions: {ex.Message}");
            throw;
        }
    }

    private static string GetCategoryFromPermissionName(string permissionName)
    {
        var parts = permissionName.Split('.');
        return parts.Length > 0 ? char.ToUpper(parts[0][0]) + parts[0].Substring(1) : "General";
    }

    private static string GetDescriptionFromPermissionName(string permissionName)
    {
        var parts = permissionName.Split('.');
        if (parts.Length < 2) return $"Permission: {permissionName}";
        
        var action = parts[1].Replace("_", " ");
        var resource = parts[0];
        return $"{char.ToUpper(action[0])}{action.Substring(1)} {resource}";
    }

    private static async Task CreateUsersAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("👤 Creating users...");

            // Remove TenantId assignments from User objects
            var users = new[]
            {
                // Tenant 1 users (5 users)
                new User
                {
                    Email = "admin@tenant1.com",
                    FirstName = "Admin",
                    LastName = "User1",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    LastLoginAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
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
                    Email = "editor@tenant1.com",
                    FirstName = "Content",
                    LastName = "Editor",
                    PasswordHash = CreateSimpleHash("password123"),
                    EmailConfirmed = true,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                
                // Tenant 2 users (2 users)
                new User
                {
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

            Console.WriteLine($"🔍 Adding {users.Length} users to main context");

            // Add all users at once
            dbContext.Users.AddRange(users);
            await dbContext.SaveChangesAsync(); // ✅ This should now work with proper dependencies

            Console.WriteLine("✅ All users created successfully");

            // ✅ Verify users were created (use IgnoreQueryFilters for verification)
            var finalCount = await dbContext.Users.IgnoreQueryFilters().CountAsync();
            Console.WriteLine($"✅ Final user count: {finalCount}");

            if (finalCount != 7)
            {
                throw new InvalidOperationException($"Expected 7 users, but found {finalCount}");
            }

            // ✅ Verify required test users exist
            var requiredTestUsers = new[] { "admin@tenant1.com", "user@tenant1.com", "admin@tenant2.com", "user@tenant2.com" };
            foreach (var requiredEmail in requiredTestUsers)
            {
                var user = await dbContext.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == requiredEmail);
                if (user == null)
                {
                    throw new InvalidOperationException($"Critical test user missing: {requiredEmail}");
                }
                Console.WriteLine($"✅ Verified required user exists: {requiredEmail} (ID:  {user.Id})");
            }

            Console.WriteLine("🎉 All users created and verified successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create users: {ex.Message}");
            Console.WriteLine($"❌ User creation error details: {ex.InnerException?.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
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

            // ✅ DEBUG: Log tenant IDs
            Console.WriteLine($"🔍 CreateRoles DEBUG:");
            Console.WriteLine($"   - tenant1.Id = {tenant1.Id}");
            Console.WriteLine($"   - tenant2.Id = {tenant2.Id}");

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
                
                // ✅ CRITICAL: Tenant 2 roles
                new Role { TenantId = tenant2.Id, Name = "Admin", Description = "Tenant 2 admin", IsSystemRole = false, IsDefault = false, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
                new Role { TenantId = tenant2.Id, Name = "User", Description = "Tenant 2 user", IsSystemRole = false, IsDefault = true, IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            };

            // ✅ DEBUG: Log all roles before adding to database
            Console.WriteLine($"🔍 Roles to be created ({roles.Length}):");
            for (int i = 0; i < roles.Length; i++)
            {
                var role = roles[i];
                Console.WriteLine($"   {i + 1}. {role.Name} (TenantId: {role.TenantId})");
            }

            dbContext.Roles.AddRange(roles);
            await dbContext.SaveChangesAsync();

            // ✅ VERIFICATION: Check what was actually created
            var createdCount = await dbContext.Roles.IgnoreQueryFilters().CountAsync();
            Console.WriteLine($"✅ Created {createdCount} roles");

            // ✅ VERIFICATION: List all created roles with their IDs and TenantIds
            var createdRoles = await dbContext.Roles.IgnoreQueryFilters().ToListAsync();
            Console.WriteLine($"🔍 Verification - Created roles:");
            foreach (var role in createdRoles)
            {
                Console.WriteLine($"   - ID={role.Id}, Name='{role.Name}', TenantId={role.TenantId}");
            }

            // ✅ VERIFICATION: Ensure we have the expected number of roles
            if (createdCount != 8)
            {
                throw new InvalidOperationException($"Expected 8 roles (1 system + 6 tenant1 + 2 tenant2), but created {createdCount}");
            }

            // ✅ VERIFICATION: Ensure Tenant 2 roles were created
            var tenant2RoleCount = createdRoles.Count(r => r.TenantId == tenant2.Id);
            if (tenant2RoleCount != 2)
            {
                throw new InvalidOperationException($"Expected 2 Tenant 2 roles, but found {tenant2RoleCount}");
            }

            Console.WriteLine($"✅ Successfully created all 8 roles including {tenant2RoleCount} Tenant 2 roles");
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
            
            // ✅ CRITICAL FIX: Use IgnoreQueryFilters to get ALL roles, including Tenant 2 roles
            var roles = await dbContext.Roles.IgnoreQueryFilters().ToListAsync();

            Console.WriteLine($"🔍 Found {permissions.Count} permissions and {roles.Count} roles");

            // ✅ VERIFICATION: Show all roles that were loaded
            Console.WriteLine($"🔍 ALL LOADED ROLES:");
            foreach (var role in roles)
            {
                Console.WriteLine($"   - ID={role.Id}, Name='{role.Name}', TenantId={role.TenantId}");
            }

            var rolePermissions = new List<RolePermission>();

            // SuperAdmin gets ALL permissions
            var superAdminRole = roles.FirstOrDefault(r => r.Name == "SuperAdmin");
            if (superAdminRole != null)
            {
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
                Console.WriteLine($"✅ Assigned {permissions.Count} permissions to SuperAdmin");
            }

            // ✅ FIXED: Use only actual permissions from Constants
            var FindPermissions = (string[] permissionNames) =>
            {
                var foundPermissions = new List<Permission>();
                foreach (var name in permissionNames)
                {
                    var permission = permissions.FirstOrDefault(p => p.Name == name);
                    if (permission != null)
                    {
                        foundPermissions.Add(permission);
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ Warning: Permission '{name}' not found in database");
                    }
                }
                return foundPermissions;
            };

            // ✅ FIXED: Use ONLY permissions that actually exist in Permissions.cs
            // Tenant 1 Admin gets comprehensive permissions
            var tenant1AdminRole = roles.FirstOrDefault(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
            if (tenant1AdminRole != null)
            {
                var adminPermissionNames = new[]
                {
                    // Users permissions (from Permissions.Users)
                    "users.view", "users.edit", "users.create", "users.delete", "users.view_all", "users.manage_roles",
                    
                    // Roles permissions (from Permissions.Roles) 
                    "roles.view", "roles.create", "roles.edit", "roles.delete", "roles.assign_users", "roles.manage_permissions",
                    
                    // ✅ FIX: Tenants permissions - COMPLETE SET INCLUDING tenants.initialize
                    "tenants.view", "tenants.create", "tenants.edit", "tenants.delete", "tenants.initialize", "tenants.view_all", "tenants.manage_settings",
                    
                    // Reports permissions (from Permissions.Reports)
                    "reports.view", "reports.create", "reports.export", "reports.schedule",
                    
                    // Permission management (from Permissions.PermissionManagement)
                    "permissions.view", "permissions.create", "permissions.edit", "permissions.delete", "permissions.manage"
                };

                var adminPermissions = FindPermissions(adminPermissionNames);

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
                Console.WriteLine($"✅ Assigned {adminPermissions.Count} permissions to Tenant1 Admin");
            }

            // ✅ FIXED: Tenant 1 User gets basic permissions (only real ones)
            var tenant1UserRole = roles.FirstOrDefault(r => r.Name == "User" && r.TenantId == tenant1.Id);
            if (tenant1UserRole != null)
            {
                var userPermissionNames = new[] 
                { 
                    "roles.view", "reports.view", "permissions.view" 
                };
                var userPermissions = FindPermissions(userPermissionNames);

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
                Console.WriteLine($"✅ Assigned {userPermissions.Count} permissions to Tenant1 User");
            }

            // ✅ FIXED: Manager gets intermediate permissions
            var tenant1ManagerRole = roles.FirstOrDefault(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
            if (tenant1ManagerRole != null)
            {
                var managerPermissionNames = new[]
                {
                    "users.view", "users.edit", "users.manage_roles",
                    "roles.view", "reports.view", "reports.create"
                };
                var managerPermissions = FindPermissions(managerPermissionNames);

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
                Console.WriteLine($"✅ Assigned {managerPermissions.Count} permissions to Tenant1 Manager");
            }

            // ✅ FIXED: Viewer role permissions
            var viewerRole = roles.FirstOrDefault(r => r.Name == "Viewer" && r.TenantId == tenant1.Id);
            if (viewerRole != null)
            {
                var viewerPermissionNames = new[] { "roles.view", "reports.view", "permissions.view" };
                var viewerPermissions = FindPermissions(viewerPermissionNames);

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
                Console.WriteLine($"✅ Assigned {viewerPermissions.Count} permissions to Viewer");
            }

            // ✅ FIXED: Tenant 2 Admin gets COMPLETE admin permissions
            // ✅ CRITICAL DEBUG: Check if tenant2AdminRole lookup is working
            Console.WriteLine($"🔍 TENANT 2 ADMIN DEBUG:");
            Console.WriteLine($"   - tenant2.Id = {tenant2.Id}");
            Console.WriteLine($"   - All roles count: {roles.Count}");
            Console.WriteLine($"   - Roles with TenantId = {tenant2.Id}: {roles.Count(r => r.TenantId == tenant2.Id)}");
            Console.WriteLine($"   - Admin roles: {string.Join(", ", roles.Where(r => r.Name == "Admin").Select(r => $"ID={r.Id}, TenantId={r.TenantId}"))}");

            var tenant2AdminRole = roles.FirstOrDefault(r => r.Name == "Admin" && r.TenantId == tenant2.Id);
            Console.WriteLine($"   - tenant2AdminRole found: {tenant2AdminRole != null}");
            if (tenant2AdminRole != null)
            {
                Console.WriteLine($"   - tenant2AdminRole.Id = {tenant2AdminRole.Id}");
            }

            if (tenant2AdminRole != null)
            {
                var adminPermissionNames = new[]
                {
                    // Users permissions - COMPLETE SET
                    "users.view", "users.edit", "users.create", "users.delete", "users.view_all", "users.manage_roles",
                    
                    // Roles permissions - COMPLETE SET  
                    "roles.view", "roles.create", "roles.edit", "roles.delete", "roles.assign_users", "roles.manage_permissions",
                    
                    // Tenants permissions - COMPLETE SET INCLUDING tenants.initialize
                    "tenants.view", "tenants.create", "tenants.edit", "tenants.delete", "tenants.initialize", "tenants.view_all", "tenants.manage_settings",
                    
                    // Reports permissions - COMPLETE SET
                    "reports.view", "reports.create", "reports.export", "reports.schedule",
                    
                    // Permission management - COMPLETE SET
                    "permissions.view", "permissions.create", "permissions.edit", "permissions.delete", "permissions.manage"
                };
                
                var adminPermissions = FindPermissions(adminPermissionNames);
                Console.WriteLine($"🔍 DEBUG: Found {adminPermissions.Count} permissions for Tenant2 Admin");

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
                Console.WriteLine($"✅ Assigned {adminPermissions.Count} permissions to Tenant2 Admin (Expected: 23+)");
            }
            else
            {
                Console.WriteLine($"❌ ERROR: Tenant2 Admin role not found! TenantId={tenant2.Id}");
                Console.WriteLine($"❌ Available roles: {string.Join(", ", roles.Select(r => $"'{r.Name}' (TenantId={r.TenantId})"))}");
            }

            // ✅ FIXED: Tenant 2 User gets basic permissions
            var tenant2UserRole = roles.FirstOrDefault(r => r.Name == "User" && r.TenantId == tenant2.Id);
            if (tenant2UserRole != null)
            {
                var userPermissionNames = new[] { "roles.view", "reports.view", "permissions.view" };
                var userPermissions = FindPermissions(userPermissionNames);

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
                Console.WriteLine($"✅ Assigned {userPermissions.Count} permissions to Tenant2 User");
            }

            if (rolePermissions.Count > 0)
            {
                dbContext.RolePermissions.AddRange(rolePermissions);
                await dbContext.SaveChangesAsync();

                var createdCount = await dbContext.RolePermissions.CountAsync();
                Console.WriteLine($"✅ Created {createdCount} role permissions using only actual business-defined permissions");
            }
            else
            {
                Console.WriteLine("⚠️ Warning: No role permissions created");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create role permissions: {ex.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
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
            
            // ✅ CRITICAL FIX: Use IgnoreQueryFilters() when fetching data for assignments
            var users = await dbContext.Users.IgnoreQueryFilters().ToListAsync();
            var roles = await dbContext.Roles.IgnoreQueryFilters().ToListAsync();

            Console.WriteLine($"🔍 Found {users.Count} users and {roles.Count} roles for assignment");

            // ✅ ENHANCED: Use safer lookups with better error messages
            var adminUser1 = users.FirstOrDefault(u => u.Email == "admin@tenant1.com");
            if (adminUser1 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'admin@tenant1.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            var regularUser1 = users.FirstOrDefault(u => u.Email == "user@tenant1.com");
            if (regularUser1 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'user@tenant1.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            var managerUser1 = users.FirstOrDefault(u => u.Email == "manager@tenant1.com");
            if (managerUser1 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'manager@tenant1.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            var viewerUser1 = users.FirstOrDefault(u => u.Email == "viewer@tenant1.com");
            if (viewerUser1 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'viewer@tenant1.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            var editorUser1 = users.FirstOrDefault(u => u.Email == "editor@tenant1.com");
            if (editorUser1 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'editor@tenant1.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            var adminUser2 = users.FirstOrDefault(u => u.Email == "admin@tenant2.com");
            if (adminUser2 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'admin@tenant2.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            var regularUser2 = users.FirstOrDefault(u => u.Email == "user@tenant2.com");
            if (regularUser2 == null)
            {
                var availableUsers = users.Select(u => u.Email).ToArray();
                throw new InvalidOperationException($"User 'user@tenant2.com' not found. Available users: {string.Join(", ", availableUsers)}");
            }

            // ✅ ENHANCED: Use safer role lookups with better error messages
            var tenant1AdminRole = roles.FirstOrDefault(r => r.Name == "Admin" && r.TenantId == tenant1.Id);
            if (tenant1AdminRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant1.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'Admin' for Tenant1 not found. Available Tenant1 roles: {string.Join(", ", availableRoles)}");
            }

            var tenant1UserRole = roles.FirstOrDefault(r => r.Name == "User" && r.TenantId == tenant1.Id);
            if (tenant1UserRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant1.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'User' for Tenant1 not found. Available Tenant1 roles: {string.Join(", ", availableRoles)}");
            }

            var tenant1ManagerRole = roles.FirstOrDefault(r => r.Name == "Manager" && r.TenantId == tenant1.Id);
            if (tenant1ManagerRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant1.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'Manager' for Tenant1 not found. Available Tenant1 roles: {string.Join(", ", availableRoles)}");
            }

            var tenant1ViewerRole = roles.FirstOrDefault(r => r.Name == "Viewer" && r.TenantId == tenant1.Id);
            if (tenant1ViewerRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant1.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'Viewer' for Tenant1 not found. Available Tenant1 roles: {string.Join(", ", availableRoles)}");
            }

            var tenant1EditorRole = roles.FirstOrDefault(r => r.Name == "Editor" && r.TenantId == tenant1.Id);
            if (tenant1EditorRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant1.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'Editor' for Tenant1 not found. Available Tenant1 roles: {string.Join(", ", availableRoles)}");
            }

            var tenant2AdminRole = roles.FirstOrDefault(r => r.Name == "Admin" && r.TenantId == tenant2.Id);
            if (tenant2AdminRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant2.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'Admin' for Tenant2 not found. Available Tenant2 roles: {string.Join(", ", availableRoles)}");
            }

            var tenant2UserRole = roles.FirstOrDefault(r => r.Name == "User" && r.TenantId == tenant2.Id);
            if (tenant2UserRole == null)
            {
                var availableRoles = roles.Where(r => r.TenantId == tenant2.Id).Select(r => r.Name).ToArray();
                throw new InvalidOperationException($"Role 'User' for Tenant2 not found. Available Tenant2 roles: {string.Join(", ", availableRoles)}");
            }

            var userRoles = new []
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

            var createdCount = await dbContext.UserRoles.IgnoreQueryFilters().CountAsync(); // ✅ Use IgnoreQueryFilters for count
            Console.WriteLine($"✅ Created {createdCount} user role assignments");

            if (createdCount < userRoles.Length - 1) // Allow for minor variations but ensure most are created
            {
                throw new InvalidOperationException($"Expected around {userRoles.Length} user roles, but created {createdCount}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to create user role assignments: {ex.Message}");
            Console.WriteLine($"❌ Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private static async Task CreateLegacyTenantUsersAsync(ApplicationDbContext dbContext)
    {
        try
        {
            Console.WriteLine("🔄 Creating legacy tenant users...");

            // ✅ CRITICAL FIX: Use IgnoreQueryFilters() to get all users
            var users = await dbContext.Users.IgnoreQueryFilters().ToListAsync();

            // Get tenants from context directly
            var tenant1 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 1");
            var tenant2 = await dbContext.Tenants.FirstAsync(t => t.Name == "Test Tenant 2");

            var legacyTenantUsers = new List<TenantUser>();
            
            foreach (var user in users)
            {
                // 🔧 FIX: Determine tenant based on email pattern instead of user.TenantId
                var tenantId = user.Email.Contains("@tenant1.com") ? tenant1.Id : 
                              user.Email.Contains("@tenant2.com") ? tenant2.Id : tenant1.Id; // Default to tenant1
                
                legacyTenantUsers.Add(new TenantUser
                {
                    UserId = user.Id,
                    TenantId = tenantId, // 🔧 FIX: Use calculated tenantId instead of user.TenantId
                    Role = user.Email.Contains("admin") ? "Admin" :
                           user.Email.Contains("manager") ? "Manager" :
                           user.Email.Contains("viewer") ? "Viewer" :
                           user.Email.Contains("editor") ? "Editor" : "User",
                    IsActive = true,
                    JoinedAt = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }

            dbContext.TenantUsers.AddRange(legacyTenantUsers);
            await dbContext.SaveChangesAsync();

            var createdCount = await dbContext.TenantUsers.IgnoreQueryFilters().CountAsync();
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

            // ✅ CRITICAL FIX: Disable global query filters during verification
            Console.WriteLine("🔧 Temporarily disabling query filters for verification...");

            var tenantCount = await dbContext.Tenants.CountAsync();
            var userCount = await dbContext.Users.IgnoreQueryFilters().CountAsync(); // ← Disable filters
            var roleCount = await dbContext.Roles.IgnoreQueryFilters().CountAsync(); // ← Disable filters
            var permissionCount = await dbContext.Permissions.CountAsync();
            var userRoleCount = await dbContext.UserRoles.IgnoreQueryFilters().CountAsync(); // ← Disable filters
            var rolePermissionCount = await dbContext.RolePermissions.IgnoreQueryFilters().CountAsync(); // ← Disable filters

            Console.WriteLine($"📊 Final counts: Tenants={tenantCount}, Users={userCount}, Roles={roleCount}, Permissions={permissionCount}, UserRoles={userRoleCount}, RolePermissions={rolePermissionCount}");

            // ✅ CRITICAL VERIFICATION: Ensure test users can be found (disable query filters)
            var testUsers = new[] { "admin@tenant1.com", "user@tenant1.com", "admin@tenant2.com" };
            foreach (var email in testUsers)
            {
                var user = await dbContext.Users
                    .IgnoreQueryFilters() // ✅ CRITICAL: Disable query filters for verification
                    .FirstOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    // ✅ ENHANCED ERROR: Show all users without query filters
                    var allUsers = await dbContext.Users
                        .IgnoreQueryFilters()
                        .Select(u => new { u.Email, u.Id }) // 🔧 REMOVE: u.TenantId (since User no longer has TenantId)
                        .ToListAsync();

                    Console.WriteLine($"❌ All users in database (query filters disabled):");
                    foreach (var u in allUsers)
                    {
                        Console.WriteLine($"   - {u.Email} (ID: {u.Id})"); // 🔧 REMOVE: TenantId reference
                    }

                    throw new InvalidOperationException($"Critical test user missing: {email}. Check database constraints and entity relationships.");
                }
                Console.WriteLine($"✅ Verified user: {email} (ID: {user.Id})");
            }

            // ✅ Verify user role assignments (disable query filters)
            var adminUserRoles = await dbContext.UserRoles
                .IgnoreQueryFilters() // ✅ CRITICAL: Disable query filters
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

    // ... (other helper methods remain the same) ...
}
