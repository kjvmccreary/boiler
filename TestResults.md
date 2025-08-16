
PS C:\Users\mccre\dev\boiler> docker-compose -f docker/docker-compose.yml ps
>>
NAME              IMAGE                   COMMAND                  SERVICE        CREATED         STATUS                   PORTS
boiler-auth       docker-auth-service     "dotnet AuthService.…"   auth-service   4 minutes ago   Up 4 minutes (healthy)   0.0.0.0:5001->5001/tcp, [::]:5001->5001/tcp, 0.0.0.0:7001->7001/tcp, [::]:7001->7001/tcp
boiler-frontend   docker-frontend         "/docker-entrypoint.…"   frontend       4 minutes ago   Up 4 minutes (healthy)   0.0.0.0:3080->80/tcp, [::]:3080->80/tcp, 0.0.0.0:3000->443/tcp, [::]:3000->443/tcp
boiler-gateway    docker-api-gateway      "dotnet ApiGateway.d…"   api-gateway    4 minutes ago   Up 4 minutes (healthy)   0.0.0.0:5000->5000/tcp, [::]:5000->5000/tcp, 0.0.0.0:7000->7000/tcp, [::]:7000->7000/tcp
boiler-pgadmin    dpage/pgadmin4:latest   "/entrypoint.sh"         pgadmin        4 minutes ago   Up 4 minutes             0.0.0.0:8080->80/tcp, [::]:8080->80/tcp
boiler-postgres   postgres:15-alpine      "docker-entrypoint.s…"   postgres       4 minutes ago   Up 4 minutes (healthy)   0.0.0.0:5432->5432/tcp, [::]:5432->5432/tcp
boiler-redis      redis:7-alpine          "docker-entrypoint.s…"   redis          4 minutes ago   Up 4 minutes (healthy)   0.0.0.0:6379->6379/tcp, [::]:6379->6379/tcp
boiler-user       docker-user-service     "dotnet UserService.…"   user-service   4 minutes ago   Up 4 minutes (healthy)   0.0.0.0:5002->5002/tcp, [::]:5002->5002/tcp, 0.0.0.0:7002->7002/tcp, [::]:7002->7002/tcp

C:\Users\mccre\dev\boiler\src\frontend\react-app>docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
NAMES             STATUS                   PORTS
boiler-frontend   Up 4 minutes (healthy)   0.0.0.0:3080->80/tcp, [::]:3080->80/tcp, 0.0.0.0:3000->443/tcp, [::]:3000->443/tcp
boiler-gateway    Up 4 minutes (healthy)   0.0.0.0:5000->5000/tcp, [::]:5000->5000/tcp, 0.0.0.0:7000->7000/tcp, [::]:7000->7000/tcp
boiler-pgadmin    Up 4 minutes             0.0.0.0:8080->80/tcp, [::]:8080->80/tcp
boiler-auth       Up 4 minutes (healthy)   0.0.0.0:5001->5001/tcp, [::]:5001->5001/tcp, 0.0.0.0:7001->7001/tcp, [::]:7001->7001/tcp
boiler-user       Up 4 minutes (healthy)   0.0.0.0:5002->5002/tcp, [::]:5002->5002/tcp, 0.0.0.0:7002->7002/tcp, [::]:7002->7002/tcp
boiler-redis      Up 4 minutes (healthy)   0.0.0.0:6379->6379/tcp, [::]:6379->6379/tcp
boiler-postgres   Up 4 minutes (healthy)   0.0.0.0:5432->5432/tcp, [::]:5432->5432/tcp

C:\Users\mccre\dev\boiler>docker-compose -f docker/docker-compose.yml logs auth-service
boiler-auth  | [20:37:54 INF] Starting AuthService {}
boiler-auth  | === AuthService Starting ===
boiler-auth  | [20:37:54 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
boiler-auth  | [20:37:54 WRN] Using an in-memory repository. Keys will not be persisted to storage. {"EventId": {"Id": 50, "Name": "UsingInMemoryRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository"}
boiler-auth  | [20:37:54 WRN] Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits. {"EventId": {"Id": 59, "Name": "UsingEphemeralKeyRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-auth  | [20:37:54 WRN] No XML encryptor configured. Key {fa54ec12-5899-4489-a71a-884a574e7747} may be persisted to storage in unencrypted form. {"EventId": {"Id": 35, "Name": "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-auth  | [20:37:54 WRN] Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://0.0.0.0:5001;https://0.0.0.0:7001'. {"EventId": {"Id": 15}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics"}
boiler-auth  | Now listening on: http://0.0.0.0:5001
boiler-auth  | [20:37:55 INF] Now listening on: http://0.0.0.0:5001 {}
boiler-auth  | Now listening on: https://0.0.0.0:7001
boiler-auth  | [20:37:55 INF] Now listening on: https://0.0.0.0:7001 {}
boiler-auth  | [20:37:59 INF] HTTP GET /health responded 200 in 21.3794 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:38:29 INF] HTTP GET /health responded 200 in 2.2555 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:38:59 INF] HTTP GET /health responded 200 in 11.3222 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:39:29 INF] HTTP GET /health responded 200 in 0.4303 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:39:59 INF] HTTP GET /health responded 200 in 0.3264 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:40:29 INF] HTTP GET /health responded 200 in 0.3261 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:40:59 INF] HTTP GET /health responded 200 in 0.2155 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:41:30 INF] HTTP GET /health responded 200 in 0.2090 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:42:00 INF] HTTP GET /health responded 200 in 0.1945 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:42:30 INF] HTTP GET /health responded 200 in 0.1828 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:43:00 INF] HTTP GET /health responded 200 in 0.3023 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:43:30 INF] HTTP GET /health responded 200 in 0.2008 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}

C:\Users\mccre\dev\boiler>docker-compose -f docker/docker-compose.yml logs --tail 20 auth-service
boiler-auth  | [20:37:54 WRN] Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits. {"EventId": {"Id": 59, "Name": "UsingEphemeralKeyRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-auth  | [20:37:54 WRN] No XML encryptor configured. Key {fa54ec12-5899-4489-a71a-884a574e7747} may be persisted to storage in unencrypted form. {"EventId": {"Id": 35, "Name": "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-auth  | [20:37:54 WRN] Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://0.0.0.0:5001;https://0.0.0.0:7001'. {"EventId": {"Id": 15}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics"}
boiler-auth  | Now listening on: http://0.0.0.0:5001
boiler-auth  | [20:37:55 INF] Now listening on: http://0.0.0.0:5001 {}
boiler-auth  | Now listening on: https://0.0.0.0:7001
boiler-auth  | [20:37:55 INF] Now listening on: https://0.0.0.0:7001 {}
boiler-auth  | [20:37:59 INF] HTTP GET /health responded 200 in 21.3794 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:38:29 INF] HTTP GET /health responded 200 in 2.2555 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:38:59 INF] HTTP GET /health responded 200 in 11.3222 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:39:29 INF] HTTP GET /health responded 200 in 0.4303 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:39:59 INF] HTTP GET /health responded 200 in 0.3264 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:40:29 INF] HTTP GET /health responded 200 in 0.3261 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:40:59 INF] HTTP GET /health responded 200 in 0.2155 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:41:30 INF] HTTP GET /health responded 200 in 0.2090 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:42:00 INF] HTTP GET /health responded 200 in 0.1945 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:42:30 INF] HTTP GET /health responded 200 in 0.1828 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:43:00 INF] HTTP GET /health responded 200 in 0.3023 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:43:30 INF] HTTP GET /health responded 200 in 0.2008 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth  | [20:44:00 INF] HTTP GET /health responded 200 in 0.2732 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}

C:\Users\mccre\dev\boiler>docker-compose -f docker/docker-compose.yml logs postgres
boiler-postgres  |
boiler-postgres  | PostgreSQL Database directory appears to contain a database; Skipping initialization
boiler-postgres  |
boiler-postgres  | 2025-08-16 20:37:43.192 UTC [1] LOG:  starting PostgreSQL 15.14 on aarch64-unknown-linux-musl, compiled by gcc (Alpine 14.2.0) 14.2.0, 64-bit
boiler-postgres  | 2025-08-16 20:37:43.192 UTC [1] LOG:  listening on IPv4 address "0.0.0.0", port 5432
boiler-postgres  | 2025-08-16 20:37:43.192 UTC [1] LOG:  listening on IPv6 address "::", port 5432
boiler-postgres  | 2025-08-16 20:37:43.197 UTC [1] LOG:  listening on Unix socket "/var/run/postgresql/.s.PGSQL.5432"
boiler-postgres  | 2025-08-16 20:37:43.205 UTC [30] LOG:  database system was shut down at 2025-08-16 20:37:19 UTC
boiler-postgres  | 2025-08-16 20:37:43.212 UTC [1] LOG:  database system is ready to accept connections
boiler-postgres  | 2025-08-16 20:42:43.273 UTC [28] LOG:  checkpoint starting: time
boiler-postgres  | 2025-08-16 20:42:43.299 UTC [28] LOG:  checkpoint complete: wrote 3 buffers (0.0%); 0 WAL file(s) added, 0 removed, 0 recycled; write=0.006 s, sync=0.003 s, total=0.026 s; sync files=2, longest=0.002 s, average=0.002 s; distance=0 kB, estimate=0 kB

C:\Users\mccre\dev\boiler>docker exec boiler-auth curl -f http://localhost:5001/health
  % Total    % Received % Xferd  Average Speed   Time    Time     Time  Current
                                 Dload  Upload   Total   Spent    Left  Speed
  0     0    0     0    0     0      0      0 --:--:-- --:--:-- --:--:--     0
