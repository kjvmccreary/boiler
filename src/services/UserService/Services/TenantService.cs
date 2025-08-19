using AutoMapper;
using Common.Data;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using DTOs.Common;
using DTOs.Entities;
using DTOs.Tenant;
using DTOs.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace UserService.Services;

public class TenantService : ITenantService
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<TenantService> _logger;
    private readonly IPasswordService _passwordService;
    private readonly IRoleTemplateService _roleTemplateService;
    private readonly IAuditService _auditService;
    private readonly ITenantProvider _tenantProvider;

    public TenantService(
        ApplicationDbContext context,
        IMapper mapper,
        ILogger<TenantService> logger,
        IPasswordService passwordService,
        IRoleTemplateService roleTemplateService,
        IAuditService auditService,
        ITenantProvider tenantProvider)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
        _passwordService = passwordService;
        _roleTemplateService = roleTemplateService;
        _auditService = auditService;
        _tenantProvider = tenantProvider;
    }

    public async Task<ApiResponseDto<TenantDto>> CreateTenantAsync(CreateTenantDto createDto)
    {
        try
        {
            _logger.LogInformation("Creating new tenant: {TenantName}", createDto.Name);

            // Check if domain is already taken
            if (!string.IsNullOrEmpty(createDto.Domain))
            {
                var existingTenant = await _context.Tenants
                    .Where(t => t.Domain == createDto.Domain)
                    .FirstOrDefaultAsync();

                if (existingTenant != null)
                {
                    return ApiResponseDto<TenantDto>.ErrorResult($"Domain '{createDto.Domain}' is already in use");
                }
            }

            // Create tenant
            var tenant = new Tenant
            {
                Name = createDto.Name,
                Domain = createDto.Domain,
                SubscriptionPlan = createDto.SubscriptionPlan,
                Settings = System.Text.Json.JsonSerializer.Serialize(createDto.Settings),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tenant created with ID: {TenantId}", tenant.Id);

            // 🔧 STEP 1: Create default roles FIRST (before admin user)
            _logger.LogInformation("Creating default roles for tenant {TenantId}", tenant.Id);
            await _roleTemplateService.CreateDefaultRolesForTenantAsync(tenant.Id);

            // 🔧 STEP 2: Create admin user and assign proper RBAC role
            var adminUser = await CreateTenantAdminUserAsync(tenant.Id, createDto.AdminUser);
            
            if (!adminUser.Success)
            {
                // Rollback tenant creation if admin user creation fails
                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();
                return ApiResponseDto<TenantDto>.ErrorResult($"Failed to create admin user: {adminUser.Message}");
            }

            // Map to DTO
            var tenantDto = _mapper.Map<TenantDto>(tenant);
            
            // Audit the creation
            await _auditService.LogAsync(AuditAction.UserCreated, $"tenants/{tenant.Id}", 
                $"Tenant '{tenant.Name}' created with admin user", true);

            return ApiResponseDto<TenantDto>.SuccessResult(tenantDto, "Tenant created successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant: {TenantName}", createDto.Name);
            return ApiResponseDto<TenantDto>.ErrorResult("An error occurred while creating the tenant");
        }
    }

    public async Task<ApiResponseDto<TenantDto>> GetTenantAsync(int tenantId)
    {
        try
        {
            var tenant = await _context.Tenants
                .Where(t => t.Id == tenantId)
                .Select(t => new
                {
                    Tenant = t,
                    UserCount = t.Users.Count(u => u.IsActive),
                    RoleCount = _context.Roles.Count(r => r.TenantId == t.Id && r.IsActive)
                })
                .FirstOrDefaultAsync();

            if (tenant == null)
            {
                return ApiResponseDto<TenantDto>.ErrorResult("Tenant not found");
            }

            var tenantDto = _mapper.Map<TenantDto>(tenant.Tenant);
            tenantDto.UserCount = tenant.UserCount;
            tenantDto.RoleCount = tenant.RoleCount;

            return ApiResponseDto<TenantDto>.SuccessResult(tenantDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenant: {TenantId}", tenantId);
            return ApiResponseDto<TenantDto>.ErrorResult("An error occurred while retrieving the tenant");
        }
    }

    public async Task<ApiResponseDto<PagedResultDto<TenantDto>>> GetTenantsAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            var query = _context.Tenants.AsQueryable();

            var totalCount = await query.CountAsync();
            var tenants = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new
                {
                    Tenant = t,
                    UserCount = t.Users.Count(u => u.IsActive),
                    RoleCount = _context.Roles.Count(r => r.TenantId == t.Id && r.IsActive)
                })
                .ToListAsync();

            var tenantDtos = tenants.Select(t =>
            {
                var dto = _mapper.Map<TenantDto>(t.Tenant);
                dto.UserCount = t.UserCount;
                dto.RoleCount = t.RoleCount;
                return dto;
            }).ToList();

            var pagedResult = new PagedResultDto<TenantDto>
            {
                Items = tenantDtos,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };

            return ApiResponseDto<PagedResultDto<TenantDto>>.SuccessResult(pagedResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving tenants");
            return ApiResponseDto<PagedResultDto<TenantDto>>.ErrorResult("An error occurred while retrieving tenants");
        }
    }

    public async Task<ApiResponseDto<TenantDto>> UpdateTenantAsync(int tenantId, UpdateTenantDto updateDto)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                return ApiResponseDto<TenantDto>.ErrorResult("Tenant not found");
            }

            // Check domain uniqueness if changed
            if (!string.IsNullOrEmpty(updateDto.Domain) && updateDto.Domain != tenant.Domain)
            {
                var existingTenant = await _context.Tenants
                    .Where(t => t.Domain == updateDto.Domain && t.Id != tenantId)
                    .FirstOrDefaultAsync();

                if (existingTenant != null)
                {
                    return ApiResponseDto<TenantDto>.ErrorResult($"Domain '{updateDto.Domain}' is already in use");
                }
            }

            // Update tenant properties
            tenant.Name = updateDto.Name;
            tenant.Domain = updateDto.Domain;
            tenant.SubscriptionPlan = updateDto.SubscriptionPlan;
            tenant.IsActive = updateDto.IsActive;
            tenant.Settings = System.Text.Json.JsonSerializer.Serialize(updateDto.Settings);
            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var tenantDto = _mapper.Map<TenantDto>(tenant);

            await _auditService.LogAsync(AuditAction.UserUpdated, $"tenants/{tenant.Id}", 
                $"Tenant '{tenant.Name}' updated", true);

            return ApiResponseDto<TenantDto>.SuccessResult(tenantDto, "Tenant updated successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating tenant: {TenantId}", tenantId);
            return ApiResponseDto<TenantDto>.ErrorResult("An error occurred while updating the tenant");
        }
    }

    public async Task<ApiResponseDto<bool>> DeleteTenantAsync(int tenantId)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant not found");
            }

            // Check if tenant has users (prevent accidental deletion)
            var userCount = await _context.Users.CountAsync(u => u.TenantId == tenantId);
            if (userCount > 0)
            {
                return ApiResponseDto<bool>.ErrorResult($"Cannot delete tenant with {userCount} users. Deactivate instead.");
            }

            _context.Tenants.Remove(tenant);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(AuditAction.UserDeactivated, $"tenants/{tenant.Id}", 
                $"Tenant '{tenant.Name}' deleted", true);

            return ApiResponseDto<bool>.SuccessResult(true, "Tenant deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting tenant: {TenantId}", tenantId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while deleting the tenant");
        }
    }

    public async Task<ApiResponseDto<bool>> ActivateTenantAsync(int tenantId)
    {
        return await SetTenantActiveStatusAsync(tenantId, true);
    }

    public async Task<ApiResponseDto<bool>> DeactivateTenantAsync(int tenantId)
    {
        return await SetTenantActiveStatusAsync(tenantId, false);
    }

    public async Task<ApiResponseDto<TenantDto>> InitializeTenantAsync(TenantInitializationDto initDto)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(initDto.TenantId);
            if (tenant == null)
            {
                return ApiResponseDto<TenantDto>.ErrorResult("Tenant not found");
            }

            if (initDto.CreateDefaultRoles)
            {
                await _roleTemplateService.CreateDefaultRolesForTenantAsync(tenant.Id);
            }

            // Create specific role templates if requested
            foreach (var template in initDto.RoleTemplates)
            {
                await _roleTemplateService.CreateRoleFromTemplateAsync(tenant.Id, template);
            }

            var tenantDto = _mapper.Map<TenantDto>(tenant);

            await _auditService.LogAsync(AuditAction.UserCreated, $"tenants/{tenant.Id}", 
                $"Tenant '{tenant.Name}' initialized", true);

            return ApiResponseDto<TenantDto>.SuccessResult(tenantDto, "Tenant initialized successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing tenant: {TenantId}", initDto.TenantId);
            return ApiResponseDto<TenantDto>.ErrorResult("An error occurred while initializing the tenant");
        }
    }

    private async Task<ApiResponseDto<bool>> SetTenantActiveStatusAsync(int tenantId, bool isActive)
    {
        try
        {
            var tenant = await _context.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                return ApiResponseDto<bool>.ErrorResult("Tenant not found");
            }

            tenant.IsActive = isActive;
            tenant.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var action = isActive ? "activated" : "deactivated";
            await _auditService.LogAsync(isActive ? AuditAction.UserCreated : AuditAction.UserDeactivated, 
                $"tenants/{tenant.Id}", $"Tenant '{tenant.Name}' {action}", true);

            return ApiResponseDto<bool>.SuccessResult(true, $"Tenant {action} successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error setting tenant status: {TenantId}", tenantId);
            return ApiResponseDto<bool>.ErrorResult("An error occurred while updating tenant status");
        }
    }

    public async Task<ApiResponseDto<UserDto>> CreateTenantAdminUserAsync(int tenantId, CreateTenantAdminDto adminDto)
    {
        try
        {
            // Check if user already exists
            var existingUser = await _context.Users
                .Where(u => u.Email == adminDto.Email)
                .FirstOrDefaultAsync();

            if (existingUser != null)
            {
                return ApiResponseDto<UserDto>.ErrorResult($"User with email '{adminDto.Email}' already exists");
            }

            // Create user
            var user = new User
            {
                Email = adminDto.Email,
                FirstName = adminDto.FirstName,
                LastName = adminDto.LastName,
                PasswordHash = _passwordService.HashPassword(adminDto.Password),
                TenantId = tenantId,
                IsActive = true,
                EmailConfirmed = true, // Auto-confirm for tenant admin
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // 🔧 STEP 3: Find the "Tenant Admin" role that was just created
            var tenantAdminRole = await _context.Roles
                .Where(r => r.TenantId == tenantId && r.Name == "Tenant Admin")
                .FirstOrDefaultAsync();

            if (tenantAdminRole == null)
            {
                _logger.LogError("Tenant Admin role not found for tenant {TenantId}. Available roles: {Roles}", 
                    tenantId, string.Join(", ", await _context.Roles.Where(r => r.TenantId == tenantId).Select(r => r.Name).ToListAsync()));
                return ApiResponseDto<UserDto>.ErrorResult("Failed to find Tenant Admin role");
            }

            // 🔧 STEP 4: Create proper RBAC UserRole assignment
            var userRole = new UserRole
            {
                UserId = user.Id,
                RoleId = tenantAdminRole.Id,
                TenantId = tenantId,
                IsActive = true,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);

            // 🔧 STEP 5: Create legacy TenantUser relationship (for fallback compatibility)
            var tenantUser = new TenantUser
            {
                TenantId = tenantId,
                UserId = user.Id,
                Role = "TenantAdmin", // Legacy string role for backward compatibility
                IsActive = true,
                JoinedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TenantUsers.Add(tenantUser);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created tenant admin user {Email} for tenant {TenantId} with role {RoleId}", 
                adminDto.Email, tenantId, tenantAdminRole.Id);

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant admin user for tenant {TenantId}", tenantId);
            return ApiResponseDto<UserDto>.ErrorResult("Failed to create tenant admin user");
        }
    }

    /// <summary>
    /// Create a new tenant and associate it with an existing user as admin
    /// </summary>
    public async Task<ApiResponseDto<TenantDto>> CreateTenantForExistingUserAsync(
        int userId, 
        string tenantName, 
        string? tenantDomain = null)
    {
        // 🔧 CRITICAL DEBUG: Add extensive logging at method entry
        _logger.LogWarning("🚀 STARTING CreateTenantForExistingUserAsync - UserId: {UserId}, TenantName: {TenantName}", userId, tenantName);
        
        using var transaction = await _context.Database.BeginTransactionAsync();
        
        try
        {
            _logger.LogWarning("🔍 DEBUG: Transaction started for tenant creation");
            _logger.LogInformation("Creating additional tenant {TenantName} for user {UserId}", 
                tenantName, userId);

            // 1. Verify user exists and is active
            _logger.LogWarning("🔍 DEBUG: Step 1 - Looking for user {UserId}", userId);
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null || !user.IsActive)
            {
                _logger.LogError("❌ User {UserId} not found or inactive", userId);
                return ApiResponseDto<TenantDto>.ErrorResult("User not found or inactive");
            }

            _logger.LogWarning("✅ User {UserId} found and active: {Email}", userId, user.Email);

            // 2. Check if tenant name already exists
            _logger.LogWarning("🔍 DEBUG: Step 2 - Checking if tenant name '{TenantName}' already exists", tenantName);
            var existingTenant = await _context.Tenants
                .FirstOrDefaultAsync(t => t.Name == tenantName);

            if (existingTenant != null)
            {
                _logger.LogError("❌ Tenant name '{TenantName}' already exists with ID {ExistingTenantId}", tenantName, existingTenant.Id);
                return ApiResponseDto<TenantDto>.ErrorResult(
                    $"An organization named '{tenantName}' already exists.");
            }

            _logger.LogWarning("✅ Tenant name '{TenantName}' is available", tenantName);

            // 3. Create new tenant
            _logger.LogWarning("🔍 DEBUG: Step 3 - Creating new tenant");
            var tenant = new Tenant
            {
                Name = tenantName,
                Domain = tenantDomain,
                SubscriptionPlan = "Basic",
                IsActive = true,
                Settings = "{}",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            _logger.LogWarning("✅ Created tenant {TenantId}: {TenantName}", tenant.Id, tenant.Name);

            // 4. Create default roles for the tenant and capture the admin role
            _logger.LogWarning("🔍 DEBUG: Step 4 - Creating default roles for tenant {TenantId}", tenant.Id);
            Role? tenantAdminRole = null;

            try
            {
                await _roleTemplateService.CreateDefaultRolesForTenantAsync(tenant.Id);
                _logger.LogWarning("✅ Default roles created successfully for tenant {TenantId}", tenant.Id);
                
                // 🔧 CRITICAL FIX: Get the Tenant Admin role immediately after creation
                tenantAdminRole = _context.Roles.Local
                    .FirstOrDefault(r => r.TenantId == tenant.Id && r.Name == "Tenant Admin");
                    
                if (tenantAdminRole == null)
                {
                    // Fallback to database query
                    tenantAdminRole = await _context.Roles
                        .Where(r => r.TenantId == tenant.Id && r.Name == "Tenant Admin")
                        .FirstOrDefaultAsync();
                }
            }
            catch (Exception roleEx)
            {
                _logger.LogError(roleEx, "❌ FAILED to create default roles for tenant {TenantId}", tenant.Id);
                await transaction.RollbackAsync();
                return ApiResponseDto<TenantDto>.ErrorResult("Failed to create default roles for organization");
            }

            if (tenantAdminRole == null)
            {
                _logger.LogError("❌ Tenant Admin role not found immediately after creation for tenant {TenantId}", tenant.Id);
                await transaction.RollbackAsync();
                return ApiResponseDto<TenantDto>.ErrorResult("Failed to create admin role for new organization");
            }

            _logger.LogWarning("✅ Found Tenant Admin role {RoleId} for tenant {TenantId}", 
                tenantAdminRole.Id, tenant.Id);

            // 6. Create RBAC UserRole assignment
            _logger.LogWarning("🔍 DEBUG: Step 7 - Creating UserRole assignment");
            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = tenantAdminRole.Id,
                TenantId = tenant.Id,
                IsActive = true,
                AssignedAt = DateTime.UtcNow,
                AssignedBy = "System",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.UserRoles.Add(userRole);

            // 7. Create legacy TenantUser relationship (for tenant switcher)
            _logger.LogWarning("🔍 DEBUG: Step 8 - Creating legacy TenantUser relationship");
            var tenantUser = new TenantUser
            {
                TenantId = tenant.Id,
                UserId = userId,
                Role = "TenantAdmin", // Legacy string role for backward compatibility
                IsActive = true,
                JoinedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TenantUsers.Add(tenantUser);
            
            _logger.LogWarning("🔍 DEBUG: Step 9 - Final save and commit");
            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            _logger.LogWarning("🎉 SUCCESS: User {UserId} created tenant {TenantName} (ID: {TenantId})", 
                userId, tenant.Name, tenant.Id);

            // 8. Map to DTO and return
            var tenantDto = _mapper.Map<TenantDto>(tenant);
            
            return ApiResponseDto<TenantDto>.SuccessResult(tenantDto, 
                $"Organization '{tenantName}' created successfully! You have been granted administrator access.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ CRITICAL FAILURE: Failed to create additional tenant {TenantName} for user {UserId}", 
                tenantName, userId);
            
            try
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("✅ Transaction rolled back successfully");
            }
            catch (Exception rollbackEx)
            {
                _logger.LogError(rollbackEx, "❌ FAILED to rollback transaction during tenant creation");
            }

            return ApiResponseDto<TenantDto>.ErrorResult("Failed to create organization. Please try again.");
        }
    }
}
