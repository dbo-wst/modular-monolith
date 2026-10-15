using Testcontainers.MsSql;

namespace Acme.Core.TestCompanion;

public static partial class Support
{
    public static class Infrastructure
    {
        public static MsSqlContainer UseContainer() =>
            new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                .WithPassword("MyP@ssword")
                .WithCleanUp(true)
                .Build();

    }

    public static class Domain
    {
        public static string NewIsinAsString()
        {
            var faker = NewFaker();
            return $"{faker.Random.String2(2).ToUpper()}{faker.Random.String2(10, "0123456789")}";
        }

    }
}

