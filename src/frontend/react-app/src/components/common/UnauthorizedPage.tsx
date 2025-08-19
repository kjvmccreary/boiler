import { Box, Typography, Button, Paper } from '@mui/material';
import { Lock, ArrowBack } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext.js';
import { useTenant } from '@/contexts/TenantContext.js';

export function UnauthorizedPage() {
  const navigate = useNavigate();
  const { user } = useAuth();
  const { currentTenant } = useTenant();

  const handleGoToDashboard = () => {
    navigate('/app/dashboard', { replace: true });
  };

  const handleGoBack = () => {
    navigate(-1);
  };

  return (
    <Box
      display="flex"
      flexDirection="column"
      alignItems="center"
      justifyContent="center"
      minHeight="100vh"
      sx={{ p: 3, backgroundColor: 'grey.50' }}
    >
      <Paper
        elevation={3}
        sx={{
          p: 4,
          maxWidth: 400,
          textAlign: 'center',
          borderRadius: 2
        }}
      >
        <Box sx={{ mb: 3 }}>
          <Lock
            sx={{
              fontSize: 64,
              color: 'warning.main',
              mb: 2
            }}
          />
          <Typography variant="h4" component="h1" gutterBottom>
            Access Denied
          </Typography>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 2 }}>
            You don't have permission to access this page
            {currentTenant && (
              <> in <strong>{currentTenant.name}</strong></>
            )}.
          </Typography>
          {user && (
            <Typography variant="body2" color="text.secondary">
              Signed in as <strong>{user.firstName} {user.lastName}</strong>
            </Typography>
          )}
        </Box>

        <Box sx={{ display: 'flex', gap: 2, justifyContent: 'center' }}>
          <Button
            variant="contained"
            startIcon={<ArrowBack />}
            onClick={handleGoToDashboard}
          >
            Go to Dashboard
          </Button>
          <Button
            variant="outlined"
            onClick={handleGoBack}
          >
            Go Back
          </Button>
        </Box>
      </Paper>
    </Box>
  );
}
