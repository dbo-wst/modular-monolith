# 3.2 Technical Context

| Interface | Direction | Technology |
|-----------|-----------|------------|
| Gateway (`Template.Api`) | clients -> system | HTTP / JSON |
| Core host (`Template.Core.Api`) | gateway -> core | gRPC (`src/Protocol`) |
| Database | core -> SQL Server | EF Core |
