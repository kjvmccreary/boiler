import React, { useEffect } from 'react';
import {
  Box,
  Typography,
  Button,
} from '@mui/material';
import { Add } from '@mui/icons-material';
import { RoleList } from '../components/roles/RoleList';
import { useNavigate } from 'react-router-dom';
import { useRoles } from '../hooks/useRoles';
import type { RoleDto } from '../types';

export const RoleManagement: React.FC = () => {
  const navigate = useNavigate();
  const { fetchRoles } = useRoles();

  useEffect(() => {
    fetchRoles();
  }, [fetchRoles]);

  const handleCreateRole = () => {
    navigate('/roles/new');
  };

  const handleEditRole = (role: RoleDto) => {
    navigate(`/roles/${role.id}/edit`);
  };

  const handleDeleteRole = (roleId: number) => {
    // Implement delete logic
    console.log('Delete role:', roleId);
  };

  return (
    <Box p={3}>
      <Box display="flex" justifyContent="space-between" alignItems="center" mb={3}>
        <Typography variant="h4">Role Management</Typography>
        <Button
          variant="contained"
          startIcon={<Add />}
          onClick={handleCreateRole}
        >
          Create Role
        </Button>
      </Box>

      <RoleList
        onEditRole={handleEditRole}
        onDeleteRole={handleDeleteRole}
      />
    </Box>
  );
};
