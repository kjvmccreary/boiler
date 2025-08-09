import { apiClient } from './api.client.js';
import { API_ENDPOINTS } from '@/utils/api.constants.js';

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
    const response = await apiClient.get<PermissionDto[]>(API_ENDPOINTS.PERMISSIONS.BASE);
    return response.data;
  }

  async getPermissionCategories(): Promise<string[]> {
    const response = await apiClient.get<string[]>(API_ENDPOINTS.PERMISSIONS.CATEGORIES);
    return response.data;
  }

  async getPermissionsByCategory(category: string): Promise<PermissionDto[]> {
    const response = await apiClient.get<PermissionDto[]>(`${API_ENDPOINTS.PERMISSIONS.CATEGORIES}/${category}`);
    return response.data;
  }

  async getPermissionsGrouped(): Promise<PermissionsByCategory> {
    const response = await apiClient.get<PermissionsByCategory>(`${API_ENDPOINTS.PERMISSIONS.BASE}/grouped`);
    return response.data;
  }

  async getUserPermissions(userId: number): Promise<string[]> {
    const response = await apiClient.get<string[]>(`${API_ENDPOINTS.PERMISSIONS.BASE}/users/${userId}`);
    return response.data;
  }

  async getMyPermissions(): Promise<string[]> {
    const response = await apiClient.get<string[]>(`${API_ENDPOINTS.PERMISSIONS.BASE}/me`);
    return response.data;
  }

  async checkUserPermission(userId: number, permission: string): Promise<boolean> {
    const response = await apiClient.get<boolean>(`${API_ENDPOINTS.PERMISSIONS.BASE}/users/${userId}/check/${permission}`);
    return response.data;
  }

  async checkUserHasAnyPermissions(userId: number, permissions: string[]): Promise<boolean> {
    const response = await apiClient.post<boolean>(`${API_ENDPOINTS.PERMISSIONS.BASE}/users/${userId}/check-any`, permissions);
    return response.data;
  }

  async checkUserHasAllPermissions(userId: number, permissions: string[]): Promise<boolean> {
    const response = await apiClient.post<boolean>(`${API_ENDPOINTS.PERMISSIONS.BASE}/users/${userId}/check-all`, permissions);
    return response.data;
  }
}

export const permissionService = new PermissionService();
