import { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
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
  Divider,
  FormControlLabel,
  Checkbox,
  Tooltip,
  InputAdornment,
  IconButton,
} from '@mui/material';
import { HelpOutline, Business, Person } from '@mui/icons-material';
import { useAuth } from '@/contexts/AuthContext.js';
import { ROUTES } from '@/routes/route.constants.js';

function SelfServeRegisterForm() {
  const [formData, setFormData] = useState({
    // Personal Information
    email: '',
    password: '',
    confirmPassword: '',
    firstName: '',
    lastName: '',
    
    // Organization Information
    tenantName: '',
    createNewTenant: true,
  });
  
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({});
  
  const { register, isLoading, error, clearError } = useAuth();
  const navigate = useNavigate();

  const handleInputChange = (field: string) => (event: React.ChangeEvent<HTMLInputElement>) => {
    const value = field === 'createNewTenant' ? event.target.checked : event.target.value;
    
    setFormData(prev => ({
      ...prev,
      [field]: value,
    }));
    
    // Clear field error when user starts typing
    if (fieldErrors[field]) {
      setFieldErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
    
    // Clear general error
    if (error) {
      clearError();
    }
  };

  const validateForm = (): boolean => {
    const errors: Record<string, string> = {};
    
    // Personal Information Validation
    if (!formData.firstName.trim()) {
      errors.firstName = 'First name is required';
    }
    
    if (!formData.lastName.trim()) {
      errors.lastName = 'Last name is required';
    }
    
    if (!formData.email) {
      errors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
      errors.email = 'Please enter a valid email address';
    }
    
    if (!formData.password) {
      errors.password = 'Password is required';
    } else if (formData.password.length < 8) {
      errors.password = 'Password must be at least 8 characters';
    } else if (!/(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]/.test(formData.password)) {
      errors.password = 'Password must contain uppercase, lowercase, number, and special character';
    }
    
    if (!formData.confirmPassword) {
      errors.confirmPassword = 'Please confirm your password';
    } else if (formData.password !== formData.confirmPassword) {
      errors.confirmPassword = 'Passwords do not match';
    }
    
    // Organization Information Validation
    if (formData.createNewTenant) {
      if (!formData.tenantName.trim()) {
        errors.tenantName = 'Organization name is required';
      } else if (formData.tenantName.trim().length < 3) {
        errors.tenantName = 'Organization name must be at least 3 characters';
      } else if (!/^[a-zA-Z0-9\s\-&.,'()]+$/.test(formData.tenantName.trim())) {
        errors.tenantName = 'Organization name contains invalid characters';
      }
    }
    
    setFieldErrors(errors);
    return Object.keys(errors).length === 0;
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    if (!validateForm()) {
      return;
    }

    try {
      const registrationData = {
        email: formData.email.trim(),
        password: formData.password,
        confirmPassword: formData.confirmPassword,
        firstName: formData.firstName.trim(),
        lastName: formData.lastName.trim(),
        ...(formData.createNewTenant && { tenantName: formData.tenantName.trim() })
      };

      await register(registrationData);
      navigate(ROUTES.DASHBOARD); // 🔧 FIX: Use constant instead of hardcoded path
    } catch (error) {
      console.error('Registration failed:', error);
    }
  };

  return (
    <Container component="main" maxWidth="md">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        <Card sx={{ width: '100%', maxWidth: 600 }}>
          <CardContent sx={{ p: 4 }}>
            <Typography component="h1" variant="h4" align="center" gutterBottom>
              Create Your Account
            </Typography>
            
            <Typography variant="body2" align="center" color="text.secondary" sx={{ mb: 3 }}>
              Join thousands of organizations using our platform
            </Typography>
            
            {error && (
              <Alert severity="error" sx={{ mb: 3 }}>
                {error}
              </Alert>
            )}

            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
              
              {/* Personal Information Section */}
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Person color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6" color="primary">
                  Personal Information
                </Typography>
              </Box>
              
              <Box sx={{ display: 'flex', gap: 2, mb: 2 }}>
                <TextField
                  required
                  fullWidth
                  id="firstName"
                  label="First Name"
                  name="firstName"
                  autoComplete="given-name"
                  value={formData.firstName}
                  onChange={handleInputChange('firstName')}
                  error={!!fieldErrors.firstName}
                  helperText={fieldErrors.firstName}
                  disabled={isLoading}
                />
                
                <TextField
                  required
                  fullWidth
                  id="lastName"
                  label="Last Name"
                  name="lastName"
                  autoComplete="family-name"
                  value={formData.lastName}
                  onChange={handleInputChange('lastName')}
                  error={!!fieldErrors.lastName}
                  helperText={fieldErrors.lastName}
                  disabled={isLoading}
                />
              </Box>
              
              <TextField
                margin="normal"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                value={formData.email}
                onChange={handleInputChange('email')}
                error={!!fieldErrors.email}
                helperText={fieldErrors.email || "You'll use this to sign in to your account"}
                disabled={isLoading}
              />
              
              <TextField
                margin="normal"
                required
                fullWidth
                name="password"
                label="Password"
                type="password"
                id="password"
                autoComplete="new-password"
                value={formData.password}
                onChange={handleInputChange('password')}
                error={!!fieldErrors.password}
                helperText={fieldErrors.password || "Minimum 8 characters with uppercase, lowercase, number, and special character"}
                disabled={isLoading}
              />
              
              <TextField
                margin="normal"
                required
                fullWidth
                name="confirmPassword"
                label="Confirm Password"
                type="password"
                id="confirmPassword"
                autoComplete="new-password"
                value={formData.confirmPassword}
                onChange={handleInputChange('confirmPassword')}
                error={!!fieldErrors.confirmPassword}
                helperText={fieldErrors.confirmPassword}
                disabled={isLoading}
              />
              
              <Divider sx={{ my: 3 }} />
              
              {/* Organization Information Section */}
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <Business color="primary" sx={{ mr: 1 }} />
                <Typography variant="h6" color="primary">
                  Organization Setup
                </Typography>
                <Tooltip title="Create your own organization or join an existing one later">
                  <IconButton size="small" sx={{ ml: 1 }}>
                    <HelpOutline fontSize="small" />
                  </IconButton>
                </Tooltip>
              </Box>
              
              <FormControlLabel
                control={
                  <Checkbox
                    checked={formData.createNewTenant}
                    onChange={handleInputChange('createNewTenant')}
                    name="createNewTenant"
                    color="primary"
                    disabled={isLoading}
                  />
                }
                label={
                  <Box>
                    <Typography variant="body1">
                      Create a new organization
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      You'll be the administrator and can invite team members
                    </Typography>
                  </Box>
                }
                sx={{ mb: 2, alignItems: 'flex-start' }}
              />
              
              {formData.createNewTenant && (
                <TextField
                  margin="normal"
                  required
                  fullWidth
                  id="tenantName"
                  label="Organization Name"
                  name="tenantName"
                  placeholder="e.g., Acme Corporation"
                  value={formData.tenantName}
                  onChange={handleInputChange('tenantName')}
                  error={!!fieldErrors.tenantName}
                  helperText={fieldErrors.tenantName || "This will be your organization's display name"}
                  disabled={isLoading}
                  InputProps={{
                    startAdornment: (
                      <InputAdornment position="start">
                        <Business color="action" />
                      </InputAdornment>
                    ),
                  }}
                />
              )}
              
              {formData.createNewTenant && (
                <Alert severity="info" sx={{ mt: 2 }}>
                  <Typography variant="body2" sx={{ mb: 1 }}>
                    <strong>Already have an account?</strong>
                  </Typography>
                  <Typography variant="body2">
                    If you're setting up a new organization for a client or partner, 
                    you can use your existing email address. We'll add this organization 
                    to your account so you can manage multiple organizations.
                  </Typography>
                </Alert>
              )}
              
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 4, mb: 2, py: 1.5 }}
                disabled={isLoading}
                startIcon={isLoading ? <CircularProgress size={20} /> : undefined}
              >
                {isLoading 
                  ? 'Creating Account...' 
                  : formData.createNewTenant 
                    ? 'Create Account & Organization' 
                    : 'Create Account'
                }
              </Button>
              
              <Box sx={{ textAlign: 'center' }}>
                <Typography variant="body2">
                  Already have an account?{' '}
                  <Link to="/login" style={{ textDecoration: 'none' }}>
                    <Typography component="span" color="primary">
                      Sign in
                    </Typography>
                  </Link>
                </Typography>
              </Box>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}

export { SelfServeRegisterForm };
export default SelfServeRegisterForm;
