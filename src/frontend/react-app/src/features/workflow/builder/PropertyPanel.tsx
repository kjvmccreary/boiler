import { useState, useEffect, useRef } from 'react';
import {
  Drawer,
  Box,
  Typography,
  TextField,
  Button,
  Divider,
  IconButton,
  Chip,
  Alert,
  Accordion,
  AccordionSummary,
  AccordionDetails
} from '@mui/material';
import {
  Close as CloseIcon,
  Add as AddIcon,
  ExpandMore as ExpandMoreIcon,
} from '@mui/icons-material';
import type { DslNode, GatewayNode, JoinNode, HumanTaskNode } from '../dsl/dsl.types';
import { ConditionBuilder } from './components/ConditionBuilder';
import { roleService } from '@/services/role.service';
import { useTenant } from '@/contexts/TenantContext';
import type { RoleDto } from '@/types';
import GatewayStrategyEditor from './components/GatewayStrategyEditor';
import JoinConfigurationPanel from './components/JoinConfigurationPanel';
import { validateDefinition, type ExtendedValidationResult } from '../dsl/dsl.validate';
import type { DslDefinition } from '../dsl/dsl.types';
import AssignmentSection from './components/AssignmentSection';
import FormSchemaSection from './components/FormSchemaSection';
import ActionSection from './components/ActionSection';
import { workflowService } from '@/services/workflow.service';

interface PropertyPanelProps {
  open: boolean;
  onClose: () => void;
  selectedNode: DslNode | null;
  onNodeUpdate: (node: DslNode) => void;
  workflowName: string;
  workflowDescription: string;
  onWorkflowNameChange: (name: string) => void;
  onWorkflowDescriptionChange: (description: string) => void;
  currentDefinition?: DslDefinition;           // NEW: for diagnostics lookup
  latestValidation?: ExtendedValidationResult; // NEW: pass existing validation (avoid re-run)
  workflowTags?: string;                       // OPTIONAL: comma / space separated tags string
  onWorkflowTagsChange?: (tags: string) => void;
  definitionId?: number;                       // OPTIONAL: needed for live server tag validation
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
  currentDefinition,
  latestValidation,
  workflowTags,
  onWorkflowTagsChange,
  definitionId
}: PropertyPanelProps) {
  const [localNode, setLocalNode] = useState<DslNode | null>(null);
  const [localValidation, setLocalValidation] = useState<ExtendedValidationResult | null>(null);
  // Tags (local fallback if host does not provide handlers)
  const [localTags, setLocalTags] = useState<string>(workflowTags || '');
  const effectiveTags = workflowTags !== undefined ? workflowTags : localTags;
  const updateTags = (v: string) => {
    if (onWorkflowTagsChange) onWorkflowTagsChange(v);
    else setLocalTags(v);
  };

  // --- Server-side tag validation (debounced, clean implementation) ---
  const [tagErrors, setTagErrors] = useState<string[]>([]);
  const [tagValidating, setTagValidating] = useState(false);
  const tagDebounceRef = useRef<number | null>(null);
  const lastValidatedRef = useRef<string>('');

  useEffect(() => {
    if (!definitionId) {
      // No definition yet (new draft) – skip server validation
      setTagErrors([]);
      return;
    }
    if (tagDebounceRef.current) window.clearTimeout(tagDebounceRef.current);

    // Empty tags => clear errors
    if (!effectiveTags || !effectiveTags.trim()) {
      setTagErrors([]);
      lastValidatedRef.current = '';
      return;
    }

    // Debounce call
    tagDebounceRef.current = window.setTimeout(async () => {
      if (effectiveTags === lastValidatedRef.current) return;
      try {
        setTagValidating(true);
        const res = await workflowService.previewValidateTags(definitionId, effectiveTags);
        lastValidatedRef.current = effectiveTags;
        setTagErrors(res.errors);
      } catch {
        // Ignore network / 404 (service method already 404-safe)
        setTagErrors([]);
      } finally {
        setTagValidating(false);
      }
    }, 500);

    return () => {
      if (tagDebounceRef.current) window.clearTimeout(tagDebounceRef.current);
    };
  }, [effectiveTags, definitionId]);

  useEffect(() => { setLocalNode(selectedNode); }, [selectedNode]);

  // Run validation only if not supplied (fallback)
  useEffect(() => {
    if (latestValidation) {
      setLocalValidation(latestValidation);
    } else if (currentDefinition) {
      setLocalValidation(validateDefinition(currentDefinition));
    }
  }, [latestValidation, currentDefinition]);

  const handleNodeChange = (field: string, value: any) => {
    if (!localNode) return;
    const updatedNode = { ...localNode, [field]: value };
    setLocalNode(updatedNode);
    onNodeUpdate(updatedNode);
  };

  const gatewayDiagnostics = (localNode?.type === 'gateway' && localValidation)
    ? localValidation.diagnostics.parallelGateways[localNode.id]
    : undefined;

  const joinDiagnostics = (localNode?.type === 'join' && localValidation)
    ? localValidation.diagnostics.joins[localNode.id]
    : undefined;

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
        return (
          <HumanTaskProperties
            node={localNode as HumanTaskNode}
            onChange={handleNodeChange}
          />
        );
      case 'automatic':
        return <AutomaticProperties node={localNode as any} onChange={handleNodeChange} />;
      case 'gateway':
        return (
          <Box>
            <TextField
              fullWidth
              label="Label"
              value={localNode.label || ''}
              onChange={(e) => handleNodeChange('label', e.target.value)}
              sx={{ mb: 2 }}
            />
            <GatewayStrategyEditor
              node={localNode as GatewayNode}
              onChange={(patch) => {
                Object.entries(patch).forEach(([k, v]) => handleNodeChange(k, v));
              }}
            />
            {gatewayDiagnostics && (
              <Alert
                severity={gatewayDiagnostics.hasError ? 'error' : (gatewayDiagnostics.multipleCommon || gatewayDiagnostics.subsetJoins.length || gatewayDiagnostics.orphanBranches.length) ? 'warning' : 'info'}
                sx={{ mt: 2, p: 1 }}
              >
                <Typography variant="caption" component="div" sx={{ whiteSpace: 'pre-line' }}>
                  {gatewayDiagnostics.hasError && 'Structural error: no downstream join.'}
                  {gatewayDiagnostics.multipleCommon && `Multiple convergence joins: ${gatewayDiagnostics.commonJoins.join(', ')}\n`}
                  {gatewayDiagnostics.subsetJoins.length > 0 && `Subset joins: ${gatewayDiagnostics.subsetJoins.join(', ')}\n`}
                  {gatewayDiagnostics.orphanBranches.length > 0 && `Orphan branches: ${gatewayDiagnostics.orphanBranches.join(', ')}\n`}
                </Typography>
              </Alert>
            )}
          </Box>
        );
      case 'join':
        return (
          <Box>
            <TextField
              fullWidth
              label="Label"
              value={localNode.label || ''}
              onChange={(e) => handleNodeChange('label', e.target.value)}
              sx={{ mb: 2 }}
            />
            <JoinConfigurationPanel
              node={localNode as JoinNode}
              diagnostics={joinDiagnostics}
              onChange={(patch) => {
                Object.entries(patch).forEach(([k, v]) => handleNodeChange(k, v));
              }}
            />
          </Box>
        );
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
          width: 360,
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
            sx={{ mb: 2 }}
          />
          <TextField
            fullWidth
            label="Tags"
            placeholder="comma or space separated"
            value={effectiveTags}
            onChange={(e) => updateTags(e.target.value)}
            helperText={tagValidating ? 'Validating…' : 'Server-validated tag set (invalid tags block publish)'}
            error={tagErrors.length > 0}
            FormHelperTextProps={{ sx: { color: tagErrors.length ? 'error.main' : 'text.secondary' } }}
          />
          {tagErrors.length > 0 && (
            <Alert severity="error" sx={{ mt: 1, p: 1 }}>
              <Typography variant="caption" component="div">
                {tagErrors.map((t, i) => <div key={i}>{t}</div>)}
              </Typography>
            </Alert>
          )}
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

/* ---------------- Human Task ---------------- */
function HumanTaskProperties({ node, onChange }: { node: HumanTaskNode; onChange: (field: string, value: any) => void }) {
  const [newRole, setNewRole] = useState('');
  const [tenantRoles, setTenantRoles] = useState<RoleDto[]>([]);
  const [loadingRoles, setLoadingRoles] = useState(false);
  const [rolesError, setRolesError] = useState<string | null>(null);
  const { currentTenant } = useTenant();
  // Track previous assignment snapshot for history
  const assignmentRef = useRef(node.assignment);

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

  useEffect(() => {
    if (node.assignment && node.assignment !== assignmentRef.current) {
      const historyEntry = {
        ts: new Date().toISOString(),
        mode: node.assignment.mode,
        roles: node.assignment.roles,
        users: node.assignment.users,
        slaTarget: node.assignment.sla?.targetMinutes
      };
      const existing = node.assignmentHistory || [];
      onChange('assignmentHistory', [...existing.slice(-19), historyEntry]); // cap history at 20
      assignmentRef.current = node.assignment;
    }
  }, [node.assignment, node.assignmentHistory, onChange]);

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

  // Legacy UI retained as a "Legacy Roles" section if assignment model not yet used.
  const usingNewModel = !!node.assignment;

  return (
    <Box>
      <TextField
        fullWidth
        label="Task Label"
        value={node.label || ''}
        onChange={(e) => onChange('label', e.target.value)}
        sx={{ mb: 2 }}
      />

      <AssignmentSection
        node={node}
        onPatch={(patch) => {
          Object.entries(patch).forEach(([k, v]) => onChange(k, v));
        }}
      />

      {!usingNewModel && (
        <Box mt={3}>
          <Typography variant="subtitle2" gutterBottom>Legacy Roles (will migrate to Assignment)</Typography>
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
      )}

      <Box mt={3}>
        <FormSchemaSection
          value={node.formSchema ?? node.formSchemaSource}
          onChange={(parsed, src) => {
            onChange('formSchema', parsed);
            onChange('formSchemaSource', src);
          }}
        />
      </Box>

      {node.assignmentHistory && node.assignmentHistory.length > 0 && (
        <Box mt={3}>
          <Typography variant="subtitle2" gutterBottom>
            Assignment History (session)
          </Typography>
            <Box
              sx={{
                maxHeight: 140,
                overflowY: 'auto',
                border: theme => `1px solid ${theme.palette.divider}`,
                borderRadius: 1,
                p: 1,
                fontFamily: 'monospace',
                fontSize: '0.7rem',
                backgroundColor: 'background.paper'
              }}
            >
              {node.assignmentHistory.slice().reverse().map((h, i) => (
                <Box key={i} sx={{ mb: 0.5 }}>
                  [{new Date(h.ts).toLocaleTimeString()}] mode={h.mode}
                  {h.roles?.length ? ` roles=${h.roles.join(',')}` : ''}
                  {h.users?.length ? ` users=${h.users.join(',')}` : ''}
                  {h.slaTarget != null ? ` sla=${h.slaTarget}m` : ''}
                </Box>
              ))}
            </Box>
        </Box>
      )}
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
      <ActionSection
        node={node}
        onPatch={(patch) => {
          Object.entries(patch).forEach(([k, v]) => onChange(k, v));
        }}
      />
    </Box>
  );
}

/* ---------------- Timer (unchanged from latest enhanced version) ---------------- */
function TimerProperties({ node, onChange }: { node: any; onChange: (f: string, v: any) => void }) {
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
    if (isNaN(n) || n < 0) { onChange('delaySeconds', undefined); return; }
    onChange('delaySeconds', n);
  };

  const updateMinutes = (val: string) => {
    const n = Number(val);
    if (isNaN(n) || n < 0) { onChange('delayMinutes', undefined); return; }
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
      <TextField
        fullWidth
        type="datetime-local"
        label="Until (local)"
        value={timerType === 'until' && node.untilIso ? toLocalInput(node.untilIso) : ''}
        onChange={(e) => {
          if (!e.target.value) {
            onChange('untilIso', undefined);
            if (timerType === 'until') switchTimerType('delay');
            return;
          }
          switchTimerType('until');
          const iso = new Date(e.target.value).toISOString();
          onChange('untilIso', iso);
        }}
        helperText="Set an absolute fire time or clear to use relative delay."
        sx={{ mb: 2 }}
      />

      {timerType === 'delay' && (
        <>
          <TextField
            fullWidth
            label={`Delay (${mode === 'seconds' ? 'Seconds' : 'Minutes'})`}
            type="number"
            inputProps={{ step: mode === 'seconds' ? 0.1 : 0.01, min: 0 }}
            value={mode === 'seconds' ? (node.delaySeconds ?? '') : (node.delayMinutes ?? '')}
            onChange={(e) => mode === 'seconds' ? updateSeconds(e.target.value) : updateMinutes(e.target.value)}
            sx={{ mb: 2 }}
          />
          <Box sx={{ display: 'flex', gap: 1, mb: 1 }}>
            <Button
              size="small"
              variant={mode === 'seconds' ? 'contained' : 'outlined'}
              onClick={() => switchMode('seconds')}
            >
              Seconds
            </Button>
            <Button
              size="small"
              variant={mode === 'minutes' ? 'contained' : 'outlined'}
              onClick={() => switchMode('minutes')}
            >
              Minutes
            </Button>
          </Box>
        </>
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
