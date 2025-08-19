import { Container, Box, Typography, Button, Card, CardContent, Grid } from '@mui/material';
import { Business, Login, PersonAdd } from '@mui/icons-material';
import { Link, useNavigate } from 'react-router-dom'; // Add useNavigate

export function LandingPage() {
  const navigate = useNavigate(); // Add navigate hook

  const handleSignInClick = () => {
    console.log('🔍 Sign In button clicked, navigating to /login');
    navigate('/login');
  };

  const handleRegisterClick = () => {
    console.log('🔍 Register button clicked, navigating to /register');
    navigate('/register');
  };

  return (
    <Container component="main" maxWidth="lg">
      <Box
        sx={{
          marginTop: 8,
          display: 'flex',
          flexDirection: 'column',
          alignItems: 'center',
        }}
      >
        {/* Hero Section */}
        <Typography component="h1" variant="h3" align="center" gutterBottom>
          Welcome to Your Platform
        </Typography>
        
        <Typography variant="h6" align="center" color="text.secondary" sx={{ mb: 4, maxWidth: 600 }}>
          Manage your team, projects, and organization with our comprehensive platform.
        </Typography>

        {/* Action Cards */}
        <Grid container spacing={3} sx={{ mt: 4 }}>
          {/* Existing Users */}
          <Grid size={{ xs: 12, md: 6 }}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
              <CardContent sx={{ flexGrow: 1, textAlign: 'center', p: 4 }}>
                <Login color="primary" sx={{ fontSize: 48, mb: 2 }} />
                <Typography variant="h5" gutterBottom>
                  Already have an account?
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                  Sign in to access your organization and continue where you left off.
                </Typography>
                {/* 🔧 TRY: Using onClick handler instead of Link component */}
                <Button
                  onClick={handleSignInClick}
                  variant="contained"
                  size="large"
                  fullWidth
                >
                  Sign In
                </Button>
              </CardContent>
            </Card>
          </Grid>

          {/* New Users */}
          <Grid size={{ xs: 12, md: 6 }}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
              <CardContent sx={{ flexGrow: 1, textAlign: 'center', p: 4 }}>
                <Business color="primary" sx={{ fontSize: 48, mb: 2 }} />
                <Typography variant="h5" gutterBottom>
                  Start Your Organization
                </Typography>
                <Typography variant="body1" color="text.secondary" sx={{ mb: 3 }}>
                  Create your account and organization. You'll become the admin and can invite your team.
                </Typography>
                {/* 🔧 TRY: Using onClick handler instead of Link component */}
                <Button
                  onClick={handleRegisterClick}
                  variant="outlined"
                  size="large"
                  fullWidth
                  startIcon={<PersonAdd />}
                >
                  Create Account & Organization
                </Button>
              </CardContent>
            </Card>
          </Grid>
        </Grid>

        {/* Additional Info */}
        <Box sx={{ mt: 4, textAlign: 'center' }}>
          <Typography variant="body2" color="text.secondary">
            Join thousands of organizations already using our platform to streamline their operations.
          </Typography>
        </Box>
      </Box>
    </Container>
  );
}
