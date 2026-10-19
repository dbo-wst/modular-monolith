using Serilog;
using Acme.Core.BuildingBlocks.Application.Communication;
using Acme.Core.BuildingBlocks.Observability;
// using Acme.Core.Api.Prelaunch.Allocation;
using Acme.Core.Api.Prelaunch.Ordering;
// using Acme.Core.Api.Prelaunch.ClientSide;
// using Acme.Core.Api.Prelaunch.MasterData;

const string ServiceName = "acme-core-api";

// File sink paths in appsettings.json start with %BASEDIR%, so logs go to the execution folder, not the working directory.
Environment.SetEnvironmentVariable("BASEDIR", AppContext.BaseDirectory);

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
        .Enrich.FromLogContext()
        .WriteToOtlp(context.Configuration, ServiceName));

    builder.Services.AddObservability(builder.Configuration, ServiceName);

    builder.Services.AddGrpc();

    // One mediator (and its pipeline behaviours) shared by all modules; modules only register their handlers.
    builder.Services.AddMediator();

    builder.Services.AddOrderingModule(builder.Configuration);

    var app = builder.Build();

    app.UseSerilogRequestLogging();

    app.MapOrderingGrpcServices();

    app.MapGet("/",
        () =>
            "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

    Log.Information("Start Acme.Core.Api. Env: {Environment}, OTLP: {OtlpEndpoint}", environmentName ?? "Unknown",
        builder.Configuration[ObservabilityExtensions.EndpointKey] ?? "off");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Acme.Core.Api terminated unexpectedly");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

namespace Acme.Core.Api
{
    public partial class Program;
}
