import React, { useState } from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  TextField,
  Stack,
  Typography
} from '@mui/material';

interface TaskAssignDialogProps {
  open: boolean;
  onClose: () => void;
  onSubmit: (userId?: number, role?: string) => Promise<void> | void;
}

export const TaskAssignDialog: React.FC<TaskAssignDialogProps> = ({
  open,
  onClose,
  onSubmit
}) => {
  const [userId, setUserId] = useState<string>('');
  const [role, setRole] = useState<string>('');
  const [submitting, setSubmitting] = useState(false);

  const handleSubmit = async () => {
    setSubmitting(true);
    try {
      await onSubmit(
        userId.trim() ? Number(userId) : undefined,
        role.trim() || undefined
      );
      onClose();
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Dialog open={open} onClose={submitting ? undefined : onClose} fullWidth maxWidth="sm">
      <DialogTitle>Assign Task</DialogTitle>
      <DialogContent>
        <Stack spacing={2} sx={{ mt: 1 }}>
          <Typography variant="body2" color="text.secondary">
            Provide either a user ID or a role (role takes precedence if both given). Leave blank to clear assignment.
          </Typography>
          <TextField
            label="User ID"
            type="number"
            size="small"
            value={userId}
            disabled={submitting}
            onChange={e => setUserId(e.target.value)}
          />
            <TextField
            label="Role"
            size="small"
            value={role}
            disabled={submitting}
            onChange={e => setRole(e.target.value)}
          />
        </Stack>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} disabled={submitting}>Cancel</Button>
        <Button onClick={handleSubmit} variant="contained" disabled={submitting}>
          {submitting ? 'Assigning…' : 'Assign'}
        </Button>
      </DialogActions>
    </Dialog>
  );
};
