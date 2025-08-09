import { apiClient } from './api.client.js';
import { API_ENDPOINTS } from '@/utils/api.constants.js';
import type { 
  LoginRequest, 
  RegisterRequest, 
  AuthResponse,
  User
} from '@/types/index.js';

export class AuthService {
  async login(credentials: LoginRequest): Promise<AuthResponse> {
    console.log('🔍 AuthService: Attempting login...');
    const response = await apiClient.post<AuthResponse>(
      API_ENDPOINTS.AUTH.LOGIN, 
      credentials
    );
    
    console.log('🔍 AuthService: Raw login response:', response);
    console.log('🔍 AuthService: Response data structure:', {
      hasData: !!response.data,
      dataKeys: response.data ? Object.keys(response.data) : [],
      accessToken: response.data?.accessToken ? 'present' : 'missing',
      refreshToken: response.data?.refreshToken ? 'present' : 'missing',
      user: response.data?.user ? 'present' : 'missing'
    });
    
    return response.data;
  }

  async register(userData: RegisterRequest): Promise<AuthResponse> {
    const response = await apiClient.post<AuthResponse>(
      API_ENDPOINTS.AUTH.REGISTER, 
      userData
    );
    return response.data;
  }

  async logout(): Promise<void> {
    await apiClient.post(API_ENDPOINTS.AUTH.LOGOUT);
  }

  async refreshToken(refreshToken: string): Promise<{ token: string; refreshToken: string }> {
    const response = await apiClient.post<{ token: string; refreshToken: string }>(
      API_ENDPOINTS.AUTH.REFRESH,
      { refreshToken }
    );
    return response.data;
  }

  async changePassword(oldPassword: string, newPassword: string): Promise<void> {
    await apiClient.post(API_ENDPOINTS.AUTH.CHANGE_PASSWORD, {
      oldPassword,
      newPassword,
    });
  }

  async forgotPassword(email: string): Promise<void> {
    await apiClient.post(API_ENDPOINTS.AUTH.FORGOT_PASSWORD, { email });
  }

  async resetPassword(token: string, newPassword: string): Promise<void> {
    await apiClient.post(API_ENDPOINTS.AUTH.RESET_PASSWORD, {
      token,
      newPassword,
    });
  }

  async confirmEmail(token: string): Promise<void> {
    await apiClient.post(API_ENDPOINTS.AUTH.CONFIRM_EMAIL, { token });
  }

  // 🔧 FIX: Use the /api/users/profile endpoint instead
  async validateToken(): Promise<User> {
    console.log('🔍 AuthService: Validating token using /api/users/profile...');
    const response = await apiClient.get<User>(API_ENDPOINTS.USERS.PROFILE);
    console.log('✅ AuthService: Token validation successful:', response.data);
    return response.data;
  }

  async getCurrentUser(): Promise<User> {
    const response = await apiClient.get<User>(API_ENDPOINTS.USERS.PROFILE);
    return response.data;
  }
}

export const authService = new AuthService();
