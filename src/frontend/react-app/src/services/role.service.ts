import { apiClient } from './api.client.js';
import { API_ENDPOINTS } from '@/utils/api.constants.js';
import type { 
  Role,
  User
} from '@/types/index.js';

export interface RoleCreateRequest {
  name: string;
  description?: string;
  permissions: string[];
}

export interface RoleUpdateRequest {
  name: string;
  description?: string;
  permissions: string[];
}

export interface AssignRoleRequest {
  userId: number;
  roleId: number;
}

export interface RoleListParams {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
}

export class RoleService {
  async getRoles(params: RoleListParams = {}): Promise<Role[]> {
    const queryParams = new URLSearchParams();
    
    if (params.page) queryParams.append('page', params.page.toString());
    if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm);

    const url = `${API_ENDPOINTS.ROLES.BASE}?${queryParams.toString()}`;
    const response = await apiClient.get<Role[]>(url);
    return response.data;
  }

  async getRoleById(id: string): Promise<Role> {
    const response = await apiClient.get<Role>(API_ENDPOINTS.ROLES.BY_ID(id));
    return response.data;
  }

  async createRole(roleData: RoleCreateRequest): Promise<Role> {
    const response = await apiClient.post<Role>(API_ENDPOINTS.ROLES.BASE, roleData);
    return response.data;
  }

  async updateRole(id: string, roleData: RoleUpdateRequest): Promise<Role> {
    const response = await apiClient.put<Role>(API_ENDPOINTS.ROLES.BY_ID(id), roleData);
    return response.data;
  }

  async deleteRole(id: string): Promise<boolean> {
    const response = await apiClient.delete<boolean>(API_ENDPOINTS.ROLES.BY_ID(id));
    return response.data;
  }

  async getRolePermissions(id: string): Promise<string[]> {
    const response = await apiClient.get<string[]>(API_ENDPOINTS.ROLES.PERMISSIONS(id));
    return response.data;
  }

  async updateRolePermissions(id: string, permissions: string[]): Promise<boolean> {
    const response = await apiClient.put<boolean>(
      API_ENDPOINTS.ROLES.PERMISSIONS(id), 
      { permissions }
    );
    return response.data;
  }

  async assignRoleToUser(userId: number, roleId: number): Promise<boolean> {
    const response = await apiClient.post<boolean>(
      `${API_ENDPOINTS.ROLES.BASE}/assign`, 
      { userId, roleId }
    );
    return response.data;
  }

  async removeRoleFromUser(roleId: string, userId: string): Promise<boolean> {
    const response = await apiClient.delete<boolean>(
      API_ENDPOINTS.ROLES.ASSIGN_USER(roleId, userId)
    );
    return response.data;
  }

  async getUserRoles(userId: string): Promise<Role[]> {
    const response = await apiClient.get<Role[]>(`${API_ENDPOINTS.ROLES.BASE}/users/${userId}`);
    return response.data;
  }

  async getRoleUsers(roleId: string): Promise<User[]> {
    const response = await apiClient.get<User[]>(`${API_ENDPOINTS.ROLES.BY_ID(roleId)}/users`);
    return response.data;
  }
}

export const roleService = new RoleService();
