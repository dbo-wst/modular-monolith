using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Testcontainers.MsSql;
using Wst.Trios.Core.BrokerSide.Persistence;

namespace Wst.Trios.Core.BrokerSide.TestCompanion;

public sealed class IntegrationTestsFixture : IAsyncLifetime
{
    public readonly Bogus.Faker Faker = new();
    public const string CollectionName = nameof(IntegrationTestsFixture);
    private const string PersistenceConnectionStringKey = "Persistence:ConnectionString";
    private ServiceProvider? _serviceProvider;

    private MsSqlContainer? _dbContainer;


    public async ValueTask InitializeAsync()
    {
        var useContainer = ShouldUseContainerFromTestSettings();

        if (useContainer)
        {
            _dbContainer = Trios.Core.TestCompanion.Support.Infrastructure.UseContainer();
            await _dbContainer.StartAsync();
            var connectionString = _dbContainer.GetConnectionString();
            _serviceProvider = BuildServiceProvider(InMemoryConfig(connectionString));
        }
        else
        {
            _serviceProvider = BuildServiceProvider(FileConfig);
        }
    }

    public T GetRequiredService<T>() where T : notnull
    {
        ArgumentNullException.ThrowIfNull(_serviceProvider);
        return _serviceProvider.GetRequiredService<T>();
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_dbContainer != null)
        {
            await _dbContainer.DisposeAsync();
        }

        if (_serviceProvider != null)
        {
            await _serviceProvider.DisposeAsync();
        }

        await Log.CloseAndFlushAsync();
    }

    private static bool ShouldUseContainerFromTestSettings()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: true)
            .Build();

        var connectionString = configuration[PersistenceConnectionStringKey];
        return string.IsNullOrWhiteSpace(connectionString);
    }

    private static Func<string, Func<IConfigurationRoot>> InMemoryConfig => connectionString
        => () => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            [PersistenceConnectionStringKey] = connectionString
        })
        .Build();
    private static Func<IConfigurationRoot> FileConfig => () => new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .AddJsonFile("appsettings.test.json")
        .Build();
    
    private ServiceProvider BuildServiceProvider(Func<IConfigurationRoot> configurationFunc)
    {
        var configuration = configurationFunc();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .CreateLogger();

        var services = new ServiceCollection();
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddSerilog(Log.Logger, dispose: false);
        });
        services.AddOptions();


        services.AddPersistence(configuration);

        return services.BuildServiceProvider();
    }
}
