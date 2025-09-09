import { useEffect, useState, useCallback } from 'react';
import {
  Box,
  Typography,
  Chip,
  Button,
  IconButton,
  Divider,
  Alert,
  CircularProgress,
  Tooltip,
  Tabs,
  Tab,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Stack,
  TextField,
  Collapse
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Refresh as RefreshIcon,
  WarningAmber as WarningIcon,
  PowerSettingsNew as TerminateIcon,
  FactCheck as RevalidateIcon,
  LibraryAdd as NewVersionIcon,
  ExpandMore as ExpandMoreIcon,
  ExpandLess as ExpandLessIcon
} from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowDefinitionDto, ValidationResultDto, CreateNewVersionRequestDto } from '@/types/workflow';
import { InstanceStatus } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import DefinitionDiagram from './DefinitionDiagram';
import DefinitionNodeDetailsPanel from './DefinitionNodeDetailsPanel';
import type { DslNode } from '../dsl/dsl.types';

/* Optional permissions hook (fallback permissive) */
function useOptionalPermissions() {
  try {
    // eslint-disable-next-line @typescript-eslint/no-var-requires
    const mod = require('@/context/PermissionContext');
    if (mod?.usePermissions) {
      const p = mod.usePermissions();
      return { has: (perm: string) => p.has(perm) };
    }
  } catch {
    /* ignore */
  }
  return { has: () => true };
}

export function DefinitionDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const defId = id ? parseInt(id, 10) : NaN;
  const navigate = useNavigate();
  const { currentTenant } = useTenant();
  const perms = useOptionalPermissions();

  const [definition, setDefinition] = useState<WorkflowDefinitionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState<'diagram' | 'json'>('diagram');
  const [selectedNode, setSelectedNode] = useState<DslNode | null>(null);

  // C8 state
  const [runningCount, setRunningCount] = useState<number | null>(null);
  const [countLoading, setCountLoading] = useState(false);
  const [terminateDialogOpen, setTerminateDialogOpen] = useState(false);
  const [terminating, setTerminating] = useState(false);
  const [terminateResult, setTerminateResult] = useState<number | null>(null);

  // Revalidate state
  const [revalidating, setRevalidating] = useState(false);
  const [validation, setValidation] = useState<ValidationResultDto | null>(null);
  const [showValidation, setShowValidation] = useState(false);

  // New Version state
  const [newVersionDialogOpen, setNewVersionDialogOpen] = useState(false);
  const [newVersionNotes, setNewVersionNotes] = useState('');
  const [creatingVersion, setCreatingVersion] = useState(false);

  const loadDefinition = useCallback(async () => {
    if (isNaN(defId)) return;
    try {
      setLoading(true);
      const def = await workflowService.getDefinition(defId);
      setDefinition(def);
    } catch {
      toast.error('Failed to load workflow definition');
    } finally {
      setLoading(false);
    }
  }, [defId]);

  const loadRunningCount = useCallback(async () => {
    if (isNaN(defId)) return;
    try {
      setCountLoading(true);
      const statuses: InstanceStatus[] = [InstanceStatus.Running, InstanceStatus.Suspended];
      let total = 0;
      for (const status of statuses) {
        const page = await workflowService.getInstancesPaged({
          workflowDefinitionId: defId,
          status,
          page: 1,
          pageSize: 1
        } as any);
        if (typeof page.totalCount === 'number') {
          total += page.totalCount;
        } else {
          const pageAll = await workflowService.getInstancesPaged({
            workflowDefinitionId: defId,
            status,
            page: 1,
            pageSize: 500
          } as any);
          total += pageAll.items.length;
        }
      }
      setRunningCount(total);
    } catch {
      setRunningCount(null);
    } finally {
      setCountLoading(false);
    }
  }, [defId]);

  useEffect(() => {
    if (currentTenant && !isNaN(defId)) {
      loadDefinition();
      loadRunningCount();
    }
  }, [currentTenant, defId, loadDefinition, loadRunningCount]);

  const openTerminateDialog = () => {
    setTerminateResult(null);
    setTerminateDialogOpen(true);
  };

  const handleTerminate = async () => {
    if (!definition) return;
    setTerminating(true);
    try {
      const res = await workflowService.terminateDefinitionInstances(definition.id);
      const terminated = (res as any)?.terminated ?? 0;
      setTerminateResult(terminated);
      toast.success(`Terminated ${terminated} running instance${terminated === 1 ? '' : 's'}`);
      loadRunningCount();
    } catch (e: any) {
      toast.error(e?.message || 'Terminate running failed');
    } finally {
      setTerminating(false);
    }
  };

  const handleRevalidate = async () => {
    if (!definition) return;
    setRevalidating(true);
    try {
      const result = await workflowService.revalidateDefinition(definition.id);
      setValidation(result as any);
      setShowValidation(true);
      if (result.errors?.length) {
        toast.error('Definition has validation errors');
      } else if (result.warnings?.length) {
        toast.success('Revalidated with warnings');
      } else {
        toast.success('Revalidation passed');
      }
    } catch (e: any) {
      toast.error(e?.message || 'Revalidation failed');
    } finally {
      setRevalidating(false);
    }
  };

  const handleCreateNewVersion = async () => {
    if (!definition) return;
    setCreatingVersion(true);
    try {
      const payload: CreateNewVersionRequestDto = {
        notes: newVersionNotes || undefined
      } as any;
      const draft = await workflowService.createNewVersion(definition.id, payload);
      toast.success(`Draft v${draft.version} created`);
      setNewVersionDialogOpen(false);
      setNewVersionNotes('');
      navigate(`/app/workflow/builder/${draft.id}`);
    } catch (e: any) {
      toast.error(e?.message || 'Create draft version failed');
    } finally {
      setCreatingVersion(false);
    }
  };

  if (!currentTenant) {
    return (
      <Box p={3}>
        <Alert severity="info">Select a tenant to view definitions.</Alert>
      </Box>
    );
  }

  if (loading) {
    return (
      <Box p={3} display="flex" gap={2} alignItems="center">
        <CircularProgress size={24} />
        <Typography>Loading definition...</Typography>
      </Box>
    );
  }

  if (!definition) {
    return (
      <Box p={3}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/app/workflow/definitions')}
          sx={{ mb: 2 }}
        >
          Back to Definitions
        </Button>
        <Alert severity="error">Workflow definition not found</Alert>
      </Box>
    );
  }

  const statusChip = (
    <Chip
      size="small"
      color={definition.isPublished ? 'success' : 'default'}
      label={definition.isPublished ? 'Published' : 'Draft'}
    />
  );

  let parsedJson: any = null;
  try {
    parsedJson = definition.jsonDefinition ? JSON.parse(definition.jsonDefinition) : null;
  } catch {
    // ignore
  }

  const showTerminateAction =
    perms.has('workflow.admin') &&
    definition.isPublished &&
    (runningCount ?? 0) > 0;

  const validationSeverity =
    validation?.errors?.length
      ? 'error'
      : validation?.warnings?.length
        ? 'warning'
        : validation
          ? 'success'
          : undefined;

  return (
    <Box p={3} maxWidth={1300}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3} flexWrap="wrap" gap={2}>
        <Box display="flex" alignItems="center">
          <IconButton onClick={() => navigate('/app/workflow/definitions')} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
          <Box>
            <Typography variant="h4" sx={{ lineHeight: 1 }}>
              Definition
            </Typography>
            <Typography variant="subtitle2" color="text.secondary">
              {definition.name} • v{definition.version} • ID {definition.id}
            </Typography>
          </Box>
        </Box>
        <Stack direction="row" spacing={1} alignItems="center" flexWrap="wrap">
          {statusChip}
          <Tooltip title="Refresh metadata">
            <IconButton onClick={() => { loadDefinition(); loadRunningCount(); }}>
              <RefreshIcon />
            </IconButton>
          </Tooltip>

          <Tooltip title="Revalidate current definition state">
            <span>
              <Button
                size="small"
                variant="outlined"
                startIcon={revalidating ? <CircularProgress size={14} /> : <RevalidateIcon />}
                onClick={handleRevalidate}
                disabled={revalidating}
              >
                Revalidate
              </Button>
            </span>
          </Tooltip>

          {definition.isPublished && (
            <Tooltip title="Create a new draft version from this published definition">
              <span>
                <Button
                  size="small"
                  variant="contained"
                  color="primary"
                  startIcon={<NewVersionIcon />}
                  onClick={() => setNewVersionDialogOpen(true)}
                >
                  New Version
                </Button>
              </span>
            </Tooltip>
          )}

          {showTerminateAction && (
            <Tooltip title="Terminate all running / suspended instances of this definition">
              <span>
                <Button
                  variant="outlined"
                  color="error"
                  size="small"
                  startIcon={<TerminateIcon />}
                  onClick={openTerminateDialog}
                >
                  Terminate Running ({runningCount})
                </Button>
              </span>
            </Tooltip>
          )}
          {!showTerminateAction && perms.has('workflow.admin') && definition.isPublished && (
            <Chip
              size="small"
              variant="outlined"
              color="default"
              label={
                countLoading
                  ? 'Counting...'
                  : `No active instances`
              }
            />
          )}

          {validationSeverity && (
            <Chip
              size="small"
              color={validationSeverity as any}
              variant="outlined"
              label={
                validation?.errors?.length
                  ? `Errors: ${validation.errors.length}`
                  : validation?.warnings?.length
                    ? `Warnings: ${validation.warnings.length}`
                    : 'Valid'
              }
              onClick={() => setShowValidation(v => !v)}
              icon={showValidation ? <ExpandLessIcon /> : <ExpandMoreIcon />}
            />
          )}
        </Stack>
      </Box>

      <Collapse in={showValidation && !!validation} unmountOnExit>
        {validation && (
          <Box mb={2}>
            {validation.errors?.length > 0 && (
              <Alert severity="error" sx={{ mb: 1 }}>
                <strong>Errors</strong>
                <ul style={{ margin: '4px 0 0 16px' }}>
                  {validation.errors.map((e, i) => <li key={i}>{e}</li>)}
                </ul>
              </Alert>
            )}
            {validation.warnings?.length > 0 && (
              <Alert severity="warning" sx={{ mb: 1 }}>
                <strong>Warnings</strong>
                <ul style={{ margin: '4px 0 0 16px' }}>
                  {validation.warnings.map((w, i) => <li key={i}>{w}</li>)}
                </ul>
              </Alert>
            )}
            {!validation.errors?.length && !validation.warnings?.length && (
              <Alert severity="success">
                No validation issues detected.
              </Alert>
            )}
          </Box>
        )}
      </Collapse>

      <Box mb={2} display="flex" flexWrap="wrap" gap={4}>
        {definition.description && (
          <Box sx={{ minWidth: 220 }}>
            <Typography variant="subtitle2" color="text.secondary">
              Description
            </Typography>
            <Typography>{definition.description}</Typography>
          </Box>
        )}
        <Box>
          <Typography variant="subtitle2" color="text.secondary">
            Version
          </Typography>
          <Typography>{definition.version}</Typography>
        </Box>
        <Box>
          <Typography variant="subtitle2" color="text.secondary">
            Created
          </Typography>
            <Typography>{new Date(definition.createdAt).toLocaleString()}</Typography>
        </Box>
        <Box>
          <Typography variant="subtitle2" color="text.secondary">
            Updated
          </Typography>
          <Typography>{new Date(definition.updatedAt).toLocaleString()}</Typography>
        </Box>
        {definition.publishedAt && (
          <Box>
            <Typography variant="subtitle2" color="text.secondary">
              Published
            </Typography>
            <Typography>{new Date(definition.publishedAt).toLocaleString()}</Typography>
          </Box>
        )}
        <Box>
          <Typography variant="subtitle2" color="text.secondary">
            Active Instances
          </Typography>
          <Typography>
            {countLoading ? <CircularProgress size={12} /> : (runningCount ?? '—')}
          </Typography>
        </Box>
      </Box>

      <Divider sx={{ mb: 2 }} />

      <Tabs
        value={tab}
        onChange={(_, v) => setTab(v)}
        sx={{ mb: 2 }}
      >
        <Tab value="diagram" label="Diagram" />
        <Tab value="json" label="JSON" />
      </Tabs>

      {tab === 'diagram' && (
        <>
          <DefinitionDiagram
            jsonDefinition={definition.jsonDefinition}
            onNodeSelect={setSelectedNode}
          />
          <DefinitionNodeDetailsPanel
            open={!!selectedNode}
            node={selectedNode}
            onClose={() => setSelectedNode(null)}
          />
        </>
      )}

      {tab === 'json' && (
        <Box
          sx={{
            backgroundColor: 'grey.50',
            p: 2,
            borderRadius: 1,
            fontFamily: 'monospace',
            fontSize: '0.8rem',
            maxHeight: 500,
            overflow: 'auto'
          }}
        >
          <pre>
{parsedJson
  ? JSON.stringify(parsedJson, null, 2)
  : (definition.jsonDefinition || '// No JSON definition')}
          </pre>
        </Box>
      )}

      {/* Terminate Running Dialog */}
      <Dialog
        open={terminateDialogOpen}
        onClose={() => (!terminating ? setTerminateDialogOpen(false) : undefined)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>
          Terminate Running Instances
        </DialogTitle>
        <DialogContent dividers>
          <Stack spacing={2}>
            <Alert severity="warning" icon={<WarningIcon />}>
              This will terminate all Running and Suspended instances for
              definition "{definition.name}" (ID {definition.id}). This action cannot
              be undone. In‑flight human tasks will be marked cancelled.
            </Alert>
            <Box>
              <Typography variant="subtitle2" color="text.secondary">
                Active Instances Detected
              </Typography>
              <Typography>
                {countLoading ? 'Counting...' : (runningCount ?? 0)}
              </Typography>
            </Box>
            {terminateResult != null && (
              <Alert severity="success">
                Terminated {terminateResult} instance{terminateResult === 1 ? '' : 's'}.
              </Alert>
            )}
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button
            onClick={() => setTerminateDialogOpen(false)}
            disabled={terminating}
          >
            Close
          </Button>
          <Button
            color="error"
            variant="contained"
            startIcon={terminating ? <CircularProgress size={16} /> : <TerminateIcon />}
            onClick={handleTerminate}
            disabled={terminating || terminateResult != null || (runningCount ?? 0) === 0}
          >
            {terminateResult != null
              ? 'Completed'
              : terminating
                ? 'Terminating...'
                : 'Confirm Terminate'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* New Version Dialog */}
      <Dialog
        open={newVersionDialogOpen}
        onClose={() => (!creatingVersion ? setNewVersionDialogOpen(false) : undefined)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Create New Draft Version</DialogTitle>
        <DialogContent dividers>
          <Stack spacing={2}>
            <Alert severity="info">
              A new draft version will be created from the published definition (v{definition.version}). You can edit and publish it independently.
            </Alert>
            <TextField
              fullWidth
              multiline
              minRows={2}
              label="Version Notes (Optional)"
              value={newVersionNotes}
              onChange={e => setNewVersionNotes(e.target.value)}
              placeholder="Describe changes you plan to make..."
            />
          </Stack>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setNewVersionDialogOpen(false)} disabled={creatingVersion}>
            Cancel
          </Button>
          <Button
            variant="contained"
            onClick={handleCreateNewVersion}
            disabled={creatingVersion}
            startIcon={creatingVersion ? <CircularProgress size={16} /> : <NewVersionIcon />}
          >
            {creatingVersion ? 'Creating…' : 'Create Draft'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default DefinitionDetailsPage;
