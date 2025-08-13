import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'

export class ApiClient {
  private instance: AxiosInstance
  private baseURL: string

  constructor() {
    // ✅ FIX: Use empty base URL for proxy to handle /api routing
    this.baseURL = ''
    
    console.log('🔍 API CLIENT: Creating axios instance with baseURL:', this.baseURL || 'empty (using proxy)')

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
    // Request interceptor - Add auth token
    this.instance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        // ✅ FIX: Use the same key as TokenManager
        const token = localStorage.getItem('auth_token') // Changed from 'authToken' to 'auth_token'
        
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
          console.log('🔍 API REQUEST (with auth):', config.method?.toUpperCase(), config.url)
        } else {
          console.log('🚨 API REQUEST (NO AUTH):', config.method?.toUpperCase(), config.url, '- No token found!')
        }
        
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor - Handle auth errors
    this.instance.interceptors.response.use(
      (response: AxiosResponse) => {
        console.log('✅ API RESPONSE:', response.config.url, response.status)
        return response
      },
      async (error) => {
        console.error('🚨 API ERROR:', error.config?.url, error.response?.status, error.message)
        
        const originalRequest = error.config

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true

          try {
            // ✅ FIX: Use correct key for refresh token too
            const refreshToken = localStorage.getItem('refresh_token') // Changed from 'refreshToken' to 'refresh_token'
            if (refreshToken) {
              const response = await this.post('/api/auth/refresh', { refreshToken })
              const { accessToken } = response.data

              // ✅ FIX: Store with correct key
              localStorage.setItem('auth_token', accessToken) // Changed from 'authToken' to 'auth_token'
              originalRequest.headers.Authorization = `Bearer ${accessToken}`

              return this.instance(originalRequest)
            }
          } catch (refreshError) {
            console.error('🚨 Token refresh failed:', refreshError)
            // ✅ FIX: Clear with correct keys
            localStorage.removeItem('auth_token')
            localStorage.removeItem('refresh_token')
            
            // Dispatch custom event for auth logout
            window.dispatchEvent(new CustomEvent('auth:logout', { 
              detail: { reason: 'refresh_failed' } 
            }))
          }
        }

        return Promise.reject(error)
      }
    )
  }

  // Add this method to the ApiClient class
  private unwrapApiResponse<T>(response: AxiosResponse): AxiosResponse<T> {
    // If backend returns { success: true, data: {...} }, unwrap the data
    if (response.data && typeof response.data === 'object' && 'data' in response.data) {
      return {
        ...response,
        data: response.data.data
      };
    }
    return response;
  }

  // HTTP Methods
  async get<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    const response = await this.instance.get(url, config);
    return this.unwrapApiResponse<T>(response);
  }

  async post<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    const response = await this.instance.post(url, data, config);
    return this.unwrapApiResponse<T>(response);
  }

  async put<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    const response = await this.instance.put(url, data, config);
    return this.unwrapApiResponse<T>(response); // ✅ ADD THIS
  }

  async delete<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    const response = await this.instance.delete(url, config);
    return this.unwrapApiResponse<T>(response); // ✅ ADD THIS
  }
}

// Export singleton instance
export const apiClient = new ApiClient()
