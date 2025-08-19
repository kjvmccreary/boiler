using DTOs.Common;
using DTOs.Tenant;
using DTOs.User;

namespace Contracts.Services;

/// <summary>
/// Service interface for tenant management operations
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Create a new tenant with admin user
    /// </summary>
    Task<ApiResponseDto<TenantDto>> CreateTenantAsync(CreateTenantDto createDto);

    /// <summary>
    /// Get tenant by ID
    /// </summary>
    Task<ApiResponseDto<TenantDto>> GetTenantAsync(int tenantId);

    /// <summary>
    /// Get all tenants (paginated)
    /// </summary>
    Task<ApiResponseDto<PagedResultDto<TenantDto>>> GetTenantsAsync(int page = 1, int pageSize = 20);

    /// <summary>
    /// Update tenant
    /// </summary>
    Task<ApiResponseDto<TenantDto>> UpdateTenantAsync(int tenantId, UpdateTenantDto updateDto);

    /// <summary>
    /// Delete tenant (only if no users)
    /// </summary>
    Task<ApiResponseDto<bool>> DeleteTenantAsync(int tenantId);

    /// <summary>
    /// Activate tenant
    /// </summary>
    Task<ApiResponseDto<bool>> ActivateTenantAsync(int tenantId);

    /// <summary>
    /// Deactivate tenant
    /// </summary>
    Task<ApiResponseDto<bool>> DeactivateTenantAsync(int tenantId);

    /// <summary>
    /// Initialize tenant with default roles and permissions
    /// </summary>
    Task<ApiResponseDto<TenantDto>> InitializeTenantAsync(TenantInitializationDto initDto);

    /// <summary>
    /// Create tenant admin user
    /// </summary>
    Task<ApiResponseDto<UserDto>> CreateTenantAdminUserAsync(int tenantId, CreateTenantAdminDto adminDto);

    /// <summary>
    /// Create a new tenant and associate it with an existing user as admin (consultant scenario)
    /// </summary>
    Task<ApiResponseDto<TenantDto>> CreateTenantForExistingUserAsync(
        int userId,
        string tenantName,
        string? tenantDomain = null);
}
