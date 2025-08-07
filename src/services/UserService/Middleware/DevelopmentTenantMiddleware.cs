using Common.Services;
using Contracts.Services;

namespace UserService.Middleware;

public class DevelopmentTenantMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DevelopmentTenantMiddleware> _logger;

    public DevelopmentTenantMiddleware(RequestDelegate next, ILogger<DevelopmentTenantMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ITenantProvider tenantProvider)
    {
        // Only apply in development environment
        var env = context.RequestServices.GetRequiredService<IWebHostEnvironment>();
        if (env.IsDevelopment())
        {
            // Check if X-Tenant-ID header is missing
            if (!context.Request.Headers.ContainsKey("X-Tenant-ID"))
            {
                // 🔧 FIX: Use proper header method to avoid duplicate key warning
                context.Request.Headers["X-Tenant-ID"] = "1";
                _logger.LogDebug("Added default tenant header for development testing");
            }
        }

        await _next(context);
    }
}
