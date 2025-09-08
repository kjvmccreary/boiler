import { useState, useEffect, useCallback } from 'react';
import {
  Box,
  Typography,
  Button,
  Chip,
  Tooltip,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Switch,
  FormControlLabel,
  Divider,
  Stack
} from '@mui/material';
import {
  DataGridPremium,
  GridColDef,
  GridRowParams,
  GridActionsCellItem,
  GridRowId,
  GridToolbar
} from '@mui/x-data-grid-premium';
import {
  Add as AddIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Publish as PublishIcon,
  PlayArrow as StartIcon,
  Visibility as ViewIcon,
  FileCopy as DuplicateIcon,
  Gavel as UnpublishIcon,
  Archive as ArchiveIcon,
  Cancel as TerminateIcon,
  InfoOutlined as InfoIcon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess';
import { workflowService } from '@/services/workflow.service';
import type { WorkflowDefinitionDto } from '@/types/workflow';
import toast from 'react-hot-toast';
import { useTenant } from '@/contexts/TenantContext';
import { normalizeTags } from '@/utils/tags';

const LS_KEY_SHOW_ARCHIVED = 'wf.definitions.showArchived';
const LS_KEY_ANY_TAGS = 'wf.definitions.anyTags';
const LS_KEY_ALL_TAGS = 'wf.definitions.allTags';

function readStoredShowArchived(defaultValue: boolean): boolean {
  if (typeof window === 'undefined') return defaultValue;
  const raw = window.localStorage.getItem(LS_KEY_SHOW_ARCHIVED);
  if (raw === null) return defaultValue;
  return raw === 'true';
}

function formatDate(value?: string | Date | null): string {
  if (!value) return '—';
  const d = typeof value === 'string' ? new Date(value) : value;
  if (isNaN(d.getTime())) return '—';
  return d.toLocaleDateString();
}

function splitTags(raw?: string | null): string[] {
  if (!raw) return [];
  return raw
    .split(/[, ]+/)
    .map(t => t.trim())
    .filter(Boolean);
}

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
  const [showArchived, setShowArchived] = useState<boolean>(() => readStoredShowArchived(false));
  const [anyTags, setAnyTags] = useState<string>(() => {
    if (typeof window === 'undefined') return '';
    return localStorage.getItem(LS_KEY_ANY_TAGS) || '';
  });
  const [allTags, setAllTags] = useState<string>(() => {
    if (typeof window === 'undefined') return '';
    return localStorage.getItem(LS_KEY_ALL_TAGS) || '';
  });
  const [filtersDirty, setFiltersDirty] = useState(false);
  // Tag editing dialog
  const [tagsDialogOpen, setTagsDialogOpen] = useState(false);
  const [tagsTargetDefinition, setTagsTargetDefinition] = useState<WorkflowDefinitionDto | null>(null);
  const [tagsInput, setTagsInput] = useState('');
  const [savingTags, setSavingTags] = useState(false);
  // Tag validation state
  const [tagsError, setTagsError] = useState<string | null>(null);

  // Validation policy
  const MAX_TAGS = 12;
  const MAX_TAG_LENGTH = 40;

  function validateTagsDraft(raw: string): { error: string | null; normalized: string[] } {
    if (!raw.trim()) return { error: null, normalized: [] };
    const norm = normalizeTags(raw).normalized;
    if (norm.length > MAX_TAGS) {
      return { error: `Too many tags (max ${MAX_TAGS})`, normalized: norm };
    }
    const tooLong = norm.find(t => t.length > MAX_TAG_LENGTH);
    if (tooLong) {
      return { error: `Tag "${tooLong}" exceeds ${MAX_TAG_LENGTH} characters`, normalized: norm };
    }
    return { error: null, normalized: norm };
  }

  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  // Persist toggle
  useEffect(() => {
    if (typeof window !== 'undefined') {
      window.localStorage.setItem(LS_KEY_SHOW_ARCHIVED, String(showArchived));
    }
  }, [showArchived]);

  useEffect(() => {
    if (currentTenant) {
      loadDefinitions();
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [currentTenant, showArchived]);

  const loadDefinitions = async () => {
    try {
      setLoading(true);
      console.log('🔄 DefinitionsPage: Loading workflow definitions');
      // Explicit paging & newest-first; honor archived toggle.
      const response = await workflowService.getDefinitions({
        page: 1,
        pageSize: 100,
        sortBy: 'createdAt',
        desc: true,
        includeArchived: showArchived,
        anyTags: anyTags.trim() || undefined,
        allTags: allTags.trim() || undefined
      });
      if (!Array.isArray(response)) {
        setDefinitions([]);
        return;
      }
      setDefinitions(response);
    } catch (error) {
      console.error('Failed to load definitions:', error);
      toast.error('Failed to load workflow definitions');
      setDefinitions([]);
    } finally {
      setLoading(false);
    }
  };

  const handleCreateNew = () => navigate('/app/workflow/builder/new');
  const handleEdit = (id: GridRowId) => navigate(`/app/workflow/builder/${id}`);
  const handleView = (id: GridRowId) => navigate(`/app/workflow/definitions/${id}`);

  const handleDuplicate = async (definition: WorkflowDefinitionDto) => {
    try {
      await workflowService.createDraft({
        name: `${definition.name} (Copy)`,
        jsonDefinition: definition.jsonDefinition,
        description: definition.description ?? undefined
      });
      toast.success('Workflow definition duplicated');
      loadDefinitions();
    } catch {
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
      toast.success('Workflow definition published');
      loadDefinitions();
    } catch {
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
      } as any);
      if (!instance?.id) {
        toast.error('Instance started but response invalid');
        return;
      }
      toast.success('Workflow instance started');
      navigate(`/app/workflow/instances/${instance.id}`);
    } catch {
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
      toast.success('Workflow definition deleted');
      loadDefinitions();
    } catch {
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
    } catch {
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
      field: 'tags',
      headerName: 'Tags',
      minWidth: 160,
      flex: 1,
      sortable: true,
      valueGetter: (params) => (params.row?.tags || ''),
      sortComparator: (v1, v2) => {
        // Basic alphabetical by first tag
        const a = (v1 as string || '').split(',')[0]?.toLowerCase() || '';
        const b = (v2 as string || '').split(',')[0]?.toLowerCase() || '';
        if (a < b) return -1;
        if (a > b) return 1;
        return 0;
      },
      renderCell: (params) => {
        const raw = params.row?.tags as string | undefined;
        if (!raw) return <Typography variant="caption" color="text.disabled">—</Typography>;
        const tags = raw.split(',').filter(t => t);
        const shown = tags.slice(0, 3);
        const extra = tags.length - shown.length;
        return (
          <Box sx={{ display: 'flex', gap: 0.5, flexWrap: 'nowrap', alignItems: 'center', overflow: 'hidden' }}>
            {shown.map(t => <Chip key={t} label={t} size="small" sx={{ maxWidth: 90 }} />)}
            {extra > 0 && <Chip label={`+${extra}`} size="small" variant="outlined" />}
          </Box>
        );
      }
    },
    {
      field: 'name',
      headerName: 'Name',
      flex: 1,
      minWidth: 200,
      renderCell: (params) => {
        const def = params.row as WorkflowDefinitionDto;
        return (
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
            <span>{def.name}</span>
            {def.isArchived && (
              <Chip label="Archived" size="small" color="default" variant="outlined" />
            )}
          </Box>
        );
      }
    },
    {
      field: 'description',
      headerName: 'Description',
      flex: 1,
      minWidth: 250,
      renderCell: (params) => params.value || '—',
    },
    {
      field: 'version',
      headerName: 'Version',
      width: 90,
      renderCell: (params) => `v${params.value}`,
    },
    {
      field: 'isPublished',
      headerName: 'Status',
      width: 130,
      renderCell: (params) => {
        const def = params.row as WorkflowDefinitionDto;
        if (def.isArchived) return <Chip label="Archived" size="small" color="default" />;
        return params.value
          ? <Chip label="Published" color="success" size="small" icon={<PublishIcon />} />
          : <Chip label="Draft" color="warning" size="small" variant="outlined" />;
      },
    },
    {
      field: 'activeInstanceCount',
      headerName: 'Active',
      width: 90,
      type: 'number',
      renderCell: (params) => (params.row?.activeInstanceCount ?? 0) || 0
    },
    {
      field: 'createdAt',
      headerName: 'Created',
      width: 130,
      type: 'date',
      valueGetter: (value) => value ? new Date(value as string) : null,
      renderCell: (params) => formatDate(params.value)
    },
    {
      field: 'publishedAt',
      headerName: 'Published',
      width: 130,
      type: 'date',
      valueGetter: (value) => value ? new Date(value as string) : null,
      renderCell: (params) => formatDate(params.value),
    },
    {
      field: 'actions',
      type: 'actions',
      headerName: 'Actions',
      width: 140,
      getActions: (params: GridRowParams) => {
        const definition = params.row as WorkflowDefinitionDto;
        const actions = [
          <GridActionsCellItem
            key="view"
            icon={<ViewIcon />}
            label="View"
            onClick={() => handleView(params.id)}
          />,
          <GridActionsCellItem
            key="info"
            icon={<InfoIcon />}
            label="Details"
            onClick={() => {
              // Toggle detail panel by emitting built-in event
              const grid = document.querySelector('[role="grid"]');
              if (grid) {
                // DataGridPremium toggles with keyboard/row click; here we rely on row click fallback.
                // This action serves as a hint; manual click can also expand.
              }
            }}
            showInMenu
          />
        ];

        if (!definition.isPublished && !definition.isArchived) {
          actions.push(
            <GridActionsCellItem
              key="edit"
              icon={<EditIcon />}
              label="Edit"
              onClick={() => handleEdit(params.id)}
              showInMenu
            />
          );
        }

        actions.push(
          <GridActionsCellItem
            key="dup"
            icon={<DuplicateIcon />}
            label="Duplicate"
            onClick={() => handleDuplicate(definition)}
            showInMenu
          />
        );

        if (!definition.isPublished && !definition.isArchived) {
          actions.push(
            <GridActionsCellItem
              key="publish"
              icon={<PublishIcon />}
              label="Publish"
              onClick={() => handlePublish(definition)}
              showInMenu
            />,
            <GridActionsCellItem
              key="editTagsDraft"
              icon={<EditIcon />}
              label="Edit Tags"
              onClick={() => {
                setTagsTargetDefinition(definition);
                setTagsInput(definition.tags || '');
                setTagsDialogOpen(true);
              }}
              showInMenu
            />
          );
        }

        if (definition.isPublished && !definition.isArchived) {
          actions.push(
            <GridActionsCellItem
              key="start"
              icon={<StartIcon />}
              label="Start Instance"
              onClick={() => handleStartInstance(definition)}
              showInMenu
            />,
            <GridActionsCellItem
              key="editTags"
              icon={<EditIcon />}
              label="Edit Tags"
              onClick={() => {
                setTagsTargetDefinition(definition);
                setTagsInput(definition.tags || '');
                setTagsDialogOpen(true);
              }}
              showInMenu
            />,
            <GridActionsCellItem
              key="unpub"
              icon={<UnpublishIcon />}
              label="Unpublish"
              onClick={() => openUnpublish(definition)}
              showInMenu
            />,
            <GridActionsCellItem
              key="archive"
              icon={<ArchiveIcon />}
              label="Archive"
              onClick={() => openArchive(definition)}
              showInMenu
            />,
            <GridActionsCellItem
              key="terminate"
              icon={<TerminateIcon />}
              label="Terminate Instances"
              onClick={() => openTerminate(definition)}
              showInMenu
            />
          );
        }

        if (!definition.isPublished && !definition.isArchived) {
          actions.push(
            <GridActionsCellItem
              key="delete"
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

  // Detail panel for rich metadata
  const getDetailPanelContent = useCallback<(p: GridRowParams) => React.ReactNode>((params) => {
    const def = params.row as WorkflowDefinitionDto;
    const tags = splitTags(def.tags);
    return (
      <Box
        sx={{
          p: 2,
          bgcolor: 'background.default',
          borderTop: (theme) => `1px solid ${theme.palette.divider}`,
          display: 'flex',
          flexDirection: 'column',
          gap: 2
        }}
      >
        <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>
          Metadata for: {def.name} (v{def.version})
        </Typography>
        <Stack direction="row" spacing={4} flexWrap="wrap" useFlexGap>
          <Box sx={{ minWidth: 240 }}>
            <Typography variant="caption" color="text.secondary">Publish Notes</Typography>
            <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap' }}>
              {def.publishNotes?.trim() || '—'}
            </Typography>
          </Box>
          <Box sx={{ minWidth: 240 }}>
            <Typography variant="caption" color="text.secondary">Version Notes</Typography>
            <Typography variant="body2" sx={{ whiteSpace: 'pre-wrap' }}>
              {def.versionNotes?.trim() || '—'}
            </Typography>
          </Box>
            <Box sx={{ minWidth: 160 }}>
              <Typography variant="caption" color="text.secondary">Active Instances</Typography>
              <Typography variant="body2">
                {def.activeInstanceCount ?? 0}
              </Typography>
            </Box>
            <Box sx={{ minWidth: 220 }}>
              <Typography variant="caption" color="text.secondary">
                Tags <Tooltip
                  title={
                    <Box sx={{ p: 0.5 }}>
                      <Typography variant="caption">
                        Filtering:
                        <br />All Tags (AND): show definitions containing every tag entered.
                        <br />Any Tags (OR): show definitions containing at least one.
                        <br />Backend: legacy 'tags' param = OR (deprecated).
                      </Typography>
                    </Box>
                  }
                  placement="top"
                  arrow
                >
                  <InfoIcon fontSize="inherit" sx={{ ml: 0.5, verticalAlign: 'middle', opacity: 0.7 }} />
                </Tooltip>
              </Typography>
              <Box sx={{ mt: 0.5, display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                {tags.length === 0 && <Typography variant="body2">—</Typography>}
                {tags.map(t => (
                  <Chip key={t} size="small" label={t} variant="outlined" />
                ))}
              </Box>
            </Box>
            <Box sx={{ minWidth: 200 }}>
              <Typography variant="caption" color="text.secondary">State & Timestamps</Typography>
              <Typography variant="body2">
                {def.isPublished ? 'Published' : 'Draft'}
                {def.isArchived ? ' • Archived' : ''}
              </Typography>
              <Typography variant="caption" color="text.secondary">
                Created: {formatDate(def.createdAt)}
              </Typography><br />
              <Typography variant="caption" color="text.secondary">
                Published: {formatDate(def.publishedAt as any)}
              </Typography><br />
              {def.isArchived && (
                <Typography variant="caption" color="text.secondary">
                  Archived: {formatDate(def.archivedAt as any)}
                </Typography>
              )}
            </Box>
        </Stack>
        <Divider />
        <Typography variant="caption" color="text.secondary">
          Definition Id: {def.id} • Parent Definition Id: {def.parentDefinitionId ?? '—'}
        </Typography>
      </Box>
    );
  }, []);

  const getDetailPanelHeight = useCallback(() => 210, []);

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view workflow definitions</Typography>
      </Box>
    );
    }

  return (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3, flexWrap: 'wrap', gap: 2 }}>
        <Box>
          <Typography variant="h4" component="h1">
            Workflow Definitions
          </Typography>
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', alignItems: 'center', gap: 3 }}>
          <FormControlLabel
            control={
              <Switch
                checked={showArchived}
                onChange={(e) => setShowArchived(e.target.checked)}
                color="primary"
              />
            }
            label="Show Archived"
          />
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
      </Box>

      <Box sx={{ flex: 1, minHeight: 0 }}>
        {/* Advanced Filters */}
        <Box
          sx={{
            mb: 2,
            p: 2,
            backgroundColor: 'background.paper',
            border: theme => `1px solid ${theme.palette.divider}`,
            borderRadius: 1,
            display: 'flex',
            flexWrap: 'wrap',
            gap: 2,
            alignItems: 'flex-end'
          }}
        >
          <TextField
            label="All Tags (AND)"
            size="small"
            value={allTags}
            onChange={(e) => {
              setAllTags(e.target.value);
              setFiltersDirty(true);
            }}
            placeholder="e.g. billing,core"
            sx={{ minWidth: 220 }}
          />
          <TextField
            label="Any Tags (OR)"
            size="small"
            value={anyTags}
            onChange={(e) => {
              setAnyTags(e.target.value);
              setFiltersDirty(true);
            }}
            placeholder="e.g. finance,audit"
            sx={{ minWidth: 220 }}
          />
          <Button
            variant="contained"
            disabled={loading}
            onClick={() => {
              loadDefinitions();
              setFiltersDirty(false);
              // Persist last-used applied filters
              if (anyTags.trim()) localStorage.setItem(LS_KEY_ANY_TAGS, normalizeTags(anyTags).canonicalQuery);
              else localStorage.removeItem(LS_KEY_ANY_TAGS);
              if (allTags.trim()) localStorage.setItem(LS_KEY_ALL_TAGS, normalizeTags(allTags).canonicalQuery);
              else localStorage.removeItem(LS_KEY_ALL_TAGS);
            }}
          >
            Apply Filters
          </Button>
          <Button
            variant="text"
            disabled={loading || (!anyTags && !allTags && !filtersDirty)}
            onClick={() => {
              setAnyTags('');
              setAllTags('');
              setFiltersDirty(false);
              loadDefinitions();
              localStorage.removeItem(LS_KEY_ANY_TAGS);
              localStorage.removeItem(LS_KEY_ALL_TAGS);
            }}
          >
            Reset
          </Button>
          {filtersDirty && (
            <Typography variant="caption" color="warning.main">
              Unsaved filter changes
            </Typography>
          )}

          {/* Chips display for resolved tags */}
          {(allTags.trim() || anyTags.trim()) && (
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, width: '100%', mt: 1 }}>
              {normalizeTags(allTags).normalized.map(tag => (
                <Chip
                  key={`all-${tag}`}
                  size="small"
                  color="primary"
                  label={`ALL: ${tag}`}
                  onDelete={() => {
                    const remaining = normalizeTags(allTags).normalized.filter(t => t !== tag);
                    setAllTags(remaining.join(','));
                    setFiltersDirty(true);
                  }}
                  variant="outlined"
                />
              ))}
              {normalizeTags(anyTags).normalized.map(tag => (
                <Chip
                  key={`any-${tag}`}
                  size="small"
                  color="secondary"
                  label={`ANY: ${tag}`}
                  onDelete={() => {
                    const remaining = normalizeTags(anyTags).normalized.filter(t => t !== tag);
                    setAnyTags(remaining.join(','));
                    setFiltersDirty(true);
                  }}
                  variant="outlined"
                />
              ))}
              {(!normalizeTags(allTags).normalized.length && !normalizeTags(anyTags).normalized.length) && (
                <Typography variant="caption" color="text.secondary">
                  No active tag filters
                </Typography>
              )}
            </Box>
          )}
        </Box>

        <DataGridPremium
          rows={definitions}
          columns={columns}
          getRowId={(r) => r.id}
          loading={loading}
          pagination
          pageSizeOptions={[10, 25, 50, 100]}
          initialState={{
            pagination: { paginationModel: { pageSize: 100 } },
          }}
          // NOTE: Grid page size changes are client-only until we wire server paging controls.
          // With pageSize=100 and low counts, a single fetch suffices.
          getDetailPanelContent={getDetailPanelContent}
          getDetailPanelHeight={getDetailPanelHeight}
          slots={{
            toolbar: GridToolbar
          }}
            slotProps={{
            toolbar: {
              showQuickFilter: true,
              quickFilterProps: { debounceMs: 500 }
            }
          }}
          disableRowSelectionOnClick
          sx={{
            '& .MuiDataGrid-row:hover': { backgroundColor: 'action.hover' }
          }}
        />
      </Box>

      {/* Publish Dialog */}
      <Dialog open={publishDialogOpen} onClose={() => setPublishDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Publish Workflow Definition</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            Publish "{definitionToPublish?.name}"? Published versions become immutable and usable for instances.
          </Typography>
          <TextField
            fullWidth
            label="Publish Notes (Optional)"
            multiline
            rows={3}
            value={publishNotes}
            onChange={(e) => setPublishNotes(e.target.value)}
            placeholder="Describe the changes or reason for publishing..."
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setPublishDialogOpen(false)}>Cancel</Button>
          <Button onClick={handlePublishConfirm} color="primary" variant="contained">
            Publish
          </Button>
        </DialogActions>
      </Dialog>

      {/* Delete Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Delete "{definitionToDelete?.name}"? This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDeleteConfirm} color="error" variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>

      {/* Unpublish Dialog */}
      <Dialog open={unpublishDialogOpen} onClose={() => setUnpublishDialogOpen(false)}>
        <DialogTitle>Unpublish Definition</DialogTitle>
        <DialogContent>
          <Typography>
            Unpublish "{targetDefinition?.name}"? New instances cannot start; existing ones continue.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setUnpublishDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleUnpublishConfirm} variant="contained">Unpublish</Button>
        </DialogActions>
      </Dialog>

      {/* Archive Dialog */}
      <Dialog open={archiveDialogOpen} onClose={() => setArchiveDialogOpen(false)}>
        <DialogTitle>Archive Definition</DialogTitle>
        <DialogContent>
          <Typography>
            Archive "{targetDefinition?.name}"? It will be hidden unless "Show Archived" is enabled.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setArchiveDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleArchiveConfirm} variant="contained" color="warning">Archive</Button>
        </DialogActions>
      </Dialog>

      {/* Terminate Instances Dialog */}
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

      {/* Edit Tags Dialog */}
      <Dialog
        open={tagsDialogOpen}
        onClose={() => (!savingTags && setTagsDialogOpen(false))}
        maxWidth="sm"
        fullWidth
      >
        <DialogTitle>Edit Tags{tagsTargetDefinition ? ` – ${tagsTargetDefinition.name}` : ''}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 2 }}>
          <Typography variant="body2" color="text.secondary">
            Enter comma-separated tags. Multi-word tags allowed. They will be normalized on save.
          </Typography>
          <TextField
            label="Tags"
            value={tagsInput}
            onChange={(e) => {
              setTagsInput(e.target.value);
-              // validation deferred until save
+              const v = validateTagsDraft(e.target.value);
+              setTagsError(v.error);
             }}
             placeholder="e.g. billing,core,financial reporting"
             disabled={savingTags}
             fullWidth
             multiline
             minRows={2}
+            error={!!tagsError}
+            helperText={tagsError || ' '}
           />
           {/* Preview chips */}
           <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mt: 0.5 }}>
             {normalizeTags(tagsInput).normalized.length === 0 && (
               <Typography variant="caption" color="text.secondary">
                 No tags
               </Typography>
             )}
             {normalizeTags(tagsInput).normalized.map(t => (
               <Chip
                 key={t}
                 label={t}
                 size="small"
                 onDelete={() => {
                   const remaining = normalizeTags(tagsInput).normalized.filter(x => x !== t);
                   setTagsInput(remaining.join(','));
+                  const v = validateTagsDraft(remaining.join(','));
+                  setTagsError(v.error);
                 }}
               />
             ))}
           </Box>
           <Typography variant="caption" color="text.secondary">
-            Max recommended tags: descriptive & concise.
+            Limits: up to {MAX_TAGS} tags; each ≤ {MAX_TAG_LENGTH} chars.
           </Typography>
         </DialogContent>
         <DialogActions>
           <Button
             onClick={() => setTagsDialogOpen(false)}
             disabled={savingTags}
           >
             Cancel
           </Button>
             <Button
               variant="contained"
-              disabled={savingTags}
+              disabled={savingTags || !!tagsError}
               onClick={async () => {
                 if (!tagsTargetDefinition) return;
+                const validation = validateTagsDraft(tagsInput);
+                if (validation.error) {
+                  setTagsError(validation.error);
+                  return;
+                }
                 setSavingTags(true);
                 try {
                   const norm = normalizeTags(tagsInput);
                   await workflowService.updateDefinition(tagsTargetDefinition.id, {
                     tags: norm.canonicalQuery || undefined
                   });
                   toast.success('Tags updated');
                   await loadDefinitions();
                   setTagsDialogOpen(false);
                 } catch {
                   toast.error('Failed to update tags');
                 } finally {
                   setSavingTags(false);
                 }
               }}
             >
               {savingTags ? 'Saving...' : 'Save'}
             </Button>
         </DialogActions>
       </Dialog>
     </Box>
   );
 }
 
 export default DefinitionsPage;
