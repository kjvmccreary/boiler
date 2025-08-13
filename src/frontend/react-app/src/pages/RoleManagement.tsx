import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box,
  Typography,
  Button,
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
} from '@mui/material';
import {
  Add as AddIcon,
} from '@mui/icons-material';
import { RoleList } from '@/components/roles/RoleList.js';
import { CanAccess } from '@/components/authorization/CanAccess.js';
import { PERMISSIONS } from '@/utils/api.constants.js';
import { roleService } from '@/services/role.service.js';
import toast from 'react-hot-toast';

export function RoleManagement() {
  const navigate = useNavigate();
  const [deleteDialogOpen, setDeleteDialogOpen] = useState(false);
  const [roleToDelete, setRoleToDelete] = useState<{ id: number; name: string } | null>(null);
  const [refreshKey, setRefreshKey] = useState(0);

  const handleCreateRole = () => {
    console.log('🔍 RoleManagement: Create role clicked');
    navigate('/roles/new');
  };

  // ✅ FIX: Change function signature to match RoleList expectation
  const handleEditRole = (roleId: number) => {
    console.log('🔍 RoleManagement: Edit role clicked for ID:', roleId);
    navigate(`/roles/${roleId}/edit`);
  };

  const handleDeleteRole = (roleId: number) => {
    console.log('🔍 RoleManagement: Delete role clicked for ID:', roleId);
    // Find the role name (we'd need to pass this from RoleList or fetch it)
    setRoleToDelete({ id: roleId, name: `Role ${roleId}` });
    setDeleteDialogOpen(true);
  };

  const handleDeleteConfirm = async () => {
    if (!roleToDelete) return;

    try {
      console.log('🔍 RoleManagement: Deleting role:', roleToDelete);
      await roleService.deleteRole(roleToDelete.id);
      toast.success('Role deleted successfully');
      setRefreshKey(prev => prev + 1); // Force RoleList to refresh
    } catch (error) {
      console.error('Failed to delete role:', error);
      toast.error('Failed to delete role');
    } finally {
      setDeleteDialogOpen(false);
      setRoleToDelete(null);
    }
  };

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
            onClick={handleCreateRole}
          >
            Add Role
          </Button>
        </CanAccess>
      </Box>

      <RoleList 
        key={refreshKey} // Force re-render when roles are deleted
        onEditRole={handleEditRole}
        onDeleteRole={handleDeleteRole}
      />

      {/* Delete Confirmation Dialog */}
      <Dialog open={deleteDialogOpen} onClose={() => setDeleteDialogOpen(false)}>
        <DialogTitle>Confirm Delete</DialogTitle>
        <DialogContent>
          <Typography>
            Are you sure you want to delete the role "{roleToDelete?.name}"? 
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

export default RoleManagement;
