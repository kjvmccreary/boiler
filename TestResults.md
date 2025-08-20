boiler-postgres | 2025-08-19 23:21:21.571 UTC [41] ERROR:  null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-postgres | 2025-08-19 23:21:21.571 UTC [41] DETAIL:  Failing row contains (646, 6, 9zrIzCrkfYx3BYfzYMRNycka1RZiFW/giZUPzHzA66dRhCRozzDmlHwm7oZSMij6..., 2025-08-26 23:21:21.471059+00, f, null, null, null, 127.0.0.1, null, 2025-08-19 23:21:21.507458+00, 2025-08-19 23:21:21.507458+00, null).
boiler-postgres | 2025-08-19 23:21:21.571 UTC [41] STATEMENT:  INSERT INTO "RefreshTokens" ("CreatedAt", "CreatedByIp", "DeviceInfo", "ExpiryDate", "IsRevoked", "ReplacedByToken", "RevokedAt", "RevokedByIp", "TenantId", "Token", "UpdatedAt", "UserId")
boiler-postgres |   VALUES ($1, $2, $3, $4, $5, $6, $7, $8, $9, $10, $11, $12)
boiler-postgres |   RETURNING "Id"
boiler-auth |   | 2025-08-19T23:21:21.581774317Z [23:21:21 ERR] Failed executing DbCommand (11ms) [Parameters=[@p0='2025-08-19T23:21:21.5074582Z' (DbType = DateTime), @p1='127.0.0.1' (Nullable = false), @p2=NULL, @p3='2025-08-26T23:21:21.4710594Z' (DbType = DateTime), @p4='False', @p5=NULL, @p6=NULL (DbType = DateTime), @p7=NULL, @p8=NULL (Nullable = false) (DbType = Int32), @p9='9zrIzCrkfYx3BYfzYMRNycka1RZiFW/giZUPzHzA66dRhCRozzDmlHwm7oZSMij6awRIg6EeE7Mp3/sw/5QIIA==' (Nullable = false), @p10='2025-08-19T23:21:21.5074582Z' (DbType = DateTime), @p11='6', @p14='6', @p12='2025-08-19T23:21:21.4572383Z' (Nullable = true) (DbType = DateTime), @p13='2025-08-19T23:21:21.5074582Z' (DbType = DateTime)], CommandType='Text', CommandTimeout='30']
boiler-auth |   | 2025-08-19T23:21:21.581812119Z INSERT INTO "RefreshTokens" ("CreatedAt", "CreatedByIp", "DeviceInfo", "ExpiryDate", "IsRevoked", "ReplacedByToken", "RevokedAt", "RevokedByIp", "TenantId", "Token", "UpdatedAt", "UserId")
boiler-auth |   | 2025-08-19T23:21:21.581815520Z VALUES (@p0, @p1, @p2, @p3, @p4, @p5, @p6, @p7, @p8, @p9, @p10, @p11)
boiler-auth |   | 2025-08-19T23:21:21.581817720Z RETURNING "Id";
boiler-auth |   | 2025-08-19T23:21:21.581819720Z UPDATE "Users" SET "LastLoginAt" = @p12, "UpdatedAt" = @p13
boiler-auth |   | 2025-08-19T23:21:21.581821920Z WHERE "Id" = @p14; {"EventId": {"Id": 20102, "Name": "Microsoft.EntityFrameworkCore.Database.Command.CommandError"}, "SourceContext": "Microsoft.EntityFrameworkCore.Database.Command", "ActionId": "306988c0-732c-4948-a3fe-c0c78066c58a", "ActionName": "AuthService.Controllers.AuthController.Login (AuthService)", "RequestId": "0HNEVERO8AQTA:00000001", "RequestPath": "/api/auth/login", "ConnectionId": "0HNEVERO8AQTA"}
boiler-auth |   | 2025-08-19T23:21:21.598793690Z [23:21:21 ERR] An exception occurred in the database while saving changes for context type 'Common.Data.ApplicationDbContext'.
boiler-auth |   | 2025-08-19T23:21:21.598832192Z Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
boiler-auth |   | 2025-08-19T23:21:21.598834892Z  ---> Npgsql.PostgresException (0x80004005): 23502: null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-auth |   | 2025-08-19T23:21:21.598837992Z 
boiler-auth |   | 2025-08-19T23:21:21.598839893Z DETAIL: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
boiler-auth |   | 2025-08-19T23:21:21.598841993Z    at Npgsql.Internal.NpgsqlConnector.ReadMessageLong(Boolean async, DataRowLoadingMode dataRowLoadingMode, Boolean readingNotifications, Boolean isReadingPrependedMessage)
boiler-auth |   | 2025-08-19T23:21:21.598844093Z    at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
boiler-auth |   | 2025-08-19T23:21:21.598846893Z    at Npgsql.NpgsqlDataReader.NextResult(Boolean async, Boolean isConsuming, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598848893Z    at Npgsql.NpgsqlDataReader.NextResult(Boolean async, Boolean isConsuming, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598850893Z    at Npgsql.NpgsqlCommand.ExecuteReader(Boolean async, CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598852993Z    at Npgsql.NpgsqlCommand.ExecuteReader(Boolean async, CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598854994Z    at Npgsql.NpgsqlCommand.ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598857194Z    at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598859394Z    at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598861694Z    at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598863794Z   Exception data:
boiler-auth |   | 2025-08-19T23:21:21.598865694Z     Severity: ERROR
boiler-auth |   | 2025-08-19T23:21:21.598871295Z     SqlState: 23502
boiler-auth |   | 2025-08-19T23:21:21.598874995Z     MessageText: null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-auth |   | 2025-08-19T23:21:21.598877195Z     Detail: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
boiler-auth |   | 2025-08-19T23:21:21.598890496Z     SchemaName: public
boiler-auth |   | 2025-08-19T23:21:21.598892496Z     TableName: RefreshTokens
boiler-auth |   | 2025-08-19T23:21:21.598894396Z     ColumnName: TenantId
boiler-auth |   | 2025-08-19T23:21:21.598896596Z     File: execMain.c
boiler-auth |   | 2025-08-19T23:21:21.598898596Z     Line: 1971
boiler-auth |   | 2025-08-19T23:21:21.598900496Z     Routine: ExecConstraints
boiler-auth |   | 2025-08-19T23:21:21.598902396Z    --- End of inner exception stack trace ---
boiler-auth |   | 2025-08-19T23:21:21.598904397Z    at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598906597Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598908597Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598910797Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598912997Z    at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598914997Z    at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598917097Z    at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598919298Z    at Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598921798Z    at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken) {"EventId": {"Id": 10000, "Name": "Microsoft.EntityFrameworkCore.Update.SaveChangesFailed"}, "SourceContext": "Microsoft.EntityFrameworkCore.Update", "ActionId": "306988c0-732c-4948-a3fe-c0c78066c58a", "ActionName": "AuthService.Controllers.AuthController.Login (AuthService)", "RequestId": "0HNEVERO8AQTA:00000001", "RequestPath": "/api/auth/login", "ConnectionId": "0HNEVERO8AQTA"}
boiler-auth |   | 2025-08-19T23:21:21.598924898Z Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
boiler-auth |   | 2025-08-19T23:21:21.598926998Z  ---> Npgsql.PostgresException (0x80004005): 23502: null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-auth |   | 2025-08-19T23:21:21.598929298Z 
boiler-auth |   | 2025-08-19T23:21:21.598931498Z DETAIL: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
boiler-auth |   | 2025-08-19T23:21:21.598933498Z    at Npgsql.Internal.NpgsqlConnector.ReadMessageLong(Boolean async, DataRowLoadingMode dataRowLoadingMode, Boolean readingNotifications, Boolean isReadingPrependedMessage)
boiler-auth |   | 2025-08-19T23:21:21.598937099Z    at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
boiler-auth |   | 2025-08-19T23:21:21.598939299Z    at Npgsql.NpgsqlDataReader.NextResult(Boolean async, Boolean isConsuming, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598941299Z    at Npgsql.NpgsqlDataReader.NextResult(Boolean async, Boolean isConsuming, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598943299Z    at Npgsql.NpgsqlCommand.ExecuteReader(Boolean async, CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598945499Z    at Npgsql.NpgsqlCommand.ExecuteReader(Boolean async, CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598947499Z    at Npgsql.NpgsqlCommand.ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598949499Z    at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598951600Z    at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598954600Z    at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598956600Z   Exception data:
boiler-auth |   | 2025-08-19T23:21:21.598958600Z     Severity: ERROR
boiler-auth |   | 2025-08-19T23:21:21.598960500Z     SqlState: 23502
boiler-auth |   | 2025-08-19T23:21:21.598962400Z     MessageText: null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-auth |   | 2025-08-19T23:21:21.598964400Z     Detail: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
boiler-auth |   | 2025-08-19T23:21:21.598966901Z     SchemaName: public
boiler-auth |   | 2025-08-19T23:21:21.598968801Z     TableName: RefreshTokens
boiler-auth |   | 2025-08-19T23:21:21.598970801Z     ColumnName: TenantId
boiler-auth |   | 2025-08-19T23:21:21.598972701Z     File: execMain.c
boiler-auth |   | 2025-08-19T23:21:21.598974601Z     Line: 1971
boiler-auth |   | 2025-08-19T23:21:21.598976501Z     Routine: ExecConstraints
boiler-auth |   | 2025-08-19T23:21:21.598978401Z    --- End of inner exception stack trace ---
boiler-auth |   | 2025-08-19T23:21:21.598980401Z    at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598982402Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598984602Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598986802Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598990002Z    at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598992202Z    at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598994302Z    at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598996502Z    at Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.598999303Z    at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601544263Z [23:21:21 ERR] Error during login for mccrearyforward@gmail.com {"SourceContext": "AuthService.Services.AuthServiceImplementation", "ActionId": "306988c0-732c-4948-a3fe-c0c78066c58a", "ActionName": "AuthService.Controllers.AuthController.Login (AuthService)", "RequestId": "0HNEVERO8AQTA:00000001", "RequestPath": "/api/auth/login", "ConnectionId": "0HNEVERO8AQTA"}
boiler-auth |   | 2025-08-19T23:21:21.601624068Z Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
boiler-auth |   | 2025-08-19T23:21:21.601628268Z  ---> Npgsql.PostgresException (0x80004005): 23502: null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-auth |   | 2025-08-19T23:21:21.601630968Z 
boiler-auth |   | 2025-08-19T23:21:21.601632969Z DETAIL: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
boiler-auth |   | 2025-08-19T23:21:21.601635069Z    at Npgsql.Internal.NpgsqlConnector.ReadMessageLong(Boolean async, DataRowLoadingMode dataRowLoadingMode, Boolean readingNotifications, Boolean isReadingPrependedMessage)
boiler-auth |   | 2025-08-19T23:21:21.601637269Z    at System.Runtime.CompilerServices.PoolingAsyncValueTaskMethodBuilder`1.StateMachineBox`1.System.Threading.Tasks.Sources.IValueTaskSource<TResult>.GetResult(Int16 token)
boiler-auth |   | 2025-08-19T23:21:21.601639569Z    at Npgsql.NpgsqlDataReader.NextResult(Boolean async, Boolean isConsuming, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601641569Z    at Npgsql.NpgsqlDataReader.NextResult(Boolean async, Boolean isConsuming, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601643569Z    at Npgsql.NpgsqlCommand.ExecuteReader(Boolean async, CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601645669Z    at Npgsql.NpgsqlCommand.ExecuteReader(Boolean async, CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601647769Z    at Npgsql.NpgsqlCommand.ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601649970Z    at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601671671Z    at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601674071Z    at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601676171Z   Exception data:
boiler-auth |   | 2025-08-19T23:21:21.601678171Z     Severity: ERROR
boiler-auth |   | 2025-08-19T23:21:21.601679972Z     SqlState: 23502
boiler-auth |   | 2025-08-19T23:21:21.601682272Z     MessageText: null value in column "TenantId" of relation "RefreshTokens" violates not-null constraint
boiler-auth |   | 2025-08-19T23:21:21.601684372Z     Detail: Detail redacted as it may contain sensitive data. Specify 'Include Error Detail' in the connection string to include this information.
boiler-auth |   | 2025-08-19T23:21:21.601686472Z     SchemaName: public
boiler-auth |   | 2025-08-19T23:21:21.601688572Z     TableName: RefreshTokens
boiler-auth |   | 2025-08-19T23:21:21.601690672Z     ColumnName: TenantId
boiler-auth |   | 2025-08-19T23:21:21.601692472Z     File: execMain.c
boiler-auth |   | 2025-08-19T23:21:21.601694472Z     Line: 1971
boiler-auth |   | 2025-08-19T23:21:21.601696373Z     Routine: ExecConstraints
boiler-auth |   | 2025-08-19T23:21:21.601698273Z    --- End of inner exception stack trace ---
boiler-auth |   | 2025-08-19T23:21:21.601700473Z    at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601702973Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601705073Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601707173Z    at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601709373Z    at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601711374Z    at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601713574Z    at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601716074Z    at Npgsql.EntityFrameworkCore.PostgreSQL.Storage.Internal.NpgsqlExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601718274Z    at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601720474Z    at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
boiler-auth |   | 2025-08-19T23:21:21.601723774Z    at Common.Data.ApplicationDbContext.SaveChangesAsync(CancellationToken cancellationToken) in /src/shared/Common/Data/ApplicationDbContext.cs:line 200
boiler-auth |   | 2025-08-19T23:21:21.601725974Z    at AuthService.Services.AuthServiceImplementation.LoginAsync(LoginRequestDto request, CancellationToken cancellationToken) in /src/services/AuthService/Services/AuthService.cs:line 255
boiler-auth |   | 2025-08-19T23:21:21.618962861Z [23:21:21 INF] HTTP POST /api/auth/login responded 400 in 1216.3572 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware"}
boiler-gateway | 2025-08-19T23:21:21.621232004Z [23:21:21 WRN] requestId: 0HNEVERQ17GQP:00000001, previousRequestId: No PreviousRequestId, message: '400 (Bad Request) status code of request URI: https://boiler-auth:7001/api/auth/login.' {"SourceContext": "Ocelot.Requester.Middleware.HttpRequesterMiddleware", "RequestId": "0HNEVERQ17GQP:00000001", "RequestPath": "/api/auth/login", "ConnectionId": "0HNEVERQ17GQP"}
boiler-gateway | 2025-08-19T23:21:21.628578967Z [23:21:21 INF] RESPONSE d92734dc: 400 in 1296ms | Size: Unknown {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEVERQ17GQP:00000001", "RequestPath": "/api/auth/login", "ConnectionId": "0HNEVERQ17GQP"}
boiler-gateway | 2025-08-19T23:21:21.629066897Z [23:21:21 WRN] SLOW REQUEST d92734dc: POST /api/auth/login took 1296ms {"SourceContext": "ApiGateway.Middleware.RequestLoggingMiddleware", "RequestId": "0HNEVERQ17GQP:00000001", "RequestPath": "/api/auth/login", "ConnectionId": "0HNEVERQ17GQP"}
boiler-gateway | 2025-08-19T23:21:21.629108100Z [23:21:21 INF] HTTP POST /api/auth/login responded 400 in 1297.7651 ms {"SourceContext": "Serilog.AspNetCore.RequestLoggingMiddleware", "RequestId": "0HNEVERQ17GQP:00000001", "ConnectionId": "0HNEVERQ17GQP"}
boiler-frontend | 172.18.0.1 - - [19/Aug/2025:23:21:21 +0000] "POST /api/auth/login HTTP/1.1" 400 111 "https://localhost:3000/login" "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36" "-"
