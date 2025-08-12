import { useState, useCallback } from 'react';
import type { RoleDto, CreateRoleRequest, UpdateRoleRequest } from '../types';
import { roleService } from '../services/role.service';

export const useRoles = () => {
  const [roles, setRoles] = useState<RoleDto[]>([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [pagination, setPagination] = useState({
    totalCount: 0,
    pageNumber: 1,
    pageSize: 10,
    totalPages: 0
  });

  const fetchRoles = useCallback(async (params = {}) => {
    try {
      setLoading(true);
      setError(null);
      
      const result = await roleService.getRoles({
        page: 1,
        pageSize: 10,
        ...params
      });
      
      setRoles(result.roles);
      setPagination(result.pagination);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to fetch roles');
    } finally {
      setLoading(false);
    }
  }, []);

  const createRole = useCallback(async (roleData: CreateRoleRequest) => {
    try {
      await roleService.createRole(roleData);
      await fetchRoles();
    } catch (err) {
      throw err;
    }
  }, [fetchRoles]);

  const updateRole = useCallback(async (id: number, roleData: UpdateRoleRequest) => {
    try {
      await roleService.updateRole(id, roleData);
      await fetchRoles();
    } catch (err) {
      throw err;
    }
  }, [fetchRoles]);

  const deleteRole = useCallback(async (id: number) => {
    try {
      await roleService.deleteRole(id);
      await fetchRoles();
    } catch (err) {
      throw err;
    }
  }, [fetchRoles]);

  return {
    roles,
    pagination,
    loading,
    error,
    fetchRoles,
    createRole,
    updateRole,
    deleteRole
  };
};
