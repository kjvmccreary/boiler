using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace WorkflowService.Tests.Infrastructure;

public sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string Scheme = "Test";

    public TestAuthHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock) { }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var stage = Request.Headers.TryGetValue("X-Test-Auth-Stage", out var stageVal)
            ? stageVal.ToString()
            : "2";

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Name, "workflow-test-user"),
            new("permission", "workflow.read"),
            new("permission", "workflow.write"),
            new("permission", "workflow.admin")
        };

        if (stage != "1" &&
            Request.Headers.TryGetValue("X-Tenant-ID", out var tenantHeader) &&
            int.TryParse(tenantHeader.ToString(), out var tenantId))
        {
            claims.Add(new Claim("tenantId", tenantId.ToString()));
        }

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, Scheme));
        var ticket = new AuthenticationTicket(principal, Scheme);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
