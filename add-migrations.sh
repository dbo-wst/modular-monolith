#!/usr/bin/env bash

set -euo pipefail

#dotnet ef migrations add AddAllocationModule \
#  --project ./src/Allocation/Persistence/Template.Core.Allocation.Persistence.csproj \
#  --context Template.Core.Allocation.Persistence.DbContext.AllocationDbContext

dotnet ef migrations add AddOrderingModule \
  --project ./src/Modules/Ordering/Persistence/Template.Core.Ordering.Persistence.csproj \
  --context Template.Core.Ordering.Persistence.DbContext.OrderingDbContext

#dotnet ef migrations add AddClientSideModule \
#  --project ./src/ClientSide/Persistence/Template.Core.ClientSide.Persistence.csproj \
#  --context Template.Core.ClientSide.Persistence.DbContext.ClientSideDbContext
#
#dotnet ef migrations add AddMasterDataModule \
#  --project ./src/MasterData/Persistence/Template.Core.MasterData.Persistence.csproj \
#  --context Template.Core.MasterData.Persistence.DbContext.MasterDataDbContext
