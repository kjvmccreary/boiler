using System.Text.Json;
using WorkflowService.Domain.Models;
using WorkflowService.Persistence;
using WorkflowService.Services.Interfaces;

namespace WorkflowService.Services;

/// <inheritdoc />
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
        var key = idempotencyKey == null || idempotencyKey == Guid.Empty
            ? Guid.NewGuid()
            : idempotencyKey.Value;

        var json = payload is string s
            ? s
            : JsonSerializer.Serialize(payload, SerializerOptions);

        var msg = new OutboxMessage
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

        _db.OutboxMessages.Add(msg);
        _logger.LogDebug("OUTBOX_ENQUEUE EventType={EventType} Tenant={TenantId} Key={Key}", eventType, tenantId, key);

        return msg;
    }
}
