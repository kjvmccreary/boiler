import { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Chip
} from '@mui/material';
import {
  DataGridPremium,
  GridColDef,
  GridRowParams,
  GridActionsCellItem,
  GridRowId,
  GridToolbar,
} from '@mui/x-data-grid-premium';
import {
  Visibility as ViewIcon,
  PlayArrow as StartIcon,
  Stop as StopIcon,
  Pause as PauseIcon,
  Refresh as RefreshIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowInstanceDto } from '@/types/workflow';
import { InstanceStatus } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import { InstanceStatusBadge } from './components/InstanceStatusBadge';
import { useTaskHub } from './hooks/useTaskHub';
import { useInstanceHub } from './hooks/useInstanceHub';
import type { InstanceUpdatedEvent } from '@/services/workflowNotifications';

const TERMINAL: InstanceStatus[] = [
  InstanceStatus.Completed,
  InstanceStatus.Cancelled,
  InstanceStatus.Failed
];

function staticStatusChip(status: InstanceStatus) {
  switch (status) {
    case InstanceStatus.Running: return <Chip label="Running" color="primary" size="small" />;
    case InstanceStatus.Completed: return <Chip label="Completed" color="success" size="small" />;
    case InstanceStatus.Failed: return <Chip label="Failed" color="error" size="small" />;
    case InstanceStatus.Cancelled: return <Chip label="Cancelled" size="small" />;
    case InstanceStatus.Suspended: return <Chip label="Suspended" color="warning" size="small" />;
    default: return <Chip label={String(status)} size="small" />;
  }
}

export function InstancesListPage() {
  const [instances, setInstances] = useState<WorkflowInstanceDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [terminateDialogOpen, setTerminateDialogOpen] = useState(false);
  const [instanceToTerminate, setInstanceToTerminate] = useState<WorkflowInstanceDto | null>(null);

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  const loadInstances = useCallback(async (silent = false) => {
    try {
      if (!silent) setLoading(true);
      const items = await workflowService.getInstances();
      setInstances(items);
    } catch {
      if (!silent) toast.error('Failed to load workflow instances');
    } finally {
      if (!silent) setLoading(false);
    }
  }, []);

  useEffect(() => {
    if (currentTenant) {
      void loadInstances();
    }
  }, [currentTenant, loadInstances]);

  // SignalR: refresh list on tasks changed events (throttled)
  useTaskHub(() => {
    // Only refresh if at least one row is non-terminal (benefit)
    if (instances.some(i => i.status === 'Running' || i.status === 'Suspended')) {
      void loadInstances(true);
    }
  }, 1000);

  const mergeInstancePush = (evt: InstanceUpdatedEvent) => {
    setInstances(prev => {
      const idx = prev.findIndex(p => p.id === evt.instanceId);
      if (idx === -1) return prev;
      const next = [...prev];
      const row = { ...next[idx] };
      row.status = evt.status as any;
      row.completedAt = evt.completedAt || row.completedAt;
      row.errorMessage = evt.errorMessage ?? row.errorMessage;
      // currentNodeIds normalization (backend may send JSON array string or csv)
      if (evt.currentNodeIds) row.currentNodeIds = evt.currentNodeIds;
      next[idx] = row;
      return next;
    });
  };

  useInstanceHub({
    onInstanceUpdated: mergeInstancePush,
    onInstancesChanged: () => {
      // Silent refresh only if any non-terminal present (avoid hammering backend)
      if (instances.some(i => i.status === 'Running' || i.status === 'Suspended')) {
        void loadInstances(true);
      }
    }
  });

  const handleView = (id: GridRowId) => {
    navigate(`/app/workflow/instances/${id}`);
  };

  const handleSuspend = async (instance: WorkflowInstanceDto) => {
    try {
      await workflowService.suspendInstance(instance.id);
      toast.success('Workflow instance suspended');
      void loadInstances(true);
    } catch {
      toast.error('Failed to suspend workflow instance');
    }
  };

  const handleResume = async (instance: WorkflowInstanceDto) => {
    try {
      await workflowService.resumeInstance(instance.id);
      toast.success('Workflow instance resumed');
      void loadInstances(true);
    } catch {
      toast.error('Failed to resume workflow instance');
    }
  };

  const handleTerminate = (instance: WorkflowInstanceDto) => {
    setInstanceToTerminate(instance);
    setTerminateDialogOpen(true);
  };

  const handleTerminateConfirm = async () => {
    if (!instanceToTerminate) return;
    try {
      await workflowService.terminateInstance(instanceToTerminate.id);
      toast.success('Workflow instance terminated');
      void loadInstances(true);
    } catch {
      toast.error('Failed to terminate workflow instance');
    } finally {
      setTerminateDialogOpen(false);
      setInstanceToTerminate(null);
    }
  };

  const formatDateTime = (dateString: string) => new Date(dateString).toLocaleString();

  const calculateDuration = (startedAt: string, completedAt?: string) => {
    const start = new Date(startedAt);
    const end = completedAt ? new Date(completedAt) : new Date();
    const diffMs = end.getTime() - start.getTime();
    const hours = Math.floor(diffMs / (1000 * 60 * 60));
    const minutes = Math.floor((diffMs % (1000 * 60 * 60)) / (1000 * 60));
    if (hours > 0) return `${hours}h ${minutes}m`;
    return `${minutes}m`;
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'Instance ID', width: 120 },
    {
      field: 'workflowDefinitionName',
      headerName: 'Workflow',
      flex: 1,
      minWidth: 200,
      renderCell: (params) => {
        const instance = params.row as WorkflowInstanceDto;
        return (
          <Box>
            <Typography variant="subtitle2" fontWeight="medium">
              {params.value}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              v{instance.definitionVersion}
            </Typography>
          </Box>
        );
      },
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 150,
      renderCell: (params) => {
        const inst = params.row as WorkflowInstanceDto;
        if (TERMINAL.includes(inst.status as InstanceStatus)) {
          return staticStatusChip(inst.status as InstanceStatus);
        }
        return (
          <InstanceStatusBadge
            instanceId={inst.id}
            compact
            existingStatus={inst.status as InstanceStatus}
            showProgress
          />
        );
      },
      sortable: true
    },
    {
      field: 'currentNodeIds',
      headerName: 'Current Node(s)',
      width: 150,
      renderCell: (params) => (
        <Typography variant="body2" sx={{ fontFamily: 'monospace', fontSize: '0.8rem' }}>
          {params.value || 'None'}
        </Typography>
      ),
    },
    {
      field: 'startedAt',
      headerName: 'Started',
      width: 160,
      type: 'dateTime',
      valueGetter: (value) => new Date(value),
      renderCell: (params) => formatDateTime(params.value),
    },
    {
      field: 'duration',
      headerName: 'Duration',
      width: 100,
      renderCell: (params) => {
        const instance = params.row as WorkflowInstanceDto;
        return calculateDuration(instance.startedAt, instance.completedAt);
      },
    },
    {
      field: 'startedByUserId',
      headerName: 'Started By',
      width: 120,
      renderCell: (params) => params.value || 'System',
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      getActions: (params: GridRowParams) => {
        const instance = params.row as WorkflowInstanceDto;
        const actions = [
          <GridActionsCellItem
            key="view"
            icon={<ViewIcon />}
            label="View Details"
            onClick={() => handleView(params.id)}
          />,
        ];

        if (instance.status === InstanceStatus.Running) {
          actions.push(
            <GridActionsCellItem
              key="suspend"
              icon={<PauseIcon />}
              label="Suspend"
              onClick={() => handleSuspend(instance)}
              showInMenu
            />,
            <GridActionsCellItem
              key="terminate"
              icon={<StopIcon />}
              label="Terminate"
              onClick={() => handleTerminate(instance)}
              showInMenu
            />
          );
        }

        if (instance.status === InstanceStatus.Suspended) {
          actions.push(
            <GridActionsCellItem
              key="resume"
              icon={<StartIcon />}
              label="Resume"
              onClick={() => handleResume(instance)}
              showInMenu
            />
          );
        }

        return actions;
      },
    },
  ];

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view workflow instances</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Workflow Instances
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Typography>

        <Button
          variant="outlined"
          startIcon={<RefreshIcon />}
          onClick={() => void loadInstances()}
          disabled={loading}
        >
          Refresh
        </Button>
      </Box>

      <Box sx={{ flex: 1, minHeight: 0 }}>
        <DataGridPremium
          rows={instances}
          columns={columns}
          loading={loading}
          pagination
          pageSizeOptions={[10, 25, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 25 } },
            sorting: { sortModel: [{ field: 'startedAt', sort: 'desc' }] },
          }}
          slots={{ toolbar: GridToolbar }}
          slotProps={{
            toolbar: {
              showQuickFilter: true,
              quickFilterProps: { debounceMs: 500 },
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

      <Dialog open={terminateDialogOpen} onClose={() => setTerminateDialogOpen(false)}>
        <DialogTitle>Confirm Terminate</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to terminate workflow instance "{instanceToTerminate?.id}"?
            This will stop the workflow execution immediately.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setTerminateDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleTerminateConfirm} color="error" variant="contained">
            Terminate
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default InstancesListPage;
