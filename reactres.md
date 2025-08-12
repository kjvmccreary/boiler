PS C:\Users\mccre\dev\boiler\src\frontend\react-app> npm run build

> microservices-frontend@0.0.0 build
> tsc -b && vite build

src/test/utils/test-utils.tsx:64:15 - error TS2352: Conversion of type '{ id: number; name: string; description: string; isSystemRole: true; isDefault: false; tenantId: null; permissions: Permission[]; createdAt: string; updatedAt: string; userCount: number; }' to type 'Role' may be a mistake because neither type sufficiently overlaps with the other. If this was intentional, convert the expression to 'unknown' first.
  Types of property 'tenantId' are incompatible.
    Type 'null' is not comparable to type 'number | undefined'.

 64   superAdmin: {
                  ~
 65     id: 1,
    ~~~~~~~~~~
...
 80     userCount: 1
    ~~~~~~~~~~~~~~~~
 81   } as Role,
    ~~~~~~~~~~~

src/test/utils/test-utils.tsx:83:16 - error TS2352: Conversion of type '{ id: number; name: string; description: string; isSystemRole: true; isDefault: false; tenantId: null; permissions: Permission[]; createdAt: string; updatedAt: string; userCount: number; }' to type 'Role' may be a mistake because neither type sufficiently overlaps with the other. If this was intentional, convert the expression to 'unknown' first.
  Types of property 'tenantId' are incompatible.
    Type 'null' is not comparable to type 'number | undefined'.

 83   systemAdmin: {
                   ~
 84     id: 2,
    ~~~~~~~~~~
...
 98     userCount: 2
    ~~~~~~~~~~~~~~~~
 99   } as Role,
    ~~~~~~~~~~~

src/test/utils/test-utils.tsx:382:11 - error TS2322: Type '{ children: Element; mockUser: User; mockAuthState: "authenticated" | "unauthenticated" | "loading"; mockTenantId: string; }' is not assignable to type 'IntrinsicAttributes & AuthProviderProps'.
  Property 'mockUser' does not exist on type 'IntrinsicAttributes & AuthProviderProps'.

382           mockUser={resolvedMockUser}
              ~~~~~~~~

src/test/utils/test-utils.tsx:386:31 - error TS2322: Type '{ children: ReactNode; mockContext: { hasPermission: (permission: string) => boolean; hasRole: (roleName: string) => boolean; hasAnyRole: (roleNames: string[]) => boolean; hasAllRoles: (roleNames: string[]) => boolean; ... 9 more ...; getRoleHierarchy?: undefined; } | { ...; }; }' is not assignable to type 'IntrinsicAttributes & PermissionProviderProps'.
  Property 'mockContext' does not exist on type 'IntrinsicAttributes & PermissionProviderProps'.

386           <PermissionProvider mockContext={mockPermissionContext}>
                                  ~~~~~~~~~~~

src/test/utils/test-utils.tsx:503:11 - error TS6133: 'screen' is declared but its value is never read.

503     const { screen } = await import('@testing-library/react')
              ~~~~~~~~~~


Found 5 errors.
