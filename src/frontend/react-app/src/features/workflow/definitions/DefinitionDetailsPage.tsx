import { useEffect, useState } from 'react';
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
  Tab
} from '@mui/material';
import { ArrowBack as ArrowBackIcon, Refresh as RefreshIcon } from '@mui/icons-material';
import { useParams, useNavigate } from 'react-router-dom';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowDefinitionDto } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import DefinitionDiagram from './DefinitionDiagram';
import DefinitionNodeDetailsPanel from './DefinitionNodeDetailsPanel';
import type { DslNode } from '../dsl/dsl.types';

export function DefinitionDetailsPage() {
  const { id } = useParams<{ id: string }>();
  const defId = id ? parseInt(id, 10) : NaN;
  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  const [definition, setDefinition] = useState<WorkflowDefinitionDto | null>(null);
  const [loading, setLoading] = useState(true);
  const [tab, setTab] = useState<'diagram' | 'json'>('diagram');
  const [selectedNode, setSelectedNode] = useState<DslNode | null>(null);

  const loadDefinition = async () => {
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
  };

  useEffect(() => {
    if (currentTenant && !isNaN(defId)) {
      loadDefinition();
    }
  }, [currentTenant, defId]); // eslint-disable-line

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
    // leave null; show raw below
  }

  return (
    <Box p={3} maxWidth={1200}>
      {/* Header */}
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Box display="flex" alignItems="center">
          <IconButton onClick={() => navigate('/app/workflow/definitions')} sx={{ mr: 1 }}>
            <ArrowBackIcon />
          </IconButton>
            <Typography variant="h4">
              Definition
              <Typography variant="subtitle2" color="text.secondary">
                {definition.name} • v{definition.version} • ID {definition.id}
              </Typography>
            </Typography>
        </Box>
        <Box display="flex" gap={1} alignItems="center">
          {statusChip}
          <Tooltip title="Refresh">
            <IconButton onClick={loadDefinition}>
              <RefreshIcon />
            </IconButton>
          </Tooltip>
        </Box>
      </Box>

      {/* Meta */}
      <Box mb={2} display="flex" flexWrap="wrap" gap={4}>
        {definition.description && (
          <Box sx={{ minWidth: 220 }}>
            <Typography variant="subtitle2" color="text.secondary">Description</Typography>
            <Typography>{definition.description}</Typography>
          </Box>
        )}
        <Box>
          <Typography variant="subtitle2" color="text.secondary">Version</Typography>
          <Typography>{definition.version}</Typography>
        </Box>
        <Box>
          <Typography variant="subtitle2" color="text.secondary">Created</Typography>
          <Typography>{new Date(definition.createdAt).toLocaleString()}</Typography>
        </Box>
        <Box>
          <Typography variant="subtitle2" color="text.secondary">Updated</Typography>
          <Typography>{new Date(definition.updatedAt).toLocaleString()}</Typography>
        </Box>
        {definition.publishedAt && (
          <Box>
            <Typography variant="subtitle2" color="text.secondary">Published</Typography>
            <Typography>{new Date(definition.publishedAt).toLocaleString()}</Typography>
          </Box>
        )}
      </Box>

      <Divider sx={{ mb: 2 }} />

      {/* Tabs */}
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
    </Box>
  );
}

export default DefinitionDetailsPage;
