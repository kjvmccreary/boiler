import { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Typography,
  Card,
  CardContent,
  Chip,
  Button,
  Grid, // ✅ FIX: Use regular Grid, not Grid2
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
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { 
  WorkflowInstanceDto, 
  WorkflowEventDto, 
  TaskSummaryDto, 
  InstanceStatus 
} from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import DefinitionDiagram from '@/features/workflow/definitions/DefinitionDiagram';

export function InstanceDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const [instance, setInstance] = useState<WorkflowInstanceDto | null>(null);
  const [events, setEvents] = useState<WorkflowEventDto[]>([]);
  const [tasks, setTasks] = useState<TaskSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [eventsLoading, setEventsLoading] = useState(false);
  const [tasksLoading, setTasksLoading] = useState(false);
  const [definitionJson, setDefinitionJson] = useState<string | null>(null); // ADD state

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  const loadSnapshot = async () => {
    if (!id) return;
    try {
      setLoading(true);
      const snapshot = await workflowService.getRuntimeSnapshot(parseInt(id));
      setInstance(snapshot.instance);
      setTasks(snapshot.tasks);
      setEvents(snapshot.events);
      setDefinitionJson(snapshot.definitionJson);
    } catch (e) {
      toast.error('Failed to load runtime snapshot');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (currentTenant && id) {
      loadSnapshot();
    }
  }, [currentTenant, id]);

  const handleRefresh = () => {
    loadSnapshot();
  };

  const handleTerminateInstance = async () => {
    if (!instance) return;

    try {
      await workflowService.terminateInstance(instance.id);
      toast.success('Workflow instance terminated');
      loadSnapshot();
    } catch (error) {
      console.error('Failed to terminate instance:', error);
      toast.error('Failed to terminate workflow instance');
    }
  };

  const handleSuspendInstance = async () => {
    if (!instance) return;

    try {
      await workflowService.suspendInstance(instance.id);
      toast.success('Workflow instance suspended');
      loadSnapshot();
    } catch (error) {
      console.error('Failed to suspend instance:', error);
      toast.error('Failed to suspend workflow instance');
    }
  };

  const handleResumeInstance = async () => {
    if (!instance) return;

    try {
      await workflowService.resumeInstance(instance.id);
      toast.success('Workflow instance resumed');
      loadSnapshot();
    } catch (error) {
      console.error('Failed to resume instance:', error);
      toast.error('Failed to resume workflow instance');
    }
  };

  const getStatusChip = (status: InstanceStatus) => {
    switch (status) {
      case 'Running':
        return <Chip label="Running" color="primary" size="small" icon={<StartIcon />} />;
      case 'Completed':
        return <Chip label="Completed" color="success" size="small" />;
      case 'Failed':
        return <Chip label="Failed" color="error" size="small" />;
      case 'Cancelled':
        return <Chip label="Cancelled" color="default" size="small" />;
      case 'Suspended':
        return <Chip label="Suspended" color="warning" size="small" icon={<PauseIcon />} />;
      default:
        return <Chip label={status} color="default" size="small" />;
    }
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const calculateDuration = () => {
    if (!instance) return 'Unknown';
    
    const start = new Date(instance.startedAt);
    const end = instance.completedAt ? new Date(instance.completedAt) : new Date();
    const diffMs = end.getTime() - start.getTime();
    
    const hours = Math.floor(diffMs / (1000 * 60 * 60));
    const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
    
    if (hours > 0) {
      return `${hours}h ${minutes}m`;
    }
    return `${minutes}m`;
  };

  // Event columns for DataGrid
  const eventColumns: GridColDef[] = [
    {
      field: 'occurredAt',
      headerName: 'Time',
      width: 180,
      type: 'dateTime',
      valueGetter: (value) => new Date(value),
    },
    {
      field: 'type',
      headerName: 'Type',
      width: 120,
    },
    {
      field: 'name',
      headerName: 'Event',
      flex: 1,
      minWidth: 200,
    },
    {
      field: 'data',
      headerName: 'Details',
      flex: 1,
      minWidth: 250,
      renderCell: (params) => (
        <Typography variant="body2" sx={{ fontFamily: 'monospace', fontSize: '0.8rem' }}>
          {params.value.length > 100 ? `${params.value.substring(0, 100)}...` : params.value}
        </Typography>
      ),
    },
    {
      field: 'userId',
      headerName: 'User',
      width: 100,
      renderCell: (params) => params.value || 'System',
    },
  ];

  // Task columns for DataGrid
  const taskColumns: GridColDef[] = [
    {
      field: 'taskName',
      headerName: 'Task',
      flex: 1,
      minWidth: 200,
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      renderCell: (params) => {
        const status = params.value;
        switch (status) {
          case 'Created':
            return <Chip label="Available" color="default" size="small" />;
          case 'Assigned':
            return <Chip label="Assigned" color="info" size="small" />;
          case 'Claimed':
            return <Chip label="Claimed" color="primary" size="small" />;
          case 'InProgress':
            return <Chip label="In Progress" color="warning" size="small" />;
          case 'Completed':
            return <Chip label="Completed" color="success" size="small" />;
          case 'Cancelled':
            return <Chip label="Cancelled" color="error" size="small" />;
          default:
            return <Chip label={status} color="default" size="small" />;
        }
      },
    },
    {
      field: 'dueDate',
      headerName: 'Due Date',
      width: 160,
      type: 'dateTime',
      valueGetter: (value) => value ? new Date(value) : null,
      renderCell: (params) => {
        if (!params.value) return 'No due date';
        return new Date(params.value).toLocaleString();
      },
    },
    {
      field: 'createdAt',
      headerName: 'Created',
      width: 120,
      type: 'date',
      valueGetter: (value) => new Date(value),
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 100,
      getActions: (params: GridRowParams) => [
        <GridActionsCellItem
          icon={<ViewIcon />}
          label="View Task"
          onClick={() => navigate(`/app/workflow/tasks/${params.id}`)}  // ✅ Correct
        />,
      ],
    },
  ];

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view workflow instances</Typography>
      </Box>
    );
  }

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Loading workflow instance...</Typography>
      </Box>
    );
  }

  if (!instance) {
    return (
      <Box sx={{ p: 3 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/app/workflow/instances')}
          sx={{ mb: 2 }}
        >
          Back to Instances
        </Button>
        <Alert severity="error">
          Workflow instance not found
        </Alert>
      </Box>
    );
  }

  return (
    <Box sx={{ p: 3 }}>
      {/* Header */}
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <IconButton
            onClick={() => navigate('/app/workflow/instances')}
            sx={{ mr: 1 }}
          >
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4" component="h1">
            Workflow Instance
            <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
              {instance.workflowDefinitionName} - Instance {instance.id}
            </Typography>
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', gap: 1 }}>
          <Tooltip title="Refresh">
            <IconButton onClick={handleRefresh}>
              <RefreshIcon />
            </IconButton>
          </Tooltip>

          {instance.status === 'Running' && (
            <>
              <Button
                variant="outlined"
                startIcon={<PauseIcon />}
                onClick={handleSuspendInstance}
                size="small"
              >
                Suspend
              </Button>
              <Button
                variant="outlined"
                color="error"
                startIcon={<StopIcon />}
                onClick={handleTerminateInstance}
                size="small"
              >
                Terminate
              </Button>
            </>
          )}

          {instance.status === 'Suspended' && (
            <Button
              variant="outlined"
              color="primary"
              startIcon={<StartIcon />}
              onClick={handleResumeInstance}
              size="small"
            >
              Resume
            </Button>
          )}
        </Box>
      </Box>

      {/* Instance Overview */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Instance Overview
          </Typography>
          
          <Grid container spacing={3}>
            <Grid size={{ xs: 12, md: 6 }}>
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Status
                </Typography>
                {getStatusChip(instance.status)}
              </Box>
              
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Workflow Definition
                </Typography>
                <Typography variant="body1">
                  {instance.workflowDefinitionName} (v{instance.definitionVersion})
                </Typography>
              </Box>

              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Current Nodes
                </Typography>
                <Typography variant="body1" sx={{ fontFamily: 'monospace' }}>
                  {instance.currentNodeIds || 'None'}
                </Typography>
              </Box>
            </Grid>

            <Grid size={{ xs: 12, md: 6 }}>
              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Started
                </Typography>
                <Typography variant="body1">
                  {formatDateTime(instance.startedAt)}
                </Typography>
                {instance.startedByUserId && (
                  <Typography variant="caption" color="text.secondary">
                    By User {instance.startedByUserId}
                  </Typography>
                )}
              </Box>

              {instance.completedAt && (
                <Box sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" color="text.secondary">
                    Completed
                  </Typography>
                  <Typography variant="body1">
                    {formatDateTime(instance.completedAt)}
                  </Typography>
                </Box>
              )}

              <Box sx={{ mb: 2 }}>
                <Typography variant="subtitle2" color="text.secondary">
                  Duration
                </Typography>
                <Typography variant="body1">
                  {calculateDuration()}
                </Typography>
              </Box>
            </Grid>

            {instance.errorMessage && (
              <Grid size={{ xs: 12 }}>
                <Alert severity="error" sx={{ mt: 2 }}>
                  <Typography variant="subtitle2">Error Message</Typography>
                  <Typography variant="body2">{instance.errorMessage}</Typography>
                </Alert>
              </Grid>
            )}

            <Grid size={{ xs: 12 }}>
              <Divider sx={{ my: 2 }} />
              <Typography variant="subtitle2" color="text.secondary" gutterBottom>
                Context Data
              </Typography>
              <Box sx={{ 
                backgroundColor: 'grey.50', 
                p: 2, 
                borderRadius: 1,
                fontFamily: 'monospace',
                fontSize: '0.875rem',
                maxHeight: 200,
                overflow: 'auto'
              }}>
                <pre>{JSON.stringify(JSON.parse(instance.context || '{}'), null, 2)}</pre>
              </Box>
            </Grid>
          </Grid>
        </CardContent>
      </Card>

      {/* Runtime Diagram */}
      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Runtime Diagram
          </Typography>
          <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1.5 }}>
            Visual overlay of active, completed, overdue and traversed path.
          </Typography>
          {instance.workflowDefinitionId && instance.workflowDefinitionName && (
            <DefinitionDiagram
              jsonDefinition={definitionJson}
              currentNodeIds={instance.currentNodeIds}
              tasks={tasks.map(t => ({
                nodeId: t.nodeId || '',
                status: t.status,
                dueDate: t.dueDate
              })).filter(t => t.nodeId)}
              traversedEdgeIds={events
                .filter(e => e.type === 'Edge' && e.name === 'EdgeTraversed')
                .map(e => {
                  try { return JSON.parse(e.data || '{}').edgeId; } catch { return undefined; }
                })
                .filter(Boolean) as string[]}
              visitedNodeIds={events
                .filter(e => e.type === 'Node')
                .map(e => {
                  try { return JSON.parse(e.data || '{}').nodeId; } catch { return undefined; }
                })
                .filter(Boolean) as string[]}
              dueSoonMinutes={15}
            />
          )}
          {!tasks.length && (
            <Typography variant="caption" color="text.secondary" sx={{ mt: 1, display: 'block' }}>
              (Task node positions shown after first activation if nodeId not yet emitted by backend.)
            </Typography>
          )}
        </CardContent>
      </Card>

      {/* Tasks Section */}
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
              initialState={{
                pagination: { paginationModel: { pageSize: 5 } },
              }}
              disableRowSelectionOnClick
              sx={{
                '& .MuiDataGrid-row:hover': {
                  backgroundColor: 'action.hover',
                },
              }}
            />
          </Box>
        </CardContent>
      </Card>

      {/* Events Timeline */}
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
                sorting: {
                  sortModel: [{ field: 'occurredAt', sort: 'desc' }],
                },
              }}
              slots={{
                toolbar: GridToolbar,
              }}
              slotProps={{
                toolbar: {
                  showQuickFilter: true,
                  quickFilterProps: { 
                    debounceMs: 500,
                    // ✅ FIX: Remove placeholder property
                  },
                },
              }}
              disableRowSelectionOnClick
              sx={{
                '& .MuiDataGrid-row:hover': {
                  backgroundColor: 'action.hover',
                },
              }}
            />
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}

export default InstanceDetailsPage;
