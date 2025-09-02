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
  CircularProgress,
} from '@mui/material';
import {
  Close as CloseIcon,
  Add as AddIcon,
  ExpandMore as ExpandMoreIcon,
} from '@mui/icons-material';
import type { DslNode } from '../dsl/dsl.types';
import { ConditionBuilder } from './components/ConditionBuilder';
import { roleService } from '@/services/role.service';
import { useTenant } from '@/contexts/TenantContext';
import type { RoleDto } from '@/types';

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

  useEffect(() => { setLocalNode(selectedNode); }, [selectedNode]);

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

/* ---------------- Human Task (unchanged except context) ---------------- */
function HumanTaskProperties({ node, onChange }: { node: any; onChange: (field: string, value: any) => void }) {
  const [newRole, setNewRole] = useState('');
  const [tenantRoles, setTenantRoles] = useState<RoleDto[]>([]);
  const [loadingRoles, setLoadingRoles] = useState(false);
  const [rolesError, setRolesError] = useState<string | null>(null);
  const { currentTenant } = useTenant();

  useEffect(() => {
    const loadRoles = async () => {
      if (!currentTenant) { setTenantRoles([]); return; }
      try {
        setLoadingRoles(true);
        const resp = await roleService.getRoles({ page: 1, pageSize: 100 });
        const workflowRoles = resp.roles.filter(r =>
          r.permissions?.some(p => {
            const name = typeof p === 'string' ? p : p.name;
            return name.startsWith('workflow.');
          })
        );
        setTenantRoles(workflowRoles);
        if (workflowRoles.length === 0 && resp.roles.length > 0)
          setRolesError('No roles have workflow.* permissions yet.');
      } catch {
        setRolesError('Failed to load roles');
      } finally {
        setLoadingRoles(false);
      }
    };
    loadRoles();
  }, [currentTenant]);

  const addRole = () => {
    if (newRole.trim() && !node.assigneeRoles?.includes(newRole.trim())) {
      onChange('assigneeRoles', [...(node.assigneeRoles || []), newRole.trim()]);
      setNewRole('');
    }
  };
  const toggleTenantRole = (name: string) => {
    if (node.assigneeRoles?.includes(name)) return;
    onChange('assigneeRoles', [...(node.assigneeRoles || []), name]);
  };
  const removeRole = (r: string) =>
    onChange('assigneeRoles', (node.assigneeRoles || []).filter((x: string) => x !== r));

  return (
    <Box>
      <TextField
        fullWidth
        label="Task Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />
      <Typography variant="subtitle2" gutterBottom>Assignable Roles</Typography>
      {loadingRoles && <Typography variant="caption">Loading roles…</Typography>}
      {rolesError && <Alert severity="warning" sx={{ mb: 1 }}>{rolesError}</Alert>}
      {!loadingRoles && !rolesError && tenantRoles.length > 0 && (
        <Box sx={{ mb: 1 }}>
          {tenantRoles.map(r => (
            <Chip
              key={r.id}
              label={r.name}
              size="small"
              onClick={() => toggleTenantRole(r.name)}
              sx={{ mr: .5, mb: .5, cursor: 'pointer' }}
              color={node.assigneeRoles?.includes(r.name) ? 'primary' : 'default'}
            />
          ))}
        </Box>
      )}
      <Box sx={{ display: 'flex', gap: 1, mb: 1 }}>
        <TextField
          size="small"
            placeholder="Add role"
            value={newRole}
            onChange={(e) => setNewRole(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && addRole()}
        />
        <Button variant="outlined" size="small" onClick={addRole} disabled={!newRole.trim()}>
          <AddIcon fontSize="small" />
        </Button>
      </Box>
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: .5 }}>
        {(node.assigneeRoles || []).map((role: string) => (
          <Chip key={role} label={role} size="small" onDelete={() => removeRole(role)} color="primary" />
        ))}
      </Box>
      <TextField
        fullWidth
        label="Due in Minutes"
        type="number"
        value={node.dueInMinutes ?? ''}
        onChange={(e) => onChange('dueInMinutes', e.target.value ? parseInt(e.target.value) : undefined)}
        helperText="Optional deadline"
        sx={{ mt: 2 }}
      />
    </Box>
  );
}

/* ---------------- Automatic ---------------- */
function AutomaticProperties({ node, onChange }: { node: any; onChange: (f: string, v: any) => void }) {
  return (
    <Box>
      <TextField
        fullWidth
        label="Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />
      <FormControl fullWidth sx={{ mb: 2 }}>
        <InputLabel>Action</InputLabel>
        <Select
          value={node.action?.kind || 'noop'}
          label="Action"
          onChange={(e) => onChange('action', { ...(node.action || {}), kind: e.target.value })}
        >
          <MenuItem value="noop">No Operation</MenuItem>
          <MenuItem value="webhook">Webhook</MenuItem>
        </Select>
      </FormControl>
      {node.action?.kind === 'webhook' && (
        <TextField
          fullWidth
          label="Webhook URL"
          value={node.action?.config?.url || ''}
          onChange={(e) =>
            onChange('action', {
              ...(node.action || {}),
              config: { ...(node.action?.config || {}), url: e.target.value }
            })
          }
        />
      )}
    </Box>
  );
}

/* ---------------- Gateway ---------------- */
function GatewayProperties({ node, onChange }: { node: any; onChange: (f: string, v: any) => void }) {
  return (
    <Box>
      <TextField
        fullWidth
        label="Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />
      <Typography variant="subtitle2" gutterBottom>JsonLogic Condition</Typography>
      <ConditionBuilder
        value={node.condition || '{"==":[{"var":"approved"},true]}'}
        onChange={(v) => onChange('condition', v)}
      />
    </Box>
  );
}

/* ---------------- Timer (UPDATED) ---------------- */
function TimerProperties({ node, onChange }: { node: any; onChange: (f: string, v: any) => void }) {
  // Determine initial mode
  const initialMode: 'seconds' | 'minutes' =
    node.delaySeconds != null ? 'seconds' : 'minutes';
  const [mode, setMode] = useState<'seconds' | 'minutes'>(initialMode);
  const [timerType, setTimerType] = useState<'delay' | 'until'>(
    node.untilIso ? 'until' : 'delay'
  );

  const effectiveLabel =
    node.untilIso
      ? 'untilIso overrides delaySeconds/delayMinutes'
      : node.delaySeconds != null
        ? `fires in ${node.delaySeconds}s`
        : node.delayMinutes != null
          ? `fires in ${node.delayMinutes}m`
          : 'default 1 minute (backend)';

  const switchTimerType = (v: 'delay' | 'until') => {
    setTimerType(v);
    if (v === 'until') {
      onChange('delaySeconds', undefined);
      onChange('delayMinutes', undefined);
    } else {
      onChange('untilIso', undefined);
      if (node.delaySeconds == null && node.delayMinutes == null) {
        onChange('delayMinutes', 1);
      }
    }
  };

  const switchMode = (m: 'seconds' | 'minutes') => {
    if (timerType === 'until') return;
    if (m === mode) return;
    if (m === 'seconds' && node.delayMinutes != null) {
      const seconds = Number(node.delayMinutes) * 60;
      onChange('delaySeconds', Number(seconds.toFixed(3)));
      onChange('delayMinutes', undefined);
    } else if (m === 'minutes' && node.delaySeconds != null) {
      const minutes = Number(node.delaySeconds) / 60;
      onChange('delayMinutes', Number(minutes.toFixed(4)));
      onChange('delaySeconds', undefined);
    }
    setMode(m);
  };

  const updateSeconds = (val: string) => {
    const n = Number(val);
    if (isNaN(n) || n < 0) {
      onChange('delaySeconds', undefined);
      return;
    }
    onChange('delaySeconds', n);
  };

  const updateMinutes = (val: string) => {
    const n = Number(val);
    if (isNaN(n) || n < 0) {
      onChange('delayMinutes', undefined);
      return;
    }
    onChange('delayMinutes', n);
  };

  return (
    <Box>
      <TextField
        fullWidth
        label="Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />

      <FormControl fullWidth sx={{ mb: 2 }}>
        <InputLabel>Timer Type</InputLabel>
        <Select
          value={timerType}
          label="Timer Type"
          onChange={(e) => switchTimerType(e.target.value as any)}
        >
          <MenuItem value="delay">Relative Delay</MenuItem>
          <MenuItem value="until">Absolute (untilIso)</MenuItem>
        </Select>
      </FormControl>

      {timerType === 'delay' && (
        <>
          <FormControl fullWidth sx={{ mb: 2 }}>
            <InputLabel>Delay Mode</InputLabel>
            <Select
              value={mode}
              label="Delay Mode"
              onChange={(e) => switchMode(e.target.value as any)}
            >
              <MenuItem value="seconds">Seconds</MenuItem>
              <MenuItem value="minutes">Minutes</MenuItem>
            </Select>
          </FormControl>

          {mode === 'seconds' && (
            <TextField
              fullWidth
              label="Delay Seconds (>=0, float)"
              type="number"
              inputProps={{ step: 0.1, min: 0 }}
              value={node.delaySeconds ?? ''}
              onChange={(e) => updateSeconds(e.target.value)}
              sx={{ mb: 2 }}
            />
          )}

            {mode === 'minutes' && (
            <TextField
              fullWidth
              label="Delay Minutes (>=0, float)"
              type="number"
              inputProps={{ step: 0.01, min: 0 }}
              value={node.delayMinutes ?? ''}
              onChange={(e) => updateMinutes(e.target.value)}
              sx={{ mb: 2 }}
            />
          )}
        </>
      )}

      {timerType === 'until' && (
        <TextField
          fullWidth
          type="datetime-local"
          label="Until (local)"
          value={node.untilIso ? toLocalInput(node.untilIso) : ''}
          onChange={(e) => {
            if (!e.target.value) {
              onChange('untilIso', undefined);
              return;
            }
            const iso = new Date(e.target.value).toISOString();
            onChange('untilIso', iso);
          }}
          helperText="Stored as UTC ISO"
          sx={{ mb: 2 }}
        />
      )}

      <Alert severity="info" sx={{ mt: 1 }}>
        Priority: untilIso &gt; delaySeconds &gt; delayMinutes. {effectiveLabel}.
      </Alert>
    </Box>
  );
}

function toLocalInput(iso: string) {
  const d = new Date(iso);
  const pad = (n: number) => n.toString().padStart(2, '0');
  return `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}T${pad(d.getHours())}:${pad(d.getMinutes())}`;
}
