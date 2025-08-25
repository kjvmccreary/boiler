    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
<empty line>
stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
<empty line>
stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
<empty line>
stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
<empty line>
stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
<empty line>
stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/components/__tests__/roles/RoleList.test.tsx > RoleList > shows loading state initially
🔄 RoleList: Loading roles for tenant: Test Tenant
🔍 RoleList: fetchRoles called { page: 1, pageSize: 10, search: '', tenant: 'Test Tenant' }
🔍 RoleService: getRoles called with params: { page: 1, pageSize: 10, searchTerm: undefined }
🔍 RoleService: Making request to: /api/roles?page=1&pageSize=10
🚨 API REQUEST (NO AUTH): GET /api/roles?page=1&pageSize=10 - No token found!

stderr | src/components/__tests__/roles/RoleList.test.tsx
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/roles?page=1&pageSize=10

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stderr | VirtualConsole.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\virtual-console.js:29:45)
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined

stderr | src\services\api.client.ts:65:17
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error

stderr | RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:64:15)
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stderr | RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:68:17)
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ Error details: { message: 'Network Error', name: 'AxiosError' }

stderr | fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:97:15)
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stderr | VirtualConsole.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\virtual-console.js:29:45)
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at Socket.socketErrorListener (node:_http_client:518:5)
    at Socket.emit (node:events:518:28)
    at emitErrorNT (node:internal/streams/destroy:170:8)
    at emitErrorCloseNT (node:internal/streams/destroy:129:3) undefined
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined

stderr | src\services\api.client.ts:65:17
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error
🚨 API ERROR: /api/roles?page=1&pageSize=10 undefined Network Error

stderr | RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:64:15)
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ RoleService: getRoles failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stderr | RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:68:17)
❌ Error details: { message: 'Network Error', name: 'AxiosError' }
❌ Error details: { message: 'Network Error', name: 'AxiosError' }

stderr | fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:97:15)
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
❌ RoleList: Failed to fetch roles: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at RoleService.getRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\role.service.ts:48:24)
    at fetchRoles (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\components\roles\RoleList.tsx:80:22) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/roles?page=1&pageSize=10',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}

stdout | src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Multiple Tenants > should call onTenantSelected when tenant is selected and continue clicked
🏢 TenantSelector: Tenant selected: 1

stdout | src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Single Tenant Auto-Selection > should call onTenantSelected immediately for single tenant
🏢 TenantSelector: Tenant selected: 1

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:20:13)
🔍 API CLIENT: Creating axios instance with baseURL: empty (using Vite proxy)

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:21:13)
🔍 API CLIENT: Expected proxy config:

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:22:13)
  - /api/auth/* → AuthService (port 7001)

stdout | new ApiClient (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:23:13)
  - /api/* → UserService (port 7002)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 3
🔍 TenantService: Calling API endpoint: /api/users/3/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/3/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/3/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
The current testing environment is not configured to support act(...)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should allow API calls for users with correct permissions
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/3/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should reject API calls for users without permissions
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 6
🔍 TenantService: Calling API endpoint: /api/users/6/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/6/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should reject API calls for users without permissions
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/6/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should reject API calls for users without permissions
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > API Call Authorization > should reject API calls for users without permissions
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/6/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/6/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/6/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should filter user data based on role permissions
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 3
🔍 TenantService: Calling API endpoint: /api/users/3/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/3/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should filter user data based on role permissions
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/3/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 6
🔍 TenantService: Calling API endpoint: /api/users/6/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/6/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/6/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/3/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)
Failed to fetch users: TypeError: fetch failed
    at node:internal/deps/undici/undici:13510:13
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at fetchUsers (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\test\scenarios\api-permission-integration.test.tsx:177:30) {
  [cause]: AggregateError:
      at internalConnectMultiple (node:net:1134:18)
      at afterConnectMultiple (node:net:1715:7) {
    code: 'ECONNREFUSED',
    [errors]: [ [Error], [Error] ]
  }
}

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Role-Based Data Filtering > should limit data for lower privilege users
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/6/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/6/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/6/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle 403 Forbidden responses gracefully
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 5
🔍 TenantService: Calling API endpoint: /api/users/5/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/5/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle 403 Forbidden responses gracefully
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/5/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle 403 Forbidden responses gracefully
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle 403 Forbidden responses gracefully
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/5/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/5/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/5/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 3
🔍 TenantService: Calling API endpoint: /api/users/3/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/3/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/3/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
[MSW] Warning: intercepted a request without a matching request handler:

  • DELETE /api/users/1

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/3/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
🏢 API CLIENT: Tenant context cleared (using JWT for tenant context)
🏢 TenantContext: Loading tenants for user: 3
🔍 TenantService: Calling API endpoint: /api/users/3/tenants
🚨 API REQUEST (NO AUTH): GET /api/users/3/tenants - No token found!

stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
[MSW] Warning: intercepted a request without a matching request handler:

  • GET /api/users/3/tenants

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests

stdout | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
<empty line>
stderr | src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
Error: AggregateError
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:63:19)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Request.emit (node:events:530:35)
    at ClientRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\http-request.js:127:14)
    at ClientRequest.emit (node:events:518:28)
    at emitErrorEvent (node:_http_client:104:11)
    at MockHttpSocket.socketErrorListener (node:_http_client:518:5)
    at MockHttpSocket.emit (node:events:530:35)
    at MockHttpSocket.emit (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:160:12)
    at Socket.<anonymous> (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/@mswjs/interceptors/src/interceptors/ClientRequest/MockHttpSocket.ts:293:14) undefined
🚨 API ERROR: /api/users/3/tenants undefined Network Error
🏢 TenantService: API call failed: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error loading tenants: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24) {
  code: 'ERR_NETWORK',
  config: {
    transitional: {
      silentJSONParsing: true,
      forcedJSONParsing: true,
      clarifyTimeoutError: false
    },
    adapter: [ 'xhr', 'http', 'fetch' ],
    transformRequest: [ [Function: transformRequest] ],
    transformResponse: [ [Function: transformResponse] ],
    timeout: 10000,
    xsrfCookieName: 'XSRF-TOKEN',
    xsrfHeaderName: 'X-XSRF-TOKEN',
    maxContentLength: -1,
    maxBodyLength: -1,
    env: { FormData: [Function [FormData]], Blob: [class Blob] },
    validateStatus: [Function: validateStatus],
    headers: Object [AxiosHeaders] {
      Accept: 'application/json, text/plain, */*',
      'Content-Type': 'application/json'
    },
    baseURL: '',
    method: 'get',
    url: '/api/users/3/tenants',
    allowAbsoluteUrls: true,
    data: undefined
  },
  request: XMLHttpRequest {
    open: [Function: open],
    setRequestHeader: [Function: setRequestHeader],
    send: [Function: send],
    abort: [Function: abort],
    getResponseHeader: [Function: getResponseHeader],
    getAllResponseHeaders: [Function: getAllResponseHeaders],
    overrideMimeType: [Function: overrideMimeType],
    onreadystatechange: [Getter/Setter],
    readyState: [Getter],
    timeout: [Getter/Setter],
    withCredentials: [Getter/Setter],
    upload: XMLHttpRequestUpload {},
    responseURL: [Getter],
    status: [Getter],
    statusText: [Getter],
    responseType: [Getter/Setter],
    response: [Getter],
    responseText: [Getter],
    responseXML: [Getter],
    UNSENT: 0,
    OPENED: 1,
    HEADERS_RECEIVED: 2,
    LOADING: 3,
    DONE: 4,
    [Symbol(SameObject caches)]: [Object: null prototype] { upload: XMLHttpRequestUpload {} }
  }
}
🏢 TenantContext: Error message: Network Error
🏢 TenantContext: Error stack: AxiosError: Network Error
    at XMLHttpRequest.handleError (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/adapters/xhr.js:110:14)
    at XMLHttpRequest.invokeTheCallbackFunction (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\generated\EventHandlerNonNull.js:14:28)
    at XMLHttpRequest.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\create-event-accessor.js:35:32)
    at innerInvokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:350:25)
    at invokeEventListeners (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:286:3)
    at XMLHttpRequestImpl._dispatch (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\events\EventTarget-impl.js:233:9)
    at fireAnEvent (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\helpers\events.js:18:36)
    at requestErrorSteps (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:131:3)
    at Object.dispatchError (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\xhr-utils.js:60:3)
    at Request.<anonymous> (C:\Users\mccre\dev\boiler\src\frontend\react-app\node_modules\jsdom\lib\jsdom\living\xhr\XMLHttpRequest-impl.js:655:18)
    at Axios.request (file:///C:/Users/mccre/dev/boiler/src/frontend/react-app/node_modules/axios/lib/core/Axios.js:45:41)
    at processTicksAndRejections (node:internal/process/task_queues:105:5)
    at ApiClient.get (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\api.client.ts:174:12)
    at TenantService.getUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\services\tenant.service.ts:21:24)
    at loadUserTenants (C:\Users\mccre\dev\boiler\src\frontend\react-app\src\contexts\TenantContext.tsx:150:24)
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)
[MSW] Warning: intercepted a request without a matching request handler:

  • DELETE /api/users/1

If you still wish to intercept this unhandled request, please create a request handler for it.
Read more: https://mswjs.io/docs/http/intercepting-requests
The current testing environment is not configured to support act(...)
The current testing environment is not configured to support act(...)

 ❯ src/contexts/__tests__/TenantContext.test.tsx (12) 74443ms
   ❯ TenantContext - Enhanced Tests (12) 74440ms
     ✓ Single Tenant Auto-Selection (2)
       ✓ should load and auto-select single tenant
       ✓ should call selectTenant API for single tenant
     ❯ Multiple Tenants (2) 18077ms
       × should show tenant selector for multiple tenants (retry x1) 16035ms
       × should complete tenant selection for multiple tenants (retry x1) 2042ms
     ❯ Tenant Switching (2) 4074ms
       × should switch between tenants (retry x1) 2029ms
       × should set redirect flag after successful switch (retry x1) 2044ms
     ❯ Error Handling (3) 34126ms
       × should handle getUserTenants API failure (retry x1) 16044ms
       × should handle selectTenant API failure (retry x1) 16035ms
       × should handle switchTenant API failure (retry x1) 2044ms
     ❯ Loading States (1)
       × should show loading state while fetching tenants (retry x1)
     ❯ JWT Token Integration (1) 2024ms
       × should handle page refresh with existing JWT tenant (retry x1) 2024ms
     ❯ No Tenants Scenario (1) 16059ms
       × should handle user with no tenant access (retry x1) 16059ms
 ❯ src/test/scenarios/api-permission-integration.test.tsx (6) 2336ms
   ❯ API Permission Integration Scenarios (6) 2334ms
     ✓ API Call Authorization (2)
       ✓ should allow API calls for users with correct permissions
       ✓ should reject API calls for users without permissions
     ✓ Role-Based Data Filtering (2)
       ✓ should filter user data based on role permissions
       ✓ should limit data for lower privilege users
     ❯ Permission-Based Error Handling (2) 2183ms
       ✓ should handle 403 Forbidden responses gracefully
       × should handle successful operations for authorized users (retry x1) 2118ms
 ❯ src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx (5) 32168ms
   ❯ Two-Phase Authentication Flow (5) 32167ms
     ✓ should complete Phase 1: Login without tenant context 1085ms
     × should complete Phase 2: Tenant selection (retry x1) 10078ms
     × should handle single tenant auto-selection (retry x1) 10090ms
     ✓ should handle authentication errors 873ms
     × should handle tenant loading errors (retry x1) 10041ms
 ❯ src/components/auth/__tests__/TenantSelector.test.tsx (11) 1164ms
   ❯ TenantSelector - Enhanced Tests (11) 1164ms
     ❯ Multiple Tenants (3) 420ms
       × should display available tenants (retry x1)
       ✓ should call onTenantSelected when tenant is selected and continue clicked 342ms
       ✓ should show subscription plan badges
     ✓ Single Tenant Auto-Selection (2)
       ✓ should auto-select single tenant and show continue button
       ✓ should call onTenantSelected immediately for single tenant
     ❯ Loading and Error States (4)
       ✓ should show loading state
       ✓ should show error state
       ✓ should show no tenants message
       × should disable continue button when no tenant selected (retry x1)
     ❯ Selection Interaction (2) 356ms
       × should enable continue button after tenant selection (retry x1)
       × should show visual selection feedback (retry x1)
 ❯ src/components/__tests__/roles/RoleList.test.tsx (2) 16059ms
   ❯ RoleList (2) 16058ms
     × renders roles list correctly (retry x1) 16031ms
     ✓ shows loading state initially

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯ Failed Tests 19 ⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Multiple Tenants > should show tenant selector for multiple tenants
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Multiple Tenants > should show tenant selector for multiple tenants
TestingLibraryElementError: Found multiple elements by: [data-testid="available-count"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="available-count"
        >
          2
        </div>
        <div
          data-testid="tenant-selector"
        >
          Selector shown
        </div>
        <div>
          <div
            data-testid="current-tenant"
          >
            No tenant
          </div>
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="show-selector"
          >
            true
          </div>
          <div
            data-testid="should-redirect"
          >
            false
          </div>
          <button
            data-testid="switch-tenant"
          >
            Switch Tenant
          </button>
          <button
            data-testid="complete-selection"
          >
            Complete Selection
          </button>
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="tenant-selector"
          >
            Selector shown
          </div>
          <div>
            <div
              data-testid="current-tenant"
            >
              No tenant
            </div>
            <div
              data-testid="available-count"
            >
              2
            </div>
            <div
              data-testid="show-selector"
            >
              true
            </div>
            <div
              data-testid="should-redirect"
            >
              false
            </div>
            <button
              data-testid="switch-tenant"
            >
              Switch Tenant
            </button>
            <button
              data-testid="complete-selection"
            >
              Complete Selection
            </button>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:293:13
    291|       render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
    292|
    293|       await waitFor(() => {
       |             ^
    294|         expect(screen.getByTestId('available-count')).toHaveTextContent('2')
    295|         expect(screen.getByTestId('show-selector')).toHaveTextContent('true')

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[1/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Multiple Tenants > should complete tenant selection for multiple tenants
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Multiple Tenants > should complete tenant selection for multiple tenants
TestingLibraryElementError: Found multiple elements by: [data-testid="available-count"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="available-count"
        >
          2
        </div>
        <div
          data-testid="tenant-selector"
        >
          Selector shown
        </div>
        <div>
          <div
            data-testid="current-tenant"
          >
            No tenant
          </div>
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="show-selector"
          >
            true
          </div>
          <div
            data-testid="should-redirect"
          >
            false
          </div>
          <button
            data-testid="switch-tenant"
          >
            Switch Tenant
          </button>
          <button
            data-testid="complete-selection"
          >
            Complete Selection
          </button>
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="tenant-selector"
          >
            Selector shown
          </div>
          <div>
            <div
              data-testid="current-tenant"
            >
              No tenant
            </div>
            <div
              data-testid="available-count"
            >
              2
            </div>
            <div
              data-testid="show-selector"
            >
              true
            </div>
            <div
              data-testid="should-redirect"
            >
              false
            </div>
            <button
              data-testid="switch-tenant"
            >
              Switch Tenant
            </button>
            <button
              data-testid="complete-selection"
            >
              Complete Selection
            </button>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:314:13
    312|       render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
    313|
    314|       await waitFor(() => {
       |             ^
    315|         expect(screen.getByTestId('available-count')).toHaveTextContent('2')
    316|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[2/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Tenant Switching > should switch between tenants
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Tenant Switching > should switch between tenants
TestingLibraryElementError: Found multiple elements by: [data-testid="available-count"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="available-count"
        >
          2
        </div>
        <div
          data-testid="tenant-selector"
        >
          Selector shown
        </div>
        <div>
          <div
            data-testid="current-tenant"
          >
            No tenant
          </div>
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="show-selector"
          >
            true
          </div>
          <div
            data-testid="should-redirect"
          >
            false
          </div>
          <button
            data-testid="switch-tenant"
          >
            Switch Tenant
          </button>
          <button
            data-testid="complete-selection"
          >
            Complete Selection
          </button>
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="tenant-selector"
          >
            Selector shown
          </div>
          <div>
            <div
              data-testid="current-tenant"
            >
              No tenant
            </div>
            <div
              data-testid="available-count"
            >
              2
            </div>
            <div
              data-testid="show-selector"
            >
              true
            </div>
            <div
              data-testid="should-redirect"
            >
              false
            </div>
            <button
              data-testid="switch-tenant"
            >
              Switch Tenant
            </button>
            <button
              data-testid="complete-selection"
            >
              Complete Selection
            </button>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:349:13
    347|       render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
    348|
    349|       await waitFor(() => {
       |             ^
    350|         expect(screen.getByTestId('available-count')).toHaveTextContent('2')
    351|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[3/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Tenant Switching > should set redirect flag after successful switch
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Tenant Switching > should set redirect flag after successful switch
TestingLibraryElementError: Found multiple elements by: [data-testid="available-count"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="available-count"
        >
          2
        </div>
        <div
          data-testid="tenant-selector"
        >
          Selector shown
        </div>
        <div>
          <div
            data-testid="current-tenant"
          >
            No tenant
          </div>
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="show-selector"
          >
            true
          </div>
          <div
            data-testid="should-redirect"
          >
            false
          </div>
          <button
            data-testid="switch-tenant"
          >
            Switch Tenant
          </button>
          <button
            data-testid="complete-selection"
          >
            Complete Selection
          </button>
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="tenant-selector"
          >
            Selector shown
          </div>
          <div>
            <div
              data-testid="current-tenant"
            >
              No tenant
            </div>
            <div
              data-testid="available-count"
            >
              2
            </div>
            <div
              data-testid="show-selector"
            >
              true
            </div>
            <div
              data-testid="should-redirect"
            >
              false
            </div>
            <button
              data-testid="switch-tenant"
            >
              Switch Tenant
            </button>
            <button
              data-testid="complete-selection"
            >
              Complete Selection
            </button>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:365:13
    363|       render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
    364|
    365|       await waitFor(() => {
       |             ^
    366|         expect(screen.getByTestId('available-count')).toHaveTextContent('2')
    367|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[4/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Error Handling > should handle getUserTenants API failure
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Error Handling > should handle getUserTenants API failure
TestingLibraryElementError: Found multiple elements by: [data-testid="error"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="error"
>
  Failed to load tenants
</div>

Ignored nodes: comments, script, style
<div
  data-testid="error"
>
  Failed to load tenants
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="error"
        >
          Failed to load tenants
        </div>
        <div
          data-testid="available-count"
        >
          0
        </div>
        <div
          data-testid="error"
        >
          Failed to load tenants
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="error"
          >
            Failed to load tenants
          </div>
          <div
            data-testid="available-count"
          >
            0
          </div>
          <div
            data-testid="error"
          >
            Failed to load tenants
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:382:13
    380|       render(<TestWrapper mockError="Failed to load tenants" />)
    381|
    382|       await waitFor(() => {
       |             ^
    383|         expect(screen.getByTestId('error')).toHaveTextContent('Failed to load tenants')
    384|       }, { timeout: 8000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[5/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Error Handling > should handle selectTenant API failure
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Error Handling > should handle selectTenant API failure
TestingLibraryElementError: Found multiple elements by: [data-testid="error"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="error"
>
  Failed to select tenant
</div>

Ignored nodes: comments, script, style
<div
  data-testid="error"
>
  Failed to select tenant
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="error"
        >
          Failed to select tenant
        </div>
        <div
          data-testid="available-count"
        >
          1
        </div>
        <div
          data-testid="error"
        >
          Failed to select tenant
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="error"
          >
            Failed to select tenant
          </div>
          <div
            data-testid="available-count"
          >
            1
          </div>
          <div
            data-testid="error"
          >
            Failed to select tenant
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:394:13
    392|       render(<TestWrapper />)
    393|
    394|       await waitFor(() => {
       |             ^
    395|         expect(screen.getByTestId('error')).toHaveTextContent('Failed to select tenant')
    396|       }, { timeout: 8000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[6/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Error Handling > should handle switchTenant API failure
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Error Handling > should handle switchTenant API failure
TestingLibraryElementError: Found multiple elements by: [data-testid="available-count"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="available-count"
        >
          2
        </div>
        <div
          data-testid="tenant-selector"
        >
          Selector shown
        </div>
        <div>
          <div
            data-testid="current-tenant"
          >
            No tenant
          </div>
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="show-selector"
          >
            true
          </div>
          <div
            data-testid="should-redirect"
          >
            false
          </div>
          <button
            data-testid="switch-tenant"
          >
            Switch Tenant
          </button>
          <button
            data-testid="complete-selection"
          >
            Complete Selection
          </button>
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="tenant-selector"
          >
            Selector shown
          </div>
          <div>
            <div
              data-testid="current-tenant"
            >
              No tenant
            </div>
            <div
              data-testid="available-count"
            >
              2
            </div>
            <div
              data-testid="show-selector"
            >
              true
            </div>
            <div
              data-testid="should-redirect"
            >
              false
            </div>
            <button
              data-testid="switch-tenant"
            >
              Switch Tenant
            </button>
            <button
              data-testid="complete-selection"
            >
              Complete Selection
            </button>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:406:13
    404|       render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
    405|
    406|       await waitFor(() => {
       |             ^
    407|         expect(screen.getByTestId('available-count')).toHaveTextContent('2')
    408|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[7/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Loading States > should show loading state while fetching tenants
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > Loading States > should show loading state while fetching tenants
TestingLibraryElementError: Found multiple elements with the text: Loading tenants...

Here are the matching elements:

Ignored nodes: comments, script, style
<div>
  Loading tenants...
</div>

Ignored nodes: comments, script, style
<div>
  Loading tenants...
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div>
          Loading tenants...
        </div>
        <div
          data-testid="available-count"
        >
          0
        </div>
        <div>
          Loading tenants...
        </div>
      </div>
    </div>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/contexts/__tests__/TenantContext.test.tsx:432:21
    430|
    431|       // Should show loading initially
    432|       expect(screen.getByText('Loading tenants...')).toBeInTheDocument()
       |                     ^
    433|
    434|       // Then should show tenant after loading

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[8/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > JWT Token Integration > should handle page refresh with existing JWT tenant
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > JWT Token Integration > should handle page refresh with existing JWT tenant
TestingLibraryElementError: Found multiple elements by: [data-testid="available-count"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

Ignored nodes: comments, script, style
<div
  data-testid="available-count"
>
  2
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="available-count"
        >
          2
        </div>
        <div
          data-testid="tenant-selector"
        >
          Selector shown
        </div>
        <div>
          <div
            data-testid="current-tenant"
          >
            No tenant
          </div>
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="show-selector"
          >
            true
          </div>
          <div
            data-testid="should-redirect"
          >
            false
          </div>
          <button
            data-testid="switch-tenant"
          >
            Switch Tenant
          </button>
          <button
            data-testid="complete-selection"
          >
            Complete Selection
          </button>
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="available-count"
          >
            2
          </div>
          <div
            data-testid="tenant-selector"
          >
            Selector shown
          </div>
          <div>
            <div
              data-testid="current-tenant"
            >
              No tenant
            </div>
            <div
              data-testid="available-count"
            >
              2
            </div>
            <div
              data-testid="show-selector"
            >
              true
            </div>
            <div
              data-testid="should-redirect"
            >
              false
            </div>
            <button
              data-testid="switch-tenant"
            >
              Switch Tenant
            </button>
            <button
              data-testid="complete-selection"
            >
              Complete Selection
            </button>
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:454:13
    452|       render(<TestWrapper mockUserTenants={mockMultipleTenants} />)
    453|
    454|       await waitFor(() => {
       |             ^
    455|         expect(screen.getByTestId('available-count')).toHaveTextContent('2')
    456|       })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[9/38]⎯

 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > No Tenants Scenario > should handle user with no tenant access
 FAIL  src/contexts/__tests__/TenantContext.test.tsx > TenantContext - Enhanced Tests > No Tenants Scenario > should handle user with no tenant access
TestingLibraryElementError: Found multiple elements by: [data-testid="error"]

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  data-testid="error"
>
  No tenants found for user
</div>

Ignored nodes: comments, script, style
<div
  data-testid="error"
>
  No tenants found for user
</div>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      data-testid="auth-provider"
    >
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="error"
        >
          No tenants found for user
        </div>
        <div
          data-testid="available-count"
        >
          0
        </div>
        <div
          data-testid="error"
        >
          No tenants found for user
        </div>
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head />
  <body>
    <div>
      <div
        data-testid="auth-provider"
      >
        <div
          data-testid="auth-provider"
        >
          <div
            data-testid="error"
          >
            No tenants found for user
          </div>
          <div
            data-testid="available-count"
          >
            0
          </div>
          <div
            data-testid="error"
          >
            No tenants found for user
          </div>
        </div>
      </div>
    </div>
  </body>
</html>
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/contexts/__tests__/TenantContext.test.tsx:467:13
    465|       render(<TestWrapper mockUserTenants={[]} />)
    466|
    467|       await waitFor(() => {
       |             ^
    468|         expect(screen.getByTestId('available-count')).toHaveTextContent('0')
    469|         expect(screen.getByTestId('error')).toHaveTextContent('No tenants found for user')

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[10/38]⎯

 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
 FAIL  src/test/scenarios/api-permission-integration.test.tsx > API Permission Integration Scenarios > Permission-Based Error Handling > should handle successful operations for authorized users
TestingLibraryElementError: Unable to find an element by: [data-testid="success-message"]

Ignored nodes: comments, script, style
<body>
  <div>

    <div>
      <button
        data-testid="delete-btn"
      >
        Delete User
      </button>
      <div
        data-testid="error-message"
      >
        An unexpected error occurred
      </div>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>

      <div>
        <button
          data-testid="delete-btn"
        >
          Delete User
        </button>
        <div
          data-testid="error-message"
        >
          An unexpected error occurred
        </div>
      </div>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/test/scenarios/api-permission-integration.test.tsx:334:13
    332|       })
    333|
    334|       await waitFor(() => {
       |             ^
    335|         expect(screen.getByTestId('success-message')).toHaveTextContent(
    336|           'User deleted successfully'

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[11/38]⎯

 FAIL  src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx > Two-Phase Authentication Flow > should complete Phase 2: Tenant selection
 FAIL  src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx > Two-Phase Authentication Flow > should complete Phase 2: Tenant selection
TestingLibraryElementError: Unable to find an element with the text: Tenant 1. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Select Organization
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              You have access to multiple organizations. Please select one to continue.
            </p>
            <div
              class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation0 MuiAlert-root MuiAlert-colorWarning MuiAlert-standardWarning MuiAlert-standard css-1u6i1g1-MuiPaper-root-MuiAlert-root"
              role="alert"
              style="--Paper-shadow: none;"
            >
              <div
                class="MuiAlert-icon css-vab54s-MuiAlert-icon"
              >
                <svg
                  aria-hidden="true"
                  class="MuiSvgIcon-root MuiSvgIcon-fontSizeInherit css-1ckov0h-MuiSvgIcon-root"
                  data-testid="ReportProblemOutlinedIcon"
                  focusable="false"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z"
                  />
                </svg>
              </div>
              <div
                class="MuiAlert-message css-zioonp-MuiAlert-message"
              >
                You don't have access to any organizations. Please contact your administrator.
              </div>
            </div>
            <ul
              class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
            />
            <button
              class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
              disabled=""
              tabindex="-1"
              type="button"
            >
              Select Organization
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <main
        class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
      >
        <div
          class="MuiBox-root css-binzgt"
        >
          <div
            class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
            style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
          >
            <div
              class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
            >
              <h1
                class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
                style="--Typography-textAlign: center;"
              >
                Select Organization
              </h1>
              <p
                class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
                style="--Typography-textAlign: center;"
              >
                You have access to multiple organizations. Please select one to continue.
              </p>
              <div
                class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation0 MuiAlert-root MuiAlert-colorWarning MuiAlert-standardWarning MuiAlert-standard css-1u6i1g1-MuiPaper-root-MuiAlert-root"
                role="alert"
                style="--Paper-shadow: none;"
              >
                <div
                  class="MuiAlert-icon css-vab54s-MuiAlert-icon"
                >
                  <svg
                    aria-hidden="true"
                    class="MuiSvgIcon-root MuiSvgIcon-fontSizeInherit css-1ckov0h-MuiSvgIcon-root"
                    data-testid="ReportProblemOutlinedIcon"
                    focusable="false"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z"
                    />
                  </svg>
                </div>
                <div
                  class="MuiAlert-message css-zioonp-MuiAlert-message"
                >
                  You don't have access to any organizations. Please contact your administrator.
                </div>
              </div>
              <ul
                class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
              />
              <button
                class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
                disabled=""
                tabindex="-1"
                type="button"
              >
                Select Organization
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx:227:11
    225|
    226|     // Wait for tenants to load and select one
    227|     await waitFor(() => {
       |           ^
    228|       expect(screen.getByText('Tenant 1')).toBeInTheDocument()
    229|     }, { timeout: 5000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[12/38]⎯

 FAIL  src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx > Two-Phase Authentication Flow > should handle single tenant auto-selection
 FAIL  src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx > Two-Phase Authentication Flow > should handle single tenant auto-selection
TestingLibraryElementError: Unable to find an element with the text: Continue. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Select Organization
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              You have access to multiple organizations. Please select one to continue.
            </p>
            <div
              class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation0 MuiAlert-root MuiAlert-colorWarning MuiAlert-standardWarning MuiAlert-standard css-1u6i1g1-MuiPaper-root-MuiAlert-root"
              role="alert"
              style="--Paper-shadow: none;"
            >
              <div
                class="MuiAlert-icon css-vab54s-MuiAlert-icon"
              >
                <svg
                  aria-hidden="true"
                  class="MuiSvgIcon-root MuiSvgIcon-fontSizeInherit css-1ckov0h-MuiSvgIcon-root"
                  data-testid="ReportProblemOutlinedIcon"
                  focusable="false"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z"
                  />
                </svg>
              </div>
              <div
                class="MuiAlert-message css-zioonp-MuiAlert-message"
              >
                You don't have access to any organizations. Please contact your administrator.
              </div>
            </div>
            <ul
              class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
            />
            <button
              class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
              disabled=""
              tabindex="-1"
              type="button"
            >
              Select Organization
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <main
        class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
      >
        <div
          class="MuiBox-root css-binzgt"
        >
          <div
            class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
            style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
          >
            <div
              class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
            >
              <h1
                class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
                style="--Typography-textAlign: center;"
              >
                Select Organization
              </h1>
              <p
                class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
                style="--Typography-textAlign: center;"
              >
                You have access to multiple organizations. Please select one to continue.
              </p>
              <div
                class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation0 MuiAlert-root MuiAlert-colorWarning MuiAlert-standardWarning MuiAlert-standard css-1u6i1g1-MuiPaper-root-MuiAlert-root"
                role="alert"
                style="--Paper-shadow: none;"
              >
                <div
                  class="MuiAlert-icon css-vab54s-MuiAlert-icon"
                >
                  <svg
                    aria-hidden="true"
                    class="MuiSvgIcon-root MuiSvgIcon-fontSizeInherit css-1ckov0h-MuiSvgIcon-root"
                    data-testid="ReportProblemOutlinedIcon"
                    focusable="false"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z"
                    />
                  </svg>
                </div>
                <div
                  class="MuiAlert-message css-zioonp-MuiAlert-message"
                >
                  You don't have access to any organizations. Please contact your administrator.
                </div>
              </div>
              <ul
                class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
              />
              <button
                class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
                disabled=""
                tabindex="-1"
                type="button"
              >
                Select Organization
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx:305:11
    303|
    304|     // Should auto-select and show continue
    305|     await waitFor(() => {
       |           ^
    306|       expect(screen.getByText('Continue')).toBeInTheDocument()
    307|     }, { timeout: 5000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[13/38]⎯

 FAIL  src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx > Two-Phase Authentication Flow > should handle tenant loading errors
 FAIL  src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx > Two-Phase Authentication Flow > should handle tenant loading errors
TestingLibraryElementError: Unable to find an element with the text: /failed to load tenants/i. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Select Organization
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              You have access to multiple organizations. Please select one to continue.
            </p>
            <div
              class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation0 MuiAlert-root MuiAlert-colorWarning MuiAlert-standardWarning MuiAlert-standard css-1u6i1g1-MuiPaper-root-MuiAlert-root"
              role="alert"
              style="--Paper-shadow: none;"
            >
              <div
                class="MuiAlert-icon css-vab54s-MuiAlert-icon"
              >
                <svg
                  aria-hidden="true"
                  class="MuiSvgIcon-root MuiSvgIcon-fontSizeInherit css-1ckov0h-MuiSvgIcon-root"
                  data-testid="ReportProblemOutlinedIcon"
                  focusable="false"
                  viewBox="0 0 24 24"
                >
                  <path
                    d="M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z"
                  />
                </svg>
              </div>
              <div
                class="MuiAlert-message css-zioonp-MuiAlert-message"
              >
                You don't have access to any organizations. Please contact your administrator.
              </div>
            </div>
            <ul
              class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
            />
            <button
              class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
              disabled=""
              tabindex="-1"
              type="button"
            >
              Select Organization
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <main
        class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
      >
        <div
          class="MuiBox-root css-binzgt"
        >
          <div
            class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
            style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
          >
            <div
              class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
            >
              <h1
                class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
                style="--Typography-textAlign: center;"
              >
                Select Organization
              </h1>
              <p
                class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
                style="--Typography-textAlign: center;"
              >
                You have access to multiple organizations. Please select one to continue.
              </p>
              <div
                class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation0 MuiAlert-root MuiAlert-colorWarning MuiAlert-standardWarning MuiAlert-standard css-1u6i1g1-MuiPaper-root-MuiAlert-root"
                role="alert"
                style="--Paper-shadow: none;"
              >
                <div
                  class="MuiAlert-icon css-vab54s-MuiAlert-icon"
                >
                  <svg
                    aria-hidden="true"
                    class="MuiSvgIcon-root MuiSvgIcon-fontSizeInherit css-1ckov0h-MuiSvgIcon-root"
                    data-testid="ReportProblemOutlinedIcon"
                    focusable="false"
                    viewBox="0 0 24 24"
                  >
                    <path
                      d="M12 5.99L19.53 19H4.47L12 5.99M12 2L1 21h22L12 2zm1 14h-2v2h2v-2zm0-6h-2v4h2v-4z"
                    />
                  </svg>
                </div>
                <div
                  class="MuiAlert-message css-zioonp-MuiAlert-message"
                >
                  You don't have access to any organizations. Please contact your administrator.
                </div>
              </div>
              <ul
                class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
              />
              <button
                class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
                disabled=""
                tabindex="-1"
                type="button"
              >
                Select Organization
              </button>
            </div>
          </div>
        </div>
      </main>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/auth/__tests__/TwoPhaseAuthFlow.test.tsx:351:11
    349|     )
    350|
    351|     await waitFor(() => {
       |           ^
    352|       expect(screen.getByText(/failed to load tenants/i)).toBeInTheDocument()
    353|     }, { timeout: 5000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[14/38]⎯

 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Multiple Tenants > should display available tenants
 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Multiple Tenants > should display available tenants
TestingLibraryElementError: Found multiple elements with the text: Select Organization

Here are the matching elements:

Ignored nodes: comments, script, style
<h1
  class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
  style="--Typography-textAlign: center;"
>
  Select Organization
</h1>

Ignored nodes: comments, script, style
<button
  class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
  disabled=""
  tabindex="-1"
  type="button"
>
  Select Organization
</button>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Select Organization
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              You have access to multiple organizations. Please select one to continue.
            </p>
            <ul
              class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
            >
              <li
                class="MuiListItem-root MuiListItem-gutters css-19f6boz-MuiListItem-root"
              >
                <div
                  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
                  role="button"
                  tabindex="0"
                >
                  <div
                    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
                  >
                    <div
                      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="DomainIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
                        />
                      </svg>
                    </div>
                  </div>
                  <div
                    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
                  >
                    <span
                      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
                    >
                      <div
                        class="MuiBox-root css-axw7ok"
                      >
                        Tenant One
                        <div
                          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
                        >
                          <span
                            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
                          >
                            Basic
                          </span>
                        </div>
                      </div>
                    </span>
                    <p
                      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
                    >
                      tenant1.test
                    </p>
                  </div>
                </div>
              </li>
              <li
                class="MuiListItem-root MuiListItem-gutters css-19f6boz-MuiListItem-root"
              >
                <div
                  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
                  role="button"
                  tabindex="0"
                >
                  <div
                    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
                  >
                    <div
                      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="DomainIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
                        />
                      </svg>
                    </div>
                  </div>
                  <div
                    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
                  >
                    <span
                      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
                    >
                      <div
                        class="MuiBox-root css-axw7ok"
                      >
                        Tenant Two
                        <div
                          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
                        >
                          <span
                            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
                          >
                            Pro
                          </span>
                        </div>
                      </div>
                    </span>
                    <p
                      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
                    >
                      tenant2.test
                    </p>
                  </div>
                </div>
              </li>
            </ul>
            <button
              class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
              disabled=""
              tabindex="-1"
              type="button"
            >
              Select Organization
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/components/auth/__tests__/TenantSelector.test.tsx:89:21
     87|       expect(screen.getByText('Tenant One')).toBeInTheDocument()
     88|       expect(screen.getByText('Tenant Two')).toBeInTheDocument()
     89|       expect(screen.getByText('Select Organization')).toBeInTheDocument()
       |                     ^
     90|     })
     91|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[15/38]⎯

 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Loading and Error States > should disable continue button when no tenant selected
 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Loading and Error States > should disable continue button when no tenant selected
TestingLibraryElementError: Found multiple elements with the role "button"

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
  role="button"
  tabindex="0"
>
  <div
    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
  >
    <div
      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
    >
      <svg
        aria-hidden="true"
        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
        data-testid="DomainIcon"
        focusable="false"
        viewBox="0 0 24 24"
      >
        <path
          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
        />
      </svg>
    </div>
  </div>
  <div
    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
  >
    <span
      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
    >
      <div
        class="MuiBox-root css-axw7ok"
      >
        Tenant One
        <div
          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
        >
          <span
            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
          >
            Basic
          </span>
        </div>
      </div>
    </span>
    <p
      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
    >
      tenant1.test
    </p>
  </div>
</div>

Ignored nodes: comments, script, style
<div
  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
  role="button"
  tabindex="0"
>
  <div
    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
  >
    <div
      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
    >
      <svg
        aria-hidden="true"
        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
        data-testid="DomainIcon"
        focusable="false"
        viewBox="0 0 24 24"
      >
        <path
          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
        />
      </svg>
    </div>
  </div>
  <div
    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
  >
    <span
      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
    >
      <div
        class="MuiBox-root css-axw7ok"
      >
        Tenant Two
        <div
          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
        >
          <span
            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
          >
            Pro
          </span>
        </div>
      </div>
    </span>
    <p
      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
    >
      tenant2.test
    </p>
  </div>
</div>

Ignored nodes: comments, script, style
<button
  class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
  disabled=""
  tabindex="-1"
  type="button"
>
  Select Organization
</button>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Select Organization
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              You have access to multiple organizations. Please select one to continue.
            </p>
            <ul
              class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
            >
              <li
                class="MuiListItem-root MuiListItem-gutters css-19f6boz-MuiListItem-root"
              >
                <div
                  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
                  role="button"
                  tabindex="0"
                >
                  <div
                    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
                  >
                    <div
                      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="DomainIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
                        />
                      </svg>
                    </div>
                  </div>
                  <div
                    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
                  >
                    <span
                      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
                    >
                      <div
                        class="MuiBox-root css-axw7ok"
                      >
                        Tenant One
                        <div
                          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
                        >
                          <span
                            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
                          >
                            Basic
                          </span>
                        </div>
                      </div>
                    </span>
                    <p
                      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
                    >
                      tenant1.test
                    </p>
                  </div>
                </div>
              </li>
              <li
                class="MuiListItem-root MuiListItem-gutters css-19f6boz-MuiListItem-root"
              >
                <div
                  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
                  role="button"
                  tabindex="0"
                >
                  <div
                    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
                  >
                    <div
                      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="DomainIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
                        />
                      </svg>
                    </div>
                  </div>
                  <div
                    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
                  >
                    <span
                      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
                    >
                      <div
                        class="MuiBox-root css-axw7ok"
                      >
                        Tenant Two
                        <div
                          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
                        >
                          <span
                            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
                          >
                            Pro
                          </span>
                        </div>
                      </div>
                    </span>
                    <p
                      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
                    >
                      tenant2.test
                    </p>
                  </div>
                </div>
              </li>
            </ul>
            <button
              class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
              disabled=""
              tabindex="-1"
              type="button"
            >
              Select Organization
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/components/auth/__tests__/TenantSelector.test.tsx:206:37
    204|       render(<TenantSelector onTenantSelected={mockOnTenantSelected} />)
    205|
    206|       const continueButton = screen.getByRole('button')
       |                                     ^
    207|       expect(continueButton).toBeDisabled()
    208|     })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[16/38]⎯

 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Selection Interaction > should enable continue button after tenant selection
 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Selection Interaction > should enable continue button after tenant selection
TestingLibraryElementError: Found multiple elements with the role "button"

Here are the matching elements:

Ignored nodes: comments, script, style
<div
  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
  role="button"
  tabindex="0"
>
  <div
    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
  >
    <div
      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
    >
      <svg
        aria-hidden="true"
        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
        data-testid="DomainIcon"
        focusable="false"
        viewBox="0 0 24 24"
      >
        <path
          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
        />
      </svg>
    </div>
  </div>
  <div
    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
  >
    <span
      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
    >
      <div
        class="MuiBox-root css-axw7ok"
      >
        Tenant One
        <div
          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
        >
          <span
            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
          >
            Basic
          </span>
        </div>
      </div>
    </span>
    <p
      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
    >
      tenant1.test
    </p>
  </div>
</div>

Ignored nodes: comments, script, style
<div
  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
  role="button"
  tabindex="0"
>
  <div
    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
  >
    <div
      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
    >
      <svg
        aria-hidden="true"
        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
        data-testid="DomainIcon"
        focusable="false"
        viewBox="0 0 24 24"
      >
        <path
          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
        />
      </svg>
    </div>
  </div>
  <div
    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
  >
    <span
      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
    >
      <div
        class="MuiBox-root css-axw7ok"
      >
        Tenant Two
        <div
          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
        >
          <span
            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
          >
            Pro
          </span>
        </div>
      </div>
    </span>
    <p
      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
    >
      tenant2.test
    </p>
  </div>
</div>

Ignored nodes: comments, script, style
<button
  class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
  disabled=""
  tabindex="-1"
  type="button"
>
  Select Organization
</button>

(If this is intentional, then use the `*AllBy*` variant of the query (like `queryAllByText`, `getAllByText`, or `findAllByText`)).

Ignored nodes: comments, script, style
<body>
  <div>
    <main
      class="MuiContainer-root MuiContainer-maxWidthSm css-zzzaw2-MuiContainer-root"
    >
      <div
        class="MuiBox-root css-binzgt"
      >
        <div
          class="MuiPaper-root MuiPaper-elevation MuiPaper-rounded MuiPaper-elevation1 MuiCard-root css-1e2b4v4-MuiPaper-root-MuiCard-root"
          style="--Paper-shadow: 0px 2px 1px -1px rgba(0,0,0,0.2),0px 1px 1px 0px rgba(0,0,0,0.14),0px 1px 3px 0px rgba(0,0,0,0.12);"
        >
          <div
            class="MuiCardContent-root css-21mhzp-MuiCardContent-root"
          >
            <h1
              class="MuiTypography-root MuiTypography-h4 MuiTypography-alignCenter MuiTypography-gutterBottom css-l8ztbh-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              Select Organization
            </h1>
            <p
              class="MuiTypography-root MuiTypography-body1 MuiTypography-alignCenter css-12p21tt-MuiTypography-root"
              style="--Typography-textAlign: center;"
            >
              You have access to multiple organizations. Please select one to continue.
            </p>
            <ul
              class="MuiList-root MuiList-padding css-1q05d6r-MuiList-root"
            >
              <li
                class="MuiListItem-root MuiListItem-gutters css-19f6boz-MuiListItem-root"
              >
                <div
                  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
                  role="button"
                  tabindex="0"
                >
                  <div
                    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
                  >
                    <div
                      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="DomainIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
                        />
                      </svg>
                    </div>
                  </div>
                  <div
                    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
                  >
                    <span
                      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
                    >
                      <div
                        class="MuiBox-root css-axw7ok"
                      >
                        Tenant One
                        <div
                          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
                        >
                          <span
                            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
                          >
                            Basic
                          </span>
                        </div>
                      </div>
                    </span>
                    <p
                      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
                    >
                      tenant1.test
                    </p>
                  </div>
                </div>
              </li>
              <li
                class="MuiListItem-root MuiListItem-gutters css-19f6boz-MuiListItem-root"
              >
                <div
                  class="MuiButtonBase-root MuiListItemButton-root MuiListItemButton-gutters MuiListItemButton-root MuiListItemButton-gutters css-17a2sdd-MuiButtonBase-root-MuiListItemButton-root"
                  role="button"
                  tabindex="0"
                >
                  <div
                    class="MuiListItemAvatar-root css-1ulznza-MuiListItemAvatar-root"
                  >
                    <div
                      class="MuiAvatar-root MuiAvatar-circular MuiAvatar-colorDefault css-uj44pp-MuiAvatar-root"
                    >
                      <svg
                        aria-hidden="true"
                        class="MuiSvgIcon-root MuiSvgIcon-fontSizeMedium css-1umw9bq-MuiSvgIcon-root"
                        data-testid="DomainIcon"
                        focusable="false"
                        viewBox="0 0 24 24"
                      >
                        <path
                          d="M12 7V3H2v18h20V7zM6 19H4v-2h2zm0-4H4v-2h2zm0-4H4V9h2zm0-4H4V5h2zm4 12H8v-2h2zm0-4H8v-2h2zm0-4H8V9h2zm0-4H8V5h2zm10 12h-8v-2h2v-2h-2v-2h2v-2h-2V9h8zm-2-8h-2v2h2zm0 4h-2v2h2z"
                        />
                      </svg>
                    </div>
                  </div>
                  <div
                    class="MuiListItemText-root MuiListItemText-multiline css-14ln1j6-MuiListItemText-root"
                  >
                    <span
                      class="MuiTypography-root MuiTypography-body1 MuiListItemText-primary css-rizt0-MuiTypography-root"
                    >
                      <div
                        class="MuiBox-root css-axw7ok"
                      >
                        Tenant Two
                        <div
                          class="MuiChip-root MuiChip-outlined MuiChip-sizeSmall MuiChip-colorDefault MuiChip-outlinedDefault css-8m2o99-MuiChip-root"
                        >
                          <span
                            class="MuiChip-label MuiChip-labelSmall css-19m61dl-MuiChip-label"
                          >
                            Pro
                          </span>
                        </div>
                      </div>
                    </span>
                    <p
                      class="MuiTypography-root MuiTypography-body2 MuiListItemText-secondary css-1a1whku-MuiTypography-root"
                    >
                      tenant2.test
                    </p>
                  </div>
                </div>
              </li>
            </ul>
            <button
              class="MuiButtonBase-root MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth Mui-disabled MuiButton-root MuiButton-contained MuiButton-containedPrimary MuiButton-sizeLarge MuiButton-containedSizeLarge MuiButton-colorPrimary MuiButton-fullWidth css-13tpzue-MuiButtonBase-root-MuiButton-root"
              disabled=""
              tabindex="-1"
              type="button"
            >
              Select Organization
            </button>
          </div>
        </div>
      </div>
    </main>
  </div>
</body>
 ❯ Object.getElementError node_modules/@testing-library/dom/dist/config.js:37:19
 ❯ getElementError node_modules/@testing-library/dom/dist/query-helpers.js:20:35
 ❯ getMultipleElementsFoundError node_modules/@testing-library/dom/dist/query-helpers.js:23:10
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:55:13
 ❯ node_modules/@testing-library/dom/dist/query-helpers.js:95:19
 ❯ src/components/auth/__tests__/TenantSelector.test.tsx:219:37
    217|
    218|       // Initially disabled
    219|       const continueButton = screen.getByRole('button')
       |                                     ^
    220|       expect(continueButton).toBeDisabled()
    221|

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[17/38]⎯

 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Selection Interaction > should show visual selection feedback
 FAIL  src/components/auth/__tests__/TenantSelector.test.tsx > TenantSelector - Enhanced Tests > Selection Interaction > should show visual selection feedback
Error: expect(received).toHaveAttribute()

received value must be an HTMLElement or an SVGElement.

 ❯ src/components/auth/__tests__/TenantSelector.test.tsx:240:28
    238|
    239|       // Should show selected state (this depends on your actual implementation)
    240|       expect(tenantButton).toHaveAttribute('aria-selected', 'true')
       |                            ^
    241|     })
    242|   })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[18/38]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
TestingLibraryElementError: Unable to find an element with the text: Admin. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      class="MuiBox-root css-19midj6"
    >
      <h6
        class="MuiTypography-root MuiTypography-h6 css-s7rrgg-MuiTypography-root"
      >
        Failed to fetch roles for
        Test Tenant
      </h6>
      <p
        class="MuiTypography-root MuiTypography-body2 css-1emftrx-MuiTypography-root"
      >
        Network Error
      </p>
      <p
        class="MuiTypography-root MuiTypography-body2 css-1jreg6x-MuiTypography-root"
      >
        Please check the console for more details.
      </p>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <div
        class="MuiBox-root css-19midj6"
      >
        <h6
          class="MuiTypography-root MuiTypography-h6 css-s7rrgg-MuiTypography-root"
        >
          Failed to fetch roles for
          Test Tenant
        </h6>
        <p
          class="MuiTypography-root MuiTypography-body2 css-1emftrx-MuiTypography-root"
        >
          Network Error
        </p>
        <p
          class="MuiTypography-root MuiTypography-body2 css-1jreg6x-MuiTypography-root"
        >
          Please check the console for more details.
        </p>
      </div>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/__tests__/roles/RoleList.test.tsx:143:11
    141|
    142|     // ✅ Wait for roles to load
    143|     await waitFor(() => {
       |           ^
    144|       expect(screen.getByText('Admin')).toBeInTheDocument()
    145|     }, { timeout: 8000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[19/38]⎯

 FAIL  src/components/__tests__/roles/RoleList.test.tsx > RoleList > renders roles list correctly
TestingLibraryElementError: Unable to find an element with the text: Admin. This could be because the text is broken up by multiple elements. In this case, you can provide a function for your text matcher to make your matcher more flexible.

Ignored nodes: comments, script, style
<body>
  <div>
    <div
      class="MuiBox-root css-19midj6"
    >
      <h6
        class="MuiTypography-root MuiTypography-h6 css-s7rrgg-MuiTypography-root"
      >
        Failed to fetch roles for
        Test Tenant
      </h6>
      <p
        class="MuiTypography-root MuiTypography-body2 css-1emftrx-MuiTypography-root"
      >
        Network Error
      </p>
      <p
        class="MuiTypography-root MuiTypography-body2 css-1jreg6x-MuiTypography-root"
      >
        Please check the console for more details.
      </p>
    </div>
  </div>
</body>

Ignored nodes: comments, script, style
<html>
  <head>
    <meta
      content=""
      name="emotion-insertion-point"
    />
  </head>
  <body>
    <div>
      <div
        class="MuiBox-root css-19midj6"
      >
        <h6
          class="MuiTypography-root MuiTypography-h6 css-s7rrgg-MuiTypography-root"
        >
          Failed to fetch roles for
          Test Tenant
        </h6>
        <p
          class="MuiTypography-root MuiTypography-body2 css-1emftrx-MuiTypography-root"
        >
          Network Error
        </p>
        <p
          class="MuiTypography-root MuiTypography-body2 css-1jreg6x-MuiTypography-root"
        >
          Please check the console for more details.
        </p>
      </div>
    </div>
  </body>
</html>...
 ❯ Proxy.waitForWrapper node_modules/@testing-library/dom/dist/wait-for.js:163:27
 ❯ src/components/__tests__/roles/RoleList.test.tsx:143:11
    141|
    142|     // ✅ Wait for roles to load
    143|     await waitFor(() => {
       |           ^
    144|       expect(screen.getByText('Admin')).toBeInTheDocument()
    145|     }, { timeout: 8000 })

⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯⎯[20/38]⎯

 Test Files  5 failed (5)
      Tests  19 failed | 17 passed (36)
   Start at  20:41:58
   Duration  129.80s
