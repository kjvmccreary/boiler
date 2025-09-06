import { useState, useEffect, useCallback, useRef } from 'react';
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
  Timeline as TimelineIcon,
  Done as DoneIcon,
  HowToReg as ClaimIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type {
  WorkflowInstanceDto,
  WorkflowEventDto,
  TaskSummaryDto,
  InstanceRuntimeSnapshotDto
} from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import DefinitionDiagram from '@/features/workflow/definitions/DefinitionDiagram';
import { InstanceStatusBadge } from './components/InstanceStatusBadge';
import { useTaskHub } from './hooks/useTaskHub';
import { useInstanceHub } from './hooks/useInstanceHub';
import type { InstanceUpdatedEvent } from '@/services/workflowNotifications';

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

  const [claimingTaskId, setClaimingTaskId] = useState<number | null>(null);
  const [completingTaskId, setCompletingTaskId] = useState<number | null>(null);

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  const loadSnapshot = useCallback(async (silent = false) => {
    if (!instanceId) return;
    try {
      if (!silent) setLoading(true);
      const snapshot: InstanceRuntimeSnapshotDto = await workflowService.getRuntimeSnapshot(instanceId);
      setInstance(snapshot.instance);
      setTasks(snapshot.tasks);
      setEvents(snapshot.events);
      setDefinitionJson(snapshot.definitionJson);
      setTraversedEdgeIds(snapshot.traversedEdgeIds || []);
      setVisitedNodeIds(snapshot.visitedNodeIds || []);
      setCurrentNodeIds(snapshot.currentNodeIds || []);
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

  const eventColumns: GridColDef[] = [
    { field: 'occurredAt', headerName: 'Time', width: 180, type: 'dateTime', valueGetter: v => new Date(v) },
    { field: 'type', headerName: 'Type', width: 120 },
    { field: 'name', headerName: 'Event', flex: 1, minWidth: 200 },
    {
      field: 'data',
      headerName: 'Details',
      flex: 1,
      minWidth: 250,
      renderCell: p => (
        <Typography variant="body2" sx={{ fontFamily: 'monospace', fontSize: '0.8rem' }}>
          {p.value.length > 100 ? `${p.value.substring(0, 100)}...` : p.value}
        </Typography>
      )
    },
    { field: 'userId', headerName: 'User', width: 100, renderCell: p => p.value || 'System' }
  ];

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
          <InstanceStatusBadge instanceId={instance.id} compact existingStatus={instance.status as any} /> {/* Live badge */}
          <Tooltip title="Refresh (full snapshot)">
            <IconButton onClick={handleRefresh}>
              <RefreshIcon />
            </IconButton>
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
                <InstanceStatusBadge instanceId={instance.id} existingStatus={instance.status as any} />{/* full badge with label */}
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
            instanceStatus={instance.status}
            dueSoonMinutes={15}
          />
        </CardContent>
      </Card>

      {/* Tasks */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            <TaskIcon sx={{ mr: 1, verticalAlign: 'middle' }} />
            Tasks ({tasks.length})
          </Typography>
          <Box sx={{ height: 300 }}>
            <DataGridPremium
              rows={tasks}
              columns={taskColumns}
              loading={tasksLoading}
              pagination
              pageSizeOptions={[5, 10, 25]}
              initialState={{ pagination: { paginationModel: { pageSize: 5 } } }}
              disableRowSelectionOnClick
              sx={{ '& .MuiDataGrid-row:hover': { backgroundColor: 'action.hover' } }}
            />
          </Box>
        </CardContent>
      </Card>

      {/* Events */}
      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            <TimelineIcon sx={{ mr: 1, verticalAlign: 'middle' }} />
            Event Timeline ({events.length})
          </Typography>
          <Box sx={{ height: 400 }}>
            <DataGridPremium
              rows={events}
              columns={eventColumns}
              loading={eventsLoading}
              pagination
              pageSizeOptions={[10, 25, 50]}
              initialState={{
                pagination: { paginationModel: { pageSize: 10 } },
                sorting: { sortModel: [{ field: 'occurredAt', sort: 'desc' }] }
              }}
              slots={{ toolbar: GridToolbar }}
              slotProps={{
                toolbar: { showQuickFilter: true, quickFilterProps: { debounceMs: 500 } }
              }}
              disableRowSelectionOnClick
              sx={{ '& .MuiDataGrid-row:hover': { backgroundColor: 'action.hover' } }}
            />
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}

export default InstanceDetailsPage;
