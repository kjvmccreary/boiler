import { useEffect, useRef } from 'react';
import { onTasksChanged, startTaskHub } from '@/services/taskNotifications';

/**
 * Simple hook to subscribe to TaskNotificationsHub "TasksChanged" events.
 * The hub already exists (TasksChanged event). This hook ensures connection start
 * and invokes the provided callback (with basic throttling to avoid burst reloads).
 */
export function useTaskHub(onChanged: () => void, throttleMs = 750) {
  const lastRunRef = useRef<number>(0);

  useEffect(() => {
    let unsubscribe: (() => void) | null = null;
    let cancelled = false;

    (async () => {
      await startTaskHub();
      if (cancelled) return;
      unsubscribe = onTasksChanged(() => {
        const now = Date.now();
        if (now - lastRunRef.current >= throttleMs) {
            lastRunRef.current = now;
            onChanged();
        }
      });
    })();

    return () => {
      cancelled = true;
      if (unsubscribe) unsubscribe();
    };
  }, [onChanged, throttleMs]);
}
