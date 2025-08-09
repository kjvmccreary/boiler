import { Box, Typography, Card, CardContent, Chip } from '@mui/material';
import { useAuth } from '@/contexts/AuthContext.js';
import { usePermission } from '@/contexts/PermissionContext.js';

export function Dashboard() {
  const { user } = useAuth();
  const { getUserRoles, getUserPermissions } = usePermission();

  if (!user) {
    return (
      <Box sx={{ p: 3 }}>
        <Typography variant="h4">Loading...</Typography>
      </Box>
    );
  }

  const userRoles = getUserRoles();
  const userPermissions = getUserPermissions();

  return (
    <Box sx={{ p: 3 }}>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>

      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: {
            xs: '1fr',
            md: 'repeat(2, 1fr)',
          },
          gap: 3,
          mb: 3,
        }}
      >
        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              User Profile
            </Typography>
            <Typography variant="body1">
              Welcome, {user.fullName}!
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Email: {user.email}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Email Confirmed: {user.emailConfirmed ? 'Yes' : 'No'}
            </Typography>
            <Typography variant="body2" color="text.secondary">
              Status: {user.isActive ? 'Active' : 'Inactive'}
            </Typography>
          </CardContent>
        </Card>

        <Card>
          <CardContent>
            <Typography variant="h6" gutterBottom>
              Your Roles
            </Typography>
            <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
              {userRoles.map((roleName) => (
                <Chip
                  key={roleName}
                  label={roleName}
                  color="primary"
                  variant="outlined"
                />
              ))}
            </Box>
          </CardContent>
        </Card>
      </Box>

      <Card>
        <CardContent>
          <Typography variant="h6" gutterBottom>
            Your Permissions
          </Typography>
          <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
            {userPermissions.slice(0, 10).map((permission) => (
              <Chip
                key={permission}
                label={permission}
                size="small"
                color="secondary"
                variant="outlined"
              />
            ))}
            {userPermissions.length > 10 && (
              <Chip
                label={`+${userPermissions.length - 10} more`}
                size="small"
                color="default"
              />
            )}
          </Box>
        </CardContent>
      </Card>
    </Box>
  );
}

export default Dashboard;
