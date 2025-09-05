using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Persistence;
using WorkflowService.Outbox;

namespace WorkflowService.Controllers;

public record OutboxAdminItemDto(
    long Id,
    int TenantId,
    string EventType,
    Guid IdempotencyKey,
    int RetryCount,
    DateTime CreatedAt,
    DateTime? ProcessedAt,
    DateTime? NextRetryAt,
    bool DeadLetter,
    string? Error);

public record OutboxAdminPageDto(
    int Page,
    int PageSize,
    int TotalPages,
    long TotalCount,
    string Status,
    string? EventTypeFilter,
    IReadOnlyList<OutboxAdminItemDto> Items);

[ApiController]
[Route("api/workflow/outbox")]
public class OutboxAdminController : ControllerBase
{
    private static readonly HashSet<string> AllowedStatuses = new(StringComparer.OrdinalIgnoreCase)
        { "pending", "failed", "processed", "deadletter", "all" };

    private readonly WorkflowDbContext _db;
    private readonly OutboxOptions _options;

    public OutboxAdminController(
        WorkflowDbContext db,
        Microsoft.Extensions.Options.IOptions<OutboxOptions> opts)
    {
        _db = db;
        _options = opts.Value;
    }

    /// <summary>
    /// Admin listing for Outbox messages.
    /// New filters: tenantId, minCreatedUtc, maxCreatedUtc, minRetry, maxRetry, hasError, staleSeconds, idempotencyKey.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<OutboxAdminPageDto>> GetAsync(
        [FromQuery] string status = "pending",
        [FromQuery] string? eventType = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] string? sort = null,
        // NEW FILTERS
        [FromQuery] int? tenantId = null,
        [FromQuery] DateTime? minCreatedUtc = null,
        [FromQuery] DateTime? maxCreatedUtc = null,
        [FromQuery] int? minRetry = null,
        [FromQuery] int? maxRetry = null,
        [FromQuery] bool? hasError = null,
        [FromQuery] int? staleSeconds = null,
        [FromQuery] Guid? idempotencyKey = null,
        CancellationToken ct = default)
    {
        if (!AllowedStatuses.Contains(status))
            return BadRequest($"Invalid status. Allowed: {string.Join(", ", AllowedStatuses)}");

        if (minCreatedUtc.HasValue && maxCreatedUtc.HasValue && minCreatedUtc > maxCreatedUtc)
            return BadRequest("minCreatedUtc cannot be greater than maxCreatedUtc.");

        if (minRetry.HasValue && maxRetry.HasValue && minRetry > maxRetry)
            return BadRequest("minRetry cannot be greater than maxRetry.");

        if (staleSeconds.HasValue && staleSeconds <= 0)
            return BadRequest("staleSeconds must be > 0.");

        pageSize = Math.Clamp(pageSize, 1, 200);
        page = Math.Max(1, page);

        IQueryable<Domain.Models.OutboxMessage> query = _db.OutboxMessages.AsNoTracking();

        // Status filter
        query = status.ToLowerInvariant() switch
        {
            "pending"    => query.Where(m => m.ProcessedAt == null && !m.DeadLetter && m.RetryCount == 0),
            "failed"     => query.Where(m => m.ProcessedAt == null && !m.DeadLetter && m.RetryCount > 0),
            "processed"  => query.Where(m => m.ProcessedAt != null && !m.DeadLetter),
            "deadletter" => query.Where(m => m.DeadLetter),
            "all"        => query,
            _            => query
        };

        // Event type exact or prefix (* suffix)
        if (!string.IsNullOrWhiteSpace(eventType))
        {
            var normalized = eventType.Trim();
            if (normalized.EndsWith("*", StringComparison.Ordinal))
            {
                var prefix = normalized.Substring(0, normalized.Length - 1);
                query = query.Where(m => m.EventType.StartsWith(prefix));
            }
            else
            {
                query = query.Where(m => m.EventType == normalized);
            }
        }

        // Additional filters
        if (tenantId.HasValue)
            query = query.Where(m => m.TenantId == tenantId.Value);

        if (minCreatedUtc.HasValue)
            query = query.Where(m => m.CreatedAt >= minCreatedUtc.Value);

        if (maxCreatedUtc.HasValue)
            query = query.Where(m => m.CreatedAt <= maxCreatedUtc.Value);

        if (minRetry.HasValue)
            query = query.Where(m => m.RetryCount >= minRetry.Value);

        if (maxRetry.HasValue)
            query = query.Where(m => m.RetryCount <= maxRetry.Value);

        if (hasError.HasValue)
        {
            if (hasError.Value)
                query = query.Where(m => m.Error != null);
            else
                query = query.Where(m => m.Error == null);
        }

        if (idempotencyKey.HasValue)
            query = query.Where(m => m.IdempotencyKey == idempotencyKey.Value);

        if (staleSeconds.HasValue)
        {
            var cutoff = DateTime.UtcNow.AddSeconds(-staleSeconds.Value);
            // Only meaningful for unprocessed / active backlog
            query = query.Where(m => m.ProcessedAt == null && m.CreatedAt <= cutoff);
        }

        // Sorting
        query = sort?.Equals("oldest", StringComparison.OrdinalIgnoreCase) == true
            ? query.OrderBy(m => m.CreatedAt)
            : query.OrderByDescending(m => m.CreatedAt);

        var totalCount = await query.LongCountAsync(ct);
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        if (totalPages == 0) totalPages = 1;
        if (page > totalPages) page = totalPages;

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(m => new OutboxAdminItemDto(
                m.Id,
                m.TenantId,
                m.EventType,
                m.IdempotencyKey,
                m.RetryCount,
                m.CreatedAt,
                m.ProcessedAt,
                m.NextRetryAt,
                m.DeadLetter,
                m.Error == null
                    ? null
                    : (m.Error.Length <= 200 ? m.Error : m.Error.Substring(0, 200) + "...")))
            .ToListAsync(ct);

        return Ok(new OutboxAdminPageDto(
            page,
            pageSize,
            totalPages,
            totalCount,
            status.ToLowerInvariant(),
            eventType,
            items));
    }
}
