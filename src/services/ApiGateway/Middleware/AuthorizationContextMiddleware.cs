using System.Text.Json;

namespace ApiGateway.Middleware;

public class AuthorizationContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuthorizationContextMiddleware> _logger;

    public AuthorizationContextMiddleware(RequestDelegate next, ILogger<AuthorizationContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Forward authorization headers to downstream services
        if (context.Request.Headers.ContainsKey("Authorization"))
        {
            var authHeader = context.Request.Headers["Authorization"].ToString();
            if (!string.IsNullOrEmpty(authHeader))
            {
                // Ensure authorization header is properly forwarded
                context.Request.Headers["X-Forwarded-Authorization"] = authHeader;
                
                _logger.LogDebug("Authorization header forwarded to downstream service");
            }
        }

        // Forward user context if authenticated
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userContext = new
            {
                UserId = context.User.FindFirst("sub")?.Value ?? context.User.FindFirst("user_id")?.Value,
                TenantId = context.User.FindFirst("tenant_id")?.Value,
                Roles = context.User.FindAll("role").Select(c => c.Value).ToArray(),
                Permissions = context.User.FindAll("permission").Select(c => c.Value).ToArray(),
                Email = context.User.FindFirst("email")?.Value
            };

            var userContextJson = JsonSerializer.Serialize(userContext);
            context.Request.Headers["X-User-Context"] = Convert.ToBase64String(
                System.Text.Encoding.UTF8.GetBytes(userContextJson));

            _logger.LogDebug("User context forwarded: {UserId} with {PermissionCount} permissions", 
                userContext.UserId, userContext.Permissions.Length);
        }

        // Forward tenant context
        if (context.Items.ContainsKey("TenantId"))
        {
            context.Request.Headers["X-Tenant-Context"] = context.Items["TenantId"]?.ToString();
        }

        await _next(context);
    }
}
