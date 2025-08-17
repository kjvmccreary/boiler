# Phase 9: Advanced Multi-Tenancy with RBAC - Implementation Plan

## 🎯 Phase Overview

**Duration**: 3-4 sessions (12-16 hours)  
**Complexity**: High  
**Prerequisites**: Phase 8 (Docker Configuration) completed

### Objectives
- Implement shared database, shared schema multi-tenancy with complete tenant isolation
- Ensure RBAC system maintains strict tenant boundaries
- Implement tenant-specific permission overrides
- Set up multiple tenant resolution strategies
- Add Row-Level Security (RLS) in PostgreSQL
- Create tenant management UI with role templates

## 📚 Project Context for GitHub Copilot

### Current Architecture
```
Technology Stack:
- Backend: .NET 9 with ASP.NET Core Web API
- Frontend: React + TypeScript + Material UI + Vite
- Database: PostgreSQL with Entity Framework Core
- Authentication: JWT tokens with multi-role RBAC
- Caching: Redis (Docker container)
- Containerization: Docker & Docker Compose (Phase 8 completed)
```

### Microservices Structure
```
src/
├── services/
│   ├── AuthService/      # JWT generation, login/logout
│   ├── UserService/      # User management, RBAC operations
│   └── ApiGateway/       # YARP-based API gateway
├── shared/
│   ├── DTOs/            # Shared data transfer objects
│   ├── Common/          # Common utilities
│   └── Contracts/       # Service contracts
└── frontend/
    └── react-app/       # React SPA with permission-based UI
```

### Current RBAC Implementation (From Recent Migration)
```csharp
// Key Tables in Database:
- Users                 # Core user accounts
- Tenants              # Tenant organizations
- TenantUsers          # Legacy role association (fallback only)
- Roles                # Dynamic roles per tenant
- UserRoles            # Many-to-many user-role assignments
- Permissions          # System-wide permission definitions
- RolePermissions      # Role-permission mappings

// Authorization Pattern:
- JWT tokens contain multiple roles
- All authorization based on permissions (not role names)
- Frontend uses PermissionContext for access control
- Backend uses IPermissionService for checks
```

### Docker Environment (Phase 8 Complete)
```yaml
Services Running:
- postgres:16        # Database with RBAC schema
- redis:7-alpine     # Caching layer
- auth-service       # Port 5001
- user-service       # Port 5002
- api-gateway        # Port 5000
- frontend           # Port 3000
- nginx              # Reverse proxy
```

## 🔨 Implementation Tasks

### Task 1: Tenant Context Infrastructure (Session 1)

#### 1.1 Create Tenant Resolution Service
```csharp
// Location: src/shared/Common/MultiTenancy/TenantResolver.cs

public enum TenantResolutionStrategy
{
    Domain,      // tenant1.app.com
    Path,        // app.com/tenant1
    Header,      // X-Tenant-Id header
    Claim        // JWT tenant claim
}

public interface ITenantResolver
{
    Task<TenantContext?> ResolveAsync(HttpContext context);
}

public class TenantContext
{
    public int TenantId { get; set; }
    public string TenantName { get; set; }
    public string ConnectionString { get; set; }
    public Dictionary<string, string> Settings { get; set; }
}
```

#### 1.2 Implement Tenant Middleware
```csharp
// Location: src/shared/Common/Middleware/TenantMiddleware.cs

public class TenantMiddleware
{
    // Extract tenant from request
    // Validate tenant exists and is active
    // Set tenant context for request
    // Handle missing/invalid tenant scenarios
}
```

#### 1.3 Update DbContext for Multi-Tenancy
```csharp
// Location: src/services/UserService/Data/ApplicationDbContext.cs

public class ApplicationDbContext : DbContext
{
    private readonly ITenantContext _tenantContext;
    
    // Global query filters for tenant isolation
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply tenant filter to all entities
        modelBuilder.Entity<User>()
            .HasQueryFilter(u => u.TenantId == _tenantContext.TenantId);
            
        modelBuilder.Entity<Role>()
            .HasQueryFilter(r => r.TenantId == _tenantContext.TenantId);
    }
}
```

### Task 2: Enhanced Repository Pattern (Session 1-2)

#### 2.1 Base Repository with Tenant Filtering
```csharp
// Location: src/shared/Common/Repositories/TenantAwareRepository.cs

public abstract class TenantAwareRepository<T> where T : ITenantEntity
{
    protected readonly DbContext _context;
    protected readonly ITenantContext _tenantContext;
    
    public virtual async Task<T> GetByIdAsync(int id)
    {
        return await _context.Set<T>()
            .Where(e => e.TenantId == _tenantContext.TenantId && e.Id == id)
            .FirstOrDefaultAsync();
    }
    
    // Prevent cross-tenant operations
    protected void ValidateTenantAccess(T entity)
    {
        if (entity.TenantId != _tenantContext.TenantId)
            throw new CrossTenantAccessException();
    }
}
```

#### 2.2 Role Repository with Tenant Isolation
```csharp
// Location: src/services/UserService/Repositories/RoleRepository.cs

public class RoleRepository : TenantAwareRepository<Role>
{
    // Ensure system roles cannot be modified by tenants
    public async Task<Role> CreateTenantRoleAsync(CreateRoleDto dto)
    {
        if (IsSystemRole(dto.Name))
            throw new InvalidOperationException("Cannot create system role");
            
        var role = new Role
        {
            Name = dto.Name,
            TenantId = _tenantContext.TenantId,
            IsSystemRole = false
        };
        
        return await CreateAsync(role);
    }
}
```

### Task 3: PostgreSQL Row-Level Security (Session 2)

#### 3.1 Enable RLS on Tables
```sql
-- Location: Migrations/AddRowLevelSecurity.cs

-- Enable RLS for core tables
ALTER TABLE users ENABLE ROW LEVEL SECURITY;
ALTER TABLE roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE user_roles ENABLE ROW LEVEL SECURITY;
ALTER TABLE permissions ENABLE ROW LEVEL SECURITY;
ALTER TABLE role_permissions ENABLE ROW LEVEL SECURITY;

-- Create security policies
CREATE POLICY tenant_isolation_policy ON users
    FOR ALL
    USING (tenant_id = current_setting('app.tenant_id')::int);
    
CREATE POLICY tenant_roles_policy ON roles
    FOR ALL
    USING (
        tenant_id = current_setting('app.tenant_id')::int 
        OR is_system_role = true
    );
```

#### 3.2 Set Tenant Context in Database Session
```csharp
// Location: src/services/UserService/Data/TenantDbConnection.cs

public class TenantDbConnection : IDbConnection
{
    public async Task SetTenantContextAsync(int tenantId)
    {
        await ExecuteAsync($"SET app.tenant_id = {tenantId}");
    }
}
```

### Task 4: Tenant Management Features (Session 2-3)

#### 4.1 Tenant Controller with CRUD Operations
```csharp
// Location: src/services/UserService/Controllers/TenantsController.cs

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperAdmin")]
public class TenantsController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateTenant(CreateTenantDto dto)
    {
        // Create tenant
        // Initialize default roles
        // Set up tenant admin user
        // Configure tenant settings
    }
    
    [HttpPost("{tenantId}/initialize")]
    public async Task<IActionResult> InitializeTenant(int tenantId)
    {
        // Create default roles from templates
        // Set up initial permissions
        // Configure tenant-specific settings
    }
}
```

#### 4.2 Role Template Management
```csharp
// Location: src/services/UserService/Services/RoleTemplateService.cs

public class RoleTemplateService
{
    public async Task CreateDefaultRolesForTenant(int tenantId)
    {
        var templates = new[]
        {
            new RoleTemplate 
            { 
                Name = "TenantAdmin", 
                Permissions = new[] { "users.*", "roles.*", "settings.*" }
            },
            new RoleTemplate 
            { 
                Name = "Manager", 
                Permissions = new[] { "users.read", "users.edit", "reports.*" }
            },
            new RoleTemplate 
            { 
                Name = "User", 
                Permissions = new[] { "profile.*", "dashboard.view" }
            }
        };
        
        foreach (var template in templates)
        {
            await CreateRoleFromTemplate(tenantId, template);
        }
    }
}
```

### Task 5: Frontend Multi-Tenancy (Session 3)

#### 5.1 Tenant Context Provider
```typescript
// Location: src/frontend/react-app/src/contexts/TenantContext.tsx

interface TenantContextType {
    currentTenant: Tenant | null;
    switchTenant: (tenantId: number) => Promise<void>;
    availableTenants: Tenant[];
    tenantSettings: TenantSettings;
}

export const TenantProvider: React.FC = ({ children }) => {
    // Resolve tenant from URL/subdomain
    // Load tenant-specific configuration
    // Apply tenant branding/theme
    // Handle tenant switching
};
```

#### 5.2 Tenant-Aware API Client
```typescript
// Location: src/frontend/react-app/src/services/api.ts

class TenantAwareApiClient {
    private getTenantHeaders(): Headers {
        const headers = new Headers();
        const tenant = getTenantFromContext();
        
        if (tenant) {
            headers.append('X-Tenant-Id', tenant.id.toString());
        }
        
        return headers;
    }
    
    async get(url: string): Promise<Response> {
        return fetch(url, {
            headers: this.getTenantHeaders()
        });
    }
}
```

#### 5.3 Role Management UI with Tenant Scope
```typescript
// Location: src/frontend/react-app/src/components/admin/RoleManagement.tsx

export const RoleManagement: React.FC = () => {
    const { currentTenant } = useTenant();
    const { permissions } = usePermissions();
    
    // Only show tenant-specific roles
    // Hide system roles from non-super-admins
    // Prevent modification of system roles
    // Show role templates for initialization
};
```

### Task 6: Testing Tenant Isolation (Session 3-4)

#### 6.1 Integration Tests for Tenant Isolation
```csharp
// Location: tests/UserService.Tests/TenantIsolationTests.cs

[TestClass]
public class TenantIsolationTests
{
    [TestMethod]
    public async Task Users_CannotAccessOtherTenantData()
    {
        // Create two tenants
        // Create users in each tenant
        // Verify User from Tenant A cannot see Tenant B data
        // Test all CRUD operations
    }
    
    [TestMethod]
    public async Task Roles_AreTenantScoped()
    {
        // Create same role name in different tenants
        // Verify they are isolated
        // Test permission assignments are tenant-specific
    }
}
```

#### 6.2 RLS Policy Tests
```sql
-- Location: tests/Database/RLSTests.sql

-- Test that users can only see their tenant's data
SET app.tenant_id = 1;
SELECT COUNT(*) FROM users; -- Should only return tenant 1 users

SET app.tenant_id = 2;
SELECT COUNT(*) FROM users; -- Should only return tenant 2 users

-- Test cross-tenant prevention
SET app.tenant_id = 1;
UPDATE users SET email = 'test@test.com' WHERE tenant_id = 2;
-- Should return 0 rows affected
```

## 📋 Implementation Checklist

### Session 1: Core Infrastructure (4 hours)
- [ ] Create TenantResolver service with multiple strategies
- [ ] Implement TenantMiddleware for request processing
- [ ] Update DbContext with global query filters
- [ ] Create TenantAwareRepository base class
- [ ] Test basic tenant resolution

### Session 2: Database & Security (4 hours)
- [ ] Write migration for RLS policies
- [ ] Implement tenant context in database connections
- [ ] Create tenant management controller
- [ ] Set up role template service
- [ ] Test RLS policies work correctly

### Session 3: Frontend & Integration (4 hours)
- [ ] Create TenantContext provider
- [ ] Update API client for tenant headers
- [ ] Build tenant-aware role management UI
- [ ] Implement tenant switching UI
- [ ] Add tenant branding support

### Session 4: Testing & Refinement (4 hours)
- [ ] Write comprehensive tenant isolation tests
- [ ] Test all CRUD operations with tenant context
- [ ] Verify no cross-tenant data leakage
- [ ] Performance test with multiple tenants
- [ ] Document tenant onboarding process

## 🧪 Testing Strategy

### Unit Tests
```csharp
// Test tenant resolution strategies
// Test repository tenant filtering
// Test role template creation
// Test permission inheritance
```

### Integration Tests
```csharp
// Test multi-tenant API endpoints
// Test tenant switching
// Test RLS policies
// Test tenant initialization
```

### E2E Tests
```typescript
// Test tenant-specific login flows
// Test role management per tenant
// Test data isolation in UI
// Test tenant admin capabilities
```

## 🚨 Critical Considerations

### Security
1. **Never allow cross-tenant data access**
2. **Validate tenant context on every request**
3. **Use RLS as defense in depth**
4. **Audit all tenant-related operations**
5. **Encrypt tenant-specific data at rest**

### Performance
1. **Index all tenant_id columns**
2. **Use tenant-partitioned tables for large datasets**
3. **Cache tenant configuration**
4. **Monitor query performance per tenant**

### Data Integrity
1. **Foreign keys must include tenant_id**
2. **Unique constraints should be tenant-scoped**
3. **Cascade deletes must respect tenant boundaries**

## 🎯 Success Criteria

✅ **Phase 9 is complete when:**
1. Tenant isolation is bulletproof - no data leakage
2. Multiple tenant resolution strategies work
3. RLS policies are active and tested
4. Role management is fully tenant-scoped
5. Frontend respects tenant boundaries
6. New tenant initialization is automated
7. All tests pass with >90% coverage
8. Performance remains <100ms for tenant operations

## 📚 Key Files to Modify/Create

### Backend
```
src/shared/Common/
├── MultiTenancy/
│   ├── ITenantResolver.cs
│   ├── TenantResolver.cs
│   ├── TenantContext.cs
│   └── TenantResolutionStrategy.cs
├── Middleware/
│   └── TenantMiddleware.cs
└── Repositories/
    └── TenantAwareRepository.cs

src/services/UserService/
├── Controllers/
│   └── TenantsController.cs
├── Services/
│   ├── TenantService.cs
│   └── RoleTemplateService.cs
└── Data/
    ├── Migrations/
    │   └── AddRowLevelSecurity.cs
    └── TenantDbConnection.cs
```

### Frontend
```
src/frontend/react-app/src/
├── contexts/
│   └── TenantContext.tsx
├── hooks/
│   └── useTenant.ts
├── components/
│   ├── admin/
│   │   ├── TenantManagement.tsx
│   │   └── RoleTemplates.tsx
│   └── common/
│       └── TenantSwitcher.tsx
└── services/
    └── tenantApi.ts
```

## 🔄 Migration Path from Phase 8

Since Docker is now complete, ensure:
1. Update docker-compose.yml with tenant-specific environment variables
2. Add tenant resolution strategy to .env files
3. Update nginx config for subdomain routing if using domain-based resolution
4. Ensure database migrations run on container startup

## 📖 Documentation Needs

Create/Update:
1. Tenant onboarding guide
2. RLS policy documentation
3. Tenant admin manual
4. Multi-tenancy architecture diagram
5. Troubleshooting guide for tenant issues

## 💬 Context for GitHub Copilot Chat

When working on Phase 9, provide Copilot with:
```
"I'm implementing Phase 9 of a .NET 9 microservices project with multi-tenancy and RBAC. 
Current state:
- Using PostgreSQL with Entity Framework Core
- JWT authentication with multi-role support
- Docker containers running (Phase 8 complete)
- Shared database, shared schema approach
- Need strict tenant isolation with RLS

Working on: [specific task from this plan]
Need help with: [specific implementation detail]
```

## 🚀 Next Phase Preview (Phase 10)

After Phase 9, you'll implement:
- Redis caching for permissions and tenant data
- Performance optimization for multi-tenant queries
- Cache invalidation strategies
- Permission evaluation optimization
- Batch loading for role/permission data
