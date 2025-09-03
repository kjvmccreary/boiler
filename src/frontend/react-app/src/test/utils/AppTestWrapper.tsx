/*************************************************************************************************
 * AppTestWrapper (TODO)
 *
 * PURPOSE:
 *  Unified provider composition for UI integration / operations tests once re-enabled.
 *
 * SHOULD WRAP (when implemented):
 *  - ThemeProvider (MUI theme used in app)
 *  - QueryClientProvider (react-query)
 *  - AuthProvider (seed seeded user + roles)
 *  - PermissionProvider (preload effective permissions)
 *  - TenantProvider (preselected tenant; skip network fetch)
 *  - Router (MemoryRouter with configurable initialEntries)
 *  - Toast / Notification provider (if components depend on it)
 *
 * OPTIONAL UTILITIES:
 *  - Props to inject custom user / permissions / tenant / initialRoute
 *  - Helper: renderApp(ui, overrides)
 *
 * TODO STEPS:
 *  1. Import real providers once stable.
 *  2. Expose a render helper that mirrors testing-library's render.
 *  3. Replace ad-hoc context mocks in operations tests with this wrapper.
 *  4. Re-enable operations test suites.
 *************************************************************************************************/

import React from 'react'

export interface AppTestWrapperOptions {
  initialRoute?: string
  // future: userOverride, tenantOverride, permissionsOverride, queryClientConfig, etc.
}

export const AppTestWrapper: React.FC<React.PropsWithChildren<AppTestWrapperOptions>> = ({
  children
}) => {
  // TEMP: pass-through until implemented
  return <>{children}</>
}

// Placeholder render helper (will be replaced by actual integrated render)
export function renderWithAppProviders(ui: React.ReactElement, _opts?: AppTestWrapperOptions) {
  // Intentionally not importing @testing-library here yet to avoid premature coupling.
  throw new Error(
    'renderWithAppProviders not implemented. See TODO in AppTestWrapper.tsx.'
  )
}
