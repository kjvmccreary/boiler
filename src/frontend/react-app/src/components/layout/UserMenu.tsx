import { useState } from 'react';
import {
  IconButton,
  Menu,
  MenuItem,
  Avatar,
  Typography,
  Box,
  Divider,
  ListItemIcon,
  Chip,
} from '@mui/material';
import {
  AccountCircle,
  Settings as SettingsIcon,
  Logout as LogoutIcon,
  Business,
  Add as AddIcon, // 🔧 ADD: Import Add icon
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '@/contexts/AuthContext.js';
import { useTenant } from '@/contexts/TenantContext.js';
import { LogoutButton } from '@/components/auth/LogoutButton.js';
import { TenantSwitcher } from './TenantSwitcher.js';
import { CreateAdditionalTenant } from '@/components/tenant/CreateAdditionalTenant.js'; // 🔧 ADD: Import dialog
import { ROUTES } from '@/routes/route.constants.js';

interface UserMenuProps {
  useLogoutButton?: boolean;
}

export function UserMenu({ useLogoutButton = false }: UserMenuProps) {
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [showCreateTenant, setShowCreateTenant] = useState(false); // 🔧 ADD: State for dialog
  const { user, logout } = useAuth();
  const { currentTenant, availableTenants } = useTenant();
  const navigate = useNavigate();

  const handleMenu = (event: React.MouseEvent<HTMLElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleClose = () => {
    setAnchorEl(null);
  };

  const handleProfile = () => {
    navigate(ROUTES.PROFILE);
    handleClose();
  };

  const handleSettings = () => {
    navigate(ROUTES.SETTINGS);
    handleClose();
  };

  // 🔧 ADD: Handler for create organization
  const handleCreateOrganization = () => {
    setShowCreateTenant(true);
    handleClose();
  };

  const handleLogout = async () => {
    try {
      await logout();
      navigate(ROUTES.LOGIN);
    } catch (error) {
      console.error('Logout failed:', error);
    } finally {
      handleClose();
    }
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  return (
    <Box>
      <IconButton
        size="large"
        aria-label="account of current user"
        aria-controls="user-menu"
        aria-haspopup="true"
        onClick={handleMenu}
        color="inherit"
      >
        <Avatar sx={{ width: 32, height: 32, bgcolor: 'secondary.main' }}>
          {user ? getInitials(user.firstName, user.lastName) : <AccountCircle />}
        </Avatar>
      </IconButton>
      
      <Menu
        id="user-menu"
        anchorEl={anchorEl}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'right',
        }}
        keepMounted
        transformOrigin={{
          vertical: 'top',
          horizontal: 'right',
        }}
        open={Boolean(anchorEl)}
        onClose={handleClose}
      >
        {user && (
          <Box sx={{ px: 2, py: 1 }}>
            <Typography variant="subtitle1" noWrap>
              {user.firstName} {user.lastName}
            </Typography>
            <Typography variant="body2" color="text.secondary" noWrap>
              {user.email}
            </Typography>
            {currentTenant && (
              <Box sx={{ mt: 1, display: 'flex', alignItems: 'center', gap: 1 }}>
                <Business fontSize="small" color="action" />
                <Chip
                  label={currentTenant.name}
                  size="small"
                  variant="outlined"
                  color="primary"
                />
              </Box>
            )}
          </Box>
        )}
        <Divider />
        
        <MenuItem onClick={handleProfile}>
          <ListItemIcon>
            <AccountCircle fontSize="small" />
          </ListItemIcon>
          Profile
        </MenuItem>
        
        <MenuItem onClick={handleSettings}>
          <ListItemIcon>
            <SettingsIcon fontSize="small" />
          </ListItemIcon>
          Settings
        </MenuItem>
        
        {/* 🔧 ADD: Create Organization menu item */}
        <MenuItem onClick={handleCreateOrganization}>
          <ListItemIcon>
            <AddIcon fontSize="small" />
          </ListItemIcon>
          Create Organization
        </MenuItem>
        
        {/* 🔧 REPLACE: Use new TenantSwitcher component */}
        {availableTenants.length > 1 && (
          <TenantSwitcher variant="menu-item" onClose={handleClose} />
        )}
        
        <Divider />
        
        {useLogoutButton ? (
          <Box sx={{ px: 1, py: 0.5 }}>
            <LogoutButton
              variant="text"
              showConfirmation={true}
              onLogout={handleClose}
              sx={{ 
                width: '100%', 
                justifyContent: 'flex-start',
                color: 'text.primary',
                '&:hover': {
                  bgcolor: 'action.hover',
                }
              }}
            />
          </Box>
        ) : (
          <MenuItem onClick={handleLogout}>
            <ListItemIcon>
              <LogoutIcon fontSize="small" />
            </ListItemIcon>
            Logout
          </MenuItem>
        )}
      </Menu>

      {/* 🔧 ADD: Create Additional Tenant Dialog */}
      <CreateAdditionalTenant
        open={showCreateTenant}
        onClose={() => setShowCreateTenant(false)}
        onSuccess={(tenant) => {
          console.log('✅ New organization created:', tenant.name);
          // Dialog already handles tenant switching
        }}
      />
    </Box>
  );
}
