import { useState, useEffect } from 'react';
import {
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
  useTheme,
  useMediaQuery,
  Paper,
  Divider,
  Avatar,
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
  Visibility as ViewIcon,
} from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { LoadingSpinner } from '@/components/common/LoadingStates.js';
import { roleService } from '@/services/role.service.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import type { Role } from '@/types/index.js';
import toast from 'react-hot-toast';

// Mobile card component with Tailwind classes
function RoleCard({ 
  role, 
  onMenuClick, 
  onViewClick 
}: { 
  role: Role; 
  onMenuClick: (event: React.MouseEvent<HTMLElement>, role: Role) => void;
  onViewClick: (role: Role) => void;
}) {
  return (
    <Card 
      className="mb-4 cursor-pointer transition-all duration-200 hover:shadow-lg hover:-translate-y-1"
      onClick={() => onViewClick(role)}
    >
      <CardContent className="p-4">
        {/* Header Row */}
        <div className="flex justify-between items-start mb-3">
          <div className="flex items-center flex-1">
            <Avatar className="mr-3 w-10 h-10 bg-blue-600">
              <SecurityIcon />
            </Avatar>
            <div className="flex-1">
              <Typography variant="h6" className="font-medium">
                {role.name}
              </Typography>
              <div className="flex flex-wrap gap-1 mt-1">
                {role.isDefault && (
                  <Chip label="Default" size="small" color="info" variant="outlined" />
                )}
                {role.isSystemRole ? (
                  <Chip label="System Role" color="warning" size="small" icon={<LockIcon />} />
                ) : (
                  <Chip label="Custom Role" color="primary" size="small" variant="outlined" />
                )}
              </div>
            </div>
          </div>
          
          <CanAccess permissions={[PERMISSIONS.ROLES_VIEW, PERMISSIONS.ROLES_EDIT, PERMISSIONS.ROLES_DELETE]}>
            <IconButton
              onClick={(e) => {
                e.stopPropagation();
                onMenuClick(e, role);
              }}
              size="small"
              className="ml-2"
            >
              <MoreVertIcon />
            </IconButton>
          </CanAccess>
        </div>

        {/* Description */}
        <Typography 
          variant="body2" 
          color="text.secondary" 
          className="mb-3 min-h-5 line-clamp-2"
        >
          {role.description || 'No description provided'}
        </Typography>

        <Divider className="my-3" />

        {/* Stats Row */}
        <div className="flex justify-between items-center flex-wrap gap-2">
          <div className="flex items-center">
            <Badge badgeContent={role.permissions.length} color="secondary" max={99}>
              <SecurityIcon color="action" />
            </Badge>
            <Typography variant="caption" className="ml-2">
              {role.permissions.length} permission{role.permissions.length !== 1 ? 's' : ''}
            </Typography>
          </div>
          
          <Typography variant="caption" color="text.secondary">
            Created: {new Date(role.createdAt).toLocaleDateString()}
          </Typography>
        </div>

        {/* Quick Actions for Mobile */}
        <div className="flex gap-2 justify-end mt-3">
          <CanAccess permission={PERMISSIONS.ROLES_VIEW}>
            <Button
              size="small"
              startIcon={<ViewIcon />}
              onClick={(e) => {
                e.stopPropagation();
                onViewClick(role);
              }}
            >
              View
            </Button>
          </CanAccess>
          
          <CanAccess permission={PERMISSIONS.ROLES_EDIT}>
            <Button
              size="small"
              startIcon={<EditIcon />}
              disabled={role.isSystemRole}
              onClick={(e) => {
                e.stopPropagation();
                window.location.href = `/roles/${role.id}/edit`;
              }}
            >
              Edit
            </Button>
          </CanAccess>
        </div>
      </CardContent>
    </Card>
  );
}

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
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const isSmallScreen = useMediaQuery(theme.breakpoints.down('sm'));

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
      navigate(`/roles/${selectedRole.id}/edit`);
    }
    handleMenuClose();
  };

  const handleViewRole = (role?: Role) => {
    const targetRole = role || selectedRole;
    if (targetRole) {
      navigate(`/roles/${targetRole.id}`);
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

  if (loading) {
    return (
      <div className="p-6">
        <LoadingSpinner message="Loading roles..." fullHeight />
      </div>
    );
  }

  return (
    <div className="p-2 sm:p-4 md:p-6">
      {/* Header */}
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4 mb-6">
        <Typography variant={isSmallScreen ? "h5" : "h4"} component="h1">
          Role Management
        </Typography>
        
        <CanAccess permission={PERMISSIONS.ROLES_CREATE}>
          <Button
            variant="contained"
            startIcon={<AddIcon />}
            onClick={() => navigate('/roles/new')}
            className={isSmallScreen ? 'w-full' : ''}
          >
            Create Role
          </Button>
        </CanAccess>
      </div>

      {/* Search */}
      <Card className="mb-6">
        <CardContent className="p-4">
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
        </CardContent>
      </Card>

      {/* Content - Mobile vs Desktop */}
      {isMobile ? (
        // Mobile Card View
        <div>
          {paginatedRoles.length === 0 ? (
            <Paper className="p-8 text-center">
              <SecurityIcon className="text-5xl text-gray-400 mb-4" />
              <Typography variant="h6" color="text.secondary" gutterBottom>
                {searchTerm ? 'No roles found' : 'No roles available'}
              </Typography>
              <Typography variant="body2" color="text.secondary">
                {searchTerm 
                  ? `No roles match "${searchTerm}"`
                  : 'Create your first role to get started'
                }
              </Typography>
            </Paper>
          ) : (
            paginatedRoles.map((role) => (
              <RoleCard
                key={role.id}
                role={role}
                onMenuClick={handleMenuClick}
                onViewClick={handleViewRole}
              />
            ))
          )}
        </div>
      ) : (
        // Desktop Table View
        <Card>
          <CardContent>
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
                  {paginatedRoles.length === 0 ? (
                    <TableRow>
                      <TableCell colSpan={6} align="center" className="py-8">
                        <SecurityIcon className="text-5xl text-gray-400 mb-4" />
                        <Typography variant="h6" color="text.secondary" gutterBottom>
                          {searchTerm ? 'No roles found' : 'No roles available'}
                        </Typography>
                        <Typography variant="body2" color="text.secondary">
                          {searchTerm 
                            ? `No roles match "${searchTerm}"`
                            : 'Create your first role to get started'
                          }
                        </Typography>
                      </TableCell>
                    </TableRow>
                  ) : (
                    paginatedRoles.map((role) => (
                      <TableRow 
                        key={role.id} 
                        hover 
                        className="cursor-pointer hover:bg-gray-50"
                        onClick={() => handleViewRole(role)}
                      >
                        <TableCell>
                          <div className="flex items-center">
                            <SecurityIcon className="mr-2 text-blue-600" />
                            <div>
                              <Typography variant="subtitle2" className="font-medium">
                                {role.name}
                              </Typography>
                              {role.isDefault && (
                                <Chip 
                                  label="Default" 
                                  size="small" 
                                  color="info" 
                                  variant="outlined"
                                  className="mt-1"
                                />
                              )}
                            </div>
                          </div>
                        </TableCell>
                        
                        <TableCell>
                          <Typography 
                            variant="body2" 
                            color="text.secondary"
                            className="max-w-48 overflow-hidden text-ellipsis whitespace-nowrap"
                          >
                            {role.description || 'No description'}
                          </Typography>
                        </TableCell>
                        
                        <TableCell>
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
                        </TableCell>
                        
                        <TableCell>
                          <Badge badgeContent={role.permissions.length} color="secondary" max={99}>
                            <SecurityIcon color="action" />
                          </Badge>
                          <Typography variant="caption" className="ml-2">
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
                              onClick={(e) => {
                                e.stopPropagation();
                                handleMenuClick(e, role);
                              }}
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
          </CardContent>
        </Card>
      )}

      {/* Pagination */}
      <div className="flex justify-center mt-4">
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
          labelRowsPerPage={isSmallScreen ? "Rows:" : "Rows per page:"}
          className="text-sm sm:text-base"
        />
      </div>

      {/* Actions Menu */}
      <Menu
        anchorEl={anchorEl}
        open={Boolean(anchorEl)}
        onClose={handleMenuClose}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
      >
        <CanAccess permission={PERMISSIONS.ROLES_VIEW}>
          <MenuItem onClick={() => handleViewRole()}>
            <SecurityIcon className="mr-2" fontSize="small" />
            View Details
          </MenuItem>
        </CanAccess>
        
        <CanAccess permission={PERMISSIONS.ROLES_EDIT}>
          <MenuItem 
            onClick={handleEditRole}
            disabled={selectedRole?.isSystemRole}
          >
            <EditIcon className="mr-2" fontSize="small" />
            Edit Role
          </MenuItem>
        </CanAccess>
        
        <CanAccess permission={PERMISSIONS.ROLES_EDIT}>
          <MenuItem 
            onClick={handleManagePermissions}
            disabled={selectedRole?.isSystemRole}
          >
            <PeopleIcon className="mr-2" fontSize="small" />
            Manage Permissions
          </MenuItem>
        </CanAccess>
        
        <CanAccess permission={PERMISSIONS.ROLES_DELETE}>
          <MenuItem 
            onClick={handleDeleteClick}
            disabled={selectedRole?.isSystemRole}
            className="text-red-600"
          >
            <DeleteIcon className="mr-2" fontSize="small" />
            Delete Role
          </MenuItem>
        </CanAccess>
      </Menu>

      {/* Delete Confirmation Dialog */}
      <Dialog 
        open={deleteDialogOpen} 
        onClose={() => setDeleteDialogOpen(false)}
        maxWidth="sm"
        fullWidth
        fullScreen={isSmallScreen}
      >
        <DialogTitle>
          <Typography variant="h6">
            Confirm Delete
          </Typography>
        </DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the role <strong>"{roleToDelete?.name}"</strong>?
          </Typography>
          <Typography variant="body2" color="text.secondary" className="mt-2">
            This action cannot be undone and will remove this role from all users who currently have it assigned.
          </Typography>
        </DialogContent>
        <DialogActions className="p-4 gap-2">
          <Button 
            onClick={() => setDeleteDialogOpen(false)}
            className={isSmallScreen ? 'w-full' : ''}
          >
            Cancel
          </Button>
          <Button 
            onClick={handleDeleteConfirm} 
            color="error" 
            variant="contained"
            className={isSmallScreen ? 'w-full' : ''}
          >
            Delete Role
          </Button>
        </DialogActions>
      </Dialog>
    </div>
  );
}

export default RoleList;
