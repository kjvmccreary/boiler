import { useCallback, useEffect, useRef, useState } from 'react';
import { workflowService, type InstanceStatusDto } from '@/services/workflow.service';
import { InstanceStatus } from '@/types/workflow';

export interface UseInstanceStatusOptions {
  pollIntervalMs?: number;
  autoStart?: boolean;
  pauseWhenTerminal?: boolean;
  onStatusChange?: (prev: InstanceStatusDto | null, next: InstanceStatusDto) => void;
  existingStatus?: InstanceStatus; // NEW: fallback baseline
  skip?: boolean;                  // NEW: allow caller to disable polling entirely
}

export interface UseInstanceStatusResult {
  data: InstanceStatusDto | null;
  loading: boolean;
  error: unknown;
  refresh: () => Promise<void>;
  isTerminal: boolean;
  isRunning: boolean;
  stop: () => void;
  start: () => void;
}

const TERMINAL: InstanceStatus[] = [
  InstanceStatus.Completed,
  InstanceStatus.Cancelled,
  InstanceStatus.Failed
];

export function useInstanceStatus(
  instanceId: number | null | undefined,
  opts?: UseInstanceStatusOptions
): UseInstanceStatusResult {
  const {
    pollIntervalMs = 5000,
    autoStart = true,
    pauseWhenTerminal = true,
    onStatusChange,
    existingStatus,
    skip = false
  } = opts || {};

  const [data, setData] = useState<InstanceStatusDto | null>(null);
  const [loading, setLoading] = useState<boolean>(!!instanceId && !skip);
  const [error, setError] = useState<unknown>(null);
  const timerRef = useRef<number | null>(null);
  const runningRef = useRef<boolean>(autoStart && !skip);

  const isTerminalPre = existingStatus ? TERMINAL.includes(existingStatus) : false;
  const effectiveTerminal = data
    ? TERMINAL.includes(data.status as InstanceStatus)
    : isTerminalPre;

  const isTerminal = effectiveTerminal;
  const isRunning = !!data && data.status === InstanceStatus.Running;

  const clearTimer = () => {
    if (timerRef.current) {
      window.clearTimeout(timerRef.current);
      timerRef.current = null;
    }
  };

  const schedule = useCallback(() => {
    clearTimer();
    if (skip) return;
    if (!runningRef.current) return;
    if (!instanceId) return;
    if (pauseWhenTerminal && isTerminal) return;
    timerRef.current = window.setTimeout(() => {
      void refresh();
    }, pollIntervalMs);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [instanceId, pollIntervalMs, pauseWhenTerminal, isTerminal, skip]);

  const refresh = useCallback(async () => {
    if (skip) return;
    if (!instanceId) return;
    // If we already know it's terminal (from list row), skip polling entirely
    if (isTerminalPre && !data) {
      // Seed minimal pseudo data so UI can show a badge without spinning
      setData({
        instanceId,
        status: existingStatus || InstanceStatus.Completed,
        currentNodeIds: '[]',
        currentNodeNames: [],
        progressPercentage: 100,
        lastUpdated: new Date().toISOString(),
        runtime: '0',
        activeTasksCount: 0,
        errorMessage: undefined
      });
      setLoading(false);
      return;
    }
    try {
      if (!data) setLoading(true);
      const prev = data;
      const next = await workflowService.getInstanceStatus(instanceId);
      setData(next);
      setError(null);
      if (onStatusChange && JSON.stringify(prev?.status) !== JSON.stringify(next.status)) {
        onStatusChange(prev, next);
      }
    } catch (e: any) {
      // Graceful fallback: if 404 and we have existing terminal status, treat silently
      const status = e?.response?.status;
      if (status === 404 && existingStatus && TERMINAL.includes(existingStatus)) {
        setError(null);
        setData(prev => prev ?? {
          instanceId,
            status: existingStatus,
            currentNodeIds: '[]',
            currentNodeNames: [],
            progressPercentage: 100,
            lastUpdated: new Date().toISOString(),
            runtime: '0',
            activeTasksCount: 0,
            errorMessage: undefined
        });
      } else {
        setError(e);
      }
    } finally {
      setLoading(false);
      schedule();
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [instanceId, existingStatus, isTerminalPre, schedule, skip]);

  const stop = useCallback(() => {
    runningRef.current = false;
    clearTimer();
  }, []);

  const start = useCallback(() => {
    if (skip) return;
    if (runningRef.current) return;
    runningRef.current = true;
    schedule();
  }, [schedule, skip]);

  useEffect(() => {
    if (skip) {
      clearTimer();
      setLoading(false);
      return;
    }
    if (instanceId && autoStart) {
      void refresh();
    }
    return () => clearTimer();
  }, [instanceId, autoStart, refresh, skip]);

  return {
    data,
    loading,
    error,
    refresh,
    isTerminal,
    isRunning,
    stop,
    start
  };
}
