import {
  Toolbar,
  IconButton,
  Typography,
  Box,
} from '@mui/material';
import { Menu as MenuIcon } from '@mui/icons-material';
import { UserMenu } from './UserMenu.js';
import { useTenant } from '@/contexts/TenantContext.js'; // 🔧 ADD: Import tenant context

interface HeaderProps {
  onMenuClick: () => void;
  showMenuButton: boolean;
}

export function Header({ onMenuClick, showMenuButton }: HeaderProps) {
  const { currentTenant } = useTenant(); // 🔧 ADD: Get current tenant

  return (
    <Toolbar>
      {showMenuButton && (
        <IconButton
          color="inherit"
          aria-label="open drawer"
          edge="start"
          onClick={onMenuClick}
          sx={{ mr: 2 }}
        >
          <MenuIcon />
        </IconButton>
      )}
      
      <Typography variant="h6" noWrap component="div" sx={{ flexGrow: 1 }}>
        {/* 🔧 CHANGE: Replace hardcoded text with tenant name */}
        {currentTenant?.name || 'Microservices Starter'}
      </Typography>
      
      <Box sx={{ display: 'flex', alignItems: 'center' }}>
        <UserMenu />
      </Box>
    </Toolbar>
  );
}
