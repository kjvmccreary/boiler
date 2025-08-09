// API Response Types
export interface ApiResponse<T = any> {
  data: T;
  message?: string;
  success: boolean;
}

export interface PaginatedResponse<T> {
  data: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// Auth Types
export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  user: User;
  permissions: string[];
}

export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  isEmailConfirmed: boolean;
  roles: Role[];
  tenantId: string;
  createdAt: string;
  updatedAt: string;
}

// RBAC Types
export interface Role {
  id: string;
  name: string;
  description?: string;
  isSystemRole: boolean;
  isDefault: boolean;
  tenantId: string;
  permissions: Permission[];
  createdAt: string;
  updatedAt: string;
}

export interface Permission {
  id: string;
  name: string;
  category: string;
  description?: string;
}

export interface CreateRoleRequest {
  name: string;
  description?: string;
  permissions: string[];
}

export interface UpdateRoleRequest {
  name: string;
  description?: string;
  permissions: string[];
}

export interface AssignRoleRequest {
  userId: string;
  roleId: string;
}

// UI Types
export interface LoadingState {
  isLoading: boolean;
  error?: string;
}

export interface FormErrors {
  [key: string]: string;
}

// Navigation Types
export interface NavItem {
  label: string;
  path: string;
  icon?: string;
  permission?: string;
  children?: NavItem[];
}
