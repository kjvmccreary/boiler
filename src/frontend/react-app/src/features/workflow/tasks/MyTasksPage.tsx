import { useState, useEffect, useMemo } from 'react';
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
  Alert,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Tooltip,
} from '@mui/material';
import {
  Assignment as ClaimIcon,
  CheckCircle as CompleteIcon,
  Visibility as ViewIcon,
  Schedule as TimerIcon,
  Person as AssignedIcon,
  Refresh as RefreshIcon,
  AccountTree as WorkflowIcon
} from '@mui/icons-material';
import {
  DataGridPremium,
  GridColDef,
  GridRowParams,
  GridActionsCellItem,
  GridRowId,
  GridToolbar,
} from '@mui/x-data-grid-premium';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { TaskSummaryDto, TaskStatus } from '@/types/workflow';
import { TASK_STATUSES } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';

export function MyTasksPage() {
  const [tasks, setTasks] = useState<TaskSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState<TaskStatus | ''>('');
  const [overdueOnly, setOverdueOnly] = useState(false);
  const [completeDialogOpen, setCompleteDialogOpen] = useState(false);
  const [taskToComplete, setTaskToComplete] = useState<TaskSummaryDto | null>(null);
  const [completionData, setCompletionData] = useState('{"approved": true}');
  const [completionNotes, setCompletionNotes] = useState('');

  const navigate = useNavigate();
  const { currentTenant } = useTenant();
  const [searchParams, setSearchParams] = useSearchParams();

  // Parse query params (from bell navigation)
  useEffect(() => {
    const spStatus = searchParams.get('status');
    const spOverdue = searchParams.get('overdue');

    if (spStatus && TASK_STATUSES.includes(spStatus as TaskStatus)) {
      setStatusFilter(spStatus as TaskStatus);
    } else {
      setStatusFilter('');
    }

    setOverdueOnly(spOverdue === 'true');
  }, [searchParams]);

  useEffect(() => {
    if (currentTenant) {
      loadMyTasks();
    }
  }, [statusFilter, currentTenant]);

  const loadMyTasks = async () => {
    try {
      setLoading(true);
      const response = await workflowService.getMyTasks(statusFilter || undefined);
      setTasks(response);
    } catch {
      toast.error('Failed to load your tasks');
    } finally {
      setLoading(false);
    }
  };

  const handleStatusFilterChange = (event: any) => {
    const val = event.target.value as TaskStatus | '';
    setStatusFilter(val);
    const next = new URLSearchParams(searchParams);
    if (val) next.set('status', val); else next.delete('status');
    next.delete('overdue');
    setOverdueOnly(false);
    setSearchParams(next);
  };

  const handleViewTask = (id: GridRowId) => navigate(`/app/workflow/tasks/${id}`);
  const handleViewInstance = (task: TaskSummaryDto) => navigate(`/app/workflow/instances/${task.workflowInstanceId}`);

  const handleClaimTask = async (task: TaskSummaryDto) => {
    try {
      await workflowService.claimTask(task.id, { claimNotes: 'Claimed from My Tasks page' });
      toast.success('Task claimed successfully');
      loadMyTasks();
    } catch {
      toast.error('Failed to claim task');
    }
  };

  const handleCompleteClick = (task: TaskSummaryDto) => {
    setTaskToComplete(task);
    setCompleteDialogOpen(true);
    setCompletionData('{"approved": true}');
    setCompletionNotes('');
  };

  const handleCompleteConfirm = async () => {
    if (!taskToComplete) return;
    try {
      await workflowService.completeTask(taskToComplete.id, {
        completionData,
        completionNotes: completionNotes || undefined
      });
      toast.success('Task completed successfully');
      loadMyTasks();
    } catch {
      toast.error('Failed to complete task');
    } finally {
      setCompleteDialogOpen(false);
      setTaskToComplete(null);
      setCompletionData('{"approved": true}');
      setCompletionNotes('');
    }
  };

  const getStatusChip = (status: TaskStatus) => {
    switch (status) {
      case 'Created': return <Chip label="Available" size="small" />;
      case 'Assigned': return <Chip label="Assigned" color="info" size="small" icon={<AssignedIcon />} />;
      case 'Claimed': return <Chip label="Claimed" color="primary" size="small" />;
      case 'InProgress': return <Chip label="In Progress" color="warning" size="small" />;
      case 'Completed': return <Chip label="Completed" color="success" size="small" icon={<CompleteIcon />} />;
      case 'Cancelled': return <Chip label="Cancelled" color="error" size="small" />;
      case 'Failed': return <Chip label="Failed" color="error" size="small" />;
      default: return <Chip label={status} size="small" />;
    }
  };

  const isOverdue = (dueDate?: string) => !!dueDate && new Date(dueDate) < new Date();

  const filteredByNodeType = useMemo(
    () => tasks.filter(t => (t as any).nodeType === 'human' || (t as any).nodeType === undefined),
    [tasks]
  );

  const finalTasks = useMemo(() => {
    if (!overdueOnly) return filteredByNodeType;
    return filteredByNodeType.filter(
      t => isOverdue(t.dueDate) && t.status !== 'Completed' && t.status !== 'Cancelled'
    );
  }, [filteredByNodeType, overdueOnly]);

  const columns: GridColDef[] = [
    {
      field: 'taskName',
      headerName: 'Task',
      flex: 1,
      minWidth: 200,
      renderCell: (params) => {
        const task = params.row as TaskSummaryDto & { nodeType?: string };
        const overdue = isOverdue(task.dueDate);
        return (
          <Box>
            <Typography variant="subtitle2" fontWeight="medium">{params.value}</Typography>
            <Typography variant="caption" color="text.secondary">ID: {task.id}</Typography>
            {task.nodeType && task.nodeType !== 'human' && (
              <Chip size="small" label={task.nodeType} color="secondary" variant="outlined"
                sx={{ ml: 1, mt: 0.5, height: 18, fontSize: '0.6rem' }} />
            )}
            {overdue && (
              <Box sx={{ display: 'flex', alignItems: 'center', mt: 0.5 }}>
                <TimerIcon color="error" fontSize="small" sx={{ mr: 0.5 }} />
                <Typography variant="caption" color="error">Overdue</Typography>
              </Box>
            )}
          </Box>
        );
      }
    },
    {
      field: 'workflowDefinitionName',
      headerName: 'Workflow',
      flex: 1,
      minWidth: 180,
      renderCell: (params) => {
        const task = params.row as TaskSummaryDto;
        return (
          <Box>
            <Typography variant="body2">{params.value}</Typography>
            <Typography variant="caption" color="text.secondary">Instance: {task.workflowInstanceId}</Typography>
          </Box>
        );
      }
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      renderCell: (params) => getStatusChip(params.value as TaskStatus)
    },
    {
      field: 'dueDate',
      headerName: 'Due Date',
      width: 160,
      type: 'dateTime',
      valueGetter: v => v ? new Date(v as string) : null,
      renderCell: (params) => {
        if (!params.value) return <Typography variant="body2" color="text.secondary">No due date</Typography>;
        const task = params.row as TaskSummaryDto;
        const overdue = isOverdue(task.dueDate);
        return (
          <Typography variant="body2" color={overdue ? 'error' : 'inherit'}>
            {new Date(params.value as Date).toLocaleString()}
          </Typography>
        );
      }
    },
    {
      field: 'createdAt',
      headerName: 'Created',
      width: 120,
      type: 'date',
      valueGetter: v => new Date(v as string)
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      getActions: (params: GridRowParams) => {
        const task = params.row as TaskSummaryDto & { nodeType?: string };
        const isHuman = !task.nodeType || task.nodeType === 'human';
        const acts: any[] = [
          <GridActionsCellItem icon={<ViewIcon />} label="View Task" onClick={() => handleViewTask(params.id)} />,
          <GridActionsCellItem icon={<WorkflowIcon />} label="View Instance" onClick={() => handleViewInstance(task)} showInMenu />
        ];
        if (isHuman && (task.status === 'Created' || task.status === 'Assigned')) {
          acts.push(
            <GridActionsCellItem
              icon={<ClaimIcon />}
              label="Claim Task"
              onClick={() => handleClaimTask(task)}
              showInMenu
            />
          );
        }
        if (isHuman && (task.status === 'Claimed' || task.status === 'InProgress')) {
          acts.push(
            <GridActionsCellItem
              icon={<CompleteIcon />}
              label="Complete Task"
              onClick={() => handleCompleteClick(task)}
              showInMenu
            />
          );
        }
        return acts;
      }
    }
  ];

  const overdueTasks = finalTasks.filter(t => isOverdue(t.dueDate));

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view your tasks</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          My Tasks
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Typography>
        <Box sx={{ display: 'flex', gap: 2, alignItems: 'center' }}>
          <FormControl sx={{ minWidth: 150 }}>
            <InputLabel>Status Filter</InputLabel>
            <Select
              value={statusFilter}
              onChange={handleStatusFilterChange}
              label="Status Filter"
              size="small"
            >
              <MenuItem value="">All Tasks</MenuItem>
              <MenuItem value="Created">Available</MenuItem>
              <MenuItem value="Assigned">Assigned</MenuItem>
              <MenuItem value="Claimed">Claimed</MenuItem>
              <MenuItem value="InProgress">In Progress</MenuItem>
              <MenuItem value="Completed">Completed</MenuItem>
              <MenuItem value="Failed">Failed</MenuItem>
            </Select>
          </FormControl>
          {overdueOnly && (
            <Tooltip title="Showing only overdue tasks">
              <Chip size="small" color="error" variant="outlined" label="Overdue" />
            </Tooltip>
          )}
          <Button
            variant="outlined"
            startIcon={<RefreshIcon />}
            onClick={loadMyTasks}
            disabled={loading}
          >
            Refresh
          </Button>
        </Box>
      </Box>
      {overdueTasks.length > 0 && !overdueOnly && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          You have {overdueTasks.length} overdue task{overdueTasks.length === 1 ? '' : 's'}
        </Alert>
      )}
      <Box sx={{ flex: 1, minHeight: 0 }}>
        <DataGridPremium
          rows={finalTasks}
          columns={columns}
          loading={loading}
          pagination
          pageSizeOptions={[10, 25, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 25 } },
            sorting: { sortModel: [{ field: 'dueDate', sort: 'asc' }] }
          }}
          slots={{ toolbar: GridToolbar }}
          slotProps={{
            toolbar: { showQuickFilter: true, quickFilterProps: { debounceMs: 500 } }
          }}
          disableRowSelectionOnClick
          getRowClassName={(params) => {
            const task = params.row as TaskSummaryDto;
            return isOverdue(task.dueDate) ? 'row-overdue' : '';
          }}
          sx={{
            '& .MuiDataGrid-row:hover': { backgroundColor: 'action.hover' },
            '& .row-overdue': {
              backgroundColor: 'error.50',
              '&:hover': { backgroundColor: 'error.100' }
            },
            '& .MuiDataGrid-cell': {
              borderBottom: '1px solid',
              borderColor: 'divider'
            }
          }}
        />
      </Box>
      <Dialog open={completeDialogOpen} onClose={() => setCompleteDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Complete Task</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            Complete task "{taskToComplete?.taskName}"
          </Typography>
          <TextField
            fullWidth
            label="Completion Data (JSON)"
            multiline
            rows={4}
            value={completionData}
            onChange={(e) => setCompletionData(e.target.value)}
            placeholder='{"approved": true, "comments": "Looks good"}'
            sx={{ mb: 2 }}
            helperText="Enter task completion data as JSON"
          />
          <TextField
            fullWidth
            label="Completion Notes (Optional)"
            multiline
            rows={3}
            value={completionNotes}
            onChange={(e) => setCompletionNotes(e.target.value)}
            placeholder="Add notes about task completion..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCompleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleCompleteConfirm} color="primary" variant="contained">
            Complete Task
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default MyTasksPage;
