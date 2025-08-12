import axios, { AxiosInstance, InternalAxiosRequestConfig, AxiosResponse } from 'axios'

export class ApiClient {
  private instance: AxiosInstance
  private baseURL: string

  constructor() {
    // Fix: Ensure baseURL is properly set with fallback
    this.baseURL = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000/api'

    console.log('🔍 API CLIENT: Creating axios instance with baseURL:', this.baseURL)

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
        const token = localStorage.getItem('authToken')
        if (token) {
          config.headers.Authorization = `Bearer ${token}`
        }
        return config
      },
      (error) => Promise.reject(error)
    )

    // Response interceptor - Handle auth errors
    this.instance.interceptors.response.use(
      (response: AxiosResponse) => response,
      async (error) => {
        const originalRequest = error.config

        if (error.response?.status === 401 && !originalRequest._retry) {
          originalRequest._retry = true

          try {
            const refreshToken = localStorage.getItem('refreshToken')
            if (refreshToken) {
              const response = await this.post('/auth/refresh', { refreshToken })
              const { accessToken } = response.data

              localStorage.setItem('authToken', accessToken)
              originalRequest.headers.Authorization = `Bearer ${accessToken}`

              return this.instance(originalRequest)
            }
          } catch (refreshError) {
            localStorage.removeItem('authToken')
            localStorage.removeItem('refreshToken')
            window.location.href = '/login'
          }
        }

        return Promise.reject(error)
      }
    )
  }

  // HTTP Methods
  async get<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    return this.instance.get(url, config)
  }

  async post<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    return this.instance.post(url, data, config)
  }

  async put<T>(url: string, data?: any, config?: any): Promise<AxiosResponse<T>> {
    return this.instance.put(url, data, config)
  }

  async delete<T>(url: string, config?: any): Promise<AxiosResponse<T>> {
    return this.instance.delete(url, config)
  }
}

// Export singleton instance
export const apiClient = new ApiClient()
