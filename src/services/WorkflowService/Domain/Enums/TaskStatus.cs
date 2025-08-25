namespace WorkflowService.Domain.Enums;

public enum TaskStatus
{
    Pending = 1,
    Claimed = 2,
    InProgress = 3,
    Completed = 4,
    Failed = 5,
    Cancelled = 6
}
