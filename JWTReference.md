# 🔐 JWT Configuration Reference
**Purpose**: Critical JWT settings for frontend-backend integration  
**Status**: Working Configuration  
**Last Verified**: Current Session

---

## 🎯 Executive Summary

This document contains all critical JWT configuration settings that enable proper authentication between the React frontend and .NET backend services. These settings MUST be synchronized across all services for authentication to work correctly.

---

## 📋 Critical JWT Settings

### Master Configuration Values

| Setting | Value | Used By |
|---------|-------|---------|
| **Secret Key** | `"your-super-secret-jwt-key-that-is-at-least-256-bits-long"` | All services |
| **Issuer** | `"AuthService"` | All services |
| **Audience** | `"StarterApp"` | All services |
| **Expiry Minutes** | `60` | AuthService (token generation) |
| **Refresh Token Days** | `7` | AuthService (refresh tokens) |
| **Algorithm** | `HmacSha256` | All services |
| **Clock Skew** | `5 minutes` (Gateway), `0` (Services) | Validation tolerance |

---

## 🔧 Service Configuration Files

### 1. AuthService Configuration
**File**: `src/services/AuthService/appsettings.json`

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-that-is-at-least-256-bits-long",
    "Issuer": "AuthService",
    "Audience": "StarterApp",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true
  }
}
```

### 2. UserService Configuration
**File**: `src/services/UserService/appsettings.json`

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-that-is-at-least-256-bits-long",
    "Issuer": "AuthService",
    "Audience": "StarterApp",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true
  }
}
```

### 3. API Gateway Configuration
**File**: `src/services/ApiGateway/appsettings.json`

```json
{
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-that-is-at-least-256-bits-long",
    "Issuer": "AuthService",
    "Audience": "StarterApp",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true
  }
}
```

---

## 🎨 Frontend Configuration

### Vite Proxy Configuration
**File**: `src/frontend/react-app/vite.config.ts`

```typescript
export default defineConfig({
  server: {
    port: 3000,
    proxy: {
      '/api': {
        target: 'https://localhost:7000',  // API Gateway HTTPS port
        changeOrigin: true,
        secure: false,  // Allow self-signed certificates
      }
    }
  }
})
```

### API Client Configuration
**File**: `src/frontend/react-app/src/services/api.client.ts`

```typescript
// Base URL should be empty to use Vite proxy
const baseURL = '';

// Token format in Authorization header
config.headers.Authorization = `Bearer ${token}`;
```

### Environment Variables
**File**: `src/frontend/react-app/.env.development`

```env
# Leave empty to use Vite proxy
VITE_API_BASE_URL=
```

---

## 🔑 JWT Token Structure

### Token Claims

| Claim | Type | Example | Purpose |
|-------|------|---------|---------|
| `nameid` (NameIdentifier) | string | `"1"` | User ID |
| `email` | string | `"user@example.com"` | User email |
| `given_name` | string | `"John"` | First name |
| `family_name` | string | `"Doe"` | Last name |
| `role` | string/array | `"Admin"` | User roles |
| `tenant_id` | string | `"1"` | Tenant ID |
| `tenant_name` | string | `"Default"` | Tenant name |
| `permission` | array | `["users.view", "users.edit"]` | User permissions |
| `iss` | string | `"AuthService"` | Token issuer |
| `aud` | string | `"StarterApp"` | Token audience |
| `exp` | number | `1704067200` | Expiration (Unix timestamp) |
| `iat` | number | `1704063600` | Issued at (Unix timestamp) |
| `jti` | string | `"unique-id"` | JWT ID |

### Example Decoded Token

```json
{
  "nameid": "1",
  "email": "admin@tenant1.com",
  "given_name": "Admin",
  "family_name": "User",
  "role": ["Admin", "SuperAdmin"],
  "tenant_id": "1",
  "tenant_name": "Default Tenant",
  "permission": [
    "users.view",
    "users.create",
    "users.edit",
    "users.delete",
    "roles.view",
    "roles.create"
  ],
  "iss": "AuthService",
  "aud": "StarterApp",
  "exp": 1704067200,
  "iat": 1704063600,
  "jti": "550e8400-e29b-41d4-a716-446655440000"
}
```

---

## 🔄 Token Flow

### 1. Login Flow
```
Frontend → POST /api/auth/login → API Gateway → AuthService
                                                     ↓
Frontend ← { accessToken, refreshToken } ← ← ← ← ← ←
```

### 2. API Request Flow
```
Frontend → GET /api/users (Bearer token) → API Gateway (validates) → UserService
                                                ↓
Frontend ← Response ← ← ← ← ← ← ← ← ← ← ← ← ← ←
```

### 3. Token Refresh Flow
```
Frontend → POST /api/auth/refresh → API Gateway → AuthService
                                                      ↓
Frontend ← { accessToken, refreshToken } ← ← ← ← ← ← 
```

---

## ⚠️ Common Issues and Fixes

### Issue 1: 401 Unauthorized
**Symptoms**: All API calls return 401  
**Common Causes**:
- Mismatched secret keys between services
- Wrong issuer/audience values
- Expired tokens
- Missing Bearer prefix in Authorization header

**Fix**: Ensure all services use identical JWT settings

### Issue 2: Token Validation Fails
**Symptoms**: "SecurityTokenSignatureKeyNotFoundException"  
**Common Causes**:
- Secret key too short (must be ≥256 bits/32 characters)
- Different algorithms used for signing vs validation
- Clock skew issues between servers

**Fix**: 
- Use the exact secret key shown above
- Set ClockSkew to 5 minutes in API Gateway
- Ensure HmacSha256 algorithm everywhere

### Issue 3: Permissions Not Working
**Symptoms**: User has role but can't access resources  
**Common Causes**:
- Permissions not included in token claims
- Frontend not extracting permissions correctly
- Role-permission mappings not loaded

**Fix**:
- Check TokenService includes permission claims
- Verify PermissionContext extracts from token
- Ensure RolePermissions are seeded in database

### Issue 4: CORS Errors
**Symptoms**: "Access-Control-Allow-Origin" errors  
**Common Causes**:
- Frontend making direct calls instead of using proxy
- API Gateway CORS not configured
- Wrong target URL in Vite proxy

**Fix**:
- Use relative URLs in frontend (`/api/...`)
- Enable CORS in API Gateway for development
- Verify Vite proxy target is `https://localhost:7000`

---

## 🧪 Testing Configuration

### Integration Tests
**File**: `tests/integration/UserService.IntegrationTests/TestUtilities/AuthenticationHelper.cs`

```csharp
// Must match production settings exactly
private const string SecretKey = "your-super-secret-jwt-key-that-is-at-least-256-bits-long";
private const string Issuer = "AuthService";
private const string Audience = "StarterApp";
```

### Unit Tests
Use same configuration values in all test projects to ensure consistency.

---

## 📝 Configuration Checklist

Before running the application, verify:

- [ ] All services have identical JWT settings in appsettings.json
- [ ] Secret key is exactly: `"your-super-secret-jwt-key-that-is-at-least-256-bits-long"`
- [ ] Issuer is exactly: `"AuthService"`
- [ ] Audience is exactly: `"StarterApp"`
- [ ] API Gateway ClockSkew is set to 5 minutes
- [ ] Frontend uses empty baseURL for API client
- [ ] Vite proxy targets `https://localhost:7000`
- [ ] Authorization header format is `Bearer {token}`
- [ ] Test configurations match production settings

---

## 🚀 Quick Start Commands

### Start Backend Services
```bash
# Terminal 1 - API Gateway
cd src/services/ApiGateway
dotnet run

# Terminal 2 - AuthService  
cd src/services/AuthService
dotnet run

# Terminal 3 - UserService
cd src/services/UserService
dotnet run
```

### Start Frontend
```bash
# Terminal 4 - React App
cd src/frontend/react-app
npm run dev
```

### Verify Configuration
1. Open browser to http://localhost:3000
2. Try to login with test credentials
3. Check browser console for JWT token details
4. Verify API calls include Bearer token

---

## 🔒 Security Notes

### Production Changes Required
1. **Replace secret key** with a cryptographically secure key
2. **Use environment variables** for all sensitive settings
3. **Enable HTTPS** for all services in production
4. **Implement key rotation** for JWT secrets
5. **Use shorter token expiry** for sensitive applications
6. **Store refresh tokens securely** (httpOnly cookies recommended)

### Never Commit to Git
- Real secret keys
- Production connection strings
- API keys or credentials
- Certificate files

---

**Document Generated**: Current Session  
**Purpose**: JWT configuration reference for development  
**Critical**: All services MUST use identical JWT settings
