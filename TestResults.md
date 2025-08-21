boiler-redis | 1:C 21 Aug 2025 20:37:18.907 * oO0OoO0OoO0Oo Redis is starting oO0OoO0OoO0Oo
boiler-user | 2025-08-21T20:37:29.989167136Z [20:37:29 INF] Starting UserService with Redis caching enabled {}
boiler-user | 2025-08-21T20:37:29.992101202Z === UserService Starting with Redis Caching ===
boiler-redis | 1:C 21 Aug 2025 20:37:18.907 * Redis version=7.4.5, bits=64, commit=00000000, modified=0, pid=1, just started
boiler-redis | 1:C 21 Aug 2025 20:37:18.907 # Warning: no config file specified, using the default config. In order to specify a config file use redis-server /path/to/redis.conf
boiler-redis | 1:M 21 Aug 2025 20:37:18.907 * monotonic clock: POSIX clock_gettime
boiler-redis | 1:M 21 Aug 2025 20:37:18.911 * Running mode=standalone, port=6379.
boiler-redis | 1:M 21 Aug 2025 20:37:18.912 * Server initialized
boiler-user | 2025-08-21T20:37:29.996346842Z [20:37:29 INF] Attempting to connect to Redis at: redis:6379 {"SourceContext": "Program"}
boiler-redis | 1:M 21 Aug 2025 20:37:18.912 * Loading RDB produced by version 7.4.5
boiler-redis | 1:M 21 Aug 2025 20:37:18.913 * RDB age 35 seconds
boiler-redis | 1:M 21 Aug 2025 20:37:18.913 * RDB memory usage when created 1.00 Mb
boiler-redis | 1:M 21 Aug 2025 20:37:18.913 * Done loading RDB, keys loaded: 1, keys expired: 0.
boiler-redis | 1:M 21 Aug 2025 20:37:18.913 * DB loaded from disk: 0.000 seconds
boiler-redis | 1:M 21 Aug 2025 20:37:18.913 * Ready to accept connections tcp
boiler-user | 2025-08-21T20:37:30.058623064Z [20:37:30 INF] Redis connection status: Connected to redis:6379 {"SourceContext": "Program"}
boiler-user | 2025-08-21T20:37:30.059304302Z [20:37:30 INF] Redis connection status: Connected {}
boiler-user | 2025-08-21T20:37:30.065613459Z ✅ Redis connection successful: 08/21/2025 20:37:30
boiler-user | 2025-08-21T20:37:30.066362402Z [20:37:30 INF] Redis connection test successful: 08/21/2025 20:37:30 {}
boiler-user | 2025-08-21T20:37:30.370832922Z [20:37:30 WRN] Entity 'Role' has a global query filter defined and is the required end of a relationship with the entity 'RolePermission'. This may lead to unexpected results when the required entity is filtered out. Either configure the navigation as optional, or define matching query filters for both entities in the navigation. See https://go.microsoft.com/fwlink/?linkid=2131316 for more information. {"EventId": {"Id": 10622, "Name": "Microsoft.EntityFrameworkCore.Model.Validation.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
boiler-user | 2025-08-21T20:37:30.372854936Z [20:37:30 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
boiler-user | 2025-08-21T20:37:30.625328915Z [20:37:30 WRN] Using an in-memory repository. Keys will not be persisted to storage. {"EventId": {"Id": 50, "Name": "UsingInMemoryRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository"}
boiler-user | 2025-08-21T20:37:30.625338716Z [20:37:30 WRN] Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits. {"EventId": {"Id": 59, "Name": "UsingEphemeralKeyRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-user | 2025-08-21T20:37:30.636041721Z [20:37:30 WRN] No XML encryptor configured. Key {c5f85d65-91e3-42fd-b91f-ceb2b9307fc5} may be persisted to storage in unencrypted form. {"EventId": {"Id": 35, "Name": "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-user | 2025-08-21T20:37:30.636076423Z [20:37:30 WRN] Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://0.0.0.0:5002;https://0.0.0.0:7002'. {"EventId": {"Id": 15}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics"}
boiler-user | 2025-08-21T20:37:30.746870589Z Now listening on: http://0.0.0.0:5002
boiler-user | 2025-08-21T20:37:30.746912592Z [20:37:30 INF] Now listening on: http://0.0.0.0:5002 {}
boiler-user | 2025-08-21T20:37:30.746915092Z Now listening on: https://0.0.0.0:7002
boiler-user | 2025-08-21T20:37:30.746939393Z [20:37:30 INF] Now listening on: https://0.0.0.0:7002 {}
boiler-user | 2025-08-21T20:37:34.923745566Z [20:37:34 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF0UA7LUBG0:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA7LUBG0"}
boiler-user | 2025-08-21T20:37:34.947542621Z [20:37:34 INF] HTTP GET /health responded 200 in 30.2453 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-user | 2025-08-21T20:38:05.034583036Z [20:38:05 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF0UA7LUBG1:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA7LUBG1"}
boiler-user | 2025-08-21T20:38:05.036427238Z [20:38:05 INF] HTTP GET /health responded 200 in 4.6313 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-user | 2025-08-21T20:38:35.191881087Z [20:38:35 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF0UA7LUBG2:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA7LUBG2"}
boiler-user | 2025-08-21T20:38:35.192124201Z [20:38:35 INF] HTTP GET /health responded 200 in 1.8776 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-user | 2025-08-21T20:39:05.274342310Z [20:39:05 DBG] AuthenticationScheme: Bearer was not authenticated. {"EventId": {"Id": 9, "Name": "AuthenticationSchemeNotAuthenticated"}, "SourceContext": "Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerHandler", "RequestId": "0HNF0UA7LUBG3:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA7LUBG3"}
boiler-user | 2025-08-21T20:39:05.274742534Z [20:39:05 INF] HTTP GET /health responded 200 in 1.0407 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-postgres | 
boiler-postgres | PostgreSQL Database directory appears to contain a database; Skipping initialization
boiler-postgres | 
boiler-postgres | 2025-08-21 20:37:18.993 UTC [1] LOG:  starting PostgreSQL 15.14 on aarch64-unknown-linux-musl, compiled by gcc (Alpine 14.2.0) 14.2.0, 64-bit
boiler-postgres | 2025-08-21 20:37:18.993 UTC [1] LOG:  listening on IPv4 address "0.0.0.0", port 5432
boiler-postgres | 2025-08-21 20:37:18.993 UTC [1] LOG:  listening on IPv6 address "::", port 5432
boiler-postgres | 2025-08-21 20:37:18.997 UTC [1] LOG:  listening on Unix socket "/var/run/postgresql/.s.PGSQL.5432"
boiler-postgres | 2025-08-21 20:37:19.004 UTC [29] LOG:  database system was shut down at 2025-08-21 20:36:43 UTC
boiler-postgres | 2025-08-21 20:37:19.012 UTC [1] LOG:  database system is ready to accept connections
boiler-gateway | 2025-08-21T20:37:36.081441812Z [20:37:36 INF] Starting API Gateway {}
boiler-gateway | 2025-08-21T20:37:36.081529117Z === API Gateway Starting ===
boiler-gateway | 2025-08-21T20:37:36.091657394Z [20:37:36 WRN] Using an in-memory repository. Keys will not be persisted to storage. {"EventId": {"Id": 50, "Name": "UsingInMemoryRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository"}
boiler-gateway | 2025-08-21T20:37:36.091702397Z [20:37:36 WRN] Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits. {"EventId": {"Id": 59, "Name": "UsingEphemeralKeyRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-gateway | 2025-08-21T20:37:36.102448309Z [20:37:36 WRN] No XML encryptor configured. Key {36f537b6-8f18-48ff-91bb-69ab2ddbb426} may be persisted to storage in unencrypted form. {"EventId": {"Id": 35, "Name": "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-gateway | 2025-08-21T20:37:36.106071915Z [20:37:36 WRN] Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://0.0.0.0:5000;https://0.0.0.0:7000'. {"EventId": {"Id": 15}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics"}
boiler-gateway | 2025-08-21T20:37:36.225643727Z Now listening on: http://0.0.0.0:5000
boiler-gateway | 2025-08-21T20:37:36.226294964Z [20:37:36 INF] Now listening on: http://0.0.0.0:5000 {}
boiler-gateway | 2025-08-21T20:37:36.226318165Z Now listening on: https://0.0.0.0:7000
boiler-gateway | 2025-08-21T20:37:36.226328666Z [20:37:36 INF] Now listening on: https://0.0.0.0:7000 {}
boiler-gateway | 2025-08-21T20:37:40.796187083Z [20:37:40 INF] REQUEST 38847a06: GET /health from 127.0.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD1:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA9E8AD1"}
boiler-gateway | 2025-08-21T20:37:40.804214540Z 🔧 JWT Settings: Issuer=AuthService, Audience=StarterApp, SecretKey=super-secr...
boiler-gateway | 2025-08-21T20:37:40.804263143Z 🔧 JWT Settings: SecretKey Length=85, Key Valid=True
boiler-gateway | 2025-08-21T20:37:40.805780929Z 🔧 JWT Key Info: Key Size=85 bytes, Min Required=256 bits (32 bytes)
boiler-gateway | 2025-08-21T20:37:40.809465639Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-21T20:37:40.809492841Z    Path: /health
boiler-gateway | 2025-08-21T20:37:40.813162950Z    Method: GET
boiler-gateway | 2025-08-21T20:37:40.813202152Z    Authorization Header: MISSING
boiler-gateway | 2025-08-21T20:37:40.839823868Z [20:37:40 INF] RESPONSE 38847a06: 200 in 45ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD1:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA9E8AD1"}
boiler-gateway | 2025-08-21T20:37:40.848737476Z [20:37:40 INF] HTTP GET /health responded 200 in 48.2830 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD1:00000001", "ConnectionId": "0HNF0UA9E8AD1"}
boiler-gateway | 2025-08-21T20:38:10.957015651Z [20:38:10 INF] REQUEST 5dd4fbdf: GET /health from 127.0.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD2:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA9E8AD2"}
boiler-gateway | 2025-08-21T20:38:10.958641842Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-21T20:38:10.958680945Z    Path: /health
boiler-gateway | 2025-08-21T20:38:10.958683945Z    Method: GET
boiler-gateway | 2025-08-21T20:38:10.958686545Z    Authorization Header: MISSING
boiler-gateway | 2025-08-21T20:38:10.960022720Z [20:38:10 INF] RESPONSE 5dd4fbdf: 200 in 3ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD2:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA9E8AD2"}
boiler-gateway | 2025-08-21T20:38:10.960153027Z [20:38:10 INF] HTTP GET /health responded 200 in 3.7808 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD2:00000001", "ConnectionId": "0HNF0UA9E8AD2"}
boiler-gateway | 2025-08-21T20:38:41.050466458Z [20:38:41 INF] REQUEST 9c1614b1: GET /health from 127.0.0.1 | Tenant: Unknown | User: Anonymous {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD3:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA9E8AD3"}
boiler-gateway | 2025-08-21T20:38:41.051611523Z 🔍 JWT OnMessageReceived:
boiler-gateway | 2025-08-21T20:38:41.051625423Z    Path: /health
boiler-gateway | 2025-08-21T20:38:41.051628424Z    Method: GET
boiler-gateway | 2025-08-21T20:38:41.051631124Z    Authorization Header: MISSING
boiler-gateway | 2025-08-21T20:38:41.052055048Z [20:38:41 INF] RESPONSE 9c1614b1: 200 in 1ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD3:00000001", "RequestPath": "/health", "ConnectionId": "0HNF0UA9E8AD3"}
boiler-gateway | 2025-08-21T20:38:41.052094450Z [20:38:41 INF] HTTP GET /health responded 200 in 2.0457 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNF0UA9E8AD3:00000001", "ConnectionId": "0HNF0UA9E8AD3"}
boiler-pgadmin | 2025-08-21T20:37:19.388773207Z postfix/postlog: starting the Postfix mail system
boiler-pgadmin | 2025-08-21T20:37:22.122057488Z /venv/lib/python3.12/site-packages/passlib/pwd.py:16: UserWarning: pkg_resources is deprecated as an API. See https://setuptools.pypa.io/en/latest/pkg_resources.html. The pkg_resources package is slated for removal as early as 2025-11-30. Refrain from using this package or pin to Setuptools<81.
boiler-pgadmin | 2025-08-21T20:37:22.122111291Z   import pkg_resources
boiler-pgadmin | 2025-08-21T20:37:26.390400499Z /venv/lib/python3.12/site-packages/passlib/pwd.py:16: UserWarning: pkg_resources is deprecated as an API. See https://setuptools.pypa.io/en/latest/pkg_resources.html. The pkg_resources package is slated for removal as early as 2025-11-30. Refrain from using this package or pin to Setuptools<81.
boiler-pgadmin | 2025-08-21T20:37:26.390441302Z   import pkg_resources
boiler-pgadmin | 2025-08-21T20:37:28.005235130Z [2025-08-21 20:37:28 +0000] [1] [INFO] Starting gunicorn 23.0.0
boiler-pgadmin | 2025-08-21T20:37:28.005905068Z [2025-08-21 20:37:28 +0000] [1] [INFO] Listening at: http://[::]:80 (1)
boiler-pgadmin | 2025-08-21T20:37:28.005910768Z [2025-08-21 20:37:28 +0000] [1] [INFO] Using worker: gthread
boiler-pgadmin | 2025-08-21T20:37:28.014050229Z [2025-08-21 20:37:28 +0000] [88] [INFO] Booting worker with pid: 88
boiler-auth |   | 2025-08-21T20:37:30.013075388Z [20:37:30 INF] Starting AuthService {}
boiler-auth |   | 2025-08-21T20:37:30.015653634Z === AuthService Starting ===
boiler-auth |   | 2025-08-21T20:37:30.324187483Z [20:37:30 WRN] Entity 'Role' has a global query filter defined and is the required end of a relationship with the entity 'RolePermission'. This may lead to unexpected results when the required entity is filtered out. Either configure the navigation as optional, or define matching query filters for both entities in the navigation. See https://go.microsoft.com/fwlink/?linkid=2131316 for more information. {"EventId": {"Id": 10622, "Name": "Microsoft.EntityFrameworkCore.Model.Validation.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
boiler-auth |   | 2025-08-21T20:37:30.327089548Z [20:37:30 WRN] Sensitive data logging is enabled. Log entries and exception messages may include sensitive application data; this mode should only be enabled during development. {"EventId": {"Id": 10400, "Name": "Microsoft.EntityFrameworkCore.Infrastructure.SensitiveDataLoggingEnabledWarning"}, "SourceContext": "Microsoft.EntityFrameworkCore.Model.Validation"}
boiler-auth |   | 2025-08-21T20:37:30.617971099Z [20:37:30 WRN] Using an in-memory repository. Keys will not be persisted to storage. {"EventId": {"Id": 50, "Name": "UsingInMemoryRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.Repositories.EphemeralXmlRepository"}
boiler-auth |   | 2025-08-21T20:37:30.618041003Z [20:37:30 WRN] Neither user profile nor HKLM registry available. Using an ephemeral key repository. Protected data will be unavailable when application exits. {"EventId": {"Id": 59, "Name": "UsingEphemeralKeyRepository"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-auth |   | 2025-08-21T20:37:30.624962994Z [20:37:30 WRN] No XML encryptor configured. Key {d5d931e2-ec92-44f8-a7b4-8baa524a27b8} may be persisted to storage in unencrypted form. {"EventId": {"Id": 35, "Name": "NoXMLEncryptorConfiguredKeyMayBePersistedToStorageInUnencryptedForm"}, "SourceContext": "Microsoft.AspNetCore.DataProtection.KeyManagement.XmlKeyManager"}
boiler-auth |   | 2025-08-21T20:37:30.628115073Z [20:37:30 WRN] Overriding HTTP_PORTS '8080' and HTTPS_PORTS ''. Binding to values defined by URLS instead 'http://0.0.0.0:5001;https://0.0.0.0:7001'. {"EventId": {"Id": 15}, "SourceContext": "Microsoft.AspNetCore.Hosting.Diagnostics"}
boiler-auth |   | 2025-08-21T20:37:30.731063395Z Now listening on: http://0.0.0.0:5001
boiler-auth |   | 2025-08-21T20:37:30.731670029Z [20:37:30 INF] Now listening on: http://0.0.0.0:5001 {}
boiler-auth |   | 2025-08-21T20:37:30.731697831Z Now listening on: https://0.0.0.0:7001
boiler-auth |   | 2025-08-21T20:37:30.731700031Z [20:37:30 INF] Now listening on: https://0.0.0.0:7001 {}
boiler-auth |   | 2025-08-21T20:37:34.936678502Z [20:37:34 INF] HTTP GET /health responded 200 in 21.3298 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth |   | 2025-08-21T20:38:05.035172769Z [20:38:05 INF] HTTP GET /health responded 200 in 3.4931 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth |   | 2025-08-21T20:38:35.192349413Z [20:38:35 INF] HTTP GET /health responded 200 in 1.7586 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-auth |   | 2025-08-21T20:39:05.274003590Z [20:39:05 INF] HTTP GET /health responded 200 in 0.3881 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-frontend | /docker-entrypoint.sh: /docker-entrypoint.d/ is not empty, will attempt to perform configuration
boiler-frontend | /docker-entrypoint.sh: Looking for shell scripts in /docker-entrypoint.d/
boiler-frontend | /docker-entrypoint.sh: Launching /docker-entrypoint.d/10-listen-on-ipv6-by-default.sh
boiler-frontend | 10-listen-on-ipv6-by-default.sh: info: Getting the checksum of /etc/nginx/conf.d/default.conf
boiler-frontend | 10-listen-on-ipv6-by-default.sh: info: /etc/nginx/conf.d/default.conf differs from the packaged version
boiler-frontend | /docker-entrypoint.sh: Sourcing /docker-entrypoint.d/15-local-resolvers.envsh
boiler-frontend | /docker-entrypoint.sh: Launching /docker-entrypoint.d/20-envsubst-on-templates.sh
boiler-frontend | /docker-entrypoint.sh: Launching /docker-entrypoint.d/30-tune-worker-processes.sh
boiler-frontend | /docker-entrypoint.sh: Configuration complete; ready for start up
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: using the "epoll" event method
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: nginx/1.29.1
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: built by gcc 14.2.0 (Alpine 14.2.0) 
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: OS: Linux 6.6.87.2-microsoft-standard-WSL2
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: getrlimit(RLIMIT_NOFILE): 1048576:1048576
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker processes
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 29
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 30
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 31
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 32
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 33
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 34
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 35
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 36
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 37
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 38
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 39
boiler-frontend | 2025/08/21 20:37:35 [notice] 1#1: start worker process 40
