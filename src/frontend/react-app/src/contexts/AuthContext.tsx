import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import type { User, AuthResponse } from '@/types/index.js';
import { authService } from '@/services/auth.service.js';
import { tokenManager } from '@/utils/token.manager.js';

interface AuthState {
  user: User | null;
  permissions: string[];
  isAuthenticated: boolean;
  isLoading: boolean;
  error: string | null;
}

interface AuthContextValue extends AuthState {
  login: (email: string, password: string) => Promise<void>;
  register: (userData: {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
  }) => Promise<void>;
  logout: () => Promise<void>;
  clearError: () => void;
  refreshAuth: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

interface AuthProviderProps {
  children: ReactNode;
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [state, setState] = useState<AuthState>({
    user: null,
    permissions: [],
    isAuthenticated: false,
    isLoading: true,
    error: null,
  });

  // Initialize authentication state on mount
  useEffect(() => {
    initializeAuth();
    
    // 🔧 FIX: Listen for auth events from API client
    const handleAuthLogout = (event: CustomEvent) => {
      console.log('🚪 Received auth logout event:', event.detail);
      logout();
    };

    window.addEventListener('auth:logout', handleAuthLogout as EventListener);
    
    return () => {
      window.removeEventListener('auth:logout', handleAuthLogout as EventListener);
    };
  }, []);

  // 🔧 NEW: Helper function to extract permissions from JWT token
  const getPermissionsFromToken = (token: string): string[] => {
    try {
      const claims = tokenManager.getTokenClaims(token);
      if (!claims) return [];
      
      // JWT permissions can be in different claim names
      const permissions = claims.permissions || claims.permission || claims.perms || [];
      
      console.log('🔍 AuthContext: Extracting permissions from token:', {
        tokenClaims: Object.keys(claims),
        permissions: permissions
      });
      
      // Handle both array and string formats
      if (Array.isArray(permissions)) {
        return permissions;
      } else if (typeof permissions === 'string') {
        // Sometimes permissions are comma-separated in a single string
        return permissions.split(',').map(p => p.trim()).filter(p => p.length > 0);
      }
      
      return [];
    } catch (error) {
      console.error('🔍 AuthContext: Error extracting permissions from token:', error);
      return [];
    }
  };

  const initializeAuth = async () => {
    try {
      const token = tokenManager.getToken();
      if (!token) {
        setState(prev => ({ ...prev, isLoading: false }));
        return;
      }

      // Validate token and get current user
      const user = await authService.validateToken();
      
      // 🔧 FIX: Get permissions from token instead of user.roles
      const permissions = getPermissionsFromToken(token);
      
      setState(prev => ({
        ...prev,
        user,
        permissions,
        isAuthenticated: true,
        isLoading: false,
        error: null,
      }));
    } catch (error) {
      console.error('Auth initialization failed:', error);
      tokenManager.clearTokens();
      setState(prev => ({
        ...prev,
        user: null,
        permissions: [],
        isAuthenticated: false,
        isLoading: false,
        error: null,
      }));
    }
  };

  const login = async (email: string, password: string) => {
    setState(prev => ({ ...prev, isLoading: true, error: null }));
    
    try {
      const authResponse: AuthResponse = await authService.login({ email, password });
      
      console.log('🔍 AuthContext: Login response received:', authResponse);
      
      // 🔧 FIX: Use correct property names from backend
      tokenManager.setTokens(authResponse.accessToken, authResponse.refreshToken);
      
      // 🔧 FIX: Get permissions from token instead of user.roles
      const permissions = getPermissionsFromToken(authResponse.accessToken);
      
      setState(prev => ({
        ...prev,
        user: authResponse.user,
        permissions,
        isAuthenticated: true,
        isLoading: false,
        error: null,
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Login failed';
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: errorMessage,
      }));
      throw error;
    }
  };

  const register = async (userData: {
    email: string;
    password: string;
    confirmPassword: string;
    firstName: string;
    lastName: string;
  }) => {
    setState(prev => ({ ...prev, isLoading: true, error: null }));
    
    try {
      const authResponse: AuthResponse = await authService.register(userData);
      
      // 🔧 FIX: Use correct property names from backend
      tokenManager.setTokens(authResponse.accessToken, authResponse.refreshToken);
      
      // 🔧 FIX: Get permissions from token instead of user.roles
      const permissions = getPermissionsFromToken(authResponse.accessToken);
      
      setState(prev => ({
        ...prev,
        user: authResponse.user,
        permissions,
        isAuthenticated: true,
        isLoading: false,
        error: null,
      }));
    } catch (error) {
      const errorMessage = error instanceof Error ? error.message : 'Registration failed';
      setState(prev => ({
        ...prev,
        isLoading: false,
        error: errorMessage,
      }));
      throw error;
    }
  };

  const logout = async () => {
    try {
      await authService.logout();
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      tokenManager.clearTokens();
      setState({
        user: null,
        permissions: [],
        isAuthenticated: false,
        isLoading: false,
        error: null,
      });
    }
  };

  const clearError = () => {
    setState(prev => ({ ...prev, error: null }));
  };

  const refreshAuth = async () => {
    await initializeAuth();
  };

  const value: AuthContextValue = {
    ...state,
    login,
    register,
    logout,
    clearError,
    refreshAuth,
  };

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
