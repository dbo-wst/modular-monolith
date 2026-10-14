using Testcontainers.MsSql;

namespace Template.Core.TestCompanion;

public static partial class Support
{
    public static partial class Infrastructure
    {
        public static MsSqlContainer UseContainer() =>
            new MsSqlBuilder("mcr.microsoft.com/mssql/server:2025-latest")
                .WithPassword("MyP@ssword")
                .WithCleanUp(true)
                .Build();

    }
}

