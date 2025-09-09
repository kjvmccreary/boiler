import React, { useEffect, useMemo, useRef, useState } from 'react';
import {
  Box,
  Typography,
  RadioGroup,
  FormControlLabel,
  Radio,
  TextField,
  Chip,
  Stack,
  Button,
  Divider,
  Collapse,
  IconButton,
  Tooltip,
  Alert,
  CircularProgress,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions
} from '@mui/material';
import {
  Add as AddIcon,
  Close as CloseIcon,
  ExpandMore,
  ExpandLess,
  Done as DoneIcon,
  ErrorOutline as ErrorIcon,
  Code as CodeIcon
} from '@mui/icons-material';
import type {
  HumanTaskNode,
  HumanTaskAssignment,
  HumanTaskAssignmentMode
} from '../../dsl/dsl.types';
import { validateHumanTaskAssignment } from '../../dsl/assignmentRules';
import { useRolesCache } from '../../hooks/useRolesCache';
import { useUserSearch } from '../../hooks/useUserSearch';
import { workflowService } from '@/services/workflow.service';
import { trackWorkflow } from '../../telemetry/workflowTelemetry';
// Monaco hybrid editor (already present in codebase per H5 story)
// Falls back internally if Monaco not yet loaded.
// If file path differs adjust import accordingly.
// Use the existing editor colocated in this directory
import HybridExpressionEditor from './HybridExpressionEditor';

export interface AssignmentSectionProps {
  node: HumanTaskNode;
  onPatch: (patch: Partial<HumanTaskNode>) => void;
  readonly?: boolean;
}

const MODE_OPTIONS: { value: HumanTaskAssignmentMode; label: string }[] = [
  { value: 'users', label: 'Users' },
  { value: 'roles', label: 'Roles' },
  { value: 'expression', label: 'Expression' },
  { value: 'hybrid', label: 'Hybrid (Roles + Expression)' }
];

type ExprState = 'idle' | 'validating' | 'valid' | 'error';

export const AssignmentSection: React.FC<AssignmentSectionProps> = ({ node, onPatch }) => {
  const [expanded, setExpanded] = useState(true);
  const [assignment, setAssignment] = useState<HumanTaskAssignment | undefined>(node.assignment);

  // Expression
  const [expression, setExpression] = useState(assignment?.expression || '');
  const [exprState, setExprState] = useState<ExprState>('idle');
  const [exprError, setExprError] = useState<string | null>(null);
  const [exprVersion, setExprVersion] = useState(0);

  // SLA
  const [targetMinutes, setTargetMinutes] = useState<number | ''>(assignment?.sla?.targetMinutes ?? '');
  const [softWarning, setSoftWarning] = useState<number | ''>(assignment?.sla?.softWarningMinutes ?? '');

  // Escalation
  const [escalationRole, setEscalationRole] = useState(assignment?.escalation?.escalateToRole ?? '');
  const [escalationAfter, setEscalationAfter] = useState<number | ''>(assignment?.escalation?.afterMinutes ?? '');

  // Roles
  const [roleFilter, setRoleFilter] = useState('');
  const { roles: tenantRoles, loading: rolesLoading, error: rolesError } = useRolesCache();

  // Users
  const userSearchActive = assignment?.mode === 'users' || assignment?.mode === 'hybrid';
  const { results: userResults, loading: userLoading, term: userTerm, setTerm: setUserTerm, clear: clearUserTerm } =
    useUserSearch(300, userSearchActive);

  // Migration for legacy roles
  useEffect(() => {
    if (!assignment && node.assigneeRoles?.length) {
      setAssignment({ mode: 'roles', roles: [...node.assigneeRoles] });
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [node.id])

  const updateAssignment = (patch: Partial<HumanTaskAssignment>) => {
    setAssignment(prev => {
      const next = { ...(prev || { mode: 'roles' as HumanTaskAssignmentMode }), ...patch };
      return next;
    });
  };

  // Commit to node
  useEffect(() => {
    if (!assignment) return;
    const full: HumanTaskAssignment = {
      ...assignment,
      expression: (assignment.mode === 'expression' || assignment.mode === 'hybrid') ? (expression || '') : undefined,
      sla: targetMinutes !== '' ? { targetMinutes: Number(targetMinutes), ...(softWarning !== '' ? { softWarningMinutes: Number(softWarning) } : {}) } : undefined,
      escalation: (escalationRole && escalationAfter !== '' && targetMinutes !== '')
        ? { escalateToRole: escalationRole, afterMinutes: Number(escalationAfter) }
        : undefined
    };
    onPatch({ assignment: full });
  }, [assignment, expression, targetMinutes, softWarning, escalationRole, escalationAfter, onPatch]);

  const { errors, warnings } = useMemo(
    () => assignment ? validateHumanTaskAssignment({
      ...node,
      assignment: {
        ...assignment,
        expression: (assignment.mode === 'expression' || assignment.mode === 'hybrid') ? expression : undefined,
        sla: targetMinutes !== '' ? { targetMinutes: Number(targetMinutes), ...(softWarning !== '' ? { softWarningMinutes: Number(softWarning) } : {}) } : undefined,
        escalation: (escalationRole && escalationAfter !== '' && targetMinutes !== '')
          ? { escalateToRole: escalationRole, afterMinutes: Number(escalationAfter) }
          : undefined
      }
    }) : { errors: [], warnings: [] },
    [assignment, node, expression, targetMinutes, softWarning, escalationRole, escalationAfter]
  );

  // Expression validation (debounced by expression version change)
  useEffect(() => {
    if (!(assignment?.mode === 'expression' || assignment?.mode === 'hybrid')) return;
    if (!expression.trim()) {
      setExprState('idle');
      setExprError(null);
      return;
    }
    setExprState('validating');
    setExprError(null);
    const handle = setTimeout(async () => {
      try {
        const start = performance.now();
        const res = await workflowService.validateExpression('gateway', expression); // reuse existing kind
        if (res.success && res.errors.length === 0) {
          setExprState('valid');
          setExprError(null);
          console.debug('[assignment.expression.validated]', { ms: performance.now() - start });
        } else {
          setExprState('error');
          setExprError(res.errors[0] || 'Invalid expression');
        }
      } catch (e: any) {
        setExprState('error');
        setExprError(e?.message || 'Validation failed');
      }
    }, 600);
    return () => clearTimeout(handle);
  }, [expression, assignment?.mode, exprVersion]);

  const validateNow = async () => {
    setExprVersion(v => v + 1);
  };

  // Role ops
  const filteredRoles = tenantRoles.filter(r => !roleFilter.trim() || r.name.toLowerCase().includes(roleFilter.trim().toLowerCase()));
  const addRole = (name: string) => {
    if (!assignment) return;
    if (!name.trim()) return;
    if (assignment.roles?.includes(name)) return;
    updateAssignment({ roles: [...(assignment.roles || []), name] });
  };
  const removeRole = (name: string) => {
    if (!assignment) return;
    updateAssignment({ roles: (assignment.roles || []).filter(r => r !== name) });
  };

  // User ops
  const addUser = (id: string) => {
    if (!assignment) return;
    updateAssignment({ users: [...(assignment.users || []), id] });
    clearUserTerm();
  };
  const removeUser = (id: string) => {
    if (!assignment) return;
    updateAssignment({ users: (assignment.users || []).filter(u => u !== id) });
  };

  // Mode change confirmation (data loss warning)
  const [pendingMode, setPendingMode] = useState<HumanTaskAssignmentMode | null>(null);
  const [confirmOpen, setConfirmOpen] = useState(false);

  const computeModeLoss = (next: HumanTaskAssignmentMode): string[] => {
    const lost: string[] = [];
    if (!assignment) return lost;
    if (next === assignment.mode) return lost;
    if (next === 'users') {
      if (assignment.roles?.length) lost.push('roles');
      if (assignment.expression) lost.push('expression');
    } else if (next === 'roles') {
      if (assignment.users?.length) lost.push('users');
      if (assignment.expression) lost.push('expression');
    } else if (next === 'expression') {
      if (assignment.users?.length) lost.push('users');
      if (assignment.roles?.length) lost.push('roles');
    } else if (next === 'hybrid') {
      // users optional — we keep users & roles; expression remains / becomes required
      // no loss unless switching from expression-only (then just keep expression)
    }
    return lost;
  };

  const applyMode = (m: HumanTaskAssignmentMode) => {
    switch (m) {
      case 'users':
        updateAssignment({ mode: m, roles: undefined, expression: undefined });
        break;
      case 'roles':
        updateAssignment({ mode: m, users: undefined, expression: undefined });
        break;
      case 'expression':
        updateAssignment({ mode: m, users: undefined, roles: undefined });
        break;
      case 'hybrid':
        updateAssignment({ mode: m });
        break;
    }
    trackWorkflow('assignment.mode.changed', {
      nodeId: node.id,
      mode: m
    });
  };

  const requestModeChange = (m: HumanTaskAssignmentMode) => {
    if (!assignment) {
      updateAssignment({ mode: m });
      return;
    }
    const losses = computeModeLoss(m);
    if (losses.length === 0) {
      applyMode(m);
    } else {
      setPendingMode(m);
      setConfirmOpen(true);
    }
  };

  const summaryChips: string[] = [];
  if (assignment?.mode) summaryChips.push(assignment.mode);
  if (assignment?.users?.length) summaryChips.push(`Users(${assignment.users.length})`);
  if (assignment?.roles?.length) summaryChips.push(`Roles(${assignment.roles.length})`);
  if (assignment?.mode === 'expression' || assignment?.mode === 'hybrid') {
    summaryChips.push(exprState === 'valid' ? 'Expr✓' : exprState === 'error' ? 'Expr!' : 'Expr…');
  }
  if (targetMinutes !== '') summaryChips.push(`SLA:${targetMinutes}m`);
  if (softWarning !== '') summaryChips.push(`Warn@${softWarning}m`);
  if (escalationRole && escalationAfter !== '') summaryChips.push(`Esc@${escalationAfter}m`);

  return (
    <Box>
      <Box display="flex" alignItems="center" justifyContent="space-between" mb={1}>
        <Typography variant="subtitle2">Assignment</Typography>
        <IconButton size="small" onClick={() => setExpanded(e => !e)}>
          {expanded ? <ExpandLess /> : <ExpandMore />}
        </IconButton>
      </Box>

      <Stack direction="row" spacing={0.5} flexWrap="wrap" mb={expanded ? 1 : 0}>
        {summaryChips.map(c => (
          <Chip key={c} size="small" label={c} variant="outlined" />
        ))}
        {errors.length > 0 && <Chip size="small" label={`${errors.length} err`} color="error" />}
        {warnings.length > 0 && <Chip size="small" label={`W:${warnings.length}`} color="warning" />}
      </Stack>

      <Collapse in={expanded} unmountOnExit>
        {!assignment && (
          <Alert severity="info" sx={{ mb: 2 }}>
            No assignment configured. Select a mode to begin.
          </Alert>
        )}

        <RadioGroup
          row
          value={assignment?.mode || ''}
          onChange={(e) => requestModeChange(e.target.value as HumanTaskAssignmentMode)}
        >
          {MODE_OPTIONS.map(o => (
            <FormControlLabel
              key={o.value}
              value={o.value}
              control={<Radio size="small" />}
              label={o.label}
            />
          ))}
        </RadioGroup>

        <Divider sx={{ my: 1 }} />

        {(assignment?.mode === 'users' || assignment?.mode === 'hybrid') && (
          <Box mb={2}>
            <Typography variant="caption" fontWeight={600}>Users</Typography>
            <Box display="flex" gap={1} mt={0.5} alignItems="center">
              <TextField
                size="small"
                placeholder="Search user..."
                value={userTerm}
                onChange={(e) => setUserTerm(e.target.value)}
                fullWidth
              />
              {userLoading && <CircularProgress size={18} />}
            </Box>
            {userResults.length > 0 && (
              <Box mt={0.5} display="flex" flexWrap="wrap" gap={0.5}>
                {userResults.map(u => {
                  const active = assignment?.users?.includes(u.id);
                  return (
                    <Chip
                      key={u.id}
                      size="small"
                      label={u.displayName}
                      color={active ? 'primary' : 'default'}
                      onClick={() => !active && addUser(u.id)}
                      onDelete={active ? () => removeUser(u.id) : undefined}
                      deleteIcon={active ? <CloseIcon /> : undefined}
                      variant={active ? 'filled' : 'outlined'}
                    />
                  );
                })}
              </Box>
            )}
            <Box mt={0.5} display="flex" flexWrap="wrap" gap={0.5}>
              {(assignment?.users || []).map(u => (
                <Chip
                  key={u}
                  size="small"
                  label={u}
                  onDelete={() => removeUser(u)}
                  color="primary"
                />
              ))}
            </Box>
          </Box>
        )}

        {(assignment?.mode === 'roles' || assignment?.mode === 'hybrid') && (
          <Box mb={2}>
            <Typography variant="caption" fontWeight={600}>Roles</Typography>
            <Box display="flex" gap={1} mt={0.5} alignItems="center">
              <TextField
                size="small"
                placeholder="Filter roles..."
                value={roleFilter}
                onChange={(e) => setRoleFilter(e.target.value)}
                fullWidth
              />
              {rolesLoading && <CircularProgress size={18} />}
            </Box>
            {rolesError && <Alert severity="warning" sx={{ mt: 1 }}>{rolesError}</Alert>}
            <Box mt={0.5} display="flex" flexWrap="wrap" gap={0.5} maxHeight={110} sx={{ overflowY: 'auto' }}>
              {filteredRoles.map(r => {
                const active = assignment?.roles?.includes(r.name);
                return (
                  <Chip
                    key={r.id || r.name}
                    size="small"
                    label={r.name}
                    color={active ? 'primary' : 'default'}
                    onClick={() => !active && addRole(r.name)}
                    onDelete={active ? () => removeRole(r.name) : undefined}
                    deleteIcon={active ? <CloseIcon /> : undefined}
                    variant={active ? 'filled' : 'outlined'}
                  />
                );
              })}
            </Box>
            {assignment?.mode === 'roles' && (!assignment.roles || assignment.roles.length === 0) && (
              <Typography variant="caption" color="text.secondary">
                Select at least one role.
              </Typography>
            )}
          </Box>
        )}

        {(assignment?.mode === 'expression' || assignment?.mode === 'hybrid') && (
          <Box mb={2}>
            <Stack direction="row" alignItems="center" justifyContent="space-between" mb={0.5}>
              <Typography variant="caption" fontWeight={600} display="flex" alignItems="center" gap={0.5}>
                <CodeIcon fontSize="inherit" /> Expression (JsonLogic)
              </Typography>
              <Stack direction="row" spacing={1} alignItems="center">
                <Button
                  size="small"
                  variant="outlined"
                  onClick={validateNow}
                  disabled={exprState === 'validating'}
                >
                  {exprState === 'validating' ? 'Validating…' : 'Validate'}
                </Button>
                {exprState === 'valid' && <DoneIcon color="success" fontSize="small" />}
                {exprState === 'error' && <ErrorIcon color="error" fontSize="small" />}
              </Stack>
            </Stack>
            <HybridExpressionEditor
              value={expression}
              onChange={(val: string) => {
                setExpression(val);
                setExprState('idle');
              }}
              language="json"
              height={160}
              kind="task-assignment"
              semantic
              disableSemanticOnError
              onSemanticValidation={(res: { success: boolean; errors: string[]; durationMs: number }) => {
                if (res.success && res.errors.length === 0) {
                  setExprState('valid');
                  setExprError(null);
                  trackWorkflow('assignment.expression.validated', {
                    nodeId: node.id,
                    mode: assignment?.mode,
                    durationMs: Math.round(res.durationMs)
                  });
                } else if (res.errors.length) {
                  setExprState('error');
                  setExprError(res.errors[0]);
                }
              }}
              placeholder='{"users":["user123"],"roles":["Operators"]}'
            />
            {exprError && (
              <Typography variant="caption" color="error" sx={{ mt: 0.5 }}>
                {exprError}
              </Typography>
            )}
            <Typography variant="caption" color="text.secondary" display="block" sx={{ mt: 0.5 }}>
              Returns object with optional users / roles arrays. Semantic validation runs automatically.
            </Typography>
          </Box>
        )}

        {/* SLA */}
        <Box mb={2}>
          <Stack direction="row" spacing={1} alignItems="center">
            <Typography variant="caption" fontWeight={600}>SLA (mins)</Typography>
            <Tooltip title="Target resolution time and optional warning threshold">
              <IconButton size="small">
                <AddIcon fontSize="inherit" />
              </IconButton>
            </Tooltip>
          </Stack>
            <Stack direction="row" spacing={1} mt={0.5}>
            <TextField
              size="small"
              type="number"
              label="Target"
              value={targetMinutes}
              onChange={(e) => setTargetMinutes(e.target.value === '' ? '' : Number(e.target.value))}
              sx={{ width: 120 }}
            />
            <TextField
              size="small"
              type="number"
              label="Soft Warn"
              value={softWarning}
              onChange={(e) => setSoftWarning(e.target.value === '' ? '' : Number(e.target.value))}
              sx={{ width: 120 }}
              disabled={targetMinutes === ''}
            />
          </Stack>
        </Box>

        {/* Escalation */}
        <Box mb={2}>
          <Typography variant="caption" fontWeight={600}>Escalation</Typography>
          <Stack direction="row" spacing={1} mt={0.5}>
            <TextField
              size="small"
              label="Role"
              value={escalationRole}
              onChange={(e) => setEscalationRole(e.target.value)}
              disabled={targetMinutes === ''}
              sx={{ flex: 1 }}
            />
            <TextField
              size="small"
              type="number"
              label="After (m)"
              value={escalationAfter}
              onChange={(e) => setEscalationAfter(e.target.value === '' ? '' : Number(e.target.value))}
              disabled={targetMinutes === ''}
              sx={{ width: 120 }}
            />
          </Stack>
          <Typography variant="caption" color="text.secondary">
            Requires SLA target. (Future: automated escalation handling)
          </Typography>
        </Box>

        {(errors.length > 0 || warnings.length > 0) && (
          <Stack spacing={1} mb={1}>
            {errors.length > 0 && (
              <Alert severity="error" variant="outlined" sx={{ p: 1 }}>
                <Typography variant="caption" component="div">
                  {errors.map((e, i) => <div key={i}>{e}</div>)}
                </Typography>
              </Alert>
            )}
            {warnings.length > 0 && (
              <Alert severity="warning" variant="outlined" sx={{ p: 1 }}>
                <Typography variant="caption" component="div">
                  {warnings.map((w, i) => <div key={i}>{w}</div>)}
                </Typography>
              </Alert>
            )}
          </Stack>
        )}
      </Collapse>

      {/* Mode Change Confirmation Dialog */}
      <Dialog
        open={confirmOpen}
        onClose={() => setConfirmOpen(false)}
        maxWidth="xs"
        fullWidth
      >
        <DialogTitle>Change Assignment Mode?</DialogTitle>
        <DialogContent>
          <Typography variant="body2" sx={{ mb: 1 }}>
            Switching modes will discard the following data:
          </Typography>
          <ul style={{ margin: 0, paddingLeft: 18 }}>
            {pendingMode && computeModeLoss(pendingMode).map(l => (
              <li key={l}>
                <Typography variant="caption">{l}</Typography>
              </li>
            ))}
          </ul>
          <Alert severity="info" sx={{ mt: 2 }}>
            This action cannot be undone. Proceed?
          </Alert>
        </DialogContent>
        <DialogActions>
          <Button size="small" onClick={() => { setConfirmOpen(false); setPendingMode(null); }}>Cancel</Button>
          <Button
            size="small"
            variant="contained"
            color="primary"
            onClick={() => {
              if (pendingMode) applyMode(pendingMode);
              setConfirmOpen(false);
              setPendingMode(null);
            }}
          >
            Confirm
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
};

export default AssignmentSection;
