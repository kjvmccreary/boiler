// FILE: src/shared/Common/Services/TenantProvider.cs
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Common.Configuration;
using Contracts.Services;
using System.Security.Claims;
using DTOs.Entities;


namespace Common.Services;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantSettings _tenantSettings;
    private readonly ILogger<TenantProvider> _logger;
    private readonly AsyncLocal<Guid?> _currentTenantId = new();
    private readonly AsyncLocal<string?> _currentTenantIdentifier = new();

    public TenantProvider(
        IHttpContextAccessor httpContextAccessor,
        TenantSettings tenantSettings,
        ILogger<TenantProvider> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _tenantSettings = tenantSettings;
        _logger = logger;
    }

    public bool HasTenantContext => _currentTenantId.Value.HasValue || !string.IsNullOrEmpty(_currentTenantIdentifier.Value);

    public async Task<Guid?> GetCurrentTenantIdAsync()
    {
        // First check if tenant ID is already set in the async local
        if (_currentTenantId.Value.HasValue)
            return _currentTenantId.Value;

        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            // If no HTTP context, check for default tenant
            if (!string.IsNullOrEmpty(_tenantSettings.DefaultTenantId) &&
                Guid.TryParse(_tenantSettings.DefaultTenantId, out var defaultTenantId))
            {
                return defaultTenantId;
            }
            return null;
        }

        try
        {
            var tenantId = await ResolveTenantIdAsync(context);
            if (tenantId.HasValue)
            {
                _currentTenantId.Value = tenantId;
            }
            return tenantId;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant ID");
            return null;
        }
    }

    public async Task<string?> GetCurrentTenantIdentifierAsync()
    {
        // First check if tenant identifier is already set
        if (!string.IsNullOrEmpty(_currentTenantIdentifier.Value))
            return _currentTenantIdentifier.Value;

        var context = _httpContextAccessor.HttpContext;
        if (context == null)
            return _tenantSettings.DefaultTenantId;

        try
        {
            var identifier = await ResolveTenantIdentifierAsync(context);
            if (!string.IsNullOrEmpty(identifier))
            {
                _currentTenantIdentifier.Value = identifier;
            }
            return identifier;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error resolving tenant identifier");
            return null;
        }
    }

    public async Task SetCurrentTenantAsync(Guid tenantId)
    {
        _currentTenantId.Value = tenantId;
        _currentTenantIdentifier.Value = tenantId.ToString();

        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Items["TenantId"] = tenantId;
        }

        await Task.CompletedTask;
    }

    public async Task SetCurrentTenantAsync(string tenantIdentifier)
    {
        _currentTenantIdentifier.Value = tenantIdentifier;

        if (Guid.TryParse(tenantIdentifier, out var tenantId))
        {
            _currentTenantId.Value = tenantId;
        }

        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Items["TenantIdentifier"] = tenantIdentifier;
        }

        await Task.CompletedTask;
    }

    public async Task ClearCurrentTenantAsync()
    {
        _currentTenantId.Value = null;
        _currentTenantIdentifier.Value = null;

        var context = _httpContextAccessor.HttpContext;
        if (context != null)
        {
            context.Items.Remove("TenantId");
            context.Items.Remove("TenantIdentifier");
        }

        await Task.CompletedTask;
    }

    private async Task<Guid?> ResolveTenantIdAsync(HttpContext context)
    {
        var identifier = await ResolveTenantIdentifierAsync(context);
        if (string.IsNullOrEmpty(identifier))
            return null;

        // Try to parse as GUID first
        if (Guid.TryParse(identifier, out var tenantId))
            return tenantId;

        // TODO: In a real implementation, you would look up the tenant ID by identifier (domain, etc.)
        // For now, we'll return null if it's not a valid GUID
        return null;
    }

    private async Task<string?> ResolveTenantIdentifierAsync(HttpContext context)
    {
        return _tenantSettings.ResolutionStrategy switch
        {
            TenantResolutionStrategy.Header => ResolveFromHeader(context),
            TenantResolutionStrategy.Domain => ResolveFromDomain(context),
            TenantResolutionStrategy.Subdomain => ResolveFromSubdomain(context),
            TenantResolutionStrategy.Path => ResolveFromPath(context),
            _ => await ResolveFromClaimsAsync(context) ?? ResolveFromHeader(context)
        };
    }

    private string? ResolveFromHeader(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(_tenantSettings.TenantHeaderName, out var headerValue))
        {
            var value = headerValue.FirstOrDefault();
            if (!string.IsNullOrEmpty(value))
            {
                _logger.LogDebug("Resolved tenant from header: {TenantId}", value);
                return value;
            }
        }

        return null;
    }

    private string? ResolveFromDomain(HttpContext context)
    {
        var host = context.Request.Host.Host;
        if (string.IsNullOrEmpty(host))
            return null;

        // Extract domain logic here
        // For example: tenant1.yourapp.com -> tenant1
        // This would typically involve database lookup to match domain to tenant

        _logger.LogDebug("Resolved tenant from domain: {Host}", host);
        return host; // Simplified - in real implementation, map domain to tenant ID
    }

    private string? ResolveFromSubdomain(HttpContext context)
    {
        var host = context.Request.Host.Host;
        if (string.IsNullOrEmpty(host))
            return null;

        var parts = host.Split('.');
        if (parts.Length > 2)
        {
            var subdomain = parts[0];
            _logger.LogDebug("Resolved tenant from subdomain: {Subdomain}", subdomain);
            return subdomain;
        }

        return null;
    }

    private string? ResolveFromPath(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (string.IsNullOrEmpty(path))
            return null;

        // Extract tenant from path: /tenant1/api/users -> tenant1
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length > 0)
        {
            var tenant = segments[0];
            _logger.LogDebug("Resolved tenant from path: {Tenant}", tenant);
            return tenant;
        }

        return null;
    }

    private async Task<string?> ResolveFromClaimsAsync(HttpContext context)
    {
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst(Constants.ClaimTypes.TenantId);
            if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
            {
                _logger.LogDebug("Resolved tenant from claims: {TenantId}", tenantClaim.Value);
                return tenantClaim.Value;
            }
        }

        await Task.CompletedTask;
        return null;
    }
}
