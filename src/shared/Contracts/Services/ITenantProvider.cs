// FILE: src/shared/Contracts/Services/ITenantProvider.cs
namespace Contracts.Services;

public interface ITenantProvider
{
    Task<Guid?> GetCurrentTenantIdAsync();
    Task<string?> GetCurrentTenantIdentifierAsync();
    Task SetCurrentTenantAsync(Guid tenantId);
    Task SetCurrentTenantAsync(string tenantIdentifier);
    Task ClearCurrentTenantAsync();
    bool HasTenantContext { get; }
}
