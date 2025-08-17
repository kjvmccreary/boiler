2025-08-16 18:10:22.408 | [23:10:22 INF] REQUEST 3404b185: GET /api/users/profile from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.408 | 🔍 JWT OnMessageReceived:
2025-08-16 18:10:22.408 |    Path: /api/users/profile
2025-08-16 18:10:22.408 |    Method: GET
2025-08-16 18:10:22.408 |    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
2025-08-16 18:10:22.408 | ✅ JWT Token Validated Successfully:
2025-08-16 18:10:22.408 |    UserId: 6
2025-08-16 18:10:22.408 |    Email: mccrearyforward@gmail.com
2025-08-16 18:10:22.408 |    Issuer: AuthService
2025-08-16 18:10:22.408 |    Audience: StarterApp
2025-08-16 18:10:22.408 |    Claims Count: 42
2025-08-16 18:10:22.408 | [23:10:22 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.409 | [23:10:22 INF] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users/profile' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.409 | [23:10:22 INF] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users/profile' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.409 | [23:10:22 INF] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.409 | [23:10:22 INF] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.409 | [23:10:22 INF] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.409 | [23:10:22 WRN] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: 'You have ignored all SSL warnings by using DangerousAcceptAnyServerCertificateValidator for this DownstreamRoute, UpstreamPathTemplate: Ocelot.Values.UpstreamPathTemplate, DownstreamPathTemplate: /api/users/{everything}' {"SourceContext": "Ocelot.Requester.MessageInvokerPool", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.674 | [23:10:22 INF] requestId: 0HNET2U8VVER9:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users/profile.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.675 | [23:10:22 INF] RESPONSE 3404b185: 200 in 266ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.675 | [23:10:22 INF] HTTP GET /api/users/profile responded 200 in 266.9475 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVER9:00000001", "ConnectionId": "0HNET2U8VVER9"}
2025-08-16 18:10:22.718 | [23:10:22 INF] REQUEST 154eab3d: GET /api/permissions/grouped from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERB:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERB"}
2025-08-16 18:10:22.718 | 🔍 JWT OnMessageReceived:
2025-08-16 18:10:22.718 |    Path: /api/permissions/grouped
2025-08-16 18:10:22.718 |    Method: GET
2025-08-16 18:10:22.718 |    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
2025-08-16 18:10:22.718 | ✅ JWT Token Validated Successfully:
2025-08-16 18:10:22.719 |    UserId: 6
2025-08-16 18:10:22.719 |    Email: mccrearyforward@gmail.com
2025-08-16 18:10:22.719 |    Issuer: AuthService
2025-08-16 18:10:22.719 |    Audience: StarterApp
2025-08-16 18:10:22.719 |    Claims Count: 42
2025-08-16 18:10:22.719 | [23:10:22 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNET2U8VVERB:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERB"}
2025-08-16 18:10:22.719 | [23:10:22 WRN] requestId: 0HNET2U8VVERB:00000001, previousRequestId: No PreviousRequestId, message: 'DownstreamRouteFinderMiddleware setting pipeline errors. IDownstreamRouteFinder returned Error Code: UnableToFindDownstreamRouteError Message: Failed to match Route configuration for upstream path: /api/permissions/grouped, verb: GET.' {"SourceContext": "Ocelot.DownstreamRouteFinder.Middleware.DownstreamRouteFinderMiddleware", "RequestId": "0HNET2U8VVERB:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERB"}
2025-08-16 18:10:22.719 | [23:10:22 WRN] requestId: 0HNET2U8VVERB:00000001, previousRequestId: No PreviousRequestId, message: 'Error Code: UnableToFindDownstreamRouteError Message: Failed to match Route configuration for upstream path: /api/permissions/grouped, verb: GET. errors found in ResponderMiddleware. Setting error response for request path:/api/permissions/grouped, request method: GET' {"SourceContext": "Ocelot.Responder.Middleware.ResponderMiddleware", "RequestId": "0HNET2U8VVERB:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERB"}
2025-08-16 18:10:22.719 | [23:10:22 INF] RESPONSE 154eab3d: 404 in 0ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERB:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERB"}
2025-08-16 18:10:22.719 | [23:10:22 INF] HTTP GET /api/permissions/grouped responded 404 in 0.8351 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERB:00000001", "ConnectionId": "0HNET2U8VVERB"}
2025-08-16 18:10:22.719 | [23:10:22 INF] REQUEST 5195e39f: GET /api/roles/7 from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.719 | 🔍 JWT OnMessageReceived:
2025-08-16 18:10:22.719 |    Path: /api/roles/7
2025-08-16 18:10:22.719 |    Method: GET
2025-08-16 18:10:22.719 |    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
2025-08-16 18:10:22.719 | ✅ JWT Token Validated Successfully:
2025-08-16 18:10:22.719 |    UserId: 6
2025-08-16 18:10:22.719 |    Email: mccrearyforward@gmail.com
2025-08-16 18:10:22.719 |    Issuer: AuthService
2025-08-16 18:10:22.719 |    Audience: StarterApp
2025-08-16 18:10:22.719 |    Claims Count: 42
2025-08-16 18:10:22.719 | [23:10:22 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.720 | [23:10:22 INF] requestId: 0HNET2U8VVERA:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/roles/7' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.720 | [23:10:22 INF] requestId: 0HNET2U8VVERA:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/roles/7' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.720 | [23:10:22 INF] requestId: 0HNET2U8VVERA:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.720 | [23:10:22 INF] requestId: 0HNET2U8VVERA:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.720 | [23:10:22 INF] requestId: 0HNET2U8VVERA:00000001, previousRequestId: No PreviousRequestId, message: '/api/roles/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.730 | [23:10:22 INF] requestId: 0HNET2U8VVERA:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/roles/7.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.731 | [23:10:22 INF] RESPONSE 5195e39f: 200 in 11ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "RequestPath": "/api/roles/7", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.731 | [23:10:22 INF] HTTP GET /api/roles/7 responded 200 in 11.9168 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERA:00000001", "ConnectionId": "0HNET2U8VVERA"}
2025-08-16 18:10:22.752 | [23:10:22 INF] REQUEST 57f99b19: GET /api/roles/7/users from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.753 | 🔍 JWT OnMessageReceived:
2025-08-16 18:10:22.753 |    Path: /api/roles/7/users
2025-08-16 18:10:22.753 |    Method: GET
2025-08-16 18:10:22.753 |    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
2025-08-16 18:10:22.753 | ✅ JWT Token Validated Successfully:
2025-08-16 18:10:22.753 |    UserId: 6
2025-08-16 18:10:22.753 |    Email: mccrearyforward@gmail.com
2025-08-16 18:10:22.753 |    Issuer: AuthService
2025-08-16 18:10:22.753 |    Audience: StarterApp
2025-08-16 18:10:22.753 |    Claims Count: 42
2025-08-16 18:10:22.753 | [23:10:22 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.753 | [23:10:22 INF] requestId: 0HNET2U8VVERC:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/roles/7/users' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.753 | [23:10:22 INF] requestId: 0HNET2U8VVERC:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/roles/7/users' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.753 | [23:10:22 INF] requestId: 0HNET2U8VVERC:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.753 | [23:10:22 INF] requestId: 0HNET2U8VVERC:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.753 | [23:10:22 INF] requestId: 0HNET2U8VVERC:00000001, previousRequestId: No PreviousRequestId, message: '/api/roles/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.769 | [23:10:22 INF] requestId: 0HNET2U8VVERC:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/roles/7/users.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.770 | [23:10:22 INF] RESPONSE 57f99b19: 200 in 17ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "RequestPath": "/api/roles/7/users", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.770 | [23:10:22 INF] HTTP GET /api/roles/7/users responded 200 in 17.7186 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERC:00000001", "ConnectionId": "0HNET2U8VVERC"}
2025-08-16 18:10:22.790 | [23:10:22 INF] REQUEST b1744863: GET /api/permissions/grouped from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERD:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERD"}
2025-08-16 18:10:22.790 | 🔍 JWT OnMessageReceived:
2025-08-16 18:10:22.790 |    Path: /api/permissions/grouped
2025-08-16 18:10:22.790 |    Method: GET
2025-08-16 18:10:22.790 |    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
2025-08-16 18:10:22.790 | ✅ JWT Token Validated Successfully:
2025-08-16 18:10:22.790 |    UserId: 6
2025-08-16 18:10:22.790 |    Email: mccrearyforward@gmail.com
2025-08-16 18:10:22.790 |    Issuer: AuthService
2025-08-16 18:10:22.790 |    Audience: StarterApp
2025-08-16 18:10:22.790 |    Claims Count: 42
2025-08-16 18:10:22.790 | [23:10:22 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNET2U8VVERD:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERD"}
2025-08-16 18:10:22.790 | [23:10:22 WRN] requestId: 0HNET2U8VVERD:00000001, previousRequestId: No PreviousRequestId, message: 'DownstreamRouteFinderMiddleware setting pipeline errors. IDownstreamRouteFinder returned Error Code: UnableToFindDownstreamRouteError Message: Failed to match Route configuration for upstream path: /api/permissions/grouped, verb: GET.' {"SourceContext": "Ocelot.DownstreamRouteFinder.Middleware.DownstreamRouteFinderMiddleware", "RequestId": "0HNET2U8VVERD:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERD"}
2025-08-16 18:10:22.790 | [23:10:22 WRN] requestId: 0HNET2U8VVERD:00000001, previousRequestId: No PreviousRequestId, message: 'Error Code: UnableToFindDownstreamRouteError Message: Failed to match Route configuration for upstream path: /api/permissions/grouped, verb: GET. errors found in ResponderMiddleware. Setting error response for request path:/api/permissions/grouped, request method: GET' {"SourceContext": "Ocelot.Responder.Middleware.ResponderMiddleware", "RequestId": "0HNET2U8VVERD:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERD"}
2025-08-16 18:10:22.790 | [23:10:22 INF] RESPONSE b1744863: 404 in 0ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERD:00000001", "RequestPath": "/api/permissions/grouped", "ConnectionId": "0HNET2U8VVERD"}
2025-08-16 18:10:22.790 | [23:10:22 INF] HTTP GET /api/permissions/grouped responded 404 in 0.8518 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERD:00000001", "ConnectionId": "0HNET2U8VVERD"}
2025-08-16 18:10:31.079 | [23:10:31 INF] REQUEST 2a5466e5: GET /health from 127.0.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERE:00000001", "RequestPath": "/health", "ConnectionId": "0HNET2U8VVERE"}
2025-08-16 18:10:31.080 | 🔍 JWT OnMessageReceived:
2025-08-16 18:10:31.080 |    Path: /health
2025-08-16 18:10:31.080 |    Method: GET
2025-08-16 18:10:31.080 |    Authorization Header: MISSING
2025-08-16 18:10:31.080 | [23:10:31 INF] RESPONSE 2a5466e5: 200 in 0ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERE:00000001", "RequestPath": "/health", "ConnectionId": "0HNET2U8VVERE"}
2025-08-16 18:10:31.080 | [23:10:31 INF] HTTP GET /health responded 200 in 0.3855 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNET2U8VVERE:00000001", "ConnectionId": "0HNET2U8VVERE"}
