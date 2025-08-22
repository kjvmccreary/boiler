using Microsoft.Extensions.Diagnostics.HealthChecks;
using Common.Data;

namespace UserService.HealthChecks;

/// <summary>
/// Health check for database connectivity
/// </summary>
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;

    public DatabaseHealthCheck(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            
            if (canConnect)
            {
                return HealthCheckResult.Healthy("Database connection successful");
            }
            
            return HealthCheckResult.Unhealthy("Database connection failed");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Database connection failed", ex);
        }
    }
}
