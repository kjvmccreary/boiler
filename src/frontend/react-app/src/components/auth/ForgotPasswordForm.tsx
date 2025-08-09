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
} from '@mui/material';
import { authService } from '@/services/auth.service.js';

export function ForgotPasswordForm() {
  const [email, setEmail] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState('');
  const [isSuccess, setIsSuccess] = useState(false);
  const [fieldError, setFieldError] = useState('');
  
  const navigate = useNavigate();

  const handleEmailChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setEmail(event.target.value);
    
    // Clear errors when user starts typing
    if (fieldError) setFieldError('');
    if (error) setError('');
  };

  const validateEmail = (): boolean => {
    if (!email) {
      setFieldError('Email is required');
      return false;
    }
    
    if (!/\S+@\S+\.\S+/.test(email)) {
      setFieldError('Please enter a valid email address');
      return false;
    }
    
    return true;
  };

  const handleSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    
    if (!validateEmail()) {
      return;
    }

    setIsLoading(true);
    setError('');

    try {
      await authService.forgotPassword(email);
      setIsSuccess(true);
    } catch (error) {
      console.error('Forgot password failed:', error);
      setError('Failed to send reset email. Please try again.');
    } finally {
      setIsLoading(false);
    }
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
              <Typography component="h1" variant="h4" align="center" gutterBottom>
                Check Your Email
              </Typography>
              
              <Alert severity="success" sx={{ mb: 3 }}>
                If an account with that email exists, we've sent you a password reset link.
              </Alert>

              <Typography variant="body2" align="center" sx={{ mb: 3 }}>
                Check your email inbox and click the reset link to set a new password.
              </Typography>

              <Button
                fullWidth
                variant="outlined"
                onClick={() => navigate('/login')}
                sx={{ mt: 2 }}
              >
                Back to Sign In
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
              Reset Password
            </Typography>
            
            <Typography variant="body2" align="center" color="text.secondary" sx={{ mb: 3 }}>
              Enter your email address and we'll send you a link to reset your password.
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
                id="email"
                label="Email Address"
                name="email"
                type="email"
                autoComplete="email"
                autoFocus
                value={email}
                onChange={handleEmailChange}
                error={!!fieldError}
                helperText={fieldError}
                disabled={isLoading}
              />
              
              <Button
                type="submit"
                fullWidth
                variant="contained"
                sx={{ mt: 3, mb: 2 }}
                disabled={isLoading || !email}
                startIcon={isLoading ? <CircularProgress size={20} /> : undefined}
              >
                {isLoading ? 'Sending...' : 'Send Reset Link'}
              </Button>
              
              <Box sx={{ textAlign: 'center' }}>
                <Link to="/login" style={{ textDecoration: 'none' }}>
                  <Typography variant="body2" color="primary">
                    Back to Sign In
                  </Typography>
                </Link>
              </Box>
            </Box>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}
