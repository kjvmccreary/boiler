export interface TenantDto {
  id: number;
  name: string;
  domain?: string;
  subscriptionPlan: string;
  isActive: boolean;
  createdAt: string;
  updatedAt: string;
  settings: Record<string, any>;
  userCount: number;
  roleCount: number;
  activeUserCount: number;
}

export interface CreateAdditionalTenantDto {
  tenantName: string;
  tenantDomain?: string | null;
}

export interface CreateTenantDto {
  name: string;
  domain?: string;
  subscriptionPlan: string;
  settings: Record<string, any>;
  adminUser: CreateTenantAdminDto;
}

export interface CreateTenantAdminDto {
  email: string;
  firstName: string;
  lastName: string;
  password: string;
}

export interface UpdateTenantDto {
  name: string;
  domain?: string;
  subscriptionPlan: string;
  isActive: boolean;
  settings: Record<string, any>;
}

export interface RoleTemplateDto {
  name: string;
  description: string;
  permissions: string[];
  isDefault: boolean;
}
