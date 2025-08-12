import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import type { User, AuthResponse } from '@/types/index.js';
import { authService } from '@/services/auth.service.js';
import { tokenManager } from '@/utils/token.manager.js';

interface AuthState {
  user: User | null;
  permissions: string[];
  roles: string[]; // 🔧 Track multiple roles
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
  // 🔧 NEW: Testing support
  mockUser?: User;
  mockAuthState?: 'authenticated' | 'unauthenticated' | 'loading';
  testMode?: boolean;
}

export function AuthProvider({ 
  children, 
  mockUser,
  mockAuthState = 'unauthenticated',
  testMode = false
}: AuthProviderProps) {
  const [state, setState] = useState<AuthState>({
    user: null,
    permissions: [],
    roles: [], // 🔧 Initialize multiple roles
    isAuthenticated: false,
    isLoading: true,
    error: null,
  });

  // 🔧 NEW: Test mode initialization
  useEffect(() => {
    if (testMode) {
      // Initialize test state
      setState({
        user: mockUser || null,
        permissions: [], // Will be handled by PermissionContext
        roles: mockUser?.roles ? (Array.isArray(mockUser.roles) ? mockUser.roles : [mockUser.roles]) : [],
        isAuthenticated: mockAuthState === 'authenticated',
        isLoading: mockAuthState === 'loading',
        error: null,
      });
      return;
    }

    // Normal initialization for non-test mode
    initializeAuth();
    
    // Listen for auth events from API client
    const handleAuthLogout = (event: CustomEvent) => {
      console.log('🚪 Received auth logout event:', event.detail);
      logout();
    };

    window.addEventListener('auth:logout', handleAuthLogout as EventListener);
    
    return () => {
      window.removeEventListener('auth:logout', handleAuthLogout as EventListener);
    };
  }, [testMode, mockUser, mockAuthState]);

  // 🔧 .NET 9 MULTI-ROLE: Enhanced permission extraction from JWT token
  const getPermissionsFromToken = (token: string): string[] => {
    try {
      const claims = tokenManager.getTokenClaims(token);
      if (!claims) return [];
      
      // JWT permissions can be in different claim names for .NET 9
      const permissions = claims.permissions || claims.permission || claims.perms || 
                         claims['http://schemas.microsoft.com/identity/claims/role'] || 
                         [];
      
      console.log('🔍 AuthContext: Extracting permissions from token:', {
        tokenClaims: Object.keys(claims),
        permissions: permissions
      });
      
      // Handle both array and string formats
      if (Array.isArray(permissions)) {
        return permissions;
      } else if (typeof permissions === 'string') {
        return permissions.split(',').map(p => p.trim()).filter(p => p.length > 0);
      }
      
      return [];
    } catch (error) {
      console.error('🔍 AuthContext: Error extracting permissions from token:', error);
      return [];
    }
  };

  // 🔧 .NET 9 MULTI-ROLE FIX: Enhanced role extraction from JWT token
  const getRolesFromToken = (token: string): string[] => {
    try {
      const claims = tokenManager.getTokenClaims(token);
      if (!claims) return [];
      
      // Check multiple possible role claim names for .NET 9
      const roles = claims['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] ||
                   claims['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/role'] ||
                   claims.role || 
                   claims.roles || 
                   [];
      
      console.log('🔍 AuthContext: Extracting roles from JWT token:', {
        tokenClaims: Object.keys(claims),
        rolesRaw: roles,
        rolesType: typeof roles
      });
      
      // 🔧 MULTI-ROLE FIX: Handle single role string, array, or comma-separated string
      if (Array.isArray(roles)) {
        return roles.filter(role => role && role.length > 0);
      } else if (typeof roles === 'string' && roles.length > 0) {
        // Handle comma-separated roles in single string (some JWT implementations)
        return roles.split(',').map(r => r.trim()).filter(r => r.length > 0);
      }
      
      return [];
    } catch (error) {
      console.error('🔍 AuthContext: Error extracting roles from token:', error);
      return [];
    }
  };

  const initializeAuth = async () => {
    console.log('🔍 AuthContext: Initializing authentication...');
    
    try {
      const token = tokenManager.getToken();
      const refreshToken = tokenManager.getRefreshToken();
      
      console.log('🔍 AuthContext: Token check:', {
        hasToken: !!token,
        hasRefreshToken: !!refreshToken,
        tokenExpired: token ? tokenManager.isTokenExpired(token) : 'no-token'
      });
      
      if (!token) {
        console.log('🔍 AuthContext: No token found, user not authenticated');
        setState(prev => ({ ...prev, isLoading: false }));
        return;
      }

      // Check if token is expired
      if (tokenManager.isTokenExpired(token)) {
        console.log('🔍 AuthContext: Token expired, attempting refresh...');
        
        if (refreshToken) {
          try {
            const refreshResponse = await authService.refreshToken(refreshToken);
            console.log('✅ AuthContext: Token refresh successful');
            
            // Update tokens
            tokenManager.setTokens(refreshResponse.token, refreshResponse.refreshToken);
            
            // Validate the new token
            const user = await authService.validateToken();
            const permissions = getPermissionsFromToken(refreshResponse.token);
            const roles = getRolesFromToken(refreshResponse.token); // 🔧 Extract multiple roles
            
            setState(prev => ({
              ...prev,
              user,
              permissions,
              roles, // 🔧 Set multiple roles
              isAuthenticated: true,
              isLoading: false,
              error: null,
            }));
            return;
          } catch (refreshError) {
            console.error('❌ AuthContext: Token refresh failed:', refreshError);
            tokenManager.clearTokens();
            setState(prev => ({
              ...prev,
              user: null,
              permissions: [],
              roles: [], // 🔧 Clear roles
              isAuthenticated: false,
              isLoading: false,
              error: null,
            }));
            return;
          }
        } else {
          console.log('🔍 AuthContext: No refresh token available, clearing auth');
          tokenManager.clearTokens();
          setState(prev => ({ ...prev, isLoading: false }));
          return;
        }
      }

      // Token is valid, validate it
      console.log('🔍 AuthContext: Token valid, validating with backend...');
      const user = await authService.validateToken();
      const permissions = getPermissionsFromToken(token);
      const roles = getRolesFromToken(token); // 🔧 Extract multiple roles
      
      setState(prev => ({
        ...prev,
        user,
        permissions,
        roles, // 🔧 Set multiple roles
        isAuthenticated: true,
        isLoading: false,
        error: null,
      }));
      
      console.log('✅ AuthContext: Authentication initialization successful', {
        user: user?.email,
        permissions: permissions.slice(0, 10), // Show first 10
        roles
      });
      
    } catch (error) {
      console.error('❌ AuthContext: Auth initialization failed:', error);
      tokenManager.clearTokens();
      setState(prev => ({
        ...prev,
        user: null,
        permissions: [],
        roles: [], // 🔧 Clear roles
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
      
      // Store tokens
      tokenManager.setTokens(authResponse.accessToken, authResponse.refreshToken);
      
      // Get permissions and roles from token
      const permissions = getPermissionsFromToken(authResponse.accessToken);
      const roles = getRolesFromToken(authResponse.accessToken); // 🔧 Extract multiple roles
      
      setState(prev => ({
        ...prev,
        user: authResponse.user,
        permissions,
        roles, // 🔧 Set multiple roles
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
      
      // Store tokens
      tokenManager.setTokens(authResponse.accessToken, authResponse.refreshToken);
      
      // Get permissions and roles from token
      const permissions = getPermissionsFromToken(authResponse.accessToken);
      const roles = getRolesFromToken(authResponse.accessToken); // 🔧 Extract multiple roles
      
      setState(prev => ({
        ...prev,
        user: authResponse.user,
        permissions,
        roles, // 🔧 Set multiple roles
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
      if (!testMode) {
        await authService.logout();
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      if (!testMode) {
        tokenManager.clearTokens();
      }
      setState({
        user: null,
        permissions: [],
        roles: [], // 🔧 Clear multiple roles
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
    if (!testMode) {
      await initializeAuth();
    }
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
