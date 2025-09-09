import { useEffect, useState, useMemo } from 'react';
import {
  startInstanceHub,
  onInstanceProgress,
  type InstanceProgressEvent
} from '@/services/workflowNotifications';

/**
 * Live progress event (raw from server) + client-side deduped metrics.
 * Server provides (percentage, visitedCount, totalNodes) where percentage is naive (visitedCount / totalNodes).
 * We compute a deduped executable progress excluding 'start' nodes if definition node list is provided.
 */
export interface UseInstanceProgressResult {
  percentage: number | null;                // raw percentage from server (legacy)
  visitedCount: number | null;              // raw visited count (may include revisits)
  totalNodes: number | null;                // total nodes (raw, includes start)
  status?: string;
  activeNodeIds: string[];
  hasProgress: boolean;
  // Deduped metrics (optional – require definition node ids)
  dedupedPercentage: number | null;
  dedupVisitedExecutable: number | null;
  executableNodeTotal: number | null;
  varianceDelta: number | null;             // raw - deduped (absolute) when both present
}

interface ComputeArgs {
  rawVisited: string[] | undefined;
  rawVisitedCount: number | null;   // may be 0
  rawTotal: number | null;          // may be 0
  definitionNodeIds?: { id: string; type: string }[] | null;
}

function computeDedup(args: ComputeArgs) {
  const { rawVisited, rawVisitedCount, rawTotal, definitionNodeIds } = args;
  // Allow zero values; only bail if null/undefined
  if (
    !rawVisited ||
    rawVisitedCount == null ||
    rawTotal == null ||
    !definitionNodeIds ||
    definitionNodeIds.length === 0
  ) {
    return {
      dedupedPercentage: null,
      dedupVisitedExecutable: null,
      executableNodeTotal: null,
      varianceDelta: null
    };
  }
  // Exclude 'start'
  const execIds = new Set(definitionNodeIds.filter(n => n.type !== 'start').map(n => n.id));
  if (execIds.size === 0) {
    return {
      dedupedPercentage: null,
      dedupVisitedExecutable: null,
      executableNodeTotal: 0,
      varianceDelta: null
    };
  }
  const dedupVisitedExec = new Set<string>();
  rawVisited.forEach(v => { if (execIds.has(v)) dedupVisitedExec.add(v); });
  const dedupVisitedExecutable = dedupVisitedExec.size;
  const dedupedPercentage = (dedupVisitedExecutable / execIds.size) * 100;
  // Raw percent may be (rawVisitedCount / rawTotal) *100; we use existing progress.percentage if present outside.
  const rawPercent = rawTotal > 0 ? (rawVisitedCount / rawTotal) * 100 : dedupedPercentage;
  const varianceDelta = Math.abs(rawPercent - dedupedPercentage);
  return {
    dedupedPercentage,
    dedupVisitedExecutable,
    executableNodeTotal: execIds.size,
    varianceDelta
  };
}

export function useInstanceProgress(
  instanceId: number | null | undefined,
  definitionNodes?: { id: string; type: string }[] | null
): UseInstanceProgressResult {
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

  const dedup = useMemo(
    () =>
      computeDedup({
        // visitedNodeIds is OPTIONAL in current server event contract; cast to any for forward compatibility
        rawVisited: (progress as any)?.visitedNodeIds,
        rawVisitedCount: progress?.visitedCount ?? null,
        rawTotal: progress?.totalNodes ?? null,
        definitionNodeIds: definitionNodes
      }),
    [(progress as any)?.visitedNodeIds, progress?.visitedCount, progress?.totalNodes, definitionNodes]
  );

  return {
    percentage: progress?.percentage ?? null,
    visitedCount: progress?.visitedCount ?? null,
    totalNodes: progress?.totalNodes ?? null,
    status: progress?.status,
    activeNodeIds: progress?.activeNodeIds ?? [],
    hasProgress: progress != null,
    dedupedPercentage: dedup.dedupedPercentage,
    dedupVisitedExecutable: dedup.dedupVisitedExecutable,
    executableNodeTotal: dedup.executableNodeTotal,
    varianceDelta: dedup.varianceDelta
  };
}

export default useInstanceProgress;
