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

export class UserService {
  async getCurrentUserProfile(): Promise<User> {
    const response = await apiClient.get<User>(API_ENDPOINTS.USERS.PROFILE);
    return response.data;
  }

  async updateCurrentUserProfile(userData: UserUpdateRequest): Promise<User> {
    const response = await apiClient.put<User>(API_ENDPOINTS.USERS.PROFILE, userData);
    return response.data;
  }

  async getUsers(params: UsersListParams = {}): Promise<PaginatedResponse<User>> {
    const queryParams = new URLSearchParams();
    
    if (params.page) queryParams.append('page', params.page.toString());
    if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm);
    if (params.sortBy) queryParams.append('sortBy', params.sortBy);
    if (params.sortDirection) queryParams.append('sortDirection', params.sortDirection);

    const url = `${API_ENDPOINTS.USERS.BASE}?${queryParams.toString()}`;
    
    // 🔧 .NET 9 FIX: Handle ApiResponseDto<PagedResultDto<UserDto>> structure
    const response = await apiClient.get<any>(url);
    
    // The response is already unwrapped by apiClient, check if it has success property
    if (response.success && response.data) {
      // .NET 9 ApiResponseDto wrapper format
      return {
        data: response.data.items || [],
        totalCount: response.data.totalCount || 0,
        pageNumber: response.data.pageNumber || 1,
        pageSize: response.data.pageSize || 10,
        totalPages: response.data.totalPages || 1
      };
    }
    
    // Fallback for direct response
    return response;
  }

  async getUserById(id: string): Promise<User> {
    const response = await apiClient.get<User>(API_ENDPOINTS.USERS.BY_ID(id));
    return response.data;
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
    // 🔧 .NET 9 FIX: Add admin user update method
    const response = await apiClient.put<User>(API_ENDPOINTS.USERS.BY_ID(id), userData);
    return response.data;
  }
}

export const userService = new UserService();
