import React from 'react';
import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Button,
  Typography,
  Box,
  List,
  ListItem,
  ListItemText,
  Chip,
  Alert,
  Divider,
  Paper,
} from '@mui/material';
import {
  Warning as WarningIcon,
  AccountTree as WorkflowIcon,
  Circle as NodeIcon,
} from '@mui/icons-material';
import type { RoleUsageInWorkflowsDto } from '@/types/workflow';

interface RoleWorkflowUsageDialogProps {
  open: boolean;
  onClose: () => void;
  onProceed: () => void;
  usageInfo: RoleUsageInWorkflowsDto | null;
  actionType: 'rename' | 'delete';
  newRoleName?: string;
}

export function RoleWorkflowUsageDialog({
  open,
  onClose,
  onProceed,
  usageInfo,
  actionType,
  newRoleName
}: RoleWorkflowUsageDialogProps) {
  if (!usageInfo || !usageInfo.isUsedInWorkflows) {
    return null;
  }

  const publishedDefinitions = usageInfo.usedInDefinitions.filter(d => d.isPublished);
  const draftDefinitions = usageInfo.usedInDefinitions.filter(d => !d.isPublished);

  return (
    <Dialog 
      open={open} 
      onClose={onClose} 
      maxWidth="md" 
      fullWidth
      PaperProps={{
        sx: { minHeight: '400px' }
      }}
    >
      <DialogTitle sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
        <WarningIcon color="warning" />
        <Typography variant="h6">
          Role Used in Workflow Definitions
        </Typography>
      </DialogTitle>

      <DialogContent>
        <Alert severity="warning" sx={{ mb: 2 }}>
          <Typography variant="body1" gutterBottom>
            <strong>Warning:</strong> The role "<strong>{usageInfo.roleName}</strong>" is currently being used in {usageInfo.usedInDefinitions.length} workflow definition(s).
          </Typography>
          
          {actionType === 'rename' && newRoleName && (
            <Typography variant="body2" sx={{ mt: 1 }}>
              Renaming this role to "<strong>{newRoleName}</strong>" will cause these workflow definitions to reference a non-existent role, potentially breaking task assignments.
            </Typography>
          )}
          
          {actionType === 'delete' && (
            <Typography variant="body2" sx={{ mt: 1 }}>
              Deleting this role will cause these workflow definitions to reference a non-existent role, potentially breaking task assignments.
            </Typography>
          )}
        </Alert>

        <Box sx={{ mb: 2 }}>
          <Typography variant="subtitle1" gutterBottom sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <WorkflowIcon />
            Affected Workflow Definitions ({usageInfo.usedInDefinitions.length})
          </Typography>
          
          <Paper variant="outlined" sx={{ maxHeight: 300, overflow: 'auto', p: 1 }}>
            <List dense>
              {usageInfo.usedInDefinitions.map((definition, index) => (
                <React.Fragment key={definition.definitionId}>
                  <ListItem>
                    <ListItemText
                      primary={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="subtitle2">
                            {definition.definitionName}
                          </Typography>
                          <Chip 
                            label={definition.isPublished ? 'Published' : 'Draft'}
                            color={definition.isPublished ? 'success' : 'warning'}
                            size="small"
                          />
                          <Chip 
                            label={`v${definition.version}`}
                            variant="outlined"
                            size="small"
                          />
                        </Box>
                      }
                      secondary={
                        <Box sx={{ mt: 1 }}>
                          <Typography variant="caption" color="text.secondary">
                            Used in {definition.usageCount} node(s):
                          </Typography>
                          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mt: 0.5 }}>
                            {definition.usedInNodes.map((node) => (
                              <Chip
                                key={node.nodeId}
                                icon={<NodeIcon />}
                                label={`${node.nodeName} (${node.nodeType})`}
                                size="small"
                                variant="outlined"
                                sx={{ fontSize: '0.7rem', height: '20px' }}
                              />
                            ))}
                          </Box>
                        </Box>
                      }
                    />
                  </ListItem>
                  {index < usageInfo.usedInDefinitions.length - 1 && <Divider />}
                </React.Fragment>
              ))}
            </List>
          </Paper>
        </Box>

        {publishedDefinitions.length > 0 && (
          <Alert severity="error" sx={{ mb: 2 }}>
            <Typography variant="body2">
              <strong>Critical:</strong> {publishedDefinitions.length} of these workflow definition(s) are <strong>published</strong> and may have active instances or tasks that depend on this role.
            </Typography>
          </Alert>
        )}

        <Typography variant="body2" color="text.secondary">
          <strong>Recommendations:</strong>
          <ul style={{ marginTop: '8px', paddingLeft: '20px' }}>
            <li>Review each affected workflow definition</li>
            <li>Update Human Task nodes to use a different role</li>
            <li>Consider creating a new role instead of renaming/deleting this one</li>
            {publishedDefinitions.length > 0 && (
              <li><strong>Published workflows:</strong> Create new versions with updated role assignments</li>
            )}
          </ul>
        </Typography>
      </DialogContent>

      <DialogActions>
        <Button onClick={onClose}>
          Cancel
        </Button>
        <Button 
          onClick={onProceed} 
          color="warning" 
          variant="contained"
          startIcon={<WarningIcon />}
        >
          {actionType === 'rename' ? 'Rename Anyway' : 'Delete Anyway'}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
