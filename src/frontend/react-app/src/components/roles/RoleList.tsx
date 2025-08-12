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
  CircularProgress
} from '@mui/material';
import { Search, Edit, Delete } from '@mui/icons-material';
import type { RoleDto } from '../../types'; // Add type keyword
import { roleService } from '../../services/role.service'; // Use instance

interface RoleListProps {
  onEditRole?: (role: RoleDto) => void;
  onDeleteRole?: (roleId: number) => void;
}

export const RoleList: React.FC<RoleListProps> = ({
  onEditRole,
  onDeleteRole
}) => {
  // State to handle pagination
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [searchTerm, setSearchTerm] = useState('');
  
  // Pagination state
  const [pagination, setPagination] = useState({
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 0
  });

  // Fetch roles with pagination
  const fetchRoles = async (page = 1, pageSize = 10, search = '') => {
    try {
      setLoading(true);
      setError(null);

      // Use roleService instance instead of RoleService class
      const result = await roleService.getRoles({
        page,
        pageSize,
        searchTerm: search || undefined
      });

      setRoles(result.roles);
      setPagination(result.pagination);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch roles');
    } finally {
      setLoading(false);
    }
  };

  // Initial load
  useEffect(() => {
    fetchRoles(pagination.pageNumber, pagination.pageSize, searchTerm);
  }, []);

  // Search handler with debounce
  useEffect(() => {
    const timeoutId = setTimeout(() => {
      fetchRoles(1, pagination.pageSize, searchTerm); // Reset to page 1 on search
    }, 500);

    return () => clearTimeout(timeoutId);
  }, [searchTerm, pagination.pageSize]);

  // Page change handler - prefix unused parameter with underscore
  const handlePageChange = (_event: unknown, newPage: number) => {
    const pageNumber = newPage + 1; // MUI uses 0-based indexing
    fetchRoles(pageNumber, pagination.pageSize, searchTerm);
  };

  // Page size change handler
  const handlePageSizeChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const newPageSize = parseInt(event.target.value, 10);
    fetchRoles(1, newPageSize, searchTerm); // Reset to page 1
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
        <Typography color="error">{error}</Typography>
      </Box>
    );
  }

  return (
    <Box>
      {/* Search field */}
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

      {/* Table */}
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
                <TableCell>{role.permissions.length}</TableCell>
                <TableCell>{role.userCount || 0}</TableCell>
                <TableCell>
                  <IconButton 
                    onClick={() => onEditRole?.(role)}
                    disabled={role.isSystemRole}
                  >
                    <Edit />
                  </IconButton>
                  <IconButton 
                    onClick={() => onDeleteRole?.(role.id)}
                    disabled={role.isSystemRole}
                  >
                    <Delete />
                  </IconButton>
                </TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </TableContainer>

      {/* Pagination component */}
      <TablePagination
        component="div"
        count={pagination.totalCount}
        page={pagination.pageNumber - 1} // MUI uses 0-based indexing
        onPageChange={handlePageChange}
        rowsPerPage={pagination.pageSize}
        onRowsPerPageChange={handlePageSizeChange}
        rowsPerPageOptions={[5, 10, 25, 50]}
        showFirstButton
        showLastButton
      />
    </Box>
  );
};
