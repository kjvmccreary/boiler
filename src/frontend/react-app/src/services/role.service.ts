import { apiClient } from './api.client';
import type { 
  RolesListResponse, 
  RoleResponse, 
  RoleDto, 
  ApiResponseDto,
  PaginationParams,
  CreateRoleRequest,
  UpdateRoleRequest
} from '../types';

export class RoleService {
  async getRoles(params: PaginationParams = {}): Promise<{
    roles: RoleDto[];
    pagination: {
      totalCount: number;
      pageNumber: number;
      pageSize: number;
      totalPages: number;
    };
  }> {
    const {
      page = 1,
      pageSize = 10,
      searchTerm
    } = params;

    const queryParams = new URLSearchParams({
      page: page.toString(),
      pageSize: pageSize.toString(),
      ...(searchTerm && { searchTerm })
    });

    const response = await apiClient.get<RolesListResponse>(
      `/api/roles?${queryParams}`
    );

    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to fetch roles');
    }

    const pagedData = response.data.data;

    return {
      roles: pagedData.items,
      pagination: {
        totalCount: pagedData.totalCount,
        pageNumber: pagedData.pageNumber,
        pageSize: pagedData.pageSize,
        totalPages: pagedData.totalPages
      }
    };
  }

  async getRoleById(id: number): Promise<RoleDto> {
    const response = await apiClient.get<RoleResponse>(`/api/roles/${id}`);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to fetch role');
    }

    return response.data.data;
  }

  async createRole(roleData: CreateRoleRequest): Promise<RoleDto> {
    const response = await apiClient.post<RoleResponse>('/api/roles', roleData);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to create role');
    }

    return response.data.data;
  }

  async updateRole(id: number, roleData: UpdateRoleRequest): Promise<RoleDto> {
    const response = await apiClient.put<RoleResponse>(`/api/roles/${id}`, roleData);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to update role');
    }

    return response.data.data;
  }

  async deleteRole(id: number): Promise<void> {
    const response = await apiClient.delete<ApiResponseDto<boolean>>(`/api/roles/${id}`);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to delete role');
    }
  }

  async getUserRoles(userId: string): Promise<RoleDto[]> {
    const response = await apiClient.get<ApiResponseDto<RoleDto[]>>(`/api/users/${userId}/roles`);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to fetch user roles');
    }

    return response.data.data;
  }

  async getRoleUsers(roleId: number): Promise<any[]> {
    const response = await apiClient.get<ApiResponseDto<any[]>>(`/api/roles/${roleId}/users`);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to fetch role users');
    }

    return response.data.data;
  }

  async assignRoleToUser(userId: number, roleId: number): Promise<void> {
    const response = await apiClient.post<ApiResponseDto<boolean>>('/api/roles/assign', {
      userId: userId.toString(),
      roleId
    });
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to assign role');
    }
  }

  async removeRoleFromUser(roleId: number, userId: string): Promise<void> {
    const response = await apiClient.delete<ApiResponseDto<boolean>>(`/api/roles/${roleId}/users/${userId}`);
    
    if (!response.data.success) {
      throw new Error(response.data.message || 'Failed to remove role');
    }
  }
}

// Export instance for easy use
export const roleService = new RoleService();

// Export types
export type RoleCreateRequest = CreateRoleRequest;
export type RoleUpdateRequest = UpdateRoleRequest;
