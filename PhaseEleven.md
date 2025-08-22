# Phase 11: Enhanced Security & Monitoring - Detailed Session Breakdown

## Overview
**Duration**: 2-3 sessions  
**Complexity**: Medium-High  
**Focus**: Security enhancements, monitoring setup, and RBAC audit logging

## Prerequisites Completed (Phase 10)
✅ Redis caching fully implemented  
✅ Permission checks < 10ms (cached)  
✅ Cache hit ratio > 95%  
✅ Cache invalidation working correctly  
✅ Performance optimization complete  

---

## Session 1: Security Enhancements & Audit Infrastructure (3-4 hours)

### 1.1 Enhanced Audit Entry Models
**Location**: `src/shared/DTOs/`

```csharp
// File: PermissionAuditEntry.cs
public class PermissionAuditEntry : BaseAuditEntry
{
    public int UserId { get; set; }
    public string Permission { get; set; }
    public string Resource { get; set; }
    public bool Granted { get; set; }
    public string DenialReason { get; set; }
    public TimeSpan CheckDuration { get; set; }
    public bool CacheHit { get; set; }
}

// File: RoleChangeAuditEntry.cs
public class RoleChangeAuditEntry : BaseAuditEntry
{
    public int RoleId { get; set; }
    public string RoleName { get; set; }
    public string ChangeType { get; set; } // Created, Updated, Deleted
    public string OldValue { get; set; } // JSON
    public string NewValue { get; set; } // JSON
    public int ChangedByUserId { get; set; }
}

// File: SecurityEventAuditEntry.cs
public class SecurityEventAuditEntry : BaseAuditEntry
{
    public string EventType { get; set; } // UnauthorizedAccess, SuspiciousActivity, etc.
    public string IpAddress { get; set; }
    public string UserAgent { get; set; }
    public int? UserId { get; set; }
    public string Details { get; set; } // JSON
    public string Severity { get; set; } // Low, Medium, High, Critical
}
```

### 1.2 Audit Service Enhancement
**Location**: `src/shared/Common/Services/`

```csharp
// File: IEnhancedAuditService.cs
public interface IEnhancedAuditService : IAuditService
{
    Task LogPermissionCheckAsync(PermissionAuditEntry entry);
    Task LogRoleChangeAsync(RoleChangeAuditEntry entry);
    Task LogSecurityEventAsync(SecurityEventAuditEntry entry);
    Task<List<PermissionAuditEntry>> GetPermissionAuditLogAsync(
        int? userId = null, 
        DateTime? from = null, 
        DateTime? to = null);
    Task<List<RoleChangeAuditEntry>> GetRoleChangeAuditLogAsync(
        int? roleId = null, 
        DateTime? from = null, 
        DateTime? to = null);
    Task<List<SecurityEventAuditEntry>> GetSecurityEventLogAsync(
        string severity = null, 
        DateTime? from = null, 
        DateTime? to = null);
}

// File: EnhancedAuditService.cs
public class EnhancedAuditService : AuditService, IEnhancedAuditService
{
    private readonly ILogger<EnhancedAuditService> _logger;
    private readonly IDbContext _context;
    private readonly IRedisCache _cache;
    private readonly IMetricsCollector _metrics;

    // Implementation with batching and async processing
}
```

### 1.3 Permission Authorization Handler Enhancement
**Location**: `src/services/AuthService/Authorization/`

```csharp
// File: AuditingPermissionAuthorizationHandler.cs
public class AuditingPermissionAuthorizationHandler : PermissionAuthorizationHandler
{
    private readonly IEnhancedAuditService _auditService;
    private readonly IMetricsCollector _metrics;

    protected override async Task<bool> HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = await base.HandleRequirementAsync(context, requirement);
        stopwatch.Stop();

        // Log permission check
        await _auditService.LogPermissionCheckAsync(new PermissionAuditEntry
        {
            UserId = GetUserId(context),
            Permission = requirement.Permission,
            Resource = GetResource(context),
            Granted = result,
            CheckDuration = stopwatch.Elapsed,
            CacheHit = WasCacheHit()
        });

        // Record metrics
        _metrics.RecordPermissionCheck(requirement.Permission, result, stopwatch.Elapsed);

        return result;
    }
}
```

### 1.4 Security Middleware
**Location**: `src/shared/Common/Middleware/`

```csharp
// File: SecurityHeadersMiddleware.cs
public class SecurityHeadersMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Add security headers
        context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
        context.Response.Headers.Add("X-Frame-Options", "DENY");
        context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
        context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");
        context.Response.Headers.Add("Content-Security-Policy", GetCSPPolicy());
        
        await next(context);
    }
}

// File: SuspiciousActivityDetectionMiddleware.cs
public class SuspiciousActivityDetectionMiddleware
{
    private readonly IEnhancedAuditService _auditService;
    private readonly IRedisCache _cache;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        // Detect suspicious patterns
        if (await IsSuspiciousAsync(context))
        {
            await _auditService.LogSecurityEventAsync(new SecurityEventAuditEntry
            {
                EventType = "SuspiciousActivity",
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers["User-Agent"],
                Details = GetSuspiciousDetails(context),
                Severity = "Medium"
            });
        }

        await next(context);
    }
}
```

---

## Session 2: Monitoring Infrastructure & Metrics (3-4 hours)

### 2.1 Metrics Collection Service
**Location**: `src/shared/Common/Monitoring/`

```csharp
// File: IMetricsCollector.cs
public interface IMetricsCollector
{
    void RecordPermissionCheck(string permission, bool granted, TimeSpan duration);
    void RecordRoleChange(string changeType, int roleId);
    void RecordCacheHit(string cacheKey, bool hit);
    void RecordAuthorizationPerformance(string operation, TimeSpan duration);
    void RecordSecurityEvent(string eventType, string severity);
    Task<MetricsSummary> GetSummaryAsync(TimeSpan period);
}

// File: PrometheusMetricsCollector.cs
public class PrometheusMetricsCollector : IMetricsCollector
{
    private readonly Counter _permissionChecks;
    private readonly Histogram _authorizationDuration;
    private readonly Counter _cacheHits;
    private readonly Counter _roleChanges;
    private readonly Counter _securityEvents;

    public PrometheusMetricsCollector()
    {
        _permissionChecks = Metrics.CreateCounter(
            "permission_checks_total",
            "Total number of permission checks",
            new CounterConfiguration
            {
                LabelNames = new[] { "permission", "granted", "tenant" }
            });

        _authorizationDuration = Metrics.CreateHistogram(
            "authorization_duration_seconds",
            "Duration of authorization checks",
            new HistogramConfiguration
            {
                LabelNames = new[] { "operation", "tenant" },
                Buckets = Histogram.LinearBuckets(0.001, 0.001, 20)
            });
    }
}
```

### 2.2 Health Checks
**Location**: `src/services/*/HealthChecks/`

```csharp
// File: PermissionCacheHealthCheck.cs
public class PermissionCacheHealthCheck : IHealthCheck
{
    private readonly IRedisCache _cache;
    private readonly ILogger<PermissionCacheHealthCheck> _logger;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Test cache connectivity
            await _cache.PingAsync();
            
            // Check cache hit ratio
            var hitRatio = await _cache.GetHitRatioAsync();
            if (hitRatio < 0.95)
            {
                return HealthCheckResult.Degraded(
                    $"Cache hit ratio below threshold: {hitRatio:P}");
            }

            return HealthCheckResult.Healthy(
                $"Cache operational. Hit ratio: {hitRatio:P}");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Cache check failed", ex);
        }
    }
}

// File: AuthorizationHealthCheck.cs
public class AuthorizationHealthCheck : IHealthCheck
{
    private readonly IPermissionService _permissionService;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            
            // Perform test permission check
            await _permissionService.TestPermissionCheckAsync();
            
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds > 10)
            {
                return HealthCheckResult.Degraded(
                    $"Permission check slow: {stopwatch.ElapsedMilliseconds}ms");
            }

            return HealthCheckResult.Healthy(
                $"Authorization operational: {stopwatch.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Authorization check failed", ex);
        }
    }
}
```

### 2.3 Monitoring Dashboard Controller
**Location**: `src/services/UserService/Controllers/`

```csharp
// File: MonitoringController.cs
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SystemAdmin")]
public class MonitoringController : ControllerBase
{
    private readonly IMetricsCollector _metrics;
    private readonly IEnhancedAuditService _auditService;
    private readonly IRedisCache _cache;

    [HttpGet("metrics")]
    public async Task<IActionResult> GetMetrics([FromQuery] int hours = 24)
    {
        var summary = await _metrics.GetSummaryAsync(TimeSpan.FromHours(hours));
        return Ok(summary);
    }

    [HttpGet("permission-analytics")]
    public async Task<IActionResult> GetPermissionAnalytics(
        [FromQuery] DateTime? from = null,
        [FromQuery] DateTime? to = null)
    {
        var analytics = new
        {
            TotalChecks = await GetTotalPermissionChecks(from, to),
            GrantRate = await GetPermissionGrantRate(from, to),
            TopDeniedPermissions = await GetTopDeniedPermissions(from, to),
            AverageCheckTime = await GetAverageCheckTime(from, to),
            CacheHitRate = await _cache.GetHitRatioAsync()
        };

        return Ok(analytics);
    }

    [HttpGet("role-usage")]
    public async Task<IActionResult> GetRoleUsageAnalytics()
    {
        var usage = await _auditService.GetRoleUsageAnalyticsAsync();
        return Ok(usage);
    }

    [HttpGet("security-events")]
    public async Task<IActionResult> GetSecurityEvents(
        [FromQuery] string severity = null,
        [FromQuery] int days = 7)
    {
        var events = await _auditService.GetSecurityEventLogAsync(
            severity,
            DateTime.UtcNow.AddDays(-days),
            DateTime.UtcNow);

        return Ok(events);
    }
}
```

### 2.4 Startup Configuration
**Location**: `src/services/*/Program.cs`

```csharp
// Add to each service's Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<PermissionCacheHealthCheck>("permission_cache")
    .AddCheck<AuthorizationHealthCheck>("authorization")
    .AddCheck<DatabaseHealthCheck>("database")
    .AddRedis(redisConnectionString, name: "redis");

// Add Prometheus metrics
builder.Services.AddSingleton<IMetricsCollector, PrometheusMetricsCollector>();
app.UseHttpMetrics();
app.MapMetrics();

// Add security middleware
app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<SuspiciousActivityDetectionMiddleware>();

// Map health check endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false
});
```

---

## Session 3: Compliance Features & Testing (2-3 hours)

### 3.1 Compliance Reporting Service
**Location**: `src/shared/Common/Services/`

```csharp
// File: IComplianceReportingService.cs
public interface IComplianceReportingService
{
    Task<ComplianceReport> GenerateAccessReportAsync(DateTime from, DateTime to);
    Task<ComplianceReport> GeneratePermissionUsageReportAsync(DateTime from, DateTime to);
    Task<ComplianceReport> GenerateSecurityAuditReportAsync(DateTime from, DateTime to);
    Task<byte[]> ExportReportToPdfAsync(ComplianceReport report);
    Task<byte[]> ExportReportToCsvAsync(ComplianceReport report);
}

// File: ComplianceReportingService.cs
public class ComplianceReportingService : IComplianceReportingService
{
    private readonly IEnhancedAuditService _auditService;
    private readonly IDbContext _context;

    public async Task<ComplianceReport> GenerateAccessReportAsync(
        DateTime from, DateTime to)
    {
        var report = new ComplianceReport
        {
            Type = "Access Report",
            GeneratedAt = DateTime.UtcNow,
            Period = new DateRange(from, to)
        };

        // Gather access statistics
        report.Sections.Add(await GenerateUserAccessSection(from, to));
        report.Sections.Add(await GeneratePermissionCheckSection(from, to));
        report.Sections.Add(await GenerateFailedAccessAttemptsSection(from, to));

        return report;
    }
}
```

### 3.2 Alert Configuration
**Location**: `src/shared/Common/Alerts/`

```csharp
// File: SecurityAlertService.cs
public class SecurityAlertService : ISecurityAlertService
{
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;
    private readonly ISlackService _slackService;

    public async Task SendAlertAsync(SecurityAlert alert)
    {
        // Check alert severity and routing rules
        if (alert.Severity >= AlertSeverity.High)
        {
            await SendEmailAlertAsync(alert);
            await SendSlackAlertAsync(alert);
        }

        // Log to security event log
        await LogSecurityAlertAsync(alert);
    }

    private async Task SendEmailAlertAsync(SecurityAlert alert)
    {
        var recipients = _configuration.GetSection("Security:AlertRecipients")
            .Get<List<string>>();

        var emailContent = BuildEmailContent(alert);
        await _emailService.SendAsync(recipients, 
            $"[{alert.Severity}] Security Alert: {alert.Type}", 
            emailContent);
    }
}
```

### 3.3 Integration Tests
**Location**: `tests/integration/`

```csharp
// File: SecurityAuditingTests.cs
public class SecurityAuditingTests : IntegrationTestBase
{
    [Fact]
    public async Task PermissionCheck_ShouldCreateAuditLog()
    {
        // Arrange
        var client = CreateAuthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/users");

        // Assert
        response.Should().BeSuccessful();

        var auditLogs = await GetAuditLogsAsync();
        auditLogs.Should().Contain(log => 
            log.Action == "PermissionCheck" && 
            log.Permission == "users.view");
    }

    [Fact]
    public async Task UnauthorizedAccess_ShouldLogSecurityEvent()
    {
        // Arrange
        var client = CreateUnauthenticatedClient();

        // Act
        var response = await client.GetAsync("/api/admin/settings");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

        var securityEvents = await GetSecurityEventsAsync();
        securityEvents.Should().Contain(evt => 
            evt.EventType == "UnauthorizedAccess");
    }

    [Fact]
    public async Task RoleChange_ShouldCreateDetailedAuditLog()
    {
        // Arrange
        var client = CreateAdminClient();
        var roleUpdate = new UpdateRoleDto
        {
            Name = "Updated Role",
            Permissions = new[] { "users.view", "users.edit" }
        };

        // Act
        var response = await client.PutAsJsonAsync("/api/roles/1", roleUpdate);

        // Assert
        response.Should().BeSuccessful();

        var roleAuditLogs = await GetRoleChangeAuditLogsAsync();
        var log = roleAuditLogs.First();
        log.ChangeType.Should().Be("Updated");
        log.NewValue.Should().Contain("users.edit");
    }
}

// File: MonitoringEndpointsTests.cs
public class MonitoringEndpointsTests : IntegrationTestBase
{
    [Fact]
    public async Task HealthCheck_ShouldReturnHealthyStatus()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/health");

        // Assert
        response.Should().BeSuccessful();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    [Fact]
    public async Task Metrics_ShouldReturnPrometheusFormat()
    {
        // Arrange
        var client = CreateClient();

        // Act
        var response = await client.GetAsync("/metrics");

        // Assert
        response.Should().BeSuccessful();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("permission_checks_total");
        content.Should().Contain("authorization_duration_seconds");
    }
}
```

### 3.4 Configuration Files
**Location**: `src/services/*/appsettings.json`

```json
{
  "Security": {
    "EnableAuditLogging": true,
    "AuditLogRetentionDays": 90,
    "EnableSecurityHeaders": true,
    "EnableSuspiciousActivityDetection": true,
    "AlertRecipients": [
      "security@company.com",
      "admin@company.com"
    ],
    "SecurityEventSeverityThreshold": "Medium"
  },
  "Monitoring": {
    "EnableMetrics": true,
    "MetricsEndpoint": "/metrics",
    "EnableHealthChecks": true,
    "HealthCheckInterval": 30,
    "EnableDashboard": true,
    "DashboardUrl": "/monitoring"
  },
  "Compliance": {
    "EnableComplianceReporting": true,
    "ReportRetentionDays": 365,
    "AutoGenerateMonthlyReports": true,
    "ComplianceStandards": ["SOC2", "GDPR", "HIPAA"]
  }
}
```

---

## Testing Checklist

### Security Testing
- [ ] All permission checks generate audit logs
- [ ] Unauthorized access attempts are logged
- [ ] Role changes create detailed audit entries
- [ ] Security headers are present in all responses
- [ ] Suspicious activity detection triggers alerts

### Monitoring Testing
- [ ] Health checks respond correctly
- [ ] Metrics are collected and exposed
- [ ] Permission cache metrics are accurate
- [ ] Authorization performance is tracked
- [ ] Dashboard displays real-time data

### Compliance Testing
- [ ] Audit logs are retained per policy
- [ ] Reports can be generated on demand
- [ ] Security events are properly categorized
- [ ] Export formats work correctly
- [ ] Alert notifications are sent

---

## Phase 11 Completion Criteria

✅ **Phase is complete when:**
1. Permission check audit trail implemented
2. Role change tracking operational
3. Unauthorized access attempt logging working
4. Security headers middleware active
5. Health check endpoints responding
6. Metrics collection operational
7. Monitoring dashboard accessible
8. Compliance reporting functional
9. All security tests passing
10. Documentation updated

---

## Notes for Next Phase (Phase 12: Testing & QA)

With security and monitoring complete, Phase 12 will focus on:
- Comprehensive unit test coverage (80%+)
- Integration tests for all RBAC scenarios
- End-to-end tests for critical workflows
- Performance testing and benchmarking
- Security vulnerability scanning
- Load testing with multiple tenants

---

## Quick Reference Commands

```bash
# Run security tests
dotnet test --filter Category=Security

# Check health endpoints
curl https://localhost:5001/health
curl https://localhost:5001/health/ready

# View metrics
curl https://localhost:5001/metrics

# Generate compliance report
curl -X POST https://localhost:5001/api/compliance/reports/generate \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{"type":"AccessReport","from":"2024-01-01","to":"2024-12-31"}'

# Check audit logs
curl https://localhost:5001/api/monitoring/audit-logs?days=7 \
  -H "Authorization: Bearer {token}"
```