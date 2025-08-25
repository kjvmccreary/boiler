PS C:\Users\mccre\dev\boiler\src\frontend\react-app> npm run build

> microservices-frontend@0.0.0 build
> tsc -b && vite build

src/test/examples/rbac-usage-examples.test.tsx:4:10 - error TS2305: Module '"../utils/test-utils"' has no exported member 'createMockPermissionContext'.

4 import { createMockPermissionContext } from '../utils/test-utils'  // ✅ Fixed: Use relative path
           ~~~~~~~~~~~~~~~~~~~~~~~~~~~

src/test/mocks/handlers.ts:3:10 - error TS2305: Module '"../utils/test-utils"' has no exported member 'mockUsers'.

3 import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils'
           ~~~~~~~~~

src/test/mocks/handlers.ts:3:21 - error TS2305: Module '"../utils/test-utils"' has no exported member 'mockRoles'.

3 import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils'
                      ~~~~~~~~~

src/test/mocks/handlers.ts:3:32 - error TS2305: Module '"../utils/test-utils"' has no exported member 'mockPermissions'.

3 import { mockUsers, mockRoles, mockPermissions } from '../utils/test-utils'
                                 ~~~~~~~~~~~~~~~

src/test/mocks/handlers.ts:32:3 - error TS2698: Spread types may only be created from object types.

32   ...user,
     ~~~~~~~

src/test/mocks/handlers.ts:33:16 - error TS18046: 'user' is of type 'unknown'.

33   phoneNumber: user.phoneNumber ?? undefined,
                  ~~~~

src/test/mocks/handlers.ts:34:13 - error TS18046: 'user' is of type 'unknown'.

34   timeZone: user.timeZone ?? 'UTC',
               ~~~~

src/test/mocks/handlers.ts:35:13 - error TS18046: 'user' is of type 'unknown'.

35   language: user.language ?? 'en',
               ~~~~

src/test/mocks/handlers.ts:36:16 - error TS18046: 'user' is of type 'unknown'.

36   lastLoginAt: user.lastLoginAt ?? undefined
                  ~~~~

src/test/scenarios/role-hierarchy-scenarios.test.tsx:4:10 - error TS2305: Module '"../utils/test-utils"' has no exported member 'mockRoles'.

4 import { mockRoles, createMockPermissionContext, type MockRoleType } from '../utils/test-utils' // ✅ Fixed: Remove .js
           ~~~~~~~~~

src/test/scenarios/role-hierarchy-scenarios.test.tsx:4:21 - error TS2305: Module '"../utils/test-utils"' has no exported member 'createMockPermissionContext'.

4 import { mockRoles, createMockPermissionContext, type MockRoleType } from '../utils/test-utils' // ✅ Fixed: Remove .js
                      ~~~~~~~~~~~~~~~~~~~~~~~~~~~

src/test/scenarios/role-hierarchy-scenarios.test.tsx:4:55 - error TS2305: Module '"../utils/test-utils"' has no exported member 'MockRoleType'.

4 import { mockRoles, createMockPermissionContext, type MockRoleType } from '../utils/test-utils' // ✅ Fixed: Remove .js
                                                        ~~~~~~~~~~~~

src/test/utils/rbac-test-utils.tsx:9:10 - error TS2305: Module '"./test-utils"' has no exported member 'mockUsers'.

9 import { mockUsers, mockRoles, createMockPermissionContext, type MockRoleType } from './test-utils'  // ✅ Fixed: Remove .js
           ~~~~~~~~~

src/test/utils/rbac-test-utils.tsx:9:21 - error TS2305: Module '"./test-utils"' has no exported member 'mockRoles'.

9 import { mockUsers, mockRoles, createMockPermissionContext, type MockRoleType } from './test-utils'  // ✅ Fixed: Remove .js
                      ~~~~~~~~~

src/test/utils/rbac-test-utils.tsx:9:32 - error TS2305: Module '"./test-utils"' has no exported member 'createMockPermissionContext'.

9 import { mockUsers, mockRoles, createMockPermissionContext, type MockRoleType } from './test-utils'  // ✅ Fixed: Remove .js
                                 ~~~~~~~~~~~~~~~~~~~~~~~~~~~

src/test/utils/rbac-test-utils.tsx:9:66 - error TS2305: Module '"./test-utils"' has no exported member 'MockRoleType'.

9 import { mockUsers, mockRoles, createMockPermissionContext, type MockRoleType } from './test-utils'  // ✅ Fixed: Remove .js
                                                                   ~~~~~~~~~~~~


Found 16 errors.

PS C:\Users\mccre\dev\boiler\src\frontend\react-app>
