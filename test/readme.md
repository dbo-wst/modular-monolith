# Tests

This guide is shared for all projects under `test/`.

## Prerequisites

- .NET SDK 10

## Run Tests

Run one test project:

```powershell
dotnet test .\test\<Domain>\<Module>\*.Test.csproj
```

## Notes

- Tests under `test/` are intended for fast local execution.
- Shared test helpers live under `test/TestCompanion/`.
