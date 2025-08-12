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
  Tooltip
} from '@mui/material';
import { Search, Edit, Delete } from '@mui/icons-material';
import type { RoleDto } from '../../types';
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

  // Fix: Initialize with default values
  const [pagination, setPagination] = useState({
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 0
  });

  const fetchRoles = async (page = 1, pageSize = 10, search = '') => {
    try {
      setLoading(true);
      setError(null);

      const result = await roleService.getRoles({
        page,
        pageSize,
        searchTerm: search || undefined
      });

      // Fix: Handle response structure safely
      if (result) {
        setRoles(result.items || []);
        setPagination({
          totalCount: result.totalCount || 0,
          pageNumber: result.pageNumber || page,
          pageSize: result.pageSize || pageSize,
          totalPages: result.totalPages || 0
        });
      }
    } catch (err) {
      console.error('Failed to fetch roles:', err);
      setError(err instanceof Error ? err.message : 'Failed to fetch roles');
      setRoles([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoles(1, 10, '');
  }, []);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      fetchRoles(1, pagination?.pageSize || 10, searchTerm);
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [searchTerm, pagination?.pageSize]);

  const handlePageChange = (_event: unknown, newPage: number) => {
    const pageNumber = newPage + 1;
    fetchRoles(pageNumber, pagination?.pageSize || 10, searchTerm);
  };

  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newPageSize = parseInt(event.target.value, 10);
    fetchRoles(1, newPageSize, searchTerm);
  };

  if (loading) {
    return (
      <Box display="flex" justifyContent="center" p={4}>
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box p={2}>
        <Typography color="error">Failed to fetch roles</Typography>
      </Box>
    );
  }

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
            {roles.map((role) => (
              <TableRow key={role.id}>
                <TableCell>{role.name}</TableCell>
                <TableCell>{role.description}</TableCell>
                <TableCell>
                  <Chip
                    label={role.isSystemRole ? 'System' : 'Custom'}
                    color={role.isSystemRole ? 'primary' : 'default'}
                    size="small"
                  />
                </TableCell>
                <TableCell>{role.permissions?.length || 0}</TableCell>
                <TableCell>{role.userCount || 0}</TableCell>
                <TableCell>
                  {/* Fix: Add proper accessibility labels and tooltip wrappers */}
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
                  <Tooltip title={role.isSystemRole ? "Cannot delete system roles" : "Delete role"}>
                    <span>
                      <IconButton
                        onClick={() => onDeleteRole?.(role.id)}
                        disabled={role.isSystemRole}
                        aria-label={`Delete ${role.name} role`}
                        data-testid="delete-button"
                      >
                        <Delete />
                      </IconButton>
                    </span>
                  </Tooltip>
                </TableCell>
              </TableRow>
            ))}
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
    </Box>
  );
};
