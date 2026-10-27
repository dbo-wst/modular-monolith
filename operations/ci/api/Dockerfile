# Multi-stage build for Acme Api
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# Image version (build.sh passes it); becomes the assembly version and with it service.version in the telemetry.
ARG VERSION=0.0.0
WORKDIR /repo

# Single source copy. Repository src lands at /repo/src, root files at /repo.
COPY . /repo

# Restore and publish
RUN dotnet restore "/repo/src/Host/Acme.Api/Acme.Api.csproj"
RUN dotnet publish "/repo/src/Host/Acme.Api/Acme.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false -p:Version=${VERSION}

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app/publish
COPY --from=build /app/publish /app/publish

EXPOSE 80

ENTRYPOINT ["dotnet", "Acme.Api.dll"]
