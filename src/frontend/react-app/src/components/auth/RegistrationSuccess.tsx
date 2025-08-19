import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Container,
} from '@mui/material';
import { Business, People, CheckCircle } from '@mui/icons-material'; // 🔧 FIX: Move CheckCircle here
import { useTenant } from '@/contexts/TenantContext.js';

export function RegistrationSuccess() {
  const navigate = useNavigate();
  const { currentTenant } = useTenant();

  useEffect(() => {
    // Auto redirect after 10 seconds
    const timer = setTimeout(() => {
      navigate('/dashboard');
    }, 10000);

    return () => clearTimeout(timer);
  }, [navigate]);

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
            <CheckCircle 
              color="success" 
              sx={{ fontSize: 64, mb: 2 }} 
            />
            
            <Typography component="h1" variant="h4" gutterBottom>
              Welcome to {currentTenant?.name || 'Your Organization'}!
            </Typography>
            
            <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
              Your account and organization have been successfully created.
              You're now the administrator with full access to manage your team and settings.
            </Typography>

            <Box sx={{ display: 'flex', justifyContent: 'space-around', mb: 3 }}>
              <Box>
                <Business color="primary" sx={{ fontSize: 32 }} />
                <Typography variant="body2" color="text.secondary">
                  Organization Created
                </Typography>
              </Box>
              <Box>
                <People color="primary" sx={{ fontSize: 32 }} />
                <Typography variant="body2" color="text.secondary">
                  Ready to Invite Team
                </Typography>
              </Box>
            </Box>

            <Button
              fullWidth
              variant="contained"
              size="large"
              onClick={() => navigate('/dashboard')}
              sx={{ mb: 2 }}
            >
              Go to Dashboard
            </Button>

            <Typography variant="body2" color="text.secondary">
              Redirecting automatically in 10 seconds...
            </Typography>
          </CardContent>
        </Card>
      </Box>
    </Container>
  );
}
