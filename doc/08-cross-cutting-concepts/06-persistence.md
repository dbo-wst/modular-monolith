# 8.6 Persistence

EF Core with SQL Server. Writes go through repositories and `IUnitOfWork` (one `DbContext` per scope); reads go through `IOrderingReadStore` (no-tracking projections to DTOs). Migrations: see `src/Modules/Ordering/Persistence/readme.md` and `add-migrations.sh`.
