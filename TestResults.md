?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? USER ENTITY DEBUG for admin@tenant1.com:
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO4Q", "RequestPath": "/api/roles/9"}
?? Adding 7 users to database...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Saving users to database...
? Created 7 legacy tenant users
?? Verifying test data...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO4R", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO4S", "RequestPath": "/api/roles"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] Updated permissions for role 2 {"SourceContext": "Common.Services.RoleService", "ActionId": "8ad0291c-8a74-480f-a269-c523568e54f9", "ActionName": "UserService.Controllers.RolesController.UpdateRolePermissions (UserService)", "RequestId": "0HNER84KQRO4P", "RequestPath": "/api/roles/2/permissions"}
? Created 7 users
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "c45b9a84-b374-4e70-b5cf-4187a263dd10", "ActionName": "UserService.Controllers.RolesController.UpdateRole (UserService)", "RequestId": "0HNER84KQRO4Q", "RequestPath": "/api/roles/9"}
[09:45:07 INF] Updated role 9 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "c45b9a84-b374-4e70-b5cf-4187a263dd10", "ActionName": "UserService.Controllers.RolesController.UpdateRole (UserService)", "RequestId": "0HNER84KQRO4Q", "RequestPath": "/api/roles/9"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Adding 8 user role assignments...
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO4T", "RequestPath": "/api/users/1/roles"}
[09:45:07 INF] Updated permissions for role 21 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO4S", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role StressTestRole48_46d026171b6f4099ba7c6954c4f3d3bb for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO4S", "RequestPath": "/api/roles"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO4U", "RequestPath": "/api/roles/9/permissions"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO4V", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] Stress test results: 50/50 successful operations {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 7 legacy tenant users
?? Verifying test data...
?? Ensuring clean database state...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO50", "RequestPath": "/api/users"}
? Created 2 tenants
?? Creating permissions...
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] Removed role 4 from user 3 in tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "b368b60e-a437-48d6-893d-aa344a3e3535", "ActionName": "UserService.Controllers.RolesController.RemoveRoleFromUser (UserService)", "RequestId": "0HNER84KQRO4O", "RequestPath": "/api/roles/4/users/3"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.57]     UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRole_ShouldPreserveExistingPermissions_WhenNotSpecified [FAIL]
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 8 roles
?? Creating role permissions...
? Created 2 tenants
?? Creating permissions...
? Created 2 tenants
?? Creating permissions...
[xUnit.net 00:00:01.57]       Expected permissionsResult!.Data! to be empty, but found {"users.view", "roles.view", "reports.view"}.
[xUnit.net 00:00:01.57]       Stack Trace:
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO51", "RequestPath": "/api/roles"}
[xUnit.net 00:00:01.57]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.57]            at FluentAssertions.Collections.GenericCollectionAssertions`3.BeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:01.57]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(298,0): at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRole_ShouldPreserveExistingPermissions_WhenNotSpecified()
[xUnit.net 00:00:01.57]         --- End of stack trace from previous location ---
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 20 permissions
?? Creating roles...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 20 permissions
?? Creating roles...
? Created 2 tenants
?? Creating permissions...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 roles
?? Creating role permissions...
? Created 8 roles
?? Creating role permissions...
? Created 20 permissions
?? Creating roles...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO52", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 7 users
? Created 64 role permissions
?? Creating users...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 64 role permissions
?? Creating users...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? Created 20 permissions
?? Creating roles...
?? Adding 7 users to database...
?? Saving users to database...
?? Saving users to database...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 8 roles
?? Creating role permissions...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Saving users to database...
? Created 7 users
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 7 users
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO53", "RequestPath": "/api/roles"}
?? Tenant 1 users: 5
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Tenant 2 users: 2
?? Creating user role assignments...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 legacy tenant users
?? Verifying test data...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Created 7 users
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Tenant 1 users: 5
?? Tenant 2 users: 2
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? Creating user role assignments...
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? Found 7 users and 8 roles for assignment
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Adding 8 user role assignments...
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 user role assignments
?? Creating legacy tenant users...
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Permissions: 18
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO54", "RequestPath": "/api/roles"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO55", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? DEBUGGING UserRole Query for user@tenant1.com:
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - UserRoles for this email: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - Active UserRoles for this email: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
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
?? USER ENTITY DEBUG for admin@tenant1.com:
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
   - Permissions: 1 [roles.view]
   - User ID: 1
?? JWT CLAIMS for user@tenant1.com:
   - Role: User (Actual: User)
   - User TenantId: 1
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? JWT CLAIMS for user@tenant1.com:
?? FILTERING DEBUG:
   - Role: User
   - Total loaded UserRoles: 1
   - Tenant: Test Tenant 1 (ID: 1)
   - UserRoles after TenantId filter: 1
   - Permissions: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO55", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole1 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO55", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO56", "RequestPath": "/api/roles"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO57", "RequestPath": "/api/roles"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO58", "RequestPath": "/api/users"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO59", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5A", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "58c0df2a-2ee8-44bf-9d33-daf18d74307c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO56", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role TestRole for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "58c0df2a-2ee8-44bf-9d33-daf18d74307c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO56", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? Creating tenants...
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] Updated permissions for role 10 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5A", "RequestPath": "/api/roles"}
? Created 2 tenants
?? Creating permissions...
[09:45:07 INF] Created role LoadTestRole2 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5A", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5B", "RequestPath": "/api/roles/99999"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5C", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
[09:45:07 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "adb7e8a7-31f5-4596-8fc4-53c6de95d375", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO57", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role ComprehensiveTestRole for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "adb7e8a7-31f5-4596-8fc4-53c6de95d375", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO57", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5D", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5E", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
[09:45:07 INF] Updated permissions for role 11 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5D", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole3 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5D", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5G", "RequestPath": "/api/roles"}
?? Adding 7 users to database...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5F", "RequestPath": "/api/roles/1/permissions"}
?? Saving users to database...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5H", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5I", "RequestPath": "/api/roles"}
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 7 users
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:07 INF] Updated permissions for role 12 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5I", "RequestPath": "/api/roles"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:07 INF] Created role LoadTestRole4 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5I", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5J", "RequestPath": "/api/roles"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5L", "RequestPath": "/api/users"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5K", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 users
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:07 INF] Updated permissions for role 13 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5K", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole5 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5K", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5M", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5N", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
[09:45:07 INF] Updated permissions for role 14 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5N", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole6 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5N", "RequestPath": "/api/roles"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 2 tenants
?? Creating permissions...
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? DEBUGGING UserRole Query for admin@tenant1.com:
? admin@tenant1.com has 1 role assignments
   - Total UserRoles in DB: 8
? TestDataSeeder: Test data seeding completed successfully
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5O", "RequestPath": "/api/roles"}
?? Creating roles...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5P", "RequestPath": "/api/roles"}
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
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
? Created 8 roles
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Creating role permissions...
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5Q", "RequestPath": "/api/roles/2/permissions"}
[09:45:07 INF] Updated permissions for role 15 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5P", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole7 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5P", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5R", "RequestPath": "/api/users"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5S", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] Updated permissions for role 2 {"SourceContext": "Common.Services.RoleService", "ActionId": "198218fd-43ea-407f-89f2-25f1b61706c4", "ActionName": "UserService.Controllers.RolesController.UpdateRolePermissions (UserService)", "RequestId": "0HNER84KQRO5Q", "RequestPath": "/api/roles/2/permissions"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5T", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5U", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[09:45:07 INF] Updated permissions for role 16 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5S", "RequestPath": "/api/roles"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:07 INF] Created role LoadTestRole8 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5S", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Ensuring clean database state...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? Created 2 tenants
?? Adding 7 users to database...
?? Creating permissions...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
?? Saving users to database...
? Created 20 permissions
?? Creating roles...
? Created 20 permissions
?? Creating roles...
[09:45:07 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "adb7e8a7-31f5-4596-8fc4-53c6de95d375", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5U", "RequestPath": "/api/roles"}
? Created 7 users
[09:45:07 INF] Created role TestRoleForEmptyPermissions for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "adb7e8a7-31f5-4596-8fc4-53c6de95d375", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5U", "RequestPath": "/api/roles"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 8 roles
?? Creating role permissions...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO5V", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Ensuring clean database state...
[xUnit.net 00:00:01.61]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldContainRequiredInformation [FAIL]
[xUnit.net 00:00:01.61]       Expected auditLogs not to be empty.
[xUnit.net 00:00:01.61]       Stack Trace:
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO60", "RequestPath": "/api/roles/9/permissions"}
[xUnit.net 00:00:01.61]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:01.61]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(391,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldContainRequiredInformation()
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.61]         --- End of stack trace from previous location ---
?? Ensuring clean database state...
? Created 7 legacy tenant users
?? Verifying test data...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Creating permissions...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:07 INF] Updated permissions for role 17 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5V", "RequestPath": "/api/roles"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[09:45:07 INF] Created role LoadTestRole9 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO5V", "RequestPath": "/api/roles"}
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 2 tenants
?? Creating permissions...
? Created 64 role permissions
?? Creating users...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO61", "RequestPath": "/api/roles"}
[09:45:07 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "06d576a5-8225-4574-8283-c82578b755d8", "ActionName": "UserService.Controllers.RolesController.UpdateRolePermissions (UserService)", "RequestId": "0HNER84KQRO60", "RequestPath": "/api/roles/9/permissions"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 64 role permissions
?? Creating users...
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Adding 7 users to database...
?? Saving users to database...
? Created 20 permissions
?? Creating roles...
?? Saving users to database...
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO63", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO62", "RequestPath": "/api/roles/9/permissions"}
? Created 8 roles
?? Creating role permissions...
? Created 7 users
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 8 roles
?? Creating role permissions...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF] Updated permissions for role 18 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO63", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole10 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO63", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO64", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO65", "RequestPath": "/api/roles"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO66", "RequestPath": "/api/roles"}
[xUnit.net 00:00:01.61]     UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithEmptyPermissionList_ShouldSucceed [FAIL]
[xUnit.net 00:00:01.61]       Expected permissionsResult!.Data to be empty, but found {"users.view", "roles.view"}.
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 legacy tenant users
?? Verifying test data...
[xUnit.net 00:00:01.61]       Stack Trace:
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[xUnit.net 00:00:01.61]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.61]            at FluentAssertions.Collections.GenericCollectionAssertions`3.BeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:01.61]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(69,0): at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithEmptyPermissionList_ShouldSucceed()
[xUnit.net 00:00:01.61]         --- End of stack trace from previous location ---
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? Ensuring clean database state...
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 2 tenants
?? Creating permissions...
[09:45:07 INF] Updated permissions for role 19 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO65", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole11 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO65", "RequestPath": "/api/roles"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 64 role permissions
?? Creating users...
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? Created 20 permissions
?? Creating roles...
?? Adding 7 users to database...
?? Saving users to database...
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO67", "RequestPath": "/api/roles"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO68", "RequestPath": "/api/roles"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 8 roles
?? Creating role permissions...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 7 users
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO69", "RequestPath": "/api/roles/5/permissions"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] Updated permissions for role 20 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO67", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
[09:45:07 INF] Created role LoadTestRole12 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO67", "RequestPath": "/api/roles"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 INF] Completed 100 requests without memory issues {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6A", "RequestPath": "/api/roles/7/users"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6B", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 2 tenants
?? Creating permissions...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Created 7 legacy tenant users
?? Verifying test data...
?? Ensuring clean database state...
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[xUnit.net 00:00:01.62]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRolePermissions_CrossTenantAttempt_ShouldReturnNotFound [FAIL]
[xUnit.net 00:00:01.62]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[xUnit.net 00:00:01.62]       Stack Trace:
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
? Database recreated with clean state
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
? Created 20 permissions
? admin@tenant1.com has 1 role assignments
?? Creating roles...
? TestDataSeeder: Test data seeding completed successfully
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.62]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.62]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(91,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRolePermissions_CrossTenantAttempt_ShouldReturnNotFound()
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]         --- End of stack trace from previous location ---
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 64 role permissions
?? Creating users...
? Created 2 tenants
?? Creating permissions...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]     UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_WithViewerRole_ReturnsAssignedUsers [FAIL]
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]       Expected response.StatusCode to be HttpStatusCode.OK {value: 200}, but found HttpStatusCode.NotFound {value: 404}.
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
[xUnit.net 00:00:01.62]       Stack Trace:
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
?? Saving users to database...
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
? Created 8 roles
?? Creating role permissions...
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
? Database recreated with clean state
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(331,0): at UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_WithViewerRole_ReturnsAssignedUsers()
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.62]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 7 users
[09:45:07 INF] Updated permissions for role 21 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6B", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole13 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6B", "RequestPath": "/api/roles"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
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
? Created 20 permissions
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? Creating roles...
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
?? Found 7 users and 8 roles for assignment
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Adding 8 user role assignments...
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 8 roles
?? Creating role permissions...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6C", "RequestPath": "/api/users/profile"}
? Created 8 roles
?? Creating role permissions...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6D", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
[09:45:07 INF] Updated permissions for role 22 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6D", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:07 INF] Created role LoadTestRole14 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6D", "RequestPath": "/api/roles"}
? Created 7 users
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 64 role permissions
?? Creating users...
?? Ensuring clean database state...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 64 role permissions
?? Creating users...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Saving users to database...
?? Adding 7 users to database...
? Created 2 tenants
?? Creating permissions...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6F", "RequestPath": "/api/roles"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 users
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 legacy tenant users
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6E", "RequestPath": "/api/users/3/roles"}
?? Verifying test data...
? Created 8 roles
?? Creating role permissions...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:07 INF] Updated permissions for role 23 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6F", "RequestPath": "/api/roles"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
[09:45:07 INF] Created role LoadTestRole15 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6F", "RequestPath": "/api/roles"}
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6G", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Created 7 legacy tenant users
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? Verifying test data...
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6H", "RequestPath": "/api/roles/2"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] Updated permissions for role 24 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6G", "RequestPath": "/api/roles"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
? Created 64 role permissions
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
?? Creating users...
   - UserRoles after TenantId filter: 1
[09:45:07 INF] Created role LoadTestRole16 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6G", "RequestPath": "/api/roles"}
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Adding 7 users to database...
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6I", "RequestPath": "/api/roles"}
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6J", "RequestPath": "/api/roles"}
? Created 7 users
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6K", "RequestPath": "/api/roles/5/users"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
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
?? Found 7 users and 8 roles for assignment
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
?? Adding 8 user role assignments...
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6L", "RequestPath": "/api/users/6"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:07 ERR] Error deleting role 2 {"SourceContext": "Common.Services.RoleService", "ActionId": "2be901bb-0b70-410f-8c3d-f8eaeb59bf2e", "ActionName": "UserService.Controllers.RolesController.DeleteRole (UserService)", "RequestId": "0HNER84KQRO6H", "RequestPath": "/api/roles/2"}
System.InvalidOperationException: Cannot delete role that has users assigned to it
   at Common.Services.RoleService.DeleteRoleAsync(Int32 roleId, CancellationToken cancellationToken) in C:\Users\mccre\dev\boiler\src\shared\Common\Services\RoleService.cs:line 162
[09:45:07 INF] Updated permissions for role 25 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6J", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole17 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6J", "RequestPath": "/api/roles"}
[09:45:07 ERR] Error deleting role 2 {"SourceContext": "UserService.Controllers.RolesController", "ActionId": "2be901bb-0b70-410f-8c3d-f8eaeb59bf2e", "ActionName": "UserService.Controllers.RolesController.DeleteRole (UserService)", "RequestId": "0HNER84KQRO6H", "RequestPath": "/api/roles/2"}
System.InvalidOperationException: Cannot delete role that has users assigned to it
   at Common.Services.RoleService.DeleteRoleAsync(Int32 roleId, CancellationToken cancellationToken) in C:\Users\mccre\dev\boiler\src\shared\Common\Services\RoleService.cs:line 162
   at UserService.Controllers.RolesController.DeleteRole(Int32 id) in C:\Users\mccre\dev\boiler\src\services\UserService\Controllers\RolesController.cs:line 237
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6M", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6N", "RequestPath": "/api/roles"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:07 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
[xUnit.net 00:00:01.64]     UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_CrossTenantAttempt_ReturnsNotFound [FAIL]
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.64]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.64]       Stack Trace:
?? Ensuring clean database state...
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.64]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.64]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(396,0): at UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_CrossTenantAttempt_ReturnsNotFound()
? Created 2 tenants
[xUnit.net 00:00:01.64]         --- End of stack trace from previous location ---
?? Creating permissions...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
[09:45:07 INF] Updated permissions for role 26 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6N", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole18 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6N", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 20 permissions
?? Creating roles...
? Created 20 permissions
?? Creating roles...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
[xUnit.net 00:00:01.64]     UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.DeleteRole_WithUsersAssigned_ShouldReturnConflict [FAIL]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
[xUnit.net 00:00:01.64]       Expected result.Message "An error occurred while deleting the role" to contain "has users assigned".
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[xUnit.net 00:00:01.64]       Stack Trace:
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.64]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.64]            at FluentAssertions.Primitives.StringAssertions`1.Contain(String expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.64]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(318,0): at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.DeleteRole_WithUsersAssigned_ShouldReturnConflict()
[xUnit.net 00:00:01.64]         --- End of stack trace from previous location ---
? Created 8 roles
?? Creating role permissions...
? Created 8 roles
?? Creating role permissions...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6O", "RequestPath": "/api/users"}
[09:45:07 INF] GetRoles with user counts completed in 1ms {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6P", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
[09:45:07 INF] Updated permissions for role 27 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6P", "RequestPath": "/api/roles"}
[09:45:07 INF] Created role LoadTestRole19 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6P", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 8 roles
?? Creating role permissions...
?? Saving users to database...
?? Saving users to database...
[09:45:07 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6Q", "RequestPath": "/api/roles"}
? Created 7 users
?? Ensuring clean database state...
?? Tenant 1 users: 5
? Created 7 users
?? Tenant 2 users: 2
?? Creating user role assignments...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Found 7 users and 8 roles for assignment
?? Creating user role assignments...
?? Adding 8 user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] Updated permissions for role 28 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6Q", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole20 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6Q", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 legacy tenant users
?? Verifying test data...
?? Saving users to database...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6R", "RequestPath": "/api/roles"}
?? Found 7 users and 8 roles for assignment
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Adding 8 user role assignments...
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Saving users to database...
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
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
?? FILTERING DEBUG:
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
[09:45:08 INF] Updated permissions for role 29 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6R", "RequestPath": "/api/roles"}
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
[09:45:08 INF] Created role LoadTestRole21 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6R", "RequestPath": "/api/roles"}
   - Permissions: 18
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Created 7 users
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? Tenant 1 users: 5
?? Tenant 2 users: 2
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6S", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Adding 8 user role assignments...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6T", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6U", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6T", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role TestRoleWithExtra for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6T", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 30 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6U", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole22 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6U", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Created 7 legacy tenant users
?? Verifying test data...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
? Created 2 tenants
? admin@tenant1.com has 1 role assignments
?? Creating permissions...
? TestDataSeeder: Test data seeding completed successfully
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO6V", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 20 permissions
?? Creating roles...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 2 tenants
?? Creating permissions...
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO70", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
? Created 20 permissions
?? Creating roles...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] Updated permissions for role 31 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6V", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole23 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO6V", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO72", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO71", "RequestPath": "/api/roles/5/users"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO73", "RequestPath": "/api/roles"}
?? Saving users to database...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 7 users
?? Saving users to database...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF] Updated permissions for role 32 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO72", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole24 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO72", "RequestPath": "/api/roles"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO74", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO75", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] Updated permissions for role 33 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO75", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Created role LoadTestRole25 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO75", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO76", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO77", "RequestPath": "/api/users"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO78", "RequestPath": "/api/roles"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Ensuring clean database state...
[xUnit.net 00:00:01.66]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRoleUsers_CrossTenantAttempt_ShouldReturnNotFound [FAIL]
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.66]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.66]       Stack Trace:
[xUnit.net 00:00:01.66]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.66]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.66]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.66]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.66]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.66]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.66]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.66]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(127,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRoleUsers_CrossTenantAttempt_ShouldReturnNotFound()
[xUnit.net 00:00:01.66]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO79", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
[xUnit.net 00:00:01.66]     UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetUserRoles_WithManyRoles_ShouldMaintainPerformance [FAIL]
[xUnit.net 00:00:01.66]       System.Text.Json.JsonException : The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
[xUnit.net 00:00:01.67]       Stack Trace:
[xUnit.net 00:00:01.67]            at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO78", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, TCollection& value)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state, Utf8JsonReader& reader)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOptions options, ReadStack& state)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, JsonReaderState& jsonReaderState, ReadStack& readStack, T& value)
[xUnit.net 00:00:01.67]            at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken cancellationToken)
[xUnit.net 00:00:01.67]            at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerOptions options, CancellationToken cancellationToken)
[xUnit.net 00:00:01.67]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(352,0): at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.CreateTestUserForLoadTesting()
[xUnit.net 00:00:01.67]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(170,0): at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetUserRoles_WithManyRoles_ShouldMaintainPerformance()
[09:45:08 INF] Created role Test Role with ???? and �mojis ?? for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO78", "RequestPath": "/api/roles"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
[xUnit.net 00:00:01.67]         --- End of stack trace from previous location ---
?? Creating tenants...
? Created 2 tenants
? Created 8 roles
?? Creating permissions...
?? Creating role permissions...
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "58c0df2a-2ee8-44bf-9d33-daf18d74307c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO79", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role EmptyTestRole for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "58c0df2a-2ee8-44bf-9d33-daf18d74307c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO79", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
[09:45:08 INF] Pagination performance: 5 items: 4ms, 10 items: 1ms, 20 items: 1ms, 50 items: 1ms {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7A", "RequestPath": "/api/roles/9/users"}
? Created 8 roles
?? Creating role permissions...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.67]     UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithSpecialCharacters_ShouldHandleGracefully [FAIL]
[xUnit.net 00:00:01.67]       Did not expect result.Data!.Description "Description with special chars: <script>alert('xss')</script>" to contain "<script>".
[xUnit.net 00:00:01.67]       Stack Trace:
[xUnit.net 00:00:01.67]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
[xUnit.net 00:00:01.67]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.67]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.67]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.67]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.67]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.67]            at FluentAssertions.Primitives.StringAssertions`1.NotContain(String unexpected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.67]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(113,0): at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithSpecialCharacters_ShouldHandleGracefully()
[xUnit.net 00:00:01.67]         --- End of stack trace from previous location ---
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
?? Ensuring clean database state...
? Created 20 permissions
?? Creating roles...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 64 role permissions
?? Creating users...
? Created 2 tenants
?? Creating permissions...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? Created 7 users
?? Adding 7 users to database...
? Created 8 roles
?? Creating role permissions...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Saving users to database...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 20 permissions
?? Creating roles...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 64 role permissions
?? Creating users...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? Created 8 roles
?? Creating role permissions...
?? Adding 7 users to database...
?? Saving users to database...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 users
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? Found 7 users and 8 roles for assignment
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? Adding 8 user role assignments...
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? USER ENTITY DEBUG for admin@tenant1.com:
? Created 7 legacy tenant users
   - User ID: 1
?? Verifying test data...
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
? Created 64 role permissions
?? Saving users to database...
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? Creating users...
?? JWT CLAIMS for admin@tenant1.com:
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Found Tenant 1: ID=1, Name=Test Tenant 1
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Ensuring clean database state...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 users
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 2 tenants
?? Creating permissions...
?? Found 7 users and 8 roles for assignment
?? DEBUGGING UserRole Query for admin@tenant1.com:
?? Adding 8 user role assignments...
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Saving users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7B", "RequestPath": "/api/roles/5"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7C", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
[xUnit.net 00:00:01.68]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRole_CrossTenantAttempt_ShouldReturnNotFound [FAIL]
? TestDataSeeder: Test data seeding completed successfully
[xUnit.net 00:00:01.68]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
?? Ensuring clean database state...
[xUnit.net 00:00:01.68]       Stack Trace:
? Created 64 role permissions
[xUnit.net 00:00:01.68]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
?? Creating users...
[xUnit.net 00:00:01.68]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.68]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.68]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.68]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
?? DEBUGGING UserRole Query for admin@tenant1.com:
[xUnit.net 00:00:01.68]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.68]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[xUnit.net 00:00:01.68]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(33,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRole_CrossTenantAttempt_ShouldReturnNotFound()
[xUnit.net 00:00:01.68]         --- End of stack trace from previous location ---
?? Adding 7 users to database...
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
? Created 2 tenants
?? USER ENTITY DEBUG for admin@tenant1.com:
?? Creating permissions...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7C", "RequestPath": "/api/roles"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] Created role LoadTestRole1 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7C", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7D", "RequestPath": "/api/roles"}
[09:45:08 DBG] Policy authentication schemes  did not succeed {"SourceContext": "Microsoft.AspNetCore.Authorization.AuthorizationMiddleware", "RequestId": "0HNER84KQRO7E", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] Authorization failed. These requirements were not met:
DenyAnonymousAuthorizationRequirement: Requires an authenticated user. {"EventId": {"Id": 2, "Name": "UserAuthorizationFailed"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7E", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7G", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7F", "RequestPath": "/api/users/3/roles"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
[09:45:08 INF] Updated permissions for role 10 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7G", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 INF] Created role LoadTestRole2 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7G", "RequestPath": "/api/roles"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7H", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7I", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
[09:45:08 INF] Updated permissions for role 11 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7H", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole3 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7H", "RequestPath": "/api/roles"}
? Created 7 users
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7K", "RequestPath": "/api/users/1"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7J", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7L", "RequestPath": "/api/roles"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Saving users to database...
[09:45:08 INF] Updated permissions for role 12 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7J", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole4 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7J", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 7 users
? Created 7 legacy tenant users
?? Verifying test data...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Found 7 users and 8 roles for assignment
? Created 2 tenants
?? Creating permissions...
?? Adding 8 user role assignments...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7M", "RequestPath": "/api/roles"}
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7N", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] Updated permissions for role 13 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7M", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole5 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7M", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] Assigned role 2 to user 3 in tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "fb737f3a-00ba-48e0-8ae5-04dd7058013c", "ActionName": "UserService.Controllers.UsersController.AssignRoleToUser (UserService)", "RequestId": "0HNER84KQRO7F", "RequestPath": "/api/users/3/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7O", "RequestPath": "/api/roles"}
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7P", "RequestPath": "/api/roles"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Creating tenants...
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7Q", "RequestPath": "/api/roles/5"}
? Created 2 tenants
?? Creating permissions...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Updated permissions for role 14 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7O", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole6 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7O", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
? Created 20 permissions
?? Creating roles...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 8 roles
?? Creating role permissions...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7R", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] Updated permissions for role 5 {"SourceContext": "Common.Services.RoleService", "ActionId": "21f77796-6f8e-4672-80a9-60f8d93c1516", "ActionName": "UserService.Controllers.RolesController.UpdateRole (UserService)", "RequestId": "0HNER84KQRO7Q", "RequestPath": "/api/roles/5"}
? Created 7 users
[09:45:08 INF] Updated role 5 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "21f77796-6f8e-4672-80a9-60f8d93c1516", "ActionName": "UserService.Controllers.RolesController.UpdateRole (UserService)", "RequestId": "0HNER84KQRO7Q", "RequestPath": "/api/roles/5"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7T", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7S", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 15 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7R", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole7 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7R", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7S", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role UniqueTestRole1 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7S", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7U", "RequestPath": "/api/roles"}
?? Saving users to database...
[xUnit.net 00:00:01.70]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.UpdateRole_CrossTenantAttempt_ShouldReturnNotFound [FAIL]
[xUnit.net 00:00:01.70]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.70]       Stack Trace:
?? Ensuring clean database state...
[xUnit.net 00:00:01.70]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.70]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.70]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.70]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.70]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.70]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.70]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.70]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(57,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.UpdateRole_CrossTenantAttempt_ShouldReturnNotFound()
[xUnit.net 00:00:01.70]         --- End of stack trace from previous location ---
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO80", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO7V", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Created 7 users
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 2 tenants
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating permissions...
?? Creating user role assignments...
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF] Updated permissions for role 16 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7U", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Created role LoadTestRole8 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7U", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Updated permissions for role 10 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7V", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role UniqueTestRole2 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "8bb1a891-b518-47f2-9eae-4345ecfe5b0c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO7V", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO81", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO82", "RequestPath": "/api/roles/10"}
? Created 8 roles
?? Creating role permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO83", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:08 INF] Updated permissions for role 17 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO81", "RequestPath": "/api/roles"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
[09:45:08 INF] Created role LoadTestRole9 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO81", "RequestPath": "/api/roles"}
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO84", "RequestPath": "/api/users"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO85", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO86", "RequestPath": "/api/roles"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
?? Found Tenant 2: ID=2, Name=Test Tenant 2
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Adding 7 users to database...
?? Saving users to database...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] Updated permissions for role 18 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO85", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole10 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO85", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO87", "RequestPath": "/api/roles/9999/users"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO88", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO89", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 19 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO88", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole11 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO88", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[09:45:08 ERR] Error updating role 10 {"SourceContext": "Common.Services.RoleService", "ActionId": "54e61d01-8136-48d8-b598-c814bfb53988", "ActionName": "UserService.Controllers.RolesController.UpdateRole (UserService)", "RequestId": "0HNER84KQRO82", "RequestPath": "/api/roles/10"}
System.InvalidOperationException: Role name 'UniqueTestRole1' already exists in this tenant
   at Common.Services.RoleService.UpdateRoleAsync(Int32 roleId, String name, String description, List`1 permissions, CancellationToken cancellationToken) in C:\Users\mccre\dev\boiler\src\shared\Common\Services\RoleService.cs:line 109
? Created 20 permissions
?? Creating roles...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 ERR] Error updating role 10 {"SourceContext": "UserService.Controllers.RolesController", "ActionId": "54e61d01-8136-48d8-b598-c814bfb53988", "ActionName": "UserService.Controllers.RolesController.UpdateRole (UserService)", "RequestId": "0HNER84KQRO82", "RequestPath": "/api/roles/10"}
System.InvalidOperationException: Role name 'UniqueTestRole1' already exists in this tenant
   at Common.Services.RoleService.UpdateRoleAsync(Int32 roleId, String name, String description, List`1 permissions, CancellationToken cancellationToken) in C:\Users\mccre\dev\boiler\src\shared\Common\Services\RoleService.cs:line 109
   at UserService.Controllers.RolesController.UpdateRole(Int32 id, UpdateRoleDto updateRoleDto) in C:\Users\mccre\dev\boiler\src\services\UserService\Controllers\RolesController.cs:line 194
? Created 2 tenants
?? Creating permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8A", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 20 permissions
?? Creating roles...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
[xUnit.net 00:00:01.71]     UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.UpdateRole_WithDuplicateName_ShouldReturnConflict [FAIL]
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.71]       Expected result.Message "An error occurred while updating the role" to contain "already exists".
[xUnit.net 00:00:01.71]       Stack Trace:
[xUnit.net 00:00:01.71]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.71]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.71]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.71]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.71]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.71]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.71]            at FluentAssertions.Primitives.StringAssertions`1.Contain(String expected, String because, Object[] becauseArgs)
? Created 2 tenants
?? Creating permissions...
[xUnit.net 00:00:01.71]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(157,0): at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.UpdateRole_WithDuplicateName_ShouldReturnConflict()
[xUnit.net 00:00:01.71]         --- End of stack trace from previous location ---
[09:45:08 INF] Updated permissions for role 20 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8A", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole12 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8A", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8B", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
? Created 8 roles
?? Creating role permissions...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Saving users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8C", "RequestPath": "/api/users/7"}
? Created 64 role permissions
?? Creating users...
? Created 7 users
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 INF] Updated permissions for role 21 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8B", "RequestPath": "/api/roles"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF] Created role LoadTestRole13 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8B", "RequestPath": "/api/roles"}
?? Saving users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8D", "RequestPath": "/api/roles"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 INF] Updated permissions for role 22 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8D", "RequestPath": "/api/roles"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:08 INF] Created role LoadTestRole14 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8D", "RequestPath": "/api/roles"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Created 8 user role assignments
?? Creating legacy tenant users...
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 7 users
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Created 8 user role assignments
?? Creating legacy tenant users...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8E", "RequestPath": "/api/roles"}
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=3, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Performance stats - Average: 2.1ms, Max: 4ms, Min: 1ms, Variance: 3ms {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8F", "RequestPath": "/api/roles/2/users"}
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
?? Ensuring clean database state...
? TestDataSeeder: Test data seeding completed successfully
?? Ensuring clean database state...
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 2 tenants
?? Creating permissions...
[09:45:08 WRN] User 2 attempted to view users for role 2 without users.view permission {"SourceContext": "UserService.Controllers.RolesController", "ActionId": "4e3dce15-e457-4ee8-81b6-1ded3967b155", "ActionName": "UserService.Controllers.RolesController.GetRoleUsers (UserService)", "RequestId": "0HNER84KQRO8F", "RequestPath": "/api/roles/2/users"}
? Created 2 tenants
?? Creating permissions...
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
[09:45:08 INF] Updated permissions for role 23 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8E", "RequestPath": "/api/roles"}
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Tenant: Test Tenant 1 (ID: 1)
[09:45:08 INF] Created role LoadTestRole15 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8E", "RequestPath": "/api/roles"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Permissions: 18
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
? Created 20 permissions
?? Creating roles...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8G", "RequestPath": "/api/users/999"}
? Created 2 tenants
? Created 8 roles
?? Creating permissions...
?? Creating role permissions...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8H", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8I", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] Updated permissions for role 24 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8H", "RequestPath": "/api/roles"}
? Created 2 tenants
?? Creating permissions...
[09:45:08 INF] Created role LoadTestRole16 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8H", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8J", "RequestPath": "/api/roles"}
? Created 2 tenants
?? Creating permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? Created 8 roles
? Created 64 role permissions
?? Creating role permissions...
?? Creating users...
?? Adding 7 users to database...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
?? Saving users to database...
? Created 20 permissions
?? Creating roles...
[09:45:08 INF] Updated permissions for role 25 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8J", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole17 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8J", "RequestPath": "/api/roles"}
? Created 7 users
? Created 8 roles
?? Creating role permissions...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8K", "RequestPath": "/api/roles"}
?? Saving users to database...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 users
[09:45:08 INF] Updated permissions for role 26 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8K", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole18 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8K", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Saving users to database...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Created 64 role permissions
?? Creating users...
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Adding 7 users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8L", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found 7 users and 8 roles for assignment
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Adding 8 user role assignments...
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 INF] Updated permissions for role 27 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8L", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole19 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8L", "RequestPath": "/api/roles"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Created 8 user role assignments
?? Creating legacy tenant users...
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? DEBUGGING UserRole Query for admin@tenant1.com:
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
   - Active UserRoles for this email: 1
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8M", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8N", "RequestPath": "/api/roles/6"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for manager@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? USER ENTITY DEBUG for manager@tenant1.com:
   - User ID: 3
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=4, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
   - Relevant UserRoles: RoleId=4, RoleName=Manager
?? USER manager@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 4 [users.edit, users.view, roles.view, reports.view]
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager (Actual: Manager)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 4
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 4
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Tenant: Test Tenant 1 (ID: 1)
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8O", "RequestPath": "/api/roles/2/users"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Permissions: 18
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Updated permissions for role 28 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8M", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole20 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8M", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8P", "RequestPath": "/api/roles/2/users"}
?? USER ENTITY DEBUG for user@tenant1.com:
   - User ID: 2
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
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
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8Q", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8R", "RequestPath": "/api/users/3"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8S", "RequestPath": "/api/roles/-1"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Ensuring clean database state...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 INF] Updated permissions for role 29 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8Q", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole21 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO8Q", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 2 tenants
?? Creating permissions...
?? Ensuring clean database state...
?? Ensuring clean database state...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
? Created 20 permissions
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
? Created 2 tenants
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? Creating permissions...
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8V", "RequestPath": "/api/roles/2/users"}
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8U", "RequestPath": "/api/roles/2/users"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Creating roles...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO8T", "RequestPath": "/api/roles/2/users"}
? Created 2 tenants
?? Creating permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO90", "RequestPath": "/api/roles"}
[09:45:08 ERR] Error deleting role 6 {"SourceContext": "Common.Services.RoleService", "ActionId": "f740eb4e-d430-4a6d-95bc-a63927f1ec68", "ActionName": "UserService.Controllers.RolesController.DeleteRole (UserService)", "RequestId": "0HNER84KQRO8N", "RequestPath": "/api/roles/6"}
System.InvalidOperationException: Cannot delete role that has users assigned to it
   at Common.Services.RoleService.DeleteRoleAsync(Int32 roleId, CancellationToken cancellationToken) in C:\Users\mccre\dev\boiler\src\shared\Common\Services\RoleService.cs:line 162
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 ERR] Error deleting role 6 {"SourceContext": "UserService.Controllers.RolesController", "ActionId": "f740eb4e-d430-4a6d-95bc-a63927f1ec68", "ActionName": "UserService.Controllers.RolesController.DeleteRole (UserService)", "RequestId": "0HNER84KQRO8N", "RequestPath": "/api/roles/6"}
System.InvalidOperationException: Cannot delete role that has users assigned to it
   at Common.Services.RoleService.DeleteRoleAsync(Int32 roleId, CancellationToken cancellationToken) in C:\Users\mccre\dev\boiler\src\shared\Common\Services\RoleService.cs:line 162
   at UserService.Controllers.RolesController.DeleteRole(Int32 id) in C:\Users\mccre\dev\boiler\src\services\UserService\Controllers\RolesController.cs:line 237
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 20 permissions
?? Creating roles...
? Created 20 permissions
?? Creating roles...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 8 roles
?? Creating role permissions...
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
[09:45:08 INF] Updated permissions for role 30 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO90", "RequestPath": "/api/roles"}
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO91", "RequestPath": "/api/roles/2/users"}
[09:45:08 INF] Created role LoadTestRole22 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO90", "RequestPath": "/api/roles"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 roles
?? Creating role permissions...
? Created 8 roles
?? Creating role permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO92", "RequestPath": "/api/roles/2/users"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
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
? Created 2 tenants
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? Creating permissions...
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[xUnit.net 00:00:01.74]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.DeleteRole_CrossTenantAttempt_ShouldReturnNotFound [FAIL]
[xUnit.net 00:00:01.74]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.InternalServerError {value: 500}.
[xUnit.net 00:00:01.74]       Stack Trace:
[xUnit.net 00:00:01.74]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.74]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.74]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.74]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.74]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.74]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.74]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.74]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(74,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.DeleteRole_CrossTenantAttempt_ShouldReturnNotFound()
[xUnit.net 00:00:01.74]         --- End of stack trace from previous location ---
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 20 permissions
?? Creating roles...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO93", "RequestPath": "/api/roles/2/users"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO94", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 8 roles
?? Creating role permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO95", "RequestPath": "/api/roles/2/users"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 64 role permissions
?? Creating users...
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO96", "RequestPath": "/api/roles/2/users"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
?? Saving users to database...
[09:45:08 INF] Updated permissions for role 31 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO94", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole23 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO94", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO97", "RequestPath": "/api/roles/2/users"}
? Created 7 users
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Adding 8 user role assignments...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO98", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 64 role permissions
?? Creating users...
? Created 8 user role assignments
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Creating legacy tenant users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
[09:45:08 INF] Updated permissions for role 32 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO98", "RequestPath": "/api/roles"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 INF] Created role LoadTestRole24 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO98", "RequestPath": "/api/roles"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 users
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO99", "RequestPath": "/api/roles"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Created 8 user role assignments
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? Creating legacy tenant users...
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant2.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? USER ENTITY DEBUG for admin@tenant2.com:
   - User ID: 6
   - User TenantId: 2
   - Separately loaded UserRoles count: 1
? Created 7 legacy tenant users
   - UserRoles details: RoleId=7, TenantId=2, Active=True
?? Verifying test data...
?? TENANT DEBUG:
   - Tenant ID: 2
   - Tenant Name: Test Tenant 2
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=7, RoleName=Admin
?? USER admin@tenant2.com in TENANT 2:
   - UserRoles: 1
[09:45:08 INF] Updated permissions for role 33 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO99", "RequestPath": "/api/roles"}
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant2.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 2 (ID: 2)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant2.com:
   - Role: Admin
   - Tenant: Test Tenant 2 (ID: 2)
   - Permissions: 18
[09:45:08 INF] Created role LoadTestRole25 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO99", "RequestPath": "/api/roles"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9A", "RequestPath": "/api/roles"}
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9B", "RequestPath": "/api/users"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9C", "RequestPath": "/api/roles"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9D", "RequestPath": "/api/users"}
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "58c0df2a-2ee8-44bf-9d33-daf18d74307c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO9A", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role TestRoleForPermissionRemoval for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "58c0df2a-2ee8-44bf-9d33-daf18d74307c", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQRO9A", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9E", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9F", "RequestPath": "/api/roles/9/permissions"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
[09:45:08 INF] Concurrent request performance: Average 6.2ms across 10 requests {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.75]     UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.RemoveRoles_BulkRemoval_ShouldCompleteEfficiently [FAIL]
[xUnit.net 00:00:01.75]       System.Text.Json.JsonException : The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
[xUnit.net 00:00:01.75]       Stack Trace:
?? Ensuring clean database state...
[xUnit.net 00:00:01.75]            at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, TCollection& value)
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state, Utf8JsonReader& reader)
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "198218fd-43ea-407f-89f2-25f1b61706c4", "ActionName": "UserService.Controllers.RolesController.UpdateRolePermissions (UserService)", "RequestId": "0HNER84KQRO9F", "RequestPath": "/api/roles/9/permissions"}
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOptions options, ReadStack& state)
? Database recreated with clean state
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, JsonReaderState& jsonReaderState, ReadStack& readStack, T& value)
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.75]            at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken cancellationToken)
[xUnit.net 00:00:01.75]            at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerOptions options, CancellationToken cancellationToken)
[xUnit.net 00:00:01.75]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(352,0): at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.CreateTestUserForLoadTesting()
? Created 2 tenants
[xUnit.net 00:00:01.75]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(123,0): at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.RemoveRoles_BulkRemoval_ShouldCompleteEfficiently()
?? Creating permissions...
[xUnit.net 00:00:01.75]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9G", "RequestPath": "/api/roles/9/permissions"}
? Created 8 roles
?? Creating role permissions...
? Created 20 permissions
?? Creating roles...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 8 roles
?? Creating role permissions...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
[xUnit.net 00:00:01.75]     UserService.IntegrationTests.Controllers.RolesControllerTests.UpdateRolePermissions_RemoveAllPermissions_ReturnsSuccess [FAIL]
? Created 20 permissions
?? Creating roles...
[xUnit.net 00:00:01.75]       Expected permissionsResult!.Data to be empty, but found {"users.view", "roles.view"}.
[xUnit.net 00:00:01.75]       Stack Trace:
[xUnit.net 00:00:01.75]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.76]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.76]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.76]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.76]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.76]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.76]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.76]            at FluentAssertions.Collections.GenericCollectionAssertions`3.BeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:01.76]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(545,0): at UserService.IntegrationTests.Controllers.RolesControllerTests.UpdateRolePermissions_RemoveAllPermissions_ReturnsSuccess()
[xUnit.net 00:00:01.76]         --- End of stack trace from previous location ---
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9H", "RequestPath": "/api/roles/2/users/3"}
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 64 role permissions
?? Creating users...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 7 users
?? Saving users to database...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Saving users to database...
? Created 7 users
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Created 7 legacy tenant users
?? Verifying test data...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 7 legacy tenant users
?? Verifying test data...
? Created 7 legacy tenant users
?? Verifying test data...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9I", "RequestPath": "/api/users/2"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
?? USER ENTITY DEBUG for admin@tenant1.com:
   - Relevant UserRoles: RoleId=2, RoleName=Admin
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Relevant UserRoles: RoleId=2, RoleName=Admin
   - Tenant: Test Tenant 1 (ID: 1)
?? Ensuring clean database state...
   - Permissions: 18
?? USER admin@tenant1.com in TENANT 1:
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - UserRoles: 1
   - Tenant: Test Tenant 1 (ID: 1)
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9J", "RequestPath": "/api/users"}
   - Permissions: 18
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9K", "RequestPath": "/api/roles/users/6"}
[09:45:08 WRN] Failed to create test user 1: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 20 permissions
?? Creating roles...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
   - User ID: 1
?? FILTERING DEBUG:
   - User TenantId: 1
   - Total loaded UserRoles: 1
   - Separately loaded UserRoles count: 1
   - UserRoles after TenantId filter: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
? Created 8 roles
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? Creating role permissions...
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? FILTERING DEBUG:
?? JWT CLAIMS for admin@tenant1.com:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Role: Admin (Actual: Admin)
   - Relevant UserRoles: RoleId=2, RoleName=Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9L", "RequestPath": "/api/users"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9M", "RequestPath": "/api/users"}
[09:45:08 WRN] Failed to create test user 2: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 64 role permissions
?? Creating users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9N", "RequestPath": "/api/users"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
[09:45:08 INF] HTTP POST /api/users responded 400 in 0.6002 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 WRN] Failed to create test user 3: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9O", "RequestPath": "/api/users"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 6.8263 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9O", "RequestPath": "/api/users"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9O", "RequestPath": "/api/users"}
[09:45:08 INF] Starting UserService {}
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9P", "RequestPath": "/api/users"}
=== UserService Starting ===
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9P", "RequestPath": "/api/users"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9P", "RequestPath": "/api/users"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 1.3615 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9Q", "RequestPath": "/api/users/1/roles"}
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9Q", "RequestPath": "/api/users/1/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9Q", "RequestPath": "/api/users/1/roles"}
[09:45:08 WRN] Failed to create test user 4: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 1.3381 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9R", "RequestPath": "/api/users"}
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9R", "RequestPath": "/api/users"}
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9S", "RequestPath": "/api/users"}
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9S", "RequestPath": "/api/users"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9R", "RequestPath": "/api/users"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9S", "RequestPath": "/api/users"}
[09:45:08 INF] HTTP GET /api/roles/users/6 responded 200 in 18.2826 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 0.8308 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 WRN] Failed to create test user 5: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
?? Ensuring clean database state...
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[xUnit.net 00:00:01.78]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetUserRoles_CrossTenantUser_ShouldReturnForbiddenOrNotFound [FAIL]
? Database recreated with clean state
[xUnit.net 00:00:01.78]       Expected response.StatusCode to be one of {HttpStatusCode.Forbidden {value: 403}, HttpStatusCode.NotFound {value: 404}}, but found HttpStatusCode.OK {value: 200}.
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.78]       Stack Trace:
[xUnit.net 00:00:01.78]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.78]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.78]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.78]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.78]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.78]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.78]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(IEnumerable`1 validValues, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.78]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(TEnum[] validValues)
[xUnit.net 00:00:01.78]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(210,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetUserRoles_CrossTenantUser_ShouldReturnForbiddenOrNotFound()
[xUnit.net 00:00:01.78]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9T", "RequestPath": "/api/users"}
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9T", "RequestPath": "/api/users"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9T", "RequestPath": "/api/users"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 2.7134 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Created 20 permissions
?? Creating roles...
[09:45:08 INF] HTTP GET /api/users/1/roles responded 200 in 8.6727 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 0.8012 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9U", "RequestPath": "/api/roles/2/users"}
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9U", "RequestPath": "/api/roles/2/users"}
? Created 8 roles
?? Creating role permissions...
[09:45:08 WRN] Failed to create test user 6: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9U", "RequestPath": "/api/roles/2/users"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 2 tenants
?? Creating permissions...
[09:45:08 INF] HTTP GET /api/roles/2/users responded 200 in 1.2876 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9V", "RequestPath": "/api/users"}
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQRO9V", "RequestPath": "/api/users"}
[09:45:08 INF] GetRoleUsers completed in 1ms for 1 users {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQRO9V", "RequestPath": "/api/users"}
?? Ensuring clean database state...
? Created 8 roles
?? Creating role permissions...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 INF] HTTP POST /api/users responded 400 in 0.8707 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Created 64 role permissions
?? Creating users...
[09:45:08 WRN] Failed to create test user 7: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
? Created 2 tenants
?? Creating permissions...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Saving users to database...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQROA0", "RequestPath": "/api/users"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQROA0", "RequestPath": "/api/users"}
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 roles
?? Creating role permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA0", "RequestPath": "/api/users"}
[09:45:08 INF] HTTP POST /api/users responded 400 in 1.0084 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
? Created 8 user role assignments
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Creating legacy tenant users...
?? Adding 7 users to database...
[09:45:08 WRN] Failed to create test user 8: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? Saving users to database...
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Created 7 users
[09:45:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQROA1", "RequestPath": "/api/users"}
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
[09:45:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNER84KQROA1", "RequestPath": "/api/users"}
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA1", "RequestPath": "/api/users"}
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? Found 7 users and 8 roles for assignment
? Created 64 role permissions
?? Creating users...
?? Adding 8 user role assignments...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Saving users to database...
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 WRN] Failed to create test user 9: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
? Created 7 users
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA2", "RequestPath": "/api/users"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA3", "RequestPath": "/api/users"}
[09:45:08 WRN] Failed to create test user 10: The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134. {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
?? DEBUGGING UserRole Query for admin@tenant1.com:
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Created 7 legacy tenant users
?? Verifying test data...
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA4", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] Updated permissions for role 9 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA4", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
[09:45:08 INF] Created role LoadTestRole1 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA4", "RequestPath": "/api/roles"}
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA5", "RequestPath": "/api/users/1/permissions"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA6", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA7", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 10 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA7", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole2 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA7", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA8", "RequestPath": "/api/roles"}
[09:45:08 INF] User count query completed in 1ms for 6 roles {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Updated permissions for role 11 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA8", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole3 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA8", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROA9", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 12 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA9", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole4 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROA9", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAA", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 13 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAA", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole5 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAA", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAB", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 14 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAB", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole6 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAB", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAC", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 15 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAC", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole7 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAC", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAD", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 16 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAD", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole8 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAD", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAE", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 17 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAE", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole9 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAE", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAF", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 18 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAF", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole10 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAF", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAG", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 19 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAG", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole11 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAG", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAH", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 20 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAH", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole12 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAH", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAI", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 21 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAI", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole13 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAI", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAJ", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 22 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAJ", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole14 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAJ", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAK", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 23 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAK", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole15 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAK", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAL", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[09:45:08 INF] Updated permissions for role 24 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAL", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole16 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAL", "RequestPath": "/api/roles"}
? Created 2 tenants
?? Creating permissions...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAM", "RequestPath": "/api/roles"}
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] Updated permissions for role 25 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAM", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole17 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAM", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAN", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 26 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAN", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole18 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAN", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAO", "RequestPath": "/api/roles"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
[09:45:08 INF] Updated permissions for role 27 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAO", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole19 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAO", "RequestPath": "/api/roles"}
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAP", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] Updated permissions for role 28 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAP", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole20 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAP", "RequestPath": "/api/roles"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAQ", "RequestPath": "/api/roles"}
?? DEBUGGING UserRole Query for manager@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 4 [users.edit, users.view, roles.view, reports.view]
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager (Actual: Manager)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 4
?? JWT CLAIMS for manager@tenant1.com:
   - Role: Manager
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 4
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAR", "RequestPath": "/api/users"}
[09:45:08 INF] Updated permissions for role 29 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAQ", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole21 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAQ", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAS", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 30 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAS", "RequestPath": "/api/roles"}
?? Ensuring clean database state...
[09:45:08 INF] Created role LoadTestRole22 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAS", "RequestPath": "/api/roles"}
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAT", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
[09:45:08 INF] Updated permissions for role 31 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAT", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole23 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAT", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAU", "RequestPath": "/api/roles"}
[09:45:08 INF] Updated permissions for role 32 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAU", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole24 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAU", "RequestPath": "/api/roles"}
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROAV", "RequestPath": "/api/roles"}
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
[09:45:08 INF] Updated permissions for role 33 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAV", "RequestPath": "/api/roles"}
[09:45:08 INF] Created role LoadTestRole25 for tenant 1 {"SourceContext": "Common.Services.RoleService", "ActionId": "532359a1-8ea0-481f-bc0a-e4bccf4209b7", "ActionName": "UserService.Controllers.RolesController.CreateRole (UserService)", "RequestId": "0HNER84KQROAV", "RequestPath": "/api/roles"}
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB0", "RequestPath": "/api/roles/9/users"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB1", "RequestPath": "/api/users"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
[xUnit.net 00:00:01.85]     UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetRoleUsers_WithManyUsersPerRole_ShouldScaleWell [FAIL]
[xUnit.net 00:00:01.85]       Expected result.Data!.Count to be greater than 5 because Should return multiple users for the role, but found 0 (difference of -5).
[xUnit.net 00:00:01.85]       Stack Trace:
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.85]            at FluentAssertions.Numeric.NumericAssertions`2.BeGreaterThan(T expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.85]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(232,0): at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetRoleUsers_WithManyUsersPerRole_ShouldScaleWell()
[xUnit.net 00:00:01.85]         --- End of stack trace from previous location ---
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB2", "RequestPath": "/api/users/4"}
?? Ensuring clean database state...
[xUnit.net 00:00:01.85]     UserService.IntegrationTests.Controllers.UsersControllerTests.DeleteUser_CrossTenantAttempt_ReturnsNotFound [FAIL]
[xUnit.net 00:00:01.85]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.85]       Stack Trace:
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
? Database recreated with clean state
[xUnit.net 00:00:01.85]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.85]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.85]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(415,0): at UserService.IntegrationTests.Controllers.UsersControllerTests.DeleteUser_CrossTenantAttempt_ReturnsNotFound()
[xUnit.net 00:00:01.85]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB3", "RequestPath": "/api/users/4"}
[xUnit.net 00:00:01.86]     UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_CrossTenantAccess_ReturnsNotFound [FAIL]
[xUnit.net 00:00:01.86]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
?? Ensuring clean database state...
[xUnit.net 00:00:01.86]       Stack Trace:
[xUnit.net 00:00:01.86]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.86]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.86]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.86]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.86]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.86]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.86]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.86]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(329,0): at UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_CrossTenantAccess_ReturnsNotFound()
[xUnit.net 00:00:01.86]         --- End of stack trace from previous location ---
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 8
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
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB4", "RequestPath": "/api/users"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB5", "RequestPath": "/api/users/2/roles"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
?? Ensuring clean database state...
[xUnit.net 00:00:01.89]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.RemoveRoleFromUser_ShouldLogRemovalDetails [FAIL]
[xUnit.net 00:00:01.89]       Expected removalLogs not to be empty because Role removal should generate audit log entries.
[xUnit.net 00:00:01.89]       Stack Trace:
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.89]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
? Created 2 tenants
[xUnit.net 00:00:01.89]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
?? Creating permissions...
[xUnit.net 00:00:01.89]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(239,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.RemoveRoleFromUser_ShouldLogRemovalDetails()
[xUnit.net 00:00:01.89]         --- End of stack trace from previous location ---
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 64 role permissions
? Created 8 user role assignments
?? Creating users...
?? Creating legacy tenant users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB6", "RequestPath": "/api/users/2/status"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 DBG] Policy authentication schemes  did not succeed {"SourceContext": "Microsoft.AspNetCore.Authorization.AuthorizationMiddleware", "RequestId": "0HNER84KQROB7", "RequestPath": "/api/users/profile"}
[09:45:08 INF] Authorization failed. These requirements were not met:
DenyAnonymousAuthorizationRequirement: Requires an authenticated user. {"EventId": {"Id": 2, "Name": "UserAuthorizationFailed"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB7", "RequestPath": "/api/users/profile"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB8", "RequestPath": "/api/roles"}
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROB9", "RequestPath": "/api/users/999"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBA", "RequestPath": "/api/roles"}
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBB", "RequestPath": "/api/users"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant2.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for user@tenant2.com:
   - User ID: 7
   - User TenantId: 2
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=8, TenantId=2, Active=True
?? TENANT DEBUG:
   - Tenant ID: 2
   - Tenant Name: Test Tenant 2
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=8, RoleName=User
?? USER user@tenant2.com in TENANT 2:
   - UserRoles: 1
   - Permissions: 1 [roles.view]
?? JWT CLAIMS for user@tenant2.com:
   - Role: User (Actual: User)
   - Tenant: Test Tenant 2 (ID: 2)
   - Permissions: 1
?? JWT CLAIMS for user@tenant2.com:
   - Role: User
   - Tenant: Test Tenant 2 (ID: 2)
   - Permissions: 1
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBC", "RequestPath": "/api/users/4"}
[09:45:08 INF] Starting UserService {}
=== UserService Starting ===
[xUnit.net 00:00:02.15]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldBeStructuredAndParseable [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:02.15]       Expected structuredLogs not to be empty because Should generate structured audit logs.
[xUnit.net 00:00:02.16]       Stack Trace:
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.16]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[xUnit.net 00:00:02.16]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:02.16]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(443,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldBeStructuredAndParseable()
[xUnit.net 00:00:02.16]         --- End of stack trace from previous location ---
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for user@tenant1.com:
   - Total UserRoles in DB: 8
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
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBD", "RequestPath": "/api/roles"}
[xUnit.net 00:00:02.32]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UnauthorizedRoleAccess_ShouldLogSecurityEvents [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:02.32]       Expected securityLogs not to be empty because Unauthorized access attempts should generate security audit entries.
[xUnit.net 00:00:02.32]       Stack Trace:
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.32]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[xUnit.net 00:00:02.32]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:02.32]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(328,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UnauthorizedRoleAccess_ShouldLogSecurityEvents()
[xUnit.net 00:00:02.32]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBE", "RequestPath": "/api/roles/5"}
?? Ensuring clean database state...
[xUnit.net 00:00:02.38]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CrossTenantAccess_ShouldLogSecurityViolations [FAIL]
[xUnit.net 00:00:02.38]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:02.38]       Stack Trace:
[xUnit.net 00:00:02.38]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:02.38]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:02.38]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:02.38]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.38]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.38]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.38]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:02.38]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(347,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CrossTenantAccess_ShouldLogSecurityViolations()
[xUnit.net 00:00:02.38]         --- End of stack trace from previous location ---
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:08 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:08 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBF", "RequestPath": "/api/roles"}
[09:45:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBG", "RequestPath": "/api/roles/9"}
?? Ensuring clean database state...
[xUnit.net 00:00:02.71]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.DeleteRole_ShouldLogDeletionWithDetails [FAIL]
[xUnit.net 00:00:02.71]       Expected deletionLogs not to be empty because Role deletion should generate audit log entries.
[xUnit.net 00:00:02.71]       Stack Trace:
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
? Database recreated with clean state
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.71]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[xUnit.net 00:00:02.71]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:02.71]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(158,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.DeleteRole_ShouldLogDeletionWithDetails()
[xUnit.net 00:00:02.71]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:09 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:09 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBH", "RequestPath": "/api/roles"}
[09:45:09 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBI", "RequestPath": "/api/roles/9"}
[xUnit.net 00:00:03.00]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRole_ShouldLogPermissionChanges [FAIL]
[xUnit.net 00:00:03.00]       Expected updateLogs not to be empty because Role updates should generate audit log entries.
[xUnit.net 00:00:03.00]       Stack Trace:
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:03.00]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[xUnit.net 00:00:03.00]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:03.00]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(116,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRole_ShouldLogPermissionChanges()
[xUnit.net 00:00:03.00]         --- End of stack trace from previous location ---
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:09 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:09 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBJ", "RequestPath": "/api/roles"}
[09:45:09 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBK", "RequestPath": "/api/roles/9/permissions"}
[xUnit.net 00:00:03.28]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRolePermissions_ShouldLogPermissionChanges [FAIL]
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:03.28]       Expected permissionLogs not to be empty because Permission updates should generate audit log entries.
[xUnit.net 00:00:03.28]       Stack Trace:
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:03.28]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[xUnit.net 00:00:03.28]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:03.28]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(286,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRolePermissions_ShouldLogPermissionChanges()
[xUnit.net 00:00:03.28]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
? Created 20 permissions
?? Creating roles...
? Created 8 roles
?? Creating role permissions...
? Created 64 role permissions
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Saving users to database...
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64
? Verified user: admin@tenant1.com (ID: 1, TenantId: 1)
? Verified user: user@tenant1.com (ID: 2, TenantId: 1)
? Verified user: admin@tenant2.com (ID: 6, TenantId: 2)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[09:45:09 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[09:45:09 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
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
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[09:45:09 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNER84KQROBL", "RequestPath": "/api/roles"}
[xUnit.net 00:00:03.54]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CreateRole_ShouldGenerateAuditLogEntry [FAIL]
[xUnit.net 00:00:03.54]       Expected roleCreationLogs not to be empty because Role creation should generate audit log entries.
[xUnit.net 00:00:03.54]       Stack Trace:
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:03.54]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
[xUnit.net 00:00:03.54]            at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
[xUnit.net 00:00:03.54]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(61,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CreateRole_ShouldGenerateAuditLogEntry()
[xUnit.net 00:00:03.54]         --- End of stack trace from previous location ---
[xUnit.net 00:00:03.55]   Finished:    UserService.IntegrationTests
  UserService.IntegrationTests test failed with 43 error(s) (4.1s)
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(389): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.SystemRoles_ShouldBeReadOnlyForTenantAdmins (1s 12m
      s): Error Message: Expected updateResponse.StatusCode to be HttpStatusCode.InternalServerError {value: 500}, but f
      ound HttpStatusCode.NotFound {value: 404}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.SystemRoles_ShouldBeReadOnlyForTenantAdmins()
       in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs:
      line 389
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(438): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_ConcurrentUpdates_Sho
      uldHandleGracefully (50ms): Error Message: Expected response.StatusCode to be HttpStatusCode.OK {value: 200}, but
      found HttpStatusCode.InternalServerError {value: 500}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_ConcurrentUpdat
      es_ShouldHandleGracefully() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controller
      s\RolePermissionManagementTests.cs:line 438
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(41): error TESTERROR:
      UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithEmptyName_ShouldReturnBadRequ
      est (59ms): Error Message: Expected response.StatusCode to be HttpStatusCode.BadRequest {value: 400}, but found Ht
      tpStatusCode.Created {value: 201}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithEmptyName_ShouldReturnB
      adRequest() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErro
      rHandlingTests.cs:line 41
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(352): error TESTERROR:
      UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.AssignRoles_ConcurrentAssignments_ShouldHandleRac
      eConditions (1s 34ms): Error Message: System.Text.Json.JsonException : The JSON value could not be converted to Sy
      stem.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
      Stack Trace:
         at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
         at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConver
      t, JsonSerializerOptions options, ReadStack& state, TCollection& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state
      , Utf8JsonReader& reader)
         at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type ty
      peToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOpti
      ons options, ReadStack& state)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, Jso
      nReaderState& jsonReaderState, ReadStack& readStack, T& value)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken c
      ancellationToken)
         at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerO
      ptions options, CancellationToken cancellationToken)
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.CreateTestUserForLoadTesting() in C:\Users\
      mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs:line 352
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.AssignRoles_ConcurrentAssignments_ShouldHan
      dleRaceConditions() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAs
      signmentLoadTests.cs:line 86
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(34): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithSystemRole_Should
      BeRejected (5ms): Error Message: Expected response.StatusCode to be HttpStatusCode.InternalServerError {value: 500
      }, but found HttpStatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithSystemRole_
      ShouldBeRejected() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePer
      missionManagementTests.cs:line 34
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(177): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.AssignRoleToUser_CrossTenantUser_ShouldFail (12ms):
       Error Message: Expected response.StatusCode to be HttpStatusCode.InternalServerError {value: 500}, but found Http
      StatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.AssignRoleToUser_CrossTenantUser_ShouldFail()
       in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs:
      line 177
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(410): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolesControllerTests.GetRolePermissions_CrossTenantAttempt_ReturnsNotFoun
      d (10ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpSt
      atusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolesControllerTests.GetRolePermissions_CrossTenantAttempt_ReturnsN
      otFound() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerT
      ests.cs:line 410
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(194): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.RemoveRoleFromUser_CrossTenantRole_ShouldFail (9ms)
      : Error Message: Expected response.StatusCode to be HttpStatusCode.InternalServerError {value: 500}, but found Htt
      pStatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.RemoveRoleFromUser_CrossTenantRole_ShouldFail
      () in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.c
      s:line 194
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(190): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithLargePermissionSe
      t_ShouldSucceed (12ms): Error Message: Expected permissionsResult!.Data!.Count to be greater than 50, but found 4
      (difference of -46).
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Numeric.NumericAssertions`2.BeGreaterThan(T expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithLargePermis
      sionSet_ShouldSucceed() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\Ro
      lePermissionManagementTests.cs:line 190
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(110): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.UpdateRolePermissions_CrossTenantAttempt_ShouldRetu
      rnNotFound (9ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but foun
      d HttpStatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.UpdateRolePermissions_CrossTenantAttempt_Shou
      ldReturnNotFound() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenan
      tSecurityTests.cs:line 110
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(198): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AssignRoleToUser_ShouldLogAssignmentDetails (1
      s 101ms): Error Message: Expected assignmentLogs not to be empty because Role assignment should generate audit log
       entries.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AssignRoleToUser_ShouldLogAssignmentDeta
      ils() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificatio
      nTests.cs:line 198
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(84): error TESTERROR:
      UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithExtremelyLongName_ShouldRetur
      nBadRequest (10ms): Error Message: Expected response.StatusCode to be one of {HttpStatusCode.BadRequest {value: 40
      0}, HttpStatusCode.InternalServerError {value: 500}}, but found HttpStatusCode.Created {value: 201}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(IEnumerable`1 validValues, String because, Object[] bec
      auseArgs)
         at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(TEnum[] validValues)
         at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithExtremelyLongName_Shoul
      dReturnBadRequest() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\Edge
      CaseErrorHandlingTests.cs:line 84
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(154): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.AssignRoleToUser_CrossTenantRole_ShouldFail (6ms):
      Error Message: Expected response.StatusCode to be HttpStatusCode.InternalServerError {value: 500}, but found HttpS
      tatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.AssignRoleToUser_CrossTenantRole_ShouldFail()
       in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs:
      line 154
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(352): error TESTERROR:
      UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.AssignRoles_BulkAssignments_ShouldCompleteWithinR
      easonableTime (87ms): Error Message: System.Text.Json.JsonException : The JSON value could not be converted to Sys
      tem.Collections.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
      Stack Trace:
         at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
         at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConver
      t, JsonSerializerOptions options, ReadStack& state, TCollection& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state
      , Utf8JsonReader& reader)
         at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type ty
      peToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOpti
      ons options, ReadStack& state)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, Jso
      nReaderState& jsonReaderState, ReadStack& readStack, T& value)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken c
      ancellationToken)
         at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerO
      ptions options, CancellationToken cancellationToken)
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.CreateTestUserForLoadTesting() in C:\Users\
      mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs:line 352
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.AssignRoles_BulkAssignments_ShouldCompleteW
      ithinReasonableTime() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\Role
      AssignmentLoadTests.cs:line 35
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(539): error TESTERROR:
      UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_ConcurrentCreationWithSameName_Sh
      ouldHandleRaceCondition (7ms): Error Message: Expected successCount to be 1, but found 2.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Numeric.NumericAssertions`2.Be(T expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_ConcurrentCreationWithSameN
      ame_ShouldHandleRaceCondition() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorH
      andling\EdgeCaseErrorHandlingTests.cs:line 539
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(298): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRole_ShouldPreserveExistingPermission
      s_WhenNotSpecified (10ms): Error Message: Expected permissionsResult!.Data! to be empty, but found {"users.view",
      "roles.view", "reports.view"}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.BeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRole_ShouldPreserveExistingPerm
      issions_WhenNotSpecified() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers
      \RolePermissionManagementTests.cs:line 298
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(391): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldContainRequiredInformation (14
      5ms): Error Message: Expected auditLogs not to be empty.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldContainRequiredInformati
      on() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerification
      Tests.cs:line 391
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolePermissionManagementTests.cs(69): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithEmptyPermissionLi
      st_ShouldSucceed (9ms): Error Message: Expected permissionsResult!.Data to be empty, but found {"users.view", "rol
      es.view"}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.BeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolePermissionManagementTests.UpdateRolePermissions_WithEmptyPermis
      sionList_ShouldSucceed() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\R
      olePermissionManagementTests.cs:line 69
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(91): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRolePermissions_CrossTenantAttempt_ShouldReturnN
      otFound (5ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found H
      ttpStatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRolePermissions_CrossTenantAttempt_ShouldR
      eturnNotFound() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSe
      curityTests.cs:line 91
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(331): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_WithViewerRole_ReturnsAssignedUsers (2m
      s): Error Message: Expected response.StatusCode to be HttpStatusCode.OK {value: 200}, but found HttpStatusCode.Not
      Found {value: 404}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_WithViewerRole_ReturnsAssignedUse
      rs() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.
      cs:line 331
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(396): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_CrossTenantAttempt_ReturnsNotFound (6ms
      ): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCod
      e.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolesControllerTests.GetRoleUsers_CrossTenantAttempt_ReturnsNotFoun
      d() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.c
      s:line 396
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(318): error TESTERROR:
      UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.DeleteRole_WithUsersAssigned_ShouldReturnCon
      flict (8ms): Error Message: Expected result.Message "An error occurred while deleting the role" to contain "has us
      ers assigned".
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.StringAssertions`1.Contain(String expected, String because, Object[] becauseArgs
      )
         at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.DeleteRole_WithUsersAssigned_ShouldRet
      urnConflict() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseEr
      rorHandlingTests.cs:line 318
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(127): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRoleUsers_CrossTenantAttempt_ShouldReturnNotFoun
      d (10ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpSt
      atusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRoleUsers_CrossTenantAttempt_ShouldReturnN
      otFound() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurity
      Tests.cs:line 127
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(352): error TESTERROR:
      UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetUserRoles_WithManyRoles_ShouldMaintainPerforma
      nce (82ms): Error Message: System.Text.Json.JsonException : The JSON value could not be converted to System.Collec
      tions.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
      Stack Trace:
         at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
         at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConver
      t, JsonSerializerOptions options, ReadStack& state, TCollection& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state
      , Utf8JsonReader& reader)
         at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type ty
      peToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOpti
      ons options, ReadStack& state)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, Jso
      nReaderState& jsonReaderState, ReadStack& readStack, T& value)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken c
      ancellationToken)
         at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerO
      ptions options, CancellationToken cancellationToken)
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.CreateTestUserForLoadTesting() in C:\Users\
      mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs:line 352
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetUserRoles_WithManyRoles_ShouldMaintainPe
      rformance() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignment
      LoadTests.cs:line 170
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(113): error TESTERROR:
      UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithSpecialCharacters_ShouldHandl
      eGracefully (6ms): Error Message: Did not expect result.Data!.Description "Description with special chars: <script
      >alert('xss')</script>" to contain "<script>".
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.StringAssertions`1.NotContain(String unexpected, String because, Object[] becaus
      eArgs)
         at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.CreateRole_WithSpecialCharacters_Shoul
      dHandleGracefully() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\Edge
      CaseErrorHandlingTests.cs:line 113
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(33): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRole_CrossTenantAttempt_ShouldReturnNotFound (7m
      s): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCo
      de.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetRole_CrossTenantAttempt_ShouldReturnNotFou
      nd() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests
      .cs:line 33
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(57): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.UpdateRole_CrossTenantAttempt_ShouldReturnNotFound
      (7ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatu
      sCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.UpdateRole_CrossTenantAttempt_ShouldReturnNot
      Found() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTe
      sts.cs:line 57
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseErrorHandlingTests.cs(157): error TESTERROR:
      UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.UpdateRole_WithDuplicateName_ShouldReturnCon
      flict (14ms): Error Message: Expected result.Message "An error occurred while updating the role" to contain "alrea
      dy exists".
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.StringAssertions`1.Contain(String expected, String because, Object[] becauseArgs
      )
         at UserService.IntegrationTests.ErrorHandling.EdgeCaseErrorHandlingTests.UpdateRole_WithDuplicateName_ShouldRet
      urnConflict() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\ErrorHandling\EdgeCaseEr
      rorHandlingTests.cs:line 157
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(74): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.DeleteRole_CrossTenantAttempt_ShouldReturnNotFound
      (12ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStat
      usCode.InternalServerError {value: 500}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.DeleteRole_CrossTenantAttempt_ShouldReturnNot
      Found() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTe
      sts.cs:line 74
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(352): error TESTERROR:
      UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.RemoveRoles_BulkRemoval_ShouldCompleteEfficiently
       (80ms): Error Message: System.Text.Json.JsonException : The JSON value could not be converted to System.Collectio
      ns.Generic.List`1[DTOs.Common.ErrorDto]. Path: $.errors | LineNumber: 0 | BytePositionInLine: 134.
      Stack Trace:
         at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
         at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConver
      t, JsonSerializerOptions options, ReadStack& state, TCollection& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state
      , Utf8JsonReader& reader)
         at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type ty
      peToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
         at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSeria
      lizerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
         at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOpti
      ons options, ReadStack& state)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, Jso
      nReaderState& jsonReaderState, ReadStack& readStack, T& value)
         at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken c
      ancellationToken)
         at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerO
      ptions options, CancellationToken cancellationToken)
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.CreateTestUserForLoadTesting() in C:\Users\
      mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs:line 352
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.RemoveRoles_BulkRemoval_ShouldCompleteEffic
      iently() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoa
      dTests.cs:line 123
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControllerTests.cs(545): error TESTERROR:
      UserService.IntegrationTests.Controllers.RolesControllerTests.UpdateRolePermissions_RemoveAllPermissions_ReturnsSu
      ccess (6ms): Error Message: Expected permissionsResult!.Data to be empty, but found {"users.view", "roles.view"}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.BeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.RolesControllerTests.UpdateRolePermissions_RemoveAllPermissions_Ret
      urnsSuccess() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\RolesControl
      lerTests.cs:line 545
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(210): error TESTERROR:
      UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetUserRoles_CrossTenantUser_ShouldReturnForbiddenO
      rNotFound (20ms): Error Message: Expected response.StatusCode to be one of {HttpStatusCode.Forbidden {value: 403},
       HttpStatusCode.NotFound {value: 404}}, but found HttpStatusCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(IEnumerable`1 validValues, String because, Object[] bec
      auseArgs)
         at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(TEnum[] validValues)
         at UserService.IntegrationTests.Security.CrossTenantSecurityTests.GetUserRoles_CrossTenantUser_ShouldReturnForb
      iddenOrNotFound() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenant
      SecurityTests.cs:line 210
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoadTests.cs(232): error TESTERROR:
      UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetRoleUsers_WithManyUsersPerRole_ShouldScaleWell
       (86ms): Error Message: Expected result.Data!.Count to be greater than 5 because Should return multiple users for
      the role, but found 0 (difference of -5).
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Numeric.NumericAssertions`2.BeGreaterThan(T expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.LoadTesting.RoleAssignmentLoadTests.GetRoleUsers_WithManyUsersPerRole_ShouldSca
      leWell() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\LoadTesting\RoleAssignmentLoa
      dTests.cs:line 232
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(415): error TESTERROR:
      UserService.IntegrationTests.Controllers.UsersControllerTests.DeleteUser_CrossTenantAttempt_ReturnsNotFound (4ms):
       Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.
      OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.UsersControllerTests.DeleteUser_CrossTenantAttempt_ReturnsNotFound(
      ) in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs:
      line 415
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs(329): error TESTERROR:
      UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_CrossTenantAccess_ReturnsNotFound (2ms): Err
      or Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {
      value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Controllers.UsersControllerTests.GetUser_CrossTenantAccess_ReturnsNotFound() in
       C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\UsersControllerTests.cs:line
       329
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(239): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.RemoveRoleFromUser_ShouldLogRemovalDetails (26
      9ms): Error Message: Expected removalLogs not to be empty because Role removal should generate audit log entries.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.RemoveRoleFromUser_ShouldLogRemovalDetai
      ls() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerification
      Tests.cs:line 239
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(443): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldBeStructuredAndParseable (254m
      s): Error Message: Expected structuredLogs not to be empty because Should generate structured audit logs.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.AuditLogs_ShouldBeStructuredAndParseable
      () in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTe
      sts.cs:line 443
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(328): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UnauthorizedRoleAccess_ShouldLogSecurityEvents
       (153ms): Error Message: Expected securityLogs not to be empty because Unauthorized access attempts should generat
      e security audit entries.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UnauthorizedRoleAccess_ShouldLogSecurity
      Events() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerifica
      tionTests.cs:line 328
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(347): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CrossTenantAccess_ShouldLogSecurityViolations
      (38ms): Error Message: Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStat
      usCode.OK {value: 200}.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CrossTenantAccess_ShouldLogSecurityViola
      tions() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificat
      ionTests.cs:line 347
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(158): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.DeleteRole_ShouldLogDeletionWithDetails (310ms
      ): Error Message: Expected deletionLogs not to be empty because Role deletion should generate audit log entries.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.DeleteRole_ShouldLogDeletionWithDetails(
      ) in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTes
      ts.cs:line 158
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(116): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRole_ShouldLogPermissionChanges (274ms):
       Error Message: Expected updateLogs not to be empty because Role updates should generate audit log entries.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRole_ShouldLogPermissionChanges()
      in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests
      .cs:line 116
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(286): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRolePermissions_ShouldLogPermissionChang
      es (255ms): Error Message: Expected permissionLogs not to be empty because Permission updates should generate audi
      t log entries.
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.UpdateRolePermissions_ShouldLogPermissio
      nChanges() in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerifi
      cationTests.cs:line 286
      --- End of stack trace from previous location ---
    C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(61): error TESTERROR:
      UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CreateRole_ShouldGenerateAuditLogEntry (183ms)
      : Error Message: Expected roleCreationLogs not to be empty because Role creation should generate audit log entries
      .
      Stack Trace:
         at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
         at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
         at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
         at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
         at FluentAssertions.Execution.GivenSelector`1.FailWith(String message)
         at FluentAssertions.Collections.GenericCollectionAssertions`3.NotBeEmpty(String because, Object[] becauseArgs)
         at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CreateRole_ShouldGenerateAuditLogEntry()
       in C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTest
      s.cs:line 61
      --- End of stack trace from previous location ---

Test summary: total: 134, failed: 43, succeeded: 91, skipped: 0, duration: 4.0s
Build failed with 43 error(s) in 5.5s
