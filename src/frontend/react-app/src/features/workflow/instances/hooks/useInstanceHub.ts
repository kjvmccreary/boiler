import { useEffect } from 'react';
import {
  startInstanceHub,
  onInstanceUpdated,
  onInstancesChanged,
  type InstanceUpdatedEvent,
  type InstancesChangedEvent
} from '@/services/workflowNotifications';

interface Options {
  onInstanceUpdated?: (e: InstanceUpdatedEvent) => void;
  onInstancesChanged?: (e: InstancesChangedEvent) => void;
  autoStart?: boolean;
}

export function useInstanceHub(opts: Options) {
  const { onInstanceUpdated: iu, onInstancesChanged: ic, autoStart = true } = opts;

  useEffect(() => {
    let unsubIU: (() => void) | undefined;
    let unsubIC: (() => void) | undefined;
    let cancelled = false;

    (async () => {
      if (autoStart) await startInstanceHub();
      if (cancelled) return;

      if (iu) unsubIU = onInstanceUpdated(iu);
      if (ic) unsubIC = onInstancesChanged(ic);
    })();

    return () => {
      cancelled = true;
      unsubIU?.();
      unsubIC?.();
    };
  }, [iu, ic, autoStart]);
}
