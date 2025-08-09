import { useState, useEffect } from 'react';
import {
  Box,
  Typography,
  Accordion,
  AccordionSummary,
  AccordionDetails,
  FormControlLabel,
  Checkbox,
  Chip,
  TextField,
  InputAdornment,
} from '@mui/material';
import {
  ExpandMore as ExpandMoreIcon,
  Search as SearchIcon,
} from '@mui/icons-material';
import { permissionService, type PermissionCategory } from '@/services/permission.service.js';

interface PermissionSelectorProps {
  selectedPermissions: string[];
  onChange: (permissions: string[]) => void;
  disabled?: boolean;
}

export function PermissionSelector({ selectedPermissions, onChange, disabled = false }: PermissionSelectorProps) {
  const [permissionCategories, setPermissionCategories] = useState<PermissionCategory[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadPermissions();
  }, []);

  const loadPermissions = async () => {
    try {
      setLoading(true);
      const permissions = await permissionService.getAllPermissions();
      const categories = permissionService.organizePermissionsByCategory(permissions);
      setPermissionCategories(categories);
    } catch (error) {
      console.error('Failed to load permissions:', error);
    } finally {
      setLoading(false);
    }
  };

  const handlePermissionChange = (permissionName: string, checked: boolean) => {
    if (disabled) return;

    const newPermissions = checked
      ? [...selectedPermissions, permissionName]
      : selectedPermissions.filter(p => p !== permissionName);
    
    onChange(newPermissions);
  };

  const handleCategoryChange = (category: PermissionCategory, checked: boolean) => {
    if (disabled) return;

    const categoryPermissions = category.permissions.map(p => p.name);
    
    let newPermissions: string[];
    if (checked) {
      // Add all category permissions
      newPermissions = [...new Set([...selectedPermissions, ...categoryPermissions])];
    } else {
      // Remove all category permissions
      newPermissions = selectedPermissions.filter(p => !categoryPermissions.includes(p));
    }
    
    onChange(newPermissions);
  };

  const isCategorySelected = (category: PermissionCategory): boolean => {
    const categoryPermissions = category.permissions.map(p => p.name);
    return categoryPermissions.every(p => selectedPermissions.includes(p));
  };

  const isCategoryPartiallySelected = (category: PermissionCategory): boolean => {
    const categoryPermissions = category.permissions.map(p => p.name);
    const selectedFromCategory = categoryPermissions.filter(p => selectedPermissions.includes(p));
    return selectedFromCategory.length > 0 && selectedFromCategory.length < categoryPermissions.length;
  };

  const filteredCategories = permissionCategories.map(category => ({
    ...category,
    permissions: category.permissions.filter(permission =>
      permission.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
      permission.description?.toLowerCase().includes(searchTerm.toLowerCase()) ||
      category.category.toLowerCase().includes(searchTerm.toLowerCase())
    )
  })).filter(category => category.permissions.length > 0);

  if (loading) {
    return (
      <Box sx={{ p: 2 }}>
        <Typography>Loading permissions...</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ mb: 2 }}>
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
      </Box>

      <Box sx={{ mb: 2 }}>
        <Typography variant="body2" color="text.secondary">
          Selected: {selectedPermissions.length} permissions
        </Typography>
      </Box>

      {filteredCategories.map((category) => {
        const isSelected = isCategorySelected(category);
        const isPartiallySelected = isCategoryPartiallySelected(category);

        return (
          <Accordion key={category.category} defaultExpanded>
            <AccordionSummary expandIcon={<ExpandMoreIcon />}>
              <Box sx={{ display: 'flex', alignItems: 'center', width: '100%' }}>
                <FormControlLabel
                  control={
                    <Checkbox
                      checked={isSelected}
                      indeterminate={isPartiallySelected}
                      onChange={(e) => handleCategoryChange(category, e.target.checked)}
                      disabled={disabled}
                      onClick={(e) => e.stopPropagation()}
                    />
                  }
                  label={
                    <Box sx={{ display: 'flex', alignItems: 'center' }}>
                      <Typography variant="subtitle2" sx={{ mr: 1 }}>
                        {category.category}
                      </Typography>
                      <Chip 
                        label={category.permissions.length} 
                        size="small" 
                        color="primary" 
                        variant="outlined"
                      />
                    </Box>
                  }
                  onClick={(e) => e.stopPropagation()}
                />
              </Box>
            </AccordionSummary>
            
            <AccordionDetails>
              <Box sx={{ pl: 2 }}>
                {category.permissions.map((permission) => (
                  <FormControlLabel
                    key={permission.name}
                    control={
                      <Checkbox
                        checked={selectedPermissions.includes(permission.name)}
                        onChange={(e) => handlePermissionChange(permission.name, e.target.checked)}
                        disabled={disabled}
                        size="small"
                      />
                    }
                    label={
                      <Box>
                        <Typography variant="body2">
                          {permission.name}
                        </Typography>
                        {permission.description && (
                          <Typography variant="caption" color="text.secondary">
                            {permission.description}
                          </Typography>
                        )}
                      </Box>
                    }
                    sx={{ display: 'block', mb: 1 }}
                  />
                ))}
              </Box>
            </AccordionDetails>
          </Accordion>
        );
      })}
    </Box>
  );
}
