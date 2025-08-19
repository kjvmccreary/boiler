// FILE: src/shared/Common/Services/TenantProvider.cs
using System.Security.Claims;
using Common.Configuration;
using Common.Constants; // ✅ ADD: Missing namespace import
using Contracts.Services;
using DTOs.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Common.Services;

public class TenantProvider : ITenantProvider
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly TenantSettings _tenantSettings;
    private readonly ILogger<TenantProvider> _logger;
    private readonly AsyncLocal<int?> _currentTenantId = new();
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

    public async Task<int?> GetCurrentTenantIdAsync()
    {
        // First check if tenant ID is already set in the async local
        if (_currentTenantId.Value.HasValue)
            return _currentTenantId.Value;

        var context = _httpContextAccessor.HttpContext;
        if (context == null)
        {
            // If no HTTP context, check for default tenant
            if (!string.IsNullOrEmpty(_tenantSettings.DefaultTenantId) &&
                int.TryParse(_tenantSettings.DefaultTenantId, out var defaultTenantId))
            {
                _logger.LogDebug("Using default tenant ID: {TenantId}", defaultTenantId);
                return defaultTenantId;
            }
            _logger.LogWarning("No HTTP context and no default tenant ID configured");
            return null;
        }

        try
        {
            var tenantId = await ResolveTenantIdAsync(context);
            if (tenantId.HasValue)
            {
                _currentTenantId.Value = tenantId;
                _logger.LogDebug("Resolved tenant ID: {TenantId}", tenantId);
            }
            else
            {
                _logger.LogWarning("Failed to resolve tenant ID from any source");
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

    public async Task SetCurrentTenantAsync(int tenantId)
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

        if (int.TryParse(tenantIdentifier, out var tenantId))
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

    private async Task<int?> ResolveTenantIdAsync(HttpContext context)
    {
        var identifier = await ResolveTenantIdentifierAsync(context);
        if (string.IsNullOrEmpty(identifier))
            return null;

        // Try to parse as int first
        if (int.TryParse(identifier, out var tenantId))
            return tenantId;

        // TODO: In a real implementation, you would look up the tenant ID by identifier (domain, etc.)
        // For now, we'll return null if it's not a valid int
        return null;
    }

    private async Task<string?> ResolveTenantIdentifierAsync(HttpContext context)
    {
        // ✅ IMPROVED: Always try claims first for authenticated requests
        if (context.User?.Identity?.IsAuthenticated == true)
        {
            var claimsResult = await ResolveFromClaimsAsync(context);
            if (!string.IsNullOrEmpty(claimsResult))
            {
                _logger.LogDebug("Resolved tenant from JWT claims: {TenantId}", claimsResult);
                return claimsResult;
            }
        }

        // ✅ IMPROVED: Then try the configured strategy
        var result = _tenantSettings.ResolutionStrategy switch
        {
            TenantResolutionStrategy.Header => ResolveFromHeader(context),
            TenantResolutionStrategy.Domain => ResolveFromDomain(context),
            TenantResolutionStrategy.Subdomain => ResolveFromSubdomain(context),
            TenantResolutionStrategy.Path => ResolveFromPath(context),
            _ => ResolveFromHeader(context) // Default fallback
        };

        if (!string.IsNullOrEmpty(result))
        {
            _logger.LogDebug("Resolved tenant from {Strategy}: {TenantId}", _tenantSettings.ResolutionStrategy, result);
            return result;
        }

        _logger.LogWarning("Could not resolve tenant from any source. Strategy: {Strategy}, IsAuthenticated: {IsAuth}", 
            _tenantSettings.ResolutionStrategy, context.User?.Identity?.IsAuthenticated);
        
        return null;
    }

    private string? ResolveFromHeader(HttpContext context)
    {
        // 🔧 DEBUG: Log all headers to see what's being sent
        var allHeaders = context.Request.Headers
            .Select(h => $"{h.Key}={string.Join(",", h.Value.ToArray())}")  
            .ToArray();
        _logger.LogInformation("🔍 All request headers: {Headers}", string.Join("; ", allHeaders.ToArray()));  

        // 🔧 TRY: Multiple header name variations
        var headerNames = new[] { "X-Tenant-Id", "X-Tenant-ID", _tenantSettings.TenantHeaderName };
        
        foreach (var headerName in headerNames)
        {
            if (context.Request.Headers.TryGetValue(headerName, out var headerValue))
            {
                var value = headerValue.FirstOrDefault();
                if (!string.IsNullOrEmpty(value))
                {
                    _logger.LogInformation("🏢 FOUND tenant from header '{HeaderName}': {TenantId}", headerName, value);
                    return value;
                }
            }
        }

        _logger.LogWarning("❌ No tenant found in any headers: {HeaderNames}", string.Join(", ", headerNames.ToArray()));  
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
            // ✅ FIXED: Use the correct namespace for ClaimTypes
            var tenantClaim = context.User.FindFirst(Common.Constants.ClaimTypes.TenantId);
            if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
            {
                _logger.LogDebug("Resolved tenant from claims: {TenantId}", tenantClaim.Value);
                return tenantClaim.Value;
            }

            // ✅ DEBUG: Log all claims to help troubleshoot
            var allClaims = context.User.Claims.Select(c => $"{c.Type}={c.Value}").ToArray();
            _logger.LogDebug("Available claims: {Claims}", string.Join(", ", allClaims.ToArray()));  // 🔧 FIX: Add .ToArray()
        }

        await Task.CompletedTask;
        return null;
    }
}
