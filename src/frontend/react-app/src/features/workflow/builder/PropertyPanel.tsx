import { useState, useEffect } from 'react';
import {
  Drawer,
  Box,
  Typography,
  TextField,
  Button,
  Divider,
  IconButton,
  FormControl,
  InputLabel,
  Select,
  MenuItem,
  Chip,
  Switch,
  FormControlLabel,
  Alert,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import {
  Close as CloseIcon,
  Add as AddIcon,
  Remove as RemoveIcon,
  ExpandMore as ExpandMoreIcon,
} from '@mui/icons-material';
import type { DslNode } from '../dsl/dsl.types';
import { ConditionBuilder } from './components/ConditionBuilder';

interface PropertyPanelProps {
  open: boolean;
  onClose: () => void;
  selectedNode: DslNode | null;
  onNodeUpdate: (node: DslNode) => void;
  workflowName: string;
  workflowDescription: string;
  onWorkflowNameChange: (name: string) => void;
  onWorkflowDescriptionChange: (description: string) => void;
}

export function PropertyPanel({
  open,
  onClose,
  selectedNode,
  onNodeUpdate,
  workflowName,
  workflowDescription,
  onWorkflowNameChange,
  onWorkflowDescriptionChange,
}: PropertyPanelProps) {
  const [localNode, setLocalNode] = useState<DslNode | null>(null);

  useEffect(() => {
    setLocalNode(selectedNode);
  }, [selectedNode]);

  const handleNodeChange = (field: string, value: any) => {
    if (!localNode) return;

    const updatedNode = { ...localNode, [field]: value };
    setLocalNode(updatedNode);
    onNodeUpdate(updatedNode);
  };

  const renderNodeProperties = () => {
    if (!localNode) return null;

    switch (localNode.type) {
      case 'start':
      case 'end':
        return (
          <Box>
            <TextField
              fullWidth
              label="Label"
              value={localNode.label || ''}
              onChange={(e) => handleNodeChange('label', e.target.value)}
              sx={{ mb: 2 }}
            />
          </Box>
        );

      case 'humanTask':
        return <HumanTaskProperties node={localNode as any} onChange={handleNodeChange} />;

      case 'automatic':
        return <AutomaticProperties node={localNode as any} onChange={handleNodeChange} />;

      case 'gateway':
        return <GatewayProperties node={localNode as any} onChange={handleNodeChange} />;

      case 'timer':
        return <TimerProperties node={localNode as any} onChange={handleNodeChange} />;

      default:
        return null;
    }
  };

  return (
    <Drawer
      anchor="right"
      open={open}
      onClose={onClose}
      sx={{
        '& .MuiDrawer-paper': {
          width: 350,
          p: 2,
        },
      }}
    >
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h6">Properties</Typography>
        <IconButton onClick={onClose} size="small">
          <CloseIcon />
        </IconButton>
      </Box>

      {/* Workflow Properties */}
      <Accordion defaultExpanded={!selectedNode}>
        <AccordionSummary expandIcon={<ExpandMoreIcon />}>
          <Typography variant="subtitle1">Workflow Settings</Typography>
        </AccordionSummary>
        <AccordionDetails>
          <TextField
            fullWidth
            label="Workflow Name"
            value={workflowName}
            onChange={(e) => onWorkflowNameChange(e.target.value)}
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            label="Description"
            multiline
            rows={3}
            value={workflowDescription}
            onChange={(e) => onWorkflowDescriptionChange(e.target.value)}
          />
        </AccordionDetails>
      </Accordion>

      <Divider sx={{ my: 2 }} />

      {/* Node Properties */}
      {selectedNode ? (
        <Box>
          <Typography variant="subtitle1" gutterBottom>
            {selectedNode.type.charAt(0).toUpperCase() + selectedNode.type.slice(1)} Node
          </Typography>
          {renderNodeProperties()}
        </Box>
      ) : (
        <Alert severity="info">
          Select a node to edit its properties
        </Alert>
      )}
    </Drawer>
  );
}

// Individual property components for each node type
function HumanTaskProperties({ node, onChange }: { node: any; onChange: (field: string, value: any) => void }) {
  const [newRole, setNewRole] = useState('');

  const addRole = () => {
    if (newRole.trim() && !node.assigneeRoles?.includes(newRole.trim())) {
      const updatedRoles = [...(node.assigneeRoles || []), newRole.trim()];
      onChange('assigneeRoles', updatedRoles);
      setNewRole('');
    }
  };

  const removeRole = (roleToRemove: string) => {
    const updatedRoles = (node.assigneeRoles || []).filter((role: string) => role !== roleToRemove);
    onChange('assigneeRoles', updatedRoles);
  };

  // ✅ ADD: Predefined common roles
  const commonRoles = ['Manager', 'Approver', 'TeamLead', 'Admin', 'Reviewer', 'Supervisor'];

  return (
    <Box>
      <TextField
        fullWidth
        label="Task Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />

      <Box sx={{ mb: 2 }}>
        <Typography variant="subtitle2" gutterBottom>
          Who can work on this task?
        </Typography>
        <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 1 }}>
          Only users with these roles will see this task in their "My Tasks" list. Leave empty to allow any user.
        </Typography>
        
        {/* ✅ ADD: Quick role buttons */}
        <Box sx={{ mb: 1 }}>
          <Typography variant="caption" sx={{ mr: 1 }}>Common roles:</Typography>
          {commonRoles.map(role => (
            <Chip
              key={role}
              label={role}
              size="small"
              variant="outlined"
              onClick={() => {
                if (!node.assigneeRoles?.includes(role)) {
                  const updatedRoles = [...(node.assigneeRoles || []), role];
                  onChange('assigneeRoles', updatedRoles);
                }
              }}
              sx={{ mr: 0.5, mb: 0.5, cursor: 'pointer' }}
            />
          ))}
        </Box>

        <Box sx={{ display: 'flex', gap: 1, mb: 1 }}>
          <TextField
            size="small"
            placeholder="Enter custom role name"
            value={newRole}
            onChange={(e) => setNewRole(e.target.value)}
            onKeyPress={(e) => e.key === 'Enter' && addRole()}
          />
          <Button variant="outlined" size="small" onClick={addRole} disabled={!newRole.trim()}>
            <AddIcon fontSize="small" />
          </Button>
        </Box>

        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5, mb: 1 }}>
          {(node.assigneeRoles || []).map((role: string) => (
            <Chip
              key={role}
              label={role}
              size="small"
              color="primary"
              onDelete={() => removeRole(role)}
            />
          ))}
        </Box>

        {(node.assigneeRoles || []).length === 0 ? (
          <Alert severity="info" sx={{ mt: 1 }}>
            <Typography variant="caption">
              💡 No roles specified - this task will be available to <strong>all users</strong>
            </Typography>
          </Alert>
        ) : (
          <Alert severity="success" sx={{ mt: 1 }}>
            <Typography variant="caption">
              ✅ Only users with roles: <strong>{node.assigneeRoles.join(', ')}</strong> can work on this task
            </Typography>
          </Alert>
        )}
      </Box>

      <TextField
        fullWidth
        label="Due in Minutes"
        type="number"
        value={node.dueInMinutes || ''}
        onChange={(e) => onChange('dueInMinutes', e.target.value ? parseInt(e.target.value) : undefined)}
        helperText="Leave empty for no deadline"
        sx={{ mb: 2 }}
      />
    </Box>
  );
}

function AutomaticProperties({ node, onChange }: { node: any; onChange: (field: string, value: any) => void }) {
  return (
    <Box>
      <TextField
        fullWidth
        label="Task Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />

      <FormControl fullWidth sx={{ mb: 2 }}>
        <InputLabel>Action Type</InputLabel>
        <Select
          value={node.action?.kind || 'noop'}
          onChange={(e) => onChange('action', { ...node.action, kind: e.target.value })}
        >
          <MenuItem value="noop">No Operation</MenuItem>
          <MenuItem value="webhook">Webhook Call</MenuItem>
        </Select>
      </FormControl>

      {node.action?.kind === 'webhook' && (
        <TextField
          fullWidth
          label="Webhook URL"
          value={node.action?.config?.url || ''}
          onChange={(e) => onChange('action', {
            ...node.action,
            config: { ...node.action?.config, url: e.target.value }
          })}
          placeholder="https://api.example.com/webhook"
          sx={{ mb: 2 }}
        />
      )}
    </Box>
  );
}

function GatewayProperties({ node, onChange }: { node: any; onChange: (field: string, value: any) => void }) {
  return (
    <Box>
      <TextField
        fullWidth
        label="Gateway Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />

      <Typography variant="subtitle2" gutterBottom>
        Decision Logic
      </Typography>
      <Typography variant="caption" color="text.secondary" sx={{ display: 'block', mb: 2 }}>
        Define when this gateway should take the "True" path vs the "False" path
      </Typography>
      
      <ConditionBuilder
        value={node.condition || '{"==": [{"var": "approved"}, true]}'}
        onChange={(condition) => onChange('condition', condition)}
      />
    </Box>
  );
}

function TimerProperties({ node, onChange }: { node: any; onChange: (field: string, value: any) => void }) {
  const [timerType, setTimerType] = useState<'delay' | 'until'>(
    node.untilIso ? 'until' : 'delay'
  );

  const handleTimerTypeChange = (type: 'delay' | 'until') => {
    setTimerType(type);
    if (type === 'delay') {
      onChange('untilIso', undefined);
      if (!node.delayMinutes) onChange('delayMinutes', 5);
    } else {
      onChange('delayMinutes', undefined);
    }
  };

  return (
    <Box>
      <TextField
        fullWidth
        label="Timer Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />

      <FormControl fullWidth sx={{ mb: 2 }}>
        <InputLabel>Timer Type</InputLabel>
        <Select
          value={timerType}
          onChange={(e) => handleTimerTypeChange(e.target.value as 'delay' | 'until')}
        >
          <MenuItem value="delay">Delay (Minutes)</MenuItem>
          <MenuItem value="until">Until Date/Time</MenuItem>
        </Select>
      </FormControl>

      {timerType === 'delay' ? (
        <TextField
          fullWidth
          label="Delay in Minutes"
          type="number"
          value={node.delayMinutes || 5}
          onChange={(e) => onChange('delayMinutes', parseInt(e.target.value) || 5)}
          sx={{ mb: 2 }}
        />
      ) : (
        <TextField
          fullWidth
          label="Until Date/Time"
          type="datetime-local"
          value={node.untilIso || ''}
          onChange={(e) => onChange('untilIso', e.target.value)}
          sx={{ mb: 2 }}
        />
      )}
    </Box>
  );
}
