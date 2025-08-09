import { apiClient } from './api.client.js';
import { API_ENDPOINTS } from '@/utils/api.constants.js';
import type { Permission } from '@/types/index.js';

export interface PermissionCategory {
  category: string;
  permissions: Permission[];
}

export class PermissionService {
  async getAllPermissions(): Promise<Permission[]> {
    const response = await apiClient.get<Permission[]>(API_ENDPOINTS.PERMISSIONS.BASE);
    return response.data;
  }

  async getPermissionCategories(): Promise<PermissionCategory[]> {
    const response = await apiClient.get<PermissionCategory[]>(API_ENDPOINTS.PERMISSIONS.CATEGORIES);
    return response.data;
  }

  // Helper method to organize permissions by category
  organizePermissionsByCategory(permissions: Permission[]): PermissionCategory[] {
    const categories = new Map<string, Permission[]>();
    
    permissions.forEach(permission => {
      const category = permission.category;
      if (!categories.has(category)) {
        categories.set(category, []);
      }
      categories.get(category)!.push(permission);
    });

    return Array.from(categories.entries()).map(([category, perms]) => ({
      category,
      permissions: perms.sort((a, b) => a.name.localeCompare(b.name))
    }));
  }
}

export const permissionService = new PermissionService();
