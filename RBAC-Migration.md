🔧 .NET 9 RBAC Implementation Context Document
📊 Project Overview
System Architecture
• .NET 9 Microservices: AuthService, UserService, ApiGateway
• Frontend: React + TypeScript + Material UI + Vite
• Database: PostgreSQL with Entity Framework Core
• Authentication: JWT tokens with multi-role RBAC
• Multi-tenant: Shared database with tenant isolation
Core Problem Identified
The system had conflicting role implementations between legacy string-based roles and modern RBAC permission-based authorization, causing inconsistent user access and 403 errors.
🎯 Key Issues Discovered
1. JWT Token Generation Problem
• Issue: AuthService was using legacy TenantUsers.Role (string) instead of RBAC UserRoles table
• Symptom: User showed as role "User" but had 23 admin permissions
• Impact: Frontend couldn't determine admin status correctly
2. Mixed Authorization Patterns
• Controllers: Some used [Authorize(Roles = "Admin")], others used permission checks
• Services: UserProfileService used role-based admin checks
• Frontend: Expected permission-based but received role-based data
3. Multi-Role Support Gap
• Backend: RBAC system supported multiple roles per user per tenant
• Frontend: Only handled single role strings
• JWT: Only contained single role from legacy system
🔧 Complete Backend Fixes Applied

Fix 1: Enhanced JWT Token Service
// File: src/services/AuthService/Services/EnhancedTokenService.cs
// Key Change: Use UserRoles table instead of TenantUsers for role claims
var userRoles = await _context.UserRoles
    .Where(ur => ur.UserId == user.Id && ur.TenantId == tenant.Id && ur.IsActive)
    .Include(ur => ur.Role)
    .ToListAsync();

foreach (var userRole in userRoles)
{
    claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
}

--------

Fix 2: UserProfileService Permission-Based Authorization
// File: src/services/UserService/Services/UserProfileService.cs
// Key Change: Use permission service instead of role checks
private async Task<bool> IsUserAdminAsync(int userId, CancellationToken cancellationToken = default)
{
    var adminPermissions = new[] { "users.edit", "users.create", "users.delete", "roles.edit", "tenants.edit" };
    return await _permissionService.UserHasAnyPermissionAsync(userId, adminPermissions, cancellationToken);
}

--------

Fix 3-5: Controllers Permission-Based Authorization
// Pattern applied to RolesController, PermissionsController, UsersController
// Before: [Authorize(Roles = "Admin,SuperAdmin")]
// After: Permission claim checks
var hasPermission = User.Claims.Any(c => 
    c.Type == "permission" && c.Value == "required.permission");

if (!hasPermission)
{
    return Forbid("You don't have permission to perform this action");
}

--------

🎨 Complete Frontend Fixes Applied
Fix 1: Enhanced User Type for Multi-Role
// File: src/frontend/react-app/src/types/index.ts
export interface User {
  roles: string | string[];     // Handle both single and multiple roles
  // ... other properties
}

--------

Fix 2: Enhanced AuthContext for Multi-Role JWT
// File: src/frontend/react-app/src/contexts/AuthContext.tsx
const getRolesFromToken = (token: string): string[] => {
  // Handle single role string, array, or comma-separated string
  if (Array.isArray(roles)) {
    return roles.filter(role => role && role.length > 0);
  } else if (typeof roles === 'string' && roles.length > 0) {
    return roles.split(',').map(r => r.trim()).filter(r => r.length > 0);
  }
  return [];
};

--------

Fix 3: Enhanced PermissionContext for RBAC
// File: src/frontend/react-app/src/contexts/PermissionContext.tsx
interface PermissionContextType {
  hasRole: (roleName: string) => boolean;
  hasAnyRole: (roleNames: string[]) => boolean;
  hasAllRoles: (roleNames: string[]) => boolean;  // NEW: Multi-role support
  isAdmin: () => boolean; // Permission-based admin check
}

--------

Fix 4-5: Component Updates for Multi-Role Display
• UserProfile.tsx: Enhanced role display supporting multiple roles
• UserRoleAssignment.tsx: Proper duplicate prevention and multi-role management
🗃️ Database Schema (RBAC)
Key Tables
-- Legacy (still used for fallback)
TenantUsers: UserId, TenantId, Role (string), IsActive

-- Modern RBAC (primary system)
UserRoles: UserId, RoleId, TenantId, IsActive, AssignedAt
Roles: Id, Name, TenantId, IsSystemRole, IsDefault, IsActive
RolePermissions: RoleId, PermissionId, GrantedAt
Permissions: Id, Name, Category, Description, IsActive

User's Current State
-- User ID: 6, Email: mccrearyforward@gmail.com
-- TenantUsers: Role = "User" (legacy)
-- UserRoles: Multiple admin role assignments (RBAC)
-- Effective Result: 23+ admin permissions from RBAC roles

--------

🎯 JWT Token Structure After Fixes
Before Fixes
{
  "role": "User",  // Single string from TenantUsers
  "permission": ["users.view", "users.edit", ...] // 23+ permissions from RBAC
}

After Fixes
{
  "role": ["Admin", "Manager"],  // Multiple roles from UserRoles table
  "permission": ["users.view", "users.edit", "users.create", ...]
}

--------

🚀 Testing Verification Steps
Backend Testing
1.  JWT Analysis: Decode token after login - should show multiple roles
2.  User Profile: Admin users can edit their own profiles
3.  User Management: No 403 errors when viewing/editing other users
4.  Role Management: All CRUD operations work with permission checks
Frontend Testing
1.  Role Display: User profile shows multiple roles as chips
2.  Admin Detection: isAdmin() returns true based on permissions
3.  Navigation: Admin menus visible based on permission checks
4.  Role Assignment: Can assign multiple rol

📋 Key Files Modified
Backend Files
• src/services/AuthService/Services/EnhancedTokenService.cs
• src/services/UserService/Services/UserProfileService.cs
• src/services/UserService/Controllers/RolesController.cs
• src/services/UserService/Controllers/PermissionsController.cs
• src/services/UserService/Controllers/UsersController.cs
Frontend Files
• src/frontend/react-app/src/types/index.ts
• src/frontend/react-app/src/contexts/AuthContext.tsx
• src/frontend/react-app/src/contexts/PermissionContext.tsx
• src/frontend/react-app/src/components/users/UserProfile.tsx
• src/frontend/react-app/src/components/users/UserRoleAssignment.tsx
🏆 Final Architecture State
✅ Achieved Consistency
• JWT Tokens: Multi-role support from RBAC UserRoles table
• Backend Authorization: 100% permission-based (no role-based checks)
• Frontend: Multi-role aware with permission-based admin detection
• Database: RBAC system is primary, TenantUsers as fallback only
✅ Multi-Role RBAC Features
• Users can have multiple roles per tenant (Admin + Manager + CustomRole)
• No duplicate role assignments (database constraints + frontend prevention)
• Effective permissions = union of all role permissions
• Tenant isolation maintained throughout
✅ Modern RBAC Benefits
• Granular Permissions: Each action has specific permission requirements
• Flexible Roles: Tenants can create custom roles with different permission sets
• Scalable: Easy to add new permissions without code changes
• Auditable: Full trail of role assignments and permission grants
🔄 Migration Strategy Applied
1.  Backward Compatibility: TenantUsers table maintained for legacy fallback
2.  Gradual Transition: Controllers updated one by one to use permissions
3.  Dual System: JWT generation checks UserRoles first, falls back to TenantUsers
4.  Frontend Resilience: Handles both single role strings and multiple role arrays
⚠️ Important Notes for Future Development
1.  Always use permission checks in new controllers (not role checks)
2.  JWT tokens now contain multiple roles - frontend must handle arrays
3.  UserRoles table is primary - TenantUsers is legacy fallback only
4.  Admin detection based on permissions, not role names
5.  Multi-role UI must prevent duplicates and handle system roles properly