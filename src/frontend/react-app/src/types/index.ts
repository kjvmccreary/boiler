// API Response Types
export interface ApiResponse<T = unknown> {
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

// User preferences interface
export interface UserPreferences {
  theme?: 'light' | 'dark';
  language?: string;
  timeZone?: string;
  notifications?: {
    email: boolean;
    push: boolean;
    sms: boolean;
  };
  [key: string]: unknown;
}

// 🔧 .NET 9 MULTI-ROLE FIX: Enhanced User type for multiple roles
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
  roles: string | string[];     // 🔧 Handle both single role from JWT and multiple roles from API
  tenantId: string;
  createdAt: string;
  updatedAt: string;
  preferences?: UserPreferences;
}

// Add this interface after the User interface
export interface UserInfo {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  emailConfirmed: boolean;
  isActive: boolean;
  roles: string[];
  tenantId: string;
  createdAt: string;
  updatedAt: string;
}

// Permission interface
export interface Permission {
  id: string;
  name: string;
  category: string;
  description?: string;
}

// 🔧 UNIFIED ROLE TYPE: Use consistent ID type (number) to match backend
export interface Role {
  id: number;
  name: string;
  description?: string;
  isSystemRole: boolean;
  isDefault: boolean;
  tenantId?: number;
  permissions: Permission[];
  createdAt: string;
  updatedAt: string;
  userCount?: number;
}

// 🔧 SIMPLIFIED: Use Role for both internal and API responses
export type RoleDto = Role;

// 🔧 NEW: Separate interface for RBAC role assignments
export interface UserRoleAssignment {
  userId: string;
  roleId: number;
  roleName: string;
  tenantId: number;
  assignedAt: string;
  isActive: boolean;
  role: Role;
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
  roleId: number;
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

// Pagination interface
export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

// API Response types
export interface ApiResponseDto<T> {
  success: boolean;
  data: T;
  message?: string;
  errors?: string[];
}

// Role API response types
export type RolesListResponse = ApiResponseDto<PagedResultDto<RoleDto>>;
export type RoleResponse = ApiResponseDto<RoleDto>;

// Pagination request interface
export interface PaginationParams {
  page?: number;
  pageSize?: number;
  searchTerm?: string;
}

// Add the missing types that useRoles hook expects:
