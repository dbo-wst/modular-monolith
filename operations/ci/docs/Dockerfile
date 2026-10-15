# Multi-stage build for Acme documentation
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /repo

COPY doc ./doc

RUN dotnet tool install --global docfx
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /repo/doc
RUN docfx docfx.json

FROM nginx:1.27-alpine AS final
COPY operations/ci/docs/nginx.conf /etc/nginx/conf.d/default.conf
COPY --from=build /repo/doc/_site /usr/share/nginx/html

EXPOSE 80
