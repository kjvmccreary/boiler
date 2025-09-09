import { useCallback, useMemo, useState } from 'react';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowTaskDto } from '@/types/workflow';

interface Permissions {
  has: (perm: string) => boolean;
}

function useOptionalPermissions(): Permissions {
  try {
    // eslint-disable-next-line @typescript-eslint/no-var-requires
    const mod = require('@/context/PermissionContext');
    if (mod?.usePermissions) {
      const p = mod.usePermissions();
      return { has: (perm: string) => p.has(perm) };
    }
  } catch {
    /* ignore */
  }
  return { has: () => true };
}

export function useTaskActions(initialTask: WorkflowTaskDto, onTaskUpdate: (t: WorkflowTaskDto) => void) {
  const [task, setTask] = useState(initialTask);
  const [loading, setLoading] = useState(false);
  const perms = useOptionalPermissions();

  const updateTask = (t: WorkflowTaskDto) => {
    setTask(t);
    onTaskUpdate(t);
  };

  const statusStr = task.status as string;

  const canAssign = useMemo(
    () => perms.has('workflow.write') && ['Created', 'Assigned', 'Claimed'].includes(statusStr),
    [statusStr, perms]
  );
  const canClaim = useMemo(
    () => perms.has('workflow.claim_tasks') && ['Created', 'Assigned'].includes(statusStr),
    [statusStr, perms]
  );
  const canComplete = useMemo(
    () => perms.has('workflow.complete_tasks') && ['Claimed', 'InProgress'].includes(statusStr),
    [statusStr, perms]
  );
  const canCancel = useMemo(
    () => perms.has('workflow.write') && ['Created', 'Assigned', 'Claimed', 'InProgress'].includes(statusStr),
    [statusStr, perms]
  );
  const canReset = useMemo(
    () => perms.has('workflow.admin') && ['Completed', 'Cancelled', 'Failed'].includes(statusStr),
    [statusStr, perms]
  );

  const doAction = useCallback(async (fn: () => Promise<WorkflowTaskDto>) => {
    setLoading(true);
    try {
      const updated = await fn();
      updateTask(updated);
      return updated;
    } finally {
      setLoading(false);
    }
  }, []);

  return {
    task,
    loading,
    actions: { canAssign, canClaim, canComplete, canCancel, canReset },
    claim: (claimNotes?: string) =>
      canClaim
        ? doAction(() => workflowService.claimTask(task.id, { claimNotes }))
        : Promise.reject(new Error('Not allowed')),
    complete: (completionData?: any, completionNotes?: string) =>
      canComplete
        ? doAction(() => workflowService.completeTask(task.id, {
            completionData: completionData != null
              ? (typeof completionData === 'string'
                  ? completionData
                  : JSON.stringify(completionData))
              : '{}',
            completionNotes: completionNotes || undefined
          }))
        : Promise.reject(new Error('Not allowed')),
    assign: (userId?: number, role?: string) =>
      canAssign
        ? doAction(() => workflowService.assignTask(task.id, { userId, role }))
        : Promise.reject(new Error('Not allowed')),
    cancel: () =>
      canCancel
        ? doAction(() => workflowService.cancelTask(task.id))
        : Promise.reject(new Error('Not allowed')),
    reset: (reason?: string) =>
      canReset
        ? doAction(() => workflowService.resetTask(task.id, { reason }))
        : Promise.reject(new Error('Not allowed'))
  };
}
