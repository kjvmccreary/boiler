import { Chip, LinearProgress, Stack, Tooltip, Typography } from '@mui/material';
import { useInstanceStatus } from '../hooks/useInstanceStatus';
import { InstanceStatus } from '@/types/workflow';

interface Props {
  instanceId: number;
  pollIntervalMs?: number;
  showProgress?: boolean;
  compact?: boolean;
}

const statusColor: Record<string, 'default' | 'success' | 'error' | 'warning' | 'info'> = {
  [InstanceStatus.Running]: 'info',
  [InstanceStatus.Completed]: 'success',
  [InstanceStatus.Cancelled]: 'warning',
  [InstanceStatus.Failed]: 'error',
  [InstanceStatus.Suspended]: 'warning'
};

export function InstanceStatusBadge({ instanceId, pollIntervalMs, showProgress = false, compact = false }: Props) {
  const { data, loading, error } = useInstanceStatus(instanceId, {
    pollIntervalMs,
    pauseWhenTerminal: true,
    autoStart: true
  });

  if (error) {
    return <Chip size="small" color="error" label="Status Error" />;
  }

  if (loading && !data) {
    return <Chip size="small" label="Loading..." />;
  }

  if (!data) return null;

  const color = statusColor[data.status] || 'default';
  const progress = typeof data.progressPercentage === 'number' ? Math.min(100, Math.max(0, data.progressPercentage)) : undefined;

  if (compact) {
    return (
      <Tooltip title={`Status: ${data.status}${progress !== undefined ? ` (${progress.toFixed(0)}%)` : ''}`}>
        <Chip size="small" color={color} label={data.status} />
      </Tooltip>
    );
  }

  return (
    <Stack spacing={0.5} sx={{ minWidth: 160 }}>
      <Chip size="small" color={color} label={data.status} />
      {showProgress && progress !== undefined && !Number.isNaN(progress) && (
        <Stack spacing={0.25}>
          <LinearProgress
            variant="determinate"
            value={progress}
            sx={{ height: 6, borderRadius: 3 }}
          />
          <Typography variant="caption" color="text.secondary">
            {progress.toFixed(0)}% • Active tasks: {data.activeTasksCount}
          </Typography>
        </Stack>
      )}
    </Stack>
  );
}
