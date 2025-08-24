Log level is set to Informational (Default).
Connected to test environment '< Local Windows Environment >'
Source code repository not available. Some features may not work as expected.
Test data store opened in 0.085 sec.
Test data store opened in 0.032 sec.
========== Starting test discovery ==========
========== Test discovery skipped: All test containers are up to date ==========
Building Test Projects
Starting test discovery for requested test run
========== Starting test discovery ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.09]   Discovering: UserService.IntegrationTests
[xUnit.net 00:00:00.14]   Discovered:  UserService.IntegrationTests
========== Test discovery finished: 188 Tests found in 1.5 sec ==========
========== Starting test run ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.04]   Starting:    UserService.IntegrationTests
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
[20:26:51 INF] Configuring Enhanced Security middleware {}
[20:26:51 INF] ?? Enhanced security middleware DISABLED in testing environment {"SourceContext": "SecurityExtensions"}
? Enhanced Security middleware configured successfully
[20:26:51 INF] Enhanced Security middleware configured successfully {}
? Middleware pipeline configured
?? Mapping Enhanced Health Check endpoints...
? Enhanced Health Check endpoints mapped
[20:26:51 INF] Starting UserService with Phase 11 Enhanced Security & Monitoring {}
=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===
?? Testing cache connection...
? In-memory cache configured (Performance/Testing mode)
[20:26:51 INF] In-memory cache configured for Performance/Testing environment {}
?? Seeding database...
[20:26:51 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
? Database seeded
?? Seeding monitoring user...
[20:26:51 INF] Monitoring.Enabled = 'false' (evaluated false); skipping seeding. {"SourceContext": "MonitoringUserSeeder"}
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
[20:26:51 INF] Enhanced Security & Monitoring (Phase 11) configured and operational {}
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
[20:26:51 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:51 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
[20:26:52 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNF2LL6TRBOP", "RequestPath": "/api/users"}
[20:26:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2LL6TRBOP", "RequestPath": "/api/users"}
[20:26:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2LL6TRBOP", "RequestPath": "/api/users"}
[20:26:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2LL6TRBOP", "RequestPath": "/api/users"}
[20:26:52 INF] HTTP GET /api/users responded 200 in 204.2775 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Tenant 1 admin sees users: editor@tenant1.com, viewer@tenant1.com, admin@tenant1.com, manager@tenant1.com, user@tenant1.com
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
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
[xUnit.net 00:00:01.81]     UserService.IntegrationTests.Security.TenantIsolationTests.DatabaseQueryFilters_ShouldPreventCrossTenantAccess [FAIL]
   3. User (TenantId: 1)
   4. Manager (TenantId: 1)
   5. Viewer (TenantId: 1)
   6. Editor (TenantId: 1)
   7. Admin (TenantId: 2)
   8. User (TenantId: 2)
[xUnit.net 00:00:01.81]       Expected tenant1Users to contain only items matching u.Email.Contains("@tenant1.com") because Database query filters should only return tenant 1 users, but 
[xUnit.net 00:00:01.81]       {
[xUnit.net 00:00:01.81]           DTOs.Entities.User
[xUnit.net 00:00:01.81]           {
[xUnit.net 00:00:01.81]               CreatedAt = <2025-08-24 01:26:52.296142>, 
[xUnit.net 00:00:01.81]               Email = "user@tenant2.com", 
[xUnit.net 00:00:01.81]               EmailConfirmationToken = <null>, 
[xUnit.net 00:00:01.81]               EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:01.81]               EmailConfirmed = True, 
[xUnit.net 00:00:01.81]               FailedLoginAttempts = 0, 
[xUnit.net 00:00:01.81]               FirstName = "Regular", 
[xUnit.net 00:00:01.81]               Id = 7, 
[xUnit.net 00:00:01.81]               IsActive = True, 
[xUnit.net 00:00:01.81]               Language = <null>, 
[xUnit.net 00:00:01.81]               LastLoginAt = <null>, 
[xUnit.net 00:00:01.81]               LastName = "User2", 
[xUnit.net 00:00:01.81]               LockedOutUntil = <null>, 
[xUnit.net 00:00:01.81]               PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:01.81]               PasswordResetToken = <null>, 
[xUnit.net 00:00:01.81]               PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:01.81]               PhoneNumber = <null>, 
[xUnit.net 00:00:01.81]               Preferences = <null>, 
[xUnit.net 00:00:01.81]               RefreshTokens = {empty}, 
[xUnit.net 00:00:01.81]               TenantUsers = {empty}, 
[xUnit.net 00:00:01.81]               TimeZone = <null>, 
[xUnit.net 00:00:01.81]               UpdatedAt = <2025-08-24 01:26:52.296142>, 
[xUnit.net 00:00:01.81]               UserRoles = {empty}
[xUnit.net 00:00:01.81]           }, 
[xUnit.net 00:00:01.81]           DTOs.Entities.User
[xUnit.net 00:00:01.81]           {
[xUnit.net 00:00:01.81]               CreatedAt = <2025-08-24 01:26:52.296142>, 
[xUnit.net 00:00:01.81]               Email = "admin@tenant2.com", 
[xUnit.net 00:00:01.81]               EmailConfirmationToken = <null>, 
[xUnit.net 00:00:01.81]               EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:01.81]               EmailConfirmed = True, 
[xUnit.net 00:00:01.81]               FailedLoginAttempts = 0, 
[xUnit.net 00:00:01.81]               FirstName = "Admin", 
[xUnit.net 00:00:01.81]               Id = 6, 
[xUnit.net 00:00:01.81]               IsActive = True, 
[xUnit.net 00:00:01.81]               Language = <null>, 
[xUnit.net 00:00:01.81]               LastLoginAt = <null>, 
[xUnit.net 00:00:01.81]               LastName = "User2", 
[xUnit.net 00:00:01.81]               LockedOutUntil = <null>, 
[xUnit.net 00:00:01.81]               PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:01.81]               PasswordResetToken = <null>, 
[xUnit.net 00:00:01.81]               PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:01.81]               PhoneNumber = <null>, 
[xUnit.net 00:00:01.81]               Preferences = <null>, 
[xUnit.net 00:00:01.81]               RefreshTokens = {empty}, 
[xUnit.net 00:00:01.81]               TenantUsers = {empty}, 
[xUnit.net 00:00:01.81]               TimeZone = <null>, 
[xUnit.net 00:00:01.81]               UpdatedAt = <2025-08-24 01:26:52.296142>, 
[xUnit.net 00:00:01.81]               UserRoles = {empty}
[xUnit.net 00:00:01.81]           }
[xUnit.net 00:00:01.81]       } do(es) not match.
[xUnit.net 00:00:01.81]       Stack Trace:
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.81]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Func`2[] args)
[xUnit.net 00:00:01.81]            at FluentAssertions.Collections.GenericCollectionAssertions`3.OnlyContain(Expression`1 predicate, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.81]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(102,0): at UserService.IntegrationTests.Security.TenantIsolationTests.DatabaseQueryFilters_ShouldPreventCrossTenantAccess()
[xUnit.net 00:00:01.81]         --- End of stack trace from previous location ---
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
[20:26:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2LL6TRBOQ", "RequestPath": "/api/users/me"}
[20:26:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2LL6TRBOQ", "RequestPath": "/api/users/me"}
[20:26:52 INF] HTTP GET /api/users/me responded 404 in 2.4851 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.84]     UserService.IntegrationTests.Security.TenantIsolationTests.TenantProvider_ShouldCorrectlyIdentifyCurrentTenant [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:01.84]       Expected HttpStatusCode to be successful (2xx), but found HttpStatusCode.NotFound {value: 404}.
[xUnit.net 00:00:01.84]       Stack Trace:
[xUnit.net 00:00:01.84]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.84]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.84]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.84]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.84]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.84]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.84]            at FluentAssertions.Primitives.HttpResponseMessageAssertions`1.BeSuccessful(String because, Object[] becauseArgs)
[xUnit.net 00:00:01.84]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(221,0): at UserService.IntegrationTests.Security.TenantIsolationTests.TenantProvider_ShouldCorrectlyIdentifyCurrentTenant()
[xUnit.net 00:00:01.84]         --- End of stack trace from previous location ---
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.88]     UserService.IntegrationTests.Security.TenantIsolationTests.TenantIsolation_ShouldPreventDataLeakageThroughIncludes [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:01.88]       Expected tenant1UsersWithRoles to contain only items matching u.Email.Contains("@tenant1.com"), but 
[xUnit.net 00:00:01.88]       DTOs.Entities.User
[xUnit.net 00:00:01.88]           {
[xUnit.net 00:00:01.88]               CreatedAt = <2025-08-24 01:26:52.4220838>, 
[xUnit.net 00:00:01.88]               Email = "user@tenant2.com", 
[xUnit.net 00:00:01.88]               EmailConfirmationToken = <null>, 
[xUnit.net 00:00:01.88]               EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:01.88]               EmailConfirmed = True, 
[xUnit.net 00:00:01.88]               FailedLoginAttempts = 0, 
[xUnit.net 00:00:01.88]               FirstName = "Regular", 
? Database recreated with clean state
[xUnit.net 00:00:01.88]               Id = 7, 
?? TestDataSeeder: Starting test data seeding
[xUnit.net 00:00:01.88]               IsActive = True, 
?? Creating tenants...
[xUnit.net 00:00:01.88]               Language = <null>, 
[xUnit.net 00:00:01.88]               LastLoginAt = <null>, 
[xUnit.net 00:00:01.88]               LastName = "User2", 
[xUnit.net 00:00:01.88]               LockedOutUntil = <null>, 
[xUnit.net 00:00:01.88]               PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:01.88]               PasswordResetToken = <null>, 
[xUnit.net 00:00:01.88]               PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:01.88]               PhoneNumber = <null>, 
[xUnit.net 00:00:01.88]               Preferences = <null>, 
[xUnit.net 00:00:01.88]               RefreshTokens = {empty}, 
[xUnit.net 00:00:01.88]               TenantUsers = {empty}, 
[xUnit.net 00:00:01.88]               TimeZone = <null>, 
[xUnit.net 00:00:01.88]               UpdatedAt = <2025-08-24 01:26:52.4220838>, 
[xUnit.net 00:00:01.88]               {
[xUnit.net 00:00:01.88]               UserRoles = DTOs.Entities.UserRole
[xUnit.net 00:00:01.88]                   {
[xUnit.net 00:00:01.88]                       AssignedAt = <2025-08-24 01:26:52.424209>, 
[xUnit.net 00:00:01.88]                       AssignedBy = "System", 
[xUnit.net 00:00:01.88]                       CreatedAt = <2025-08-24 01:26:52.425154>, 
[xUnit.net 00:00:01.88]                       ExpiresAt = <null>, 
[xUnit.net 00:00:01.88]                       Id = 7, 
[xUnit.net 00:00:01.88]                       IsActive = True, 
[xUnit.net 00:00:01.88]                       Role = DTOs.Entities.Role
[xUnit.net 00:00:01.88]                       {
[xUnit.net 00:00:01.88]                           CreatedAt = <2025-08-24 01:26:52.4132283>, 
[xUnit.net 00:00:01.88]                           Description = "Tenant 2 user", 
[xUnit.net 00:00:01.88]                           Id = 8, 
[xUnit.net 00:00:01.88]                           IsActive = True, 
[xUnit.net 00:00:01.88]                           IsDefault = True, 
[xUnit.net 00:00:01.88]                           IsSystemRole = False, 
[xUnit.net 00:00:01.88]                           Name = "User", 
? Created 2 tenants
[xUnit.net 00:00:01.88]                           RolePermissions = {empty}, 
[xUnit.net 00:00:01.88]                           Tenant = <null>, 
?? Creating permissions...
[xUnit.net 00:00:01.88]                           TenantId = 2, 
[xUnit.net 00:00:01.88]                           UpdatedAt = <2025-08-24 01:26:52.4132283>, 
[xUnit.net 00:00:01.88]                           UserRoles = {{Cyclic reference to type DTOs.Entities.UserRole detected}}
[xUnit.net 00:00:01.88]                       }, 
?? Creating 38 permissions from actual business requirements
[xUnit.net 00:00:01.88]                       RoleId = 8, 
[xUnit.net 00:00:01.88]                       Tenant = <null>, 
   - billing.manage (Billing)
[xUnit.net 00:00:01.88]                       TenantId = 2, 
   - billing.process_payments (Billing)
[xUnit.net 00:00:01.88]                       UpdatedAt = <2025-08-24 01:26:52.425154>, 
   - billing.view (Billing)
[xUnit.net 00:00:01.88]                       User = {Cyclic reference to type DTOs.Entities.User detected}, 
   - billing.view_invoices (Billing)
[xUnit.net 00:00:01.88]                       UserId = 7
   - permissions.create (Permissions)
   - permissions.delete (Permissions)
[xUnit.net 00:00:01.88]                           }
   - permissions.edit (Permissions)
[xUnit.net 00:00:01.88]               }
   - permissions.manage (Permissions)
[xUnit.net 00:00:01.88]           }DTOs.Entities.User
   - permissions.view (Permissions)
   - reports.create (Reports)
[xUnit.net 00:00:01.88]           {
   - reports.export (Reports)
[xUnit.net 00:00:01.88]               CreatedAt = <2025-08-24 01:26:52.4220838>, 
   - reports.schedule (Reports)
   - reports.view (Reports)
[xUnit.net 00:00:01.88]               Email = "admin@tenant2.com", 
   - roles.assign_users (Roles)
[xUnit.net 00:00:01.88]               EmailConfirmationToken = <null>, 
   - roles.create (Roles)
[xUnit.net 00:00:01.88]               EmailConfirmationTokenExpiry = <null>, 
   - roles.delete (Roles)
[xUnit.net 00:00:01.88]               EmailConfirmed = True, 
   - roles.edit (Roles)
[xUnit.net 00:00:01.88]               FailedLoginAttempts = 0, 
   - roles.manage_permissions (Roles)
[xUnit.net 00:00:01.88]               FirstName = "Admin", 
   - roles.view (Roles)
[xUnit.net 00:00:01.88]               Id = 6, 
   - system.manage (System)
[xUnit.net 00:00:01.88]               IsActive = True, 
   - system.manage_backups (System)
   - system.manage_settings (System)
[xUnit.net 00:00:01.88]               Language = <null>, 
   - system.monitor (System)
[xUnit.net 00:00:01.88]               LastLoginAt = <null>, 
   - system.view_logs (System)
   - system.view_metrics (System)
[xUnit.net 00:00:01.88]               LastName = "User2", 
   - tenants.create (Tenants)
   - tenants.delete (Tenants)
[xUnit.net 00:00:01.88]               LockedOutUntil = <null>, 
   - tenants.edit (Tenants)
   - tenants.initialize (Tenants)
   - tenants.manage_settings (Tenants)
   - tenants.view (Tenants)
   - tenants.view_all (Tenants)
[xUnit.net 00:00:01.88]               PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
   - users.create (Users)
[xUnit.net 00:00:01.88]               PasswordResetToken = <null>, 
   - users.delete (Users)
[xUnit.net 00:00:01.88]               PasswordResetTokenExpiry = <null>, 
   - users.edit (Users)
[xUnit.net 00:00:01.88]               PhoneNumber = <null>, 
   - users.manage_roles (Users)
[xUnit.net 00:00:01.88]               Preferences = <null>, 
   - users.view (Users)
[xUnit.net 00:00:01.88]               RefreshTokens = {empty}, 
   - users.view_all (Users)
[xUnit.net 00:00:01.88]               TenantUsers = {empty}, 
[xUnit.net 00:00:01.88]               TimeZone = <null>, 
[xUnit.net 00:00:01.88]               UpdatedAt = <2025-08-24 01:26:52.4220838>, 
[xUnit.net 00:00:01.88]               UserRoles = DTOs.Entities.UserRole
[xUnit.net 00:00:01.88]                   {
[xUnit.net 00:00:01.88]                       AssignedAt = <2025-08-24 01:26:52.4242086>, 
[xUnit.net 00:00:01.88]                       AssignedBy = "System", 
[xUnit.net 00:00:01.88]                       CreatedAt = <2025-08-24 01:26:52.425154>, 
[xUnit.net 00:00:01.88]                       ExpiresAt = <null>, 
[xUnit.net 00:00:01.88]                       Id = 6, 
[xUnit.net 00:00:01.88]                       IsActive = True, 
[xUnit.net 00:00:01.88]                       Role = DTOs.Entities.Role
[xUnit.net 00:00:01.88]                       {
[xUnit.net 00:00:01.88]                           CreatedAt = <2025-08-24 01:26:52.4132283>, 
[xUnit.net 00:00:01.88]                           Description = "Tenant 2 admin", 
[xUnit.net 00:00:01.88]                           Id = 7, 
[xUnit.net 00:00:01.88]                           IsActive = True, 
[xUnit.net 00:00:01.88]                           IsDefault = False, 
[xUnit.net 00:00:01.88]                           IsSystemRole = False, 
[xUnit.net 00:00:01.88]                           Name = "Admin", 
[xUnit.net 00:00:01.88]                           RolePermissions = {empty}, 
[xUnit.net 00:00:01.88]                           Tenant = <null>, 
[xUnit.net 00:00:01.88]       
[xUnit.net 00:00:01.88]       (Output has exceeded the maximum of 100 lines. Increase FormattingOptions.MaxLines on AssertionScope or AssertionOptions to include more lines.)
[xUnit.net 00:00:01.88]                           TenantId = 2,  do(es) not match.
[xUnit.net 00:00:01.88]       Stack Trace:
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.88]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Func`2[] args)
[xUnit.net 00:00:01.88]            at FluentAssertions.Collections.GenericCollectionAssertions`3.OnlyContain(Expression`1 predicate, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.88]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(303,0): at UserService.IntegrationTests.Security.TenantIsolationTests.TenantIsolation_ShouldPreventDataLeakageThroughIncludes()
[xUnit.net 00:00:01.88]         --- End of stack trace from previous location ---
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.90]     UserService.IntegrationTests.Security.TenantIsolationTests.DatabaseQueryFilters_ShouldIsolateRolesByTenant [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:01.90]       Expected tenant1Roles to contain only items matching ((r.TenantId == null) OrElse (r.TenantId == Convert(1, Nullable`1))) because Should only see system roles and tenant 1 roles, but 
[xUnit.net 00:00:01.90]       {
[xUnit.net 00:00:01.90]           DTOs.Entities.Role
[xUnit.net 00:00:01.90]           {
[xUnit.net 00:00:01.90]               CreatedAt = <2025-08-24 01:26:52.4531949>, 
[xUnit.net 00:00:01.90]               Description = "Tenant 2 user", 
[xUnit.net 00:00:01.91]               Id = 8, 
[xUnit.net 00:00:01.91]               IsActive = True, 
? Database recreated with clean state
[xUnit.net 00:00:01.91]               IsDefault = True, 
[xUnit.net 00:00:01.91]               IsSystemRole = False, 
?? TestDataSeeder: Starting test data seeding
[xUnit.net 00:00:01.91]               Name = "User", 
?? Creating tenants...
[xUnit.net 00:00:01.91]               RolePermissions = {empty}, 
[xUnit.net 00:00:01.91]               Tenant = <null>, 
[xUnit.net 00:00:01.91]               TenantId = 2, 
[xUnit.net 00:00:01.91]               UpdatedAt = <2025-08-24 01:26:52.4531949>, 
[xUnit.net 00:00:01.91]               UserRoles = {empty}
[xUnit.net 00:00:01.91]           }, 
[xUnit.net 00:00:01.91]           DTOs.Entities.Role
[xUnit.net 00:00:01.91]           {
[xUnit.net 00:00:01.91]               CreatedAt = <2025-08-24 01:26:52.4531949>, 
[xUnit.net 00:00:01.91]               Description = "Tenant 2 admin", 
[xUnit.net 00:00:01.91]               Id = 7, 
[xUnit.net 00:00:01.91]               IsActive = True, 
[xUnit.net 00:00:01.91]               IsDefault = False, 
[xUnit.net 00:00:01.91]               IsSystemRole = False, 
[xUnit.net 00:00:01.91]               Name = "Admin", 
[xUnit.net 00:00:01.91]               RolePermissions = {empty}, 
[xUnit.net 00:00:01.91]               Tenant = <null>, 
[xUnit.net 00:00:01.91]               TenantId = 2, 
[xUnit.net 00:00:01.91]               UpdatedAt = <2025-08-24 01:26:52.4531949>, 
[xUnit.net 00:00:01.91]               UserRoles = {empty}
[xUnit.net 00:00:01.91]           }
[xUnit.net 00:00:01.91]       } do(es) not match.
[xUnit.net 00:00:01.91]       Stack Trace:
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.91]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Func`2[] args)
? Created 2 tenants
[xUnit.net 00:00:01.91]            at FluentAssertions.Collections.GenericCollectionAssertions`3.OnlyContain(Expression`1 predicate, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.91]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(140,0): at UserService.IntegrationTests.Security.TenantIsolationTests.DatabaseQueryFilters_ShouldIsolateRolesByTenant()
?? Creating permissions...
[xUnit.net 00:00:01.91]         --- End of stack trace from previous location ---
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:01.93]     UserService.IntegrationTests.Security.TenantIsolationTests.CrossTenantDataAccess_ShouldBeImpossibleThroughDirectDatabaseQueries [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:01.93]       Expected usersFromTenant1Context to contain only items matching u.Email.Contains("@tenant1.com"), but 
[xUnit.net 00:00:01.93]       {
[xUnit.net 00:00:01.93]           DTOs.Entities.User
[xUnit.net 00:00:01.93]           {
[xUnit.net 00:00:01.93]               CreatedAt = <2025-08-24 01:26:52.4827266>, 
[xUnit.net 00:00:01.93]               Email = "user@tenant2.com", 
[xUnit.net 00:00:01.93]               EmailConfirmationToken = <null>, 
[xUnit.net 00:00:01.93]               EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:01.93]               EmailConfirmed = True, 
[xUnit.net 00:00:01.93]               FailedLoginAttempts = 0, 
[xUnit.net 00:00:01.93]               FirstName = "Regular", 
[xUnit.net 00:00:01.93]               Id = 7, 
[xUnit.net 00:00:01.93]               IsActive = True, 
[xUnit.net 00:00:01.93]               Language = <null>, 
[xUnit.net 00:00:01.93]               LastLoginAt = <null>, 
[xUnit.net 00:00:01.93]               LastName = "User2", 
[xUnit.net 00:00:01.93]               LockedOutUntil = <null>, 
[xUnit.net 00:00:01.93]               PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:01.93]               PasswordResetToken = <null>, 
[xUnit.net 00:00:01.93]               PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:01.93]               PhoneNumber = <null>, 
[xUnit.net 00:00:01.93]               Preferences = <null>, 
[xUnit.net 00:00:01.93]               RefreshTokens = {empty}, 
[xUnit.net 00:00:01.93]               TenantUsers = {empty}, 
? Database recreated with clean state
[xUnit.net 00:00:01.93]               TimeZone = <null>, 
[xUnit.net 00:00:01.93]               UpdatedAt = <2025-08-24 01:26:52.4827266>, 
?? TestDataSeeder: Starting test data seeding
[xUnit.net 00:00:01.93]               UserRoles = {empty}
?? Creating tenants...
[xUnit.net 00:00:01.93]           }, 
[xUnit.net 00:00:01.93]           DTOs.Entities.User
[xUnit.net 00:00:01.93]           {
[xUnit.net 00:00:01.93]               CreatedAt = <2025-08-24 01:26:52.4827266>, 
[xUnit.net 00:00:01.93]               Email = "admin@tenant2.com", 
[xUnit.net 00:00:01.93]               EmailConfirmationToken = <null>, 
[xUnit.net 00:00:01.93]               EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:01.93]               EmailConfirmed = True, 
[xUnit.net 00:00:01.93]               FailedLoginAttempts = 0, 
[xUnit.net 00:00:01.93]               FirstName = "Admin", 
[xUnit.net 00:00:01.93]               Id = 6, 
[xUnit.net 00:00:01.93]               IsActive = True, 
[xUnit.net 00:00:01.93]               Language = <null>, 
[xUnit.net 00:00:01.93]               LastLoginAt = <null>, 
[xUnit.net 00:00:01.93]               LastName = "User2", 
[xUnit.net 00:00:01.93]               LockedOutUntil = <null>, 
[xUnit.net 00:00:01.93]               PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:01.93]               PasswordResetToken = <null>, 
[xUnit.net 00:00:01.93]               PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:01.93]               PhoneNumber = <null>, 
[xUnit.net 00:00:01.93]               Preferences = <null>, 
[xUnit.net 00:00:01.93]               RefreshTokens = {empty}, 
[xUnit.net 00:00:01.93]               TenantUsers = {empty}, 
[xUnit.net 00:00:01.93]               TimeZone = <null>, 
[xUnit.net 00:00:01.93]               UpdatedAt = <2025-08-24 01:26:52.4827266>, 
[xUnit.net 00:00:01.93]               UserRoles = {empty}
[xUnit.net 00:00:01.93]           }
[xUnit.net 00:00:01.93]       } do(es) not match.
[xUnit.net 00:00:01.93]       Stack Trace:
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
? Created 2 tenants
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
?? Creating permissions...
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:01.93]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Func`2[] args)
[xUnit.net 00:00:01.93]            at FluentAssertions.Collections.GenericCollectionAssertions`3.OnlyContain(Expression`1 predicate, String because, Object[] becauseArgs)
[xUnit.net 00:00:01.93]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(266,0): at UserService.IntegrationTests.Security.TenantIsolationTests.CrossTenantDataAccess_ShouldBeImpossibleThroughDirectDatabaseQueries()
[xUnit.net 00:00:01.93]         --- End of stack trace from previous location ---
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
?? MOCKING two-phase flow for admin@tenant2.com  Tenant 2
? Phase 1 simulated: User admin@tenant2.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 2
[20:26:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2LL6TRBOR", "RequestPath": "/api/roles"}
[20:26:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2LL6TRBOR", "RequestPath": "/api/roles"}
[20:26:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2LL6TRBOR", "RequestPath": "/api/roles"}
[20:26:52 INF] HTTP GET /api/roles responded 200 in 42.9282 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:02.00]     UserService.IntegrationTests.Security.TenantIsolationTests.Tenant2Admin_CannotAccessTenant1Roles [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:02.00]       Expected rolesData!.Data!.Items to contain only items matching ((r.TenantId == null) OrElse (r.TenantId == Convert(2, Nullable`1))) because Should only see tenant 2 and system roles, but 
[xUnit.net 00:00:02.00]       {
[xUnit.net 00:00:02.00]           DTOs.Auth.RoleDto
[xUnit.net 00:00:02.00]           {
? Database recreated with clean state
[xUnit.net 00:00:02.00]               CreatedAt = <2025-08-24 01:26:52.4979192>, 
?? TestDataSeeder: Starting test data seeding
[xUnit.net 00:00:02.00]               Description = "System super admin", 
?? Creating tenants...
[xUnit.net 00:00:02.00]               Id = 1, 
[xUnit.net 00:00:02.00]               IsDefault = False, 
[xUnit.net 00:00:02.00]               IsSystemRole = True, 
[xUnit.net 00:00:02.00]               Name = "SuperAdmin", 
[xUnit.net 00:00:02.00]               Permissions = {"users.view_all", "users.view", "users.manage_roles", "users.edit", "users.delete", "users.create", "tenants.view_all", "tenants.view", "tenants.manage_settings", "tenants.initialize", "tenants.edit", "tenants.delete", "tenants.create", "system.view_metrics", "system.view_logs", "system.monitor", "system.manage_settings", "system.manage_backups", "system.manage", "roles.view", "roles.manage_permissions", "roles.edit", "roles.delete", "roles.create", "roles.assign_users", "reports.view", "reports.schedule", "reports.export", "reports.create", "permissions.view", "permissions.manage", "permissions.edit", …6 more…}, 
[xUnit.net 00:00:02.00]               TenantId = 0, 
[xUnit.net 00:00:02.00]               UpdatedAt = <2025-08-24 01:26:52.4979192>, 
[xUnit.net 00:00:02.00]               UserCount = 0
[xUnit.net 00:00:02.00]           }
[xUnit.net 00:00:02.00]       } do(es) not match.
[xUnit.net 00:00:02.00]       Stack Trace:
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.00]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Func`2[] args)
[xUnit.net 00:00:02.00]            at FluentAssertions.Collections.GenericCollectionAssertions`3.OnlyContain(Expression`1 predicate, String because, Object[] becauseArgs)
[xUnit.net 00:00:02.00]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(74,0): at UserService.IntegrationTests.Security.TenantIsolationTests.Tenant2Admin_CannotAccessTenant1Roles()
[xUnit.net 00:00:02.00]         --- End of stack trace from previous location ---
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
[20:26:52 INF] ?? Test Data Status: Tenants=2, Users=7, Roles=8, Permissions=38, UserRoles=8, RolePermissions=109 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Tenant IDs: Tenant1=1, Tenant2=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? User IDs: Admin=1, User=2 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF] ?? Admin user analysis: {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Email: admin@tenant1.com {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - UserRoles count: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Active UserRoles: 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Permissions: 28 [users.view, users.edit, users.create, users.delete, users.view_all, users.manage_roles, roles.view, roles.create, roles.edit, roles.delete, roles.assign_users, roles.manage_permissions, tenants.view, tenants.create, tenants.edit, tenants.delete, tenants.initialize, tenants.view_all, tenants.manage_settings, reports.view, reports.create, reports.export, reports.schedule, permissions.view, permissions.create, permissions.edit, permissions.delete, permissions.manage] {"SourceContext": "UserService.IntegrationTests.TestBase"}
[20:26:52 INF]    - Tenant: Test Tenant 1 {"SourceContext": "UserService.IntegrationTests.TestBase"}
[xUnit.net 00:00:02.04]     UserService.IntegrationTests.Security.TenantIsolationTests.UserRoleAssignments_ShouldBeTenantScoped [FAIL]
[xUnit.net 00:00:02.04]       Expected tenant1UserRoles to contain only items matching (ur.TenantId == Convert(1, Nullable`1)) because All user roles should be scoped to tenant 1, but DTOs.Entities.UserRole
[xUnit.net 00:00:02.04]           {
[xUnit.net 00:00:02.04]               AssignedAt = <2025-08-24 01:26:52.5800238>, 
[xUnit.net 00:00:02.04]               AssignedBy = "System", 
[xUnit.net 00:00:02.04]               CreatedAt = <2025-08-24 01:26:52.5808765>, 
[xUnit.net 00:00:02.04]               ExpiresAt = <null>, 
[xUnit.net 00:00:02.04]               Id = 7, 
[xUnit.net 00:00:02.04]               IsActive = True, 
[xUnit.net 00:00:02.04]               Role = DTOs.Entities.Role
[xUnit.net 00:00:02.04]               {
[xUnit.net 00:00:02.04]                   CreatedAt = <2025-08-24 01:26:52.5731419>, 
[xUnit.net 00:00:02.04]                   Description = "Tenant 2 user", 
[xUnit.net 00:00:02.04]                   Id = 8, 
[xUnit.net 00:00:02.04]                   IsActive = True, 
[xUnit.net 00:00:02.04]                   IsDefault = True, 
[xUnit.net 00:00:02.04]                   IsSystemRole = False, 
[xUnit.net 00:00:02.04]                   Name = "User", 
[xUnit.net 00:00:02.04]                   RolePermissions = {empty}, 
[xUnit.net 00:00:02.04]                   Tenant = <null>, 
[xUnit.net 00:00:02.04]                   TenantId = 2, 
[xUnit.net 00:00:02.04]                   UpdatedAt = <2025-08-24 01:26:52.5731419>, 
[xUnit.net 00:00:02.04]                   UserRoles = {{Cyclic reference to type DTOs.Entities.UserRole detected}}
[xUnit.net 00:00:02.04]               }, 
[xUnit.net 00:00:02.04]               RoleId = 8, 
[xUnit.net 00:00:02.04]               Tenant = <null>, 
[xUnit.net 00:00:02.04]               TenantId = 2, 
[xUnit.net 00:00:02.04]               UpdatedAt = <2025-08-24 01:26:52.5808765>, 
[xUnit.net 00:00:02.04]               User = DTOs.Entities.User
[xUnit.net 00:00:02.04]               {
[xUnit.net 00:00:02.04]                   CreatedAt = <2025-08-24 01:26:52.5783652>, 
[xUnit.net 00:00:02.04]                   Email = "user@tenant2.com", 
[xUnit.net 00:00:02.04]                   EmailConfirmationToken = <null>, 
[xUnit.net 00:00:02.04]                   EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:02.04]                   EmailConfirmed = True, 
[xUnit.net 00:00:02.04]                   FailedLoginAttempts = 0, 
[xUnit.net 00:00:02.04]                   FirstName = "Regular", 
[xUnit.net 00:00:02.04]                   Id = 7, 
[xUnit.net 00:00:02.04]                   IsActive = True, 
[xUnit.net 00:00:02.04]                   Language = <null>, 
[xUnit.net 00:00:02.04]                   LastLoginAt = <null>, 
[xUnit.net 00:00:02.04]                   LastName = "User2", 
[xUnit.net 00:00:02.04]                   LockedOutUntil = <null>, 
[xUnit.net 00:00:02.04]                   PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:02.04]                   PasswordResetToken = <null>, 
[xUnit.net 00:00:02.04]                   PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:02.04]                   PhoneNumber = <null>, 
[xUnit.net 00:00:02.04]                   Preferences = <null>, 
[xUnit.net 00:00:02.04]                   RefreshTokens = {empty}, 
[xUnit.net 00:00:02.04]                   TenantUsers = {empty}, 
[xUnit.net 00:00:02.04]                   TimeZone = <null>, 
[xUnit.net 00:00:02.04]                   UpdatedAt = <2025-08-24 01:26:52.5783652>, 
[xUnit.net 00:00:02.04]                   UserRoles = {{Cyclic reference to type DTOs.Entities.UserRole detected}}
[xUnit.net 00:00:02.04]               }, 
[xUnit.net 00:00:02.04]               UserId = 7
[xUnit.net 00:00:02.04]           }DTOs.Entities.UserRole
[xUnit.net 00:00:02.04]           {
[xUnit.net 00:00:02.04]               AssignedAt = <2025-08-24 01:26:52.5800233>, 
[xUnit.net 00:00:02.04]               AssignedBy = "System", 
[xUnit.net 00:00:02.04]               CreatedAt = <2025-08-24 01:26:52.5808765>, 
[xUnit.net 00:00:02.04]               ExpiresAt = <null>, 
[xUnit.net 00:00:02.04]               Id = 6, 
[xUnit.net 00:00:02.04]               IsActive = True, 
[xUnit.net 00:00:02.04]               Role = DTOs.Entities.Role
[xUnit.net 00:00:02.04]               {
[xUnit.net 00:00:02.04]                   CreatedAt = <2025-08-24 01:26:52.5731419>, 
[xUnit.net 00:00:02.04]                   Description = "Tenant 2 admin", 
[xUnit.net 00:00:02.04]                   Id = 7, 
[xUnit.net 00:00:02.04]                   IsActive = True, 
[xUnit.net 00:00:02.04]                   IsDefault = False, 
[xUnit.net 00:00:02.04]                   IsSystemRole = False, 
[xUnit.net 00:00:02.04]                   Name = "Admin", 
[xUnit.net 00:00:02.04]                   RolePermissions = {empty}, 
[xUnit.net 00:00:02.04]                   Tenant = <null>, 
[xUnit.net 00:00:02.04]                   TenantId = 2, 
[xUnit.net 00:00:02.04]                   UpdatedAt = <2025-08-24 01:26:52.5731419>, 
[xUnit.net 00:00:02.04]                   UserRoles = {{Cyclic reference to type DTOs.Entities.UserRole detected}}
[xUnit.net 00:00:02.04]               }, 
[xUnit.net 00:00:02.04]               RoleId = 7, 
[xUnit.net 00:00:02.04]               Tenant = <null>, 
[xUnit.net 00:00:02.04]               TenantId = 2, 
[xUnit.net 00:00:02.04]               UpdatedAt = <2025-08-24 01:26:52.5808765>, 
[xUnit.net 00:00:02.04]               User = DTOs.Entities.User
[xUnit.net 00:00:02.04]               {
[xUnit.net 00:00:02.04]                   CreatedAt = <2025-08-24 01:26:52.5783652>, 
[xUnit.net 00:00:02.04]                   Email = "admin@tenant2.com", 
[xUnit.net 00:00:02.04]                   EmailConfirmationToken = <null>, 
[xUnit.net 00:00:02.04]                   EmailConfirmationTokenExpiry = <null>, 
[xUnit.net 00:00:02.04]                   EmailConfirmed = True, 
[xUnit.net 00:00:02.04]                   FailedLoginAttempts = 0, 
[xUnit.net 00:00:02.04]                   FirstName = "Admin", 
[xUnit.net 00:00:02.04]                   Id = 6, 
[xUnit.net 00:00:02.04]                   IsActive = True, 
[xUnit.net 00:00:02.04]                   Language = <null>, 
[xUnit.net 00:00:02.04]                   LastLoginAt = <null>, 
[xUnit.net 00:00:02.04]                   LastName = "User2", 
[xUnit.net 00:00:02.04]                   LockedOutUntil = <null>, 
[xUnit.net 00:00:02.04]                   PasswordHash = "5Dj1VqGLTPN2WVaa8G7BHpa86s4nw2E5ISXQfHWGMjQ=", 
[xUnit.net 00:00:02.04]                   PasswordResetToken = <null>, 
[xUnit.net 00:00:02.04]                   PasswordResetTokenExpiry = <null>, 
[xUnit.net 00:00:02.04]                   PhoneNumber = <null>, 
[xUnit.net 00:00:02.04]       
[xUnit.net 00:00:02.04]       (Output has exceeded the maximum of 100 lines. Increase FormattingOptions.MaxLines on AssertionScope or AssertionOptions to include more lines.)
[xUnit.net 00:00:02.04]                   Preferences = <null>,  do(es) not match.
[xUnit.net 00:00:02.04]       Stack Trace:
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.XUnit2TestFramework.Throw(String message)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.TestFrameworkProvider.Throw(String message)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.DefaultAssertionStrategy.HandleFailure(String message)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.AssertionScope.FailWith(Func`1 failReasonFunc)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.AssertionScope.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Object[] args)
[xUnit.net 00:00:02.04]            at FluentAssertions.Execution.GivenSelector`1.FailWith(String message, Func`2[] args)
[xUnit.net 00:00:02.04]            at FluentAssertions.Collections.GenericCollectionAssertions`3.OnlyContain(Expression`1 predicate, String because, Object[] becauseArgs)
[xUnit.net 00:00:02.04]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Security\TenantIsolationTests.cs(184,0): at UserService.IntegrationTests.Security.TenantIsolationTests.UserRoleAssignments_ShouldBeTenantScoped()
[xUnit.net 00:00:02.04]         --- End of stack trace from previous location ---
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
[20:26:52 INF] Configuring Enhanced Security middleware {}
[20:26:52 INF] ?? Enhanced security middleware DISABLED in testing environment {"SourceContext": "SecurityExtensions"}
? Enhanced Security middleware configured successfully
[20:26:52 INF] Enhanced Security middleware configured successfully {}
? Middleware pipeline configured
?? Mapping Enhanced Health Check endpoints...
? Enhanced Health Check endpoints mapped
[20:26:52 INF] Starting UserService with Phase 11 Enhanced Security & Monitoring {}
=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===
?? Testing cache connection...
? In-memory cache configured (Performance/Testing mode)
[20:26:52 INF] In-memory cache configured for Performance/Testing environment {}
?? Seeding database...
? Database seeded
?? Seeding monitoring user...
[20:26:52 INF] Monitoring.Enabled = 'false' (evaluated false); skipping seeding. {"SourceContext": "MonitoringUserSeeder"}
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
[20:26:52 INF] Enhanced Security & Monitoring (Phase 11) configured and operational {}
[xUnit.net 00:00:02.09]   Finished:    UserService.IntegrationTests
========== Test run finished: 8 Tests (1 Passed, 7 Failed, 0 Skipped) run in 2.1 sec ==========
