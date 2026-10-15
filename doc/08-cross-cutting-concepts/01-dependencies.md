# 8.1 Dependencies

Dependency rule (enforced by `test/Architecture`): Domain <- Application <- adapters (one project per adapter, e.g. `Persistence`); adapters do not reference each other; hosts reference Contracts only. Package versions are managed centrally in `Directory.Packages.props`.
