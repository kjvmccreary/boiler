import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'

// ✅ ADD: Type for .NET 9 API Response structure
interface DotNetApiResponse<T = any> {
  success: boolean;
  data: T;
  message: string;
  errors: any[];
  traceId?: string;
}

export class ApiClient {
  private instance: AxiosInstance
  private baseURL: string

  constructor() {
    // ✅ FIX: Use empty base URL for proxy to handle /api routing
    this.baseURL = ''
    
    console.log('🔍 API CLIENT: Creating axios instance with baseURL:', this.baseURL || 'empty (using Vite proxy)');
    console.log('🔍 API CLIENT: Expected proxy config:');
    console.log('  - /api/auth/* → AuthService (port 7001)');
    console.log('  - /api/* → UserService (port 7002)');

    this.instance = axios.create({
      baseURL: this.baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json'
      }
    })

    this.setupInterceptors()
  }

  private setupInterceptors() {
    // Request interceptor - Add auth token (JWT now contains tenant context)
    this.instance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        const token = localStorage.getItem('auth_token')
        
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
          console.log('🔍 API REQUEST (with JWT):', config.method?.toUpperCase(), config.url)
        } else {
          console.log('🚨 API REQUEST (NO AUTH):', config.method?.toUpperCase(), config.url, '- No token found!')
        }
        
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor - Handle auth errors and API response unwrapping
    this.instance.interceptors.response.use(
      (response: AxiosResponse) => {
        console.log('✅ API RESPONSE:', response.config.url, response.status);
        
        // ✅ FIXED: Handle .NET 9 ApiResponseDto structure automatically
        const unwrapped = this.unwrapDotNetApiResponse(response);
        console.log('🔍 API CLIENT: Unwrapped response for', response.config.url, ':', unwrapped.data);
        return unwrapped;
      },
      async (error) => {
        console.error('🚨 API ERROR:', error.config?.url, error.response?.status, error.message);
        
        // ✅ Handle wrapped error responses from .NET 9
        if (error.response?.data && typeof error.response.data === 'object' && 'success' in error.response.data) {
          const apiError = error.response.data as DotNetApiResponse;
          if (!apiError.success) {
            // Create a more meaningful error with the backend's error message
            const enhancedError = new Error(apiError.message || 'API request failed');
            enhancedError.name = 'ApiError';
            (enhancedError as any).response = error.response;
            (enhancedError as any).status = error.response?.status;
            (enhancedError as any).errors = apiError.errors;
            console.error('🚨 API Error Details:', {
              message: apiError.message,
              errors: apiError.errors,
              traceId: apiError.traceId
            });
            throw enhancedError;
          }
        }
        
        const originalRequest = error.config;

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true;

          try {
            const refreshToken = localStorage.getItem('refresh_token');
            if (refreshToken) {
              console.log('🔄 Attempting token refresh...');
              const response = await this.post('/api/auth/refresh', { refreshToken });
              
              interface RefreshTokenResponse {
                accessToken: string;
              }

              const { accessToken } = response.data as RefreshTokenResponse;

              localStorage.setItem('auth_token', accessToken);
              originalRequest.headers.Authorization = `Bearer ${accessToken}`;

              console.log('✅ Token refreshed, retrying original request');
              return this.instance(originalRequest);
            }
          } catch (refreshError) {
            console.error('🚨 Token refresh failed:', refreshError);
            localStorage.removeItem('auth_token');
            localStorage.removeItem('refresh_token');
            
            // ✅ ENHANCED: Give user feedback before redirecting
            window.dispatchEvent(new CustomEvent('auth:logout', { 
              detail: { reason: 'refresh_failed', message: 'Session expired. Please log in again.' } 
            }));
          }
        }

        return Promise.reject(error);
      }
    )
  }

  // 🔧 SIMPLIFIED: No longer needed with JWT approach, but keep for compatibility
  setCurrentTenant(tenantId: string | null): void {
    if (tenantId) {
      console.log('🏢 API CLIENT: Tenant context set to:', tenantId, '(using JWT for tenant context)');
    } else {
      console.log('🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)');
    }
  }

  // ✅ FIXED: Properly handle .NET 9 ApiResponseDto<T> structure
  private unwrapDotNetApiResponse<T>(response: AxiosResponse): AxiosResponse<T> {
    const data = response.data;
    
    // Check if it's a .NET 9 ApiResponseDto structure
    if (data && typeof data === 'object' && 'success' in data && 'data' in data) {
      const apiResponse = data as DotNetApiResponse<T>;
      
      console.log('🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure:', {
        success: apiResponse.success,
        message: apiResponse.message,
        hasData: !!apiResponse.data,
        errors: apiResponse.errors?.length || 0
      });
      
      // If the API call was successful, unwrap the data
      if (apiResponse.success) {
        return {
          ...response,
          data: apiResponse.data
        };
      } else {
        // If API call failed, throw an error with the backend's message
        const error = new Error(apiResponse.message || 'API request failed');
        error.name = 'ApiError';
        (error as any).response = response;
        (error as any).status = response.status;
        (error as any).errors = apiResponse.errors;
        throw error;
      }
    }
    
    // Return as-is if not a wrapped response
    console.log('🔍 API CLIENT: Response not wrapped, returning as-is');
    return response;
  }

  // HTTP Methods
  async get<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.get(url, config);
  }

  async post<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.post(url, data, config);
  }

  async put<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.put(url, data, config);
  }

  async delete<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.delete(url, config);
  }
}

// Export singleton instance
export const apiClient = new ApiClient()
