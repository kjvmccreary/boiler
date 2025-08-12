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
  Avatar,
  Chip,
  Divider,
  CircularProgress,
  Alert,
} from '@mui/material';
import {
  Save as SaveIcon,
  Cancel as CancelIcon,
  ArrowBack as ArrowBackIcon,
  Edit as EditIcon,
  Security as SecurityIcon,
} from '@mui/icons-material';
import { userService, type UserUpdateRequest } from '@/services/user.service.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import { tokenManager } from '@/utils/token.manager.js';
import { usePermission } from '@/contexts/PermissionContext.js';
import type { User } from '@/types/index.js';
import toast from 'react-hot-toast';
import { normalizeRoles } from '@/utils/role.utils.js';

export function UserProfile() {
  const { userId } = useParams<{ userId: string }>();
  const navigate = useNavigate();
  const { isAdmin, getUserRoles } = usePermission(); // Removed unused hasRole
  
  // 🔧 .NET 9 FIX: Properly determine if this is own profile
  const [isOwnProfile, setIsOwnProfile] = useState(false);
  const [currentUserId, setCurrentUserId] = useState<string | null>(null);
  const [canEditProfile, setCanEditProfile] = useState(false);

  const [user, setUser] = useState<User | null>(null);
  const [editing, setEditing] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
  });
  const [errors, setErrors] = useState<Record<string, string>>({});

  // Load user data function
  const loadUser = async () => {
    console.log('🔍 UserProfile: loadUser called', { 
      isOwnProfile, 
      userId,
      currentUserId,
      canEditProfile,
      timestamp: new Date().toISOString()
    });

    try {
      setLoading(true);
      
      let userData: User;
      if (isOwnProfile) {
        console.log('🔍 UserProfile: Calling getCurrentUserProfile...');
        userData = await userService.getCurrentUserProfile();
        console.log('✅ UserProfile: getCurrentUserProfile successful', userData);
      } else {
        console.log('🔍 UserProfile: Calling getUserById...', userId);
        userData = await userService.getUserById(userId!);
        console.log('✅ UserProfile: getUserById successful', userData);
      }

      setUser(userData);
      setFormData({
        firstName: userData.firstName,
        lastName: userData.lastName,
        email: userData.email,
      });
      
      console.log('✅ UserProfile: User data loaded successfully');
    } catch (error) {
      console.error('❌ UserProfile: Failed to load user:', error);
      
      if (error instanceof Error) {
        console.error('❌ Error details:', {
          message: error.message,
          stack: error.stack,
          name: error.name
        });
      }
      
      toast.error('Failed to load user profile');
    } finally {
      setLoading(false);
    }
  };

  // 🔧 .NET 9 FIX: Determine if this is own profile and edit permissions
  useEffect(() => {
    const token = tokenManager.getToken();
    if (token) {
      const userIdFromToken = tokenManager.getUserIdFromToken(token);
      setCurrentUserId(userIdFromToken);
      
      // Check if we're viewing own profile
      const isOwn = !userId || userId === userIdFromToken;
      setIsOwnProfile(isOwn);
      
      // 🔧 .NET 9 FIX: Use permission-based admin check instead of role-based
      if (isOwn) {
        // Admin users can edit their own profile
        const userIsAdmin = isAdmin();
        setCanEditProfile(userIsAdmin);
        
        console.log('🔍 UserProfile: Permission-based admin check for own profile:', {
          userIdParam: userId,
          userIdFromToken,
          isOwnProfile: isOwn,
          isAdmin: userIsAdmin,
          canEditProfile: userIsAdmin
        });
      } else {
        // Admins can edit other users' profiles
        const canEditOthers = isAdmin();
        setCanEditProfile(canEditOthers);
        
        console.log('🔍 UserProfile: Permission-based admin check for other user profile:', {
          userIdParam: userId,
          userIdFromToken,
          isOwnProfile: isOwn,
          canEditOthers,
          canEditProfile: canEditOthers
        });
      }
    }
  }, [userId, isAdmin]);

  useEffect(() => {
    console.log('🔍 UserProfile: useEffect triggered', { userId, isOwnProfile, canEditProfile });
    if (currentUserId !== null) {
      loadUser();
    }
  }, [userId, currentUserId, isOwnProfile, canEditProfile, loadUser]); // Added missing dependencies

  const handleRetry = () => {
    console.log('🔍 UserProfile: Retry button clicked');
    loadUser();
  };

  const validateForm = (): boolean => {
    const newErrors: Record<string, string> = {};

    if (!formData.firstName.trim()) {
      newErrors.firstName = 'First name is required';
    }

    if (!formData.lastName.trim()) {
      newErrors.lastName = 'Last name is required';
    }

    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      newErrors.email = 'Please enter a valid email address';
    }

    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSave = async () => {
    if (!validateForm()) return;

    try {
      setSaving(true);
      
      const updateData: UserUpdateRequest = {
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        email: formData.email.trim(),
      };

      console.log('🔍 UserProfile: handleSave - determining update method:', {
        isOwnProfile,
        canEditProfile,
        currentUserId,
        userIdParam: userId,
        userData: updateData
      });

      let updatedUser: User;
      if (isOwnProfile) {
        console.log('🔍 UserProfile: Using updateCurrentUserProfile for own profile');
        updatedUser = await userService.updateCurrentUserProfile(updateData);
      } else {
        console.log('🔍 UserProfile: Using updateUser for admin update of user:', userId);
        updatedUser = await userService.updateUser(userId!, updateData);
      }

      setUser(updatedUser);
      setEditing(false);
      toast.success('Profile updated successfully');
    } catch (error) {
      console.error('Failed to update profile:', error);
      
      if (error instanceof Error) {
        if (error.message.includes('Only admin users can update their profile')) {
          toast.error('Only admin users can update their profile');
        } else {
          toast.error('Failed to update profile');
        }
      } else {
        toast.error('Failed to update profile');
      }
    } finally {
      setSaving(false);
    }
  };

  const handleCancel = () => {
    if (user) {
      setFormData({
        firstName: user.firstName,
        lastName: user.lastName,
        email: user.email,
      });
    }
    setEditing(false);
    setErrors({});
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

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  if (loading) {
    console.log('🔍 UserProfile: Showing loading state');
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: 400 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!user) {
    console.log('🔍 UserProfile: Showing error state (no user data)');
    return (
      <Box sx={{ p: 3 }}>
        <Alert severity="error" sx={{ mb: 2 }}>
          {isOwnProfile ? 'Unable to load your profile' : 'User not found'}
        </Alert>
        
        <Box sx={{ display: 'flex', gap: 2 }}>
          <Button 
            variant="contained" 
            onClick={handleRetry}
            color="primary"
          >
            Try Again
          </Button>
          
          <Button 
            variant="outlined" 
            onClick={() => navigate(isOwnProfile ? '/dashboard' : '/users')}
          >
            {isOwnProfile ? 'Go to Dashboard' : 'Back to Users'}
          </Button>
        </Box>
      </Box>
    );
  }

  console.log('🔍 UserProfile: Rendering profile UI with user data');
  return (
    <Box>
      <Box sx={{ display: 'flex', alignItems: 'center', mb: 3 }}>
        {!isOwnProfile && (
          <Button
            startIcon={<ArrowBackIcon />}
            onClick={() => navigate('/users')}
            sx={{ mr: 2 }}
          >
            Back to Users
          </Button>
        )}
        <Typography variant="h4" component="h1">
          {isOwnProfile ? 'My Profile' : 'User Profile'}
        </Typography>
      </Box>

      {/* 🔧 .NET 9 FIX: Show permission-based admin notice for own profile */}
      {isOwnProfile && !canEditProfile && (
        <Alert severity="info" sx={{ mb: 3 }}>
          Only users with administrative permissions can edit their profile. Contact an administrator if you need to make changes.
        </Alert>
      )}

      <Grid container spacing={3}>
        {/* Profile Information */}
        <Grid size={{ xs: 12, md: 8 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
                <Typography variant="h6">Profile Information</Typography>
                
                {canEditProfile && (
                  <Button
                    variant={editing ? "outlined" : "contained"}
                    startIcon={editing ? <CancelIcon /> : <EditIcon />}
                    onClick={editing ? handleCancel : () => setEditing(true)}
                    disabled={saving}
                  >
                    {editing ? 'Cancel' : 'Edit'}
                  </Button>
                )}
              </Box>

              <Grid container spacing={3}>
                <Grid size={{ xs: 12, md: 4 }}>
                  <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                    <Avatar sx={{ width: 120, height: 120, bgcolor: 'primary.main', mb: 2 }}>
                      <Typography variant="h3">
                        {getInitials(user.firstName, user.lastName)}
                      </Typography>
                    </Avatar>
                    <Typography variant="h6">
                      {user.firstName} {user.lastName}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {user.email}
                    </Typography>
                  </Box>
                </Grid>

                <Grid size={{ xs: 12, md: 8 }}>
                  <Box component="form" sx={{ mt: 1 }}>
                    <Grid container spacing={2}>
                      <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField
                          fullWidth
                          label="First Name"
                          value={editing ? formData.firstName : user.firstName}
                          onChange={handleInputChange('firstName')}
                          error={!!errors.firstName}
                          helperText={errors.firstName}
                          disabled={!editing || saving}
                          slotProps={{
                            input: {
                              readOnly: !editing,
                            },
                          }}
                        />
                      </Grid>

                      <Grid size={{ xs: 12, sm: 6 }}>
                        <TextField
                          fullWidth
                          label="Last Name"
                          value={editing ? formData.lastName : user.lastName}
                          onChange={handleInputChange('lastName')}
                          error={!!errors.lastName}
                          helperText={errors.lastName}
                          disabled={!editing || saving}
                          slotProps={{
                            input: {
                              readOnly: !editing,
                            },
                          }}
                        />
                      </Grid>

                      <Grid size={{ xs: 12 }}>
                        <TextField
                          fullWidth
                          label="Email"
                          type="email"
                          value={editing ? formData.email : user.email}
                          onChange={handleInputChange('email')}
                          error={!!errors.email}
                          helperText={errors.email}
                          disabled={!editing || saving}
                          slotProps={{
                            input: {
                              readOnly: !editing,
                            },
                          }}
                        />
                      </Grid>
                    </Grid>

                    {editing && (
                      <Box sx={{ mt: 3, display: 'flex', gap: 2 }}>
                        <Button
                          variant="contained"
                          startIcon={saving ? <CircularProgress size={16} /> : <SaveIcon />}
                          onClick={handleSave}
                          disabled={saving}
                        >
                          {saving ? 'Saving...' : 'Save Changes'}
                        </Button>
                        <Button
                          variant="outlined"
                          onClick={handleCancel}
                          disabled={saving}
                        >
                          Cancel
                        </Button>
                      </Box>
                    )}
                  </Box>
                </Grid>
              </Grid>
            </CardContent>
          </Card>
        </Grid>

        {/* Account Details & Roles */}
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Account Details</Typography>
              
              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Status</Typography>
                <Chip
                  label={user.emailConfirmed ? 'Active' : 'Pending Verification'}
                  color={user.emailConfirmed ? 'success' : 'warning'}
                  size="small"
                />
              </Box>

              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">User ID</Typography>
                <Typography variant="body2">{user.id}</Typography>
              </Box>

              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Tenant</Typography>
                <Typography variant="body2">{user.tenantId}</Typography>
              </Box>

              <Box sx={{ mb: 2 }}>
                <Typography variant="body2" color="text.secondary">Member Since</Typography>
                <Typography variant="body2">
                  {new Date(user.createdAt).toLocaleDateString()}
                </Typography>
              </Box>

              <Divider sx={{ my: 2 }} />

              <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
                <Typography variant="h6">
                  Roles ({(() => {
                    const roles = getUserRoles(); // Use context method for accurate count
                    return roles.length;
                  })()})
                </Typography>
                {!isOwnProfile && (
                  <CanAccess permission={PERMISSIONS.USERS_MANAGE_ROLES}>
                    <Button
                      size="small"
                      startIcon={<SecurityIcon />}
                      onClick={() => navigate(`/users/${user.id}/roles`)}
                    >
                      Manage
                    </Button>
                  </CanAccess>
                )}
              </Box>

              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                {/* 🔧 MULTI-ROLE FIX: Enhanced role display handling */}
                {(() => {
                  const userRoles = getUserRoles(); // Get from context for consistency
                  
                  // Fallback to user object if context doesn't have roles
                  const displayRoles = userRoles.length > 0 ? userRoles : normalizeRoles(user.roles);
                  
                  console.log('🔍 UserProfile: Displaying roles:', {
                    userRolesFromContext: userRoles,
                    userRolesFromProps: user.roles,
                    finalDisplayRoles: displayRoles
                  });
                  
                  if (displayRoles.length === 0) {
                    return (
                      <Typography variant="body2" color="text.secondary" sx={{ fontStyle: 'italic' }}>
                        No roles assigned
                      </Typography>
                    );
                  }
                  
                  return displayRoles.map((roleName: string, index: number) => {
                    // Determine if this is a system role (rough heuristic)
                    const isSystemRole = ['SuperAdmin', 'SystemAdmin'].includes(roleName);
                    
                    return (
                      <Chip
                        key={`${roleName}-${index}`}
                        label={roleName}
                        size="small"
                        color={isSystemRole ? 'warning' : 'primary'}
                        variant="outlined"
                        sx={{ mb: 0.5 }}
                      />
                    );
                  });
                })()}
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}

export default UserProfile;
