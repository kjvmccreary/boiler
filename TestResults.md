
 RERUN  src/contexts/__tests__/PermissionContext.test.tsx x4

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:20:13)
🔍 API CLIENT: Creating axios instance with baseURL: empty (using proxy)

 ❯ src/contexts/__tests__/PermissionContext.test.tsx (60) 315ms
   ❯ PermissionContext (60) 314ms
     ❯ PermissionProvider (3)
       ✓ should render children without permission context
       × should provide permission context with authenticated user (retry x1)
       ✓ should use mock context in test mode
     ✓ usePermission hook (2)
       ✓ should throw error when used outside PermissionProvider
       ✓ should return permission context when used within provider
     ❯ Permission checking methods (9)
       ❯ hasPermission (5)
         ✓ should return false when user is not authenticated
         × should return true when user has the permission via JWT token (retry x1)
         ✓ should return false when user does not have the permission
         × should handle string permission format from token (retry x1)
         ✓ should handle empty permissions gracefully
       ❯ hasAnyPermission (2)
         × should return true when user has at least one permission (retry x1)
         ✓ should return false when user has none of the permissions
       ❯ hasAllPermissions (2)
         × should return true when user has all required permissions (retry x1)
         ✓ should return false when user is missing some permissions
     ✓ Role checking methods (9)
       ✓ hasRole (4)
         ✓ should return false when user is not authenticated
         ✓ should return true when user has the role
         ✓ should return false when user does not have the role
         ✓ should handle single role string format
       ✓ hasAnyRole (3)
         ✓ should return true when user has at least one of the roles
         ✓ should return false when user has none of the roles
         ✓ should return false when roles array is empty
       ✓ hasAllRoles (2)
         ✓ should return true when user has all required roles
         ✓ should return false when user is missing some roles
     ❯ Admin checking methods (16)
       ❯ isAdmin (4)
         ✓ should return false when user is not authenticated
         × should return true when user has admin permissions (retry x1)
         ✓ should return true when user has admin role
         ✓ should return false when user has neither admin permissions nor roles
       ❯ isSuperAdmin (3)
         ✓ should return true when user has SuperAdmin role
         × should return true when user has system.admin permission (retry x1)
         ✓ should return false when user is not SuperAdmin
       ✓ isSystemAdmin (3)
         ✓ should return true when user has SuperAdmin role
         ✓ should return true when user has SystemAdmin role
         ✓ should return false when user has neither role
       ❯ isTenantAdmin (2)
         ✓ should return true when user has Admin role
         × should return true when user has tenants.manage permission (retry x1)
       ❯ canManageUsers (2)
         × should return true when user has user management permissions (retry x1)
         ✓ should return false when user has no user management permissions
       ❯ canManageRoles (2)
         × should return true when user has role management permissions (retry x1)
         ✓ should return false when user has no role management permissions
     ❯ Data retrieval methods (14)
       ❯ getUserRoles (6)
         × should return roles from JWT token when available (retry x1)
         × should handle Microsoft role claim format (retry x1)
         × should handle comma-separated role string (retry x1)
         ✓ should fallback to user object roles when no token roles
         ✓ should handle single role string from user object
         ✓ should return empty array when no roles available
       ❯ getUserPermissions (3)
         × should return permissions from JWT token (retry x1)
         × should handle string permission format (retry x1)
         ✓ should return empty array when no permissions
       ✓ getEffectivePermissions (1)
         ✓ should return same as getUserPermissions
       ✓ getRoleHierarchy (4)
         ✓ should return correct hierarchy level for SuperAdmin
         ✓ should return correct hierarchy level for Admin
         ✓ should return highest role level when user has multiple roles
         ✓ should return maximum level for unknown roles
     ❯ Error handling and edge cases (4)
       ✓ should handle token manager errors gracefully
       × should handle malformed JWT claims gracefully (retry x1)
       ✓ should handle null user gracefully
       ✓ should handle complex role object arrays from API
     ✓ Console logging (3)
       ✓ should log permission checks for debugging
       ✓ should log role extraction process
       ✓ should log admin checks

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Failed Tests 16 ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > PermissionProvider > should provide permission context with authenticated user
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > PermissionProvider > should provide permission context with authenticated user
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  true
Received:
  false
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:149:52
    147|       )
    148|
    149|       expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
       |                                                    ^
    150|       expect(screen.getByTestId('user-roles')).toHaveTextContent('User')
    151|     })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[1/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasPermission > should return true when user has the permission via JWT token
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasPermission > should return true when user has the permission via JWT token
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  true
Received:
  false
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:242:54
    240|         )
    241|
    242|         expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
       |                                                      ^
    243|       })
    244|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[2/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasPermission > should handle string permission format from token
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasPermission > should handle string permission format from token
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  true
Received:
  false
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:272:54
    270|         )
    271|
    272|         expect(screen.getByTestId('has-users-read')).toHaveTextContent('true')
       |                                                      ^
    273|       })
    274|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[3/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasAnyPermission > should return true when user has at least one permission
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasAnyPermission > should return true when user has at least one permission
AssertionError: expected false to be true // Object.is equality

- Expected
+ Received

- true
+ false

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:308:24
    306|
    307|         const hasAny = permissionContext?.hasAnyPermission(['users.read', 'admin.access'])
    308|         expect(hasAny).toBe(true)
       |                        ^
    309|       })
    310|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[4/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasAllPermissions > should return true when user has all required permissions
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Permission checking methods > hasAllPermissions > should return true when user has all required permissions
AssertionError: expected false to be true // Object.is equality

- Expected
+ Received

- true
+ false

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:348:24
    346|
    347|         const hasAll = permissionContext?.hasAllPermissions(['users.read', 'users.edit'])
    348|         expect(hasAll).toBe(true)
       |                        ^
    349|       })
    350|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[5/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > isAdmin > should return true when user has admin permissions
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > isAdmin > should return true when user has admin permissions
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  true
Received:
  false
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:520:48
    518|         )
    519|
    520|         expect(screen.getByTestId('is-admin')).toHaveTextContent('true')
       |                                                ^
    521|       })
    522|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[6/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > isSuperAdmin > should return true when user has system.admin permission
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > isSuperAdmin > should return true when user has system.admin permission
AssertionError: expected false to be true // Object.is equality

- Expected
+ Received

- true
+ false

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:578:51
    576|         )
    577|
    578|         expect(permissionContext?.isSuperAdmin()).toBe(true)
       |                                                   ^
    579|       })
    580|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[7/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > isTenantAdmin > should return true when user has tenants.manage permission
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > isTenantAdmin > should return true when user has tenants.manage permission
AssertionError: expected false to be true // Object.is equality

- Expected
+ Received

- true
+ false

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:665:52
    663|         )
    664|
    665|         expect(permissionContext?.isTenantAdmin()).toBe(true)
       |                                                    ^
    666|       })
    667|     })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[8/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > canManageUsers > should return true when user has user management permissions
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > canManageUsers > should return true when user has user management permissions
AssertionError: expected false to be true // Object.is equality

- Expected
+ Received

- true
+ false

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:685:53
    683|         )
    684|
    685|         expect(permissionContext?.canManageUsers()).toBe(true)
       |                                                     ^
    686|       })
    687|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[9/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > canManageRoles > should return true when user has role management permissions
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Admin checking methods > canManageRoles > should return true when user has role management permissions
AssertionError: expected false to be true // Object.is equality

- Expected
+ Received

- true
+ false

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:723:53
    721|         )
    722|
    723|         expect(permissionContext?.canManageRoles()).toBe(true)
       |                                                     ^
    724|       })
    725|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[10/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserRoles > should return roles from JWT token when available
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserRoles > should return roles from JWT token when available
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  Admin,Manager
Received:
  User
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:762:50
    760|
    761|         // Should show JWT token roles, not user object roles
    762|         expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin,Manager')
       |                                                  ^
    763|       })
    764|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[11/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserRoles > should handle Microsoft role claim format
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserRoles > should handle Microsoft role claim format
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  Admin
Received:
  User
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:777:50
    775|         )
    776|
    777|         expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin')
       |                                                  ^
    778|       })
    779|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[12/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserRoles > should handle comma-separated role string
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserRoles > should handle comma-separated role string
Error: expect(element).toHaveTextContent()

Expected element to have text content:
  Admin,Manager,SuperAdmin
Received:
  User
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:792:50
    790|         )
    791|
    792|         expect(screen.getByTestId('user-roles')).toHaveTextContent('Admin,Manager,SuperAdmin')
       |                                                  ^
    793|       })
    794|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[13/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserPermissions > should return permissions from JWT token
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserPermissions > should return permissions from JWT token
AssertionError: expected '' to contain 'users.read'

- Expected
+ Received

- users.read

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:849:29
    847|
    848|         const permissions = screen.getByTestId('user-permissions').textContent
    849|         expect(permissions).toContain('users.read')
       |                             ^
    850|         expect(permissions).toContain('users.edit')
    851|         expect(permissions).toContain('profile.read')

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[14/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserPermissions > should handle string permission format
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Data retrieval methods > getUserPermissions > should handle string permission format
AssertionError: expected '' to contain 'users.read'

- Expected
+ Received

- users.read

 ❯ src/contexts/__tests__/PermissionContext.test.tsx:867:29
    865|
    866|         const permissions = screen.getByTestId('user-permissions').textContent
    867|         expect(permissions).toContain('users.read')
       |                             ^
    868|         expect(permissions).toContain('users.edit')
    869|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[15/32]⎯

 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Error handling and edge cases > should handle malformed JWT claims gracefully
 FAIL  src/contexts/__tests__/PermissionContext.test.tsx > PermissionContext > Error handling and edge cases > should handle malformed JWT claims gracefully
Error: expect(element).toBeEmptyDOMElement()

Received:
  "User"
 ❯ src/contexts/__tests__/PermissionContext.test.tsx:991:48
    989|
    990|       expect(screen.getByTestId('user-permissions')).toBeEmptyDOMElement()
    991|       expect(screen.getByTestId('user-roles')).toBeEmptyDOMElement()
       |                                                ^
    992|     })
    993|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[16/32]⎯

 Test Files  1 failed (1)
      Tests  16 failed | 44 passed (60)
   Start at  20:35:47
   Duration  1.01s

