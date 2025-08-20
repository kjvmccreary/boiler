using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Common.Data;

namespace UserService.IntegrationTests.Diagnostics;

/// <summary>
/// ✅ TEMPORARY: Diagnostic controller to help debug test setup issues
/// This should be removed in production
/// </summary>
[ApiController]
[Route("api/test-diagnostics")]
public class TestDiagnosticsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TestDiagnosticsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("database-status")]
    public async Task<IActionResult> GetDatabaseStatus()
    {
        try
        {
            var tenantCount = await _context.Tenants.CountAsync();
            var userCount = await _context.Users.CountAsync();
            var roleCount = await _context.Roles.CountAsync();
            var permissionCount = await _context.Permissions.CountAsync();
            var userRoleCount = await _context.UserRoles.CountAsync();
            var rolePermissionCount = await _context.RolePermissions.CountAsync();

            var tenants = await _context.Tenants.Select(t => new { t.Id, t.Name, t.Domain }).ToListAsync();
            var users = await _context.Users.Select(u => new { u.Id, u.Email, u.IsActive }).ToListAsync(); // 🔧 REMOVE: u.TenantId

            return Ok(new
            {
                Status = "Database Status Check",
                Timestamp = DateTime.UtcNow,
                Counts = new
                {
                    Tenants = tenantCount,
                    Users = userCount,
                    Roles = roleCount,
                    Permissions = permissionCount,
                    UserRoles = userRoleCount,
                    RolePermissions = rolePermissionCount
                },
                Data = new
                {
                    Tenants = tenants,
                    Users = users
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = "Database status check failed",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }

    [HttpPost("force-reseed")]
    public async Task<IActionResult> ForceReseed()
    {
        try
        {
            // Delete all data
            await _context.UserRoles.ExecuteDeleteAsync();
            await _context.RolePermissions.ExecuteDeleteAsync();
            await _context.TenantUsers.ExecuteDeleteAsync();
            await _context.Users.ExecuteDeleteAsync();
            await _context.Roles.ExecuteDeleteAsync();
            await _context.Permissions.ExecuteDeleteAsync();
            await _context.Tenants.ExecuteDeleteAsync();

            // Re-seed
            await TestUtilities.TestDataSeeder.SeedTestDataAsync(_context);

            return Ok(new
            {
                Status = "Force reseed completed",
                Timestamp = DateTime.UtcNow,
                Message = "Database cleared and reseeded successfully"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = "Force reseed failed",
                Message = ex.Message,
                StackTrace = ex.StackTrace
            });
        }
    }
}
