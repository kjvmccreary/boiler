// FILE: src shared/Common/Extensions/DatabaseExtensions.cs
using Common.Data;
using Common.Repositories;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Common.Extensions;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        // Database configuration
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"), npgsqlOptions =>
            {
                npgsqlOptions.MigrationsAssembly("Common");
                npgsqlOptions.CommandTimeout(30);
            });

            // Enable in development only
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            }
        });

        // Repository registrations (existing ones)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITenantManagementRepository, TenantManagementRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
        // ADD: RBAC Repository registrations
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IPermissionRepository, PermissionRepository>();
        
        // Service registrations
        services.AddScoped<ITenantProvider, TenantProvider>();
        services.AddScoped<IPermissionService, PermissionService>();
        
        // ADD: RBAC Service registrations
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IAuditService, AuditService>();
        
        return services;
    }

    public static IServiceCollection AddDatabaseHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHealthChecks()
            .AddNpgSql(
                configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found"),
                name: "database",
                tags: new[] { "db", "postgresql" });

        return services;
    }

    // Extension method to ensure database is created and migrated
    public static async Task<IServiceProvider> EnsureDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        await context.Database.EnsureCreatedAsync();

        // Apply any pending migrations
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync();
        }

        return serviceProvider;
    }

    // Extension method to seed initial data
    public static async Task<IServiceProvider> SeedDatabaseAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await SeedInitialDataAsync(context);

        return serviceProvider;
    }

    private static async Task SeedInitialDataAsync(ApplicationDbContext context)
    {
        // Check if we already have data
        if (await context.Tenants.AnyAsync())
            return;

        // Create default tenant
        var defaultTenant = new Tenant
        {
            Name = "Default Tenant",
            Domain = "localhost",
            SubscriptionPlan = "Basic",
            IsActive = true,
            Settings = "{\"theme\":\"default\",\"features\":[\"users\",\"auth\"]}"
        };

        context.Tenants.Add(defaultTenant);
        await context.SaveChangesAsync();

        // Seed permissions and default roles
        await SeedPermissionsAsync(context);
        await SeedDefaultRolesAsync(context, defaultTenant.Id);
    }

    private static async Task SeedPermissionsAsync(ApplicationDbContext context)
    {
        // Get all permissions from constants
        var allConstantPermissions = Common.Constants.Permissions.GetAllPermissions();
        
        // Get existing permissions from database
        var existingPermissionNames = await context.Permissions
            .Select(p => p.Name)
            .ToListAsync();
        
        // Find missing permissions
        var missingPermissions = allConstantPermissions
            .Except(existingPermissionNames)
            .Select(permissionName =>
            {
                var parts = permissionName.Split('.');
                var category = parts.Length > 1 ? parts[0] : "General";
                
                return new Permission
                {
                    Name = permissionName,
                    Category = char.ToUpper(category[0]) + category.Substring(1),
                    Description = $"Permission to {permissionName.Replace('.', ' ')}",
                    IsActive = true
                };
            })
            .ToList();

        // Add only missing permissions
        if (missingPermissions.Any())
        {
            context.Permissions.AddRange(missingPermissions);
            await context.SaveChangesAsync();
        }
    }

    private static async Task SeedDefaultRolesAsync(ApplicationDbContext context, int tenantId)
    {
        // Check if roles already exist
        if (await context.Roles.AnyAsync())
            return;

        // Create system roles (null tenant)
        var systemRoles = new[]
        {
            new Role
            {
                TenantId = null,
                Name = "SuperAdmin",
                Description = "System super administrator with all permissions",
                IsSystemRole = true,
                IsDefault = false,
                IsActive = true
            },
            new Role
            {
                TenantId = null,
                Name = "SystemAdmin",
                Description = "System administrator with system management permissions",
                IsSystemRole = true,
                IsDefault = false,
                IsActive = true
            }
        };

        // Create default tenant roles
        var tenantRoles = new[]
        {
            new Role
            {
                TenantId = tenantId,
                Name = "Admin",
                Description = "Tenant administrator with full tenant permissions",
                IsSystemRole = false,
                IsDefault = false,
                IsActive = true
            },
            new Role
            {
                TenantId = tenantId,
                Name = "User",
                Description = "Standard user with basic permissions",
                IsSystemRole = false,
                IsDefault = true,
                IsActive = true
            }
        };

        context.Roles.AddRange(systemRoles);
        context.Roles.AddRange(tenantRoles);
        await context.SaveChangesAsync();

        // Assign permissions to roles
        await AssignDefaultPermissionsAsync(context);
    }

    private static async Task AssignDefaultPermissionsAsync(ApplicationDbContext context)
    {
        var permissions = await context.Permissions.ToListAsync();
        var roles = await context.Roles.ToListAsync();

        var rolePermissions = new List<RolePermission>();

        // SuperAdmin gets all permissions
        var superAdminRole = roles.First(r => r.Name == "SuperAdmin");
        rolePermissions.AddRange(permissions.Select(p => new RolePermission
        {
            RoleId = superAdminRole.Id,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow,
            GrantedBy = "System"
        }));

        // SystemAdmin gets system permissions
        var systemAdminRole = roles.First(r => r.Name == "SystemAdmin");
        var systemPermissions = permissions.Where(p => p.Category == "System" || p.Category == "Tenants").ToList();
        rolePermissions.AddRange(systemPermissions.Select(p => new RolePermission
        {
            RoleId = systemAdminRole.Id,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow,
            GrantedBy = "System"
        }));

        // Tenant Admin gets user and role management permissions
        var adminRole = roles.First(r => r.Name == "Admin" && r.TenantId != null);
        var adminPermissions = permissions.Where(p => 
            p.Category == "Users" || 
            p.Category == "Roles" || 
            p.Category == "Reports").ToList();
        rolePermissions.AddRange(adminPermissions.Select(p => new RolePermission
        {
            RoleId = adminRole.Id,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow,
            GrantedBy = "System"
        }));

        // User gets basic permissions
        var userRole = roles.First(r => r.Name == "User" && r.TenantId != null);
        var userPermissions = permissions.Where(p => 
            p.Name == "users.view" || 
            p.Name == "reports.view").ToList();
        rolePermissions.AddRange(userPermissions.Select(p => new RolePermission
        {
            RoleId = userRole.Id,
            PermissionId = p.Id,
            GrantedAt = DateTime.UtcNow,
            GrantedBy = "System"
        }));

        context.RolePermissions.AddRange(rolePermissions);
        await context.SaveChangesAsync();
    }
}
