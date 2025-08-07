{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "Microsoft.AspNetCore.Authentication": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=boiler_dev;Username=boiler_app;Password=dev_password123;Port=5432;Include Error Detail=true"
  },
  "JwtSettings": {
    "SecretKey": "your-super-secret-jwt-key-that-is-at-least-256-bits-long",
    "Issuer": "AuthService",
    "Audience": "StarterApp",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7,
    "ValidateIssuer": true,
    "ValidateAudience": true,
    "ValidateLifetime": true,
    "ValidateIssuerSigningKey": true
  },
  "TenantSettings": {
    "DefaultTenantId": "1",
    "ResolutionStrategy": "Header",
    "EnableRowLevelSecurity": true,
    "AllowCrossTenantQueries": false,
    "TenantHeaderName": "X-Tenant-ID"
  },
  "ServiceSettings": {
    "ServiceName": "UserService",
    "Version": "1.0.0",
    "Environment": "Development",
    "Port": 5001,
    "HealthCheckPath": "/health"
  },
  "CorsSettings": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:5173",
      "https://localhost:3000",
      "https://localhost:5173"
    ],
    "AllowedMethods": [ "GET", "POST", "PUT", "DELETE", "OPTIONS" ],
    "AllowedHeaders": [ "*" ],
    "AllowCredentials": true
  },
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning",
        "Microsoft.AspNetCore.Authentication": "Debug",
        "Microsoft.AspNetCore.Authorization": "Debug"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/userservice-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7,
          "outputTemplate": "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      }
    ]
  }
}
