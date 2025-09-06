import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
  IconButton,
  Chip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Menu,
  MenuItem,
} from '@mui/material';
import {
  DataGridPremium,
  GridColDef,
  GridRowParams,
  GridActionsCellItem,
  GridRowId,
  GridToolbar,
} from '@mui/x-data-grid-premium';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Publish as PublishIcon,
  PlayArrow as StartIcon,
  Visibility as ViewIcon,
  FileCopy as DuplicateIcon,
  MoreVert as MoreVertIcon,
  Gavel as UnpublishIcon,
  Archive as ArchiveIcon,
  Cancel as TerminateIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowDefinitionDto } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';

export function DefinitionsPage() {
  const [definitions, setDefinitions] = useState<WorkflowDefinitionDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [publishDialogOpen, setPublishDialogOpen] = useState(false);
  const [definitionToPublish, setDefinitionToPublish] = useState<WorkflowDefinitionDto | null>(null);
  const [publishNotes, setPublishNotes] = useState('');
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [definitionToDelete, setDefinitionToDelete] = useState<WorkflowDefinitionDto | null>(null);
  const [unpublishDialogOpen, setUnpublishDialogOpen] = useState(false);
  const [archiveDialogOpen, setArchiveDialogOpen] = useState(false);
  const [terminateDialogOpen, setTerminateDialogOpen] = useState(false);
  const [targetDefinition, setTargetDefinition] = useState<WorkflowDefinitionDto | null>(null);

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  useEffect(() => {
    if (currentTenant) {
      loadDefinitions();
    }
  }, [currentTenant]);

  const loadDefinitions = async () => {
    try {
      setLoading(true);
      console.log('🔄 DefinitionsPage: Loading workflow definitions');

      const response = await workflowService.getDefinitions();
      
      // ✅ FIX: Add defensive checks for the response
      if (!response) {
        console.warn('⚠️ DefinitionsPage: No response from getDefinitions');
        setDefinitions([]);
        return;
      }

      if (!Array.isArray(response)) {
        console.warn('⚠️ DefinitionsPage: Response is not an array:', response);
        setDefinitions([]);
        return;
      }

      console.log('✅ DefinitionsPage: Loaded', response.length, 'definitions');
      setDefinitions(response);
    } catch (error) {
      console.error('❌ DefinitionsPage: Failed to load definitions:', error);
      toast.error('Failed to load workflow definitions');
      // ✅ FIX: Set empty array on error to prevent further crashes
      setDefinitions([]);
    } finally {
      setLoading(false);
    }
  };

  // ✅ FIX: Add /app prefix to navigation paths
  const handleCreateNew = () => {
    navigate('/app/workflow/builder/new');
  };

  const handleEdit = (id: GridRowId) => {
    navigate(`/app/workflow/builder/${id}`);
  };

  const handleView = (id: GridRowId) => {
    navigate(`/app/workflow/definitions/${id}`);
  };

  const handleDuplicate = async (definition: WorkflowDefinitionDto) => {
    try {
      const duplicateRequest = {
        name: `${definition.name} (Copy)`,
        jsonDefinition: definition.jsonDefinition,
        description: definition.description
      };

      await workflowService.createDraft(duplicateRequest);
      toast.success('Workflow definition duplicated successfully');
      loadDefinitions();
    } catch (error) {
      console.error('Failed to duplicate definition:', error);
      toast.error('Failed to duplicate workflow definition');
    }
  };

  const handlePublish = (definition: WorkflowDefinitionDto) => {
    setDefinitionToPublish(definition);
    setPublishDialogOpen(true);
    setPublishNotes('');
  };

  const handlePublishConfirm = async () => {
    if (!definitionToPublish) return;

    try {
      await workflowService.publishDefinition(definitionToPublish.id, {
        publishNotes: publishNotes || undefined
      });
      toast.success('Workflow definition published successfully');
      loadDefinitions();
    } catch (error) {
      console.error('Failed to publish definition:', error);
      toast.error('Failed to publish workflow definition');
    } finally {
      setPublishDialogOpen(false);
      setDefinitionToPublish(null);
      setPublishNotes('');
    }
  };

  const handleStartInstance = async (definition: WorkflowDefinitionDto) => {
    try {
      const instance = await workflowService.startInstance({
        workflowDefinitionId: definition.id,
        initialContext: '{}',
        startNotes: 'Started from definitions page'
      });

      if (!instance || !instance.id) {
        console.error('Start instance returned invalid payload:', instance);
        toast.error('Instance started but response was invalid');
        return;
      }

      toast.success('Workflow instance started successfully');
      navigate(`/app/workflow/instances/${instance.id}`);
    } catch (error) {
      console.error('Failed to start instance:', error);
      toast.error('Failed to start workflow instance');
    }
  };

  const handleDelete = (definition: WorkflowDefinitionDto) => {
    setDefinitionToDelete(definition);
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!definitionToDelete) return;

    try {
      await workflowService.deleteDefinition(definitionToDelete.id);
      toast.success('Workflow definition deleted successfully');
      loadDefinitions();
    } catch (error) {
      console.error('Failed to delete definition:', error);
      toast.error('Failed to delete workflow definition');
    } finally {
      setDeleteDialogOpen(false);
      setDefinitionToDelete(null);
    }
  };

  const openUnpublish = (d: WorkflowDefinitionDto) => { setTargetDefinition(d); setUnpublishDialogOpen(true); };
  const openArchive = (d: WorkflowDefinitionDto) => { setTargetDefinition(d); setArchiveDialogOpen(true); };
  const openTerminate = (d: WorkflowDefinitionDto) => { setTargetDefinition(d); setTerminateDialogOpen(true); };

  const handleUnpublishConfirm = async () => {
    if (!targetDefinition) return;
    try {
      await workflowService.unpublishDefinition(targetDefinition.id);
      toast.success('Definition unpublished');
      loadDefinitions();
    } catch (e) {
      toast.error('Unpublish failed');
    } finally {
      setUnpublishDialogOpen(false); setTargetDefinition(null);
    }
  };

  const handleArchiveConfirm = async () => {
    if (!targetDefinition) return;
    try {
      await workflowService.archiveDefinition(targetDefinition.id);
      toast.success('Definition archived');
      loadDefinitions();
    } catch {
      toast.error('Archive failed');
    } finally {
      setArchiveDialogOpen(false); setTargetDefinition(null);
    }
  };

  const handleTerminateInstancesConfirm = async () => {
    if (!targetDefinition) return;
    try {
      const result = await workflowService.terminateDefinitionInstances(targetDefinition.id);
      toast.success(`Terminated ${result.terminated} running instance(s)`);
    } catch {
      toast.error('Terminate running instances failed');
    } finally {
      setTerminateDialogOpen(false); setTargetDefinition(null);
    }
  };

  const columns: GridColDef[] = [
    {
      field: 'name',
      headerName: 'Name',
      flex: 1,
      minWidth: 200,
    },
    {
      field: 'description',
      headerName: 'Description',
      flex: 1,
      minWidth: 250,
      renderCell: (params) => params.value || 'No description',
    },
    {
      field: 'version',
      headerName: 'Version',
      width: 100,
      renderCell: (params) => `v${params.value}`,
    },
    {
      field: 'isPublished',
      headerName: 'Status',
      width: 120,
      renderCell: (params) => (
        params.value ? (
          <Chip label="Published" color="success" size="small" icon={<PublishIcon />} />
        ) : (
          <Chip label="Draft" color="warning" size="small" variant="outlined" />
        )
      ),
    },
    {
      field: 'createdAt',
      headerName: 'Created',
      width: 120,
      type: 'date',
      valueGetter: (value) => new Date(value),
    },
    {
      field: 'publishedAt',
      headerName: 'Published',
      width: 120,
      type: 'date',
      valueGetter: (value) => value ? new Date(value) : null,
      renderCell: (params) => {
        if (!params.value) return 'Not published';
        return new Date(params.value).toLocaleDateString();
      },
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 120,
      getActions: (params: GridRowParams) => {
        const definition = params.row as WorkflowDefinitionDto;
        const actions = [
          <GridActionsCellItem
            icon={<ViewIcon />}
            label="View"
            onClick={() => handleView(params.id)}
          />,
        ];

        // Edit action (only for drafts)
        if (!definition.isPublished) {
          actions.push(
            <GridActionsCellItem
              icon={<EditIcon />}
              label="Edit"
              onClick={() => handleEdit(params.id)}
              showInMenu
            />
          );
        }

        // Duplicate action
        actions.push(
          <GridActionsCellItem
            icon={<DuplicateIcon />}
            label="Duplicate"
            onClick={() => handleDuplicate(definition)}
            showInMenu
          />
        );

        // Publish action (only for drafts)
        if (!definition.isPublished) {
          actions.push(
            <GridActionsCellItem
              icon={<PublishIcon />}
              label="Publish"
              onClick={() => handlePublish(definition)}
              showInMenu
            />
          );
        }

        // Start instance action (only for published)
        if (definition.isPublished) {
          actions.push(
            <GridActionsCellItem
              icon={<StartIcon />}
              label="Start Instance"
              onClick={() => handleStartInstance(definition)}
              showInMenu
            />
          );
        }

        // Unpublish, Archive, Terminate actions (only for published)
        if (definition.isPublished) {
          actions.push(
            <GridActionsCellItem
              icon={<UnpublishIcon />}
              label="Unpublish"
              onClick={() => openUnpublish(definition)}
              showInMenu
            />,
            <GridActionsCellItem
              icon={<ArchiveIcon />}
              label="Archive"
              onClick={() => openArchive(definition)}
              showInMenu
            />,
            <GridActionsCellItem
              icon={<TerminateIcon />}
              label="Terminate Instances"
              onClick={() => openTerminate(definition)}
              showInMenu
            />
          );
        }

        // Delete action (only for drafts)
        if (!definition.isPublished) {
          actions.push(
            <GridActionsCellItem
              icon={<DeleteIcon />}
              label="Delete"
              onClick={() => handleDelete(definition)}
              showInMenu
            />
          );
        }

        return actions;
      },
    },
  ];

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view workflow definitions</Typography>
      </Box>
    );
  }

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Workflow Definitions
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Typography>

        <CanAccess permission="workflow.write">
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={handleCreateNew}
          >
            Create Workflow
          </Button>
        </CanAccess>
      </Box>

      <Box sx={{ flex: 1, minHeight: 0 }}>
        <DataGridPremium
          rows={definitions}
          columns={columns}
          loading={loading}
          pagination
          pageSizeOptions={[10, 25, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 10 } },
          }}
          slots={{
            toolbar: GridToolbar,
          }}
          slotProps={{
            toolbar: {
              showQuickFilter: true,
              quickFilterProps: { debounceMs: 500 },
            },
          }}
          disableRowSelectionOnClick
          sx={{
            '& .MuiDataGrid-row:hover': {
              backgroundColor: 'action.hover',
            },
          }}
        />
      </Box>

      {/* Dialogs remain the same */}
      <Dialog open={publishDialogOpen} onClose={() => setPublishDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Publish Workflow Definition</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            Are you sure you want to publish "{definitionToPublish?.name}"? 
            Once published, the definition becomes immutable and can be used to start workflow instances.
          </Typography>
          <TextField
            fullWidth
            label="Publish Notes (Optional)"
            multiline
            rows={3}
            value={publishNotes}
            onChange={(e) => setPublishNotes(e.target.value)}
            placeholder="Add notes about this publication..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPublishDialogOpen(false)}>Cancel</Button>
          <Button onClick={handlePublishConfirm} color="primary" variant="contained">
            Publish
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete workflow definition "{definitionToDelete?.name}"?
            This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDeleteConfirm} color="error" variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={unpublishDialogOpen} onClose={() => setUnpublishDialogOpen(false)}>
        <DialogTitle>Unpublish Definition</DialogTitle>
        <DialogContent>
          <Typography>
            Unpublish "{targetDefinition?.name}"? New instances cannot start; existing continue running.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setUnpublishDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleUnpublishConfirm} variant="contained">Unpublish</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={archiveDialogOpen} onClose={() => setArchiveDialogOpen(false)}>
        <DialogTitle>Archive Definition</DialogTitle>
        <DialogContent>
          <Typography>
            Archive "{targetDefinition?.name}"? It will be hidden from normal lists.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setArchiveDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleArchiveConfirm} variant="contained" color="warning">Archive</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={terminateDialogOpen} onClose={() => setTerminateDialogOpen(false)}>
        <DialogTitle>Terminate Running Instances</DialogTitle>
        <DialogContent>
          <Typography>
            Terminate all running instances of "{targetDefinition?.name}"? This cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setTerminateDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleTerminateInstancesConfirm} variant="contained" color="error">Terminate All</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default DefinitionsPage;
