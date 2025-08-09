using System.Security.Claims;

namespace ApiGateway.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolutionMiddleware> _logger;

    public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var tenantId = ExtractTenantId(context);
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            context.Items["TenantId"] = tenantId;
            context.Request.Headers["X-Tenant-ID"] = tenantId;
            
            _logger.LogInformation("✅ Tenant resolved: {TenantId} from {Source}", 
                tenantId, GetTenantSource(context));
        }
        else
        {
            // 🔧 FIX: Only warn if this is an authenticated request that should have tenant info
            if (context.User.Identity?.IsAuthenticated == true)
            {
                _logger.LogWarning("⚠️ No tenant ID found for authenticated request to {Path}", context.Request.Path);
            }
            else
            {
                _logger.LogDebug("ℹ️ No tenant ID found for anonymous request to {Path}", context.Request.Path);
            }
        }

        await _next(context);
    }

    private string? ExtractTenantId(HttpContext context)
    {
        // 1. Try to get from JWT claims (now available since authentication ran first)
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("tenant_id");
            if (tenantClaim != null)
            {
                return tenantClaim.Value;
            }
        }

        // 2. Try to get from domain (subdomain)
        var host = context.Request.Host.Host;
        if (host.Contains('.') && !host.StartsWith("localhost"))
        {
            var subdomain = host.Split('.')[0];
            if (!string.IsNullOrEmpty(subdomain) && subdomain != "www" && subdomain != "api")
            {
                return subdomain;
            }
        }

        // 3. Try to get from header
        if (context.Request.Headers.ContainsKey("X-Tenant-ID"))
        {
            return context.Request.Headers["X-Tenant-ID"];
        }

        // 4. Try to get from path (if using path-based routing)
        var path = context.Request.Path.Value;
        if (path?.StartsWith("/tenant/") == true)
        {
            var segments = path.Split('/');
            if (segments.Length > 2)
            {
                return segments[2];
            }
        }

        return null;
    }

    private string GetTenantSource(HttpContext context)
    {
        // Check JWT claims first (since authentication now runs before this middleware)
        if (context.User.Identity?.IsAuthenticated == true && context.User.FindFirst("tenant_id") != null)
        {
            return "jwt-claim";
        }
        
        var host = context.Request.Host.Host;
        if (host.Contains('.') && !host.StartsWith("localhost"))
        {
            return "domain";
        }
        
        if (context.Request.Headers.ContainsKey("X-Tenant-ID"))
        {
            return "header";
        }
        
        if (context.Request.Path.Value?.StartsWith("/tenant/") == true)
        {
            return "path";
        }
        
        return "unknown";
    }
}
