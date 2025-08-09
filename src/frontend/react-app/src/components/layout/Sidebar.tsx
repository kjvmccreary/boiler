import {
  Box,
  List,
  ListItem,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
  Divider,
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  People as PeopleIcon,
  Security as SecurityIcon,
  Settings as SettingsIcon,
} from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { NAVIGATION_ITEMS } from '@/routes/route.constants.js';

const ICON_MAP = {
  Dashboard: DashboardIcon,
  People: PeopleIcon,
  Security: SecurityIcon,
  Settings: SettingsIcon,
} as const;

interface SidebarProps {
  onItemClick?: () => void;
}

export function Sidebar({ onItemClick }: SidebarProps) {
  const navigate = useNavigate();
  const location = useLocation();

  const handleNavigation = (path: string) => {
    navigate(path);
    onItemClick?.();
  };

  return (
    <Box>
      <Toolbar>
        <Typography variant="h6" noWrap component="div">
          Navigation
        </Typography>
      </Toolbar>
      <Divider />
      
      <List>
        {NAVIGATION_ITEMS.map((item) => {
          const IconComponent = ICON_MAP[item.icon as keyof typeof ICON_MAP];
          const isActive = location.pathname === item.path;

          return (
            <CanAccess
              key={item.path}
              permission={item.permission}
              requireAuthentication={true}
            >
              <ListItem disablePadding>
                <ListItemButton
                  selected={isActive}
                  onClick={() => handleNavigation(item.path)}
                  sx={{
                    '&.Mui-selected': {
                      backgroundColor: 'primary.main',
                      color: 'primary.contrastText',
                      '&:hover': {
                        backgroundColor: 'primary.dark',
                      },
                      '& .MuiListItemIcon-root': {
                        color: 'inherit',
                      },
                    },
                  }}
                >
                  <ListItemIcon>
                    <IconComponent />
                  </ListItemIcon>
                  <ListItemText primary={item.label} />
                </ListItemButton>
              </ListItem>
            </CanAccess>
          );
        })}
      </List>
    </Box>
  );
}
