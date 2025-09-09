import { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Chip,
  Button,
  Divider,
  Alert,
  IconButton,
  Tooltip,
  Switch,
  FormControlLabel
} from '@mui/material';
import {
  DataGridPremium,
  GridColDef,
  GridRowParams,
  GridActionsCellItem,
  GridToolbar,
} from '@mui/x-data-grid-premium';
import {
  ArrowBack as ArrowBackIcon,
  PlayArrow as StartIcon,
  Stop as StopIcon,
  Pause as PauseIcon,
  Refresh as RefreshIcon,
  Visibility as ViewIcon,
  Assignment as TaskIcon,
  Done as DoneIcon,
  HowToReg as ClaimIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import {
  WorkflowInstanceDto,
  WorkflowEventDto,
  TaskSummaryDto,
  InstanceRuntimeSnapshotDto
} from '@/types/workflow';
import { computeDedupedProgress } from './utils/progress';
import { getProgressVarianceSummary, clearProgressVariance } from './utils/progressVarianceStore';
import { trackWorkflow } from '@/features/workflow/telemetry/workflowTelemetry';
import SimulationDrawer from './components/SimulationDrawer';
import { diffWorkflowDefinitions } from '@/features/workflow/definitions/utils/diffWorkflowDefinitions';
import DefinitionDiagram from '@/features/workflow/definitions/DefinitionDiagram';
import { useTenant } from '@/contexts/TenantContext';
import { useTaskHub } from './hooks/useTaskHub';
import { useInstanceHub } from './hooks/useInstanceHub';
import type { InstanceUpdatedEvent } from '@/services/workflowNotifications';
import toast from 'react-hot-toast';
import { InstanceStatusBadge } from './components/InstanceStatusBadge';
import InstanceEventTimeline from './components/InstanceEventTimeline';

export function InstanceDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const instanceId = id ? parseInt(id, 10) : null;

  const [instance, setInstance] = useState<WorkflowInstanceDto | null>(null);
  const [events, setEvents] = useState<WorkflowEventDto[]>([]);
  const [tasks, setTasks] = useState<TaskSummaryDto[]>([]);
  const [definitionJson, setDefinitionJson] = useState<string | null>(null);
  const [traversedEdgeIds, setTraversedEdgeIds] = useState<string[]>([]);
  const [visitedNodeIds, setVisitedNodeIds] = useState<string[]>([]);
  const [currentNodeIds, setCurrentNodeIds] = useState<string[]>([]);
  const [loading, setLoading] = useState(true);
  const [eventsLoading] = useState(false);
  const [tasksLoading] = useState(false);
  // Diff overlay state
  const [overlayEnabled, setOverlayEnabled] = useState(false);
  const [overlayLoading, setOverlayLoading] = useState(false);
  const [overlayDiff, setOverlayDiff] = useState<ReturnType<typeof diffWorkflowDefinitions> | null>(null);

  const [claimingTaskId, setClaimingTaskId] = useState<number | null>(null);
  const [completingTaskId, setCompletingTaskId] = useState<number | null>(null);

  // Snapshot derived / panel state (C10)
  const [autoRefresh, setAutoRefresh] = useState<boolean>(true);
  const [lastUpdated, setLastUpdated] = useState<Date | null>(null);
  // Simulation state
  const [simulationOpen, setSimulationOpen] = useState(false);
  const [simulationHighlight, setSimulationHighlight] = useState<string[] | null>(null);

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  const loadSnapshot = useCallback(async (silent = false) => {
    if (!instanceId) return;
    try {
      if (!silent) setLoading(true);
      const snapshot: InstanceRuntimeSnapshotDto = await workflowService.getRuntimeSnapshot(instanceId);
      setInstance(snapshot.instance ?? null);
      setTasks((snapshot.tasks as any[] | undefined)?.map(t => t) ?? []);
      setEvents(snapshot.events ?? []);
      setDefinitionJson(snapshot.definitionJson ?? null);
      setTraversedEdgeIds(snapshot.traversedEdgeIds ?? []);
      setVisitedNodeIds(snapshot.visitedNodeIds || []);
      setCurrentNodeIds(snapshot.currentNodeIds || []);
      setLastUpdated(new Date());
      // If definition changes while overlay active, clear (force re-fetch)
      setOverlayDiff(null);
    } catch {
      if (!silent) toast.error('Failed to load runtime snapshot');
    } finally {
      if (!silent) setLoading(false);
    }
  }, [instanceId]);

  useEffect(() => {
    if (currentTenant && instanceId) {
      void loadSnapshot();
    }
  }, [currentTenant, instanceId, loadSnapshot]);

  // SignalR: when tasks change for tenant, refresh if instance still active
  useTaskHub(() => {
    if (instance && (instance.status === 'Running' || instance.status === 'Suspended')) {
      void loadSnapshot(true);
    }
  }, 1200);

  const handleRefresh = () => void loadSnapshot();

  const handleTerminateInstance = async () => {
    if (!instance) return;
    try {
      await workflowService.terminateInstance(instance.id);
      toast.success('Workflow instance terminated');
      await loadSnapshot(true);
    } catch {
      toast.error('Failed to terminate workflow instance');
    }
  };

  const handleSuspendInstance = async () => {
    if (!instance) return;
    try {
      await workflowService.suspendInstance(instance.id);
      toast.success('Workflow instance suspended');
      await loadSnapshot(true);
    } catch {
      toast.error('Failed to suspend workflow instance');
    }
  };

  const handleResumeInstance = async () => {
    if (!instance) return;
    try {
      await workflowService.resumeInstance(instance.id);
      toast.success('Workflow instance resumed');
      await loadSnapshot(true);
    } catch {
      toast.error('Failed to resume workflow instance');
    }
  };

  const handleClaimTask = async (taskId: number) => {
    try {
      setClaimingTaskId(taskId);
      await workflowService.claimTask(taskId, {});
      toast.success(`Task ${taskId} claimed`);
      await loadSnapshot(true);
    } catch {
      toast.error('Failed to claim task');
    } finally {
      setClaimingTaskId(null);
    }
  };

  const handleCompleteTask = async (taskId: number) => {
    try {
      setCompletingTaskId(taskId);
      await workflowService.completeTask(taskId, { completionData: 'Completed via Instance Details' });
      toast.success(`Task ${taskId} completed`);
      await loadSnapshot(true);
    } catch {
      toast.error('Failed to complete task');
    } finally {
      setCompletingTaskId(null);
    }
  };

  const formatDateTime = (d: string) => new Date(d).toLocaleString();

  const calculateDuration = () => {
    if (!instance) return 'Unknown';
    const start = new Date(instance.startedAt).getTime();
    const end = (instance.completedAt ? new Date(instance.completedAt) : new Date()).getTime();
    const diff = end - start;
    const h = Math.floor(diff / 3600000);
    const m = Math.floor((diff % 3600000) / 60000);
    return h > 0 ? `${h}h ${m}m` : `${m}m`;
  };

  const taskColumns: GridColDef[] = [
    { field: 'taskName', headerName: 'Task', flex: 1, minWidth: 200 },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      renderCell: p => {
        const s = p.value;
        switch (s) {
          case 'Created': return <Chip label="Available" size="small" />;
            case 'Assigned': return <Chip label="Assigned" color="info" size="small" />;
          case 'Claimed': return <Chip label="Claimed" color="primary" size="small" />;
          case 'InProgress': return <Chip label="In Progress" color="warning" size="small" />;
          case 'Completed': return <Chip label="Completed" color="success" size="small" />;
          case 'Cancelled': return <Chip label="Cancelled" color="error" size="small" />;
          default: return <Chip label={s} size="small" />;
        }
      }
    },
    {
      field: 'dueDate',
      headerName: 'Due Date',
      width: 160,
      type: 'dateTime',
      valueGetter: v => (v ? new Date(v) : null),
      renderCell: p => p.value ? new Date(p.value).toLocaleString() : 'No due date'
    },
    { field: 'createdAt', headerName: 'Created', width: 120, type: 'date', valueGetter: v => new Date(v) },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 140,
      getActions: (params: GridRowParams) => {
        const row = params.row as TaskSummaryDto;
        const actions = [
          <GridActionsCellItem
            key="view"
            icon={<ViewIcon />}
            label="View Task"
            onClick={() => navigate(`/app/workflow/tasks/${params.id}`)}
            showInMenu={false}
          />
        ];
        if (instance?.status === 'Running') {
          if (['Created', 'Assigned'].includes(row.status) && claimingTaskId !== row.id) {
            actions.push(
              <GridActionsCellItem
                key="claim"
                icon={<ClaimIcon />}
                label="Claim"
                disabled={claimingTaskId === row.id || completingTaskId === row.id}
                onClick={() => handleClaimTask(row.id)}
                showInMenu={false}
              />
            );
          }
          if (['Claimed', 'InProgress'].includes(row.status) && completingTaskId !== row.id) {
            actions.push(
              <GridActionsCellItem
                key="complete"
                icon={<DoneIcon />}
                label="Complete"
                disabled={completingTaskId === row.id || claimingTaskId === row.id}
                onClick={() => handleCompleteTask(row.id)}
                showInMenu={false}
              />
            );
          }
        }
        return actions;
      }
    }
  ];

  const handleInstancePush = useCallback((evt: InstanceUpdatedEvent) => {
    if (!instance || evt.instanceId !== instance.id) return;
    const statusChanged = instance.status !== evt.status;
    const nodesChanged = evt.currentNodeIds && evt.currentNodeIds !== instance.currentNodeIds;
    // Cheap merge
    setInstance(prev => prev ? ({
      ...prev,
      status: evt.status as any,
      completedAt: evt.completedAt || prev.completedAt,
      errorMessage: evt.errorMessage ?? prev.errorMessage,
      currentNodeIds: evt.currentNodeIds || prev.currentNodeIds
    }) : prev);

    if (statusChanged || nodesChanged) {
      // Silent snapshot refresh (will update tasks/events if needed)
      void loadSnapshot(true);
    }
  }, [instance, loadSnapshot]);

  useInstanceHub({
    onInstanceUpdated: handleInstancePush,
    onInstancesChanged: () => {
      // InstancesChanged is coarse; if still running, do a light refresh
      if (instance && (instance.status === 'Running' || instance.status === 'Suspended')) {
        void loadSnapshot(true);
      }
    }
  });

  // Auto refresh effect (C10)
  useEffect(() => {
    if (!autoRefresh) return;
    if (!instance) return;
    if (!(instance.status === 'Running')) return;
    const id = window.setTimeout(() => {
      void loadSnapshot(true);
    }, 5000);
    return () => window.clearTimeout(id);
  }, [autoRefresh, instance, loadSnapshot, tasks.length, currentNodeIds.join(',')]);

  // Progress metrics (raw + deduped)
  const {
    totalNodes,
    executableTotal,
    dedupVisitedExecutable,
    rawPercent,
    dedupedPercent
  } = computeDedupedProgress(definitionJson, visitedNodeIds);

  /* ---------------- Version Diff Overlay (Instance Context) ----------------
     Highlights added (green) & modified (amber) nodes vs the immediately previous
     definition version, if available.
  */
  const fetchOverlayDiff = useCallback(async () => {
    if (!instance) return;
    // Need the definition version and an API path to previous JSON
    const currentVersion = instance.definitionVersion;
    // Only makes sense if version > 1
    if (!currentVersion || currentVersion <= 1) {
      toast('No previous version to diff', { icon: 'ℹ️' });
      setOverlayEnabled(false);
      return;
    }
    if (!definitionJson) {
      toast.error('Definition JSON not loaded yet');
      setOverlayEnabled(false);
      return;
    }
    setOverlayLoading(true);
    try {
      // Attempt to find previous version: use list endpoint (already available via workflowService.getDefinitions)
      const list = await workflowService.getDefinitions({
        page: 1,
        pageSize: 200,
        sortBy: 'createdAt',
        desc: true
      });
      // We match by name + prior version (assuming stable name across versions)
      const prev = list.find(
        d =>
          d.name === instance.workflowDefinitionName &&
          d.version === currentVersion - 1
      );
      if (!prev?.jsonDefinition) {
        toast('Previous version JSON not found', { icon: '⚠️' });
        setOverlayEnabled(false);
        return;
      }
      const diff = diffWorkflowDefinitions(definitionJson, prev.jsonDefinition);
      setOverlayDiff(diff);
      trackWorkflow('diff.viewer.overlay.toggle', {
        enabled: true,
        context: 'instance',
        added: diff.summary.addedNodes,
        modified: diff.summary.modifiedNodes,
        removed: diff.summary.removedNodes,
        currentVersion,
        previousVersion: currentVersion - 1
      });
    } catch {
      toast.error('Failed to compute diff overlay');
      setOverlayEnabled(false);
    } finally {
      setOverlayLoading(false);
    }
  }, [instance, definitionJson]);

  // When user toggles overlay on, fetch diff once
  useEffect(() => {
    if (overlayEnabled && !overlayDiff) {
      void fetchOverlayDiff();
    }
    if (!overlayEnabled) {
      setOverlayDiff(null);
    }
  }, [overlayEnabled, overlayDiff, fetchOverlayDiff]);

  const diffOverlayData = overlayEnabled && overlayDiff
    ? {
        added: new Set(overlayDiff.addedNodes.map(n => n.id)),
        modified: new Set(overlayDiff.modifiedNodes.map(m => m.id)),
        removed: new Set(overlayDiff.removedNodes.map(n => n.id))
      }
    : undefined;

  // Disable auto when terminal
  useEffect(() => {
    if (instance && ['Completed', 'Failed', 'Cancelled', 'Suspended'].includes(String(instance.status))) {
      setAutoRefresh(false);
      // Emit variance summary telemetry once when terminal (Running -> terminal transition)
      const summary = getProgressVarianceSummary(instance.id);
      if (summary) {
        trackWorkflow('instance.progress.dedupe.summary', {
          instanceId: summary.instanceId,
          samples: summary.samples,
          avgDelta: Number(summary.avgDelta.toFixed(3)),
          maxDelta: Number(summary.maxDelta.toFixed(3)),
          firstDelta: Number(summary.firstDelta.toFixed(3)),
          lastDelta: Number(summary.lastDelta.toFixed(3))
        });
        // Optionally clear to avoid double emission if page re-renders with same instance
        clearProgressVariance(instance.id);
      }
    }
  }, [instance?.status, instance?.id]);

  // Lazy import panel component (could also static import if preferred)
  // eslint-disable-next-line @typescript-eslint/no-var-requires
  const InstanceRuntimeSnapshotPanel = require('./components/InstanceRuntimeSnapshotPanel').InstanceRuntimeSnapshotPanel;

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Select a tenant to view workflow instances</Typography>
      </Box>
    );
  }

  if (loading && !instance) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Loading workflow instance...</Typography>
      </Box>
    );
  }

  if (!instance) {
    return (
      <Box sx={{ p: 3 }}>
        <Button startIcon={<ArrowBackIcon />} onClick={() => navigate('/app/workflow/instances')} sx={{ mb: 2 }}>
          Back to Instances
        </Button>
        <Alert severity="error">Workflow instance not found</Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <IconButton onClick={() => navigate('/app/workflow/instances')} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4" component="h1">
            Workflow Instance
            <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
              {instance.workflowDefinitionName} - Instance {instance.id}
            </Typography>
          </Typography>
        </Box>
        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
          <InstanceStatusBadge
            instanceId={instance.id}
            compact
            existingStatus={instance.status as any}
            showProgress
          /> {/* Live badge */}
          <Tooltip title="Refresh (full snapshot)">
            <IconButton onClick={handleRefresh}>
              <RefreshIcon />
            </IconButton>
          </Tooltip>
          <Tooltip title="Simulate paths (dry-run)">
            <Button
              size="small"
              variant="outlined"
              onClick={() => {
                setSimulationOpen(true);
                trackWorkflow('simulation.drawer.open', {
                  definitionVersion: instance.definitionVersion
                });
              }}
            >
              Simulate
            </Button>
          </Tooltip>
          {instance.status === 'Running' && (
            <>
              <Button variant="outlined" startIcon={<PauseIcon />} onClick={handleSuspendInstance} size="small">
                Suspend
              </Button>
              <Button variant="outlined" color="error" startIcon={<StopIcon />} onClick={handleTerminateInstance} size="small">
                Terminate
              </Button>
            </>
          )}
          {instance.status === 'Suspended' && (
            <Button variant="outlined" color="primary" startIcon={<StartIcon />} onClick={handleResumeInstance} size="small">
              Resume
            </Button>
          )}
        </Box>
      </Box>

      {/* Runtime Snapshot Panel (C10) */}
      <InstanceRuntimeSnapshotPanel
        instance={instance}
        visitedNodeIds={visitedNodeIds}
        currentNodeIds={currentNodeIds}
        traversedEdgeIds={traversedEdgeIds}
        totalNodes={totalNodes}
        progressPercent={dedupedPercent}
        rawProgressPercent={rawPercent}
        executableNodeTotal={executableTotal}
        dedupVisitedExecutable={dedupVisitedExecutable}
        tasksCount={tasks.length}
        eventsCount={events.length}
        onRefresh={handleRefresh}
        autoRefresh={autoRefresh}
        setAutoRefresh={setAutoRefresh}
        lastUpdated={lastUpdated}
      />

      {/* Overview */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>Instance Overview</Typography>
          <Box
            sx={{
              display: 'grid',
              gap: 3,
              gridTemplateColumns: { xs: '1fr', md: '1fr 1fr' }
            }}
          >
            {/* Left Column */}
            <Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">Status (live)</Typography>
                <InstanceStatusBadge
                  instanceId={instance.id}
                  existingStatus={instance.status as any}
                  showProgress
                />{/* full badge with label */}
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">Workflow Definition</Typography>
                <Typography>{instance.workflowDefinitionName} (v{instance.definitionVersion})</Typography>
              </Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">Current Nodes</Typography>
                <Typography sx={{ fontFamily: 'monospace' }}>
                  {currentNodeIds.join(', ') || 'None'}
                </Typography>
              </Box>
            </Box>

            {/* Right Column */}
            <Box>
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">Started</Typography>
                <Typography>{formatDateTime(instance.startedAt)}</Typography>
                {instance.startedByUserId && (
                  <Typography variant="caption" color="text.secondary">
                    By User {instance.startedByUserId}
                  </Typography>
                )}
              </Box>
              {instance.completedAt && (
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">Completed</Typography>
                  <Typography>{formatDateTime(instance.completedAt)}</Typography>
                </Box>
              )}
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">Duration</Typography>
                <Typography>{calculateDuration()}</Typography>
              </Box>
            </Box>

            {instance.errorMessage && (
              <Box sx={{ gridColumn: '1 / -1' }}>
                <Alert severity="error" sx={{ mt: 1 }}>
                  <Typography variant="subtitle2">Error Message</Typography>
                  <Typography variant="body2">{instance.errorMessage}</Typography>
                </Alert>
              </Box>
            )}

            <Box sx={{ gridColumn: '1 / -1' }}>
              <Divider sx={{ my: 2 }} />
              <Typography variant="subtitle2" color="text.secondary" gutterBottom>Context Data</Typography>
              <Box
                sx={{
                  backgroundColor: 'grey.50',
                  p: 2,
                  borderRadius: 1,
                  fontFamily: 'monospace',
                  fontSize: '0.875rem',
                  maxHeight: 200,
                  overflow: 'auto'
                }}
              >
                <pre>{JSON.stringify(JSON.parse(instance.context || '{}'), null, 2)}</pre>
              </Box>
            </Box>
          </Box>
        </CardContent>
      </Card>

      {/* Runtime Diagram */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>Runtime Diagram</Typography>
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1.5 }}>
            Visual overlay of active, completed, overdue and traversed path.
          </Typography>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 1 }}>
            <FormControlLabel
              control={
                <Switch
                  size="small"
                  disabled={overlayLoading}
                  checked={overlayEnabled}
                  onChange={(e) => {
                    const en = e.target.checked;
                    setOverlayEnabled(en);
                    if (!en) {
                      trackWorkflow('diff.viewer.overlay.toggle', {
                        enabled: false,
                        context: 'instance'
                      });
                    }
                  }}
                />
              }
              label={
                <Typography variant="caption">
                  Diff Overlay {overlayLoading && '(loading...)'}
                </Typography>
              }
            />
            {overlayEnabled && overlayDiff && (
              <Box sx={{ display: 'flex', gap: 1 }}>
                <Chip size="small" color="success" label={`+${overlayDiff.summary.addedNodes}`} />
                <Chip size="small" color="warning" label={`Δ${overlayDiff.summary.modifiedNodes}`} />
                <Chip size="small" variant="outlined" color="error" label={`-${overlayDiff.summary.removedNodes}`} />
              </Box>
            )}
          </Box>
          <DefinitionDiagram
            jsonDefinition={definitionJson}
            currentNodeIds={currentNodeIds}
            tasks={tasks
              .map(t => ({
                nodeId: t.nodeId || '',
                status: t.status as string,
                dueDate: t.dueDate
              }))
              .filter(t => t.nodeId)}
            traversedEdgeIds={traversedEdgeIds}
            visitedNodeIds={visitedNodeIds}
            instanceStatus={String(instance.status)}
            dueSoonMinutes={15}
            diffOverlay={diffOverlayData}
            simulationHighlightNodeIds={simulationHighlight}
          />
        </CardContent>
      </Card>

      {/* Event Timeline (C11) */}
      <InstanceEventTimeline
        instanceId={instance.id}
        initialEvents={events}
      />

      <SimulationDrawer
        open={simulationOpen}
        onClose={() => {
          setSimulationOpen(false);
          setSimulationHighlight(null);
          trackWorkflow('simulation.drawer.close', {});
        }}
        definitionJson={definitionJson}
        onHighlightPath={(nodes) => setSimulationHighlight(nodes)}
      />
    </Box>
  );
}

export default InstanceDetailsPage;
