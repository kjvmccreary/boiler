import { useState, useEffect, useMemo } from 'react';
import {
  Box,
  Typography,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  Chip,
  Alert,
  TextField,
  InputAdornment,
  Collapse,
  Tooltip,
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  Security as SecurityIcon,
  VpnKey as VpnKeyIcon,
  People as PeopleIcon,
  Settings as SettingsIcon,
  Business as BusinessIcon,
  Search as SearchIcon,
  Visibility as ViewIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  SupervisorAccount as AdminIcon,
  Assessment as AssessmentIcon,
} from '@mui/icons-material';
import { LoadingSpinner } from '@/components/common/LoadingStates.js';
import { permissionService, type PermissionsByCategory, type PermissionDto } from '@/services/permission.service.js';
import toast from 'react-hot-toast';

interface PermissionTreeViewProps {
  selectedPermissions?: string[];
  onPermissionClick?: (permission: PermissionDto) => void;
  showSearch?: boolean;
  highlightSelected?: boolean;
  readOnly?: boolean;
  compactMode?: boolean;
}

const getCategoryIcon = (category: string) => {
  const categoryLower = category.toLowerCase();
  
  if (categoryLower.includes('user')) return <PeopleIcon />;
  if (categoryLower.includes('role')) return <SecurityIcon />;
  if (categoryLower.includes('tenant')) return <BusinessIcon />;
  if (categoryLower.includes('system')) return <AdminIcon />;
  if (categoryLower.includes('report')) return <AssessmentIcon />;
  if (categoryLower.includes('billing')) return <BusinessIcon />;
  
  return <SettingsIcon />;
};

const getPermissionIcon = (permissionName: string) => {
  const name = permissionName.toLowerCase();
  
  if (name.includes('view') || name.includes('read')) return <ViewIcon fontSize="small" />;
  if (name.includes('edit') || name.includes('update')) return <EditIcon fontSize="small" />;
  if (name.includes('delete') || name.includes('remove')) return <DeleteIcon fontSize="small" />;
  if (name.includes('create') || name.includes('add')) return <AddIcon fontSize="small" />;
  if (name.includes('manage') || name.includes('admin')) return <AdminIcon fontSize="small" />;
  
  return <VpnKeyIcon fontSize="small" />;
};

const getPermissionColor = (permissionName: string): 'default' | 'primary' | 'secondary' | 'error' | 'warning' | 'info' | 'success' => {
  const name = permissionName.toLowerCase();
  
  if (name.includes('delete') || name.includes('remove')) return 'error';
  if (name.includes('edit') || name.includes('update')) return 'warning';
  if (name.includes('create') || name.includes('add')) return 'success';
  if (name.includes('view') || name.includes('read')) return 'info';
  if (name.includes('manage') || name.includes('admin')) return 'secondary';
  
  return 'primary';
};

export function PermissionTreeView({
  selectedPermissions = [],
  onPermissionClick,
  showSearch = true,
  highlightSelected = true,
  readOnly = false,
  compactMode = false,
}: PermissionTreeViewProps) {
  const [permissionsByCategory, setPermissionsByCategory] = useState<PermissionsByCategory>({});
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [expandedCategories, setExpandedCategories] = useState<Set<string>>(new Set());

  // Load permissions on mount
  useEffect(() => {
    loadPermissions();
  }, []);

  // Auto-expand categories with selected permissions
  useEffect(() => {
    if (Object.keys(permissionsByCategory).length > 0 && selectedPermissions.length > 0) {
      const categoriesToExpand = new Set<string>();
      
      Object.entries(permissionsByCategory).forEach(([category, permissions]) => {
        const hasSelectedPermission = permissions.some(p => selectedPermissions.includes(p.name));
        if (hasSelectedPermission) {
          categoriesToExpand.add(category);
        }
      });
      
      setExpandedCategories(categoriesToExpand);
    }
  }, [permissionsByCategory, selectedPermissions]);

  const loadPermissions = async () => {
    try {
      setLoading(true);
      setError(null);
      const permissions = await permissionService.getPermissionsGrouped();
      setPermissionsByCategory(permissions);
      
      // Expand first category by default
      const firstCategory = Object.keys(permissions)[0];
      if (firstCategory) {
        setExpandedCategories(new Set([firstCategory]));
      }
    } catch (err) {
      const errorMessage = err instanceof Error ? err.message : 'Failed to load permissions';
      setError(errorMessage);
      toast.error(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  // Filter permissions based on search term
  const filteredPermissions = useMemo(() => {
    if (!searchTerm) return permissionsByCategory;

    const filtered: PermissionsByCategory = {};
    const searchLower = searchTerm.toLowerCase();

    Object.entries(permissionsByCategory).forEach(([category, permissions]) => {
      const matchingPermissions = permissions.filter(
        permission =>
          permission.name.toLowerCase().includes(searchLower) ||
          permission.description?.toLowerCase().includes(searchLower) ||
          category.toLowerCase().includes(searchLower)
      );

      if (matchingPermissions.length > 0) {
        filtered[category] = matchingPermissions;
      }
    });

    return filtered;
  }, [permissionsByCategory, searchTerm]);

  const handleCategoryExpansion = (category: string) => {
    const newExpanded = new Set(expandedCategories);
    if (newExpanded.has(category)) {
      newExpanded.delete(category);
    } else {
      newExpanded.add(category);
    }
    setExpandedCategories(newExpanded);
  };

  const handlePermissionClick = (permission: PermissionDto) => {
    if (!readOnly && onPermissionClick) {
      onPermissionClick(permission);
    }
  };

  const formatPermissionName = (name: string) => {
    // Convert "users.view" to "View Users"
    const parts = name.split('.');
    if (parts.length === 2) {
      const action = parts[1].replace(/_/g, ' ');
      const resource = parts[0];
      return `${action.charAt(0).toUpperCase() + action.slice(1)} ${resource.charAt(0).toUpperCase() + resource.slice(1)}`;
    }
    return name.replace(/_/g, ' ').replace(/\b\w/g, l => l.toUpperCase());
  };

  const getCategoryStats = (permissions: PermissionDto[]) => {
    const selectedCount = permissions.filter(p => selectedPermissions.includes(p.name)).length;
    const totalCount = permissions.length;
    return { selectedCount, totalCount };
  };

  if (loading) {
    return (
      <Box sx={{ p: 2 }}>
        <LoadingSpinner message="Loading permissions..." />
      </Box>
    );
  }

  if (error) {
    return (
      <Alert severity="error">
        {error}
      </Alert>
    );
  }

  return (
    <Box>
      {/* Header */}
      <Box sx={{ mb: 2 }}>
        <Typography variant="h6" sx={{ display: 'flex', alignItems: 'center', mb: 1 }}>
          <SecurityIcon sx={{ mr: 1 }} />
          Permission Hierarchy
        </Typography>

        {showSearch && (
          <TextField
            fullWidth
            size="small"
            placeholder="Search permissions..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            slotProps={{
              input: {
                startAdornment: (
                  <InputAdornment position="start">
                    <SearchIcon />
                  </InputAdornment>
                ),
              },
            }}
          />
        )}
      </Box>

      {/* Permission tree */}
      <Box>
        {Object.keys(filteredPermissions).length === 0 ? (
          <Alert severity="info">
            {searchTerm ? `No permissions found matching "${searchTerm}"` : 'No permissions available'}
          </Alert>
        ) : (
          Object.entries(filteredPermissions).map(([category, permissions]) => {
            const { selectedCount, totalCount } = getCategoryStats(permissions);
            const isExpanded = expandedCategories.has(category);
            
            return (
              <Accordion
                key={category}
                expanded={isExpanded}
                onChange={() => handleCategoryExpansion(category)}
                sx={{ mb: 1 }}
              >
                <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                  <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
                    {getCategoryIcon(category)}
                    
                    <Box sx={{ flex: 1, ml: 2 }}>
                      <Typography variant="subtitle1" sx={{ fontWeight: 'medium' }}>
                        {category}
                      </Typography>
                      
                      {highlightSelected && selectedCount > 0 && (
                        <Typography variant="caption" color="text.secondary">
                          {selectedCount} of {totalCount} selected
                        </Typography>
                      )}
                    </Box>

                    <Box sx={{ display: 'flex', gap: 1 }}>
                      <Chip
                        label={totalCount}
                        size="small"
                        variant="outlined"
                      />
                      
                      {highlightSelected && selectedCount > 0 && (
                        <Chip
                          label={selectedCount}
                          size="small"
                          color="primary"
                        />
                      )}
                    </Box>
                  </Box>
                </AccordionSummary>

                <AccordionDetails sx={{ pt: 0 }}>
                  <List dense={compactMode} sx={{ width: '100%' }}>
                    {permissions.map((permission) => {
                      const isSelected = selectedPermissions.includes(permission.name);
                      
                      return (
                        <>
                          {!readOnly && !!onPermissionClick ? (
                            <ListItem
                              key={permission.name}
                              component="div"
                              onClick={() => handlePermissionClick(permission)}
                              sx={{
                                borderRadius: 1,
                                mb: 0.5,
                                bgcolor: highlightSelected && isSelected ? 'action.selected' : 'transparent',
                                '&:hover': {
                                  bgcolor: 'action.hover',
                                },
                                cursor: 'pointer',
                              }}
                            >
                              <ListItemIcon sx={{ minWidth: 36 }}>
                                {getPermissionIcon(permission.name)}
                              </ListItemIcon>
                              
                              <ListItemText
                                primary={
                                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                    <Typography variant="body2" sx={{ fontWeight: isSelected ? 'medium' : 'normal' }}>
                                      {formatPermissionName(permission.name)}
                                    </Typography>
                                    
                                    <Chip
                                      label={getPermissionColor(permission.name)}
                                      size="small"
                                      color={getPermissionColor(permission.name)}
                                      variant="outlined"
                                      sx={{ fontSize: '0.7rem', height: 20 }}
                                    />
                                  </Box>
                                }
                                secondary={
                                  <Box>
                                    <Typography variant="caption" color="text.secondary" sx={{ fontFamily: 'monospace' }}>
                                      {permission.name}
                                    </Typography>
                                    {permission.description && (
                                      <Typography variant="caption" color="text.secondary" display="block">
                                        {permission.description}
                                      </Typography>
                                    )}
                                  </Box>
                                }
                              />

                              {highlightSelected && isSelected && (
                                <Tooltip title="Permission selected">
                                  <SecurityIcon color="primary" fontSize="small" />
                                </Tooltip>
                              )}
                            </ListItem>
                          ) : (
                            <ListItem
                              key={permission.name}
                              sx={{
                                borderRadius: 1,
                                mb: 0.5,
                                bgcolor: highlightSelected && isSelected ? 'action.selected' : 'transparent',
                              }}
                            >
                              <ListItemIcon sx={{ minWidth: 36 }}>
                                {getPermissionIcon(permission.name)}
                              </ListItemIcon>
                              
                              <ListItemText
                                primary={
                                  <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                                    <Typography variant="body2" sx={{ fontWeight: isSelected ? 'medium' : 'normal' }}>
                                      {formatPermissionName(permission.name)}
                                    </Typography>
                                    
                                    <Chip
                                      label={getPermissionColor(permission.name)}
                                      size="small"
                                      color={getPermissionColor(permission.name)}
                                      variant="outlined"
                                      sx={{ fontSize: '0.7rem', height: 20 }}
                                    />
                                  </Box>
                                }
                                secondary={
                                  <Box>
                                    <Typography variant="caption" color="text.secondary" sx={{ fontFamily: 'monospace' }}>
                                      {permission.name}
                                    </Typography>
                                    {permission.description && (
                                      <Typography variant="caption" color="text.secondary" display="block">
                                        {permission.description}
                                      </Typography>
                                    )}
                                  </Box>
                                }
                              />

                              {highlightSelected && isSelected && (
                                <Tooltip title="Permission selected">
                                  <SecurityIcon color="primary" fontSize="small" />
                                </Tooltip>
                              )}
                            </ListItem>
                          )}
                        </>
                      );
                    })}
                  </List>
                </AccordionDetails>
              </Accordion>
            );
          })
        )}
      </Box>

      {/* Summary */}
      {highlightSelected && selectedPermissions.length > 0 && (
        <Collapse in>
          <Alert severity="info" sx={{ mt: 2 }}>
            <Typography variant="body2">
              {selectedPermissions.length} permission{selectedPermissions.length !== 1 ? 's' : ''} selected across{' '}
              {Object.keys(filteredPermissions).filter(category => 
                filteredPermissions[category].some(p => selectedPermissions.includes(p.name))
              ).length} categor{Object.keys(filteredPermissions).filter(category => 
                filteredPermissions[category].some(p => selectedPermissions.includes(p.name))
              ).length !== 1 ? 'ies' : 'y'}
            </Typography>
          </Alert>
        </Collapse>
      )}
    </Box>
  );
}

export default PermissionTreeView;
