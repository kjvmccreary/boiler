import { createContext, useContext, useEffect, useState, useRef, type ReactNode } from 'react';
import type { Tenant, TenantSettings } from '@/types/index.ts';
import { useAuth } from './AuthContext.tsx';
import { tenantService } from '@/services/tenant.service.ts';
import { apiClient } from '@/services/api.client.ts';

interface TenantContextType {
  currentTenant: Tenant | null;
  availableTenants: Tenant[];
  switchTenant: (tenantId: string) => Promise<void>;
  tenantSettings: TenantSettings;
  isLoading: boolean;
  error: string | null;
  showTenantSelector: boolean;
  setShowTenantSelector: (show: boolean) => void;
  completeTenantSelection: (tenantId: string) => Promise<void>;
  shouldRedirectToDashboard: boolean;
  clearRedirectFlag: () => void;
  refreshUserTenants: () => Promise<Tenant[]>;
}

const TenantContext = createContext<TenantContextType | undefined>(undefined);

interface TenantProviderProps {
  children: ReactNode;
  enableTestInstrumentation?: boolean;
}

export function TenantProvider({ children, enableTestInstrumentation }: TenantProviderProps) {
  const [currentTenant, setCurrentTenant] = useState<Tenant | null>(null);
  const [availableTenants, setAvailableTenants] = useState<Tenant[]>([]);
  const [tenantSettings, setTenantSettings] = useState<TenantSettings>({});
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [showTenantSelector, setShowTenantSelector] = useState(false);
  const [shouldRedirectToDashboard, setShouldRedirectToDashboard] = useState(false);

  const hasInitializedTenantsRef = useRef(false);
  const selectionInProgressRef = useRef(false);
  const lastUserIdRef = useRef<string | null>(null);

  const { user, isAuthenticated, refreshAuth } = useAuth();

  useEffect(() => {
    const handleTenantError = (event: CustomEvent) => {
      setError(`Tenant Error: ${event.detail.message}`);
    };
    window.addEventListener('tenant:error', handleTenantError as EventListener);
    return () => window.removeEventListener('tenant:error', handleTenantError as EventListener);
  }, []);

  useEffect(() => {
    if (isAuthenticated && user) {
      if (lastUserIdRef.current !== user.id) {
        hasInitializedTenantsRef.current = false;
        lastUserIdRef.current = user.id;
      }
      if (hasInitializedTenantsRef.current && currentTenant) return;
      void loadUserTenants();
    } else {
      clearTenantState();
      hasInitializedTenantsRef.current = false;
      lastUserIdRef.current = null;
    }
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [isAuthenticated, user]);

  const clearTenantState = () => {
    setCurrentTenant(null);
    setAvailableTenants([]);
    setTenantSettings({});
    setShowTenantSelector(false);
    setError(null);
    setShouldRedirectToDashboard(false);
    apiClient.setCurrentTenant(null);
  };

  const clearRedirectFlag = () => {
    setShouldRedirectToDashboard(false);
    sessionStorage.removeItem('tenant_switch_in_progress');
  };

  const isLikelyJwt = (token: string | null | undefined) =>
    !!token && token.split('.').length === 3;

  const getCurrentJwtTenantId = (): string | null => {
    try {
      const token = localStorage.getItem('auth_token');
      if (!isLikelyJwt(token)) return null;
      const payload = JSON.parse(atob(token!.split('.')[1]));
      return payload.tenant_id?.toString() || payload.tenantId?.toString() || null;
    } catch {
      return null;
    }
  };

  const loadUserTenants = async () => {
    if (!user) return;
    if (selectionInProgressRef.current) return;

    setIsLoading(true);
    setError(null);
    try {
      const response = await tenantService.getUserTenants(user.id);
      if (response?.success && Array.isArray(response.data) && response.data.length > 0) {
        setAvailableTenants(response.data);
        await handleTenantSelection(response.data);
        hasInitializedTenantsRef.current = true;
      } else if (response?.success && response.data?.length === 0) {
        setError('No tenants found for user');
      } else {
        setError('Unexpected response from server');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load tenants');
    } finally {
      setIsLoading(false);
    }
  };

  const handleTenantSelection = async (tenants: Tenant[]) => {
    if (selectionInProgressRef.current) return;
    if (currentTenant && tenants.some(t => t.id === currentTenant.id)) return;

    const jwtTenantId = getCurrentJwtTenantId();
    if (jwtTenantId) {
      const jwtTenant = tenants.find(t => t.id === jwtTenantId);
      if (jwtTenant) {
        selectionInProgressRef.current = true;
        try {
          await selectTenant(jwtTenant);
        } finally {
          selectionInProgressRef.current = false;
        }
        return;
      }
    }

    if (tenants.length === 1) {
      selectionInProgressRef.current = true;
      try {
        await callSelectTenantApi(tenants[0].id);
      } finally {
        selectionInProgressRef.current = false;
      }
    } else if (tenants.length > 1) {
      setShowTenantSelector(true);
    } else {
      setError('User has no tenant access');
    }
  };

  const callSelectTenantApi = async (tenantId: string) => {
    try {
      const response = await tenantService.selectTenant(tenantId);
      if (response?.success && response.data) {
        const tokenManager = await import('@/utils/token.manager.ts');
        tokenManager.tokenManager.setTokens(response.data.accessToken, response.data.refreshToken);
        await refreshAuth();
        const selectedTenant = availableTenants.find(t => t.id === tenantId);
        if (selectedTenant) await selectTenant(selectedTenant);
      }
    } catch {
      setError('Failed to select tenant');
      throw new Error('Failed to select tenant');
    }
  };

  const refreshUserTenants = async (): Promise<Tenant[]> => {
    if (!user) return [];
    try {
      const response = await tenantService.getUserTenants(user.id);
      if (response?.success && response.data?.length) {
        setAvailableTenants(response.data);
        return response.data;
      }
      setAvailableTenants([]);
      return [];
    } catch (error) {
      throw error;
    }
  };

  const switchTenant = async (tenantId: string) => {
    const jwtTenantId = getCurrentJwtTenantId();
    if (jwtTenantId === tenantId && currentTenant?.id === tenantId) return;

    let tenant = availableTenants.find(t => t.id === tenantId);
    if (!tenant && user) {
      const refreshed = await refreshUserTenants().catch(() => []);
      tenant = refreshed.find(t => t.id === tenantId);
    }
    if (!tenant) throw new Error(`Tenant ${tenantId} not found in available tenants`);

    setIsLoading(true);
    sessionStorage.setItem('tenant_switch_in_progress', 'true');
    selectionInProgressRef.current = true;
    try {
      const switchResponse = await tenantService.switchTenant(tenantId);
      if (switchResponse?.success && switchResponse.data) {
        await refreshAuth();
        await selectTenant(tenant);
        setShouldRedirectToDashboard(true);
      } else {
        throw new Error(switchResponse?.message || 'Tenant switch API failed');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to switch tenant');
      sessionStorage.removeItem('tenant_switch_in_progress');
      // Intentionally not rethrowing to avoid unhandled promise in tests
    } finally {
      selectionInProgressRef.current = false;
      setIsLoading(false);
    }
  };

  const selectTenant = async (tenant: Tenant) => {
    if (currentTenant?.id === tenant.id) return;
    setCurrentTenant(tenant);
    try {
      const settingsResponse = await tenantService.getTenantSettings(tenant.id);
      if (settingsResponse?.success) {
        setTenantSettings(settingsResponse.data || {});
      } else {
        setTenantSettings({});
      }
    } catch {
      setTenantSettings({});
    }
    if (user) {
      localStorage.setItem(`lastTenant_${user.id}`, tenant.id);
    }
    setShowTenantSelector(false);
    setError(null);
  };

  const completeTenantSelection = async (tenantId: string) => {
    try {
      selectionInProgressRef.current = true;
      await callSelectTenantApi(tenantId);
    } finally {
      selectionInProgressRef.current = false;
    }
  };

  const testMode = enableTestInstrumentation || process.env.NODE_ENV === 'test';

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
    shouldRedirectToDashboard,
    clearRedirectFlag,
    refreshUserTenants,
  };

  return (
    <TenantContext.Provider value={value}>
      {children}
      {testMode && (
        <div style={{ display: 'none' }} data-testid="tenant-context-probe">
          <span data-testid="probe-current-tenant">{currentTenant?.name || ''}</span>
          <span data-testid="probe-available-count">{availableTenants.length}</span>
          <span data-testid="probe-tenant-loading">{isLoading ? '1' : '0'}</span>
          <span data-testid="probe-tenant-error">{error || ''}</span>
          <span data-testid="probe-tenant-show-selector">{showTenantSelector ? '1' : '0'}</span>
          <span data-testid="probe-tenant-redirect">{shouldRedirectToDashboard ? '1' : '0'}</span>
        </div>
      )}
    </TenantContext.Provider>
  );
}

export function useTenant() {
  const context = useContext(TenantContext);
  if (context === undefined) {
    throw new Error('useTenant must be used within a TenantProvider');
  }
  return context;
}
