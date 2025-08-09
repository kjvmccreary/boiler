# 📁 Complete Project Structure Reference
**Root Directory**: `C:\Users\mccre\dev\boiler`  
**Solution Name**: boiler.sln  
**Last Updated**: Current Session

---

## 🏗️ Solution Architecture Overview

```
C:\Users\mccre\dev\boiler\
├── boiler.sln                           # Main solution file
├── README.md                            # Project documentation
├── .gitignore                           # Git ignore configuration
├── .editorconfig                        # Code style configuration
│
├── 📁 docs/                            # Documentation files
│   ├── MicroservicesStarterApp.md      # Original project specification
│   ├── BoilerplatePhasedApproach.md    # Original phased implementation plan
│   ├── RevisedProjectPhases.md         # Enhanced phases with RBAC
│   ├── PhaseFiveDetail.md              # Phase 5 detailed documentation
│   ├── PhaseSix.md                     # Phase 6 completion status
│   └── PhaseSeven.md                   # Phase 7 React Frontend requirements
│
├── 📁 src/                             # Source code root
│   ├── 📁 shared/                      # Shared libraries
│   │   ├── 📁 DTOs/                    # Data Transfer Objects
│   │   │   ├── DTOs.csproj
│   │   │   ├── 📁 Auth/
│   │   │   │   ├── LoginRequestDto.cs
│   │   │   │   ├── RegisterRequestDto.cs
│   │   │   │   ├── TokenResponseDto.cs
│   │   │   │   ├── RefreshTokenDto.cs
│   │   │   │   └── ChangePasswordDto.cs
│   │   │   ├── 📁 User/
│   │   │   │   ├── UserDto.cs
│   │   │   │   ├── CreateUserDto.cs
│   │   │   │   └── UpdateUserDto.cs
│   │   │   ├── 📁 Role/
│   │   │   │   ├── RoleDto.cs
│   │   │   │   ├── CreateRoleDto.cs
│   │   │   │   └── AssignRoleDto.cs
│   │   │   ├── 📁 Common/
│   │   │   │   ├── ApiResponseDto.cs
│   │   │   │   ├── PagedResultDto.cs
│   │   │   │   └── ErrorDto.cs
│   │   │   ├── 📁 Entities/
│   │   │   │   ├── User.cs
│   │   │   │   ├── Role.cs
│   │   │   │   ├── Permission.cs
│   │   │   │   ├── Tenant.cs
│   │   │   │   └── RefreshToken.cs
│   │   │   └── 📁 Validators/
│   │   │       └── (FluentValidation validators)
│   │   │
│   │   ├── 📁 Common/                  # Common utilities
│   │   │   ├── Common.csproj
│   │   │   ├── 📁 Configuration/
│   │   │   │   ├── JwtSettings.cs
│   │   │   │   └── TenantSettings.cs
│   │   │   ├── 📁 Data/
│   │   │   │   ├── ApplicationDbContext.cs
│   │   │   │   └── DbInitializer.cs
│   │   │   ├── 📁 Extensions/
│   │   │   │   ├── ServiceCollectionExtensions.cs
│   │   │   │   └── ConfigurationExtensions.cs
│   │   │   ├── 📁 Middleware/
│   │   │   │   ├── ErrorHandlingMiddleware.cs
│   │   │   │   └── TenantResolutionMiddleware.cs
│   │   │   ├── 📁 Repositories/
│   │   │   │   ├── BaseRepository.cs
│   │   │   │   ├── UserRepository.cs
│   │   │   │   └── TenantRepository.cs
│   │   │   └── 📁 Services/
│   │   │       ├── PasswordService.cs
│   │   │       └── TenantProvider.cs
│   │   │
│   │   └── 📁 Contracts/                # Service interfaces
│   │       ├── Contracts.csproj
│   │       ├── 📁 Auth/
│   │       │   └── IAuthService.cs
│   │       ├── 📁 User/
│   │       │   ├── IUserService.cs
│   │       │   └── IRoleService.cs
│   │       ├── 📁 Services/
│   │       │   ├── ITokenService.cs
│   │       │   └── IPasswordService.cs
│   │       └── 📁 Repositories/
│   │           └── IRepository.cs
│   │
│   ├── 📁 services/                    # Microservices
│   │   ├── 📁 AuthService/
│   │   │   ├── AuthService.csproj
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── 📁 Controllers/
│   │   │   │   └── AuthController.cs
│   │   │   ├── 📁 Services/
│   │   │   │   ├── AuthService.cs
│   │   │   │   └── TokenService.cs
│   │   │   ├── 📁 Mappings/
│   │   │   │   └── MappingProfile.cs
│   │   │   └── 📁 logs/
│   │   │       └── authservice-*.txt
│   │   │
│   │   ├── 📁 UserService/
│   │   │   ├── UserService.csproj
│   │   │   ├── Program.cs
│   │   │   ├── appsettings.json
│   │   │   ├── appsettings.Development.json
│   │   │   ├── 📁 Controllers/
│   │   │   │   ├── UsersController.cs
│   │   │   │   └── RolesController.cs
│   │   │   ├── 📁 Services/
│   │   │   │   ├── UserService.cs
│   │   │   │   ├── UserProfileService.cs
│   │   │   │   ├── RoleService.cs
│   │   │   │   └── PermissionService.cs
│   │   │   ├── 📁 Mappings/
│   │   │   │   └── UserMappingProfile.cs
│   │   │   ├── 📁 Middleware/
│   │   │   │   └── TenantContextMiddleware.cs
│   │   │   └── 📁 logs/
│   │   │       └── userservice-*.txt
│   │   │
│   │   └── 📁 ApiGateway/
│   │       ├── ApiGateway.csproj
│   │       ├── Program.cs
│   │       ├── ocelot.json
│   │       ├── appsettings.json
│   │       ├── appsettings.Development.json
│   │       ├── 📁 Controllers/
│   │       │   ├── HealthController.cs
│   │       │   └── ServiceDiscoveryController.cs
│   │       ├── 📁 Middleware/
│   │       │   ├── RequestLoggingMiddleware.cs
│   │       │   ├── TenantResolutionMiddleware.cs
│   │       │   ├── RateLimitingMiddleware.cs
│   │       │   └── AuthorizationContextMiddleware.cs
│   │       ├── 📁 Services/
│   │       │   └── ServiceDiscovery.cs
│   │       └── 📁 logs/
│   │           └── gateway-*.txt
│   │
│   └── 📁 frontend/                    # Frontend application
│       └── 📁 react-app/
│           ├── package.json
│           ├── package-lock.json
│           ├── vite.config.ts
│           ├── tsconfig.json
│           ├── tsconfig.node.json
│           ├── tailwind.config.js
│           ├── eslint.config.js
│           ├── README.md
│           ├── index.html
│           ├── .env.development
│           ├── 📁 src/
│           │   ├── main.tsx
│           │   ├── App.tsx
│           │   ├── index.css
│           │   ├── vite-env.d.ts
│           │   ├── 📁 components/
│           │   │   ├── 📁 auth/
│           │   │   │   ├── LoginForm.tsx
│           │   │   │   └── RegisterForm.tsx
│           │   │   ├── 📁 authorization/
│           │   │   │   ├── CanAccess.tsx
│           │   │   │   └── ProtectedRoute.tsx
│           │   │   ├── 📁 layout/
│           │   │   │   ├── AppLayout.tsx
│           │   │   │   ├── Sidebar.tsx
│           │   │   │   └── UserMenu.tsx
│           │   │   ├── 📁 roles/
│           │   │   │   ├── RoleList.tsx
│           │   │   │   └── RoleEditor.tsx
│           │   │   └── 📁 users/
│           │   │       ├── UserList.tsx
│           │   │       ├── UserProfile.tsx
│           │   │       └── UserRoleAssignment.tsx
│           │   ├── 📁 contexts/
│           │   │   ├── AuthContext.tsx
│           │   │   └── PermissionContext.tsx
│           │   ├── 📁 hooks/
│           │   │   └── useAuth.ts
│           │   ├── 📁 pages/
│           │   │   └── Dashboard.tsx
│           │   ├── 📁 routes/
│           │   │   ├── index.tsx
│           │   │   └── route.constants.ts
│           │   ├── 📁 services/
│           │   │   ├── api.client.ts
│           │   │   ├── auth.service.ts
│           │   │   ├── user.service.ts
│           │   │   └── role.service.ts
│           │   ├── 📁 types/
│           │   │   └── index.ts
│           │   └── 📁 utils/
│           │       ├── api.constants.ts
│           │       └── token.manager.ts
│           ├── 📁 public/
│           └── 📁 dist/                # Build output
│
└── 📁 tests/                           # Test projects
    ├── 📁 unit/
    │   ├── 📁 AuthService.Tests/
    │   │   ├── AuthService.Tests.csproj
    │   │   ├── TestBase.cs
    │   │   ├── 📁 Controllers/
    │   │   │   └── AuthControllerTests.cs
    │   │   ├── 📁 Services/
    │   │   │   ├── AuthServiceImplementationTests.cs
    │   │   │   └── TokenServiceTests.cs
    │   │   └── 📁 Validators/
    │   │       └── ValidatorTests.cs
    │   │
    │   ├── 📁 ApiGateway.Tests/
    │   │   ├── ApiGateway.Tests.csproj
    │   │   ├── 📁 Configuration/
    │   │   │   └── GatewayConfigurationTests.cs
    │   │   ├── 📁 Controllers/
    │   │   │   └── ServiceDiscoveryControllerTests.cs
    │   │   ├── 📁 Middleware/
    │   │   │   ├── TenantResolutionMiddlewareTests.cs
    │   │   │   ├── AuthorizationContextMiddlewareTests.cs
    │   │   │   └── RateLimitingMiddlewareTests.cs
    │   │   └── 📁 Services/
    │   │       └── ServiceDiscoveryTests.cs
    │   │
    │   ├── 📁 RoleService.Tests/
    │   │   └── RoleService.Tests.csproj
    │   │
    │   └── 📁 PermissionService.Tests/
    │       └── PermissionService.Tests.csproj
    │
    └── 📁 integration/
        └── 📁 UserService.IntegrationTests/
            ├── UserService.IntegrationTests.csproj
            ├── 📁 Fixtures/
            │   └── WebApplicationTestFixture.cs
            ├── 📁 TestUtilities/
            │   ├── AuthenticationHelper.cs
            │   └── TestDataSeeder.cs
            └── 📁 Controllers/
                └── UsersControllerTests.cs
```

---

## 📊 Project Statistics

### Solution Components
- **Total Projects**: 12
  - Backend Services: 3 (AuthService, UserService, ApiGateway)
  - Shared Libraries: 3 (DTOs, Common, Contracts)
  - Frontend: 1 (React App)
  - Unit Test Projects: 4
  - Integration Test Projects: 1

### Technology Stack
- **Backend**: .NET 8.0, ASP.NET Core
- **Database**: PostgreSQL with Entity Framework Core
- **Frontend**: React 19, TypeScript, Vite, Material UI, Tailwind CSS
- **Authentication**: JWT Bearer tokens
- **API Gateway**: Ocelot
- **Testing**: xUnit, Moq, FluentAssertions, Vitest

### Database Schema
- **Tables**: 8 main tables
  - Users, Roles, Permissions, UserRoles, RolePermissions
  - Tenants, RefreshTokens, TenantUsers

### API Endpoints
- **AuthService**: 9 endpoints
- **UserService**: 16 endpoints (5 user, 11 role)
- **ApiGateway**: Health checks, service discovery

---

## 🔑 Key Configuration Files

### Backend Services
1. **appsettings.json** - Production settings
2. **appsettings.Development.json** - Development overrides
3. **ocelot.json** - API Gateway routing configuration

### Frontend
1. **package.json** - NPM dependencies and scripts
2. **vite.config.ts** - Build and dev server configuration
3. **tsconfig.json** - TypeScript compiler settings
4. **.env.development** - Environment variables

### Solution Level
1. **boiler.sln** - Visual Studio solution file
2. **.gitignore** - Git ignore patterns
3. **.editorconfig** - Code style settings

---

## 🚀 Service Ports

| Service | Development Port | HTTPS Port |
|---------|-----------------|------------|
| API Gateway | 5000 | 7000 |
| AuthService | 5001 | 7001 |
| UserService | 5002 | 7002 |
| Frontend | 3000 | - |

---

## 📝 Notes

### Project Conventions
- **Naming**: PascalCase for C# files, kebab-case for React files
- **Structure**: Feature-based organization in services
- **Testing**: Unit tests mirror source structure
- **Logs**: Each service has its own log directory

### Development Workflow
1. Backend services run on HTTPS with self-signed certificates
2. Frontend uses Vite proxy to forward `/api` requests to gateway
3. JWT tokens handle authentication across all services
4. Tenant isolation through middleware and EF Core filters

### Current Phase Status
- **Completed**: Phases 1-6 (Backend complete with RBAC)
- **In Progress**: Phase 7 (React Frontend ~60% complete)
- **Remaining**: Phases 8-12 (Docker, Multi-tenant, etc.)

---

**Generated**: Current Session  
**Purpose**: Visual Studio development reference  
**Usage**: Navigate project structure and locate files quickly
