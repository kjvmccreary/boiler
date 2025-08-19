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
  tenantName?: string; // 🔧 NEW: Optional for self-serve tenant creation
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  tokenType: string;
  user: User;
  tenant: Tenant;
}

// 🔧 REVERT: Keep your original Tenant interface that works with tests
export interface Tenant {
  id: string; // ✅ REVERTED: Back to string to match your tests
  name: string;
  domain?: string;
  subscriptionPlan?: string; // ✅ ADD: This was missing
  isActive: boolean;
  settings?: TenantSettings; // ✅ ADD: For tenant-specific settings
  createdAt: string;
  updatedAt: string;
  // Optional display properties
  userCount?: number;
  roleCount?: number;
}

// 🔧 NEW: Tenant settings interface (this was missing)
export interface TenantSettings {
  theme?: {
    primaryColor?: string;
    logo?: string;
    companyName?: string;
  };
  features?: {
    [key: string]: boolean;
  };
  subscriptionPlan?: string;
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

// 🔧 REVERT: Keep your original User interface that works with tests
export interface User {
  id: string; // ✅ REVERTED: Back to string to match your tests
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
  roles: string | string[];
  tenantId: string; // ✅ REVERTED: Back to string to match your tests
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
  tenantId: string; // ✅ REVERTED: Back to string
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

// 🔧 REVERT: Keep number for Role IDs (this part can stay)
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

export type RoleDto = Role;

export interface UserRoleAssignment {
  userId: string; // ✅ REVERTED: Back to string
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
  userId: string; // ✅ REVERTED: Back to string
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

// 🔧 ADD: Export tenant types
export type {
  TenantDto,
  CreateAdditionalTenantDto,
  CreateTenantDto,
  CreateTenantAdminDto,
  UpdateTenantDto,
  RoleTemplateDto
} from './tenant.js';
