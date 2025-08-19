BROWSER CONSOLE
TenantContext.tsx:204 🏢 TenantContext: Switching to tenant via JWT refresh: My Number Two
TenantContext.tsx:205 🔍 TenantContext: Current auth token exists: true
TenantContext.tsx:206 🔍 TenantContext: Current refresh token exists: true
tenant.service.ts:92 🏢 TenantService: Switching to tenant: 6
tenant.service.ts:93 🔍 TenantService: Request payload: {tenantId: 6}
tenant.service.ts:98 🔍 TenantService: BEFORE request - Auth state: {hasAccessToken: true, hasRefreshToken: true, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...'}
tenant.service.ts:108 🔍 TenantService: BEFORE token tenant info: {tenant_id: '1', tenant_name: 'Default Tenant'}
tenant.service.ts:117 🔍 TenantService: Making API call to switch-tenant...
api.client.ts:41 🔍 API REQUEST (with JWT): POST /api/auth/switch-tenant
api.client.ts:54 ✅ API RESPONSE: /api/auth/switch-tenant 200
api.client.ts:140 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: 'Successfully switched to My Number Two', hasData: true, errors: 0}
api.client.ts:58 🔍 API CLIENT: Unwrapped response for /api/auth/switch-tenant : {accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc…HAifQ.pEszOnmPWhXOxw2OwEsQS6R86JAmSS-NJHyghZtdt0s', refreshToken: 'cLQZgtSWiHNWe5sFUvZIqdKZGVq1698C8crZfxW5YxC34QJ+elIqAva8Fq9K/epJT4URKIcWzotKk4KngAVKpA==', expiresAt: '2025-08-26T00:42:08.0565128Z', tokenType: 'Bearer', user: {…}, …}
tenant.service.ts:122 🔍 TenantService: API response received: {status: 200, hasData: true, dataKeys: Array(6), dataType: 'object'}
tenant.service.ts:129 🔍 TenantService: Full response data: {accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc…HAifQ.pEszOnmPWhXOxw2OwEsQS6R86JAmSS-NJHyghZtdt0s', refreshToken: 'cLQZgtSWiHNWe5sFUvZIqdKZGVq1698C8crZfxW5YxC34QJ+elIqAva8Fq9K/epJT4URKIcWzotKk4KngAVKpA==', expiresAt: '2025-08-26T00:42:08.0565128Z', tokenType: 'Bearer', user: {…}, …}
tenant.service.ts:134 🔍 TenantService: Token data analysis: {tokenData: {…}, hasAccessToken: true, hasRefreshToken: true, hasUser: true, hasTenant: true, …}
tenant.service.ts:145 ✅ TenantService: Valid token data found, storing tokens...
tenant.service.ts:159 🔧 TenantService: Token storage verification: {tokenChanged: true, refreshTokenChanged: true, newTokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', newRefreshTokenPreview: 'cLQZgtSWiHNWe5sFUvZI...'}
tenant.service.ts:170 🔍 TenantService: NEW token tenant verification: {tenant_id: '6', tenant_name: 'My Number Two', user_id: '6', expected_tenant_id: '6', tenant_id_matches: true}
tenant.service.ts:185 ✅ TenantService: New token has correct tenant_id!
TenantContext.tsx:212 ✅ TenantContext: Tenant switch API successful
TenantContext.tsx:215 🔍 TenantContext: Calling refreshAuth to reload user state...
AuthContext.tsx:151 🔍 AuthContext: Initializing authentication...
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
token.manager.ts:45 🔍 TokenManager: Refresh token info: {hasRefreshToken: true, refreshTokenLength: 88}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
AuthContext.tsx:157 🔍 AuthContext: Token check: {hasToken: true, hasRefreshToken: true, tokenExpired: false}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
AuthContext.tsx:219 🔍 AuthContext: Token valid, validating with backend...
auth.service.ts:84 🔍 AuthService: Validating token using /api/users/profile...
api.client.ts:41 🔍 API REQUEST (with JWT): GET /api/users/profile
api.client.ts:54 ✅ API RESPONSE: /api/users/profile 200
api.client.ts:140 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: '', hasData: true, errors: 0}
api.client.ts:58 🔍 API CLIENT: Unwrapped response for /api/users/profile : {id: 6, tenantId: 6, email: 'mccrearyforward@gmail.com', firstName: 'Joe Mad Dog', lastName: 'MacDaddy', …}
auth.service.ts:86 ✅ AuthService: Token validation successful: {id: 6, tenantId: 6, email: 'mccrearyforward@gmail.com', firstName: 'Joe Mad Dog', lastName: 'MacDaddy', …}
AuthContext.tsx:97 🔍 AuthContext: Extracting permissions from token: {tokenClaims: Array(15), permissions: Array(0)}
AuthContext.tsx:129 🔍 AuthContext: Extracting roles from JWT token: {tokenClaims: Array(15), rolesRaw: 'TenantAdmin', rolesType: 'string'}
AuthContext.tsx:234 ✅ AuthContext: Authentication initialization successful {user: 'mccrearyforward@gmail.com', permissions: Array(0), roles: Array(1)}
TenantContext.tsx:217 ✅ TenantContext: refreshAuth completed successfully
 🏢 TenantContext: Selecting tenant: My Number Two
 🔍 API REQUEST (with JWT): GET /api/tenants/6/settings
 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(15), permissions: Array(0)}
 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: false, allPermissions: Array(0)}
 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(15), permissions: Array(0)}
 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: false, allPermissions: Array(0)}
 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(15), permissions: Array(0)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: false, allPermissions: Array(0)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(15), permissions: Array(0)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: false, allPermissions: Array(0)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:98 🔍 PermissionContext: Extracting roles from JWT token: {tokenClaims: Array(15), rolesRaw: 'TenantAdmin', rolesType: 'string'}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Mon Aug 18 2025 20:42:08 GMT-0500 (Central Daylight Time), currentTime: Mon Aug 18 2025 19:42:08 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 927, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(15), permissions: Array(0)}
TenantContext.tsx:79 🏢 TenantContext: Loading tenants for user: 6
tenant.service.ts:20 🔍 TenantService: Calling API endpoint: /api/users/6/tenants
api.client.ts:41 🔍 API REQUEST (with JWT): GET /api/users/6/tenants
api.client.ts:171  GET https://localhost:3000/api/tenants/6/settings 404 (Not Found)
(anonymous) @ xhr.js:195
xhr @ xhr.js:15
We @ dispatchRequest.js:49
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:171
getTenantSettings @ tenant.service.ts:57
I @ TenantContext.tsx:278
zl @ TenantContext.tsx:220
await in zl
U @ TenantSwitcher.tsx:59
onClick @ TenantSwitcher.tsx:167
kr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
as @ react-dom-client.production.js:1470
ef @ react-dom-client.production.js:11996
yf @ react-dom-client.production.js:14699
c0 @ react-dom-client.production.js:14667
api.client.ts:62 🚨 API ERROR: /api/tenants/6/settings 404 Request failed with status code 404
(anonymous) @ api.client.ts:62
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:171
getTenantSettings @ tenant.service.ts:57
I @ TenantContext.tsx:278
zl @ TenantContext.tsx:220
await in zl
U @ TenantSwitcher.tsx:59
onClick @ TenantSwitcher.tsx:167
kr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
as @ react-dom-client.production.js:1470
ef @ react-dom-client.production.js:11996
yf @ react-dom-client.production.js:14699
c0 @ react-dom-client.production.js:14667
tenant.service.ts:60 🏢 TenantService: Settings API not available, using defaults
getTenantSettings @ tenant.service.ts:60
await in getTenantSettings
I @ TenantContext.tsx:278
zl @ TenantContext.tsx:220
await in zl
U @ TenantSwitcher.tsx:59
onClick @ TenantSwitcher.tsx:167
kr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
as @ react-dom-client.production.js:1470
ef @ react-dom-client.production.js:11996
yf @ react-dom-client.production.js:14699
c0 @ react-dom-client.production.js:14667
TenantContext.tsx:296 🏢 TenantContext: Tenant selection complete: My Number Two
TenantContext.tsx:222 🏢 TenantContext: Tenant switched successfully with new JWT: My Number Two
:3000/app/dashboard:1 Blocked aria-hidden on an element because its descendant retained focus. The focus must not be hidden from assistive technology users. Avoid using aria-hidden on a focused element or its ancestor. Consider using the inert attribute instead, which will also prevent focus. For more details, see the aria-hidden section of the WAI-ARIA specification at https://w3c.github.io/aria/#aria-hidden.
Element with focus: <button.MuiButtonBase-root MuiIconButton-root MuiIconButton-colorInherit MuiIconButton-sizeLarge css-1vpch0n>
Ancestor with aria-hidden: <div#root> <div id=​"root" aria-hidden=​"true">​…​</div>​
api.client.ts:54 ✅ API RESPONSE: /api/users/6/tenants 200
api.client.ts:140 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: 'User tenants retrieved successfully', hasData: true, errors: 0}
api.client.ts:58 🔍 API CLIENT: Unwrapped response for /api/users/6/tenants : (2) [{…}, {…}]
tenant.service.ts:23 🔍 TenantService: Raw API response: (2) [{…}, {…}]
tenant.service.ts:29 🔍 TenantService: Converted tenants: (2) [{…}, {…}]
TenantContext.tsx:82 🏢 TenantContext: API Response: {success: true, message: 'User tenants loaded', data: Array(2)}
TenantContext.tsx:85 🏢 TenantContext: Setting available tenants: (2) [{…}, {…}]
TenantContext.tsx:259 🔍 TenantContext: Extracted tenant from JWT: {tenantId: '6', tenantName: 'My Number Two'}
TenantContext.tsx:119 🏢 TenantContext: Tenant selection debug: {jwtTenantId: '6', lastSelectedTenantId: '6', availableTenants: Array(2)}
TenantContext.tsx:131 🏢 TenantContext: Found tenants: {jwtTenant: {…}, lastSelected: {…}}
TenantContext.tsx:153 🏢 TenantContext: Using JWT tenant: My Number Two
TenantContext.tsx:272 🏢 TenantContext: Selecting tenant: My Number Two
api.client.ts:41 🔍 API REQUEST (with JWT): GET /api/tenants/6/settings
api.client.ts:171  GET https://localhost:3000/api/tenants/6/settings 404 (Not Found)
(anonymous) @ xhr.js:195
xhr @ xhr.js:15
We @ dispatchRequest.js:49
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:171
getTenantSettings @ tenant.service.ts:57
I @ TenantContext.tsx:278
ft @ TenantContext.tsx:154
Ul @ TenantContext.tsx:87
await in Ul
(anonymous) @ TenantContext.tsx:54
mn @ react-dom-client.production.js:8292
br @ react-dom-client.production.js:9771
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9782
wr @ react-dom-client.production.js:11313
(anonymous) @ react-dom-client.production.js:11048
gl @ scheduler.production.js:151
api.client.ts:62 🚨 API ERROR: /api/tenants/6/settings 404 Request failed with status code 404
(anonymous) @ api.client.ts:62
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:171
getTenantSettings @ tenant.service.ts:57
I @ TenantContext.tsx:278
ft @ TenantContext.tsx:154
Ul @ TenantContext.tsx:87
await in Ul
(anonymous) @ TenantContext.tsx:54
mn @ react-dom-client.production.js:8292
br @ react-dom-client.production.js:9771
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9782
wr @ react-dom-client.production.js:11313
(anonymous) @ react-dom-client.production.js:11048
gl @ scheduler.production.js:151
tenant.service.ts:60 🏢 TenantService: Settings API not available, using defaults
getTenantSettings @ tenant.service.ts:60
await in getTenantSettings
I @ TenantContext.tsx:278
ft @ TenantContext.tsx:154
Ul @ TenantContext.tsx:87
await in Ul
(anonymous) @ TenantContext.tsx:54
mn @ react-dom-client.production.js:8292
br @ react-dom-client.production.js:9771
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
br @ react-dom-client.production.js:9782
wr @ react-dom-client.production.js:11313
(anonymous) @ react-dom-client.production.js:11048
gl @ scheduler.production.js:151
TenantContext.tsx:296 🏢 TenantContext: Tenant selection complete: My Number Two



AUTH CONSOLE
2025-08-18 19:42:07.978 | [00:42:07 WRN] 🔍 TENANT SWITCH DEBUG: User 6 requesting to switch to tenant 6 {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:07.988 | [00:42:07 WRN] Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'. {"EventId": {"Id": 20504, "Name": "Microsoft.EntityFrameworkCore.Query.MultipleCollectionIncludeWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Query", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.020 | [00:42:08 WRN] 🔍 TENANT SWITCH DEBUG: User loaded. TenantUsers count: 2 {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.020 | [00:42:08 WRN] 🔍 TENANT SWITCH DEBUG: User has access to tenant 6: True {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.020 | [00:42:08 WRN] 🔍 TENANT SWITCH DEBUG: Loading tenant 6 from repository {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.029 | [00:42:08 WRN] 🔍 TENANT SWITCH DEBUG: Tenant loaded - ID: 6, Name: My Number Two, IsActive: True {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.052 | [00:42:08 WRN] 🔍 TENANT SWITCH DEBUG: About to generate token with - User ID: 6, Tenant ID: 6, Tenant Name: My Number Two {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.055 | [00:42:08 INF] 🔍 JWT: Found 0 active roles for user 6 in tenant 6 {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.055 | [00:42:08 WRN] 🔍 JWT: No UserRoles found for user 6, falling back to TenantUsers {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.055 | [00:42:08 INF] 🔍 JWT: Added legacy role claim 'TenantAdmin' from TenantUsers for user 6 {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.055 | [00:42:08 INF] 🔍 PERMISSION DEBUG: Getting permissions for user 6 in tenant 6 {"SourceContext": "Common.Services.PermissionService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.056 | [00:42:08 INF] 🔍 PERMISSION DEBUG: Found 0 active user roles for user 6 in tenant 6:  {"SourceContext": "Common.Services.PermissionService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.056 | [00:42:08 WRN] 🔍 PERMISSION DEBUG: No active user roles found for user 6 in tenant 6 {"SourceContext": "Common.Services.PermissionService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.056 | [00:42:08 INF] 🔍 JWT: Added 0 permissions to JWT for user 6 in tenant 6 {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.056 | [00:42:08 INF] 🔍 JWT: Generated token for user 6 with 1 roles and 0 permissions {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.065 | [00:42:08 WRN] 🔍 TENANT SWITCH DEBUG: Response DTOs created - UserDto.TenantId: 6, TenantDto.Id: 6, TenantDto.Name: My Number Two {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.065 | [00:42:08 INF] User 6 successfully switched to tenant 6 (My Number Two) {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "9cc2fbc8-3c6d-46b5-8037-8096bb693b19", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEUN3LKOTO7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEUN3LKOTO7"}
2025-08-18 19:42:08.066 | [00:42:08 INF] HTTP POST /api/auth/switch-tenant responded 200 in 126.5542 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}


USER CONSOLE
2025-08-18 19:39:51.202 | [00:39:51 INF] Starting UserService {}
2025-08-18 19:39:51.205 | === UserService Starting ===
2025-08-18 19:39:51.545 | [00:39:51 WRN] Entity 'Role' has a global query filter defined and is the required end of a relationship with the entity 'RolePermission'. This may lead to unexpected results when the required entity is filtered out. Either configure the navigation as optional, or define matching query filters for both entities in the navigation. See https://go.microsoft.com/fwlink/?linkid=2131316 for more information. {"EventId": {"Id": 10622, "Name": "Microsoft.EntityFrameworkCore.Model.Validation.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
2025-08-18 19:39:51.548 | [00:39:51 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
2025-08-18 19:39:51.829 | [00:39:51 WRN] Using an in-memory repository. Keys will not be persisted to storage. {"EventId": {"Id": 50, "Name": "UsingInMemoryRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository"}
2025-08-18 19:39:51.829 | [00:39:51 WRN] Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits. {"EventId": {"Id": 59, "Name": "UsingEphemeralKeyRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
2025-08-18 19:39:51.837 | [00:39:51 WRN] No XML encryptor configured. Key {c097d588-d006-4f3b-a616-45ea3dbc0565} may be persisted to storage in unencrypted form. {"EventId": {"Id": 35, "Name": "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
2025-08-18 19:39:51.842 | [00:39:51 WRN] Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://0.0.0.0:5002;https://0.0.0.0:7002'. {"EventId": {"Id": 15}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics"}
2025-08-18 19:39:51.952 | Now listening on: http://0.0.0.0:5002
2025-08-18 19:39:51.953 | [00:39:51 INF] Now listening on: http://0.0.0.0:5002 {}
2025-08-18 19:39:51.953 | Now listening on: https://0.0.0.0:7002
2025-08-18 19:39:51.953 | [00:39:51 INF] Now listening on: https://0.0.0.0:7002 {}
2025-08-18 19:39:56.020 | [00:39:56 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FC:00000001", "RequestPath": "/health", "ConnectionId": "0HNEUN3LKQ0FC"}
2025-08-18 19:39:56.042 | [00:39:56 INF] HTTP GET /health responded 200 in 28.9504 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:40:26.204 | [00:40:26 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FD:00000001", "RequestPath": "/health", "ConnectionId": "0HNEUN3LKQ0FD"}
2025-08-18 19:40:26.206 | [00:40:26 INF] HTTP GET /health responded 200 in 5.0298 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:40:56.382 | [00:40:56 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FE:00000001", "RequestPath": "/health", "ConnectionId": "0HNEUN3LKQ0FE"}
2025-08-18 19:40:56.382 | [00:40:56 INF] HTTP GET /health responded 200 in 3.0226 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:41:26.563 | [00:41:26 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FF:00000001", "RequestPath": "/health", "ConnectionId": "0HNEUN3LKQ0FF"}
2025-08-18 19:41:26.563 | [00:41:26 INF] HTTP GET /health responded 200 in 0.7748 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:41:53.823 | [00:41:53 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxIiwidGVuYW50X25hbWUiOiJEZWZhdWx0IFRlbmFudCIsInRlbmFudF9kb21haW4iOiJsb2NhbGhvc3QiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiMjE2ZTQwZjYtMzZhZi00ZDU3LWE4NTEtMWUzYzI3MmIwYmJmIiwiaWF0IjoxNzU1NTY0MTEzLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsInBlcm1pc3Npb24iOlsiYmlsbGluZy5tYW5hZ2UiLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJwZXJtaXNzaW9ucy5kZWxldGUiLCJwZXJtaXNzaW9ucy5lZGl0IiwicGVybWlzc2lvbnMubWFuYWdlIiwicGVybWlzc2lvbnMudmlldyIsInJlcG9ydHMuY3JlYXRlIiwicmVwb3J0cy5leHBvcnQiLCJyZXBvcnRzLnNjaGVkdWxlIiwicmVwb3J0cy52aWV3Iiwicm9sZXMuYXNzaWduX3VzZXJzIiwicm9sZXMuY3JlYXRlIiwicm9sZXMuZGVsZXRlIiwicm9sZXMuZWRpdCIsInJvbGVzLm1hbmFnZV9wZXJtaXNzaW9ucyIsInJvbGVzLnZpZXciLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJzeXN0ZW0ubWFuYWdlX3NldHRpbmdzIiwic3lzdGVtLnZpZXdfbG9ncyIsInN5c3RlbS52aWV3X21ldHJpY3MiLCJ0ZW5hbnRzLmNyZWF0ZSIsInRlbmFudHMuZGVsZXRlIiwidGVuYW50cy5lZGl0IiwidGVuYW50cy5tYW5hZ2Vfc2V0dGluZ3MiLCJ0ZW5hbnRzLnZpZXciLCJ0ZW5hbnRzLnZpZXdfYWxsIiwidXNlcnMuY3JlYXRlIiwidXNlcnMuZGVsZXRlIiwidXNlcnMuZWRpdCIsInVzZXJzLm1hbmFnZV9yb2xlcyIsInVzZXJzLnZpZXciLCJ1c2Vycy52aWV3X2FsbCJdLCJleHAiOjE3NTU1Njc3MTMsImlzcyI6IkF1dGhTZXJ2aWNlIiwiYXVkIjoiU3RhcnRlckFwcCJ9.NfG_tKMzwJXeumUZ3fgRRwSE8jBv9r5emmbiQWZ3nBU; Referer=https://localhost:3000/login; traceparent=00-3ed3901292c0ff932e32856d24f62200-569e3b26391e1187-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=1; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxIiwidGVuYW50X25hbWUiOiJEZWZhdWx0IFRlbmFudCIsInRlbmFudF9kb21haW4iOiJsb2NhbGhvc3QiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiMjE2ZTQwZjYtMzZhZi00ZDU3LWE4NTEtMWUzYzI3MmIwYmJmIiwiaWF0IjoxNzU1NTY0MTEzLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsInBlcm1pc3Npb24iOlsiYmlsbGluZy5tYW5hZ2UiLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJwZXJtaXNzaW9ucy5kZWxldGUiLCJwZXJtaXNzaW9ucy5lZGl0IiwicGVybWlzc2lvbnMubWFuYWdlIiwicGVybWlzc2lvbnMudmlldyIsInJlcG9ydHMuY3JlYXRlIiwicmVwb3J0cy5leHBvcnQiLCJyZXBvcnRzLnNjaGVkdWxlIiwicmVwb3J0cy52aWV3Iiwicm9sZXMuYXNzaWduX3VzZXJzIiwicm9sZXMuY3JlYXRlIiwicm9sZXMuZGVsZXRlIiwicm9sZXMuZWRpdCIsInJvbGVzLm1hbmFnZV9wZXJtaXNzaW9ucyIsInJvbGVzLnZpZXciLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJzeXN0ZW0ubWFuYWdlX3NldHRpbmdzIiwic3lzdGVtLnZpZXdfbG9ncyIsInN5c3RlbS52aWV3X21ldHJpY3MiLCJ0ZW5hbnRzLmNyZWF0ZSIsInRlbmFudHMuZGVsZXRlIiwidGVuYW50cy5lZGl0IiwidGVuYW50cy5tYW5hZ2Vfc2V0dGluZ3MiLCJ0ZW5hbnRzLnZpZXciLCJ0ZW5hbnRzLnZpZXdfYWxsIiwidXNlcnMuY3JlYXRlIiwidXNlcnMuZGVsZXRlIiwidXNlcnMuZWRpdCIsInVzZXJzLm1hbmFnZV9yb2xlcyIsInVzZXJzLnZpZXciLCJ1c2Vycy52aWV3X2FsbCJdLCJleHAiOjE3NTU1Njc3MTMsImlzcyI6IkF1dGhTZXJ2aWNlIiwiYXVkIjoiU3RhcnRlckFwcCJ9.NfG_tKMzwJXeumUZ3fgRRwSE8jBv9r5emmbiQWZ3nBU; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMSIsIlJvbGVzIjpbXSwiUGVybWlzc2lvbnMiOlsiYmlsbGluZy5tYW5hZ2UiLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJwZXJtaXNzaW9ucy5kZWxldGUiLCJwZXJtaXNzaW9ucy5lZGl0IiwicGVybWlzc2lvbnMubWFuYWdlIiwicGVybWlzc2lvbnMudmlldyIsInJlcG9ydHMuY3JlYXRlIiwicmVwb3J0cy5leHBvcnQiLCJyZXBvcnRzLnNjaGVkdWxlIiwicmVwb3J0cy52aWV3Iiwicm9sZXMuYXNzaWduX3VzZXJzIiwicm9sZXMuY3JlYXRlIiwicm9sZXMuZGVsZXRlIiwicm9sZXMuZWRpdCIsInJvbGVzLm1hbmFnZV9wZXJtaXNzaW9ucyIsInJvbGVzLnZpZXciLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJzeXN0ZW0ubWFuYWdlX3NldHRpbmdzIiwic3lzdGVtLnZpZXdfbG9ncyIsInN5c3RlbS52aWV3X21ldHJpY3MiLCJ0ZW5hbnRzLmNyZWF0ZSIsInRlbmFudHMuZGVsZXRlIiwidGVuYW50cy5lZGl0IiwidGVuYW50cy5tYW5hZ2Vfc2V0dGluZ3MiLCJ0ZW5hbnRzLnZpZXciLCJ0ZW5hbnRzLnZpZXdfYWxsIiwidXNlcnMuY3JlYXRlIiwidXNlcnMuZGVsZXRlIiwidXNlcnMuZWRpdCIsInVzZXJzLm1hbmFnZV9yb2xlcyIsInVzZXJzLnZpZXciLCJ1c2Vycy52aWV3X2FsbCJdLCJFbWFpbCI6bnVsbH0=; X-Tenant-Context=1 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEUN3LKQ0FG:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FG"}
2025-08-18 19:41:53.823 | [00:41:53 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 1 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEUN3LKQ0FG:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FG"}
2025-08-18 19:41:53.945 | [00:41:53 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FG:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FG"}
2025-08-18 19:41:53.945 | [00:41:53 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FG:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FG"}
2025-08-18 19:41:53.947 | [00:41:53 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEUN3LKQ0FG:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FG"}
2025-08-18 19:41:54.046 | [00:41:54 INF] Found 2 tenants for user 6 (unfiltered by tenant context) {"SourceContext": "UserService.Controllers.UsersController", "ActionId": "1ab4bbb1-148d-4127-a586-c928b841f835", "ActionName": "UserService.Controllers.UsersController.GetUserTenants (UserService)", "RequestId": "0HNEUN3LKQ0FG:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FG"}
2025-08-18 19:41:54.058 | [00:41:54 INF] HTTP GET /api/users/6/tenants responded 200 in 237.8511 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:41:54.082 | [00:41:54 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FH:00000001", "RequestPath": "/api/tenants/1/settings", "ConnectionId": "0HNEUN3LKQ0FH"}
2025-08-18 19:41:54.082 | [00:41:54 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FH:00000001", "RequestPath": "/api/tenants/1/settings", "ConnectionId": "0HNEUN3LKQ0FH"}
2025-08-18 19:41:54.082 | [00:41:54 INF] HTTP GET /api/tenants/1/settings responded 404 in 2.0587 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:41:56.666 | [00:41:56 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FI:00000001", "RequestPath": "/health", "ConnectionId": "0HNEUN3LKQ0FI"}
2025-08-18 19:41:56.667 | [00:41:56 INF] HTTP GET /health responded 200 in 0.5721 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:42:08.081 | [00:42:08 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiI2IiwidGVuYW50X25hbWUiOiJNeSBOdW1iZXIgVHdvIiwidGVuYW50X2RvbWFpbiI6IiIsInN1YiI6IjYiLCJlbWFpbCI6Im1jY3JlYXJ5Zm9yd2FyZEBnbWFpbC5jb20iLCJqdGkiOiIzZTc4MDMxNi0yYzg1LTRmNWUtYWZhNS02OWVmNDZiYmQzZjMiLCJpYXQiOjE3NTU1NjQxMjgsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlRlbmFudEFkbWluIiwiZXhwIjoxNzU1NTY3NzI4LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.pEszOnmPWhXOxw2OwEsQS6R86JAmSS-NJHyghZtdt0s; Referer=https://localhost:3000/app/dashboard; traceparent=00-3c5c54b2803146012dbd1f7ad4ed857e-339dafed0dfe9d8e-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=6; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiI2IiwidGVuYW50X25hbWUiOiJNeSBOdW1iZXIgVHdvIiwidGVuYW50X2RvbWFpbiI6IiIsInN1YiI6IjYiLCJlbWFpbCI6Im1jY3JlYXJ5Zm9yd2FyZEBnbWFpbC5jb20iLCJqdGkiOiIzZTc4MDMxNi0yYzg1LTRmNWUtYWZhNS02OWVmNDZiYmQzZjMiLCJpYXQiOjE3NTU1NjQxMjgsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlRlbmFudEFkbWluIiwiZXhwIjoxNzU1NTY3NzI4LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.pEszOnmPWhXOxw2OwEsQS6R86JAmSS-NJHyghZtdt0s; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiNiIsIlJvbGVzIjpbXSwiUGVybWlzc2lvbnMiOltdLCJFbWFpbCI6bnVsbH0=; X-Tenant-Context=6 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.082 | [00:42:08 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 6 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.115 | [00:42:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.115 | [00:42:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.116 | [00:42:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.148 | [00:42:08 WRN] Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'. {"EventId": {"Id": 20504, "Name": "Microsoft.EntityFrameworkCore.Query.MultipleCollectionIncludeWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Query", "ActionId": "207762b2-574b-491b-87a1-de51d3066c38", "ActionName": "UserService.Controllers.UsersController.GetCurrentUserProfile (UserService)", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.218 | [00:42:08 INF] User 6 successfully accessed profile in tenant 6 {"SourceContext": "UserService.Services.UserProfileService", "ActionId": "207762b2-574b-491b-87a1-de51d3066c38", "ActionName": "UserService.Controllers.UsersController.GetCurrentUserProfile (UserService)", "RequestId": "0HNEUN3LKQ0FJ:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEUN3LKQ0FJ"}
2025-08-18 19:42:08.253 | [00:42:08 INF] HTTP GET /api/users/profile responded 200 in 141.4337 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:42:08.269 | [00:42:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FK:00000001", "RequestPath": "/api/tenants/6/settings", "ConnectionId": "0HNEUN3LKQ0FK"}
2025-08-18 19:42:08.269 | [00:42:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FK:00000001", "RequestPath": "/api/tenants/6/settings", "ConnectionId": "0HNEUN3LKQ0FK"}
2025-08-18 19:42:08.269 | [00:42:08 INF] HTTP GET /api/tenants/6/settings responded 404 in 0.8513 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:42:08.279 | [00:42:08 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiI2IiwidGVuYW50X25hbWUiOiJNeSBOdW1iZXIgVHdvIiwidGVuYW50X2RvbWFpbiI6IiIsInN1YiI6IjYiLCJlbWFpbCI6Im1jY3JlYXJ5Zm9yd2FyZEBnbWFpbC5jb20iLCJqdGkiOiIzZTc4MDMxNi0yYzg1LTRmNWUtYWZhNS02OWVmNDZiYmQzZjMiLCJpYXQiOjE3NTU1NjQxMjgsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlRlbmFudEFkbWluIiwiZXhwIjoxNzU1NTY3NzI4LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.pEszOnmPWhXOxw2OwEsQS6R86JAmSS-NJHyghZtdt0s; Referer=https://localhost:3000/app/dashboard; traceparent=00-29cfa1512c21b32045ae4d60de4f984a-40281fbca44c815c-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=6; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiI2IiwidGVuYW50X25hbWUiOiJNeSBOdW1iZXIgVHdvIiwidGVuYW50X2RvbWFpbiI6IiIsInN1YiI6IjYiLCJlbWFpbCI6Im1jY3JlYXJ5Zm9yd2FyZEBnbWFpbC5jb20iLCJqdGkiOiIzZTc4MDMxNi0yYzg1LTRmNWUtYWZhNS02OWVmNDZiYmQzZjMiLCJpYXQiOjE3NTU1NjQxMjgsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IlRlbmFudEFkbWluIiwiZXhwIjoxNzU1NTY3NzI4LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.pEszOnmPWhXOxw2OwEsQS6R86JAmSS-NJHyghZtdt0s; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiNiIsIlJvbGVzIjpbXSwiUGVybWlzc2lvbnMiOltdLCJFbWFpbCI6bnVsbH0=; X-Tenant-Context=6 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEUN3LKQ0FL:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FL"}
2025-08-18 19:42:08.280 | [00:42:08 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 6 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEUN3LKQ0FL:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FL"}
2025-08-18 19:42:08.286 | [00:42:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FL:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FL"}
2025-08-18 19:42:08.286 | [00:42:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FL:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FL"}
2025-08-18 19:42:08.287 | [00:42:08 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEUN3LKQ0FL:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FL"}
2025-08-18 19:42:08.292 | [00:42:08 INF] Found 2 tenants for user 6 (unfiltered by tenant context) {"SourceContext": "UserService.Controllers.UsersController", "ActionId": "1ab4bbb1-148d-4127-a586-c928b841f835", "ActionName": "UserService.Controllers.UsersController.GetUserTenants (UserService)", "RequestId": "0HNEUN3LKQ0FL:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEUN3LKQ0FL"}
2025-08-18 19:42:08.292 | [00:42:08 INF] HTTP GET /api/users/6/tenants responded 200 in 13.9456 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:42:08.305 | [00:42:08 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FM:00000001", "RequestPath": "/api/tenants/6/settings", "ConnectionId": "0HNEUN3LKQ0FM"}
2025-08-18 19:42:08.305 | [00:42:08 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FM:00000001", "RequestPath": "/api/tenants/6/settings", "ConnectionId": "0HNEUN3LKQ0FM"}
2025-08-18 19:42:08.305 | [00:42:08 INF] HTTP GET /api/tenants/6/settings responded 404 in 0.5902 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
2025-08-18 19:42:26.823 | [00:42:26 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEUN3LKQ0FN:00000001", "RequestPath": "/health", "ConnectionId": "0HNEUN3LKQ0FN"}
2025-08-18 19:42:26.823 | [00:42:26 INF] HTTP GET /health responded 200 in 0.8393 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
