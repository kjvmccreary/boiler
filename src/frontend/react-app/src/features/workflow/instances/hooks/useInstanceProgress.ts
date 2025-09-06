import { useEffect, useState } from 'react';
import {
  startInstanceHub,
  onInstanceProgress,
  type InstanceProgressEvent
} from '@/services/workflowNotifications';

export interface UseInstanceProgressResult {
  percentage: number | null;
  visitedCount: number | null;
  totalNodes: number | null;
  status?: string;
  activeNodeIds: string[];
  hasProgress: boolean;
}

export function useInstanceProgress(instanceId: number | null | undefined): UseInstanceProgressResult {
  const [progress, setProgress] = useState<InstanceProgressEvent | null>(null);

  useEffect(() => {
    if (!instanceId) return;
    let unsub: (() => void) | undefined;
    let cancelled = false;

    (async () => {
      await startInstanceHub();
      if (cancelled) return;
      unsub = onInstanceProgress(evt => {
        if (evt.instanceId === instanceId) {
          setProgress(evt);
        }
      });
    })();

    return () => {
      cancelled = true;
      unsub?.();
    };
  }, [instanceId]);

  return {
    percentage: progress?.percentage ?? null,
    visitedCount: progress?.visitedCount ?? null,
    totalNodes: progress?.totalNodes ?? null,
    status: progress?.status,
    activeNodeIds: progress?.activeNodeIds ?? [],
    hasProgress: progress != null
  };
}
