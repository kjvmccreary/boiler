import { useState, useEffect } from 'react';
import {
  Box,
  Card,
  CardContent,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  TablePagination,
  TextField,
  InputAdornment,
  IconButton,
  Chip,
  Avatar,
  Button,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Popover,
  List,
  ListItem,
  ListItemText,
  Divider,
  CircularProgress,
  Tooltip,
} from '@mui/material';
import {
  Search as SearchIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  PersonAdd as PersonAddIcon,
  Security as SecurityIcon,
  Close as CloseIcon,
  LockOpen as LockOpenIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { userService } from '@/services/user.service.js';
import { roleService } from '@/services/role.service.js'; // ✅ ADD: Import role service
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { User } from '@/types/index.js';
import toast from 'react-hot-toast';
import { normalizeRoles } from '@/utils/role.utils.js';
import { ROUTES } from '@/routes/route.constants.js'; // Add this import at the top (after other imports)
import { useTenant } from '@/contexts/TenantContext.js'; // 🔧 ADD: Import useTenant

export function UserList() {
  const [users, setUsers] = useState<User[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [totalCount, setTotalCount] = useState(0);
  const [selectedUser, setSelectedUser] = useState<User | null>(null);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [userToDelete, setUserToDelete] = useState<User | null>(null);

  // ✅ NEW: State for permissions popover
  const [permissionsPopover, setPermissionsPopover] = useState<{
    anchorEl: HTMLElement | null;
    roleName: string | null;
    permissions: string[];
    loading: boolean;
  }>(
    {
      anchorEl: null,
      roleName: null,
      permissions: [],
      loading: false,
    }
  );

  const navigate = useNavigate();
  const { currentTenant } = useTenant(); // 🔧 ADD: Get current tenant

  // 🔧 FIX: Add currentTenant as dependency to reload users when tenant changes
  useEffect(() => {
    if (currentTenant) {
      loadUsers();
    }
  }, [page, rowsPerPage, searchTerm, currentTenant]); // 🔧 ADD: currentTenant dependency

  const loadUsers = async () => {
    try {
      setLoading(true);
      console.log('🔄 UserList: Loading users for tenant:', currentTenant?.name || 'Unknown');

      const response = await userService.getUsers({
        page: page + 1, // API uses 1-based pagination
        pageSize: rowsPerPage,
        searchTerm: searchTerm || undefined,
      });

      console.log('✅ UserList: Loaded', response.data.length, 'users for tenant:', currentTenant?.name);
      setUsers(response.data);
      setTotalCount(response.totalCount);
    } catch (error) {
      console.error('❌ UserList: Failed to load users:', error);
      toast.error('Failed to load users');
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
    setPage(0); // Reset to first page when searching
  };

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>, user: User) => {
    setAnchorEl(event.currentTarget);
    setSelectedUser(user);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedUser(null);
  };

  const handleEditUser = () => {
    if (selectedUser) {
      navigate(`${ROUTES.USERS}/${selectedUser.id}`); // 🔧 FIX: Use constant instead of '/users/${selectedUser.id}'
    }
    handleMenuClose();
  };

  const handleManageRoles = () => {
    if (selectedUser) {
      navigate(`${ROUTES.USERS}/${selectedUser.id}/roles`); // 🔧 FIX: Use constant instead of '/users/${selectedUser.id}/roles'
    }
    handleMenuClose();
  };

  const handleDeleteClick = () => {
    setUserToDelete(selectedUser);
    setDeleteDialogOpen(true);
    handleMenuClose();
  };

  const handleDeleteConfirm = async () => {
    if (!userToDelete) return;

    try {
      await userService.deleteUser(userToDelete.id);
      toast.success('User deleted successfully');
      loadUsers(); // Reload the list
    } catch (error) {
      console.error('Failed to delete user:', error);
      toast.error('Failed to delete user');
    } finally {
      setDeleteDialogOpen(false);
      setUserToDelete(null);
    }
  };

  // ✅ NEW: Handle role chip hover to show permissions
  const handleRoleHover = async (event: React.MouseEvent<HTMLElement>, roleName: string) => {
    console.log('🔍 UserList: Role hovered:', roleName);

    setPermissionsPopover({
      anchorEl: event.currentTarget,
      roleName,
      permissions: [],
      loading: true,
    });

    try {
      // First get the role by name to get its ID
      const role = await roleService.getRoleByName(roleName);
      if (!role) {
        throw new Error(`Role "${roleName}" not found`);
      }

      // Then get permissions for the role
      const permissions = await roleService.getRolePermissions(role.id);
      console.log('✅ UserList: Permissions loaded for role:', roleName, permissions);

      setPermissionsPopover((prev) => ({
        ...prev,
        permissions,
        loading: false,
      }));
    } catch (error) {
      console.error('❌ UserList: Failed to load permissions for role:', roleName, error);
      setPermissionsPopover((prev) => ({
        ...prev,
        permissions: [],
        loading: false,
      }));
      toast.error(`Could not load permissions for role "${roleName}"`);
    }
  };

  // ✅ NEW: Handle permissions popover close
  const handlePermissionsPopoverClose = () => {
    console.log('🔍 UserList: Closing permissions popover');
    setPermissionsPopover({
      anchorEl: null,
      roleName: null,
      permissions: [],
      loading: false,
    });
  };

  // ✅ NEW: Group permissions by category for better display
  const groupPermissionsByCategory = (permissions: string[]) => {
    return permissions.reduce((groups, permission) => {
      const parts = permission.split('.');
      const category = parts.length > 1 ? parts[0] : 'General';
      const categoryName = category.charAt(0).toUpperCase() + category.slice(1);

      if (!groups[categoryName]) {
        groups[categoryName] = [];
      }
      groups[categoryName].push(permission);
      return groups;
    }, {} as Record<string, string[]>);
  };

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  const isPermissionsPopoverOpen = Boolean(permissionsPopover.anchorEl);

  // 🔧 ADD: Don't render if no tenant is selected
  if (!currentTenant) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: 200 }}>
        <Typography>Please select a tenant to view users</Typography>
      </Box>
    );
  }

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          User Management
          {/* 🔧 ADD: Show current tenant name */}
          <Typography variant="subtitle2" color="text.secondary" sx={{ mt: 0.5 }}>
            {currentTenant.name}
          </Typography>
        </Typography>

        <CanAccess permission={PERMISSIONS.USERS_CREATE}>
          <Button
            variant="contained"
            startIcon={<PersonAddIcon />}
            onClick={() => navigate(ROUTES.USER_NEW)} // 🔧 FIX: Use constant instead of '/users/new'
          >
            Add User
          </Button>
        </CanAccess>
      </Box>

      <Card>
        <CardContent>
          <Box sx={{ mb: 2 }}>
            <TextField
              fullWidth
              placeholder="Search users..."
              value={searchTerm}
              onChange={handleSearchChange}
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

          <TableContainer>
            <Table>
              <TableHead>
                <TableRow>
                  <TableCell>User</TableCell>
                  <TableCell>Email</TableCell>
                  <TableCell>Roles</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Created</TableCell>
                  <TableCell>Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {loading ? (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <CircularProgress size={20} sx={{ mr: 1 }} />
                        Loading users for {currentTenant.name}...
                      </Box>
                    </TableCell>
                  </TableRow>
                ) : users.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      <Typography>No users found in {currentTenant.name}</Typography>
                    </TableCell>
                  </TableRow>
                ) : (
                  users.map((user) => (
                    <TableRow key={user.id} hover>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          <Avatar sx={{ mr: 2, bgcolor: 'primary.main' }}>
                            {getInitials(user.firstName, user.lastName)}
                          </Avatar>
                          <Box>
                            <Typography variant="subtitle2">
                              {user.firstName} {user.lastName}
                            </Typography>
                            <Typography variant="caption" color="text.secondary">
                              ID: {user.id}
                            </Typography>
                          </Box>
                        </Box>
                      </TableCell>

                      <TableCell>{user.email}</TableCell>

                      <TableCell>
                        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                          {/* ✅ ENHANCED: Role chips with hover for permissions */}
                          {normalizeRoles(user.roles).slice(0, 2).map((role: string, index: number) => (
                            <Chip
                              key={`${role}-${index}`}
                              label={role}
                              size="small"
                              color="primary"
                              variant="outlined"
                              onMouseEnter={(e) => handleRoleHover(e, role)}
                              sx={{
                                mr: 0.5,
                                mb: 0.5,
                                cursor: 'pointer',
                                '&:hover': {
                                  transform: 'scale(1.05)',
                                  transition: 'transform 0.2s ease-in-out',
                                  backgroundColor: 'primary.light',
                                  color: 'primary.contrastText',
                                },
                              }}
                            />
                          ))}
                          {normalizeRoles(user.roles).length > 2 && (
                            <Chip
                              label={`+${normalizeRoles(user.roles).length - 2} more`}
                              size="small"
                              color="default"
                              variant="outlined"
                            />
                          )}
                        </Box>
                      </TableCell>

                      <TableCell>
                        <Chip
                          label={user.emailConfirmed ? 'Active' : 'Pending'}
                          color={user.emailConfirmed ? 'success' : 'warning'}
                          size="small"
                        />
                      </TableCell>

                      <TableCell>{formatDate(user.createdAt)}</TableCell>

                      <TableCell>
                        <CanAccess permissions={[PERMISSIONS.USERS_EDIT, PERMISSIONS.USERS_DELETE]}>
                          <IconButton
                            onClick={(e) => handleMenuClick(e, user)}
                            size="small"
                          >
                            <MoreVertIcon />
                          </IconButton>
                        </CanAccess>
                      </TableCell>
                    </TableRow>
                  ))
                )}
              </TableBody>
            </Table>
          </TableContainer>

          <TablePagination
            component="div"
            count={totalCount}
            page={page}
            onPageChange={(_, newPage) => setPage(newPage)}
            rowsPerPage={rowsPerPage}
            onRowsPerPageChange={(e) => {
              setRowsPerPage(parseInt(e.target.value, 10));
              setPage(0);
            }}
            rowsPerPageOptions={[5, 10, 25, 50]}
          />
        </CardContent>
      </Card>

      {/* Actions Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
      >
        <CanAccess permission={PERMISSIONS.USERS_EDIT}>
          <MenuItem onClick={handleEditUser}>
            <EditIcon sx={{ mr: 1 }} fontSize="small" />
            Edit User
          </MenuItem>
        </CanAccess>

        <CanAccess permission={PERMISSIONS.USERS_MANAGE_ROLES}>
          <MenuItem onClick={handleManageRoles}>
            <SecurityIcon sx={{ mr: 1 }} fontSize="small" />
            Manage Roles
          </MenuItem>
        </CanAccess>

        <CanAccess permission={PERMISSIONS.USERS_DELETE}>
          <MenuItem onClick={handleDeleteClick}>
            <DeleteIcon sx={{ mr: 1 }} fontSize="small" />
            Delete User
          </MenuItem>
        </CanAccess>
      </Menu>

      {/* ✅ NEW: Permissions Popover */}
      <Popover
        open={isPermissionsPopoverOpen}
        anchorEl={permissionsPopover.anchorEl}
        onClose={handlePermissionsPopoverClose}
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
              maxWidth: 350,
              maxHeight: 400,
              overflow: 'auto',
              mt: 1,
            },
          },
        }}
        disableRestoreFocus
        disableAutoFocus
        disableEnforceFocus
      >
        <Box sx={{ p: 2 }}>
          {/* Header with close button */}
          <Box sx={{
              display: 'flex',
              alignItems: 'center',
              justifyContent: 'space-between',
              mb: 1,
            }}
          >
            <Box sx={{ display: 'flex', alignItems: 'center' }}>
              <LockOpenIcon sx={{ mr: 1, color: 'primary.main' }} />
              <Typography variant="h6" component="div">
                Role Permissions
              </Typography>
            </Box>
            <Tooltip title="Close">
              <IconButton
                onClick={handlePermissionsPopoverClose}
                size="small"
                sx={{
                  ml: 1,
                  color: 'text.secondary',
                  '&:hover': {
                    color: 'text.primary',
                    backgroundColor: 'action.hover',
                  },
                }}
                aria-label="Close permissions list"
              >
                <CloseIcon fontSize="small" />
              </IconButton>
            </Tooltip>
          </Box>

          {/* Role name */}
          <Typography variant="subtitle1" fontWeight="medium" sx={{ mb: 1 }}>
            {permissionsPopover.roleName}
          </Typography>

          {permissionsPopover.loading ? (
            <Box sx={{ display: 'flex', justifyContent: 'center', p: 2 }}>
              <CircularProgress size={24} />
              <Typography sx={{ ml: 1 }}>Loading permissions...</Typography>
            </Box>
          ) : permissionsPopover.permissions.length === 0 ? (
            <Typography color="text.secondary" sx={{ p: 2, textAlign: 'center' }}>
              No permissions found for this role
            </Typography>
          ) : (
            <>
              <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
                {permissionsPopover.permissions.length} permission{permissionsPopover.permissions.length === 1 ? '' : 's'} assigned
              </Typography>
              <Divider sx={{ mb: 1 }} />

              {/* Group permissions by category */}
              {Object.entries(groupPermissionsByCategory(permissionsPopover.permissions)).map(([category, permissions]) => (
                <Box key={category} sx={{ mb: 2 }}>
                  <Typography variant="subtitle2" fontWeight="medium" color="primary.main" sx={{ mb: 1 }}>
                    {category} ({permissions.length})
                  </Typography>
                  <List dense sx={{ pl: 1 }}>
                    {permissions.map((permission, index) => (
                      <ListItem
                        key={permission}
                        divider={index < permissions.length - 1}
                        sx={{ px: 0, py: 0.5 }}
                      >
                        <ListItemText
                          primary={
                            <Typography variant="body2" sx={{ fontFamily: 'monospace', fontSize: '0.8rem' }}>
                              {permission}
                            </Typography>
                          }
                        />
                      </ListItem>
                    ))}
                  </List>
                </Box>
              ))}

              <Box sx={{ mt: 1, p: 1, bgcolor: 'background.default', borderRadius: 1 }}>
                <Typography variant="caption" color="text.secondary">
                  💡 Tip: Click "Manage Roles" to modify user role assignments
                </Typography>
              </Box>
            </>
          )}
        </Box>
      </Popover>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete user "{userToDelete?.firstName} {userToDelete?.lastName}"?
            This action cannot be undone.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDeleteConfirm} color="error" variant="contained">
            Delete
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default UserList;
