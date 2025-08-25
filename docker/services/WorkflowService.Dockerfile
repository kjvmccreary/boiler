# Multi-stage build for WorkflowService
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003
EXPOSE 7003

# Create non-root user for security
RUN adduser --disabled-password --home /app --gecos '' appuser && chown -R appuser /app
USER appuser

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files
COPY ["src/services/WorkflowService/WorkflowService.csproj", "src/services/WorkflowService/"]
COPY ["src/shared/DTOs/DTOs.csproj", "src/shared/DTOs/"]
COPY ["src/shared/Common/Common.csproj", "src/shared/Common/"]
COPY ["src/shared/Contracts/Contracts.csproj", "src/shared/Contracts/"]

# Restore dependencies
RUN dotnet restore "src/services/WorkflowService/WorkflowService.csproj"

# Copy source code
COPY . .

# Build
WORKDIR "/src/src/services/WorkflowService"
RUN dotnet build "WorkflowService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WorkflowService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Switch to root to install packages and setup certificates
USER root

# Install curl for health checks (consistent with other services) and openssl for certificates
RUN apt-get update && apt-get install -y curl openssl && rm -rf /var/lib/apt/lists/*

# Generate self-signed certificates for HTTPS
RUN openssl req -x509 -newkey rsa:4096 -keyout /app/workflow-service.key -out /app/workflow-service.crt -days 365 -nodes -subj "/CN=workflow-service"
RUN chown appuser:appuser /app/workflow-service.*

# Switch back to non-root user
USER appuser

# Configure ASP.NET Core for HTTPS
ENV ASPNETCORE_URLS="https://+:7003;http://+:5003"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path=/app/workflow-service.crt
ENV ASPNETCORE_Kestrel__Certificates__Default__KeyPath=/app/workflow-service.key

ENTRYPOINT ["dotnet", "WorkflowService.dll"]
