using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;
using Npgsql;

namespace WorkflowService.Services;

public class OutboxWriter : IOutboxWriter
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly WorkflowDbContext _db;
    private readonly ILogger<OutboxWriter> _logger;

    public OutboxWriter(WorkflowDbContext db, ILogger<OutboxWriter> logger)
    {
        _db = db;
        _logger = logger;
    }

    public OutboxMessage Enqueue(int tenantId, string eventType, object payload, Guid? idempotencyKey = null)
    {
        var (json, key) = SerializeAndKey(payload, idempotencyKey);
        WarnIfMissingDeterministicKey(eventType, idempotencyKey);
        var msg = BuildMessage(tenantId, eventType, json, key);
        _db.OutboxMessages.Add(msg);
        _logger.LogDebug("OUTBOX_ENQUEUE EventType={EventType} Tenant={TenantId} Key={Key}", eventType, tenantId, key);
        return msg;
    }

    public async Task<OutboxEnqueueResult> TryAddAsync(
        int tenantId,
        string eventType,
        object payload,
        Guid? idempotencyKey = null,
        CancellationToken ct = default)
    {
        var (json, key) = SerializeAndKey(payload, idempotencyKey);
        WarnIfMissingDeterministicKey(eventType, idempotencyKey);

        var msg = BuildMessage(tenantId, eventType, json, key);

        _db.OutboxMessages.Add(msg);

        try
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("OUTBOX_IDEMPOTENT_ADD EventType={EventType} Tenant={TenantId} Key={Key} Id={Id}",
                eventType, tenantId, key, msg.Id);
            return new OutboxEnqueueResult(msg, false);
        }
        catch (DbUpdateException ex) when (IsUniqueViolation(ex))
        {
            Detach(msg);

            var existing = await _db.OutboxMessages
                .AsNoTracking()
                .Where(o => o.TenantId == tenantId && o.IdempotencyKey == key)
                .FirstOrDefaultAsync(ct);

            if (existing != null)
            {
                _logger.LogInformation("OUTBOX_IDEMPOTENT_DUPLICATE tenant={Tenant} key={Key} eventType={EventType} existingId={Id}",
                    tenantId, key, eventType, existing.Id);
                return new OutboxEnqueueResult(existing, true);
            }

            _logger.LogWarning(ex,
                "OUTBOX_IDEMPOTENT_CONFLICT_NO_ROW tenant={Tenant} key={Key} eventType={EventType}",
                tenantId, key, eventType);
            throw;
        }
    }

    private static (string Json, Guid Key) SerializeAndKey(object payload, Guid? key)
    {
        var finalKey = key == null || key == Guid.Empty ? Guid.NewGuid() : key.Value;
        var json = payload is string s ? s : JsonSerializer.Serialize(payload, SerializerOptions);
        return (json, finalKey);
    }

    private static OutboxMessage BuildMessage(int tenantId, string eventType, string json, Guid key) =>
        new()
        {
            TenantId = tenantId,
            EventType = eventType,
            EventData = json,
            IdempotencyKey = key,
            IsProcessed = false,
            RetryCount = 0,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

    private void WarnIfMissingDeterministicKey(string eventType, Guid? providedKey)
    {
        if (providedKey.HasValue && providedKey.Value != Guid.Empty) return;

        // Heuristic: workflow.* events should have deterministic keys from DeterministicOutboxKey
        if (eventType.StartsWith("workflow.", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogWarning("OUTBOX_IDEMPOTENCY_MISSING eventType={EventType} - producer did not supply a deterministic key", eventType);
        }
    }

    private bool IsUniqueViolation(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pg && pg.SqlState == PostgresErrorCodes.UniqueViolation)
            return true;

        var inner = ex.InnerException;
        if (inner != null && inner.GetType().FullName == "Microsoft.Data.Sqlite.SqliteException")
        {
            var codeProp = inner.GetType().GetProperty("SqliteErrorCode");
            if (codeProp != null && inner.Message.IndexOf("UNIQUE", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var val = (int)codeProp.GetValue(inner)!;
                if (val == 19) return true;
            }
        }
        return false;
    }

    private void Detach(OutboxMessage msg)
    {
        var entry = _db.Entry(msg);
        if (entry != null)
            entry.State = EntityState.Detached;
    }
}
