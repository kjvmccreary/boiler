BROWSER CONSOLE
TenantContext.tsx:245 🏢 TenantContext: Switching to tenant via JWT refresh: Number Five
TenantContext.tsx:246 🔍 TenantContext: Current auth token exists: true
TenantContext.tsx:247 🔍 TenantContext: Current refresh token exists: true
tenant.service.ts:92 🏢 TenantService: Switching to tenant: 18
tenant.service.ts:93 🔍 TenantService: Request payload: {tenantId: 18}
tenant.service.ts:98 🔍 TenantService: BEFORE request - Auth state: {hasAccessToken: true, hasRefreshToken: true, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...'}
tenant.service.ts:108 🔍 TenantService: BEFORE token tenant info: {tenant_id: '1', tenant_name: 'Default Tenant'}
tenant.service.ts:117 🔍 TenantService: Making API call to switch-tenant...
api.client.ts:44 🔍 API REQUEST (with JWT): POST /api/auth/switch-tenant
api.client.ts:57 ✅ API RESPONSE: /api/auth/switch-tenant 200
api.client.ts:143 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: 'Successfully switched to Number Five', hasData: true, errors: 0}
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/auth/switch-tenant : {accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc…HAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0', refreshToken: 'wm9litTqdTLksKNXeFUkw5B2/Y/zubJjnNyPTKGay3xGXOZsTgzreKqfL5iZ3Kt0VkwgcJ/2mPdHDVeX8ba9CQ==', expiresAt: '2025-08-26T15:43:35.2353385Z', tokenType: 'Bearer', user: {…}, …}
tenant.service.ts:122 🔍 TenantService: API response received: {status: 200, hasData: true, dataKeys: Array(6), dataType: 'object'}
tenant.service.ts:129 🔍 TenantService: Full response data: {accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc…HAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0', refreshToken: 'wm9litTqdTLksKNXeFUkw5B2/Y/zubJjnNyPTKGay3xGXOZsTgzreKqfL5iZ3Kt0VkwgcJ/2mPdHDVeX8ba9CQ==', expiresAt: '2025-08-26T15:43:35.2353385Z', tokenType: 'Bearer', user: {…}, …}
tenant.service.ts:134 🔍 TenantService: Token data analysis: {tokenData: {…}, hasAccessToken: true, hasRefreshToken: true, hasUser: true, hasTenant: true, …}
tenant.service.ts:145 ✅ TenantService: Valid token data found, storing tokens...
tenant.service.ts:159 🔧 TenantService: Token storage verification: {tokenChanged: true, refreshTokenChanged: true, newTokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', newRefreshTokenPreview: 'wm9litTqdTLksKNXeFUk...'}
tenant.service.ts:170 🔍 TenantService: NEW token tenant verification: {tenant_id: '18', tenant_name: 'Number Five', user_id: '6', expected_tenant_id: '18', tenant_id_matches: true}
tenant.service.ts:185 ✅ TenantService: New token has correct tenant_id!
TenantContext.tsx:253 ✅ TenantContext: Tenant switch API successful
TenantContext.tsx:256 🔍 TenantContext: Calling refreshAuth to reload user state...
AuthContext.tsx:151 🔍 AuthContext: Initializing authentication...
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
token.manager.ts:45 🔍 TokenManager: Refresh token info: {hasRefreshToken: true, refreshTokenLength: 88}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
AuthContext.tsx:157 🔍 AuthContext: Token check: {hasToken: true, hasRefreshToken: true, tokenExpired: false}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
AuthContext.tsx:219 🔍 AuthContext: Token valid, validating with backend...
auth.service.ts:84 🔍 AuthService: Validating token using /api/users/profile...
api.client.ts:44 🔍 API REQUEST (with JWT): GET /api/users/profile
api.client.ts:57 ✅ API RESPONSE: /api/users/profile 200
api.client.ts:143 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: '', hasData: true, errors: 0}
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/users/profile : {id: 6, tenantId: 18, email: 'mccrearyforward@gmail.com', firstName: 'Joe Mad Dog', lastName: 'MacDaddy', …}
auth.service.ts:86 ✅ AuthService: Token validation successful: {id: 6, tenantId: 18, email: 'mccrearyforward@gmail.com', firstName: 'Joe Mad Dog', lastName: 'MacDaddy', …}
AuthContext.tsx:97 🔍 AuthContext: Extracting permissions from token: {tokenClaims: Array(16), permissions: Array(35)}
 🔍 AuthContext: Extracting roles from JWT token: {tokenClaims: Array(16), rolesRaw: 'Tenant Admin', rolesType: 'string'}
 ✅ AuthContext: Authentication initialization successful {user: 'mccrearyforward@gmail.com', permissions: Array(10), roles: Array(1)}
 ✅ TenantContext: refreshAuth completed successfully
 🏢 TenantContext: Selecting tenant: Number Five
 🔍 API REQUEST (with JWT): GET /api/tenants/18/settings
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:98 🔍 PermissionContext: Extracting roles from JWT token: {tokenClaims: Array(16), rolesRaw: 'Tenant Admin', rolesType: 'string'}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
TenantContext.tsx:92 🏢 TenantContext: Loading tenants for user: 6
tenant.service.ts:20 🔍 TenantService: Calling API endpoint: /api/users/6/tenants
api.client.ts:44 🔍 API REQUEST (with JWT): GET /api/users/6/tenants
api.client.ts:174  GET https://localhost:3000/api/tenants/18/settings 404 (Not Found)
(anonymous) @ xhr.js:195
xhr @ xhr.js:15
We @ dispatchRequest.js:49
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:174
getTenantSettings @ tenant.service.ts:57
Pl @ TenantContext.tsx:320
Zl @ TenantContext.tsx:261
await in Zl
N @ TenantSwitcher.tsx:59
onClick @ TenantSwitcher.tsx:167
Fr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
af @ react-dom-client.production.js:1470
ss @ react-dom-client.production.js:11996
Ts @ react-dom-client.production.js:14699
fg @ react-dom-client.production.js:14667
api.client.ts:65 🚨 API ERROR: /api/tenants/18/settings 404 Request failed with status code 404
(anonymous) @ api.client.ts:65
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:174
getTenantSettings @ tenant.service.ts:57
Pl @ TenantContext.tsx:320
Zl @ TenantContext.tsx:261
await in Zl
N @ TenantSwitcher.tsx:59
onClick @ TenantSwitcher.tsx:167
Fr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
af @ react-dom-client.production.js:1470
ss @ react-dom-client.production.js:11996
Ts @ react-dom-client.production.js:14699
fg @ react-dom-client.production.js:14667
tenant.service.ts:60 🏢 TenantService: Settings API not available, using defaults
getTenantSettings @ tenant.service.ts:60
await in getTenantSettings
Pl @ TenantContext.tsx:320
Zl @ TenantContext.tsx:261
await in Zl
N @ TenantSwitcher.tsx:59
onClick @ TenantSwitcher.tsx:167
Fr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
af @ react-dom-client.production.js:1470
ss @ react-dom-client.production.js:11996
Ts @ react-dom-client.production.js:14699
fg @ react-dom-client.production.js:14667
TenantContext.tsx:338 🏢 TenantContext: Tenant selection complete: Number Five
TenantContext.tsx:263 🏢 TenantContext: Tenant switched successfully with new JWT: Number Five
TenantNavigationHandler.tsx:14 🔧 TenantNavigationHandler: Redirect flag detected, checking current route accessibility
TenantNavigationHandler.tsx:45 🔧 TenantNavigationHandler: Redirecting to dashboard due to insufficient permissions
dashboard:1 Blocked aria-hidden on an element because its descendant retained focus. The focus must not be hidden from assistive technology users. Avoid using aria-hidden on a focused element or its ancestor. Consider using the inert attribute instead, which will also prevent focus. For more details, see the aria-hidden section of the WAI-ARIA specification at https://w3c.github.io/aria/#aria-hidden.
Element with focus: <button.MuiButtonBase-root MuiIconButton-root MuiIconButton-colorInherit MuiIconButton-sizeLarge css-1vpch0n>
Ancestor with aria-hidden: <div#root> <div id=​"root">​…​</div>​<div class=​"MuiBox-root css-k008qs">​…​</div>​flex<header class=​"MuiPaper-root MuiPaper-elevation MuiPaper-elevation4 MuiAppBar-root MuiAppBar-colorPrimary MuiAppBar-positionFixed mui-fixed css-kqdmuh" style=​"--Paper-shadow:​ 0px 2px 4px -1px rgba(0,0,0,0.2)​,0px 4px 5px 0px rgba(0,0,0,0.14)​,0px 1px 10px 0px rgba(0,0,0,0.12)​;​">​…​</header>​flex<nav class=​"MuiBox-root css-1at9qkq">​…​</nav>​<main class=​"MuiBox-root css-1jyar68">​…​</main>​<div class=​"MuiBox-root css-0">​…​</div>​<div class=​"MuiBox-root css-v08z5u">​…​</div>​flex<div class=​"MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-q8lpi9" style=​"--Paper-shadow:​ 0px 2px 1px -1px rgba(0,0,0,0.2)​,0px 1px 1px 0px rgba(0,0,0,0.14)​,0px 1px 3px 0px rgba(0,0,0,0.12)​;​">​…​</div>​<div class=​"MuiCardContent-root css-15q2cw4">​…​</div>​<div class=​"MuiBox-root css-1qm1lh">​…​</div>​<div class=​"MuiTableContainer-root css-1p6ntod">​…​</div>​<div class=​"MuiTablePagination-root css-1ixt7qf">​…​</div>​<div class=​"MuiToolbar-root MuiToolbar-gutters MuiToolbar-regular MuiTablePagination-toolbar css-l45izh">​…​</div>​flex<div class=​"MuiTablePagination-spacer css-1f63zk">​</div>​<p id=​"«rd»" class=​"MuiTablePagination-selectLabel css-wqp0ve">​Rows per page:​</p>​<div class=​"MuiInputBase-root MuiInputBase-colorPrimary MuiTablePagination-select MuiSelect-root MuiTablePagination-input css-qe2pfu">​…​</div>​flex<div tabindex=​"0" role=​"combobox" aria-expanded=​"false" aria-haspopup=​"listbox" aria-labelledby=​"«rd» «rc»" id=​"«rc»" class=​"MuiSelect-select MuiTablePagination-select MuiSelect-standard MuiInputBase-input css-1yxt9mb">​10​</div>​<input aria-invalid=​"false" aria-hidden=​"true" tabindex=​"-1" class=​"MuiSelect-nativeInput css-147e5lo" value=​"10">​<svg class=​"MuiSvgIcon-root MuiSvgIcon-fontSizeMedium MuiSelect-icon MuiTablePagination-selectIcon MuiSelect-iconStandard css-86oyf8" focusable=​"false" aria-hidden=​"true" viewBox=​"0 0 24 24">​…​</svg>​</div>​<p class=​"MuiTablePagination-displayedRows css-wqp0ve">​0–0 of 0​</p>​<div class=​"MuiTablePaginationActions-root MuiTablePagination-actions css-1xdhyk6">​…​</div>​</div>​</div>​</div>​</div>​</div>​</main>​</div>​<div id=​"_rht_toaster" style=​"position:​ fixed;​ z-index:​ 9999;​ inset:​ 16px;​ pointer-events:​ none;​">​…​</div>​</div>​
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:35 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
api.client.ts:57 ✅ API RESPONSE: /api/users/6/tenants 200
api.client.ts:143 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: 'User tenants retrieved successfully', hasData: true, errors: 0}
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/users/6/tenants : (4) [{…}, {…}, {…}, {…}]
tenant.service.ts:23 🔍 TenantService: Raw API response: (4) [{…}, {…}, {…}, {…}]
tenant.service.ts:29 🔍 TenantService: Converted tenants: (4) [{…}, {…}, {…}, {…}]
TenantContext.tsx:95 🏢 TenantContext: API Response: {success: true, message: 'User tenants loaded', data: Array(4)}
TenantContext.tsx:98 🏢 TenantContext: Setting available tenants: (4) [{…}, {…}, {…}, {…}]
TenantContext.tsx:301 🔍 TenantContext: Extracted tenant from JWT: {tenantId: '18', tenantName: 'Number Five'}
TenantContext.tsx:132 🏢 TenantContext: Tenant selection debug: {jwtTenantId: '18', lastSelectedTenantId: '18', availableTenants: Array(4)}
TenantContext.tsx:144 🏢 TenantContext: Found tenants: {jwtTenant: {…}, lastSelected: {…}}
TenantContext.tsx:166 🏢 TenantContext: Using JWT tenant: Number Five
TenantContext.tsx:314 🏢 TenantContext: Selecting tenant: Number Five
api.client.ts:44 🔍 API REQUEST (with JWT): GET /api/tenants/18/settings
api.client.ts:174  GET https://localhost:3000/api/tenants/18/settings 404 (Not Found)
(anonymous) @ xhr.js:195
xhr @ xhr.js:15
We @ dispatchRequest.js:49
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:174
getTenantSettings @ tenant.service.ts:57
Pl @ TenantContext.tsx:320
ol @ TenantContext.tsx:167
ml @ TenantContext.tsx:100
await in ml
(anonymous) @ TenantContext.tsx:61
gn @ react-dom-client.production.js:8292
pr @ react-dom-client.production.js:9771
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9782
Qr @ react-dom-client.production.js:11313
(anonymous) @ react-dom-client.production.js:11048
It @ scheduler.production.js:151
api.client.ts:65 🚨 API ERROR: /api/tenants/18/settings 404 Request failed with status code 404
(anonymous) @ api.client.ts:65
Promise.then
_request @ Axios.js:163
request @ Axios.js:40
M.<computed> @ Axios.js:213
(anonymous) @ bind.js:5
get @ api.client.ts:174
getTenantSettings @ tenant.service.ts:57
Pl @ TenantContext.tsx:320
ol @ TenantContext.tsx:167
ml @ TenantContext.tsx:100
await in ml
(anonymous) @ TenantContext.tsx:61
gn @ react-dom-client.production.js:8292
pr @ react-dom-client.production.js:9771
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9782
Qr @ react-dom-client.production.js:11313
(anonymous) @ react-dom-client.production.js:11048
It @ scheduler.production.js:151
tenant.service.ts:60 🏢 TenantService: Settings API not available, using defaults
getTenantSettings @ tenant.service.ts:60
await in getTenantSettings
Pl @ TenantContext.tsx:320
ol @ TenantContext.tsx:167
ml @ TenantContext.tsx:100
await in ml
(anonymous) @ TenantContext.tsx:61
gn @ react-dom-client.production.js:8292
pr @ react-dom-client.production.js:9771
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9765
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9878
Dt @ react-dom-client.production.js:9746
pr @ react-dom-client.production.js:9782
Qr @ react-dom-client.production.js:11313
(anonymous) @ react-dom-client.production.js:11048
It @ scheduler.production.js:151
TenantContext.tsx:338 🏢 TenantContext: Tenant selection complete: Number Five
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.create', hasPermission: true, allPermissions: Array(10)}
user.service.ts:58 🔍 UserService: getUsers called with params: {page: 1, pageSize: 10, searchTerm: undefined}
user.service.ts:70 🔍 UserService: Making request to: /api/users?page=1&pageSize=10
api.client.ts:44 🔍 API REQUEST (with JWT): GET /api/users?page=1&pageSize=10
api.client.ts:57 ✅ API RESPONSE: /api/users?page=1&pageSize=10 200
api.client.ts:143 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: '', hasData: true, errors: 0}
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/users?page=1&pageSize=10 : {items: Array(0), totalCount: 0, pageNumber: 1, pageSize: 10, totalPages: 0, …}
user.service.ts:74 🔍 UserService: Processed response: {items: Array(0), totalCount: 0, pageNumber: 1, pageSize: 10, totalPages: 0, …}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 11:43:35 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 10:43:38 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1823, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(35)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.create', hasPermission: true, allPermissions: Array(10)}



SERVICES LOGS
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:15:43:35 +0000] "POST /api/auth/switch-tenant HTTP/1.1" 200 1494 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T15:43:35.250099222Z [15:43:35 INF] requestId: 0HNEV6RFU4762:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-auth:7001/api/auth/switch-tenant.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV6RFU4762:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV6RFU4762"}
boiler-gateway | 2025-08-19T15:43:35.250679457Z [15:43:35 INF] RESPONSE 0b890090: 200 in 168ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4762:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV6RFU4762"}
boiler-gateway | 2025-08-19T15:43:35.250688257Z [15:43:35 INF] HTTP POST /api/auth/switch-tenant responded 200 in 168.9882 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4762:00000001", "ConnectionId": "0HNEV6RFU4762"}
boiler-gateway | 2025-08-19T15:43:35.259719995Z [15:43:35 INF] REQUEST 7d9ec30a: GET /api/users/profile from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.259820001Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T15:43:35.259842802Z    Path: /api/users/profile
boiler-gateway | 2025-08-19T15:43:35.259845502Z    Method: GET
boiler-gateway | 2025-08-19T15:43:35.259847503Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T15:43:35.260544244Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T15:43:35.260585547Z    UserId: 6
boiler-gateway | 2025-08-19T15:43:35.260587847Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T15:43:35.260589847Z    Issuer: AuthService
boiler-gateway | 2025-08-19T15:43:35.260591747Z    Audience: StarterApp
boiler-gateway | 2025-08-19T15:43:35.260593647Z    Claims Count: 50
boiler-gateway | 2025-08-19T15:43:35.260599547Z [15:43:35 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.261055074Z [15:43:35 INF] requestId: 0HNEV6RFU4763:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users/profile' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.261062775Z [15:43:35 INF] requestId: 0HNEV6RFU4763:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users/profile' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.261065375Z [15:43:35 INF] requestId: 0HNEV6RFU4763:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.261067975Z [15:43:35 INF] requestId: 0HNEV6RFU4763:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.261086276Z [15:43:35 INF] requestId: 0HNEV6RFU4763:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-user |   | 2025-08-19T15:43:35.265998469Z [15:43:35 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiOGMzZGRiYjctZmRmMy00OTg5LTk2NDQtZjNhZThkNWQ4Yzk2IiwiaWF0IjoxNzU1NjE4MjE1LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiZXhwIjoxNzU1NjIxODE1LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0; Referer=https://localhost:3000/app/dashboard; traceparent=00-9576922ef5cd09d95203b04f9eeac8ba-557070ff362fec0a-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=18; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiOGMzZGRiYjctZmRmMy00OTg5LTk2NDQtZjNhZThkNWQ4Yzk2IiwiaWF0IjoxNzU1NjE4MjE1LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiZXhwIjoxNzU1NjIxODE1LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMTgiLCJSb2xlcyI6W10sIlBlcm1pc3Npb25zIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiRW1haWwiOm51bGx9; X-Tenant-Context=18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.266164179Z [15:43:35 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.298668114Z [15:43:35 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.298735318Z [15:43:35 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.299597169Z [15:43:35 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.330006079Z [15:43:35 WRN] Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'. {"EventId": {"Id": 20504, "Name": "Microsoft.EntityFrameworkCore.Query.MultipleCollectionIncludeWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Query", "ActionId": "b938d1b4-0a68-4da4-b29c-335a13113b1c", "ActionName": "UserService.Controllers.UsersController.GetCurrentUserProfile (UserService)", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.416290116Z [15:43:35 INF] User 6 successfully accessed profile in tenant 18 {"SourceContext": "UserService.Services.UserProfileService", "ActionId": "b938d1b4-0a68-4da4-b29c-335a13113b1c", "ActionName": "UserService.Controllers.UsersController.GetCurrentUserProfile (UserService)", "RequestId": "0HNEV6RE6PA0E:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RE6PA0E"}
boiler-user |   | 2025-08-19T15:43:35.426615131Z [15:43:35 INF] HTTP GET /api/users/profile responded 200 in 158.5857 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T15:43:35.426561628Z [15:43:35 INF] requestId: 0HNEV6RFU4763:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users/profile.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.426610230Z [15:43:35 INF] RESPONSE 7d9ec30a: 200 in 165ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV6RFU4763"}
boiler-gateway | 2025-08-19T15:43:35.426613531Z [15:43:35 INF] HTTP GET /api/users/profile responded 200 in 165.8912 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4763:00000001", "ConnectionId": "0HNEV6RFU4763"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:15:43:35 +0000] "GET /api/users/profile HTTP/1.1" 200 315 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T15:43:35.441769433Z [15:43:35 INF] REQUEST 0bc67839: GET /api/tenants/18/settings from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.441839937Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T15:43:35.441843337Z    Path: /api/tenants/18/settings
boiler-gateway | 2025-08-19T15:43:35.441845837Z    Method: GET
boiler-gateway | 2025-08-19T15:43:35.441848038Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T15:43:35.441850038Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T15:43:35.441852038Z    UserId: 6
boiler-gateway | 2025-08-19T15:43:35.441855538Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T15:43:35.441857438Z    Issuer: AuthService
boiler-gateway | 2025-08-19T15:43:35.441859438Z    Audience: StarterApp
boiler-gateway | 2025-08-19T15:43:35.441861338Z    Claims Count: 50
boiler-gateway | 2025-08-19T15:43:35.441863339Z [15:43:35 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.442281963Z [15:43:35 INF] requestId: 0HNEV6RFU4764:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/tenants/18/settings' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.442314265Z [15:43:35 INF] requestId: 0HNEV6RFU4764:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/tenants/18/settings' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.442317566Z [15:43:35 INF] requestId: 0HNEV6RFU4764:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.442320266Z [15:43:35 INF] requestId: 0HNEV6RFU4764:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.442322866Z [15:43:35 INF] requestId: 0HNEV6RFU4764:00000001, previousRequestId: No PreviousRequestId, message: '/api/tenants/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-user |   | 2025-08-19T15:43:35.448016805Z [15:43:35 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0F:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RE6PA0F"}
boiler-user |   | 2025-08-19T15:43:35.448188615Z [15:43:35 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0F:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RE6PA0F"}
boiler-user |   | 2025-08-19T15:43:35.448196416Z [15:43:35 INF] HTTP GET /api/tenants/18/settings responded 404 in 0.7697 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T15:43:35.448884757Z [15:43:35 WRN] requestId: 0HNEV6RFU4764:00000001, previousRequestId: No PreviousRequestId, message: '404 (Not Found) status code of request URI: https://boiler-user:7002/api/tenants/18/settings.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.449162373Z [15:43:35 INF] RESPONSE 0bc67839: 404 in 7ms | Size: 0 {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4764"}
boiler-gateway | 2025-08-19T15:43:35.449210776Z [15:43:35 INF] HTTP GET /api/tenants/18/settings responded 404 in 7.9889 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4764:00000001", "ConnectionId": "0HNEV6RFU4764"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:15:43:35 +0000] "GET /api/tenants/18/settings HTTP/1.1" 404 0 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T15:43:35.454458188Z [15:43:35 INF] REQUEST 0d43f9ac: GET /api/users/6/tenants from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.454555094Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T15:43:35.454558794Z    Path: /api/users/6/tenants
boiler-gateway | 2025-08-19T15:43:35.454560994Z    Method: GET
boiler-gateway | 2025-08-19T15:43:35.454562995Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T15:43:35.454663301Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T15:43:35.454682602Z    UserId: 6
boiler-gateway | 2025-08-19T15:43:35.454684902Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T15:43:35.454686802Z    Issuer: AuthService
boiler-gateway | 2025-08-19T15:43:35.454711303Z    Audience: StarterApp
boiler-gateway | 2025-08-19T15:43:35.454713304Z    Claims Count: 50
boiler-gateway | 2025-08-19T15:43:35.454715504Z [15:43:35 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.455269937Z [15:43:35 INF] requestId: 0HNEV6RFU4765:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users/6/tenants' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.455304139Z [15:43:35 INF] requestId: 0HNEV6RFU4765:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users/6/tenants' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.455307739Z [15:43:35 INF] requestId: 0HNEV6RFU4765:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.455310439Z [15:43:35 INF] requestId: 0HNEV6RFU4765:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.455312939Z [15:43:35 INF] requestId: 0HNEV6RFU4765:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-user |   | 2025-08-19T15:43:35.460924473Z [15:43:35 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiOGMzZGRiYjctZmRmMy00OTg5LTk2NDQtZjNhZThkNWQ4Yzk2IiwiaWF0IjoxNzU1NjE4MjE1LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiZXhwIjoxNzU1NjIxODE1LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0; Referer=https://localhost:3000/app/dashboard; traceparent=00-68e95236111763c255665230cb9102f1-ed766e002ed9e8d4-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=18; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiOGMzZGRiYjctZmRmMy00OTg5LTk2NDQtZjNhZThkNWQ4Yzk2IiwiaWF0IjoxNzU1NjE4MjE1LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiZXhwIjoxNzU1NjIxODE1LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMTgiLCJSb2xlcyI6W10sIlBlcm1pc3Npb25zIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiRW1haWwiOm51bGx9; X-Tenant-Context=18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV6RE6PA0G:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RE6PA0G"}
boiler-user |   | 2025-08-19T15:43:35.461086583Z [15:43:35 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV6RE6PA0G:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RE6PA0G"}
boiler-user |   | 2025-08-19T15:43:35.468992354Z [15:43:35 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0G:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RE6PA0G"}
boiler-user |   | 2025-08-19T15:43:35.469035156Z [15:43:35 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0G:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RE6PA0G"}
boiler-user |   | 2025-08-19T15:43:35.470200826Z [15:43:35 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEV6RE6PA0G:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RE6PA0G"}
boiler-user |   | 2025-08-19T15:43:35.474668292Z [15:43:35 INF] Found 4 tenants for user 6 (unfiltered by tenant context) {"SourceContext": "UserService.Controllers.UsersController", "ActionId": "0fbc015e-3b02-462d-9257-c2f3102a6f91", "ActionName": "UserService.Controllers.UsersController.GetUserTenants (UserService)", "RequestId": "0HNEV6RE6PA0G:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RE6PA0G"}
boiler-user |   | 2025-08-19T15:43:35.475868163Z [15:43:35 INF] HTTP GET /api/users/6/tenants responded 200 in 14.7100 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T15:43:35.476241985Z [15:43:35 INF] requestId: 0HNEV6RFU4765:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users/6/tenants.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.476881023Z [15:43:35 INF] RESPONSE 0d43f9ac: 200 in 22ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV6RFU4765"}
boiler-gateway | 2025-08-19T15:43:35.476928226Z [15:43:35 INF] HTTP GET /api/users/6/tenants responded 200 in 22.6126 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4765:00000001", "ConnectionId": "0HNEV6RFU4765"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:15:43:35 +0000] "GET /api/users/6/tenants HTTP/1.1" 200 382 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T15:43:35.487388249Z [15:43:35 INF] REQUEST f83da0a1: GET /api/tenants/18/settings from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.487460953Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T15:43:35.487464553Z    Path: /api/tenants/18/settings
boiler-gateway | 2025-08-19T15:43:35.487466653Z    Method: GET
boiler-gateway | 2025-08-19T15:43:35.487469354Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T15:43:35.487600461Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T15:43:35.487630963Z    UserId: 6
boiler-gateway | 2025-08-19T15:43:35.487633363Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T15:43:35.487635363Z    Issuer: AuthService
boiler-gateway | 2025-08-19T15:43:35.487637364Z    Audience: StarterApp
boiler-gateway | 2025-08-19T15:43:35.487639464Z    Claims Count: 50
boiler-gateway | 2025-08-19T15:43:35.487641464Z [15:43:35 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.488098591Z [15:43:35 INF] requestId: 0HNEV6RFU4766:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/tenants/18/settings' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.488163395Z [15:43:35 INF] requestId: 0HNEV6RFU4766:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/tenants/18/settings' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.488166895Z [15:43:35 INF] requestId: 0HNEV6RFU4766:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-user |   | 2025-08-19T15:43:35.492919978Z [15:43:35 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0H:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RE6PA0H"}
boiler-gateway | 2025-08-19T15:43:35.488169495Z [15:43:35 INF] requestId: 0HNEV6RFU4766:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-user |   | 2025-08-19T15:43:35.492995383Z [15:43:35 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0H:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RE6PA0H"}
boiler-user |   | 2025-08-19T15:43:35.492998983Z [15:43:35 INF] HTTP GET /api/tenants/18/settings responded 404 in 0.7803 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:15:43:35 +0000] "GET /api/tenants/18/settings HTTP/1.1" 404 0 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T15:43:35.488171995Z [15:43:35 INF] requestId: 0HNEV6RFU4766:00000001, previousRequestId: No PreviousRequestId, message: '/api/tenants/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.493773329Z [15:43:35 WRN] requestId: 0HNEV6RFU4766:00000001, previousRequestId: No PreviousRequestId, message: '404 (Not Found) status code of request URI: https://boiler-user:7002/api/tenants/18/settings.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.493897536Z [15:43:35 INF] RESPONSE f83da0a1: 404 in 6ms | Size: 0 {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:35.493922938Z [15:43:35 INF] HTTP GET /api/tenants/18/settings responded 404 in 6.7551 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4766:00000001", "ConnectionId": "0HNEV6RFU4766"}
boiler-gateway | 2025-08-19T15:43:38.837420086Z [15:43:38 INF] REQUEST 6bf20c21: GET /api/users?page=1&pageSize=10 from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.837494791Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T15:43:38.837499791Z    Path: /api/users
boiler-gateway | 2025-08-19T15:43:38.837503091Z    Method: GET
boiler-gateway | 2025-08-19T15:43:38.837505691Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T15:43:38.837913916Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T15:43:38.837919616Z    UserId: 6
boiler-gateway | 2025-08-19T15:43:38.837922516Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T15:43:38.837925016Z    Issuer: AuthService
boiler-gateway | 2025-08-19T15:43:38.837927516Z    Audience: StarterApp
boiler-gateway | 2025-08-19T15:43:38.837929817Z    Claims Count: 50
boiler-gateway | 2025-08-19T15:43:38.837932317Z [15:43:38 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.838545053Z [15:43:38 INF] requestId: 0HNEV6RFU4767:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.838607657Z [15:43:38 INF] requestId: 0HNEV6RFU4767:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.838611157Z [15:43:38 INF] requestId: 0HNEV6RFU4767:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.838613857Z [15:43:38 INF] requestId: 0HNEV6RFU4767:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.838637759Z [15:43:38 INF] requestId: 0HNEV6RFU4767:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-user |   | 2025-08-19T15:43:38.843581453Z [15:43:38 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiOGMzZGRiYjctZmRmMy00OTg5LTk2NDQtZjNhZThkNWQ4Yzk2IiwiaWF0IjoxNzU1NjE4MjE1LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiZXhwIjoxNzU1NjIxODE1LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0; Referer=https://localhost:3000/app/users; traceparent=00-25ef926d432014c8720b0093d4fa5429-8a98cf707075e73d-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=18; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiOGMzZGRiYjctZmRmMy00OTg5LTk2NDQtZjNhZThkNWQ4Yzk2IiwiaWF0IjoxNzU1NjE4MjE1LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiZXhwIjoxNzU1NjIxODE1LCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.xvkZhWmg5iqxWCpUzwVczHkffP_ONERDpMRWrjwNvN0; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMTgiLCJSb2xlcyI6W10sIlBlcm1pc3Npb25zIjpbInBlcm1pc3Npb25zLmRlbGV0ZSIsInJvbGVzLmNyZWF0ZSIsInJlcG9ydHMudmlldyIsInVzZXJzLnZpZXdfYWxsIiwic3lzdGVtLnZpZXdfbWV0cmljcyIsInVzZXJzLmRlbGV0ZSIsInRlbmFudHMudmlld19hbGwiLCJ1c2Vycy52aWV3IiwidGVuYW50cy5jcmVhdGUiLCJiaWxsaW5nLnZpZXdfaW52b2ljZXMiLCJzeXN0ZW0ubWFuYWdlX2JhY2t1cHMiLCJyb2xlcy5hc3NpZ25fdXNlcnMiLCJiaWxsaW5nLnZpZXciLCJiaWxsaW5nLnByb2Nlc3NfcGF5bWVudHMiLCJyb2xlcy5kZWxldGUiLCJ0ZW5hbnRzLmVkaXQiLCJ0ZW5hbnRzLmRlbGV0ZSIsInVzZXJzLm1hbmFnZV9yb2xlcyIsImJpbGxpbmcubWFuYWdlIiwicm9sZXMudmlldyIsInJlcG9ydHMuc2NoZWR1bGUiLCJwZXJtaXNzaW9ucy5tYW5hZ2UiLCJyZXBvcnRzLmV4cG9ydCIsInVzZXJzLmVkaXQiLCJyb2xlcy5tYW5hZ2VfcGVybWlzc2lvbnMiLCJ1c2Vycy5jcmVhdGUiLCJzeXN0ZW0udmlld19sb2dzIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJ0ZW5hbnRzLnZpZXciLCJwZXJtaXNzaW9ucy5jcmVhdGUiLCJ0ZW5hbnRzLm1hbmFnZV9zZXR0aW5ncyIsInN5c3RlbS5tYW5hZ2Vfc2V0dGluZ3MiLCJyZXBvcnRzLmNyZWF0ZSIsInBlcm1pc3Npb25zLmVkaXQiXSwiRW1haWwiOm51bGx9; X-Tenant-Context=18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.843689959Z [15:43:38 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.846496126Z [15:43:38 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.846590132Z [15:43:38 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.846643535Z [15:43:38 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.856420117Z [15:43:38 WRN] 🔍 GetUsersAsync DEBUG: CurrentTenantId = 18 {"SourceContext": "UserService.Services.UserServiceImplementation", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.867774293Z [15:43:38 WRN] 🔍 GetUsersAsync DEBUG: Found 0 TenantUsers records for tenant 18:  {"SourceContext": "UserService.Services.UserServiceImplementation", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.873678445Z [15:43:38 WRN] 🔍 GetUsersAsync DEBUG: Found 0 active users total:  {"SourceContext": "UserService.Services.UserServiceImplementation", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.879284378Z [15:43:38 WRN] 🔍 GetUsersAsync DEBUG: Join query found 0 users:  {"SourceContext": "UserService.Services.UserServiceImplementation", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.893064299Z [15:43:38 WRN] 🔍 GetUsersAsync DEBUG: Total count after all filters: 0 {"SourceContext": "UserService.Services.UserServiceImplementation", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.901014972Z [15:43:38 WRN] Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'. {"EventId": {"Id": 20504, "Name": "Microsoft.EntityFrameworkCore.Query.MultipleCollectionIncludeWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Query", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.921686903Z [15:43:38 WRN] 🔍 GetUsersAsync DEBUG: Final result count: 0 {"SourceContext": "UserService.Services.UserServiceImplementation", "ActionId": "a240533c-c769-424c-bef3-7a8023c9a49c", "ActionName": "UserService.Controllers.UsersController.GetUsers (UserService)", "RequestId": "0HNEV6RE6PA0I:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RE6PA0I"}
boiler-user |   | 2025-08-19T15:43:38.927346140Z [15:43:38 INF] HTTP GET /api/users responded 200 in 84.0673 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T15:43:38.928164788Z [15:43:38 INF] requestId: 0HNEV6RFU4767:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users?page=1&pageSize=10.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.928536711Z [15:43:38 INF] RESPONSE 6bf20c21: 200 in 91ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "RequestPath": "/api/users", "ConnectionId": "0HNEV6RFU4767"}
boiler-gateway | 2025-08-19T15:43:38.928545111Z [15:43:38 INF] HTTP GET /api/users responded 200 in 91.4369 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV6RFU4767:00000001", "ConnectionId": "0HNEV6RFU4767"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:15:43:38 +0000] "GET /api/users?page=1&pageSize=10 HTTP/1.1" 200 160 "https://localhost:3000/app/users" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
