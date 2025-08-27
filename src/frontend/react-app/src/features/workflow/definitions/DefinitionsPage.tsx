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
      const response = await workflowService.startInstance({
        workflowDefinitionId: definition.id,
        initialContext: '{}',
        startNotes: 'Started from definitions page'
      });

      toast.success('Workflow instance started successfully');
      // ✅ FIX: Add /app prefix
      navigate(`/app/workflow/instances/${response.id}`);
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

  // ... rest of the component remains the same

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
    </Box>
  );
}

export default DefinitionsPage;
