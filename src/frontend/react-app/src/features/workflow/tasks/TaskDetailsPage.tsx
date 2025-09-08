import { useEffect, useState } from 'react';
import {
  Box,
  Typography,
  Chip,
  Button,
  Alert,
  IconButton,
  Tooltip,
  Divider,
  TextField,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  CircularProgress
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Refresh as RefreshIcon,
  AssignmentTurnedIn as CompleteIcon,
  AssignmentInd as ClaimIcon
} from '@mui/icons-material';
import { useNavigate, useParams } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowTaskDto, TaskStatus } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';

export function TaskDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const taskId = id ? parseInt(id, 10) : NaN;
  const [task, setTask] = useState<WorkflowTaskDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [claiming, setClaiming] = useState(false);
  const [completing, setCompleting] = useState(false);
  const [completeDialogOpen, setCompleteDialogOpen] = useState(false);
  const [completionData, setCompletionData] = useState('{"approved": true}');
  const [completionNotes, setCompletionNotes] = useState('');
  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  useEffect(() => {
    if (currentTenant && !isNaN(taskId)) {
      loadTask();
    }
  // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentTenant, taskId]);

  const loadTask = async () => {
    if (isNaN(taskId)) return;
    try {
      setLoading(true);
      const t = await workflowService.getTask(taskId);
      setTask(t);
    } catch {
      toast.error('Failed to load task');
    } finally {
      setLoading(false);
    }
  };

  const isClaimable = (s: TaskStatus) => s === 'Created' || s === 'Assigned';
  const isCompletable = (s: TaskStatus) => s === 'Claimed' || s === 'InProgress';

  const handleClaim = async () => {
    if (!task) return;
    try {
      setClaiming(true);
      const updated = await workflowService.claimTask(task.id, { claimNotes: 'Claimed from Task Details' });
      toast.success('Task claimed');
      setTask(updated);
    } catch {
      toast.error('Failed to claim task');
    } finally {
      setClaiming(false);
    }
  };

  const handleOpenComplete = () => {
    setCompletionData('{"approved": true}');
    setCompletionNotes('');
    setCompleteDialogOpen(true);
  };

  const handleComplete = async () => {
    if (!task) return;
    try {
      setCompleting(true);
      const updated = await workflowService.completeTask(task.id, {
        completionData,
        completionNotes: completionNotes || undefined
      });
      toast.success('Task completed');
      setTask(updated);
      setCompleteDialogOpen(false);
    } catch {
      toast.error('Failed to complete task');
    } finally {
      setCompleting(false);
    }
  };

  const statusChip = (s: TaskStatus) => {
    switch (s) {
      case 'Created': return <Chip label="Available" size="small" />;
      case 'Assigned': return <Chip label="Assigned" color="info" size="small" />;
      case 'Claimed': return <Chip label="Claimed" color="primary" size="small" />;
      case 'InProgress': return <Chip label="In Progress" color="warning" size="small" />;
      case 'Completed': return <Chip label="Completed" color="success" size="small" />;
      case 'Cancelled': return <Chip label="Cancelled" color="error" size="small" />;
      case 'Failed': return <Chip label="Failed" color="error" size="small" />;
      default: return <Chip label={s} size="small" />;
    }
  };

  if (!currentTenant) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="info">Please select a tenant.</Alert>
      </Box>
    );
  }

  if (loading) {
    return (
      <Box sx={{ p: 3, display: 'flex', gap: 2, alignItems: 'center' }}>
        <CircularProgress size={24} />
        <Typography>Loading task...</Typography>
      </Box>
    );
  }

  if (!task) {
    return (
      <Box sx={{ p: 3 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/app/workflow/tasks/mine')}
          sx={{ mb: 2 }}
        >
          Back to My Tasks
        </Button>
        <Alert severity="error">Task not found</Alert>
      </Box>
    );
  }

  const taskStatus = String(task.status) as any;

  return (
    <Box sx={{ p: 3, maxWidth: 900 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <IconButton onClick={() => navigate('/app/workflow/tasks/mine')} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h4">
            Task
            <Typography variant="subtitle2" color="text.secondary">
              ID {task.id} • Instance {task.workflowInstanceId}
            </Typography>
          </Typography>
        </Box>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <Tooltip title="Refresh">
            <IconButton onClick={loadTask}>
              <RefreshIcon />
            </IconButton>
          </Tooltip>
          {isClaimable(taskStatus) && (
            <Button
              startIcon={<ClaimIcon />}
              onClick={handleClaim}
              disabled={claiming}
              variant="outlined"
              size="small"
            >
              {claiming ? 'Claiming...' : 'Claim'}
            </Button>
          )}
          {isCompletable(taskStatus) && (
            <Button
              startIcon={<CompleteIcon />}
              onClick={handleOpenComplete}
              disabled={completing}
              variant="contained"
              size="small"
            >
              Complete
            </Button>
          )}
        </Box>
      </Box>

      <Box sx={{ mb: 3 }}>
        <Typography variant="h6" gutterBottom>Overview</Typography>
        <Divider sx={{ mb: 2 }} />
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 4 }}>
          <Box>
            <Typography variant="subtitle2" color="text.secondary">Status</Typography>
            {statusChip(taskStatus)}
          </Box>
          <Box>
            <Typography variant="subtitle2" color="text.secondary">Task Name</Typography>
            <Typography>{task.taskName}</Typography>
          </Box>
          <Box>
            <Typography variant="subtitle2" color="text.secondary">Node</Typography>
            <Typography>{task.nodeId || '—'}</Typography>
          </Box>
          <Box>
            <Typography variant="subtitle2" color="text.secondary">Due Date</Typography>
            <Typography>{task.dueDate ? new Date(task.dueDate).toLocaleString() : 'None'}</Typography>
          </Box>
          <Box>
            <Typography variant="subtitle2" color="text.secondary">Created</Typography>
            <Typography>{new Date(task.createdAt).toLocaleString()}</Typography>
          </Box>
          {task.completedAt && (
            <Box>
              <Typography variant="subtitle2" color="text.secondary">Completed</Typography>
              <Typography>{new Date(task.completedAt).toLocaleString()}</Typography>
            </Box>
          )}
        </Box>
      </Box>

      <Box sx={{ mb: 3 }}>
        <Typography variant="h6" gutterBottom>Data</Typography>
        <Divider sx={{ mb: 2 }} />
        <Box sx={{
          backgroundColor: 'grey.50',
          p: 2,
          borderRadius: 1,
          fontFamily: 'monospace',
          fontSize: '0.8rem',
          maxHeight: 260,
          overflow: 'auto'
        }}>
          <pre>{task.data ? JSON.stringify(JSON.parse(task.data), null, 2) : '{}'}</pre>
        </Box>
      </Box>

      {task.errorMessage && (
        <Alert severity="error" sx={{ mb: 3 }}>
          <Typography variant="subtitle2">Error</Typography>
          <Typography variant="body2">{task.errorMessage}</Typography>
        </Alert>
      )}

      <Dialog open={completeDialogOpen} onClose={() => setCompleteDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Complete Task</DialogTitle>
        <DialogContent>
          <Typography variant="body2" sx={{ mb: 2 }}>
            Completing: {task.taskName}
          </Typography>
          <TextField
            label="Completion Data (JSON)"
            fullWidth
            multiline
            rows={4}
            value={completionData}
            onChange={(e) => setCompletionData(e.target.value)}
            sx={{ mb: 2 }}
          />
          <TextField
            label="Completion Notes"
            fullWidth
            multiline
            rows={3}
            value={completionNotes}
            onChange={(e) => setCompletionNotes(e.target.value)}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCompleteDialogOpen(false)}>Cancel</Button>
          <Button
            onClick={handleComplete}
            variant="contained"
            disabled={completing}
          >
            {completing ? 'Completing...' : 'Complete Task'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default TaskDetailsPage;
