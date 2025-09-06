import { Chip, LinearProgress, Stack, Tooltip, Typography } from '@mui/material';
import { useInstanceStatus } from '../hooks/useInstanceStatus';
import { useInstanceHub } from '../hooks/useInstanceHub';
import type { InstanceUpdatedEvent } from '@/services/workflowNotifications';
import { InstanceStatus } from '@/types/workflow';
import { useEffect, useState } from 'react';

interface Props {
  instanceId: number;
  pollIntervalMs?: number;
  showProgress?: boolean;
  compact?: boolean;
  existingStatus?: InstanceStatus; // NEW
  disableIfTerminal?: boolean;     // NEW default true
}

const statusColor: Record<string, 'default' | 'success' | 'error' | 'warning' | 'info'> = {
  [InstanceStatus.Running]: 'info',
  [InstanceStatus.Completed]: 'success',
  [InstanceStatus.Cancelled]: 'warning',
  [InstanceStatus.Failed]: 'error',
  [InstanceStatus.Suspended]: 'warning'
};

const TERMINAL: InstanceStatus[] = [
  InstanceStatus.Completed,
  InstanceStatus.Cancelled,
  InstanceStatus.Failed
];

export function InstanceStatusBadge({
  instanceId,
  pollIntervalMs,
  showProgress = false,
  compact = false,
  existingStatus,
  disableIfTerminal = true
}: Props) {
  const skip = disableIfTerminal && existingStatus && TERMINAL.includes(existingStatus);

  const { data, loading, error } = useInstanceStatus(instanceId, {
    pollIntervalMs,
    pauseWhenTerminal: true,
    autoStart: !skip,
    existingStatus,
    skip
  });

  const [pushStatus, setPushStatus] = useState<string | undefined>(undefined);

  useInstanceHub({
    onInstanceUpdated: (e: InstanceUpdatedEvent) => {
      if (e.instanceId === instanceId) {
        setPushStatus(e.status);
      }
    },
    autoStart: !skip
  });

  const effectiveStatus = (pushStatus || data?.status || existingStatus || InstanceStatus.Running) as InstanceStatus;
  const color = statusColor[effectiveStatus] || 'default';

  // Graceful fallback: if error but existing status is terminal, show terminal
  if (error && existingStatus && TERMINAL.includes(existingStatus)) {
    return <Chip size="small" color={statusColor[existingStatus] || 'default'} label={existingStatus} />;
  }

  if (error) {
    return <Chip size="small" color="error" label="Status Error" />;
  }

  if (loading && !data && !skip) {
    return <Chip size="small" label="Loading..." />;
  }

  if (compact) {
    return (
      <Tooltip title={`Status: ${effectiveStatus}`}>
        <Chip size="small" color={color} label={effectiveStatus} />
      </Tooltip>
    );
  }

  const progress = showProgress
    ? (typeof data?.progressPercentage === 'number'
        ? Math.min(100, Math.max(0, data.progressPercentage))
        : undefined)
    : undefined;

  // If we have pushStatus and it's terminal, optionally stop polling:
  useEffect(() => {
    if (pushStatus && ['Completed', 'Cancelled', 'Failed'].includes(pushStatus)) {
      // Just rely on push; we could call stop() if returned by hook (not currently exposed)
    }
  }, [pushStatus]);

  return (
    <Stack spacing={0.5} sx={{ minWidth: 160 }}>
      <Chip size="small" color={color} label={effectiveStatus} />
      {progress !== undefined && !Number.isNaN(progress) && (
        <Stack spacing={0.25}>
          <LinearProgress
            variant="determinate"
            value={progress}
            sx={{ height: 6, borderRadius: 3 }}
          />
          <Typography variant="caption" color="text.secondary">
            {progress.toFixed(0)}% • Active tasks: {data?.activeTasksCount ?? 0}
          </Typography>
        </Stack>
      )}
    </Stack>
  );
}
