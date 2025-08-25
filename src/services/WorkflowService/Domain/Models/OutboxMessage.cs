using System.ComponentModel.DataAnnotations;
using DTOs.Entities;

namespace WorkflowService.Domain.Models;

public class OutboxMessage : BaseEntity
{
    [Required]
    [MaxLength(255)]
    public string Type { get; set; } = string.Empty;
    
    [Required]
    public string Payload { get; set; } = string.Empty; // JSON payload
    
    public bool Processed { get; set; } = false;
    
    public DateTime? ProcessedAt { get; set; }
    
    [MaxLength(1000)]
    public string? ErrorMessage { get; set; }
    
    public int RetryCount { get; set; } = 0;
    
    public DateTime? NextRetryAt { get; set; }
}
