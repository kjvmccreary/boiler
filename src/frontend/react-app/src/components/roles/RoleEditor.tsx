import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  Typography,
  TextField,
  Button,
  Grid,
  Divider,
  Alert,
  CircularProgress,
  List,
  ListItem,
  ListItemText,
  Avatar,
  Chip,
  Accordion,
  AccordionSummary,
  AccordionDetails,
} from '@mui/material';
import {
  Save as SaveIcon,
  Cancel as CancelIcon,
  People as PeopleIcon,
  ExpandMore as ExpandMoreIcon,
  AccountCircle as AccountCircleIcon,
  Info as InfoIcon,
} from '@mui/icons-material';
import { PermissionSelector } from './PermissionSelector.js';
import { RoleWorkflowUsageDialog } from './RoleWorkflowUsageDialog'; // ✅ ADD: Import the dialog
import { roleService, type RoleCreateRequest, type RoleUpdateRequest } from '@/services/role.service.js';
import type { Role, UserInfo, RoleUsageInWorkflowsDto } from '@/types/index.js'; // ✅ ADD: Import the type
import toast from 'react-hot-toast';
import { ROUTES } from '@/routes/route.constants.js';

export function RoleEditor() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditing = Boolean(id && id !== 'new');

  const [role, setRole] = useState<Role | null>(null);
  const [users, setUsers] = useState<UserInfo[]>([]);
  const [usersLoading, setUsersLoading] = useState(false);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    permissions: [] as string[],
  });
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  // ✅ NEW: State for workflow usage dialog
  const [workflowUsageDialog, setWorkflowUsageDialog] = useState<{
    open: boolean;
    usageInfo: RoleUsageInWorkflowsDto | null;
    pendingUpdate: RoleUpdateRequest | null;
    actionType: 'rename' | 'delete';
  }>({
    open: false,
    usageInfo: null,
    pendingUpdate: null,
    actionType: 'rename'
  });

  const loadRole = async (roleId: string) => {
    try {
      setLoading(true);
      const roleData = await roleService.getRoleById(parseInt(roleId));
      setRole(roleData);
      setFormData({
        name: roleData.name,
        description: roleData.description || '',
        permissions: roleData.permissions.map((p: any) => typeof p === 'string' ? p : p.name),
      });

      // Load users assigned to this role
      setUsersLoading(true);
      try {
        console.log('🔍 RoleEditor: Loading users for role:', parseInt(roleId));
        const usersData = await roleService.getRoleUsers(parseInt(roleId));
        console.log('🔍 RoleEditor: Users data received:', usersData);
        setUsers(usersData);
      } catch (error) {
        console.error('❌ RoleEditor: Failed to load role users:', error);
        // Don't fail the whole component if users can't be loaded
        toast.error('Could not load users for this role');
        setUsers([]);
      } finally {
        setUsersLoading(false);
      }

    } catch (error) {
      console.error('Failed to load role:', error);
      toast.error('Failed to load role');
      navigate(ROUTES.ROLES);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (isEditing && id) {
      loadRole(id);
    }
  }, [id, isEditing]);

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.name.trim()) {
      newErrors.name = 'Role name is required';
    } else if (formData.name.length < 2) {
      newErrors.name = 'Role name must be at least 2 characters';
    }

    if (formData.permissions.length === 0) {
      newErrors.permissions = 'At least one permission must be selected';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  // ✅ ENHANCED: Update the handleSubmit method to include workflow validation
  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!validateForm()) {
      return;
    }

    // Check if trying to delete permissions when users are assigned
    if (isEditing && users.length > 0 && formData.permissions.length === 0) {
      toast.error('Cannot remove all permissions from a role that has users assigned');
      return;
    }

    try {
      setSaving(true);

      if (isEditing && id) {
        const updateData: RoleUpdateRequest = {
          name: formData.name.trim(),
          description: formData.description.trim() || undefined,
          permissions: formData.permissions,
        };

        // ✅ NEW: Check for workflow usage before updating if name is changing
        if (role && role.name !== updateData.name) {
          console.log('🔍 RoleEditor: Role name changing, checking workflow usage...');
          const result = await roleService.updateRoleWithValidation(parseInt(id), updateData);
          
          if (result.success) {
            toast.success('Role updated successfully');
            navigate(ROUTES.ROLES);
            return;
          } else if (result.workflowUsage) {
            // Show warning dialog
            console.log('⚠️ RoleEditor: Role is used in workflows, showing dialog');
            setWorkflowUsageDialog({
              open: true,
              usageInfo: result.workflowUsage,
              pendingUpdate: updateData,
              actionType: 'rename'
            });
            return;
          } else {
            toast.error(result.error || 'Failed to update role');
            return;
          }
        } else {
          // Name not changing, use regular update
          await roleService.updateRole(parseInt(id), updateData);
          toast.success('Role updated successfully');
        }
      } else {
        const createData: RoleCreateRequest = {
          name: formData.name.trim(),
          description: formData.description.trim() || undefined,
          permissions: formData.permissions,
        };
        await roleService.createRole(createData);
        toast.success('Role created successfully');
      }

      navigate(ROUTES.ROLES);
    } catch (error) {
      console.error('Failed to save role:', error);
      toast.error(`Failed to ${isEditing ? 'update' : 'create'} role`);
    } finally {
      setSaving(false);
    }
  };

  // ✅ NEW: Handle proceeding with update despite workflow warnings
  const handleProceedWithUpdate = async () => {
    if (!workflowUsageDialog.pendingUpdate || !id) return;
    
    try {
      setSaving(true);
      
      // Force update by calling the regular update method
      await roleService.updateRole(parseInt(id), {
        ...workflowUsageDialog.pendingUpdate,
        forceUpdate: true // This flag might need to be handled by your backend
      });
      
      toast.success('Role updated successfully (workflow definitions may need attention)');
      setWorkflowUsageDialog({ 
        open: false, 
        usageInfo: null, 
        pendingUpdate: null, 
        actionType: 'rename' 
      });
      navigate(ROUTES.ROLES);
    } catch (error) {
      console.error('Failed to force update role:', error);
      toast.error('Failed to update role');
    } finally {
      setSaving(false);
    }
  };

  // ✅ NEW: Handle closing the workflow usage dialog
  const handleCloseWorkflowDialog = () => {
    setWorkflowUsageDialog({ 
      open: false, 
      usageInfo: null, 
      pendingUpdate: null, 
      actionType: 'rename' 
    });
  };

  const handleInputChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({
      ...prev,
      [field]: event.target.value,
    }));

    // Clear error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
  };

  const handlePermissionsChange = (permissions: string[]) => {
    setFormData(prev => ({
      ...prev,
      permissions,
    }));

    // Clear permissions error
    if (errors.permissions) {
      setErrors(prev => ({
        ...prev,
        permissions: '',
      }));
    }
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        {isEditing ? 'Edit Role' : 'Create New Role'}
      </Typography>

      {role?.isSystemRole && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          This is a system role and cannot be modified.
        </Alert>
      )}

      <form onSubmit={handleSubmit}>
        <Grid container spacing={3}>
          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Role Information
                </Typography>

                <TextField
                  fullWidth
                  label="Role Name"
                  value={formData.name}
                  onChange={handleInputChange('name')}
                  error={!!errors.name}
                  helperText={errors.name}
                  disabled={role?.isSystemRole || saving}
                  sx={{ mb: 2 }}
                />

                <TextField
                  fullWidth
                  label="Description"
                  value={formData.description}
                  onChange={handleInputChange('description')}
                  multiline
                  rows={3}
                  disabled={role?.isSystemRole || saving}
                  helperText="Optional description of the role's purpose"
                />
              </CardContent>
            </Card>
          </Grid>

          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Permissions
                </Typography>

                {errors.permissions && (
                  <Alert severity="error" sx={{ mb: 2 }}>
                    {errors.permissions}
                  </Alert>
                )}

                {users.length > 0 && formData.permissions.length === 0 && (
                  <Alert severity="warning" sx={{ mb: 2 }}>
                    <Typography variant="body2">
                      This role has {users.length} user(s) assigned. Removing all permissions may affect their access.
                    </Typography>
                  </Alert>
                )}

                <PermissionSelector
                  value={formData.permissions}
                  onChange={handlePermissionsChange}
                  disabled={role?.isSystemRole || saving}
                />
              </CardContent>
            </Card>
          </Grid>

          {/* Assigned Users Section - Only show when editing */}
          {isEditing && (
            <Grid size={{ xs: 12 }}>
              <Card>
                <CardContent>
                  <Accordion defaultExpanded={users.length > 0}>
                    <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <PeopleIcon />
                        <Typography variant="h6">
                          Assigned Users ({users.length})
                        </Typography>
                        {users.length > 0 && (
                          <Chip 
                            label={`${users.length} users`}
                            size="small"
                            color="primary"
                            variant="outlined"
                          />
                        )}
                      </Box>
                    </AccordionSummary>
                    <AccordionDetails>
                      {usersLoading ? (
                        <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
                          <CircularProgress size={24} />
                        </Box>
                      ) : users.length === 0 ? (
                        <Alert severity="info">
                          No users are currently assigned to this role.
                        </Alert>
                      ) : (
                        <>
                          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                            The following users have this role assigned and will be affected by any permission changes:
                          </Typography>
                          
                          <List dense>
                            {users.slice(0, 10).map((user) => (
                              <ListItem key={user.id} divider>
                                <Avatar sx={{ mr: 2, bgcolor: 'primary.main', width: 32, height: 32 }}>
                                  {getInitials(user.firstName, user.lastName)}
                                </Avatar>
                                <ListItemText
                                  primary={user.fullName}
                                  secondary={user.email}
                                />
                                <Chip
                                  label={user.isActive ? 'Active' : 'Inactive'}
                                  color={user.isActive ? 'success' : 'warning'}
                                  size="small"
                                />
                              </ListItem>
                            ))}
                            {users.length > 10 && (
                              <ListItem>
                                <AccountCircleIcon sx={{ mr: 2, color: 'text.secondary' }} />
                                <Typography variant="body2" color="text.secondary">
                                  ... and {users.length - 10} more users
                                </Typography>
                              </ListItem>
                            )}
                          </List>

                          <Box sx={{ mt: 2, p: 1, bgcolor: 'background.default', borderRadius: 1 }}>
                            <Typography variant="caption" color="text.secondary">
                              <InfoIcon fontSize="small" sx={{ mr: 0.5, verticalAlign: 'middle' }} />
                              Changes to this role's permissions will immediately affect all assigned users' access.
                            </Typography>
                          </Box>
                        </>
                      )}
                    </AccordionDetails>
                  </Accordion>
                </CardContent>
              </Card>
            </Grid>
          )}
        </Grid>

        <Divider sx={{ my: 3 }} />

        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button
            type="submit"
            variant="contained"
            startIcon={saving ? <CircularProgress size={20} /> : <SaveIcon />}
            disabled={role?.isSystemRole || saving}
          >
            {saving ? 'Saving...' : (isEditing ? 'Update Role' : 'Create Role')}
          </Button>

          <Button
            variant="outlined"
            startIcon={<CancelIcon />}
            onClick={() => navigate(ROUTES.ROLES)}
            disabled={saving}
          >
            Cancel
          </Button>
        </Box>
      </form>

      {/* ✅ NEW: Workflow Usage Warning Dialog */}
      <RoleWorkflowUsageDialog
        open={workflowUsageDialog.open}
        onClose={handleCloseWorkflowDialog}
        onProceed={handleProceedWithUpdate}
        usageInfo={workflowUsageDialog.usageInfo}
        actionType={workflowUsageDialog.actionType}
        newRoleName={workflowUsageDialog.pendingUpdate?.name}
      />
    </Box>
  );
}

export default RoleEditor;
