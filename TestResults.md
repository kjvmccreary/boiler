Building Test Projects
========== Starting test run ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.05]   Starting:    UserService.IntegrationTests
[17:57:30 INF] Starting UserService {}
[17:57:30 INF] Starting UserService {}
=== UserService Starting ===
=== UserService Starting ===
[17:57:31 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
?? Ensuring clean database state...
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
?? Creating tenants...
? Created 2 tenants
? Created 2 tenants
?? Creating permissions...
?? Creating permissions...
? Created 20 permissions
? Created 20 permissions
?? Creating roles...
?? Creating roles...
? Created 8 roles
? Created 8 roles
?? Creating role permissions...
?? Creating role permissions...
? Created 64 role permissions
? Created 64 role permissions
?? Creating users...
?? Creating users...
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Found Tenant 1: ID=1, Name=Test Tenant 1
?? Found Tenant 2: ID=2, Name=Test Tenant 2
?? Adding 7 users to database...
?? Adding 7 users to database...
?? Saving users to database...
?? Saving users to database...
? Created 7 users
? Created 7 users
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Tenant 1 users: 5
?? Tenant 2 users: 2
?? Creating user role assignments...
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
?? Adding 8 user role assignments...
? Created 8 user role assignments
? Created 8 user role assignments
?? Creating legacy tenant users...
?? Creating legacy tenant users...
? Created 7 legacy tenant users
? Created 7 legacy tenant users
?? Verifying test data...
?? Verifying test data...
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
[17:57:31 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=20, UserRoles=8, RolePermissions=64 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - TenantId: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - PrimaryTenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:57:31 INF]    - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? DEBUGGING UserRole Query for admin@tenant1.com:
   - Total UserRoles in DB: 8
   - UserRoles for this email: 1
   - Active UserRoles for this email: 1
?? USER ENTITY DEBUG for admin@tenant1.com:
?? USER ENTITY DEBUG for admin@tenant1.com:
   - User ID: 1
   - User ID: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - User TenantId: 1
   - Separately loaded UserRoles count: 1
   - UserRoles details: RoleId=2, TenantId=1, Active=True
   - UserRoles details: RoleId=2, TenantId=1, Active=True
?? TENANT DEBUG:
?? TENANT DEBUG:
   - Tenant ID: 1
   - Tenant ID: 1
   - Tenant Name: Test Tenant 1
   - Tenant Name: Test Tenant 1
?? FILTERING DEBUG:
?? FILTERING DEBUG:
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Total loaded UserRoles: 1
   - UserRoles after TenantId filter: 1
   - Relevant UserRoles: RoleId=2, RoleName=Admin
   - Relevant UserRoles: RoleId=2, RoleName=Admin
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
?? USER admin@tenant1.com in TENANT 1:
   - UserRoles: 1
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
   - Permissions: 18 [users.edit, users.view, users.create, users.delete, users.manage, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.manage_permissions, tenants.view, tenants.edit, reports.view, reports.create, reports.export, permissions.view, permissions.manage]
?? JWT CLAIMS for admin@tenant1.com:
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin (Actual: Admin)
   - Role: Admin (Actual: Admin)
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
?? JWT CLAIMS for admin@tenant1.com:
   - Role: Admin
   - Tenant: Test Tenant 1 (ID: 1)
   - Permissions: 18
[17:57:31 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNERGNPL22R2", "RequestPath": "/api/roles/7/users/2"}
[17:57:31 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNERGNPL22R1", "RequestPath": "/api/roles/5"}
[17:57:31 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNERGNPL22R2", "RequestPath": "/api/roles/7/users/2"}
[17:57:31 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNERGNPL22R1", "RequestPath": "/api/roles/5"}
[17:57:31 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNERGNPL22R1", "RequestPath": "/api/roles/5"}
[17:57:31 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNERGNPL22R2", "RequestPath": "/api/roles/7/users/2"}
[17:57:31 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNERGNPL22R2", "RequestPath": "/api/roles/7/users/2"}
[17:57:31 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNERGNPL22R1", "RequestPath": "/api/roles/5"}
[17:57:31 INF] HTTP GET /api/roles/5 responded 200 in 59.3676 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[17:57:31 INF] HTTP DELETE /api/roles/7/users/2 responded 200 in 59.2106 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.09]     UserService.IntegrationTests.Security.CrossTenantSecurityTests.RemoveRoleFromUser_CrossTenantRole_ShouldFail [FAIL]
[xUnit.net 00:00:01.09]       Expected response.StatusCode to be HttpStatusCode.InternalServerError {value: 500}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.09]       Stack Trace:
[xUnit.net 00:00:01.09]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.09]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.09]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.09]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.09]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.09]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.09]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.09]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\CrossTenantSecurityTests.cs(194,0): at UserService.IntegrationTests.Security.CrossTenantSecurityTests.RemoveRoleFromUser_CrossTenantRole_ShouldFail()
[xUnit.net 00:00:01.09]         --- End of stack trace from previous location ---
[xUnit.net 00:00:01.10]     UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CrossTenantAccess_ShouldLogSecurityViolations [FAIL]
[xUnit.net 00:00:01.10]       Expected response.StatusCode to be HttpStatusCode.NotFound {value: 404}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.10]       Stack Trace:
[xUnit.net 00:00:01.10]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.10]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.10]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.10]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.10]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.10]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.10]            at FluentAssertions.Primitives.EnumAssertions`2.Be(TEnum expected, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.10]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Auditing\AuditLoggingVerificationTests.cs(328,0): at UserService.IntegrationTests.Auditing.AuditLoggingVerificationTests.CrossTenantAccess_ShouldLogSecurityViolations()
[xUnit.net 00:00:01.10]         --- End of stack trace from previous location ---
[xUnit.net 00:00:01.11]   Finished:    UserService.IntegrationTests
========== Test run finished: 2 Tests (0 Passed, 2 Failed, 0 Skipped) run in 1.1 sec ==========
