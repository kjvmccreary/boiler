import React, { useState } from 'react';
import {
  Drawer,
  Box,
  Typography,
  IconButton,
  Stack,
  Button,
  Chip,
  Divider,
  Tooltip
} from '@mui/material';
import CloseIcon from '@mui/icons-material/Close';
import RestartAltIcon from '@mui/icons-material/RestartAlt';
import CancelIcon from '@mui/icons-material/Cancel';
import AssignmentIndIcon from '@mui/icons-material/AssignmentInd';
import DoneIcon from '@mui/icons-material/Done';
import LoginIcon from '@mui/icons-material/Login';
import type { WorkflowTaskDto } from '@/types/workflow';
import { useTaskActions } from './hooks/useTaskActions';
import { TaskAssignDialog } from './TaskAssignDialog';

export interface TaskDetailDrawerProps {
  open: boolean;
  task: WorkflowTaskDto | undefined;
  onClose: () => void;
  onTaskUpdate: (t: WorkflowTaskDto) => void;
}

export const TaskDetailDrawer: React.FC<TaskDetailDrawerProps> = ({
  open,
  task,
  onClose,
  onTaskUpdate
}) => {
  const [assignOpen, setAssignOpen] = useState(false);
  if (!task) {
    return <Drawer anchor="right" open={open} onClose={onClose}><Box sx={{ width: 360 }} /></Drawer>;
  }

  const {
    task: live,
    loading,
    actions,
    claim,
    complete,
    assign,
    cancel,
    reset
  } = useTaskActions(task, onTaskUpdate);

  return (
    <>
      <Drawer anchor="right" open={open} onClose={onClose}>
        <Box sx={{ width: 380, p: 2, display: 'flex', flexDirection: 'column', gap: 2 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
            <Typography variant="h6" fontSize={16} fontWeight={600}>
              Task Details
            </Typography>
            <IconButton size="small" onClick={onClose}>
              <CloseIcon fontSize="small" />
            </IconButton>
          </Box>

            <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap">
              <Chip
                label={live.status}
                size="small"
                color={
                  live.status === 'Completed' ? 'success'
                    : live.status === 'Failed' ? 'error'
                      : live.status === 'Cancelled' ? 'default'
                        : 'primary'
                }
              />
              {live.assignedToUserId && <Chip label={`User ${live.assignedToUserId}`} size="small" variant="outlined" />}
              {live.assignedToRole && <Chip label={`Role ${live.assignedToRole}`} size="small" variant="outlined" />}
            </Stack>

          <Divider />

          <Typography variant="subtitle2">Actions</Typography>
          <Stack spacing={1} direction="column">
            <Stack direction="row" spacing={1} flexWrap="wrap">
              <Tooltip title="Claim (assign to self)">
                <span>
                  <Button
                    size="small"
                    variant="outlined"
                    startIcon={<LoginIcon fontSize="small" />}
                    disabled={!actions.canClaim || loading}
                    onClick={() => claim('Claimed via drawer')}
                  >Claim</Button>
                </span>
              </Tooltip>
              <Tooltip title="Assign to user or role">
                <span>
                  <Button
                    size="small"
                    variant="outlined"
                    startIcon={<AssignmentIndIcon fontSize="small" />}
                    disabled={!actions.canAssign || loading}
                    onClick={() => setAssignOpen(true)}
                  >Assign</Button>
                </span>
              </Tooltip>
              <Tooltip title="Cancel task">
                <span>
                  <Button
                    size="small"
                    color="warning"
                    variant="outlined"
                    startIcon={<CancelIcon fontSize="small" />}
                    disabled={!actions.canCancel || loading}
                    onClick={() => cancel()}
                  >Cancel</Button>
                </span>
              </Tooltip>
              <Tooltip title="Reset (admin)">
                <span>
                  <Button
                    size="small"
                    color="secondary"
                    variant="outlined"
                    startIcon={<RestartAltIcon fontSize="small" />}
                    disabled={!actions.canReset || loading}
                    onClick={() => reset('Admin reset')}
                  >Reset</Button>
                </span>
              </Tooltip>
              <Tooltip title="Complete task">
                <span>
                  <Button
                    size="small"
                    color="success"
                    variant="contained"
                    startIcon={<DoneIcon fontSize="small" />}
                    disabled={!actions.canComplete || loading}
                    onClick={() => complete()}
                  >Complete</Button>
                </span>
              </Tooltip>
            </Stack>
          </Stack>

          <Divider />

          <Typography variant="subtitle2">Metadata</Typography>
          <Typography variant="body2" color="text.secondary">
            Node: {live.nodeId} • Instance: {live.workflowInstanceId}
          </Typography>
          {live.dueDate && (
            <Typography variant="body2" color="text.secondary">
              Due: {new Date(live.dueDate).toLocaleString()}
            </Typography>
          )}
        </Box>
      </Drawer>

      <TaskAssignDialog
        open={assignOpen}
        onClose={() => setAssignOpen(false)}
        onSubmit={async (userId, role) => {
          await assign(userId, role);
        }}
      />
    </>
  );
};

export default TaskDetailDrawer;
