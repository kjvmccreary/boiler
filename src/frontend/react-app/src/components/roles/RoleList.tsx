import React, { useState, useEffect, useRef } from 'react';
import {
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow,
  Paper, TablePagination, TextField, InputAdornment, IconButton, Chip,
  Box, Typography, CircularProgress, Tooltip, Popover, List, ListItem,
  ListItemText, Avatar, Divider
} from '@mui/material';
import {
  Search, Edit, Delete, People as PeopleIcon, Close as CloseIcon
} from '@mui/icons-material';
import type { RoleDto, UserInfo } from '@/types';
import { roleService } from '@/services/role.service';
import { useTenant } from '@/contexts/TenantContext.js';

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
  const fetchInFlightRef = useRef(false); // prevent overlapping fetches (StrictMode/test double calls)

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

  const { currentTenant } = useTenant();

  const fetchRoles = async (page = 1, pageSize = 10, search = '') => {
    if (fetchInFlightRef.current) return;
    fetchInFlightRef.current = true;
    try {
      setLoading(true);
      setError(null);
      const result = await roleService.getRoles({
        page,
        pageSize,
        searchTerm: search || undefined
      });
      setRoles(result.roles || []);
      setPagination(result.pagination || {
        totalCount: 0,
        pageNumber: page,
        pageSize: pageSize,
        totalPages: 0
      });
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch roles');
      setRoles([]);
    } finally {
      setLoading(false);
      fetchInFlightRef.current = false;
    }
  };

  useEffect(() => {
    if (currentTenant) {
      fetchRoles(1, 10, '');
    }
  }, [currentTenant]);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      if (currentTenant) {
        fetchRoles(1, pagination?.pageSize || 10, searchTerm);
      }
    }, 500);
    return () => clearTimeout(timeoutId);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [searchTerm, currentTenant]);

  const handlePageChange = (_event: unknown, newPage: number) => {
    const pageNumber = newPage + 1;
    fetchRoles(pageNumber, pagination?.pageSize || 10, searchTerm);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newPageSize = parseInt(event.target.value, 10);
    fetchRoles(1, newPageSize, searchTerm);
  };

  const canDeleteRole = (role: RoleDto): boolean =>
    !role.isSystemRole && (role.userCount || 0) === 0;

  const getDeleteTooltipText = (role: RoleDto): string => {
    if (role.isSystemRole) return 'Cannot delete system roles';
    if ((role.userCount || 0) > 0) return `Cannot delete role with ${role.userCount} assigned user${role.userCount === 1 ? '' : 's'}`;
    return 'Delete role';
  };

  const handleUserCountHover = async (event: React.MouseEvent<HTMLElement>, roleId: number, userCount: number) => {
    if (userCount === 0) return;
    setUserPopover({
      anchorEl: event.currentTarget,
      roleId,
      users: [],
      loading: true
    });
    try {
      const users = await roleService.getRoleUsers(roleId);
      setUserPopover(prev => ({
        ...prev,
        users,
        loading: false
      }));
    } catch {
      setUserPopover(prev => ({
        ...prev,
        users: [],
        loading: false
      }));
    }
  };

  const handleUserPopoverClose = () => {
    setUserPopover({
      anchorEl: null,
      roleId: null,
      users: [],
      loading: false
    });
  };

  const getInitials = (firstName: string, lastName: string) =>
    `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();

  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view roles</Typography>
      </Box>
    );
  }

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
        <Typography sx={{ ml: 2 }}>Loading roles for {currentTenant.name}...</Typography>
      </Box>
    );
  }

  if (error) {
    return (
      <Box p={2}>
        <Typography color="error" variant="h6">
          Failed to fetch roles for {currentTenant.name}
        </Typography>
        <Typography color="error" variant="body2" sx={{ mt: 1 }}>
          {error}
        </Typography>
      </Box>
    );
  }

  const isPopoverOpen = Boolean(userPopover.anchorEl);

  return (
    <Box data-testid="role-list-root">
      <Box sx={{ mb: 2, display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="h6" component="div">
          Role Management
          <Typography variant="subtitle2" component="span" color="text.secondary" sx={{ mt: 0.5, ml: 1 }}>
            {currentTenant.name}
          </Typography>
        </Typography>
      </Box>

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
                    No roles found in {currentTenant.name}
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
                    <Box
                      onMouseEnter={(role.userCount || 0) > 0
                        ? (e) => handleUserCountHover(e, role.id, role.userCount || 0)
                        : undefined
                      }
                      sx={{ display: 'inline-block' }}
                    >
                      <Chip
                        label={role.userCount || 0}
                        size="small"
                        color={(role.userCount || 0) > 0 ? 'primary' : 'default'}
                        variant={(role.userCount || 0) > 0 ? 'filled' : 'outlined'}
                      />
                    </Box>
                  </TableCell>
                  <TableCell>
                    <Tooltip title={role.isSystemRole ? 'Cannot edit system roles' : 'Edit role'}>
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

                    <Tooltip title={canDeleteRole(role) ? 'Delete role' : getDeleteTooltipText(role)}>
                      <span>
                        <IconButton
                          onClick={() => onDeleteRole?.(role.id)}
                          disabled={!canDeleteRole(role)}
                          aria-label={`Delete ${role.name} role`}
                          data-testid="delete-button"
                          color={canDeleteRole(role) ? 'error' : 'default'}
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

      <Popover
        open={isPopoverOpen}
        anchorEl={userPopover.anchorEl}
        onClose={handleUserPopoverClose}
        anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}
        transformOrigin={{ vertical: 'top', horizontal: 'center' }}
        disableRestoreFocus
        disableAutoFocus
        disableEnforceFocus
      >
        <Box sx={{ p: 2 }}>
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
            <Tooltip title="Close">
              <IconButton
                onClick={handleUserPopoverClose}
                size="small"
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
                {userPopover.users.length} user{userPopover.users.length === 1 ? '' : 's'} assigned
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
                        <Typography variant="caption" color="text.secondary" component="div">
                          {user.email}
                          <Chip
                            label={user.isActive ? 'Active' : 'Inactive'}
                            size="small"
                            color={user.isActive ? 'success' : 'warning'}
                            sx={{ ml: 1, height: 16, fontSize: '0.65rem' }}
                          />
                        </Typography>
                      }
                    />
                  </ListItem>
                ))}
              </List>
            </>
          )}
        </Box>
      </Popover>
    </Box>
  );
};
