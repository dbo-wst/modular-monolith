#!/usr/bin/env bash

set -euo pipefail

#dotnet ef migrations add AddAllocationModule \
#  --project ./src/Allocation/Persistence/Wst.Trios.Core.Allocation.Persistence.csproj \
#  --context Wst.Trios.Core.Allocation.Persistence.DbContext.AllocationDbContext

dotnet ef migrations add AddBrokerSideModule \
  --project ./src/BrokerSide/Persistence/Wst.Trios.Core.BrokerSide.Persistence.csproj \
  --context Wst.Trios.Core.BrokerSide.Persistence.DbContext.BrokerSideDbContext

#dotnet ef migrations add AddClientSideModule \
#  --project ./src/ClientSide/Persistence/Wst.Trios.Core.ClientSide.Persistence.csproj \
#  --context Wst.Trios.Core.ClientSide.Persistence.DbContext.ClientSideDbContext
#
#dotnet ef migrations add AddMasterDataModule \
#  --project ./src/MasterData/Persistence/Wst.Trios.Core.MasterData.Persistence.csproj \
#  --context Wst.Trios.Core.MasterData.Persistence.DbContext.MasterDataDbContext
