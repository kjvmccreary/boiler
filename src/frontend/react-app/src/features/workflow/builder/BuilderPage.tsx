import { useState, useEffect, useCallback } from 'react';
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
  Tooltip
} from '@mui/material';
import {
  Save as SaveIcon,
  Publish as PublishIcon,
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

  const isNewWorkflow = id === 'new';

  useEffect(() => {
    if (!isNewWorkflow && id) {
      loadDefinition();
    } else {
      setWorkflowName('New Workflow');
      setWorkflowDescription('');
    }
  }, [id, isNewWorkflow]);

  const loadDefinition = async () => {
    if (!id || isNewWorkflow) return;
    try {
      setLoading(true);
      const definitionData = await workflowService.getDefinition(parseInt(id));
      setDefinition(definitionData);
      setWorkflowName(definitionData.name);
      setWorkflowDescription(definitionData.description || '');

      if (definitionData.jsonDefinition) {
        const dsl: DslDefinition = JSON.parse(definitionData.jsonDefinition);
        const { nodes: flowNodes, edges: flowEdges } = deserializeFromDsl(dsl);

        // Defer edges until after nodes mount so gateway handles exist
        setNodes(flowNodes);
        requestAnimationFrame(() => {
          // Safety: ensure sourceHandle set if label indicates branch
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
      }
    } catch (err) {
      console.error('Failed to load definition:', err);
      toast.error('Failed to load workflow definition');
    } finally {
      setLoading(false);
    }
  };

  // FIX: Narrow source/target before constructing Edge object
  const onConnect = useCallback(
    (params: Connection) => {
      const { source, target, sourceHandle, targetHandle } = params;
      if (!source || !target) return; // Narrow to string

      const branch =
        sourceHandle === 'true' || sourceHandle === 'false'
          ? sourceHandle
          : undefined;

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
    },
    [setEdges]
  );

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

      if (isNewWorkflow) {
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
    } catch (err) {
      console.error('Failed to save workflow:', err);
      toast.error('Failed to save workflow');
    } finally {
      setSaving(false);
    }
  };

  const handlePublish = async () => {
    if (!definition) {
      toast.error('Please save the workflow first');
      return;
    }
    const dslDefinition = applyGatewayBranchMetadata(
      serializeToDsl(nodes, edges, workflowName.toLowerCase().replace(/\s+/g, '-')),
      edges
    );
    const validation = validateDefinition(dslDefinition);
    if (!validation.isValid) {
      toast.error(`Cannot publish: ${validation.errors.join(', ')}`);
      return;
    }
    setPublishDialogOpen(true);
  };

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
    } catch (err) {
      console.error('Failed to publish workflow:', err);
      toast.error('Failed to publish workflow');
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
            onChange={e => setWorkflowName(e.target.value)}
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
          {definition && !definition.isPublished && (
            <Button
              variant="contained"
              startIcon={<PublishIcon />}
              onClick={handlePublish}
              disabled={saving}
            >
              Publish
            </Button>
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
            onNodesChange={onNodesChange}
            onEdgesChange={onEdgesChange}
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
          onWorkflowNameChange={setWorkflowName}
          onWorkflowDescriptionChange={setWorkflowDescription}
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
    </Box>
  );
}

export default BuilderPage;
