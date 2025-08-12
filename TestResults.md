PS C:\Users\mccre\dev\boiler\src\frontend\react-app> npm test -- --run --reporter=verbose

> microservices-frontend@0.0.0 test
> vitest --config vitest.config.js --run --reporter=verbose


 RUN  v1.6.1 C:/Users/mccre/dev/boiler/src/frontend/react-app

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/roles/RoleDetails.test.tsx > RoleDetails > handles role not found error
Failed to load role details: Error: Role not found
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\roles\RoleDetails.test.tsx:261:58
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7
    at withEnv (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:83:5)

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > shows error state when loading fails
Failed to fetch roles: Error: Failed to fetch roles
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\roles\RoleList.test.tsx:263:55
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7
    at withEnv (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:83:5)

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/auth/__tests__/ChangePasswordForm.test.tsx > ChangePasswordForm > handles API error gracefully
Change password failed: {
  response: { status: 400, data: { message: 'Current password is incorrect' } }
}

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > displays error message when confirmation fails
Email confirmation failed: Error: Confirmation failed
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:101:65
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > handles expired token error specifically
Email confirmation failed: Error: The confirmation link has expired
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:114:28
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > handles invalid token error specifically
Email confirmation failed: Error: The confirmation link is invalid
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:125:28
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Error Handling > handles not found error specifically
Email confirmation failed: Error: Token not found
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:136:29
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > API Integration > handles network errors gracefully
Email confirmation failed: Error: Network error
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:193:28
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/EmailConfirmation.test.tsx > EmailConfirmation > Accessibility > provides appropriate error announcements
Email confirmation failed: Error: Test error
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\EmailConfirmation.test.tsx:208:65
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/auth/LogoutButton.test.tsx > LogoutButton > Error Handling > handles logout error gracefully
Logout failed: Error: Logout failed
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\LogoutButton.test.tsx:171:21
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Email Confirmation Flow > handles confirmation failure and provides recovery options
Email confirmation failed: Error: The confirmation link has expired
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\auth-integration.test.tsx:80:9
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Error Recovery > provides clear error messages and recovery paths
Email confirmation failed: Error: Network error occurred
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\auth-integration.test.tsx:149:9
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stderr | src/components/__tests__/auth/auth-integration.test.tsx > Authentication Integration Tests > Accessibility Integration > provides appropriate ARIA announcements for state changes
Email confirmation failed: Error: Confirmation failed
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\auth\auth-integration.test.tsx:188:9
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)
    at startTests (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:967:3)
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/vitest/dist/chunks/runtime-runBaseTests.oAvMKtQC.js:116:7

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should render permissions after loading
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should show search box when showSearch is true
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should call onChange when permission is selected
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should select all permissions in category when category checkbox is clicked
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should be disabled when disabled prop is true
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.
MUI: You are providing a disabled `button` child to the Tooltip component.
A disabled element does not fire events.
Tooltip needs to listen to the child element's events to display the title.

Add a simple wrapper element, such as a `span`.

stderr | src/components/__tests__/roles/PermissionSelector.test.tsx > PermissionSelector > should handle API error gracefully
Failed to load permissions: Error: API Error
    at C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\__tests__\roles\PermissionSelector.test.tsx:126:74
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:135:14
    at file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:60:26
    at runTest (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:781:17)
    at runNextTicks (node:internal/process/task_queues:65:5)
    at listOnTimeout (node:internal/timers:549:9)
    at processTimers (node:internal/timers:523:7)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runSuite (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:909:15)
    at runFiles (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@vitest/runner/dist/index.js:958:5)

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:11:13)
🔍 API CLIENT: Creating axios instance with baseURL: http://localhost:5000/api

stderr | src/components/__tests__/roles/PermissionTreeView.test.tsx > PermissionTreeView > should render permission hierarchy
Each child in a list should have a unique "key" prop.

Check the render method of `ul`. It was passed a child from PermissionTreeView. See https://react.dev/link/warning-keys for more information.
In HTML, <div> cannot be a descendant of <p>.
This will cause a hydration error.

  ...
    <MuiList-root as="ul" className="MuiList-ro..." ref={null} ownerState={{dense:false, ...}} sx={{width:"100%"}}>
      <Insertion>
      <ul className="MuiList-ro...">
        <ListItem sx={{borderRadius:1, ...}}>
          <MuiListItem-root as="li" ref={function} ownerState={{sx:{...}, ...}} className="MuiListIte..." ...>
            <Insertion>
            <li className="MuiListIte..." ref={function}>
              <ListItemIcon>
              <ListItemText primary={<ForwardRef(Box)>} secondary={<ForwardRef(Box)>}>
                <MuiListItemText-root className="MuiListIte..." ref={null} ownerState={{primary:true, ...}}>
                  <Insertion>
                  <div className="MuiListIte...">
                    <Typography>
                    <Typography variant="body2" color="textSecondary" className="MuiListIte..." ref={null} ...>
                      <MuiTypography-root as="p" ref={null} className="MuiTypogra..." ownerState={{variant:"b...", ...}} ...>
                        <Insertion>
>                       <p
>                         className="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiT..."
>                         style={{}}
>                       >
                          <Box>
                            <Styled(div) as="div" ref={null} className="MuiBox-root" theme={{...}} sx={{}}>
                              <Insertion>
>                             <div className="MuiBox-root css-0">
        ...

<p> cannot contain a nested <div>.
See this log for the ancestor stack trace.

stdout | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > renders error UI when error occurs
🚨 Error Boundary Report

stderr | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > renders error UI when error occurs
Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  [stack]: [Getter/Setter],
  [message]: 'Test error'
}

The above error occurred in the <ThrowError> component.

React will try to recreate this component tree from scratch using the error boundary you provided, ErrorBoundary.

ErrorBoundary caught an error: Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  componentStack: '\n' +
    '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
    '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
}
  Error: Error: Test error
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
      at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
      at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
      at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
      at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
      at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
      at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
      at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
      at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
      at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44)
  Error Info: {
    componentStack: '\n' +
      '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
      '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
  }
  Component Stack:
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:9:23)
      at ErrorBoundary (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\ErrorBoundary.tsx:13:5)

stdout | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > shows page-level error UI when level is page
🚨 Error Boundary Report

stderr | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > shows page-level error UI when level is page
Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  [stack]: [Getter/Setter],
  [message]: 'Test error'
}

The above error occurred in the <ThrowError> component.

React will try to recreate this component tree from scratch using the error boundary you provided, ErrorBoundary.

ErrorBoundary caught an error: Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  componentStack: '\n' +
    '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
    '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
}
  Error: Error: Test error
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
      at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
      at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
      at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
      at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
      at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
      at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
      at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
      at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
      at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44)
  Error Info: {
    componentStack: '\n' +
      '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
      '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
  }
  Component Stack:
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:9:23)
      at ErrorBoundary (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\ErrorBoundary.tsx:13:5)

stdout | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > calls onError callback when error occurs
🚨 Error Boundary Report

stderr | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > calls onError callback when error occurs
Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  [stack]: [Getter/Setter],
  [message]: 'Test error'
}

The above error occurred in the <ThrowError> component.

React will try to recreate this component tree from scratch using the error boundary you provided, ErrorBoundary.

ErrorBoundary caught an error: Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  componentStack: '\n' +
    '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
    '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
}
  Error: Error: Test error
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
      at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
      at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
      at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
      at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
      at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
      at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
      at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
      at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
      at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44)
  Error Info: {
    componentStack: '\n' +
      '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
      '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
  }
  Component Stack:
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:9:23)
      at ErrorBoundary (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\ErrorBoundary.tsx:13:5)

stdout | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > shows error details when details button is clicked
🚨 Error Boundary Report

stderr | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > shows error details when details button is clicked
Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  [stack]: [Getter/Setter],
  [message]: 'Test error'
}

The above error occurred in the <ThrowError> component.

React will try to recreate this component tree from scratch using the error boundary you provided, ErrorBoundary.

ErrorBoundary caught an error: Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  componentStack: '\n' +
    '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
    '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
}
  Error: Error: Test error
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
      at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
      at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
      at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
      at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
      at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
      at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
      at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
      at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
      at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44)
  Error Info: {
    componentStack: '\n' +
      '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
      '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
  }
  Component Stack:
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:9:23)
      at ErrorBoundary (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\ErrorBoundary.tsx:13:5)

stdout | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > renders custom fallback when provided
🚨 Error Boundary Report

stderr | src/components/common/__tests__/ErrorBoundary.test.tsx > ErrorBoundary > renders custom fallback when provided
Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  [stack]: [Getter/Setter],
  [message]: 'Test error'
}

The above error occurred in the <ThrowError> component.

React will try to recreate this component tree from scratch using the error boundary you provided, ErrorBoundary.

ErrorBoundary caught an error: Error: Test error
    at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
    at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
    at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
    at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
    at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
    at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
    at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
    at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
    at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
    at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44) {
  componentStack: '\n' +
    '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
    '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
}
  Error: Error: Test error
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:12:11)
      at Object.react_stack_bottom_frame (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:23863:20)
      at renderWithHooks (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:5529:22)
      at updateFunctionComponent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:8897:19)
      at beginWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:10522:18)
      at runWithFiberInDEV (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:1522:13)
      at performUnitOfWork (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:15140:22)
      at workLoopSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14956:41)
      at renderRootSync (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14936:11)
      at performWorkOnRoot (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\react-dom\cjs\react-dom-client.development.js:14462:44)
  Error Info: {
    componentStack: '\n' +
      '    at ThrowError (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\__tests__\\ErrorBoundary.test.tsx:9:23)\n' +
      '    at ErrorBoundary (C:\\Users\\mccre\\dev\\boiler\\src\\frontend\\react-app\\src\\components\\common\\ErrorBoundary.tsx:13:5)'
  }
  Component Stack:
      at ThrowError (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\__tests__\ErrorBoundary.test.tsx:9:23)
      at ErrorBoundary (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\common\ErrorBoundary.tsx:13:5)

 ✓ src/test/scenarios/api-permission-integration.test.tsx (6)
   ✓ API Permission Integration Scenarios (6)
     ✓ API Call Authorization (2)
       ✓ should allow API calls for users with correct permissions
       ✓ should reject API calls for users without permissions
     ✓ Role-Based Data Filtering (2)
       ✓ should filter user data based on role permissions
       ✓ should limit data for lower privilege users
     ✓ Permission-Based Error Handling (2)
       ✓ should handle 403 Forbidden responses gracefully
       ✓ should handle successful operations for authorized users
 ✓ src/components/__tests__/roles/RoleDetails.test.tsx (10) 2302ms
   ✓ RoleDetails (10) 2302ms
     ✓ renders role information correctly 419ms
     ✓ displays permissions grouped by category
     ✓ shows assigned users with correct status chips
     ✓ handles edit button click for non-system roles 466ms
     ✓ disables edit and delete buttons for system roles 450ms
     ✓ handles delete role with confirmation 482ms
     ✓ handles role not found error
     ✓ shows loading state initially
     ✓ handles empty permissions list
     ✓ handles empty users list
 ✓ src/components/__tests__/roles/RoleList.test.tsx (10) 3193ms
   ✓ RoleList (10) 3192ms
     ✓ renders roles list correctly 342ms
     ✓ handles search functionality 951ms
     ✓ handles pagination correctly
     ✓ calls onEditRole when edit button is clicked 314ms
     ✓ calls onDeleteRole when delete button is clicked
     ✓ disables edit and delete buttons for system roles
     ✓ shows loading state initially
     ✓ shows error state when loading fails
     ✓ displays role information correctly
     ✓ handles page size changes 814ms
 ❯ src/components/auth/__tests__/ChangePasswordForm.test.tsx (9) 15936ms
   ❯ ChangePasswordForm (9) 15936ms
     ✓ renders the form fields 580ms
     × prevents submission with empty fields (retry x1) 892ms
     ✓ shows error when new password is same as current 2149ms
     ✓ shows error when passwords do not match 2068ms
     ✓ successfully changes password and shows success screen 2161ms
     ✓ toggles password visibility 318ms
     ✓ calls logout when clicking "Sign In Again" 2228ms
     × prevents submission with weak password (retry x1) 3149ms
     ✓ handles API error gracefully 2391ms
 ✓ src/components/__tests__/auth/EmailConfirmation.test.tsx (13) 1792ms
   ✓ EmailConfirmation (13) 1791ms
     ✓ Token Validation (2) 437ms
       ✓ displays invalid token message when no token is provided
       ✓ shows loading state initially when token is present
     ✓ Successful Confirmation (2) 399ms
       ✓ displays success message when confirmation succeeds
       ✓ navigates to login when Sign In Now button is clicked
     ✓ Error Handling (4)
       ✓ displays error message when confirmation fails
       ✓ handles expired token error specifically
       ✓ handles invalid token error specifically
       ✓ handles not found error specifically
     ✓ User Actions (2) 620ms
       ✓ handles resend confirmation button click 342ms
       ✓ handles back to login button click
     ✓ API Integration (2)
       ✓ calls authService.confirmEmail with correct token
       ✓ handles network errors gracefully
     ✓ Accessibility (1)
       ✓ provides appropriate error announcements
 ✓ src/test/scenarios/permission-component-patterns.test.tsx (4)
   ✓ RBAC Permission Component Patterns (4)
     ✓ Conditional UI Rendering (1)
       ✓ should show/hide UI elements based on user permissions
     ✓ Form Field Permissions (1)
       ✓ should enable/disable form fields based on permissions
     ✓ Navigation Menu Permissions (1)
       ✓ should show navigation items based on user permissions
     ✓ Data Table Action Buttons (1)
       ✓ should show/hide action buttons based on permissions
 ✓ src/components/__tests__/auth/LogoutButton.test.tsx (12) 2892ms
   ✓ LogoutButton (12) 2892ms
     ✓ Button Variants (4) 436ms
       ✓ renders button variant by default
       ✓ renders icon variant correctly
       ✓ renders text variant correctly
       ✓ renders custom children text
     ✓ Logout Functionality (4) 1721ms
       ✓ performs logout without confirmation by default
       ✓ shows confirmation dialog when showConfirmation is true 452ms
       ✓ cancels logout from confirmation dialog 655ms
       ✓ confirms logout from confirmation dialog 491ms
     ✓ Error Handling (2) 419ms
       ✓ handles logout error gracefully
       ✓ shows loading state during logout
     ✓ Accessibility (2) 316ms
       ✓ has proper ARIA attributes for icon variant
       ✓ manages dialog accessibility correctly
 ✓ src/components/common/__tests__/LoadingStates.test.tsx (22) 1176ms
   ✓ LoadingStates (22) 1175ms
     ✓ LoadingSpinner (3)
       ✓ renders with default props
       ✓ renders with custom message
       ✓ renders with full height when specified
     ✓ PageLoading (5) 309ms
       ✓ renders with default message
       ✓ renders with custom message
       ✓ shows progress percentage when provided
       ✓ renders indeterminate progress when no percentage provided
       ✓ renders determinate progress when percentage provided
     ✓ TableSkeleton (5)
       ✓ renders skeleton elements
       ✓ renders custom number of rows and columns
       ✓ can hide header
       ✓ renders correct number of skeleton elements based on props
       ✓ renders correct number of skeleton elements without header
     ✓ UserListSkeleton (5)
       ✓ renders default number of user items
       ✓ renders custom number of user items
       ✓ renders correct number of user skeleton items
       ✓ renders within a MUI List component
       ✓ renders within a MUI List component with default count
     ✓ LoadingSpinner edge cases (2)
       ✓ renders with custom size
       ✓ renders without message when not provided
     ✓ Accessibility (2)
       ✓ has proper ARIA attributes for progress indicators
       ✓ has proper ARIA attributes for determinate progress
 ✓ src/components/__tests__/auth/auth-integration.test.tsx (7) 2422ms
   ✓ Authentication Integration Tests (7) 2421ms
     ✓ Email Confirmation Flow (2) 528ms
       ✓ completes email confirmation and shows success state
       ✓ handles confirmation failure and provides recovery options 308ms
     ✓ Logout Flow (2) 1288ms
       ✓ performs logout with confirmation dialog 519ms
       ✓ cancels logout when user chooses cancel 769ms
     ✓ Error Recovery (1)
       ✓ provides clear error messages and recovery paths
     ✓ Accessibility Integration (2) 338ms
       ✓ maintains proper focus management through auth flows
       ✓ provides appropriate ARIA announcements for state changes
 ✓ src/test/examples/rbac-usage-examples.test.tsx (5)
   ✓ RBAC Test Utilities - Usage Examples (5)
     ✓ Basic Role Rendering (1)
       ✓ should render differently for different roles
     ✓ Batch Role Testing (1)
       ✓ should test all roles against component
     ✓ Form Testing (1)
       ✓ should test form field permissions
     ✓ Navigation Testing (1)
       ✓ should test menu visibility
     ✓ Error State Testing (1)
       ✓ should test permission-based errors
 ✓ src/test/scenarios/role-hierarchy-scenarios.test.tsx (6)
   ✓ Role Hierarchy Validation Scenarios (6)
     ✓ Permission Inheritance (2)
       ✓ should validate role hierarchy levels
       ✓ should ensure higher roles have more permissions than lower roles
     ✓ System vs Tenant Role Separation (2)
       ✓ should separate system-level and tenant-level permissions
       ✓ should validate system role permissions
     ✓ Multi-Role User Scenarios (2)
       ✓ should handle users with multiple roles correctly
       ✓ should prioritize highest role level for admin checks
 ✓ src/components/__tests__/roles/PermissionSelector.test.tsx (8) 3964ms
   ✓ PermissionSelector (8) 3964ms
     ✓ should render loading state initially
     ✓ should render permissions after loading 596ms
     ✓ should show search box when showSearch is true
     ✓ should call onChange when permission is selected 1827ms
     ✓ should select all permissions in category when category checkbox is clicked 1064ms
     ✓ should show selected count
     ✓ should be disabled when disabled prop is true
     ✓ should handle API error gracefully
 ✓ src/test/scenarios/multi-tenant-isolation.test.tsx (6)
   ✓ Multi-Tenant Permission Isolation Scenarios (6)
     ✓ Tenant Data Isolation (2)
       ✓ should isolate tenant data access
       ✓ should prevent cross-tenant role assignment
     ✓ Tenant-Scoped Permissions (2)
       ✓ should scope permissions to specific tenants
       ✓ should allow system admins to access multiple tenants
     ✓ Tenant Context Switching (2)
       ✓ should handle tenant context switching for multi-tenant users
       ✓ should restrict tenant switching for single-tenant users
 ✓ src/components/__tests__/routes/AppRoutes.test.tsx (6)
   ✓ AppRoutes (6)
     ✓ renders role details on /roles/:id route
     ✓ renders role editor on /roles/:id/edit route
     ✓ renders role editor on /roles/new route
     ✓ renders role list on /roles route
     ✓ redirects root to dashboard
     ✓ renders login form on /login route
 ✓ src/components/__tests__/roles/PermissionTreeView.test.tsx (5) 763ms
   ✓ PermissionTreeView (5) 762ms
     ✓ should render permission hierarchy 402ms
     ✓ should highlight selected permissions
     ✓ should show search when enabled
     ✓ should handle permission click when callback provided
     ✓ should expand categories with selected permissions
 ✓ src/components/common/__tests__/ErrorBoundary.test.tsx (6) 1318ms
   ✓ ErrorBoundary (6) 1318ms
     ✓ renders children when no error occurs
     ✓ renders error UI when error occurs 458ms
     ✓ shows page-level error UI when level is page 406ms
     ✓ calls onError callback when error occurs
     ✓ shows error details when details button is clicked 405ms
     ✓ renders custom fallback when provided

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Failed Tests 2 ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

 FAIL  src/components/auth/__tests__/ChangePasswordForm.test.tsx > ChangePasswordForm > prevents submission with empty fields
 FAIL  src/components/auth/__tests__/ChangePasswordForm.test.tsx > ChangePasswordForm > prevents submission with empty fields
TestingLibraryElementError: Found multiple elements with the text: Change Password

Here are the matching elements:

Ignored nodes: comments, script, style
<h1
  class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
  style="--Typography-textAlign: center;"
>
  Change Password
</h1>

Ignored nodes: comments, script, style
<button
  class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeMedium MuiButton-containedSizeMedium MuiButton-colorPrimary MuiButton-fullWidth MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeMedium MuiButton-containedSizeMedium MuiButton-colorPrimary MuiButton-fullWidth css-3yo4hs-MuiButtonBase-root-MuiButton-root"
  tabindex="0"
  type="submit"
>
  Change Password
  <span
    class="MuiTouchRipple-root css-r3djoj-MuiTouchRipple-root"
  />
</button>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body
  style=""
>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1u3hgzx-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Change Password
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body2 MuiTypography-alignCenter css-t3ijqe-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Enter your current password and choose a new secure password.
            </p>
            <form
              class="MuiBox-root css-164r41r"
            >
              <div
                class="MuiFormControl-root MuiFormControl-marginNormal MuiFormControl-fullWidth MuiTextField-root css-16bevx5-MuiFormControl-root-MuiTextField-root"
              >
                <label
                  class="MuiFormLabel-root MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-outlined MuiFormLabel-colorPrimary Mui-required MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-outlined css-19qnlrw-MuiFormLabel-root-MuiInputLabel-root"
                  data-shrink="false"
                  for="currentPassword"
                  id="currentPassword-label"
                >
                  Current Password
                  <span
                    aria-hidden="true"
                    class="MuiFormLabel-asterisk MuiInputLabel-asterisk css-1ljffdk-MuiFormLabel-asterisk"
                  >
                     
                    *
                  </span>
                </label>
                <div
                  class="MuiInputBase-root MuiOutlinedInput-root MuiInputBase-colorPrimary MuiInputBase-fullWidth MuiInputBase-formControl MuiInputBase-adornedEnd css-1n04w30-MuiInputBase-root-MuiOutlinedInput-root"
                >
                  <input
                    aria-invalid="false"
                    autocomplete="current-password"
                    class="MuiInputBase-input MuiOutlinedInput-input MuiInputBase-inputAdornedEnd css-1dune0f-MuiInputBase-input-MuiOutlinedInput-input"
                    id="currentPassword"
                    name="currentPassword"
                    required=""
                    type="password"
                    value=""
                  />
                  <div
                    class="MuiInputAdornment-root MuiInputAdornment-positionEnd MuiInputAdornment-outlined MuiInputAdornment-sizeMedium css-elo8k2-MuiInputAdornment-root"
                  >
                    <button
                      aria-label="Show password"
                      class="MuiButtonBase-root MuiIconButton-root MuiIconButton-edgeEnd MuiIconButton-sizeMedium css-1ysp02-MuiButtonBase-root-MuiIconButton-root"
                      tabindex="0"
                      type="button"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="VisibilityIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5M12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5m0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3"
                        />
                      </svg>
                    </button>
                  </div>
                  <fieldset
                    aria-hidden="true"
                    class="MuiOutlinedInput-notchedOutline css-18p5xg2-MuiNotchedOutlined-root-MuiOutlinedInput-notchedOutline"
                  >
                    <legend
                      class="css-1n64csd-MuiNotchedOutlined-root"
                    >
                      <span>
                        Current Password
                         
                        *
                      </span>
                    </legend>
                  </fieldset>
                </div>
              </div>
              <div
                class="MuiFormControl-root MuiFormControl-marginNormal MuiFormControl-fullWidth MuiTextField-root css-16bevx5-MuiFormControl-root-MuiTextField-root"
              >
                <label
                  class="MuiFormLabel-root MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-outlined MuiFormLabel-colorPrimary Mui-required MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-outlined css-19qnlrw-MuiFormLabel-root-MuiInputLabel-root"
                  data-shrink="false"
                  for="newPassword"
                  id="newPassword-label"
               ...
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/components/auth/__tests__/ChangePasswordForm.test.tsx:65:19
     63|
     64|     // Also verify we're still on the form page (not success page)
     65|     expect(screen.getByText('Change Password')).toBeInTheDocument()
       |                   ^
     66|     expect(screen.queryByText(/password changed/i)).not.toBeInTheDocument()
     67|   })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[1/4]⎯

 FAIL  src/components/auth/__tests__/ChangePasswordForm.test.tsx > ChangePasswordForm > prevents submission with weak password
 FAIL  src/components/auth/__tests__/ChangePasswordForm.test.tsx > ChangePasswordForm > prevents submission with weak password
TestingLibraryElementError: Found multiple elements with the text: Change Password

Here are the matching elements:

Ignored nodes: comments, script, style
<h1
  class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
  style="--Typography-textAlign: center;"
>
  Change Password
</h1>

Ignored nodes: comments, script, style
<button
  class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeMedium MuiButton-containedSizeMedium MuiButton-colorPrimary MuiButton-fullWidth MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeMedium MuiButton-containedSizeMedium MuiButton-colorPrimary MuiButton-fullWidth css-3yo4hs-MuiButtonBase-root-MuiButton-root"
  tabindex="0"
  type="submit"
>
  Change Password
  <span
    class="MuiTouchRipple-root css-r3djoj-MuiTouchRipple-root"
  />
</button>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body
  style=""
>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1u3hgzx-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Change Password
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body2 MuiTypography-alignCenter css-t3ijqe-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Enter your current password and choose a new secure password.
            </p>
            <form
              class="MuiBox-root css-164r41r"
            >
              <div
                class="MuiFormControl-root MuiFormControl-marginNormal MuiFormControl-fullWidth MuiTextField-root css-16bevx5-MuiFormControl-root-MuiTextField-root"
              >
                <label
                  class="MuiFormLabel-root MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-shrink MuiInputLabel-outlined MuiFormLabel-colorPrimary MuiFormLabel-filled Mui-required MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-shrink MuiInputLabel-outlined css-113d811-MuiFormLabel-root-MuiInputLabel-root"
                  data-shrink="true"
                  for="currentPassword"
                  id="currentPassword-label"
                >
                  Current Password
                  <span
                    aria-hidden="true"
                    class="MuiFormLabel-asterisk MuiInputLabel-asterisk css-1ljffdk-MuiFormLabel-asterisk"
                  >
                     
                    *
                  </span>
                </label>
                <div
                  class="MuiInputBase-root MuiOutlinedInput-root MuiInputBase-colorPrimary MuiInputBase-fullWidth MuiInputBase-formControl MuiInputBase-adornedEnd css-1n04w30-MuiInputBase-root-MuiOutlinedInput-root"
                >
                  <input
                    aria-invalid="false"
                    autocomplete="current-password"
                    class="MuiInputBase-input MuiOutlinedInput-input MuiInputBase-inputAdornedEnd css-1dune0f-MuiInputBase-input-MuiOutlinedInput-input"
                    id="currentPassword"
                    name="currentPassword"
                    required=""
                    type="password"
                    value="ValidPassword123!"
                  />
                  <div
                    class="MuiInputAdornment-root MuiInputAdornment-positionEnd MuiInputAdornment-outlined MuiInputAdornment-sizeMedium css-elo8k2-MuiInputAdornment-root"
                  >
                    <button
                      aria-label="Show password"
                      class="MuiButtonBase-root MuiIconButton-root MuiIconButton-edgeEnd MuiIconButton-sizeMedium css-1ysp02-MuiButtonBase-root-MuiIconButton-root"
                      tabindex="0"
                      type="button"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="VisibilityIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 4.5C7 4.5 2.73 7.61 1 12c1.73 4.39 6 7.5 11 7.5s9.27-3.11 11-7.5c-1.73-4.39-6-7.5-11-7.5M12 17c-2.76 0-5-2.24-5-5s2.24-5 5-5 5 2.24 5 5-2.24 5-5 5m0-8c-1.66 0-3 1.34-3 3s1.34 3 3 3 3-1.34 3-3-1.34-3-3-3"
                        />
                      </svg>
                    </button>
                  </div>
                  <fieldset
                    aria-hidden="true"
                    class="MuiOutlinedInput-notchedOutline css-18p5xg2-MuiNotchedOutlined-root-MuiOutlinedInput-notchedOutline"
                  >
                    <legend
                      class="css-ex8a5f-MuiNotchedOutlined-root"
                    >
                      <span>
                        Current Password
                         
                        *
                      </span>
                    </legend>
                  </fieldset>
                </div>
              </div>
              <div
                class="MuiFormControl-root MuiFormControl-marginNormal MuiFormControl-fullWidth MuiTextField-root css-16bevx5-MuiFormControl-root-MuiTextField-root"
              >
                <label
                  class="MuiFormLabel-root MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-shrink MuiInputLabel-outlined MuiFormLabel-colorPrimary MuiFormLabel-filled Mui-required MuiInputLabel-root MuiInputLabel-formControl MuiInputLabel-animated MuiInputLabel-shrink MuiInputLabel-outlined css-113d811-MuiFormLabel-root-MuiInputLabel-root"
                  data-shrink="true"...
 ❯ Object.getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/react/node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/components/auth/__tests__/ChangePasswordForm.test.tsx:194:19
    192|     })
    193|
    194|     expect(screen.getByText('Change Password')).toBeInTheDocument()
       |                   ^
    195|     expect(screen.queryByText(/password changed/i)).not.toBeInTheDocument()
    196|   })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[2/4]⎯

 Test Files  1 failed | 15 passed (16)
      Tests  2 failed | 133 passed (135)
   Start at  16:48:16
   Duration  43.14s (transform 573ms, setup 460ms, collect 5.56s, tests 36.23s, environment 445ms, prepare 81ms)
