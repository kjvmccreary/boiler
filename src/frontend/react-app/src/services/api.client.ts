import axios from 'axios';
import type { AxiosInstance, AxiosRequestConfig, AxiosResponse, AxiosError } from 'axios';
import type { ApiResponse } from '@/types/index.js';
import { tokenManager } from '@/utils/token.manager.js';

interface ExtendedAxiosRequestConfig extends AxiosRequestConfig {
  _retry?: boolean;
}

class ApiClient {
  private client: AxiosInstance;

  constructor() {
    // 🚨 NUCLEAR DEBUG: Log everything about environment
    console.log('🔍 API CLIENT CONSTRUCTOR - COMPLETE DEBUG:', {
      'import.meta.env.MODE': import.meta.env.MODE,
      'import.meta.env.DEV': import.meta.env.DEV,
      'import.meta.env.PROD': import.meta.env.PROD,
      'import.meta.env.VITE_API_BASE_URL': import.meta.env.VITE_API_BASE_URL,
      'window.location': {
        origin: window.location.origin,
        hostname: window.location.hostname,
        port: window.location.port,
        protocol: window.location.protocol
      },
      'All environment variables': import.meta.env
    });

    // Force empty baseURL for development to use Vite proxy
    const baseURL = '';
    
    console.log('🔍 API CLIENT: Creating axios instance with baseURL:', baseURL);

    this.client = axios.create({
      baseURL,
      timeout: 10000,
      headers: {
        'Content-Type': 'application/json',
      },
      withCredentials: false,
    });

    // 🚨 CRITICAL: Log the actual axios instance configuration
    console.log('🔍 AXIOS INSTANCE CONFIG:', {
      baseURL: this.client.defaults.baseURL,
      timeout: this.client.defaults.timeout,
      headers: this.client.defaults.headers,
      adapter: this.client.defaults.adapter
    });

    this.setupInterceptors();
  }

  private setupInterceptors() {
    // Request interceptor to add auth token
    this.client.interceptors.request.use(
      (config) => {
        const token = tokenManager.getToken();
        
        // 🚨 NUCLEAR DEBUG: Log EVERYTHING about the request
        console.log('🚨 REQUEST INTERCEPTOR - COMPLETE DEBUG:', {
          url: config.url,
          baseURL: config.baseURL,
          method: config.method,
          headers: config.headers,
          fullURL: `${config.baseURL || ''}${config.url}`,
          hasToken: !!token,
          tokenPreview: token ? `${token.substring(0, 20)}...` : 'none',
          adapter: config.adapter,
          timeout: config.timeout
        });
        
        if (token && config.headers) {
          config.headers.Authorization = `Bearer ${token}`;
        }
        
        // Add correlation ID for debugging
        if (config.headers) {
          config.headers['X-Correlation-ID'] = this.generateUUID();
        }
        
        // 🚨 LOG FINAL REQUEST CONFIG
        console.log('🚨 FINAL REQUEST CONFIG BEFORE SENDING:', {
          method: config.method,
          url: config.url,
          baseURL: config.baseURL,
          headers: Object.keys(config.headers || {}),
          hasAuthHeader: !!(config.headers?.Authorization)
        });
        
        return config;
      },
      (error) => {
        console.error('❌ Request interceptor error:', error);
        return Promise.reject(error);
      }
    );

    // Response interceptor for error handling and token refresh
    this.client.interceptors.response.use(
      (response: AxiosResponse) => {
        console.log(`✅ API Response: ${response.config.method?.toUpperCase()} ${response.config.url}`, {
          status: response.status,
          requestURL: response.request?.responseURL || 'unknown',
          data: response.data,
        });
        return response;
      },
      async (error: AxiosError) => {
        const originalRequest = error.config as ExtendedAxiosRequestConfig;
        
        // 🚨 NUCLEAR DEBUG: Log EVERYTHING about the error
        console.error(`🚨 API ERROR - COMPLETE DEBUG:`, {
          message: error.message,
          status: error.response?.status,
          requestMethod: originalRequest?.method,
          requestURL: originalRequest?.url,
          requestBaseURL: originalRequest?.baseURL,
          fullURL: `${originalRequest?.baseURL || ''}${originalRequest?.url}`,
          actualURL: error.request?.responseURL || 'unknown',
          headers: originalRequest?.headers,
          responseData: error.response?.data,
          requestStack: error.stack
        });

        // Handle 401 errors (unauthorized)
        if (error.response?.status === 401 && originalRequest && !originalRequest._retry) {
          originalRequest._retry = true;

          try {
            const refreshToken = tokenManager.getRefreshToken();
            if (refreshToken) {
              console.log('🔄 Attempting token refresh...');
              
              // Attempt to refresh token
              const response = await this.post<{ token: string; refreshToken: string }>('/api/auth/refresh', {
                refreshToken,
              });

              if (response.data.token && originalRequest.headers) {
                console.log('✅ Token refresh successful');
                tokenManager.setTokens(response.data.token, response.data.refreshToken);
                originalRequest.headers.Authorization = `Bearer ${response.data.token}`;
                return this.client(originalRequest);
              }
            }
          } catch (refreshError) {
            console.error('❌ Token refresh failed:', refreshError);
            
            tokenManager.clearTokens();
            
            // Dispatch a custom event that the AuthContext can listen to
            window.dispatchEvent(new CustomEvent('auth:logout', { 
              detail: { reason: 'token_refresh_failed' }
            }));
          }
        }

        return Promise.reject(error);
      }
    );
  }

  // UUID generator with fallback
  private generateUUID(): string {
    if (typeof crypto !== 'undefined' && crypto.randomUUID) {
      return crypto.randomUUID();
    }
    // Fallback UUID generation
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  }

  // Generic request method
  async request<T = unknown>(config: AxiosRequestConfig): Promise<ApiResponse<T>> {
    try {
      console.log('🔍 API CLIENT REQUEST METHOD CALLED:', {
        method: config.method,
        url: config.url,
        data: config.data
      });
      
      const response = await this.client.request<ApiResponse<T>>(config);
      return response.data;
    } catch (error) {
      console.error('🚨 API CLIENT REQUEST METHOD ERROR:', error);
      throw this.handleError(error as AxiosError);
    }
  }

  // HTTP methods
  async get<T = unknown>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    console.log('🔍 API CLIENT GET METHOD CALLED:', { url, config });
    return this.request<T>({ ...config, method: 'GET', url });
  }

  async post<T = unknown>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    console.log('🔍 API CLIENT POST METHOD CALLED:', { url, data, config });
    return this.request<T>({ ...config, method: 'POST', url, data });
  }

  async put<T = unknown>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    return this.request<T>({ ...config, method: 'PUT', url, data });
  }

  async patch<T = unknown>(url: string, data?: unknown, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    return this.request<T>({ ...config, method: 'PATCH', url, data });
  }

  async delete<T = unknown>(url: string, config?: AxiosRequestConfig): Promise<ApiResponse<T>> {
    return this.request<T>({ ...config, method: 'DELETE', url });
  }

  private handleError(error: AxiosError): Error {
    if (error.response) {
      // Server responded with error status
      const message = (error.response.data as { message?: string })?.message || 
                     error.response.statusText || 
                     'An error occurred';
      return new Error(message);
    } else if (error.request) {
      // Request was made but no response received
      return new Error('Network error: Please check your connection');
    } else {
      // Something else happened
      return new Error(error.message || 'An unexpected error occurred');
    }
  }
}

const apiClient = new ApiClient();
export { apiClient };
