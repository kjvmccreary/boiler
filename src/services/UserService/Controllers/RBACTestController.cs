using Common.Constants;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace UserService.Controllers;

/// <summary>
/// Temporary controller to test RBAC functionality after migration
/// This should be removed or secured in production
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RBACTestController : ControllerBase
{
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ITenantProvider _tenantProvider;
    private readonly ILogger<RBACTestController> _logger;

    public RBACTestController(
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ITenantProvider tenantProvider,
        ILogger<RBACTestController> logger)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _tenantProvider = tenantProvider;
        _logger = logger;
    }

    /// <summary>
    /// Test endpoint to verify RBAC tables are created and seeded
    /// </summary>
    [HttpGet("verify-migration")]
    public async Task<IActionResult> VerifyMigration()
    {
        try
        {
            var result = new
            {
                Message = "RBAC Migration Verification",
                Timestamp = DateTime.UtcNow,
                Tests = new List<object>()
            };

            var tests = (List<object>)result.Tests;

            // Test 1: Check Permissions table
            try
            {
                var permissions = await _permissionRepository.GetAllAsync();
                tests.Add(new
                {
                    Test = "Permissions Table",
                    Status = "✅ SUCCESS",
                    Count = permissions.Count(),
                    Sample = permissions.Take(3).Select(p => new { p.Name, p.Category })
                });
            }
            catch (Exception ex)
            {
                tests.Add(new
                {
                    Test = "Permissions Table",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Test 2: Check Roles table
            try
            {
                var roles = await _roleRepository.GetAllAsync();
                tests.Add(new
                {
                    Test = "Roles Table",
                    Status = "✅ SUCCESS",
                    Count = roles.Count(),
                    Sample = roles.Take(3).Select(r => new { r.Name, r.IsSystemRole, r.TenantId })
                });
            }
            catch (Exception ex)
            {
                tests.Add(new
                {
                    Test = "Roles Table",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Test 3: Check Permission Categories
            try
            {
                var categories = await _permissionRepository.GetCategoriesAsync();
                tests.Add(new
                {
                    Test = "Permission Categories",
                    Status = "✅ SUCCESS",
                    Categories = categories.ToList()
                });
            }
            catch (Exception ex)
            {
                tests.Add(new
                {
                    Test = "Permission Categories",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Test 4: Check System Roles
            try
            {
                var systemRoles = await _roleRepository.GetSystemRolesAsync();
                tests.Add(new
                {
                    Test = "System Roles",
                    Status = "✅ SUCCESS",
                    Count = systemRoles.Count(),
                    Roles = systemRoles.Select(r => new { r.Name, r.Description })
                });
            }
            catch (Exception ex)
            {
                tests.Add(new
                {
                    Test = "System Roles",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Test 5: Test Permission Constants
            try
            {
                var allPermissionConstants = Permissions.GetAllPermissions();
                var permissionsByCategory = Permissions.GetPermissionsByCategory();
                
                tests.Add(new
                {
                    Test = "Permission Constants",
                    Status = "✅ SUCCESS",
                    TotalConstants = allPermissionConstants.Count,
                    Categories = permissionsByCategory.Keys.ToList(),
                    SampleByCategory = permissionsByCategory.ToDictionary(
                        kvp => kvp.Key, 
                        kvp => kvp.Value.Take(2).ToList()
                    )
                });
            }
            catch (Exception ex)
            {
                tests.Add(new
                {
                    Test = "Permission Constants",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Summary
            var successCount = tests.Count(t => t.ToString()!.Contains("✅ SUCCESS"));
            var failCount = tests.Count(t => t.ToString()!.Contains("❌ FAILED"));

            return Ok(new
            {
                result.Message,
                result.Timestamp,
                Summary = new
                {
                    Total = tests.Count,
                    Passed = successCount,
                    Failed = failCount,
                    Status = failCount == 0 ? "🎉 ALL TESTS PASSED" : "⚠️ SOME TESTS FAILED"
                },
                Details = result.Tests
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RBAC migration verification");
            return StatusCode(500, new
            {
                Message = "RBAC Migration Verification Failed",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// NEW: Seed the RBAC data into the database
    /// </summary>
    [HttpPost("seed-rbac-data")]
    public async Task<IActionResult> SeedRBACData()
    {
        try
        {
            var result = new
            {
                Message = "RBAC Data Seeding",
                Timestamp = DateTime.UtcNow,
                Operations = new List<object>()
            };

            var operations = (List<object>)result.Operations;

            // Step 1: Seed Permissions
            try
            {
                var existingPermissions = await _permissionRepository.GetAllAsync();
                if (!existingPermissions.Any())
                {
                    var allPermissionConstants = Permissions.GetAllPermissions();
                    var permissionsToCreate = allPermissionConstants.Select(permissionName =>
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
                    }).ToList();

                    foreach (var permission in permissionsToCreate)
                    {
                        await _permissionRepository.AddAsync(permission);
                    }

                    operations.Add(new
                    {
                        Operation = "Seed Permissions",
                        Status = "✅ SUCCESS",
                        Count = permissionsToCreate.Count,
                        Message = $"Created {permissionsToCreate.Count} permissions"
                    });
                }
                else
                {
                    operations.Add(new
                    {
                        Operation = "Seed Permissions",
                        Status = "ℹ️ SKIPPED",
                        Count = existingPermissions.Count(),
                        Message = "Permissions already exist"
                    });
                }
            }
            catch (Exception ex)
            {
                operations.Add(new
                {
                    Operation = "Seed Permissions",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Step 2: Seed System Roles
            try
            {
                var existingSystemRoles = await _roleRepository.GetSystemRolesAsync();
                if (!existingSystemRoles.Any())
                {
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

                    foreach (var role in systemRoles)
                    {
                        await _roleRepository.AddAsync(role);
                    }

                    operations.Add(new
                    {
                        Operation = "Seed System Roles",
                        Status = "✅ SUCCESS",
                        Count = systemRoles.Length,
                        Roles = systemRoles.Select(r => r.Name)
                    });
                }
                else
                {
                    operations.Add(new
                    {
                        Operation = "Seed System Roles",
                        Status = "ℹ️ SKIPPED",
                        Count = existingSystemRoles.Count(),
                        Message = "System roles already exist"
                    });
                }
            }
            catch (Exception ex)
            {
                operations.Add(new
                {
                    Operation = "Seed System Roles",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Step 3: Create Default Tenant Roles
            try
            {
                var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
                if (tenantId.HasValue)
                {
                    var existingTenantRoles = await _roleRepository.GetTenantRolesAsync(tenantId.Value);
                    var tenantSpecificRoles = existingTenantRoles.Where(r => r.TenantId == tenantId.Value);

                    if (!tenantSpecificRoles.Any())
                    {
                        var tenantRoles = new[]
                        {
                            new Role
                            {
                                TenantId = tenantId.Value,
                                Name = "Admin",
                                Description = "Tenant administrator with full tenant permissions",
                                IsSystemRole = false,
                                IsDefault = false,
                                IsActive = true
                            },
                            new Role
                            {
                                TenantId = tenantId.Value,
                                Name = "User",
                                Description = "Standard user with basic permissions",
                                IsSystemRole = false,
                                IsDefault = true,
                                IsActive = true
                            },
                            new Role
                            {
                                TenantId = tenantId.Value,
                                Name = "Manager",
                                Description = "Manager with enhanced permissions",
                                IsSystemRole = false,
                                IsDefault = false,
                                IsActive = true
                            }
                        };

                        foreach (var role in tenantRoles)
                        {
                            await _roleRepository.AddAsync(role);
                        }

                        operations.Add(new
                        {
                            Operation = "Seed Tenant Roles",
                            Status = "✅ SUCCESS",
                            TenantId = tenantId.Value,
                            Count = tenantRoles.Length,
                            Roles = tenantRoles.Select(r => r.Name)
                        });
                    }
                    else
                    {
                        operations.Add(new
                        {
                            Operation = "Seed Tenant Roles",
                            Status = "ℹ️ SKIPPED",
                            TenantId = tenantId.Value,
                            Count = tenantSpecificRoles.Count(),
                            Message = "Tenant roles already exist"
                        });
                    }
                }
                else
                {
                    operations.Add(new
                    {
                        Operation = "Seed Tenant Roles",
                        Status = "⚠️ WARNING",
                        Message = "No tenant context available"
                    });
                }
            }
            catch (Exception ex)
            {
                operations.Add(new
                {
                    Operation = "Seed Tenant Roles",
                    Status = "❌ FAILED",
                    Error = ex.Message
                });
            }

            // Summary
            var successCount = operations.Count(o => o.ToString()!.Contains("✅ SUCCESS"));
            var skipCount = operations.Count(o => o.ToString()!.Contains("ℹ️ SKIPPED"));
            var failCount = operations.Count(o => o.ToString()!.Contains("❌ FAILED"));

            return Ok(new
            {
                result.Message,
                result.Timestamp,
                Summary = new
                {
                    Total = operations.Count,
                    Success = successCount,
                    Skipped = skipCount,
                    Failed = failCount,
                    Status = failCount == 0 ? "🎉 SEEDING COMPLETED" : "⚠️ SOME OPERATIONS FAILED"
                },
                Operations = operations,
                NextStep = "Run /verify-migration again to see the seeded data"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RBAC data seeding");
            return StatusCode(500, new
            {
                Message = "RBAC Data Seeding Failed",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Test creating a sample role with permissions
    /// </summary>
    [HttpPost("test-role-creation")]
    public async Task<IActionResult> TestRoleCreation()
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available for testing");
            }

            // Create a test role
            var testRole = new Role
            {
                TenantId = tenantId.Value,
                Name = $"TestRole_{DateTime.UtcNow:yyyyMMddHHmmss}",
                Description = "Temporary test role created by RBAC verification",
                IsSystemRole = false,
                IsDefault = false,
                IsActive = true
            };

            var createdRole = await _roleRepository.AddAsync(testRole);

            // Get some permissions to assign
            var permissions = await _permissionRepository.GetAllAsync();
            var samplePermissions = permissions.Take(3).ToList();

            var rolePermissions = new List<RolePermission>();
            foreach (var permission in samplePermissions)
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = createdRole.Id,
                    PermissionId = permission.Id,
                    GrantedAt = DateTime.UtcNow,
                    GrantedBy = "RBAC Test System"
                });
            }

            // Note: You'd need a RolePermission repository to save these
            // For now, just return the test result

            return Ok(new
            {
                Message = "Role Creation Test",
                Status = "✅ SUCCESS",
                CreatedRole = new
                {
                    createdRole.Id,
                    createdRole.Name,
                    createdRole.Description,
                    createdRole.TenantId
                },
                AvailablePermissions = samplePermissions.Select(p => new { p.Name, p.Category }),
                Note = "Role created successfully! Remember to clean up test data."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during role creation test");
            return StatusCode(500, new
            {
                Message = "Role Creation Test Failed",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Clean up test data
    /// </summary>
    [HttpDelete("cleanup-test-data")]
    public async Task<IActionResult> CleanupTestData()
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available for cleanup");
            }

            // Find and delete test roles
            var allRoles = await _roleRepository.GetTenantRolesAsync(tenantId.Value);
            var testRoles = allRoles.Where(r => r.Name.StartsWith("TestRole_")).ToList();

            var deletedCount = 0;
            foreach (var role in testRoles)
            {
                await _roleRepository.DeleteAsync(role.Id);
                deletedCount++;
            }

            return Ok(new
            {
                Message = "Test Data Cleanup",
                Status = "✅ SUCCESS",
                DeletedRoles = deletedCount,
                Timestamp = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during test data cleanup");
            return StatusCode(500, new
            {
                Message = "Test Data Cleanup Failed",
                Error = ex.Message
            });
        }
    }
}
