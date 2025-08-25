import { createContext, useContext, useEffect, useState, type ReactNode } from 'react';
import type { Tenant, User, TenantSettings } from '@/types/index.ts';  // ✅ FIXED: .js → .ts
import { useAuth } from './AuthContext.tsx';  // ✅ FIXED: .js → .tsx
import { tenantService } from '@/services/tenant.service.ts';  // ✅ FIXED: .js → .ts
import { apiClient } from '@/services/api.client.ts';  // ✅ FIXED: .js → .ts

interface TenantContextType {
  currentTenant: Tenant | null;
  availableTenants: Tenant[];
  switchTenant: (tenantId: string) => Promise<void>;
  tenantSettings: TenantSettings;
  isLoading: boolean;
  error: string | null;
  // For login flow
  showTenantSelector: boolean;
  setShowTenantSelector: (show: boolean) => void;
  completeTenantSelection: (tenantId: string) => Promise<void>;
  // 🔧 NEW: Navigation flags for router-aware components
  shouldRedirectToDashboard: boolean;
  clearRedirectFlag: () => void;
  // 🔧 NEW: Add refresh method for external use
  refreshUserTenants: () => Promise<Tenant[]>;
}

const TenantContext = createContext<TenantContextType | undefined>(undefined);

interface TenantProviderProps {
  children: ReactNode;
}

export function TenantProvider({ children }: TenantProviderProps) {
  const [currentTenant, setCurrentTenant] = useState<Tenant | null>(null);
  const [availableTenants, setAvailableTenants] = useState<Tenant[]>([]);
  const [tenantSettings, setTenantSettings] = useState<TenantSettings>({});
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showTenantSelector, setShowTenantSelector] = useState(false);
  
  // 🔧 NEW: Navigation flags instead of direct navigation
  const [shouldRedirectToDashboard, setShouldRedirectToDashboard] = useState(false);
  
  const { user, isAuthenticated, refreshAuth } = useAuth();

  // Listen for tenant errors from API client
  useEffect(() => {
    const handleTenantError = (event: CustomEvent) => {
      console.error('🏢 TENANT ERROR:', event.detail);
      setError(`Tenant Error: ${event.detail.message}`);
    };

    window.addEventListener('tenant:error', handleTenantError as EventListener);
    
    return () => {
      window.removeEventListener('tenant:error', handleTenantError as EventListener);
    };
  }, []);

  // Load user's available tenants when authenticated
  useEffect(() => {
    if (isAuthenticated && user) {
      loadUserTenants();
    } else {
      // Clear tenant state when user logs out
      clearTenantState();
    }
  }, [isAuthenticated, user]);

  const clearTenantState = () => {
    setCurrentTenant(null);
    setAvailableTenants([]);
    setTenantSettings({});
    setShowTenantSelector(false);
    setError(null);
    setShouldRedirectToDashboard(false);
    
    // API client no longer needs tenant context (JWT approach)
    apiClient.setCurrentTenant(null);
  };

  const clearRedirectFlag = () => {
    setShouldRedirectToDashboard(false);
    sessionStorage.removeItem('tenant_switch_in_progress');
  };

  const getCurrentJwtTenantId = (): string | null => {
    try {
      const token = localStorage.getItem('auth_token');
      if (!token) {
        console.log('🔍 TenantContext: No token found for tenant extraction');
        return null;
      }
      
      const payload = JSON.parse(atob(token.split('.')[1]));
      const tenantId = payload.tenant_id?.toString();
      
      console.log('🔍 TenantContext: Extracted tenant from JWT:', {
        tenantId,
        tenantName: payload.tenant_name
      });
      
      return tenantId || null;
    } catch (error) {
      console.error('🔍 TenantContext: Error extracting tenant from JWT:', error);
      return null;
    }
  };

  // 🔧 FIXED: Corrected TypeScript types for page refresh detection
  const isPageRefresh = (): boolean => {
    // Check if we have tenant data in localStorage from previous selection
    // This indicates the user was previously using the app (page refresh scenario)
    const lastTenant = user ? localStorage.getItem(`lastTenant_${user.id}`) : null;
    
    let hasNavigationEntries = false;
    try {
      // 🔧 FIX: Proper TypeScript casting for PerformanceNavigationTiming
      if (window.performance) {
        if ('navigation' in window.performance && window.performance.navigation) {
          // Legacy API
          hasNavigationEntries = (window.performance.navigation as any).type === 1; // TYPE_RELOAD
        } else {
          // Modern API
          const navigationEntries = window.performance.getEntriesByType('navigation') as PerformanceNavigationTiming[];
          hasNavigationEntries = navigationEntries.length > 0 && navigationEntries[0].type === 'reload';
        }
      }
    } catch (error) {
      console.warn('🔍 TenantContext: Error checking navigation type:', error);
      hasNavigationEntries = false;
    }
    
    console.log('🔍 TenantContext: Page refresh detection:', {
      hasLastTenant: !!lastTenant,
      hasNavigationEntries,
      result: !!lastTenant || hasNavigationEntries
    });
    
    return !!lastTenant || hasNavigationEntries;
  };

  const loadUserTenants = async () => {
    if (!user) return;

    setIsLoading(true);
    setError(null);

    try {
      console.log('🏢 TenantContext: Loading tenants for user:', user.id);
      
      const response = await tenantService.getUserTenants(user.id);
      console.log('🏢 TenantContext: API Response:', response);
      
      if (response.success && response.data?.length > 0) {
        console.log('🏢 TenantContext: Setting available tenants:', response.data);
        setAvailableTenants(response.data);
        await handleTenantSelection(response.data);
      } else if (response.success && response.data?.length === 0) {
        console.log('🏢 TenantContext: User has no tenants');
        setError('No tenants found for user');
      } else {
        console.log('🏢 TenantContext: Unexpected response format:', response);
        setError('Unexpected response from server');
      }

    } catch (err) {
      console.error('🏢 TenantContext: Error loading tenants:', err);
      
      if (err instanceof Error) {
        console.error('🏢 TenantContext: Error message:', err.message);
        console.error('🏢 TenantContext: Error stack:', err.stack);
      }
      
      setError(err instanceof Error ? err.message : 'Failed to load tenants');
    } finally {
      setIsLoading(false);
    }
  };

  // Replace the handleTenantSelection method in your existing TenantContext
  const handleTenantSelection = async (tenants: Tenant[]) => {
    console.log('🏢 TenantContext: handleTenantSelection called with:', {
      tenantCount: tenants.length,
      tenants: tenants.map(t => ({ id: t.id, name: t.name }))
    });

    // 🔧 NEW LOGIC: Check JWT token for tenant FIRST
    const jwtTenantId = getCurrentJwtTenantId();
    
    if (jwtTenantId) {
      // JWT has tenant = this is a page refresh, auto-select
      const jwtTenant = tenants.find(t => t.id === jwtTenantId);
      if (jwtTenant) {
        console.log('🏢 TenantContext: Page refresh - JWT tenant found, auto-selecting:', jwtTenant.name);
        await selectTenant(jwtTenant);
        return;
      } else {
        console.warn('🏢 TenantContext: JWT tenant not found in available tenants:', {
          jwtTenantId,
          availableIds: tenants.map(t => t.id)
        });
      }
    }
    
    // 🔧 NEW LOGIC: No JWT tenant = fresh login, apply selection logic
    if (tenants.length === 1) {
      console.log('🏢 TenantContext: Single tenant, calling select-tenant API:', tenants[0].name);
      await callSelectTenantApi(tenants[0].id);
    } else if (tenants.length > 1) {
      console.log('🏢 TenantContext: Multiple tenants, showing selector');
      setShowTenantSelector(true);
    } else {
      setError('User has no tenant access');
    }
  };

  // 🔧 NEW METHOD: Call backend to complete login with tenant selection
  const callSelectTenantApi = async (tenantId: string) => {
    try {
      console.log('🏢 TenantContext: Calling select-tenant API for tenantId:', tenantId);
      
      const response = await tenantService.selectTenant(tenantId);
      if (response.success && response.data) {
        console.log('🏢 TenantContext: Select-tenant API successful, updating tokens...');
        
        // Update tokens with tenant-enabled JWT
        const tokenManager = await import('@/utils/token.manager.ts');
        tokenManager.tokenManager.setTokens(response.data.accessToken, response.data.refreshToken);
        
        // Refresh auth context to load new token
        console.log('🏢 TenantContext: Refreshing auth context with new token...');
        await refreshAuth();
        
        // Now select tenant in frontend
        const selectedTenant = availableTenants.find(t => t.id === tenantId);
        if (selectedTenant) {
          console.log('🏢 TenantContext: Setting selected tenant:', selectedTenant.name);
          await selectTenant(selectedTenant);
        }
      }
    } catch (error) {
      console.error('Failed to select tenant via API:', error);
      setError('Failed to select tenant');
      throw error;
    }
  };

  // Add a new method to refresh user tenants
  const refreshUserTenants = async (): Promise<Tenant[]> => {
    if (!user) return [];
    
    console.log('🔄 TenantContext: Refreshing user tenants...');
    
    try {
      const response = await tenantService.getUserTenants(user.id);
      if (response.success && response.data?.length > 0) {
        console.log('✅ TenantContext: Tenant list refreshed with', response.data.length, 'tenants');
        setAvailableTenants(response.data);
        return response.data;
      } else {
        console.log('⚠️ TenantContext: No tenants found after refresh');
        setAvailableTenants([]);
        return [];
      }
    } catch (error) {
      console.error('❌ TenantContext: Failed to refresh tenants:', error);
      throw error;
    }
  };

  const switchTenant = async (tenantId: string) => {
    // 🔧 FIX: Check if we're already on the requested tenant
    const jwtTenantId = getCurrentJwtTenantId();
    if (jwtTenantId === tenantId && currentTenant?.id === tenantId) {
      console.log('🏢 TenantContext: Already on requested tenant, skipping switch:', tenantId);
      return;
    }
    
    // 🔧 FIX: Wait a bit for availableTenants to be set if needed
    let tenant = availableTenants.find(t => t.id === tenantId);
    
    // If tenant not found, try to refresh the tenant list
    if (!tenant && user) {
      console.log('🔄 TenantContext: Tenant not found, refreshing tenant list...');
      try {
        const refreshedTenants = await refreshUserTenants();
        tenant = refreshedTenants.find(t => t.id === tenantId);
        
        if (tenant) {
          console.log('✅ TenantContext: Found tenant after refresh:', tenant.name);
        }
      } catch (error) {
        console.error('❌ TenantContext: Failed to refresh tenants during switch:', error);
      }
    }
    
    if (!tenant) {
      console.error('🏢 TenantContext: Tenant not found in available tenants:', {
        requestedId: tenantId,
        availableIds: availableTenants.map(t => t.id),
        availableTenantsCount: availableTenants.length
      });
      throw new Error(`Tenant ${tenantId} not found in available tenants`);
    }

    setIsLoading(true);
    
    // 🔧 NEW: Mark tenant switch in progress for navigation logic
    sessionStorage.setItem('tenant_switch_in_progress', 'true');
    
    try {
      console.log('🏢 TenantContext: Switching to tenant via JWT refresh:', tenant.name);
      console.log('🔍 TenantContext: Current auth token exists:', !!localStorage.getItem('auth_token'));
      console.log('🔍 TenantContext: Current refresh token exists:', !!localStorage.getItem('refresh_token'));
      
      // Call backend to get new JWT tokens for this tenant
      const switchResponse = await tenantService.switchTenant(tenantId);
      
      if (switchResponse.success && switchResponse.data) {
        console.log('✅ TenantContext: Tenant switch API successful');
        
        try {
          console.log('🔍 TenantContext: Calling refreshAuth to reload user state...');
          await refreshAuth();
          console.log('✅ TenantContext: refreshAuth completed successfully');
          
          // Update current tenant
          await selectTenant(tenant);
          
          console.log('🏢 TenantContext: Tenant switched successfully with new JWT:', tenant.name);
          
          // 🔧 NEW: Set flag for navigation instead of direct navigation
          setShouldRedirectToDashboard(true);
          
        } catch (refreshError) {
          console.error('🚨 TenantContext: refreshAuth failed:', refreshError);
          throw new Error(`Failed to refresh authentication: ${refreshError instanceof Error ? refreshError.message : 'Unknown error'}`);
        }
        
      } else {
        throw new Error(switchResponse.message || 'Tenant switch API failed - no data received');
      }
      
    } catch (err) {
      console.error('🚨 TenantContext: switchTenant failed:', err);
      setError(err instanceof Error ? err.message : 'Failed to switch tenant');
      
      // Clear the flag on error
      sessionStorage.removeItem('tenant_switch_in_progress');
      
      throw err;
    } finally {
      setIsLoading(false);
    }
  };

  const selectTenant = async (tenant: Tenant) => {
    console.log('🏢 TenantContext: Selecting tenant:', tenant.name);
    
    setCurrentTenant(tenant);
    
    // Load tenant-specific settings (but don't fail if it doesn't work)
    try {
      const settingsResponse = await tenantService.getTenantSettings(tenant.id);
      if (settingsResponse.success) {
        setTenantSettings(settingsResponse.data || {});
      }
    } catch (err) {
      console.warn('🏢 TenantContext: Failed to load tenant settings (this is OK):', err);
      // Don't fail tenant selection for settings
      setTenantSettings({});
    }

    // Remember user's tenant selection
    if (user) {
      localStorage.setItem(`lastTenant_${user.id}`, tenant.id);
    }

    // Hide tenant selector
    setShowTenantSelector(false);
    setError(null);
    
    console.log('🏢 TenantContext: Tenant selection complete:', tenant.name);
  };

  const completeTenantSelection = async (tenantId: string) => {
    console.log('🏢 TenantContext: Completing tenant selection for:', tenantId);

    try {
      // For initial tenant selection from selector dialog, call the API
      await callSelectTenantApi(tenantId);
    } catch (error) {
      console.error('Failed to complete tenant selection:', error);
      setError('Failed to select tenant');
      throw error;
    }
  };

  const value: TenantContextType = {
    currentTenant,
    availableTenants,
    switchTenant,
    tenantSettings,
    isLoading,
    error,
    showTenantSelector,
    setShowTenantSelector,
    completeTenantSelection,
    // 🔧 NEW: Navigation flags
    shouldRedirectToDashboard,
    clearRedirectFlag,
    // 🔧 NEW: Add refresh method for external use
    refreshUserTenants,
  };

  return <TenantContext.Provider value={value}>{children}</TenantContext.Provider>;
}

export function useTenant() {
  const context = useContext(TenantContext);
  if (context === undefined) {
    throw new Error('useTenant must be used within a TenantProvider');
  }
  return context;
}
