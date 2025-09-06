import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
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
import type { WorkflowInstanceDto, InstanceStatus } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import { InstanceStatusBadge } from './components/InstanceStatusBadge';

export function InstancesListPage() {
  const [instances, setInstances] = useState<WorkflowInstanceDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [terminateDialogOpen, setTerminateDialogOpen] = useState(false);
  const [instanceToTerminate, setInstanceToTerminate] = useState<WorkflowInstanceDto | null>(null);

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  useEffect(() => {
    if (currentTenant) {
      loadInstances();
    }
  }, [currentTenant]);

  const loadInstances = async () => {
    try {
      setLoading(true);
      const items = await workflowService.getInstances(); // unwrapped array
      setInstances(items);
    } catch {
      toast.error('Failed to load workflow instances');
    } finally {
      setLoading(false);
    }
  };

  const handleView = (id: GridRowId) => {
    navigate(`/app/workflow/instances/${id}`);
  };

  const handleSuspend = async (instance: WorkflowInstanceDto) => {
    try {
      await workflowService.suspendInstance(instance.id);
      toast.success('Workflow instance suspended');
      loadInstances();
    } catch (error) {
      console.error('Failed to suspend instance:', error);
      toast.error('Failed to suspend workflow instance');
    }
  };

  const handleResume = async (instance: WorkflowInstanceDto) => {
    try {
      await workflowService.resumeInstance(instance.id);
      toast.success('Workflow instance resumed');
      loadInstances();
    } catch (error) {
      console.error('Failed to resume instance:', error);
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
      loadInstances();
    } catch (error) {
      console.error('Failed to terminate instance:', error);
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
      width: 140,
      renderCell: (params) => {
        const instance = params.row as WorkflowInstanceDto;
        return (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5 }}>
            <InstanceStatusBadge instanceId={instance.id} compact />
          </Box>
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

        if (instance.status === 'Running') {
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

        if (instance.status === 'Suspended') {
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
          onClick={loadInstances}
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
