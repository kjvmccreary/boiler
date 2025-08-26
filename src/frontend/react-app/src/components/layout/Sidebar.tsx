import { useState } from 'react';
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
  Collapse,
} from '@mui/material';
import {
  Dashboard as DashboardIcon,
  People as PeopleIcon,
  Security as SecurityIcon,
  Settings as SettingsIcon,
  AccountTree as WorkflowIcon,
  Assignment as TaskIcon,
  Description as DefinitionIcon,
  PlayArrow as InstanceIcon,
  Edit as EditIcon,
  ExpandLess,
  ExpandMore,
} from '@mui/icons-material';
import { useNavigate, useLocation } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess';
import { NAVIGATION_ITEMS } from '@/routes/route.constants';

const ICON_MAP = {
  Dashboard: DashboardIcon,
  People: PeopleIcon,
  Security: SecurityIcon,
  Settings: SettingsIcon,
  AccountTree: WorkflowIcon,
  Assignment: TaskIcon,
  Description: DefinitionIcon,
  PlayArrow: InstanceIcon,
  Edit: EditIcon,
} as const;

interface SidebarProps {
  onItemClick?: () => void;
}

export function Sidebar({ onItemClick }: SidebarProps) {
  const navigate = useNavigate();
  const location = useLocation();
  const [expandedSections, setExpandedSections] = useState<Set<string>>(
    new Set(['Workflows']) // Default expand workflow section
  );

  const handleNavigation = (path: string) => {
    navigate(path);
    onItemClick?.();
  };

  const handleSectionToggle = (label: string) => {
    setExpandedSections(prev => {
      const newSet = new Set(prev);
      if (newSet.has(label)) {
        newSet.delete(label);
      } else {
        newSet.add(label);
      }
      return newSet;
    });
  };

  const isPathActive = (path?: string) => {
    if (!path) return false;
    return location.pathname === path || location.pathname.startsWith(path);
  };

  const isSectionActive = (children?: any[]) => {
    if (!children) return false;
    return children.some(child => isPathActive(child.path));
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
          const hasChildren = item.children && item.children.length > 0;
          const isExpanded = expandedSections.has(item.label);
          const isActive = hasChildren ? isSectionActive(item.children) : isPathActive(item.path);

          return (
            <CanAccess
              key={item.label}
              permission={item.permission}
            >
              <ListItem disablePadding>
                <ListItemButton
                  selected={isActive && !hasChildren}
                  onClick={() => {
                    if (hasChildren) {
                      handleSectionToggle(item.label);
                    } else if (item.path) {
                      handleNavigation(item.path);
                    }
                  }}
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
                    ...(isActive && hasChildren && {
                      backgroundColor: 'action.selected',
                    }),
                  }}
                >
                  <ListItemIcon>
                    <IconComponent />
                  </ListItemIcon>
                  <ListItemText primary={item.label} />
                  {hasChildren && (isExpanded ? <ExpandLess /> : <ExpandMore />)}
                </ListItemButton>
              </ListItem>

              {/* Nested items for sections with children */}
              {hasChildren && (
                <Collapse in={isExpanded} timeout="auto" unmountOnExit>
                  <List component="div" disablePadding>
                    {item.children?.map((child) => {
                      const ChildIconComponent = ICON_MAP[child.icon as keyof typeof ICON_MAP];
                      const isChildActive = isPathActive(child.path);

                      return (
                        <CanAccess
                          key={child.path}
                          permission={child.permission}
                        >
                          <ListItem disablePadding>
                            <ListItemButton
                              selected={isChildActive}
                              onClick={() => handleNavigation(child.path)}
                              sx={{
                                pl: 4, // Indent nested items
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
                                <ChildIconComponent fontSize="small" />
                              </ListItemIcon>
                              <ListItemText 
                                primary={child.label}
                                primaryTypographyProps={{ variant: 'body2' }}
                              />
                            </ListItemButton>
                          </ListItem>
                        </CanAccess>
                      );
                    })}
                  </List>
                </Collapse>
              )}
            </CanAccess>
          );
        })}
      </List>
    </Box>
  );
}
