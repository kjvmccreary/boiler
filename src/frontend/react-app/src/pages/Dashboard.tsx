import {
  Box,
  Typography,
  Card,
  CardContent,
  Grid,
  Chip,
} from '@mui/material';
import {
  People as PeopleIcon,
  Security as SecurityIcon,
  Dashboard as DashboardIcon,
} from '@mui/icons-material';
import { useAuth } from '@/contexts/AuthContext.js';
import { usePermissions } from '@/contexts/PermissionContext.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';

export function Dashboard() {
  const { user } = useAuth();
  const { getUserPermissions, getUserRoles } = usePermissions();

  const userPermissions = getUserPermissions();
  const userRoles = getUserRoles();

  return (
    <Box>
      <Typography variant="h4" gutterBottom>
        Dashboard
      </Typography>
      
      <Typography variant="h6" color="text.secondary" gutterBottom>
        Welcome back, {user?.firstName}!
      </Typography>

      <Grid container spacing={3} sx={{ mt: 2 }}>
        {/* User Info Card */}
        <Grid size={{ xs: 12, md: 6, lg: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <PeopleIcon sx={{ mr: 1 }} />
                <Typography variant="h6">User Information</Typography>
              </Box>
              
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Name: {user?.firstName} {user?.lastName}
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Email: {user?.email}
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Tenant: {user?.tenantId}
              </Typography>
              <Typography variant="body2" color="text.secondary" gutterBottom>
                Email Confirmed: {user?.isEmailConfirmed ? 'Yes' : 'No'}
              </Typography>
            </CardContent>
          </Card>
        </Grid>

        {/* Roles Card */}
        <Grid size={{ xs: 12, md: 6, lg: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <SecurityIcon sx={{ mr: 1 }} />
                <Typography variant="h6">Your Roles</Typography>
              </Box>
              
              <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
                {userRoles.map((role) => (
                  <Chip
                    key={role}
                    label={role}
                    color="primary"
                    variant="outlined"
                    size="small"
                  />
                ))}
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Permissions Card */}
        <Grid size={{ xs: 12, md: 6, lg: 4 }}>
          <Card>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', mb: 2 }}>
                <DashboardIcon sx={{ mr: 1 }} />
                <Typography variant="h6">Permissions</Typography>
              </Box>
              
              <Typography variant="body2" color="text.secondary" gutterBottom>
                You have {userPermissions.length} permissions
              </Typography>
              
              <Box sx={{ maxHeight: 200, overflow: 'auto', mt: 1 }}>
                {userPermissions.slice(0, 10).map((permission) => (
                  <Chip
                    key={permission}
                    label={permission}
                    color="secondary"
                    variant="outlined"
                    size="small"
                    sx={{ m: 0.25 }}
                  />
                ))}
                {userPermissions.length > 10 && (
                  <Typography variant="caption" display="block" sx={{ mt: 1 }}>
                    +{userPermissions.length - 10} more...
                  </Typography>
                )}
              </Box>
            </CardContent>
          </Card>
        </Grid>

        {/* Quick Actions */}
        <CanAccess permission="users.view">
          <Grid size={{ xs: 12, md: 6 }}>
            <Card>
              <CardContent>
                <Typography variant="h6" gutterBottom>
                  Quick Actions
                </Typography>
                <Typography variant="body2" color="text.secondary">
                  Manage users, roles, and permissions from the navigation menu.
                </Typography>
              </CardContent>
            </Card>
          </Grid>
        </CanAccess>
      </Grid>
    </Box>
  );
}
