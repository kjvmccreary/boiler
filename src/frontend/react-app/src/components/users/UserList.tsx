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
} from '@mui/material';
import {
  Search as SearchIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  PersonAdd as PersonAddIcon,
  Security as SecurityIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { userService } from '@/services/user.service.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { User } from '@/types/index.js';
import toast from 'react-hot-toast';

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

  const navigate = useNavigate();

  useEffect(() => {
    loadUsers();
  }, [page, rowsPerPage, searchTerm]);

  const loadUsers = async () => {
    try {
      setLoading(true);
      const response = await userService.getUsers({
        page: page + 1, // API uses 1-based pagination
        pageSize: rowsPerPage,
        searchTerm: searchTerm || undefined,
      });
      
      setUsers(response.data);
      setTotalCount(response.totalCount);
    } catch (error) {
      console.error('Failed to load users:', error);
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
      navigate(`/users/${selectedUser.id}`);
    }
    handleMenuClose();
  };

  const handleManageRoles = () => {
    if (selectedUser) {
      navigate(`/users/${selectedUser.id}/roles`);
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

  const getInitials = (firstName: string, lastName: string) => {
    return `${firstName.charAt(0)}${lastName.charAt(0)}`.toUpperCase();
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          User Management
        </Typography>
        
        <CanAccess permission={PERMISSIONS.USERS_CREATE}>
          <Button
            variant="contained"
            startIcon={<PersonAddIcon />}
            onClick={() => navigate('/users/new')}
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
                      Loading...
                    </TableCell>
                  </TableRow>
                ) : users.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      No users found
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
                          {user.roles.slice(0, 2).map((role) => (
                            <Chip
                              key={role.id}
                              label={role.name}
                              size="small"
                              color="primary"
                              variant="outlined"
                            />
                          ))}
                          {user.roles.length > 2 && (
                            <Chip
                              label={`+${user.roles.length - 2} more`}
                              size="small"
                              variant="outlined"
                            />
                          )}
                        </Box>
                      </TableCell>
                      
                      <TableCell>
                        <Chip
                          label={user.isEmailConfirmed ? 'Active' : 'Pending'}
                          color={user.isEmailConfirmed ? 'success' : 'warning'}
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
