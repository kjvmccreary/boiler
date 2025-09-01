using System.Security.Claims;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

public class UserContext : IUserContext
{
    public int? UserId { get; }
    public IReadOnlyCollection<string> Roles { get; }

    public UserContext(IHttpContextAccessor accessor)
    {
        var httpUser = accessor.HttpContext?.User;
        if (httpUser == null)
        {
            UserId = null;
            Roles = Array.Empty<string>();
            return;
        }
        var idClaim = httpUser.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (int.TryParse(idClaim, out var parsed))
            UserId = parsed;
        Roles = httpUser.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type == "role" || c.Type == "roles")
            .SelectMany(c => c.Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();
    }
}
