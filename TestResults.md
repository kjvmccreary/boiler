PS C:\Users\mccre\dev\boiler> dotnet test
Restore complete (0.5s)
  DTOs succeeded (0.2s) → src\shared\DTOs\bin\Debug\net9.0\DTOs.dll
  Contracts succeeded (0.1s) → src\shared\Contracts\bin\Debug\net9.0\Contracts.dll
  Common succeeded (0.1s) → src\shared\Common\bin\Debug\net9.0\Common.dll
  UserService succeeded (0.1s) → src\services\UserService\bin\Debug\net9.0\UserService.dll
  RoleService.Tests succeeded (0.1s) → tests\unit\RoleService.Tests\bin\Debug\net9.0\RoleService.Tests.dll
  PermissionService.Tests succeeded (0.1s) → tests\unit\PermissionService.Tests\bin\Debug\net9.0\PermissionService.Tests.dll
  AuthService succeeded (0.2s) → src\services\AuthService\bin\Debug\net9.0\AuthService.dll
  ApiGateway succeeded (0.2s) → src\services\ApiGateway\bin\Debug\net9.0\ApiGateway.dll
  UserService.Tests succeeded (0.1s) → tests\unit\UserService.Tests\bin\Debug\net9.0\UserService.Tests.dll
  UserService.IntegrationTests succeeded (0.1s) → tests\integration\UserService.IntegrationTests\bin\Debug\net9.0\UserService.IntegrationTests.dll
  ApiGateway.Tests succeeded (0.1s) → tests\unit\ApiGateway.Tests\bin\Debug\net9.0\ApiGateway.Tests.dll
  AuthService.Tests succeeded (0.2s) → tests\unit\AuthService.Tests\bin\Debug\net9.0\AuthService.Tests.dll
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.04]   Discovering: PermissionService.Tests
[xUnit.net 00:00:00.05]   Discovering: RoleService.Tests
[xUnit.net 00:00:00.06]   Discovered:  PermissionService.Tests
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.06]   Starting:    PermissionService.Tests
[xUnit.net 00:00:00.07]   Discovered:  RoleService.Tests
[xUnit.net 00:00:00.07]   Starting:    RoleService.Tests
[xUnit.net 00:00:00.05]   Discovering: UserService.Tests
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.06]   Discovered:  UserService.Tests
[xUnit.net 00:00:00.06]   Starting:    UserService.Tests
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.07]   Discovering: UserService.IntegrationTests
[xUnit.net 00:00:00.08]   Discovered:  UserService.IntegrationTests
[xUnit.net 00:00:00.09]   Starting:    UserService.IntegrationTests
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.06]   Discovering: ApiGateway.Tests
[xUnit.net 00:00:00.06]   Discovering: AuthService.Tests
[xUnit.net 00:00:00.08]   Discovered:  ApiGateway.Tests
[xUnit.net 00:00:00.08]   Starting:    ApiGateway.Tests
[xUnit.net 00:00:00.08]   Discovered:  AuthService.Tests
[xUnit.net 00:00:00.09]   Starting:    AuthService.Tests
[xUnit.net 00:00:00.29]   Finished:    UserService.Tests
  UserService.Tests test succeeded (0.8s)
[18:53:37 INF] Starting UserService {}
[18:53:37 INF] Starting UserService {}
[18:53:37 INF] Starting UserService {}
=== UserService Starting ===
=== UserService Starting ===
=== UserService Starting ===
[xUnit.net 00:00:00.61]   Finished:    PermissionService.Tests
[18:53:38 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
[18:53:38 WRN] The foreign key property 'UserRole.UserId1' was created in shadow state because a conflicting property with the simple name 'UserId' exists in the entity type, but is either not mapped, is already used for another relationship, or is incompatible with the associated primary key type. See https://aka.ms/efcore-relationships for information on mapping relationships in EF Core. {"EventId": {"Id": 10625, "Name": "Microsoft.EntityFrameworkCore.Model.Validation.ShadowForeignKeyPropertyCreated"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
  PermissionService.Tests test succeeded (1.1s)
[xUnit.net 00:00:00.73]   Finished:    RoleService.Tests
  RoleService.Tests test succeeded (1.2s)
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[18:53:38 INF] ? Admin user has 13 permissions: users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ? Generated JWT token successfully for admin user {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNEP695FD7N6", "RequestPath": "/api/users/3"}
[18:53:38 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNEP695FD7N7", "RequestPath": "/api/roles"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=7, Permissions=13, UserRoles=5, RolePermissions=44 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=7, Permissions=13, UserRoles=5, RolePermissions=44 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=7, Permissions=13, UserRoles=5, RolePermissions=44 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=7, Permissions=13, UserRoles=5, RolePermissions=44 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=8, Permissions=13, UserRoles=5, RolePermissions=46 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=8, Permissions=13, UserRoles=5, RolePermissions=36 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] Starting UserService {}
[18:53:38 INF] HTTP PUT /api/users/profile responded 400 in 15.4628 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
=== UserService Starting ===
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEP695FD7NT", "RequestPath": "/api/users"}
[18:53:38 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEP695FD7NT", "RequestPath": "/api/users"}
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant2.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant2.com:
   - User ID: 4
   - User TenantId: 2
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=5, TenantId=2, Active=True
?? TENANT DEBUG:
   - Tenant ID: 2
   - Tenant Name: Test Tenant 2
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=5, RoleName=Admin
?? USER admin@tenant2.com in TENANT 2:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant2.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 2 (ID: 2)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant2.com:
   - Role: Admin
   - Tenant: Test Tenant 2 (ID: 2)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for manager@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for manager@tenant1.com:
   - User ID: 3
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=4, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=4, RoleName=Manager
?? USER manager@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 3 [users.edit, users.view, roles.view]
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager (Actual: Manager)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 3
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 3
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for manager@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for manager@tenant1.com:
   - User ID: 3
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=4, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=4, RoleName=Manager
?? USER manager@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 3 [users.edit, users.view, roles.view]
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager (Actual: Manager)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 3
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 3
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[xUnit.net 00:00:01.42]     UserService.IntegrationTests.Controllers.UsersControllerTests.GetUsers_WithManagerRole_ReturnsForbidden [FAIL]
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=3, RoleName=User
?? USER user@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
[xUnit.net 00:00:01.42]       Expected response.StatusCode to be HttpStatusCode.Forbidden {value: 403}, but found HttpStatusCode.OK {value: 200}.
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
[xUnit.net 00:00:01.42]       Stack Trace:
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.42]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.42]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(484,0): at UserService.IntegrationTests.Controllers.UsersControllerTests.GetUsers_WithManagerRole_ReturnsForbidden()
[xUnit.net 00:00:01.42]         --- End of stack trace from previous location ---
[xUnit.net 00:00:01.42]     UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_UserAccessingOtherUser_ReturnsSuccess [FAIL]
[xUnit.net 00:00:01.42]       Expected response.StatusCode to be HttpStatusCode.OK {value: 200}, but found HttpStatusCode.Forbidden {value: 403}.
[xUnit.net 00:00:01.42]       Stack Trace:
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.42]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.42]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.42]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(249,0): at UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_UserAccessingOtherUser_ReturnsSuccess()
[xUnit.net 00:00:01.42]         --- End of stack trace from previous location ---
[18:53:38 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=5, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:38 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 5
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:39 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=6, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 6
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:39 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=6, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 DBG] Policy authentication schemes  did not succeed {"SourceContext": "Microsoft.AspNetCore.Authorization.AuthorizationMiddleware", "RequestId": "0HNEP695FD7OF", "RequestPath": "/api/users/profile"}
[18:53:39 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=6, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 6
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:39 INF] ?? Test Data Status: Tenants=2, Users=5, Roles=6, Permissions=13, UserRoles=6, RolePermissions=43 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - UserRoles count: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Active UserRoles: 0 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[18:53:39 INF]    - Permissions: 0 [] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 6
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 13 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 13
[18:53:39 INF] Starting UserService {}
=== UserService Starting ===
[xUnit.net 00:00:01.48]   Finished:    UserService.IntegrationTests
  UserService.IntegrationTests test failed with 2 error(s) (2.0s)
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(484): error TESTERROR:
      UserService.IntegrationTests.Controllers.UsersControllerTests.GetUsers_WithManagerRole_ReturnsForbidden (37ms): E
      rror Message: Expected response.StatusCode to be HttpStatusCode.Forbidden {value: 403}, but found HttpStatusCode.
      OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.UsersControllerTests.GetUsers_WithManagerRole_ReturnsForbidden() i
      n C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs:li
      ne 484
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(249): error TESTERROR:
      UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_UserAccessingOtherUser_ReturnsSuccess (3ms)
      : Error Message: Expected response.StatusCode to be HttpStatusCode.OK {value: 200}, but found HttpStatusCode.Forb
      idden {value: 403}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_UserAccessingOtherUser_ReturnsSuccess
      () in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.c
      s:line 249
      --- End of stack trace from previous location ---
[xUnit.net 00:00:01.80]   Finished:    AuthService.Tests
  AuthService.Tests test succeeded (2.3s)
[xUnit.net 00:00:02.34]   Finished:    ApiGateway.Tests
  ApiGateway.Tests test succeeded (2.8s)

Test summary: total: 149, failed: 2, succeeded: 147, skipped: 0, duration: 3.0s
Build failed with 2 error(s) in 4.2s
