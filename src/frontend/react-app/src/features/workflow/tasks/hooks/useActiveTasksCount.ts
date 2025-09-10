import { useCallback, useEffect, useRef, useState } from "react";
import { ActiveTasksCount, emptyActiveTasksCount } from "../types/ActiveTasksCount";
import { getActiveTasksCount } from "../api/getActiveTasksCount";
import { taskHub } from "../realtime/taskHub";

interface UseActiveTasksCountOptions {
  /**
   * Poll interval in ms if real-time hub not available or fails.
   * Default: 60s
   */
  pollIntervalMs?: number;
  /**
   * Disable SignalR subscription (fallback to polling only)
   */
  disableRealtime?: boolean;
}

export function useActiveTasksCount(options?: UseActiveTasksCountOptions) {
  const {
    pollIntervalMs = 60_000,
    disableRealtime = false
  } = options || {};

  const [data, setData] = useState<ActiveTasksCount>(emptyActiveTasksCount);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);
  const abortRef = useRef<AbortController | null>(null);
  const timerRef = useRef<number | null>(null);

  const fetchCounts = useCallback(async () => {
    abortRef.current?.abort();
    const ctrl = new AbortController();
    abortRef.current = ctrl;
    try {
      setError(null);
      const counts = await getActiveTasksCount(ctrl.signal);
      setData(counts);
    } catch (e: any) {
      if (e?.name !== "AbortError") {
        setError("Failed to load active tasks count");
      }
    } finally {
      setLoading(false);
    }
  }, []);

  // Initial & polling
  useEffect(() => {
    fetchCounts();

    if (pollIntervalMs > 0) {
      timerRef.current = window.setInterval(fetchCounts, pollIntervalMs);
    }

    return () => {
      abortRef.current?.abort();
      if (timerRef.current != null) window.clearInterval(timerRef.current);
    };
  }, [fetchCounts, pollIntervalMs]);

  // Realtime subscription
  useEffect(() => {
    if (disableRealtime) return;

    const realtimeHandler = (payload: ActiveTasksCount) => {
      setData(prev => {
        // Avoid re-render if identical
        const changed =
          prev.total !== payload.total ||
          prev.available !== payload.available ||
          prev.assignedToMe !== payload.assignedToMe ||
            prev.assignedToMyRoles !== payload.assignedToMyRoles ||
            prev.claimed !== payload.claimed ||
            prev.inProgress !== payload.inProgress ||
            prev.overdue !== payload.overdue ||
            prev.failed !== payload.failed;
        return changed ? payload : prev;
      });
    };

    taskHub.onActiveCounts(realtimeHandler);
    return () => {
      taskHub.offActiveCounts(realtimeHandler);
    };
  }, [disableRealtime]);

  const manualRefresh = useCallback(() => {
    setLoading(true);
    fetchCounts();
  }, [fetchCounts]);

  return {
    data,
    loading,
    error,
    refresh: manualRefresh
  };
}
