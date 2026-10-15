# Template.Api.Infrastructure.CoreApi

Gateway-side integration layer (assembly `Template.Api.Infrastructure.CoreApi`, class library). It owns the typed gRPC clients that call `Template.Core.Api`, the client-facing request/response records used by `Template.Api` endpoints, and the mappers between those records and Protocol messages.

## Contents

| Item | Kind | Description |
|---|---|---|
| `ServiceCollectionExtensions.cs` | static class | `AddCoreApiInfrastructure(services, configuration)`: binds and validates `CoreApiOptions`, then calls `AddOrderingGrpcClients`. |
| `CoreApiOptions.cs` | class | Options bound from section `CoreApi`; `BaseUrl` is `[Required]`, `[Url]`. |
| `ApiError.cs` | record | `ApiError : LanguageExt.Common.Expected` carrying the wire `ErrorCode`; created with `ApiError.New(code, message)`. |
| [`Prelaunch/`](Prelaunch/README.md) | folder | Per-module clients (currently `Ordering`). |
| `Template.Api.Infrastructure.CoreApi.csproj` | project | Project file. |

## How it works
- `AddOptions<CoreApiOptions>().Bind("CoreApi").Validate(...)` requires `CoreApi:BaseUrl` to be a well-formed absolute URI ("CoreApi:BaseUrl must be a valid absolute URL.").
- Each module registers `AddGrpcClient<TServiceClient>` (gRPC client factory) whose `Address` is read from `IOptions<CoreApiOptions>` at resolve time, plus a scoped `IGrpcApiClient` wrapper.
- Wrappers return `Fin<TResponse>`: the proto `oneof result` success branch is mapped to a response record; the error branch becomes an `ApiError` with the code produced by Core.Api's `ErrorCodeClassifier`. An empty error yields `ApiError(internal, "The Core API returned an empty error.")`.
- Mappers are `readonly struct`s implementing `IMap`/`IOnewayMap` from `Template.Core.BuildingBlocks.Domain`, exposed through a static `ProtoMappers` registry per resource.

## Dependencies
- References: `Template.Core.Protocol`, `Template.Core.BuildingBlocks.Domain`; packages `Grpc.Net.ClientFactory`, `LanguageExt.Core`, `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Options`, `Microsoft.Extensions.Options.ConfigurationExtensions`.
- Used by: `Template.Api` (`Program.cs` and `Ordering/*/Endpoints.cs`).

## Extending
Add `Prelaunch/<Module>/` with an `Add<Module>GrpcClients` extension (same pattern as Ordering) and call it from `AddCoreApiInfrastructure`.
