using Common.Constants;
using Common.Data;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    private readonly ApplicationDbContext _context;

    public RBACTestController(
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ITenantProvider tenantProvider,
        ILogger<RBACTestController> logger,
        ApplicationDbContext context)
    {
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _tenantProvider = tenantProvider;
        _logger = logger;
        _context = context;
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

    /// <summary>
    /// Assign a role to a user for testing purposes
    /// </summary>
    [HttpPost("assign-user-role")]
    public async Task<IActionResult> AssignUserRole([FromBody] AssignUserRoleRequest request)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available");
            }

            // Find the role by name in the current tenant
            var allRoles = await _roleRepository.GetTenantRolesAsync(tenantId.Value);
            var role = allRoles.FirstOrDefault(r => r.Name == request.RoleName && r.TenantId == tenantId.Value);
            if (role == null)
            {
                return BadRequest($"Role '{request.RoleName}' not found in tenant {tenantId.Value}");
            }

            // Use the correct method name
            var existingUserRole = await _userRoleRepository.GetUserRoleAsync(request.UserId, role.Id, tenantId.Value);
            if (existingUserRole != null)
            {
                return Ok(new
                {
                    Message = "User Role Assignment",
                    Status = "ℹ️ ALREADY EXISTS",
                    UserId = request.UserId,
                    RoleName = request.RoleName,
                    RoleId = role.Id,
                    TenantId = tenantId.Value,
                    Note = "User already has this role"
                });
            }

            // Create new user role assignment
            var userRole = new UserRole
            {
                UserId = request.UserId,
                RoleId = role.Id,
                TenantId = tenantId.Value,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = "RBAC Test System",
                IsActive = true
            };

            await _userRoleRepository.AddAsync(userRole);

            return Ok(new
            {
                Message = "User Role Assignment",
                Status = "✅ SUCCESS",
                UserId = request.UserId,
                RoleName = request.RoleName,
                RoleId = role.Id,
                TenantId = tenantId.Value,
                AssignedAt = userRole.AssignedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role {RoleName} to user {UserId}", request.RoleName, request.UserId);
            return StatusCode(500, new
            {
                Message = "User Role Assignment Failed",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Get user roles for testing purposes
    /// </summary>
    [HttpGet("user-roles/{userId}")]
    public async Task<IActionResult> GetUserRoles(int userId)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available");
            }

            var userRoles = await _userRoleRepository.GetUserRolesAsync(userId, tenantId.Value);
            
            return Ok(new
            {
                Message = "User Roles",
                UserId = userId,
                TenantId = tenantId.Value,
                Roles = userRoles.Select(ur => new
                {
                    ur.RoleId,
                    RoleName = ur.Role?.Name,
                    ur.AssignedAt,
                    ur.IsActive
                }),
                Count = userRoles.Count()
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting roles for user {UserId}", userId);
            return StatusCode(500, new
            {
                Message = "Get User Roles Failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Check if user has a specific role (testing helper)
    /// </summary>
    [HttpGet("check-user-role/{userId}/{roleName}")]
    public async Task<IActionResult> CheckUserRole(int userId, string roleName)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available");
            }

            // Find the role
            var allRoles = await _roleRepository.GetTenantRolesAsync(tenantId.Value);
            var role = allRoles.FirstOrDefault(r => r.Name == roleName && r.TenantId == tenantId.Value);
            if (role == null)
            {
                return BadRequest($"Role '{roleName}' not found in tenant {tenantId.Value}");
            }

            // Check if user has role
            var hasRole = await _userRoleRepository.UserHasRoleAsync(userId, role.Id, tenantId.Value);
            
            return Ok(new
            {
                Message = "User Role Check",
                UserId = userId,
                RoleName = roleName,
                RoleId = role.Id,
                TenantId = tenantId.Value,
                HasRole = hasRole,
                Status = hasRole ? "✅ USER HAS ROLE" : "❌ USER DOES NOT HAVE ROLE"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking role {RoleName} for user {UserId}", roleName, userId);
            return StatusCode(500, new
            {
                Message = "Check User Role Failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Comprehensive RBAC diagnostic endpoint
    /// </summary>
    [HttpGet("debug-rbac-state")]
    public async Task<IActionResult> DebugRBACState()
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            
            var permissionData = new Dictionary<string, object>();
            var roleData = new Dictionary<string, object>();
            var rolePermissionData = new Dictionary<string, object>();
            var userRoleData = new Dictionary<string, object>();
            var userData = new Dictionary<string, object>();

            // Check Permissions
            var permissions = await _permissionRepository.GetAllAsync();
            permissionData["Count"] = permissions.Count();
            permissionData["Sample"] = permissions.Take(5).Select(p => new { p.Id, p.Name, p.Category, p.IsActive });

            // Check Roles
            var roles = await _roleRepository.GetAllAsync();
            roleData["Count"] = roles.Count();
            roleData["TenantRoles"] = roles.Where(r => r.TenantId == tenantId).Select(r => new { r.Id, r.Name, r.TenantId, r.IsActive });
            roleData["SystemRoles"] = roles.Where(r => r.TenantId == null).Select(r => new { r.Id, r.Name, r.IsSystemRole, r.IsActive });

            // Check RolePermissions - THIS IS KEY!
            var rolePermissions = await _context.RolePermissions.ToListAsync();
            rolePermissionData["Count"] = rolePermissions.Count;
            rolePermissionData["Sample"] = rolePermissions.Take(10).Select(rp => new { rp.RoleId, rp.PermissionId, rp.GrantedAt, rp.GrantedBy });

            // Check UserRoles
            var userRoles = await _userRoleRepository.GetUserRolesAsync(1, tenantId ?? 1);
            userRoleData["Count"] = userRoles.Count();
            userRoleData["UserRoleDetails"] = userRoles.Select(ur => new { ur.UserId, ur.RoleId, ur.TenantId, ur.IsActive, RoleName = ur.Role?.Name });

            // Check Users
            var userCount = await _context.Users.CountAsync();
            var userSample = await _context.Users.Take(5).Select(u => new { u.Id, u.Email, u.TenantId, u.IsActive }).ToListAsync();
            userData["Count"] = userCount;
            userData["Sample"] = userSample;

            return Ok(new
            {
                Message = "RBAC State Diagnostics",
                TenantId = tenantId,
                Timestamp = DateTime.UtcNow,
                Data = new
                {
                    Permissions = permissionData,
                    Roles = roleData,
                    RolePermissions = rolePermissionData,
                    UserRoles = userRoleData,
                    Users = userData
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during RBAC state diagnostics");
            return StatusCode(500, new
            {
                Message = "RBAC State Diagnostics Failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Create role-permission assignments (the missing piece!)
    /// </summary>
    [HttpPost("assign-role-permissions")]
    public async Task<IActionResult> AssignRolePermissions()
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available");
            }

            var assignments = new List<object>();

            // Get all permissions and roles
            var permissions = await _permissionRepository.GetAllAsync();
            var roles = await _roleRepository.GetTenantRolesAsync(tenantId.Value);

            // Check if role permissions already exist
            var existingRolePermissions = await _context.RolePermissions.AnyAsync();
            if (existingRolePermissions)
            {
                assignments.Add(new
                {
                    Status = "ℹ️ SKIPPED",
                    Message = "Role permissions already exist"
                });
                
                return Ok(new
                {
                    Message = "Role Permission Assignment",
                    TenantId = tenantId.Value,
                    Summary = new
                    {
                        Status = "ℹ️ ALREADY EXISTS"
                    },
                    Assignments = assignments
                });
            }

            var rolePermissions = new List<RolePermission>();

            // Admin Role - Gets most permissions
            var adminRole = roles.FirstOrDefault(r => r.Name == "Admin" && r.TenantId == tenantId.Value);
            if (adminRole != null)
            {
                var adminPermissions = permissions.Where(p => 
                    p.Category == "Users" || 
                    p.Category == "Roles" || 
                    p.Category == "Reports" ||
                    p.Category == "Tenants" ||
                    p.Category == "Permissions").ToList(); // Admin gets comprehensive permissions including new Permissions category

                foreach (var permission in adminPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = "RBAC Test System"
                    });
                }

                assignments.Add(new
                {
                    Status = "✅ SUCCESS",
                    RoleName = "Admin",
                    PermissionCount = adminPermissions.Count,
                    Permissions = adminPermissions.Select(p => p.Name)
                });
            }

            // User Role - Gets basic permissions
            var userRole = roles.FirstOrDefault(r => r.Name == "User" && r.TenantId == tenantId.Value);
            if (userRole != null)
            {
                var userPermissions = permissions.Where(p => 
                    p.Name == "users.view" || 
                    p.Name == "reports.view").ToList();

                foreach (var permission in userPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = userRole.Id,
                        PermissionId = permission.Id,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = "RBAC Test System"
                    });
                }

                assignments.Add(new
                {
                    Status = "✅ SUCCESS",
                    RoleName = "User",
                    PermissionCount = userPermissions.Count,
                    Permissions = userPermissions.Select(p => p.Name)
                });
            }

            // Manager Role - Gets intermediate permissions
            var managerRole = roles.FirstOrDefault(r => r.Name == "Manager" && r.TenantId == tenantId.Value);
            if (managerRole != null)
            {
                var managerPermissions = permissions.Where(p => 
                    p.Category == "Users" || 
                    p.Category == "Reports").ToList();

                foreach (var permission in managerPermissions)
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        PermissionId = permission.Id,
                        GrantedAt = DateTime.UtcNow,
                        GrantedBy = "RBAC Test System"
                    });
                }

                assignments.Add(new
                {
                    Status = "✅ SUCCESS",
                    RoleName = "Manager",
                    PermissionCount = managerPermissions.Count,
                    Permissions = managerPermissions.Select(p => p.Name)
                });
            }

            // Save all role permissions
            _context.RolePermissions.AddRange(rolePermissions);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Role Permission Assignment",
                TenantId = tenantId.Value,
                Summary = new
                {
                    TotalRolePermissions = rolePermissions.Count,
                    RolesConfigured = assignments.Count,
                    Status = "🎉 ROLE PERMISSIONS ASSIGNED"
                },
                Assignments = assignments
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error assigning role permissions");
            return StatusCode(500, new
            {
                Message = "Role Permission Assignment Failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Check permissions for a specific role
    /// </summary>
    [HttpGet("check-role-permissions/{roleId}")]
    public async Task<IActionResult> CheckRolePermissions(int roleId)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            
            // Get role info
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest($"Role with ID {roleId} not found");
            }
            
            // Get permissions for this role
            var rolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Join(_context.Permissions,
                    rp => rp.PermissionId,
                    p => p.Id,
                    (rp, p) => new { p.Name, p.Category, rp.GrantedAt, rp.GrantedBy })
                .ToListAsync();

            return Ok(new
            {
                Message = "Role Permission Check",
                RoleId = roleId,
                RoleName = role.Name,
                TenantId = role.TenantId,
                PermissionCount = rolePermissions.Count,
                Permissions = rolePermissions.Take(20), // Show first 20
                Status = rolePermissions.Any() ? "✅ HAS PERMISSIONS" : "❌ NO PERMISSIONS"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking permissions for role {RoleId}", roleId);
            return StatusCode(500, new
            {
                Message = "Check Role Permissions Failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Force assign permissions to a specific role (for testing)
    /// </summary>
    [HttpPost("force-assign-permissions-to-role/{roleId}")]
    public async Task<IActionResult> ForceAssignPermissionsToRole(int roleId)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available");
            }

            // Get the role
            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
            {
                return BadRequest($"Role with ID {roleId} not found");
            }

            // Check if this role already has permissions
            var existingPermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .CountAsync();

            if (existingPermissions > 0)
            {
                return Ok(new
                {
                    Message = "Role already has permissions",
                    RoleId = roleId,
                    RoleName = role.Name,
                    ExistingPermissionCount = existingPermissions,
                    Status = "ℹ️ ALREADY HAS PERMISSIONS"
                });
            }

            // Get all permissions
            var permissions = await _permissionRepository.GetAllAsync();
            
            // Assign permissions based on role name
            var permissionsToAssign = new List<Permission>();
            
            if (role.Name == "Admin")
            {
                // Admin gets comprehensive permissions
                permissionsToAssign = permissions.Where(p => 
                    p.Category == "Users" || 
                    p.Category == "Roles" || 
                    p.Category == "Reports" ||
                    p.Category == "Tenants").ToList();
            }
            else if (role.Name == "User")
            {
                // User gets basic permissions
                permissionsToAssign = permissions.Where(p => 
                    p.Name == "users.view" || 
                    p.Name == "reports.view").ToList();
            }
            else
            {
                // Default: give basic permissions
                permissionsToAssign = permissions.Where(p => 
                    p.Name == "users.view" || 
                    p.Name == "reports.view").ToList();
            }

            // Create role permission assignments
            var rolePermissions = permissionsToAssign.Select(p => new RolePermission
            {
                RoleId = roleId,
                PermissionId = p.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "RBAC Force Assignment"
            }).ToList();

            _context.RolePermissions.AddRange(rolePermissions);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Permissions Force Assigned",
                RoleId = roleId,
                RoleName = role.Name,
                PermissionCount = rolePermissions.Count,
                Permissions = permissionsToAssign.Select(p => p.Name),
                Status = "✅ PERMISSIONS ASSIGNED"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error force assigning permissions to role {RoleId}", roleId);
            return StatusCode(500, new
            {
                Message = "Force Assign Permissions Failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Force seed missing permissions from constants (tests Option 3 logic)
    /// </summary>
    [HttpPost("force-seed-missing-permissions")]
    public async Task<IActionResult> ForceSeedMissingPermissions()
    {
        try
        {
            // Get all permissions from constants (same logic as Option 3)
            var allConstantPermissions = Permissions.GetAllPermissions();
            
            // Get existing permissions from database
            var existingPermissions = await _permissionRepository.GetAllAsync();
            var existingPermissionNames = existingPermissions.Select(p => p.Name).ToList();
            
            // Find missing permissions (same logic as your Option 3 fix)
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

            if (missingPermissions.Any())
            {
                foreach (var permission in missingPermissions)
                {
                    await _permissionRepository.AddAsync(permission);
                }
                
                return Ok(new
                {
                    Message = "Missing Permissions Added Successfully",
                    Status = "✅ SUCCESS",
                    TotalConstantPermissions = allConstantPermissions.Count,
                    ExistingInDatabase = existingPermissionNames.Count,
                    NewlyAdded = missingPermissions.Count,
                    AddedPermissions = missingPermissions.Select(p => new { p.Name, p.Category }).ToList(),
                    Note = "This tests your Option 3 logic!"
                });
            }
            else
            {
                return Ok(new
                {
                    Message = "All Permissions Already Exist",
                    Status = "ℹ️ UP TO DATE",
                    TotalConstantPermissions = allConstantPermissions.Count,
                    ExistingInDatabase = existingPermissionNames.Count
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error seeding missing permissions");
            return StatusCode(500, new
            {
                Message = "Force Seed Missing Permissions Failed",
                Error = ex.Message,
                Timestamp = DateTime.UtcNow
            });
        }
    }

    /// <summary>
    /// Add missing Permissions category to Admin role
    /// </summary>
    [HttpPost("add-missing-permissions-category-to-admin")]
    public async Task<IActionResult> AddMissingPermissionsCategoryToAdmin()
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
            {
                return BadRequest("No tenant context available");
            }

            // Find Admin role for current tenant
            var roles = await _roleRepository.GetTenantRolesAsync(tenantId.Value);
            var adminRole = roles.FirstOrDefault(r => r.Name == "Admin" && r.TenantId == tenantId.Value);
            if (adminRole == null)
            {
                return BadRequest("Admin role not found in current tenant");
            }

            // Get all permissions in Permissions category
            var allPermissions = await _permissionRepository.GetAllAsync();
            var permissionsCategory = allPermissions.Where(p => p.Category == "Permissions").ToList();
            
            if (!permissionsCategory.Any())
            {
                return BadRequest("No permissions found in 'Permissions' category");
            }

            // Check which permissions are already assigned
            var existingRolePermissions = await _context.RolePermissions
                .Where(rp => rp.RoleId == adminRole.Id)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var missingPermissions = permissionsCategory
                .Where(p => !existingRolePermissions.Contains(p.Id))
                .ToList();

            if (!missingPermissions.Any())
            {
                return Ok(new
                {
                    Message = "Admin already has all Permissions category permissions",
                    RoleName = adminRole.Name,
                    PermissionsCategory = permissionsCategory.Select(p => p.Name),
                    Status = "ℹ️ ALREADY COMPLETE"
                });
            }

            // Add missing permissions
            var newRolePermissions = missingPermissions.Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id,
                GrantedAt = DateTime.UtcNow,
                GrantedBy = "Missing Category Fix"
            }).ToList();

            _context.RolePermissions.AddRange(newRolePermissions);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Missing Permissions category added to Admin role",
                RoleName = adminRole.Name,
                RoleId = adminRole.Id,
                TenantId = tenantId.Value,
                AddedPermissions = missingPermissions.Select(p => new { p.Name, p.Category }),
                NewPermissionCount = newRolePermissions.Count,
                Status = "✅ SUCCESS"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding Permissions category to Admin role");
            return StatusCode(500, new
            {
                Message = "Add Missing Permissions Category Failed",
                Error = ex.Message
            });
        }
    }
}

public class AssignUserRoleRequest
{
    public int UserId { get; set; }
    public string RoleName { get; set; } = string.Empty;
}
