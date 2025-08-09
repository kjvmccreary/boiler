import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Grid,
  Chip,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  List,
  ListItem,
  ListItemButton,
  ListItemText,
  IconButton,
  CircularProgress,
  Alert,
  Divider,
  Avatar,
} from '@mui/material';
import {
  Add as AddIcon,
  Remove as RemoveIcon,
  ArrowBack as ArrowBackIcon,
  Security as SecurityIcon,
  Person as PersonIcon,
} from '@mui/icons-material';
import { userService } from '@/services/user.service.js';
import { roleService } from '@/services/role.service.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { User, Role } from '@/types/index.js';
import toast from 'react-hot-toast';

export function UserRoleAssignment() {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();

  const [user, setUser] = useState<User | null>(null);
  const [userRoles, setUserRoles] = useState<Role[]>([]);
  const [availableRoles, setAvailableRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [assignDialogOpen, setAssignDialogOpen] = useState(false);
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);
  const [removing, setRemoving] = useState<string | null>(null);
  const [assigning, setAssigning] = useState(false);

  useEffect(() => {
    if (userId) {
      loadUserData();
    }
  }, [userId]);

  const loadUserData = async () => {
    if (!userId) return;

    try {
      setLoading(true);
      
      // Load user details and roles in parallel
      const [userResponse, userRolesResponse, allRolesResponse] = await Promise.all([
        userService.getUserById(userId),
        roleService.getUserRoles(userId),
        roleService.getRoles(),
      ]);

      setUser(userResponse);
      setUserRoles(userRolesResponse);
      
      // Filter out roles that user already has
      const userRoleIds = userRolesResponse.map(role => role.id);
      const available = allRolesResponse.filter(role => 
        !userRoleIds.includes(role.id) && !role.isSystemRole
      );
      setAvailableRoles(available);

    } catch (error) {
      console.error('Failed to load user data:', error);
      toast.error('Failed to load user data');
      navigate('/users');
    } finally {
      setLoading(false);
    }
  };

  const handleAssignRole = async () => {
    if (!selectedRole || !userId) return;

    try {
      setAssigning(true);
      await roleService.assignRoleToUser(parseInt(userId), parseInt(selectedRole.id));
      
      toast.success(`Role "${selectedRole.name}" assigned successfully`);
      setAssignDialogOpen(false);
      setSelectedRole(null);
      await loadUserData(); // Refresh data
    } catch (error) {
      console.error('Failed to assign role:', error);
      toast.error('Failed to assign role');
    } finally {
      setAssigning(false);
    }
  };

  const handleRemoveRole = async (role: Role) => {
    if (!userId) return;

    try {
      setRemoving(role.id);
      await roleService.removeRoleFromUser(role.id, userId);
      
      toast.success(`Role "${role.name}" removed successfully`);
      await loadUserData(); // Refresh data
    } catch (error) {
      console.error('Failed to remove role:', error);
      toast.error('Failed to remove role');
    } finally {
      setRemoving(null);
    }
  };

  const handleAssignDialogOpen = () => {
    setAssignDialogOpen(true);
  };

  const handleAssignDialogClose = () => {
    setAssignDialogOpen(false);
    setSelectedRole(null);
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  const getUserEffectivePermissions = (): string[] => {
    const allPermissions = new Set<string>();
    
    userRoles.forEach(role => {
      role.permissions.forEach(permission => {
        allPermissions.add(permission.name);
      });
    });

    return Array.from(allPermissions).sort();
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!user) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">User not found</Alert>
      </Box>
    );
  }

  const effectivePermissions = getUserEffectivePermissions();

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate('/users')}
          sx={{ mr: 2 }}
        >
          Back to Users
        </Button>
        <Typography variant="h4" component="h1">
          Manage User Roles
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {/* User Information */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <PersonIcon sx={{ mr: 1 }} />
                <Typography variant="h6">User Information</Typography>
              </Box>

              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Avatar sx={{ mr: 2, bgcolor: 'primary.main', width: 56, height: 56 }}>
                  {getInitials(user.firstName, user.lastName)}
                </Avatar>
                <Box>
                  <Typography variant="h6">
                    {user.firstName} {user.lastName}
                  </Typography>
                  <Typography variant="body2" color="text.secondary">
                    {user.email}
                  </Typography>
                  <Chip
                    label={user.isEmailConfirmed ? 'Active' : 'Pending'}
                    color={user.isEmailConfirmed ? 'success' : 'warning'}
                    size="small"
                    sx={{ mt: 1 }}
                  />
                </Box>
              </Box>

              <Divider sx={{ my: 2 }} />

              <Typography variant="body2" color="text.secondary">
                <strong>User ID:</strong> {user.id}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                <strong>Tenant:</strong> {user.tenantId}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                <strong>Created:</strong> {new Date(user.createdAt).toLocaleDateString()}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        {/* Current Roles */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Box sx={{ display: 'flex', alignItems: 'center' }}>
                  <SecurityIcon sx={{ mr: 1 }} />
                  <Typography variant="h6">Current Roles ({userRoles.length})</Typography>
                </Box>
                
                <CanAccess permission={PERMISSIONS.USERS_MANAGE_ROLES}>
                  <Button
                    variant="outlined"
                    size="small"
                    startIcon={<AddIcon />}
                    onClick={handleAssignDialogOpen}
                    disabled={availableRoles.length === 0}
                  >
                    Assign Role
                  </Button>
                </CanAccess>
              </Box>

              {userRoles.length === 0 ? (
                <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 3 }}>
                  No roles assigned
                </Typography>
              ) : (
                <List dense>
                  {userRoles.map((role) => (
                    <ListItem key={role.id} divider>
                      <Box sx={{ display: 'flex', alignItems: 'center', width: '100%', justifyContent: 'space-between' }}>
                        <ListItemText
                          primary={
                            <Box sx={{ display: 'flex', alignItems: 'center' }}>
                              <Typography variant="subtitle2">{role.name}</Typography>
                              {role.isSystemRole && (
                                <Chip 
                                  label="System" 
                                  size="small" 
                                  color="warning" 
                                  sx={{ ml: 1 }}
                                />
                              )}
                              {role.isDefault && (
                                <Chip 
                                  label="Default" 
                                  size="small" 
                                  color="info" 
                                  sx={{ ml: 1 }}
                                />
                              )}
                            </Box>
                          }
                          secondary={role.description || 'No description'}
                        />
                        
                        <CanAccess permission={PERMISSIONS.USERS_MANAGE_ROLES}>
                          <IconButton
                            edge="end"
                            size="small"
                            color="error"
                            onClick={() => handleRemoveRole(role)}
                            disabled={role.isSystemRole || removing === role.id}
                            title={role.isSystemRole ? 'Cannot remove system role' : 'Remove role'}
                            sx={{ ml: 1 }}
                          >
                            {removing === role.id ? (
                              <CircularProgress size={16} />
                            ) : (
                              <RemoveIcon />
                            )}
                          </IconButton>
                        </CanAccess>
                      </Box>
                    </ListItem>
                  ))}
                </List>
              )}
            </CardContent>
          </Card>
        </Grid>

        {/* Effective Permissions */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Effective Permissions ({effectivePermissions.length})
              </Typography>

              {effectivePermissions.length === 0 ? (
                <Typography variant="body2" color="text.secondary" sx={{ textAlign: 'center', py: 3 }}>
                  No permissions granted
                </Typography>
              ) : (
                <Box sx={{ maxHeight: 400, overflow: 'auto' }}>
                  {effectivePermissions.map((permission) => (
                    <Chip
                      key={permission}
                      label={permission}
                      size="small"
                      variant="outlined"
                      color="secondary"
                      sx={{ m: 0.5 }}
                    />
                  ))}
                </Box>
              )}
            </CardContent>
          </Card>
        </Grid>
      </Grid>

      {/* Assign Role Dialog */}
      <Dialog open={assignDialogOpen} onClose={handleAssignDialogClose} maxWidth="sm" fullWidth>
        <DialogTitle>Assign Role to {user.firstName} {user.lastName}</DialogTitle>
        <DialogContent>
          {availableRoles.length === 0 ? (
            <Alert severity="info">
              No additional roles available to assign.
            </Alert>
          ) : (
            <List>
              {availableRoles.map((role) => (
                <ListItem key={role.id} disablePadding>
                  <ListItemButton
                    selected={selectedRole?.id === role.id}
                    onClick={() => setSelectedRole(role)}
                  >
                    <ListItemText
                      primary={role.name}
                      secondary={
                        <Box>
                          <Typography variant="body2" color="text.secondary">
                            {role.description || 'No description'}
                          </Typography>
                          <Typography variant="caption" color="text.secondary">
                            {role.permissions.length} permissions
                          </Typography>
                        </Box>
                      }
                    />
                  </ListItemButton>
                </ListItem>
              ))}
            </List>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleAssignDialogClose}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleAssignRole}
            disabled={!selectedRole || assigning}
            startIcon={assigning ? <CircularProgress size={16} /> : <AddIcon />}
          >
            {assigning ? 'Assigning...' : 'Assign Role'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}
