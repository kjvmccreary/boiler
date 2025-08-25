namespace DTOs.Workflow;

public class WorkflowEventDto
{
    public int Id { get; set; }
    public int WorkflowInstanceId { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Data { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public int? UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
