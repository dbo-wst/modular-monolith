# 2. Architecture Constraints

## 2.1 Technical Constraints

- Platform: .NET 10, C# (nullable reference types enabled), central package management (`Directory.Packages.props`)
- Database: Microsoft SQL Server via EF Core
- Communication: gRPC between the gateway and the core host
- Deployment: Docker images (`operations/ci`, `operations/cd`)

## 2.2 Organizational Constraints

- Version control: Git
- Tests: xUnit v3, FakeItEasy, NetArchTest (architecture rules)
- Add team, schedule and release constraints here

## 2.3 Conventions

- Coding conventions: `.editorconfig`, default Microsoft C# conventions
- Language: English, including technical terms
