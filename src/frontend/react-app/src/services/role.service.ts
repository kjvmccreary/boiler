import { apiClient } from './api.client';
import type { 
  RoleDto, 
  PaginationParams,
  CreateRoleRequest,
  UpdateRoleRequest,
  UserInfo
} from '../types';

// ✅ ADD: Type for backend's PagedResultDto
interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

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
    console.log('🔍 RoleService: getRoles called with params:', params);
    
    try {
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

      const url = `/api/roles?${queryParams}`;
      console.log('🔍 RoleService: Making request to:', url);

      // ✅ SIMPLIFIED: ApiClient now handles unwrapping automatically
      const response = await apiClient.get<PagedResultDto<RoleDto>>(url);
      console.log('🔍 RoleService: Processed response:', response.data);

      // Convert backend PagedResultDto to expected format
      const backendData = response.data;
      return {
        roles: backendData.items,
        pagination: {
          totalCount: backendData.totalCount,
          pageNumber: backendData.pageNumber,
          pageSize: backendData.pageSize,
          totalPages: backendData.totalPages
        }
      };
      
    } catch (error) {
      console.error('❌ RoleService: getRoles failed:', error);
      
      // Enhanced error logging
      if (error instanceof Error) {
        console.error('❌ Error details:', {
          message: error.message,
          name: error.name
        });
        
        // Handle specific HTTP errors with user-friendly messages
        if ((error as any).status === 403) {
          throw new Error('You do not have permission to view roles');
        } else if ((error as any).status === 401) {
          throw new Error('Please log in to view roles');
        } else if ((error as any).status === 404) {
          throw new Error('Roles service not found');
        } else if ((error as any).status >= 500) {
          throw new Error('Server error - please try again later');
        }
      }
      
      throw error;
    }
  }

  async getRoleById(id: number): Promise<RoleDto> {
    console.log('🔍 RoleService: getRoleById called with id:', id);
    try {
      const response = await apiClient.get<RoleDto>(`/api/roles/${id}`);
      return response.data;
    } catch (error) {
      console.error('❌ RoleService: getRoleById failed:', error);
      throw error;
    }
  }

  async getRolePermissions(roleId: number): Promise<string[]> {
    console.log('🔍 RoleService: getRolePermissions called with roleId:', roleId);
    try {
      const response = await apiClient.get<string[]>(`/api/roles/${roleId}/permissions`);
      console.log('✅ RoleService: Role permissions retrieved successfully');
      return response.data;
    } catch (error) {
      console.error('❌ RoleService: getRolePermissions failed:', error);
      throw error;
    }
  }

  async getRoleByName(roleName: string): Promise<RoleDto | null> {
    console.log('🔍 RoleService: getRoleByName called with roleName:', roleName);
    try {
      // Get all roles and find by name
      const result = await this.getRoles({ page: 1, pageSize: 100 });
      const role = result.roles.find(r => r.name === roleName);
      return role || null;
    } catch (error) {
      console.error('❌ RoleService: getRoleByName failed:', error);
      return null;
    }
  }

  async createRole(roleData: CreateRoleRequest): Promise<RoleDto> {
    const response = await apiClient.post<RoleDto>('/api/roles', roleData);
    return response.data;
  }

  async updateRole(id: number, roleData: UpdateRoleRequest): Promise<RoleDto> {
    const response = await apiClient.put<RoleDto>(`/api/roles/${id}`, roleData);
    return response.data;
  }

  async deleteRole(id: number): Promise<void> {
    console.log('🔍 RoleService: deleteRole called with id:', id);
    try {
      await apiClient.delete<boolean>(`/api/roles/${id}`);
      console.log('✅ RoleService: deleteRole successful');
    } catch (error) {
      console.error('❌ RoleService: deleteRole failed:', error);
      throw error;
    }
  }

  async getUserRoles(userId: string): Promise<RoleDto[]> {
    console.log('🔍 RoleService: getUserRoles called with userId:', userId);
    try {
      const response = await apiClient.get<RoleDto[]>(`/api/roles/users/${userId}`);
      console.log('🔍 RoleService: getUserRoles response:', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ RoleService: getUserRoles failed:', error);
      throw error;
    }
  }

  async getRoleUsers(roleId: number): Promise<UserInfo[]> {
    console.log('🔍 RoleService: getRoleUsers called with roleId:', roleId);
    try {
      const response = await apiClient.get<UserInfo[]>(`/api/roles/${roleId}/users`);
      console.log('✅ RoleService: Role users retrieved successfully');
      return response.data;
    } catch (error) {
      console.error('❌ RoleService: getRoleUsers failed:', error);
      throw error;
    }
  }

  async assignRoleToUser(userId: number, roleId: number): Promise<void> {
    console.log('🔍 RoleService: assignRoleToUser called with:', { userId, roleId });
    try {
      await apiClient.post<boolean>('/api/roles/assign', {
        userId,
        roleId
      });
      console.log('✅ RoleService: Role assigned successfully');
    } catch (error) {
      console.error('❌ RoleService: assignRoleToUser failed:', error);
      throw error;
    }
  }

  async removeRoleFromUser(roleId: number, userId: string): Promise<void> {
    console.log('🔍 RoleService: removeRoleFromUser called with:', { roleId, userId });
    try {
      await apiClient.delete<boolean>(`/api/roles/${roleId}/users/${userId}`);
      console.log('✅ RoleService: Role removed successfully');
    } catch (error) {
      console.error('❌ RoleService: removeRoleFromUser failed:', error);
      throw error;
    }
  }

  async getUserPermissions(userId: string): Promise<string[]> {
    console.log('🔍 RoleService: getUserPermissions called with userId:', userId);
    try {
      const response = await apiClient.get<string[]>(`/api/users/${userId}/permissions`);
      console.log('🔍 RoleService: getUserPermissions response:', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ RoleService: getUserPermissions failed:', error);
      throw error;
    }
  }
}

// Export instance for easy use
export const roleService = new RoleService();

// Export types
export type RoleCreateRequest = CreateRoleRequest;
export type RoleUpdateRequest = UpdateRoleRequest;
