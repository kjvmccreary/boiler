import { useState, useEffect, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Alert,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Drawer,
  IconButton,
  Tooltip,
  Divider,
} from '@mui/material';
import {
  Save as SaveIcon,
  Publish as PublishIcon,
  ArrowBack as ArrowBackIcon,
  Settings as SettingsIcon,
  Close as CloseIcon,
} from '@mui/icons-material';
import { ReactFlow, Background, Controls, MiniMap, useNodesState, useEdgesState, addEdge, Connection, Edge } from 'reactflow';
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

export function BuilderPage() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  // ReactFlow state
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges, onEdgesChange] = useEdgesState([]);
  
  // Builder state
  const [selectedNode, setSelectedNode] = useState<DslNode | null>(null);
  const [propertyPanelOpen, setPropertyPanelOpen] = useState(false);
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [definition, setDefinition] = useState<WorkflowDefinitionDto | null>(null);
  
  // Workflow metadata
  const [workflowName, setWorkflowName] = useState('');
  const [workflowDescription, setWorkflowDescription] = useState('');
  
  // Dialogs
  const [publishDialogOpen, setPublishDialogOpen] = useState(false);
  const [publishNotes, setPublishNotes] = useState('');

  const isNewWorkflow = id === 'new';

  useEffect(() => {
    if (!isNewWorkflow && id) {
      loadDefinition();
    } else {
      // Initialize new workflow
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

      // ✅ FIX: Parse JSON string to DslDefinition object
      if (definitionData.jsonDefinition) {
        const dslDefinition: DslDefinition = JSON.parse(definitionData.jsonDefinition);
        const { nodes: flowNodes, edges: flowEdges } = deserializeFromDsl(dslDefinition);
        setNodes(flowNodes);
        setEdges(flowEdges);
      }
    } catch (error) {
      console.error('Failed to load definition:', error);
      toast.error('Failed to load workflow definition');
    } finally {
      setLoading(false);
    }
  };

  const handleSave = async () => {
    try {
      setSaving(true);
      
      // Serialize ReactFlow to DSL
      const dslDefinition: DslDefinition = serializeToDsl(
        nodes,
        edges,
        workflowName.toLowerCase().replace(/\s+/g, '-'),
        definition?.version
      );

      // ✅ FIX: Convert DslDefinition to JSON string
      const jsonDefinitionString = JSON.stringify(dslDefinition);

      if (isNewWorkflow) {
        // Create new draft
        const response = await workflowService.createDraft({
          name: workflowName,
          description: workflowDescription,
          jsonDefinition: jsonDefinitionString
        });
        setDefinition(response);
        toast.success('Workflow saved as draft');
        
        // Update URL to edit mode
        navigate(`/app/workflow/builder/${response.id}`, { replace: true });
      } else {
        // Update existing draft
        const response = await workflowService.updateDefinition(parseInt(id!), {
          name: workflowName,
          description: workflowDescription,
          jsonDefinition: jsonDefinitionString
        });
        setDefinition(response);
        toast.success('Workflow updated');
      }
    } catch (error) {
      console.error('Failed to save workflow:', error);
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

    // Validate before publishing
    const dslDefinition = serializeToDsl(nodes, edges, workflowName.toLowerCase().replace(/\s+/g, '-'));
    const validation = validateDefinition(dslDefinition);

    if (!validation.isValid) {
      toast.error(`Cannot publish: ${validation.errors.join(', ')}`);
      return;
    }

    if (validation.warnings.length > 0) {
      console.warn('Workflow has warnings:', validation.warnings);
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
      
      // Reload to get updated status
      loadDefinition();
    } catch (error) {
      console.error('Failed to publish workflow:', error);
      toast.error('Failed to publish workflow');
    }
  };

  const onConnect = useCallback((params: Connection) => {
    setEdges((eds) => addEdge(params, eds));
  }, [setEdges]);

  const handleNodeClick = useCallback((event: React.MouseEvent, node: any) => {
    // Find the corresponding DSL node
    const dslNode = nodes.find(n => n.id === node.id)?.data as DslNode;
    if (dslNode) {
      setSelectedNode(dslNode);
      setPropertyPanelOpen(true);
    }
  }, [nodes]);

  const handleNodeUpdate = useCallback((updatedNode: DslNode) => {
    setNodes((nds) =>
      nds.map((node) =>
        node.id === updatedNode.id
          ? { ...node, data: updatedNode }
          : node
      )
    );
  }, [setNodes]);

  const handleBack = () => {
    navigate('/app/workflow/definitions');
  };

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to use the workflow builder</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ height: '100vh', display: 'flex', flexDirection: 'column' }}>
      {/* Header */}
      <Box sx={{ 
        display: 'flex', 
        justifyContent: 'space-between', 
        alignItems: 'center', 
        p: 2, 
        borderBottom: 1, 
        borderColor: 'divider',
        bgcolor: 'background.paper'
      }}>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <IconButton onClick={handleBack} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
          <Typography variant="h5" component="h1">
            {isNewWorkflow ? 'Create Workflow' : 'Edit Workflow'}
          </Typography>
          {definition && (
            <Typography variant="body2" color="text.secondary" sx={{ ml: 2 }}>
              v{definition.version} • {definition.isPublished ? 'Published' : 'Draft'}
            </Typography>
          )}
        </Box>

        <Box sx={{ display: 'flex', gap: 1, alignItems: 'center' }}>
          <TextField
            size="small"
            placeholder="Workflow Name"
            value={workflowName}
            onChange={(e) => setWorkflowName(e.target.value)}
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

      {/* Main Content */}
      <Box sx={{ flex: 1, display: 'flex', overflow: 'hidden' }}>
        {/* Node Palette */}
        <NodePalette />

        {/* Builder Canvas */}
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

        {/* Property Panel */}
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

      {/* Publish Dialog */}
      <Dialog open={publishDialogOpen} onClose={() => setPublishDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Publish Workflow</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            Publishing will make this workflow immutable and available for creating instances.
            Are you sure you want to publish "{workflowName}"?
          </Typography>
          <TextField
            fullWidth
            label="Publish Notes (Optional)"
            multiline
            rows={3}
            value={publishNotes}
            onChange={(e) => setPublishNotes(e.target.value)}
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
