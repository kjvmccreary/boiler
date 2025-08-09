import { useState } from 'react';
import {
  Box,
  Card,
  CardContent,
  TextField,
  Button,
  Typography,
  Alert,
  CircularProgress,
  Container,
  InputAdornment,
  IconButton,
} from '@mui/material';
import {
  Visibility,
  VisibilityOff,
  CheckCircle as CheckCircleIcon,
} from '@mui/icons-material';
import { authService, type ChangePasswordRequest } from '@/services/auth.service.js';
import { useAuth } from '@/contexts/AuthContext.js';

interface FormData {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

interface FormErrors {
  currentPassword?: string;
  newPassword?: string;
  confirmNewPassword?: string;
}

interface PasswordVisibility {
  current: boolean;
  new: boolean;
  confirm: boolean;
}

export function ChangePasswordForm() {
  const [formData, setFormData] = useState<FormData>({
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: '',
  });
  const [fieldErrors, setFieldErrors] = useState<FormErrors>({});
  const [showPasswords, setShowPasswords] = useState<PasswordVisibility>({
    current: false,
    new: false,
    confirm: false,
  });
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);

  const { logout } = useAuth();

  const handleInputChange = (field: keyof FormData) => (event: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({
      ...prev,
      [field]: event.target.value,
    }));
    
    // Clear field error when user starts typing
    if (fieldErrors[field]) {
      setFieldErrors(prev => ({
        ...prev,
        [field]: undefined,
      }));
    }
    
    // Clear general error
    if (error) setError('');
    
    // Clear success state if user starts editing again
    if (isSuccess) setIsSuccess(false);
  };

  const togglePasswordVisibility = (field: keyof PasswordVisibility) => {
    setShowPasswords(prev => ({
      ...prev,
      [field]: !prev[field],
    }));
  };

  const validatePassword = (password: string): string | undefined => {
    if (!password) {
      return 'Password is required';
    }
    
    if (password.length < 8) {
      return 'Password must be at least 8 characters';
    }
    
    if (!/(?=.*[a-z])/.test(password)) {
      return 'Password must contain at least one lowercase letter';
    }
    
    if (!/(?=.*[A-Z])/.test(password)) {
      return 'Password must contain at least one uppercase letter';
    }
    
    if (!/(?=.*\d)/.test(password)) {
      return 'Password must contain at least one number';
    }
    
    if (!/(?=.*[@$!%*?&])/.test(password)) {
      return 'Password must contain at least one special character (@$!%*?&)';
    }
    
    return undefined;
  };

  const validateForm = (): boolean => {
    const errors: FormErrors = {};
    
    // Current password validation
    if (!formData.currentPassword) {
      errors.currentPassword = 'Current password is required';
    }
    
    // New password validation
    const newPasswordError = validatePassword(formData.newPassword);
    if (newPasswordError) {
      errors.newPassword = newPasswordError;
    }
    
    // Check if new password is different from current
    if (formData.currentPassword && formData.newPassword && 
        formData.currentPassword === formData.newPassword) {
      errors.newPassword = 'New password must be different from current password';
    }
    
    // Confirm password validation
    if (!formData.confirmNewPassword) {
      errors.confirmNewPassword = 'Please confirm your new password';
    } else if (formData.newPassword !== formData.confirmNewPassword) {
      errors.confirmNewPassword = 'New password and confirmation do not match';
    }
    
    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      const changePasswordRequest: ChangePasswordRequest = {
        currentPassword: formData.currentPassword,
        newPassword: formData.newPassword,
        confirmNewPassword: formData.confirmNewPassword,
      };

      await authService.changePassword(changePasswordRequest);
      setIsSuccess(true);
      
      // Clear form
      setFormData({
        currentPassword: '',
        newPassword: '',
        confirmNewPassword: '',
      });
      
      // Note: Backend revokes all refresh tokens, so user will need to login again
      // We could auto-logout here, but let's give them a choice
      
    } catch (err: any) {
      console.error('Change password failed:', err);
      
      // Handle specific error messages from backend
      if (err.response?.data?.message) {
        setError(err.response.data.message);
      } else if (err.response?.status === 400) {
        setError('Current password is incorrect or new password is invalid.');
      } else {
        setError('Failed to change password. Please try again.');
      }
    } finally {
      setIsLoading(false);
    }
  };

  const handleLoginAgain = () => {
    logout();
  };

  if (isSuccess) {
    return (
      <Container component="main" maxWidth="sm">
        <Box
          sx={{
            marginTop: 8,
            display: 'flex',
            flexDirection: 'column',
            alignItems: 'center',
          }}
        >
          <Card sx={{ width: '100%', maxWidth: 400 }}>
            <CardContent sx={{ p: 4 }}>
              <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center', mb: 2 }}>
                <CheckCircleIcon color="success" sx={{ fontSize: 48 }} />
              </Box>
              
              <Typography component="h1" variant="h4" align="center" gutterBottom>
                Password Changed
              </Typography>
              
              <Alert severity="success" sx={{ mb: 3 }}>
                Your password has been successfully changed!
              </Alert>

              <Typography variant="body2" align="center" sx={{ mb: 3 }}>
                For security reasons, you'll need to sign in again with your new password.
              </Typography>

              <Button
                fullWidth
                variant="contained"
                onClick={handleLoginAgain}
              >
                Sign In Again
              </Button>
            </CardContent>
          </Card>
        </Box>
      </Container>
    );
  }

  return (
    <Container component="main" maxWidth="sm">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Card sx={{ width: '100%', maxWidth: 400 }}>
          <CardContent sx={{ p: 4 }}>
            <Typography component="h1" variant="h4" align="center" gutterBottom>
              Change Password
            </Typography>
            
            <Typography variant="body2" align="center" color="text.secondary" sx={{ mb: 3 }}>
              Enter your current password and choose a new secure password.
            </Typography>
            
            {error && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error}
              </Alert>
            )}

            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
              <TextField
                margin="normal"
                required
                fullWidth
                name="currentPassword"
                label="Current Password"
                type={showPasswords.current ? 'text' : 'password'}
                id="currentPassword"
                autoComplete="current-password"
                value={formData.currentPassword}
                onChange={handleInputChange('currentPassword')}
                error={!!fieldErrors.currentPassword}
                helperText={fieldErrors.currentPassword}
                disabled={isLoading}
                slotProps={{
                  input: {
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => togglePasswordVisibility('current')}
                          edge="end"
                          disabled={isLoading}
                          aria-label={showPasswords.current ? 'Hide password' : 'Show password'}
                        >
                          {showPasswords.current ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  },
                }}
              />
              
              <TextField
                margin="normal"
                required
                fullWidth
                name="newPassword"
                label="New Password"
                type={showPasswords.new ? 'text' : 'password'}
                id="newPassword"
                autoComplete="new-password"
                value={formData.newPassword}
                onChange={handleInputChange('newPassword')}
                error={!!fieldErrors.newPassword}
                helperText={fieldErrors.newPassword || 'At least 8 characters with uppercase, lowercase, number, and special character'}
                disabled={isLoading}
                slotProps={{
                  input: {
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => togglePasswordVisibility('new')}
                          edge="end"
                          disabled={isLoading}
                          aria-label={showPasswords.new ? 'Hide password' : 'Show password'}
                        >
                          {showPasswords.new ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  },
                }}
              />
              
              <TextField
                margin="normal"
                required
                fullWidth
                name="confirmNewPassword"
                label="Confirm New Password"
                type={showPasswords.confirm ? 'text' : 'password'}
                id="confirmNewPassword"
                autoComplete="new-password"
                value={formData.confirmNewPassword}
                onChange={handleInputChange('confirmNewPassword')}
                error={!!fieldErrors.confirmNewPassword}
                helperText={fieldErrors.confirmNewPassword}
                disabled={isLoading}
                slotProps={{
                  input: {
                    endAdornment: (
                      <InputAdornment position="end">
                        <IconButton
                          onClick={() => togglePasswordVisibility('confirm')}
                          edge="end"
                          disabled={isLoading}
                          aria-label={showPasswords.confirm ? 'Hide password' : 'Show password'}
                        >
                          {showPasswords.confirm ? <VisibilityOff /> : <Visibility />}
                        </IconButton>
                      </InputAdornment>
                    ),
                  },
                }}
              />
              
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={isLoading}
                startIcon={isLoading ? <CircularProgress size={20} /> : undefined}
              >
                {isLoading ? 'Changing Password...' : 'Change Password'}
              </Button>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}

export default ChangePasswordForm;
