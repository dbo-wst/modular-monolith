# 3.2 Technical Context

| Interface | Direction | Technology |
|-----------|-----------|------------|
| Gateway (`Acme.Api`) | clients -> system | HTTP / JSON |
| Core host (`Acme.Core.Api`) | gateway -> core | gRPC (`src/Protocol`) |
| Database | core -> SQL Server | EF Core |
