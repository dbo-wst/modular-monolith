# Operations

How the solution is built, distributed and run with the scripts in `operations/`. The layout
follows `wst-tools-regimate/operations`.

## Layout

Build/publish side (`ci`) and deploy side (`cd`), one folder per application:

| Path | Runs on | Purpose |
|------|---------|---------|
| `operations/ci/core-api/` | Build host | `Dockerfile`, `build.sh`, `push.sh` for the Core.Api image. |
| `operations/ci/api/` | Build host | `Dockerfile`, `build.sh`, `push.sh` for the Api (gateway) image. |
| `operations/ci/docs/` | Build host | `Dockerfile`, `nginx.conf`, `build.sh`, `push.sh` for the docfx site. |
| `operations/cd/core-api/` | Target host | `pull.sh`, `run.sh`, compose files and `conf.d/` for Core.Api. |
| `operations/cd/api/` | Target host | `pull.sh`, `run.sh`, compose files and `conf.d/` for the Api. |
| `operations/cd/docs/` | Target host | `pull.sh`, `run.sh` and compose files for the docs. |
| `operations/sql/` | Developer machine | Local SQL Server container. |

The top-level wrappers `operations/ci/build.sh`, `operations/ci/push.sh`, `operations/cd/pull.sh`
and `operations/cd/run.sh` run the step for **all three** apps; the per-app scripts run it for one.

## What gets deployed

Compose project `acme`:

| Container | Image | Kind |
|-----------|-------|------|
| `acme-core-api` | `build.steubing.com/acme-core-api:<version>` | gRPC host (`Acme.Core.Api`), HTTP/2 only, port `80`. |
| `acme-api` | `build.steubing.com/acme-api:<version>` | REST gateway (`Acme.Api`), port `80`, `PathBase=/acme/api`. |
| `acme-docs` | `build.steubing.com/acme-docs:<version>` | Static docfx site behind nginx, port `80`. |

- All apps join the external networks `acme-network` and `rp_default_net`; the local compose
  files also attach Core.Api and the Api to `mssql-network` and publish host ports
  (`CORE_API_HOST_PORT` 5080, `API_HOST_PORT` 5081, `DOCS_HOST_PORT` 5082).
- The Api calls Core.Api over `acme-network` (`CoreApi__BaseUrl=http://acme-core-api:80`).
- Core.Api and the Api keep Data Protection keys and logs in external volumes
  (`acme-<app>-dp-keys`, `acme-<app>-logs`), created by `run.sh`.
- Core.Api uses its own database `acme-local` on the local SQL Server
  (`ms_sql_server_local`, `operations/sql`); it does not share a database with other solutions.
  EF Core migrations create and update it on first use.
- Every container, including the local SQL Server, runs in `TZ=Europe/Berlin`.
- Behind the local reverse proxy (`wst-cicd/reverse-proxy-2`), a machine-local snippet
  `cd/sites.d/acme/Caddyfile` (not committed) routes, on `https://apps-local.steubing.com`:

  | Path | Upstream | Prefix |
  |------|----------|--------|
  | `/acme/api/` | `acme-api:80` (REST, Scalar at `/acme/api/scalar/`) | preserved (`PathBase`) |
  | `/acme/core/api/` | `acme-core-api:80` (gRPC, h2c to the container) | stripped |
  | `/acme/docs/` | `acme-docs:80` | stripped |

- gRPC through the proxy needs HTTP/2 and a client that keeps the `/acme/core/api` prefix in
  front of `/<package>.<Service>/<Method>`. `GrpcChannel` drops the path of its address, so a
  .NET client needs a `DelegatingHandler` that prepends the prefix. The gateway is not affected:
  it calls `http://acme-core-api:80` directly over `acme-network`.

## Environments

Acme runs **locally only**.

- Each app folder has its environment files under `conf.d/`: `.env.local` (and `.env.dev`).
  They contain credentials and are gitignored (`.env*`); `core-api/conf.d/runtime.env.example`
  is the tracked template with placeholders.
- `run.sh` reads `conf.d/.env`, so copy the environment file onto `.env` first; the orchestrator
  `run.sh` refuses to start when a `.env` is missing. The local compose files default to
  `conf.d/.env` as well.

## Deployment

- CI: `operations/ci/build.sh <version>` builds the three images; `push.sh <version>` pushes them
  to `build.steubing.com`.
- CD: `operations/cd/pull.sh <version>`, then `operations/cd/run.sh <version>`. The orchestrator
  does not stop the stack: it recreates each app in place
  (`up -d --force-recreate <app>`) with that app's image tag. Per-app `run.sh` recreates only
  its own stack (`down`, `up -d`).
- Local build and run without the registry:
  `docker compose -f operations/cd/docker-compose.local.yml up -d --build` (start
  `operations/sql` first).

## Monitoring

- Logs: Serilog console output (`docker logs acme-<app>`) and rolling files in the
  `acme-<app>-logs` volumes (file lines carry the trace id).
- OpenTelemetry: Core.Api and the Api send logs and traces (no metrics) over OTLP to the
  `otel` stack of wst-cicd (Grafana at `https://otel-local.steubing.com`) as `acme-core-api` and
  `acme-api`, service namespace `acme`. Three keys in `conf.d/.env` configure it:
  `OTEL_EXPORTER_OTLP_ENDPOINT=https://otel-local.steubing.com:4318`,
  `OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf`,
  `OTEL_RESOURCE_ATTRIBUTES=deployment.environment.name=local` (dev: `otel.steubing.com`, `dev`).
  Without the endpoint nothing is exported; the start line of each app shows the endpoint or `off`.
- No Docker `HEALTHCHECK` and no `/health` endpoint.
