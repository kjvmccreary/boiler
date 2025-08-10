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
  Button,
  Menu,
  MenuItem,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  Badge,
} from '@mui/material';
import {
  Search as SearchIcon,
  MoreVert as MoreVertIcon,
  Edit as EditIcon,
  Delete as DeleteIcon,
  Add as AddIcon,
  Security as SecurityIcon,
  People as PeopleIcon,
  Lock as LockIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { roleService } from '@/services/role.service.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { Role } from '@/types/index.js';
import toast from 'react-hot-toast';

export function RoleList() {
  const [roles, setRoles] = useState<Role[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(10);
  const [selectedRole, setSelectedRole] = useState<Role | null>(null);
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [roleToDelete, setRoleToDelete] = useState<Role | null>(null);

  const navigate = useNavigate();

  useEffect(() => {
    loadRoles();
  }, [searchTerm]);

  const loadRoles = async () => {
    try {
      setLoading(true);
      const response = await roleService.getRoles({
        searchTerm: searchTerm || undefined,
      });
      
      setRoles(response);
    } catch (error) {
      console.error('Failed to load roles:', error);
      toast.error('Failed to load roles');
    } finally {
      setLoading(false);
    }
  };

  const handleSearchChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    setSearchTerm(event.target.value);
    setPage(0);
  };

  const handleMenuClick = (event: React.MouseEvent<HTMLElement>, role: Role) => {
    setAnchorEl(event.currentTarget);
    setSelectedRole(role);
  };

  const handleMenuClose = () => {
    setAnchorEl(null);
    setSelectedRole(null);
  };

  const handleEditRole = () => {
    if (selectedRole) {
      navigate(`/roles/${selectedRole.id}/edit`); // Added /edit
    }
    handleMenuClose();
  };

  const handleViewRole = () => {
    if (selectedRole) {
      navigate(`/roles/${selectedRole.id}`); // This will now go to RoleDetails
    }
    handleMenuClose();
  };

  const handleManagePermissions = () => {
    if (selectedRole) {
      navigate(`/roles/${selectedRole.id}/permissions`);
    }
    handleMenuClose();
  };

  const handleDeleteClick = () => {
    if (selectedRole?.isSystemRole) {
      toast.error('System roles cannot be deleted');
      handleMenuClose();
      return;
    }
    
    setRoleToDelete(selectedRole);
    setDeleteDialogOpen(true);
    handleMenuClose();
  };

  const handleDeleteConfirm = async () => {
    if (!roleToDelete) return;

    try {
      await roleService.deleteRole(roleToDelete.id);
      toast.success('Role deleted successfully');
      loadRoles();
    } catch (error) {
      console.error('Failed to delete role:', error);
      toast.error('Failed to delete role');
    } finally {
      setDeleteDialogOpen(false);
      setRoleToDelete(null);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  // Filter and paginate roles
  const filteredRoles = roles.filter(role => 
    role.name.toLowerCase().includes(searchTerm.toLowerCase()) ||
    role.description?.toLowerCase().includes(searchTerm.toLowerCase())
  );

  const paginatedRoles = filteredRoles.slice(
    page * rowsPerPage,
    page * rowsPerPage + rowsPerPage
  );

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 3 }}>
        <Typography variant="h4" component="h1">
          Role Management
        </Typography>
        
        <CanAccess permission={PERMISSIONS.ROLES_CREATE}>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/roles/new')} // Updated route
          >
            Create Role
          </Button>
        </CanAccess>
      </Box>

      <Card>
        <CardContent>
          <Box sx={{ mb: 2 }}>
            <TextField
              fullWidth
              placeholder="Search roles..."
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
                  <TableCell>Role Name</TableCell>
                  <TableCell>Description</TableCell>
                  <TableCell>Type</TableCell>
                  <TableCell>Permissions</TableCell>
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
                ) : paginatedRoles.length === 0 ? (
                  <TableRow>
                    <TableCell colSpan={6} align="center">
                      No roles found
                    </TableCell>
                  </TableRow>
                ) : (
                  paginatedRoles.map((role) => (
                    <TableRow key={role.id} hover>
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          <SecurityIcon sx={{ mr: 1, color: 'primary.main' }} />
                          <Box>
                            <Typography variant="subtitle2" fontWeight="medium">
                              {role.name}
                            </Typography>
                            {role.isDefault && (
                              <Chip 
                                label="Default" 
                                size="small" 
                                color="info" 
                                variant="outlined"
                                sx={{ mt: 0.5 }}
                              />
                            )}
                          </Box>
                        </Box>
                      </TableCell>
                      
                      <TableCell>
                        <Typography variant="body2" color="text.secondary">
                          {role.description || 'No description'}
                        </Typography>
                      </TableCell>
                      
                      <TableCell>
                        <Box sx={{ display: 'flex', alignItems: 'center' }}>
                          {role.isSystemRole ? (
                            <Chip 
                              label="System Role" 
                              color="warning" 
                              size="small"
                              icon={<LockIcon />}
                            />
                          ) : (
                            <Chip 
                              label="Custom Role" 
                              color="primary" 
                              size="small"
                              variant="outlined"
                            />
                          )}
                        </Box>
                      </TableCell>
                      
                      <TableCell>
                        <Badge badgeContent={role.permissions.length} color="secondary">
                          <SecurityIcon color="action" />
                        </Badge>
                        <Typography variant="caption" sx={{ ml: 1 }}>
                          {role.permissions.length} permissions
                        </Typography>
                      </TableCell>
                      
                      <TableCell>
                        <Typography variant="body2">
                          {formatDate(role.createdAt)}
                        </Typography>
                      </TableCell>
                      
                      <TableCell>
                        <CanAccess permissions={[PERMISSIONS.ROLES_VIEW, PERMISSIONS.ROLES_EDIT, PERMISSIONS.ROLES_DELETE]}>
                          <IconButton
                            onClick={(e) => handleMenuClick(e, role)}
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
            count={filteredRoles.length}
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
        <CanAccess permission={PERMISSIONS.ROLES_VIEW}>
          <MenuItem onClick={handleViewRole}>
            <SecurityIcon sx={{ mr: 1 }} fontSize="small" />
            View Details
          </MenuItem>
        </CanAccess>
        
        <CanAccess permission={PERMISSIONS.ROLES_EDIT}>
          <MenuItem 
            onClick={handleEditRole}
            disabled={selectedRole?.isSystemRole}
          >
            <EditIcon sx={{ mr: 1 }} fontSize="small" />
            Edit Role
          </MenuItem>
        </CanAccess>
        
        <CanAccess permission={PERMISSIONS.ROLES_EDIT}>
          <MenuItem 
            onClick={handleManagePermissions}
            disabled={selectedRole?.isSystemRole}
          >
            <PeopleIcon sx={{ mr: 1 }} fontSize="small" />
            Manage Permissions
          </MenuItem>
        </CanAccess>
        
        <CanAccess permission={PERMISSIONS.ROLES_DELETE}>
          <MenuItem 
            onClick={handleDeleteClick}
            disabled={selectedRole?.isSystemRole}
          >
            <DeleteIcon sx={{ mr: 1 }} fontSize="small" />
            Delete Role
          </MenuItem>
        </CanAccess>
      </Menu>

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the role "{roleToDelete?.name}"? 
            This action cannot be undone and will remove this role from all users.
          </Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleDeleteConfirm} color="error" variant="contained">
            Delete Role
          </Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default RoleList;
