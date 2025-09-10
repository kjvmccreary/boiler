using DTOs.Common;
using DTOs.Workflow;
using DTOs.Workflow.Enums;
using Microsoft.EntityFrameworkCore;
using TaskStatusAlias = DTOs.Workflow.Enums.TaskStatus;

namespace WorkflowService.Services;

public partial class TaskService
{
    public async Task<ApiResponseDto<ActiveTasksCountDto>> GetActiveTasksCountAsync(CancellationToken ct = default)
    {
        try
        {
            var tenantId = await _tenantProvider.GetCurrentTenantIdAsync();
            if (!tenantId.HasValue)
                return ApiResponseDto<ActiveTasksCountDto>.ErrorResult("Tenant context required");

            var userId = _userContext.UserId;
            if (!userId.HasValue)
                return ApiResponseDto<ActiveTasksCountDto>.ErrorResult("User context required");

            var roles = _userContext.Roles?.ToArray() ?? Array.Empty<string>();
            var roleSet = roles.ToHashSet(StringComparer.OrdinalIgnoreCase);
            var now = DateTime.UtcNow;

            // IMPORTANT: WorkflowTask does not reliably expose TenantId directly in this project build;
            // filter via the WorkflowInstance navigation (pattern used elsewhere in TaskService).
            var rows = await _context.WorkflowTasks
                .AsNoTracking()
                .Where(t => t.WorkflowInstance.TenantId == tenantId.Value)
                .Select(t => new
                {
                    t.Status,
                    t.AssignedToUserId,
                    t.AssignedToRole,
                    t.DueDate
                })
                .ToListAsync(ct);

            int available = 0,
                assignedToMe = 0,
                assignedToMyRoles = 0,
                claimed = 0,
                inProgress = 0,
                overdue = 0,
                failed = 0;

            foreach (var t in rows)
            {
                bool isMine = t.AssignedToUserId == userId.Value;
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
                        (t.Status == TaskStatusAlias.Created &&
                         t.AssignedToUserId == null &&
                         string.IsNullOrEmpty(t.AssignedToRole)))
                    {
                        overdue++;
                    }
                }
            }

            var dto = new ActiveTasksCountDto
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

            return ApiResponseDto<ActiveTasksCountDto>.SuccessResult(dto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WF_ACTIVE_TASKS_COUNT_ERROR");
            return ApiResponseDto<ActiveTasksCountDto>.ErrorResult("Failed to compute active tasks count");
        }
    }
}
