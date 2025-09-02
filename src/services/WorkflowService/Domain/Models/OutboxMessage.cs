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

    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public string? LastError { get; set; }

    // Idempotency – assign eagerly so we never persist Guid.Empty
    public Guid IdempotencyKey { get; set; } = Guid.NewGuid();

    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
}
