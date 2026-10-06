using Serilog;
// using Wst.Trios.Core.Api.Prelaunch.Allocation;
using Wst.Trios.Core.Api.Prelaunch.BrokerSide;
// using Wst.Trios.Core.Api.Prelaunch.ClientSide;
// using Wst.Trios.Core.Api.Prelaunch.MasterData;

var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")
                      ?? Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");

var bootstrapConfig = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true);

if (!string.IsNullOrWhiteSpace(environmentName))
{
    bootstrapConfig.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(bootstrapConfig.Build())
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    builder.Services.AddGrpc();

    builder.Services.AddBrokerSideModule(builder.Configuration);
    // builder.Services.AddClientSideModule(builder.Configuration);
    // builder.Services.AddAllocationModule(builder.Configuration);
    // builder.Services.AddMasterDataModule(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapBrokerSideGrpcServices();
    // app.MapClientSideGrpcServices();
    // app.MapAllocationGrpcServices();
    // app.MapMasterDataGrpcServices();

    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    Log.Information("Start Trios.Core.Api. Env: {Environment}", environmentName ?? "Unknown");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Trios.Core.Api terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

namespace Wst.Trios.Core.Api
{
    public partial class Program;
}
