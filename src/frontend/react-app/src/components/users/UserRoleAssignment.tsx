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
} from '@mui/material';
import {
  Add as AddIcon,
  Remove as RemoveIcon,
  ArrowBack as ArrowBackIcon,
  Security as SecurityIcon,
} from '@mui/icons-material';
import { roleService } from '@/services/role.service.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import { usePermission } from '@/contexts/PermissionContext.js';
import type { Role } from '@/types/index.js';
import toast from 'react-hot-toast';
import { ROUTES } from '@/routes/route.constants.js';

interface UserData {
  userCurrentRoles: { id: number; name: string }[];
  availableRoles: { id: number; name: string }[];
  totalRoles: number;
  duplicatePrevention: string;
}

export function UserRoleAssignment() {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const { getUserRoles } = usePermission();

  // Use getUserRoles to show context data
  console.log('User roles from context:', getUserRoles());

  // Remove unused user state variables
  // const [user, setUser] = useState<User | null>(null);
  const [userRoles, setUserRoles] = useState<Role[]>([]);
  const [availableRoles, setAvailableRoles] = useState<Role[]>([]);
  const [userData, setUserData] = useState<UserData | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);
  const [assignDialogOpen, setAssignDialogOpen] = useState(false);
  const [selectedRole, setSelectedRole] = useState<{ id: number; name: string } | null>(null);
  const [removing, setRemoving] = useState<number | null>(null);
  const [assigning, setAssigning] = useState(false);
  // Add state for user permissions
  const [userPermissions, setUserPermissions] = useState<string[]>([]);

  useEffect(() => {
    if (userId) {
      loadUserData();
    }
  }, [userId]);

  const loadUserData = async () => {
    if (!userId) return;

    try {
      setLoading(true);
      setError(null);
      
      const [userRolesResponse, allRolesResponse, userPermissionsResponse] = await Promise.all([
        roleService.getUserRoles(userId), // Returns RoleDto[]
        roleService.getRoles({ pageSize: 100 }),
        roleService.getUserPermissions(userId) // New method for permissions
      ]);

      setUserRoles(userRolesResponse);
      setUserPermissions(userPermissionsResponse);
      
      // Set the actual roles
      setUserRoles(userRolesResponse);
      
      // Extract roles array from the response
      const userRoleIds = new Set(userRolesResponse.map((role: Role) => role.id));
      const available = allRolesResponse.roles.filter((role: Role) =>
        !userRoleIds.has(role.id) && !role.isSystemRole
      );

      setAvailableRoles(available);

      setUserData({
        userCurrentRoles: userRolesResponse.map((r: Role) => ({ id: r.id, name: r.name })),
        availableRoles: available.map((r: Role) => ({ id: r.id, name: r.name })),
        totalRoles: allRolesResponse.roles.length,
        duplicatePrevention: `Filtered ${allRolesResponse.roles.length - available.length} existing/system roles`
      });
      
      console.log('✅ UserRoleAssignment: User data loaded successfully');
    } catch (error) {
      console.error('❌ UserRoleAssignment: Failed to load user data:', error);
      setError('Failed to load user role data');
    } finally {
      setLoading(false);
    }
  };

  const handleAssignRole = async () => {
    if (!selectedRole || !userId) return;

    try {
      setAssigning(true);
      console.log('🔍 UserRoleAssignment: Assigning role:', { userId, roleId: selectedRole.id, roleName: selectedRole.name });
      
      await roleService.assignRoleToUser(parseInt(userId), selectedRole.id);
      
      console.log('✅ UserRoleAssignment: Role assigned, reloading data...');
      
      setSelectedRole(null);
      setAssignDialogOpen(false);
      
      // ✅ FIXED: Reload data first, then show success
      await loadUserData();
      toast.success(`Role "${selectedRole.name}" assigned successfully`);
      
    } catch (error) {
      console.error('❌ UserRoleAssignment: Failed to assign role:', error);
      
      // ✅ ENHANCED: Show specific error messages
      let errorMessage = 'Failed to assign role';
      if (error instanceof Error) {
        errorMessage = error.message;
      }
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as any;
        if (axiosError.response?.status === 401) {
          errorMessage = 'Session expired. Please log in again.';
        } else if (axiosError.response?.status === 403) {
          errorMessage = 'You do not have permission to assign roles.';
        } else if (axiosError.response?.data?.message) {
          errorMessage = axiosError.response.data.message;
        }
      }
      
      toast.error(errorMessage);
    } finally {
      setAssigning(false);
    }
  };

  const handleRemoveRole = async (role: { id: number; name: string }) => {
    try {
      setRemoving(role.id);
      await roleService.removeRoleFromUser(role.id, userId!);
      await loadUserData();
      toast.success(`Role "${role.name}" removed successfully`);
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

  const getUserEffectivePermissions = (): string[] => {
    return userPermissions.sort();
  };

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">{error}</Alert>
        <Button onClick={() => navigate(ROUTES.USERS)} sx={{ mt: 2 }}>
          Back to Users
        </Button>
      </Box>
    );
  }

  if (!userData) {
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error">Failed to load user data</Alert>
      </Box>
    );
  }

  const effectivePermissions = getUserEffectivePermissions();

  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
        <Button
          startIcon={<ArrowBackIcon />}
          onClick={() => navigate(ROUTES.USERS)}
          sx={{ mr: 2 }}
        >
          Back to Users
        </Button>
        <Typography variant="h4" component="h1">
          Manage User Roles
        </Typography>
      </Box>

      <Grid container spacing={3}>
        {/* Current Roles */}
        <Grid size={{ xs: 12, md: 6 }}>
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
                          secondary={
                            <Box>
                              <Typography variant="body2" color="text.secondary">
                                {role.description || 'No description'}
                              </Typography>
                              <Typography variant="caption" color="text.secondary">
                                {role.permissions?.length || 0} permissions
                              </Typography>
                            </Box>
                          }
                        />
                        
                        <CanAccess permission={PERMISSIONS.USERS_MANAGE_ROLES}>
                          <IconButton
                            edge="end"
                            size="small"
                            color="error"
                            onClick={() => handleRemoveRole({ id: role.id, name: role.name })}
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
        <Grid size={{ xs: 12, md: 6 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>
                Effective Permissions ({effectivePermissions.length})
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Combined permissions from all assigned roles
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
        <DialogTitle>Assign Role</DialogTitle>
        <DialogContent>
          {availableRoles.length === 0 ? (
            <Alert severity="info">
              No additional roles available to assign.
            </Alert>
          ) : (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
                Select a role to assign.
              </Typography>
              <List>
                {userData.availableRoles.map((role) => (
                  <ListItem key={role.id} disablePadding>
                    <ListItemButton
                      selected={selectedRole?.id === role.id}
                      onClick={() => setSelectedRole(role)}
                    >
                      <ListItemText
                        primary={role.name}
                        secondary="Click to select"
                      />
                    </ListItemButton>
                  </ListItem>
                ))}
              </List>
            </>
          )}
        </DialogContent>
        <DialogActions>
          <Button onClick={handleAssignDialogClose}>Cancel</Button>
          <Button
            variant="contained"
            onClick={handleAssignRole}
            disabled={!selectedRole || assigning || availableRoles.length === 0}
            startIcon={assigning ? <CircularProgress size={16} /> : <AddIcon />}
          >
            {assigning ? 'Assigning...' : 'Assign Role'}
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default UserRoleAssignment;
