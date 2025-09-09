import { apiClient } from './api.client';
import { API_ENDPOINTS } from '@/utils/api.constants';
import type {
  User,
  PaginatedResponse
} from '@/types/index';

/* === Added (PR3 fix) =========================================
   Types required by the new search() method used in AssignmentSection.
   Keeping them lightweight & local to avoid cross‑module coupling.
================================================================ */
export interface UserSearchResult {
  id: string;
  displayName: string;
  email?: string;
}

interface RawPaged<T> {
  items?: T[];
  data?: any;
  page?: number;
  pageSize?: number;
  totalCount?: number;
  [k: string]: any;
}

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

// Backend PagedResultDto shape (kept local)
interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export class UserService {
  async getCurrentUserProfile(): Promise<User> {
    try {
      const response = await apiClient.get<User>(API_ENDPOINTS.USERS.PROFILE);
      return response.data;
    } catch (error) {
      throw error;
    }
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
    const response = await apiClient.get<PagedResultDto<User>>(url);
    const backendData = response.data;

    return {
      data: backendData.items,
      totalCount: backendData.totalCount,
      pageNumber: backendData.pageNumber,
      pageSize: backendData.pageSize,
      totalPages: backendData.totalPages
    };
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
    const response = await apiClient.put<User>(API_ENDPOINTS.USERS.BY_ID(id), userData);
    return response.data;
  }

  async createUser(userData: CreateUserRequest): Promise<User> {
    const response = await apiClient.post<User>('/api/users', userData);
    return response.data;
  }

  /**
   * Lightweight user search used by HumanTask AssignmentSection.
   * Accepts multiple backend response shapes:
   *  - Array<UserSearchResult>
   *  - { items: UserSearchResult[] }
   *  - { data: { items: UserSearchResult[] } }
   */
  async search(term: string, page = 1, pageSize = 20): Promise<UserSearchResult[]> {
    if (!term.trim()) return [];

    let raw: RawPaged<UserSearchResult> | UserSearchResult[] | undefined;

    try {
      const resp = await apiClient.get(
        `/api/users?search=${encodeURIComponent(term)}&page=${page}&pageSize=${pageSize}`
      );
      const top = (resp as any)?.data;
      raw = (top && typeof top === 'object' && 'data' in top) ? (top as any).data : top;
    } catch {
      return [];
    }

    if (!raw) return [];

    if (Array.isArray(raw)) return raw as UserSearchResult[];
    if (Array.isArray((raw as any).items)) return (raw as any).items as UserSearchResult[];
    if ((raw as any).data && Array.isArray((raw as any).data.items)) {
      return (raw as any).data.items as UserSearchResult[];
    }
    return [];
  }
}

export const userService = new UserService();
