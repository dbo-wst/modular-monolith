# 8.1 Dependencies

Dependency rule (enforced by `test/Architecture`): Domain <- Application <- Infrastructure; hosts reference Contracts only. Package versions are managed centrally in `Directory.Packages.props`.
