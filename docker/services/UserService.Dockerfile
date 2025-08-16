# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first (for better caching)
COPY ["src/services/UserService/UserService.csproj", "services/UserService/"]
COPY ["src/shared/DTOs/DTOs.csproj", "shared/DTOs/"]
COPY ["src/shared/Common/Common.csproj", "shared/Common/"]
COPY ["src/shared/Contracts/Contracts.csproj", "shared/Contracts/"]

# Restore dependencies
RUN dotnet restore "services/UserService/UserService.csproj"

# Copy source code
COPY src/services/UserService/ services/UserService/
COPY src/shared/ shared/

# Build application
WORKDIR "/src/services/UserService"
RUN dotnet build "UserService.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "UserService.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Generate HTTPS certificate in build stage (has SDK)
RUN dotnet dev-certs https --clean
RUN dotnet dev-certs https -ep /app/publish/aspnetapp.pfx -p ""

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Create non-root user
RUN groupadd -r appuser && useradd -r -g appuser appuser

# Install curl for health checks
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published app and certificate from build stage
COPY --from=publish /app/publish .

# Set proper permissions for certificate
RUN chown appuser:appuser /app/aspnetapp.pfx
RUN chmod 644 /app/aspnetapp.pfx

# Switch to non-root user
USER appuser

# Set URLs for both HTTP and HTTPS
ENV ASPNETCORE_URLS=http://0.0.0.0:5002;https://0.0.0.0:7002
ENV ASPNETCORE_HTTPS_PORT=7002
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=""

# Health check (use HTTPS)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f -k https://localhost:7002/health || curl -f http://localhost:5002/health || exit 1

EXPOSE 5002 7002
ENTRYPOINT ["dotnet", "UserService.dll"]
