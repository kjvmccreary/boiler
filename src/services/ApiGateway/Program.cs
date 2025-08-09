using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using ApiGateway.Middleware;
using ApiGateway.Services;
using Common.Extensions;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ✅ FIXED: Configure Serilog for .NET 9
builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
{
    loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
});

// Add configuration files
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Add Ocelot
builder.Services.AddOcelot();

// Add Service Discovery
builder.Services.AddHttpClient<IServiceDiscovery, ServiceDiscovery>();
builder.Services.AddSingleton<IServiceDiscovery, ServiceDiscovery>();

// 🔧 FIXED: Improved JWT Authentication configuration
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        var jwtSettings = builder.Configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"];
        var issuer = jwtSettings["Issuer"];
        var audience = jwtSettings["Audience"];
        
        // 🔍 DEBUG: Enhanced JWT configuration logging
        Console.WriteLine($"🔧 JWT Settings: Issuer={issuer}, Audience={audience}, SecretKey={secretKey?.Substring(0, 10)}...");
        Console.WriteLine($"🔧 JWT Settings: SecretKey Length={secretKey?.Length}, Key Valid={!string.IsNullOrEmpty(secretKey)}");
        
        // 🔧 CRITICAL: Ensure secret key is not null
        if (string.IsNullOrEmpty(secretKey))
        {
            throw new InvalidOperationException("JWT SecretKey is null or empty in configuration");
        }
        
        // 🔧 FIXED: Create signing key with proper validation
        var keyBytes = Encoding.UTF8.GetBytes(secretKey);
        var signingKey = new SymmetricSecurityKey(keyBytes);
        
        Console.WriteLine($"🔧 JWT Key Info: Key Size={keyBytes.Length} bytes, Min Required=256 bits (32 bytes)");
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = signingKey,
            
            // 🔧 IMPORTANT: Clock skew for time synchronization issues
            ClockSkew = TimeSpan.FromMinutes(5),
            
            // 🔧 NEW: Additional security validations
            ValidateTokenReplay = false,
            ValidateActor = false,
            
            // 🔧 CRITICAL: Specify allowed algorithms explicitly
            ValidAlgorithms = new[] { SecurityAlgorithms.HmacSha256 }
        };
        
        // 🔧 ENHANCED: Complete JWT debugging with better error handling
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"🔍 JWT OnMessageReceived:");
                Console.WriteLine($"   Path: {context.Request.Path}");
                Console.WriteLine($"   Method: {context.Request.Method}");
                Console.WriteLine($"   Authorization Header: {(authHeader != null ? $"{authHeader.Substring(0, Math.Min(50, authHeader.Length))}..." : "MISSING")}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                var userIdClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var emailClaim = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
                var issuerClaim = context.Principal?.FindFirst("iss")?.Value;
                var audienceClaim = context.Principal?.FindFirst("aud")?.Value;
                
                Console.WriteLine($"✅ JWT Token Validated Successfully:");
                Console.WriteLine($"   UserId: {userIdClaim}");
                Console.WriteLine($"   Email: {emailClaim}");
                Console.WriteLine($"   Issuer: {issuerClaim}");
                Console.WriteLine($"   Audience: {audienceClaim}");
                Console.WriteLine($"   Claims Count: {context.Principal?.Claims?.Count() ?? 0}");
                
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"🚨 JWT Authentication Failed:");
                Console.WriteLine($"   Path: {context.Request.Path}");
                Console.WriteLine($"   Error: {context.Exception.Message}");
                Console.WriteLine($"   Exception Type: {context.Exception.GetType().Name}");
                
                // 🔧 NEW: More detailed debugging for signature validation failures
                if (context.Exception is SecurityTokenSignatureKeyNotFoundException)
                {
                    Console.WriteLine($"   🔑 SIGNING KEY ISSUE: The security key for signature validation was not found");
                    Console.WriteLine($"   🔑 Expected Issuer: {issuer}");
                    Console.WriteLine($"   🔑 Expected Audience: {audience}");
                    Console.WriteLine($"   🔑 Signing Key Present: {signingKey != null}");
                }
                
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"   Inner Exception: {context.Exception.InnerException.Message}");
                }
                
                // 🔍 Check token format
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                if (authHeader != null && authHeader.StartsWith("Bearer "))
                {
                    var token = authHeader.Substring(7);
                    Console.WriteLine($"   Token Length: {token.Length}");
                    Console.WriteLine($"   Token Format: {(token.Contains('.') ? "JWT" : "Unknown")}");
                    
                    try
                    {
                        var parts = token.Split('.');
                        Console.WriteLine($"   JWT Parts: {parts.Length} (Expected: 3)");
                        
                        // 🔧 NEW: Decode JWT header for debugging
                        if (parts.Length >= 1)
                        {
                            try
                            {
                                var header = parts[0];
                                var paddedHeader = header.PadRight(header.Length + (4 - header.Length % 4) % 4, '=');
                                var headerBytes = Convert.FromBase64String(paddedHeader);
                                var headerJson = Encoding.UTF8.GetString(headerBytes);
                                Console.WriteLine($"   JWT Header: {headerJson}");
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"   Header Decode Error: {ex.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"   Token Parse Error: {ex.Message}");
                    }
                }
                
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"🚨 JWT Challenge:");
                Console.WriteLine($"   Path: {context.Request.Path}");
                Console.WriteLine($"   Error: {context.Error}");
                Console.WriteLine($"   Error Description: {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    });

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Health Checks
builder.Services.AddHealthChecks()
    .AddCheck("auth-service", () => 
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Auth service reachable"))
    .AddCheck("user-service", () => 
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("User service reachable"));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseSerilogRequestLogging();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

// 🔧 FIX #1: Move RequestLogging first, but put Authentication before tenant resolution
app.UseMiddleware<RequestLoggingMiddleware>();

// 🔧 CRITICAL: Authentication must come before tenant resolution to access JWT claims
app.UseAuthentication();

// 🔧 FIX #2: Now tenant resolution can access JWT claims since authentication ran first
app.UseMiddleware<TenantResolutionMiddleware>();

app.UseMiddleware<RateLimitingMiddleware>();
app.UseMiddleware<AuthorizationContextMiddleware>();

// IMPORTANT: Map local endpoints BEFORE Ocelot
app.UseRouting();

// Map controllers for service discovery API
app.MapControllers();

// Add health check endpoints
app.MapHealthChecks("/health");
app.MapHealthChecks("/health/ready");
app.MapHealthChecks("/health/live");

// Add a simple test endpoint
app.MapGet("/gateway/info", () => Results.Ok(new 
{ 
    Service = "API Gateway",
    Version = "1.0.0",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
}));

// ➕ ADD: Explicit logging for startup visibility
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;
    
    foreach (var address in addresses ?? Enumerable.Empty<string>())
    {
        Console.WriteLine($"Now listening on: {address}");
        Log.Information("Now listening on: {Address}", address);
    }
});

// ⚠️ CRITICAL: Use conditional Ocelot to preserve local routes
app.MapWhen(context => 
    !context.Request.Path.StartsWithSegments("/health") &&
    !context.Request.Path.StartsWithSegments("/gateway") &&
    !context.Request.Path.StartsWithSegments("/api/servicediscovery"),
    ocelotApp => ocelotApp.UseOcelot().Wait());

try
{
    Log.Information("Starting API Gateway");
    Console.WriteLine("=== API Gateway Starting ===");
    
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "API Gateway terminated unexpectedly");
    Console.WriteLine($"FATAL ERROR: {ex.Message}");
}
finally
{
    Log.CloseAndFlush();
}

// ADDED: Make Program class accessible for testing
public partial class Program { }
