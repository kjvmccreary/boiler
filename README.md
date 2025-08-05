# Microservices Starter Application

## Overview
Create a simple microservices starter application that can be used as a template for future projects. The application should include basic functionality such as user authentication, data storage, and a simple user interface.

## Tech Stack & Assumptions

### Backend
- **ASP.NET Core** - Primary backend framework
- **Entity Framework Core** - ORM for data access
- **Code-first approach** - Database schema management
- **EF Migrations** - Database version control and updates
- **Repository pattern** - Data access abstraction

### Frontend
- **Vite + React** - Modern frontend build tool and framework
- **Material UI Pro** - Premium component library
- **Tailwind CSS** - Utility-first CSS framework

### Database & Storage
- **PostgreSQL** - Primary database
- **Redis** - Caching and session storage

### Infrastructure
- **Docker** - Containerization for all services
- **nginx** - Reverse proxy and load balancer
- **RabbitMQ** - Message broker for async communication

### Architecture
- **Microservices** - Distributed service architecture
- **Multi-tenant** - Support for multiple tenants with data isolation
- **Shared libraries** - DTOs and common logic (not business functionality)
- **Deployment flexibility** - Cloud, on-premise, and government cloud ready

## Project Structure

```
starter-app/
├── src/
│   ├── services/
│   │   ├── UserService/
│   │   ├── AuthService/
│   │   └── ApiGateway/
│   ├── frontend/
│   │   └── react-app/
│   ├── shared/
│   │   ├── DTOs/
│   │   ├── Common/
│   │   └── Contracts/
│   └── infrastructure/
│       ├── docker/
│       ├── nginx/
│       └── scripts/
├── tests/
├── docs/
├── docker-compose.yml
├── docker-compose.override.yml
└── README.md
```

## Core Services

### Authentication Service
- User registration and login
- JWT token management
- Password reset functionality
- Role-based authorization

### User Service
- User profile management
- User data CRUD operations
- User preferences

### API Gateway
- Request routing
- Authentication middleware
- Rate limiting
- Request/response logging
- Tenant resolution and routing

## Multi-Tenancy Architecture

### Early Stage Implementation (MVP)

#### Shared Database, Shared Schema (Default)
- **Primary model** for both cloud and on-premise deployments
- Single database with tenant ID in all tables
- Cost-effective and operationally simple
- Shared infrastructure and maintenance
- Row-level security for data isolation
- Consistent schema across all deployment types

**Benefits for Early Stage:**
- Single codebase for all deployment scenarios
- Simplified database migrations and updates
- Reduced operational complexity
- Faster time to market
- Lower infrastructure costs

### Future Scaling Options

#### Shared Database, Separate Schema (Growth Phase)
- Database per tenant schema within shared PostgreSQL instance
- Good balance of isolation and cost efficiency
- Easier compliance for regulated industries
- Schema-level customization per tenant
- Migration path from shared schema

#### Separate Database (Enterprise/High-Security)
- Dedicated database instance per tenant
- Maximum data isolation and security
- Required for government and high-security deployments
- Custom backup and recovery per tenant
- Higher operational overhead
- Future option for large enterprise clients

### Tenant Resolution Strategy

#### Domain-Based Resolution
```
tenant1.yourapp.com → Tenant: tenant1
tenant2.yourapp.com → Tenant: tenant2
custom-domain.com → Tenant: enterprise-client
```

#### Path-Based Resolution
```
yourapp.com/tenant1/api/users → Tenant: tenant1
yourapp.com/tenant2/api/users → Tenant: tenant2
```

#### Header-Based Resolution
```
X-Tenant-ID: tenant1
Authorization: Bearer jwt-with-tenant-claim
```

### Data Isolation Patterns

#### Row-Level Security (RLS)
- PostgreSQL RLS policies per tenant
- Automatic filtering at database level
- Transparent to application code
- Performance considerations for large datasets

#### Application-Level Filtering
- Tenant context in all queries
- Repository pattern with tenant injection
- Explicit tenant validation
- Custom query interceptors

#### Service-Level Isolation
- Tenant-specific service instances
- Container orchestration per tenant
- Network-level isolation
- Independent scaling per tenant

### Deployment Models

#### Cloud-Native Multi-Tenant (Default - Early Stage)
- **Target**: AWS, Azure, GCP
- **Architecture**: Shared database, shared schema
- Shared infrastructure with logical separation
- Horizontal scaling across tenants
- Centralized monitoring and logging
- Cost optimization through resource sharing

#### On-Premise Deployment (Early Stage)
- **Target**: Enterprise data centers
- **Architecture**: Same shared schema as cloud deployment
- Customer-controlled infrastructure
- Single database instance with tenant ID isolation
- Simplified deployment and maintenance
- Local authentication integration (AD/LDAP)
- Custom SSL certificate management

**Early Stage Benefits:**
- Consistent codebase between cloud and on-premise
- Single database migration strategy
- Unified testing and development process
- Easier support and troubleshooting

#### Government Cloud (FedRAMP/IL2-5)
- **Target**: AWS GovCloud, Azure Government, Google Cloud for Government
- **Architecture**: Initially shared schema, migration path to separate databases
- Enhanced security controls and compliance
- US persons only access requirements
- Advanced audit logging and monitoring
- Encryption at rest and in transit (FIPS 140-2)
- Network segmentation and VPC isolation

### Architecture Evolution Path

#### Phase 1: MVP (Months 0-12)
- Shared database, shared schema for all deployments
- Row-level security for tenant isolation
- Single codebase across all environments
- Focus on feature development and market validation

#### Phase 2: Growth (Months 12-24)
- Introduce schema-per-tenant for larger enterprise clients
- Maintain shared schema for smaller tenants
- Performance optimization and scaling improvements
- Enhanced security features

#### Phase 3: Enterprise Scale (24+ Months)
- Database-per-tenant for high-security requirements
- Hybrid deployment models per customer needs
- Advanced compliance and audit features
- Custom tenant-specific configurations

### Tenant Configuration Management

#### Tenant Registry Service
- Centralized tenant metadata
- Feature flag management per tenant
- Subscription and billing integration
- Custom branding and themes
- Regional data residency settings

#### Environment Variables per Deployment
```bash
# Early Stage - All Deployments (Cloud, On-Premise, Gov Cloud)
TENANT_RESOLUTION_STRATEGY=domain
TENANT_MODEL=shared_database_shared_schema
ENABLE_ROW_LEVEL_SECURITY=true

# Cloud Deployment
DEPLOYMENT_TYPE=cloud
ENABLE_AUTO_SCALING=true
ENABLE_MULTI_REGION=false

# On-Premise Deployment
DEPLOYMENT_TYPE=on_premise
LOCAL_AUTH_PROVIDER=active_directory
OFFLINE_MODE=true
CUSTOM_SSL_ENABLED=true

# Government Cloud Deployment
DEPLOYMENT_TYPE=gov_cloud
COMPLIANCE_LEVEL=fedramp_moderate
AUDIT_RETENTION_YEARS=7
ENCRYPTION_STANDARD=fips_140_2
ACCESS_CONTROL_LEVEL=strict

# Future Evolution Flags (Disabled Initially)
ENABLE_SCHEMA_PER_TENANT=false
ENABLE_DATABASE_PER_TENANT=false
```

### Security Considerations for Multi-Tenancy

#### Tenant Isolation
- Database-level access controls
- API endpoint authorization per tenant
- File storage isolation (separate S3 buckets/containers)
- Cache namespace separation
- Message queue topic isolation

#### Cross-Tenant Data Protection
- Strict tenant context validation
- Audit trails for all cross-tenant access attempts
- Data classification and labeling
- Regular security assessments
- Penetration testing for tenant isolation

#### Compliance Requirements
- **GDPR**: Data portability and right to be forgotten per tenant
- **HIPAA**: Enhanced security for healthcare tenants
- **SOX**: Financial data isolation and audit trails
- **FedRAMP**: Government compliance for federal tenants

### Operational Considerations

#### Monitoring and Alerting
- Tenant-specific dashboards
- Per-tenant performance metrics
- Isolated log streams
- Custom alerting rules per tenant type
- Tenant usage analytics

#### Backup and Recovery
- Tenant-specific backup schedules
- Point-in-time recovery per tenant
- Cross-tenant disaster recovery testing
- Data export capabilities
- Tenant migration tools

#### Performance and Scaling
- Tenant-aware load balancing
- Resource quotas per tenant
- Auto-scaling based on tenant usage
- Performance isolation (noisy neighbor prevention)
- Database sharding strategies

### Tenant Onboarding and Management

#### Automated Tenant Provisioning
- Self-service tenant registration
- Automated database schema/namespace creation
- Default configuration and feature flags
- Initial admin user setup
- Welcome email and onboarding workflow

#### Tenant Administration Portal
- Tenant settings and configuration management
- User and role management per tenant
- Billing and subscription management
- Usage analytics and reporting
- Support ticket integration

#### Tenant Migration and Data Portability
- Export tenant data in standard formats
- Import/migration tools for tenant data
- Schema version compatibility checks
- Zero-downtime tenant migrations
- Compliance with data portability regulations

## Database Design

### Early Stage: Code First with EF Migrations

#### Primary Entity Structure (All Deployments)
```csharp
// Base entity for all tenant-scoped models
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

// All entities include TenantId for tenant isolation
public class User : BaseEntity
{
    public Guid TenantId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
}
```

#### DbContext Configuration with Fluent API
```csharp
public class ApplicationDbContext : DbContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, 
        IHttpContextAccessor httpContextAccessor) : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<TenantUser> TenantUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.TenantId, e.Email }).IsUnique();
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            
            // Tenant relationship
            entity.HasOne(e => e.Tenant)
                  .WithMany()
                  .HasForeignKey(e => e.TenantId)
                  .OnDelete(DeleteBehavior.Restrict);
                  
            // Row-level security via check constraint
            entity.HasCheckConstraint("CK_User_TenantId", 
                "\"TenantId\" = current_setting('app.current_tenant')::UUID");
        });

        base.OnModelCreating(modelBuilder);
    }

    // Set tenant context before saving
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var tenantId = GetCurrentTenantId();
        if (tenantId.HasValue)
        {
            await Database.ExecuteSqlRawAsync(
                "SELECT set_config('app.current_tenant', {0}, false)", tenantId);
        }
        
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    private Guid? GetCurrentTenantId()
    {
        // Extract tenant ID from HTTP context, JWT claims, etc.
        // Implementation depends on tenant resolution strategy
        return null; // Placeholder
    }
}
```

#### Migration Example for Row-Level Security
```csharp
// Generated migration file: 20240101000000_AddRowLevelSecurity.cs
public partial class AddRowLevelSecurity : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Enable Row Level Security on Users table
        migrationBuilder.Sql(@"
            ALTER TABLE ""Users"" ENABLE ROW LEVEL SECURITY;
            
            CREATE POLICY tenant_isolation_policy ON ""Users""
                USING (""TenantId"" = current_setting('app.current_tenant')::UUID);
        ");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(@"
            DROP POLICY IF EXISTS tenant_isolation_policy ON ""Users"";
            ALTER TABLE ""Users"" DISABLE ROW LEVEL SECURITY;
        ");
    }
}
```

### Core Entities

#### Tenant Management Entities
```csharp
public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string SubscriptionPlan { get; set; } = "Basic";
    public bool IsActive { get; set; } = true;
    public string Settings { get; set; } = "{}"; // JSON string
    
    // Navigation properties
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<TenantUser> TenantUsers { get; set; } = new List<TenantUser>();
}

public class TenantUser : BaseEntity
{
    public Guid TenantId { get; set; }
    public int UserId { get; set; }
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}
```

#### Entity Configuration in DbContext
```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // Tenant entity configuration
    modelBuilder.Entity<Tenant>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => e.Domain).IsUnique();
        entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
        entity.Property(e => e.Domain).HasMaxLength(255);
        entity.Property(e => e.SubscriptionPlan).HasMaxLength(50);
        entity.Property(e => e.Settings).HasColumnType("jsonb");
    });

    // TenantUser entity configuration (junction table)
    modelBuilder.Entity<TenantUser>(entity =>
    {
        entity.HasKey(e => e.Id);
        entity.HasIndex(e => new { e.TenantId, e.UserId }).IsUnique();
        entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
        
        // Relationships
        entity.HasOne(e => e.Tenant)
              .WithMany(t => t.TenantUsers)
              .HasForeignKey(e => e.TenantId)
              .OnDelete(DeleteBehavior.Cascade);
              
        entity.HasOne(e => e.User)
              .WithMany(u => u.TenantUsers)
              .HasForeignKey(e => e.UserId)
              .OnDelete(DeleteBehavior.Cascade);
    });

    base.OnModelCreating(modelBuilder);
}
```

#### Standard Multi-Tenant Entity Example
```csharp
// Example: Orders entity with tenant isolation
public class Order : BaseEntity
{
    public Guid TenantId { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    
    // Navigation properties
    public Tenant Tenant { get; set; } = null!;
    public User User { get; set; } = null!;
}

// Configuration for Orders entity
modelBuilder.Entity<Order>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.HasIndex(e => new { e.TenantId, e.OrderNumber }).IsUnique();
    entity.Property(e => e.TenantId).IsRequired();
    entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
    entity.Property(e => e.Status).IsRequired().HasMaxLength(50);
    entity.Property(e => e.TotalAmount).HasColumnType("decimal(10,2)");
    
    // Tenant relationship
    entity.HasOne(e => e.Tenant)
          .WithMany()
          .HasForeignKey(e => e.TenantId)
          .OnDelete(DeleteBehavior.Restrict);
          
    // User relationship with tenant validation
    entity.HasOne(e => e.User)
          .WithMany()
          .HasForeignKey(e => e.UserId)
          .OnDelete(DeleteBehavior.Restrict);
          
    // Row-level security
    entity.HasCheckConstraint("CK_Order_TenantId", 
        "\"TenantId\" = current_setting('app.current_tenant')::UUID");
});
```

### Future Evolution Options

#### Schema-Per-Tenant (Phase 2)
```csharp
// Future DbContext enhancement for schema-per-tenant
public class MultiSchemaDbContext : DbContext
{
    private readonly string _schema;
    
    public MultiSchemaDbContext(DbContextOptions<MultiSchemaDbContext> options, 
        string schema = "public") : base(options)
    {
        _schema = schema;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Apply schema to all entities
        modelBuilder.HasDefaultSchema(_schema);
        
        // Same entity configurations but in tenant-specific schema
        ConfigureEntities(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}

// Migration strategy for schema creation
public partial class CreateTenantSchema : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql("CREATE SCHEMA IF NOT EXISTS tenant_abc123;");
        // Create tables in new schema...
    }
}
```

#### Database-Per-Tenant (Phase 3)
```csharp
// Future DbContext factory for separate databases
public class TenantDbContextFactory : IDbContextFactory<ApplicationDbContext>
{
    private readonly IServiceProvider _serviceProvider;
    
    public ApplicationDbContext CreateDbContext(string tenantId)
    {
        var connectionString = GetTenantConnectionString(tenantId);
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(connectionString);
        
        return new ApplicationDbContext(optionsBuilder.Options, 
            _serviceProvider.GetRequiredService<IHttpContextAccessor>());
    }
    
    private string GetTenantConnectionString(string tenantId)
    {
        // Return tenant-specific database connection string
        return $"Host=localhost;Database=tenant_{tenantId};Username=app;Password=...";
    }
}
```

### EF Core Best Practices

#### Repository Pattern Implementation
```csharp
// Base repository interface
public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
    IQueryable<T> Query();
}

// Tenant-aware repository base class
public class TenantRepository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;
    protected readonly Guid _tenantId;

    public TenantRepository(ApplicationDbContext context, ITenantProvider tenantProvider)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _tenantId = tenantProvider.GetCurrentTenantId();
    }

    public virtual IQueryable<T> Query()
    {
        // All queries automatically filtered by tenant
        return _dbSet.Where(e => EF.Property<Guid>(e, "TenantId") == _tenantId);
    }

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    // Other repository methods...
}

// Specific repository interfaces
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}

// Implementation with tenant isolation
public class UserRepository : TenantRepository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext context, ITenantProvider tenantProvider) 
        : base(context, tenantProvider)
    {
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Query()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AnyAsync(u => u.Email == email, cancellationToken);
    }
}
```

#### Migration Management Strategy
```bash
# Development workflow
dotnet ef migrations add InitialCreate
dotnet ef database update

# Environment-specific migrations
dotnet ef migrations add AddRowLevelSecurity
dotnet ef database update --connection "ConnectionString"

# Production deployment
dotnet ef migrations script --from LastMigration --to LatestMigration --output migration.sql
```

### User Management
- User entities with tenant-scoped profile information
- Roles and permissions system per tenant via EF configuration
- Audit trail for user actions with tenant context using EF change tracking
- Cross-tenant user support for admin users via navigation properties

### Configuration
- Application settings with tenant-specific overrides stored as JSON columns
- Feature flags with tenant inheritance using EF value converters
- Environment-specific configurations via IConfiguration
- Global settings with tenant customization through entity relationships

## Development Guidelines

### Early Stage Architecture Benefits
- **Single Codebase**: Same application logic for cloud, on-premise, and government deployments
- **Simplified Testing**: One database schema to test across all deployment scenarios
- **Faster Development**: No need to abstract tenant isolation patterns initially
- **Easy Migrations**: Single migration path for schema changes
- **Reduced Complexity**: Focus on business features rather than architectural abstractions

### Code Standards
- Follow C# coding conventions
- Use async/await for I/O operations
- Implement proper error handling and logging
- Write unit tests for business logic
- Document public APIs with XML comments

### Git Workflow
- Feature branch workflow
- Pull request reviews required
- Automated testing on CI/CD pipeline
- Semantic versioning for releases

### API Design
- RESTful endpoints
- Consistent response formats
- Proper HTTP status codes
- API versioning strategy
- OpenAPI/Swagger documentation

## Environment Setup

### Prerequisites
- .NET 8.0 SDK
- Node.js 18+
- Docker Desktop
- PostgreSQL (for local development)
- Redis (for local development)

### Local Development
1. Clone repository
2. Run `docker-compose up -d` for infrastructure
3. Navigate to each service and run `dotnet restore`
4. Run database migrations
5. Start frontend with `npm run dev`
6. Start backend services

### Environment Variables
```bash
# Database
DATABASE_CONNECTION_STRING=
REDIS_CONNECTION_STRING=

# Authentication
JWT_SECRET_KEY=
JWT_EXPIRY_MINUTES=

# External Services
RABBITMQ_CONNECTION_STRING=

# Frontend
VITE_API_BASE_URL=
```

## Docker Configuration

### Services
- **postgres** - Database service
- **redis** - Caching service
- **rabbitmq** - Message broker
- **nginx** - Reverse proxy
- **auth-service** - Authentication microservice
- **user-service** - User management microservice
- **api-gateway** - API gateway service
- **frontend** - React application

### Networking
- All services communicate through Docker network
- nginx exposes ports 80/443 externally
- Internal service communication on private network

## Security Considerations

### Authentication & Authorization
- JWT tokens with appropriate expiry
- Refresh token rotation
- Role-based access control (RBAC)
- API key authentication for service-to-service

### Data Protection
- Encrypt sensitive data at rest
- Use HTTPS for all communications
- Input validation and sanitization
- SQL injection prevention through parameterized queries

### Infrastructure Security
- Docker secrets for sensitive configuration
- Network segmentation
- Regular security updates
- Monitoring and alerting

## Testing Strategy

### Unit Tests
- Service layer business logic
- Repository pattern implementations
- Utility functions and helpers

### Integration Tests
- Database operations
- External API integrations
- Message queue operations

### End-to-End Tests
- Critical user journeys
- Authentication flows
- API contract testing

## Deployment

### Cloud-Native SaaS Deployment
- **Target**: AWS, Azure, GCP public cloud
- Docker Swarm or Kubernetes orchestration
- Automated deployment from main branch
- Multi-tenant shared infrastructure
- Auto-scaling based on aggregate tenant usage
- Centralized monitoring across all tenants

### On-Premise Enterprise Deployment
- **Target**: Customer data centers
- Docker Compose or Kubernetes deployment
- Air-gapped network support
- Single-tenant or limited multi-tenant setup
- Customer-managed infrastructure
- Integration with existing enterprise systems:
  - Active Directory/LDAP authentication
  - Enterprise monitoring tools
  - Corporate SSL certificates
  - Network security appliances

#### On-Premise Deployment Package
```bash
# Offline deployment bundle
enterprise-deployment/
├── docker-images/           # Pre-built container images
├── helm-charts/            # Kubernetes deployment charts
├── docker-compose/         # Docker Compose files
├── scripts/
│   ├── install.sh         # Automated installation
│   ├── backup.sh          # Backup procedures
│   └── update.sh          # Version updates
├── config/
│   ├── production.env     # Environment templates
│   └── ssl/               # Certificate management
└── docs/
    ├── installation-guide.md
    ├── admin-guide.md
    └── troubleshooting.md
```

### Government Cloud Deployment
- **Target**: AWS GovCloud, Azure Government, Google Cloud for Government
- FedRAMP compliance requirements
- Enhanced security controls and monitoring
- US persons only access requirements
- Advanced audit logging and data retention
- FIPS 140-2 encryption standards
- Network segmentation and VPC isolation

#### Government Cloud Requirements
```bash
# Additional security configurations
FIPS_ENCRYPTION_ENABLED=true
AUDIT_LOG_RETENTION_YEARS=7
ACCESS_CONTROL_LEVEL=strict
NETWORK_SEGMENTATION=enabled
DATA_RESIDENCY=us_only
COMPLIANCE_REPORTING=fedramp
```

### Deployment Automation

#### Infrastructure as Code
- Terraform modules for each deployment type
- Environment-specific variable files
- Automated provisioning and teardown
- Resource tagging and cost allocation

#### CI/CD Pipeline Variations
```yaml
# Multi-environment pipeline
stages:
  - build
  - test
  - security-scan
  - deploy-dev
  - deploy-staging-saas
  - deploy-staging-onprem
  - deploy-staging-govcloud
  - production-approval
  - deploy-production
```

### Environment-Specific Considerations

#### SaaS Cloud Environment
- High availability across multiple regions
- Load balancing with nginx or cloud load balancers
- Database clustering and read replicas
- Automated backup and disaster recovery
- Cost optimization and resource sharing

#### On-Premise Environment
- Single-region deployment with local redundancy
- Customer-provided backup solutions
- Local certificate authority integration
- Firewall and proxy configuration
- Limited internet connectivity support

#### Government Cloud Environment
- Isolated network environments
- Enhanced monitoring and alerting
- Compliance reporting automation
- Restricted access controls
- Advanced threat detection

## Monitoring & Observability

### Logging
- Structured logging with Serilog
- Centralized log aggregation
- Log levels and filtering
- Request correlation IDs

### Metrics
- Application performance metrics
- Business metrics tracking
- Infrastructure monitoring
- Custom dashboards

### Health Checks
- Service health endpoints
- Database connectivity checks
- External dependency monitoring
- Automated alerting

## Performance Considerations

### Caching Strategy
- Redis for session data
- Application-level caching
- Database query optimization
- CDN for static assets

### Database Optimization
- Proper indexing strategy
- Query optimization
- Connection pooling
- Read replicas for scaling

### Frontend Performance
- Code splitting and lazy loading
- Asset optimization
- Progressive web app features
- Performance monitoring

## Future Enhancements

### Potential Features
- Multi-tenant support
- Advanced analytics
- Mobile application support
- Third-party integrations
- Advanced workflow engine

### Architecture Evolution
- Event sourcing patterns
- CQRS implementation
- Distributed caching
- Service mesh adoption
- Serverless functions integration

## Documentation

### API Documentation
- OpenAPI specifications
- Postman collections
- Integration examples
- SDK generation

### Developer Documentation
- Setup guides
- Architecture decisions
- Troubleshooting guides
- Best practices

## Support & Maintenance

### Issue Tracking
- Bug reporting process
- Feature request workflow
- Documentation updates
- Version release notes

### Community
- Contributing guidelines
- Code review process
- Knowledge sharing sessions
- Technical debt management


# Microservices Starter App - Implementation Phases

## Overview
This document breaks down the microservices starter application into manageable implementation phases. Each phase builds upon the previous ones and can be completed in separate development sessions.

## Phase 1: Foundation & Project Structure
**Duration**: 1-2 sessions  
**Complexity**: Low

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
   - Create ASP.NET Core Web API projects for each service
   - Set up shared class libraries for DTOs and common utilities
   - Initialize Vite + React frontend project
   - Configure solution file (.sln)

3. **Development Environment**
   - Basic README.md with setup instructions
   - .gitignore files
   - EditorConfig for consistent coding standards
   - Basic project dependencies (without implementation)

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

### Objectives
- Implement shared DTOs and common utilities
- Set up logging, configuration, and middleware foundations
- Create base classes and interfaces

### Deliverables
1. **Shared DTOs Project**
   ```csharp
   // User-related DTOs
   public class UserDto { }
   public class LoginRequestDto { }
   public class RegisterRequestDto { }
   public class TokenResponseDto { }
   
   // Common DTOs
   public class ApiResponseDto<T> { }
   public class ErrorDto { }
   public class PagedResultDto<T> { }
   ```

2. **Common Utilities Project**
   ```csharp
   // Base entity classes
   public abstract class BaseEntity { }
   
   // Configuration helpers
   public static class ConfigurationExtensions { }
   
   // Logging helpers
   public static class LoggingExtensions { }
   
   // Validation helpers
   public static class ValidationExtensions { }
   ```

3. **Service Contracts**
   ```csharp
   // Service interfaces
   public interface IAuthService { }
   public interface IUserService { }
   public interface ITenantService { }
   ```

4. **Configuration Setup**
   - appsettings.json templates for each environment
   - Configuration models for each service
   - Logging configuration with Serilog

### Dependencies
- Phase 1 completion
- NuGet packages: Serilog, FluentValidation, AutoMapper

### Next Phase Prerequisites
- Shared libraries compile and are referenced by service projects
- Basic configuration and logging framework is in place
- Common patterns are established

---

## Phase 3: Database Foundation & Entity Framework
**Duration**: 2-3 sessions  
**Complexity**: Medium

### Objectives
- Set up PostgreSQL database with Entity Framework Core
- Implement core entities with multi-tenant support
- Create repository pattern and DbContext

### Deliverables
1. **Core Entities**
   ```csharp
   public class Tenant : BaseEntity
   public class User : BaseEntity
   public class TenantUser : BaseEntity
   public class RefreshToken : BaseEntity
   ```

2. **DbContext Implementation**
   ```csharp
   public class ApplicationDbContext : DbContext
   {
       // DbSets
       // OnModelCreating with fluent API
       // Tenant context management
       // Row-level security setup
   }
   ```

3. **Repository Pattern**
   ```csharp
   public interface IRepository<T> where T : BaseEntity
   public class TenantRepository<T> : IRepository<T>
   public interface IUserRepository : IRepository<User>
   public class UserRepository : TenantRepository<User>, IUserRepository
   ```

4. **Database Migrations**
   - Initial migration with core tables
   - Row-level security migration
   - Seed data migration

5. **Tenant Provider**
   ```csharp
   public interface ITenantProvider
   public class TenantProvider : ITenantProvider
   ```

### Dependencies
- Phase 2 completion
- PostgreSQL (local or Docker)
- NuGet packages: Npgsql.EntityFrameworkCore.PostgreSQL, Microsoft.EntityFrameworkCore.Tools

### Next Phase Prerequisites
- Database successfully created and migrated
- Repository pattern working with basic CRUD operations
- Tenant context properly isolated data

---

## Phase 4: Authentication Service
**Duration**: 2-3 sessions  
**Complexity**: Medium-High

### Objectives
- Implement JWT-based authentication
- Create user registration and login endpoints
- Set up password hashing and validation

### Deliverables
1. **Authentication Controller**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class AuthController : ControllerBase
   {
       // POST /api/auth/register
       // POST /api/auth/login
       // POST /api/auth/refresh
       // POST /api/auth/logout
   }
   ```

2. **Authentication Services**
   ```csharp
   public interface IAuthService
   public class AuthService : IAuthService
   public interface ITokenService
   public class TokenService : ITokenService
   public interface IPasswordService
   public class PasswordService : IPasswordService
   ```

3. **JWT Configuration**
   - JWT middleware setup
   - Token generation and validation
   - Refresh token functionality
   - Tenant claims in JWT

4. **Validation & Error Handling**
   - FluentValidation for DTOs
   - Global exception handler
   - Standardized API responses

5. **Unit Tests**
   - Authentication service tests
   - Token service tests
   - Controller tests

### Dependencies
- Phase 3 completion
- NuGet packages: Microsoft.AspNetCore.Authentication.JwtBearer, BCrypt.Net-Next

### Next Phase Prerequisites
- User registration and login working
- JWT tokens generated and validated correctly
- Tenant context preserved in authentication

---

## Phase 5: User Service
**Duration**: 1-2 sessions  
**Complexity**: Medium

### Objectives
- Implement user management functionality
- Create user profile endpoints
- Set up user-tenant relationship management

### Deliverables
1. **User Controller**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   [Authorize]
   public class UsersController : ControllerBase
   {
       // GET /api/users/profile
       // PUT /api/users/profile
       // GET /api/users
       // GET /api/users/{id}
       // DELETE /api/users/{id}
   }
   ```

2. **User Services**
   ```csharp
   public interface IUserService
   public class UserService : IUserService
   public interface IUserProfileService
   public class UserProfileService : IUserProfileService
   ```

3. **Authorization Setup**
   - Role-based authorization
   - Tenant-scoped user access
   - Permission-based actions

4. **Integration Tests**
   - User CRUD operations
   - Authorization scenarios
   - Tenant isolation verification

### Dependencies
- Phase 4 completion
- Authentication working properly

### Next Phase Prerequisites
- User management endpoints functional
- Proper authorization and tenant isolation
- Integration tests passing

---

## Phase 6: API Gateway
**Duration**: 2-3 sessions  
**Complexity**: Medium-High

### Objectives
- Implement API Gateway with Ocelot or YARP
- Set up request routing and load balancing
- Implement rate limiting and middleware

### Deliverables
1. **API Gateway Project**
   ```csharp
   public class Program
   {
       // API Gateway startup
       // Service registration
       // Middleware pipeline
   }
   ```

2. **Gateway Configuration**
   ```json
   {
     "Routes": [
       {
         "DownstreamPathTemplate": "/api/auth/{everything}",
         "DownstreamScheme": "http",
         "DownstreamHostAndPorts": [...],
         "UpstreamPathTemplate": "/api/auth/{everything}",
         "UpstreamHttpMethod": ["GET", "POST"]
       }
     ]
   }
   ```

3. **Middleware Components**
   ```csharp
   public class TenantResolutionMiddleware
   public class RequestLoggingMiddleware
   public class RateLimitingMiddleware
   public class AuthenticationMiddleware
   ```

4. **Service Discovery**
   - Basic service registration
   - Health check endpoints
   - Automatic failover setup

### Dependencies
- Phase 5 completion
- Ocelot or YARP NuGet package

### Next Phase Prerequisites
- API Gateway successfully routing requests
- All services accessible through gateway
- Rate limiting and logging working

---

## Phase 7: React Frontend Foundation
**Duration**: 2-3 sessions  
**Complexity**: Medium

### Objectives
- Set up React application with Material UI Pro and Tailwind
- Implement authentication flows
- Create basic layout and routing

### Deliverables
1. **Project Setup**
   ```bash
   # Vite + React + TypeScript
   # Material UI Pro configuration
   # Tailwind CSS integration
   # ESLint and Prettier setup
   ```

2. **Authentication Components**
   ```tsx
   // LoginForm component
   // RegisterForm component
   // AuthProvider context
   // ProtectedRoute component
   ```

3. **Layout Components**
   ```tsx
   // AppLayout with navigation
   // Header with user menu
   // Sidebar navigation
   // Footer component
   ```

4. **Routing Setup**
   ```tsx
   // React Router configuration
   // Public and protected routes
   // Route guards
   ```

5. **API Client**
   ```tsx
   // Axios configuration
   // API service classes
   // Error handling
   // Loading states
   ```

### Dependencies
- Phase 6 completion
- Node.js and npm/yarn
- Material UI Pro license (or fallback to free version)

### Next Phase Prerequisites
- Frontend application loads and displays correctly
- Authentication flow working end-to-end
- API integration successful

---

## Phase 8: Docker Configuration
**Duration**: 2-3 sessions  
**Complexity**: Medium

### Objectives
- Containerize all services
- Set up Docker Compose for local development
- Configure networking and service communication

### Deliverables
1. **Dockerfiles**
   ```dockerfile
   # Dockerfile for each service
   # Multi-stage builds for optimization
   # Non-root user configuration
   ```

2. **Docker Compose**
   ```yaml
   # docker-compose.yml
   # docker-compose.override.yml for development
   # Services: postgres, redis, rabbitmq, nginx
   # Application services configuration
   ```

3. **Database Setup**
   ```bash
   # PostgreSQL container configuration
   # Database initialization scripts
   # Migration execution on startup
   ```

4. **nginx Configuration**
   ```nginx
   # Reverse proxy setup
   # Load balancing configuration
   # SSL termination
   ```

5. **Development Scripts**
   ```bash
   # start-dev.sh
   # stop-dev.sh
   # rebuild.sh
   # logs.sh
   ```

### Dependencies
- Phase 7 completion
- Docker Desktop installed
- All services working individually

### Next Phase Prerequisites
- All services running in Docker containers
- Services can communicate with each other
- Development workflow streamlined

---

## Phase 9: Multi-Tenant Implementation
**Duration**: 3-4 sessions  
**Complexity**: High

### Objectives
- Implement shared database, shared schema multi-tenancy
- Set up tenant resolution strategies
- Ensure complete data isolation

### Deliverables
1. **Tenant Resolution**
   ```csharp
   public enum TenantResolutionStrategy
   {
       Domain,
       Path,
       Header
   }
   
   public class TenantResolver
   public class TenantMiddleware
   ```

2. **Enhanced Repository Pattern**
   ```csharp
   // Automatic tenant filtering
   // Tenant-scoped queries
   // Cross-tenant prevention
   ```

3. **Tenant Management**
   ```csharp
   [ApiController]
   [Route("api/[controller]")]
   public class TenantsController : ControllerBase
   {
       // Tenant CRUD operations
       // Tenant settings management
       // User-tenant associations
   }
   ```

4. **Row-Level Security**
   ```sql
   -- PostgreSQL RLS policies
   -- Tenant context functions
   -- Security testing
   ```

5. **Frontend Multi-Tenancy**
   ```tsx
   // Tenant context provider
   // Dynamic branding
   // Tenant-specific routing
   ```

### Dependencies
- Phase 8 completion
- PostgreSQL RLS understanding
- Multi-tenant testing strategy

### Next Phase Prerequisites
- Complete tenant isolation verified
- Multiple tenants can operate independently
- No cross-tenant data leakage

---

## Phase 10: Enhanced Security & Monitoring
**Duration**: 2-3 sessions  
**Complexity**: Medium-High

### Objectives
- Implement comprehensive security measures
- Set up monitoring and health checks
- Add audit logging and compliance features

### Deliverables
1. **Security Enhancements**
   ```csharp
   // Input validation middleware
   // XSS protection
   // CORS configuration
   // Security headers
   // Rate limiting per tenant
   ```

2. **Monitoring Setup**
   ```csharp
   // Health check endpoints
   // Performance counters
   // Custom metrics
   // Alerting thresholds
   ```

3. **Audit Logging**
   ```csharp
   public class AuditEntry
   public class AuditService
   // User action tracking
   // Data change logging
   // Compliance reporting
   ```

4. **Configuration Management**
   ```csharp
   // Environment-specific settings
   // Feature flags
   // Tenant-specific configurations
   ```

### Dependencies
- Phase 9 completion
- Monitoring tools selected
- Security requirements defined

### Next Phase Prerequisites
- Security measures tested and verified
- Monitoring dashboards operational
- Audit logging capturing all required events

---

## Phase 11: Testing & Quality Assurance
**Duration**: 2-3 sessions  
**Complexity**: Medium

### Objectives
- Implement comprehensive testing strategy
- Set up automated testing pipeline
- Ensure code quality and coverage

### Deliverables
1. **Unit Tests**
   ```csharp
   // Service layer tests
   // Repository tests
   // Utility function tests
   // 80%+ code coverage
   ```

2. **Integration Tests**
   ```csharp
   // Database integration tests
   // API endpoint tests
   // Multi-tenant isolation tests
   // Authentication flow tests
   ```

3. **End-to-End Tests**
   ```typescript
   // Frontend E2E tests with Playwright
   // Critical user journey tests
   // Cross-browser testing
   ```

4. **Performance Tests**
   ```csharp
   // Load testing scenarios
   // Database performance tests
   // API response time validation
   ```

5. **Quality Gates**
   ```yaml
   # CI/CD pipeline configuration
   # Code quality checks
   # Security scanning
   # Automated deployment gates
   ```

### Dependencies
- Phase 10 completion
- Testing frameworks selected
- CI/CD pipeline tools available

### Next Phase Prerequisites
- All tests passing consistently
- Quality gates met
- Performance benchmarks established

---

## Phase 12: Deployment Preparation
**Duration**: 2-3 sessions  
**Complexity**: Medium-High

### Objectives
- Prepare for multiple deployment scenarios
- Create deployment scripts and documentation
- Set up environment-specific configurations

### Deliverables
1. **Cloud Deployment (SaaS)**
   ```yaml
   # Kubernetes manifests
   # Helm charts
   # Auto-scaling configuration
   # Load balancer setup
   ```

2. **On-Premise Deployment**
   ```bash
   # Installation scripts
   # Configuration templates
   # Backup procedures
   # Upgrade scripts
   ```

3. **Government Cloud Setup**
   ```yaml
   # Enhanced security configurations
   # Compliance settings
   # Audit requirements
   # Access controls
   ```

4. **Documentation**
   ```markdown
   # Installation guides
   # Administrator manual
   # Troubleshooting guide
   # API documentation
   ```

5. **Deployment Automation**
   ```yaml
   # CI/CD pipelines for each environment
   # Infrastructure as Code (Terraform)
   # Automated testing in deployment pipeline
   ```

### Dependencies
- Phase 11 completion
- Deployment target environments identified
- Infrastructure requirements defined

### Next Phase Prerequisites
- Successful deployment to all target environments
- Documentation complete and tested
- Deployment automation working reliably

---

## Implementation Notes

### Session Management
- Each phase is designed to fit within 1-4 development sessions
- Phases build upon each other sequentially
- Clear entry and exit criteria for each phase
- Checkpoint documentation at each phase completion

### Development Approach
- Start with minimal viable implementation in each phase
- Enhance and iterate within phases as needed
- Maintain working state at end of each phase
- Document decisions and trade-offs made

### Resource Planning
- Estimated total development time: 25-35 sessions
- Each session: 2-4 hours of focused development
- Buffer time built in for learning and debugging
- Parallel development possible after Phase 6

### Quality Checkpoints
- Code review after every 2-3 phases
- Integration testing at major milestones
- Performance validation after Phase 10
- Security review after Phase 10
- Final acceptance testing after Phase 12

## Getting Started

To begin implementation, start with Phase 1 and work through each phase sequentially. Each phase document should be created as you begin that phase, detailing the specific implementation steps, code samples, and acceptance criteria.

Remember to commit your work frequently and tag releases at the end of major phases for easy rollback if needed.
