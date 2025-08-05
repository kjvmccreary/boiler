// FILE: src/shared/Contracts/Services/ITenantProvider.cs
namespace Contracts.Services;

public interface ITenantProvider
{
    Task<int?> GetCurrentTenantIdAsync();
    Task<string?> GetCurrentTenantIdentifierAsync();
    Task SetCurrentTenantAsync(int tenantId);
    Task SetCurrentTenantAsync(string tenantIdentifier);
    Task ClearCurrentTenantAsync();
    bool HasTenantContext { get; }
}
