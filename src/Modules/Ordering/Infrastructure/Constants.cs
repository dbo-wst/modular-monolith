namespace Template.Core.Ordering.Infrastructure;

public static class Constants
{
    public const string MigrationTableName = "__EFMigrationsHistory";


    public static Func<string> GetConnectionString => () =>
        Environment.GetEnvironmentVariable("SQL_CONNECTION_STRING") ??
        throw new ArgumentNullException(
            $"SQL_CONNECTION_STRING environment variable is not found.");

    public const string Schema = "ordering";
}


