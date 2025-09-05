using System.ComponentModel.DataAnnotations;
using DTOs.Entities;

namespace WorkflowService.Domain.Models;

public class OutboxMessage : BaseEntity
{
    [Required, MaxLength(255)]
    public string EventType { get; set; } = string.Empty;

    [Required] // jsonb
    public string EventData { get; set; } = "{}";

    public int TenantId { get; set; }

    // Legacy completion flag (will be superseded by ProcessedAt presence)
    public bool IsProcessed { get; set; }

    // Target semantics: NULL until successfully dispatched
    public DateTime? ProcessedAt { get; set; }

    public int RetryCount { get; set; }

    // (Future) optional scheduling – kept for forward compatibility
    public DateTime? NextRetryAt { get; set; }

    // Normalized error field (renamed from LastError per migration)
    public string? Error { get; set; }

    // Idempotency – eagerly assigned so Guid.Empty never persisted
    public Guid IdempotencyKey { get; set; } = Guid.NewGuid();

    public bool DeadLetter { get; set; }   // NEW
}
