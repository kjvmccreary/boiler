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
    const response = await apiClient.get<PaginatedResponse<User>>(url);
    return response.data;
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
}

export const userService = new UserService();
