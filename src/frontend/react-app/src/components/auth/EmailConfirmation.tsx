import { useState, useEffect } from 'react';
import { useSearchParams, useNavigate, Link } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Alert,
  Button,
  CircularProgress,
  Container,
} from '@mui/material';
import {
  CheckCircle as CheckCircleIcon,
  Error as ErrorIcon,
  Email as EmailIcon,
} from '@mui/icons-material';
import { authService } from '@/services/auth.service.js';
import { ROUTES } from '@/routes/route.constants.js';
import toast from 'react-hot-toast';

interface ConfirmationState {
  status: 'loading' | 'success' | 'error' | 'invalid';
  message: string;
}

export function EmailConfirmation() {
  const [searchParams] = useSearchParams();
  const navigate = useNavigate();
  const [state, setState] = useState<ConfirmationState>({
    status: 'loading',
    message: 'Confirming your email address...',
  });

  const token = searchParams.get('token');

  useEffect(() => {
    if (!token) {
      setState({
        status: 'invalid',
        message: 'Invalid confirmation link. The token is missing.',
      });
      return;
    }

    confirmEmail(token);
  }, [token]);

  const confirmEmail = async (confirmationToken: string) => {
    try {
      setState({
        status: 'loading',
        message: 'Confirming your email address...',
      });

      await authService.confirmEmail(confirmationToken);

      setState({
        status: 'success',
        message: 'Your email address has been successfully confirmed! You can now sign in with your account.',
      });

      toast.success('Email confirmed successfully!');
    } catch (error) {
      console.error('Email confirmation failed:', error);
      
      let errorMessage = 'Failed to confirm your email address.';
      
      if (error instanceof Error) {
        // Handle specific error messages from the backend
        if (error.message.includes('expired')) {
          errorMessage = 'The confirmation link has expired. Please request a new confirmation email.';
        } else if (error.message.includes('invalid')) {
          errorMessage = 'The confirmation link is invalid or has already been used.';
        } else if (error.message.includes('not found')) {
          errorMessage = 'The confirmation token was not found. Please request a new confirmation email.';
        } else {
          errorMessage = error.message;
        }
      }

      setState({
        status: 'error',
        message: errorMessage,
      });

      toast.error('Email confirmation failed');
    }
  };

  const handleResendConfirmation = () => {
    // Navigate to a resend confirmation page or show a form
    // For now, redirect to login with a message
    toast('Please log in and request a new confirmation email from your profile.', {
      duration: 4000,
      icon: '📧',
    });
    navigate(ROUTES.LOGIN);
  };

  const handleGoToLogin = () => {
    navigate(ROUTES.LOGIN);
  };

  const renderContent = () => {
    switch (state.status) {
      case 'loading':
        return (
          <>
            <Box sx={{ display: 'flex', justifyContent: 'center', mb: 2 }}>
              <CircularProgress size={48} />
            </Box>
            <Typography variant="h5" align="center" gutterBottom>
              Confirming Email
            </Typography>
            <Typography variant="body1" align="center" color="text.secondary">
              {state.message}
            </Typography>
          </>
        );

      case 'success':
        return (
          <>
            <Box sx={{ display: 'flex', justifyContent: 'center', mb: 2 }}>
              <CheckCircleIcon sx={{ fontSize: 48, color: 'success.main' }} />
            </Box>
            <Typography variant="h5" align="center" gutterBottom color="success.main">
              Email Confirmed!
            </Typography>
            <Typography variant="body1" align="center" color="text.secondary" sx={{ mb: 3 }}>
              {state.message}
            </Typography>
            <Box sx={{ display: 'flex', justifyContent: 'center' }}>
              <Button
                variant="contained"
                onClick={handleGoToLogin}
                size="large"
              >
                Sign In Now
              </Button>
            </Box>
          </>
        );

      case 'error':
      case 'invalid':
        return (
          <>
            <Box sx={{ display: 'flex', justifyContent: 'center', mb: 2 }}>
              <ErrorIcon sx={{ fontSize: 48, color: 'error.main' }} />
            </Box>
            <Typography variant="h5" align="center" gutterBottom color="error.main">
              Confirmation Failed
            </Typography>
            <Alert severity="error" sx={{ mb: 3 }}>
              {state.message}
            </Alert>
            <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
              <Button
                variant="outlined"
                onClick={handleResendConfirmation}
                startIcon={<EmailIcon />}
              >
                Resend Confirmation
              </Button>
              <Button
                variant="contained"
                onClick={handleGoToLogin}
              >
                Back to Login
              </Button>
            </Box>
          </>
        );

      default:
        return null;
    }
  };

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
        <Card sx={{ width: '100%', maxWidth: 500 }}>
          <CardContent sx={{ p: 4, textAlign: 'center' }}>
            {renderContent()}
          </CardContent>
        </Card>

        <Box sx={{ mt: 2, textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            Need help?{' '}
            <Link to={ROUTES.LOGIN} style={{ textDecoration: 'none' }}>
              Contact Support
            </Link>
          </Typography>
        </Box>
      </Box>
    </Container>
  );
}

export default EmailConfirmation;
