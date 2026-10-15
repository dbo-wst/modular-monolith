# Setup

## Prerequisites

- .NET 10 SDK
- Docker (for SQL Server and the container workflow)

## Quick Start

```bash
dotnet build Template.Core.slnx
dotnet test Template.Core.slnx
```

## Local database

```bash
docker compose -f operations/sql/docker-compose.mssql.local.yml up -d
```

Migrations: set `SQL_CONNECTION_STRING` and run `./add-migrations.sh` (see `src/Modules/Ordering/Infrastructure/readme.md`).

## Containerised workflow

Images are built with `operations/ci/build.sh <version>` and started with `operations/cd/run.sh <version>`.
