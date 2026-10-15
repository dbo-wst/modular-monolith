# Multi-stage build for Template Api
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

# Single source copy. Repository src lands at /repo/src, root files at /repo.
COPY . /repo

# Restore and publish
RUN dotnet restore "/repo/src/Host/Template.Api/Template.Api.csproj"
RUN dotnet publish "/repo/src/Host/Template.Api/Template.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app/publish
COPY --from=build /app/publish /app/publish

EXPOSE 80

ORDER_ITEMPOINT ["dotnet", "Template.Api.dll"]
