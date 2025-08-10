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
} from '@mui/icons-material';
import { roleService } from '@/services/role.service.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { Role, User, Permission } from '@/types/index.js';
import toast from 'react-hot-toast';

export function RoleDetails() {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();

  const [role, setRole] = useState<Role | null>(null);
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [usersLoading, setUsersLoading] = useState(false);

  useEffect(() => {
    if (id) {
      loadRoleDetails();
    }
  }, [id]);

  const loadRoleDetails = async () => {
    if (!id) return;

    try {
      setLoading(true);
      
      // Load role details
      const roleData = await roleService.getRoleById(id);
      setRole(roleData);

      // Load users with this role
      setUsersLoading(true);
      try {
        const usersData = await roleService.getRoleUsers(id);
        setUsers(usersData);
      } catch (error) {
        console.error('Failed to load role users:', error);
        // Don't fail the whole component if users can't be loaded
      } finally {
        setUsersLoading(false);
      }

    } catch (error) {
      console.error('Failed to load role details:', error);
      toast.error('Failed to load role details');
      navigate('/roles');
    } finally {
      setLoading(false);
    }
  };

  const handleEdit = () => {
    if (role && !role.isSystemRole) {
      navigate(`/roles/${role.id}/edit`);
    }
  };

  const handleDeleteRole = async () => {
    if (!role || role.isSystemRole) return;

    if (window.confirm(`Are you sure you want to delete the role "${role.name}"? This action cannot be undone.`)) {
      try {
        await roleService.deleteRole(role.id);
        toast.success('Role deleted successfully');
        navigate('/roles');
      } catch (error) {
        console.error('Failed to delete role:', error);
        toast.error('Failed to delete role');
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

  if (!role) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">Role not found</Alert>
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
              disabled={role.isSystemRole}
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
                  Users ({users.length})
                </Typography>
              </Box>

              {usersLoading ? (
                <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
                  <CircularProgress size={24} />
                </Box>
              ) : users.length === 0 ? (
                <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 3 }}>
                  No users assigned to this role
                </Typography>
              ) : (
                <List dense>
                  {users.slice(0, 5).map((user) => (
                    <ListItem key={user.id} divider>
                      <Avatar sx={{ mr: 2, bgcolor: 'primary.main', width: 32, height: 32 }}>
                        {getInitials(user.firstName, user.lastName)}
                      </Avatar>
                      <ListItemText
                        primary={`${user.firstName} ${user.lastName}`}
                        secondary={user.email}
                      />
                      <Chip
                        label={user.emailConfirmed ? 'Active' : 'Pending'}
                        color={user.emailConfirmed ? 'success' : 'warning'}
                        size="small"
                      />
                    </ListItem>
                  ))}
                  {users.length > 5 && (
                    <ListItem>
                      <Typography variant="body2" color="text.secondary">
                        ... and {users.length - 5} more users
                      </Typography>
                    </ListItem>
                  )}
                </List>
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
