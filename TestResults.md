SERVICE LOGS
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:51 +0000] "POST /api/tenants/create-additional HTTP/1.1" 201 409 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T14:47:51.863839150Z [14:47:51 INF] requestId: 0HNEV5S9DLOH5:00000001, previousRequestId: No PreviousRequestId, message: '201 (Created) status code of request URI: https://boiler-user:7002/api/tenants/create-additional.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOH5:00000001", "RequestPath": "/api/tenants/create-additional", "ConnectionId": "0HNEV5S9DLOH5"}
boiler-gateway | 2025-08-19T14:47:51.864445392Z [14:47:51 INF] RESPONSE aafdb262: 201 in 419ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH5:00000001", "RequestPath": "/api/tenants/create-additional", "ConnectionId": "0HNEV5S9DLOH5"}
boiler-gateway | 2025-08-19T14:47:51.864491995Z [14:47:51 INF] HTTP POST /api/tenants/create-additional responded 201 in 419.6283 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH5:00000001", "ConnectionId": "0HNEV5S9DLOH5"}
boiler-gateway | 2025-08-19T14:47:51.878439167Z [14:47:51 INF] REQUEST bf5b934d: GET /api/users/6/tenants from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.878567876Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T14:47:51.878591278Z    Path: /api/users/6/tenants
boiler-gateway | 2025-08-19T14:47:51.878593478Z    Method: GET
boiler-gateway | 2025-08-19T14:47:51.878596078Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T14:47:51.878702286Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T14:47:51.878714587Z    UserId: 6
boiler-gateway | 2025-08-19T14:47:51.878717787Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T14:47:51.878719887Z    Issuer: AuthService
boiler-gateway | 2025-08-19T14:47:51.878721787Z    Audience: StarterApp
boiler-gateway | 2025-08-19T14:47:51.878780891Z    Claims Count: 50
boiler-gateway | 2025-08-19T14:47:51.878809093Z [14:47:51 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.879512742Z [14:47:51 INF] requestId: 0HNEV5S9DLOH6:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users/6/tenants' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.879532444Z [14:47:51 INF] requestId: 0HNEV5S9DLOH6:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users/6/tenants' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.879535244Z [14:47:51 INF] requestId: 0HNEV5S9DLOH6:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.879537944Z [14:47:51 INF] requestId: 0HNEV5S9DLOH6:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.879540444Z [14:47:51 INF] requestId: 0HNEV5S9DLOH6:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-user |   | 2025-08-19T14:47:51.885122833Z [14:47:51 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxIiwidGVuYW50X25hbWUiOiJEZWZhdWx0IFRlbmFudCIsInRlbmFudF9kb21haW4iOiJsb2NhbGhvc3QiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiYTc0Y2U4ZDUtODcxMy00NTg0LWE1YjEtNzlkMDFiMmIxYzcwIiwiaWF0IjoxNzU1NjE0ODE5LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsInBlcm1pc3Npb24iOlsicGVybWlzc2lvbnMuZGVsZXRlIiwicm9sZXMuY3JlYXRlIiwicmVwb3J0cy52aWV3IiwidXNlcnMudmlld19hbGwiLCJzeXN0ZW0udmlld19tZXRyaWNzIiwidXNlcnMuZGVsZXRlIiwidGVuYW50cy52aWV3X2FsbCIsInVzZXJzLnZpZXciLCJ0ZW5hbnRzLmNyZWF0ZSIsImJpbGxpbmcudmlld19pbnZvaWNlcyIsInN5c3RlbS5tYW5hZ2VfYmFja3VwcyIsInJvbGVzLmFzc2lnbl91c2VycyIsImJpbGxpbmcudmlldyIsImJpbGxpbmcucHJvY2Vzc19wYXltZW50cyIsInJvbGVzLmRlbGV0ZSIsInRlbmFudHMuZWRpdCIsInRlbmFudHMuZGVsZXRlIiwidXNlcnMubWFuYWdlX3JvbGVzIiwiYmlsbGluZy5tYW5hZ2UiLCJyb2xlcy52aWV3IiwicmVwb3J0cy5zY2hlZHVsZSIsInBlcm1pc3Npb25zLm1hbmFnZSIsInJlcG9ydHMuZXhwb3J0IiwidXNlcnMuZWRpdCIsInJvbGVzLm1hbmFnZV9wZXJtaXNzaW9ucyIsInVzZXJzLmNyZWF0ZSIsInN5c3RlbS52aWV3X2xvZ3MiLCJyb2xlcy5lZGl0IiwicGVybWlzc2lvbnMudmlldyIsInRlbmFudHMudmlldyIsInBlcm1pc3Npb25zLmNyZWF0ZSIsInRlbmFudHMubWFuYWdlX3NldHRpbmdzIiwic3lzdGVtLm1hbmFnZV9zZXR0aW5ncyIsInJlcG9ydHMuY3JlYXRlIiwicGVybWlzc2lvbnMuZWRpdCJdLCJleHAiOjE3NTU2MTg0MTksImlzcyI6IkF1dGhTZXJ2aWNlIiwiYXVkIjoiU3RhcnRlckFwcCJ9.HvLotuIEhurNl3-6iMDVHchLhzWtMC9TzyoOYrRu-YU; Referer=https://localhost:3000/app/dashboard; traceparent=00-abbe5457672d1575a366783c4feb9afd-a46041056452d7b9-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=1; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxIiwidGVuYW50X25hbWUiOiJEZWZhdWx0IFRlbmFudCIsInRlbmFudF9kb21haW4iOiJsb2NhbGhvc3QiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiYTc0Y2U4ZDUtODcxMy00NTg0LWE1YjEtNzlkMDFiMmIxYzcwIiwiaWF0IjoxNzU1NjE0ODE5LCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJBZG1pbiIsInBlcm1pc3Npb24iOlsicGVybWlzc2lvbnMuZGVsZXRlIiwicm9sZXMuY3JlYXRlIiwicmVwb3J0cy52aWV3IiwidXNlcnMudmlld19hbGwiLCJzeXN0ZW0udmlld19tZXRyaWNzIiwidXNlcnMuZGVsZXRlIiwidGVuYW50cy52aWV3X2FsbCIsInVzZXJzLnZpZXciLCJ0ZW5hbnRzLmNyZWF0ZSIsImJpbGxpbmcudmlld19pbnZvaWNlcyIsInN5c3RlbS5tYW5hZ2VfYmFja3VwcyIsInJvbGVzLmFzc2lnbl91c2VycyIsImJpbGxpbmcudmlldyIsImJpbGxpbmcucHJvY2Vzc19wYXltZW50cyIsInJvbGVzLmRlbGV0ZSIsInRlbmFudHMuZWRpdCIsInRlbmFudHMuZGVsZXRlIiwidXNlcnMubWFuYWdlX3JvbGVzIiwiYmlsbGluZy5tYW5hZ2UiLCJyb2xlcy52aWV3IiwicmVwb3J0cy5zY2hlZHVsZSIsInBlcm1pc3Npb25zLm1hbmFnZSIsInJlcG9ydHMuZXhwb3J0IiwidXNlcnMuZWRpdCIsInJvbGVzLm1hbmFnZV9wZXJtaXNzaW9ucyIsInVzZXJzLmNyZWF0ZSIsInN5c3RlbS52aWV3X2xvZ3MiLCJyb2xlcy5lZGl0IiwicGVybWlzc2lvbnMudmlldyIsInRlbmFudHMudmlldyIsInBlcm1pc3Npb25zLmNyZWF0ZSIsInRlbmFudHMubWFuYWdlX3NldHRpbmdzIiwic3lzdGVtLm1hbmFnZV9zZXR0aW5ncyIsInJlcG9ydHMuY3JlYXRlIiwicGVybWlzc2lvbnMuZWRpdCJdLCJleHAiOjE3NTU2MTg0MTksImlzcyI6IkF1dGhTZXJ2aWNlIiwiYXVkIjoiU3RhcnRlckFwcCJ9.HvLotuIEhurNl3-6iMDVHchLhzWtMC9TzyoOYrRu-YU; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMSIsIlJvbGVzIjpbXSwiUGVybWlzc2lvbnMiOlsicGVybWlzc2lvbnMuZGVsZXRlIiwicm9sZXMuY3JlYXRlIiwicmVwb3J0cy52aWV3IiwidXNlcnMudmlld19hbGwiLCJzeXN0ZW0udmlld19tZXRyaWNzIiwidXNlcnMuZGVsZXRlIiwidGVuYW50cy52aWV3X2FsbCIsInVzZXJzLnZpZXciLCJ0ZW5hbnRzLmNyZWF0ZSIsImJpbGxpbmcudmlld19pbnZvaWNlcyIsInN5c3RlbS5tYW5hZ2VfYmFja3VwcyIsInJvbGVzLmFzc2lnbl91c2VycyIsImJpbGxpbmcudmlldyIsImJpbGxpbmcucHJvY2Vzc19wYXltZW50cyIsInJvbGVzLmRlbGV0ZSIsInRlbmFudHMuZWRpdCIsInRlbmFudHMuZGVsZXRlIiwidXNlcnMubWFuYWdlX3JvbGVzIiwiYmlsbGluZy5tYW5hZ2UiLCJyb2xlcy52aWV3IiwicmVwb3J0cy5zY2hlZHVsZSIsInBlcm1pc3Npb25zLm1hbmFnZSIsInJlcG9ydHMuZXhwb3J0IiwidXNlcnMuZWRpdCIsInJvbGVzLm1hbmFnZV9wZXJtaXNzaW9ucyIsInVzZXJzLmNyZWF0ZSIsInN5c3RlbS52aWV3X2xvZ3MiLCJyb2xlcy5lZGl0IiwicGVybWlzc2lvbnMudmlldyIsInRlbmFudHMudmlldyIsInBlcm1pc3Npb25zLmNyZWF0ZSIsInRlbmFudHMubWFuYWdlX3NldHRpbmdzIiwic3lzdGVtLm1hbmFnZV9zZXR0aW5ncyIsInJlcG9ydHMuY3JlYXRlIiwicGVybWlzc2lvbnMuZWRpdCJdLCJFbWFpbCI6bnVsbH0=; X-Tenant-Context=1 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV5S7LDCU4:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU4"}
boiler-user |   | 2025-08-19T14:47:51.885208439Z [14:47:51 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 1 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV5S7LDCU4:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU4"}
boiler-user |   | 2025-08-19T14:47:51.910944533Z [14:47:51 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU4:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU4"}
boiler-user |   | 2025-08-19T14:47:51.911020238Z [14:47:51 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU4:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU4"}
boiler-user |   | 2025-08-19T14:47:51.912439237Z [14:47:51 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEV5S7LDCU4:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU4"}
boiler-user |   | 2025-08-19T14:47:51.917956022Z [14:47:51 INF] Found 4 tenants for user 6 (unfiltered by tenant context) {"SourceContext": "UserService.Controllers.UsersController", "ActionId": "36cee95c-293b-4687-8bb1-45eb81cd2bb7", "ActionName": "UserService.Controllers.UsersController.GetUserTenants (UserService)", "RequestId": "0HNEV5S7LDCU4:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU4"}
boiler-user |   | 2025-08-19T14:47:51.918673472Z [14:47:51 INF] HTTP GET /api/users/6/tenants responded 200 in 34.2288 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T14:47:51.919597536Z [14:47:51 INF] requestId: 0HNEV5S9DLOH6:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users/6/tenants.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.919969062Z [14:47:51 INF] RESPONSE bf5b934d: 200 in 41ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-gateway | 2025-08-19T14:47:51.919975262Z [14:47:51 INF] HTTP GET /api/users/6/tenants responded 200 in 41.7887 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH6:00000001", "ConnectionId": "0HNEV5S9DLOH6"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:51 +0000] "GET /api/users/6/tenants HTTP/1.1" 200 382 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T14:47:51.929272710Z [14:47:51 INF] REQUEST 216a02c9: POST /api/auth/switch-tenant from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.929315613Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T14:47:51.929318714Z    Path: /api/auth/switch-tenant
boiler-gateway | 2025-08-19T14:47:51.929320814Z    Method: POST
boiler-gateway | 2025-08-19T14:47:51.929322814Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T14:47:51.929735543Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T14:47:51.929747844Z    UserId: 6
boiler-gateway | 2025-08-19T14:47:51.929750644Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T14:47:51.929752644Z    Issuer: AuthService
boiler-gateway | 2025-08-19T14:47:51.929754644Z    Audience: StarterApp
boiler-gateway | 2025-08-19T14:47:51.929756544Z    Claims Count: 50
boiler-gateway | 2025-08-19T14:47:51.929758744Z [14:47:51 INF] ✅ Tenant resolved: 1 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.930156572Z [14:47:51 INF] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/auth/switch-tenant' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.930264780Z [14:47:51 INF] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/auth/switch-tenant' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.930287781Z [14:47:51 INF] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.930290581Z [14:47:51 INF] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.930293182Z [14:47:51 INF] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: '/api/auth/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:51.930390588Z [14:47:51 WRN] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: 'You have ignored all SSL warnings by using DangerousAcceptAnyServerCertificateValidator for this DownstreamRoute, UpstreamPathTemplate: Ocelot.Values.UpstreamPathTemplate, DownstreamPathTemplate: /api/auth/{everything}' {"SourceContext": "Ocelot.Requester.MessageInvokerPool", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-auth |   | 2025-08-19T14:47:52.010770891Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: User 6 requesting to switch to tenant 18 {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.019652510Z [14:47:52 WRN] Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'. {"EventId": {"Id": 20504, "Name": "Microsoft.EntityFrameworkCore.Query.MultipleCollectionIncludeWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Query", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.047131025Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: User loaded. TenantUsers count: 4 {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.047682963Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: User has access to tenant 18: True {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.047691364Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: Loading tenant 18 from repository {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.055735225Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: Tenant loaded - ID: 18, Name: Number Five, IsActive: True {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.075026369Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: About to generate token with - User ID: 6, Tenant ID: 18, Tenant Name: Number Five {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.077763260Z [14:47:52 INF] 🔍 JWT: Found 1 active roles for user 6 in tenant 18 {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.077816264Z [14:47:52 INF] 🔍 JWT: Added role claim 'Tenant Admin' for user 6 {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.078140986Z [14:47:52 INF] 🔍 PERMISSION DEBUG: Getting permissions for user 6 in tenant 18 {"SourceContext": "Common.Services.PermissionService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.078990445Z [14:47:52 INF] 🔍 PERMISSION DEBUG: Found 1 active user roles for user 6 in tenant 18: 42 {"SourceContext": "Common.Services.PermissionService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.081466118Z [14:47:52 INF] 🔍 PERMISSION DEBUG: Found 11 role permissions for roles 42 {"SourceContext": "Common.Services.PermissionService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.082822212Z [14:47:52 INF] 🔍 PERMISSION DEBUG: Final permissions for user 6 in tenant 18: [users.edit, roles.view, users.create, roles.edit, permissions.view, roles.create, reports.view, users.delete, users.view, roles.delete, reports.export] {"SourceContext": "Common.Services.PermissionService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.082833313Z [14:47:52 INF] 🔍 JWT: Added 11 permissions to JWT for user 6 in tenant 18 {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.083277944Z [14:47:52 INF] 🔍 JWT: Generated token for user 6 with 1 roles and 11 permissions {"SourceContext": "AuthService.Services.EnhancedTokenService", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.097146211Z [14:47:52 WRN] 🔍 TENANT SWITCH DEBUG: Response DTOs created - UserDto.TenantId: 18, TenantDto.Id: 18, TenantDto.Name: Number Five {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-auth |   | 2025-08-19T14:47:52.097218616Z [14:47:52 INF] User 6 successfully switched to tenant 18 (Number Five) {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "c03d707f-2892-4315-97b7-c873875912c1", "ActionName": "AuthService.Controllers.AuthController.SwitchTenant (AuthService)", "RequestId": "0HNEV5S7LG4S2:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S7LG4S2"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:52 +0000] "POST /api/auth/switch-tenant HTTP/1.1" 200 1234 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-auth |   | 2025-08-19T14:47:52.097981169Z [14:47:52 INF] HTTP POST /api/auth/switch-tenant responded 200 in 154.4937 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T14:47:52.098480604Z [14:47:52 INF] requestId: 0HNEV5S9DLOH7:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-auth:7001/api/auth/switch-tenant.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:52.099180053Z [14:47:52 INF] RESPONSE 216a02c9: 200 in 169ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "RequestPath": "/api/auth/switch-tenant", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:52.099212155Z [14:47:52 INF] HTTP POST /api/auth/switch-tenant responded 200 in 169.9049 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH7:00000001", "ConnectionId": "0HNEV5S9DLOH7"}
boiler-gateway | 2025-08-19T14:47:52.107439828Z [14:47:52 INF] REQUEST e6948c4b: GET /api/users/profile from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.107829755Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T14:47:52.107883559Z    Path: /api/users/profile
boiler-gateway | 2025-08-19T14:47:52.107886859Z    Method: GET
boiler-gateway | 2025-08-19T14:47:52.107888960Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T14:47:52.108518103Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T14:47:52.108534605Z    UserId: 6
boiler-gateway | 2025-08-19T14:47:52.108537005Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T14:47:52.108557506Z    Issuer: AuthService
boiler-gateway | 2025-08-19T14:47:52.108559806Z    Audience: StarterApp
boiler-gateway | 2025-08-19T14:47:52.108562507Z    Claims Count: 26
boiler-gateway | 2025-08-19T14:47:52.108564607Z [14:47:52 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.109088543Z [14:47:52 INF] requestId: 0HNEV5S9DLOH8:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users/profile' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.109114545Z [14:47:52 INF] requestId: 0HNEV5S9DLOH8:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users/profile' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.109117645Z [14:47:52 INF] requestId: 0HNEV5S9DLOH8:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.109120145Z [14:47:52 INF] requestId: 0HNEV5S9DLOH8:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.109122746Z [14:47:52 INF] requestId: 0HNEV5S9DLOH8:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-user |   | 2025-08-19T14:47:52.114217801Z [14:47:52 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiODBhZTNjZTYtY2FiNi00YTYxLWFmNGMtNjQyMzUwNDM4MGQ2IiwiaWF0IjoxNzU1NjE0ODcyLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInVzZXJzLmVkaXQiLCJyb2xlcy52aWV3IiwidXNlcnMuY3JlYXRlIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJyb2xlcy5jcmVhdGUiLCJyZXBvcnRzLnZpZXciLCJ1c2Vycy5kZWxldGUiLCJ1c2Vycy52aWV3Iiwicm9sZXMuZGVsZXRlIiwicmVwb3J0cy5leHBvcnQiXSwiZXhwIjoxNzU1NjE4NDcyLCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.829_8AC6ockMjQQQVvGce6-dZa310zq2rLrBohHWO7g; Referer=https://localhost:3000/app/dashboard; traceparent=00-ef5e81b8c2fbaf20820c661f96ececa9-912ea37558cde947-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=18; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiODBhZTNjZTYtY2FiNi00YTYxLWFmNGMtNjQyMzUwNDM4MGQ2IiwiaWF0IjoxNzU1NjE0ODcyLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInVzZXJzLmVkaXQiLCJyb2xlcy52aWV3IiwidXNlcnMuY3JlYXRlIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJyb2xlcy5jcmVhdGUiLCJyZXBvcnRzLnZpZXciLCJ1c2Vycy5kZWxldGUiLCJ1c2Vycy52aWV3Iiwicm9sZXMuZGVsZXRlIiwicmVwb3J0cy5leHBvcnQiXSwiZXhwIjoxNzU1NjE4NDcyLCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.829_8AC6ockMjQQQVvGce6-dZa310zq2rLrBohHWO7g; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMTgiLCJSb2xlcyI6W10sIlBlcm1pc3Npb25zIjpbInVzZXJzLmVkaXQiLCJyb2xlcy52aWV3IiwidXNlcnMuY3JlYXRlIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJyb2xlcy5jcmVhdGUiLCJyZXBvcnRzLnZpZXciLCJ1c2Vycy5kZWxldGUiLCJ1c2Vycy52aWV3Iiwicm9sZXMuZGVsZXRlIiwicmVwb3J0cy5leHBvcnQiXSwiRW1haWwiOm51bGx9; X-Tenant-Context=18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.114322708Z [14:47:52 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.116932090Z [14:47:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.116955091Z [14:47:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.116958092Z [14:47:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.141572407Z [14:47:52 WRN] Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'. {"EventId": {"Id": 20504, "Name": "Microsoft.EntityFrameworkCore.Query.MultipleCollectionIncludeWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Query", "ActionId": "6069562c-9264-40f4-a16e-8484bc39c420", "ActionName": "UserService.Controllers.UsersController.GetCurrentUserProfile (UserService)", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.171262877Z [14:47:52 INF] User 6 successfully accessed profile in tenant 18 {"SourceContext": "UserService.Services.UserProfileService", "ActionId": "6069562c-9264-40f4-a16e-8484bc39c420", "ActionName": "UserService.Controllers.UsersController.GetCurrentUserProfile (UserService)", "RequestId": "0HNEV5S7LDCU5:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S7LDCU5"}
boiler-user |   | 2025-08-19T14:47:52.179843775Z [14:47:52 INF] HTTP GET /api/users/profile responded 200 in 66.0613 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T14:47:52.180334509Z [14:47:52 INF] requestId: 0HNEV5S9DLOH8:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users/profile.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.180839544Z [14:47:52 INF] RESPONSE e6948c4b: 200 in 73ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "RequestPath": "/api/users/profile", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-gateway | 2025-08-19T14:47:52.180863746Z [14:47:52 INF] HTTP GET /api/users/profile responded 200 in 73.4774 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH8:00000001", "ConnectionId": "0HNEV5S9DLOH8"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:52 +0000] "GET /api/users/profile HTTP/1.1" 200 321 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T14:47:52.187123682Z [14:47:52 INF] REQUEST 032a99b6: GET /api/tenants/18/settings from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.187214688Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T14:47:52.187218889Z    Path: /api/tenants/18/settings
boiler-gateway | 2025-08-19T14:47:52.187221189Z    Method: GET
boiler-gateway | 2025-08-19T14:47:52.187223189Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T14:47:52.187296394Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T14:47:52.187299694Z    UserId: 6
boiler-gateway | 2025-08-19T14:47:52.187302395Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T14:47:52.187304195Z    Issuer: AuthService
boiler-gateway | 2025-08-19T14:47:52.187306195Z    Audience: StarterApp
boiler-gateway | 2025-08-19T14:47:52.187308095Z    Claims Count: 26
boiler-gateway | 2025-08-19T14:47:52.187351998Z [14:47:52 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.187919938Z [14:47:52 INF] requestId: 0HNEV5S9DLOH9:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/tenants/18/settings' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.187933739Z [14:47:52 INF] requestId: 0HNEV5S9DLOH9:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/tenants/18/settings' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.187936639Z [14:47:52 INF] requestId: 0HNEV5S9DLOH9:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.187939439Z [14:47:52 INF] requestId: 0HNEV5S9DLOH9:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.187941939Z [14:47:52 INF] requestId: 0HNEV5S9DLOH9:00000001, previousRequestId: No PreviousRequestId, message: '/api/tenants/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.199176722Z [14:47:52 WRN] requestId: 0HNEV5S9DLOH9:00000001, previousRequestId: No PreviousRequestId, message: '404 (Not Found) status code of request URI: https://boiler-user:7002/api/tenants/18/settings.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.199425839Z [14:47:52 INF] RESPONSE 032a99b6: 404 in 12ms | Size: 0 {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-gateway | 2025-08-19T14:47:52.199438340Z [14:47:52 INF] HTTP GET /api/tenants/18/settings responded 404 in 12.4367 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOH9:00000001", "ConnectionId": "0HNEV5S9DLOH9"}
boiler-user |   | 2025-08-19T14:47:52.197946336Z [14:47:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU6:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S7LDCU6"}
boiler-user |   | 2025-08-19T14:47:52.197987539Z [14:47:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU6:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S7LDCU6"}
boiler-user |   | 2025-08-19T14:47:52.198168252Z [14:47:52 INF] HTTP GET /api/tenants/18/settings responded 404 in 0.7821 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:52 +0000] "GET /api/tenants/18/settings HTTP/1.1" 404 0 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T14:47:52.202529356Z [14:47:52 INF] REQUEST 13b054c8: GET /api/users/6/tenants from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.202598261Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T14:47:52.202601861Z    Path: /api/users/6/tenants
boiler-gateway | 2025-08-19T14:47:52.202603961Z    Method: GET
boiler-gateway | 2025-08-19T14:47:52.202605961Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T14:47:52.202753471Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T14:47:52.202756572Z    UserId: 6
boiler-gateway | 2025-08-19T14:47:52.202758572Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T14:47:52.202760572Z    Issuer: AuthService
boiler-gateway | 2025-08-19T14:47:52.202762572Z    Audience: StarterApp
boiler-gateway | 2025-08-19T14:47:52.202764672Z    Claims Count: 26
boiler-gateway | 2025-08-19T14:47:52.202766572Z [14:47:52 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.203173901Z [14:47:52 INF] requestId: 0HNEV5S9DLOHA:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/users/6/tenants' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.203207003Z [14:47:52 INF] requestId: 0HNEV5S9DLOHA:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/users/6/tenants' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.203210403Z [14:47:52 INF] requestId: 0HNEV5S9DLOHA:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.203213303Z [14:47:52 INF] requestId: 0HNEV5S9DLOHA:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.203231705Z [14:47:52 INF] requestId: 0HNEV5S9DLOHA:00000001, previousRequestId: No PreviousRequestId, message: '/api/users/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-user |   | 2025-08-19T14:47:52.208375063Z [14:47:52 INF] 🔍 All request headers: Accept=application/json, text/plain, */*; Connection=close; Host=boiler-user:7002; User-Agent=Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36; Accept-Encoding=gzip, deflate, br, zstd; Accept-Language=en-US,en;q=0.9; Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiODBhZTNjZTYtY2FiNi00YTYxLWFmNGMtNjQyMzUwNDM4MGQ2IiwiaWF0IjoxNzU1NjE0ODcyLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInVzZXJzLmVkaXQiLCJyb2xlcy52aWV3IiwidXNlcnMuY3JlYXRlIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJyb2xlcy5jcmVhdGUiLCJyZXBvcnRzLnZpZXciLCJ1c2Vycy5kZWxldGUiLCJ1c2Vycy52aWV3Iiwicm9sZXMuZGVsZXRlIiwicmVwb3J0cy5leHBvcnQiXSwiZXhwIjoxNzU1NjE4NDcyLCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.829_8AC6ockMjQQQVvGce6-dZa310zq2rLrBohHWO7g; Referer=https://localhost:3000/app/dashboard; traceparent=00-8c5df777633005da89bc41b84478d96d-9a77ddf3feb52620-00; X-Real-IP=172.18.0.1; X-Forwarded-For=172.18.0.1; X-Forwarded-Proto=https; sec-ch-ua-platform="Windows"; sec-ch-ua="Not;A=Brand";v="99", "Google Chrome";v="139", "Chromium";v="139"; sec-ch-ua-mobile=?0; Sec-Fetch-Site=same-origin; Sec-Fetch-Mode=cors; Sec-Fetch-Dest=empty; X-Tenant-ID=18; X-Forwarded-Authorization=Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjYiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9lbWFpbGFkZHJlc3MiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwiaHR0cDovL3NjaGVtYXMueG1sc29hcC5vcmcvd3MvMjAwNS8wNS9pZGVudGl0eS9jbGFpbXMvZ2l2ZW5uYW1lIjoiSm9lIE1hZCBEb2ciLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9zdXJuYW1lIjoiTWFjRGFkZHkiLCJ0ZW5hbnRfaWQiOiIxOCIsInRlbmFudF9uYW1lIjoiTnVtYmVyIEZpdmUiLCJ0ZW5hbnRfZG9tYWluIjoiZml2ZS5kZXYiLCJzdWIiOiI2IiwiZW1haWwiOiJtY2NyZWFyeWZvcndhcmRAZ21haWwuY29tIiwianRpIjoiODBhZTNjZTYtY2FiNi00YTYxLWFmNGMtNjQyMzUwNDM4MGQ2IiwiaWF0IjoxNzU1NjE0ODcyLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL3dzLzIwMDgvMDYvaWRlbnRpdHkvY2xhaW1zL3JvbGUiOiJUZW5hbnQgQWRtaW4iLCJwZXJtaXNzaW9uIjpbInVzZXJzLmVkaXQiLCJyb2xlcy52aWV3IiwidXNlcnMuY3JlYXRlIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJyb2xlcy5jcmVhdGUiLCJyZXBvcnRzLnZpZXciLCJ1c2Vycy5kZWxldGUiLCJ1c2Vycy52aWV3Iiwicm9sZXMuZGVsZXRlIiwicmVwb3J0cy5leHBvcnQiXSwiZXhwIjoxNzU1NjE4NDcyLCJpc3MiOiJBdXRoU2VydmljZSIsImF1ZCI6IlN0YXJ0ZXJBcHAifQ.829_8AC6ockMjQQQVvGce6-dZa310zq2rLrBohHWO7g; X-User-Context=eyJVc2VySWQiOm51bGwsIlRlbmFudElkIjoiMTgiLCJSb2xlcyI6W10sIlBlcm1pc3Npb25zIjpbInVzZXJzLmVkaXQiLCJyb2xlcy52aWV3IiwidXNlcnMuY3JlYXRlIiwicm9sZXMuZWRpdCIsInBlcm1pc3Npb25zLnZpZXciLCJyb2xlcy5jcmVhdGUiLCJyZXBvcnRzLnZpZXciLCJ1c2Vycy5kZWxldGUiLCJ1c2Vycy52aWV3Iiwicm9sZXMuZGVsZXRlIiwicmVwb3J0cy5leHBvcnQiXSwiRW1haWwiOm51bGx9; X-Tenant-Context=18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV5S7LDCU7:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU7"}
boiler-user |   | 2025-08-19T14:47:52.208478370Z [14:47:52 INF] 🏢 FOUND tenant from header 'X-Tenant-Id': 18 {"SourceContext": "Common.Services.TenantProvider", "RequestId": "0HNEV5S7LDCU7:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU7"}
boiler-user |   | 2025-08-19T14:47:52.211851806Z [14:47:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU7:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU7"}
boiler-user |   | 2025-08-19T14:47:52.211909210Z [14:47:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU7:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU7"}
boiler-user |   | 2025-08-19T14:47:52.211912910Z [14:47:52 DBG] Authorization was successful. {"EventId": {"Id": 1, "Name": "UserAuthorizationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authorization.DefaultAuthorizationService", "RequestId": "0HNEV5S7LDCU7:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU7"}
boiler-user |   | 2025-08-19T14:47:52.213622929Z [14:47:52 INF] Found 4 tenants for user 6 (unfiltered by tenant context) {"SourceContext": "UserService.Controllers.UsersController", "ActionId": "36cee95c-293b-4687-8bb1-45eb81cd2bb7", "ActionName": "UserService.Controllers.UsersController.GetUserTenants (UserService)", "RequestId": "0HNEV5S7LDCU7:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S7LDCU7"}
boiler-user |   | 2025-08-19T14:47:52.214189368Z [14:47:52 INF] HTTP GET /api/users/6/tenants responded 200 in 6.2993 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T14:47:52.214626799Z [14:47:52 INF] requestId: 0HNEV5S9DLOHA:00000001, previousRequestId: No PreviousRequestId, message: '200 (OK) status code of request URI: https://boiler-user:7002/api/users/6/tenants.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.214925020Z [14:47:52 INF] RESPONSE 13b054c8: 200 in 12ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "RequestPath": "/api/users/6/tenants", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-gateway | 2025-08-19T14:47:52.215031127Z [14:47:52 INF] HTTP GET /api/users/6/tenants responded 200 in 12.6503 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOHA:00000001", "ConnectionId": "0HNEV5S9DLOHA"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:52 +0000] "GET /api/users/6/tenants HTTP/1.1" 200 382 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
boiler-gateway | 2025-08-19T14:47:52.225755375Z [14:47:52 INF] REQUEST f1e99c8c: GET /api/tenants/18/settings from 172.18.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.225810178Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-19T14:47:52.225814079Z    Path: /api/tenants/18/settings
boiler-gateway | 2025-08-19T14:47:52.225816279Z    Method: GET
boiler-gateway | 2025-08-19T14:47:52.225818179Z    Authorization Header: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodH...
boiler-gateway | 2025-08-19T14:47:52.226164403Z ✅ JWT Token Validated Successfully:
boiler-gateway | 2025-08-19T14:47:52.226221307Z    UserId: 6
boiler-gateway | 2025-08-19T14:47:52.226224107Z    Email: mccrearyforward@gmail.com
boiler-gateway | 2025-08-19T14:47:52.226226207Z    Issuer: AuthService
boiler-gateway | 2025-08-19T14:47:52.226228208Z    Audience: StarterApp
boiler-gateway | 2025-08-19T14:47:52.226230108Z    Claims Count: 26
boiler-gateway | 2025-08-19T14:47:52.226232108Z [14:47:52 INF] ✅ Tenant resolved: 18 from jwt-claim {"SourceContext": "ApiGateway.Middleware.TenantResolutionMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.226308013Z [14:47:52 INF] requestId: 0HNEV5S9DLOHB:00000001, previousRequestId: No PreviousRequestId, message: 'The path '/api/tenants/18/settings' is an authenticated route! AuthenticationMiddleware checking if client is authenticated...' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.226331315Z [14:47:52 INF] requestId: 0HNEV5S9DLOHB:00000001, previousRequestId: No PreviousRequestId, message: 'Client has been authenticated for path '/api/tenants/18/settings' by 'AuthenticationTypes.Federation' scheme.' {"SourceContext": "Ocelot.Authentication.Middleware.AuthenticationMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.226334615Z [14:47:52 INF] requestId: 0HNEV5S9DLOHB:00000001, previousRequestId: No PreviousRequestId, message: 'route is authenticated scopes must be checked' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.226337215Z [14:47:52 INF] requestId: 0HNEV5S9DLOHB:00000001, previousRequestId: No PreviousRequestId, message: 'user scopes is authorized calling next authorization checks' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.226339715Z [14:47:52 INF] requestId: 0HNEV5S9DLOHB:00000001, previousRequestId: No PreviousRequestId, message: '/api/tenants/{everything} route does not require user to be authorized' {"SourceContext": "Ocelot.Authorization.Middleware.AuthorizationMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-user |   | 2025-08-19T14:47:52.229985669Z [14:47:52 DBG] Successfully validated the token. {"EventId": {"Id": 2, "Name": "TokenValidationSucceeded"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU8:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S7LDCU8"}
boiler-user |   | 2025-08-19T14:47:52.230032073Z [14:47:52 DBG] AuthenticationScheme: Bearer was successfully authenticated. {"EventId": {"Id": 8, "Name": "AuthenticationSchemeAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNEV5S7LDCU8:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S7LDCU8"}
boiler-user |   | 2025-08-19T14:47:52.230035373Z [14:47:52 INF] HTTP GET /api/tenants/18/settings responded 404 in 0.5175 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T14:47:52.230407299Z [14:47:52 WRN] requestId: 0HNEV5S9DLOHB:00000001, previousRequestId: No PreviousRequestId, message: '404 (Not Found) status code of request URI: https://boiler-user:7002/api/tenants/18/settings.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.230543008Z [14:47:52 INF] RESPONSE f1e99c8c: 404 in 4ms | Size: 0 {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "RequestPath": "/api/tenants/18/settings", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-gateway | 2025-08-19T14:47:52.230549609Z [14:47:52 INF] HTTP GET /api/tenants/18/settings responded 404 in 5.0308 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEV5S9DLOHB:00000001", "ConnectionId": "0HNEV5S9DLOHB"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:14:47:52 +0000] "GET /api/tenants/18/settings HTTP/1.1" 404 0 "https://localhost:3000/app/dashboard" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"


BROWSER CONSOLE:
api.client.ts:44 🔍 API REQUEST (with JWT): POST /api/tenants/create-additional
api.client.ts:57 ✅ API RESPONSE: /api/tenants/create-additional 201
api.client.ts:143 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: "Organization 'Number Five' created successfully! You have been granted administrator access.", hasData: true, errors: 0}
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/tenants/create-additional : {id: 18, name: 'Number Five', domain: 'five.dev', subscriptionPlan: 'Basic', isActive: true, …}
CreateAdditionalTenant.tsx:47 ✅ Created tenant: {id: 18, name: 'Number Five', domain: 'five.dev', subscriptionPlan: 'Basic', isActive: true, …}
TenantContext.tsx:217 🔄 TenantContext: Tenant not found, refreshing tenant list...
TenantContext.tsx:192 🔄 TenantContext: Refreshing user tenants...
tenant.service.ts:20 🔍 TenantService: Calling API endpoint: /api/users/6/tenants
api.client.ts:44 🔍 API REQUEST (with JWT): GET /api/users/6/tenants
api.client.ts:57 ✅ API RESPONSE: /api/users/6/tenants 200
api.client.ts:143 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: 'User tenants retrieved successfully', hasData: true, errors: 0}
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/users/6/tenants : (4) [{…}, {…}, {…}, {…}]
tenant.service.ts:23 🔍 TenantService: Raw API response: (4) [{…}, {…}, {…}, {…}]
tenant.service.ts:29 🔍 TenantService: Converted tenants: (4) [{…}, {…}, {…}, {…}]
TenantContext.tsx:197 ✅ TenantContext: Tenant list refreshed with 4 tenants
TenantContext.tsx:223 ✅ TenantContext: Found tenant after refresh: Number Five
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
api.client.ts:61 🔍 API CLIENT: Unwrapped response for /api/auth/switch-tenant : {accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc…HAifQ.829_8AC6ockMjQQQVvGce6-dZa310zq2rLrBohHWO7g', refreshToken: 'gSp+//erW9H3a/0htm2WRD7/8ibeFL0ZeomqqXVOfF+IJIwgYzqWWgDFufX3GGuCoDLwjxvNNHqW4t2em69Nng==', expiresAt: '2025-08-26T14:47:52.0832141Z', tokenType: 'Bearer', user: {…}, …}
tenant.service.ts:122 🔍 TenantService: API response received: {status: 200, hasData: true, dataKeys: Array(6), dataType: 'object'}
tenant.service.ts:129 🔍 TenantService: Full response data: {accessToken: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc…HAifQ.829_8AC6ockMjQQQVvGce6-dZa310zq2rLrBohHWO7g', refreshToken: 'gSp+//erW9H3a/0htm2WRD7/8ibeFL0ZeomqqXVOfF+IJIwgYzqWWgDFufX3GGuCoDLwjxvNNHqW4t2em69Nng==', expiresAt: '2025-08-26T14:47:52.0832141Z', tokenType: 'Bearer', user: {…}, …}
tenant.service.ts:134 🔍 TenantService: Token data analysis: {tokenData: {…}, hasAccessToken: true, hasRefreshToken: true, hasUser: true, hasTenant: true, …}
tenant.service.ts:145 ✅ TenantService: Valid token data found, storing tokens...
tenant.service.ts:159 🔧 TenantService: Token storage verification: {tokenChanged: true, refreshTokenChanged: true, newTokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', newRefreshTokenPreview: 'gSp+//erW9H3a/0htm2W...'}
tenant.service.ts:170 🔍 TenantService: NEW token tenant verification: {tenant_id: '18', tenant_name: 'Number Five', user_id: '6', expected_tenant_id: '18', tenant_id_matches: true}
tenant.service.ts:185 ✅ TenantService: New token has correct tenant_id!
TenantContext.tsx:253 ✅ TenantContext: Tenant switch API successful
TenantContext.tsx:256 🔍 TenantContext: Calling refreshAuth to reload user state...
AuthContext.tsx:151 🔍 AuthContext: Initializing authentication...
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
token.manager.ts:45 🔍 TokenManager: Refresh token info: {hasRefreshToken: true, refreshTokenLength: 88}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
AuthContext.tsx:157 🔍 AuthContext: Token check: {hasToken: true, hasRefreshToken: true, tokenExpired: false}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
AuthContext.tsx:219 🔍 AuthContext: Token valid, validating with backend...
auth.service.ts:84 🔍 AuthService: Validating token using /api/users/profile...
api.client.ts:44 🔍 API REQUEST (with JWT): GET /api/users/profile
 ✅ API RESPONSE: /api/users/profile 200
 🔍 API CLIENT: Detected .NET 9 ApiResponseDto structure: {success: true, message: '', hasData: true, errors: 0}
 🔍 API CLIENT: Unwrapped response for /api/users/profile : {id: 6, tenantId: 18, email: 'mccrearyforward@gmail.com', firstName: 'Joe Mad Dog', lastName: 'MacDaddy', …}
 ✅ AuthService: Token validation successful: {id: 6, tenantId: 18, email: 'mccrearyforward@gmail.com', firstName: 'Joe Mad Dog', lastName: 'MacDaddy', …}
 🔍 AuthContext: Extracting permissions from token: {tokenClaims: Array(16), permissions: Array(11)}
 🔍 AuthContext: Extracting roles from JWT token: {tokenClaims: Array(16), rolesRaw: 'Tenant Admin', rolesType: 'string'}
 ✅ AuthContext: Authentication initialization successful {user: 'mccrearyforward@gmail.com', permissions: Array(10), roles: Array(1)}
 ✅ TenantContext: refreshAuth completed successfully
 🏢 TenantContext: Selecting tenant: Number Five
 🔍 API REQUEST (with JWT): GET /api/tenants/18/settings
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting roles from JWT token: {tokenClaims: Array(16), rolesRaw: 'Tenant Admin', rolesType: 'string'}
 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
 🏢 TenantContext: Loading tenants for user: 6
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
N @ CreateAdditionalTenant.tsx:50
await in N
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
N @ CreateAdditionalTenant.tsx:50
await in N
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
N @ CreateAdditionalTenant.tsx:50
await in N
Fr @ react-dom-client.production.js:11858
(anonymous) @ react-dom-client.production.js:12410
af @ react-dom-client.production.js:1470
ss @ react-dom-client.production.js:11996
Ts @ react-dom-client.production.js:14699
fg @ react-dom-client.production.js:14667
TenantContext.tsx:338 🏢 TenantContext: Tenant selection complete: Number Five
TenantContext.tsx:263 🏢 TenantContext: Tenant switched successfully with new JWT: Number Five
UserMenu.tsx:191 ✅ New organization created: Number Five
TenantNavigationHandler.tsx:14 🔧 TenantNavigationHandler: Redirect flag detected, checking current route accessibility
TenantNavigationHandler.tsx:45 🔧 TenantNavigationHandler: Redirecting to dashboard due to insufficient permissions
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'roles.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
PermissionContext.tsx:126 🔍 PermissionContext: Permission check: {permission: 'users.view', hasPermission: true, allPermissions: Array(10)}
token.manager.ts:87 🔍 TokenManager: Token expiry check: {expiryTime: Tue Aug 19 2025 10:47:52 GMT-0500 (Central Daylight Time), currentTime: Tue Aug 19 2025 09:47:52 GMT-0500 (Central Daylight Time), isExpired: false}
token.manager.ts:26 🔍 TokenManager: Current token info: {hasToken: true, tokenLength: 1175, isExpired: false, tokenPreview: 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc...', claims: {…}}
PermissionContext.tsx:63 🔍 PermissionContext: Extracting permissions from JWT token: {tokenClaims: Array(16), permissions: Array(11)}
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
dashboard:1 Blocked aria-hidden on an element because its descendant retained focus. The focus must not be hidden from assistive technology users. Avoid using aria-hidden on a focused element or its ancestor. Consider using the inert attribute instead, which will also prevent focus. For more details, see the aria-hidden section of the WAI-ARIA specification at https://w3c.github.io/aria/#aria-hidden.
Element with focus: <button.MuiButtonBase-root MuiIconButton-root MuiIconButton-colorInherit MuiIconButton-sizeLarge css-1vpch0n>
Ancestor with aria-hidden: <div#root> <div id=​"root" aria-hidden=​"true">​…​</div>​
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
