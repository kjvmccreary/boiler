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
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  tokenType: string;
  user: User;
  tenant: Tenant;
}

export interface Tenant {
  id: string;
  name: string;
  subdomain?: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
}

// Updated User type to match .NET 9 backend
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  phoneNumber?: string;
  timeZone?: string;
  language?: string;
  lastLoginAt?: string;
  emailConfirmed: boolean;
  isActive: boolean;
  roles: string[];          // .NET 9 FIX: Changed from Role[] to string[]
  tenantId: string;
  createdAt: string;
  updatedAt: string;
  preferences?: any;
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
