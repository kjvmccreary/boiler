import React, { useEffect } from 'react';
import {
  Card,
  CardContent,
  Typography,
  Box,
  Chip,
  LinearProgress,
  Alert,
  Tooltip,
  IconButton,
  Switch,
  FormControlLabel,
  Stack
} from '@mui/material';
import RefreshIcon from '@mui/icons-material/Refresh';
import PauseCircleIcon from '@mui/icons-material/PauseCircle';
import PlayCircleIcon from '@mui/icons-material/PlayCircle';
import type { WorkflowInstanceDto } from '@/types/workflow';
import { trackWorkflow } from '@/features/workflow/telemetry/workflowTelemetry';
import { shouldEmitVariance } from '../utils/progress';
import {
  recordProgressVariance,
  getProgressVarianceSummary
} from '../utils/progressVarianceStore';

export interface InstanceRuntimeSnapshotPanelProps {
  instance: WorkflowInstanceDto;
  visitedNodeIds: string[];
  currentNodeIds: string[];
  traversedEdgeIds: string[];
  totalNodes: number;
  progressPercent: number;
  rawProgressPercent?: number;              // NEW: raw (legacy) percent (all nodes / visited length)
  executableNodeTotal?: number;             // NEW: total executable nodes (excludes start)
  dedupVisitedExecutable?: number;          // NEW: dedup visited executable count
   tasksCount: number;
   eventsCount: number;
   onRefresh: () => void;
   autoRefresh: boolean;
   setAutoRefresh: (v: boolean) => void;
   lastUpdated?: Date | null;
}

export const InstanceRuntimeSnapshotPanel: React.FC<InstanceRuntimeSnapshotPanelProps> = ({
  instance,
  visitedNodeIds,
  currentNodeIds,
  traversedEdgeIds,
  totalNodes,
  progressPercent,
  rawProgressPercent,
  executableNodeTotal,
  dedupVisitedExecutable,
  tasksCount,
  eventsCount,
  onRefresh,
  autoRefresh,
  setAutoRefresh,
  lastUpdated
 }) => {
  const running = instance.status === 'Running';
  const suspended = instance.status === 'Suspended';
  const completed = instance.status === 'Completed';
  const failed = instance.status === 'Failed';
  const noActiveWhileRunning = running && currentNodeIds.length === 0;

  // Telemetry: emit variance if meaningful
  useEffect(() => {
    if (rawProgressPercent == null) return;
    if (shouldEmitVariance(rawProgressPercent, progressPercent)) {
      const delta = Math.abs(rawProgressPercent - progressPercent);
      recordProgressVariance(instance.id, rawProgressPercent, progressPercent);
      trackWorkflow('instance.progress.variance', {
        instanceId: instance.id,
        raw: Number(rawProgressPercent.toFixed(2)),
        deduped: Number(progressPercent.toFixed(2)),
        delta: Number(delta.toFixed(2))
      });
    }
  }, [rawProgressPercent, progressPercent, instance.id]);

  // Derive variance summary for UI (on each render; cheap)
  const varianceSummary = getProgressVarianceSummary(instance.id);

  return (
    <Card sx={{ mb: 3 }}>
      <CardContent>
        <Box display="flex" justifyContent="space-between" flexWrap="wrap" gap={2} alignItems="center" mb={1}>
          <Typography variant="h6" component="div">
            Runtime Snapshot
          </Typography>
          <Stack direction="row" spacing={1} alignItems="center">
            <FormControlLabel
              sx={{ mr: 0 }}
              control={
                <Switch
                  size="small"
                  checked={autoRefresh}
                  onChange={(e) => setAutoRefresh(e.target.checked)}
                  disabled={!running}
                />
              }
              label={
                <Typography variant="caption" color={!running ? 'text.disabled' : 'text.primary'}>
                  Auto Refresh {running ? '(5s)' : ''}
                </Typography>
              }
            />
            <Tooltip title="Refresh snapshot now">
              <span>
                <IconButton size="small" onClick={onRefresh} disabled={autoRefresh && running}>
                  <RefreshIcon fontSize="small" />
                </IconButton>
              </span>
            </Tooltip>
            {autoRefresh && running && (
              <Tooltip title="Auto refresh active">
                <PlayCircleIcon fontSize="small" color="success" />
              </Tooltip>
            )}
            {!autoRefresh && running && (
              <Tooltip title="Auto refresh paused">
                <PauseCircleIcon fontSize="small" color="warning" />
              </Tooltip>
            )}
          </Stack>
        </Box>

        {suspended && (
          <Alert severity="warning" sx={{ mb: 2 }} variant="outlined">
            Instance is suspended. Live state is frozen until resumed.
          </Alert>
        )}
        {failed && instance.errorMessage && (
          <Alert severity="error" sx={{ mb: 2 }} variant="outlined">
            {instance.errorMessage}
          </Alert>
        )}
        {completed && (
          <Alert severity="success" sx={{ mb: 2 }} variant="outlined">
            Instance completed. Snapshot is final.
          </Alert>
        )}
        {noActiveWhileRunning && (
          <Alert severity="info" sx={{ mb: 2 }} variant="outlined">
            No active nodes reported while status is Running. This can occur during transition or if background work
            is pending. Refresh if this persists.
          </Alert>
        )}

        <Box mb={2}>
          <Typography variant="caption" color="text.secondary">
            Progress (deduped {dedupVisitedExecutable ?? visitedNodeIds.length}
            {executableNodeTotal != null && executableNodeTotal > 0
              ? `/${executableNodeTotal} exec`
              : `/${totalNodes}`} nodes)
          </Typography>
          <LinearProgress
            variant="determinate"
            value={progressPercent}
            sx={{ height: 8, borderRadius: 1, mt: 0.5 }}
            color={completed ? 'success' : failed ? 'error' : 'primary'}
          />
          <Typography variant="caption" color="text.secondary" sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
            <span>{progressPercent.toFixed(1)}% deduped</span>
            {rawProgressPercent != null && Math.abs(rawProgressPercent - progressPercent) >= 0.1 && (
              <Tooltip title={`Raw (legacy) progress: ${rawProgressPercent.toFixed(1)}% based on total nodes=${totalNodes}`}>
                <span style={{ opacity: 0.75 }}>raw {rawProgressPercent.toFixed(1)}%</span>
              </Tooltip>
            )}
            {varianceSummary && varianceSummary.samples >= 3 && (
              <Tooltip
                title={
                  `Variance samples: ${varianceSummary.samples}\n` +
                  `Avg Δ: ${varianceSummary.avgDelta.toFixed(2)} pts\n` +
                  `Max Δ: ${varianceSummary.maxDelta.toFixed(2)} (raw ${varianceSummary.rawAtMax.toFixed(1)}% vs dedup ${varianceSummary.dedupedAtMax.toFixed(1)}%)`
                }
              >
                <span style={{ opacity: 0.85 }}>
                  Δ(avg {varianceSummary.avgDelta.toFixed(2)})
                </span>
              </Tooltip>
            )}
          </Typography>
         </Box>

        <Box
          sx={{
            display: 'grid',
            gap: 2,
            gridTemplateColumns: { xs: 'repeat(auto-fill,minmax(160px,1fr))', md: 'repeat(auto-fill,minmax(180px,1fr))' }
          }}
        >
          <Metric label="Active Nodes" value={currentNodeIds.length} items={currentNodeIds} color="info" />
          <Metric label="Visited Nodes" value={visitedNodeIds.length} />
          {dedupVisitedExecutable != null && executableNodeTotal != null && executableNodeTotal > 0 && (
            <Metric
              label="Exec Visited (dedup)"
              value={`${dedupVisitedExecutable}/${executableNodeTotal}`}
              color="primary"
            />
          )}
          <Metric label="Traversed Edges" value={traversedEdgeIds.length} />
          <Metric label="Tasks" value={tasksCount} />
          <Metric label="Events" value={eventsCount} />
          <Metric label="Status" value={instance.status} color={
            instance.status === 'Running' ? 'primary'
              : instance.status === 'Completed' ? 'success'
                : instance.status === 'Failed' ? 'error'
                  : instance.status === 'Suspended' ? 'warning'
                    : 'default'
          } />
        </Box>

        <Box mt={2} display="flex" flexWrap="wrap" gap={1} alignItems="center">
          {currentNodeIds.length > 0 && (
            <>
              <Typography variant="caption" color="text.secondary">
                Active:
              </Typography>
              {currentNodeIds.map(n => (
                <Chip
                  key={n}
                  size="small"
                  label={n}
                  color="info"
                  variant="outlined"
                  sx={{ fontSize: '0.65rem', height: 22 }}
                />
              ))}
            </>
          )}
          <Box flexGrow={1} />
          {lastUpdated && (
            <Tooltip title={lastUpdated.toLocaleString()}>
              <Typography variant="caption" color="text.secondary">
                Updated {timeAgo(lastUpdated)}
              </Typography>
            </Tooltip>
          )}
        </Box>
      </CardContent>
    </Card>
  );
};

const Metric: React.FC<{
  label: string;
  value: number | string;
  items?: string[];
  color?: 'default' | 'primary' | 'secondary' | 'error' | 'info' | 'warning' | 'success';
}> = ({ label, value, items, color = 'default' }) => (
  <Box>
    <Typography variant="subtitle2" color="text.secondary" sx={{ fontSize: '0.7rem', textTransform: 'uppercase', letterSpacing: 0.5 }}>
      {label}
    </Typography>
    <Chip
      size="small"
      label={value}
      color={color === 'default' ? undefined : color}
      variant={color === 'default' ? 'outlined' : 'filled'}
      sx={{ mt: 0.5 }}
    />
    {items && items.length === 0 && (
      <Typography variant="caption" color="text.secondary" display="block">—</Typography>
    )}
  </Box>
);

function timeAgo(dt: Date): string {
  const diff = Date.now() - dt.getTime();
  const sec = Math.floor(diff / 1000);
  if (sec < 60) return `${sec}s ago`;
  const min = Math.floor(sec / 60);
  if (min < 60) return `${min}m ago`;
  const hrs = Math.floor(min / 60);
  if (hrs < 24) return `${hrs}h ago`;
  const days = Math.floor(hrs / 24);
  return `${days}d ago`;
}

export default InstanceRuntimeSnapshotPanel;
