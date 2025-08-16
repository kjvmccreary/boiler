# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution and project files first (for better caching)
COPY ["src/services/ApiGateway/ApiGateway.csproj", "services/ApiGateway/"]
COPY ["src/shared/DTOs/DTOs.csproj", "shared/DTOs/"]
COPY ["src/shared/Common/Common.csproj", "shared/Common/"]
COPY ["src/shared/Contracts/Contracts.csproj", "shared/Contracts/"]

# Restore dependencies
RUN dotnet restore "services/ApiGateway/ApiGateway.csproj"

# Copy source code
COPY src/services/ApiGateway/ services/ApiGateway/
COPY src/shared/ shared/

# Build application
WORKDIR "/src/services/ApiGateway"
RUN dotnet build "ApiGateway.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "ApiGateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Generate HTTPS certificate with empty password (valid approach)
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
ENV ASPNETCORE_URLS=http://0.0.0.0:5000;https://0.0.0.0:7000
ENV ASPNETCORE_HTTPS_PORT=7000
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/aspnetapp.pfx
ENV ASPNETCORE_Kestrel__Certificates__Default__Password=""

# Health check (use HTTPS)
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f -k https://localhost:7000/health || curl -f http://localhost:5000/health || exit 1

EXPOSE 5000 7000
ENTRYPOINT ["dotnet", "ApiGateway.dll"]
