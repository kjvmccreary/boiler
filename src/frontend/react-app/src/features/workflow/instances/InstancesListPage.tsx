import { useState, useEffect, useCallback, useMemo } from 'react';
import {
  Box,
  Typography,
  Button,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Tooltip,
  Stack,
  CircularProgress
} from '@mui/material';
import {
  Visibility as ViewIcon,
  PlayArrow as StartIcon,
  Stop as StopIcon,
  Pause as PauseIcon,
  Refresh as RefreshIcon,
  Cancel as CancelIcon,
  PlaylistRemove as BulkCancelIcon
} from '@mui/icons-material';
import {
  DataGridPremium,
  GridColDef,
  GridRowParams,
  GridActionsCellItem,
  GridRowId,
  GridToolbar
} from '@mui/x-data-grid-premium';
import { useNavigate } from 'react-router-dom';
import toast from 'react-hot-toast';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowInstanceDto, InstanceStatus } from '@/types/workflow';
import { useTenant } from '@/contexts/TenantContext';

function useOptionalPermissions() {
  try {
    // eslint-disable-next-line @typescript-eslint/no-var-requires
    const mod = require('@/context/PermissionContext');
    if (mod?.usePermissions) {
      const p = mod.usePermissions();
      return { has: (perm: string) => p.has(perm) };
    }
  } catch { /* ignore */ }
  return { has: () => true };
}

export function InstancesListPage() {
  const [instances, setInstances] = useState<WorkflowInstanceDto[]>([]);
  const [loading, setLoading] = useState(true);

  // Terminate single
  const [terminateDialogOpen, setTerminateDialogOpen] = useState(false);
  const [instanceToTerminate, setInstanceToTerminate] = useState<WorkflowInstanceDto | null>(null);

  // Single cancel
  const [cancelDialogOpen, setCancelDialogOpen] = useState(false);
  const [instanceToCancel, setInstanceToCancel] = useState<WorkflowInstanceDto | null>(null);

  // Bulk cancel
  const [bulkCancelDialogOpen, setBulkCancelDialogOpen] = useState(false);
  const [bulkCancelReason, setBulkCancelReason] = useState('');
  const [bulkOpLoading, setBulkOpLoading] = useState(false);
  const [selectionIds, setSelectionIds] = useState<GridRowId[]>([]); // uncontrolled grid, we just track latest selection

  const navigate = useNavigate();
  const { currentTenant } = useTenant();
  const perms = useOptionalPermissions();

  useEffect(() => {
    if (currentTenant) loadInstances();
  }, [currentTenant]);

  const loadInstances = useCallback(async () => {
    try {
      setLoading(true);
      const items = await workflowService.getInstances();
      setInstances(items);
    } catch {
      toast.error('Failed to load workflow instances');
    } finally {
      setLoading(false);
    }
  }, []);

  const handleView = (id: GridRowId) => navigate(`/app/workflow/instances/${id}`);

  const handleSuspend = async (instance: WorkflowInstanceDto) => {
    try {
      await workflowService.suspendInstance(instance.id);
      toast.success(`Instance ${instance.id} suspended`);
      loadInstances();
    } catch {
      toast.error('Suspend failed');
    }
  };

  const handleResume = async (instance: WorkflowInstanceDto) => {
    try {
      await workflowService.resumeInstance(instance.id);
      toast.success(`Instance ${instance.id} resumed`);
      loadInstances();
    } catch {
      toast.error('Resume failed');
    }
  };

  const isCancellable = (i: WorkflowInstanceDto) =>
    i.status === 'Running' || i.status === 'Suspended';

  const handleCancel = (instance: WorkflowInstanceDto) => {
    setInstanceToCancel(instance);
    setCancelDialogOpen(true);
  };

  const handleCancelConfirm = async () => {
    if (!instanceToCancel) return;
    try {
      const res = await workflowService.bulkCancelInstances({
        instanceIds: [instanceToCancel.id],
        reason: 'User cancelled'
      } as any);
      const cancelledCount =
        (res as any)?.successCount ??
        (Array.isArray((res as any)?.succeededIds) ? (res as any).succeededIds.length : 1);
      toast.success(`Cancelled ${cancelledCount} instance`);
      loadInstances();
    } catch {
      toast.error('Cancel failed');
    } finally {
      setCancelDialogOpen(false);
      setInstanceToCancel(null);
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
      toast.success(`Instance ${instanceToTerminate.id} terminated`);
      loadInstances();
    } catch {
      toast.error('Terminate failed');
    } finally {
      setTerminateDialogOpen(false);
      setInstanceToTerminate(null);
    }
  };

  // Bulk cancel operations
  const selectedInstances = useMemo(
    () => instances.filter(i => selectionIds.includes(i.id)),
    [instances, selectionIds]
  );

  const cancellableSelected = useMemo(
    () => selectedInstances.filter(isCancellable),
    [selectedInstances]
  );

  const anySelected = selectionIds.length > 0;

  const openBulkCancel = () => {
    if (cancellableSelected.length === 0) {
      toast.error('No cancellable selected instances');
      return;
    }
    setBulkCancelReason('');
    setBulkCancelDialogOpen(true);
  };

  const executeBulkCancel = async () => {
    try {
      setBulkOpLoading(true);
      const ids = cancellableSelected.map(i => i.id);
      const res = await workflowService.bulkCancelInstances({
        instanceIds: ids,
        reason: bulkCancelReason || undefined
      } as any);
      const ok =
        (res as any)?.successCount ??
        (Array.isArray((res as any)?.succeededIds) ? (res as any).succeededIds.length : ids.length);
      toast.success(`Bulk cancelled ${ok}/${ids.length}`);
      setBulkCancelDialogOpen(false);
      setSelectionIds([]);
      loadInstances();
    } catch {
      toast.error('Bulk cancel failed');
    } finally {
      setBulkOpLoading(false);
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
        return <Chip label={status} size="small" />;
    }
  };

  const formatDateTime = (dt: string) => new Date(dt).toLocaleString();

  const calculateDuration = (startedAt: string, completedAt?: string) => {
    const start = new Date(startedAt);
    const end = completedAt ? new Date(completedAt) : new Date();
    const diffMs = end.getTime() - start.getTime();
    const hrs = Math.floor(diffMs / 3600000);
    const mins = Math.floor((diffMs % 3600000) / 60000);
    return hrs > 0 ? `${hrs}h ${mins}m` : `${mins}m`;
  };

  const columns: GridColDef[] = [
    { field: 'id', headerName: 'Instance ID', width: 120 },
    {
      field: 'workflowDefinitionName',
      headerName: 'Workflow',
      flex: 1,
      minWidth: 200,
      renderCell: (params) => {
        const i = params.row as WorkflowInstanceDto;
        return (
          <Box>
            <Typography variant="subtitle2" fontWeight="medium">{params.value}</Typography>
            <Typography variant="caption" color="text.secondary">v{i.definitionVersion}</Typography>
          </Box>
        );
      }
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      renderCell: (p) => getStatusChip(p.value as InstanceStatus)
    },
    {
      field: 'currentNodeIds',
      headerName: 'Current Node(s)',
      width: 170,
      renderCell: (p) => (
        <Typography
          variant="body2"
          sx={{ fontFamily: 'monospace', fontSize: '0.7rem', whiteSpace: 'nowrap', overflow: 'hidden', textOverflow: 'ellipsis' }}
          title={Array.isArray(p.value) ? (p.value as any[]).join(',') : p.value}
        >
          {Array.isArray(p.value) ? (p.value as any[]).join(',') : (p.value || 'None')}
        </Typography>
      )
    },
    {
      field: 'startedAt',
      headerName: 'Started',
      width: 170,
      type: 'dateTime',
      valueGetter: v => new Date(v as string),
      renderCell: (p) => formatDateTime(p.row.startedAt)
    },
    {
      field: 'duration',
      headerName: 'Duration',
      width: 110,
      valueGetter: (_, row) => calculateDuration(row.startedAt, row.completedAt),
      renderCell: (p) => <Typography variant="body2">{p.value}</Typography>
    },
    {
      field: 'startedByUserId',
      headerName: 'Started By',
      width: 120,
      renderCell: (p) => p.value || 'System'
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 160,
      getActions: (params: GridRowParams) => {
        const instance = params.row as WorkflowInstanceDto;
        const actions: any[] = [
          <GridActionsCellItem
            key="view"
            icon={<ViewIcon />}
            label="View Details"
            onClick={() => handleView(params.id)}
          />
        ];
        if (perms.has('workflow.admin')) {
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
          if (isCancellable(instance)) {
            actions.push(
              <GridActionsCellItem
                key="cancel"
                icon={<CancelIcon />}
                label="Cancel"
                onClick={() => handleCancel(instance)}
                showInMenu
              />
            );
          }
        }
        return actions;
      }
    }
  ];

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view workflow instances</Typography>
      </Box>
    );
  }

  const cancellableSelectedCount = cancellableSelected.length;
  const selectedSummary = cancellableSelectedCount > 0
    ? `${cancellableSelectedCount}/${selectionIds.length} cancellable`
    : `${selectionIds.length} selected`;

  const handleRowSelectionChange = (model: any) => {
    // New data-grid may pass either array or object; normalize:
    const ids = Array.isArray(model)
      ? model
      : Array.isArray(model?.ids)
        ? model.ids
        : [];
    setSelectionIds(ids as GridRowId[]);
  };

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: { xs: 'flex-start', sm: 'center' },
          mb: 3,
          gap: 2,
          flexWrap: 'wrap'
        }}
      >
        <Box>
          <Typography variant="h4" component="h1">Workflow Instances</Typography>
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Box>
        <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap">
          <Tooltip title="Refresh">
            <span>
              <Button
                variant="outlined"
                startIcon={<RefreshIcon />}
                onClick={loadInstances}
                disabled={loading}
              >
                Refresh
              </Button>
            </span>
          </Tooltip>
          {perms.has('workflow.admin') && (
            <Tooltip title={cancellableSelectedCount === 0 ? 'Select running or suspended instances' : 'Bulk cancel selected'}>
              <span>
                <Button
                  variant="contained"
                  color="warning"
                  startIcon={<BulkCancelIcon />}
                  disabled={cancellableSelectedCount === 0 || bulkOpLoading}
                  onClick={openBulkCancel}
                >
                  Bulk Cancel
                </Button>
              </span>
            </Tooltip>
          )}
          {anySelected && (
            <Chip
              size="small"
              color={cancellableSelectedCount > 0 ? 'primary' : 'default'}
              label={selectedSummary}
              variant="outlined"
            />
          )}
          {loading && <CircularProgress size={20} />}
        </Stack>
      </Box>

      <Box sx={{ flex: 1, minHeight: 0 }}>
        <DataGridPremium
          rows={instances}
          columns={columns}
          checkboxSelection
          disableRowSelectionOnClick
          loading={loading}
          pagination
          pageSizeOptions={[10, 25, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 25 } },
            sorting: { sortModel: [{ field: 'startedAt', sort: 'desc' }] }
          }}
          slots={{ toolbar: GridToolbar }}
          slotProps={{
            toolbar: { showQuickFilter: true, quickFilterProps: { debounceMs: 500 } }
          }}
          onRowSelectionModelChange={handleRowSelectionChange}
          sx={{
            '& .MuiDataGrid-row:hover': { backgroundColor: 'action.hover' }
          }}
        />
      </Box>

      {/* Terminate Confirmation */}
      <Dialog open={terminateDialogOpen} onClose={() => setTerminateDialogOpen(false)}>
        <DialogTitle>Confirm Terminate</DialogTitle>
        <DialogContent>
          <Typography>
            Terminate workflow instance "{instanceToTerminate?.id}"? This stops execution immediately.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setTerminateDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleTerminateConfirm} color="error" variant="contained">
            Terminate
          </Button>
        </DialogActions>
      </Dialog>

      {/* Single Cancel Confirmation */}
      <Dialog open={cancelDialogOpen} onClose={() => setCancelDialogOpen(false)}>
        <DialogTitle>Confirm Cancel</DialogTitle>
        <DialogContent>
          <Typography>
            Cancel workflow instance "{instanceToCancel?.id}"? Active tasks will be marked cancelled.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCancelDialogOpen(false)}>Dismiss</Button>
          <Button onClick={handleCancelConfirm} color="warning" variant="contained">
            Cancel Instance
          </Button>
        </DialogActions>
      </Dialog>

      {/* Bulk Cancel Dialog */}
      <Dialog open={bulkCancelDialogOpen} onClose={() => setBulkCancelDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Bulk Cancel Instances</DialogTitle>
        <DialogContent dividers>
          <Typography sx={{ mb: 2 }}>
            Cancelling {cancellableSelectedCount} instance{cancellableSelectedCount === 1 ? '' : 's'} (Running or Suspended).
          </Typography>
          <TextField
            fullWidth
            multiline
            minRows={2}
            label="Reason (optional)"
            value={bulkCancelReason}
            onChange={e => setBulkCancelReason(e.target.value)}
            placeholder="Reason for cancellation..."
          />
          <Box sx={{ mt: 2 }}>
            <Typography variant="caption" color="text.secondary">
              Non-cancellable (Completed / Cancelled / Failed) selections are ignored automatically.
            </Typography>
          </Box>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setBulkCancelDialogOpen(false)} disabled={bulkOpLoading}>Close</Button>
          <Button
            onClick={executeBulkCancel}
            color="warning"
            variant="contained"
            disabled={bulkOpLoading || cancellableSelectedCount === 0}
            startIcon={bulkOpLoading ? <CircularProgress size={16} /> : <BulkCancelIcon />}
          >
            {bulkOpLoading ? 'Cancelling...' : 'Confirm Bulk Cancel'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default InstancesListPage;
