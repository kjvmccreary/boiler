import { useEffect, useState, useCallback } from 'react';
import { roleService } from '@/services/role.service';
import type { RoleDto } from '@/types';
import { useTenant } from '@/contexts/TenantContext';

let cachedRoles: RoleDto[] | null = null;
let cacheTenantId: string | number | null = null;
let inflight: Promise<RoleDto[]> | null = null;

export function useRolesCache() {
  const { currentTenant } = useTenant();
  const [roles, setRoles] = useState<RoleDto[]>(cachedRoles || []);
  const [loading, setLoading] = useState(!cachedRoles);
  const [error, setError] = useState<string | null>(null);

  const load = useCallback(async (force = false) => {
    if (!currentTenant) {
      setRoles([]);
      return;
    }
    if (!force && cachedRoles && cacheTenantId === currentTenant.id) {
      setRoles(cachedRoles);
      setLoading(false);
      return;
    }
    if (inflight) {
      const r = await inflight;
      setRoles(r);
      setLoading(false);
      return;
    }
    try {
      setLoading(true);
      inflight = (async () => {
        const resp = await roleService.getRoles({ page: 1, pageSize: 500 });
        const list = resp.roles || [];
        cachedRoles = list;
        cacheTenantId = currentTenant.id;
        return list;
      })();
      const result = await inflight;
      setRoles(result);
    } catch {
      setError('Failed to load roles');
      setRoles([]);
    } finally {
      setLoading(false);
      inflight = null;
    }
  }, [currentTenant]);

  useEffect(() => {
    load();
  }, [load]);

  return {
    roles,
    loading,
    error,
    reload: () => load(true)
  };
}
