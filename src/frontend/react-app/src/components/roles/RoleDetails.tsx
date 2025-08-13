import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Grid,
  Chip,
  Divider,
  CircularProgress,
  Alert,
  List,
  ListItem,
  ListItemText,
  Avatar,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  Tooltip,
} from '@mui/material';
import {
  ArrowBack as ArrowBackIcon,
  Edit as EditIcon,
  Security as SecurityIcon,
  People as PeopleIcon,
  Delete as DeleteIcon,
  Lock as LockIcon,
  ExpandMore as ExpandMoreIcon,
  Info as InfoIcon,
  AccountCircle as AccountCircleIcon,
} from '@mui/icons-material';
import { roleService } from '@/services/role.service.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { Role, UserInfo, Permission } from '@/types/index.js';
import toast from 'react-hot-toast';

export function RoleDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [role, setRole] = useState<Role | null>(null);
  const [users, setUsers] = useState<UserInfo[]>([]);
  const [loading, setLoading] = useState(true);
  const [usersLoading, setUsersLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const loadRoleDetails = async () => {
    if (!id) return;

    try {
      setLoading(true);
      setError(null);
      
      // Convert string ID to number for API call
      const roleId = parseInt(id, 10);
      if (isNaN(roleId)) {
        throw new Error('Invalid role ID');
      }
      
      console.log('🔍 RoleDetails: Loading role details for ID:', roleId);
      
      // Load role details
      const roleData = await roleService.getRoleById(roleId);
      console.log('🔍 RoleDetails: Role data received:', roleData);
      setRole(roleData);

      // Load users with this role
      setUsersLoading(true);
      try {
        console.log('🔍 RoleDetails: Loading users for role:', roleId);
        const usersData = await roleService.getRoleUsers(roleId);
        console.log('🔍 RoleDetails: Users data received:', usersData);
        setUsers(usersData);
      } catch (error) {
        console.error('❌ RoleDetails: Failed to load role users:', error);
        // Show a warning but don't fail the whole component
        toast.error('Could not load users for this role');
        setUsers([]);
      } finally {
        setUsersLoading(false);
      }

    } catch (error) {
      console.error('❌ RoleDetails: Failed to load role details:', error);
      let errorMessage = 'Failed to load role details';
      
      if (error instanceof Error) {
        errorMessage = error.message;
      }
      
      setError(errorMessage);
      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (id) {
      loadRoleDetails();
    }
  }, [id]);

  const handleEdit = () => {
    if (role && !role.isSystemRole) {
      navigate(`/roles/${role.id}/edit`);
    }
  };

  const handleDeleteRole = async () => {
    if (!role || role.isSystemRole) return;

    if (users.length > 0) {
      toast.error(`Cannot delete role "${role.name}" because it has ${users.length} user(s) assigned to it. Remove all users from this role first.`);
      return;
    }

    if (window.confirm(`Are you sure you want to delete the role "${role.name}"? This action cannot be undone.`)) {
      try {
        await roleService.deleteRole(role.id);
        toast.success('Role deleted successfully');
        navigate('/roles');
      } catch (error) {
        console.error('❌ RoleDetails: Failed to delete role:', error);
        let errorMessage = 'Failed to delete role';
        
        if (error instanceof Error) {
          if (error.message.includes('users assigned')) {
            errorMessage = 'Cannot delete role that has users assigned to it';
          } else {
            errorMessage = error.message;
          }
        }
        
        toast.error(errorMessage);
      }
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  const groupPermissionsByCategory = (permissions: Permission[]) => {
    return permissions.reduce((groups, permission) => {
      const category = permission.category || 'Other';
      if (!groups[category]) {
        groups[category] = [];
      }
      groups[category].push(permission);
      return groups;
    }, {} as Record<string, Permission[]>);
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error || !role) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">{error || 'Role not found'}</Alert>
        <Button 
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/roles')}
          sx={{ mt: 2 }}
        >
          Back to Roles
        </Button>
      </Box>
    );
  }

  const permissionGroups = groupPermissionsByCategory(role.permissions);

  return (
    <Box>
      {/* Header */}
      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', mb: 3 }}>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <Button
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate('/roles')}
            sx={{ mr: 2 }}
          >
            Back to Roles
          </Button>
          <Typography variant="h4" component="h1">
            Role Details
          </Typography>
        </Box>

        <Box sx={{ display: 'flex', gap: 1 }}>
          <CanAccess permission={PERMISSIONS.ROLES_EDIT}>
            <Button
              variant="outlined"
              startIcon={<EditIcon />}
              onClick={handleEdit}
              disabled={role.isSystemRole}
            >
              Edit
            </Button>
          </CanAccess>

          <CanAccess permission={PERMISSIONS.ROLES_DELETE}>
            <Button
              variant="outlined"
              color="error"
              startIcon={<DeleteIcon />}
              onClick={handleDeleteRole}
              disabled={role.isSystemRole || users.length > 0}
            >
              Delete
            </Button>
          </CanAccess>
        </Box>
      </Box>

      <Grid container spacing={3}>
        {/* Role Information */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <SecurityIcon sx={{ mr: 1 }} />
                <Typography variant="h6">Role Information</Typography>
              </Box>

              <Box sx={{ mb: 3 }}>
                <Typography variant="h5" gutterBottom>
                  {role.name}
                </Typography>
                <Typography variant="body1" color="text.secondary" paragraph>
                  {role.description || 'No description provided'}
                </Typography>

                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1, mb: 2 }}>
                  {role.isSystemRole && (
                    <Chip 
                      label="System Role" 
                      color="warning" 
                      icon={<LockIcon />}
                    />
                  )}
                  {role.isDefault && (
                    <Chip 
                      label="Default Role" 
                      color="info" 
                    />
                  )}
                  <Chip 
                    label={`${role.permissions.length} Permissions`} 
                    color="primary" 
                    variant="outlined"
                  />
                </Box>
              </Box>

              <Divider sx={{ my: 2 }} />

              <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                <Typography variant="body2" color="text.secondary">
                  <strong>Created:</strong>
                </Typography>
                <Typography variant="body2">
                  {formatDate(role.createdAt)}
                </Typography>
              </Box>

              {role.updatedAt && (
                <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 1 }}>
                  <Typography variant="body2" color="text.secondary">
                    <strong>Last Updated:</strong>
                  </Typography>
                  <Typography variant="body2">
                    {formatDate(role.updatedAt)}
                  </Typography>
                </Box>
              )}

              <Box sx={{ display: 'flex', justifyContent: 'space-between' }}>
                <Typography variant="body2" color="text.secondary">
                  <strong>Role ID:</strong>
                </Typography>
                <Typography variant="body2" sx={{ fontFamily: 'monospace' }}>
                  {role.id}
                </Typography>
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Users with this Role */}
        <Grid size={{ xs: 12, md: 6 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <PeopleIcon sx={{ mr: 1 }} />
                <Typography variant="h6">
                  Assigned Users ({users.length})
                </Typography>
              </Box>

              {usersLoading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
                  <CircularProgress size={24} />
                </Box>
              ) : users.length === 0 ? (
                <Alert severity="info" sx={{ mt: 1 }}>
                  No users are currently assigned to this role.
                </Alert>
              ) : (
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
              )}

              {users.length > 0 && (
                <Box sx={{ mt: 2, p: 1, bgcolor: 'background.default', borderRadius: 1 }}>
                  <Typography variant="caption" color="text.secondary">
                    <InfoIcon fontSize="small" sx={{ mr: 0.5, verticalAlign: 'middle' }} />
                    {users.length > 0 && role.isSystemRole ? 
                      'This system role cannot be deleted while users are assigned.' :
                      users.length > 0 ? 
                      'Remove all users before deleting this role.' :
                      'This role can be safely deleted.'
                    }
                  </Typography>
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* Permissions */}
        <Grid size={{ xs: 12 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Permissions ({role.permissions.length})
              </Typography>

              {role.permissions.length === 0 ? (
                <Alert severity="info">
                  This role has no permissions assigned.
                </Alert>
              ) : (
                <Box>
                  {Object.entries(permissionGroups).map(([category, permissions]) => (
                    <Accordion key={category} defaultExpanded>
                      <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="subtitle1" fontWeight="medium">
                            {category}
                          </Typography>
                          <Chip 
                            label={permissions.length}
                            size="small"
                            color="primary"
                            variant="outlined"
                          />
                        </Box>
                      </AccordionSummary>
                      <AccordionDetails>
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                          {permissions.map((permission) => (
                            <Tooltip 
                              key={permission.id}
                              title={permission.description || 'No description'}
                              arrow
                            >
                              <Chip
                                label={permission.name}
                                size="small"
                                variant="outlined"
                                color="secondary"
                                icon={<InfoIcon fontSize="small" />}
                              />
                            </Tooltip>
                          ))}
                        </Box>
                      </AccordionDetails>
                    </Accordion>
                  ))}
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}

export default RoleDetails;
