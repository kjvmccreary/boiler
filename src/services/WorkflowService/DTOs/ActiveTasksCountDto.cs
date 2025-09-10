namespace DTOs.Workflow;

/// <summary>
/// Aggregate counts of actionable tasks for the current user (and their roles) within a tenant.
/// Mirrors semantics used in existing TaskCounts but focused on “active / actionable” slices.
/// </summary>
public sealed class ActiveTasksCountDto
{
    public int Total { get; set; }
    public int Available { get; set; }          // Created, no user, no role
    public int AssignedToMe { get; set; }       // Assigned directly (status=Assigned)
    public int AssignedToMyRoles { get; set; }  // Assigned/Created with a role match, not directly mine
    public int Claimed { get; set; }            // Claimed by me
    public int InProgress { get; set; }         // InProgress by me
    public int Overdue { get; set; }            // Overdue actionable (Created/Assigned/Claimed/InProgress and relevant)
    public int Failed { get; set; }             // Failed tasks tied to me or my roles (if surfaced)
}
