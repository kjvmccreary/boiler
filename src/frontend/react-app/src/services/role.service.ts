import { apiClient } from './api.client';
import type { 
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

      const response = await apiClient.get<any>(url);
      console.log('🔍 RoleService: Raw response:', response);

      // Handle .NET 9 API response structure
      let responseData = response.data;
      
      // If response has success/data structure, unwrap it
      if (responseData && typeof responseData === 'object' && 'success' in responseData) {
        console.log('🔍 RoleService: Detected wrapped response structure');
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to fetch roles');
        }
        responseData = responseData.data;
      }
      
      console.log('🔍 RoleService: Processed response data:', responseData);
      
      // Handle PagedResultDto structure from backend
      if (responseData && typeof responseData === 'object') {
        // Check if it's a PagedResult structure (what the backend returns)
        if ('items' in responseData && Array.isArray(responseData.items)) {
          console.log('✅ RoleService: Found PagedResult structure');
          return {
            roles: responseData.items,
            pagination: {
              totalCount: responseData.totalCount || 0,
              pageNumber: responseData.pageNumber || 1,
              pageSize: responseData.pageSize || 10,
              totalPages: responseData.totalPages || 0
            }
          };
        }
        
        // Check if it's a direct array
        if (Array.isArray(responseData)) {
          console.log('✅ RoleService: Found direct array structure');
          return {
            roles: responseData,
            pagination: {
              totalCount: responseData.length,
              pageNumber: 1,
              pageSize: responseData.length,
              totalPages: 1
            }
          };
        }
      }
      
      // Fallback - treat as empty result
      console.log('⚠️ RoleService: Unexpected response structure, returning empty result');
      return {
        roles: [],
        pagination: {
          totalCount: 0,
          pageNumber: 1,
          pageSize: 10,
          totalPages: 0
        }
      };
      
    } catch (error) {
      console.error('❌ RoleService: getRoles failed:', error);
      
      // Enhanced error logging
      if (error instanceof Error) {
        console.error('❌ Error details:', {
          message: error.message,
          stack: error.stack,
          name: error.name
        });
      }
      
      // Check if it's an axios error with response data
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as any;
        console.error('❌ Axios error details:', {
          status: axiosError.response?.status,
          statusText: axiosError.response?.statusText,
          data: axiosError.response?.data,
          headers: axiosError.response?.headers
        });
        
        // Handle specific HTTP errors
        if (axiosError.response?.status === 403) {
          throw new Error('You do not have permission to view roles');
        } else if (axiosError.response?.status === 401) {
          throw new Error('Please log in to view roles');
        } else if (axiosError.response?.status === 404) {
          throw new Error('Roles service not found');
        } else if (axiosError.response?.status >= 500) {
          throw new Error('Server error - please try again later');
        }
      }
      
      throw error;
    }
  }

  async getRoleById(id: number): Promise<RoleDto> {
    console.log('🔍 RoleService: getRoleById called with id:', id);
    try {
      const response = await apiClient.get<any>(`/api/roles/${id}`);
      
      // Handle wrapped response
      if (response.data && 'success' in response.data) {
        if (!response.data.success) {
          throw new Error(response.data.message || 'Failed to fetch role');
        }
        return response.data.data;
      }
      
      return response.data;
    } catch (error) {
      console.error('❌ RoleService: getRoleById failed:', error);
      throw error;
    }
  }

  async createRole(roleData: CreateRoleRequest): Promise<RoleDto> {
    const response = await apiClient.post<any>('/api/roles', roleData);
    
    // Handle wrapped response
    if (response.data && 'success' in response.data) {
      if (!response.data.success) {
        throw new Error(response.data.message || 'Failed to create role');
      }
      return response.data.data;
    }
    
    return response.data;
  }

  async updateRole(id: number, roleData: UpdateRoleRequest): Promise<RoleDto> {
    const response = await apiClient.put<any>(`/api/roles/${id}`, roleData);
    
    // Handle wrapped response
    if (response.data && 'success' in response.data) {
      if (!response.data.success) {
        throw new Error(response.data.message || 'Failed to update role');
      }
      return response.data.data;
    }
    
    return response.data;
  }

  async deleteRole(id: number): Promise<void> {
    console.log('🔍 RoleService: deleteRole called with id:', id);
    try {
      const response = await apiClient.delete<any>(`/api/roles/${id}`);
      
      // Handle wrapped response
      if (response.data && 'success' in response.data) {
        if (!response.data.success) {
          throw new Error(response.data.message || 'Failed to delete role');
        }
      }
      
      console.log('✅ RoleService: deleteRole successful');
    } catch (error) {
      console.error('❌ RoleService: deleteRole failed:', error);
      throw error;
    }
  }

  async getUserRoles(userId: string): Promise<RoleDto[]> {
    const response = await apiClient.get<any>(`/api/users/${userId}/roles`);
    
    // Handle wrapped response
    if (response.data && 'success' in response.data) {
      if (!response.data.success) {
        throw new Error(response.data.message || 'Failed to fetch user roles');
      }
      return response.data.data;
    }
    
    return response.data;
  }

  async getRoleUsers(roleId: number): Promise<any[]> {
    const response = await apiClient.get<any>(`/api/roles/${roleId}/users`);
    
    // Handle wrapped response
    if (response.data && 'success' in response.data) {
      if (!response.data.success) {
        throw new Error(response.data.message || 'Failed to fetch role users');
      }
      return response.data.data;
    }
    
    return response.data;
  }

  async assignRoleToUser(userId: number, roleId: number): Promise<void> {
    const response = await apiClient.post<any>('/api/roles/assign', {
      userId: userId.toString(),
      roleId
    });
    
    // Handle wrapped response
    if (response.data && 'success' in response.data) {
      if (!response.data.success) {
        throw new Error(response.data.message || 'Failed to assign role');
      }
    }
  }

  async removeRoleFromUser(roleId: number, userId: string): Promise<void> {
    const response = await apiClient.delete<any>(`/api/roles/${roleId}/users/${userId}`);
    
    // Handle wrapped response
    if (response.data && 'success' in response.data) {
      if (!response.data.success) {
        throw new Error(response.data.message || 'Failed to remove role');
      }
    }
  }
}

// Export instance for easy use
export const roleService = new RoleService();

// Export types
export type RoleCreateRequest = CreateRoleRequest;
export type RoleUpdateRequest = UpdateRoleRequest;
