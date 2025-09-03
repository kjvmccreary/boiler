import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'

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
    this.baseURL = ''

    console.log('🔍 API CLIENT: Creating axios instance with baseURL:', this.baseURL || 'empty (using Vite proxy)');
    console.log('🔍 API CLIENT: Expected proxy config:');
    console.log('  - /api/auth/* → AuthService (port 7001)');
    console.log('  - /api/* → UserService (port 7002)');

    this.instance = axios.create({
      baseURL: this.baseURL,
      timeout: 10000,
      headers: { 'Content-Type': 'application/json' }
    })

    this.setupInterceptors()
  }

  private setupInterceptors() {
    this.instance.interceptors.request.use(
      (config: InternalAxiosRequestConfig) => {
        // Inject silent test token BEFORE logging to suppress "NO AUTH" noise
        if (import.meta.env.MODE === 'test') {
          config.headers = config.headers || {}
          if (!config.headers.Authorization) {
            config.headers.Authorization = 'Bearer test-token'
          }
        }

        // Inject tenant header in test mode (multi-tenant isolation tests)
        if (import.meta.env.MODE === 'test') {
          const testTenant = localStorage.getItem('test_tenant_id');
          if (testTenant) {
            config.headers['X-Tenant-Id'] = testTenant;
          }
        }

        const explicitToken = localStorage.getItem('auth_token')
        const effectiveToken =
          explicitToken ||
          (import.meta.env.MODE === 'test' ? 'test-token' : null)

        if (effectiveToken) {
          config.headers.Authorization = `Bearer ${effectiveToken}`
          if (import.meta.env.MODE !== 'test') {
            console.log('🔍 API REQUEST (auth):', config.method?.toUpperCase(), config.url)
          }
        } else {
          // Only log missing auth outside test mode
            console.log('🚨 API REQUEST (NO AUTH):', config.method?.toUpperCase(), config.url, '- No token found!')
        }

        return config
      },
      (error) => Promise.reject(error)
    )

    this.instance.interceptors.response.use(
      (response: AxiosResponse) => {
        if (import.meta.env.MODE !== 'test') {
          console.log('✅ API RESPONSE:', response.config.url, response.status)
        }
        const unwrapped = this.unwrapDotNetApiResponse(response)
        if (import.meta.env.MODE !== 'test') {
          console.log('🔍 API CLIENT: Unwrapped response for', response.config.url, ':', unwrapped.data)
        }
        return unwrapped
      },
      async (error) => {
        if (import.meta.env.MODE !== 'test') {
          console.error('🚨 API ERROR:', error.config?.url, error.response?.status, error.message)
        }

        if (error.response?.data && typeof error.response.data === 'object' && 'success' in error.response.data) {
          const apiError = error.response.data as DotNetApiResponse
          if (!apiError.success) {
            const enhancedError = new Error(apiError.message || 'API request failed')
            enhancedError.name = 'ApiError'
            ;(enhancedError as any).response = error.response
            ;(enhancedError as any).status = error.response?.status
            ;(enhancedError as any).errors = apiError.errors
            if (import.meta.env.MODE !== 'test') {
              console.error('🚨 API Error Details:', {
                message: apiError.message,
                errors: apiError.errors,
                traceId: apiError.traceId
              })
            }
            throw enhancedError
          }
        }

        const originalRequest = error.config
        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true
          try {
            const refreshToken = localStorage.getItem('refresh_token')
            if (refreshToken) {
              if (import.meta.env.MODE !== 'test') console.log('🔄 Attempting token refresh...')
              const response = await this.post('/api/auth/refresh', { refreshToken })
              interface RefreshTokenResponse { accessToken: string }
              const { accessToken } = response.data as RefreshTokenResponse
              localStorage.setItem('auth_token', accessToken)
              originalRequest.headers.Authorization = `Bearer ${accessToken}`
              if (import.meta.env.MODE !== 'test') console.log('✅ Token refreshed, retrying original request')
              return this.instance(originalRequest)
            }
          } catch (refreshError) {
            if (import.meta.env.MODE !== 'test') console.error('🚨 Token refresh failed:', refreshError)
            localStorage.removeItem('auth_token')
            localStorage.removeItem('refresh_token')
            window.dispatchEvent(new CustomEvent('auth:logout', {
              detail: { reason: 'refresh_failed', message: 'Session expired. Please log in again.' }
            }))
          }
        }

        return Promise.reject(error)
      }
    )
  }

  setCurrentTenant(tenantId: string | null): void {
    if (tenantId) {
      console.log('🏢 API CLIENT: Tenant context set to:', tenantId, '(using JWT for tenant context)')
    } else {
      console.log('🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)')
    }
  }

  private unwrapDotNetApiResponse<T>(response: AxiosResponse): AxiosResponse<T> {
    const data = response.data
    if (data && typeof data === 'object' && 'success' in data && 'data' in data) {
      const apiResponse = data as DotNetApiResponse<T>
      if (apiResponse.success) {
        return { ...response, data: apiResponse.data }
      } else {
        const error = new Error(apiResponse.message || 'API request failed')
        error.name = 'ApiError'
        ;(error as any).response = response
        ;(error as any).status = response.status
        ;(error as any).errors = apiResponse.errors
        throw error
      }
    }
    return response
  }

  async get<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.get(url, config)
  }
  async post<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.post(url, data, config)
  }
  async put<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.put(url, data, config)
  }
  async delete<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    return await this.instance.delete(url, config)
  }
}

export const apiClient = new ApiClient()
