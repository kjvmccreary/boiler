import React, { useState, useEffect } from 'react';
import {
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  TablePagination,
  TextField,
  InputAdornment,
  IconButton,
  Chip,
  Box,
  Typography,
  CircularProgress,
  Tooltip,
  Popover,
  List,
  ListItem,
  ListItemText,
  Avatar,
  Divider
} from '@mui/material';
import { 
  Search, 
  Edit, 
  Delete, 
  People as PeopleIcon,
  Close as CloseIcon // ✅ ADD: Close icon for the popover
} from '@mui/icons-material';
import type { RoleDto, UserInfo } from '../../types';
import { roleService } from '../../services/role.service';

interface RoleListProps {
  onEditRole?: (roleId: number) => void;
  onDeleteRole?: (roleId: number) => void;
}

export const RoleList: React.FC<RoleListProps> = ({
  onEditRole,
  onDeleteRole
}) => {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  
  // ✅ SIMPLIFIED: Basic popover state
  const [userPopover, setUserPopover] = useState<{
    anchorEl: HTMLElement | null;
    roleId: number | null;
    users: UserInfo[];
    loading: boolean;
  }>({
    anchorEl: null,
    roleId: null,
    users: [],
    loading: false
  });

  const [pagination, setPagination] = useState({
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 0
  });

  const fetchRoles = async (page = 1, pageSize = 10, search = '') => {
    console.log('🔍 RoleList: fetchRoles called', { page, pageSize, search });
    
    try {
      setLoading(true);
      setError(null);

      const result = await roleService.getRoles({
        page,
        pageSize,
        searchTerm: search || undefined
      });

      console.log('✅ RoleList: fetchRoles result:', result);

      setRoles(result.roles || []);
      setPagination(result.pagination || {
        totalCount: 0,
        pageNumber: page,
        pageSize: pageSize,
        totalPages: 0
      });
      
    } catch (err) {
      console.error('❌ RoleList: Failed to fetch roles:', err);
      setError(err instanceof Error ? err.message : 'Failed to fetch roles');
      setRoles([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    console.log('🔍 RoleList: Initial load');
    fetchRoles(1, 10, '');
  }, []);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      console.log('🔍 RoleList: Search term changed:', searchTerm);
      fetchRoles(1, pagination?.pageSize || 10, searchTerm);
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [searchTerm]);

  const handlePageChange = (_event: unknown, newPage: number) => {
    const pageNumber = newPage + 1;
    console.log('🔍 RoleList: Page change to:', pageNumber);
    fetchRoles(pageNumber, pagination?.pageSize || 10, searchTerm);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newPageSize = parseInt(event.target.value, 10);
    console.log('🔍 RoleList: Page size change to:', newPageSize);
    fetchRoles(1, newPageSize, searchTerm);
  };

  const canDeleteRole = (role: RoleDto): boolean => {
    return !role.isSystemRole && (role.userCount || 0) === 0;
  };

  const getDeleteTooltipText = (role: RoleDto): string => {
    if (role.isSystemRole) {
      return "Cannot delete system roles";
    }
    if ((role.userCount || 0) > 0) {
      return `Cannot delete role with ${role.userCount} assigned user${role.userCount === 1 ? '' : 's'}`;
    }
    return "Delete role";
  };

  // ✅ SIMPLE: Show popup on hover
  const handleUserCountHover = async (event: React.MouseEvent<HTMLElement>, roleId: number, userCount: number) => {
    if (userCount === 0) return;

    console.log('🔍 RoleList: User count hovered for role:', roleId);
    
    setUserPopover({
      anchorEl: event.currentTarget,
      roleId,
      users: [],
      loading: true
    });

    try {
      const users = await roleService.getRoleUsers(roleId);
      console.log('✅ RoleList: Users loaded for role:', roleId, users);
      
      setUserPopover(prev => ({
        ...prev,
        users,
        loading: false
      }));
    } catch (error) {
      console.error('❌ RoleList: Failed to load users for role:', roleId, error);
      setUserPopover(prev => ({
        ...prev,
        users: [],
        loading: false
      }));
    }
  };

  // ✅ SIMPLE: Close popup
  const handleUserPopoverClose = () => {
    console.log('🔍 RoleList: Closing popover');
    setUserPopover({
      anchorEl: null,
      roleId: null,
      users: [],
      loading: false
    });
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>Loading roles...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box p={2}>
        <Typography color="error" variant="h6">
          Failed to fetch roles
        </Typography>
        <Typography color="error" variant="body2" sx={{ mt: 1 }}>
          {error}
        </Typography>
        <Typography variant="body2" sx={{ mt: 2 }}>
          Please check the console for more details.
        </Typography>
      </Box>
    );
  }

  const isPopoverOpen = Boolean(userPopover.anchorEl);

  return (
    <Box>
      <Box mb={2}>
        <TextField
          fullWidth
          placeholder="Search roles..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          InputProps={{
            startAdornment: (
              <InputAdornment position="start">
                <Search />
              </InputAdornment>
            )
          }}
        />
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Description</TableCell>
              <TableCell>Type</TableCell>
              <TableCell>Permissions</TableCell>
              <TableCell>Users</TableCell>
              <TableCell>Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {roles.length === 0 ? (
              <TableRow>
                <TableCell colSpan={6} align="center">
                  <Typography color="text.secondary" sx={{ py: 4 }}>
                    No roles found
                  </Typography>
                </TableCell>
              </TableRow>
            ) : (
              roles.map((role) => (
                <TableRow key={role.id} hover>
                  <TableCell>{role.name}</TableCell>
                  <TableCell>{role.description || 'No description'}</TableCell>
                  <TableCell>
                    <Chip
                      label={role.isSystemRole ? 'System' : 'Custom'}
                      color={role.isSystemRole ? 'primary' : 'default'}
                      size="small"
                    />
                  </TableCell>
                  <TableCell>{role.permissions?.length || 0}</TableCell>
                  <TableCell>
                    {/* ✅ SIMPLIFIED: Just hover to show, click outside to hide */}
                    <Box
                      onMouseEnter={(role.userCount || 0) > 0 ? 
                        (e) => handleUserCountHover(e, role.id, role.userCount || 0) : 
                        undefined
                      }
                      sx={{ display: 'inline-block' }}
                    >
                      <Chip
                        label={role.userCount || 0}
                        size="small"
                        color={(role.userCount || 0) > 0 ? 'primary' : 'default'}
                        variant={(role.userCount || 0) > 0 ? 'filled' : 'outlined'}
                        sx={{
                          cursor: (role.userCount || 0) > 0 ? 'pointer' : 'default',
                          '&:hover': (role.userCount || 0) > 0 ? {
                            transform: 'scale(1.05)',
                            transition: 'transform 0.2s ease-in-out'
                          } : undefined
                        }}
                      />
                    </Box>
                  </TableCell>
                  <TableCell>
                    <Tooltip title={role.isSystemRole ? "Cannot edit system roles" : "Edit role"}>
                      <span>
                        <IconButton
                          onClick={() => onEditRole?.(role.id)}
                          disabled={role.isSystemRole}
                          aria-label={`Edit ${role.name} role`}
                          data-testid="edit-button"
                        >
                          <Edit />
                        </IconButton>
                      </span>
                    </Tooltip>
                    
                    <Tooltip title={getDeleteTooltipText(role)}>
                      <span>
                        <IconButton
                          onClick={() => onDeleteRole?.(role.id)}
                          disabled={!canDeleteRole(role)}
                          aria-label={`Delete ${role.name} role`}
                          data-testid="delete-button"
                          color={!canDeleteRole(role) ? 'default' : 'error'}
                        >
                          <Delete />
                        </IconButton>
                      </span>
                    </Tooltip>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>

      <TablePagination
        component="div"
        count={pagination?.totalCount || 0}
        page={Math.max(0, (pagination?.pageNumber || 1) - 1)}
        onPageChange={handlePageChange}
        rowsPerPage={pagination?.pageSize || 10}
        onRowsPerPageChange={handlePageSizeChange}
        rowsPerPageOptions={[5, 10, 25, 50]}
        showFirstButton
        showLastButton
      />

      {/* ✅ ENHANCED: Popover with close button in header */}
      <Popover
        open={isPopoverOpen}
        anchorEl={userPopover.anchorEl}
        onClose={handleUserPopoverClose}
        anchorOrigin={{
          vertical: 'bottom',
          horizontal: 'center',
        }}
        transformOrigin={{
          vertical: 'top',
          horizontal: 'center',
        }}
        slotProps={{
          paper: {
            sx: {
              maxWidth: 320,
              maxHeight: 400,
              overflow: 'auto',
              mt: 1
            }
          }
        }}
        disableRestoreFocus
        disableAutoFocus
        disableEnforceFocus
      >
        <Box sx={{ p: 2 }}>
          {/* ✅ NEW: Header with close button */}
          <Box sx={{ 
            display: 'flex', 
            alignItems: 'center', 
            justifyContent: 'space-between',
            mb: 1 
          }}>
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              <PeopleIcon sx={{ mr: 1, color: 'primary.main' }} />
              <Typography variant="h6" component="div">
                Assigned Users
              </Typography>
            </Box>
            {/* ✅ NEW: Close button */}
            <Tooltip title="Close">
              <IconButton
                onClick={handleUserPopoverClose}
                size="small"
                sx={{ 
                  ml: 1,
                  color: 'text.secondary',
                  '&:hover': {
                    color: 'text.primary',
                    backgroundColor: 'action.hover'
                  }
                }}
                aria-label="Close user list"
              >
                <CloseIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Box>
          
          {userPopover.loading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
              <CircularProgress size={24} />
              <Typography sx={{ ml: 1 }}>Loading users...</Typography>
            </Box>
          ) : userPopover.users.length === 0 ? (
            <Typography color="text.secondary" sx={{ p: 2, textAlign: 'center' }}>
              No users found
            </Typography>
          ) : (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                {userPopover.users.length} user{userPopover.users.length === 1 ? '' : 's'} assigned to this role
              </Typography>
              <Divider sx={{ mb: 1 }} />
              <List dense sx={{ maxHeight: 280, overflow: 'auto' }}>
                {userPopover.users.map((user, index) => (
                  <ListItem 
                    key={user.id} 
                    divider={index < userPopover.users.length - 1}
                    sx={{ px: 0 }}
                  >
                    <Avatar sx={{ mr: 2, bgcolor: 'primary.main', width: 32, height: 32 }}>
                      {getInitials(user.firstName, user.lastName)}
                    </Avatar>
                    <ListItemText
                      primary={
                        <Typography variant="body2" fontWeight="medium">
                          {user.fullName}
                        </Typography>
                      }
                      secondary={
                        <Box>
                          <Typography variant="caption" color="text.secondary">
                            {user.email}
                          </Typography>
                          <Chip
                            label={user.isActive ? 'Active' : 'Inactive'}
                            size="small"
                            color={user.isActive ? 'success' : 'warning'}
                            sx={{ ml: 1, height: 16, fontSize: '0.65rem' }}
                          />
                        </Box>
                      }
                    />
                  </ListItem>
                ))}
              </List>
              
              {userPopover.users.length > 5 && (
                <Box sx={{ mt: 1, p: 1, bgcolor: 'background.default', borderRadius: 1 }}>
                  <Typography variant="caption" color="text.secondary">
                    💡 Tip: Click "Edit Role" to manage user assignments
                  </Typography>
                </Box>
              )}
            </>
          )}
        </Box>
      </Popover>
    </Box>
  );
};
