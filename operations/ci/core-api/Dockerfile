# Multi-stage build for Acme Core gRPC API
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Single source copy. Repository src lands at /repo/src, root files at /repo.
COPY . /repo

# Restore and publish
RUN dotnet restore "/repo/src/Host/Acme.Core.Api/Acme.Core.Api.csproj"
RUN dotnet publish "/repo/src/Host/Acme.Core.Api/Acme.Core.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app/publish
COPY --from=build /app/publish /app/publish

EXPOSE 80

ENTRYPOINT ["dotnet", "Acme.Core.Api.dll"]
