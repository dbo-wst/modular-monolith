# 7. Deployment View

Each host (`Acme.Api`, `Acme.Core.Api`) and the documentation site is built into its own Docker image (`operations/ci/*`) and started with Docker Compose (`operations/cd/*`). SQL Server for local development: `operations/sql/docker-compose.mssql.local.yml`.
