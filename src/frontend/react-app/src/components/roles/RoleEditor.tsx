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
} from '@mui/material';
import {
  Save as SaveIcon,
  Cancel as CancelIcon,
} from '@mui/icons-material';
import { PermissionSelector } from './PermissionSelector.js';
import { roleService, type RoleCreateRequest, type RoleUpdateRequest } from '@/services/role.service.js';
import type { Role } from '@/types/index.js';
import toast from 'react-hot-toast';

export function RoleEditor() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const isEditing = Boolean(id && id !== 'new');

  const [role, setRole] = useState<Role | null>(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    permissions: [] as string[],
  });
  const [loading, setLoading] = useState(false);
  const [saving, setSaving] = useState(false);
  const [errors, setErrors] = useState<Record<string, string>>({});

  useEffect(() => {
    if (isEditing && id) {
      loadRole(id);
    }
  }, [id, isEditing]);

  const loadRole = async (roleId: string) => {
    try {
      setLoading(true);
      const roleData = await roleService.getRoleById(roleId);
      setRole(roleData);
      setFormData({
        name: roleData.name,
        description: roleData.description || '',
        permissions: roleData.permissions.map(p => p.name),
      });
    } catch (error) {
      console.error('Failed to load role:', error);
      toast.error('Failed to load role');
      navigate('/roles');
    } finally {
      setLoading(false);
    }
  };

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

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();

    if (!validateForm()) {
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
        await roleService.updateRole(id, updateData);
        toast.success('Role updated successfully');
      } else {
        const createData: RoleCreateRequest = {
          name: formData.name.trim(),
          description: formData.description.trim() || undefined,
          permissions: formData.permissions,
        };
        await roleService.createRole(createData);
        toast.success('Role created successfully');
      }

      navigate('/roles');
    } catch (error) {
      console.error('Failed to save role:', error);
      toast.error(`Failed to ${isEditing ? 'update' : 'create'} role`);
    } finally {
      setSaving(false);
    }
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

                <PermissionSelector
                  selectedPermissions={formData.permissions}
                  onChange={handlePermissionsChange}
                  disabled={role?.isSystemRole || saving}
                />
              </CardContent>
            </Card>
          </Grid>
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
            onClick={() => navigate('/roles')}
            disabled={saving}
          >
            Cancel
          </Button>
        </Box>
      </form>
    </Box>
  );
}
