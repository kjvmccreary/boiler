import {
  Toolbar,
  IconButton,
  Typography,
  Box,
} from '@mui/material';
import { Menu as MenuIcon } from '@mui/icons-material';
import { UserMenu } from './UserMenu.js';
import { useTenant } from '@/contexts/TenantContext.js';
import { AppTaskBell } from './AppTaskBell'; // <-- NEW import

interface HeaderProps {
  onMenuClick: () => void;
  showMenuButton: boolean;
}

export function Header({ onMenuClick, showMenuButton }: HeaderProps) {
  const { currentTenant } = useTenant();

  return (
    <Toolbar sx={{ minHeight: '64px !important', gap: 1 }}>
      {showMenuButton && (
        <IconButton
          color="inherit"
          aria-label="open drawer"
          edge="start"
          onClick={onMenuClick}
          sx={{ mr: 1 }}
        >
          <MenuIcon />
        </IconButton>
      )}

      <Typography
        variant="h6"
        noWrap
        component="div"
        sx={{ flexGrow: 1, fontWeight: 600, letterSpacing: '.25px' }}
      >
        {currentTenant?.name || 'Microservices Starter'}
      </Typography>

      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.25 }}>
        <AppTaskBell /> {/* <-- NEW task bell */}
        <UserMenu />
      </Box>
    </Toolbar>
  );
}

export default Header;
