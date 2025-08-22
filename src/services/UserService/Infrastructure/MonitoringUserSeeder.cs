using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Constants;
using Contracts.Services;
using DTOs.Entities;

namespace UserService.Infrastructure;

public static class MonitoringUserSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        try
        {
            using var scope = services.CreateScope();
            var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("MonitoringUserSeeder");
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();

            var section = config.GetSection("Monitoring");
            if (!section.Exists())
            {
                logger.LogInformation("Monitoring section missing; skipping monitoring user seeding.");
                return;
            }

            var rawEnabled = section["Enabled"];
            if (!bool.TryParse(rawEnabled, out var enabled) || !enabled)
            {
                logger.LogInformation("Monitoring.Enabled = '{Raw}' (evaluated false); skipping seeding.", rawEnabled);
                return;
            }

            var email = section.GetValue<string>("Email") ?? "monitor@local";
            var rawPassword = section.GetValue<string>("Password") ?? "ChangeMe123!";
            var fallbackTenantId = section.GetValue<int?>("TenantId") ?? 1; // Only for UserRole row if schema demands tenant linkage.

            logger.LogInformation("Config snapshot: Monitoring:Enabled='{Enabled}'", rawEnabled);
            logger.LogInformation("Seeding system-wide monitoring account (Email: {Email})", email);

            // Ensure permission exists
            var viewMetrics = await db.Permissions.FirstOrDefaultAsync(p => p.Name == Permissions.System.ViewMetrics);
            if (viewMetrics == null)
            {
                viewMetrics = new Permission
                {
                    Name = Permissions.System.ViewMetrics,
                    Category = "System",
                    Description = "Permission to view system / performance metrics",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.Permissions.Add(viewMetrics);
                await db.SaveChangesAsync();
            }

            // Find existing system Monitor role OR tenant-scoped one to upgrade
            var monitorRole = await db.Roles
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(r => r.Name == "Monitor" && (r.TenantId == null || r.TenantId == fallbackTenantId));

            if (monitorRole == null)
            {
                monitorRole = new Role
                {
                    Name = "Monitor",
                    Description = "Read-only monitoring role (system-wide)",
                    TenantId = null,          // System role
                    IsSystemRole = true,
                    IsActive = true,
                    IsDefault = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                db.Roles.Add(monitorRole);
                await db.SaveChangesAsync();
                logger.LogInformation("Created new system Monitor role");
            }
            else
            {
                // Upgrade if needed
                if (monitorRole.TenantId != null || !monitorRole.IsSystemRole)
                {
                    monitorRole.TenantId = null;
                    monitorRole.IsSystemRole = true;
                    monitorRole.UpdatedAt = DateTime.UtcNow;
                    await db.SaveChangesAsync();
                    logger.LogInformation("Upgraded existing Monitor role to system role");
                }
            }

            // Ensure role-permission link
            var rpExists = await db.RolePermissions
                .AnyAsync(rp => rp.RoleId == monitorRole.Id && rp.PermissionId == viewMetrics.Id);

            if (!rpExists)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = monitorRole.Id,
                    PermissionId = viewMetrics.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "MonitoringUserSeeder",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
                await db.SaveChangesAsync();
                logger.LogInformation("Linked system Monitor role to permission {Permission}", viewMetrics.Name);
            }

            // Ensure user (ignore filters)
            var user = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    FirstName = "Monitor",
                    LastName = "Account",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                var passwordHashProp = user.GetType().GetProperty("PasswordHash");
                if (passwordHashProp != null)
                {
                    var hash = passwordService.HashPassword(rawPassword);
                    passwordHashProp.SetValue(user, hash);
                }

                db.Users.Add(user);
                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateException ex) when (IsUniqueEmailViolation(ex))
                {
                    logger.LogWarning(ex, "Race inserting monitoring user; reloading existing record.");
                    user = await db.Users.IgnoreQueryFilters().FirstOrDefaultAsync(u => u.Email == email);
                }
            }
            else if (!user.IsActive)
            {
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
                logger.LogInformation("Reactivated existing monitoring user.");
            }

            // Ensure UserRole (if schema requires TenantId, keep fallbackTenantId; system role logic only uses Role.IsSystemRole)
            var userRoleExists = await db.UserRoles
                .AnyAsync(ur => ur.UserId == user!.Id && ur.RoleId == monitorRole.Id);

            if (!userRoleExists)
            {
                db.UserRoles.Add(new UserRole
                {
                    UserId = user!.Id,
                    RoleId = monitorRole.Id,
                    TenantId = fallbackTenantId, // Keep a tenant id if non-nullable; ignored for system role token logic
                    AssignedAt = DateTime.UtcNow,
                    AssignedBy = "MonitoringUserSeeder",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });

                try
                {
                    await db.SaveChangesAsync();
                    logger.LogInformation("Assigned system Monitor role to monitoring user");
                }
                catch (DbUpdateException ex) when (IsDuplicateUserRole(ex))
                {
                    logger.LogWarning("Race assigning Monitor role to user; continuing.");
                }
            }

            logger.LogInformation("Monitoring user ready. login_email={Email} password={Password}", email, rawPassword);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[MonitoringUserSeeder] Non-fatal error: {ex.Message}");
            var factory = services.GetService<ILoggerFactory>();
            factory?.CreateLogger("MonitoringUserSeeder")?.LogError(ex, "Monitoring user seeding failed (suppressed).");
        }
    }

    private static bool IsUniqueEmailViolation(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("IX_Users_Email", StringComparison.OrdinalIgnoreCase) == true;

    private static bool IsDuplicateUserRole(DbUpdateException ex)
        => ex.InnerException?.Message.Contains("UserRoles", StringComparison.OrdinalIgnoreCase) == true;
}
