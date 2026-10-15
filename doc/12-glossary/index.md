# 12. Glossary

| Term | Definition |
|------|------------|
| Module | A bounded context under `src/Modules`, split into Domain, Application, Contracts and one project per adapter (e.g. Persistence) |
| Slice | One use case folder in Application: handler plus validator |
| Command / Query | A state-changing / read-only request dispatched by `IMediator` |
| Adapter | A project implementing ports for one external concern, e.g. `Template.Core.Ordering.Persistence` |
