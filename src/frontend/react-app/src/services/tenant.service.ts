import { apiClient } from './api.client.js';
import type { Tenant, ApiResponse, TenantSettings } from '@/types/index.js';

class TenantService {
  // 🔧 HELPER: Convert backend TenantDto to frontend Tenant
  private convertTenantDto(backendTenant: any): Tenant {
    return {
      id: backendTenant.id.toString(), // 🔧 Convert int to string
      name: backendTenant.name,
      domain: backendTenant.domain,
      subscriptionPlan: backendTenant.subscriptionPlan,
      isActive: backendTenant.isActive,
      createdAt: backendTenant.createdAt,
      updatedAt: backendTenant.updatedAt,
    };
  }

  async getUserTenants(userId: string): Promise<ApiResponse<Tenant[]>> {
    try {
      console.log('🔍 TenantService: Calling API endpoint:', `/api/users/${userId}/tenants`);
      const response = await apiClient.get<any[]>(`/api/users/${userId}/tenants`);
      
      console.log('🔍 TenantService: Raw API response:', response.data);
      
      // 🔧 FIX: The API client already unwraps the response, so response.data is the array directly
      if (Array.isArray(response.data) && response.data.length > 0) {
        const convertedTenants = response.data.map(this.convertTenantDto);
        
        console.log('🔍 TenantService: Converted tenants:', convertedTenants);
        
        return {
          success: true,
          message: 'User tenants loaded',
          data: convertedTenants
        };
      } else if (Array.isArray(response.data) && response.data.length === 0) {
        // Handle empty array case
        return {
          success: true,
          message: 'No tenants found',
          data: []
        };
      }
      
      // If we get here, the response format is unexpected
      console.warn('🔍 TenantService: Unexpected response format:', response.data);
      throw new Error(`Unexpected API response format: ${typeof response.data}`);
      
    } catch (error) {
      console.error('🏢 TenantService: API call failed:', error);
      throw error;
    }
  }

  async getTenantSettings(tenantId: string): Promise<ApiResponse<TenantSettings>> {
    try {
      const response = await apiClient.get<ApiResponse<TenantSettings>>(`/api/tenants/${tenantId}/settings`);
      return response.data;
    } catch (error) {
      console.warn('🏢 TenantService: Settings API not available, using defaults');
      
      // 🔧 MOCK: Return default settings
      return {
        success: true,
        message: 'Default settings loaded',
        data: {
          theme: {
            primaryColor: '#1976d2',
            companyName: 'Default Tenant'
          },
          features: {
            multiUser: true,
            reports: true,
            analytics: false
          },
          subscriptionPlan: 'Development'
        }
      };
    }
  }

  /// <summary>
  /// Switch tenant by issuing new JWT tokens
  /// </summary>
  async switchTenant(tenantId: string): Promise<ApiResponse<{
    accessToken: string;
    refreshToken: string;
    user: any;
    tenant: any;
  }>> {
    try {
      console.log('🏢 TenantService: Switching to tenant:', tenantId);
      console.log('🔍 TenantService: Request payload:', { tenantId: parseInt(tenantId) });
      
      // Check current authentication state BEFORE request
      const currentTokenBefore = localStorage.getItem('auth_token');
      const currentRefreshTokenBefore = localStorage.getItem('refresh_token');
      console.log('🔍 TenantService: BEFORE request - Auth state:', {
        hasAccessToken: !!currentTokenBefore,
        hasRefreshToken: !!currentRefreshTokenBefore,
        tokenPreview: currentTokenBefore?.substring(0, 50) + '...'
      });
      
      // Decode current token to see tenant BEFORE request
      if (currentTokenBefore) {
        try {
          const beforePayload = JSON.parse(atob(currentTokenBefore.split('.')[1]));
          console.log('🔍 TenantService: BEFORE token tenant info:', {
            tenant_id: beforePayload.tenant_id,
            tenant_name: beforePayload.tenant_name
          });
        } catch (e) {
          console.warn('Could not decode before token:', e);
        }
      }
      
      console.log('🔍 TenantService: Making API call to switch-tenant...');
      const response = await apiClient.post<any>('/api/auth/switch-tenant', { 
        tenantId: parseInt(tenantId) 
      });
      
      console.log('🔍 TenantService: API response received:', {
        status: response.status,
        hasData: !!response.data,
        dataKeys: response.data ? Object.keys(response.data) : [],
        dataType: typeof response.data
      });
      
      console.log('🔍 TenantService: Full response data:', response.data);
      
      // The API client already unwraps ApiResponseDto, so response.data should be TokenResponseDto directly
      const tokenData = response.data;
      
      console.log('🔍 TenantService: Token data analysis:', {
        tokenData: tokenData,
        hasAccessToken: !!(tokenData?.accessToken),
        hasRefreshToken: !!(tokenData?.refreshToken),
        hasUser: !!(tokenData?.user),
        hasTenant: !!(tokenData?.tenant),
        accessTokenPreview: tokenData?.accessToken?.substring(0, 50) + '...',
        refreshTokenPreview: tokenData?.refreshToken?.substring(0, 20) + '...'
      });
      
      if (tokenData && tokenData.accessToken && tokenData.refreshToken) {
        console.log('✅ TenantService: Valid token data found, storing tokens...');
        
        // Store old tokens for comparison
        const oldToken = localStorage.getItem('auth_token');
        const oldRefreshToken = localStorage.getItem('refresh_token');
        
        // Store new tokens
        localStorage.setItem('auth_token', tokenData.accessToken);
        localStorage.setItem('refresh_token', tokenData.refreshToken);
        
        // Verify tokens were stored
        const newToken = localStorage.getItem('auth_token');
        const newRefreshToken = localStorage.getItem('refresh_token');
        
        console.log('🔧 TenantService: Token storage verification:', {
          tokenChanged: oldToken !== newToken,
          refreshTokenChanged: oldRefreshToken !== newRefreshToken,
          newTokenPreview: newToken?.substring(0, 50) + '...',
          newRefreshTokenPreview: newRefreshToken?.substring(0, 20) + '...'
        });
        
        // Decode new token to verify tenant ID
        if (newToken) {
          try {
            const newPayload = JSON.parse(atob(newToken.split('.')[1]));
            console.log('🔍 TenantService: NEW token tenant verification:', {
              tenant_id: newPayload.tenant_id,
              tenant_name: newPayload.tenant_name,
              user_id: newPayload.nameid || newPayload.sub,
              expected_tenant_id: tenantId,
              tenant_id_matches: newPayload.tenant_id.toString() === tenantId.toString()
            });
            
            if (newPayload.tenant_id.toString() !== tenantId.toString()) {
              console.error('🚨 TenantService: CRITICAL ERROR - New token has wrong tenant_id!', {
                expected: tenantId,
                actual: newPayload.tenant_id,
                backend_probably_returned_wrong_token: true
              });
            } else {
              console.log('✅ TenantService: New token has correct tenant_id!');
            }
          } catch (decodeError) {
            console.error('🔍 TenantService: Could not decode new token:', decodeError);
          }
        }
        
        return {
          success: true,
          message: 'Tenant switched successfully',
          data: tokenData
        };
      }
      
      console.error('🚨 TenantService: Invalid response structure - missing tokens:', {
        responseData: tokenData,
        hasAccessToken: !!(tokenData?.accessToken),
        hasRefreshToken: !!(tokenData?.refreshToken)
      });
      throw new Error('Invalid response from switch-tenant endpoint - missing tokens');
      
    } catch (error) {
      console.error('🚨 TenantService: Switch tenant failed:', error);
      
      // Enhanced error logging
      if (error instanceof Error) {
        console.error('🚨 TenantService: Error details:', {
          name: error.name,
          message: error.message,
          stack: error.stack?.split('\n').slice(0, 5) // First 5 lines of stack
        });
      }
      
      // Check for HTTP error details
      if ((error as any).response) {
        const httpError = (error as any).response;
        console.error('🚨 TenantService: HTTP error details:', {
          status: httpError.status,
          statusText: httpError.statusText,
          data: httpError.data
        });
      }
      
      throw error;
    }
  }

  // 🔧 NEW: Additional methods for future use
  async getAllTenants(): Promise<ApiResponse<{ items: Tenant[]; totalCount: number }>> {
    try {
      const response = await apiClient.get<ApiResponse<{ items: any[]; totalCount: number }>>('/api/tenants');
      
      if (response.data.success && response.data.data) {
        // 🔧 Convert backend DTOs to frontend Tenant objects
        const convertedTenants = response.data.data.items.map(this.convertTenantDto);
        
        return {
          success: true,
          message: 'Tenants loaded',
          data: {
            items: convertedTenants,
            totalCount: response.data.data.totalCount
          }
        };
      }
      
      throw new Error('Failed to load tenants');
    } catch (error) {
      console.warn('🏢 TenantService: Get all tenants API not available');
      return {
        success: false,
        message: 'API not available',
        data: { items: [], totalCount: 0 }
      };
    }
  }

  async createTenant(tenantData: { name: string; domain?: string; subscriptionPlan?: string }): Promise<ApiResponse<Tenant>> {
    try {
      const response = await apiClient.post<ApiResponse<any>>('/api/tenants', tenantData);
      
      if (response.data.success && response.data.data) {
        return {
          success: true,
          message: 'Tenant created',
          data: this.convertTenantDto(response.data.data)
        };
      }
      
      throw new Error('Failed to create tenant');
    } catch (error) {
      console.warn('🏢 TenantService: Create tenant API not available');
      throw error;
    }
  }
}

export const tenantService = new TenantService();
