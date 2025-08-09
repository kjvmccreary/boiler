import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Checkbox,
  FormGroup,
  FormControlLabel,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  TextField,
  InputAdornment,
  FormControl,
  FormLabel,
  Alert,
  CircularProgress,
  Chip,
  Button,
  IconButton,
  Tooltip,
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  Search as SearchIcon,
  SelectAll as SelectAllIcon,
  ClearAll as ClearAllIcon,
  Info as InfoIcon,
} from '@mui/icons-material';
import { permissionService, type PermissionsByCategory } from '@/services/permission.service.js';

interface PermissionSelectorProps {
  value: string[];
  onChange: (permissions: string[]) => void;
  disabled?: boolean;
  label?: string;
  helperText?: string;
  error?: boolean;
  fullWidth?: boolean;
  showSearch?: boolean;
  showSelectAll?: boolean;
  showSelectedCount?: boolean;
}

interface CategoryState {
  isExpanded: boolean;
  selectedCount: number;
  totalCount: number;
  indeterminate: boolean;
  allSelected: boolean;
}

export function PermissionSelector({
  value = [],
  onChange,
  disabled = false,
  label = 'Permissions',
  helperText,
  error = false,
  fullWidth = true,
  showSearch = true,
  showSelectAll = true,
  showSelectedCount = true,
}: PermissionSelectorProps) {
  const [permissions, setPermissions] = useState<PermissionsByCategory>({});
  const [filteredPermissions, setFilteredPermissions] = useState<PermissionsByCategory>({});
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [errorMessage, setErrorMessage] = useState<string>('');
  const [categoryStates, setCategoryStates] = useState<Record<string, CategoryState>>({});

  // Load permissions on mount
  useEffect(() => {
    loadPermissions();
  }, []);

  // Update filtered permissions when search term changes
  useEffect(() => {
    filterPermissions();
  }, [permissions, searchTerm]);

  // Update category states when filtered permissions or selected values change
  useEffect(() => {
    updateCategoryStates();
  }, [filteredPermissions, value]);

  const loadPermissions = async () => {
    try {
      setIsLoading(true);
      setErrorMessage('');
      
      const data = await permissionService.getPermissionsGrouped();
      setPermissions(data);
    } catch (err) {
      console.error('Failed to load permissions:', err);
      setErrorMessage('Failed to load permissions. Please try again.');
    } finally {
      setIsLoading(false);
    }
  };

  const filterPermissions = () => {
    if (!searchTerm.trim()) {
      setFilteredPermissions(permissions);
      return;
    }

    const filtered: PermissionsByCategory = {};
    const searchLower = searchTerm.toLowerCase();

    Object.entries(permissions).forEach(([category, perms]) => {
      const filteredPerms = perms.filter(
        (perm) =>
          perm.name.toLowerCase().includes(searchLower) ||
          perm.description.toLowerCase().includes(searchLower) ||
          category.toLowerCase().includes(searchLower)
      );

      if (filteredPerms.length > 0) {
        filtered[category] = filteredPerms;
      }
    });

    setFilteredPermissions(filtered);
  };

  const updateCategoryStates = () => {
    const newStates: Record<string, CategoryState> = {};

    Object.entries(filteredPermissions).forEach(([category, perms]) => {
      const categoryPermissions = perms.map(p => p.name);
      const selectedInCategory = categoryPermissions.filter(p => value.includes(p));
      const selectedCount = selectedInCategory.length;
      const totalCount = categoryPermissions.length;
      const allSelected = selectedCount === totalCount && totalCount > 0;
      const indeterminate = selectedCount > 0 && selectedCount < totalCount;

      newStates[category] = {
        isExpanded: indeterminate || allSelected || searchTerm.trim() !== '',
        selectedCount,
        totalCount,
        indeterminate,
        allSelected,
      };
    });

    setCategoryStates(newStates);
  };

  const handlePermissionChange = (permissionName: string, checked: boolean) => {
    if (disabled) return;

    const newValue = checked
      ? [...value, permissionName]
      : value.filter(p => p !== permissionName);

    onChange(newValue);
  };

  const handleCategoryChange = (category: string, checked: boolean) => {
    if (disabled) return;

    const categoryPermissions = filteredPermissions[category]?.map(p => p.name) || [];
    
    const newValue = checked
      ? [...new Set([...value, ...categoryPermissions])]
      : value.filter(p => !categoryPermissions.includes(p));

    onChange(newValue);
  };

  const handleSelectAll = () => {
    if (disabled) return;

    const allPermissions = Object.values(filteredPermissions)
      .flat()
      .map(p => p.name);

    onChange([...new Set([...value, ...allPermissions])]);
  };

  const handleDeselectAll = () => {
    if (disabled) return;

    const allPermissions = Object.values(filteredPermissions)
      .flat()
      .map(p => p.name);

    onChange(value.filter(p => !allPermissions.includes(p)));
  };

  const handleExpandCategory = (category: string) => {
    setCategoryStates(prev => ({
      ...prev,
      [category]: {
        ...prev[category],
        isExpanded: !prev[category]?.isExpanded,
      },
    }));
  };

  const totalSelected = value.length;
  const totalAvailable = Object.values(filteredPermissions).flat().length;

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', p: 3 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (errorMessage) {
    return (
      <Alert severity="error" action={
        <Button onClick={loadPermissions} size="small">
          Retry
        </Button>
      }>
        {errorMessage}
      </Alert>
    );
  }

  return (
    <FormControl fullWidth={fullWidth} error={error} disabled={disabled}>
      <FormLabel component="legend" sx={{ mb: 1, fontWeight: 'medium' }}>
        {label}
        {showSelectedCount && (
          <Chip 
            label={`${totalSelected} selected`} 
            size="small" 
            sx={{ ml: 1 }} 
            color={totalSelected > 0 ? 'primary' : 'default'}
          />
        )}
      </FormLabel>

      {/* Search and Controls */}
      {(showSearch || showSelectAll) && (
        <Box sx={{ mb: 2, display: 'flex', gap: 1, alignItems: 'center' }}>
          {showSearch && (
            <TextField
              size="small"
              placeholder="Search permissions..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              disabled={disabled}
              slotProps={{
                input: {
                  startAdornment: (
                    <InputAdornment position="start">
                      <SearchIcon />
                    </InputAdornment>
                  ),
                },
              }}
              sx={{ flexGrow: 1 }}
            />
          )}

          {showSelectAll && (
            <Box sx={{ display: 'flex', gap: 1 }}>
              <Tooltip title="Select all visible permissions">
                <IconButton 
                  onClick={handleSelectAll} 
                  disabled={disabled || totalAvailable === 0}
                  size="small"
                >
                  <SelectAllIcon />
                </IconButton>
              </Tooltip>
              <Tooltip title="Deselect all visible permissions">
                <IconButton 
                  onClick={handleDeselectAll} 
                  disabled={disabled || totalSelected === 0}
                  size="small"
                >
                  <ClearAllIcon />
                </IconButton>
              </Tooltip>
            </Box>
          )}
        </Box>
      )}

      {/* No Results */}
      {totalAvailable === 0 && (
        <Alert severity="info" sx={{ mb: 2 }}>
          {searchTerm ? 'No permissions match your search.' : 'No permissions available.'}
        </Alert>
      )}

      {/* Permission Categories */}
      <Box sx={{ maxHeight: 400, overflow: 'auto' }}>
        {Object.entries(filteredPermissions).map(([category, perms]) => {
          const categoryState = categoryStates[category] || {
            isExpanded: false,
            selectedCount: 0,
            totalCount: perms.length,
            indeterminate: false,
            allSelected: false,
          };

          return (
            <Accordion
              key={category}
              expanded={categoryState.isExpanded}
              onChange={() => handleExpandCategory(category)}
              disabled={disabled}
              sx={{ 
                '&:before': { display: 'none' },
                boxShadow: 1,
                mb: 1,
              }}
            >
              <AccordionSummary expandIcon={<ExpandMoreIcon />}>
                <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
                  <FormControlLabel
                    control={
                      <Checkbox
                        checked={categoryState.allSelected}
                        indeterminate={categoryState.indeterminate}
                        onChange={(e) => handleCategoryChange(category, e.target.checked)}
                        disabled={disabled}
                        onClick={(e) => e.stopPropagation()}
                      />
                    }
                    label={
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <Typography variant="subtitle2" component="span">
                          {category}
                        </Typography>
                        <Chip 
                          label={`${categoryState.selectedCount}/${categoryState.totalCount}`}
                          size="small"
                          variant="outlined"
                          color={categoryState.selectedCount > 0 ? 'primary' : 'default'}
                        />
                      </Box>
                    }
                    sx={{ flexGrow: 1, mr: 0 }}
                  />
                </Box>
              </AccordionSummary>

              <AccordionDetails sx={{ pt: 0 }}>
                <FormGroup>
                  {perms.map((permission) => (
                    <FormControlLabel
                      key={permission.id}
                      control={
                        <Checkbox
                          checked={value.includes(permission.name)}
                          onChange={(e) => handlePermissionChange(permission.name, e.target.checked)}
                          disabled={disabled}
                          size="small"
                        />
                      }
                      label={
                        <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                          <Typography variant="body2" component="span">
                            {permission.name}
                          </Typography>
                          {permission.description && (
                            <Tooltip title={permission.description}>
                              <InfoIcon sx={{ fontSize: 16, color: 'text.secondary' }} />
                            </Tooltip>
                          )}
                        </Box>
                      }
                      sx={{ ml: 2 }}
                    />
                  ))}
                </FormGroup>
              </AccordionDetails>
            </Accordion>
          );
        })}
      </Box>

      {/* Helper Text */}
      {helperText && (
        <Typography variant="caption" color="text.secondary" sx={{ mt: 1 }}>
          {helperText}
        </Typography>
      )}
    </FormControl>
  );
}
