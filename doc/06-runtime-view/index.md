# 6. Runtime View

Typical request flow: client -> `Acme.Api` (HTTP) -> gRPC -> `Acme.Core.Api` -> `IMediator` -> pipeline behaviours (validation) -> command/query handler -> repository / read store -> SQL Server.

Add sequence diagrams for the important scenarios here.
