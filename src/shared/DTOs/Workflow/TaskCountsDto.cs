namespace DTOs.Workflow;

public class TaskCountsDto
{
    public int Available { get; set; }          // Created & unassigned (claimable)
    public int AssignedToMe { get; set; }       // Explicitly assigned to user
    public int AssignedToMyRoles { get; set; }  // Assigned to one of user roles (not yet claimed)
    public int Claimed { get; set; }
    public int InProgress { get; set; }
    public int CompletedToday { get; set; }
    public int Overdue { get; set; }
    public int Failed { get; set; }
    public int TotalActionable => Available + AssignedToMe + AssignedToMyRoles + Claimed + InProgress;
}
