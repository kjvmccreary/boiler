import { useState, useEffect, useCallback, useRef } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  IconButton,
  Tooltip,
  Alert,
  CircularProgress,
  Badge
} from '@mui/material';
import {
  Save as SaveIcon,
  ArrowBack as ArrowBackIcon,
  Settings as SettingsIcon
} from '@mui/icons-material';
import {
  useNodesState,
  useEdgesState,
  addEdge,
  Connection,
  Edge
} from 'reactflow';
import 'reactflow/dist/style.css';

import { BuilderCanvas } from './BuilderCanvas';
import { NodePalette } from './NodePalette';
import { PropertyPanel } from './PropertyPanel';
import { workflowService } from '@/services/workflow.service';
import { validateDefinition } from '../dsl/dsl.validate';
import { serializeToDsl, deserializeFromDsl } from '../dsl/dsl.serialize';
import type { DslDefinition, DslNode } from '../dsl/dsl.types';
import type { WorkflowDefinitionDto } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import { useWorkflowValidation } from '../hooks/useWorkflowValidation';

// --- Helpers to enforce gateway branch metadata ---
function extractBranch(edge: Edge): 'true' | 'false' | undefined {
  const sh = edge.sourceHandle;
  if (sh === 'true' || sh === 'false') return sh;
  const d = (edge as any).data?.fromHandle;
  if (d === 'true' || d === 'false') return d;
  const lbl = typeof edge.label === 'string' ? edge.label.toLowerCase() : undefined;
  if (lbl === 'true' || lbl === 'false') return lbl as 'true' | 'false';
  return undefined;
}

function applyGatewayBranchMetadata(dsl: DslDefinition, flowEdges: Edge[]): DslDefinition {
  const map = new Map(flowEdges.map(e => [e.id, e]));
  const nextEdges = dsl.edges.map(e => {
    const fe = map.get(e.id);
    if (!fe) return e;
    const branch = extractBranch(fe);
    if (!branch) return e;
    return {
      ...e,
      label: branch,
      fromHandle: branch
    } as any;
  });

  // Deduplicate per gateway (one true, one false)
  const byFrom: Record<string, any[]> = {};
  for (const e of nextEdges) (byFrom[e.from] ||= []).push(e);
  const filtered: any[] = [];
  for (const list of Object.values(byFrom)) {
    let trueSeen = false, falseSeen = false;
    for (const e of list) {
      if (e.label === 'true') {
        if (trueSeen) continue;
        trueSeen = true;
      } else if (e.label === 'false') {
        if (falseSeen) continue;
        falseSeen = falseSeen ? true : false;
        if (falseSeen) continue;
        falseSeen = true;
      }
      filtered.push(e);
    }
  }
  return { ...dsl, edges: filtered };
}

export function BuilderPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges, onEdgesChange] = useEdgesState([]);

  const [selectedNode, setSelectedNode] = useState<DslNode | null>(null);
  const [propertyPanelOpen, setPropertyPanelOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [definition, setDefinition] = useState<WorkflowDefinitionDto | null>(null);

  const [workflowName, setWorkflowName] = useState('');
  const [workflowDescription, setWorkflowDescription] = useState('');

  const [publishDialogOpen, setPublishDialogOpen] = useState(false);
  const [publishNotes, setPublishNotes] = useState('');

  const { loading: validating, result: validation, validateJson, clear } = useWorkflowValidation();
  const [showValidation, setShowValidation] = useState(false);

  const isNewWorkflow = id === 'new';

  // Dirty tracking
  const [dirty, setDirty] = useState(false);
  const lastSavedJsonRef = useRef<string | null>(null);

  useEffect(() => {
    if (!isNewWorkflow && id) {
      loadDefinition();
    } else {
      setWorkflowName('New Workflow');
      setWorkflowDescription('');
    }
  }, [id, isNewWorkflow]);

  // Load definition
  const loadDefinition = async () => {
    if (!id || isNewWorkflow) return;
    try {
      setLoading(true);
      const definitionData = await workflowService.getDefinition(parseInt(id));
      setDefinition(definitionData);
      setWorkflowName(definitionData.name);
      setWorkflowDescription(definitionData.description || '');

      if (definitionData.jsonDefinition) {
        lastSavedJsonRef.current = definitionData.jsonDefinition;
        const dsl: DslDefinition = JSON.parse(definitionData.jsonDefinition);
        const { nodes: flowNodes, edges: flowEdges } = deserializeFromDsl(dsl);

        setNodes(flowNodes);
        requestAnimationFrame(() => {
          const fixed = flowEdges.map(e => {
            const lbl = typeof e.label === 'string' ? e.label.toLowerCase() : '';
            if ((lbl === 'true' || lbl === 'false') && !e.sourceHandle) {
              return {
                ...e,
                sourceHandle: lbl,
                data: { ...(e.data || {}), fromHandle: lbl }
              };
            }
            return e;
          });
          setEdges(fixed);
        });
      } else {
        lastSavedJsonRef.current = null;
      }
      setDirty(false);
    } catch (err) {
      console.error('Failed to load definition:', err);
      toast.error('Failed to load workflow definition');
    } finally {
      setLoading(false);
    }
  };

  const markDirty = () => setDirty(true);

  const onNodesChangeWrapped = useCallback((changes: any) => {
    markDirty();
    onNodesChange(changes);
  }, [onNodesChange]);

  const onEdgesChangeWrapped = useCallback((changes: any) => {
    markDirty();
    onEdgesChange(changes);
  }, [onEdgesChange]);

  // Edge connect
  const onConnect = useCallback((params: Connection) => {
    const { source, target, sourceHandle, targetHandle } = params;
    if (!source || !target) return;
    const branch = (sourceHandle === 'true' || sourceHandle === 'false') ? sourceHandle : undefined;
    setEdges(eds =>
      addEdge(
        {
          id: `e-${source}-${sourceHandle ?? 'h'}-${target}-${Date.now()}`,
          source,
          target,
          sourceHandle: sourceHandle ?? undefined,
          ...(targetHandle ? { targetHandle } : {}),
          label: branch,
          data: branch ? { fromHandle: branch } : undefined,
          type: 'default'
        } as Edge,
        eds
      )
    );
    markDirty();
  }, [setEdges]);

  // Save draft
  const handleSave = async () => {
    try {
      setSaving(true);
      let dslDefinition: DslDefinition = serializeToDsl(
        nodes,
        edges,
        workflowName.toLowerCase().replace(/\s+/g, '-'),
        definition?.version
      );
      dslDefinition = applyGatewayBranchMetadata(dslDefinition, edges);
      const jsonDefinitionString = JSON.stringify(dslDefinition);

      if (isNewWorkflow || !definition) {
        const response = await workflowService.createDraft({
          name: workflowName,
          description: workflowDescription,
          jsonDefinition: jsonDefinitionString
        });
        setDefinition(response);
        toast.success('Workflow saved as draft');
        navigate(`/app/workflow/builder/${response.id}`, { replace: true });
      } else {
        const response = await workflowService.updateDefinition(parseInt(id!), {
          name: workflowName,
          description: workflowDescription,
          jsonDefinition: jsonDefinitionString
        });
        setDefinition(response);
        toast.success('Workflow updated');
      }
      lastSavedJsonRef.current = jsonDefinitionString;
      setDirty(false);
    } catch (err) {
      console.error('Failed to save workflow:', err);
      toast.error('Failed to save workflow');
    } finally {
      setSaving(false);
    }
  };

  // Ensure latest saved (auto-save for publish)
  const ensureLatestSaved = async (): Promise<boolean> => {
    const dsl = serializeToDsl(
      nodes,
      edges,
      workflowName.toLowerCase().replace(/\s+/g, '-'),
      definition?.version
    );
    const json = JSON.stringify(applyGatewayBranchMetadata(dsl, edges));
    if (!definition) {
      await handleSave();
      return true;
    }
    if (dirty || lastSavedJsonRef.current !== json) {
      try {
        await workflowService.updateDefinition(definition.id, {
          name: workflowName,
          description: workflowDescription,
          jsonDefinition: json
        });
        lastSavedJsonRef.current = json;
        setDirty(false);
        toast.success('Draft auto-saved before publish');
      } catch {
        toast.error('Auto-save failed; cannot publish');
        return false;
      }
    }
    return true;
  };

  // Validate (button)
  const handleValidate = async () => {
    const dsl = serializeToDsl(nodes, edges, workflowName.toLowerCase().replace(/\s+/g, '-'));
    const jsonString = JSON.stringify(dsl);
    const vr = await validateJson(jsonString);
    setShowValidation(true);
    if (!vr.success) {
      toast.error(vr.errors[0] ?? 'Validation failed');
    } else if (vr.warnings.length) {
      toast.success('Validation passed with warnings');
    } else {
      toast.success('Validation passed');
    }
  };

  // Publish (open dialog)
  const handlePublish = async () => {
    if (!definition && !isNewWorkflow) {
      toast.error('Definition not loaded yet');
      return;
    }

    const dsl = serializeToDsl(nodes, edges, workflowName.toLowerCase().replace(/\s+/g, '-'));
    const jsonString = JSON.stringify(dsl);
    const vr = await validateJson(jsonString);
    setShowValidation(true);

    if (!vr.success) {
      toast.error('Fix validation errors before publishing');
      return;
    }

    const savedOk = await ensureLatestSaved();
    if (!savedOk) return;

    const local = validateDefinition(applyGatewayBranchMetadata(dsl, edges));
    if (!local.isValid) {
      toast.error(`Cannot publish: ${local.errors.join(', ')}`);
      return;
    }

    setPublishDialogOpen(true);
  };

  // Confirm publish (call API)
  const handlePublishConfirm = async () => {
    if (!definition) return;
    try {
      await workflowService.publishDefinition(definition.id, {
        publishNotes: publishNotes || undefined
      });
      toast.success('Workflow published successfully');
      setPublishDialogOpen(false);
      setPublishNotes('');
      loadDefinition();
    } catch (err: any) {
      const errs = (err as any).errors;
      if (errs?.length) {
        toast.error(`Publish failed: ${errs[0]}`);
        setShowValidation(true);
      } else {
        toast.error('Failed to publish workflow');
      }
    }
  };

  const handleNodeClick = useCallback(
    (_evt: React.MouseEvent, node: any) => {
      const dslNode = nodes.find(n => n.id === node.id)?.data as DslNode;
      if (dslNode) {
        setSelectedNode(dslNode);
        setPropertyPanelOpen(true);
      }
    },
    [nodes]
  );

  const handleNodeUpdate = useCallback(
    (updated: DslNode) => {
      setNodes(nds =>
        nds.map(n => (n.id === updated.id ? { ...n, data: updated } : n))
      );
      markDirty();
    },
    [setNodes]
  );

  const handleBack = () => navigate('/app/workflow/definitions');

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to use the workflow builder</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ height: '100vh', display: 'flex', flexDirection: 'column' }}>
      <Box
        sx={{
          display: 'flex',
          justifyContent: 'space-between',
          alignItems: 'center',
          p: 2,
          borderBottom: 1,
          borderColor: 'divider',
          bgcolor: 'background.paper'
        }}
      >
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
          <IconButton onClick={handleBack}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h5">
            {isNewWorkflow ? 'Create Workflow' : 'Edit Workflow'}
          </Typography>
          {definition && (
            <Typography variant="body2" color="text.secondary">
              v{definition.version} • {definition.isPublished ? 'Published' : 'Draft'}
            </Typography>
          )}
        </Box>

        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
          <TextField
            size="small"
            placeholder="Workflow Name"
            value={workflowName}
            onChange={e => { setWorkflowName(e.target.value); markDirty(); }}
            sx={{ width: 200 }}
          />
          <Button
            variant="outlined"
            startIcon={<SaveIcon />}
            onClick={handleSave}
            disabled={saving || !workflowName.trim()}
          >
            {saving ? 'Saving...' : 'Save'}
          </Button>
          <Button
            variant="outlined"
            onClick={handleValidate}
            disabled={validating}
          >
            {validating ? 'Validating…' : 'Validate'}
          </Button>

          {definition && !definition.isPublished && (
            <Badge
              color={validation?.errors.length ? 'error' : (validation?.warnings.length ? 'warning' : 'success')}
              badgeContent={
                validation
                  ? validation.errors.length
                    ? validation.errors.length
                    : validation.warnings.length
                      ? `W:${validation.warnings.length}`
                      : '✓'
                  : undefined
              }
              overlap="circular"
              anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
            >
              <Button
                variant="contained"
                color="primary"
                onClick={handlePublish}
                disabled={validating || !definition?.id}
              >
                Publish
              </Button>
            </Badge>
          )}
          <Tooltip title="Properties">
            <IconButton
              onClick={() => setPropertyPanelOpen(true)}
              color={propertyPanelOpen ? 'primary' : 'default'}
            >
              <SettingsIcon />
            </IconButton>
          </Tooltip>
        </Box>
      </Box>

      <Box sx={{ flex: 1, display: 'flex', overflow: 'hidden' }}>
        <NodePalette />
        <Box sx={{ flex: 1, position: 'relative' }}>
          <BuilderCanvas
            nodes={nodes}
            edges={edges}
            onNodesChange={onNodesChangeWrapped}
            onEdgesChange={onEdgesChangeWrapped}
            onConnect={onConnect}
            onNodeClick={handleNodeClick}
            setNodes={setNodes}
            setEdges={setEdges}
          />
        </Box>
        <PropertyPanel
          open={propertyPanelOpen}
          onClose={() => setPropertyPanelOpen(false)}
          selectedNode={selectedNode}
          onNodeUpdate={handleNodeUpdate}
          workflowName={workflowName}
          workflowDescription={workflowDescription}
          onWorkflowNameChange={(v) => { setWorkflowName(v); markDirty(); }}
          onWorkflowDescriptionChange={(v) => { setWorkflowDescription(v); markDirty(); }}
        />
      </Box>

      <Dialog
        open={publishDialogOpen}
        onClose={() => setPublishDialogOpen(false)}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Publish Workflow</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            Publishing will make this workflow immutable and available for creating
            instances. Are you sure you want to publish "{workflowName}"?
          </Typography>
          <TextField
            fullWidth
            label="Publish Notes (Optional)"
            multiline
            rows={3}
            value={publishNotes}
            onChange={e => setPublishNotes(e.target.value)}
            placeholder="Add notes about this version..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPublishDialogOpen(false)}>Cancel</Button>
          <Button onClick={handlePublishConfirm} variant="contained">
            Publish
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={showValidation} onClose={() => { setShowValidation(false); clear(); }} maxWidth="sm" fullWidth>
        <DialogTitle>Graph Validation</DialogTitle>
        <DialogContent>
          {validating && <CircularProgress size={24} />}
          {validation && (
            <>
              {!validation.success && (
                <Alert severity="error" sx={{ mb: 2 }}>Fix errors before publishing.</Alert>
              )}
              {validation.errors.length > 0 && (
                <div>
                  <strong>Errors:</strong>
                  <ul>{validation.errors.map((e, i) => <li key={i}>{e}</li>)}</ul>
                </div>
              )}
              {validation.warnings.length > 0 && (
                <div style={{ marginTop: 8 }}>
                  <strong>Warnings:</strong>
                  <ul>{validation.warnings.map((w, i) => <li key={i}>{w}</li>)}</ul>
                </div>
              )}
              {validation.success && validation.errors.length === 0 && (
                <Alert severity="success" sx={{ mt: 2 }}>Validation passed.</Alert>
              )}
            </>
          )}
        </DialogContent>
      </Dialog>
    </Box>
  );
}

export default BuilderPage;
