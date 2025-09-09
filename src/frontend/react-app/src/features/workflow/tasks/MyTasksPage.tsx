// Integrated TaskDetailDrawer + fetch full task details for extended actions
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
  Switch,
  FormControlLabel,
  IconButton,
  CircularProgress
} from '@mui/material';
import {
  Assignment as ClaimIcon,
  CheckCircle as CompleteIcon,
  Visibility as ViewIcon,
  Schedule as TimerIcon,
  Person as AssignedIcon,
  Refresh as RefreshIcon,
  AccountTree as WorkflowIcon,
  Tune as DetailIcon
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
import type { TaskSummaryDto, TaskStatus, WorkflowTaskDto } from '@/types/workflow';
import { TASK_STATUSES } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import TaskDetailDrawer from './TaskDetailDrawer';

const LS_KEY_HIDE_COMPLETED_TASKS = 'wf.tasks.hideCompleted';

function readStoredHideCompleted(defaultValue: boolean): boolean {
  if (typeof window === 'undefined') return defaultValue;
  const raw = window.localStorage.getItem(LS_KEY_HIDE_COMPLETED_TASKS);
  if (raw === null) return defaultValue;
  return raw === 'true';
}

export function MyTasksPage() {
  const [tasks, setTasks] = useState<TaskSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState<TaskStatus | ''>('');
  const [overdueOnly, setOverdueOnly] = useState(false);
  const [hideCompleted, setHideCompleted] = useState<boolean>(() => readStoredHideCompleted(true));
  const [completeDialogOpen, setCompleteDialogOpen] = useState(false);
  const [taskToComplete, setTaskToComplete] = useState<TaskSummaryDto | null>(null);
  const [completionData, setCompletionData] = useState('{"approved": true}');
  const [completionNotes, setCompletionNotes] = useState('');

  // Extended drawer state
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [selectedFullTask, setSelectedFullTask] = useState<WorkflowTaskDto | undefined>();
  const [fetchingFull, setFetchingFull] = useState(false);

  const navigate = useNavigate();
  const { currentTenant } = useTenant();
  const [searchParams, setSearchParams] = useSearchParams();

  // Persist hideCompleted
  useEffect(() => {
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(LS_KEY_HIDE_COMPLETED_TASKS, String(hideCompleted));
    }
  }, [hideCompleted]);

  // Parse query params (for existing filters)
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
      toast.success('Task claimed');
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
      toast.success('Task completed');
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

  const openDrawerForTask = async (taskId: number) => {
    setFetchingFull(true);
    try {
      const full = await workflowService.getTask(taskId);
      setSelectedFullTask(full);
      setDrawerOpen(true);
    } catch {
      toast.error('Failed to load task details');
    } finally {
      setFetchingFull(false);
    }
  };

  const handleDrawerTaskUpdate = (updated: WorkflowTaskDto) => {
    // update list if status or assignment changed
    setTasks(prev => prev.map(t => t.id === updated.id
      ? { ...t, status: updated.status as any, assignedToUserId: updated.assignedToUserId, assignedToRole: updated.assignedToRole }
      : t));
    setSelectedFullTask(updated);
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

  const humanOrUndefined = useMemo(
    () => tasks.filter(t => (t as any).nodeType === 'human' || (t as any).nodeType === undefined),
    [tasks]
  );

  const preFiltered = useMemo(
    () => hideCompleted ? humanOrUndefined.filter(t => t.status !== 'Completed') : humanOrUndefined,
    [humanOrUndefined, hideCompleted]
  );

  const finalTasks = useMemo(() => {
    if (!overdueOnly) return preFiltered;
    return preFiltered.filter(
      t => isOverdue(t.dueDate) && t.status !== 'Cancelled' && t.status !== 'Failed'
    );
  }, [preFiltered, overdueOnly]);

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
            <Typography
              variant="subtitle2"
              fontWeight="medium"
              sx={{ cursor: 'pointer' }}
              onClick={() => openDrawerForTask(task.id)}
            >
              {params.value}
            </Typography>
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
      width: 140,
      getActions: (params: GridRowParams) => {
        const task = params.row as TaskSummaryDto & { nodeType?: string };
        const isHuman = !task.nodeType || task.nodeType === 'human';
        const acts: any[] = [
          <GridActionsCellItem icon={<ViewIcon />} label="View Route" onClick={() => handleViewTask(params.id)} />,
          <GridActionsCellItem icon={<WorkflowIcon />} label="View Instance" onClick={() => handleViewInstance(task)} showInMenu />,
          <GridActionsCellItem icon={<DetailIcon />} label="Details" onClick={() => openDrawerForTask(task.id)} showInMenu />
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

  const hiddenCompleted = hideCompleted ? tasks.filter(t => t.status === 'Completed').length : 0;

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3, flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Typography variant="h4" component="h1">
            My Tasks
          </Typography>
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Box>
        <Box sx={{ display: 'flex', gap: 2, alignItems: 'center', flexWrap: 'wrap' }}>
          <FormControl sx={{ minWidth: 150 }} size="small">
            <InputLabel>Status Filter</InputLabel>
            <Select
              value={statusFilter}
              onChange={handleStatusFilterChange}
              label="Status Filter"
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
          <FormControlLabel
            control={
              <Switch
                checked={hideCompleted}
                onChange={(e) => setHideCompleted(e.target.checked)}
                color="primary"
              />
            }
            label="Hide Completed"
          />
          {hideCompleted && hiddenCompleted > 0 && (
            <Tooltip title={`${hiddenCompleted} completed hidden`}>
              <Chip size="small" label={`Hidden: ${hiddenCompleted}`} />
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
          {fetchingFull && <IconButton disabled><CircularProgress size={18} /></IconButton>}
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

      <TaskDetailDrawer
        open={drawerOpen}
        task={selectedFullTask}
        onClose={() => setDrawerOpen(false)}
        onTaskUpdate={handleDrawerTaskUpdate}
      />
    </Box>
  );
}

export default MyTasksPage;
