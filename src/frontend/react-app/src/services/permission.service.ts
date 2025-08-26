import { apiClient } from './api.client';
import { API_ENDPOINTS } from '@/utils/api.constants';

export interface PermissionDto {
  id: number;
  name: string;
  category: string;
  description: string;
  isActive: boolean;
}

export interface PermissionsByCategory {
  [category: string]: PermissionDto[];
}

export class PermissionService {
  async getAllPermissions(): Promise<PermissionDto[]> {
    console.log('🔍 PermissionService: getAllPermissions called');
    try {
      const response = await apiClient.get<PermissionDto[]>(API_ENDPOINTS.PERMISSIONS.BASE);
      console.log('🔍 PermissionService: getAllPermissions response:', response.data);
      return response.data || [];
    } catch (error) {
      console.error('❌ PermissionService: getAllPermissions failed:', error);
      throw error;
    }
  }

  async getPermissionCategories(): Promise<string[]> {
    console.log('🔍 PermissionService: getPermissionCategories called');
    try {
      const response = await apiClient.get<string[]>(API_ENDPOINTS.PERMISSIONS.CATEGORIES);
      console.log('🔍 PermissionService: getPermissionCategories response:', response.data);
      return response.data || [];
    } catch (error) {
      console.error('❌ PermissionService: getPermissionCategories failed:', error);
      throw error;
    }
  }

  async getPermissionsByCategory(category: string): Promise<PermissionDto[]> {
    console.log('🔍 PermissionService: getPermissionsByCategory called with:', category);
    try {
      const response = await apiClient.get<any>(`${API_ENDPOINTS.PERMISSIONS.CATEGORIES}/${category}`);
      console.log('🔍 PermissionService: getPermissionsByCategory raw response:', response.data);
      
      // Handle wrapped response
      let responseData = response.data;
      if (responseData && 'success' in responseData) {
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to fetch permissions by category');
        }
        responseData = responseData.data;
      }
      
      return responseData || [];
    } catch (error) {
      console.error('❌ PermissionService: getPermissionsByCategory failed:', error);
      throw error;
    }
  }

  async getPermissionsGrouped(): Promise<PermissionsByCategory> {
    console.log('🔍 PermissionService: getPermissionsGrouped called');
    try {
      const response = await apiClient.get<PermissionsByCategory>(API_ENDPOINTS.PERMISSIONS.GROUPED);
      console.log('🔍 PermissionService: getPermissionsGrouped response:', response.data);
      
      // ✅ REMOVE manual unwrapping since ApiClient now handles it consistently
      return response.data || {};
    } catch (error) {
      console.error('❌ PermissionService: getPermissionsGrouped failed:', error);
      
      // Enhanced error logging for permissions
      if (error && typeof error === 'object' && 'response' in error) {
        const axiosError = error as any;
        console.error('❌ Axios error details:', {
          status: axiosError.response?.status,
          statusText: axiosError.response?.statusText,
          data: axiosError.response?.data,
          headers: axiosError.response?.headers
        });
        
        // Handle specific permissions errors
        if (axiosError.response?.status === 403) {
          throw new Error('You do not have permission to view permissions. Please contact an administrator.');
        } else if (axiosError.response?.status === 401) {
          throw new Error('Please log in to view permissions');
        } else if (axiosError.response?.status === 404) {
          throw new Error('Permissions service not found');
        } else if (axiosError.response?.status >= 500) {
          throw new Error('Server error - please try again later');
        }
      }
      
      throw error;
    }
  }

  async getUserPermissions(userId: number): Promise<string[]> {
    console.log('🔍 PermissionService: getUserPermissions called with userId:', userId);
    try {
      const response = await apiClient.get<any>(`${API_ENDPOINTS.PERMISSIONS.USER_PERMISSIONS(userId.toString())}`);
      console.log('🔍 PermissionService: getUserPermissions raw response:', response.data);
      
      // Handle wrapped response
      let responseData = response.data;
      if (responseData && 'success' in responseData) {
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to fetch user permissions');
        }
        responseData = responseData.data;
      }
      
      return responseData || [];
    } catch (error) {
      console.error('❌ PermissionService: getUserPermissions failed:', error);
      throw error;
    }
  }

  async getMyPermissions(): Promise<string[]> {
    console.log('🔍 PermissionService: getMyPermissions called');
    try {
      const response = await apiClient.get<any>(API_ENDPOINTS.PERMISSIONS.ME);
      console.log('🔍 PermissionService: getMyPermissions raw response:', response.data);
      
      // Handle wrapped response
      let responseData = response.data;
      if (responseData && 'success' in responseData) {
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to fetch your permissions');
        }
        responseData = responseData.data;
      }
      
      return responseData || [];
    } catch (error) {
      console.error('❌ PermissionService: getMyPermissions failed:', error);
      throw error;
    }
  }

  async checkUserPermission(userId: number, permission: string): Promise<boolean> {
    console.log('🔍 PermissionService: checkUserPermission called with:', userId, permission);
    try {
      const response = await apiClient.get<any>(`${API_ENDPOINTS.PERMISSIONS.CHECK_PERMISSION(userId.toString(), permission)}`);
      console.log('🔍 PermissionService: checkUserPermission raw response:', response.data);
      
      // Handle wrapped response
      let responseData = response.data;
      if (responseData && 'success' in responseData) {
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to check user permission');
        }
        responseData = responseData.data;
      }
      
      return responseData || false;
    } catch (error) {
      console.error('❌ PermissionService: checkUserPermission failed:', error);
      throw error;
    }
  }

  async checkUserHasAnyPermissions(userId: number, permissions: string[]): Promise<boolean> {
    console.log('🔍 PermissionService: checkUserHasAnyPermissions called with:', userId, permissions);
    try {
      const response = await apiClient.post<any>(`${API_ENDPOINTS.PERMISSIONS.CHECK_ANY(userId.toString())}`, permissions);
      console.log('🔍 PermissionService: checkUserHasAnyPermissions raw response:', response.data);
      
      // Handle wrapped response
      let responseData = response.data;
      if (responseData && 'success' in responseData) {
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to check user permissions');
        }
        responseData = responseData.data;
      }
      
      return responseData || false;
    } catch (error) {
      console.error('❌ PermissionService: checkUserHasAnyPermissions failed:', error);
      throw error;
    }
  }

  async checkUserHasAllPermissions(userId: number, permissions: string[]): Promise<boolean> {
    console.log('🔍 PermissionService: checkUserHasAllPermissions called with:', userId, permissions);
    try {
      const response = await apiClient.post<any>(`${API_ENDPOINTS.PERMISSIONS.CHECK_ALL(userId.toString())}`, permissions);
      console.log('🔍 PermissionService: checkUserHasAllPermissions raw response:', response.data);
      
      // Handle wrapped response
      let responseData = response.data;
      if (responseData && 'success' in responseData) {
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to check user permissions');
        }
        responseData = responseData.data;
      }
      
      return responseData || false;
    } catch (error) {
      console.error('❌ PermissionService: checkUserHasAllPermissions failed:', error);
      throw error;
    }
  }
}

export const permissionService = new PermissionService();
