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

            // Create admin user for the tenant
            var adminUser = await CreateTenantAdminUserAsync(tenant.Id, createDto.AdminUser);
            
            if (!adminUser.Success)
            {
                // Rollback tenant creation if admin user creation fails
                _context.Tenants.Remove(tenant);
                await _context.SaveChangesAsync();
                return ApiResponseDto<TenantDto>.ErrorResult($"Failed to create admin user: {adminUser.Message}");
            }

            // Initialize tenant with default roles
            await _roleTemplateService.CreateDefaultRolesForTenantAsync(tenant.Id);

            // Map to DTO
            var tenantDto = _mapper.Map<TenantDto>(tenant);
            
            // Audit the creation
            await _auditService.LogAsync(AuditAction.UserCreated, $"tenants/{tenant.Id}", 
                $"Tenant '{tenant.Name}' created", true);

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

    private async Task<ApiResponseDto<UserDto>> CreateTenantAdminUserAsync(int tenantId, CreateTenantAdminDto adminDto)
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

            // Create TenantUser relationship
            var tenantUser = new TenantUser
            {
                TenantId = tenantId,
                UserId = user.Id,
                Role = "TenantAdmin",
                IsActive = true,
                JoinedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TenantUsers.Add(tenantUser);
            await _context.SaveChangesAsync();

            var userDto = _mapper.Map<UserDto>(user);
            return ApiResponseDto<UserDto>.SuccessResult(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tenant admin user");
            return ApiResponseDto<UserDto>.ErrorResult("Failed to create tenant admin user");
        }
    }
}
