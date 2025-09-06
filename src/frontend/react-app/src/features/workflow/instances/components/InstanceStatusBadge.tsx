import { Chip, LinearProgress, Stack, Tooltip, Typography } from '@mui/material';
import { useInstanceStatus } from '../hooks/useInstanceStatus';
import { InstanceStatus } from '@/types/workflow';
import { useInstanceProgress } from '../hooks/useInstanceProgress';

interface Props {
  instanceId: number;
  pollIntervalMs?: number;
  showProgress?: boolean;
  compact?: boolean;
  existingStatus?: InstanceStatus;
  disableIfTerminal?: boolean;
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
  // Progress push (SignalR)
  const progress = useInstanceProgress(instanceId);
  const skip = disableIfTerminal && existingStatus && TERMINAL.includes(existingStatus);

  const { data, loading, error } = useInstanceStatus(instanceId, {
    pollIntervalMs,
    pauseWhenTerminal: true,
    autoStart: !skip,
    existingStatus,
    skip
  });

  const effectiveStatus = (progress.status ||
    data?.status ||
    existingStatus ||
    InstanceStatus.Running) as InstanceStatus;

  const color = statusColor[effectiveStatus] || 'default';

  // Prefer push progress if present
  const computedProgress = progress.hasProgress
    ? progress.percentage ?? undefined
    : (showProgress && typeof data?.progressPercentage === 'number'
        ? data.progressPercentage
        : undefined);

  if (error && existingStatus && TERMINAL.includes(existingStatus)) {
    return <Chip size="small" color={statusColor[existingStatus] || 'default'} label={existingStatus} />;
  }

  if (error) {
    return <Chip size="small" color="error" label="Status Error" />;
  }

  if (loading && !data && !progress.hasProgress && !skip) {
    return <Chip size="small" label="Loading..." />;
  }

  if (compact) {
    return (
      <Tooltip title={`Status: ${effectiveStatus}${computedProgress != null ? ` • ${computedProgress}%` : ''}`}>
        <Chip size="small" color={color} label={effectiveStatus} />
      </Tooltip>
    );
  }

  return (
    <Stack spacing={0.5} sx={{ minWidth: 160 }}>
      <Chip size="small" color={color} label={effectiveStatus} />
      {computedProgress != null && !Number.isNaN(computedProgress) && (
        <Stack spacing={0.25}>
          <LinearProgress
            variant="determinate"
            value={Math.min(100, Math.max(0, computedProgress))}
            sx={{ height: 6, borderRadius: 3 }}
          />
          <Typography variant="caption" color="text.secondary">
            {computedProgress.toFixed(0)}%
            {progress.hasProgress && progress.visitedCount != null && progress.totalNodes != null &&
              ` • ${progress.visitedCount}/${progress.totalNodes}`}
          </Typography>
        </Stack>
      )}
    </Stack>
  );
}
