using Common.Data;
using Common.Extensions;
using Common.Middleware;
using Common.Services;
using Contracts.Repositories;
using Contracts.Services;
using Contracts.User;
using Microsoft.EntityFrameworkCore;
using UserService.Mappings;
using UserService.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((context, configuration) =>
    configuration.ReadFrom.Configuration(context.Configuration)); // ✅ ADDED MISSING CLOSING PARENTHESIS

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add common services (JWT, configuration, etc.)
builder.Services.AddCommonServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);

// ✅ USE DATABASE EXTENSION (includes DbContext + Repositories)
builder.Services.AddDatabase(builder.Configuration);

// AutoMapper - Register your UserMappingProfile
builder.Services.AddAutoMapper(typeof(UserMappingProfile));

// Add your service implementations
builder.Services.AddScoped<Contracts.User.IUserService, UserServiceImplementation>();
builder.Services.AddScoped<Contracts.User.IUserProfileService, UserProfileService>();

// Add FluentValidation
builder.Services.AddFluentValidation();

// Add authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => 
        policy.RequireRole("Admin"));
    
    options.AddPolicy("OwnerOrAdmin", policy =>
        policy.RequireAssertion(context =>
        {
            var userId = context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = context.User.IsInRole("Admin");
            return isAdmin || (userId != null);
        }));
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();

// Add validation middleware
app.UseMiddleware<ValidationMiddleware>();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Add health check endpoint
app.MapGet("/health", () => Results.Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow }));

// Add explicit URL logging
app.Lifetime.ApplicationStarted.Register(() =>
{
    var addresses = app.Services.GetRequiredService<Microsoft.AspNetCore.Hosting.Server.IServer>()
        .Features.Get<Microsoft.AspNetCore.Hosting.Server.Features.IServerAddressesFeature>()?.Addresses;
    
    foreach (var address in addresses ?? Enumerable.Empty<string>())
    {
        Console.WriteLine($"Now listening on: {address}");
    }
});

try
{
    Log.Information("Starting UserService");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "UserService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
