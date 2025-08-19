import { useState, useEffect } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
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
} from '@mui/material';
import { useAuth } from '@/contexts/AuthContext.js';
import { useTenant } from '@/contexts/TenantContext.js';
import { TenantSelector } from './TenantSelector.js';
import { ROUTES } from '@/routes/route.constants.js';

interface LocationState {
  from?: {
    pathname: string;
  };
}

interface FormErrors {
  email: string;
  password: string;
}

export function EnhancedLoginForm() {
  const [formData, setFormData] = useState({
    email: '',
    password: '',
  });
  const [errors, setErrors] = useState<FormErrors>({
    email: '',
    password: '',
  });
  
  const { login, isLoading, error, clearError, isAuthenticated } = useAuth();
  const { 
    showTenantSelector, 
    completeTenantSelection, 
    currentTenant,
    error: tenantError 
  } = useTenant();
  
  const navigate = useNavigate();
  const location = useLocation();
  
  const state = location.state as LocationState;
  const from = state?.from?.pathname || ROUTES.DASHBOARD; // 🔧 FIX: Use constant instead of hardcoded path

  // Redirect to dashboard when both authenticated and tenant selected
  useEffect(() => {
    if (isAuthenticated && currentTenant && !showTenantSelector) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, currentTenant, showTenantSelector, navigate, from]);

  const handleInputChange = (field: 'email' | 'password') => (event: React.ChangeEvent<HTMLInputElement>) => {
    setFormData(prev => ({
      ...prev,
      [field]: event.target.value,
    }));
    
    // Clear field error when user starts typing
    if (errors[field]) {
      setErrors(prev => ({
        ...prev,
        [field]: '',
      }));
    }
    
    // Clear general error
    if (error) {
      clearError();
    }
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    // Create a fresh errors object
    const newErrors: FormErrors = {
      email: '',
      password: '',
    };
    
    // Validate email
    if (!formData.email.trim()) {
      newErrors.email = 'Email is required';
    } else if (!/\S+@\S+\.\S+/.test(formData.email.trim())) {
      newErrors.email = 'Please enter a valid email address';
    }
    
    // Validate password
    if (!formData.password.trim()) {
      newErrors.password = 'Password is required';
    }
    
    // Check if there are any errors
    const hasErrors = newErrors.email !== '' || newErrors.password !== '';
    
    if (hasErrors) {
      setErrors(newErrors);
      return;
    }

    try {
      // This will trigger the auth flow and tenant loading
      await login(formData.email, formData.password);
      // Navigation will be handled by the useEffect above
    } catch (error) {
      // Error is handled by the auth context
      console.error('Login failed:', error);
    }
  };

  const handleTenantSelected = async (tenantId: string) => {
    try {
      await completeTenantSelection(tenantId);
      // Navigation will be handled by the useEffect above
    } catch (err) {
      console.error('Tenant selection failed:', err);
    }
  };

  // Show tenant selector if user is authenticated but needs to select tenant
  if (isAuthenticated && showTenantSelector) {
    return <TenantSelector onTenantSelected={handleTenantSelected} />;
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
              Sign In
            </Typography>
            
            {(error || tenantError) && (
              <Alert severity="error" sx={{ mb: 2 }}>
                {error || tenantError}
              </Alert>
            )}

            <Box component="form" onSubmit={handleSubmit} sx={{ mt: 1 }}>
              <TextField
                margin="normal"
                required
                fullWidth
                id="email"
                label="Email Address"
                name="email"
                autoComplete="email"
                autoFocus
                value={formData.email}
                onChange={handleInputChange('email')}
                error={!!errors.email}
                helperText={errors.email}
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
                autoComplete="current-password"
                value={formData.password}
                onChange={handleInputChange('password')}
                error={!!errors.password}
                helperText={errors.password}
                disabled={isLoading}
              />
              
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={isLoading}
                startIcon={isLoading ? <CircularProgress size={20} /> : undefined}
              >
                {isLoading ? 'Signing In...' : 'Sign In'}
              </Button>
              
              <Box sx={{ textAlign: 'center' }}>
                <Link to="/forgot-password" style={{ textDecoration: 'none' }}>
                  <Typography variant="body2" color="primary">
                    Forgot password?
                  </Typography>
                </Link>
                
                <Typography variant="body2" sx={{ mt: 1 }}>
                  Don't have an account?{' '}
                  <Link to="/register" style={{ textDecoration: 'none' }}>
                    <Typography component="span" color="primary">
                      Sign up
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

export default EnhancedLoginForm;
