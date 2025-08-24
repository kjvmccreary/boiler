Building Test Projects
Starting test discovery for requested test run
========== Starting test discovery ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.07]   Discovering: UserService.IntegrationTests
[xUnit.net 00:00:00.11]   Discovered:  UserService.IntegrationTests
========== Test discovery finished: 210 Tests found in 532.7 ms ==========
========== Starting test run ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.03]   Starting:    ApiGateway.Tests
[xUnit.net 00:00:00.82]   Finished:    ApiGateway.Tests
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.05]   Starting:    UserService.IntegrationTests
=== UserService Startup Diagnostics ===
Environment: Testing
Application Name: UserService
Content Root: C:\Users\mccre\dev\boiler\src\services\UserService
Running in Container: 
[Startup] Monitoring:Enabled raw config value = 'False'
?? Registering base services...
? Base services registered
?? Configuring Swagger...
? Swagger configured
?? Registering common services...
? Common services registered
?? Configuring Redis...
?? Redis connection string resolved to: localhost:6379
?? Performance/Testing environment: True
? Using in-memory cache services (Performance/Testing mode)
? Cache services configured
?? Registering database services...
? Database services registered
?? Registering user services...
? User services registered
?? Registering Enhanced Security and Monitoring services...
? Enhanced Security and Monitoring services registered successfully
?? Registering Compliance and Alert services...
? Compliance and Alert services registered successfully
?? Registering Enhanced Health Checks...
? Enhanced Health Checks registered
?? Configuring authorization policies...
? Authorization policies configured
?? Configuring CORS...
? CORS configured
?? Registering additional services...
? Additional services registered
??? Building application...
? Application built successfully
?? Configuring middleware pipeline...
?? Configuring Enhanced Security middleware...
[17:02:51 INF] Configuring Enhanced Security middleware {}
[17:02:51 INF] ?? Enhanced security middleware DISABLED in testing environment {"SourceContext": "SecurityExtensions"}
? Enhanced Security middleware configured successfully
[17:02:51 INF] Enhanced Security middleware configured successfully {}
? Middleware pipeline configured
?? Mapping Enhanced Health Check endpoints...
? Enhanced Health Check endpoints mapped
[17:02:51 INF] Starting UserService with Phase 11 Enhanced Security & Monitoring {}
=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===
?? Testing cache connection...
? In-memory cache configured (Performance/Testing mode)
[17:02:51 INF] In-memory cache configured for Performance/Testing environment {}
?? Seeding database...
[17:02:51 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
? Database seeded
?? Seeding monitoring user...
[17:02:51 INF] Monitoring.Enabled = 'false' (evaluated false); skipping seeding. {"SourceContext": "MonitoringUserSeeder"}
? Monitoring user seeded (monitor@local / ChangeMe123!)
?? Starting application...
?? Enhanced Health Check endpoints available:
  - GET /health (comprehensive overview)
  - GET /health/critical (database + Redis status)
  - GET /health/enhanced (Phase 11 enhanced monitoring)
  - GET /health/performance (cache and performance metrics)
  - GET /health/system (overall system health score)
  - GET /health/ready (Kubernetes readiness)
  - GET /health/live (Kubernetes liveness)
?? Enhanced Security & Monitoring features:
  - Rate limiting with tenant awareness
  - Security event detection and logging
  - Performance metrics collection with Redis storage
  - Enhanced audit trail for all actions
  - Comprehensive health monitoring with performance scoring
  - Real-time system metrics and alerts
[17:02:51 INF] Enhanced Security & Monitoring (Phase 11) configured and operational {}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
?? Creating 38 permissions from actual business requirements
   - billing.manage (Billing)
   - billing.process_payments (Billing)
   - billing.view (Billing)
   - billing.view_invoices (Billing)
   - permissions.create (Permissions)
   - permissions.delete (Permissions)
   - permissions.edit (Permissions)
   - permissions.manage (Permissions)
   - permissions.view (Permissions)
   - reports.create (Reports)
   - reports.export (Reports)
   - reports.schedule (Reports)
   - reports.view (Reports)
   - roles.assign_users (Roles)
   - roles.create (Roles)
   - roles.delete (Roles)
   - roles.edit (Roles)
   - roles.manage_permissions (Roles)
   - roles.view (Roles)
   - system.manage (System)
   - system.manage_backups (System)
   - system.manage_settings (System)
   - system.monitor (System)
   - system.view_logs (System)
   - system.view_metrics (System)
   - tenants.create (Tenants)
   - tenants.delete (Tenants)
   - tenants.edit (Tenants)
   - tenants.initialize (Tenants)
   - tenants.manage_settings (Tenants)
   - tenants.view (Tenants)
   - tenants.view_all (Tenants)
   - users.create (Users)
   - users.delete (Users)
   - users.edit (Users)
   - users.manage_roles (Users)
   - users.view (Users)
   - users.view_all (Users)
? Created 38 permissions
?? Creating roles...
?? CreateRoles DEBUG:
   - tenant1.Id = 1
   - tenant2.Id = 2
?? Roles to be created (8):
   1. SuperAdmin (TenantId: )
   2. Admin (TenantId: 1)
   3. User (TenantId: 1)
   4. Manager (TenantId: 1)
   5. Viewer (TenantId: 1)
   6. Editor (TenantId: 1)
   7. Admin (TenantId: 2)
   8. User (TenantId: 2)
? Created 8 roles
?? Verification - Created roles:
   - ID=8, Name='User', TenantId=2
   - ID=7, Name='Admin', TenantId=2
   - ID=6, Name='Editor', TenantId=1
   - ID=5, Name='Viewer', TenantId=1
   - ID=4, Name='Manager', TenantId=1
   - ID=3, Name='User', TenantId=1
   - ID=2, Name='Admin', TenantId=1
   - ID=1, Name='SuperAdmin', TenantId=
? Successfully created all 8 roles including 2 Tenant 2 roles
?? Creating role permissions...
?? Found 38 permissions and 8 roles
?? ALL LOADED ROLES:
   - ID=8, Name='User', TenantId=2
   - ID=7, Name='Admin', TenantId=2
   - ID=6, Name='Editor', TenantId=1
   - ID=5, Name='Viewer', TenantId=1
   - ID=4, Name='Manager', TenantId=1
   - ID=3, Name='User', TenantId=1
   - ID=2, Name='Admin', TenantId=1
   - ID=1, Name='SuperAdmin', TenantId=
? Assigned 38 permissions to SuperAdmin
? Assigned 28 permissions to Tenant1 Admin
? Assigned 3 permissions to Tenant1 User
? Assigned 6 permissions to Tenant1 Manager
? Assigned 3 permissions to Viewer
? Assigned 0 permissions to Editor (intentionally for testing)
?? TENANT 2 ADMIN DEBUG:
   - tenant2.Id = 2
   - All roles count: 8
   - Roles with TenantId = 2: 2
   - Admin roles: ID=7, TenantId=2, ID=2, TenantId=1
   - tenant2AdminRole found: True
   - tenant2AdminRole.Id = 7
?? DEBUG: Found 28 permissions for Tenant2 Admin
? Assigned 28 permissions to Tenant2 Admin (Expected: 23+)
? Assigned 3 permissions to Tenant2 User
? Created 109 role permissions using only actual business-defined permissions
?? Creating users...
?? Adding 7 users to main context
? All users created successfully
? Final user count: 7
? Verified required user exists: admin@tenant1.com (ID:  1)
? Verified required user exists: user@tenant1.com (ID:  2)
? Verified required user exists: admin@tenant2.com (ID:  6)
? Verified required user exists: user@tenant2.com (ID:  7)
?? All users created and verified successfully!
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Temporarily disabling query filters for verification...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109
? Verified user: admin@tenant1.com (ID: 1)
? Verified user: user@tenant1.com (ID: 2)
? Verified user: admin@tenant2.com (ID: 6)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[17:02:51 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:51 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? MOCKING two-phase flow for editor@tenant1.com  Tenant 1
? Phase 1 simulated: User editor@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? DEBUG: editor@tenant1.com has 3 permissions: [roles.view, reports.view, permissions.view]
?? DEBUG: Looking for permission: roles.view
?? DEBUG: Has permission in token: True
[17:02:51 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNF3B7S9LCSD", "RequestPath": "/api/roles"}
[17:02:51 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF3B7S9LCSD", "RequestPath": "/api/roles"}
[17:02:51 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF3B7S9LCSD", "RequestPath": "/api/roles"}
[17:02:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF3B7S9LCSD", "RequestPath": "/api/roles"}
[17:02:52 INF] HTTP GET /api/roles responded 200 in 161.2691 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
?? DEBUG: Response status: OK
?? DEBUG: Response body: {"success":true,"data":{"items":[{"id":1,"tenantId":0,"name":"SuperAdmin","description":"System super admin","isSystemRole":true,"isDefault":false,"userCount":0,"permissions":["users.view_all","users.view","users.manage_roles","users.edit","users.delete","users.create","tenants.view_all","tenants.view","tenants.manage_settings","tenants.initialize","tenants.edit","tenants.delete","tenants.create","system.view_metrics","system.view_logs","system.monitor","system.manage_settings","system.manage_backups","system.manage","roles.view","roles.manage_permissions","roles.edit","roles.delete","roles.create","roles.assign_users","reports.view","reports.schedule","reports.export","reports.create","permissions.view","permissions.manage","permissions.edit","permissions.delete","permissions.create","billing.view_invoices","billing.view","billing.manage","billing.process_payments"],"createdAt":"2025-08-24T22:02:51.6722885Z","updatedAt":"2025-08-24T22:02:51.6722885Z"},{"id":2,"tenantId":1,"name":"Admin","description":"Tenant admin","isSystemRole":false,"isDefault":false,"userCount":1,"permissions":["users.view","users.edit","users.create","users.delete","users.view_all","users.manage_roles","roles.view","roles.create","roles.edit","roles.delete","roles.assign_users","roles.manage_permissions","tenants.view","tenants.create","tenants.edit","tenants.delete","tenants.initialize","tenants.view_all","tenants.manage_settings","reports.view","reports.create","reports.export","reports.schedule","permissions.view","permissions.create","permissions.edit","permissions.delete","permissions.manage"],"createdAt":"2025-08-24T22:02:51.6722885Z","updatedAt":"2025-08-24T22:02:51.6722885Z"},{"id":6,"tenantId":1,"name":"Editor","description":"Content editor","isSystemRole":false,"isDefault":false,"userCount":1,"permissions":[],"createdAt":"2025-08-24T22:02:51.6722885Z","updatedAt":"2025-08-24T22:02:51.6722885Z"},{"id":4,"tenantId":1,"name":"Manager","description":"Department manager","isSystemRole":false,"isDefault":false,"userCount":1,"permissions":["users.view","users.edit","users.manage_roles","roles.view","reports.view","reports.create"],"createdAt":"2025-08-24T22:02:51.6722885Z","updatedAt":"2025-08-24T22:02:51.6722885Z"},{"id":3,"tenantId":1,"name":"User","description":"Regular user","isSystemRole":false,"isDefault":true,"userCount":1,"permissions":["roles.view","reports.view","permissions.view"],"createdAt":"2025-08-24T22:02:51.6722885Z","updatedAt":"2025-08-24T22:02:51.6722885Z"},{"id":5,"tenantId":1,"name":"Viewer","description":"Read-only access","isSystemRole":false,"isDefault":false,"userCount":2,"permissions":["roles.view","reports.view","permissions.view"],"createdAt":"2025-08-24T22:02:51.6722885Z","updatedAt":"2025-08-24T22:02:51.6722885Z"}],"totalCount":6,"pageNumber":1,"pageSize":10,"totalPages":1,"hasPreviousPage":false,"hasNextPage":false},"message":"Roles retrieved successfully","errors":[],"traceId":null}
?? Ensuring clean database state...
[xUnit.net 00:00:01.32]     UserService.IntegrationTests.Security.PermissionBoundaryTests.PermissionCheck_WithTenantContext_ShouldEnforceProperAccess(userEmail: "editor@tenant1.com", endpoint: "/api/roles", requiredPermission: "roles.view", shouldHaveAccess: False) [FAIL]
[xUnit.net 00:00:01.32]       Expected response.StatusCode to be one of {HttpStatusCode.Forbidden {value: 403}, HttpStatusCode.Unauthorized {value: 401}}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.32]       Stack Trace:
[xUnit.net 00:00:01.32]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.32]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.32]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.32]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.32]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.32]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.32]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(IEnumerable`1 validValues, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.32]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(TEnum[] validValues)
[xUnit.net 00:00:01.32]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\PermissionBoundaryTests.cs(127,0): at UserService.IntegrationTests.Security.PermissionBoundaryTests.PermissionCheck_WithTenantContext_ShouldEnforceProperAccess(String userEmail, String endpoint, String requiredPermission, Boolean shouldHaveAccess)
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.32]         --- End of stack trace from previous location ---
? Created 2 tenants
?? Creating permissions...
?? Creating 38 permissions from actual business requirements
   - billing.manage (Billing)
   - billing.process_payments (Billing)
   - billing.view (Billing)
   - billing.view_invoices (Billing)
   - permissions.create (Permissions)
   - permissions.delete (Permissions)
   - permissions.edit (Permissions)
   - permissions.manage (Permissions)
   - permissions.view (Permissions)
   - reports.create (Reports)
   - reports.export (Reports)
   - reports.schedule (Reports)
   - reports.view (Reports)
   - roles.assign_users (Roles)
   - roles.create (Roles)
   - roles.delete (Roles)
   - roles.edit (Roles)
   - roles.manage_permissions (Roles)
   - roles.view (Roles)
   - system.manage (System)
   - system.manage_backups (System)
   - system.manage_settings (System)
   - system.monitor (System)
   - system.view_logs (System)
   - system.view_metrics (System)
   - tenants.create (Tenants)
   - tenants.delete (Tenants)
   - tenants.edit (Tenants)
   - tenants.initialize (Tenants)
   - tenants.manage_settings (Tenants)
   - tenants.view (Tenants)
   - tenants.view_all (Tenants)
   - users.create (Users)
   - users.delete (Users)
   - users.edit (Users)
   - users.manage_roles (Users)
   - users.view (Users)
   - users.view_all (Users)
? Created 38 permissions
?? Creating roles...
?? CreateRoles DEBUG:
   - tenant1.Id = 1
   - tenant2.Id = 2
?? Roles to be created (8):
   1. SuperAdmin (TenantId: )
   2. Admin (TenantId: 1)
   3. User (TenantId: 1)
   4. Manager (TenantId: 1)
   5. Viewer (TenantId: 1)
   6. Editor (TenantId: 1)
   7. Admin (TenantId: 2)
   8. User (TenantId: 2)
? Created 8 roles
?? Verification - Created roles:
   - ID=8, Name='User', TenantId=2
   - ID=7, Name='Admin', TenantId=2
   - ID=6, Name='Editor', TenantId=1
   - ID=5, Name='Viewer', TenantId=1
   - ID=4, Name='Manager', TenantId=1
   - ID=3, Name='User', TenantId=1
   - ID=2, Name='Admin', TenantId=1
   - ID=1, Name='SuperAdmin', TenantId=
? Successfully created all 8 roles including 2 Tenant 2 roles
?? Creating role permissions...
?? Found 38 permissions and 8 roles
?? ALL LOADED ROLES:
   - ID=8, Name='User', TenantId=2
   - ID=7, Name='Admin', TenantId=2
   - ID=6, Name='Editor', TenantId=1
   - ID=5, Name='Viewer', TenantId=1
   - ID=4, Name='Manager', TenantId=1
   - ID=3, Name='User', TenantId=1
   - ID=2, Name='Admin', TenantId=1
   - ID=1, Name='SuperAdmin', TenantId=
? Assigned 38 permissions to SuperAdmin
? Assigned 28 permissions to Tenant1 Admin
? Assigned 3 permissions to Tenant1 User
? Assigned 6 permissions to Tenant1 Manager
? Assigned 3 permissions to Viewer
? Assigned 0 permissions to Editor (intentionally for testing)
?? TENANT 2 ADMIN DEBUG:
   - tenant2.Id = 2
   - All roles count: 8
   - Roles with TenantId = 2: 2
   - Admin roles: ID=7, TenantId=2, ID=2, TenantId=1
   - tenant2AdminRole found: True
   - tenant2AdminRole.Id = 7
?? DEBUG: Found 28 permissions for Tenant2 Admin
? Assigned 28 permissions to Tenant2 Admin (Expected: 23+)
? Assigned 3 permissions to Tenant2 User
? Created 109 role permissions using only actual business-defined permissions
?? Creating users...
?? Adding 7 users to main context
? All users created successfully
? Final user count: 7
? Verified required user exists: admin@tenant1.com (ID:  1)
? Verified required user exists: user@tenant1.com (ID:  2)
? Verified required user exists: admin@tenant2.com (ID:  6)
? Verified required user exists: user@tenant2.com (ID:  7)
?? All users created and verified successfully!
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Temporarily disabling query filters for verification...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109
? Verified user: admin@tenant1.com (ID: 1)
? Verified user: user@tenant1.com (ID: 2)
? Verified user: admin@tenant2.com (ID: 6)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[17:02:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? MOCKING two-phase flow for viewer@tenant1.com  Tenant 1
? Phase 1 simulated: User viewer@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? DEBUG: viewer@tenant1.com has 3 permissions: [roles.view, reports.view, permissions.view]
?? DEBUG: Looking for permission: roles.create
?? DEBUG: Has permission in token: False
[17:02:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF3B7S9LCSE", "RequestPath": "/api/roles"}
[17:02:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF3B7S9LCSE", "RequestPath": "/api/roles"}
[17:02:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF3B7S9LCSE", "RequestPath": "/api/roles"}
[17:02:52 INF] HTTP GET /api/roles responded 200 in 14.5110 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
?? DEBUG: Response status: OK
?? DEBUG: Response body: {"success":true,"data":{"items":[{"id":1,"tenantId":0,"name":"SuperAdmin","description":"System super admin","isSystemRole":true,"isDefault":false,"userCount":0,"permissions":["users.view_all","users.view","users.manage_roles","users.edit","users.delete","users.create","tenants.view_all","tenants.view","tenants.manage_settings","tenants.initialize","tenants.edit","tenants.delete","tenants.create","system.view_metrics","system.view_logs","system.monitor","system.manage_settings","system.manage_backups","system.manage","roles.view","roles.manage_permissions","roles.edit","roles.delete","roles.create","roles.assign_users","reports.view","reports.schedule","reports.export","reports.create","permissions.view","permissions.manage","permissions.edit","permissions.delete","permissions.create","billing.view_invoices","billing.view","billing.manage","billing.process_payments"],"createdAt":"2025-08-24T22:02:52.1457158Z","updatedAt":"2025-08-24T22:02:52.1457158Z"},{"id":2,"tenantId":1,"name":"Admin","description":"Tenant admin","isSystemRole":false,"isDefault":false,"userCount":1,"permissions":["users.view","users.edit","users.create","users.delete","users.view_all","users.manage_roles","roles.view","roles.create","roles.edit","roles.delete","roles.assign_users","roles.manage_permissions","tenants.view","tenants.create","tenants.edit","tenants.delete","tenants.initialize","tenants.view_all","tenants.manage_settings","reports.view","reports.create","reports.export","reports.schedule","permissions.view","permissions.create","permissions.edit","permissions.delete","permissions.manage"],"createdAt":"2025-08-24T22:02:52.1457158Z","updatedAt":"2025-08-24T22:02:52.1457158Z"},{"id":6,"tenantId":1,"name":"Editor","description":"Content editor","isSystemRole":false,"isDefault":false,"userCount":1,"permissions":[],"createdAt":"2025-08-24T22:02:52.1457158Z","updatedAt":"2025-08-24T22:02:52.1457158Z"},{"id":4,"tenantId":1,"name":"Manager","description":"Department manager","isSystemRole":false,"isDefault":false,"userCount":1,"permissions":["users.view","users.edit","users.manage_roles","roles.view","reports.view","reports.create"],"createdAt":"2025-08-24T22:02:52.1457158Z","updatedAt":"2025-08-24T22:02:52.1457158Z"},{"id":3,"tenantId":1,"name":"User","description":"Regular user","isSystemRole":false,"isDefault":true,"userCount":1,"permissions":["roles.view","reports.view","permissions.view"],"createdAt":"2025-08-24T22:02:52.1457158Z","updatedAt":"2025-08-24T22:02:52.1457158Z"},{"id":5,"tenantId":1,"name":"Viewer","description":"Read-only access","isSystemRole":false,"isDefault":false,"userCount":2,"permissions":["roles.view","reports.view","permissions.view"],"createdAt":"2025-08-24T22:02:52.1457158Z","updatedAt":"2025-08-24T22:02:52.1457158Z"}],"totalCount":6,"pageNumber":1,"pageSize":10,"totalPages":1,"hasPreviousPage":false,"hasNextPage":false},"message":"Roles retrieved successfully","errors":[],"traceId":null}
[xUnit.net 00:00:01.36]     UserService.IntegrationTests.Security.PermissionBoundaryTests.PermissionCheck_WithTenantContext_ShouldEnforceProperAccess(userEmail: "viewer@tenant1.com", endpoint: "/api/roles", requiredPermission: "roles.create", shouldHaveAccess: False) [FAIL]
[xUnit.net 00:00:01.36]       Expected response.StatusCode to be one of {HttpStatusCode.Forbidden {value: 403}, HttpStatusCode.Unauthorized {value: 401}}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.36]       Stack Trace:
[xUnit.net 00:00:01.36]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.36]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.36]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.36]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.36]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.36]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.36]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(IEnumerable`1 validValues, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.36]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(TEnum[] validValues)
[xUnit.net 00:00:01.36]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\PermissionBoundaryTests.cs(127,0): at UserService.IntegrationTests.Security.PermissionBoundaryTests.PermissionCheck_WithTenantContext_ShouldEnforceProperAccess(String userEmail, String endpoint, String requiredPermission, Boolean shouldHaveAccess)
[xUnit.net 00:00:01.36]         --- End of stack trace from previous location ---
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
? Created 2 tenants
?? Creating permissions...
?? Creating 38 permissions from actual business requirements
   - billing.manage (Billing)
   - billing.process_payments (Billing)
   - billing.view (Billing)
   - billing.view_invoices (Billing)
   - permissions.create (Permissions)
   - permissions.delete (Permissions)
   - permissions.edit (Permissions)
   - permissions.manage (Permissions)
   - permissions.view (Permissions)
   - reports.create (Reports)
   - reports.export (Reports)
   - reports.schedule (Reports)
   - reports.view (Reports)
   - roles.assign_users (Roles)
   - roles.create (Roles)
   - roles.delete (Roles)
   - roles.edit (Roles)
   - roles.manage_permissions (Roles)
   - roles.view (Roles)
   - system.manage (System)
   - system.manage_backups (System)
   - system.manage_settings (System)
   - system.monitor (System)
   - system.view_logs (System)
   - system.view_metrics (System)
   - tenants.create (Tenants)
   - tenants.delete (Tenants)
   - tenants.edit (Tenants)
   - tenants.initialize (Tenants)
   - tenants.manage_settings (Tenants)
   - tenants.view (Tenants)
   - tenants.view_all (Tenants)
   - users.create (Users)
   - users.delete (Users)
   - users.edit (Users)
   - users.manage_roles (Users)
   - users.view (Users)
   - users.view_all (Users)
? Created 38 permissions
?? Creating roles...
?? CreateRoles DEBUG:
   - tenant1.Id = 1
   - tenant2.Id = 2
?? Roles to be created (8):
   1. SuperAdmin (TenantId: )
   2. Admin (TenantId: 1)
   3. User (TenantId: 1)
   4. Manager (TenantId: 1)
   5. Viewer (TenantId: 1)
   6. Editor (TenantId: 1)
   7. Admin (TenantId: 2)
   8. User (TenantId: 2)
? Created 8 roles
?? Verification - Created roles:
   - ID=8, Name='User', TenantId=2
   - ID=7, Name='Admin', TenantId=2
   - ID=6, Name='Editor', TenantId=1
   - ID=5, Name='Viewer', TenantId=1
   - ID=4, Name='Manager', TenantId=1
   - ID=3, Name='User', TenantId=1
   - ID=2, Name='Admin', TenantId=1
   - ID=1, Name='SuperAdmin', TenantId=
? Successfully created all 8 roles including 2 Tenant 2 roles
?? Creating role permissions...
?? Found 38 permissions and 8 roles
?? ALL LOADED ROLES:
   - ID=8, Name='User', TenantId=2
   - ID=7, Name='Admin', TenantId=2
   - ID=6, Name='Editor', TenantId=1
   - ID=5, Name='Viewer', TenantId=1
   - ID=4, Name='Manager', TenantId=1
   - ID=3, Name='User', TenantId=1
   - ID=2, Name='Admin', TenantId=1
   - ID=1, Name='SuperAdmin', TenantId=
? Assigned 38 permissions to SuperAdmin
? Assigned 28 permissions to Tenant1 Admin
? Assigned 3 permissions to Tenant1 User
? Assigned 6 permissions to Tenant1 Manager
? Assigned 3 permissions to Viewer
? Assigned 0 permissions to Editor (intentionally for testing)
?? TENANT 2 ADMIN DEBUG:
   - tenant2.Id = 2
   - All roles count: 8
   - Roles with TenantId = 2: 2
   - Admin roles: ID=7, TenantId=2, ID=2, TenantId=1
   - tenant2AdminRole found: True
   - tenant2AdminRole.Id = 7
?? DEBUG: Found 28 permissions for Tenant2 Admin
? Assigned 28 permissions to Tenant2 Admin (Expected: 23+)
? Assigned 3 permissions to Tenant2 User
? Created 109 role permissions using only actual business-defined permissions
?? Creating users...
?? Adding 7 users to main context
? All users created successfully
? Final user count: 7
? Verified required user exists: admin@tenant1.com (ID:  1)
? Verified required user exists: user@tenant1.com (ID:  2)
? Verified required user exists: admin@tenant2.com (ID:  6)
? Verified required user exists: user@tenant2.com (ID:  7)
?? All users created and verified successfully!
?? Creating user role assignments...
?? Found 7 users and 8 roles for assignment
?? Adding 8 user role assignments...
? Created 8 user role assignments
?? Creating legacy tenant users...
? Created 7 legacy tenant users
?? Verifying test data...
?? Temporarily disabling query filters for verification...
?? Final counts: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109
? Verified user: admin@tenant1.com (ID: 1)
? Verified user: user@tenant1.com (ID: 2)
? Verified user: admin@tenant2.com (ID: 6)
? admin@tenant1.com has 1 role assignments
? TestDataSeeder: Test data seeding completed successfully
[17:02:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[17:02:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? MOCKING two-phase flow for editor@tenant1.com  Tenant 1
? Phase 1 simulated: User editor@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
[17:02:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF3B7S9LCSF", "RequestPath": "/api/roles"}
[17:02:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF3B7S9LCSF", "RequestPath": "/api/roles"}
[17:02:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF3B7S9LCSF", "RequestPath": "/api/roles"}
[17:02:52 INF] HTTP GET /api/roles responded 200 in 7.6281 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.39]     UserService.IntegrationTests.Security.PermissionBoundaryTests.UserWithoutPermissions_ShouldBeRejected [FAIL]
[xUnit.net 00:00:01.39]       Expected response.StatusCode to be one of {HttpStatusCode.Forbidden {value: 403}, HttpStatusCode.Unauthorized {value: 401}}, but found HttpStatusCode.OK {value: 200}.
[xUnit.net 00:00:01.39]       Stack Trace:
[xUnit.net 00:00:01.39]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.39]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.39]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.39]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.39]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.39]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.39]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(IEnumerable`1 validValues, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.39]            at FluentAssertions.Primitives.EnumAssertions`2.BeOneOf(TEnum[] validValues)
[xUnit.net 00:00:01.39]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\PermissionBoundaryTests.cs(164,0): at UserService.IntegrationTests.Security.PermissionBoundaryTests.UserWithoutPermissions_ShouldBeRejected()
[xUnit.net 00:00:01.39]         --- End of stack trace from previous location ---
=== UserService Startup Diagnostics ===
Environment: Testing
Application Name: UserService
Content Root: C:\Users\mccre\dev\boiler\src\services\UserService
Running in Container: 
[Startup] Monitoring:Enabled raw config value = 'False'
?? Registering base services...
? Base services registered
?? Configuring Swagger...
? Swagger configured
?? Registering common services...
? Common services registered
?? Configuring Redis...
?? Redis connection string resolved to: localhost:6379
?? Performance/Testing environment: True
? Using in-memory cache services (Performance/Testing mode)
? Cache services configured
?? Registering database services...
? Database services registered
?? Registering user services...
? User services registered
?? Registering Enhanced Security and Monitoring services...
? Enhanced Security and Monitoring services registered successfully
?? Registering Compliance and Alert services...
? Compliance and Alert services registered successfully
?? Registering Enhanced Health Checks...
? Enhanced Health Checks registered
?? Configuring authorization policies...
? Authorization policies configured
?? Configuring CORS...
? CORS configured
?? Registering additional services...
? Additional services registered
??? Building application...
? Application built successfully
?? Configuring middleware pipeline...
?? Configuring Enhanced Security middleware...
[17:02:52 INF] Configuring Enhanced Security middleware {}
[17:02:52 INF] ?? Enhanced security middleware DISABLED in testing environment {"SourceContext": "SecurityExtensions"}
? Enhanced Security middleware configured successfully
[17:02:52 INF] Enhanced Security middleware configured successfully {}
? Middleware pipeline configured
?? Mapping Enhanced Health Check endpoints...
? Enhanced Health Check endpoints mapped
[17:02:52 INF] Starting UserService with Phase 11 Enhanced Security & Monitoring {}
=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===
?? Testing cache connection...
? In-memory cache configured (Performance/Testing mode)
[17:02:52 INF] In-memory cache configured for Performance/Testing environment {}
?? Seeding database...
? Database seeded
?? Seeding monitoring user...
[17:02:52 INF] Monitoring.Enabled = 'false' (evaluated false); skipping seeding. {"SourceContext": "MonitoringUserSeeder"}
? Monitoring user seeded (monitor@local / ChangeMe123!)
?? Starting application...
?? Enhanced Health Check endpoints available:
  - GET /health (comprehensive overview)
  - GET /health/critical (database + Redis status)
  - GET /health/enhanced (Phase 11 enhanced monitoring)
  - GET /health/performance (cache and performance metrics)
  - GET /health/system (overall system health score)
  - GET /health/ready (Kubernetes readiness)
  - GET /health/live (Kubernetes liveness)
?? Enhanced Security & Monitoring features:
  - Rate limiting with tenant awareness
  - Security event detection and logging
  - Performance metrics collection with Redis storage
  - Enhanced audit trail for all actions
  - Comprehensive health monitoring with performance scoring
  - Real-time system metrics and alerts
[17:02:52 INF] Enhanced Security & Monitoring (Phase 11) configured and operational {}
[xUnit.net 00:00:01.44]   Finished:    UserService.IntegrationTests
========== Test run finished: 4 Tests (1 Passed, 3 Failed, 0 Skipped) run in 1.5 sec ==========
