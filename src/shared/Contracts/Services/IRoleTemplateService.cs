using DTOs.Tenant;

namespace Contracts.Services;

public interface IRoleTemplateService
{
    Task CreateDefaultRolesForTenantAsync(int tenantId);
    Task CreateRoleFromTemplateAsync(int tenantId, string templateName);
    Task<List<RoleTemplateDto>> GetAvailableTemplatesAsync();
}
