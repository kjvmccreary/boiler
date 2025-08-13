import { apiClient } from './api.client.js';
import { API_ENDPOINTS } from '@/utils/api.constants.js';
import type { 
  User,
  PaginatedResponse
} from '@/types/index.js';

export interface UserUpdateRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface UsersListParams {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface CreateUserRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export class UserService {
  async getCurrentUserProfile(): Promise<User> {
    console.log('🔍 UserService: getCurrentUserProfile called');
    try {
      const response = await apiClient.get<User>(API_ENDPOINTS.USERS.PROFILE);
      console.log('✅ UserService: getCurrentUserProfile response:', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ UserService: getCurrentUserProfile failed:', error);
      throw error;
    }
  }

  async updateCurrentUserProfile(userData: UserUpdateRequest): Promise<User> {
    const response = await apiClient.put<User>(API_ENDPOINTS.USERS.PROFILE, userData);
    return response.data;
  }

  async getUsers(params: UsersListParams = {}): Promise<PaginatedResponse<User>> {
    console.log('🔍 UserService: getUsers called with params:', params);
    
    try {
      const queryParams = new URLSearchParams();
      
      if (params.page) queryParams.append('page', params.page.toString());
      if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
      if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm);
      if (params.sortBy) queryParams.append('sortBy', params.sortBy);
      if (params.sortDirection) queryParams.append('sortDirection', params.sortDirection);

      const url = `${API_ENDPOINTS.USERS.BASE}?${queryParams.toString()}`;
      console.log('🔍 UserService: Making request to:', url);
      
      const response = await apiClient.get<any>(url);
      console.log('🔍 UserService: Raw response:', response);
      
      // Handle .NET 9 API response structure
      let responseData = response.data;
      
      // If response has success/data structure, unwrap it
      if (responseData && typeof responseData === 'object' && 'success' in responseData) {
        console.log('🔍 UserService: Detected wrapped response structure');
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to fetch users');
        }
        responseData = responseData.data;
      }
      
      console.log('🔍 UserService: Processed response data:', responseData);
      
      // Handle both PagedResult and direct array responses
      if (responseData && typeof responseData === 'object') {
        // Check if it's a PagedResult structure
        if ('items' in responseData && Array.isArray(responseData.items)) {
          console.log('✅ UserService: Found PagedResult structure');
          return {
            data: responseData.items,
            totalCount: responseData.totalCount || 0,
            pageNumber: responseData.pageNumber || 1,
            pageSize: responseData.pageSize || 10,
            totalPages: responseData.totalPages || 0
          };
        }
        
        // Check if it's a direct array
        if (Array.isArray(responseData)) {
          console.log('✅ UserService: Found direct array structure');
          return {
            data: responseData,
            totalCount: responseData.length,
            pageNumber: 1,
            pageSize: responseData.length,
            totalPages: 1
          };
        }
        
        // Check if it has other pagination properties
        if ('data' in responseData && Array.isArray(responseData.data)) {
          console.log('✅ UserService: Found nested data array structure');
          return {
            data: responseData.data,
            totalCount: responseData.totalCount || responseData.data.length,
            pageNumber: responseData.pageNumber || responseData.page || 1,
            pageSize: responseData.pageSize || responseData.data.length,
            totalPages: responseData.totalPages || 1
          };
        }
      }
      
      // Fallback - treat as empty result
      console.log('⚠️ UserService: Unexpected response structure, returning empty result');
      return {
        data: [],
        totalCount: 0,
        pageNumber: 1,
        pageSize: 10,
        totalPages: 0
      };
      
    } catch (error) {
      console.error('❌ UserService: getUsers failed:', error);
      
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
          throw new Error('You do not have permission to view users');
        } else if (axiosError.response?.status === 401) {
          throw new Error('Please log in to view users');
        } else if (axiosError.response?.status === 404) {
          throw new Error('User service not found');
        } else if (axiosError.response?.status >= 500) {
          throw new Error('Server error - please try again later');
        }
      }
      
      throw error;
    }
  }

  async getUserById(id: string): Promise<User> {
    console.log('🔍 UserService: getUserById called with id:', id);
    try {
      const response = await apiClient.get<User>(API_ENDPOINTS.USERS.BY_ID(id));
      console.log('✅ UserService: getUserById response:', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ UserService: getUserById failed:', error);
      throw error;
    }
  }

  async deleteUser(id: string): Promise<boolean> {
    const response = await apiClient.delete<boolean>(API_ENDPOINTS.USERS.BY_ID(id));
    return response.data;
  }

  async getUserRoles(id: string): Promise<string[]> {
    const response = await apiClient.get<string[]>(API_ENDPOINTS.USERS.ROLES(id));
    return response.data;
  }

  async updateUser(id: string, userData: UserUpdateRequest): Promise<User> {
    console.log('🔍 UserService: updateUser called with id:', id, 'data:', userData);
    try {
      const response = await apiClient.put<any>(API_ENDPOINTS.USERS.BY_ID(id), userData);
      console.log('✅ UserService: updateUser raw response:', response.data);
      
      // ✅ Handle .NET 9 API response structure
      let responseData = response.data;
      
      // If response has success/data structure, unwrap it
      if (responseData && typeof responseData === 'object' && 'success' in responseData) {
        console.log('🔍 UserService: Detected wrapped response structure');
        if (!responseData.success) {
          throw new Error(responseData.message || 'Failed to update user');
        }
        responseData = responseData.data;
      }
      
      console.log('✅ UserService: updateUser processed response:', responseData);
      return responseData;
    } catch (error) {
      console.error('❌ UserService: updateUser failed:', error);
      throw error;
    }
  }

  async createUser(userData: CreateUserRequest): Promise<User> {
    console.log('🔍 UserService: createUser called');
    try {
      const response = await apiClient.post<User>('/api/users', userData);
      console.log('✅ UserService: createUser successful', response.data);
      return response.data;
    } catch (error) {
      console.error('❌ UserService: createUser failed:', error);
      throw error;
    }
  }
}

export const userService = new UserService();
