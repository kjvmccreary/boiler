import { apiClient } from './api.client';
import { API_ENDPOINTS } from '@/utils/api.constants';
import type { 
  User,
  PaginatedResponse
} from '@/types/index';

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

// ✅ ADD: Type for backend's PagedResultDto
interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
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
      
      // ✅ SIMPLIFIED: ApiClient now handles unwrapping automatically
      const response = await apiClient.get<PagedResultDto<User>>(url);
      console.log('🔍 UserService: Processed response:', response.data);
      
      // Convert backend PagedResultDto to frontend PaginatedResponse
      const backendData = response.data;
      return {
        data: backendData.items,
        totalCount: backendData.totalCount,
        pageNumber: backendData.pageNumber,
        pageSize: backendData.pageSize,
        totalPages: backendData.totalPages
      };
      
    } catch (error) {
      console.error('❌ UserService: getUsers failed:', error);
      
      // Enhanced error logging
      if (error instanceof Error) {
        console.error('❌ Error details:', {
          message: error.message,
          name: error.name
        });
        
        // Handle specific HTTP errors with user-friendly messages
        if ((error as any).status === 403) {
          throw new Error('You do not have permission to view users');
        } else if ((error as any).status === 401) {
          throw new Error('Please log in to view users');
        } else if ((error as any).status === 404) {
          throw new Error('User service not found');
        } else if ((error as any).status >= 500) {
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
      // ✅ SIMPLIFIED: ApiClient handles unwrapping
      const response = await apiClient.put<User>(API_ENDPOINTS.USERS.BY_ID(id), userData);
      console.log('✅ UserService: updateUser response:', response.data);
      return response.data;
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
