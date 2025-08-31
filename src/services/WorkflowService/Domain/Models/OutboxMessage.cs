using System.ComponentModel.DataAnnotations;
using DTOs.Entities;

namespace WorkflowService.Domain.Models;

public class OutboxMessage : BaseEntity
{
    // Canonical event identification
    [Required]
    [MaxLength(255)]
    public string EventType { get; set; } = string.Empty;

    // JSON payload (stored as jsonb in DB)
    [Required]
    public string EventData { get; set; } = "{}";

    public int TenantId { get; set; }

    // Processing state
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
    public int RetryCount { get; set; }
    public DateTime? NextRetryAt { get; set; }
    public string? LastError { get; set; }

    // Legacy diagnostic fields removed: Type, Payload, Processed, ErrorMessage
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; } // (Optional) keep if still referenced elsewhere; safe to remove later
}
