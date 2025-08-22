using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Common.Data;
using Common.Monitoring;
using System.Diagnostics;

namespace UserService.HealthChecks;

/// <summary>
/// Enhanced database health check with performance monitoring
/// Phase 11 Session 2 - Monitoring Infrastructure
/// </summary>
public class EnhancedDatabaseHealthCheck : IHealthCheck
{
    private readonly ApplicationDbContext _context;
    private readonly IMetricsCollector _metricsCollector;
    private readonly ILogger<EnhancedDatabaseHealthCheck> _logger;

    public EnhancedDatabaseHealthCheck(
        ApplicationDbContext context,
        IMetricsCollector metricsCollector,
        ILogger<EnhancedDatabaseHealthCheck> logger)
    {
        _context = context;
        _metricsCollector = metricsCollector;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var healthData = new Dictionary<string, object>();
        var issues = new List<string>();
        
        try
        {
            // Test 1: Database Connectivity
            await TestDatabaseConnectivity(healthData, issues, cancellationToken);
            
            // Test 2: Query Performance
            await TestQueryPerformance(healthData, issues, cancellationToken);
            
            // Test 3: Data Integrity
            await TestDataIntegrity(healthData, issues, cancellationToken);
            
            // Test 4: Database Statistics
            await CollectDatabaseStatistics(healthData, issues, cancellationToken);
            
            stopwatch.Stop();
            
            // Record health check metrics
            var isHealthy = !issues.Any(i => i.Contains("CRITICAL"));
            _metricsCollector.RecordHealthCheck("database", isHealthy, stopwatch.Elapsed);
            
            healthData["check_duration_ms"] = stopwatch.ElapsedMilliseconds;
            healthData["timestamp"] = DateTime.UtcNow;
            healthData["issues_found"] = issues.Count;
            
            if (issues.Count == 0)
            {
                return HealthCheckResult.Healthy(
                    "Database is operating optimally",
                    healthData);
            }
            else if (issues.Any(i => i.Contains("CRITICAL")))
            {
                return HealthCheckResult.Unhealthy(
                    $"Critical database issues: {string.Join("; ", issues)}",
                    data: healthData);
            }
            else
            {
                return HealthCheckResult.Degraded(
                    $"Database performance issues: {string.Join("; ", issues)}",
                    data: healthData);
            }
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _metricsCollector.RecordHealthCheck("database", false, stopwatch.Elapsed);
            
            _logger.LogError(ex, "Database health check failed");
            
            return HealthCheckResult.Unhealthy(
                "Database health check failed",
                ex,
                new Dictionary<string, object>
                {
                    ["check_duration_ms"] = stopwatch.ElapsedMilliseconds,
                    ["error_type"] = ex.GetType().Name,
                    ["error_message"] = ex.Message
                });
        }
    }

    private async Task TestDatabaseConnectivity(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        var connectivityStopwatch = Stopwatch.StartNew();
        
        try
        {
            // Test basic connectivity
            var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
            
            connectivityStopwatch.Stop();
            
            healthData["can_connect"] = canConnect;
            healthData["connectivity_test_ms"] = connectivityStopwatch.ElapsedMilliseconds;
            
            if (!canConnect)
            {
                issues.Add("CRITICAL: Cannot connect to database");
            }
            else if (connectivityStopwatch.ElapsedMilliseconds > 1000)
            {
                issues.Add($"WARNING: Database connectivity slow ({connectivityStopwatch.ElapsedMilliseconds}ms > 1000ms)");
            }
            
            // Record database operation metric
            _metricsCollector.RecordDatabaseQuery("connectivity_test", connectivityStopwatch.Elapsed, canConnect);
        }
        catch (Exception ex)
        {
            connectivityStopwatch.Stop();
            issues.Add($"CRITICAL: Database connectivity test failed - {ex.Message}");
            healthData["connectivity_error"] = ex.Message;
            
            _metricsCollector.RecordDatabaseQuery("connectivity_test", connectivityStopwatch.Elapsed, false);
        }
    }

    private async Task TestQueryPerformance(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        var queryTimes = new List<long>();
        
        try
        {
            // Test 1: Simple count query
            var countStopwatch = Stopwatch.StartNew();
            var userCount = await _context.Users.CountAsync(cancellationToken);
            countStopwatch.Stop();
            queryTimes.Add(countStopwatch.ElapsedMilliseconds);
            
            healthData["user_count"] = userCount;
            healthData["count_query_ms"] = countStopwatch.ElapsedMilliseconds;
            
            _metricsCollector.RecordDatabaseQuery("count_users", countStopwatch.Elapsed, true);
            
            // Test 2: Join query (permissions)
            var joinStopwatch = Stopwatch.StartNew();
            var permissionCount = await _context.UserRoles
                .Join(_context.RolePermissions, ur => ur.RoleId, rp => rp.RoleId, (ur, rp) => rp)
                .CountAsync(cancellationToken);
            joinStopwatch.Stop();
            queryTimes.Add(joinStopwatch.ElapsedMilliseconds);
            
            healthData["permission_associations_count"] = permissionCount;
            healthData["join_query_ms"] = joinStopwatch.ElapsedMilliseconds;
            
            _metricsCollector.RecordDatabaseQuery("count_permissions", joinStopwatch.Elapsed, true);
            
            // Test 3: Complex query with filters
            var complexStopwatch = Stopwatch.StartNew();
            var activeUsersCount = await _context.Users
                .Where(u => u.IsActive)
                .Join(_context.UserRoles.Where(ur => ur.IsActive), 
                    u => u.Id, ur => ur.UserId, (u, ur) => u)
                .Distinct()
                .CountAsync(cancellationToken);
            complexStopwatch.Stop();
            queryTimes.Add(complexStopwatch.ElapsedMilliseconds);
            
            healthData["active_users_with_roles_count"] = activeUsersCount;
            healthData["complex_query_ms"] = complexStopwatch.ElapsedMilliseconds;
            
            _metricsCollector.RecordDatabaseQuery("count_active_users_with_roles", complexStopwatch.Elapsed, true);
            
            // Analyze performance
            var avgQueryTime = queryTimes.Average();
            var maxQueryTime = queryTimes.Max();
            
            healthData["avg_query_time_ms"] = avgQueryTime;
            healthData["max_query_time_ms"] = maxQueryTime;
            healthData["total_queries_tested"] = queryTimes.Count;
            
            if (avgQueryTime > 500)
            {
                issues.Add($"CRITICAL: Average query time too high ({avgQueryTime:F1}ms > 500ms)");
            }
            else if (avgQueryTime > 200)
            {
                issues.Add($"WARNING: Average query time elevated ({avgQueryTime:F1}ms > 200ms)");
            }
            
            if (maxQueryTime > 2000)
            {
                issues.Add($"WARNING: Maximum query time high ({maxQueryTime}ms > 2000ms)");
            }
        }
        catch (Exception ex)
        {
            issues.Add($"CRITICAL: Database query performance test failed - {ex.Message}");
            healthData["query_performance_error"] = ex.Message;
        }
    }

    private async Task TestDataIntegrity(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        try
        {
            var integrityStopwatch = Stopwatch.StartNew();
            
            // Test 1: Check for orphaned records
            var orphanedUserRoles = await _context.UserRoles
                .Where(ur => !_context.Users.Any(u => u.Id == ur.UserId))
                .CountAsync(cancellationToken);
            
            var orphanedRolePermissions = await _context.RolePermissions
                .Where(rp => !_context.Roles.Any(r => r.Id == rp.RoleId))
                .CountAsync(cancellationToken);
            
            // Test 2: Check required system data
            var systemRoleCount = await _context.Roles
                .Where(r => r.IsSystemRole)
                .CountAsync(cancellationToken);
            
            var permissionCount = await _context.Permissions.CountAsync(cancellationToken);
            
            integrityStopwatch.Stop();
            
            healthData["orphaned_user_roles"] = orphanedUserRoles;
            healthData["orphaned_role_permissions"] = orphanedRolePermissions;
            healthData["system_roles_count"] = systemRoleCount;
            healthData["total_permissions_count"] = permissionCount;
            healthData["integrity_check_ms"] = integrityStopwatch.ElapsedMilliseconds;
            
            if (orphanedUserRoles > 0)
            {
                issues.Add($"WARNING: Found {orphanedUserRoles} orphaned user role assignments");
            }
            
            if (orphanedRolePermissions > 0)
            {
                issues.Add($"WARNING: Found {orphanedRolePermissions} orphaned role permissions");
            }
            
            if (systemRoleCount == 0)
            {
                issues.Add("CRITICAL: No system roles found - database may not be properly seeded");
            }
            
            if (permissionCount == 0)
            {
                issues.Add("CRITICAL: No permissions found - database may not be properly seeded");
            }
            
            _metricsCollector.RecordDatabaseQuery("integrity_check", integrityStopwatch.Elapsed, true);
        }
        catch (Exception ex)
        {
            issues.Add($"CRITICAL: Database integrity check failed - {ex.Message}");
            healthData["integrity_check_error"] = ex.Message;
        }
    }

    private async Task CollectDatabaseStatistics(Dictionary<string, object> healthData, List<string> issues, CancellationToken cancellationToken)
    {
        try
        {
            var statsStopwatch = Stopwatch.StartNew();
            
            // Collect table statistics
            var tableStats = new Dictionary<string, object>();
            
            tableStats["users"] = await _context.Users.CountAsync(cancellationToken);
            tableStats["roles"] = await _context.Roles.CountAsync(cancellationToken);
            tableStats["permissions"] = await _context.Permissions.CountAsync(cancellationToken);
            tableStats["user_roles"] = await _context.UserRoles.CountAsync(cancellationToken);
            tableStats["role_permissions"] = await _context.RolePermissions.CountAsync(cancellationToken);
            tableStats["tenants"] = await _context.Tenants.CountAsync(cancellationToken);
            
            // Check if audit tables exist and get counts
            try
            {
                var auditTableStats = new Dictionary<string, object>();
                
                // Try to get audit counts (may not exist in all environments)
                if (_context.Model.FindEntityType(typeof(DTOs.Entities.PermissionAuditEntry)) != null)
                {
                    auditTableStats["permission_audits"] = await _context.Set<DTOs.Entities.PermissionAuditEntry>().CountAsync(cancellationToken);
                }
                
                if (_context.Model.FindEntityType(typeof(DTOs.Entities.RoleChangeAuditEntry)) != null)
                {
                    auditTableStats["role_change_audits"] = await _context.Set<DTOs.Entities.RoleChangeAuditEntry>().CountAsync(cancellationToken);
                }
                
                if (_context.Model.FindEntityType(typeof(DTOs.Entities.SecurityEventAuditEntry)) != null)
                {
                    auditTableStats["security_event_audits"] = await _context.Set<DTOs.Entities.SecurityEventAuditEntry>().CountAsync(cancellationToken);
                }
                
                healthData["audit_table_stats"] = auditTableStats;
            }
            catch (Exception auditEx)
            {
                _logger.LogDebug(auditEx, "Could not collect audit table statistics (may not be available)");
                healthData["audit_tables_status"] = "NOT_AVAILABLE";
            }
            
            statsStopwatch.Stop();
            
            healthData["table_stats"] = tableStats;
            healthData["stats_collection_ms"] = statsStopwatch.ElapsedMilliseconds;
            
            // Analyze table health
            var totalUsers = (int)tableStats["users"];
            var totalRoles = (int)tableStats["roles"];
            var totalPermissions = (int)tableStats["permissions"];
            
            if (totalUsers == 0)
            {
                issues.Add("WARNING: No users found in database");
            }
            else if (totalUsers > 10000)
            {
                healthData["user_scale"] = "LARGE";
            }
            else if (totalUsers > 1000)
            {
                healthData["user_scale"] = "MEDIUM";
            }
            else
            {
                healthData["user_scale"] = "SMALL";
            }
            
            if (totalRoles < 2)
            {
                issues.Add("WARNING: Very few roles defined - may indicate setup issue");
            }
            
            if (totalPermissions < 10)
            {
                issues.Add("WARNING: Very few permissions defined - may indicate setup issue");
            }
            
            _metricsCollector.RecordDatabaseQuery("collect_statistics", statsStopwatch.Elapsed, true);
        }
        catch (Exception ex)
        {
            issues.Add($"WARNING: Could not collect database statistics - {ex.Message}");
            healthData["stats_collection_error"] = ex.Message;
        }
    }
}
