using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatusAlias = DTOs.Workflow.Enums.TaskStatus;
using WorkflowService.Persistence;

namespace WorkflowService.Hubs;

/// <summary>
/// Lightweight helper for computing active task counters for push updates.
/// Kept intentionally separate from TaskService to avoid circular dependency in hub dispatchers.
/// </summary>
public static class TaskNotificationsHubExtensions
{
    public static async Task<ActiveTasksCountDto> ComputeActiveCountsAsync(
        this WorkflowDbContext db,
        int tenantId,
        int userId,
        IReadOnlyCollection<string> roles,
        CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var rows = await db.WorkflowTasks
            .AsNoTracking()
            .Where(t => t.TenantId == tenantId)
            .Select(t => new { t.Status, t.AssignedToUserId, t.AssignedToRole, t.DueDate })
            .ToListAsync(ct);

        int available = 0, assignedToMe = 0, assignedToMyRoles = 0,
            claimed = 0, inProgress = 0, overdue = 0, failed = 0;

        foreach (var t in rows)
        {
            bool isMine = t.AssignedToUserId == userId;
            bool roleMatch = !string.IsNullOrEmpty(t.AssignedToRole) && roleSet.Contains(t.AssignedToRole!);

            switch (t.Status)
            {
                case TaskStatusAlias.Created:
                    if (t.AssignedToUserId == null && string.IsNullOrEmpty(t.AssignedToRole))
                        available++;
                    else if (roleMatch && !isMine)
                        assignedToMyRoles++;
                    break;
                case TaskStatusAlias.Assigned:
                    if (isMine) assignedToMe++;
                    else if (roleMatch) assignedToMyRoles++;
                    break;
                case TaskStatusAlias.Claimed:
                    if (isMine) claimed++;
                    break;
                case TaskStatusAlias.InProgress:
                    if (isMine) inProgress++;
                    break;
                case TaskStatusAlias.Failed:
                    if (isMine || roleMatch) failed++;
                    break;
            }

            if (t.DueDate.HasValue &&
                t.DueDate.Value < now &&
                t.Status is TaskStatusAlias.Created or TaskStatusAlias.Assigned or TaskStatusAlias.Claimed or TaskStatusAlias.InProgress)
            {
                if (isMine || roleMatch ||
                    (t.Status == TaskStatusAlias.Created && t.AssignedToUserId == null && string.IsNullOrEmpty(t.AssignedToRole)))
                    overdue++;
            }
        }

        return new ActiveTasksCountDto
        {
            Available = available,
            AssignedToMe = assignedToMe,
            AssignedToMyRoles = assignedToMyRoles,
            Claimed = claimed,
            InProgress = inProgress,
            Overdue = overdue,
            Failed = failed,
            Total = available + assignedToMe + assignedToMyRoles + claimed + inProgress
        };
    }
}
