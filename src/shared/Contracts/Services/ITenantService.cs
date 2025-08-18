using DTOs.Common;
using DTOs.Tenant;

namespace Contracts.Services;

public interface ITenantService
{
    Task<ApiResponseDto<TenantDto>> CreateTenantAsync(CreateTenantDto createDto);
    Task<ApiResponseDto<TenantDto>> GetTenantAsync(int tenantId);
    Task<ApiResponseDto<PagedResultDto<TenantDto>>> GetTenantsAsync(int page = 1, int pageSize = 20);
    Task<ApiResponseDto<TenantDto>> UpdateTenantAsync(int tenantId, UpdateTenantDto updateDto);
    Task<ApiResponseDto<bool>> DeleteTenantAsync(int tenantId);
    Task<ApiResponseDto<bool>> ActivateTenantAsync(int tenantId);
    Task<ApiResponseDto<bool>> DeactivateTenantAsync(int tenantId);
    Task<ApiResponseDto<TenantDto>> InitializeTenantAsync(TenantInitializationDto initDto);
}
