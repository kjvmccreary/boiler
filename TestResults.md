Building Test Projects
Starting test discovery for requested test run
========== Starting test discovery ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.07]   Discovering: UserService.IntegrationTests
[xUnit.net 00:00:00.10]   Discovered:  UserService.IntegrationTests
========== Test discovery finished: 180 Tests found in 557.4 ms ==========
========== Starting test run ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.06]   Starting:    UserService.IntegrationTests
=== UserService Startup Diagnostics ===
Environment: Testing
Application Name: UserService
Content Root: C:\Users\mccre\dev\boiler\src\services\UserService
Running in Container: 
[Startup] Monitoring:Enabled raw config value = 'True'
?? Registering base services...
? Base services registered
?? Configuring Swagger...
? Swagger configured
?? Registering common services...
? Common services registered
?? Configuring Redis...
?? Redis connection string resolved to: localhost:6379
?? Running in container: 
? Redis configured
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
[09:15:14 INF] Configuring Enhanced Security middleware {}
? Enhanced Security middleware configured successfully
[09:15:14 INF] Enhanced Security middleware configured successfully {}
? Middleware pipeline configured
?? Mapping Enhanced Health Check endpoints...
? Enhanced Health Check endpoints mapped
[09:15:14 INF] Starting UserService with Phase 11 Enhanced Security & Monitoring {}
=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===
?? Testing Redis connection...
[09:15:14 INF] Redis connection status: Connected {}
? Redis connection successful: 8/23/2025 2:15:14 PM
[09:15:14 INF] Redis connection test successful: 8/23/2025 2:15:14 PM {}
?? Seeding database...
[09:15:14 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
? Database seeded
?? Seeding monitoring user...
[09:15:15 INF] Config snapshot: Monitoring:Enabled='True' {"SourceContext": "MonitoringUserSeeder"}
[09:15:15 INF] Seeding system-wide monitoring account (Email: monitor@local) {"SourceContext": "MonitoringUserSeeder"}
[09:15:15 INF] Created new system Monitor role {"SourceContext": "MonitoringUserSeeder"}
[09:15:15 INF] Linked system Monitor role to permission system.view_metrics {"SourceContext": "MonitoringUserSeeder"}
[09:15:15 INF] Assigned system Monitor role to monitoring user {"SourceContext": "MonitoringUserSeeder"}
[09:15:15 INF] Monitoring user ready. login_email=monitor@local password=ChangeMe123! {"SourceContext": "MonitoringUserSeeder"}
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
[09:15:15 INF] Enhanced Security & Monitoring (Phase 11) configured and operational {}
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
?? Testing two-phase flow for admin@tenant2.com  Tenant 2
[09:15:15 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNF29TTNSAV2", "RequestPath": "/api/auth/login"}
[09:15:15 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF29TTNSAV2", "RequestPath": "/api/auth/login"}
[09:15:15 INF] HTTP POST /api/auth/login responded 404 in 110.4187 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
?? Ensuring clean database state...
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.70]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.PermissionCheck_ShouldEnforceProperAccess(userEmail: "admin@tenant2.com", tenantId: 2, shouldHaveAccess: True) [FAIL]
? Created 2 tenants
?? Creating permissions...
[xUnit.net 00:00:01.70]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 404 (Not Found).
?? Creating 38 permissions from actual business requirements
   - billing.manage (Billing)
[xUnit.net 00:00:01.70]       Stack Trace:
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
[xUnit.net 00:00:01.70]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
   - tenants.initialize (Tenants)
   - tenants.manage_settings (Tenants)
   - tenants.view (Tenants)
[xUnit.net 00:00:01.70]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
   - tenants.view_all (Tenants)
[xUnit.net 00:00:01.70]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
   - users.create (Users)
[xUnit.net 00:00:01.70]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(177,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.PermissionCheck_ShouldEnforceProperAccess(String userEmail, Int32 tenantId, Boolean shouldHaveAccess)
   - users.delete (Users)
[xUnit.net 00:00:01.70]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for user@tenant1.com  Tenant 1
[09:15:16 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF29TTNSAV3", "RequestPath": "/api/auth/login"}
[09:15:16 INF] HTTP POST /api/auth/login responded 404 in 8.8457 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.74]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.PermissionCheck_ShouldEnforceProperAccess(userEmail: "user@tenant1.com", tenantId: 1, shouldHaveAccess: False) [FAIL]
[xUnit.net 00:00:01.74]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 404 (Not Found).
?? Ensuring clean database state...
[xUnit.net 00:00:01.74]       Stack Trace:
[xUnit.net 00:00:01.74]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:01.74]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
[xUnit.net 00:00:01.74]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:01.74]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(177,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.PermissionCheck_ShouldEnforceProperAccess(String userEmail, Int32 tenantId, Boolean shouldHaveAccess)
[xUnit.net 00:00:01.74]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 1
[09:15:16 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF29TTNSAV4", "RequestPath": "/api/auth/login"}
[09:15:16 INF] HTTP POST /api/auth/login responded 404 in 7.9991 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.77]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.PermissionCheck_ShouldEnforceProperAccess(userEmail: "admin@tenant1.com", tenantId: 1, shouldHaveAccess: True) [FAIL]
[xUnit.net 00:00:01.77]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 404 (Not Found).
[xUnit.net 00:00:01.77]       Stack Trace:
[xUnit.net 00:00:01.77]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:01.77]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
[xUnit.net 00:00:01.77]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:01.77]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(177,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.PermissionCheck_ShouldEnforceProperAccess(String userEmail, Int32 tenantId, Boolean shouldHaveAccess)
[xUnit.net 00:00:01.77]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 1
[09:15:16 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF29TTNSAV5", "RequestPath": "/api/auth/login"}
[09:15:16 INF] HTTP POST /api/auth/login responded 404 in 5.8482 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.80]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.UserService_WithTenantToken_CanAccessUsers [FAIL]
[xUnit.net 00:00:01.80]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 404 (Not Found).
[xUnit.net 00:00:01.80]       Stack Trace:
[xUnit.net 00:00:01.80]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:01.80]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
?? Ensuring clean database state...
[xUnit.net 00:00:01.80]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:01.80]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(79,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.UserService_WithTenantToken_CanAccessUsers()
[xUnit.net 00:00:01.80]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for admin@tenant2.com  Tenant 2
[09:15:16 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF29TTNSAV6", "RequestPath": "/api/auth/login"}
[09:15:16 INF] HTTP POST /api/auth/login responded 404 in 5.6428 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.83]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.TenantIsolation_Tenant2Admin_CannotSeeTenant1Users [FAIL]
[xUnit.net 00:00:01.83]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 404 (Not Found).
[xUnit.net 00:00:01.83]       Stack Trace:
[xUnit.net 00:00:01.83]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:01.83]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
?? Ensuring clean database state...
[xUnit.net 00:00:01.83]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:01.83]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(125,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.TenantIsolation_Tenant2Admin_CannotSeeTenant1Users()
[xUnit.net 00:00:01.83]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 2
[09:15:16 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF29TTNSAV7", "RequestPath": "/api/auth/login"}
[09:15:16 INF] HTTP POST /api/auth/login responded 404 in 5.7713 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
?? Testing two-phase flow for admin@tenant2.com  Tenant 1
[09:15:16 INF] HTTP POST /api/auth/login responded 429 in 22.7659 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Security Utility Test: Cross-tenant access properly blocked
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
? Phase 1 JWT Test: Token generated without tenant context
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 1
[09:15:16 INF] HTTP POST /api/auth/login responded 429 in 8.4833 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:01.97]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.AuthenticationHelper_CanGenerateTenantAwareJwt [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:01.97]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 429 (Too Many Requests).
[xUnit.net 00:00:01.97]       Stack Trace:
[xUnit.net 00:00:01.97]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
? Database recreated with clean state
?? TestDataSeeder: Starting test data seeding
?? Creating tenants...
[xUnit.net 00:00:01.97]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
[xUnit.net 00:00:01.97]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:01.97]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(58,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.AuthenticationHelper_CanGenerateTenantAwareJwt()
[xUnit.net 00:00:01.97]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 1
[09:15:16 INF] HTTP POST /api/auth/login responded 429 in 6.7287 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:02.00]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.TenantIsolation_Tenant1Admin_CannotSeeTenant2Users [FAIL]
[xUnit.net 00:00:02.00]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 429 (Too Many Requests).
[xUnit.net 00:00:02.00]       Stack Trace:
[xUnit.net 00:00:02.00]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:02.00]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
[xUnit.net 00:00:02.00]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
?? Ensuring clean database state...
[xUnit.net 00:00:02.00]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(102,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.TenantIsolation_Tenant1Admin_CannotSeeTenant2Users()
[xUnit.net 00:00:02.00]         --- End of stack trace from previous location ---
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 1
[09:15:16 INF] HTTP POST /api/auth/login responded 429 in 7.0710 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:02.03]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.Roles_TenantSpecific_ShouldBeIsolated [FAIL]
?? Ensuring clean database state...
[xUnit.net 00:00:02.03]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 429 (Too Many Requests).
[xUnit.net 00:00:02.03]       Stack Trace:
[xUnit.net 00:00:02.03]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:02.03]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
[xUnit.net 00:00:02.03]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:02.03]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(148,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.Roles_TenantSpecific_ShouldBeIsolated()
[xUnit.net 00:00:02.03]         --- End of stack trace from previous location ---
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
? Created 8 user role assignmeBuilding Test Projects
Starting test discovery for requested test run
========== Starting test discovery ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.07]   Discovering: UserService.IntegrationTests
[xUnit.net 00:00:00.11]   Discovered:  UserService.IntegrationTests
========== Test discovery finished: 180 Tests found in 578.2 ms ==========
========== Starting test run ==========
[xUnit.net 00:00:00.00] xUnit.net VSTest Adapter v2.5.3.1+6b60a9e56a (64-bit .NET 9.0.7)
[xUnit.net 00:00:00.06]   Starting:    UserService.IntegrationTests
=== UserService Startup Diagnostics ===
Environment: Testing
Application Name: UserService
Content Root: C:\Users\mccre\dev\boiler\src\services\UserService
Running in Container: 
[Startup] Monitoring:Enabled raw config value = 'True'
?? Registering base services...
? Base services registered
?? Configuring Swagger...
? Swagger configured
?? Registering common services...
? Common services registered
?? Configuring Redis...
?? Redis connection string resolved to: localhost:6379
?? Running in container: 
? Redis configured
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
[09:19:10 INF] Configuring Enhanced Security middleware {}
? Enhanced Security middleware configured successfully
[09:19:10 INF] Enhanced Security middleware configured successfully {}
? Middleware pipeline configured
?? Mapping Enhanced Health Check endpoints...
? Enhanced Health Check endpoints mapped
[09:19:10 INF] Starting UserService with Phase 11 Enhanced Security & Monitoring {}
=== UserService Starting with Phase 11 Enhanced Security & Monitoring ===
?? Testing Redis connection...
[09:19:10 INF] Redis connection status: Connected {}
? Redis connection successful: 8/23/2025 2:19:10 PM
[09:19:10 INF] Redis connection test successful: 8/23/2025 2:19:10 PM {}
?? Seeding database...
[09:19:10 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
? Database seeded
?? Seeding monitoring user...
[09:19:10 INF] Config snapshot: Monitoring:Enabled='True' {"SourceContext": "MonitoringUserSeeder"}
[09:19:10 INF] Seeding system-wide monitoring account (Email: monitor@local) {"SourceContext": "MonitoringUserSeeder"}
[09:19:10 INF] Created new system Monitor role {"SourceContext": "MonitoringUserSeeder"}
[09:19:10 INF] Linked system Monitor role to permission system.view_metrics {"SourceContext": "MonitoringUserSeeder"}
[09:19:11 INF] Assigned system Monitor role to monitoring user {"SourceContext": "MonitoringUserSeeder"}
[09:19:11 INF] Monitoring user ready. login_email=monitor@local password=ChangeMe123! {"SourceContext": "MonitoringUserSeeder"}
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
[09:19:11 INF] Enhanced Security & Monitoring (Phase 11) configured and operational {}
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
?? MOCKING two-phase flow for admin@tenant2.com  Tenant 2
? Phase 1 simulated: User admin@tenant2.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 2
?? Generated tenant-aware JWT for admin@tenant2.com:
   - Tenant: Test Tenant 2 (ID: 2)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[09:19:11 WRN] Failed to determine the https port for redirect. {"EventId": {"Id": 3, "Name": "FailedToDeterminePort"}, "SourceContext": "Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionMiddleware", "RequestId": "0HNF2A03T5ON6", "RequestPath": "/api/users"}
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON6", "RequestPath": "/api/users"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON6", "RequestPath": "/api/users"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ON6", "RequestPath": "/api/users"}
[09:19:11 INF] HTTP GET /api/users responded 200 in 259.3547 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Permission Test: admin@tenant2.com access = OK
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
?? MOCKING two-phase flow for user@tenant1.com  Tenant 1
? Phase 1 simulated: User user@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for user@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: User
   - Permissions: 3
? Two-phase flow simulation completed successfully
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON7", "RequestPath": "/api/users"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON7", "RequestPath": "/api/users"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ON7", "RequestPath": "/api/users"}
[09:19:11 WRN] User 2 attempted to access users list without users.view permission {"SourceContext": "UserService.Controllers.UsersController", "ActionId": "16a017a0-b8c5-4103-b5f0-818228b01a6b", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNF2A03T5ON7", "RequestPath": "/api/users"}
[09:19:11 INF] HTTP GET /api/users responded 403 in 32.3550 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Permission Test: user@tenant1.com access = Forbidden
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for admin@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON8", "RequestPath": "/api/users"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON8", "RequestPath": "/api/users"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ON8", "RequestPath": "/api/users"}
[09:19:11 INF] HTTP GET /api/users responded 200 in 31.8810 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Permission Test: admin@tenant1.com access = OK
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for admin@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON9", "RequestPath": "/api/users"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ON9", "RequestPath": "/api/users"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ON9", "RequestPath": "/api/users"}
[09:19:11 INF] HTTP GET /api/users responded 200 in 8.5462 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? UserService Access Test: Retrieved 5 users
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
?? MOCKING two-phase flow for admin@tenant2.com  Tenant 2
? Phase 1 simulated: User admin@tenant2.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 2
?? Generated tenant-aware JWT for admin@tenant2.com:
   - Tenant: Test Tenant 2 (ID: 2)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ONA", "RequestPath": "/api/users"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ONA", "RequestPath": "/api/users"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ONA", "RequestPath": "/api/users"}
[09:19:11 INF] HTTP GET /api/users responded 200 in 8.1433 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Tenant Isolation Test: Tenant 2 admin sees 2 users (tenant 2 only)
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 2
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
?? MOCKING two-phase flow for admin@tenant2.com  Tenant 1
? Phase 1 simulated: User admin@tenant2.com authenticated (mocked)
? Security Utility Test: Cross-tenant access properly blocked
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
? Phase 1 JWT Test: Token generated without tenant context
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for admin@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
? Phase 2 JWT Test: Token generated with tenant context
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for admin@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ONB", "RequestPath": "/api/users"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ONB", "RequestPath": "/api/users"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ONB", "RequestPath": "/api/users"}
[09:19:11 INF] HTTP GET /api/users responded 200 in 10.0696 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
? Tenant Isolation Test: Tenant 1 admin sees 5 users (tenant 1 only)
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for admin@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[09:19:11 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ONC", "RequestPath": "/api/roles"}
[09:19:11 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF2A03T5ONC", "RequestPath": "/api/roles"}
[09:19:11 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNF2A03T5ONC", "RequestPath": "/api/roles"}
[09:19:11 INF] HTTP GET /api/roles responded 200 in 51.2740 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
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
[xUnit.net 00:00:02.27]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.Roles_TenantSpecific_ShouldBeIsolated [FAIL]
   - users.create (Users)
   - users.delete (Users)
   - users.edit (Users)
   - users.manage_roles (Users)
   - users.view (Users)
   - users.view_all (Users)
[xUnit.net 00:00:02.27]       System.Text.Json.JsonException : The JSON value could not be converted to System.Collections.Generic.List`1[DTOs.Auth.RoleDto]. Path: $.data | LineNumber: 0 | BytePositionInLine: 24.
[xUnit.net 00:00:02.27]       Stack Trace:
? Created 38 permissions
?? Creating roles...
[xUnit.net 00:00:02.27]            at System.Text.Json.ThrowHelper.ThrowJsonException_DeserializeUnableToConvertValue(Type propertyType)
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.JsonCollectionConverter`2.OnTryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, TCollection& value)
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
?? CreateRoles DEBUG:
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.Metadata.JsonPropertyInfo`1.ReadJsonAndSetMember(Object obj, ReadStack& state, Utf8JsonReader& reader)
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.Converters.ObjectDefaultConverter`1.OnTryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value)
   - tenant1.Id = 1
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.JsonConverter`1.TryRead(Utf8JsonReader& reader, Type typeToConvert, JsonSerializerOptions options, ReadStack& state, T& value, Boolean& isPopulatedValue)
   - tenant2.Id = 2
?? Roles to be created (8):
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.JsonConverter`1.ReadCore(Utf8JsonReader& reader, T& value, JsonSerializerOptions options, ReadStack& state)
   1. SuperAdmin (TenantId: )
   2. Admin (TenantId: 1)
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.ContinueDeserialize(ReadBufferState& bufferState, JsonReaderState& jsonReaderState, ReadStack& readStack, T& value)
   3. User (TenantId: 1)
[xUnit.net 00:00:02.27]            at System.Text.Json.Serialization.Metadata.JsonTypeInfo`1.DeserializeAsync(Stream utf8Json, CancellationToken cancellationToken)
   4. Manager (TenantId: 1)
   5. Viewer (TenantId: 1)
   6. Editor (TenantId: 1)
[xUnit.net 00:00:02.27]            at System.Net.Http.Json.HttpContentJsonExtensions.ReadFromJsonAsyncCore[T](HttpContent content, JsonSerializerOptions options, CancellationToken cancellationToken)
   7. Admin (TenantId: 2)
   8. User (TenantId: 2)
[xUnit.net 00:00:02.27]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(157,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.Roles_TenantSpecific_ShouldBeIsolated()
[xUnit.net 00:00:02.27]         --- End of stack trace from previous location ---
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
?? MOCKING two-phase flow for admin@tenant1.com  Tenant 1
? Phase 1 simulated: User admin@tenant1.com authenticated (mocked)
? Phase 2 simulated: Tenant access verified for tenant 1
?? Generated tenant-aware JWT for admin@tenant1.com:
   - Tenant: Test Tenant 1 (ID: 1)
   - Roles: Admin
   - Permissions: 28
? Two-phase flow simulation completed successfully
[xUnit.net 00:00:02.29]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.JwtValidation_ShouldWork [FAIL]
[xUnit.net 00:00:02.29]       Assert.Equal() Failure: Strings differ
[xUnit.net 00:00:02.29]                  ↓ (pos 0)
[xUnit.net 00:00:02.29]       Expected: "admin@tenant1.com"
[xUnit.net 00:00:02.29]       Actual:   "["admin@tenant1.com","admin@tenant1.com"]"
[xUnit.net 00:00:02.29]                  ↑ (pos 0)
[xUnit.net 00:00:02.29]       Stack Trace:
[xUnit.net 00:00:02.29]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(203,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.ValidateJwtClaims(String jwt, Dictionary`2 expectedClaims)
[xUnit.net 00:00:02.29]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(205,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.JwtValidation_ShouldWork()
[xUnit.net 00:00:02.29]         --- End of stack trace from previous location ---
[xUnit.net 00:00:02.30]   Finished:    UserService.IntegrationTests
========== Test run finished: 11 Tests (9 Passed, 2 Failed, 0 Skipped) run in 2.3 sec ==========
nts
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
?? Testing two-phase flow for admin@tenant1.com  Tenant 1
[09:15:16 INF] HTTP POST /api/auth/login responded 429 in 5.8233 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
[xUnit.net 00:00:02.06]     UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.JwtValidation_ShouldWork [FAIL]
[xUnit.net 00:00:02.06]       System.Net.Http.HttpRequestException : Response status code does not indicate success: 429 (Too Many Requests).
[xUnit.net 00:00:02.06]       Stack Trace:
[xUnit.net 00:00:02.06]            at System.Net.Http.HttpResponseMessage.EnsureSuccessStatusCode()
[xUnit.net 00:00:02.06]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(42,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantTokenViaTwoPhaseFlow(HttpClient client, ApplicationDbContext dbContext, String email, Int32 preferredTenantId)
[xUnit.net 00:00:02.06]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\TestUtilities\AuthenticationHelper.cs(278,0): at UserService.IntegrationTests.TestUtilities.AuthenticationHelper.GetTenantAwareJwtAsync(HttpClient client, ApplicationDbContext dbContext, String email, Nullable`1 preferredTenantId)
[xUnit.net 00:00:02.06]         C:\Users\mccre\dev\boiler\tests\integration\UserService.IntegrationTests\Controllers\TwoPhaseAuthTests.cs(201,0): at UserService.IntegrationTests.Controllers.TwoPhaseAuthTests.JwtValidation_ShouldWork()
[xUnit.net 00:00:02.06]         --- End of stack trace from previous location ---
[xUnit.net 00:00:02.07]   Finished:    UserService.IntegrationTests
========== Test run finished: 11 Tests (2 Passed, 9 Failed, 0 Skipped) run in 2.1 sec ==========
