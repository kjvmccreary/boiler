import { useCallback, useEffect, useRef, useState } from 'react';
import { workflowService, type InstanceStatusDto } from '@/services/workflow.service';
import { InstanceStatus } from '@/types/workflow';

export interface UseInstanceStatusOptions {
  pollIntervalMs?: number;
  autoStart?: boolean;
  pauseWhenTerminal?: boolean;
  onStatusChange?: (prev: InstanceStatusDto | null, next: InstanceStatusDto) => void;
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

export function useInstanceStatus(instanceId: number | null | undefined, opts?: UseInstanceStatusOptions): UseInstanceStatusResult {
  const {
    pollIntervalMs = 5000,
    autoStart = true,
    pauseWhenTerminal = true,
    onStatusChange
  } = opts || {};

  const [data, setData] = useState<InstanceStatusDto | null>(null);
  const [loading, setLoading] = useState<boolean>(!!instanceId);
  const [error, setError] = useState<unknown>(null);
  const timerRef = useRef<number | null>(null);
  const runningRef = useRef<boolean>(autoStart);

  const isTerminal = !!data && TERMINAL.includes(data.status as InstanceStatus);
  const isRunning = !!data && data.status === InstanceStatus.Running;

  const clearTimer = () => {
    if (timerRef.current) {
      window.clearTimeout(timerRef.current);
      timerRef.current = null;
    }
  };

  const schedule = useCallback(() => {
    clearTimer();
    if (!runningRef.current) return;
    if (!instanceId) return;
    if (pauseWhenTerminal && isTerminal) return;
    timerRef.current = window.setTimeout(() => {
      void refresh();
    }, pollIntervalMs);
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [instanceId, pollIntervalMs, pauseWhenTerminal, isTerminal]);

  const refresh = useCallback(async () => {
    if (!instanceId) return;
    try {
      if (!data) setLoading(true);
      const prev = data;
      const next = await workflowService.getInstanceStatus(instanceId);
      setData(next);
      setError(null);
      if (onStatusChange && JSON.stringify(prev?.status) !== JSON.stringify(next.status)) {
        onStatusChange(prev, next);
      }
    } catch (e) {
      setError(e);
    } finally {
      setLoading(false);
      schedule();
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [instanceId, onStatusChange, schedule]);

  const stop = useCallback(() => {
    runningRef.current = false;
    clearTimer();
  }, []);

  const start = useCallback(() => {
    if (runningRef.current) return;
    runningRef.current = true;
    schedule();
  }, [schedule]);

  useEffect(() => {
    if (instanceId && autoStart) {
      void refresh();
    }
    return () => clearTimer();
  }, [instanceId, autoStart, refresh]);

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
