using DTOs.Entities;
using Common.Data;
using Contracts.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Security.Claims;
using System.Diagnostics;

namespace Common.Services;

public interface IEnhancedAuditService : IAuditService
{
    Task LogPermissionCheckAsync(PermissionAuditEntry entry);
    Task LogRoleChangeAsync(RoleChangeAuditEntry entry);
    Task LogSecurityEventAsync(SecurityEventAuditEntry entry);
    Task<List<PermissionAuditEntry>> GetPermissionAuditLogAsync(
        int? userId = null, 
        DateTime? from = null, 
        DateTime? to = null,
        int pageSize = 100);
    Task<List<RoleChangeAuditEntry>> GetRoleChangeAuditLogAsync(
        int? roleId = null, 
        DateTime? from = null, 
        DateTime? to = null,
        int pageSize = 100);
    Task<List<SecurityEventAuditEntry>> GetSecurityEventLogAsync(
        string? severity = null, 
        DateTime? from = null, 
        DateTime? to = null,
        int pageSize = 100);
    Task<Dictionary<string, object>> GetSecurityMetricsAsync(TimeSpan period);
}

public class EnhancedAuditService : AuditService, IEnhancedAuditService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<EnhancedAuditService> _logger;
    private readonly ITenantProvider _tenantProvider;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public EnhancedAuditService(
        ApplicationDbContext context,
        ITenantProvider tenantProvider,
        IHttpContextAccessor httpContextAccessor,
        ILogger<EnhancedAuditService> logger) 
        : base(context, tenantProvider, httpContextAccessor, logger)
    {
        _context = context;
        _logger = logger;
        _tenantProvider = tenantProvider;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogPermissionCheckAsync(PermissionAuditEntry entry)
    {
        try
        {
            // Ensure tenant context is set
            if (entry.TenantId == 0)
            {
                entry.TenantId = await _tenantProvider.GetCurrentTenantIdAsync() ?? 0;
            }

            // Set IP and User Agent if not provided
            if (string.IsNullOrEmpty(entry.IpAddress))
            {
                entry.IpAddress = GetClientIpAddress();
            }
            
            if (string.IsNullOrEmpty(entry.UserAgent))
            {
                entry.UserAgent = GetUserAgent();
            }

            _context.Set<PermissionAuditEntry>().Add(entry);
            await _context.SaveChangesAsync();

            // Log critical permission denials immediately
            if (!entry.Granted)
            {
                _logger.LogWarning("Permission denied: User {UserId} attempted {Permission} on {Resource}. Reason: {Reason}",
                    entry.UserId, entry.Permission, entry.Resource, entry.DenialReason);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log permission check audit entry");
        }
    }

    public async Task LogRoleChangeAsync(RoleChangeAuditEntry entry)
    {
        try
        {
            // Ensure tenant context is set
            if (entry.TenantId == 0)
            {
                entry.TenantId = await _tenantProvider.GetCurrentTenantIdAsync() ?? 0;
            }

            // Set additional context
            if (string.IsNullOrEmpty(entry.IpAddress))
            {
                entry.IpAddress = GetClientIpAddress();
            }
            
            if (string.IsNullOrEmpty(entry.UserAgent))
            {
                entry.UserAgent = GetUserAgent();
            }

            _context.Set<RoleChangeAuditEntry>().Add(entry);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Role {ChangeType}: {RoleName} by user {UserId}", 
                entry.ChangeType, entry.RoleName, entry.ChangedByUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log role change audit entry");
        }
    }

    public async Task LogSecurityEventAsync(SecurityEventAuditEntry entry)
    {
        try
        {
            // Ensure tenant context is set
            if (entry.TenantId == 0)
            {
                entry.TenantId = await _tenantProvider.GetCurrentTenantIdAsync() ?? 0;
            }

            // Set additional context
            if (string.IsNullOrEmpty(entry.IpAddress))
            {
                entry.IpAddress = GetClientIpAddress();
            }
            
            if (string.IsNullOrEmpty(entry.UserAgent))
            {
                entry.UserAgent = GetUserAgent();
            }

            _context.Set<SecurityEventAuditEntry>().Add(entry);
            await _context.SaveChangesAsync();

            // Log high severity events immediately to application logs
            if (entry.Severity == "High" || entry.Severity == "Critical")
            {
                _logger.LogWarning("Security Event [{Severity}]: {EventType} from {IpAddress}. Details: {Details}",
                    entry.Severity, entry.EventType, entry.IpAddress, entry.Details);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log security event audit entry");
        }
    }

    public async Task<List<PermissionAuditEntry>> GetPermissionAuditLogAsync(
        int? userId = null, 
        DateTime? from = null, 
        DateTime? to = null,
        int pageSize = 100)
    {
        var query = _context.Set<PermissionAuditEntry>().AsQueryable();

        if (userId.HasValue)
            query = query.Where(x => x.UserId == userId.Value);

        if (from.HasValue)
            query = query.Where(x => x.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.Timestamp <= to.Value);

        return await query
            .OrderByDescending(x => x.Timestamp)
            .Take(pageSize)
            .Include(x => x.User)
            .Include(x => x.Tenant)
            .ToListAsync();
    }

    public async Task<List<RoleChangeAuditEntry>> GetRoleChangeAuditLogAsync(
        int? roleId = null, 
        DateTime? from = null, 
        DateTime? to = null,
        int pageSize = 100)
    {
        var query = _context.Set<RoleChangeAuditEntry>().AsQueryable();

        if (roleId.HasValue)
            query = query.Where(x => x.RoleId == roleId.Value);

        if (from.HasValue)
            query = query.Where(x => x.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.Timestamp <= to.Value);

        return await query
            .OrderByDescending(x => x.Timestamp)
            .Take(pageSize)
            .Include(x => x.Role)
            .Include(x => x.ChangedByUser)
            .Include(x => x.Tenant)
            .ToListAsync();
    }

    public async Task<List<SecurityEventAuditEntry>> GetSecurityEventLogAsync(
        string? severity = null, 
        DateTime? from = null, 
        DateTime? to = null,
        int pageSize = 100)
    {
        var query = _context.Set<SecurityEventAuditEntry>().AsQueryable();

        if (!string.IsNullOrEmpty(severity))
            query = query.Where(x => x.Severity == severity);

        if (from.HasValue)
            query = query.Where(x => x.Timestamp >= from.Value);

        if (to.HasValue)
            query = query.Where(x => x.Timestamp <= to.Value);

        return await query
            .OrderByDescending(x => x.Timestamp)
            .Take(pageSize)
            .Include(x => x.User)
            .Include(x => x.Tenant)
            .ToListAsync();
    }

    public async Task<Dictionary<string, object>> GetSecurityMetricsAsync(TimeSpan period)
    {
        var since = DateTime.UtcNow - period;
        
        var permissionChecks = await _context.Set<PermissionAuditEntry>()
            .Where(x => x.Timestamp >= since)
            .GroupBy(x => x.Granted)
            .Select(g => new { Granted = g.Key, Count = g.Count() })
            .ToListAsync();

        var securityEvents = await _context.Set<SecurityEventAuditEntry>()
            .Where(x => x.Timestamp >= since)
            .GroupBy(x => x.Severity)
            .Select(g => new { Severity = g.Key, Count = g.Count() })
            .ToListAsync();

        var avgPermissionCheckTime = await _context.Set<PermissionAuditEntry>()
            .Where(x => x.Timestamp >= since)
            .AverageAsync(x => x.CheckDurationMs);

        var cacheHitRate = await _context.Set<PermissionAuditEntry>()
            .Where(x => x.Timestamp >= since)
            .GroupBy(x => x.CacheHit)
            .Select(g => new { CacheHit = g.Key, Count = g.Count() })
            .ToListAsync();

        return new Dictionary<string, object>
        {
            ["period"] = period.ToString(),
            ["permissionChecks"] = permissionChecks,
            ["securityEvents"] = securityEvents,
            ["averagePermissionCheckMs"] = avgPermissionCheckTime,
            ["cacheHitRate"] = cacheHitRate,
            ["totalEvents"] = permissionChecks.Sum(x => x.Count) + securityEvents.Sum(x => x.Count)
        };
    }

    private string? GetClientIpAddress()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null) return null;

        // Check for forwarded IP first (behind proxy/load balancer)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Check for real IP header
        var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp))
        {
            return realIp;
        }

        // Fallback to connection IP
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private string? GetUserAgent()
    {
        return _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
    }
}
