# Microservices Starter App - Implementation Phases (Revised with Dynamic RBAC)

## Overview
This document breaks down the microservices starter application into manageable implementation phases, now including a dynamic, tenant-scoped Role-Based Access Control (RBAC) system where each tenant can define their own roles and permissions.

## Key Architectural Change: Dynamic Tenant-Scoped RBAC
The application now supports:
- System-level roles (SuperAdmin, TenantAdmin) that are fixed across all tenants
- Tenant-specific custom roles that each tenant can create and manage
- Permission-based authorization that allows flexible role definitions
- Dynamic authorization evaluation at runtime using database-stored role definitions

---

## Phase 1: Foundation & Project Structure
**Duration**: 1-2 sessions  
**Complexity**: Low  
*[No changes from original]*

### Objectives
- Set up the basic project structure
- Initialize solution and core projects
- Configure basic development environment

### Deliverables
1. **Solution Structure**
   ```
   starter-app/
   ├── src/
   │   ├── services/
   │   │   ├── AuthService/
   │   │   ├── UserService/
   │   │   └── ApiGateway/
   │   ├── shared/
   │   │   ├── DTOs/
   │   │   ├── Common/
   │   │   └── Contracts/
   │   └── frontend/
   │       └── react-app/
   ├── tests/
   ├── docker/
   └── docs/
   ```

2. **Core Projects Setup**
3. **Development Environment**

### Dependencies
- .NET 8.0 SDK
- Node.js 18+
- Code editor (VS Code/Visual Studio)

### Next Phase Prerequisites
- All projects compile successfully
- Solution structure is established
- Development environment is configured

---

## Phase 2: Shared Libraries & Common Infrastructure
**Duration**: 1-2 sessions  
**Complexity**: Low-Medium  
**[ENHANCED with Permission Infrastructure]**

### Objectives
- Implement shared DTOs and common utilities
- Set up logging, configuration, and middleware foundations
- Create base classes and interfaces
- **NEW: Define permission constants and authorization interfaces**

### Deliverables
1. **Shared DTOs Project** *[Unchanged]*

2. **Common Utilities Project** *[Unchanged]*

3. **Service Contracts** *[Unchanged]*

4. **Permission Infrastructure** *[NEW]*
   ```csharp
   // Permission definitions
   public static class Permissions
   {
       public static class Users
       {
           public const string View = "users.view";
           public const string Edit = "users.edit";
           public const string Delete = "users.delete";
       }
       // More permission categories...
   }
   
   // Authorization interfaces
   public interface IPermissionService
   public interface IRoleService
   public interface IAuthorizationService
   ```

### Dependencies
- Phase 1 completion
- NuGet packages: Serilog, FluentValidation, AutoMapper

### Next Phase Prerequisites
- Shared libraries compile and are referenced by service projects
- Basic configuration and logging framework is in place
- **Permission constants defined and available**

---

## Phase 3: Database Foundation & Entity Framework with RBAC
**Duration**: 3-4 sessions  
**Complexity**: Medium-High  
**[SIGNIFICANTLY ENHANCED]**

### Objectives
- Set up PostgreSQL database with Entity Framework Core
- Implement core entities with multi-tenant support
- **NEW: Implement complete RBAC schema**
- Create repository pattern and DbContext

### Deliverables
1. **Core Entities** *[ENHANCED]*
   ```csharp
   public class Tenant : BaseEntity
   public class User : BaseEntity
   public class TenantUser : BaseEntity
   public class RefreshToken : BaseEntity
   
   // NEW RBAC Entities
   public class Role : BaseEntity
   {
       public Guid TenantId { get; set; }
       public string Name { get; set; }
       public string Description { get; set; }
       public bool IsSystemRole { get; set; }
       public bool IsDefault { get; set; }
   }
   
   public class Permission : BaseEntity
   {
       public string Name { get; set; }
       public string Category { get; set; }
       public string Description { get; set; }
   }
   
   public class RolePermission : BaseEntity
   {
       public Guid RoleId { get; set; }
       public Guid PermissionId { get; set; }
   }
   
   public class UserRole : BaseEntity
   {
       public Guid UserId { get; set; }
       public Guid RoleId { get; set; }
       public Guid TenantId { get; set; }
   }
   ```

2. **DbContext Implementation** *[ENHANCED]*
   - Include RBAC DbSets
   - Configure relationships and constraints
   - Implement row-level security for roles

3. **Repository Pattern** *[ENHANCED]*
   ```csharp
   public interface IRoleRepository : IRepository<Role>
   public class RoleRepository : TenantRepository<Role>, IRoleRepository
   public interface IPermissionRepository : IRepository<Permission>
   public interface IUserRoleRepository : IRepository<UserRole>
   ```

4. **Database Migrations** *[ENHANCED]*
   - Initial migration with core tables
   - RBAC schema migration
   - Permission seed data migration
   - Default system roles migration

5. **Authorization Services** *[NEW]*
   ```csharp
   public interface IPermissionService
   {
       Task<bool> UserHasPermissionAsync(Guid userId, string permission);
       Task<IEnumerable<string>> GetUserPermissionsAsync(Guid userId);
   }
   
   public interface IRoleService
   {
       Task<Role> CreateRoleAsync(Guid tenantId, string name, List<string> permissions);
       Task AssignRoleToUserAsync(Guid userId, Guid roleId);
   }
   ```

### Dependencies
- Phase 2 completion
- PostgreSQL (local or Docker)
- NuGet packages: Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.EntityFrameworkCore.Tools

### Next Phase Prerequisites
- Database successfully created with RBAC schema
- Permission and role repositories working
- Seed data for permissions loaded
- Default roles created for new tenants

---

## Phase 4: Authentication Service with Dynamic Authorization
**Duration**: 3-4 sessions  
**Complexity**: High  
**[ENHANCED with Dynamic Authorization]**

### Objectives
- Implement JWT-based authentication
- Create user registration and login endpoints
- **NEW: Include user permissions in JWT claims**
- **NEW: Implement custom authorization handlers**

### Deliverables
1. **Authentication Controller** *[Unchanged]*

2. **Authentication Services** *[ENHANCED]*
   ```csharp
   public interface IAuthService
   {
       // Original methods plus:
       Task<List<string>> GetUserPermissionsAsync(Guid userId);
   }
   ```

3. **JWT Configuration** *[ENHANCED]*
   - Include permissions or role IDs in claims
   - Implement claim transformation
   - Cache permission lookups

4. **Custom Authorization** *[NEW]*
   ```csharp
   // Custom authorization attribute
   public class RequiresPermissionAttribute : AuthorizeAttribute
   
   // Authorization handler
   public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
   
   // Authorization requirement
   public class PermissionRequirement : IAuthorizationRequirement
   ```

5. **Authorization Policies** *[NEW]*
   ```csharp
   // Dynamic policy provider
   public class PermissionPolicyProvider : IAuthorizationPolicyProvider
   ```

### Dependencies
- Phase 3 completion with RBAC schema
- NuGet packages: Microsoft.AspNetCore.Authentication.JwtBearer, BCrypt.Net-Next

### Next Phase Prerequisites
- Authentication working with permission claims
- Dynamic authorization evaluating correctly
- Permission checks integrated with controllers

---

## Phase 5: User & Role Management Service
**Duration**: 2-3 sessions  
**Complexity**: Medium-High  
**[EXPANDED from original Phase 5]**

### Objectives
- Implement user management functionality
- **NEW: Implement role management endpoints**
- **NEW: Implement permission assignment**
- Set up user-tenant-role relationship management

### Deliverables
1. **User Controller** *[Original]*

2. **Role Controller** *[NEW]*
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class RolesController : ControllerBase
   {
       // GET /api/roles (tenant-scoped)
       // POST /api/roles
       // PUT /api/roles/{id}
       // DELETE /api/roles/{id}
       // GET /api/roles/{id}/permissions
       // PUT /api/roles/{id}/permissions
       // POST /api/roles/{roleId}/users/{userId}
   }
   ```

3. **Permission Controller** *[NEW]*
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize(Policy = "SystemAdmin")]
   public class PermissionsController : ControllerBase
   {
       // GET /api/permissions (list all available permissions)
       // GET /api/permissions/categories
   }
   ```

4. **Services** *[ENHANCED]*
   ```csharp
   public interface IUserService
   public interface IRoleManagementService
   public interface IPermissionManagementService
   public interface IUserRoleService
   ```

5. **Authorization Setup** *[ENHANCED]*
   - Dynamic role-based authorization
   - Permission-based access control
   - Tenant-scoped role management

### Dependencies
- Phase 4 completion with dynamic authorization
- RBAC schema fully implemented

### Next Phase Prerequisites
- Role CRUD operations working
- Permission assignment functional
- User-role relationships managed correctly
- Dynamic authorization fully integrated

---

## Phase 6: API Gateway with Authorization
**Duration**: 2-3 sessions  
**Complexity**: Medium-High  
**[ENHANCED with authorization forwarding]**

### Objectives
- Implement API Gateway with Ocelot or YARP
- Set up request routing and load balancing
- **NEW: Forward authorization context between services**
- Implement rate limiting and middleware

### Deliverables
1. **API Gateway Project** *[Unchanged]*

2. **Gateway Configuration** *[ENHANCED]*
   - Include authorization header forwarding
   - Preserve tenant context across services

3. **Middleware Components** *[ENHANCED]*
   ```csharp
   public class TenantResolutionMiddleware
   public class RequestLoggingMiddleware
   public class RateLimitingMiddleware
   public class AuthorizationContextMiddleware // NEW
   ```

4. **Service Discovery** *[Unchanged]*

### Dependencies
- Phase 5 completion
- Ocelot or YARP NuGet package

### Next Phase Prerequisites
- Authorization context preserved across service calls
- All services accessible through gateway with proper authorization

---

## Phase 7: React Frontend with RBAC UI
**Duration**: 3-4 sessions  
**Complexity**: Medium-High  
**[ENHANCED with role management UI]**

### Objectives
- Set up React application with Material UI Pro and Tailwind
- Implement authentication flows
- **NEW: Build role management interface**
- **NEW: Implement permission-based UI rendering**

### Deliverables
1. **Project Setup** *[Unchanged]*

2. **Authentication Components** *[ENHANCED]*
   ```tsx
   // Original components plus:
   // PermissionProvider context
   // usePermission hook
   // CanAccess component for conditional rendering
   ```

3. **Role Management UI** *[NEW]*
   ```tsx
   // RoleList component
   // RoleEditor component
   // PermissionSelector component
   // UserRoleAssignment component
   ```

4. **Permission-Based Rendering** *[NEW]*
   ```tsx
   // Dynamic menu based on permissions
   // Conditional form fields
   // Protected UI elements
   ```

### Dependencies
- Phase 6 completion
- Node.js and npm/yarn
- Material UI Pro license (or fallback to free version)

### Next Phase Prerequisites
- Role management UI functional
- Permission-based rendering working
- Frontend fully integrated with RBAC backend

---

## Phase 8: Docker Configuration
**Duration**: 2-3 sessions  
**Complexity**: Medium  
*[No changes from original]*

---

## Phase 9: Advanced Multi-Tenancy with RBAC
**Duration**: 3-4 sessions  
**Complexity**: High  
**[ENHANCED from original Phase 9]**

### Objectives
- Implement shared database, shared schema multi-tenancy
- **Enhanced: Ensure role isolation between tenants**
- **NEW: Implement tenant-specific permission overrides**
- Set up tenant resolution strategies

### Deliverables
1. **Tenant Resolution** *[Unchanged]*

2. **Enhanced Repository Pattern** *[ENHANCED]*
   - Automatic tenant filtering for roles
   - Cross-tenant role prevention
   - Permission inheritance logic

3. **Tenant Management** *[ENHANCED]*
   - Default role creation for new tenants
   - Role template management
   - Bulk permission assignment

4. **Row-Level Security** *[ENHANCED]*
   - RLS policies for role tables
   - Tenant-scoped permission checks

5. **Frontend Multi-Tenancy** *[ENHANCED]*
   - Tenant-specific role UI
   - Permission visualization per tenant

### Dependencies
- Phase 8 completion
- PostgreSQL RLS understanding
- Multi-tenant RBAC testing strategy

### Next Phase Prerequisites
- Complete tenant isolation for roles verified
- No cross-tenant permission leakage
- Default roles created for new tenants

---

## Phase 10: Caching & Performance Optimization
**Duration**: 2-3 sessions  
**Complexity**: Medium-High  
**[NEW PHASE - Inserted before original Phase 10]**

### Objectives
- Implement Redis caching for permissions
- Optimize authorization performance
- Add permission cache invalidation strategies

### Deliverables
1. **Redis Integration**
   ```csharp
   public interface IPermissionCache
   public class RedisPermissionCache : IPermissionCache
   ```

2. **Cache Strategies**
   - User permission caching
   - Role-permission mapping cache
   - Tenant-scoped cache invalidation

3. **Performance Optimization**
   - Batch permission loading
   - Eager loading strategies
   - Query optimization

### Dependencies
- Phase 9 completion
- Redis instance
- StackExchange.Redis NuGet package

### Next Phase Prerequisites
- Permission checks < 10ms response time
- Cache hit ratio > 95%
- Cache invalidation working correctly

---

## Phase 11: Enhanced Security & Monitoring
**Duration**: 2-3 sessions  
**Complexity**: Medium-High  
**[Originally Phase 10, enhanced with RBAC auditing]**

### Objectives
- Implement comprehensive security measures
- Set up monitoring and health checks
- **NEW: Add permission audit logging**
- Add compliance features

### Deliverables
1. **Security Enhancements** *[ENHANCED]*
   - Permission check audit trail
   - Role change tracking
   - Unauthorized access attempt logging

2. **Monitoring Setup** *[ENHANCED]*
   - Permission cache metrics
   - Authorization performance monitoring
   - Role usage analytics

3. **Audit Logging** *[ENHANCED]*
   ```csharp
   public class PermissionAuditEntry
   public class RoleChangeAuditEntry
   ```

---

## Phase 12: Testing & Quality Assurance
**Duration**: 3-4 sessions  
**Complexity**: Medium-High  
**[Originally Phase 11, enhanced with RBAC testing]**

### Objectives
- Implement comprehensive testing strategy
- **NEW: Test permission isolation**
- **NEW: Test role management flows**
- Ensure code quality and coverage

### Deliverables
1. **Unit Tests** *[ENHANCED]*
   - Permission service tests
   - Role service tests
   - Authorization handler tests

2. **Integration Tests** *[ENHANCED]*
   - Multi-tenant role isolation tests
   - Permission inheritance tests
   - Cache invalidation tests

3. **End-to-End Tests** *[ENHANCED]*
   - Role management workflows
   - Permission-based access scenarios

---

## Phase 13: Deployment Preparation
**Duration**: 2-3 sessions  
**Complexity**: Medium-High  
**[Originally Phase 12, with RBAC considerations]**

### Objectives
- Prepare for multiple deployment scenarios
- Create deployment scripts and documentation
- **NEW: Document RBAC configuration**

### Deliverables
1. **Deployment Configurations** *[ENHANCED]*
   - Permission seed data scripts
   - Default role templates
   - Tenant initialization scripts

2. **Documentation** *[ENHANCED]*
   - RBAC administration guide
   - Permission reference documentation
   - Role management best practices

---

## Implementation Notes

### RBAC Design Principles
- **Principle of Least Privilege**: Users get minimum permissions needed
- **Separation of Duties**: Critical operations require multiple permissions
- **Defense in Depth**: Multiple authorization layers
- **Audit Everything**: All permission checks are logged

### Permission Naming Convention
```
resource.action
Examples:
- users.view
- users.edit
- users.delete
- reports.create
- reports.export
- billing.manage
```

### Caching Strategy
- Cache Duration: 5 minutes for permissions
- Invalidation: On role change, permission change, or user role assignment
- Scope: Per-tenant caching with Redis key prefixing

### Testing Considerations
- Always test cross-tenant isolation
- Verify permission inheritance
- Test cache invalidation scenarios
- Validate audit logging

### Performance Targets
- Authorization check: < 10ms (cached)
- Role assignment: < 100ms
- Permission query: < 50ms
- Cache hit ratio: > 95%

---

## Getting Started with RBAC Implementation

1. **Phase 2**: Define all permission constants upfront
2. **Phase 3**: Design database schema with proper constraints
3. **Phase 4**: Implement dynamic authorization early
4. **Phase 5**: Build management interfaces
5. **Phase 10**: Add caching before going to production

Remember: Permissions are defined in code (compile-time), but roles are defined by tenants (runtime). This separation is crucial for maintainability and flexibility.