import { useCallback, useEffect, useState } from 'react';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowInstanceDto, WorkflowTaskDto } from '@/types/workflow';

interface UseWorkflowInstanceResult {
  instance: WorkflowInstanceDto | null;
  tasks: WorkflowTaskDto[];
  loading: boolean;
  error: string | null;
  refresh: () => Promise<void>;
  completing: boolean;
  completeTask: (taskId: number, completionData?: string | object) => Promise<void>;
}

export function useWorkflowInstance(instanceId: number): UseWorkflowInstanceResult {
  const [instance, setInstance] = useState<WorkflowInstanceDto | null>(null);
  const [tasks, setTasks] = useState<WorkflowTaskDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [completing, setCompleting] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const inst = await workflowService.getInstance(instanceId);
      // You may want a dedicated "tasks by instance" endpoint later; reuse filters now:
      const taskList = await workflowService.getTasks({ workflowInstanceId: instanceId, pageSize: 200 });
      setInstance(inst);
      setTasks(taskList);
    } catch (e: any) {
      setError(e.message || 'Failed to load instance');
    } finally {
      setLoading(false);
    }
  }, [instanceId]);

  useEffect(() => {
    load();
  }, [load]);

  const completeTask = useCallback(async (taskId: number, completionData: string | object = {}) => {
    setCompleting(true);
    setError(null);
    try {
      await workflowService.completeTask(taskId, {
        completionData: typeof completionData === 'string'
          ? completionData
          : JSON.stringify(completionData)
      });
      // Immediately refresh to pick up instance status transition to Completed
      await load();
    } catch (e: any) {
      setError(e.message || 'Task completion failed');
    } finally {
      setCompleting(false);
    }
  }, [load]);

  return {
    instance,
    tasks,
    loading,
    error,
    refresh: load,
    completing,
    completeTask
  };
}
