#!/usr/bin/env bash

set -euo pipefail

#dotnet ef migrations add AddAllocationModule \
#  --project ./src/Allocation/Persistence/Acme.Core.Allocation.Persistence.csproj \
#  --context Acme.Core.Allocation.Persistence.DbContext.AllocationDbContext

dotnet ef migrations add AddOrderingModule \
  --project ./src/Modules/Ordering/Persistence/Acme.Core.Ordering.Persistence.csproj \
  --context Acme.Core.Ordering.Persistence.DbContext.OrderingDbContext

#dotnet ef migrations add AddClientSideModule \
#  --project ./src/ClientSide/Persistence/Acme.Core.ClientSide.Persistence.csproj \
#  --context Acme.Core.ClientSide.Persistence.DbContext.ClientSideDbContext
#
#dotnet ef migrations add AddMasterDataModule \
#  --project ./src/MasterData/Persistence/Acme.Core.MasterData.Persistence.csproj \
#  --context Acme.Core.MasterData.Persistence.DbContext.MasterDataDbContext
