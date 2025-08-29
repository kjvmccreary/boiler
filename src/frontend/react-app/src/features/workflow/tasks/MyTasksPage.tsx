import { useState, useEffect } from 'react';
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
  Assignment as ClaimIcon,
  CheckCircle as CompleteIcon,
  Visibility as ViewIcon,
  Schedule as TimerIcon,
  Person as AssignedIcon,
  Refresh as RefreshIcon,
  AccountTree as WorkflowIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { TaskSummaryDto, TaskStatus } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';

export function MyTasksPage() {
  const [tasks, setTasks] = useState<TaskSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [statusFilter, setStatusFilter] = useState<TaskStatus | ''>('');
  const [completeDialogOpen, setCompleteDialogOpen] = useState(false);
  const [taskToComplete, setTaskToComplete] = useState<TaskSummaryDto | null>(null);
  const [completionData, setCompletionData] = useState('{"approved": true}');
  const [completionNotes, setCompletionNotes] = useState('');

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

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
    } catch (error) {
      toast.error('Failed to load your tasks');
    } finally {
      setLoading(false);
    }
  };

  const handleStatusFilterChange = (event: any) => {
    setStatusFilter(event.target.value);
  };

  // ✅ FIX: Correct route base to include /app
  const handleViewTask = (id: GridRowId) => {
    navigate(`/app/workflow/tasks/${id}`);
  };

  // ✅ FIX: Correct route base to include /app
  const handleViewInstance = (task: TaskSummaryDto) => {
    navigate(`/app/workflow/instances/${task.workflowInstanceId}`);
  };

  const handleClaimTask = async (task: TaskSummaryDto) => {
    try {
      await workflowService.claimTask(task.id, {
        claimNotes: 'Claimed from My Tasks page'
      });
      toast.success('Task claimed successfully');
      loadMyTasks();
    } catch (error) {
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
    } catch (error) {
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
      case 'Created':
        return <Chip label="Available" color="default" size="small" />;
      case 'Assigned':
        return <Chip label="Assigned" color="info" size="small" icon={<AssignedIcon />} />;
      case 'Claimed':
        return <Chip label="Claimed" color="primary" size="small" />;
      case 'InProgress':
        return <Chip label="In Progress" color="warning" size="small" />;
      case 'Completed':
        return <Chip label="Completed" color="success" size="small" icon={<CompleteIcon />} />;
      case 'Cancelled':
        return <Chip label="Cancelled" color="error" size="small" />;
      case 'Failed':
        return <Chip label="Failed" color="error" size="small" />;
      default:
        return <Chip label={status} color="default" size="small" />;
    }
  };

  const isOverdue = (dueDate?: string) => {
    if (!dueDate) return false;
    return new Date(dueDate) < new Date();
  };

  const columns: GridColDef[] = [
    {
      field: 'taskName',
      headerName: 'Task',
      flex: 1,
      minWidth: 200,
      renderCell: (params) => {
        const task = params.row as TaskSummaryDto;
        const overdue = isOverdue(task.dueDate);
        return (
          <Box>
            <Typography variant="subtitle2" fontWeight="medium">
              {params.value}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              ID: {task.id}
            </Typography>
            {overdue && (
              <Box sx={{ display: 'flex', alignItems: 'center', mt: 0.5 }}>
                <TimerIcon color="error" fontSize="small" sx={{ mr: 0.5 }} />
                <Typography variant="caption" color="error">
                  Overdue
                </Typography>
              </Box>
            )}
          </Box>
        );
      },
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
            <Typography variant="body2">
              {params.value}
            </Typography>
            <Typography variant="caption" color="text.secondary">
              Instance: {task.workflowInstanceId}
            </Typography>
          </Box>
        );
      },
    },
    {
      field: 'status',
      headerName: 'Status',
      width: 130,
      renderCell: (params) => getStatusChip(params.value as TaskStatus),
    },
    {
      field: 'dueDate',
      headerName: 'Due Date',
      width: 160,
      type: 'dateTime',
      valueGetter: (value) => value ? new Date(value) : null,
      renderCell: (params) => {
        if (!params.value) {
          return <Typography variant="body2" color="text.secondary">No due date</Typography>;
        }
        const task = params.row as TaskSummaryDto;
        const overdue = isOverdue(task.dueDate);
        return (
          <Typography variant="body2" color={overdue ? 'error' : 'inherit'}>
            {new Date(params.value).toLocaleString()}
          </Typography>
        );
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
      width: 120,
      getActions: (params: GridRowParams) => {
        const task = params.row as TaskSummaryDto;
        const actions = [
          <GridActionsCellItem
            icon={<ViewIcon />}
            label="View Task"
            onClick={() => handleViewTask(params.id)}
          />,
          <GridActionsCellItem
            icon={<WorkflowIcon />}
            label="View Instance"
            onClick={() => handleViewInstance(task)}
            showInMenu
          />
        ];

        if (task.status === 'Created' || task.status === 'Assigned') {
            actions.push(
              <GridActionsCellItem
                icon={<ClaimIcon />}
                label="Claim Task"
                onClick={() => handleClaimTask(task)}
                showInMenu
              />
            );
        }

        if (task.status === 'Claimed' || task.status === 'InProgress') {
          actions.push(
            <GridActionsCellItem
              icon={<CompleteIcon />}
              label="Complete Task"
              onClick={() => handleCompleteClick(task)}
              showInMenu
            />
          );
        }

        return actions;
      },
    },
  ];

  const overdueTasks = tasks.filter(task => isOverdue(task.dueDate));

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
            </Select>
          </FormControl>

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

      {overdueTasks.length > 0 && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          You have {overdueTasks.length} overdue task{overdueTasks.length === 1 ? '' : 's'}
        </Alert>
      )}

      <Box sx={{ flex: 1, minHeight: 0 }}>
        <DataGridPremium
          rows={tasks}
          columns={columns}
          loading={loading}
          pagination
          pageSizeOptions={[10, 25, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 25 } },
            sorting: { sortModel: [{ field: 'dueDate', sort: 'asc' }] },
          }}
          slots={{ toolbar: GridToolbar }}
          slotProps={{
            toolbar: {
              showQuickFilter: true,
              quickFilterProps: { debounceMs: 500 },
            },
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
              '&:hover': { backgroundColor: 'error.100' },
            },
            '& .MuiDataGrid-cell': {
              borderBottom: '1px solid',
              borderColor: 'divider',
            },
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
