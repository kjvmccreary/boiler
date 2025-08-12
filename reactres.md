PS C:\Users\mccre\dev\boiler\src\frontend\react-app> npm run build

> microservices-frontend@0.0.0 build
> tsc -b && vite build

src/test/utils/rbac-component-helpers.tsx:2:1 - error TS6133: 'userEvent' is declared but its value is never read.

2 import userEvent from '@testing-library/user-event'
  ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

src/test/utils/rbac-component-helpers.tsx:3:22 - error TS6133: 'rbacAssert' is declared but its value is never read.

3 import { rbacRender, rbacAssert, rbacUserEvent } from './rbac-test-utils.js'
                       ~~~~~~~~~~

src/test/utils/rbac-component-helpers.tsx:100:5 - error TS6133: 'requiredPermission' is declared but its value is never read.

100     requiredPermission: string,
        ~~~~~~~~~~~~~~~~~~

src/test/utils/rbac-component-helpers.tsx:101:5 - error TS6133: 'unauthorizedRedirect' is declared but its value is never read.

101     unauthorizedRedirect = '/unauthorized'
        ~~~~~~~~~~~~~~~~~~~~

src/test/utils/rbac-test-utils.tsx:123:11 - error TS2322: Type '{ children: ReactNode; mockUser: User; mockAuthState: "authenticated" | "unauthenticated" | "loading" | undefined; mockTenantId: string | undefined; testMode: true; }' is not assignable to type 'IntrinsicAttributes & AuthProviderProps'.
  Property 'mockTenantId' does not exist on type 'IntrinsicAttributes & AuthProviderProps'.

123           mockTenantId={config.tenantId}
              ~~~~~~~~~~~~

src/test/utils/rbac-test-utils.tsx:258:31 - error TS6133: 'route' is declared but its value is never read.

258   async expectRouteAccessible(route: string, shouldBeAccessible: boolean) {
                                  ~~~~~

src/test/utils/rbac-test-utils.tsx:258:46 - error TS6133: 'shouldBeAccessible' is declared but its value is never read.

258   async expectRouteAccessible(route: string, shouldBeAccessible: boolean) {
                                                 ~~~~~~~~~~~~~~~~~~


Found 7 errors.
